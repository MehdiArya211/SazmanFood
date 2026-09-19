(function () {
    "use strict";

    const settings = {
        kioskKey: "KioskId",
        hubPath: "/kioskHub",
        finalizeUrl: "/Authentication/FinalizeFaceLogin",
        maxStartRetries: 8,
        retryDelayMs: 1500
    };

    let connection = null;
    let redirected = false;
    let lastConnectionError = null;

    function logInfo(message, data) {
        if (data === undefined) {
            console.log("[ورود بیومتریک] " + message);
        } else {
            console.log("[ورود بیومتریک] " + message, data);
        }
    }

    function logWarning(message, data) {
        console.warn("[ورود بیومتریک] " + message, data ?? "");
    }

    function logError(message, error) {
        console.error("[ورود بیومتریک] " + message, error ?? "");
    }

    function setStatus(message) {
        const element = document.getElementById("connectionStatus");
        if (element) {
            element.textContent = message;
        }
    }

    function wait(milliseconds) {
        return new Promise(resolve => setTimeout(resolve, milliseconds));
    }

    function getKioskId() {
        let kioskId = 0;

        try {
            kioskId = Number(localStorage.getItem(settings.kioskKey));
        } catch (error) {
            logWarning("خواندن شناسه کیوسک از مرورگر ناموفق بود.", error);
        }

        if (!Number.isInteger(kioskId) || kioskId <= 0) {
            throw new Error(
                "شناسه کیوسک در مرورگر ثبت نشده است. ابتدا KioskId دستگاه را تنظیم کنید."
            );
        }

        window.KIOSK_ID = kioskId;
        return kioskId;
    }

    function getHubUrl() {
        const apiBaseUrl = String(window.API_BASE_URL || "").replace(/\/$/, "");

        if (!apiBaseUrl) {
            throw new Error("آدرس سرویس ZP تنظیم نشده است.");
        }

        return apiBaseUrl + settings.hubPath;
    }

    function redirectToFinalize(kioskId, enrollId) {
        const parsedEnrollId = Number(enrollId);

        if (redirected) {
            return;
        }

        if (!Number.isInteger(parsedEnrollId) || parsedEnrollId <= 0) {
            logError("EnrollId دریافت‌شده از سرویس ZP معتبر نیست.", {
                kioskId: kioskId,
                enrollId: enrollId
            });
            setStatus("اطلاعات چهره شناسایی‌شده معتبر نیست.");
            return;
        }

        redirected = true;
        setStatus("چهره شناسایی شد؛ در حال ورود به سامانه...");

        const query = new URLSearchParams({
            kioskId: String(kioskId),
            enrollId: String(parsedEnrollId)
        });

        logInfo("چهره شناسایی شد؛ انتقال به مرحله نهایی ورود.", {
            kioskId: kioskId,
            enrollId: parsedEnrollId
        });

        window.location.replace(
            settings.finalizeUrl + "?" + query.toString()
        );
    }

    function buildConnection(hubUrl, kioskId) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                withCredentials: false,
                transport:
                    signalR.HttpTransportType.WebSockets |
                    signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .build();

        connection.on("ReceiveAutoLogin", enrollId => {
            redirectToFinalize(kioskId, enrollId);
        });

        connection.onreconnecting(error => {
            logWarning("ارتباط با سرویس ZP قطع شد؛ اتصال مجدد آغاز شد.", error);
            setStatus("ارتباط موقتاً قطع شد؛ در حال اتصال مجدد...");
        });

        connection.onreconnected(async connectionId => {
            try {
                await connection.invoke("Register", kioskId);
                lastConnectionError = null;

                logInfo("اتصال مجدد به سرویس ZP برقرار شد.", {
                    kioskId: kioskId,
                    connectionId: connectionId
                });

                setStatus("اتصال برقرار است؛ در انتظار تشخیص چهره...");
            } catch (error) {
                logError("ثبت مجدد کیوسک در سرویس ZP ناموفق بود.", error);
                setStatus("ثبت مجدد دستگاه ناموفق بود؛ صفحه را تازه‌سازی کنید.");
            }
        });

        connection.onclose(error => {
            logError("ارتباط با سرویس ZP بسته شد.", error);
            setStatus("ارتباط با دستگاه قطع شد؛ صفحه را تازه‌سازی کنید.");
        });
    }

    async function startConnection(kioskId) {
        for (let attempt = 1; attempt <= settings.maxStartRetries; attempt++) {
            try {
                setStatus(
                    "در حال اتصال به دستگاه... (" +
                    attempt + " از " + settings.maxStartRetries + ")"
                );

                await connection.start();
                await connection.invoke("Register", kioskId);

                lastConnectionError = null;

                logInfo("اتصال و ثبت کیوسک در سرویس ZP موفق بود.", {
                    kioskId: kioskId,
                    connectionId: connection.connectionId
                });

                setStatus("اتصال برقرار است؛ در انتظار تشخیص چهره...");
                return;
            } catch (error) {
                const errorMessage = error?.message || String(error);

                if (lastConnectionError !== errorMessage) {
                    lastConnectionError = errorMessage;
                    logError(
                        "اتصال یا ثبت کیوسک در سرویس ZP ناموفق بود.",
                        error
                    );
                }

                if (attempt === settings.maxStartRetries) {
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

            const kioskId = getKioskId();
            const hubUrl = getHubUrl();

            logInfo("راه‌اندازی ورود بیومتریک ZP آغاز شد.", {
                kioskId: kioskId,
                hubUrl: hubUrl
            });

            buildConnection(hubUrl, kioskId);
            await startConnection(kioskId);
        } catch (error) {
            logError("راه‌اندازی ورود بیومتریک ZP متوقف شد.", error);
            setStatus(error.message || "خطا در راه‌اندازی تشخیص چهره.");
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize, { once: true });
    } else {
        initialize();
    }
})();
