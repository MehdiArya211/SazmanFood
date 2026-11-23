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
    const mapping = document.getElementById("mealbooker-mapping");
    const hasHeader = document.getElementById("mealbooker-has-header");
    const combinedName = document.getElementById("mealbooker-combined-name");
    const familyField = document.getElementById("mealbooker-family-field");
    const mappingHint = document.getElementById("mealbooker-name-hint");
    const codeColumn = document.getElementById("mealbooker-code-column");
    const nameColumn = document.getElementById("mealbooker-name-column");
    const familyColumn = document.getElementById("mealbooker-family-column");
    let previewReady = false;
    let previewSequence = 0;

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

    function normalizedHeader(value) {
        return String(value || "").trim().replace(/ي/g, "ی").replace(/ك/g, "ک")
            .replace(/[^\p{L}\p{N}]/gu, "").toLowerCase();
    }

    function fillColumnSelect(select, columns) {
        select.replaceChildren();
        const placeholder = new Option("ستون را انتخاب کنید", "");
        select.add(placeholder);
        for (const column of columns) {
            const title = column.title || "بدون عنوان";
            const sample = column.sample ? " · نمونه: " + String(column.sample).slice(0, 28) : "";
            select.add(new Option(column.letter + " — " + title + sample, String(column.index)));
        }
    }

    function chooseByHeader(select, columns, names) {
        const found = columns.find(column => names.includes(normalizedHeader(column.title)));
        if (found) select.value = String(found.index);
        return !!found;
    }

    function selectedColumnsAreValid() {
        const code = codeColumn.value;
        const name = nameColumn.value;
        const family = combinedName.checked ? "" : familyColumn.value;
        return !!code && !!name && code !== name &&
            (combinedName.checked || (!!family && family !== code && family !== name));
    }

    function updateMappingState() {
        familyField.hidden = combinedName.checked;
        familyColumn.disabled = combinedName.checked;
        const valid = selectedColumnsAreValid();
        submit.disabled = !previewReady || !valid;
        mappingHint.textContent = valid
            ? (combinedName.checked
                ? "نام و نشان همان‌طور که در ستون انتخابی است ذخیره می‌شود؛ نام خانوادگی جداگانه لازم نیست."
                : "ستون‌های نام و نام خانوادگی جداگانه خوانده می‌شوند.")
            : "برای ادامه، ستون‌های لازم را انتخاب کنید. هر ستون فقط برای یک مورد قابل استفاده است.";
    }

    async function previewColumns() {
        const requestId = ++previewSequence;
        previewReady = false;
        mapping.hidden = true;
        submit.disabled = true;
        resultsPanel.hidden = true;
        alertBox.hidden = true;
        if (!fileInput.files.length) return;
        const file = fileInput.files[0];
        if (!/\.xlsx$/i.test(file.name) || file.size > 5 * 1024 * 1024) {
            showError("فقط فایل xlsx با حجم حداکثر ۵ مگابایت پذیرفته می‌شود.");
            return;
        }

        fileName.textContent = file.name + " · در حال خواندن ستون‌ها...";
        try {
            const body = new FormData(form);
            body.set("hasHeader", String(hasHeader.checked));
            const response = await fetch(form.dataset.previewUrl, {
                method: "POST", body, credentials: "same-origin",
                headers: { "Accept": "application/json", "X-Requested-With": "XMLHttpRequest" }
            });
            if (!response.ok) throw new Error("پیش‌نمایش فایل ممکن نشد (HTTP " + response.status + ").");
            const data = await response.json();
            if (requestId !== previewSequence) return;
            if (!data.status || !Array.isArray(data.columns)) {
                showError(data.message || "ستون‌های فایل خوانده نشد.");
                return;
            }
            for (const select of [codeColumn, nameColumn, familyColumn])
                fillColumnSelect(select, data.columns);

            if (hasHeader.checked) {
                chooseByHeader(codeColumn, data.columns,
                    ["کدپرسنلی", "personcode", "personnelcode"]);
                const isFull = chooseByHeader(nameColumn, data.columns,
                    ["نامونشان", "نامکامل", "نامونامخانوادگی", "fullname"]);
                if (!isFull) chooseByHeader(nameColumn, data.columns, ["نام", "firstname"]);
                chooseByHeader(familyColumn, data.columns,
                    ["نامخانوادگی", "lastname", "familyname"]);
                combinedName.checked = isFull;
            } else {
                combinedName.checked = false;
            }
            previewReady = true;
            mapping.hidden = false;
            updateMappingState();
        } catch (error) {
            if (requestId === previewSequence)
                showError(error.message || "پیش‌نمایش اکسل ممکن نشد.");
        } finally {
            if (requestId === previewSequence) fileName.textContent = file.name;
        }
    }

    fileInput.addEventListener("change", previewColumns);
    hasHeader.addEventListener("change", previewColumns);
    combinedName.addEventListener("change", updateMappingState);
    for (const select of [codeColumn, nameColumn, familyColumn])
        select.addEventListener("change", updateMappingState);

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
        if (!fileInput.files.length || !previewReady || !selectedColumnsAreValid()) {
            showError("ابتدا فایل را بارگذاری کنید و ستون‌های کد پرسنلی و نام را درست انتخاب کنید.");
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
                body: (() => {
                    const body = new FormData(form);
                    body.set("hasHeader", String(hasHeader.checked));
                    body.set("familyColumn", combinedName.checked ? "0" : familyColumn.value);
                    return body;
                })(),
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
            updateMappingState();
            submit.innerHTML = '<i class="material-icons">group_add</i> بررسی و ایجاد حساب‌ها';
        }
    });
})();
