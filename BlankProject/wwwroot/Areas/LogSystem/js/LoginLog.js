/*breadcrumb*/
var url = window.location.href.toLowerCase();

var breadcrumb = [];
breadcrumb.push({ title: "پنل ادمین", link: "/Admin/Dashboard" });
breadcrumb.push({ title: "لاگ ورود و خروج کاربران", link: "#" });




// عملیات مربوط به مدیریت لاگ لاگین ها
var _area = "logsystem";
var _controller = "loginlogs";
var entity = {

    // لیست لاگ لاگین ها
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
                    "dataType": "json",
                    "data": function (d) {
                        return $.extend({}, d, filter.collect());
                    },
                },
                "columns": [
                    { "data": "userName", "name": "نام کاربر" },
                    { "data": "userIp", "name": "آی پی کاربر" },
                    { "data": "actionName", "name": "نام فعالیت" },
                    {
                        data: "createDate",
                        render: function (data, type, row) {
                            return row.createDateFa;
                        }
                    },
                    {
                        data: "isSuccess",
                        render: function (data, type, row) {
                            var st = '<a class="text-success">موفق</a>';
                            if (!data) st = '<a class="text-danger">ناموفق</a>';
                            return st;
                        }
                    },
                    { "data": "description", "name": "توضیحات" },
                ],
                createdRow: (row, data, dataIndex, cells) => {
                    if (data.isExp)
                        $(row).addClass('bg-danger');
                },
                "serverSide": "true",
                "order": [3, "desc"],
                "processing": "true",
                'columnDefs': [{
                    'targets': [5], /* column index */
                    'orderable': false, /* true or false */
                }]

            });
        },



        //رفرش کردن دیتا تیبل
        reload: function (resetPage) {
            if (resetPage != true)
                resetPage = false;

            entity.list.table.ajax.reload(function () {
                //$.material.init();
            }, resetPage);
        }
    },



    // خروجی اکسل
    excelExport: function () {
        var data = filter.collect();
        var queryString = $.param(data);

        var url = "/" + _area + "/" + _controller + '/DownloadExcel';
        if (queryString.length > 0)
            url += "?" + queryString;
        window.open(url);
    },


}



entity.list.initial();






//==============================================
// فیلتر جستجو درخواست ها
//==============================================
var filter = {
    initial: _ => {
        filter.startDate.initial();
        filter.endDate.initial();
        $('.selectpicker').selectpicker('refresh');
    },



    //جمع آوری داده فیلتر
    collect: _ => {
        var data = {
            CreateStartDate: $("input[name=FilterCreateStartDate]").val(),
            CreateEndDate: $("input[name=FilterCreateEndDate]").val(),
            UserName: $("#FilterUserName").val(),
            IsSuccess: $("#FilterIsSuccess").val(),
        }
        
        return data;
    },


    // تاریخ شروع
    startDate: {

        // آبجکت vue
        obj: null,

        //راه اندازی تقویم تاریخ شروع
        initial: function () {
            if ($('#VueCreateStartDate').length == 0)
                return;
            var sdate = ''; //moment()/*.subtract(7, 'd')*/.format('jYYYY/jMM/jDD');
            this.obj = new Vue({
                el: '#VueCreateStartDate',
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
            if ($('#VueCreateEndDate').length == 0)
                return;
            var edate = ''; // moment().format('jYYYY/jMM/jDD');
            this.obj = new Vue({
                el: '#VueCreateEndDate',
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
            $("input[name=FilterCreateStartDate]").val('').prev('input').val('');
            $("input[name=FilterCreateEndDate]").val('').prev('input').val('');
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


filter.initial();