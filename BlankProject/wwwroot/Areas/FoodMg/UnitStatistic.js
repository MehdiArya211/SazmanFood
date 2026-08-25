var startSubmition = false;

var unitStatistic = {
    urls: {
        getList: "/FoodMang/UnitStatistics/GetList",
        loadCreateForm: "/FoodMang/UnitStatistics/LoadCreateForm",
        create: "/FoodMang/UnitStatistics/Create",
        loadEditForm: "/FoodMang/UnitStatistics/LoadEditForm/",
        edit: "/FoodMang/UnitStatistics/Edit",
        loadDetailsForm: "/FoodMang/UnitStatistics/LoadDetailsForm/",
        saveDetails: "/FoodMang/UnitStatistics/SaveDetails",
        send: "/FoodMang/UnitStatistics/Send",
        approve: "/FoodMang/UnitStatistics/Approve"
    },

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
                lengthMenu: [
                    [10, 25, 50, -1],
                    [10, 25, 50, "همه"]
                ],
                responsive: true,
                serverSide: true,
                processing: true,
                order: [[0, "desc"]],
                ajax: {
                    url: unitStatistic.urls.getList,
                    type: "POST",
                    dataType: "json",
                    data: function (data) {
                        return $.extend(
                            {},
                            data,
                            unitStatistic.filter.collect()
                        );
                    },
                    error: function (xhr) {
                        unitStatistic.helpers.showAjaxError(
                            xhr,
                            "دریافت لیست آمار با خطا همراه بود."
                        );
                    }
                },
                columns: [
                    {
                        data: "id",
                        name: "شناسه"
                    },
                    {
                        data: "orgTitle",
                        name: "یگان"
                    },
                    {
                        data: "createDateFa",
                        name: "تاریخ ثبت"
                    },
                    {
                        data: "creatorFullName",
                        name: "ثبت‌کننده"
                    },
                    {
                        data: "totalOfficialCount",
                        name: "تعداد کل کادر",
                        render: function (data) {
                            return unitStatistic.helpers.formatNumber(data);
                        }
                    },
                    {
                        data: "totalDutyCount",
                        name: "تعداد کل وظیفه",
                        render: function (data) {
                            return unitStatistic.helpers.formatNumber(data);
                        }
                    },
                    {
                        data: "status",
                        name: "وضعیت",
                        render: function (data) {
                            return unitStatistic.helpers.statusBadge(
                                Number(data)
                            );
                        }
                    },
                    {
                        data: "isActive",
                        name: "فعال/غیرفعال",
                        render: function (data) {
                            if (data === true) {
                                return '<span class="text-success">فعال</span>';
                            }

                            return '<span class="text-danger">غیرفعال</span>';
                        }
                    },
                    {
                        data: null,
                        orderable: false,
                        className: "text-left",
                        render: function (data, type, row) {
                            return unitStatistic.list.operations(row);
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: [1, 8],
                        orderable: false
                    }
                ]
            });
        },

        operations: function (row) {
            var id = row.id;
            var status = Number(row.status);
            var buttons = "";

            buttons +=
                '<a onclick="unitStatistic.details.loadForm(' + id + ')" ' +
                'class="btn btn-simple btn-primary btn-icon operation-button" ' +
                'title="جزئیات" data-toggle="tooltip">' +
                '<i class="material-icons">list</i></a>';

            if (unitStatisticPermissions.canRegister === true &&
                status !== 3) {
                buttons +=
                    '<a onclick="unitStatistic.edit.loadForm(' + id + ')" ' +
                    'class="btn btn-simple btn-info btn-icon operation-button" ' +
                    'title="ویرایش" data-toggle="tooltip">' +
                    '<i class="material-icons">edit</i></a>';
            }

            if (unitStatisticPermissions.canRegister === true &&
                status === 1) {
                buttons +=
                    '<a onclick="unitStatistic.send.confirm(' + id + ')" ' +
                    'class="btn btn-simple btn-warning btn-icon operation-button" ' +
                    'title="ارسال برای تأیید" data-toggle="tooltip">' +
                    '<i class="material-icons">send</i></a>';
            }

            if (unitStatisticPermissions.canApprove === true &&
                status === 2) {
                buttons +=
                    '<a onclick="unitStatistic.approve.confirm(' + id + ')" ' +
                    'class="btn btn-simple btn-success btn-icon operation-button" ' +
                    'title="تأیید نهایی" data-toggle="tooltip">' +
                    '<i class="material-icons">check_circle</i></a>';
            }

            return buttons;
        },

        reload: function () {
            if (this.table) {
                this.table.ajax.reload(null, false);
            }
        }
    },

    form: {
        initial: function () {
            $(".selectpicker").selectpicker("refresh");

            if ($.material) {
                $.material.init();
            }
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

    create: {
        loadForm: function () {
            $.get(unitStatistic.urls.loadCreateForm)
                .done(function (result) {
                    $("#modal-form").html(result);

                    unitStatistic.form.initial();
                    unitStatistic.form.parseValidation(
                        ".create-form"
                    );

                    $("#base-modal").modal("show");
                })
                .fail(function (xhr) {
                    unitStatistic.helpers.showAjaxError(
                        xhr,
                        "بارگذاری فرم ثبت آمار با خطا همراه بود."
                    );
                });
        },

        save: function (event) {
            event.preventDefault();

            if (startSubmition)
                return false;

            var form = $(".create-form");

            form.validate();

            if (!form.valid())
                return false;

            startSubmition = true;

            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    startSubmition = false;

                    if (unitStatistic.helpers.isSuccess(result)) {
                        $("#base-modal").modal("hide");
                        unitStatistic.list.reload();

                        unitStatistic.helpers.success(
                            "ذخیره شد!",
                            "آمار یگان با موفقیت ثبت شد."
                        );
                    } else {
                        $(".create-form .error").html(
                            unitStatistic.helpers.getMessage(
                                result,
                                "ثبت آمار با خطا همراه بود."
                            )
                        );
                    }
                },
                error: function (xhr) {
                    startSubmition = false;

                    unitStatistic.helpers.showAjaxError(
                        xhr,
                        "ثبت آمار با خطا همراه بود."
                    );
                }
            });

            return false;
        }
    },

    edit: {
        loadForm: function (id) {
            if (!id)
                return;

            $.get(
                unitStatistic.urls.loadEditForm + id
            )
                .done(function (result) {
                    $("#modal-form").html(result);

                    unitStatistic.form.initial();
                    unitStatistic.form.parseValidation(
                        ".edit-form"
                    );

                    $("#base-modal").modal("show");
                })
                .fail(function (xhr) {
                    unitStatistic.helpers.showAjaxError(
                        xhr,
                        "بارگذاری فرم ویرایش با خطا همراه بود."
                    );
                });
        },

        save: function (event) {
            event.preventDefault();

            if (startSubmition)
                return false;

            var form = $(".edit-form");

            form.validate();

            if (!form.valid())
                return false;

            startSubmition = true;

            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    startSubmition = false;

                    if (unitStatistic.helpers.isSuccess(result)) {
                        $("#base-modal").modal("hide");
                        unitStatistic.list.reload();

                        unitStatistic.helpers.success(
                            "ویرایش شد!",
                            "آمار یگان با موفقیت ویرایش شد."
                        );
                    } else {
                        $(".edit-form .error").html(
                            unitStatistic.helpers.getMessage(
                                result,
                                "ویرایش آمار با خطا همراه بود."
                            )
                        );
                    }
                },
                error: function (xhr) {
                    startSubmition = false;

                    unitStatistic.helpers.showAjaxError(
                        xhr,
                        "ویرایش آمار با خطا همراه بود."
                    );
                }
            });

            return false;
        }
    },

    details: {
        loadForm: function (id) {
            if (!id)
                return;

            $.get(
                unitStatistic.urls.loadDetailsForm + id
            )
                .done(function (result) {
                    $("#details-modal-form").html(result);

                    unitStatistic.form.initial();
                    unitStatistic.form.parseValidation(
                        ".details-form"
                    );

                    unitStatistic.details.bind();
                    unitStatistic.details.calculate();

                    $("#details-modal").modal("show");
                })
                .fail(function (xhr) {
                    unitStatistic.helpers.showAjaxError(
                        xhr,
                        "بارگذاری جزئیات آمار با خطا همراه بود."
                    );
                });
        },

        bind: function () {
            $(".official-count,.duty-count")
                .off("input.unitStatistic")
                .on("input.unitStatistic", function () {
                    var value =
                        unitStatistic.helpers.toInteger(
                            $(this).val()
                        );

                    if (value < 0) {
                        $(this).val(0);
                    }

                    unitStatistic.details.calculate();
                });
        },

        calculate: function () {
            var officialSum = 0;
            var dutySum = 0;

            $(".official-count").each(function () {
                officialSum +=
                    unitStatistic.helpers.toInteger(
                        $(this).val()
                    );
            });

            $(".duty-count").each(function () {
                dutySum +=
                    unitStatistic.helpers.toInteger(
                        $(this).val()
                    );
            });

            var officialExpected =
                unitStatistic.helpers.toInteger(
                    $("#OfficialExpected").text()
                );

            var dutyExpected =
                unitStatistic.helpers.toInteger(
                    $("#DutyExpected").text()
                );

            unitStatistic.details.showTotal(
                "#OfficialTotalText",
                officialSum,
                officialExpected,
                "کادر"
            );

            unitStatistic.details.showTotal(
                "#DutyTotalText",
                dutySum,
                dutyExpected,
                "وظیفه"
            );

            return officialSum === officialExpected &&
                dutySum === dutyExpected;
        },

        showTotal: function (
            selector,
            current,
            expected,
            title
        ) {
            var element = $(selector);

            element
                .removeClass(
                    "details-valid details-invalid"
                )
                .addClass(
                    current === expected
                        ? "details-valid"
                        : "details-invalid"
                );

            element.text(
                "جمع " + title + ": " +
                unitStatistic.helpers.formatNumber(current) +
                " از " +
                unitStatistic.helpers.formatNumber(expected)
            );
        },

        save: function (event) {
            event.preventDefault();

            if (startSubmition)
                return false;

            var form = $(".details-form");

            form.validate();

            if (!form.valid())
                return false;

            if (!unitStatistic.details.calculate()) {
                unitStatistic.helpers.error(
                    "عدم همخوانی تعداد",
                    "جمع جزئیات کادر و وظیفه باید با تعداد کل آنها برابر باشد."
                );

                return false;
            }

            startSubmition = true;

            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    startSubmition = false;

                    if (unitStatistic.helpers.isSuccess(result)) {
                        $("#details-modal").modal("hide");
                        unitStatistic.list.reload();

                        unitStatistic.helpers.success(
                            "ذخیره شد!",
                            "جزئیات آمار با موفقیت ذخیره شد."
                        );
                    } else {
                        $(".details-form .error").html(
                            unitStatistic.helpers.getMessage(
                                result,
                                "ذخیره جزئیات با خطا همراه بود."
                            )
                        );
                    }
                },
                error: function (xhr) {
                    startSubmition = false;

                    unitStatistic.helpers.showAjaxError(
                        xhr,
                        "ذخیره جزئیات با خطا همراه بود."
                    );
                }
            });

            return false;
        }
    },

    send: {
        confirm: function (id) {
            swal({
                title: "ارسال آمار",
                text: "آیا از ارسال آمار برای تأیید مطمئن هستید؟",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-warning",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "بله، ارسال شود",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm) {
                    unitStatistic.send.execute(id);
                }
            });
        },

        execute: function (id) {
            $.ajax({
                url: unitStatistic.urls.send,
                type: "POST",
                data: {
                    id: id,
                    __RequestVerificationToken:
                        unitStatistic.helpers.getToken()
                },
                success: function (result) {
                    if (unitStatistic.helpers.isSuccess(result)) {
                        unitStatistic.list.reload();

                        unitStatistic.helpers.success(
                            "ارسال شد!",
                            "آمار با موفقیت برای تأیید ارسال شد."
                        );
                    } else {
                        unitStatistic.helpers.error(
                            "ارسال انجام نشد!",
                            unitStatistic.helpers.getMessage(
                                result,
                                "ارسال آمار با خطا همراه بود."
                            )
                        );
                    }
                },
                error: function (xhr) {
                    unitStatistic.helpers.showAjaxError(
                        xhr,
                        "ارسال آمار با خطا همراه بود."
                    );
                }
            });
        }
    },

    approve: {
        confirm: function (id) {
            swal({
                title: "تأیید نهایی آمار",
                text: "بعد از تأیید نهایی، آمار قابل ویرایش نیست و سهمیه‌های یگان محاسبه می‌شوند.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-success",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "بله، تأیید شود",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm) {
                    unitStatistic.approve.execute(id);
                }
            });
        },

        execute: function (id) {
            $.ajax({
                url: unitStatistic.urls.approve,
                type: "POST",
                data: {
                    id: id,
                    __RequestVerificationToken:
                        unitStatistic.helpers.getToken()
                },
                success: function (result) {
                    if (unitStatistic.helpers.isSuccess(result)) {
                        $("#details-modal").modal("hide");
                        unitStatistic.list.reload();

                        unitStatistic.helpers.success(
                            "تأیید شد!",
                            "آمار تأیید نهایی و سهمیه‌های یگان محاسبه شدند."
                        );
                    } else {
                        unitStatistic.helpers.error(
                            "عملیات انجام نشد!",
                            unitStatistic.helpers.getMessage(
                                result,
                                "تأیید آمار با خطا همراه بود."
                            )
                        );
                    }
                },
                error: function (xhr) {
                    unitStatistic.helpers.showAjaxError(
                        xhr,
                        "تأیید آمار با خطا همراه بود."
                    );
                }
            });
        }
    },

    filter: {
        collect: function () {
            return {
                Status: $("#FilterStatus").val(),
                IsActive: $("#FilterIsActive").val()
            };
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

        getMessage: function (
            result,
            defaultMessage
        ) {
            if (!result)
                return defaultMessage;

            return result.message ||
                result.Message ||
                defaultMessage;
        },

        getErrorMessage: function (
            xhr,
            defaultMessage
        ) {
            if (xhr && xhr.responseJSON) {
                return xhr.responseJSON.message ||
                    xhr.responseJSON.Message ||
                    defaultMessage;
            }

            if (xhr && xhr.status === 403) {
                return "شما مجوز انجام این عملیات را ندارید.";
            }

            if (xhr && xhr.status === 404) {
                return "اطلاعات مورد نظر یافت نشد.";
            }

            return defaultMessage;
        },

        showAjaxError: function (
            xhr,
            defaultMessage
        ) {
            unitStatistic.helpers.error(
                "عملیات انجام نشد!",
                unitStatistic.helpers.getErrorMessage(
                    xhr,
                    defaultMessage
                )
            );
        },

        success: function (
            title,
            message
        ) {
            swal({
                title: title,
                text: message,
                type: "success",
                confirmButtonClass: "btn btn-success",
                confirmButtonText: "باشه",
                buttonsStyling: false
            });
        },

        error: function (
            title,
            message
        ) {
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

        toInteger: function (value) {
            if (value === null ||
                value === undefined) {
                return 0;
            }

            var text = value
                .toString()
                .replace(/,/g, "")
                .replace(/[۰-۹]/g, function (digit) {
                    return "۰۱۲۳۴۵۶۷۸۹".indexOf(digit);
                })
                .replace(/[٠-٩]/g, function (digit) {
                    return "٠١٢٣٤٥٦٧٨٩".indexOf(digit);
                });

            var number = parseInt(text, 10);

            return isNaN(number) ? 0 : number;
        },

        formatNumber: function (value) {
            return unitStatistic.helpers
                .toInteger(value)
                .toLocaleString("en-US");
        },

        statusBadge: function (status) {
            if (status === 1) {
                return '<span class="label label-default">ثبت اولیه</span>';
            }

            if (status === 2) {
                return '<span class="label label-warning">ارسال شده</span>';
            }

            if (status === 3) {
                return '<span class="label label-success">تأیید نهایی</span>';
            }

            return '<span class="label label-default">نامشخص</span>';
        }
    }
};

$(document).ready(function () {
    $(".selectpicker").selectpicker("refresh");
    unitStatistic.list.initial();
});