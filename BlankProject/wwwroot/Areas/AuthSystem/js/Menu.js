//breadcrumb
var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/AuthSystem/Dashboard" });
breadcrumb.push({ title: "منو‌ ها", link: "#" });


// عملیات مربوط به مدیریت منوها
var menu = {
    // منو انتخاب شده
    selectedItem: null,


    // رفرش کردن لیست منو ها
    refreshList: function () {
        $.get("/AuthSystem/" + controller + "/GetAll",
            function (data) {
                menu.treeView.initial(data);
            });
    },
    


    // تری ویو
    treeView: {

        // راه اندازی تری ویو
        initial: function(data) {
            //menu.selectedItem = null;
            
            var model = menu.treeView.reshapeData(data, null);

            if (!model || model.length == 0) {
                $("#treeview").html("<div class='text-center'>موردی برای نمایش یافت نشد.</div>");
                return;
            }

            $("#treeview").treeview({
                data: model,
                levels: 0,
                onNodeSelected: function (event, node) {
                    menu.selectedItem = node;
                    $(".edit-btn,.delete-btn").removeClass("hide");
                    $(".create-btn").html("افزودن زیر منو");
                },
                onNodeUnselected: function (event, node) {
                    menu.selectedItem = null;
                    $(".edit-btn,.delete-btn").addClass("hide");
                    $(".create-btn").html("افزودن منو جدید");
                }
            });

            // انتخاب و باز کردن ساختار درختی آخرین نودی که انتخاب شده بود
            if (menu.selectedItem) {
                var UnselectedNode = $("#treeview").treeview('getUnselected');
                for (var j = 0; j < UnselectedNode.length; j++) {
                    var node = UnselectedNode[j];
                    if (node.id == menu.selectedItem.id) {
                        $("#treeview").treeview('selectNode', [node, { silent: false }]);
                        if (node.nodes != null && node.nodes.length > 0)
                            $("#treeview").treeview('expandNode', node);
                        while (node.parentId != null) {
                            var parent = $("#treeview").treeview('getParent', node);
                            $("#treeview").treeview('expandNode', parent);
                            node = parent;
                        }
                    }
                }
            }

        },


        // استخراج داده ها با ساختار مناسب برای تری ویو
        reshapeData: function (data, parentId) {
            if (data == null || data.length == 0)
                return null;

            var model = data.filter(function (obj) {
                return obj.parentId === parentId;
            });

            if (model == null || model.length == 0)
                return null;
            
            $.each(model, function () {
                this.nodes = menu.treeView.reshapeData(data, this.id);
                if (!this.isEnabled)
                    this.backColor = "rgba(0, 0, 0, 0.1)";
                var icon = ''; // '<i>' + this.text.substring(0, 3) + '</i>';
                if (this.icon)
                    icon = '<i class="material-icons">' + this.icon + '</i>'
                this.text = icon + '<span class="node-title">' +this.text+ '</span>';
            });
            return model;
        }
    },




    // ایجاد منو جدید
    create: {
        // لود کردن فرم افزودن منو جدید
        loadForm: function () {
            var data = {
                ParentId: menu.selectedItem == null ? null : menu.selectedItem.id,
                ParentName: menu.selectedItem == null ? null : menu.selectedItem.title
            };
            $.get("/AuthSystem/" + controller + "/LoadCreateForm",
                data,
                function (res) {
                    $("#modal-form").html(res);
                    $.material.init();
                    modal.open();
                    $('[data-toggle="tooltip"]').tooltip();

                    /*اعمال ولیدیشن به فرمی که با اجکس لود شده است*/
                    var form = $(".create-form")
                        .removeData("validator")
                        .removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse(form);
                });
        },


        // ذخیره کردن فرم افزودن منو جدید
        save: function (e) {
            e.preventDefault();
            var targetUrl = $(".create-form").attr("action");

            /*ولیدیت کردن فرم*/
            var $form = $(".create-form");
            $form.validate();
            if (!$form.valid()) return false;


            /*اعتبار سنجی فرم*/
            var isValid = menu.validate(".create-form");
            if (!isValid) return false;


            /*رندر کردن پارامترها*/
            menu.parameters.render();

            /*جمع آوری دیتای فرم*/
            var data = $(".create-form").serialize();

            /*ثبت فرم با اجکس*/
            $.post(targetUrl,
                data,
                function (res) {
                    if (res.status) {
                        modal.close();
                        menu.refreshList();
                    }
                    else {
                        $("#modal-form .create-error").html(res.message);
                        $("#base-modal").scrollTop(0);
                    }
                }).fail(function () {
                    $("#modal-form .create-error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                    $("#base-modal").scrollTop(0);
                });
            return false;
        }
    },





    // ویرایش منو
    edit: {
        // لود کردن فرم ویرایش منو
        loadForm: function () {
            if (menu.selectedItem == null) {
                alert("منوی مورد نظر را انتخاب کنید!");
                return;
            }
            var id = menu.selectedItem.id;
            $.get("/AuthSystem/" + controller + "/LoadEditForm/" + id,
                function (res) {
                    $("#modal-form").html(res);
                    $.material.init();
                    menu.parameters.expand();
                    modal.open();
                    $('[data-toggle="tooltip"]').tooltip();
                    
                    /*اعمال ولیدیشن به فرمی که با اجکس لود شده است*/
                    var form = $(".edit-form")
                        .removeData("validator")
                        .removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse(form);
                });
        },


        // ذخیره کردن فرم ویرایش منو
        save: function (e) {
            e.preventDefault();
            var targetUrl = $(".edit-form").attr("action");

            /*ولیدیت کردن فرم*/
            var $form = $(".edit-form");
            $form.validate();
            if (!$form.valid()) return false;


            /*اعتبار سنجی فرم*/
            var isValid = menu.validate(".edit-form");
            if (!isValid) return false;


            /*رندر کردن پارامترها*/
            menu.parameters.render();


            /*جمع آوری دیتای فرم*/
            var data = $(".edit-form").serialize();

            /*ثبت فرم با اجکس*/
            $.post(targetUrl,
                data,
                function (res) {
                    if (res.status) {
                        modal.close();
                        menu.refreshList();
                        $(".edit-btn,.delete-btn").addClass("hide");
                        $(".create-btn").html("افزودن منو جدید");
                    }
                    else {
                        $("#modal-form .edit-error").html(res.message);
                        $("#base-modal").scrollTop(0);
                    }
                }).fail(function () {
                    $("#modal-form .edit-error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                    $("#base-modal").scrollTop(0);
                });
            return false;
        }
    },




    // حذف منو
    delete: {
        loadForm: function () {
            if (!menu.selectedItem)
                showNotification("لطفا ابتدا منو مورد نظر را انتخاب نمایید !");

            else if (menu.selectedItem.nodes != null && menu.selectedItem.nodes.length > 0)
                showNotification('لطفا ابتدا زیر مجموعه های این منو را حذف کنید.');

            else
                swal({
                    title: 'آیا مطمئنید ؟',
                    text: "بعد از حذف اطلاعات این منو قابل بازگشت نیست.",
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonClass: 'btn btn-danger',
                    cancelButtonClass: 'btn btn-default',
                    confirmButtonText: 'بله ، حذف شود !',
                    cancelButtonText: 'لغو',
                    buttonsStyling: false
                }).then(function (isConfirm) {
                    if (isConfirm)
                        menu.delete.confirm();
                });
        },

        confirm: function () {
            $.post("/AuthSystem/" + controller + "/Delete/" + menu.selectedItem.id,
                function (res) {
                    if (res.status) {
                        menu.refreshList();
                        $(".edit-btn,.delete-btn").addClass("hide");
                        $(".create-btn").html("افزودن منو جدید");
                        swal({
                            title: 'حذف شد !',
                            text: 'منو مورد نظر با موفقیت حذف شد',
                            type: 'success',
                            confirmButtonClass: "btn btn-success",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    }
                    else {
                        swal({
                            title: 'حذف نشد !',
                            text: res.message,
                            type: 'error',
                            confirmButtonClass: "btn btn-danger",
                            confirmButtonText: "باشه",
                            buttonsStyling: false
                        });
                    }
                }).fail(function () {
                    swal({
                        title: 'حذف نشد !',
                        text: 'لطفا ابتدا زیر مجموعه های این منو را حذف کنید.',
                        type: 'error',
                        confirmButtonClass: "btn btn-danger",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                });

        }

    },



    // اعتبار سنجی
    validate: function (form) {
        if ($(form).find('.HasLink').is(':checked')) {
            if (!$(form).find('#Controller').val()) {
                $("#modal-form .error").html('کنترلر را وارد کنید!');
                $("#base-modal").scrollTop(0);
                return false;
            }
            if (!$(form).find('#Action').val()) {
                $("#modal-form .erro").html('اکشن را وارد کنید!');
                $("#base-modal").scrollTop(0);
                return false;
            }
        }
        return true;
    },



    // پارامتر ها
    parameters: {

        // آیتم افزودن پارامتر جدید
        item: function (index, key, value) {
            var keyStr = "";
            var valStr = "";
            if (key)
                keyStr = ' value="' + key + '"';
            if (value)
                valStr = ' value="' + value + '"';

            var tmp = '<div class="col-md-12 parameter-item" rel="' + index + '">'
                + '<div class="form-group col-md-4">'
                + '<label class="control-label text-right col-md-12">کلید</label>'
                + '<div class="col-md-12">'
                + '<input name="key' + index + '" id="key' + index + '" class="form-control" ' + keyStr + ' />'
                + '</div>'
                + '</div>'
                + '<div class="form-group col-md-4">'
                + '<label class="control-label text-right col-md-12">مقدار</label>'
                + '<div class="col-md-12">'
                + '<input name="value' + index + '" id="value' + index + '" class="form-control" ' + valStr + '/>'
                + '</div>'
                + '</div>'
                + '<div class="form-group col-md-4">'
                + '<a class="btn btn-simple btn-danger btn-icon delete" onclick="menu.parameters.delete(this)" data-toggle="tooltip" title = "حذف پارامتر" >'
                + '<i class="material-icons text-danger">delete</i>'
                + '</a>'
                + '</div >'
                + '</div>';

            return tmp;
        },



        // افزودن پارامتر جدید
        add: function () {
            var index = 1;
            if ($('.parameters-section .parameter-item:last-of-type').length > 0) {
                var rel = $('.parameters-section .parameter-item:last-of-type').attr('rel');
                var index = parseInt(rel) + 1;
            }
            $('.parameters-section .card-body').append(menu.parameters.item(index));
            $('[data-toggle="tooltip"]').tooltip();
        },


        // حذف پارامتر
        delete: function (el) {
            $(el).parents('.parameter-item').remove();
        },


        // جمع آوری پارامترها و تبدیل به رشته
        render: function () {
            var parameters = {};
            var parameterItems = $('.parameters-section .parameter-item');
            $.each(parameterItems, function () {
                var index = $(this).attr('rel');
                var key = $(this).find('#key' + index).val().trim();
                var val = $(this).find('#value' + index).val().trim();
                if (key && val)
                    parameters[key] = val;
            });

            if (!$.isEmptyObject(parameters))
                $('#Parameters').val(JSON.stringify(parameters));
            else
                $('#Parameters').val('');
        },



        // تبدیل رشته به پارامتر ها هنگام ویرایش
        expand: function () {
            var parameters = $("#Parameters").val();
            if (!parameters) {
                menu.parameters.add();
                return;
            }

            var obj = JSON.parse(parameters);
            if (!obj) {
                menu.parameters.add();
                return;
            }

            var index = 1;
            Object.keys(obj).forEach(function (key) {
                var value = obj[key];
                $('.parameters-section .card-body').append(menu.parameters.item(index, key, value));
                $('[data-toggle="tooltip"]').tooltip();
                index++;
            });
        },


    }

}



menu.refreshList();




//باز کردن یا بستن بخش مربوط به لینک
var toggleSlide = function (el, target) {
    if (!$(el).is(":checked")) {
        $(target).slideUp();
    }
    else {
        $(target).slideDown();
    }
}














