(function () {
    "use strict";

    const settings = {
        hubPath: "/kioskHub"
    };

    const connections = [];
    const legacyProtocol = window.ZP_LEGACY_FACE_LOGIN === true;
    let redirected = false;
    let connectionFailure = "";
    let lastDeviceEventAt = Date.now();
    let waitMonitor = null;

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

        if (!baseUrl) return [];

        const mixedContent =
            window.location.protocol === "https:" && baseUrl.startsWith("http:");

        // On an HTTP site the browser can use the existing ZP hub directly.
        // An HTTPS site uses the same-origin server relay for the internal HTTP hub.
        if (legacyProtocol) {
            return [mixedContent
                ? window.location.origin + "/faceZpRelayHub"
                : baseUrl + settings.hubPath];
        }

        if (mixedContent) {
            throw new Error("قرارداد قدیمی ZP در تنظیمات انتشار غیرفعال است و مرورگر اتصال HTTP را از صفحه HTTPS مسدود می‌کند.");
        }

        return [baseUrl + settings.hubPath];
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
        for (const value of args) {
            if (legacyProtocol && (typeof value === "string" || typeof value === "number")) {
                const enrollId = parseEnrollId(value);
                if (enrollId) return { enrollId, sessionChallenge: "" };
            }

            if (!value || typeof value !== "object") continue;

            const enrollId = parseEnrollId(
                value.enrollId ?? value.EnrollId ??
                value.personCode ?? value.PersonCode ??
                value.personalCode ?? value.PersonalCode
            );
            const sessionChallenge = String(
                value.sessionChallenge ?? value.SessionChallenge ?? ""
            );
            if (enrollId && (legacyProtocol || sessionChallenge)) {
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
        setStatus("چهره شناسایی شد؛ در حال بررسی حساب و استعلام اطلاعات پرسنلی...");
        void finalizeFaceLogin(form);
    }

    async function finalizeFaceLogin(form) {
        try {
            console.info("[ورود بیومتریک][PersonnelLookup] درخواست بررسی حساب و پرسنل ارسال شد.");
            const response = await fetch(form.action, {
                method: "POST",
                body: new FormData(form),
                credentials: "same-origin",
                headers: { "X-Face-Status": "1", "Accept": "application/json" }
            });
            if (!response.ok || !String(response.headers.get("content-type")).includes("application/json")) {
                throw new Error("پاسخ سرور معتبر نیست (HTTP " + response.status + ").");
            }

            const result = await response.json();
            const details = {
                personnelLookup: result.lookup ?? "Unknown",
                personnelHttpStatus: result.personnelHttpStatus ?? null,
                requestedCodeLength: result.lookupCodeLength ?? null,
                returnedCodeLength: result.returnedCodeLength ?? null,
                account: result.account ?? "Unknown",
                success: result.status === true
            };
            if (result.status !== true) {
                console.warn("[ورود بیومتریک][PersonnelLookup] بررسی یا ایجاد حساب ناموفق بود.", details, result.message);
                setStatus(result.message || "ورود با چهره انجام نشد. دوباره تلاش کنید.");
                window.setTimeout(() => window.location.reload(), 8000);
                return;
            }

            console.info("[ورود بیومتریک][PersonnelLookup] ورود تأیید شد.", details);
            if (typeof result.redirectUrl !== "string" ||
                !result.redirectUrl.startsWith("/") ||
                result.redirectUrl.startsWith("//")) {
                throw new Error("نشانی انتقال پس از ورود معتبر نیست.");
            }
            setStatus("ورود تأیید شد؛ در حال انتقال...");
            window.location.assign(result.redirectUrl);
        } catch (error) {
            console.error("[ورود بیومتریک][PersonnelLookup] دریافت نتیجه از سرور ناموفق بود.", error);
            setStatus("نتیجهٔ ورود دریافت نشد. صفحه پس از چند ثانیه دوباره آماده می‌شود.");
            window.setTimeout(() => window.location.reload(), 8000);
        }
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

            // پیام‌های دستگاه باید روی صفحه هم دیده شوند؛ Console برای کاربر کیوسک قابل مشاهده نیست.
            if (!redirected && ["error", "warning", "warn", "success"].includes(level)) {
                setStatus(String(message).slice(0, 200));
            }

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


        connection.on("ZpConnectionStatus", (connected) => {
            setStatus(connected
                ? "اتصال برقرار است؛ در انتظار تشخیص چهره..."
                : "اتصال به ZP قطع شد؛ در حال تلاش برای اتصال مجدد...");
        });

        connection.on("ReceiveAutoLogin", (...args) => {
            lastDeviceEventAt = Date.now();
            console.info("[ورود بیومتریک][DeviceEvent] پیام تشخیص چهره از ZP دریافت شد.", { kioskId, argumentCount: args.length });
            const event = getBiometricEvent(args);
            if (!event) {
                logWarning("پیام ZP فاقد کد پرسنلی معتبر بود.");
                setStatus("دستگاه چهره‌ای اعلام کرد، اما کد پرسنلی معتبر نفرستاد. با پشتیبانی تماس بگیرید.");
                return;
            }

            if (!legacyProtocol && event.sessionChallenge !== getFaceChallenge()) {
                logWarning("پیام ZP فاقد شناسه نشست معتبر بود یا به مرورگر دیگری تعلق داشت.");
                setStatus("داده تشخیص چهره به این نشست تعلق ندارد؛ منتظر تأیید معتبر دستگاه بمانید.");
                return;
            }

            console.info("[ورود بیومتریک][DeviceEvent] پیام معتبر است؛ درخواست استعلام پرسنلی به سرور ارسال می‌شود.", { kioskId });
            redirectToFinalize(kioskId, event.enrollId);
        });

        connection.onreconnecting((error) => {
            logWarning("اتصال به ZP قطع شد؛ تلاش برای اتصال مجدد...", error);
            setStatus("اتصال دستگاه قطع شده؛ در حال اتصال مجدد...");
        });

        connection.onclose((error) => {
            logError("اتصال ZP بسته شد؛ صفحه را تازه‌سازی کنید.", error);
            setStatus("ارتباط با دستگاه بسته شد؛ صفحه را تازه‌سازی کنید.");
        });

        connection.onreconnected(async () => {
            try {
                if (legacyProtocol) {
                    await connection.invoke("Register", kioskId);
                } else {
                    await connection.invoke("Register", kioskId, getFaceChallenge());
                }
                logInfo("اتصال و ثبت مجدد کیوسک موفق بود؛ منتظر رویداد دستگاه هستیم.");
                setStatus("اتصال برقرار است؛ در انتظار تشخیص چهره...");
                lastDeviceEventAt = Date.now();
            } catch (error) {
                logError("ثبت مجدد کیوسک ناموفق بود.", error);
                setStatus("اتصال دستگاه بازگشت، اما ثبت کیوسک ناموفق بود. صفحه را تازه‌سازی کنید.");
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

            if (legacyProtocol) {
                await connection.invoke("Register", kioskId);
            } else {
                await connection.invoke("Register", kioskId, getFaceChallenge());
            }

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

            const detail = String(error?.message || "");
            if (detail.includes("سرور برنامه به آدرس سرویس ZP")) {
                connectionFailure = "سرور برنامه به سرویس ZP دسترسی ندارد؛ شبکه و پورت ZP را روی سرور بررسی کنید.";
            } else if (detail.includes("ثبت شناسه دستگاه")) {
                connectionFailure = "اتصال به ZP برقرار شد، اما شناسه دستگاه در سرویس ثبت نشد.";
            } else if (detail.includes("آدرس سرویس ZP در تنظیمات")) {
                connectionFailure = "آدرس سرویس ZP در تنظیمات سرور معتبر نیست.";
            } else {
                connectionFailure = legacyProtocol
                    ? "اتصال به Hub برنامه یا سرویس ZP برقرار نشد؛ جزئیات در Console مرورگر و لاگ سرور ثبت شده است."
                    : "اتصال به سرویس ZP برقرار نشد؛ تنظیمات انتشار را بررسی کنید.";
            }

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
                    connectionFailure || "اتصال به سرویس ZP برقرار نشد. آدرس سرویس و ثبت دستگاه را بررسی کنید."
                );
                return;
            }

            setStatus("اتصال برقرار است؛ در انتظار تشخیص چهره...");
            lastDeviceEventAt = Date.now();
            if (waitMonitor) window.clearInterval(waitMonitor);
            waitMonitor = window.setInterval(() => {
                if (redirected) {
                    window.clearInterval(waitMonitor);
                    waitMonitor = null;
                    return;
                }
                const seconds = Math.floor((Date.now() - lastDeviceEventAt) / 1000);
                const connected = connections.some(c => c.state === signalR.HubConnectionState.Connected);
                if (connected) {
                    console.info("[ورود بیومتریک][Waiting]", `${seconds} ثانیه از آخرین پیام دستگاه گذشته است؛ هنوز رویداد تشخیص چهره دریافت نشده و استعلام پرسنل آغاز نشده است.`);
                    if (seconds >= 30) setStatus("اتصال برقرار است، اما هنوز پیامی از دستگاه دریافت نشده است؛ وضعیت شناسایی چهره در ZP را بررسی کنید.");
                } else {
                    logWarning("هیچ اتصال فعالی به ZP وجود ندارد؛ استعلام پرسنل آغاز نشده است.");
                    setStatus("اتصال به دستگاه برقرار نیست؛ وضعیت شبکه یا سرویس ZP را بررسی کنید.");
                }
            }, 15000);

            // چالش امنیتی سمت سرور فقط سه دقیقه اعتبار دارد.
            // صفحه کیوسک ممکن است ساعت‌ها باز بماند؛ پیش از انقضا
            // چالش نشست و ثبت دستگاه خودکار تازه می‌شوند.
            window.setTimeout(() => {
                if (!redirected) {
                    setStatus("در حال تازه‌سازی نشست امن تشخیص چهره...");
                    window.location.reload();
                }
            }, 150000);
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
