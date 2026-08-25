
var PropertyType = Object.freeze({ "Input": 0, "DropDown": 1, "TrueOrFalse": 2 });
var NormalPropertyType = Object.freeze({ "SingleLineText": 0, "MultiLineText": 1, "Numeric": 2, "Mobile": 3, "Email": 4 });
var ValidationType = Object.freeze({ "Empty": 0, "Fa": 1, "En": 2 });
var OldSelectedCategoryIds = "-1";

//بررسی برای وجود پروپرتی وابسته به آپشن
$("body").on("change", ".selectpicker.check-for-depended-property", function () {
    var $ThisParent = $(this).parents(".form-group");
    var $next = $ThisParent.next();
    while ($next.hasClass("depended-property")) {
        $next = $next.next();
        $ThisParent.next().remove();
    }

    $.get("/Admin/Properties/GetPropertyByHeadOptionId/" + $(this).val(), function (res) {
        if (res != null)
            CreateExtraProperty($ThisParent, res);
    });
});






//لود کردن پروپرتی های دسته بندی پس از انتخاب
var LoadCategoryProperties = function (SelectedCategoryIds) {
    //اگر در حال لود کردن دسته بندی های انتخاب شده برای پست است نیاز به لود پروپرتی نیست.
    if (!InitialFinished)
        return;

    //دسته بندی ها عوض نشدن که بخواد پروپرتی جدید لود کنه
    if (SelectedCategoryIds == OldSelectedCategoryIds)
        return;

    OldSelectedCategoryIds = SelectedCategoryIds;
    var Properties = $(".category-propery select.extra-property , .category-propery input.extra-property , .category-propery textarea.extra-property");
    var PropertyIds = "";
    for (var i = 0; i < Properties.length; i++) {
        PropertyIds += "," + $(Properties[i]).attr("rel");
    }
    if (PropertyIds.length > 1)
        PropertyIds = PropertyIds.substring(1);

    var data = {
        SelectedCategoryIds: SelectedCategoryIds,
        PropertyIds: PropertyIds
    };

    $.get("/Admin/Properties/GetCategoryProperties", data, function (res) {
        if (res != null) {
            //حذف پروپرتی های قدیمی
            //alert("del = " + res.DeletedPropertiesId.length)
            for (var i = 0; i < res.DeletedPropertiesId.length; i++) {
                var id = res.DeletedPropertiesId[i];
                $(".extra-property[rel='" + id + "']").parents('.category-propery').remove();
            }

            //ساخت پروپرتی های جدید
            //alert("new = " + res.NewProperties.length)
            for (var i = 0; i < res.NewProperties.length; i++) {
                CreateExtraProperty($("#ExtraPropertiesId"), res.NewProperties[i]);
            }

        }
    }).fail(function () {
        alert("خطا رخ داده است.")
    });
};




//ساخت اکسترا پروپرتی
var CreateExtraProperty = function (el, data) {
    var Property;
    if (data.PropertyType == PropertyType.Input) {
        Property = CreateNormalProperty(data);
        $(Property).insertAfter($(el));
    }
    else if (data.PropertyType == PropertyType.DropDown) {
        Property = CreatePropertyWithOption(data);
        $(Property).insertAfter($(el));
        $($(el).next().find('.selectpicker')).selectpicker('refresh');
    }
    else if (data.PropertyType == PropertyType.TrueOrFalse) {
        Property = CreateTrueOrFalseProperty(data);
        $(Property).insertAfter($(el));
        $.material.init();
    }
};



