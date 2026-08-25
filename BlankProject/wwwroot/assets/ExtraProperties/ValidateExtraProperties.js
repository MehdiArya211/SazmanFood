

//پیدا کردن آیدی پروپرتی های اضافه هنگام ذخیره فرم
var FindAndValidateExtraProperties = function () {
    var Properties = $("select.extra-property , input.extra-property , textarea.extra-property");
    var Ids = "";
    for (var i = 0; i < Properties.length; i++) {
        Ids += "-" + $(Properties[i]).attr("rel");
    }
    if (Ids.length > 1)
        Ids = Ids.substring(1);

    $("#ExtraPropertiesId").val(Ids);
    return ValidateExtraProperties();
};


//ولیدیت کردن اکسترا پروپرتی ها
var ValidateExtraProperties = function () {
    var IsValid = true;
    $(".form-group").find("span.error").html("");


    // min value
    $MinValueItems = $('input.min-value , textarea.min-value');
    for (var i = 0; i < $MinValueItems.length; i++) {
        var $item = $($MinValueItems[i]);
        var MinValue = $item.attr("data-min-value");
        if (MinValue != null && MinValue != "") {
            var minVal = parseInt(MinValue);
            var val = parseInt($item.val());
            if (val < minVal) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html(Title + " نباید کمتر از " + minVal + " باشد.");
                IsValid = false;
            }
        }
    }


    // max value
    $MaxValueItems = $('input.max-value , textarea.max-value');
    for (var i = 0; i < $MaxValueItems.length; i++) {
        var $item = $($MaxValueItems[i]);
        var MaxValue = $item.attr("data-max-value");
        if (MaxValue != null && MaxValue != "") {
            var maxVal = parseInt(MaxValue);
            var val = parseInt($item.val());
            if (val > maxVal) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html(Title + " نباید بیشتر از " + maxVal + " باشد.");
                IsValid = false;
            }
        }
    }



    // min character
    $MinCharacterItems = $('input.min-character , textarea.min-character');
    for (var i = 0; i < $MinCharacterItems.length; i++) {
        var $item = $($MinCharacterItems[i]);
        var MinCharacter = $item.attr("data-min-character");
        if (MinCharacter != null && MinCharacter != "") {
            var minVal = parseInt(MinCharacter);
            if ($item.val().length < minVal) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html(Title + " نباید کمتر از " + minVal + " کاراکتر باشد.");
                IsValid = false;
            }
        }
    }


    // max character
    $MaxCharacterItems = $('input.max-character , textarea.max-character');
    for (var i = 0; i < $MaxCharacterItems.length; i++) {
        var $item = $($MaxCharacterItems[i]);
        var MaxCharacter = $item.attr("data-max-character");
        if (MaxCharacter != null && MaxCharacter != "") {
            var maxVal = parseInt(MaxCharacter);
            if ($item.val().length > maxVal) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html(Title + " نباید بیشتر از " + maxVal + " کاراکتر باشد.");
                IsValid = false;
            }
        }
    }

    

    //requierd items
    $requierdItems = $('input.requierd, textarea.requierd , select.requierd');
    for (var i = 0; i < $requierdItems.length; i++) {
        var $item = $($requierdItems[i]);
        if ($item.val() == null || $item.val() == "") {
            var Title = $item.parents(".form-group").find("label").html();
            $item.parents(".form-group").find("span.error").html(Title + " الزامی است.");
            IsValid = false;
        }
    }



    //Unique items
        $uniqueItems = $('input.IsUnique , textarea.IsUnique');
        for (var i = 0; i < $uniqueItems.length; i++) {
            var $item = $($uniqueItems[i]);
            var val = $item.val();
            if (val != null && val != "") {
                data = {
                    Id: $("#Id").val(),
                    PropertyId: $item.attr("rel"),
                    Value: $item.val()
                }

                $.ajax({
                    url: "/Admin/Properties/CheckValueIsUnique",
                    type: 'post',
                    data: data,
                    async: false,
                    success: function (res) {
                        if (!res) {
                            var Title = $item.parents(".form-group").find("label").html();
                            $item.parents(".form-group").find("span.error").html("این " + Title + " یکتا نیست و قبلا استفاده شده است.");
                            IsValid = false;
                        }
                    },
                    error: function () {
                        var Title = $item.parents(".form-group").find("label").html();
                        $item.parents(".form-group").find("span.error").html("خطا رخ داده است. مجددا امتحان کنید");
                        IsValid = false;
                    }
                });
            }
        }





    //Nember
    $numberItems = $('input[type="number"]');
    for (var i = 0; i < $numberItems.length; i++) {
        var $item = $($numberItems[i]);
        if ($item.val() != null && $item.val() != "") {
            if (!$item.val().match(/\d/g)) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html(Title + " باید فقط شامل اعداد باشد.");
                IsValid = false;
            }            
        }
    }


    //Mobile
    $telItems = $('input[type="tel"]');
    for (var i = 0; i < $telItems.length; i++) {
        var $item = $($telItems[i]);
        if ($item.val() != null && $item.val() != "") {
            if (!$item.val().match(/^0[\d]{10}$/)) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html("فرمت وارد شده برای " + Title + " صحیح نیست.");
                IsValid = false;
            }
        }
    }


    //Email
    $EmailItems = $('input[type="email"]');
    for (var i = 0; i < $EmailItems.length; i++) {
        var $item = $($EmailItems[i]);
        if ($item.val() != null && $item.val() != "") {
            var reg = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            if (!$item.val().toLowerCase().match(reg)) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html("فرمت وارد شده برای " + Title + " صحیح نیست.");
                IsValid = false;
            }
        }
    }



    //متن انگلیسی، اعداد و کاراکتر های خاص
    $EnItems = $('input[type="text"].En , textarea.En');
    for (var i = 0; i < $EnItems.length; i++) {
        var $item = $($EnItems[i]);
        if ($item.val() != null && $item.val() != "") {
            var reg = /^[A-Za-z0-9\s-_.]*$/;
            if (!$item.val().toLowerCase().match(reg)) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html(Title + " میتواند تنها شامل حروف انگلیسی، اعداد و کاراکترهای (- ، _ ، .) باشد.");
                IsValid = false;
            }
        }
    }



    //متن فارسی، اعداد و کاراکتر های خاص
    $FaItems = $('input[type="text"].Fa , textarea.Fa');
    for (var i = 0; i < $FaItems.length; i++) {
        var $item = $($FaItems[i]);
        if ($item.val() != null && $item.val() != "") {
            var reg = /^[0-9\u0600-\u06FF\s-_.]*$/;
            if (!$item.val().toLowerCase().match(reg)) {
                var Title = $item.parents(".form-group").find("label").html();
                $item.parents(".form-group").find("span.error").html(Title + " میتواند تنها شامل حروف فارسی، اعداد و کاراکترهای (- ، _ ، .) باشد.");
                IsValid = false;
            }
        }
    }

    return IsValid;
}















