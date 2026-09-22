var startSubmition = false;
var extraQuotaPersons = {
    list: {
        table: null,
        initial: function () {
            this.table = $("#datatables").DataTable({
                drawCallback: function () { $('[data-toggle="tooltip"]').tooltip(); },
                language: { url: "/assets/datatables/fa-lang.json" },
                pagingType: "full_numbers", responsive: true,
                ajax: { url: "/FoodPlan/ExtraQuotaPersons/GetList", type: "POST", dataType: "json" },
                columns: [
                    { data: "row" }, { data: "orgTitle" }, { data: "mealTitle" },
                    { data: "fromDateFa" }, { data: "toDateFa" }, { data: "count" },
                    { data: null, render: function (_, __, row) { return row.registeredCount + " / " + row.count; } },
                    { data: null, className: "text-left", render: function (_, __, row) {
                        return "<button type='button' onclick='extraQuotaPersons.person.load(" + row.requestId + ")' class='btn btn-simple btn-info btn-icon' title='اختصاص پرسنل' data-toggle='tooltip'><i class='material-icons'>people</i></button>";
                    }}
                ],
                serverSide: false, order: [0, "asc"], processing: true,
                columnDefs: [{ targets: [7], orderable: false }]
            });
        },
        reload: function () { if (this.table) this.table.ajax.reload(null, false); }
    },
    form: {
        initial: function () {
            $(".selectpicker").selectpicker("refresh");
            if ($.material) $.material.init();
            var forms = $(".official-form,.duty-form").removeData("validator").removeData("unobtrusiveValidation");
            if ($.validator && $.validator.unobtrusive) $.validator.unobtrusive.parse(forms);
            $("#OfficialPersonCode").on("input", extraQuotaPersons.official.clear)
                .on("keydown", function (e) { if (e.keyCode === 13) { e.preventDefault(); extraQuotaPersons.official.search(); } });
            $(".numeric-only").on("input", function () { this.value = this.value.replace(/\D/g, ""); });
        },
        errorText: function (xhr, fallback) {
            var data = xhr && xhr.responseJSON;
            return (data && (data.message || data.Message)) || fallback;
        },
        submit: function (form, onSuccess) {
            if (startSubmition) return false;
            form.validate();
            if (!form.valid()) return false;
            startSubmition = true;
            var button = form.find("button[type=submit]").prop("disabled", true);
            form.find(".error").hide().empty();
            $.ajax({ url: form.attr("action"), type: "POST", data: form.serialize(), dataType: "json" })
                .done(function (res) {
                    if (res.status) onSuccess(res);
                    else form.find(".error").html(res.message || "ثبت اطلاعات انجام نشد.").show();
                })
                .fail(function (xhr) { form.find(".error").text(extraQuotaPersons.form.errorText(xhr, "ارتباط با سرور برقرار نشد.")).show(); })
                .always(function () { startSubmition = false; button.prop("disabled", false); });
            return false;
        }
    },
    person: {
        load: function (id) {
            $("#modal-form").html("<div class='modal-body text-center'><i class='fa fa-spinner fa-spin fa-2x'></i><p>در حال دریافت اطلاعات...</p></div>");
            modal.open();
            $.ajax({ url: "/FoodPlan/ExtraQuotaPersons/LoadPersonsForm/" + id, cache: false })
                .done(function (res) { $("#modal-form").html(res); extraQuotaPersons.form.initial(); })
                .fail(function (xhr) { $("#modal-form").html("<div class='modal-body'><div class='alert alert-danger'>" + extraQuotaPersons.form.errorText(xhr, "دریافت اطلاعات با خطا همراه بود.") + "</div></div>"); });
        },
        delete: function (id, requestId) {
            swal({ title: "آیا مطمئنید؟", text: "پرسنل انتخاب‌شده حذف می‌شود.", type: "warning", showCancelButton: true,
                confirmButtonClass: "btn btn-danger", cancelButtonClass: "btn btn-default", confirmButtonText: "بله، حذف شود", cancelButtonText: "لغو", buttonsStyling: false
            }).then(function (confirmed) { if (confirmed) extraQuotaPersons.person.confirmDelete(id, requestId); });
        },
        confirmDelete: function (id, requestId) {
            var token = $("#operation-token-form input[name='__RequestVerificationToken']").val();
            $.post("/FoodPlan/ExtraQuotaPersons/DeletePerson/" + id, { __RequestVerificationToken: token })
                .done(function (res) {
                    if (res.status) { extraQuotaPersons.list.reload(); extraQuotaPersons.person.load(requestId); showNotification(res.message, "success"); }
                    else swal("حذف نشد", res.message, "error");
                })
                .fail(function (xhr) { swal("حذف نشد", extraQuotaPersons.form.errorText(xhr, "ارتباط با سرور برقرار نشد."), "error"); });
        }
    },
    official: {
        clear: function () {
            $("#OfficialPersonId,#OfficialNationalCode,#OfficialRankTitle,#OfficialFullName,#OfficialRankTitleDisplay,#OfficialFullNameDisplay").val("");
            $("#OfficialSaveButton").prop("disabled", true);
        },
        search: function () {
            var code = $.trim($("#OfficialPersonCode").val());
            if (!code) { showNotification("کد پرسنلی را وارد کنید.", "danger"); return; }
            extraQuotaPersons.official.clear();
            var button = $("#OfficialSearchButton").prop("disabled", true);
            $.getJSON("/FoodPlan/ExtraQuotaPersons/GetOfficialPerson", { personCode: code })
                .done(function (res) {
                    if (!res.status) { showNotification(res.message || "پرسنل یافت نشد.", "danger"); return; }
                    var m = res.model;
                    $("#OfficialPersonId").val(m.id); $("#OfficialNationalCode").val(m.nationalCode);
                    $("#OfficialRankTitle,#OfficialRankTitleDisplay").val(m.rankTitle);
                    $("#OfficialFullName,#OfficialFullNameDisplay").val(m.fullName);
                    $("#OfficialSaveButton").prop("disabled", false);
                    showNotification("پرسنل یافت شد؛ سالن را انتخاب و ذخیره کنید.", "success");
                })
                .fail(function (xhr) { showNotification(extraQuotaPersons.form.errorText(xhr, "جستجوی پرسنل با خطا همراه بود."), "danger"); })
                .always(function () { button.prop("disabled", false); });
        },
        save: function (e) {
            e.preventDefault();
            var form = $(".official-form");
            if (!$("#OfficialPersonId").val()) { form.find(".error").text("ابتدا کد پرسنلی را جستجو و فرد را انتخاب کنید.").show(); return false; }
            return extraQuotaPersons.form.submit(form, function (res) {
                var id = form.find("[name=RequestId]").val(); extraQuotaPersons.list.reload(); extraQuotaPersons.person.load(id); showNotification(res.message, "success");
            });
        }
    },
    duty: {
        save: function (e) {
            e.preventDefault();
            var form = $(".duty-form");
            var nationalCode = form.find("[name=NationalCode]").val();
            if (!/^\d{10}$/.test(nationalCode)) { form.find(".error").text("کد ملی باید دقیقاً ۱۰ رقم باشد.").show(); return false; }
            return extraQuotaPersons.form.submit(form, function (res) {
                var id = form.find("[name=RequestId]").val(); extraQuotaPersons.list.reload(); extraQuotaPersons.person.load(id); showNotification(res.message, "success");
            });
        }
    }
};
$(document).ready(function () { extraQuotaPersons.list.initial(); });