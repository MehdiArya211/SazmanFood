/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Admin/Dashboard" });

// آیا سابمیت آغاز شده است؟
var startSubmition = false;

var softwares = {

    urls: {
        getList: "/FoodMang/Kitchens/GetList",
        loadCreateForm: "/FoodMang/Kitchens/LoadCreateForm",
        create: "/FoodMang/Kitchens/Create",
        loadEditForm: "/FoodMang/Kitchens/LoadEditForm/",
        edit: "/FoodMang/Kitchens/Edit",
        delete: "/FoodMang/Kitchens/Delete/"
    },

    // لیست آشپزخانه‌ها
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
                    url: softwares.urls.getList,
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        return $.extend({}, d, filter.collect());
                    }
                },
                columns: [
                    { data: "id", name: "شناسه" },
                    { data: "title", name: "نام آشپزخانه" },
                    { data: "orgTitle", name: "یگان" },
                    { data: "cookingCapacityStr", name: "ظرفیت پخت" },
                    {
                        data: "isActive",
                        name: "وضعیت",
                        render: function (data, type, row) {
                            if (row.isActive === true)
                                return `<span class="text-success">فعال</span>`;

                            if (row.isActive === false)
                                return `<span class="text-danger">غیرفعال</span>`;

                            return `<span class="text-muted">نامشخص</span>`;
                        }
                    },
                    {
                        data: null,
                        className: "text-left",
                        orderable: false,
                        render: function (data, type, row) {
                            return `
                                <a onclick="softwares.edit.loadForm(${row.id})" class="btn btn-simple btn-info btn-icon" title="ویرایش" data-toggle="tooltip">
                                    <i class="material-icons">edit</i>
                                </a>
                                <a onclick="softwares.delete.loadForm(${row.id})" class="btn btn-simple btn-danger btn-icon" title="حذف" data-toggle="tooltip">
                                    <i class="material-icons">close</i>
                                </a>`;
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: [2, 5],
                        orderable: false
                    }
                ]
            });
        },

        // رفرش کردن دیتاتیبل
        reload: function () {
            if (softwares.list.table) {
                softwares.list.table.ajax.reload(function () { }, false);
            }
        }
    },

    // آماده سازی فرم ها
    form: {
        initial: function () {
            $('#base-modal').removeClass('small-modal');

            $('.selectpicker').selectpicker('refresh');

            if ($.material) {
                $.material.init();
            }

            softwares.helpers.initCapacityMask();
        }
    },

    create: {
        // لود کردن فرم افزودن آشپزخانه
        loadForm: function () {
            $.get(softwares.urls.loadCreateForm, function (res) {
                $("#modal-form").html(res);

                softwares.form.initial();
                modal.open();

                // اعمال ولیدیشن به فرمی که با اجکس لود شده است
                var form = $(".create-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                $.validator.unobtrusive.parse(form);
            });
        },

        // ذخیره اطلاعات آشپزخانه جدید
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

                var status = res.status === true || res.Status === true;
                var message = res.message || res.Message;

                if (status) {
                    softwares.list.reload();
                    modal.close();

                    swal({
                        title: "ذخیره شد!",
                        text: "آشپزخانه مورد نظر با موفقیت ذخیره شد",
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
        // لود کردن فرم ویرایش آشپزخانه
        loadForm: function (id) {
            if (!id) {
                showNotification("لطفا ابتدا اطلاعات مورد نظر را انتخاب نمایید!", "danger");
                return;
            }

            $.get(softwares.urls.loadEditForm + id, function (res) {
                $("#modal-form").html(res);

                softwares.form.initial();
                modal.open();

                // اعمال ولیدیشن به فرمی که با اجکس لود شده است
                var form = $(".edit-form")
                    .removeData("validator")
                    .removeData("unobtrusiveValidation");

                $.validator.unobtrusive.parse(form);
            });
        },

        // ذخیره اطلاعات ویرایش شده
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

                var status = res.status === true || res.Status === true;
                var message = res.message || res.Message;

                if (status) {
                    softwares.list.reload();
                    modal.close();

                    swal({
                        title: "ویرایش شد!",
                        text: "آشپزخانه مورد نظر با موفقیت ویرایش شد",
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

    // حذف آشپزخانه
    delete: {
        loadForm: function (id) {
            if (!id) {
                showNotification("لطفا ابتدا اطلاعات مورد نظر را انتخاب نمایید!", "danger");
                return;
            }

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
                if (isConfirm) {
                    softwares.delete.confirm(id);
                }
            });
        },

        confirm: function (id) {
            var token = softwares.helpers.getAntiForgeryToken();

            $.ajax({
                url: softwares.urls.delete + id,
                type: "POST",
                data: {
                    __RequestVerificationToken: token
                },
                success: function (res) {
                    var status = res.status === true || res.Status === true;
                    var message = res.message || res.Message;

                    if (status) {
                        softwares.list.reload();

                        swal({
                            title: "حذف شد!",
                            text: "اطلاعات مورد نظر با موفقیت حذف شد",
                            type: "success",
                            confirmButtonClass: "btn btn-success",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    }
                    else {
                        swal({
                            title: "حذف نشد!",
                            text: message || "حذف اطلاعات انجام نشد.",
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
                        text: "حذف با خطا همراه بوده است. مجددا اقدام کنید!",
                        type: "error",
                        confirmButtonClass: "btn btn-danger",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
            });
        }
    },

    helpers: {
        formatNumber: function (value) {
            if (!value)
                return "";

            value = value.toString().replace(/,/g, "");

            var number = parseFloat(value);

            if (isNaN(number))
                return "";

            return number.toLocaleString("en-US");
        },

        initCapacityMask: function () {
            var $capacitySep = $("#CookingCapacitySep");
            var $capacity = $("#CookingCapacity");

            if ($capacitySep.length === 0 || $capacity.length === 0)
                return;

            $capacitySep.off("input").on("input", function () {
                var formatted = softwares.helpers.formatNumber($(this).val());

                $(this).val(formatted);
                $capacity.val(formatted.replace(/,/g, ""));
            });

            if ($capacitySep.val()) {
                var formatted = softwares.helpers.formatNumber($capacitySep.val());

                $capacitySep.val(formatted);
                $capacity.val(formatted.replace(/,/g, ""));
            }
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
            Title: $("#FilterTitle").val(),
            OrgId: $("#FilterOrgId").val(),
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
    breadcrumb.push({ title: "مدیریت آشپزخانه", link: "#" });

    softwares.list.initial();
    filter.initial();
}

// تغییر تصویر کپچا
var changeCaptcha = function () {
    var d = new Date();

    $("#imgcpatcha").attr("src", "/Captcha/CaptchaImage?" + d.getTime());
    $("#captcha").val("");
};