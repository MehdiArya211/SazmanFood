/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Qouta/Dashboard" });
breadcrumb.push({ title: "سهمیه بندی", link: "#" });

var startSubmition = false;

var qoutaAllocation = {

    urls: {
        getList: "/Qouta/QoutaAllocation/GetList",
        loadCreateForm: "/Qouta/QoutaAllocation/LoadCreateForm",
        create: "/Qouta/QoutaAllocation/Create",
        loadEditForm: "/Qouta/QoutaAllocation/LoadEditForm/",
        edit: "/Qouta/QoutaAllocation/Edit",
        delete: "/Qouta/QoutaAllocation/Delete/",
        getOrganGarrison: "/Qouta/QoutaAllocation/GetOrganGarrison/",

        loadCreateFormAddPerson: "/Qouta/QoutaAllocation/LoadCreateFormAddPerson/",
        getListQoutaPerson: "/Qouta/QoutaAllocation/GetListQoutaPerson",
        createAddPerson: "/Qouta/QoutaAllocation/CreateAddPerson",
        createAddPersonBulk: "/Qouta/QoutaAllocation/CreateAddPersonBulk",
        createAddPersonSingle: "/Qouta/QoutaAllocation/CreateAddPersonSingle",
        deletePerson: "/Qouta/QoutaAllocation/DeletePerson/",

        loadCreateGuestFood: "/Qouta/QoutaAllocation/LoadCreateGuestFood",
        createGuestFood: "/Qouta/QoutaAllocation/CreateGuestFood",
        getCapacityStatus: "/Qouta/QoutaAllocation/GetCapacityStatus"
    },

    //==============================================
    // لیست سهمیه بندی‌ها
    //==============================================
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
                    url: qoutaAllocation.urls.getList,
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        return $.extend({}, d, filter.collect());
                    }
                },
                columns: [
                    { data: "id", name: "شناسه" },
                    { data: "organGarrisonParentTitle", name: "پادگان" },
                    { data: "organGarrisonTitle", name: "یگان پادگان" },
                    { data: "dayTitle", name: "روز" },
                    { data: "qoutaAllocationDateFa", name: "تاریخ" },
                    { data: "mealTitle", name: "وعده" },
                    {
                        data: null,
                        className: "text-left",
                        orderable: false,
                        searchable: false,
                        render: function (data, type, row) {
                            return `
                                <a onclick="qoutaAllocation.createAddPerson.loadForm(${row.id})"
                                   class="btn btn-simple btn-info btn-icon"
                                   title="افزودن نفرات"
                                   data-toggle="tooltip">
                                    <i class="fas fa-coffee"></i>
                                </a>

                                <a onclick="qoutaAllocation.edit.loadForm(${row.id})"
                                   class="btn btn-simple btn-info btn-icon"
                                   title="ویرایش"
                                   data-toggle="tooltip">
                                    <i class="material-icons">edit</i>
                                </a>

                                <a onclick="qoutaAllocation.delete.loadForm(${row.id})"
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
                        targets: [1, 2, 3, 4, 5, 6],
                        orderable: false
                    }
                ]
            });
        },

        reload: function () {
            if (qoutaAllocation.list.table) {
                qoutaAllocation.list.table.ajax.reload(function () { }, false);
            }
        }
    },

    //==============================================
    // آماده سازی فرم‌ها
    //==============================================
    form: {
        initial: function () {
            $("#base-modal").removeClass("small-modal");

            $(".selectpicker").selectpicker("refresh");

            if ($.material) {
                $.material.init();
            }
        },

        bindOrganGarrisonCascade: function (selectedChildId) {
            $("#provinceList").off("change").on("change", function () {
                var id = $(this).val();
                var $cityList = $("#cityList");

                $cityList.empty();
                $cityList.append("<option value=''>انتخاب نمایید</option>");

                if (!id) {
                    $cityList.selectpicker("refresh");
                    return;
                }

                $.get(qoutaAllocation.urls.getOrganGarrison + id, function (res) {
                    $.each(res, function (i, item) {
                        $cityList.append(`<option value="${item.id}">${item.title}</option>`);
                    });

                    if (selectedChildId) {
                        $cityList.val(selectedChildId);
                    }

                    $cityList.selectpicker("refresh");
                });
            });
        }
    },

    //==============================================
    // ایجاد سهمیه بندی
    //==============================================
    create: {
        loadForm: function () {
            $.get(qoutaAllocation.urls.loadCreateForm, function (res) {
                $("#modal-form").html(res);

                qoutaAllocation.form.initial();
                modal.open();

                filter.RegDate.initial();
                filter.RegisterDeadline.initial();

                qoutaAllocation.form.bindOrganGarrisonCascade(null);
                qoutaAllocation.helpers.parseValidation(".create-form");
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

                var status = qoutaAllocation.helpers.getStatus(res);
                var message = qoutaAllocation.helpers.getMessage(res);

                if (status) {
                    qoutaAllocation.list.reload();
                    modal.close();

                    swal({
                        title: "ذخیره شد!",
                        text: "سهمیه بندی با موفقیت ذخیره شد",
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

    //==============================================
    // ویرایش سهمیه بندی
    //==============================================
    edit: {
        loadForm: function (id) {
            if (!id) {
                showNotification("لطفا ابتدا سهمیه بندی مورد نظر را انتخاب نمایید!", "danger");
                return;
            }

            $.get(qoutaAllocation.urls.loadEditForm + id, function (res) {
                $("#modal-form").html(res);

                qoutaAllocation.form.initial();
                modal.open();

                var selectedCityId = $("#SelectedCityId").val();

                filter.EditDate.initial();
                filter.RegisterDeadline.initial();

                qoutaAllocation.form.bindOrganGarrisonCascade(selectedCityId);
                qoutaAllocation.helpers.parseValidation(".edit-form");
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

                var status = qoutaAllocation.helpers.getStatus(res);
                var message = qoutaAllocation.helpers.getMessage(res);

                if (status) {
                    qoutaAllocation.list.reload();
                    modal.close();

                    swal({
                        title: "ویرایش شد!",
                        text: "سهمیه بندی با موفقیت ویرایش شد",
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

    //==============================================
    // افزودن نفرات به سهمیه
    //==============================================
    createAddPerson: {
        loadForm: function (id) {
            if (!id) {
                showNotification("شناسه سهمیه بندی نامعتبر است", "danger");
                return;
            }

            $.get(qoutaAllocation.urls.loadCreateFormAddPerson + id, function (res) {
                $("#seccond-modal-form").html(res);

                qoutaAllocation.form.initial();
                modal.open("#seccond-modal");

                qoutaAllocation.createAddPerson.list.initial(id);
                qoutaAllocation.helpers.parseValidation(".create-form-RegPortalCode");
            });
        },

        list: {
            table: null,

            initial: function (id) {
                if ($.fn.DataTable.isDataTable("#listPersonCourse")) {
                    $("#listPersonCourse").DataTable().destroy();
                    $("#listPersonCourse tbody").empty();
                }

                qoutaAllocation.createAddPerson.list.table = $("#listPersonCourse").DataTable({
                    drawCallback: function () {
                        $('[data-toggle="tooltip"]').tooltip();
                    },
                    language: {
                        url: "/assets/datatables/fa-lang.json"
                    },
                    pagingType: "full_numbers",
                    lengthMenu: [
                        [200, 500, 1000, 2000],
                        [200, 500, 1000, 2000]
                    ],
                    responsive: true,
                    processing: true,
                    serverSide: true,
                    order: [[1, "asc"]],
                    ajax: {
                        url: qoutaAllocation.urls.getListQoutaPerson,
                        type: "POST",
                        dataType: "json",
                        data: function (d) {
                            d.id = id;
                            return d;
                        }
                    },
                    columns: [
                        {
                            data: "personId",
                            orderable: false,
                            searchable: false,
                            render: function (personId, type, row) {
                                if (!personId || row.isRegistered === true)
                                    return "";

                                return `<input type="checkbox" class="person-select" value="${personId}" />`;
                            }
                        },
                        { data: "fName", name: "FName" },
                        { data: "lName", name: "LName" },
                        { data: "personalCode", name: "PersonalCode" },
                        {
                            data: "foodTitle",
                            name: "FoodTitle",
                            render: function (data, type, row) {
                                if (row.isRegistered === true) {
                                    return `<span class="text-success">${data || "-"}</span>`;
                                }

                                return `<span class="text-muted">ثبت نشده</span>`;
                            }
                        },
                        {
                            data: "personId",
                            orderable: false,
                            searchable: false,
                            render: function (personId, type, row) {
                                if (!personId)
                                    return "-";

                                if (row.isRegistered === true)
                                    return `<span class="text-success">ثبت شده</span>`;

                                var opts = `<option value="">انتخاب غذا</option>`;

                                (window.availableMainFoods || []).forEach(function (x) {
                                    opts += `<option value="${x.value}">${x.text}</option>`;
                                });

                                return `<select class="form-control form-control-sm main-food" data-person-id="${personId}">
                                            ${opts}
                                        </select>`;
                            }
                        },
                        {
                            data: "personId",
                            orderable: false,
                            searchable: false,
                            render: function (personId, type, row) {
                                if (!personId)
                                    return "-";

                                if (row.isRegistered === true)
                                    return `<span class="text-muted">-</span>`;

                                return `
                                    <select class="form-control form-control-sm food-token-type" data-person-id="${personId}">
                                        <option value="1">عادی</option>
                                        <option value="2">مدیریتی</option>
                                    </select>`;
                            }
                        },
                        {
                            data: "personId",
                            orderable: false,
                            searchable: false,
                            render: function (personId, type, row) {
                                if (!personId)
                                    return "-";

                                if (row.isRegistered === true) {
                                    return `
                                        <a onclick="qoutaAllocation.createAddPerson.deletePerson.loadForm(${row.qoutaPersonId})"
                                           class="btn btn-simple btn-danger btn-icon"
                                           title="حذف غذای ثبت شده"
                                           data-toggle="tooltip">
                                            <i class="material-icons">close</i>
                                        </a>`;
                                }

                                return `<button type="button"
                                                class="btn btn-success btn-sm save-person-food"
                                                data-person-id="${personId}">
                                            ثبت
                                        </button>`;
                            }
                        }
                    ],
                    columnDefs: [
                        {
                            targets: [0, 5, 6, 7],
                            orderable: false
                        }
                    ]
                });

                qoutaAllocation.bulk.initial();
                qoutaAllocation.createAddPerson.bindSingleSave();
            },

            reload: function () {
                if (qoutaAllocation.createAddPerson.list.table) {
                    qoutaAllocation.createAddPerson.list.table.ajax.reload(function () { }, false);
                }
            }
        },

        save: function (e) {
            e.preventDefault();

            var $form = $(".create-form-RegPortalCode");

            $form.validate();

            if (!$form.valid())
                return false;

            $.post($form.attr("action"), $form.serialize(), function (res) {
                var status = qoutaAllocation.helpers.getStatus(res);
                var message = qoutaAllocation.helpers.getMessage(res);

                if (status) {
                    qoutaAllocation.createAddPerson.list.reload();

                    swal({
                        title: "ذخیره شد!",
                        text: message || "با موفقیت ثبت شد",
                        type: "success",
                        confirmButtonClass: "btn btn-success",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
                else {
                    $(".create-form-RegPortalCode .error").html(message || "ثبت اطلاعات با خطا همراه بوده است.");
                    setScrollPosition();
                }
            }).fail(function () {
                $(".create-form-RegPortalCode .error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                setScrollPosition();
            });

            return false;
        },

        bindSingleSave: function () {
            $(document)
                .off("click", ".save-person-food")
                .on("click", ".save-person-food", function () {
                    var personId = $(this).data("person-id");
                    var $tr = $(this).closest("tr");
                    var foodId = $tr.find("select.main-food").val();
                    var foodTokenTypeId = $tr.find("select.food-token-type").val() || 1;
                    var qoutaAllocationId = $("input[name='QoutaAllocationId']").val();
                    var token = qoutaAllocation.helpers.getAntiForgeryToken();

                    if (!personId) {
                        showNotification("شناسه پرسنل نامعتبر است");
                        return;
                    }

                    if (!foodId) {
                        showNotification("لطفا غذا را انتخاب کنید");
                        return;
                    }

                    $.ajax({
                        url: qoutaAllocation.urls.createAddPersonSingle,
                        type: "POST",
                        headers: {
                            "RequestVerificationToken": token,
                            "X-CSRF-TOKEN": token
                        },
                        data: {
                            __RequestVerificationToken: token,
                            QoutaAllocationId: qoutaAllocationId,
                            PersonId: personId,
                            FoodId: foodId,
                            FoodTokenTypeId: foodTokenTypeId
                        },
                        success: function (res) {
                            var status = qoutaAllocation.helpers.getStatus(res);
                            var message = qoutaAllocation.helpers.getMessage(res);

                            if (status) {
                                qoutaAllocation.createAddPerson.list.reload();

                                swal({
                                    title: "ثبت شد!",
                                    text: message || "غذا با موفقیت ثبت شد",
                                    type: "success",
                                    confirmButtonClass: "btn btn-success",
                                    confirmButtonText: "باشه",
                                    buttonsStyling: false
                                });
                            }
                            else {
                                showNotification(message || "خطا در ثبت");
                            }
                        },
                        error: function () {
                            showNotification("ثبت اطلاعات با خطا همراه بوده است");
                        }
                    });
                });
        },

        deletePerson: {
            loadForm: function (id) {
                if (!id) {
                    showNotification("لطفا ابتدا پرسنل را انتخاب نمایید!");
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
                        qoutaAllocation.createAddPerson.deletePerson.confirm(id);
                    }
                });
            },

            confirm: function (id) {
                var token = qoutaAllocation.helpers.getAntiForgeryToken();

                $.ajax({
                    url: qoutaAllocation.urls.deletePerson + id,
                    type: "POST",
                    data: {
                        __RequestVerificationToken: token
                    },
                    success: function (res) {
                        var status = qoutaAllocation.helpers.getStatus(res);
                        var message = qoutaAllocation.helpers.getMessage(res);

                        if (status) {
                            qoutaAllocation.createAddPerson.list.reload();

                            swal({
                                title: "حذف شد!",
                                text: "با موفقیت حذف شد",
                                type: "success",
                                confirmButtonClass: "btn btn-success",
                                confirmButtonText: "باشه",
                                buttonsStyling: false
                            });
                        }
                        else {
                            swal({
                                title: "حذف نشد!",
                                text: message || "حذف با خطا همراه بوده است",
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
        }
    },

    //==============================================
    // ثبت گروهی
    //==============================================
    bulk: {
        initial: function () {
            $("#selectAllPeople").off("change").on("change", function () {
                var checked = $(this).is(":checked");
                $(".person-select").prop("checked", checked).trigger("change");
            });

            $(document).off("change", ".person-select").on("change", ".person-select", function () {
                var $tr = $(this).closest("tr");

                if ($(this).is(":checked"))
                    $tr.addClass("table-success");
                else
                    $tr.removeClass("table-success");
            });

            $("#btnApplyFoodToSelected").off("click").on("click", function () {
                var foodId = $("#bulkMainFood").val();

                if (!foodId) {
                    showNotification("لطفا یک غذا انتخاب کنید");
                    return;
                }

                var selected = $(".person-select:checked");

                if (selected.length === 0) {
                    showNotification("حداقل یک نفر را انتخاب کنید");
                    return;
                }

                selected.each(function () {
                    var $tr = $(this).closest("tr");

                    $tr.find("select.main-food")
                        .val(foodId)
                        .trigger("change");
                });

                showNotification("غذا روی افراد انتخاب‌شده اعمال شد");
            });

            $("#btnSaveBulk").off("click").on("click", function () {
                var selected = $(".person-select:checked");

                if (selected.length === 0) {
                    showNotification("حداقل یک نفر را انتخاب کنید");
                    return;
                }

                var items = [];
                var hasEmptyFood = false;

                selected.each(function () {
                    var $tr = $(this).closest("tr");
                    var personId = $(this).val();
                    var foodId = $tr.find("select.main-food").val();

                    if (!foodId)
                        hasEmptyFood = true;

                    items.push({
                        personId: parseInt(personId),
                        foodId: foodId ? parseInt(foodId) : null
                    });
                });

                if (hasEmptyFood) {
                    showNotification("برای همه افراد انتخاب‌شده غذا انتخاب نشده است");
                    return;
                }

                var qoutaAllocationId = $("input[name='QoutaAllocationId']").val();
                var foodTokenTypeId = $("#bulkFoodTokenTypeId").val() || 1;
                var token = qoutaAllocation.helpers.getAntiForgeryToken();

                $.ajax({
                    url: qoutaAllocation.urls.createAddPersonBulk,
                    type: "POST",
                    contentType: "application/json",
                    headers: {
                        "RequestVerificationToken": token,
                        "X-CSRF-TOKEN": token
                    },
                    data: JSON.stringify({
                        qoutaAllocationId: parseInt(qoutaAllocationId),
                        foodTokenTypeId: parseInt(foodTokenTypeId),
                        items: items
                    }),
                    success: function (res) {
                        var status = qoutaAllocation.helpers.getStatus(res);
                        var message = qoutaAllocation.helpers.getMessage(res);

                        if (status) {
                            qoutaAllocation.createAddPerson.list.reload();

                            swal({
                                title: "ذخیره شد!",
                                text: message || "ثبت گروهی با موفقیت انجام شد",
                                type: "success",
                                confirmButtonClass: "btn btn-success",
                                confirmButtonText: "باشه",
                                buttonsStyling: false
                            });
                        }
                        else {
                            swal({
                                title: "خطا!",
                                text: message || "ثبت گروهی با خطا همراه بوده است",
                                type: "error",
                                confirmButtonClass: "btn btn-danger",
                                confirmButtonText: "باشه",
                                buttonsStyling: false
                            });
                        }
                    },
                    error: function () {
                        showNotification("ثبت گروهی با خطا همراه بوده است. مجددا تلاش کنید");
                    }
                });
            });
        }
    },

    //==============================================
    // حذف سهمیه
    //==============================================
    delete: {
        loadForm: function (id) {
            if (!id) {
                showNotification("لطفا ابتدا سهمیه بندی مورد نظر را انتخاب نمایید!");
                return;
            }

            swal({
                title: "آیا مطمئنید؟",
                text: "بعد از حذف، اطلاعات سهمیه بندی قابل برگشت نیست!",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-danger",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "بله، حذف شود!",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm) {
                    qoutaAllocation.delete.confirm(id);
                }
            });
        },

        confirm: function (id) {
            var token = qoutaAllocation.helpers.getAntiForgeryToken();

            $.ajax({
                url: qoutaAllocation.urls.delete + id,
                type: "POST",
                data: {
                    __RequestVerificationToken: token
                },
                success: function (res) {
                    var status = qoutaAllocation.helpers.getStatus(res);
                    var message = qoutaAllocation.helpers.getMessage(res);

                    if (status) {
                        qoutaAllocation.list.reload();

                        swal({
                            title: "حذف شد!",
                            text: "سهمیه بندی مورد نظر با موفقیت حذف شد",
                            type: "success",
                            confirmButtonClass: "btn btn-success",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    }
                    else {
                        swal({
                            title: "حذف نشد!",
                            text: message || "حذف با خطا همراه بوده است",
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
                        text: "حذف سهمیه بندی با خطا همراه بوده است. مجددا اقدام کنید!",
                        type: "error",
                        confirmButtonClass: "btn btn-danger",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
            });
        }
    },

    //==============================================
    // ثبت مهمان
    //==============================================
    guest: {
        loadForm: function () {
            var qoutaAllocationId = $("input[name='QoutaAllocationId']").val();

            if (!qoutaAllocationId) {
                showNotification("شناسه سهمیه نامعتبر است");
                return;
            }

            $.get(qoutaAllocation.urls.loadCreateGuestFood + "?qoutaAllocationId=" + qoutaAllocationId, function (res) {
                $("#third-modal-form").html(res);

                qoutaAllocation.form.initial();
                modal.open("#third-modal");

                qoutaAllocation.helpers.parseValidation(".guest-food-form");
            });
        },

        save: function (e) {
            e.preventDefault();

            var $form = $(".guest-food-form");

            $form.validate();

            if (!$form.valid())
                return false;

            $.post($form.attr("action"), $form.serialize(), function (res) {
                var status = qoutaAllocation.helpers.getStatus(res);
                var message = qoutaAllocation.helpers.getMessage(res);

                if (status) {
                    modal.close("#third-modal");

                    qoutaAllocation.createAddPerson.list.reload();

                    swal({
                        title: "ثبت شد!",
                        text: message || "مهمان با موفقیت ثبت شد",
                        type: "success",
                        confirmButtonClass: "btn btn-success",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
                else {
                    $(".guest-food-form .error").html(message || "ثبت مهمان با خطا همراه بوده است.");
                    setScrollPosition();
                }
            }).fail(function () {
                $(".guest-food-form .error").html("ثبت مهمان با خطا همراه بوده است.");
                setScrollPosition();
            });

            return false;
        }
    },

    //==============================================
    // ابزارها
    //==============================================
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
        filter.startDate.initial();
        filter.bindFilterOrganGarrison();

        $(".selectpicker").selectpicker("refresh");
    },

    collect: function () {
        return {
            OrganGarrisonParentId: $("#FilterOrganGarrisonParentId").val(),
            OrganGarrisonId: $("#FilterOrganGarrisonId").val(),
            StartDate: $("input[name=FilterStartDate]").val(),
            DayId: $("#FilterDayId").val(),
            MealId: $("#FilterMealId").val()
        };
    },

    bindFilterOrganGarrison: function () {
        $("#FilterOrganGarrisonParentId").off("change").on("change", function () {
            var id = $(this).val();
            var $child = $("#FilterOrganGarrisonId");

            $child.empty();
            $child.append("<option value=''>انتخاب کنید</option>");

            if (!id) {
                $child.selectpicker("refresh");
                return;
            }

            $.get(qoutaAllocation.urls.getOrganGarrison + id, function (res) {
                $.each(res, function (i, item) {
                    $child.append(`<option value="${item.id}">${item.title}</option>`);
                });

                $child.selectpicker("refresh");
            });
        });
    },

    startDate: {
        obj: null,

        initial: function () {
            if ($("#VueStartDate").length === 0)
                return;

            this.obj = new Vue({
                el: "#VueStartDate",
                data: {
                    date: ""
                },
                components: {
                    DatePicker: VuePersianDatetimePicker
                },
                methods: {
                    onClose: function () { }
                }
            });
        }
    },

    RegDate: {
        obj: null,

        initial: function () {
            if ($("#VueStartDate1").length === 0)
                return;

            this.obj = new Vue({
                el: "#VueStartDate1",
                data: {
                    date: ""
                },
                components: {
                    DatePicker: VuePersianDatetimePicker
                },
                methods: {
                    onClose: function () { }
                }
            });
        }
    },

    EditDate: {
        obj: null,

        initial: function () {
            if ($("#VueStartDate1").length === 0)
                return;

            this.obj = new Vue({
                el: "#VueStartDate1",
                data: {
                    date: moment().format("jYYYY/jM/jD")
                },
                components: {
                    DatePicker: VuePersianDatetimePicker
                }
            });
        }
    },

    RegisterDeadline: {
        obj: null,

        initial: function () {
            if ($("#VueRegisterDeadline").length === 0)
                return;

            this.obj = new Vue({
                el: "#VueRegisterDeadline",
                data: {
                    date: ""
                },
                components: {
                    DatePicker: VuePersianDatetimePicker
                },
                methods: {
                    onClose: function () { }
                }
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
    breadcrumb.push({ title: "سهمیه بندی", link: "#" });

    qoutaAllocation.list.initial();
    filter.initial();
}