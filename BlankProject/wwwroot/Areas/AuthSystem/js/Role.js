/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/AuthSystem/Dashboard" });
breadcrumb.push({ title: "نقش‌ ها", link: "#" });



var test = {
    ok: function () {
        alert('ok');
    },
}


// عملیات مربوط به مدیریت نقش ها
var roles = {

    ok: function () {
        alert('ok');
    },
    // لیست نقش ها
    list: {
        // آبجکت دیتاتیبل
        table: null,


        // راه اندازی دیتاتیبل
        initial: function () {
            this.table = $('#datatables').DataTable({
                "drawCallback": function (settings) {
                    $('[data-toggle="tooltip"]').tooltip();
                },
                language: {
                    url: "/assets/datatables/fa-lang.json"
                },
                "pagingType": "full_numbers",
                "lengthMenu": [
                    [10, 25, 50, -1],
                    [10, 25, 50, "All"]
                ],
                responsive: true,
                "ajax": {
                    "url": "/AuthSystem/Roles/GetList",
                    "type": "POST",
                    "dataType": "json"
                },
                "columns": [
                    { "data": "id", "name": "شناسه" },
                    { "data": "title", "name": "عنوان" },
                    { "data": "description", "name": "توضیحات" },
                    {
                        data: "isEnabled",
                        render: function (data, type, row) {
                            var checked = '';
                            if (data) checked = 'checked';
                            return '<label class="toggle for-isEnabled"><input onchange="roles.toggleEnable(this, ' + row.id + ')" type="checkbox" ' + checked + '><span class="slider"></span><span class="labels" data-on="فعال" data-off="غیرفعال"></span></label>';

                            //if (data)
                            //    return "<span class='btn btn-sm btn-success' onclick='roles.toggleEnable(this, " + row.id + ")'>فعال</span>"
                            //else
                            //    return "<span class='btn btn-sm btn-danger' onclick='roles.toggleEnable(this, " + row.id + ")'>غیرفعال</span>"
                        }
                    },
                    {
                        data: null,
                        className: "text-left",
                        render: function (data, type, row) {
                            var btns = "<a onclick='roles.edit.loadForm(" + data.id + ")' class='btn btn-simple btn-info btn-icon' title='ویرایش' data-toggle='tooltip'><i class='material-icons'>edit</i></a>"
                                + "<a onclick='roles.access.load(" + data.id + ",\"" + data.title + "\")' class='user-application-btn btn btn-simple btn-success btn-icon' title='دسترسی ها' data-toggle='tooltip'><i class='material-icons'>apps</i></a>"
                                + "<a onclick='roles.delete.loadForm(" + data.id + ")' class='btn btn-simple btn-danger btn-icon' title='حذف' data-toggle='tooltip'><i class='material-icons'>close</i></a>";

                            return btns;
                        }
                    }
                ],
                "serverSide": "true",
                "order": [0, "desc"],
                "processing": "true",
                'columnDefs': [{
                    'targets': [4], /* column index */
                    'orderable': false, /* true or false */
                }]

            });
        },


        //رفرش کردن دیتا تیبل
        reload: function () {
            roles.list.table.ajax.reload(function () {
                //$.material.init();
            }, false);
        }
    },




    // افزودن نقش جدید
    create: {
        // لود کردن فرم افزودن نقش جدید
        loadForm: function () {
            $.get("/AuthSystem/roles/LoadCreateForm",
                function (res) {
                    $("#modal-form").html(res);
                    $.material.init();
                    modal.open();

                    /*اعمال ولیدیشن به فرمی که با اجکس لود شده است*/
                    var form = $(".create-form")
                        .removeData("validator")
                        .removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse(form);
                });
        },



        // ذخیره اطلاعات نقش جدید
        save: function (e) {
            e.preventDefault();
            var targetUrl = $(".create-form").attr("action");

            /*ولیدیت کردن فرم*/
            var $form = $("form");
            $form.validate();
            if (!$form.valid())
                return false;

            /*جمع آوری دیتای فرم*/
            var data = $(".create-form").serialize();

            /*ثبت فرم با اجکس*/
            $.post(targetUrl,
                data,
                function (res) {
                    if (res.status) {
                        roles.list.reload();
                        modal.close();
                    }
                    else {
                        $(".create-form .error").html(res.message);
                        setScrollPosition();
                    }
                }).fail(function () {
                    $(".create-form .error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                    setScrollPosition();
                });
            return false;
        }
    },



    // ویرایش نقش
    edit: {
        // لود کردن فرم ویرایش نقش
        loadForm: function (id) {
            $.get("/AuthSystem/roles/LoadEditForm/" + id,
                function (res) {
                    $("#modal-form").html(res);
                    $.material.init();
                    modal.open();

                    /*اعمال ولیدیشن به فرمی که با اجکس لود شده است*/
                    var form = $(".edit-form")
                        .removeData("validator")
                        .removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse(form);
                });
        },


        // ذخیره اطلاعات ویرایش شده
        save: function (e) {
            e.preventDefault();
            var targetUrl = $(".edit-form").attr("action");

            /*ولیدیت کردن فرم*/
            var $form = $("form");
            $form.validate();
            if (!$form.valid())
                return false;

            /*جمع آوری دیتای فرم*/
            var data = $(".edit-form").serialize();

            /*ثبت فرم با اجکس*/
            $.post(targetUrl,
                data,
                function (res) {
                    if (res.status) {
                        roles.list.reload();
                        modal.close();
                    }
                    else {
                        $(".error").html(res.message);
                        setScrollPosition();
                    }
                }).fail(function () {
                    $(".error").html("ذخیره اطلاعات با خطا همراه بوده است. مجددا اقدام کنید.");
                    setScrollPosition();
                });
            return false;
        },



        // گرفتن دسترسی های نقش و ست کردن در فرم
        loadAccess: function () {
            var id = $('#Id').val();
            $.get('/AuthSystem/Roles/LoadAccess/' + id, function (res) {
                if (!res)
                    return;

                // تعیین مقادیر دسترسی ها
                for (var i = 0; i < res.length; i++) {
                    var item = $('select.access-select[rel=' + res[i].menuId + ']');
                    var val = $(item).val();
                    if (val == null)
                        val = [];
                    val.push(res[i].access.toString());
                    $(item).val(val);
                }

                // تنظیم رنگ دسترسی ها
                $(".selectpicker").selectpicker('refresh');
                var selects = $('select.access-select');
                for (var i = 0; i < selects.length; i++)
                    menuAccess.setClass($(selects[i]));
            }).fail(function () { alert('بارگذاری دسترسی ها با خطا همراه بوده است!') })
        }
    },




    // حذف نقش
    delete: {
        loadForm: function (id) {
            if (!id)
                showNotification("لطفا ابتدا نقش مورد نظر را انتخاب نمایید !");
            else
                swal({
                    title: 'آیا مطمئنید ؟',
                    text: "تمامی دسترسی های این نقش حذف خواهد شد!",
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonClass: 'btn btn-danger',
                    cancelButtonClass: 'btn btn-default',
                    confirmButtonText: 'بله ، حذف شود !',
                    cancelButtonText: 'لغو',
                    buttonsStyling: false
                }).then(function (isConfirm) {
                    if (isConfirm)
                        roles.delete.confirm(id);
                });
        },

        confirm: function (id) {
            $.post("/AuthSystem/Roles/Delete/" + id,
                function (res) {
                    if (res.status) {
                        roles.list.reload();
                        swal({
                            title: 'حذف شد !',
                            text: 'نقش مورد نظر با موفقیت حذف شد',
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
                        text: 'حذف نقش با خطا همراه بوده است. مجددا اقدام کنید!',
                        type: 'error',
                        confirmButtonClass: "btn btn-danger",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                });

        }

    },



    // دسترسی های نقش
    access: {

        // بارگذاری لیست منو ها به همراه دسترسی های نقش
        load: function (id, title) {
            $('.role-title').html(title);
            $('#RoleId').val(id);
            $.get('/AuthSystem/roles/LoadAccess/' + id, function (res) {
                var ids = [];
                if (res && res.length > 0)
                    ids = Array.prototype.map.call(res, function (item) { return item.menuId; });
                menu.accessItems = ids;
                menu.load();
            }).fail(function () { alert('بارگذاری دسترسی ها با خطا همراه بوده است!') })
        },


        // گرفتن تایید برای تغییر دسترسی به کاربرانی که این نقش را دارند
        checkApplyAccess: function () {
            swal({
                title: 'تغییر دسترسی ها',
                text: "تغییر در دسترسی های این نقش منجر به تغییر دسترسی تمامی کاربرانی که این نقش را دارند می شود!",
                type: 'warning',
                showCancelButton: true,
                confirmButtonClass: 'btn btn-success',
                cancelButtonClass: 'btn btn-danger',
                confirmButtonText: 'بله، دسترسی کاربران بروز شود',
                cancelButtonText: 'لغو',
                buttonsStyling: false
            }).then(function (isConfirm) {
                if (isConfirm)
                    roles.access.save();
            }).catch(function () {
                swal.noop;
            });

        },


        //ذخیره دسترسی ها
        save: function () {
            var data = {
                RoleId: $('#RoleId').val(),
                MenuIds: menu.treeView.getSelectedIds()
            }
            console.log(data);

            $.post('/AuthSystem/Roles/SaveAccess', data, function (res) {
                if (!res.status)
                    showNotification(res.message, 'danger');
                else
                    modal.close('#menu-modal');
            }).fail(function () { alert('ثبت دسترسی ها با خطا همراه بوده است!') })
        }

    },




    //تغییر وضعیت فعال بودن یا نبودن نقش در صفحه ایندکس
    toggleEnable: function (el, id) {
        $.post('/AuthSystem/Roles/ToggleEnable/' + id, function (res) {
            if (!res.status)
                alert('تغییر وضعیت نقش با خطا همراه بوده است!');
            if (res.isEnabled == true)
                $(el).removeClass('btn-danger').addClass('btn-success').text('فعال');
            else
                $(el).removeClass('btn-success').addClass('btn-danger').text('غیرفعال');
        })
    },
}



roles.list.initial();





//بارگذاری منو ها
$tree = $('#treeview');
var menu = {
    // آیا منو ها بارگذاری شده اند؟
    isLoaded: false,

    // منوهایی که مورد دسترسی است و باید انتخاب شوند.
    accessItems: null,

    // لود کردن منو ها و تبدیل به تری ویو
    load: function () {
        if (this.isLoaded) {
            menu.treeView.reset();
            menu.treeView.initialSelectedNodes();
            modal.open('#menu-modal');
            return;
        }
        else {
            $.get('/AuthSystem/roles/LoadMenus/', function (data) {
                menu.isLoaded = true;
                menu.treeView.initial(data);
                modal.open('#menu-modal');
            }).fail(function () { alert('بارگذاری منو ها با خطا همراه بوده است!') })
        }
    },



    // تری ویو
    treeView: {
        // تری ویو راه اندازی شده است؟
        InitializeFinished: false,
        
        // منو انتخاب شده
        selectedItems: [],

        // راه اندازی تری ویو
        initial: function (data) {
            menu.treeView.selectedItems = [];

            var model = menu.treeView.reshapeData(data, null);

            if (!model || model.length == 0) {
                $tree.html("<div class='text-center'>منویی برای نمایش یافت نشد.</div>");
                return;
            }

            $tree.treeview({
                data: model,
                levels: 0,
                multiSelect: true,
                onNodeSelected: function (event, node) {
                    menu.treeView.selectedItems = $tree.treeview('getSelected');
                    menu.treeView.chip.refresh();
                    menu.treeView.selectParent(node);

                },
                onNodeUnselected: function (event, node) {
                    menu.treeView.selectedItems = $tree.treeview('getSelected');
                    menu.treeView.chip.refresh();
                    menu.treeView.unselectChild(node);
                }
            });

            this.initialSelectedNodes();

            $('.treeview-holder').mCustomScrollbar('destroy');
            $('.treeview-holder').mCustomScrollbar({ theme: "minimal" });
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
                this.text = icon + '<span class="node-title">' + this.text + '</span>';
            });
            return model;
        },



        // انتخاب نودهای از قبل انتخاب شده
        initialSelectedNodes: function () {
            var ids = menu.accessItems;
            if (!ids || ids.length == 0) {
                menu.treeView.InitializeFinished = true;
                return;
            }

            var UnselectedNode = $tree.treeview('getUnselected');
            for (var i = 0; i < ids.length; i++) {
                for (var j = 0; j < UnselectedNode.length; j++) {
                    var node = UnselectedNode[j];
                    if (node.id == ids[i]) {
                        $tree.treeview('selectNode', node);
                        while (node.parentId != null) {
                            var parent = $tree.treeview('getParent', node);
                            $tree.treeview('expandNode', parent);
                            node = parent;
                        }
                    }
                }
            }
            this.InitializeFinished = true;
        },



        // چیپ های مرتبط با منو انتخاب شده در تری ویو
        chip: {
            //ساختن چیپ ها با توجه به ایتم های انتخاب شده
            refresh: function (el, id) {
                $(".pmd-chip").remove();
                var SelectedNode = $tree.treeview('getSelected');
                for (var i = 0; i < SelectedNode.length; i++)
                    $("#chip-holder").append(menu.treeView.chip.create(SelectedNode[i]));
            },


            //ساختن یک چیپ
            create: function (node) {
                return '<div class="pmd-chip">'
                    + node.title
                    + '<a class="pmd-chip-action" onclick="menu.treeView.chip.delete(this,' + node.id + ');" data-id="' + node.id + '"><i class="material-icons">close</i></a>'
                    + '</div>'
            },


            //حذف چیپ و برداشتن انتخاب نُود مورد نظر
            delete: function (el, id) {
                var SelectedNode = $tree.treeview('getSelected');
                for (var i = 0; i < SelectedNode.length; i++)
                    if (SelectedNode[i].id == id)
                        $tree.treeview('unselectNode', SelectedNode[i]);
            }

        },



        // گرفتن ایدی منوهای انتخاب شده
        getSelectedIds: function () {
            if (!menu.treeView.InitializeFinished)
                return;
            var ids = Array.prototype.map.call(menu.treeView.selectedItems, function (item) { return item.id; });
            return ids
        },


        // ریست کردن تری ویو و پاک کردن مقادیر انتخاب شده
        reset: function () {
            this.InitializeFinished = false;
            this.selectedItems = [];
            this.unselectAll();
            $tree.treeview('collapseAll', { silent: true });
            $('#chip-holder').empty();
        },


        // برداشتن انتخاب همه نود ها
        unselectAll: function () {
            var selectedNodes = $tree.treeview('getSelected');
            for (var i = 0; i < selectedNodes.length; i++) {
                var node = selectedNodes[i];
                $tree.treeview('unselectNode', node);
            }
        },


        // انتخاب والدها با انتخاب فرزند
        selectParent: function (node, isSilent) {
            if (!isSilent)
                isSilent = false;
            if (node.parentId) {
                var parent = $tree.treeview('getParent', node);
                $tree.treeview('selectNode', [parent.nodeId, { silent: isSilent }] );
            }
        },


        // برداشتن انتخاب فرزندان با آنسلکت شدن والد 
        unselectChild: function (node, isSilent) {
            if (!isSilent)
                isSilent = false;
            if (node.nodes && node.nodes.length > 0) {
                for (var i = 0; i < node.nodes.length; i++) {
                    var child = node.nodes[i];
                    if (child.state.selected) {
                        $tree.treeview('unselectNode', [child.nodeId, { silent: isSilent }]);
                    }
                }
            }
        }


    }



}

















