(function () {
    "use strict";

    const settings = {
        deviceId: null,
        kioskKey: "KioskId",
        hubUrl: "/authHub",
        pollIntervalMs: 1000,
        maxStartRetries: 8,
        retryDelayMs: 1500,
        dashboardUrl: "/Admin/Dashboard"
    };

    let connection = null;
    let pollTimer = null;
    let requestInProgress = false;
    let redirected = false;
    let lastAuthState = null;
    let lastInvokeError = null;

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

    function stopPolling() {
        if (pollTimer !== null) {
            window.clearInterval(pollTimer);
            pollTimer = null;
            logInfo("بررسی دوره‌ای Redis متوقف شد.");
        }
    }

    async function notifyAuthEvent() {
        if (redirected || requestInProgress ||
            !connection || connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        requestInProgress = true;

        try {
            await connection.invoke("NotifyAuthEvent", settings.deviceId);
        } catch (error) {
            var errorMessage = error?.message || String(error);
            if (lastInvokeError !== errorMessage) {
                lastInvokeError = errorMessage;
                logError("فراخوانی NotifyAuthEvent ناموفق بود.", error);
            }
        } finally {
            requestInProgress = false;
        }
    }

    function startPolling() {
        stopPolling();

        logInfo("بررسی Redis از طریق NotifyAuthEvent آغاز شد.", {
            deviceId: settings.deviceId,
            intervalMs: settings.pollIntervalMs
        });

        notifyAuthEvent();
        pollTimer = window.setInterval(notifyAuthEvent, settings.pollIntervalMs);
    }

    function handleAuthEvent(result) {
        if (!result) {
            if (lastAuthState !== "empty") {
                lastAuthState = "empty";
                logWarning("پاسخ AuthHub خالی است.");
            }
            return;
        }

        lastInvokeError = null;
        setStatus(result.message || "در انتظار دستگاه تشخیص چهره ...");

        var currentState =
            String(result.isSucces === true) + "|" +
            String(result.userId || 0) + "|" +
            String(result.message || "");

        // پاسخ تکراری «در انتظار دستگاه» فقط بار اول ثبت می‌شود.
        if (lastAuthState !== currentState) {
            lastAuthState = currentState;
            logInfo("وضعیت ورود بیومتریک تغییر کرد.", result);
        }

        if (result.isSucces !== true || redirected) {
            return;
        }

        if (!result.userId || Number(result.userId) <= 0) {
            logError("ورود موفق اعلام شد اما UserId معتبر نیست.", result);
            setStatus("اطلاعات کاربر شناسایی‌شده معتبر نیست؛ دوباره تلاش کنید.");
            return;
        }

        redirected = true;
        stopPolling();

        logInfo("ورود با چهره موفق بود؛ انتقال به داشبورد انجام می‌شود.", {
            userId: result.userId
        });

        setStatus("ورود موفق؛ در حال انتقال به صفحه اصلی...");
        window.location.replace(settings.dashboardUrl);
    }

    function buildConnection() {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(settings.hubUrl)
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .build();

        connection.on("NotifyAuthEvent", handleAuthEvent);

        connection.onreconnecting(error => {
            stopPolling();
            logWarning("ارتباط با AuthHub موقتاً قطع شد؛ اتصال مجدد آغاز شد.", error);
            setStatus("ارتباط موقتاً قطع شد؛ در حال اتصال مجدد...");
        });

        connection.onreconnected(connectionId => {
            logInfo("اتصال مجدد به AuthHub برقرار شد.", { connectionId: connectionId });
            setStatus("اتصال برقرار است؛ در انتظار تشخیص چهره...");
            startPolling();
        });

        connection.onclose(error => {
            stopPolling();
            logError("ارتباط با AuthHub بسته شد.", error);
            setStatus("ارتباط با سامانه تشخیص چهره قطع شد؛ صفحه را تازه‌سازی کنید.");
        });
    }

    async function startConnection() {
        for (let attempt = 1; attempt <= settings.maxStartRetries; attempt++) {
            try {
                setStatus(
                    "در حال اتصال به سامانه تشخیص چهره... (" +
                    attempt + " از " + settings.maxStartRetries + ")"
                );

                logInfo("تلاش برای اتصال به AuthHub.", {
                    attempt: attempt,
                    maxAttempts: settings.maxStartRetries,
                    deviceId: settings.deviceId
                });

                await connection.start();

                logInfo("اتصال به AuthHub برقرار شد.", {
                    connectionId: connection.connectionId,
                    deviceId: settings.deviceId
                });

                setStatus("اتصال برقرار است؛ در انتظار تشخیص چهره...");
                startPolling();
                return;
            } catch (error) {
                logError("تلاش شماره " + attempt + " برای اتصال به AuthHub ناموفق بود.", error);

                if (attempt === settings.maxStartRetries) {
                    setStatus("اتصال به سامانه تشخیص چهره برقرار نشد؛ صفحه را تازه‌سازی کنید.");
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

            var storedDeviceId = 0;
            try {
                storedDeviceId = Number(localStorage.getItem(settings.kioskKey));
            } catch (error) {
                logWarning("خواندن شناسه کیوسک از مرورگر ناموفق بود.", error);
            }

            settings.deviceId =
                Number.isInteger(storedDeviceId) && storedDeviceId > 0
                    ? storedDeviceId
                    : 1;

            logInfo("راه‌اندازی جریان AuthHub + NotifyAuthEvent + Redis آغاز شد.", {
                deviceId: settings.deviceId,
                source: settings.deviceId === 1 ? "fallback" : "localStorage",
                hubUrl: settings.hubUrl
            });

            buildConnection();
            await startConnection();
        } catch (error) {
            logError("راه‌اندازی ورود بیومتریک با خطا متوقف شد.", error);
            setStatus(error.message || "خطا در راه‌اندازی تشخیص چهره.");
        }
    }

    window.addEventListener("beforeunload", stopPolling);

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize, { once: true });
    } else {
        initialize();
    }
})();
