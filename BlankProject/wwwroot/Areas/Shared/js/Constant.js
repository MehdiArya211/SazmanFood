/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Admin/Dashboard" });
breadcrumb.push({ title: "اطلاعات تماس", link: "#" });




// عملیات مربوط به مدیریت اطلاعات تماس
var _area = "shared";
var _controller = "Constants";
var entity = {

    // لیست اطلاعات تماس
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
                    [10, 25, 50],
                    [10, 25, 50]
                ],
                responsive: true,
                "ajax": {
                    "url": "/" + _area + "/" + _controller + "/GetList",
                    "type": "POST",
                    "dataType": "json"
                },
                "columns": [
                    { "data": "id", "name": "شناسه" },
                    { "data": "title", "name": "عنوان" },
                    { "data": "value", "name": "مقدار" },
                    {
                        data: null,
                        //className: "text-left",
                        render: function (data, type, row) {
                            var btns = "<a onclick='entity.edit.loadForm(" + data.id + ")' class='btn btn-simple btn-info btn-icon' title='ویرایش' data-toggle='tooltip'><i class='material-icons'>edit</i></a>"

                            return btns;
                        }
                    }
                ],
                "serverSide": "true",
                "order": [0, "desc"],
                "processing": "true",
                'columnDefs': [{
                    'targets': [3], /* column index */
                    'orderable': false, /* true or false */
                }]

            });
        },



        //رفرش کردن دیتا تیبل
        reload: function () {
            entity.list.table.ajax.reload(function () {
                //$.material.init();
            }, false);
        }
    },




    form: {

        initial: function () {
            $('.selectpicker').selectpicker('refresh');
            $.material.init();
        },



    },



    // ویرایش اطلاعات تماس
    edit: {
        // لود کردن فرم ویرایش اطلاعات تماس
        loadForm: function (id) {
            $.get("/" + _area + "/" + _controller + "/LoadEditForm/" + id,
                function (res) {
                    $('#base-modal').addClass('small-modal');
                    $("#modal-form").html(res);
                    entity.form.initial();
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
            var $form = $(".edit-form");
            $form.validate();
            if (!$form.valid()) return false;

            /*جمع آوری دیتای فرم*/
            var data = $(".edit-form").serialize();

            /*ثبت فرم با اجکس*/
            $.post(targetUrl,
                data,
                function (res) {
                    if (res.status) {
                        modal.close();
                        entity.list.reload();
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
        },



    },



}



entity.list.initial();



