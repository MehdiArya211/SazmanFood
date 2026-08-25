//===========================================================
// معادل enum
//===========================================================

// انواع لیوت ها - LayoutType
const layoutTypeEnum = Object.freeze({ "HomePage": 0});


// انواع محتوایی که در هر سطر لیوت لود میشود - LayoutContentType
const layoutContentTypeEnum = Object.freeze({ "Category": 0, "Tag": 1, "MostDiscount": 2, "MostSeller": 3, "Pinned": 4, "offer":5, "BannerType": 6 });

// انواع وضعیت سبد خرید
const basketStatus = Object.freeze({ "Open": 0, "Paid": 1, "Shopper": 2, "Supervisor": 3, "Delivery": 4, "Delivered": 5, "Rejected": 6, "Canceled": 7 });

// انواع وضعیت موجودی محصول موجود در سفارش
const basketItemStatus = Object.freeze({ "Unknown": 0, "Exist": 1, "NotExist": 2, "Imperfect": 3, "Updated": 4, "Added": 5 });


//===========================================================
// نمایش مدال لودینگ هنگام اجکس
//===========================================================
var showAjaxLoadingModal = true;
$(document).ajaxStart(function (e) {
    if (showAjaxLoadingModal == true) {
        swal({
            title: 'لطفا صبر کنید',
            animation: false,
            customClass: "animated fadeInUp loading-swal",
            allowOutsideClick: false,
            onOpen: function () {
                swal.showLoading();
            }
        });
    }
}).ajaxStop(function () {
    showAjaxLoadingModal = true;
    if ($('.loading-swal').length > 0) {
        swal.close("loading-swal");
    }
    if ($('form').length > 0)
        $('form').attr('autocomplete', 'off');
}).ajaxError(function (event, jqxhr, settings, thrownError) {
    showAjaxLoadingModal = true;
    if ($('.loading-swal').length > 0) 
        swal.close("loading-swal");

    var msg = "عملیات با خطا همراه بوده است!";
    if (jqxhr.responseJSON) {
        if (jqxhr.responseJSON.message)
            msg = jqxhr.responseJSON.message;
        else if (jqxhr.responseJSON.Message)
            msg = jqxhr.responseJSON.Message;
    }

    swal({
        type:'error',
        title: 'خطا',
        text: msg,
        confirmButtonText: 'تایید',
        confirmButtonClass: 'btn btn-danger'
    });
    //console.log(jqxhr);

});


// اجکس هایی که نیازی به نشان دادن مدال "لطفا صبر کنید" ندارند
var allowedList = ["files", "checkunique", "global/tags", "postfiles/upload", "global/users", "global/projects", "global/contacts", "admin/dashboard", "requestsystem/ticketreports", "global/companies"]
$(document).ajaxSend(function (event, jqxhr, settings) {
    var url = settings.url.toLowerCase();
    if (typeof startSubmition == 'undefined' || startSubmition == false) {
        for (var i = 0; i < allowedList.length; i++)
            if (url.indexOf(allowedList[i]) >= 0) {
                swal.close("loading-swal");
            }
    }
});



// فوکس روی اینپوت دراپدون هنگام باز شدن
$('body').on("click", '.bootstrap-select button.dropdown-toggle', function () {
    if (!$(this).parent().hasClass("open")) {
        var input = $(this).parent().find(".bs-searchbox input");
        if (input && input.length > 0)
            $(input).focusin();
    }
});




//باز و بسته کردن مدال
var modal = {
    //باز کردن مدال
    open: function (target) {
        if (!target)
            $("#base-modal").modal("show");
        else
            $(target).modal("show");
    },

    //بستن مدال
    close: function (target) {
        if (!target)
            $("#base-modal").modal("hide");
        else
            $(target).modal("hide");

        if ($('.modal.in').length > 0)
            $('.modal.in').css('overflow-y', 'scroll');
    },
}

$('#base-modal').on('hidden.bs.modal', function (e) {
    $('#base-modal').removeClass('small-modal').removeClass('large-modal').removeClass('full-screen-modal');
})




