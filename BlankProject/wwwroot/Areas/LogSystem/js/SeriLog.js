/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Admin/Dashboard" });




// عملیات مربوط به مدیریت پرسنل
// آیا سابمیت آغاز شده است؟
var startSubmition = false;
var user = {

    // لیست کاربر ها
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
                    "url": "/LogSystem/SeriLog/GetList",
                    "type": "POST",
                    "dataType": "json",
                    "data": function (d) {
                        return $.extend({}, d, filter.collect());
                    },
                },
                "columns": [
                    { "data": "id", "name": "شناسه" },
                    { "data": "message", "name": "نام کامل" },
                    { "data": "level", "name": "نام کاربری" },
                    { "data": "timeStamp", "name": "تلفن" },
                    { "data": "exception", "name": "تلفن" },
                    { "data": "properties", "name": "تلفن" },
                    { "data": "logEvent", "name": "تلفن" },

                ],
                "serverSide": "true",
                "order": [0, "desc"],
                "processing": "true",
                'columnDefs': [{
                    'targets': [1, 2, 3, 4, 5], /* column index */
                    'orderable': false, /* true or false */
                }]

            });
        },


        //رفرش کردن دیتا تیبل
        reload: function () {
            user.list.table.ajax.reload(function () {
                //$.material.init();
            }, false);
        }
    },


    // آماده سازی فرم ها
    form: {
        initial: function () {
            $('#base-modal').removeClass('small-modal');
            $('.selectpicker').selectpicker('refresh');
            $.material.init();
        },

    },



    //تغییر وضعیت فعال بودن یا نبودن کاربر در صفحه ایندکس
    toggleEnable: function (el, id) {
        $.post('/AuthSystem/users/ToggleEnable/' + id, function (res) {
            if (!res.status)
                alert('تغییر وضعیت کاربر با خطا همراه بوده است!');
            //if (res.isEnabled == true)
            //    $(el).removeClass('btn-danger').addClass('btn-success').text('فعال');
            //else
            //    $(el).removeClass('btn-success').addClass('btn-danger').text('غیرفعال');
        })
    }

}

//==============================================
// فیلتر جستجو درخواست ها
//==============================================
var filter = {
    initial: _ => {
        filter.startDate.initial();
        filter.endDate.initial();
        filter.role.initial();

        $('.selectpicker').selectpicker('refresh');
    },



    //جمع آوری داده فیلتر
    collect: _ => {
        var data = {
            CreateStartDate: $("input[name=FilterStartDate]").val(),
            CreateEndDate: $("input[name=FilterEndDate]").val(),
            Level: $("#Level").val(),
            //Username: $("#FilterUsername").val(),
            //Mobile: $("#FilterMobile").val(),
            //RoleId: $("#FilterRoleId").val(),
            IsEnabled: $("#FilterIsEnabled").val(),
        }

        return data;
    },


    // تاریخ شروع
    startDate: {

        // آبجکت vue
        obj: null,

        //راه اندازی تقویم تاریخ شروع
        initial: function () {
            if ($('#VueStartDate').length == 0)
                return;
            var sdate = ''; //moment()/*.subtract(7, 'd')*/.format('jYYYY/jMM/jDD');
            this.obj = new Vue({
                el: '#VueStartDate',
                data: {
                    date: sdate
                },
                components: {
                    DatePicker: VuePersianDatetimePicker
                },
                methods: {
                    onClose: function (x) {
                        filter.checkDates(filter.startDate, filter.endDate);
                    }
                }
            });
        }
    },



    // تاریخ پایان
    endDate: {
        // آبجکت vue
        obj: null,

        //راه اندازی تقویم تاریخ پایان
        initial: function () {
            if ($('#VueEndDate').length == 0)
                return;
            var edate = ''; // moment().format('jYYYY/jMM/jDD');
            this.obj = new Vue({
                el: '#VueEndDate',
                data: {
                    date: edate
                },
                components: {
                    DatePicker: VuePersianDatetimePicker
                },
                methods: {
                    onClose: function (x) {
                        filter.checkDates(filter.startDate, filter.endDate);
                    }
                }
            });
        }
    },



    // بررسی تاریخ شروع و پایان
    checkDates: function (start, end) {
        var sdate = start.obj.date;
        var edate = end.obj.date;
        if (sdate && edate && sdate > edate) {
            showNotification("تاریخ شروع نمیتواند بعد از تاریخ پایان باشد!", 'danger');
            $("input[name=FilterStartDate]").val('').prev('input').val('');
            $("input[name=FilterEndDate]").val('').prev('input').val('');
        }
    },



    //نقش ها
    role: {
        initial: _ => {
            if ($('#FilterRoleId').length == 0)
                return;

            var opt = "<option value='' selected>همه</option>"
            $('#FilterRoleId').prepend(opt);
        }
    },



    // خالی کردن سلکتایز از ایتم انتخاب شده
    clearSelectize: function (el) {
        var $select = $(el).selectize();
        var control = $select[0].selectize;
        control.clear();
        control.renderCache = {};
        control.clearOptions();
        control.refreshOptions(true);
    },



}




if (controller == 'profile' && action == 'edit')
    breadcrumb.push({ title: "ویرایش پروفایل", link: "#" });
else if (controller == 'profile' && action == 'changepassword')
    breadcrumb.push({ title: "تغییر کلمه عبور", link: "#" });
else if (controller == 'profile' && action == 'loginlog') {
    breadcrumb.push({ title: "لاگ ورود و خروج", link: "#" });
    user.profileLoginLog.list.initial();
    user.profileLoginLog.filter.initial();
}
else {
    breadcrumb.push({ title: "کاربران", link: "#" });
    user.list.initial();
    filter.initial();
}


// تغییر تصویر کپچا
var changeCaptcha = () => {
    var d = new Date();
    $("#imgcpatcha").attr("src", "/Captcha/CaptchaImage?" + d.getTime());
    $('#captcha').val('');
}