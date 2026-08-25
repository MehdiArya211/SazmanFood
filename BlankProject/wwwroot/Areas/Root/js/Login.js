


// ولیدیت کردن فرم
var isSubmited = false;
var Validate = function () {
    if (isSubmited)
        return false;
    isSubmited = true;

    var username = $('#Mobile').val();
    var password = $('#Password').val();
    if (!username || !password) {
        $('.error').html('نام کاربری و کلمه عبور را وارد کنید!');
        isSubmited = false;
        return false;
    }

    if ($('#captcha').length > 0) {
        var captcha = $('#captcha').val();
        if (!captcha) {
            $('.error').html('کد امنیتی را وارد کنید!');
            isSubmited = false;
            return false;
        }
    }

    //if (!validLogin()) return false;

    return true;
}



var loginCount = {
    setValue: function (count) {
        if (!count)
            count = 0;
        var dd = moment(new Date()).format('HHYYmmMMssDD');
        localStorage.setItem(".l.c.d.T", count + '.' + dd);
    },

    getCount: (lcdt) => {
        if (!lcdt)
            return 0;
        return parseInt(lcdt.substring(0, 1));
    },

    getDate: (lcdt) => {
        if (!lcdt)
            return null;

        var hour = lcdt.substring(2, 4);
        var year = "20" + lcdt.substring(4, 6);
        var min = lcdt.substring(6, 8);
        var month = lcdt.substring(8, 10);
        var sec = lcdt.substring(10, 12);
        var day = lcdt.substring(12, 14);
        var dateSt = `${year}/${month}/${day} ${hour}:${min}:${sec}`;
        var date = moment(dateSt, 'YYYY/MM/DD HH:mm:ss');
        return date;
    },

    dateIsExpire: (date) => {
        var now = new moment();
        var duration = moment.duration(now.diff(date))
        var minutesDiff = duration.as('minutes');
        if (minutesDiff < 0 || minutesDiff > 30) {
            var data = {
                status: true,
                diff: 0
            };
            return data;
        }

        var data2 = {
            status: false,
            diff: 30 - Math.round(minutesDiff) 
        };
        return data2;
    }

}


function validLogin() {
    var lcdt = localStorage.getItem(".l.c.d.T");
    if (!lcdt) {
        loginCount.setValue(0);
        return true;
    }

    var count = loginCount.getCount(lcdt);
    var date = loginCount.getDate(lcdt);

    if (count < 5) {
        loginCount.setValue(count + 1);
        return true;
    } else {
        var isExpired = loginCount.dateIsExpire(date);
        if (!isExpired.status) {
            swal('ورود غیر مجاز', 'حساب کاربری شما بدلیل ورود اشتباه کلمه عبور تا ' + isExpired.diff + ' دقیقه آینده مسدود می باشد.', 'error');
            return false;
        }
        else {
            loginCount.setValue(0);
            return true;
        }
    }
}




$('#imgcpatcha').click(function () {
    var d = new Date();
    $("#imgcpatcha").attr("src", "Captcha/CaptchaImage?" + d.getTime());
});


$('.full-page').addClass('login-page');
var _lcdt = localStorage.getItem(".l.c.d.T");
if (!_lcdt) 
    loginCount.setValue(0);


// حذف لوکال استوریج برای نمایش مدال توی داشبورد
localStorage.removeItem('showDashboardModal');