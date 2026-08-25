/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Admin/Dashboard" });

var startSubmition = false;

var foodPlan = {

    urls: {
        getList: "/FoodPlan/FoodPlan/GetList",
        loadCreateForm: "/FoodPlan/FoodPlan/LoadCreateForm",
        create: "/FoodPlan/FoodPlan/Create",
        loadEditForm: "/FoodPlan/FoodPlan/LoadEditForm/",
        edit: "/FoodPlan/FoodPlan/Edit",
        delete: "/FoodPlan/FoodPlan/Delete/",

        getListFoodPlanDay: "/FoodPlan/FoodPlan/GetListFoodPlanDay/",
        loadCreateFormAddFood: "/FoodPlan/FoodPlan/LoadCreateFormAddFood",
        loadEditFoodPlanDay: "/FoodPlan/FoodPlan/LoadEditFoodPlanDay",
        createAddFood: "/FoodPlan/FoodPlan/CreateAddFood",
        deleteFoodPlanDay: "/FoodPlan/FoodPlan/DeleteFoodPlanDay/"
    },

    list: {
        table: null,

        initial: function () {
            this.table = $('#datatables').DataTable({
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
                    url: foodPlan.urls.getList,
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        return $.extend({}, d, filter.collect());
                    }
                },
                columns: [
                    { data: "id", name: "شناسه" },
                    { data: "yearsTitle", name: "سال" },
                    { data: "seasonTitle", name: "فصل" },
                    { data: "organTitle", name: "یگان" },
                    {
                        data: null,
                        className: "text-left",
                        orderable: false,
                        render: function (data, type, row) {
                            return `
                                <a onclick="foodPlan.foodPlanDay.loadForm(${row.id})" class="btn btn-simple btn-primary btn-icon" style="color:#f212cf" title="افزودن وعده غذایی" data-toggle="tooltip">
                                    <i class="material-icons">add</i>
                                </a>
                                <a onclick="foodPlan.edit.loadForm(${row.id})" class="btn btn-simple btn-info btn-icon" title="ویرایش" data-toggle="tooltip">
                                    <i class="material-icons">edit</i>
                                </a>
                                <a onclick="foodPlan.delete.loadForm(${row.id})" class="btn btn-simple btn-danger btn-icon" title="حذف" data-toggle="tooltip">
                                    <i class="material-icons">close</i>
                                </a>`;
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: [3, 4],
                        orderable: false
                    }
                ]
            });
        },

        reload: function () {
            if (foodPlan.list.table) {
                foodPlan.list.table.ajax.reload(function () { }, false);
            }
        }
    },

    form: {
        initial: function () {
            $('#base-modal').removeClass('small-modal');

            $('.selectpicker').selectpicker('refresh');

            if ($.material) {
                $.material.init();
            }
        }
    },

    create: {
        loadForm: function () {
            $.get(foodPlan.urls.loadCreateForm, function (res) {
                $("#modal-form").html(res);

                foodPlan.form.initial();
                modal.open();

                foodPlan.helpers.parseValidation(".create-form");
            });
        },

        save: function (e) {
            e.preventDefault();

            if (startSubmition === true)
                return false;

            startSubmition = true;

            var $form = $(".create-form");

            $form.validate();

            if (!$form.valid()) {
                startSubmition = false;
                return false;
            }

            var targetUrl = $form.attr("action");
            var data = $form.serialize();

            $.post(targetUrl, data, function (res) {
                startSubmition = false;

                var status = foodPlan.helpers.getStatus(res);
                var message = foodPlan.helpers.getMessage(res);

                if (status) {
                    foodPlan.list.reload();
                    modal.close();

                    swal({
                        title: "ذخیره شد!",
                        text: "برنامه غذایی مورد نظر با موفقیت ذخیره شد",
                        type: "success",
                        confirmButtonClass: "btn btn-success",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
                else {
                    $(".create-form .error").html(message || "ثبت اطلاعات با خطا همراه بوده است.");
                    setScrollPosition();
                }
            }).fail(function () {
                startSubmition = false;

                $(".create-form .error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                setScrollPosition();
            });

            return false;
        }
    },

    edit: {
        loadForm: function (id) {
            if (!id) {
                showNotification("لطفا ابتدا برنامه غذایی مورد نظر را انتخاب نمایید!", "danger");
                return;
            }

            $.get(foodPlan.urls.loadEditForm + id, function (res) {
                $("#modal-form").html(res);

                foodPlan.form.initial();
                modal.open();

                foodPlan.helpers.parseValidation(".edit-form");
            });
        },

        save: function (e) {
            e.preventDefault();

            if (startSubmition === true)
                return false;

            startSubmition = true;

            var $form = $(".edit-form");

            $form.validate();

            if (!$form.valid()) {
                startSubmition = false;
                return false;
            }

            var targetUrl = $form.attr("action");
            var data = $form.serialize();

            $.post(targetUrl, data, function (res) {
                startSubmition = false;

                var status = foodPlan.helpers.getStatus(res);
                var message = foodPlan.helpers.getMessage(res);

                if (status) {
                    foodPlan.list.reload();
                    modal.close();

                    swal({
                        title: "ویرایش شد!",
                        text: "برنامه غذایی مورد نظر با موفقیت ویرایش شد",
                        type: "success",
                        confirmButtonClass: "btn btn-success",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
                else {
                    $(".edit-form .error").html(message || "ویرایش اطلاعات با خطا همراه بوده است.");
                    setScrollPosition();
                }
            }).fail(function () {
                startSubmition = false;

                $(".edit-form .error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                setScrollPosition();
            });

            return false;
        }
    },

    delete: {
        loadForm: function (id) {
            if (!id) {
                showNotification("لطفا ابتدا برنامه غذایی مورد نظر را انتخاب نمایید!", "danger");
                return;
            }

            swal({
                title: "آیا مطمئنید؟",
                text: "بعد از حذف، اطلاعات برنامه غذایی قابل برگشت نیست!",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-danger",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "بله، حذف شود!",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm) {
                    foodPlan.delete.confirm(id);
                }
            });
        },

        confirm: function (id) {
            $.ajax({
                url: foodPlan.urls.delete + id,
                type: "POST",
                data: {
                    __RequestVerificationToken: foodPlan.helpers.getAntiForgeryToken()
                },
                success: function (res) {
                    var status = foodPlan.helpers.getStatus(res);
                    var message = foodPlan.helpers.getMessage(res);

                    if (status) {
                        foodPlan.list.reload();

                        swal({
                            title: "حذف شد!",
                            text: "برنامه غذایی مورد نظر با موفقیت حذف شد",
                            type: "success",
                            confirmButtonClass: "btn btn-success",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    }
                    else {
                        swal({
                            title: "حذف نشد!",
                            text: message || "حذف برنامه غذایی انجام نشد.",
                            type: "error",
                            confirmButtonClass: "btn btn-danger",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    }
                },
                error: function () {
                    swal({
                        title: "حذف نشد!",
                        text: "حذف برنامه غذایی با خطا همراه بوده است. مجددا اقدام کنید!",
                        type: "error",
                        confirmButtonClass: "btn btn-danger",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
            });
        }
    },

    foodPlanDay: {
        table: null,

        loadForm: function (foodPlanId) {
            if (!foodPlanId) {
                showNotification("شناسه برنامه غذایی نامعتبر است!", "danger");
                return;
            }

            $.get(foodPlan.urls.loadCreateFormAddFood + "?foodPlanId=" + foodPlanId, function (res) {
                $("#modal-form").html(res);

                foodPlan.form.initial();
                modal.open();

                foodPlan.foodPlanDay.initialTable();
                foodPlan.helpers.parseValidation(".food-plan-day-form");
            });
        },

        loadFormEdit: function (foodPlanDayId) {
            var foodPlanId = $("#FoodPlanId").val();

            if (!foodPlanId || !foodPlanDayId) {
                showNotification("اطلاعات وعده غذایی نامعتبر است!", "danger");
                return;
            }

            $.get(foodPlan.urls.loadEditFoodPlanDay + "?foodPlanDayId=" + foodPlanDayId + "&foodPlanId=" + foodPlanId, function (res) {
                $("#modal-form").html(res);

                foodPlan.form.initial();
                modal.open();

                foodPlan.foodPlanDay.initialTable();
                foodPlan.helpers.parseValidation(".food-plan-day-form");
            });
        },

        initialTable: function () {
            var foodPlanId = $("#FoodPlanId").val();

            if (!foodPlanId || $("#AddFoodDaydatatables").length === 0)
                return;

            if ($.fn.DataTable.isDataTable("#AddFoodDaydatatables")) {
                $("#AddFoodDaydatatables").DataTable().destroy();
            }

            foodPlan.foodPlanDay.table = $("#AddFoodDaydatatables").DataTable({
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
                    url: foodPlan.urls.getListFoodPlanDay + foodPlanId,
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        return $.extend({}, d);
                    }
                },
                columns: [
                    { data: "id", name: "شناسه" },
                    { data: "dayName", name: "روز" },
                    { data: "mealName", name: "وعده غذایی" },
                    { data: "foodTitle", name: "غذا" },
                    { data: "foodDesserTitle", name: "دسر" },
                    { data: "count", name: "تعداد" },
                    {
                        data: null,
                        className: "text-left",
                        orderable: false,
                        render: function (data, type, row) {
                            return `
                                <a onclick="foodPlan.foodPlanDay.loadFormEdit(${row.id})" class="btn btn-simple btn-info btn-icon" title="ویرایش" data-toggle="tooltip">
                                    <i class="material-icons">edit</i>
                                </a>
                                <a onclick="foodPlan.foodPlanDay.delete.loadForm(${row.id})" class="btn btn-simple btn-danger btn-icon" title="حذف" data-toggle="tooltip">
                                    <i class="material-icons">close</i>
                                </a>`;
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: [3, 6],
                        orderable: false
                    }
                ]
            });
        },

        reload: function () {
            if (foodPlan.foodPlanDay.table) {
                foodPlan.foodPlanDay.table.ajax.reload(function () { }, false);
            }
        },

        save: function (e) {
            e.preventDefault();

            if (startSubmition === true)
                return false;

            startSubmition = true;

            var $form = $(".food-plan-day-form");

            $form.validate();

            if (!$form.valid()) {
                startSubmition = false;
                return false;
            }

            var targetUrl = $form.attr("action");
            var data = $form.serialize();
            var foodPlanId = $("#FoodPlanId").val();

            $.post(targetUrl, data, function (res) {
                startSubmition = false;

                var status = foodPlan.helpers.getStatus(res);
                var message = foodPlan.helpers.getMessage(res);

                if (status) {
                    swal({
                        title: "ذخیره شد!",
                        text: message || "وعده غذایی با موفقیت ذخیره شد",
                        type: "success",
                        confirmButtonClass: "btn btn-success",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });

                    foodPlan.foodPlanDay.loadForm(foodPlanId);
                }
                else {
                    $(".food-plan-day-form .error").html(message || "ثبت وعده غذایی با خطا همراه بوده است.");
                    setScrollPosition();
                }
            }).fail(function () {
                startSubmition = false;

                $(".food-plan-day-form .error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                setScrollPosition();
            });

            return false;
        },

        delete: {
            loadForm: function (id) {
                if (!id) {
                    showNotification("لطفا ابتدا وعده غذایی مورد نظر را انتخاب نمایید!", "danger");
                    return;
                }

                swal({
                    title: "آیا مطمئنید؟",
                    text: "بعد از حذف، اطلاعات وعده غذایی قابل برگشت نیست!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonClass: "btn btn-danger",
                    cancelButtonClass: "btn btn-default",
                    confirmButtonText: "بله، حذف شود!",
                    cancelButtonText: "لغو",
                    buttonsStyling: false
                }).then(function (isConfirm) {
                    if (isConfirm) {
                        foodPlan.foodPlanDay.delete.confirm(id);
                    }
                });
            },

            confirm: function (id) {
                $.ajax({
                    url: foodPlan.urls.deleteFoodPlanDay + id,
                    type: "POST",
                    data: {
                        __RequestVerificationToken: foodPlan.helpers.getAntiForgeryToken()
                    },
                    success: function (res) {
                        var status = foodPlan.helpers.getStatus(res);
                        var message = foodPlan.helpers.getMessage(res);

                        if (status) {
                            foodPlan.foodPlanDay.reload();

                            swal({
                                title: "حذف شد!",
                                text: "وعده غذایی مورد نظر با موفقیت حذف شد",
                                type: "success",
                                confirmButtonClass: "btn btn-success",
                                confirmButtonText: "باشه",
                                buttonsStyling: false
                            });
                        }
                        else {
                            swal({
                                title: "حذف نشد!",
                                text: message || "حذف وعده غذایی انجام نشد.",
                                type: "error",
                                confirmButtonClass: "btn btn-danger",
                                confirmButtonText: "باشه",
                                buttonsStyling: false
                            });
                        }
                    },
                    error: function () {
                        swal({
                            title: "حذف نشد!",
                            text: "حذف وعده غذایی با خطا همراه بوده است. مجددا اقدام کنید!",
                            type: "error",
                            confirmButtonClass: "btn btn-danger",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    }
                });
            }
        }
    },

    helpers: {
        parseValidation: function (formSelector) {
            var form = $(formSelector)
                .removeData("validator")
                .removeData("unobtrusiveValidation");

            $.validator.unobtrusive.parse(form);
        },

        getStatus: function (res) {
            return res.status === true || res.Status === true;
        },

        getMessage: function (res) {
            return res.message || res.Message;
        },

        getAntiForgeryToken: function () {
            var token = $("input[name='__RequestVerificationToken']").first().val();

            if (token)
                return token;

            return "";
        }
    }
};

//==============================================
// فیلتر جستجو
//==============================================
var filter = {
    initial: function () {
        $('.selectpicker').selectpicker('refresh');
    },

    collect: function () {
        return {
            OrganTitle: $("#FilterOrganTitle").val(),
            YearsId: $("#FilterYearsId").val(),
            SeasonId: $("#FilterSeasonId").val(),
            IsActive: $("#FilterIsActive").val()
        };
    }
};

if (controller === "profile" && action === "edit") {
    breadcrumb.push({ title: "ویرایش پروفایل", link: "#" });
}
else if (controller === "profile" && action === "changepassword") {
    breadcrumb.push({ title: "تغییر کلمه عبور", link: "#" });
}
else if (controller === "profile" && action === "loginlog") {
    breadcrumb.push({ title: "لاگ ورود و خروج", link: "#" });
}
else {
    breadcrumb.push({ title: "برنامه غذایی", link: "#" });

    foodPlan.list.initial();
    filter.initial();
}

// تغییر تصویر کپچا
var changeCaptcha = function () {
    var d = new Date();

    $("#imgcpatcha").attr("src", "/Captcha/CaptchaImage?" + d.getTime());
    $("#captcha").val("");
};