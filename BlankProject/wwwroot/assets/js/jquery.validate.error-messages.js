jQuery.extend(jQuery.validator.messages, {
    required: "الزامی است!",
    remote: "فرمت وارد شده صحیح نیست!",
    email: "ایمیل وارد شده صحیح نیست!",
    url: "آدرس وارد شده صحیح نیست!",
    date: "تاریخ وارد شده صحیح نیست!",
    dateISO: "Please enter a valid date (ISO).",
    number: "عدد وارد کنید!",
    digits: "Please enter only digits.",
    creditcard: "Please enter a valid credit card number.",
    equalTo: "تکرار اشتباه است!",
    accept: "Please enter a value with a valid extension.",
    maxlength: jQuery.validator.format("حداکثر باید {0} کاراکتر باشد."),
    minlength: jQuery.validator.format("حداقل باید {0} کاراکتر باشد."),
    rangelength: jQuery.validator.format("مقداری به طول {0} و {1} کاراکتر وارد کنید."),
    range: jQuery.validator.format("مقدار وارد شده باید بین {0} و {1} باشد."),
    max: jQuery.validator.format("حداکثر مقدار مجاز {0} می باشد."),
    min: jQuery.validator.format("حداقل مقدار مجاز {0} می باشد.")
});



// رشته با تعداد کاراکتر مشخص
$.validator.addMethod("length", function (value, element, param) {
    return this.optional(element) || value.length == param;
}, $.validator.format("باید {0} کاراکتر باشد."));


// ولیدیت کردن رگولار
$.validator.addMethod(
    "regex",
    function (value, element, regexp) {
        var re = new RegExp(regexp);
        return this.optional(element) || re.test(value);
    },
    "فرمت وارد شده صحیح نیست!"
);




