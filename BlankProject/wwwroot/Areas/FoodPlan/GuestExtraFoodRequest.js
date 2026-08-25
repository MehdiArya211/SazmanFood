var startSubmition = false;

var guestExtraFoodRequest = {
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
                ajax: {
                    url: "/FoodPlan/GuestExtraFoodRequest/GetList",
                    type: "POST",
                    dataType: "json",
                    data: function (d) {
                        return $.extend({}, d, guestExtraFoodRequest.filter.collect());
                    }
                },
                columns: [
                    { data: "row", name: "ردیف" },
                    { data: "orgTitle", name: "یگان" },
                    { data: "mealTitle", name: "وعده" },
                    { data: "personalTypeTitle", name: "نوع پرسنل" },
                    { data: "yeganTypeTitle", name: "نوع خدمت" },
                    { data: "fromDateFa", name: "از تاریخ" },
                    { data: "toDateFa", name: "تا تاریخ" },
                    { data: "guestCount", name: "تعداد مهمان" },
                    { data: "extraQuotaCount", name: "تعداد مازاد" },
                    {
                        data: "statusTitle",
                        render: function (data, type, row) {
                            var cls = "text-info";

                            if (row.status == 1)
                                cls = "text-warning";

                            if (row.status == 2)
                                cls = "text-primary";

                            if (row.status == 3)
                                cls = "text-success";

                            if (row.status == 4)
                                cls = "text-warning";

                            if (row.status == 5)
                                cls = "text-danger";

                            var description = "";

                            if (row.status == 4)
                                description = row.returnReason || "";

                            if (row.status == 5)
                                description = row.cancelReason || "";

                            var title = description
                                ? " title='" + $("<div>").text(description).html() + "' data-toggle='tooltip'"
                                : "";

                            return '<span class="' + cls + '"' + title + '>' + data + '</span>';
                        }
                    },
                    {
                        data: null,
                        className: "text-left",
                        render: function (data, type, row) {
                            var btns = "";

                            btns += "<a onclick='guestExtraFoodRequest.attachment.load(" + row.id + ")' class='btn btn-simple btn-primary btn-icon' title='الصاقات' data-toggle='tooltip'><i class='material-icons'>attach_file</i></a>";

                            if (row.canEdit) {
                                btns += "<a onclick='guestExtraFoodRequest.edit.loadForm(" + row.id + ")' class='btn btn-simple btn-info btn-icon' title='ویرایش' data-toggle='tooltip'><i class='material-icons'>edit</i></a>";
                            }

                            if (row.canDelete) {
                                btns += "<a onclick='guestExtraFoodRequest.delete.loadForm(" + row.id + ")' class='btn btn-simple btn-danger btn-icon' title='حذف' data-toggle='tooltip'><i class='material-icons'>close</i></a>";
                            }

                            if (row.canSend) {
                                btns += "<a onclick='guestExtraFoodRequest.send.loadForm(" + row.id + ")' class='btn btn-simple btn-warning btn-icon' title='ارسال' data-toggle='tooltip'><i class='material-icons'>send</i></a>";
                            }

                            if (row.canApprove) {
                                btns += "<a onclick='guestExtraFoodRequest.approve.loadForm(" + row.id + ")' class='btn btn-simple btn-success btn-icon' title='تأیید' data-toggle='tooltip'><i class='material-icons'>done_all</i></a>";
                            }

                            if (row.canReturn) {
                                btns += "<a onclick='guestExtraFoodRequest.returnRequest.loadForm(" + row.id + ")' class='btn btn-simple btn-warning btn-icon' title='عودت برای اصلاح' data-toggle='tooltip'><i class='material-icons'>keyboard_return</i></a>";
                            }

                            if (row.canCancel) {
                                btns += "<a onclick='guestExtraFoodRequest.cancelRequest.loadForm(" + row.id + ")' class='btn btn-simple btn-danger btn-icon' title='لغو درخواست' data-toggle='tooltip'><i class='material-icons'>cancel</i></a>";
                            }

                            if (row.canPrintGuest) {
                                btns += "<a target='_blank' href='/FoodPlan/GuestExtraFoodRequest/PrintGuestTokens/" + row.id + "' class='btn btn-simple btn-default btn-icon' title='چاپ بن مهمان' data-toggle='tooltip'><i class='material-icons'>print</i></a>";
                            }

                            return btns;
                        }
                    }
                ],
                serverSide: false,
                order: [0, "asc"],
                processing: true,
                columnDefs: [{
                    targets: [10],
                    orderable: false
                }]
            });
        },

        reload: function () {
            guestExtraFoodRequest.list.table.ajax.reload(null, false);
        }
    },

    form: {
        initial: function () {
            $('.selectpicker').selectpicker('refresh');
            $.material.init();

            guestExtraFoodRequest.datePicker.initial();

            var form = $(".create-form,.edit-form")
                .removeData("validator")
                .removeData("unobtrusiveValidation");

            $.validator.unobtrusive.parse(form);
        }
    },

    datePicker: {
        fromDate: null,
        toDate: null,

        initial: function () {
            if ($('#VueFromDate').length > 0) {
                var from = $('#VueFromDate').data('date') || '';

                this.fromDate = new Vue({
                    el: '#VueFromDate',
                    data: {
                        date: from
                    },
                    components: {
                        DatePicker: VuePersianDatetimePicker
                    },
                    methods: {
                        onClose: function () {
                            guestExtraFoodRequest.datePicker.checkDates();
                        }
                    }
                });
            }

            if ($('#VueToDate').length > 0) {
                var to = $('#VueToDate').data('date') || '';

                this.toDate = new Vue({
                    el: '#VueToDate',
                    data: {
                        date: to
                    },
                    components: {
                        DatePicker: VuePersianDatetimePicker
                    },
                    methods: {
                        onClose: function () {
                            guestExtraFoodRequest.datePicker.checkDates();
                        }
                    }
                });
            }
        },

        checkDates: function () {
            if (!this.fromDate || !this.toDate)
                return;

            var from = this.fromDate.date;
            var to = this.toDate.date;

            if (from && to && from > to) {
                showNotification("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.", "danger");

                $("input[name=FromDate]").val("").prev("input").val("");
                $("input[name=ToDate]").val("").prev("input").val("");

                this.fromDate.date = "";
                this.toDate.date = "";
            }
        }
    },

    filter: {
        initial: function () {
            $('.selectpicker').selectpicker('refresh');
        },

        collect: function () {
            return {
                OrgId: $("#FilterOrgId").val(),
                MealId: $("#FilterMealId").val(),
                PersonalTypeId: $("#FilterPersonalTypeId").val(),
                YeganTypeId: $("#FilterYeganTypeId").val(),
                Status: $("#FilterStatus").val()
            };
        }
    },

    create: {
        loadForm: function () {
            $.get("/FoodPlan/GuestExtraFoodRequest/LoadCreateForm", function (res) {
                $("#modal-form").html(res);
                guestExtraFoodRequest.form.initial();
                modal.open();
            });
        },

        save: function (e) {
            e.preventDefault();

            if (startSubmition)
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

                if (res.status) {
                    guestExtraFoodRequest.list.reload();
                    modal.close();
                    showNotification(res.message || "اطلاعات با موفقیت ذخیره شد.", "success");
                }
                else {
                    $(".create-form .error").html(res.message);
                    setScrollPosition();
                }
            }).fail(function () {
                startSubmition = false;
                $(".create-form .error").html("ثبت اطلاعات با خطا همراه بود.");
                setScrollPosition();
            });

            return false;
        }
    },

    edit: {
        loadForm: function (id) {
            $.get("/FoodPlan/GuestExtraFoodRequest/LoadEditForm/" + id, function (res) {
                $("#modal-form").html(res);
                guestExtraFoodRequest.form.initial();
                modal.open();
            });
        },

        save: function (e) {
            e.preventDefault();

            if (startSubmition)
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

                if (res.status) {
                    guestExtraFoodRequest.list.reload();
                    modal.close();
                    showNotification(res.message || "اطلاعات با موفقیت ویرایش شد.", "success");
                }
                else {
                    $(".edit-form .error").html(res.message);
                    setScrollPosition();
                }
            }).fail(function () {
                startSubmition = false;
                $(".edit-form .error").html("ویرایش اطلاعات با خطا همراه بود.");
                setScrollPosition();
            });

            return false;
        }
    },

    delete: {
        loadForm: function (id) {
            swal({
                title: "آیا مطمئنید؟",
                text: "درخواست حذف می‌شود.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-danger",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "بله، حذف شود",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm)
                    guestExtraFoodRequest.delete.confirm(id);
            });
        },

        confirm: function (id) {
            var token = $("#operation-token-form input[name='__RequestVerificationToken']").val();

            $.post("/FoodPlan/GuestExtraFoodRequest/Delete/" + id, {
                __RequestVerificationToken: token
            }, function (res) {
                if (res.status) {
                    guestExtraFoodRequest.list.reload();
                    swal("حذف شد", res.message, "success");
                }
                else {
                    swal("حذف نشد", res.message, "error");
                }
            });
        }
    },

    send: {
        loadForm: function (id) {
            swal({
                title: "ارسال درخواست",
                text: "بعد از ارسال، درخواست قابل ویرایش نیست.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-warning",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "ارسال شود",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm)
                    guestExtraFoodRequest.send.confirm(id);
            });
        },

        confirm: function (id) {
            var token = $("#operation-token-form input[name='__RequestVerificationToken']").val();

            $.post("/FoodPlan/GuestExtraFoodRequest/Send/" + id, {
                __RequestVerificationToken: token
            }, function (res) {
                if (res.status) {
                    guestExtraFoodRequest.list.reload();
                    swal("ارسال شد", res.message, "success");
                }
                else {
                    swal("ارسال نشد", res.message, "error");
                }
            });
        }
    },

    approve: {
        loadForm: function (id) {
            swal({
                title: "تأیید درخواست",
                text: "بعد از تأیید، الصاقات قابل حذف نخواهد بود.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-success",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "تأیید شود",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm)
                    guestExtraFoodRequest.approve.confirm(id);
            });
        },

        confirm: function (id) {
            var token = $("#operation-token-form input[name='__RequestVerificationToken']").val();

            $.post("/FoodPlan/GuestExtraFoodRequest/Approve/" + id, {
                __RequestVerificationToken: token
            }, function (res) {
                if (res.status) {
                    guestExtraFoodRequest.list.reload();
                    swal("تأیید شد", res.message, "success");
                }
                else {
                    swal("تأیید نشد", res.message, "error");
                }
            });
        }
    },

    returnRequest: {
        loadForm: function (id) {
            swal({
                title: "عودت درخواست برای اصلاح",
                text: "توضیح دهید کدام بخش درخواست باید اصلاح شود.",
                type: "warning",
                input: "textarea",
                inputPlaceholder: "توضیحات و دلیل عودت...",
                inputAttributes: { maxlength: 1000 },
                showCancelButton: true,
                confirmButtonClass: "btn btn-warning",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "عودت درخواست",
                cancelButtonText: "انصراف",
                buttonsStyling: false,
                inputValidator: function (value) {
                    return new Promise(function (resolve, reject) {
                        value && value.trim()
                            ? resolve()
                            : reject("ثبت توضیحات عودت الزامی است.");
                    });
                }
            }).then(function (result) {
                var reason = result && result.value !== undefined
                    ? result.value
                    : result;

                if (reason && reason.trim())
                    guestExtraFoodRequest.returnRequest.confirm(id, reason);
            });
        },

        confirm: function (id, reason) {
            var token = $("#operation-token-form input[name='__RequestVerificationToken']").val();

            $.post("/FoodPlan/GuestExtraFoodRequest/Return/" + id, {
                reason: reason,
                __RequestVerificationToken: token
            }, function (res) {
                if (res.status) {
                    guestExtraFoodRequest.list.reload();
                    swal("عودت شد", res.message, "success");
                } else {
                    swal("عودت انجام نشد", res.message, "error");
                }
            });
        }
    },

    cancelRequest: {
        loadForm: function (id) {
            swal({
                title: "لغو درخواست",
                text: "دلیل لغو را وارد کنید. سابقه درخواست حذف نخواهد شد.",
                type: "error",
                input: "textarea",
                inputPlaceholder: "توضیحات و دلیل لغو...",
                inputAttributes: { maxlength: 1000 },
                showCancelButton: true,
                confirmButtonClass: "btn btn-danger",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "لغو درخواست",
                cancelButtonText: "انصراف",
                buttonsStyling: false,
                inputValidator: function (value) {
                    return new Promise(function (resolve, reject) {
                        value && value.trim()
                            ? resolve()
                            : reject("ثبت توضیحات لغو الزامی است.");
                    });
                }
            }).then(function (result) {
                var reason = result && result.value !== undefined
                    ? result.value
                    : result;

                if (reason && reason.trim())
                    guestExtraFoodRequest.cancelRequest.confirm(id, reason);
            });
        },

        confirm: function (id, reason) {
            var token = $("#operation-token-form input[name='__RequestVerificationToken']").val();

            $.post("/FoodPlan/GuestExtraFoodRequest/Cancel/" + id, {
                reason: reason,
                __RequestVerificationToken: token
            }, function (res) {
                if (res.status) {
                    guestExtraFoodRequest.list.reload();
                    swal("لغو شد", res.message, "success");
                } else {
                    swal("لغو انجام نشد", res.message, "error");
                }
            });
        }
    },

    attachment: {
        load: function (id) {
            $.get("/FoodPlan/GuestExtraFoodRequest/LoadAttachments/" + id, function (res) {
                $("#modal-form").html(res);
                $('.selectpicker').selectpicker('refresh');
                $.material.init();
                $('[data-toggle="tooltip"]').tooltip();
                modal.open();
            });
        },

        save: function (e) {
            e.preventDefault();

            var form = $(".attachment-form")[0];
            var data = new FormData(form);

            $.ajax({
                url: "/FoodPlan/GuestExtraFoodRequest/AddAttachment",
                type: "POST",
                data: data,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.status) {
                        var requestId = $("#AttachmentRequestId").val();
                        guestExtraFoodRequest.attachment.load(requestId);
                        guestExtraFoodRequest.list.reload();
                        showNotification(res.message, "success");
                    }
                    else {
                        $(".attachment-form .error").html(res.message);
                    }
                },
                error: function () {
                    $(".attachment-form .error").html("ثبت پیوست با خطا همراه بود.");
                }
            });

            return false;
        },

        delete: function (id) {
            swal({
                title: "حذف پیوست",
                text: "پیوست انتخاب‌شده حذف می‌شود.",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn btn-danger",
                cancelButtonClass: "btn btn-default",
                confirmButtonText: "حذف شود",
                cancelButtonText: "لغو",
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm)
                    guestExtraFoodRequest.attachment.confirmDelete(id);
            });
        },

        confirmDelete: function (id) {
            var token = $("#operation-token-form input[name='__RequestVerificationToken']").val();

            $.post("/FoodPlan/GuestExtraFoodRequest/DeleteAttachment/" + id, {
                __RequestVerificationToken: token
            }, function (res) {
                if (res.status) {
                    var requestId = $("#AttachmentRequestId").val();
                    guestExtraFoodRequest.attachment.load(requestId);
                    guestExtraFoodRequest.list.reload();
                    showNotification(res.message, "success");
                }
                else {
                    swal("حذف نشد", res.message, "error");
                }
            });
        }
    }
};

$(document).ready(function () {
    guestExtraFoodRequest.list.initial();
    guestExtraFoodRequest.filter.initial();
});