// تعیین مکان اسکرول به یک المان خاص و یا اول صفحه
var setScrollPosition = function (el) {
    if (!el || $(el).length == 0)
        el = '.main-content';
    $(".main-panel").mCustomScrollbar("scrollTo", el);
}





// لود کردن منو کناری 
var loadMenus = function () {
    $.get('/Global/ClientMenus/LoadMenu',
        { CController: controller, CAction: action, UserId: UserId },
        function (res) {
            $('.sidebar-wrapper .main-nav').html(res);

            //باز کردن والدهای منو انتخاب شده
            var navs = $('.nav-item.active').parents('.nav-item');
            var collapse = $('.nav-item.active').parents('.collapse');

            for (var i = 0; i < navs.length; i++)
                $(navs[i]).addClass('active');

            for (var i = 0; i < collapse.length; i++)
                $(collapse[i]).addClass('in');

            // قرار دادن اسکرول منو کناری روی ایتم انتخاب شده
            $('.sidebar .sidebar-wrapper').mCustomScrollbar("scrollTo", '.sidebar .nav-item.active > .nav-link');
        }).fail(function (xhr) { alert('بارگذاری منوها با خطا همراه بوده است.') })
}

if ($('.sidebar').length > 0) {
    loadMenus();

    localStorage.setItem('.l.c.d.T', '');
}
    



//$(function () {
//    var el = $('.nav-item.active')
//    $('.sidebar .sidebar-wrapper').mCustomScrollbar("scrollTo", '.sidebar .nav-item.active > .nav-link');
//})





//فیلتر های مربوط به جستجو
var searchFilter = {
    // باز و بسته کردن پنجره فیلتر
    toggle: function () {
        if ($(".filter-caption").hasClass('closed')) {
            $(".filter-container").slideDown();
            $(".filter-caption").removeClass('closed');
        }
        else {
            $(".filter-container").slideUp();
            $(".filter-caption").addClass('closed');
        }
    }
}





// توابع عمومی استفاده شده در فرم ها
var form = {

    // قرار دادن ویرگول بین قیمت
    renderPrice: function (el, event) {
        // skip for arrow keys
        if (event)
            if (event.which >= 37 && event.which <= 40) return;

        // format number
        $(el).val(function (index, value) {
            var neg = '';
            if (value && parseInt(value) < 0) {
                neg = '-';
                value = value.substr(1, value.length - 1);
            }
            return value
                .replace(/\D/g, "")
                .replace(/\B(?=(\d{3})+(?!\d))/g, ",")
                + neg
                ;
        });
    },



    // گرفتن فرمت قیمت
    getPriceFormat: function (price) {
        if (!price)
            return price;
        price = price.toString();
        if (price) {
            var neg = '';
            if (parseInt(price) < 0) {
                neg = '-';
                price = price.substr(1, price.length - 1);
            }
            price = price
                .replace(/\D/g, "")
                .replace(/\B(?=(\d{3})+(?!\d))/g, ",") + neg;
            return price;
        }
        else if (price == "0")
            return '0';
        else
            return '';
    }
}




var renderPrice = function (el, event) {
    // skip for arrow keys
    if (event)
        if (event.which >= 37 && event.which <= 40) return;

    // format number
    $(el).val(function (index, value) {
        var neg = '';
        if (value && parseInt(value) < 0) {
            neg = '-';
            value = value.substr(1, value.length - 1);
        }
        return value
            .replace(/\D/g, "")
            .replace(/\B(?=(\d{3})+(?!\d))/g, ",")
            + neg
            ;
    });
};

var getPriceFormat = function (price) {
    if (!price)
        return price;
    price = price.toString();
    if (price) {
        var neg = '';
        if (parseInt(price) < 0) {
            neg = '-';
            price = price.substr(1, price.length - 1);
        }
        price = price
            .replace(/\D/g, "")
            .replace(/\B(?=(\d{3})+(?!\d))/g, ",") + neg;
        return price;
    }
    else if (price == "0")
        return '0';
    else
        return '';
};





