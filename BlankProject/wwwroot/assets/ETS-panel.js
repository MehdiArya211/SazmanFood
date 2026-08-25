var EtsPanel = {
    Notifications: null,
    Setting: null
};

$(function () {
    EtsPanel.Setting.config();
    EtsPanel.Setting.load();

});



showNotification = function (text, type, delay) {
    EtsPanel.Notifications.showNotification(text, type, delay);
};

showCustomNotification = function (type, text) {
    EtsPanel.Notifications.showCustomNotification(type, text);
};


dismisNotifications = function (text) {
    EtsPanel.Notifications.dismisNotifications();
};


EtsPanel.Notifications = {
    notificationList: $(".notification-list"),
    notificationCount: $(".notification-count"),
    playNotificationSound: function () {
        var noti = new Audio("/assets/Sounds/notification48.MP3");
        noti.play();
    },
    setNotificationCountsTo: function (count) {
        if (count == 0) {
            this.notificationCount.hide();
        } else {
            this.notificationCount.show().text(count);
        }
    },
    updateNotificationsList: function (notifications) {
        var emptyPlaceHolder = ' <li><a>اعلان تازه ای ندارید</a></li>';
        var newNoticTempalet = function (item) {
            return ' <li><a href="' + item.Link + '">' + item.Text + ' <small class="label label-default label-xs pull-left">' + item.CreateDate + '</a></li>';
        };

        this.notificationList.empty();
        if (notifications.length == 0)
            this.notificationList.append(emptyPlaceHolder);
        else
            for (var i = 0; i < notifications.length; i++) {
                this.notificationList.append(newNoticTempalet(notifications[i]));
            }
        this.notificationList.append('<a href="/admin/notifications" style="margin-top:10px;" class="text-center col-md-12" >مشاهده همه</a>');
    },
    showNotification: function (text, type, delay) {
        types = ['', 'info', 'success', 'warning', 'danger', 'rose', 'primary'];
        color = Math.floor((Math.random() * 6) + 1);

        if (type == undefined) type = types[color];
        if (delay == undefined) delay = 2000;

        var from = "bottom";
        var align = "center";
        icon = "notifications",

            $.notify({
                icon: icon,
                message: text

            }, {
                type: type,
                timer: delay,
                delay: delay,
                placement: {
                    from: from,
                    align: align
                }
            });
    },
    showCustomNotification: function (type, text) {

        type0 = ['', 'info', 'success', 'warning', 'danger', 'rose', 'primary'];
        //color = Math.floor((Math.random() * 6) + 1);

        var from = "top";
        var align = "center";
        icon = "notifications",

            $.notify({
                icon: icon,
                message: text
            }, {
                type: type,
                timer: 500,
                delay: 500,
                placement: {
                    from: from,
                    align: align
                }
            });
    },
    dismisNotifications: function () {
        $(".alert.alert-with-icon").addClass("hide")
    },
    makeNotificationRead: function () {
        this.setNotificationCountsTo(0);
    }
};

EtsPanel.Setting = {
    config: function () {
        // save settings
        $("#setting-sidebar-active-color .badge.filter").click(function () {
            var color = $(this).data("color");
            localStorage.setItem("menu-active-item-bg-color", color);
        });

        $("#setting-sidebar-bg-color .badge.filter").click(function () {
            var color = $(this).data("color");
            localStorage.setItem("menu-bg-color", color);
        });

        $("#setting-switch-sidebar-mini").change(function () {
            var val = $(this).is(":checked");
            localStorage.setItem("sidebar-mini", val);
        });

        $("#setting-switch-sidebar-image").change(function () {
            var val = $(this).is(":checked");
            localStorage.setItem("sidebar-image", val);
        });

        $(".setting-sidebar-bg-image a").click(function () {
            var img = $(this).parent().index();
            localStorage.setItem("sidebar-bg-image", img);
        });
    },
    // load settings
    load: function () {
        // Menu active item color
        var menuColor = localStorage.getItem("menu-active-item-bg-color");
        $("#setting-sidebar-active-color .badge-" + menuColor).click();

        // sidebar backgroun color
        var bgColor = localStorage.getItem("menu-bg-color");
        $("#setting-sidebar-bg-color .badge-" + bgColor).click();

        // sidebar background image
        var imgIndex = localStorage.getItem("sidebar-bg-image");
        $(".setting-sidebar-bg-image").parent().find("li").eq(imgIndex).find("a").click();

        // sidebar mini toggle
        var switchMini = localStorage.getItem("sidebar-mini");
        $("#setting-switch-sidebar-mini").prop("checked", eval(switchMini));

        if ($("#setting-switch-sidebar-mini").is(":checked"))
            $("#minimizeSidebar").click();

        // sidebar background image toggle
        var switchBgImage = localStorage.getItem("sidebar-image");
        $("#setting-switch-sidebar-image").prop("checked", eval(switchBgImage));
        if (!$("#setting-switch-sidebar-image").is(":checked")) {
            if ($sidebar_img_container.length !== 0) {
                $sidebar.removeAttr("data-image");
                $sidebar_img_container.fadeOut("fast");
            }


            //if ($full_page_background.length != 0) {
            //    $full_page.removeAttr("data-image", "#");
            //    $full_page_background.fadeOut("fast");
            //}

            background_image = false;
        }

    }

};



/*نمایش تولتیپ ها بعد از لود صفحه*/
$(function () {
    $(".remove").attr("data-toggle", "tooltip")
        .attr("data-placement", "top")
        .attr("title", "حذف");

    $(".edit").attr("data-toggle", "tooltip")
        .attr("data-placement", "top")
        .attr("title", "ویرایش");

    $(".details").attr("data-toggle", "tooltip")
        .attr("data-placement", "top")
        .attr("title", "نمایش کامل");

    $(".medias").attr("data-toggle", "tooltip")
        .attr("data-placement", "top")
        .attr("title", "رسانه های مرتبط");

    $(".relations").attr("data-toggle", "tooltip")
        .attr("data-placement", "top")
        .attr("title", "مطالب مرتبط");

    $(".gallery").attr("data-toggle", "tooltip")
        .attr("data-placement", "top")
        .attr("title", "گالری تصاویر");

    $('[data-toggle="tooltip"]').tooltip();
});




// اگر اجکس خطای عدم لاگین گرفته بود 
// صفحه را رفرش کن تا به صفحه لاگین منتقل شود
$(document).ajaxError(
    function (e, request) {
        if (request.status == 401) {
            window.location.reload();
        }
    });




// وقتی یکی از پنل های آکاردیون باز شد، بقیه پنل های باز بسته شوند
$('body').on('click', ' #accordion .card-collapse .card-header a', function () {
    var id = $(this).attr('id');
    $('#accordion .card-collapse .card-header:not(#' + id + ')').find('+ .collapse.in').collapse('hide');
});



/*ایجاد breadcrumb*/
if (typeof breadcrumb !== 'undefined') {
    if (breadcrumb != null && breadcrumb.length > 0) {
        for (var i = 0; i < breadcrumb.length; i++) {
            if (i == breadcrumb.length - 1)
                $('.breadcrumb').append('<li class="breadcrumb-item active" aria-current="page">' + breadcrumb[i].title + '</li>');
            else
                $('.breadcrumb').append('<li class="breadcrumb-item"><a href="' + breadcrumb[i].link + '">' + breadcrumb[i].title + '</a></li>');
        }
    }
}




