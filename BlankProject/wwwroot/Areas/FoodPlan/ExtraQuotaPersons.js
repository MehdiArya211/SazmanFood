var startSubmition = false;

var extraQuotaPersons = {
    list: {
        table: null,

        initial: function () {
            this.table = $("#datatables").DataTable({
                drawCallback: function () {
                    $('[data-toggle="tooltip"]').tooltip();
                },
                language: {
                    url: "/assets/datatables/fa-lang.json"
                },
                pagingType: "full_numbers",
                responsive: true,
                ajax: {
                    url: "/FoodPlan/ExtraQuotaPersons/GetList",
                    type: "POST",
                    dataType: "json"
                },
                columns: [
                    { data: "row" },
                    { data: "orgTitle" },
                    { data: "mealTitle" },
                    { data: "fromDateFa" },
                    { data: "toDateFa" },
                    { data: "count" },
                    {
                        data: null,
                        render: function (data, type, row) {
                            return row.registeredCount + " / " + row.count;
                        }
                    },
                    {
                        data: null,
                        className: "text-left",
                        render: function (data, type, row) {
                            return "<a onclick='extraQuotaPersons.person.load(" + row.requestId + ")' class='btn btn-simple btn-info btn-icon' title='اختصاص پرسنل' data-toggle='tooltip'><i class='material-icons'>people</i></a>";
                        }
                    }
                ],
                serverSide: false,
                order: [0, "asc"],
                processing: true,
                columnDefs: [{
                    targets: [7],
                    orderable: false
                }]
            });
        },

        reload: function () {
            extraQuotaPersons.list.table.ajax.reload(null, false);
        }
    },

    form: {
        initial: function () {
            $(".selectpicker").selectpicker("refresh");
            $.material.init();

            var form = $(".official-form,.duty-form")
                .removeData("validator")
                .removeData("unobtrusiveValidation");

            $.validator.unobtrusive.parse(form);
        }
    },

    person: {
        load: function (id) {
            $.get("/FoodPlan/ExtraQuotaPersons/LoadPersonsForm/" + id, function (res) {
                $("#modal-form").html(res);
                extraQuotaPersons.form.initial();
                modal.open();
            });
        },

        delete: function (id, requestId) {
            swal({
                title: "آیا مطمئنید؟",
                text: "پرسنل انتخاب‌شده حذف می‌شود.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-danger",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm)
                    extraQuotaPersons.person.confirmDelete(id, requestId);
            });
        },

        confirmDelete: function (id, requestId) {
            var token =
                $("#operation-token-form input[name='__RequestVerificationToken']").val();

            $.post("/FoodPlan/ExtraQuotaPersons/DeletePerson/" + id, {
                __RequestVerificationToken: token
            }, function (res) {
                if (res.status) {
                    extraQuotaPersons.list.reload();
                    extraQuotaPersons.person.load(requestId);
                    showNotification(res.message, "success");
                }
                else {
                    swal("حذف نشد", res.message, "error");
                }
            });
        }
    },

    official: {
        search: function () {
            var personCode = $("#OfficialPersonCode").val();

            if (!personCode) {
                showNotification("کد پرسنلی را وارد کنید.", "danger");
                return;
            }

            $.get("/FoodPlan/ExtraQuotaPersons/GetOfficialPerson?personCode=" + encodeURIComponent(personCode), function (res) {
                if (res.status) {
                    var model = res.model;

                    $("#OfficialPersonId").val(model.id);
                    $("#OfficialNationalCode").val(model.nationalCode);
                    $("#OfficialRankTitle").val(model.rankTitle);
                    $("#OfficialFullName").val(model.fullName);

                    $("#OfficialRankTitleDisplay").val(model.rankTitle);
                    $("#OfficialFullNameDisplay").val(model.fullName);

                    showNotification("پرسنل یافت شد.", "success");
                }
                else {
                    $("#OfficialPersonId").val("");
                    $("#OfficialNationalCode").val("");
                    $("#OfficialRankTitle").val("");
                    $("#OfficialFullName").val("");
                    $("#OfficialRankTitleDisplay").val("");
                    $("#OfficialFullNameDisplay").val("");

                    showNotification(res.message || "پرسنل یافت نشد.", "danger");
                }
            });
        },

        save: function (e) {
            e.preventDefault();

            if (startSubmition)
                return false;

            startSubmition = true;

            var form = $(".official-form");
            form.validate();

            if (!form.valid()) {
                startSubmition = false;
                return false;
            }

            $.post(form.attr("action"), form.serialize(), function (res) {
                startSubmition = false;

                if (res.status) {
                    extraQuotaPersons.list.reload();

                    var requestId =
                        $(".official-form input[name='RequestId']").val();

                    extraQuotaPersons.person.load(requestId);
                    showNotification(res.message, "success");
                }
                else {
                    $(".official-form .error").html(res.message);
                }
            }).fail(function () {
                startSubmition = false;
                $(".official-form .error").html("ثبت پرسنل با خطا همراه بود.");
            });

            return false;
        }
    },

    duty: {
        save: function (e) {
            e.preventDefault();

            if (startSubmition)
                return false;

            startSubmition = true;

            var form = $(".duty-form");
            form.validate();

            if (!form.valid()) {
                startSubmition = false;
                return false;
            }

            $.post(form.attr("action"), form.serialize(), function (res) {
                startSubmition = false;

                if (res.status) {
                    extraQuotaPersons.list.reload();

                    var requestId =
                        $(".duty-form input[name='RequestId']").val();

                    extraQuotaPersons.person.load(requestId);
                    showNotification(res.message, "success");
                }
                else {
                    $(".duty-form .error").html(res.message);
                }
            }).fail(function () {
                startSubmition = false;
                $(".duty-form .error").html("ثبت پرسنل با خطا همراه بود.");
            });

            return false;
        }
    }
};

$(document).ready(function () {
    extraQuotaPersons.list.initial();
});