// تنظیمات چارت ها
var chartSetting = {

    // گرفتن خروجی از چارت
    export: {
        setting: function (showMoreInfo, mainContainer) {
            var option = {
                fallbackToExportServer: false,
                //allowHTML: true,
                buttons: {
                    contextButton: {
                        menuItems: [{
                            text: 'نمایش تمام صفحه',
                            onclick: function () {
                                this.fullscreen.toggle();
                            }
                        }, {
                            text: 'پرینت',
                            onclick: function () {
                                this.print();
                            }
                        }, {
                            separator: true
                        }, {
                            text: 'دانلود png',
                            onclick: function () {
                                this.exportChartLocal();
                            }
                        }, {
                            text: 'دانلود jpeg',
                            onclick: function () {
                                this.exportChartLocal({
                                    type: 'image/jpeg'
                                });
                            }
                        }, {
                            text: 'دانلود pdf',
                            onclick: function () {
                                this.exportChartLocal({
                                    type: 'application/pdf'
                                });
                            }
                        }, {
                            text: 'دانلود svg',
                            onclick: function () {
                                this.exportChartLocal({
                                    type: 'image/svg+xml'
                                });
                            }
                        }]
                    }
                }
            };

            if (showMoreInfo) {
                option.buttons.contextButton.menuItems.push({ separator: true });
                option.buttons.contextButton.menuItems.push({
                    text: 'نمایش اطلاعات تکمیلی',
                    onclick: function () {
                        $(mainContainer).find('.more-info').toggleClass('hide');
                        if ($(mainContainer).find('.more-info').hasClass('hide'))
                            $(mainContainer).find('.highcharts-menu-item:last-of-type').html('نمایش اطلاعات تکمیلی');
                        else
                            $(mainContainer).find('.highcharts-menu-item:last-of-type').html('عدم نمایش اطلاعات تکمیلی');
                    }
                });
            }
            return option;
        },


        //چاپ کردن چارت
        print: function (el) {
            var chart = $(el).highcharts();
            chart.print();
        },


        // نمایش تمام صفحه
        fullscreen: function (el) {
            var chart = $(el).highcharts();
            chart.fullscreen.toggle();
        },


        //ذخیره بصورت PNG
        exportPNG: function (el) {
            var chart = $(el).highcharts();
            chart.exportChartLocal({ allowHTML: true});
        },


        // ذخیره بصورت JPEG
        exportJPEG: function (el) {
            var chart = $(el).highcharts();
            chart.exportChartLocal({
                type: 'image/jpeg'
            });
        },


        ///ذخیره بصورت PDF
        exportPDF: function (el) {
            var chart = $(el).highcharts();
            chart.exportChartLocal({
                type: 'application/pdf'
            });
        },

        ///ذخیره بصورت SVG
        exportSVG: function (el) {
            var chart = $(el).highcharts();
            chart.exportChartLocal({
                type: 'image/svg+xml'
            });
        },


        // نمایش اطلاعات تکمیلی چارت
        moreInfo: function (el, mainContainer) {
            $(mainContainer).find('.more-info').toggleClass('hide');
            setTimeout(function () {
                if ($(mainContainer).find('.more-info').hasClass('hide'))
                    $(el).html('نمایش اطلاعات تکمیلی');
                else
                    $(el).html('عدم نمایش اطلاعات تکمیلی');
            }, 500);

        }
    },


    // عنوان چارت
    titleSetting: function(title){
        var option = {
            text: title,
            style: {
                fontSize: '14px',
                direction: 'rtl',
                textAlign:'center',
                //lineHeight: '40px;',
                fontWeight: 'bold',
                fontFamily: 'IranSans'
            }
        };
        return option;
    },


    // زیر عنوان چارت
    subtitleSetting: function (text) {
        var option = {
            text: text,
            style: {
                fontSize: '12px',
                direction: 'rtl;',
                textAlign: 'center',
                fontWeight: 'bold',
                fontFamily: 'IranSans'
            }
        };
        return option;
    },


    //چارت از نوع میله ای
    column: {
        xAxis: function (cats) {
            var model = {
                type: 'category',
                categories: cats,
                labels: {
                    rotation: 0,
                    style: {
                        fontSize: '12px',
                        textAlign: "center",
                        direction: 'rtl',
                        fontFamily: 'IranSans',
                        lineHeight: '20px',
                    },
                    //useHTML: true
                },
                reversed: true
            };
            return model;
        },


        yAxis: function (title) {
            var model = {
                min: 0,
                title: {
                    text: title,
                    //useHTML: true,
                    style: {
                        fontSize: '13px',
                        fontWeight: 'bold',
                        fontFamily: 'IranSans'
                    }
                },
                opposite: true
            };
            return model;
        },



        series: function (title, values) {
            var model = [{
                name: title,
                data: values,
                dataLabels: {
                    enabled: true,
                    //rotation: 0,
                    //color: '#FFFFFF',
                    //align: 'center',
                    format: '{point.y:.1f}', // one decimal
                    //y: 35, // 10 pixels down from the top
                    style: {
                        fontSize: '13px'
                    }
                }
            }];
            return model;
        }
    }

}







