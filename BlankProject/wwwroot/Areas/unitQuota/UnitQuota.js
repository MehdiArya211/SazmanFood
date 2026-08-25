var unitQuotaSubmission = false;

var unitQuota = {
    urls: {
        getList: "/Qouta/UnitQuotas/GetList",
        loadOfficialForm: "/Qouta/UnitQuotas/LoadOfficialForm",
        searchOfficial: "/Qouta/UnitQuotas/SearchOfficial",
        createOfficial: "/Qouta/UnitQuotas/CreateOfficial",
        loadDutyForm: "/Qouta/UnitQuotas/LoadDutyForm",
        createDuty: "/Qouta/UnitQuotas/CreateDuty",
        getPersons: "/Qouta/UnitQuotas/GetPersons",
        deletePerson: "/Qouta/UnitQuotas/DeletePerson"
    },

    list: {
        table: null,

        initial: function () {
            this.table = $("#datatables").DataTable({
                language: {
                    url: "/assets/datatables/fa-lang.json"
                },
                processing: true,
                serverSide: false,
                responsive: true,
                pagingType: "full_numbers",
                order: [],
                ajax: {
                    url: unitQuota.urls.getList,
                    type: "POST",
                    data: function (data) {
                        return $.extend(
                            {},
                            data,
                            unitQuota.filter.collect()
                        );
                    },
                    error: function (xhr) {
                        unitQuota.helpers.showAjaxError(
                            xhr,
                            "دریافت سهمیه‌ها با خطا همراه بود."
                        );
                    }
                },
                columns: [
                    {
                        data: "row",
                        name: "ردیف"
                    },
                    {
                        data: "orgTitle",
                        name: "یگان",
                        defaultContent: "-"
                    },
                    {
                        data: "yeganTypeTitle",
                        name: "نوع سهمیه",
                        defaultContent: "-"
                    },
                    {
                        data: "mealTitle",
                        name: "وعده",
                        defaultContent: "-"
                    },
                    {
                        data: "dayTypeTitle",
                        name: "نوع روز",
                        defaultContent: "-"
                    },
                    {
                        data: "totalQuotaCount",
                        name: "تعداد",
                        render: function (data, type, row) {
                            var title =
                                "کادر: " +
                                row.officialRegisteredCount +
                                " از " +
                                row.officialQuotaCount +
                                " | وظیفه: " +
                                row.dutyRegisteredCount +
                                " از " +
                                row.dutyQuotaCount;

                            return '<span title="' +
                                title +
                                '" data-toggle="tooltip">' +
                                unitQuota.helpers.formatNumber(data) +
                                "</span>";
                        }
                    },
                    {
                        data: null,
                        orderable: false,
                        className: "text-left",
                        render: function (data, type, row) {
                            return unitQuota.list.operations(row);
                        }
                    }
                ],
                drawCallback: function () {
                    $('[data-toggle="tooltip"]').tooltip();
                }
            });
        },

        operations: function (row) {
            var buttons = "";

            var canManage =
                Number(row.orgId) ===
                Number(unitQuotaPermissions.currentOrgId);

            if (!canManage)
                return '<span class="text-muted">فقط مشاهده</span>';

            if (row.officialUnitQuotaId &&
                row.officialQuotaCount > 0) {
                buttons +=
                    '<a class="btn btn-simple btn-info btn-icon operation-button" ' +
                    'title="ثبت کارکنان کادر" data-toggle="tooltip" ' +
                    'onclick="unitQuota.official.loadForm(' +
                    row.officialUnitQuotaId +
                    "," +
                    row.mealId +
                    ')">' +
                    '<i class="material-icons">person_add</i>' +
                    "</a>";
            }

            if (row.dutyUnitQuotaId &&
                row.dutyQuotaCount > 0) {
                buttons +=
                    '<a class="btn btn-simple btn-warning btn-icon operation-button" ' +
                    'title="ثبت کارکنان وظیفه" data-toggle="tooltip" ' +
                    'onclick="unitQuota.duty.loadForm(' +
                    row.dutyUnitQuotaId +
                    "," +
                    row.mealId +
                    ')">' +
                    '<i class="material-icons">group_add</i>' +
                    "</a>";
            }

            return buttons ||
                '<span class="text-muted">بدون عملیات</span>';
        },

        reload: function () {
            if (this.table)
                this.table.ajax.reload(null, false);
        }
    },

    filter: {
        collect: function () {
            return {
                OrgId: $("#FilterOrgId").val()
            };
        }
    },

    form: {
        initial: function () {
            $(".selectpicker").each(function () {
                var picker = $(this);

                if (picker.data("selectpicker"))
                    picker.selectpicker("destroy");

                picker.selectpicker({
                    liveSearch: true,
                    size: 7
                });
            });

            if ($.material)
                $.material.init();
        },

        parseValidation: function (selector) {
            var form = $(selector);

            form.removeData("validator");
            form.removeData("unobtrusiveValidation");

            if ($.validator &&
                $.validator.unobtrusive) {
                $.validator.unobtrusive.parse(form);
            }
        }
    },

    official: {
        loadForm: function (unitQuotaId, mealId) {
            $.ajax({
                url: unitQuota.urls.loadOfficialForm,
                type: "GET",
                data: {
                    unitQuotaId: unitQuotaId,
                    mealId: mealId
                },
                success: function (result) {
                    $("#modal-form").html(result);

                    unitQuota.form.initial();
                    unitQuota.form.parseValidation(
                        ".official-form"
                    );

                    $("#base-modal").modal("show");

                    unitQuota.personTable.initialOfficial();

                    $("#OfficialPersonCode")
                        .off("input.unitQuota")
                        .on("input.unitQuota", function () {
                            unitQuota.official.clearPerson();
                            $("#OfficialSearchResult").html("");
                            $(".official-form .error").html("");
                        });

                    setTimeout(function () {
                        $("#OfficialPersonCode").focus();
                    }, 300);
                },
                error: function (xhr) {
                    unitQuota.helpers.showAjaxError(
                        xhr,
                        "بارگذاری فرم ثبت کادر با خطا همراه بود."
                    );
                }
            });
        },

        search: function () {
            var personCode =
                unitQuota.helpers.toEnglishDigits(
                    $("#OfficialPersonCode")
                        .val()
                        .trim()
                );

            $("#OfficialPersonCode").val(personCode);

            unitQuota.official.clearPerson();

            if (!personCode) {
                $("#OfficialSearchResult")
                    .removeClass("text-success")
                    .addClass("text-danger")
                    .html("کد پرسنلی را وارد کنید.");

                return;
            }

            $("#OfficialSearchResult")
                .removeClass("text-success text-danger")
                .html("در حال دریافت اطلاعات...");

            $.ajax({
                url: unitQuota.urls.searchOfficial,
                type: "GET",
                data: {
                    personCode: personCode
                },
                success: function (result) {
                    if (!unitQuota.helpers.isSuccess(result)) {
                        $("#OfficialSearchResult")
                            .removeClass("text-success")
                            .addClass("text-danger")
                            .html(
                                unitQuota.helpers.getMessage(
                                    result,
                                    "پرسنل یافت نشد."
                                )
                            );

                        return;
                    }

                    var person =
                        result.model ||
                        result.Model;

                    if (!person) {
                        $("#OfficialSearchResult")
                            .removeClass("text-success")
                            .addClass("text-danger")
                            .html("اطلاعات پرسنل دریافت نشد.");

                        return;
                    }

                    var personId =
                        person.personId ??
                        person.PersonId ??
                        "";

                    var nationalCode =
                        person.nationalCode ??
                        person.NationalCode ??
                        "";

                    var fullName =
                        person.fullName ??
                        person.FullName ??
                        "";

                    var rankTitle =
                        person.rankTitle ??
                        person.RankTitle ??
                        "";

                    $("#OfficialPersonId").val(personId);
                    $("#OfficialNationalCode").val(nationalCode);
                    $("#OfficialFullName").val(fullName);
                    $("#OfficialRankTitle").val(rankTitle);
                    $("#OfficialFullNameDisplay").val(fullName);
                    $("#OfficialRankTitleDisplay").val(rankTitle);

                    $("#OfficialSearchResult")
                        .removeClass("text-danger")
                        .addClass("text-success")
                        .html("اطلاعات پرسنل دریافت شد.");
                },
                error: function (xhr) {
                    $("#OfficialSearchResult")
                        .removeClass("text-success")
                        .addClass("text-danger")
                        .html(
                            unitQuota.helpers.getErrorMessage(
                                xhr,
                                "دریافت اطلاعات پرسنل با خطا همراه بود."
                            )
                        );
                }
            });
        },

        clearPerson: function () {
            $("#OfficialPersonId").val("");
            $("#OfficialNationalCode").val("");
            $("#OfficialFullName").val("");
            $("#OfficialRankTitle").val("");
            $("#OfficialFullNameDisplay").val("");
            $("#OfficialRankTitleDisplay").val("");
        },

        resetForm: function () {
            $("#OfficialPersonCode").val("");
            $("#OfficialPersonId").val("");
            $("#OfficialNationalCode").val("");
            $("#OfficialFullName").val("");
            $("#OfficialRankTitle").val("");
            $("#OfficialFullNameDisplay").val("");
            $("#OfficialRankTitleDisplay").val("");
            $("#OfficialSearchResult").html("");
            $(".official-form .error").html("");

            $("#OfficialPersonCode").focus();
        },

        save: function (event) {
            event.preventDefault();

            if (unitQuotaSubmission)
                return false;

            var form = $(".official-form");

            var unitQuotaId =
                $("#quota-person-modal-data")
                    .attr("data-unit-quota-id");

            var mealId =
                $("#quota-person-modal-data")
                    .attr("data-meal-id");

            $("#OfficialUnitQuotaId").val(unitQuotaId);
            $("#OfficialMealId").val(mealId);

            var personCode =
                unitQuota.helpers.toEnglishDigits(
                    $("#OfficialPersonCode")
                        .val()
                        .trim()
                );

            $("#OfficialPersonCode").val(personCode);
            $(".official-form .error").html("");

            if (!unitQuotaId ||
                Number(unitQuotaId) <= 0) {
                $(".official-form .error")
                    .html("شناسه سهمیه معتبر نیست.");

                return false;
            }

            if (!mealId ||
                Number(mealId) <= 0) {
                $(".official-form .error")
                    .html("شناسه وعده غذایی معتبر نیست.");

                return false;
            }

            if (!personCode) {
                $(".official-form .error")
                    .html("کد پرسنلی را وارد کنید.");

                return false;
            }

            if (!$("#OfficialDiningHallId").val()) {
                $(".official-form .error")
                    .html("سالن غذاخوری را انتخاب کنید.");

                return false;
            }

            form.validate();

            if (!form.valid())
                return false;

            unitQuotaSubmission = true;

            $.ajax({
                url: unitQuota.urls.createOfficial,
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    unitQuotaSubmission = false;

                    if (unitQuota.helpers.isSuccess(result)) {
                        unitQuota.helpers.showModalSuccess(
                            "پرسنل کادر با موفقیت ثبت شد."
                        );

                        unitQuota.official.resetForm();
                        unitQuota.personTable.reloadCurrent();
                        unitQuota.list.reload();
                    } else {
                        $(".official-form .error").html(
                            unitQuota.helpers.getMessage(
                                result,
                                "ثبت پرسنل کادر با خطا همراه بود."
                            )
                        );
                    }
                },
                error: function (xhr) {
                    unitQuotaSubmission = false;

                    unitQuota.helpers.showModalError(
                        unitQuota.helpers.getErrorMessage(
                            xhr,
                            "ثبت پرسنل کادر با خطا همراه بود."
                        )
                    );
                }
            });

            return false;
        }
    },

    duty: {
        loadForm: function (unitQuotaId, mealId) {
            $.ajax({
                url: unitQuota.urls.loadDutyForm,
                type: "GET",
                data: {
                    unitQuotaId: unitQuotaId,
                    mealId: mealId
                },
                success: function (result) {
                    $("#modal-form").html(result);

                    unitQuota.form.initial();
                    unitQuota.form.parseValidation(
                        ".duty-form"
                    );

                    $("#base-modal").modal("show");

                    unitQuota.personTable.initialDuty();

                    setTimeout(function () {
                        $("#DutyNationalCode").focus();
                    }, 300);
                },
                error: function (xhr) {
                    unitQuota.helpers.showAjaxError(
                        xhr,
                        "بارگذاری فرم ثبت وظیفه با خطا همراه بود."
                    );
                }
            });
        },

        resetForm: function () {
            $("#DutyNationalCode").val("");
            $("#DutyPersonCode").val("");
            $("#DutyFullName").val("");
            $(".duty-form .error").html("");

            $("#DutyNationalCode").focus();
        },

        save: function (event) {
            event.preventDefault();

            if (unitQuotaSubmission)
                return false;

            var form = $(".duty-form");

            var unitQuotaId =
                $("#quota-person-modal-data")
                    .attr("data-unit-quota-id");

            var mealId =
                $("#quota-person-modal-data")
                    .attr("data-meal-id");

            $("#DutyUnitQuotaId").val(unitQuotaId);
            $("#DutyMealId").val(mealId);

            var nationalCode =
                unitQuota.helpers.toEnglishDigits(
                    $("#DutyNationalCode")
                        .val()
                        .trim()
                );

            var personCode =
                unitQuota.helpers.toEnglishDigits(
                    $("#DutyPersonCode")
                        .val()
                        .trim()
                );

            var fullName =
                $("#DutyFullName")
                    .val()
                    .trim();

            $("#DutyNationalCode").val(nationalCode);
            $("#DutyPersonCode").val(personCode);
            $(".duty-form .error").html("");

            if (!unitQuotaId ||
                Number(unitQuotaId) <= 0) {
                $(".duty-form .error")
                    .html("شناسه سهمیه معتبر نیست.");

                return false;
            }

            if (!mealId ||
                Number(mealId) <= 0) {
                $(".duty-form .error")
                    .html("شناسه وعده غذایی معتبر نیست.");

                return false;
            }

            if (!nationalCode ||
                nationalCode.length !== 10 ||
                !/^\d{10}$/.test(nationalCode)) {
                $(".duty-form .error")
                    .html("کد ملی باید 10 رقم باشد.");

                return false;
            }

            if (!personCode) {
                $(".duty-form .error")
                    .html("کد پرسنلی را وارد کنید.");

                return false;
            }

            if (!fullName) {
                $(".duty-form .error")
                    .html("نام و نشان را وارد کنید.");

                return false;
            }

            if (!$("#DutyDiningHallId").val()) {
                $(".duty-form .error")
                    .html("سالن غذاخوری را انتخاب کنید.");

                return false;
            }

            form.validate();

            if (!form.valid())
                return false;

            unitQuotaSubmission = true;

            $.ajax({
                url: unitQuota.urls.createDuty,
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    unitQuotaSubmission = false;

                    if (unitQuota.helpers.isSuccess(result)) {
                        unitQuota.helpers.showModalSuccess(
                            "پرسنل وظیفه با موفقیت ثبت شد."
                        );

                        unitQuota.duty.resetForm();
                        unitQuota.personTable.reloadCurrent();
                        unitQuota.list.reload();
                    } else {
                        $(".duty-form .error").html(
                            unitQuota.helpers.getMessage(
                                result,
                                "ثبت پرسنل وظیفه با خطا همراه بود."
                            )
                        );
                    }
                },
                error: function (xhr) {
                    unitQuotaSubmission = false;

                    unitQuota.helpers.showModalError(
                        unitQuota.helpers.getErrorMessage(
                            xhr,
                            "ثبت پرسنل وظیفه با خطا همراه بود."
                        )
                    );
                }
            });

            return false;
        }
    },

    personTable: {
        officialTable: null,
        dutyTable: null,

        initialOfficial: function () {
            var selector =
                "#official-person-datatable";

            var element = $(selector);

            if (!element.length)
                return;

            var modalData =
                $("#quota-person-modal-data");

            var unitQuotaId =
                modalData.attr("data-unit-quota-id");

            var mealId =
                modalData.attr("data-meal-id");

            if ($.fn.DataTable.isDataTable(selector))
                element.DataTable().destroy();

            this.officialTable =
                element.DataTable({
                    language: {
                        url: "/assets/datatables/fa-lang.json"
                    },
                    processing: true,
                    serverSide: false,
                    responsive: true,
                    pagingType: "full_numbers",
                    pageLength: 10,
                    order: [],
                    ajax: {
                        url: unitQuota.urls.getPersons,
                        type: "GET",
                        data: {
                            unitQuotaId: unitQuotaId,
                            mealId: mealId
                        },
                        dataSrc: function (result) {
                            return result.model ||
                                result.Model ||
                                [];
                        },
                        error: function (xhr) {
                            unitQuota.helpers.showModalError(
                                unitQuota.helpers.getErrorMessage(
                                    xhr,
                                    "دریافت کارکنان کادر با خطا همراه بود."
                                )
                            );
                        }
                    },
                    columns: [
                        {
                            data: null,
                            orderable: false,
                            render: function (
                                data,
                                type,
                                row,
                                meta) {
                                return meta.row +
                                    meta.settings._iDisplayStart +
                                    1;
                            }
                        },
                        {
                            data: "personCode",
                            defaultContent: "-"
                        },
                        {
                            data: "rankTitle",
                            defaultContent: "-"
                        },
                        {
                            data: "fullName",
                            defaultContent: "-"
                        },
                        {
                            data: "diningHallTitle",
                            defaultContent: "-"
                        },
                        {
                            data: null,
                            orderable: false,
                            className: "text-left",
                            render: function (data, type, row) {
                                return unitQuota.person
                                    .deleteButton(row.id);
                            }
                        }
                    ],
                    drawCallback: function () {
                        var api = this.api();

                        unitQuota.summary.update(
                            api.rows().count()
                        );

                        $('[data-toggle="tooltip"]')
                            .tooltip();
                    }
                });
        },

        initialDuty: function () {
            var selector =
                "#duty-person-datatable";

            var element = $(selector);

            if (!element.length)
                return;

            var modalData =
                $("#quota-person-modal-data");

            var unitQuotaId =
                modalData.attr("data-unit-quota-id");

            var mealId =
                modalData.attr("data-meal-id");

            if ($.fn.DataTable.isDataTable(selector))
                element.DataTable().destroy();

            this.dutyTable =
                element.DataTable({
                    language: {
                        url: "/assets/datatables/fa-lang.json"
                    },
                    processing: true,
                    serverSide: false,
                    responsive: true,
                    pagingType: "full_numbers",
                    pageLength: 10,
                    order: [],
                    ajax: {
                        url: unitQuota.urls.getPersons,
                        type: "GET",
                        data: {
                            unitQuotaId: unitQuotaId,
                            mealId: mealId
                        },
                        dataSrc: function (result) {
                            return result.model ||
                                result.Model ||
                                [];
                        },
                        error: function (xhr) {
                            unitQuota.helpers.showModalError(
                                unitQuota.helpers.getErrorMessage(
                                    xhr,
                                    "دریافت کارکنان وظیفه با خطا همراه بود."
                                )
                            );
                        }
                    },
                    columns: [
                        {
                            data: null,
                            orderable: false,
                            render: function (
                                data,
                                type,
                                row,
                                meta) {
                                return meta.row +
                                    meta.settings._iDisplayStart +
                                    1;
                            }
                        },
                        {
                            data: "nationalCode",
                            defaultContent: "-"
                        },
                        {
                            data: "personCode",
                            defaultContent: "-"
                        },
                        {
                            data: "fullName",
                            defaultContent: "-"
                        },
                        {
                            data: "diningHallTitle",
                            defaultContent: "-"
                        },
                        {
                            data: null,
                            orderable: false,
                            className: "text-left",
                            render: function (data, type, row) {
                                return unitQuota.person
                                    .deleteButton(row.id);
                            }
                        }
                    ],
                    drawCallback: function () {
                        var api = this.api();

                        unitQuota.summary.update(
                            api.rows().count()
                        );

                        $('[data-toggle="tooltip"]')
                            .tooltip();
                    }
                });
        },

        reloadCurrent: function () {
            var type =
                $("#quota-person-modal-data")
                    .attr("data-type");

            if (type === "official" &&
                this.officialTable) {
                this.officialTable.ajax.reload(
                    null,
                    false);
            }

            if (type === "duty" &&
                this.dutyTable) {
                this.dutyTable.ajax.reload(
                    null,
                    false);
            }
        }
    },

    summary: {
        update: function (registeredCount) {
            var quotaCount =
                parseInt(
                    $("#QuotaCountDisplay")
                        .attr("data-quota-count") ||
                    0,
                    10);

            var remainingCount =
                Math.max(
                    0,
                    quotaCount - registeredCount);

            $("#RegisteredCountDisplay")
                .text(registeredCount);

            $("#RemainingCountDisplay")
                .text(remainingCount)
                .removeClass(
                    "quota-capacity-success quota-capacity-danger"
                )
                .addClass(
                    remainingCount > 0
                        ? "quota-capacity-success"
                        : "quota-capacity-danger"
                );

            $("#OfficialSaveButton,#DutySaveButton")
                .prop(
                    "disabled",
                    remainingCount <= 0);
        }
    },

    person: {
        deleteButton: function (id) {
            return '<a class="btn btn-simple btn-danger btn-icon" ' +
                'title="حذف" data-toggle="tooltip" ' +
                'onclick="unitQuota.person.remove(' +
                id +
                ')">' +
                '<i class="material-icons">close</i>' +
                "</a>";
        },

        remove: function (id) {
            swal({
                title: "حذف فرد",
                text: "آیا از حذف این فرد از سهمیه مطمئن هستید؟",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-danger",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm)
                    unitQuota.person.confirmRemove(id);
            });
        },

        confirmRemove: function (id) {
            $.ajax({
                url: unitQuota.urls.deletePerson,
                type: "POST",
                data: {
                    id: id,
                    __RequestVerificationToken:
                        unitQuota.helpers.getToken()
                },
                success: function (result) {
                    if (unitQuota.helpers.isSuccess(result)) {
                        unitQuota.helpers.showModalSuccess(
                            "فرد با موفقیت از سهمیه حذف شد."
                        );

                        unitQuota.personTable.reloadCurrent();
                        unitQuota.list.reload();
                    } else {
                        unitQuota.helpers.showModalError(
                            unitQuota.helpers.getMessage(
                                result,
                                "حذف فرد با خطا همراه بود."
                            )
                        );
                    }
                },
                error: function (xhr) {
                    unitQuota.helpers.showModalError(
                        unitQuota.helpers.getErrorMessage(
                            xhr,
                            "حذف فرد با خطا همراه بود."
                        )
                    );
                }
            });
        }
    },

    helpers: {
        isSuccess: function (result) {
            return result &&
                (
                    result.status === true ||
                    result.Status === true
                );
        },

        getMessage: function (result, defaultMessage) {
            if (!result)
                return defaultMessage;

            return result.message ||
                result.Message ||
                defaultMessage;
        },

        getErrorMessage: function (xhr, defaultMessage) {
            if (xhr && xhr.responseJSON) {
                return xhr.responseJSON.message ||
                    xhr.responseJSON.Message ||
                    defaultMessage;
            }

            if (xhr && xhr.status === 403)
                return "شما مجوز انجام این عملیات را ندارید.";

            if (xhr && xhr.status === 404)
                return "اطلاعات مورد نظر یافت نشد.";

            return defaultMessage;
        },

        showAjaxError: function (xhr, defaultMessage) {
            unitQuota.helpers.error(
                "عملیات انجام نشد!",
                unitQuota.helpers.getErrorMessage(
                    xhr,
                    defaultMessage
                )
            );
        },

        showModalSuccess: function (message) {
            var box =
                $("#QuotaModalNotification");

            box.stop(true, true)
                .removeClass(
                    "alert-danger alert-warning"
                )
                .addClass(
                    "alert alert-success"
                )
                .html(
                    '<i class="material-icons">check_circle</i> ' +
                    message
                )
                .fadeIn(200);

            setTimeout(function () {
                box.fadeOut(400);
            }, 3000);
        },

        showModalError: function (message) {
            var box =
                $("#QuotaModalNotification");

            box.stop(true, true)
                .removeClass(
                    "alert-success alert-warning"
                )
                .addClass(
                    "alert alert-danger"
                )
                .html(
                    '<i class="material-icons">error</i> ' +
                    message
                )
                .fadeIn(200);

            setTimeout(function () {
                box.fadeOut(400);
            }, 3000);
        },

        error: function (title, message) {
            swal({
                title: title,
                text: message,
                type: "error",
                confirmButtonClass: "btn btn-danger",
                confirmButtonText: "باشه",
                buttonsStyling: false
            });
        },

        getToken: function () {
            return $("#operation-token-form")
                .find(
                    "input[name='__RequestVerificationToken']"
                )
                .val();
        },

        toEnglishDigits: function (value) {
            if (value === null ||
                value === undefined) {
                return "";
            }

            return value
                .toString()
                .replace(/[۰-۹]/g, function (digit) {
                    return "۰۱۲۳۴۵۶۷۸۹"
                        .indexOf(digit);
                })
                .replace(/[٠-٩]/g, function (digit) {
                    return "٠١٢٣٤٥٦٧٨٩"
                        .indexOf(digit);
                });
        },

        formatNumber: function (value) {
            var number =
                parseInt(value || 0, 10);

            if (isNaN(number))
                number = 0;

            return number.toLocaleString("en-US");
        }
    }
};

$(document).ready(function () {
    $(".selectpicker").selectpicker("refresh");
    unitQuota.list.initial();
});