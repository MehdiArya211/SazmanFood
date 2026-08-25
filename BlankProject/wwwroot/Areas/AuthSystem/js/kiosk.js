(function () {
    "use strict";

    const SETTINGS = {
        kioskKey: "KioskId",
        hubPath: "/kioskHub",
        finalizeUrl: "/Authentication/FinalizeFaceLogin",
        maxStartRetries: 10,
        retryDelayMs: 1500,
        debug: true
    };

    let redirected = false;
    let connection = null;

    // =========================
    // Logger (Dev)
    // =========================
    const LOG_PREFIX = "[kiosk]";
    let stepNo = 0;

    function now() {
        // زمان برای خوانایی لاگ‌ها
        const d = new Date();
        return d.toISOString();
    }

    function log(...args) { if (SETTINGS.debug) console.log(LOG_PREFIX, ...args); }
    function warn(...args) { if (SETTINGS.debug) console.warn(LOG_PREFIX, ...args); }
    function errLog(...args) { console.error(LOG_PREFIX, ...args); }

    function step(name, meta) {
        stepNo++;
        if (SETTINGS.debug) {
            console.log(`${LOG_PREFIX} [${stepNo}] ${name} @ ${now()}`, meta ?? "");
        }
    }

    function sleep(ms) {
        return new Promise(r => setTimeout(r, ms));
    }

    // =========================
    // UI Status (User-friendly)
    // =========================
    function setStatus(text) {
        const el = document.getElementById("connectionStatus");
        if (!el) {
            // اگر المنت نبود حداقل توی لاگ بگیم
            warn("Status element #connectionStatus not found. UI status skipped:", text);
            return;
        }
        el.innerText = text;
    }

    function setUserStatus(stage, extra) {
        // پیام‌های تمیز برای کاربر
        // stage را کوتاه نگه داریم، extra اختیاری
        const map = {
            boot: "در حال راه‌اندازی...",
            checking: "در حال بررسی پیش‌نیازها...",
            preparing: "در حال آماده‌سازی اتصال...",
            connecting: "در حال اتصال به دستگاه...",
            connected: "اتصال برقرار شد. در انتظار تشخیص چهره...",
            reconnecting: "اتصال قطع شد. در حال اتصال مجدد...",
            closed: "ارتباط با دستگاه قطع شد.",
            faceDetected: "چهره شناسایی شد. در حال ورود...",
            failed: "خطا در اتصال به دستگاه. لطفاً دوباره تلاش کنید.",
            internal: "خطای داخلی در راه‌اندازی. لطفاً صفحه را رفرش کنید."
        };

        const msg = map[stage] || stage || "";
        setStatus(extra ? `${msg} ${extra}` : msg);
    }

    // =========================
    // KioskId
    // =========================
    function generateKioskId() {
        step("generateKioskId()");
        return Math.floor(100000 + Math.random() * 900000);
    }

    function getOrCreateKioskId() {
        step("getOrCreateKioskId() - read localStorage", { key: SETTINGS.kioskKey });

        let raw = null;
        try {
            raw = localStorage.getItem(SETTINGS.kioskKey);
            step("localStorage.getItem() success", { raw });
        } catch (e) {
            // اگر localStorage در محیطی (مثل iframe/خصوصی) خطا داد
            errLog("localStorage.getItem failed:", e);
            step("localStorage.getItem() failed", { error: String(e) });
        }

        let kioskId = Number(raw);

        const isValid = Number.isInteger(kioskId) && kioskId > 0;
        step("validate kioskId", { kioskId, isValid });

        if (!isValid) {
            kioskId = generateKioskId();
            step("kioskId generated", { kioskId });

            try {
                localStorage.setItem(SETTINGS.kioskKey, String(kioskId));
                step("localStorage.setItem() success", { kioskId });
            } catch (e) {
                // حتی اگر ذخیره نشد، باز هم می‌تونیم ادامه بدیم
                errLog("localStorage.setItem failed:", e);
                step("localStorage.setItem() failed", { error: String(e) });
            }
        }

        return kioskId;
    }

    // =========================
    // Hub URL
    // =========================
    function getHubUrl() {
        step("getHubUrl() - check API_BASE_URL", { API_BASE_URL: window.API_BASE_URL });

        if (!window.API_BASE_URL) {
            // این یکی خطای جدی است
            throw new Error("API_BASE_URL is not defined.");
        }

        const url = `${window.API_BASE_URL}${SETTINGS.hubPath}`;
        step("hubUrl built", { url });
        return url;
    }

    function safeEncode(v) {
        return encodeURIComponent(v == null ? "" : String(v));
    }

    // =========================
    // Redirect
    // =========================
    function redirectToFinalize(kioskId, enrollId) {
        step("redirectToFinalize() called", { redirected, kioskId, enrollId });

        if (redirected) {
            warn("redirect blocked: already redirected once");
            step("redirect blocked - already redirected");
            return;
        }

        redirected = true;
        const url = `${SETTINGS.finalizeUrl}?kioskId=${safeEncode(kioskId)}&enrollId=${safeEncode(enrollId)}`;

        step("redirecting user", { url });
        window.location.href = url;
    }

    // =========================
    // SignalR Prerequisite
    // =========================
    function ensureSignalRLoaded() {
        step("ensureSignalRLoaded()");

        const ok = !!(window.signalR && window.signalR.HubConnectionBuilder);
        step("signalR presence", {
            hasSignalR: !!window.signalR,
            hasBuilder: !!(window.signalR && window.signalR.HubConnectionBuilder),
            ok
        });

        if (!ok) {
            throw new Error("signalR is not loaded. Load signalr2.js before kiosk.js");
        }
    }

    // =========================
    // Connection Builder
    // =========================
    function buildConnection(hubUrl) {
        step("buildConnection()", { hubUrl });

        const conn = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                withCredentials: false,
                transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .build();

        step("connection built");
        return conn;
    }

    // =========================
    // Handlers
    // =========================
    function registerHandlers(kioskId) {
        step("registerHandlers()", { kioskId });

        connection.on("ReceiveAutoLogin", (enrollId) => {
            step("event ReceiveAutoLogin", { enrollId });

            log("ReceiveAutoLogin:", enrollId);
            setUserStatus("faceDetected");
            redirectToFinalize(kioskId, enrollId);
        });

        connection.onreconnecting((e) => {
            step("event onreconnecting", { error: e ? String(e) : null });
            setUserStatus("reconnecting");
        });

        connection.onreconnected((connectionId) => {
            step("event onreconnected", { connectionId });
            setUserStatus("connected");
        });

        connection.onclose((e) => {
            step("event onclose", { error: e ? String(e) : null });
            setUserStatus("closed");
        });

        step("handlers registered");
    }

    // =========================
    // Start & Register with retry
    // =========================
    async function startConnectionWithRetry(kioskId) {
        step("startConnectionWithRetry() begin", { kioskId, maxRetries: SETTINGS.maxStartRetries });

        for (let attempt = 1; attempt <= SETTINGS.maxStartRetries; attempt++) {
            try {
                setUserStatus("connecting", `(تلاش ${attempt}/${SETTINGS.maxStartRetries})`);
                step("connection.start() attempt", { attempt });

                await connection.start();
                step("connection.start() success", { attempt, state: connection.state });

                step("connection.invoke(Register) begin", { kioskId });
                await connection.invoke("Register", kioskId);
                step("connection.invoke(Register) success", { kioskId });

                setUserStatus("connected");
                step("startConnectionWithRetry() done");
                return;

            } catch (e) {
                warn(`Start/Register failed (${attempt}/${SETTINGS.maxStartRetries})`, e);
                step("start/register failed", {
                    attempt,
                    message: e?.message,
                    name: e?.name,
                    stack: e?.stack
                });

                if (attempt === SETTINGS.maxStartRetries) {
                    errLog("Could not connect to SignalR hub after retries.", e);
                    setUserStatus("failed");
                    step("all retries exhausted - giving up");
                    return;
                }

                step("sleep before retry", { ms: SETTINGS.retryDelayMs });
                await sleep(SETTINGS.retryDelayMs);
            }
        }
    }

    // =========================
    // Init
    // =========================
    async function init() {
        step("init() start");
        setUserStatus("boot");

        try {
            setUserStatus("checking");
            ensureSignalRLoaded();

            setUserStatus("preparing");
            const kioskId = getOrCreateKioskId();
            const hubUrl = getHubUrl();

            log("kioskId:", kioskId);
            log("HubUrl:", hubUrl);
            step("init() got kioskId & hubUrl", { kioskId, hubUrl });

            connection = buildConnection(hubUrl);
            registerHandlers(kioskId);

            await startConnectionWithRetry(kioskId);

            step("init() finished");
        } catch (e) {
            errLog("Init error:", e);
            step("init() error", { message: e?.message, name: e?.name, stack: e?.stack });
            setUserStatus("internal");
        }
    }

    // =========================
    // DOM Ready
    // =========================
    step("bootstrap - document.readyState", { readyState: document.readyState });

    if (document.readyState === "loading") {
        step("waiting for DOMContentLoaded");
        document.addEventListener("DOMContentLoaded", () => {
            step("DOMContentLoaded fired");
            init();
        });
    } else {
        step("DOM already ready - init now");
        init();
    }
})();