// نمایش base64 بصورت پی دی اف در پنجره جدید
var printPdfPreview = (data, type = 'application/pdf') => {
    let blob = null;
    blob = this.b64toBlob(data, type);
    const blobURL = URL.createObjectURL(blob);
    const theWindow = window.open(blobURL);
    const theDoc = theWindow.document;
    const theScript = document.createElement('script');
    function injectThis() {
        window.print();
    }
    theScript.innerHTML = `window.onload = ${injectThis.toString()};`;
    theDoc.body.appendChild(theScript);
};

var b64toBlob = (content, contentType) => {
    contentType = contentType || '';
    const sliceSize = 512;
    // method which converts base64 to binary
    const byteCharacters = window.atob(content);

    const byteArrays = [];
    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        const slice = byteCharacters.slice(offset, offset + sliceSize);
        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }
    const blob = new Blob(byteArrays, {
        type: contentType
    }); // statement which creates the blob
    return blob;
};






// گرفتن عرض صفحه
if ($(window).width() < 768) {
    $('.filter .filter-caption').addClass('closed');
}





// ولیدیت کردن با رگولار اکسپرشن
var validateRole = {
    mobile: {
        regex: /^[\u06F0|0][\u06F0-\u06F90-9]{10}/g,
        minLength: 11,
        maxLength: 11,

        isMatch: function (text) {
            if (!text)
                return { status: false, message: 'الزامی است!' };
            if (text.length < this.minLength)
                return { status: false, message: 'حداقل ' + this.minLength + ' کاراکتر باشد!' };
            if (text.length > this.maxLength)
                return { status: false, message: 'حداکثر ' + this.maxLength + ' کاراکتر باشد!' };
            if (!text.match(this.regex))
                return { status: false, message: 'فرمت وارد شده صحیح نیست!' };
            return { status: true };
        }
    },


    email: {
        regex: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,

        isMatch: function (text) {
            if (!text)
                return { status: false, message: 'الزامی است!' };
            if (!text.match(this.regex))
                return { status: false, message: 'فرمت وارد شده صحیح نیست!' };
            return { status: true };
        }
    },


    number: {
        regex: /^[0-9]+$/,

        isMatch: function (text) {
            if (!text)
                return { status: false, message: 'الزامی است!' };
            if (!text.match(this.regex))
                return { status: false, message: 'فقط عدد مجاز است!' };
            return { status: true };
        }
    },



    minLength: {
        isMatch: function (text, minLength) {
            if (!text)
                return { status: false, message: 'الزامی است!' };
            if (text.length < minLength)
                return { status: false, message: 'حداقل ' + minLength + ' کاراکتر باشد!' };
            return { status: true };
        }
    },


    maxLength: {
        isMatch: function (text , maxLength) {
            if (!text)
                return { status: false, message: 'الزامی است!' };
            if (text.length > maxLength)
                return { status: false, message: 'حداکثر ' + maxLength + ' کاراکتر باشد!' };
            return { status: true };
        }
    },

}




//=================================================================
// گرفتن خلاصه از رشته ها
//=================================================================
Object.defineProperty(String.prototype, "getSummary", {
    value: function getSummary(count) {
        var st = this;
        if (!st)
            return null;
        if (st.length < count)
            return st;
        return st.substr(0, count) + ' ...';
    },
    writable: true,
    configurable: true
});



