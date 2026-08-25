/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Admin/Dashboard" });
breadcrumb.push({ title: "غذای من", link: "#" });

var startSubmition = false;

var myFood = {

    urls: {
        getList: "/FoodUser/MyFood/GetList",
        loadChangeFoodForm: "/FoodUser/MyFood/LoadChangeFoodForm/",
        changeFood: "/FoodUser/MyFood/ChangeFood"
    },

    // لیست غذای من
    list: {
        table: null,

        initial: function () {
            this.table = $("#myFoodTable").DataTable({
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
                    url: myFood.urls.getList,
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        return $.extend({}, d, myFood.filter.collect());
                    }
                },
                columns: [
                    { data: "id", name: "شناسه" },
                    { data: "qoutaAllocationDateFa", name: "تاریخ" },
                    { data: "dayTitle", name: "روز" },
                    { data: "mealTitle", name: "وعده" },
                    { data: "foodTitle", name: "غذا" },
                    { data: "foodTokenTypeTitle", name: "نوع ژتون" },
                    {
                        data: "isDelivered",
                        name: "وضعیت تحویل",
                        render: function (data, type, row) {
                            if (row.isDelivered === true) {
                                return `<span class="text-success">تحویل شده</span>`;
                            }

                            return `<span class="text-warning">تحویل نشده</span>`;
                        }
                    },
                    {
                        data: null,
                        className: "text-left",
                        orderable: false,
                        searchable: false,
                        render: function (data, type, row) {
                            var btns = "";

                            if (row.canChange === true && row.isDelivered !== true) {
                                btns += `
                                    <a onclick="myFood.changeFood.loadForm(${row.id})"
                                       class="btn btn-simple btn-info btn-icon"
                                       title="تغییر غذا"
                                       data-toggle="tooltip">
                                        <i class="material-icons">edit</i>
                                    </a>`;
                            }

                            if (row.deliveryCode) {
                                btns += `
                                    <a onclick="myFood.token.showCode('${row.deliveryCode}')"
                                       class="btn btn-simple btn-primary btn-icon"
                                       title="مشاهده کد ژتون"
                                       data-toggle="tooltip">
                                        <i class="material-icons">confirmation_number</i>
                                    </a>`;
                            }

                            if (!btns) {
                                return `<span class="text-muted">-</span>`;
                            }

                            return btns;
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: [2, 3, 5, 6, 7],
                        orderable: false
                    }
                ]
            });
        },

        reload: function () {
            if (myFood.list.table) {
                myFood.list.table.ajax.reload(function () { }, false);
            }
        }
    },

    // آماده سازی فرم‌ها
    form: {
        initial: function () {
            $("#base-modal").removeClass("small-modal");

            $(".selectpicker").selectpicker("refresh");

            if ($.material) {
                $.material.init();
            }
        }
    },

    // تغییر غذا
    changeFood: {
        loadForm: function (qoutaPersonId) {
            if (!qoutaPersonId) {
                myFood.helpers.notify("شناسه غذای ثبت‌شده نامعتبر است", "danger");
                return;
            }

            $.get(myFood.urls.loadChangeFoodForm + qoutaPersonId, function (res) {
                $("#modal-form").html(res);

                myFood.form.initial();
                modal.open();

                myFood.helpers.parseValidation(".change-food-form");
            }).fail(function () {
                myFood.helpers.notify("لود فرم تغییر غذا با خطا همراه بوده است", "danger");
            });
        },

        save: function (e) {
            e.preventDefault();

            if (startSubmition === true) {
                return false;
            }

            startSubmition = true;

            var $form = $(".change-food-form");

            $form.validate();

            if (!$form.valid()) {
                startSubmition = false;
                return false;
            }

            var targetUrl = $form.attr("action") || myFood.urls.changeFood;
            var data = $form.serialize();

            $.post(targetUrl, data, function (res) {
                startSubmition = false;

                var status = myFood.helpers.getStatus(res);
                var message = myFood.helpers.getMessage(res);

                if (status) {
                    myFood.list.reload();
                    modal.close();

                    swal({
                        title: "ویرایش شد!",
                        text: message || "غذا با موفقیت تغییر کرد",
                        type: "success",
                        confirmButtonClass: "btn btn-success",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
                else {
                    $(".change-food-form .error").html(message || "تغییر غذا با خطا همراه بوده است.");
                    setScrollPosition();
                }
            }).fail(function () {
                startSubmition = false;

                $(".change-food-form .error").html("تغییر غذا با خطا همراه بوده است. مجددا اقدام کنید.");
                setScrollPosition();
            });

            return false;
        }
    },

    // نمایش کد ژتون
    token: {
        showCode: function (deliveryCode) {
            if (!deliveryCode) {
                myFood.helpers.notify("کد ژتون یافت نشد", "danger");
                return;
            }

            swal({
                title: "کد ژتون",
                text: deliveryCode,
                type: "info",
                confirmButtonClass: "btn btn-info",
                confirmButtonText: "باشه",
                buttonsStyling: false
            });
        }
    },

    // فیلترها
    filter: {
        initial: function () {
            $(".selectpicker").selectpicker("refresh");
        },

        collect: function () {
            return {
                StartDate: $("input[name=FilterStartDate]").val(),
                EndDate: $("input[name=FilterEndDate]").val(),
                MealId: $("#FilterMealId").val(),
                IsDelivered: $("#FilterIsDelivered").val()
            };
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

            if (token) {
                return token;
            }

            return "";
        },

        notify: function (message, type) {
            if (typeof showNotification === "function") {
                showNotification(message, type || "info");
                return;
            }

            swal({
                title: type === "danger" ? "خطا!" : "پیام",
                text: message,
                type: type === "danger" ? "error" : "info",
                confirmButtonClass: "btn btn-info",
                confirmButtonText: "باشه",
                buttonsStyling: false
            });
        }
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
    myFood.list.initial();
    myFood.filter.initial();
}