var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Admin/Dashboard" });

var startSubmition = false;

var foodSource = {
    urls: {
        getList: "/FoodPlan/FoodSource/GetList",
        loadCreateForm: "/FoodPlan/FoodSource/LoadCreateForm",
        create: "/FoodPlan/FoodSource/Create",
        loadEditForm: "/FoodPlan/FoodSource/LoadEditForm/",
        edit: "/FoodPlan/FoodSource/Edit",
        delete: "/FoodPlan/FoodSource/Delete/"
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
                    [10, 25, 50, "All"]
                ],
                responsive: true,
                serverSide: true,
                processing: true,
                order: [[0, "desc"]],
                ajax: {
                    url: foodSource.urls.getList,
                    type: "POST",
                    dataType: "json",
                    data: function (data) {
                        return $.extend({}, data, filter.collect());
                    }
                },
                columns: [
                    { data: "id", name: "شناسه" },
                    { data: "yeganTypeName", name: "نوع خدمت" },
                    { data: "personalTypeName", name: "نوع پرسنل" },
                    { data: "dayTypeTitle", name: "نوع روز" },
                    { data: "percentBreakfast", name: "درصد صبحانه" },
                    { data: "percentLunch", name: "درصد ناهار" },
                    { data: "percentDinner", name: "درصد شام" },
                    {
                        data: null,
                        className: "text-left",
                        orderable: false,
                        render: function (data, type, row) {
                            return `
                                <a onclick="foodSource.edit.loadForm(${row.id})"
                                   class="btn btn-simple btn-info btn-icon"
                                   title="ویرایش"
                                   data-toggle="tooltip">
                                    <i class="material-icons">edit</i>
                                </a>
                                <a onclick="foodSource.delete.loadForm(${row.id})"
                                   class="btn btn-simple btn-danger btn-icon"
                                   title="حذف"
                                   data-toggle="tooltip">
                                    <i class="material-icons">close</i>
                                </a>`;
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: [1, 2, 3, 7],
                        orderable: false
                    }
                ]
            });
        },

        reload: function () {
            if (!foodSource.list.table)
                return;

            foodSource.list.table.ajax.reload(function () { }, false);
        }
    },

    form: {
        initial: function () {
            $("#base-modal").removeClass("small-modal");
            $(".selectpicker").selectpicker("refresh");

            if ($.material)
                $.material.init();

            foodSource.helpers.initPercentInputs();
        }
    },

    create: {
        loadForm: function () {
            $.get(
                foodSource.urls.loadCreateForm,
                function (response) {
                    $("#modal-form").html(response);
                    foodSource.form.initial();
                    modal.open();
                    foodSource.helpers.parseValidation(".create-form");
                }
            );
        },

        save: function (event) {
            event.preventDefault();

            if (startSubmition)
                return false;

            var $form = $(".create-form");

            $form.validate();

            if (!$form.valid())
                return false;

            startSubmition = true;

            $.post(
                $form.attr("action"),
                $form.serialize(),
                function (response) {
                    startSubmition = false;

                    if (foodSource.helpers.getStatus(response)) {
                        foodSource.list.reload();
                        modal.close();

                        swal({
                            title: "ذخیره شد!",
                            text: "مأخذ غذایی با موفقیت ثبت شد.",
                            type: "success",
                            confirmButtonClass: "btn btn-success",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    } else {
                        $(".create-form .error").html(
                            foodSource.helpers.getMessage(response)
                        );
                        setScrollPosition();
                    }
                }
            ).fail(function () {
                startSubmition = false;

                $(".create-form .error").html(
                    "ذخیره اطلاعات با خطا همراه بوده است."
                );
            });

            return false;
        }
    },

    edit: {
        loadForm: function (id) {
            $.get(
                foodSource.urls.loadEditForm + id,
                function (response) {
                    $("#modal-form").html(response);
                    foodSource.form.initial();
                    modal.open();
                    foodSource.helpers.parseValidation(".edit-form");
                }
            );
        },

        save: function (event) {
            event.preventDefault();

            if (startSubmition)
                return false;

            var $form = $(".edit-form");

            $form.validate();

            if (!$form.valid())
                return false;

            startSubmition = true;

            $.post(
                $form.attr("action"),
                $form.serialize(),
                function (response) {
                    startSubmition = false;

                    if (foodSource.helpers.getStatus(response)) {
                        foodSource.list.reload();
                        modal.close();

                        swal({
                            title: "ویرایش شد!",
                            text: "مأخذ غذایی با موفقیت ویرایش شد.",
                            type: "success",
                            confirmButtonClass: "btn btn-success",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    } else {
                        $(".edit-form .error").html(
                            foodSource.helpers.getMessage(response)
                        );
                    }
                }
            ).fail(function () {
                startSubmition = false;

                $(".edit-form .error").html(
                    "ویرایش اطلاعات با خطا همراه بوده است."
                );
            });

            return false;
        }
    },

    delete: {
        loadForm: function (id) {
            swal({
                title: "آیا مطمئنید؟",
                text: "بعد از حذف، اطلاعات قابل برگشت نیست!",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-danger",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "بله، حذف شود!",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm)
                    foodSource.delete.confirm(id);
            });
        },

        confirm: function (id) {
            $.ajax({
                url: foodSource.urls.delete + id,
                type: "POST",
                data: {
                    __RequestVerificationToken:
                        foodSource.helpers.getAntiForgeryToken()
                },
                success: function (response) {
                    if (foodSource.helpers.getStatus(response)) {
                        foodSource.list.reload();

                        swal({
                            title: "حذف شد!",
                            text: "مأخذ غذایی با موفقیت حذف شد.",
                            type: "success",
                            confirmButtonClass: "btn btn-success",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    } else {
                        swal({
                            title: "حذف نشد!",
                            text: foodSource.helpers.getMessage(response),
                            type: "error",
                            confirmButtonClass: "btn btn-danger",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    }
                }
            });
        }
    },

    helpers: {
        parseValidation: function (selector) {
            var form = $(selector)
                .removeData("validator")
                .removeData("unobtrusiveValidation");

            $.validator.unobtrusive.parse(form);
        },

        initPercentInputs: function () {
            $(".percent-input").on("input", function () {
                var value = parseFloat($(this).val());

                if (value < 0)
                    $(this).val(0);

                if (value > 100)
                    $(this).val(100);
            });
        },

        getStatus: function (response) {
            return response.status === true ||
                response.Status === true;
        },

        getMessage: function (response) {
            return response.message ||
                response.Message ||
                "عملیات انجام نشد.";
        },

        getAntiForgeryToken: function () {
            return $("#anti-forgery-form " +
                "input[name='__RequestVerificationToken']")
                .val() || "";
        }
    }
};

var filter = {
    initial: function () {
        $(".selectpicker").selectpicker("refresh");
    },

    collect: function () {
        return {
            YeganTypeId:
                $("#FilterYeganTypeId").val(),

            PersonalTypeId:
                $("#FilterPersonalTypeId").val(),

            DayType:
                $("#FilterDayType").val()
        };
    }
};

breadcrumb.push({
    title: "مأخذ غذایی",
    link: "#"
});

foodSource.list.initial();
filter.initial();