// ساخت پروپرتی معمولی
var CreateNormalProperty = function (data) {
    var InputType = "text";
    var TypeClass = "SingleLineText";
    var validation = "";
    var Data_attr = "";

    if (data.IsNotNull) validation += "requierd ";
    if (data.IsUnique) validation += "IsUnique ";

    validation += data.MinCharacter == null ? "" : "min-character ";
    validation += data.MaxCharacter == null ? "" : "max-character ";
    Data_attr += data.MaxCharacter == null ? "" : "data-max-character='" + data.MaxCharacter + "' ";
    Data_attr += data.MinCharacter == null ? "" : "data-min-character='" + data.MinCharacter + "' ";

    if (data.NormalPropertyType == NormalPropertyType.SingleLineText || data.NormalPropertyType == NormalPropertyType.MultiLineText) {
        if (data.ValidationType == ValidationType.En) {
            validation += "En ";
        }
        else if (data.ValidationType == ValidationType.Fa) {
            validation += "Fa ";
        }
    }
    else if (data.NormalPropertyType == NormalPropertyType.Email) {
        InputType = "email";
        TypeClass = "Email";
    }
    else if (data.NormalPropertyType == NormalPropertyType.Mobile) {
        InputType = "tel";
        TypeClass = "Mobile";
    }
    else if (data.NormalPropertyType == NormalPropertyType.Numeric) {
        InputType = "number";
        TypeClass = "Numeric";
        validation += data.MinValue == null ? "" : "min-value ";
        validation += data.MaxValue == null ? "" : "max-value ";
        Data_attr += data.MaxValue == null ? "" : "data-max-value='" + data.MaxValue + "' ";
        Data_attr += data.MinValue == null ? "" : "data-min-value='" + data.MinValue + "' ";
    }


    var DependencyType = "";
    if (data.IsDependedProperty)
        DependencyType += ' depended-property ';
    if (data.Categories.length > 0) {
        DependencyType += ' category-propery ';
        for (var i = 0; i < data.Categories.length; i++) {
            DependencyType += " cat" + data.Categories[i].Id;
        }
    }


    var Item = '<div class="form-group ' + DependencyType + '" ' + (data.Categories.length == 0 ? '' : 'data-categoryid="' + data.CategoryIds + '"') + '>'
              + '<label class="control-label col-md-2" for="' + data.UniqueKey + '">' + data.Title + '</label>'
              + '<div class="col-md-10">';

    if (data.NormalPropertyType == NormalPropertyType.MultiLineText) {
        Item += '<textarea class="form-control text-box multi-line extra-property MultiLineText ' + validation + '" ' + Data_attr + ' rel="' + data.Id + '" id="' + data.UniqueKey + '" name="' + data.UniqueKey + '" placeholder="' + (data.Placeholder == null ? '' : data.Placeholder) + '"></textarea>';
    }
    else {
        Item += '<input type="' + InputType + '" class="form-control extra-property ' + TypeClass + ' ' + validation + '" ' + Data_attr + ' rel="' + data.Id + '" id="' + data.UniqueKey + '" name="' + data.UniqueKey + '" placeholder="' + (data.Placeholder == null ? '' : data.Placeholder) + '" />';
    }
    Item += '<span class="text-danger error"> </span>'
      + '</div>'
      + '</div>';
    return Item;
};



//ساخت پروپرتی با آپشن
var CreatePropertyWithOption = function (data) {
    var DependencyType = "";
    if (data.IsDependedProperty)
        DependencyType += ' depended-property ';
    if (data.Categories.length > 0) {
        DependencyType += ' category-propery ';
        for (var i = 0; i < data.Categories.length; i++) {
            DependencyType += " cat" + data.Categories[i].Id;
        }
    }

    var Item = '<div class="form-group ' + DependencyType + '" ' + (data.Categories.length == 0 ? '' : 'data-categoryid="' + data.CategoryIds + '"') + '>'
                + '<label class="control-label col-md-2" for="' + data.UniqueKey + '">' + data.Title + '</label>'
                + '<div class="col-md-10">'
                + '<select name="' + data.UniqueKey + '" rel="' + data.Id + '" id="' + data.UniqueKey + '" title="' + data.Title + '" data-size="7" tabindex="-98" ';
    if (data.IsMultiSelect)
        Item += 'class="selectpicker extra-property ' + (data.IsNotNull ? 'requierd' : '') + '" data-style="btn select-with-transition" multiple="">';
    else
        Item += 'class="selectpicker check-for-depended-property extra-property ' + (data.IsNotNull ? 'requierd' : '') + '" data-style="btn btn-primary btn-round">';

    for (var i = 0; i < data.Options.length; i++) {
        Item += '<option value="' + data.Options[i].Id + '">' + data.Options[i].Title + '</option>';
    }
    Item += '</select>'
        + '<span class="text-danger error"> </span>'
        + '</div>'
        + '</div>';
    return Item;
};



//ساخت پروپرتی بله/خیر
var CreateTrueOrFalseProperty = function (data) {
    var DependencyType = "";
    if (data.IsDependedProperty)
        DependencyType += ' depended-property ';
    if (data.Categories.length > 0) {
        DependencyType += ' category-propery ';
        for (var i = 0; i < data.Categories.length; i++) {
            DependencyType += " cat" + data.Categories[i].Id;
        }
    }

    var Item = '<div class="form-group ' + DependencyType + '" ' + (data.Categories.length == 0 ? '' : 'data-categoryid="' + data.CategoryIds + '"') + '>'
            + '<div class="togglebutton col-md-11 col-md-offset-1">'
                + '<label>'
                    + data.Title
                    + '<input class="extra-property" type="checkbox" name="' + data.UniqueKey + '" id="' + data.UniqueKey + '" rel="' + data.Id + '" />'
                + '</label>'
            + '</div>'
        + '</div>';
    return Item;
};


