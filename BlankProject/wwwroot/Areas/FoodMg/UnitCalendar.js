/* breadcrumb */
var url = window.location.href.toLowerCase();

var breadcrumb = [];

breadcrumb.push({
    title: "پنل ادمین",
    link: "/Admin/Dashboard"
});

breadcrumb.push({
    title: "تقویم یگان",
    link: "#"
});


var unitCalendarSubmission = false;


var unitCalendar = {

    urls: {
        getList:
            "/FoodMang/UnitCalendars/GetList",

        loadCreateForm:
            "/FoodMang/UnitCalendars/LoadCreateForm",

        create:
            "/FoodMang/UnitCalendars/Create",

        loadEditForm:
            "/FoodMang/UnitCalendars/LoadEditForm/",

        edit:
            "/FoodMang/UnitCalendars/Edit",

        delete:
            "/FoodMang/UnitCalendars/Delete"
    },


    /* =========================================
       لیست
       ========================================= */
    list: {

        table: null,

        initial: function () {

            this.table =
                $("#unit-calendar-table")
                    .DataTable({

                        drawCallback: function () {
                            $('[data-toggle="tooltip"]')
                                .tooltip();
                        },

                        language: {
                            url:
                                "/assets/datatables/fa-lang.json"
                        },

                        pagingType:
                            "full_numbers",

                        lengthMenu: [
                            [10, 25, 50, -1],
                            [10, 25, 50, "همه"]
                        ],

                        responsive:
                            true,

                        processing:
                            true,

                        serverSide:
                            false,

                        ajax: {

                            url:
                                unitCalendar.urls.getList,

                            type:
                                "POST",

                            dataType:
                                "json",

                            data: function (data) {

                                return $.extend(
                                    {},
                                    data,
                                    unitCalendar.filter.collect()
                                );
                            }
                        },

                        columns: [

                            {
                                data: "id",
                                name: "شناسه"
                            },

                            {
                                data: "orgTitle",
                                name: "یگان",

                                render: function (data) {
                                    return data || "-";
                                }
                            },

                            {
                                data: "calendarDate",

                                render: function (
                                    data,
                                    type,
                                    row) {

                                    return row.calendarDateFa;
                                }
                            },

                            {
                                data: "dayType",

                                render: function (
                                    data,
                                    type,
                                    row) {

                                    var cssClass =
                                        "calendar-day-normal";

                                    if (data === 2) {
                                        cssClass =
                                            "calendar-day-holiday";
                                    }
                                    else if (data === 3) {
                                        cssClass =
                                            "calendar-day-half-holiday";
                                    }

                                    return (
                                        '<span class="' +
                                        cssClass +
                                        '">' +
                                        row.dayTypeTitle +
                                        "</span>"
                                    );
                                }
                            },

                            {
                                data: "description",

                                render: function (data) {
                                    return data || "-";
                                }
                            },

                            {
                                data: null,

                                className:
                                    "text-left",

                                orderable:
                                    false,

                                render: function (
                                    data,
                                    type,
                                    row) {

                                    var buttons = "";

                                    buttons +=
                                        "<a " +
                                        "onclick='unitCalendar.edit.loadForm(" +
                                        row.id +
                                        ")' " +
                                        "class='btn btn-simple btn-info btn-icon' " +
                                        "title='ویرایش' " +
                                        "data-toggle='tooltip'>" +
                                        "<i class='material-icons'>edit</i>" +
                                        "</a>";

                                    buttons +=
                                        "<a " +
                                        "onclick='unitCalendar.remove.load(" +
                                        row.id +
                                        ")' " +
                                        "class='btn btn-simple btn-danger btn-icon' " +
                                        "title='حذف' " +
                                        "data-toggle='tooltip'>" +
                                        "<i class='material-icons'>close</i>" +
                                        "</a>";

                                    return buttons;
                                }
                            }
                        ],

                        order: [
                            [2, "desc"]
                        ],

                        columnDefs: [
                            {
                                targets: [1, 3, 4, 5],
                                orderable: false
                            }
                        ]
                    });
        },

        reload: function () {

            if (!unitCalendar.list.table)
                return;

            unitCalendar.list.table
                .ajax
                .reload(
                    null,
                    false
                );
        }
    },


    /* =========================================
       آماده‌سازی فرم Modal
       ========================================= */
    form: {

        datePickerObject: null,

        initial: function () {

            $(".selectpicker")
                .selectpicker("refresh");

            $.material.init();

            unitCalendar.form
                .initialDatePicker();

            /*
             * اعمال Validation روی فرم Ajax
             */
            var form =
                $("#modal-form form")
                    .removeData("validator")
                    .removeData(
                        "unobtrusiveValidation"
                    );

            $.validator
                .unobtrusive
                .parse(form);
        },

        initialDatePicker: function () {

            var element =
                $("#VueUnitCalendarDate");

            if (element.length === 0)
                return;

            /*
             * در صورت وجود Vue قبلی،
             * قبل از ساخت نمونه جدید Destroy می‌شود.
             */
            if (this.datePickerObject) {

                this.datePickerObject
                    .$destroy();

                this.datePickerObject =
                    null;
            }

            var gregorianDate =
                element.attr(
                    "data-initial-date"
                );

            var persianDate = "";

            if (gregorianDate) {

                persianDate =
                    moment(
                        gregorianDate,
                        "YYYY-MM-DD"
                    ).format(
                        "jYYYY/jMM/jDD"
                    );
            }

            this.datePickerObject =
                new Vue({

                    el:
                        "#VueUnitCalendarDate",

                    data: {
                        date:
                            persianDate
                    },

                    components: {
                        DatePicker:
                            VuePersianDatetimePicker
                    },

                    methods: {

                        onClose: function () {

                            /*
                             * DatePicker با alt-name
                             * مقدار میلادی را در Input مخفی
                             * CalendarDate قرار می‌دهد.
                             */
                        }
                    }
                });
        }
    },


    /* =========================================
       ثبت
       ========================================= */
    create: {

        loadForm: function () {

            $.get(
                unitCalendar.urls
                    .loadCreateForm,

                function (result) {

                    $("#modal-form")
                        .html(result);

                    unitCalendar.form
                        .initial();

                    modal.open();
                }
            );
        },

        save: function (event) {

            event.preventDefault();

            if (unitCalendarSubmission)
                return false;

            var form =
                $(".create-form");

            form.validate();

            if (!form.valid())
                return false;

            var calendarDate =
                form.find(
                    "input[name='CalendarDate']"
                ).val();

            if (!calendarDate) {

                form.find(".error")
                    .html(
                        "تاریخ الزامی است."
                    );

                return false;
            }

            unitCalendarSubmission =
                true;

            $.post(
                unitCalendar.urls.create,
                form.serialize(),

                function (result) {

                    unitCalendarSubmission =
                        false;

                    if (result.status) {

                        unitCalendar.list
                            .reload();

                        modal.close();

                        swal({
                            title:
                                "ثبت شد",

                            text:
                                result.message,

                            type:
                                "success",

                            confirmButtonClass:
                                "btn btn-success",

                            confirmButtonText:
                                "باشه",

                            buttonsStyling:
                                false
                        });
                    }
                    else {

                        form.find(".error")
                            .html(
                                result.message
                            );

                        setScrollPosition();
                    }
                }
            ).fail(function () {

                unitCalendarSubmission =
                    false;

                form.find(".error")
                    .html(
                        "ثبت اطلاعات با خطا همراه بوده است."
                    );

                setScrollPosition();
            });

            return false;
        }
    },


    /* =========================================
       ویرایش
       ========================================= */
    edit: {

        loadForm: function (id) {

            $.get(
                unitCalendar.urls
                    .loadEditForm +
                id,

                function (result) {

                    $("#modal-form")
                        .html(result);

                    unitCalendar.form
                        .initial();

                    modal.open();
                }
            );
        },

        save: function (event) {

            event.preventDefault();

            if (unitCalendarSubmission)
                return false;

            var form =
                $(".edit-form");

            form.validate();

            if (!form.valid())
                return false;

            var calendarDate =
                form.find(
                    "input[name='CalendarDate']"
                ).val();

            if (!calendarDate) {

                form.find(".error")
                    .html(
                        "تاریخ الزامی است."
                    );

                return false;
            }

            unitCalendarSubmission =
                true;

            $.post(
                unitCalendar.urls.edit,
                form.serialize(),

                function (result) {

                    unitCalendarSubmission =
                        false;

                    if (result.status) {

                        unitCalendar.list
                            .reload();

                        modal.close();

                        swal({
                            title:
                                "ویرایش شد",

                            text:
                                result.message,

                            type:
                                "success",

                            confirmButtonClass:
                                "btn btn-success",

                            confirmButtonText:
                                "باشه",

                            buttonsStyling:
                                false
                        });
                    }
                    else {

                        form.find(".error")
                            .html(
                                result.message
                            );

                        setScrollPosition();
                    }
                }
            ).fail(function () {

                unitCalendarSubmission =
                    false;

                form.find(".error")
                    .html(
                        "ویرایش اطلاعات با خطا همراه بوده است."
                    );

                setScrollPosition();
            });

            return false;
        }
    },


    /* =========================================
       حذف
       ========================================= */
    remove: {

        load: function (id) {

            if (!id) {

                showNotification(
                    "رکورد موردنظر مشخص نشده است.",
                    "danger"
                );

                return;
            }

            swal({

                title:
                    "آیا مطمئن هستید؟",

                text:
                    "وضعیت ثبت‌شده این روز حذف می‌شود.",

                type:
                    "warning",

                showCancelButton:
                    true,

                confirmButtonClass:
                    "btn btn-danger",

                cancelButtonClass:
                    "btn btn-default",

                confirmButtonText:
                    "بله، حذف شود",

                cancelButtonText:
                    "انصراف",

                buttonsStyling:
                    false

            }).then(function (result) {

                /*
                 * سازگاری با نسخه‌های مختلف SweetAlert
                 */
                if (result === false ||
                    result.dismiss) {
                    return;
                }

                unitCalendar.remove
                    .confirm(id);
            });
        },

        confirm: function (id) {

            var token =
                $("#operation-token-form")
                    .find(
                        "input[name='__RequestVerificationToken']"
                    )
                    .val();

            $.post(
                unitCalendar.urls.delete,
                {
                    id: id,

                    __RequestVerificationToken:
                        token
                },

                function (result) {

                    if (result.status) {

                        unitCalendar.list
                            .reload();

                        swal({

                            title:
                                "حذف شد",

                            text:
                                result.message,

                            type:
                                "success",

                            confirmButtonClass:
                                "btn btn-success",

                            confirmButtonText:
                                "باشه",

                            buttonsStyling:
                                false
                        });
                    }
                    else {

                        swal({

                            title:
                                "حذف نشد",

                            text:
                                result.message,

                            type:
                                "error",

                            confirmButtonClass:
                                "btn btn-danger",

                            confirmButtonText:
                                "باشه",

                            buttonsStyling:
                                false
                        });
                    }
                }
            ).fail(function () {

                showNotification(
                    "حذف اطلاعات با خطا همراه بوده است.",
                    "danger"
                );
            });
        }
    },


    /* =========================================
       فیلتر
       ========================================= */
    filter: {

        fromDateObject: null,

        toDateObject: null,

        initial: function () {

            this.initialFromDate();

            this.initialToDate();

            $(".selectpicker")
                .selectpicker("refresh");
        },

        initialFromDate: function () {

            if (
                $("#VueCalendarFromDate")
                    .length === 0
            ) {
                return;
            }

            this.fromDateObject =
                new Vue({

                    el:
                        "#VueCalendarFromDate",

                    data: {
                        date: ""
                    },

                    components: {
                        DatePicker:
                            VuePersianDatetimePicker
                    },

                    methods: {

                        onClose: function () {

                            unitCalendar.filter
                                .checkDates();
                        }
                    }
                });
        },

        initialToDate: function () {

            if (
                $("#VueCalendarToDate")
                    .length === 0
            ) {
                return;
            }

            this.toDateObject =
                new Vue({

                    el:
                        "#VueCalendarToDate",

                    data: {
                        date: ""
                    },

                    components: {
                        DatePicker:
                            VuePersianDatetimePicker
                    },

                    methods: {

                        onClose: function () {

                            unitCalendar.filter
                                .checkDates();
                        }
                    }
                });
        },

        collect: function () {

            return {

                FromDate:
                    $(
                        "input[name='FromDate']"
                    ).val(),

                ToDate:
                    $(
                        "input[name='ToDate']"
                    ).val(),

                DayType:
                    $("#FilterDayType")
                        .val()
            };
        },

        checkDates: function () {

            var fromDate =
                this.fromDateObject
                    ? this.fromDateObject.date
                    : "";

            var toDate =
                this.toDateObject
                    ? this.toDateObject.date
                    : "";

            if (
                fromDate &&
                toDate &&
                fromDate > toDate
            ) {

                showNotification(
                    "تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.",
                    "danger"
                );

                this.clearDates();
            }
        },

        clearDates: function () {

            if (this.fromDateObject) {
                this.fromDateObject.date =
                    "";
            }

            if (this.toDateObject) {
                this.toDateObject.date =
                    "";
            }

            $("input[name='FromDate']")
                .val("");

            $("input[name='ToDate']")
                .val("");
        },

        clear: function () {

            this.clearDates();

            $("#FilterDayType")
                .val("")
                .selectpicker(
                    "refresh"
                );

            unitCalendar.list
                .reload();
        }
    }
};


/* =========================================
   راه‌اندازی صفحه
   ========================================= */
$(document).ready(function () {

    unitCalendar.list.initial();

    unitCalendar.filter.initial();
});