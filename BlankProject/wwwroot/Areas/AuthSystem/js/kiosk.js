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

    function getOrCreateKioskId() {
        let kioskId;
        let isNew = false;

        try {
            kioskId = Number(localStorage.getItem(settings.kioskKey));
        } catch {
            kioskId = 0;
        }

        if (!Number.isInteger(kioskId) || kioskId <= 0) {
            kioskId = Math.floor(100000 + Math.random() * 900000);
            isNew = true;

            try {
                localStorage.setItem(settings.kioskKey, String(kioskId));
            } catch {
                // ذخیره‌سازی محلی ممکن است در حالت خصوصی مرورگر غیرفعال باشد.
            }
        }

        // برای مشاهده و کپی سریع شناسه در Console
        window.KIOSK_ID = kioskId;

        console.log("==============================================");
        console.log(
            "%cKIOSK ID: " + kioskId,
            "color:#ffffff;background:#0b7a3e;font-size:22px;font-weight:bold;padding:8px 14px;border-radius:6px"
        );
        console.log("وضعیت شناسه: " + (isNew ? "جدید ساخته شد" : "از مرورگر خوانده شد"));
        console.log("برای مشاهده مجدد در Console بنویسید: KIOSK_ID");
        console.log("==============================================");

        return kioskId;
    }

    function getHubUrl() {
        const apiBaseUrl = String(window.API_BASE_URL || "").replace(/\/$/, "");

        if (!apiBaseUrl) {
            throw new Error("آدرس سرویس تشخیص چهره تنظیم نشده است.");
        }

        const hubUrl = apiBaseUrl + settings.hubPath;
        logInfo("آدرس Hub آماده شد: " + hubUrl);
        return hubUrl;
    }

    function redirectToLogin(kioskId, enrollId) {
        const parsedEnrollId = Number(enrollId);

        if (redirected) {
            logWarning("انتقال قبلاً انجام شده و پیام تکراری نادیده گرفته شد.", {
                kioskId: kioskId,
                enrollId: enrollId
            });
            return;
        }

        if (!Number.isInteger(parsedEnrollId) || parsedEnrollId <= 0) {
            logError("شناسه پرسنلی دریافتی از Hub معتبر نیست.", {
                kioskId: kioskId,
                enrollId: enrollId
            });
            return;
        }

        redirected = true;
        logInfo("چهره شناسایی شد و انتقال به سامانه آغاز می‌شود.", {
            kioskId: kioskId,
            enrollId: parsedEnrollId
        });
        setStatus("چهره شناسایی شد؛ در حال ورود به سامانه...");

        const query = new URLSearchParams({
            kioskId: String(kioskId),
            enrollId: String(parsedEnrollId)
        });

        const destination = settings.finalizeUrl + "?" + query.toString();
        logInfo("انتقال به آدرس نهایی ورود: " + destination);
        window.location.assign(destination);
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

        connection.on("ReceiveAutoLogin", enrollId => {
            logInfo("پیام ReceiveAutoLogin از Hub دریافت شد.", {
                kioskId: kioskId,
                enrollId: enrollId
            });
            redirectToLogin(kioskId, enrollId);
        });

        connection.onreconnecting(error => {
            logWarning("ارتباط با Hub موقتاً قطع شد؛ اتصال مجدد آغاز شد.", error);
            setStatus("ارتباط موقتاً قطع شد؛ در حال اتصال مجدد...");
        });

        connection.onreconnected(async connectionId => {
            logInfo("اتصال مجدد با Hub برقرار شد.", {
                kioskId: kioskId,
                connectionId: connectionId
            });
            try {
                // پس از اتصال مجدد، دستگاه باید دوباره در Hub ثبت شود.
                await connection.invoke("Register", kioskId);
                console.info("[FaceLogin] Kiosk registered after reconnect.", {
                    kioskId: kioskId,
                    connectionId: connection.connectionId
                });
                setStatus("کیوسک " + kioskId + " متصل است؛ در انتظار تشخیص چهره...");
            } catch (error) {
                logError("ثبت مجدد KioskId در Hub ناموفق بود.", error);
                setStatus("ثبت مجدد دستگاه انجام نشد؛ صفحه را تازه‌سازی کنید.");
            }
        });

        connection.onclose(error => {
            logError("ارتباط با Hub کاملاً بسته شد.", error);
            setStatus("ارتباط با دستگاه قطع شد؛ صفحه را تازه‌سازی کنید.");
        });
    }

    async function startConnection(kioskId) {
        for (let attempt = 1; attempt <= settings.maxStartRetries; attempt++) {
            try {
                setStatus("در حال اتصال به دستگاه... (" + attempt + " از " + settings.maxStartRetries + ")");
                logInfo("تلاش برای اتصال به Hub.", {
                    attempt: attempt,
                    maxAttempts: settings.maxStartRetries,
                    kioskId: kioskId
                });

                await connection.start();

                logInfo("اتصال WebSocket/SignalR برقرار شد؛ Register ارسال می‌شود.", {
                    kioskId: kioskId,
                    connectionId: connection.connectionId
                });

                await connection.invoke("Register", kioskId);

                console.info("[FaceLogin] Kiosk registered successfully.", {
                    kioskId: kioskId,
                    connectionId: connection.connectionId
                });

                setStatus("کیوسک " + kioskId + " متصل است؛ در انتظار تشخیص چهره...");
                return;
            } catch (error) {
                logError(
                    "تلاش شماره " + attempt + " برای اتصال یا Register ناموفق بود.",
                    error
                );

                if (attempt === settings.maxStartRetries) {
                    setStatus("اتصال به دستگاه برقرار نشد؛ صفحه را تازه‌سازی کنید.");
                    return;
                }

                logInfo("پس از " + settings.retryDelayMs + " میلی‌ثانیه دوباره تلاش می‌شود.");
                await wait(settings.retryDelayMs);
            }
        }
    }

    async function initialize() {
        try {
            if (!window.signalR || !window.signalR.HubConnectionBuilder) {
                throw new Error("کتابخانه SignalR بارگذاری نشده است.");
            }

            logInfo("راه‌اندازی ورود بیومتریک آغاز شد.");

            const kioskId = getOrCreateKioskId();
            logInfo("شناسه کیوسک برای Register آماده است.", { kioskId: kioskId });

            buildConnection(getHubUrl(), kioskId);
            logInfo("Connection و Handler رویداد ReceiveAutoLogin ساخته شدند.");

            await startConnection(kioskId);
        } catch (error) {
            logError("راه‌اندازی ورود بیومتریک با خطا متوقف شد.", error);
            setStatus(error.message || "خطا در راه‌اندازی تشخیص چهره.");
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize, { once: true });
    } else {
        initialize();
    }
})();
