(function () {
    "use strict";

    let submitting = false;

    function showError(message) {
        const errorBox = document.querySelector(".auth-alert.error");
        if (!errorBox) {
            return;
        }

        errorBox.replaceChildren();

        const icon = document.createElement("i");
        icon.className = "material-icons";
        icon.setAttribute("aria-hidden", "true");
        icon.textContent = "error_outline";

        const text = document.createElement("span");
        text.textContent = message;

        errorBox.append(icon, text);
    }

    // این تابع برای سازگاری با onsubmit فرم به‌صورت عمومی در دسترس است.
    window.Validate = function () {
        if (submitting) {
            return false;
        }

        const username = document.getElementById("Mobile");
        const password = document.getElementById("Password");
        const captcha = document.getElementById("captcha");

        if (!username.value.trim() || !password.value) {
            showError("نام کاربری و کلمه عبور را وارد کنید.");
            (!username.value.trim() ? username : password).focus();
            return false;
        }

        if (captcha && !captcha.value.trim()) {
            showError("کد امنیتی را وارد کنید.");
            captcha.focus();
            return false;
        }

        submitting = true;
        const submitButton = document.getElementById("loginSubmit");

        if (submitButton) {
            submitButton.disabled = true;
            submitButton.textContent = "در حال بررسی اطلاعات...";
        }

        return true;
    };

    const captchaImage = document.getElementById("imgcpatcha");
    if (captchaImage) {
        captchaImage.addEventListener("click", function () {
            const separator = this.src.includes("?") ? "&" : "?";
            this.src = this.src.split("?")[0] + separator + "t=" + Date.now();
        });
    }

    // نمایش پنجره راهنمای داشبورد باید در هر ورود از نو محاسبه شود.
    localStorage.removeItem("showDashboardModal");
})();
