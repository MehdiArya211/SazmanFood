//انواع نوتیفیکیشن ها
const notificationType = Object.freeze({ "Comment": 0, "ContactUs": 1, "SuppervisorNewOrder": 2, "SuppervisorReturnOrder": 3, "ShopperOrder": 4, "DeliveryOrder": 5, "Ticket": 6 });


var connection = new signalR.HubConnectionBuilder().withUrl("/notifHub").build();


// هنگام دریافت توتیفیکیشن جدید
connection.on("newNotification", function (notic) {
    notif.newNotification(notic);
});


// هنگام بروزرسانی همه نوتیفیکیشن ها
connection.on("updateNotificationsList", function (notifications) {
    notif.refreshAll(notifications);
});

// هنگام نیاز به بروزرسانی لیست نوتیفیکیشن ها
connection.on("needUpdateNotification", function () {
    signalRNotification.getAllNotifications();
});


// بروزرسانی نوتیف ها هنگام اتصال به سیگنال آر
connection.start().then(function () {
    //console.log("hub: " , connection);
    signalRNotification.getAllNotifications();
}).catch(function (err) {
    return console.error(err.toString());
});


// بروزرسانی تعداد نوتیفیکیشن ها
//notics.client.updateNotificationsCount = function (count) {
//    EtsPanel.Notifications.setNotificationCountsTo(count);
//}


// سین کردن نوتیفیکیشن ها با باز کردن دراپدون نوتیف ها
//$(".notification-trigger").click(function () {
//    SignalRNotification.makeNotificationsRead();
//});


// ارسال درخواست از هاب سیگنال آر
signalRNotification = {
    getAllNotifications : function () {
        connection.invoke("RefreshNotifications").catch(function (err) {
            console.log('refresh all error:', err);
            return console.error(err.toString());
        });
    },
    seenNotifications : function(type, ids) {
        connection.invoke("SeenNotifications", type, ids).catch(function (err) {
            return console.error(err.toString());
        });
    }
}






