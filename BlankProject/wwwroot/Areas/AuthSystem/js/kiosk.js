(function () {
    "use strict";

    const settings = {
        kioskKey: "KioskId",
        hubPath: "/kioskHub",
        finalizeUrl: "/Authentication/FinalizeFaceLogin"
    };

    const connections = [];
    let redirected = false;

    function logInfo(message) {
        console.info("[ورود بیومتریک] " + message);
    }

    function logWarning(message, error) {
        const detail = error?.message ? " - " + error.message : "";
        console.warn("[ورود بیومتریک] " + message + detail);
    }

    function logError(message, error) {
        const detail = error?.message ? " - " + error.message : "";
        console.error("[ورود بیومتریک] " + message + detail);
    }

    function setStatus(message) {
        const element = document.getElementById("connectionStatus");
        if (element) {
            element.textContent = message;
        }
    }

    function getKioskId() {
        const queryKioskId = Number(
            new URLSearchParams(window.location.search).get("kioskId")
        );
        const configuredKioskId = Number(window.ZP_KIOSK_ID);
        let kioskId = 0;

        try {
            if (Number.isInteger(queryKioskId) && queryKioskId > 0) {
                kioskId = queryKioskId;

                const cleanUrl = new URL(window.location.href);
                cleanUrl.searchParams.delete("kioskId");
                window.history.replaceState({}, document.title, cleanUrl.toString());
            } else if (Number.isInteger(configuredKioskId) && configuredKioskId > 0) {
                kioskId = configuredKioskId;
            } else {
                kioskId = Number(localStorage.getItem(settings.kioskKey));
            }

            if (Number.isInteger(kioskId) && kioskId > 0) {
                localStorage.setItem(settings.kioskKey, String(kioskId));
            }
        } catch (error) {
            logWarning("خواندن شناسه کیوسک ناموفق بود.", error);
        }

        if (!Number.isInteger(kioskId) || kioskId <= 0) {
            throw new Error("شناسه کیوسک تنظیم نشده است.");
        }

        return kioskId;
    }

    function getHubUrls() {
        const baseUrl = String(window.API_BASE_URL || "")
            .trim()
            .replace(/\/$/, "");

        return baseUrl
            ? [baseUrl + settings.hubPath]
            : [];
    }

    function extractEnrollId(args) {
        for (const value of args) {
            if (value === null || value === undefined) {
                continue;
            }

            if (typeof value === "number" || typeof value === "string") {
                const parsed = Number(value);
                if (Number.isInteger(parsed) && parsed > 0) {
                    return parsed;
                }
                continue;
            }

            if (typeof value === "object") {
                const candidate =
                    value.enrollId ??
                    value.EnrollId ??
                    value.personCode ??
                    value.PersonCode ??
                    value.personalCode ??
                    value.PersonalCode;

                const parsed = Number(candidate);
                if (Number.isInteger(parsed) && parsed > 0) {
                    return parsed;
                }
            }
        }

        return 0;
    }

    function redirectToFinalize(kioskId, enrollId) {
        if (redirected) {
            return;
        }

        const parsedEnrollId = Number(enrollId);
        if (!Number.isInteger(parsedEnrollId) || parsedEnrollId <= 0) {
            logError("کد پرسنلی دریافتی از ZP معتبر نیست.");
            setStatus("کد پرسنلی دریافتی معتبر نیست.");
            return;
        }

        redirected = true;
        setStatus("چهره شناسایی شد؛ در حال ورود...");
        logInfo("کد پرسنلی " + parsedEnrollId + " دریافت شد.");

        const query = new URLSearchParams({
            kioskId: String(kioskId),
            enrollId: String(parsedEnrollId)
        });

        window.location.replace(settings.finalizeUrl + "?" + query.toString());
    }

    function createConnection(hubUrl, kioskId) {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                withCredentials: false,
                transport:
                    signalR.HttpTransportType.WebSockets |
                    signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .build();

        connection.on("ReceiveAutoLogin", (...args) => {
            const enrollId = extractEnrollId(args);
            if (enrollId <= 0) {
                logError("پیام ZP دریافت شد، اما کد پرسنلی معتبر نبود.");
                return;
            }

            redirectToFinalize(kioskId, enrollId);
        });

        connection.onreconnected(async () => {
            try {
                await connection.invoke("Register", kioskId);
            } catch (error) {
                logError("ثبت مجدد کیوسک ناموفق بود.", error);
            }
        });

        return connection;
    }

    async function connect(connection, hubUrl, kioskId) {
        try {
            await connection.start();
            await connection.invoke("Register", kioskId);

            connections.push(connection);
            logInfo("کیوسک " + kioskId + " به " + hubUrl + " متصل شد.");
            return true;
        } catch (error) {
            logWarning("اتصال به " + hubUrl + " برقرار نشد.", error);

            try {
                await connection.stop();
            } catch {
                // اتصال آغاز نشده است.
            }

            return false;
        }
    }

    async function initialize() {
        try {
            if (!window.signalR?.HubConnectionBuilder) {
                throw new Error("کتابخانه SignalR بارگذاری نشده است.");
            }

            const kioskId = getKioskId();
            const hubUrls = getHubUrls();

            if (hubUrls.length === 0) {
                throw new Error("آدرس سرویس ZP تنظیم نشده است.");
            }

            logInfo("شروع اتصال ZP برای کیوسک " + kioskId + ".");
            setStatus("در حال اتصال به دستگاه...");

            const results = await Promise.all(
                hubUrls.map(hubUrl => {
                    const connection = createConnection(hubUrl, kioskId);
                    return connect(connection, hubUrl, kioskId);
                })
            );

            if (!results.some(Boolean)) {
                setStatus("اتصال به سرویس ZP برقرار نشد.");
                return;
            }

            setStatus("اتصال برقرار است؛ در انتظار تشخیص چهره...");
        } catch (error) {
            logError("راه‌اندازی ورود بیومتریک متوقف شد.", error);
            setStatus(error.message || "خطا در اتصال تشخیص چهره.");
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize, { once: true });
    } else {
        initialize();
    }
})();
