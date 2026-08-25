/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Admin/Dashboard" });

var startSubmition = false;

var orgGari = {

    urls: {
        getList: "/OrganGarrison/OrganGarrison/GetList",
        loadCreateForm: "/OrganGarrison/OrganGarrison/LoadCreateForm",
        create: "/OrganGarrison/OrganGarrison/Create",
        loadEditForm: "/OrganGarrison/OrganGarrison/LoadEditForm/",
        edit: "/OrganGarrison/OrganGarrison/Edit",

        loadCreatePersons: "/OrganGarrison/OrganGarrison/LoadCreatePersons/",
        addPerson: "/OrganGarrison/OrganGarrison/AddPerson",
        getListPerson: "/OrganGarrison/OrganGarrison/GetListPerson/",
        getPersonalByCode: "/OrganGarrison/OrganGarrison/OnGetGetPersonalId/",
        deletePerson: "/OrganGarrison/OrganGarrison/DeletePerson/"
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
                    url: orgGari.urls.getList,
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        return $.extend({}, d, filter.collect());
                    }
                },
                columns: [
                    { data: "id", name: "ردیف" },
                    { data: "title", name: "یگان" },
                    { data: "parentTitle", name: "یگان مادر" },
                    { data: "organGarrisonTypeTitle", name: "نوع یگان" },
                    { data: "estedadKadr", name: "استعداد کادر" },
                    { data: "estedadVazife", name: "استعداد وظیفه" },
                    {
                        data: null,
                        className: "text-left",
                        orderable: false,
                        render: function (data, type, row) {
                            return `
                                <a onclick="orgGari.person.loadForm(${row.id})" class="btn btn-simple btn-primary btn-icon" title="افزودن پرسنل" data-toggle="tooltip">
                                    <i class="material-icons">person_add</i>
                                </a>
                                <a onclick="orgGari.edit.loadForm(${row.id})" class="btn btn-simple btn-info btn-icon" title="ویرایش" data-toggle="tooltip">
                                    <i class="material-icons">edit</i>
                                </a>`;
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: [2, 3, 6],
                        orderable: false
                    }
                ]
            });
        },

        reload: function () {
            if (orgGari.list.table) {
                orgGari.list.table.ajax.reload(function () { }, false);
            }
        }
    },

    form: {
        initial: function () {
            $("#base-modal").removeClass("small-modal");

            $(".selectpicker").selectpicker("refresh");

            if ($.material) {
                $.material.init();
            }
        }
    },

    create: {
        loadForm: function () {
            $.get(orgGari.urls.loadCreateForm, function (res) {
                $("#modal-form").html(res);

                orgGari.form.initial();
                modal.open();

                orgGari.helpers.parseValidation(".create-form");
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

            $.post($form.attr("action"), $form.serialize(), function (res) {
                startSubmition = false;

                var status = orgGari.helpers.getStatus(res);
                var message = orgGari.helpers.getMessage(res);

                if (status) {
                    orgGari.list.reload();
                    modal.close();

                    swal({
                        title: "ذخیره شد!",
                        text: "یگان مورد نظر با موفقیت ذخیره شد",
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
                showNotification("لطفا ابتدا یگان مورد نظر را انتخاب نمایید!", "danger");
                return;
            }

            $.get(orgGari.urls.loadEditForm + id, function (res) {
                $("#modal-form").html(res);

                orgGari.form.initial();
                modal.open();

                orgGari.helpers.parseValidation(".edit-form");
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

            $.post($form.attr("action"), $form.serialize(), function (res) {
                startSubmition = false;

                var status = orgGari.helpers.getStatus(res);
                var message = orgGari.helpers.getMessage(res);

                if (status) {
                    orgGari.list.reload();
                    modal.close();

                    swal({
                        title: "ویرایش شد!",
                        text: "یگان مورد نظر با موفقیت ویرایش شد",
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

    person: {
        table: null,

        loadForm: function (organGarrisonId) {
            if (!organGarrisonId) {
                showNotification("شناسه یگان نامعتبر است!", "danger");
                return;
            }

            $.get(orgGari.urls.loadCreatePersons + organGarrisonId, function (res) {
                $("#modal-form").html(res);

                orgGari.form.initial();
                modal.open();

                orgGari.person.bindEvents();
                orgGari.person.initialTable();
                orgGari.helpers.parseValidation(".person-create-form");
            });
        },

        bindEvents: function () {
            var $form = $(".person-create-form");

            $form.find("#PersonTypeId")
                .off("changed.bs.select change")
                .on("changed.bs.select change", function () {
                    orgGari.person.refreshFields();
                });

            $form.find("#PersonCode")
                .off("blur")
                .on("blur", function () {
                    orgGari.person.loadPersonalInfo($(this).val());
                });
        },

        refreshFields: function () {
            var $form = $(".person-create-form");

            var $personType = $form.find("#PersonTypeId");
            var personTypeId = $personType.val();

            // مقدار نوع پرسنل را نگه می‌داریم که selectpicker خالی نشود
            $personType.selectpicker("val", personTypeId);
            $personType.selectpicker("refresh");

            $form.find("#PersonCode").val("");
            $form.find("#NationalCode").val("");
            $form.find("#FullName").val("");
            $form.find("#person-not-found").hide();

            if (personTypeId == "1") {
                $form.find("#personCodeBox").show();
                $form.find("#nationalCodeBox").hide();
            }
            else if (personTypeId == "2") {
                $form.find("#nationalCodeBox").show();
                $form.find("#personCodeBox").hide();
            }
            else {
                $form.find("#personCodeBox").hide();
                $form.find("#nationalCodeBox").hide();
            }
        },

        loadPersonalInfo: function (personCode) {
            var $form = $(".person-create-form");

            if (!personCode)
                return;

            $form.find("#person-not-found").hide();

            $.get(orgGari.urls.getPersonalByCode + personCode, function (res) {
                if (!res) {
                    $form.find("#person-not-found").show();
                    $form.find("#FullName").val("");
                    return;
                }

                var firstName = res.firstName || res.FirstName || "";
                var lastName = res.lastName || res.LastName || "";

                var fullName = (firstName + " " + lastName).trim();

                if (!fullName) {
                    $form.find("#person-not-found").show();
                    $form.find("#FullName").val("");
                    return;
                }

                $form.find("#FullName").val(fullName);
            }).fail(function () {
                $form.find("#person-not-found").show();
                $form.find("#FullName").val("");
            });
        },

        initialTable: function () {
            var organGarrisonId = $("#OrganGarrisonId").val();

            if (!organGarrisonId || $("#datatables2").length === 0)
                return;

            if ($.fn.DataTable.isDataTable("#datatables2")) {
                $("#datatables2").DataTable().destroy();
            }

            orgGari.person.table = $("#datatables2").DataTable({
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
                    url: orgGari.urls.getListPerson + organGarrisonId,
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        return $.extend({}, d);
                    }
                },
                columns: [
                    { data: "id", name: "ردیف" },
                    { data: "fullName", name: "نام و نشان" },
                    { data: "personTypeTitle", name: "نوع پرسنل" },
                    {
                        data: null,
                        name: "کد",
                        render: function (data, type, row) {
                            return row.personCode || row.nationalCode || "-";
                        }
                    },
                    {
                        data: null,
                        className: "text-left",
                        orderable: false,
                        render: function (data, type, row) {
                            return `
                                <a onclick="orgGari.person.delete.loadForm(${row.id})" class="btn btn-simple btn-danger btn-icon" title="حذف" data-toggle="tooltip">
                                    <i class="material-icons">close</i>
                                </a>`;
                        }
                    }
                ],
                columnDefs: [
                    {
                        targets: [4],
                        orderable: false
                    }
                ]
            });
        },

        reload: function () {
            if (orgGari.person.table) {
                orgGari.person.table.ajax.reload(function () { }, false);
            }
        },

        save: function (e) {
            e.preventDefault();

            if (startSubmition === true)
                return false;

            startSubmition = true;

            var $form = $(".person-create-form");

            $form.validate();

            if (!$form.valid()) {
                startSubmition = false;
                return false;
            }

            $.post($form.attr("action"), $form.serialize(), function (res) {
                startSubmition = false;

                var status = orgGari.helpers.getStatus(res);
                var message = orgGari.helpers.getMessage(res);

                if (status) {
                    orgGari.person.reload();

                    $("#PersonCode").val("");
                    $("#NationalCode").val("");
                    $("#FullName").val("");
                    $("#person-not-found").hide();

                    swal({
                        title: "ذخیره شد!",
                        text: "پرسنل مورد نظر با موفقیت ثبت شد",
                        type: "success",
                        confirmButtonClass: "btn btn-success",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
                else {
                    $(".person-create-form .error").html(message || "ثبت اطلاعات با خطا همراه بوده است.");
                    setScrollPosition();
                }
            }).fail(function () {
                startSubmition = false;

                $(".person-create-form .error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                setScrollPosition();
            });

            return false;
        },

        delete: {
            loadForm: function (id) {
                if (!id) {
                    showNotification("لطفا ابتدا پرسنل مورد نظر را انتخاب نمایید!", "danger");
                    return;
                }

                swal({
                    title: "آیا مطمئنید؟",
                    text: "بعد از حذف، اطلاعات پرسنل قابل برگشت نیست!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonClass: "btn btn-danger",
                    cancelButtonClass: "btn btn-default",
                    confirmButtonText: "بله، حذف شود!",
                    cancelButtonText: "لغو",
                    buttonsStyling: false
                }).then(function (isConfirm) {
                    if (isConfirm) {
                        orgGari.person.delete.confirm(id);
                    }
                });
            },

            confirm: function (id) {
                $.ajax({
                    url: orgGari.urls.deletePerson + id,
                    type: "POST",
                    data: {
                        __RequestVerificationToken: orgGari.helpers.getAntiForgeryToken()
                    },
                    success: function (res) {
                        var status = orgGari.helpers.getStatus(res);
                        var message = orgGari.helpers.getMessage(res);

                        if (status) {
                            orgGari.person.reload();

                            swal({
                                title: "حذف شد!",
                                text: "پرسنل مورد نظر با موفقیت حذف شد",
                                type: "success",
                                confirmButtonClass: "btn btn-success",
                                confirmButtonText: "باشه",
                                buttonsStyling: false
                            });
                        }
                        else {
                            swal({
                                title: "حذف نشد!",
                                text: message || "حذف پرسنل انجام نشد.",
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
                            text: "حذف پرسنل با خطا همراه بوده است. مجددا اقدام کنید!",
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
        $(".selectpicker").selectpicker("refresh");
    },

    collect: function () {
        return {
            Title: $("#FilterTitle").val(),
            ParentId: $("#FilterParentId").val(),
            OrganGarrisonTypeId: $("#FilterOrganGarrisonTypeId").val()
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
    breadcrumb.push({ title: "مدیریت یگان", link: "#" });

    orgGari.list.initial();
    filter.initial();
}