// رندر کردن ویو و نمایش html
var notif = {

    // بروزرسانی همه نوتیفیکیشن ها
    refreshAll: (data) => {
        console.log('all Notifs:' , data)
        if (!data)
            return;
        notif.order.refresh(data.orderNotificationCount, data.orderNotificatios);
        notif.comment.refresh(data.commentNotificationCount, data.commentNotificatios);
        notif.contactUs.refresh(data.contactUsNotificationCount, data.contactUsNotificatios);
        notif.ticket.refresh(data.ticketNotificationCount, data.ticketNotificatios);

        //راه اندازی منو بغل برای نمایش نوتیف در موبایل
        setTimeout(() => {
            md.initSidebarsCheck();
        },500)
    },



    //افزودن نوتیفیکیشن جدید
    newNotification: (data) => {
        if (data.type == notificationType.Comment)
            notif.comment.newNotification(data);
        else if (data.type == notificationType.ContactUs)
            notif.contactUs.newNotification(data);
        else if(data.type == notificationType.Ticket)
            notif.ticket.newNotification(data);
        else
            notif.order.newNotification(data);

        var text = `<b>${data.title}</b> <div>${data.text}</div>`;
        showNotification(text, 'primary');
        notif.playNotificationSound();

        //راه اندازی منو بغل برای نمایش نوتیف در موبایل
        setTimeout(() => {
            md.initSidebarsCheck();
        }, 500)
    },


    // نوتیفیکیشن سفارشات
    order: {
        list: $(".order-list"),
        count: $(".order-count"),

        // بروزرسانی نوتیفیکیشن ها
        refresh: (count, data) => {
            notif.order.setCount(count);
            notif.order.updateList(data)
        },

        // تعیین تعداد نوتیفیکیشن ها
        setCount: (count) => {
            if (count == 0) {
                notif.order.count.hide();
            } else {
                notif.order.count.show().text(count);
            }
        },

        // آپدیت کردن لیست نوتیفیکیشن ها
        updateList: (notifications) => {
            notif.order.list.empty();
            if (notifications.length == 0)
                notif.order.list.append(notif.emptyPlaceHolder('سفارش'));
            else {
                for (var i = 0; i < notifications.length; i++)
                    notif.order.list.append(notif.notificationItem(notifications[i]));

                notif.order.list.append(notif.showAllBtn(notifications[0].type));
            }
        },

        //افزودن نوتیفیکیشن جدید
        newNotification: (data) => {
            notif.order.list.find('.no-notif').remove();
            notif.order.list.prepend(notif.notificationItem(data));
            var count = parseInt(notif.order.count.text());
            if (!count) count = 0;
            count++;
            notif.order.setCount(count);
            if (count == 1)
                notif.order.list.append(notif.showAllBtn(data.type));
        }
    },


    // نوتیفیکیشن کامنت ها
    comment: {
        list: $(".comment-list"),
        count: $(".comment-count"),

        // بروزرسانی نوتیفیکیشن ها
        refresh: (count, data) => {
            notif.comment.setCount(count);
            notif.comment.updateList(data)
        },

        // تعیین تعداد نوتیفیکیشن ها
        setCount: (count) => {
            if (count == 0) {
                notif.comment.count.hide();
            } else {
                notif.comment.count.show().text(count);
            }
        },

        // آپدیت کردن لیست نوتیفیکیشن ها
        updateList: (notifications) => {
            notif.comment.list.empty();
            if (notifications.length == 0)
                notif.comment.list.append(notif.emptyPlaceHolder('نظر'));
            else {
                for (var i = 0; i < notifications.length; i++)
                    notif.comment.list.append(notif.notificationItem(notifications[i]));

                notif.comment.list.append(notif.showAllBtn(notifications[0].type));
            }
        },

        //افزودن نوتیفیکیشن جدید
        newNotification: (data) => {
            notif.comment.list.find('.no-notif').remove();
            notif.comment.list.prepend(notif.notificationItem(data));
            var count = parseInt(notif.comment.count.text());
            if (!count) count = 0;
            count++;
            notif.comment.setCount(count);
            if (count == 1)
                notif.comment.list.append(notif.showAllBtn(data.type));
        }
    },


    // نوتیفیکیشن تماس با ما
    contactUs: {
        list: $(".contactUs-list"),
        count: $(".contactUs-count"),

        // بروزرسانی نوتیفیکیشن ها
        refresh: (count, data) => {
            notif.contactUs.setCount(count);
            notif.contactUs.updateList(data)
        },

        // تعیین تعداد نوتیفیکیشن ها
        setCount: (count) => {
            if (count == 0) {
                notif.contactUs.count.hide();
            } else {
                notif.contactUs.count.show().text(count);
            }
        },

        // آپدیت کردن لیست نوتیفیکیشن ها
        updateList: (notifications) => {
            notif.contactUs.list.empty();
            if (notifications.length == 0)
                notif.contactUs.list.append(notif.emptyPlaceHolder('پیام'));
            else {
                for (var i = 0; i < notifications.length; i++)
                    notif.contactUs.list.append(notif.notificationItem(notifications[i]));

                notif.contactUs.list.append(notif.showAllBtn(notifications[0].type));
            }
        },

        //افزودن نوتیفیکیشن جدید
        newNotification: (data) => {
            notif.contactUs.list.find('.no-notif').remove();
            notif.contactUs.list.prepend(notif.notificationItem(data));
            var count = parseInt(notif.contactUs.count.text());
            if (!count) count = 0;
            count++;
            notif.contactUs.setCount(count);
            if (count == 1)
                notif.contactUs.list.append(notif.showAllBtn(data.type));
        }
    },


    // نوتیفیکیشن تیکت
    ticket: {
        list: $(".ticket-list"),
        count: $(".ticket-count"),

        // بروزرسانی نوتیفیکیشن ها
        refresh: (count, data) => {
            notif.ticket.setCount(count);
            notif.ticket.updateList(data)
        },

        // تعیین تعداد نوتیفیکیشن ها
        setCount: (count) => {
            if (count == 0) {
                notif.ticket.count.hide();
            } else {
                notif.ticket.count.show().text(count);
            }
        },

        // آپدیت کردن لیست نوتیفیکیشن ها
        updateList: (notifications) => {
            notif.ticket.list.empty();
            if (notifications.length == 0)
                notif.ticket.list.append(notif.emptyPlaceHolder('تیکت'));
            else {
                for (var i = 0; i < notifications.length; i++)
                    notif.ticket.list.append(notif.notificationItem(notifications[i]));

                notif.ticket.list.append(notif.showAllBtn(notifications[0].type));
            }
        },

        //افزودن نوتیفیکیشن جدید
        newNotification: (data) => {
            notif.ticket.list.find('.no-notif').remove();
            notif.ticket.list.prepend(notif.notificationItem(data));
            var count = parseInt(notif.ticket.count.text());
            if (!count) count = 0;
            count++;
            notif.ticket.setCount(count);
            if (count == 1)
                notif.ticket.list.append(notif.showAllBtn(data.type));
        }
    },


    // آیتم عدم وجود اعلان جدید
    emptyPlaceHolder: (title) => `<li class="no-notif"><a>${title} تازه ای ندارید</a></li>`,


    // آیتم یک نوتیفیکیشن
    notificationItem: (item) => {
        return '<li class="notif-item" rel="' + item.id + '"><a href="' + item.link + '"><b>' + item.user + '</b></br>' + item.text.getSummary(40) + ' <small class="label label-default label-xs pull-left">' + item.createDateFa + '</a></li>';
    },


    // دکمه مشاهده همه در انتهای اعلان ها
    showAllBtn: (type) => {
        var url = '';
        if (type == notificationType.SuppervisorNewOrder || type == notificationType.SuppervisorReturnOrder)
            url = '/shoppingSystem/SupervisorOrders';
        else if (type == notificationType.ShopperOrder)
            url = '/shoppingSystem/ShopperOrders';
        else if (type == notificationType.DeliveryOrder)
            url = '/shoppingSystem/DeliveryOrders';
        else if (type == notificationType.Comment)
            url = '/crm/comments';
        else if (type == notificationType.ContactUs)
            url = '/crm/contactus';
        else if (type == notificationType.Ticket)
            url = '/ticketsystem/tickets';

        return `<li class="all-notif"><a href="${url}" class="text-center" >مشاهده همه</a></a>`
    },


    // صدای نوتیفیکیشن جدید
    playNotificationSound: function () {
        var noti = new Audio("/assets/Sounds/notification48.MP3");
        noti.play();

        //var audio = document.createElement("AUDIO")
        //document.body.appendChild(audio);
        //audio.src = "/assets/Sounds/notification48.MP3"
        //audio.play();
    },


};
