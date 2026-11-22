(function () {
    "use strict";

    const form = document.getElementById("mealbooker-import-form");
    if (!form) return;

    const fileInput = document.getElementById("mealbooker-file");
    const fileName = document.getElementById("mealbooker-file-name");
    const submit = document.getElementById("mealbooker-submit");
    const alertBox = document.getElementById("mealbooker-alert");
    const resultsPanel = document.getElementById("mealbooker-results");
    const tableBody = document.getElementById("mealbooker-result-rows");
    const emptyFilter = document.getElementById("mealbooker-empty-filter");
    const downloadFailures = document.getElementById("mealbooker-download-failures");
    let results = [];
    let activeFilter = "all";

    function showError(message) {
        alertBox.textContent = message;
        alertBox.hidden = false;
    }

    function setNumber(id, value) {
        document.getElementById(id).textContent =
            Number(value || 0).toLocaleString("fa-IR");
    }

    function renderRows() {
        tableBody.replaceChildren();
        const visible = results.filter(row =>
            activeFilter === "all" || row.status === activeFilter);

        for (const row of visible) {
            const tr = document.createElement("tr");
            for (const value of [row.row, row.personCode, row.fullName]) {
                const td = document.createElement("td");
                td.textContent = value ?? "";
                tr.appendChild(td);
            }

            const state = document.createElement("td");
            const badge = document.createElement("span");
            badge.className = "meal-import-badge " + row.status;
            badge.textContent = {
                created: "ایجاد شد",
                existing: "موجود بود",
                failed: "ناموفق"
            }[row.status] || row.status;
            state.appendChild(badge);
            tr.appendChild(state);

            const reason = document.createElement("td");
            reason.textContent = row.message ?? "";
            tr.appendChild(reason);
            tableBody.appendChild(tr);
        }
        emptyFilter.hidden = visible.length !== 0;
    }

    function csvValue(value) {
        let text = String(value ?? "");
        if (/^\s*[=+\-@]/.test(text)) text = "'" + text;
        return '"' + text.replace(/"/g, '""') + '"';
    }

    fileInput.addEventListener("change", function () {
        fileName.textContent = this.files.length
            ? this.files[0].name
            : "هنوز فایلی انتخاب نشده است.";
    });

    document.querySelectorAll(".meal-import-filters button").forEach(button => {
        button.addEventListener("click", function () {
            activeFilter = this.dataset.filter;
            document.querySelectorAll(".meal-import-filters button")
                .forEach(item => item.classList.toggle("active", item === this));
            renderRows();
        });
    });

    downloadFailures.addEventListener("click", function () {
        const failed = results.filter(row => row.status === "failed");
        const lines = [
            ["ردیف اکسل", "کد پرسنلی", "نام و نام خانوادگی", "علت"].map(csvValue).join(","),
            ...failed.map(row =>
                [row.row, row.personCode, row.fullName, row.message]
                    .map(csvValue).join(","))
        ];
        const blob = new Blob(["\uFEFF" + lines.join("\r\n")], {
            type: "text/csv;charset=utf-8"
        });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = "MealBookers-Failed.csv";
        document.body.appendChild(link);
        link.click();
        link.remove();
        window.setTimeout(() => URL.revokeObjectURL(url), 1000);
    });

    form.addEventListener("submit", async function (event) {
        event.preventDefault();
        alertBox.hidden = true;
        if (!fileInput.files.length) {
            showError("فایل اکسل را انتخاب کنید.");
            return;
        }

        const file = fileInput.files[0];
        if (!/\.xlsx$/i.test(file.name) || file.size > 5 * 1024 * 1024) {
            showError("فقط فایل xlsx با حجم حداکثر ۵ مگابایت پذیرفته می‌شود.");
            return;
        }

        submit.disabled = true;
        submit.textContent = "در حال بررسی و ثبت ردیف‌ها...";
        try {
            const response = await fetch(form.action, {
                method: "POST",
                body: new FormData(form),
                credentials: "same-origin",
                headers: { "Accept": "application/json", "X-Requested-With": "XMLHttpRequest" }
            });
            if (!response.ok) throw new Error("پاسخ سرور معتبر نیست (HTTP " + response.status + ").");
            const data = await response.json();
            if (!data.status) {
                showError(data.message || "پردازش فایل انجام نشد.");
                return;
            }

            results = Array.isArray(data.rows) ? data.rows : [];
            setNumber("mealbooker-total", data.total);
            setNumber("mealbooker-created", data.created);
            setNumber("mealbooker-existing", data.existing);
            setNumber("mealbooker-failed", data.failed);
            downloadFailures.hidden = !results.some(row => row.status === "failed");
            activeFilter = "all";
            document.querySelectorAll(".meal-import-filters button")
                .forEach(item => item.classList.toggle("active", item.dataset.filter === "all"));
            renderRows();
            resultsPanel.hidden = false;
            resultsPanel.scrollIntoView({ behavior: "smooth", block: "start" });
        } catch (error) {
            showError(error.message || "ارتباط با سرور برقرار نشد. دوباره تلاش کنید.");
        } finally {
            submit.disabled = false;
            submit.innerHTML = '<i class="material-icons">group_add</i> بررسی و ایجاد حساب‌ها';
        }
    });
})();
