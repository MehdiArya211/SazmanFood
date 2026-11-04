(function () {
    "use strict";

    const settings = {
        hubPath: "/kioskHub"
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
        // شناسه کیوسک فقط از تنظیمات معتبر سرور گرفته می‌شود.
        // query string یا localStorage نباید بتوانند دستگاه را عوض کنند.
        const kioskId = Number(window.ZP_KIOSK_ID);
        if (!Number.isSafeInteger(kioskId) || kioskId <= 0) {
            throw new Error("شناسه دستگاه در تنظیمات سامانه معتبر نیست.");
        }
        return kioskId;
    }

    function getFaceChallenge() {
        const challenge = String(window.ZP_FACE_CHALLENGE || "");
        if (!/^[0-9A-F]{64}$/.test(challenge)) {
            throw new Error("شناسه نشست انتظار تشخیص چهره معتبر نیست؛ صفحه را دوباره باز کنید.");
        }
        return challenge;
    }

    function getHubUrls() {
        const baseUrl = String(window.API_BASE_URL || "")
            .trim()
            .replace(/\/$/, "");

        return baseUrl
            ? [baseUrl + settings.hubPath]
            : [];
    }

    function parseEnrollId(value) {
        // Number() کدهای طولانی را گرد می‌کند و ممکن است باعث ورود فرد دیگری شود.
        let text = null;
        if (typeof value === "string") {
            text = value.trim();
        } else if (typeof value === "number" && Number.isSafeInteger(value)) {
            text = String(value);
        }

        if (!text || !/^[0-9]{1,19}$/.test(text)) return null;
        try {
            const id = BigInt(text);
            return id > 0n && id <= 9223372036854775807n ? text : null;
        } catch {
            return null;
        }
    }

    function getBiometricEvent(args) {
        // رویداد باید شناسه نشست را همراه کد پرسنلی برگرداند؛
        // صرف دریافت enrollId از یک Hub عمومی اثبات احراز هویت نیست.
        for (const value of args) {
            if (!value || typeof value !== "object") continue;

            const enrollId = parseEnrollId(
                value.enrollId ?? value.EnrollId ??
                value.personCode ?? value.PersonCode ??
                value.personalCode ?? value.PersonalCode
            );
            const sessionChallenge = String(
                value.sessionChallenge ?? value.SessionChallenge ?? ""
            );
            if (enrollId && sessionChallenge) {
                return { enrollId, sessionChallenge };
            }
        }

        return null;
    }

    function redirectToFinalize(kioskId, enrollId) {
        if (redirected) return;

        const form = document.getElementById("face-finalize-form");
        if (!form || !form.elements.namedItem("__RequestVerificationToken")) {
            setStatus("فرم امن ورود پیدا نشد؛ صفحه را دوباره باز کنید.");
            return;
        }

        const safeEnrollId = parseEnrollId(enrollId);
        if (!safeEnrollId) {
            setStatus("کد پرسنلی دریافتی از دستگاه معتبر نیست.");
            return;
        }

        form.elements.namedItem("kioskId").value = String(kioskId);
        form.elements.namedItem("enrollId").value = safeEnrollId;
        redirected = true;
        setStatus("چهره شناسایی شد؛ در حال تأیید ورود...");
        form.submit();
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


        connection.on("KioskDiagnostic", (diagnostic) => {
            if (!diagnostic) {
                console.warn(
                    "[ورود بیومتریک] پیام تشخیصی خالی دریافت شد."
                );

                return;
            }

            const level = String(
                diagnostic.level ??
                diagnostic.Level ??
                "info"
            ).toLowerCase();

            const step =
                diagnostic.step ??
                diagnostic.Step ??
                "Unknown";

            const message =
                diagnostic.message ??
                diagnostic.Message ??
                "بدون توضیح";

            const details = {
                kioskId:
                    diagnostic.kioskId ??
                    diagnostic.KioskId ??
                    null,

                enrollId:
                    diagnostic.enrollId ??
                    diagnostic.EnrollId ??
                    null,

                serialNumber:
                    diagnostic.serialNumber ??
                    diagnostic.SerialNumber ??
                    null,

                timestamp:
                    diagnostic.timestamp ??
                    diagnostic.Timestamp ??
                    new Date().toISOString()
            };

            const output =
                `[ورود بیومتریک][${step}] ${message}`;

            switch (level) {
                case "error":
                    console.error(output, details);
                    break;

                case "warning":
                case "warn":
                    console.warn(output, details);
                    break;

                case "success":
                    console.info(
                        "%c" + output,
                        "color:#198754;font-weight:bold",
                        details
                    );
                    break;

                default:
                    console.info(output, details);
                    break;
            }
        });


        connection.on("ReceiveAutoLogin", (...args) => {
            const event = getBiometricEvent(args);
            const challenge = getFaceChallenge();
            if (!event || event.sessionChallenge !== challenge) {
                logWarning("پیام ZP فاقد شناسه نشست معتبر بود یا به مرورگر دیگری تعلق داشت.");
                setStatus("داده تشخیص چهره به این نشست تعلق ندارد؛ منتظر تأیید معتبر دستگاه بمانید.");
                return;
            }

            redirectToFinalize(kioskId, event.enrollId);
        });

        connection.onreconnected(async () => {
            try {
                await connection.invoke("Register", kioskId, getFaceChallenge());
            } catch (error) {
                logError("ثبت مجدد کیوسک ناموفق بود.", error);
            }
        });

        return connection;
    }

    async function connect(
        connection,
        hubUrl,
        kioskId
    ) {
        console.info(
            "[ورود بیومتریک][Connect] تلاش برای اتصال.",
            {
                kioskId,
                hubUrl
            }
        );

        try {
            await connection.start();

            console.info(
                "[ورود بیومتریک][Connect] SignalR متصل شد.",
                {
                    kioskId,
                    connectionId: connection.connectionId
                }
            );

            await connection.invoke(
                "Register",
                kioskId,
                getFaceChallenge()
            );

            connections.push(connection);

            console.info(
                "[ورود بیومتریک][Register] کیوسک ثبت شد.",
                {
                    kioskId,
                    connectionId: connection.connectionId
                }
            );

            return true;
        } catch (error) {
            console.error(
                "[ورود بیومتریک][Connect] اتصال یا ثبت کیوسک ناموفق بود.",
                {
                    kioskId,
                    hubUrl,
                    error: error?.message ?? error
                }
            );

            try {
                await connection.stop();
            } catch {
                // اتصال شروع نشده است.
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
                setStatus(
                    "اتصال امن به ZP برقرار نشد. سرویس باید Register(kioskId, sessionChallenge) و رویداد امضاشده را پشتیبانی کند."
                );
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
