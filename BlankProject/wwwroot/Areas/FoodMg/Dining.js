var startSubmition = false;

var diningHall = {
    urls: {
        getList:
            "/FoodMang/DiningHalls/GetList",

        loadCreateForm:
            "/FoodMang/DiningHalls/LoadCreateForm",

        create:
            "/FoodMang/DiningHalls/Create",

        loadEditForm:
            "/FoodMang/DiningHalls/LoadEditForm/",

        edit:
            "/FoodMang/DiningHalls/Edit",

        delete:
            "/FoodMang/DiningHalls/Delete"
    },

    list: {
        table: null,

        initial: function () {
            this.table =
                $("#datatables").DataTable({
                    drawCallback: function () {
                        $('[data-toggle="tooltip"]')
                            .tooltip();
                    },

                    language: {
                        url:
                            "/assets/datatables/fa-lang.json"
                    },

                    pagingType:
                        "full_numbers",

                    lengthMenu: [
                        [10, 25, 50, -1],
                        [10, 25, 50, "همه"]
                    ],

                    responsive: true,
                    serverSide: true,
                    processing: true,
                    searching: false,
                    order: [[0, "desc"]],

                    ajax: {
                        url:
                            diningHall.urls.getList,

                        type:
                            "POST",

                        dataType:
                            "json",

                        data: function (data) {
                            return $.extend(
                                {},
                                data,
                                diningHall.filter.collect()
                            );
                        }
                    },

                    columns: [
                        {
                            data: "id",
                            name: "Id"
                        },
                        {
                            data: "title",
                            name: "Title"
                        },
                        {
                            data: "orgTitle",
                            name: "OrgTitle"
                        },
                        {
                            data: "managerName",
                            name: "ManagerName"
                        },
                        {
                            data: "phoneNumber",
                            name: "PhoneNumber"
                        },
                        {
                            data: "personalTypeTitle",
                            name: "PersonalTypeTitle"
                        },
                        {
                            data: "usageTypeTitle",
                            name: "UsageType"
                        },
                        {
                            data: "isInternal",
                            name: "IsInternal",

                            render: function (
                                data,
                                type,
                                row
                            ) {
                                if (
                                    row.isInternal === true
                                ) {
                                    return `
                                        <span class="text-info">
                                            داخلی یگان
                                        </span>`;
                                }

                                return `
                                    <span class="text-muted">
                                        عادی
                                    </span>`;
                            }
                        },
                        {
                            data: "hallCapacityStr",
                            name: "HallCapacity"
                        },
                        {
                            data: "isActive",
                            name: "IsActive",

                            render: function (
                                data,
                                type,
                                row
                            ) {
                                if (
                                    row.isActive === true
                                ) {
                                    return `
                                        <span class="text-success">
                                            فعال
                                        </span>`;
                                }

                                return `
                                    <span class="text-danger">
                                        غیرفعال
                                    </span>`;
                            }
                        },
                        {
                            data: null,
                            className: "text-left",
                            orderable: false,

                            render: function (
                                data,
                                type,
                                row
                            ) {
                                return `
                                    <a onclick="diningHall.edit.loadForm(${row.id})"
                                       class="btn btn-simple btn-info btn-icon"
                                       title="ویرایش"
                                       data-toggle="tooltip">

                                        <i class="material-icons">
                                            edit
                                        </i>
                                    </a>

                                    <a onclick="diningHall.delete.loadForm(${row.id})"
                                       class="btn btn-simple btn-danger btn-icon"
                                       title="حذف"
                                       data-toggle="tooltip">

                                        <i class="material-icons">
                                            close
                                        </i>
                                    </a>`;
                            }
                        }
                    ],

                    columnDefs: [
                        {
                            targets: [
                                2,
                                5,
                                6,
                                7,
                                9,
                                10
                            ],

                            orderable: false
                        }
                    ]
                });
        },

        reload: function () {
            if (diningHall.list.table) {
                diningHall.list.table.ajax.reload(
                    function () {
                    },
                    false
                );
            }
        }
    },

    form: {
        initial: function () {
            $("#base-modal")
                .removeClass("small-modal");

            $(".selectpicker")
                .selectpicker("refresh");

            if ($.material) {
                $.material.init();
            }

            diningHall.helpers
                .initCapacityMask();

            diningHall.helpers
                .initPhoneNumber();
        },

        rebindValidation: function ($form) {
            $form
                .removeData("validator")
                .removeData(
                    "unobtrusiveValidation"
                );

            $.validator
                .unobtrusive
                .parse($form);
        }
    },

    create: {
        loadForm: function () {
            $.get(
                diningHall.urls.loadCreateForm,
                function (response) {
                    $("#modal-form")
                        .html(response);

                    diningHall.form.initial();

                    modal.open();

                    diningHall.form
                        .rebindValidation(
                            $(".create-form")
                        );
                }
            ).fail(function () {
                showNotification(
                    "بارگذاری فرم با خطا همراه بود.",
                    "danger"
                );
            });
        },

        save: function (event) {
            event.preventDefault();

            if (startSubmition === true) {
                return false;
            }

            diningHall.helpers
                .setCapacityValue(
                    $(".create-form")
                );

            var $form =
                $(".create-form");

            $form.validate();

            if (!$form.valid()) {
                return false;
            }

            startSubmition = true;

            var targetUrl =
                $form.attr("action");

            var data =
                $form.serialize();

            $.post(
                targetUrl,
                data,
                function (response) {
                    startSubmition = false;

                    var status =
                        response.status === true ||
                        response.Status === true;

                    var message =
                        response.message ||
                        response.Message;

                    if (status) {
                        diningHall.list.reload();

                        modal.close();

                        swal({
                            title: "ذخیره شد!",
                            text:
                                "سالن غذاخوری با موفقیت ذخیره شد.",
                            type: "success",
                            confirmButtonClass:
                                "btn btn-success",
                            confirmButtonText:
                                "باشه",
                            buttonsStyling:
                                false
                        });
                    }
                    else {
                        $(".create-form .error")
                            .html(
                                message ||
                                "ثبت اطلاعات انجام نشد."
                            );

                        setScrollPosition();
                    }
                }
            ).fail(function () {
                startSubmition = false;

                $(".create-form .error")
                    .html(
                        "ثبت اطلاعات با خطا همراه بوده است. مجدداً اقدام کنید."
                    );

                setScrollPosition();
            });

            return false;
        }
    },

    edit: {
        loadForm: function (id) {
            if (!id) {
                showNotification(
                    "اطلاعات مورد نظر را انتخاب کنید.",
                    "danger"
                );

                return;
            }

            $.get(
                diningHall.urls.loadEditForm + id,
                function (response) {
                    $("#modal-form")
                        .html(response);

                    diningHall.form.initial();

                    modal.open();

                    diningHall.form
                        .rebindValidation(
                            $(".edit-form")
                        );
                }
            ).fail(function () {
                showNotification(
                    "بارگذاری فرم ویرایش با خطا همراه بود.",
                    "danger"
                );
            });
        },

        save: function (event) {
            event.preventDefault();

            if (startSubmition === true) {
                return false;
            }

            diningHall.helpers
                .setCapacityValue(
                    $(".edit-form")
                );

            var $form =
                $(".edit-form");

            $form.validate();

            if (!$form.valid()) {
                return false;
            }

            startSubmition = true;

            var targetUrl =
                $form.attr("action");

            var data =
                $form.serialize();

            $.post(
                targetUrl,
                data,
                function (response) {
                    startSubmition = false;

                    var status =
                        response.status === true ||
                        response.Status === true;

                    var message =
                        response.message ||
                        response.Message;

                    if (status) {
                        diningHall.list.reload();

                        modal.close();

                        swal({
                            title: "ویرایش شد!",
                            text:
                                "سالن غذاخوری با موفقیت ویرایش شد.",
                            type: "success",
                            confirmButtonClass:
                                "btn btn-success",
                            confirmButtonText:
                                "باشه",
                            buttonsStyling:
                                false
                        });
                    }
                    else {
                        $(".edit-form .error")
                            .html(
                                message ||
                                "ویرایش اطلاعات انجام نشد."
                            );

                        setScrollPosition();
                    }
                }
            ).fail(function () {
                startSubmition = false;

                $(".edit-form .error")
                    .html(
                        "ویرایش اطلاعات با خطا همراه بوده است. مجدداً اقدام کنید."
                    );

                setScrollPosition();
            });

            return false;
        }
    },

    delete: {
        loadForm: function (id) {
            if (!id) {
                showNotification(
                    "اطلاعات مورد نظر را انتخاب کنید.",
                    "danger"
                );

                return;
            }

            swal({
                title: "آیا مطمئن هستید؟",
                text:
                    "بعد از حذف، اطلاعات قابل برگشت نیست!",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass:
                    "btn btn-danger",
                cancelButtonClass:
                    "btn btn-default",
                confirmButtonText:
                    "بله، حذف شود!",
                cancelButtonText:
                    "لغو",
                buttonsStyling:
                    false
            }).then(function (isConfirm) {
                if (isConfirm) {
                    diningHall.delete
                        .confirm(id);
                }
            });
        },

        confirm: function (id) {
            var token =
                diningHall.helpers
                    .getAntiForgeryToken();

            $.ajax({
                url:
                    diningHall.urls.delete,

                type:
                    "POST",

                data: {
                    id: id,

                    __RequestVerificationToken:
                        token
                },

                success: function (response) {
                    var status =
                        response.status === true ||
                        response.Status === true;

                    var message =
                        response.message ||
                        response.Message;

                    if (status) {
                        diningHall.list.reload();

                        swal({
                            title: "حذف شد!",
                            text:
                                message ||
                                "سالن غذاخوری با موفقیت حذف شد.",
                            type: "success",
                            confirmButtonClass:
                                "btn btn-success",
                            confirmButtonText:
                                "باشه",
                            buttonsStyling:
                                false
                        });
                    }
                    else {
                        swal({
                            title: "حذف نشد!",
                            text:
                                message ||
                                "حذف اطلاعات انجام نشد.",
                            type: "error",
                            confirmButtonClass:
                                "btn btn-danger",
                            confirmButtonText:
                                "باشه",
                            buttonsStyling:
                                false
                        });
                    }
                },

                error: function () {
                    swal({
                        title: "حذف نشد!",
                        text:
                            "حذف اطلاعات با خطا همراه بوده است!",
                        type: "error",
                        confirmButtonClass:
                            "btn btn-danger",
                        confirmButtonText:
                            "باشه",
                        buttonsStyling:
                            false
                    });
                }
            });
        }
    },

    filter: {
        collect: function () {
            return {
                Title:
                    $("#FilterTitle").val(),

                OrgId:
                    $("#FilterOrgId").val(),

                PersonalTypeId:
                    $("#FilterPersonalTypeId")
                        .val(),

                UsageType:
                    $("#FilterUsageType")
                        .val(),

                IsInternal:
                    $("#FilterIsInternal")
                        .val(),

                IsActive:
                    $("#FilterIsActive")
                        .val()
            };
        },

        clear: function () {
            $("#FilterTitle").val("");
            $("#FilterOrgId").val("");
            $("#FilterPersonalTypeId").val("");
            $("#FilterUsageType").val("");
            $("#FilterIsInternal").val("");
            $("#FilterIsActive").val("");

            $(".filter .selectpicker")
                .selectpicker("refresh");

            diningHall.list.reload();
        }
    },

    helpers: {
        formatNumber: function (value) {
            if (!value) {
                return "";
            }

            value = value
                .toString()
                .replace(/,/g, "")
                .replace(/[^0-9]/g, "");

            if (!value) {
                return "";
            }

            return Number(value)
                .toLocaleString("en-US");
        },

        initCapacityMask: function () {
            var $capacitySep =
                $(".hall-capacity-separator");

            var $capacity =
                $(".hall-capacity");

            if (
                $capacitySep.length === 0 ||
                $capacity.length === 0
            ) {
                return;
            }

            $capacitySep
                .off("input.diningHall")
                .on(
                    "input.diningHall",
                    function () {
                        var value =
                            diningHall.helpers
                                .toEnglishDigits(
                                    $(this).val()
                                )
                                .replace(
                                    /[^0-9]/g,
                                    ""
                                );

                        $(this).val(
                            diningHall.helpers
                                .formatNumber(value)
                        );

                        $capacity.val(value);
                    }
                );

            if ($capacitySep.val()) {
                var value =
                    diningHall.helpers
                        .toEnglishDigits(
                            $capacitySep.val()
                        )
                        .replace(
                            /[^0-9]/g,
                            ""
                        );

                $capacitySep.val(
                    diningHall.helpers
                        .formatNumber(value)
                );

                $capacity.val(value);
            }
        },

        setCapacityValue: function ($form) {
            var value =
                diningHall.helpers
                    .toEnglishDigits(
                        $form
                            .find(
                                ".hall-capacity-separator"
                            )
                            .val() || ""
                    )
                    .replace(
                        /[^0-9]/g,
                        ""
                    );

            $form
                .find(".hall-capacity")
                .val(value);
        },

        initPhoneNumber: function () {
            $(".phone-number")
                .off("input.diningHall")
                .on(
                    "input.diningHall",
                    function () {
                        var value =
                            diningHall.helpers
                                .toEnglishDigits(
                                    $(this).val()
                                )
                                .replace(
                                    /[^0-9+\-\s]/g,
                                    ""
                                );

                        $(this).val(value);
                    }
                );
        },

        toEnglishDigits: function (value) {
            if (
                value === null ||
                value === undefined
            ) {
                return "";
            }

            var persianDigits =
                "۰۱۲۳۴۵۶۷۸۹";

            var arabicDigits =
                "٠١٢٣٤٥٦٧٨٩";

            return value
                .toString()
                .replace(
                    /[۰-۹]/g,
                    function (character) {
                        return persianDigits
                            .indexOf(character);
                    }
                )
                .replace(
                    /[٠-٩]/g,
                    function (character) {
                        return arabicDigits
                            .indexOf(character);
                    }
                );
        },

        getAntiForgeryToken: function () {
            return $(
                "#delete-token-form " +
                "input[name='__RequestVerificationToken']"
            ).val() || "";
        }
    }
};

$(document).ready(function () {
    diningHall.list.initial();

    $(".filter .selectpicker")
        .selectpicker("refresh");
});