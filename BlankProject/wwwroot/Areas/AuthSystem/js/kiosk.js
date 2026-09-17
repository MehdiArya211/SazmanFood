(function () {
    "use strict";

    const settings = {
        kioskKey: "KioskId",
        hubPath: "/kioskHub",
        finalizeUrl: "/Authentication/FinalizeFaceLogin",
        maxStartRetries: 8,
        retryDelayMs: 1500
    };

    let connection;
    let redirected = false;

    function setStatus(message) {
        const element = document.getElementById("connectionStatus");
        if (element) {
            element.textContent = message;
        }
    }

    function wait(milliseconds) {
        return new Promise(resolve => setTimeout(resolve, milliseconds));
    }

    function getConfiguredKioskId() {
        const kioskId = Number(window.KIOSK_ID);

        if (!Number.isInteger(kioskId) || kioskId <= 0) {
            throw new Error("شناسه کیوسک در تنظیمات سامانه معتبر نیست.");
        }

        try {
            // فقط برای سازگاری و امکان بررسی در Console ذخیره می‌شود؛
            // منبع اصلی شناسه، تنظیمات سرور است و مقدار تصادفی تولید نمی‌شود.
            localStorage.setItem(settings.kioskKey, String(kioskId));
        } catch {
            // غیرفعال بودن Local Storage مانع اتصال کیوسک نمی‌شود.
        }

        console.info("[FaceLogin] Configured KioskId:", kioskId);
        return kioskId;
    }

    function getHubUrl() {
        const apiBaseUrl = String(window.API_BASE_URL || "").replace(/\/$/, "");

        if (!apiBaseUrl) {
            throw new Error("آدرس سرویس تشخیص چهره تنظیم نشده است.");
        }

        return apiBaseUrl + settings.hubPath;
    }

    function redirectToLogin(kioskId, enrollId) {
        const parsedEnrollId = Number(enrollId);

        if (redirected || !Number.isInteger(parsedEnrollId) || parsedEnrollId <= 0) {
            return;
        }

        redirected = true;
        setStatus("چهره شناسایی شد؛ در حال ورود به سامانه...");

        const query = new URLSearchParams({
            kioskId: String(kioskId),
            enrollId: String(parsedEnrollId)
        });

        window.location.assign(settings.finalizeUrl + "?" + query.toString());
    }

    function buildConnection(hubUrl, kioskId) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                withCredentials: false,
                transport: signalR.HttpTransportType.WebSockets |
                    signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .build();

        connection.on("ReceiveAutoLogin", enrollId => redirectToLogin(kioskId, enrollId));

        connection.onreconnecting(() => {
            setStatus("ارتباط موقتاً قطع شد؛ در حال اتصال مجدد...");
        });

        connection.onreconnected(async () => {
            try {
                // پس از اتصال مجدد، دستگاه باید دوباره در Hub ثبت شود.
                await connection.invoke("Register", kioskId);
                console.info("[FaceLogin] Kiosk registered after reconnect.", {
                    kioskId: kioskId,
                    connectionId: connection.connectionId
                });
                setStatus("اتصال برقرار است؛ کیوسک " + kioskId + " در انتظار تشخیص چهره است...");
            } catch (error) {
                console.error("[FaceLogin] Register after reconnect failed.", error);
                setStatus("ثبت مجدد دستگاه انجام نشد؛ صفحه را تازه‌سازی کنید.");
            }
        });

        connection.onclose(() => {
            setStatus("ارتباط با دستگاه قطع شد؛ صفحه را تازه‌سازی کنید.");
        });
    }

    async function startConnection(kioskId) {
        for (let attempt = 1; attempt <= settings.maxStartRetries; attempt++) {
            try {
                setStatus("در حال اتصال به دستگاه... (" + attempt + " از " + settings.maxStartRetries + ")");
                await connection.start();
                await connection.invoke("Register", kioskId);
                console.info("[FaceLogin] Kiosk registered successfully.", {
                    kioskId: kioskId,
                    connectionId: connection.connectionId
                });
                setStatus("اتصال برقرار است؛ کیوسک " + kioskId + " در انتظار تشخیص چهره است...");
                return;
            } catch (error) {
                if (attempt === settings.maxStartRetries) {
                    console.error("[FaceLogin] Connection failed.", error);
                    setStatus("اتصال به دستگاه برقرار نشد؛ صفحه را تازه‌سازی کنید.");
                    return;
                }

                await wait(settings.retryDelayMs);
            }
        }
    }

    async function initialize() {
        try {
            if (!window.signalR || !window.signalR.HubConnectionBuilder) {
                throw new Error("کتابخانه SignalR بارگذاری نشده است.");
            }

            const kioskId = getConfiguredKioskId();
            buildConnection(getHubUrl(), kioskId);
            await startConnection(kioskId);
        } catch (error) {
            console.error("[FaceLogin] Initialization failed.", error);
            setStatus(error.message || "خطا در راه‌اندازی تشخیص چهره.");
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize, { once: true });
    } else {
        initialize();
    }
})();
