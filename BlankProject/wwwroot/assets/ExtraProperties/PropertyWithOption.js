//////////////////////////////////////
//   تنظیمات کلی
/////////////////////////////////////

//Enums
var PropertyType = Object.freeze({ "Input": 0, "DropDown": 1, "TrueOrFalse": 2 });
var NormalPropertyType = Object.freeze({ "SingleLineText": 0, "MultiLineText": 1, "Numeric": 2, "Mobile": 3, "Email": 4 });
var ValidationType = Object.freeze({ "Empty": 0, "Fa": 1, "En": 2 });



// Get properties.
var getProperties = function () {
    $.get("/Admin/Properties/GetAllProperties", function (res) {
        AllProperties.ContentTypeProperties = res.ContentTypeProperties;
        AllProperties.CategoryProperties = res.CategoryProperties;
        AllProperties.CustomerProperties = res.CustomerProperties;
    });
};
//getProperties();



// Get ContentTypes.
var getContentTypes = function () {
    $.get("/Admin/ContentTypes/GetAllContentTypes", function (res) {
        NormalpropertyModalData.ContentTypes = res;
        PropertyWithOptionModalData.ContentTypes = res;
    });
};
//getContentTypes();



// Close Modal by #id.
var closeModal = function (el) {
    $(el).modal("hide");
};

// Open Modal by #id.
var openModal = function (el) {
    $(".error").html("");
    $(el).modal("show");
};


//////////////////////////////////////
//   پروپرتی ای که چند گزینه دارد و مدیر
//    گزینه را انتخاب میکند.
/////////////////////////////////////


// Add Property With Option preparation.
var addPropertyWithOption = function () {
    PropertyWithOptionModalData.property = new EmptyPropertyWithOption();
    //if (PropertyWithOptionModalData.ContentTypes.length > 0)
    //    PropertyWithOptionModalData.property.ContentTypeId = PropertyWithOptionModalData.ContentTypes[0].Id;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#property-with-option-modal");
    });
};


// Add Option preparation.
var addOption = function (property) {
    optionModalData.option = new EmptyOption();
    optionModalData.option.propertyId = property.Id;
    Vue.nextTick(function () {
        openModal("#option-modal");
    });
};


//add option to property
var AddOptionToProperty = function () {
    var IsValid = ValidateForm('#option-modal');
    if (!IsValid) return;
    PropertyWithOptionModalData.property.Options.push(optionModalData.option);
    PropertyWithOptionModalData.property.Options.sort(function (a, b) { return a.Order - b.Order });
    $(".option-count-error").html("");
    closeModal("#option-modal");
};



//remove option from property
var RemoveOption = function (option) {
    PropertyWithOptionModalData.property.Options = PropertyWithOptionModalData.property.Options.filter(function (el) {
        return el !== option;
    });
    PropertyWithOptionModalData.property.Options.sort(function (a, b) { return a.Order - b.Order });
};



// Save property.
var SavePropertyWithOption = function () {
    var IsValid = ValidateForm("#property-with-option-modal");
    if (PropertyWithOptionModalData.property.Options.length == 0) {
        $(".option-count-error").html("حداقل یک آپشن برای پروپرتی وارد کنید!");
        IsValid = false;
    }
    else {
        $(".option-count-error").html("");
    }
    if (!IsValid) return;
      
    var Ids = "";
    var SelectedOptions = $(".selectpicker.PropertyWithOptionCategoryIds option:selected");
    for (var i = 0; i < SelectedOptions.length; i++) {
        if (Ids == "")
            Ids += $(SelectedOptions[i]).attr("value");
        else
            Ids += "," + $(SelectedOptions[i]).attr("value");
    }
    PropertyWithOptionModalData.property.CategoryIds = Ids;
    $.post("/Admin/properties/AddOrUpdatepropertyWithOption",
        PropertyWithOptionModalData.property,
        function (res) {
            if (res) {
                getProperties();
            }
            closeModal("#property-with-option-modal");
        });
};



// Edit property with Option.
var EditPropertyWithOption = function (property) {
    PropertyWithOptionModalData.property = property;
    if (property.ContentType != null)
        PropertyWithOptionModalData.Categories = property.ContentType.Categories;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#property-with-option-modal");
    });
};



// Delete property with option Confirm Modal.
var StartDeletePropertyWithOption = function (property) {
    swal({
        title: "آیا مطمئنید ؟",
        text: "اطلاعات بعد از حذف قابل بازگشت نیست !",
        type: "warning",
        showCancelButton: true,
        cancelButtonClass: "btn btn-default",
        confirmButtonClass: "btn btn-danger",
        confirmButtonText: "بله ، حذف شود !",
        cancelButtonText: "خیر",
        buttonsStyling: false
    }).then(function () {
        $.get("/Admin/properties/DeletePropertyWithOption/" + property.Id,
            function (res) {
                if (res) {
                    getProperties();
                }
            });
    });
};



// Open Option modal to edit.
var StartEditOption = function (option) {
    optionModalData.option = option;
    Vue.nextTick(function () {
        openModal("#option-modal");
    });
};


// Update Option.
var UpdateOption = function () {
    var IsValid = ValidateForm("#option-modal");
    if (!IsValid) return;
    if (optionModalData.option.Id) {
        $.post("/Admin/Options/Update", optionModalData.option, function (res) {
            if (res) {
                getProperties();
            }
        });
    }
    Vue.nextTick(function () {
        closeModal("#option-modal");
    });
};



// Delete option property.
var DeleteOption = function (option) {
    swal({
        type: "warning",
        showCancelButton: true,
        title: "آیا مطمئنید ؟",
        text: "اطلاعات بعد از حذف قابل بازگشت نیست !",
        confirmButtonText: "بله ، حذف شود !",
        confirmButtonClass: "btn btn-danger",
        cancelButtonText: "خیر",
        cancelButtonClass: "btn btn-default",
        buttonsStyling: false
    }).then(() => {
        $.get("/Admin/Options/Delete/" + option.Id,
        function (res) {
            if (res) {
                getProperties();
            }
        });
        closeModal("#option-modal");
    });
};





//////////////////////////////////
// اضافه کردن سوال به آپشن
// وقتی این آپشن انتخاب شد بعدش چه سوالی پرسیده بشه ؟
//////////////////////////////////


// Add Property With option for Option preparation.
var StartAddPropertyForOption = function (option) {
    var q = new EmptyPropertyWithOption();
    q.IsDependedProperty = true;
    q.HeadOptionId = option.Id;
    var parentProp = AllProperties.ContentTypeProperties.find(x => x.Id == option.PropertyId);
    if (parentProp == null)
        parentProp = AllProperties.CategoryProperties.find(x => x.Id == option.PropertyId);
    if (parentProp == null)
        parentProp = AllProperties.CustomerProperties.find(x => x.Id == option.PropertyId);
    if (parentProp.ContentTypeId != null)
        q.ContentTypeId = parentProp.ContentTypeId;
    if (parentProp.CategoryIds != null) {
        q.CategoryIds = parentProp.CategoryIds;
        q.Categories = parentProp.Categories;
    }

    q.IsUserProperty = parentProp.IsUserProperty
    PropertyWithOptionModalData.property = q;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#property-with-option-modal");
    });
};


// Edit depended property with option.
var EditDependedPropertyWithOption = function (property) {
    PropertyWithOptionModalData.property = property;
    if (property.ContentType != null)
        PropertyWithOptionModalData.Categories = property.ContentType.Categories;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#property-with-option-modal");
    });
};



///////////////////////
//  objects
//////////////////////


// Empty property with option view model.
var EmptyPropertyWithOption = function () {
    this.Id = null;
    this.Title = null;
    this.UniqueKey = null;
    this.PropertyType = PropertyType.DropDown;
    this.ContentTypeId = null;
    this.CategoryIds = ""; // آیدی دسته هایی که این پروپرتی را دارند به صورت استرینگ که با ویرگول جدا شدند.
    this.Categories = []; // آرایه ای از دسته بندی های این پروپرتی
    this.IsUserProperty = false;
    this.IsCheckableInBasket = false; // در سبد خرید مقایسه شود؟
    this.Order = 9999;
    this.IsDependedProperty = false;
    this.HeadOptionId = null;
    this.IsMultiSelect = false;
    this.IsNotNull = false;
    this.Options = [];
};



// Empty Option view model.
var EmptyOption = function () {
    this.Id = null;
    this.propertyId = null;
    this.Title = null;
    this.Order = 9999;
    this.DependedPropertyId = null;
};



// load all properties
var AllProperties = new Vue({
    el: "#properties",
    data: {
        ContentTypeProperties: [],
        CategoryProperties: [],
        CustomerProperties: []
    },
    mounted: function () {
        axios.get("/Admin/Properties/GetAllProperties")
            .then(response => {
                this.ContentTypeProperties = response.data.ContentTypeProperties;
                this.CategoryProperties = response.data.CategoryProperties;
                this.CustomerProperties = response.data.CustomerProperties;
            })
    },
    methods: {
        GetNormalPropertyTypeTitle: function (arr, propName, propValue) {
            var el = null;
            for (var i = 0; i < arr.length; i++)
                if (arr[i][propName] == propValue)
                    el = arr[i];
            if (el == null)
                return "---";
            else
                return el.Title;
        },
        GetPropertyTypeTitle: function (Type) {
            var out = "---";
            if (Type == PropertyType.Input)
                out = "Input";
            else if (Type == PropertyType.DropDown)
                out = "DropDown";
            else if (Type == PropertyType.TrueOrFalse)
                out = "True / False";

            return out;
        }
    }
});


// property Modal Data.
var PropertyWithOptionModalData = new Vue({
    el: "#property-with-option-modal-body",
    data: {
        property: new EmptyPropertyWithOption(),
        ContentTypes: [],
        Categories: []
    },
    mounted: function () {
        axios.get("/Admin/ContentTypes/GetAllContentTypes")
            .then(response => { this.ContentTypes = response.data })
    },
    methods: {
        PropertyWithOptionHasCategory: function (CategoryId) {
            var IsExist = false;
            for (var i = 0; i < this.property.Categories.length ; i++) {
                if (this.property.Categories[i].Id == CategoryId) {
                    IsExist = true;
                    break;
                }
            }
            return IsExist;
        }
    }
});


// Option Modal Data.
var optionModalData = new Vue({
    el: "#option-modal",
    data: {
        option: new EmptyOption()
    }
});





/////////////////////////////////////////////
// Controll JQuery
/////////////////////////////////////////////

//انتخاب اینکه پروپرتی برای کاربران است یا محتوا
$(".selectpicker.IsUserProperty-dropdown-withoption").on("change", function () {
    if ($(this).val() == "0") {
        PropertyWithOptionModalData.property.IsUserProperty = false;
    }
    else if ($(this).val() == "1") {
        PropertyWithOptionModalData.property.IsUserProperty = true;
    }
    PropertyWithOptionModalData.property.ContentTypeId = null;
    PropertyWithOptionModalData.property.CategoryIds = "";
    PropertyWithOptionModalData.property.Categories = [];
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
    });
});



//لود کردن دسته بندی های مربوط به یک نوع محتوا
$(".selectpicker.ContentType-withoption").on("change", function () {
    var val = $(this).val();
    if (val != "" && val != null) {
        for (var i = 0; i < PropertyWithOptionModalData.ContentTypes.length; i++)
            if (parseInt(val, 10) == PropertyWithOptionModalData.ContentTypes[i].Id)
                PropertyWithOptionModalData.Categories = PropertyWithOptionModalData.ContentTypes[i].Categories;
    }
    else
        PropertyWithOptionModalData.property.ContentTypeId = null;

    PropertyWithOptionModalData.property.CategoryIds = "";
    PropertyWithOptionModalData.property.Categories = [];
    Vue.nextTick(function () {
        $('.selectpicker.PropertyWithOptionCategoryIds').selectpicker('refresh');
    });
});





//ولیدیت کردن فرم
var ValidateForm = function (form) {
    var IsValid = true;
    var requierdItems = $(form).find(" input.requierd");
    $(requierdItems).each(function () {
        var Parent = $(this).parents(".form-group");
        var Title = $(Parent).find("label").html();
        if ($(this).val() == "") {
            $(Parent).find(".error").html(Title + " الزامی است.");
            IsValid = false;
        }
        else {
            $(Parent).find(".error").html();
        }
    });

    var UniqueKey = $(form).find(" input.UniqueKey");
    var Parent = UniqueKey.parents(".form-group");
    var Title = $(Parent).find("label").html();
    var Id = $(form).find(".PropertyId").val();
    var val = UniqueKey.val();
    if (val != "") {
        var Temp = AllProperties.ContentTypeProperties.find(x => x.UniqueKey == val);
        if (Temp == null)
            Temp = AllProperties.CategoryProperties.find(x => x.UniqueKey == val);
        if (Temp == null)
            Temp = AllProperties.CustomerProperties.find(x => x.UniqueKey == val);

        if (Temp != null && Temp.Id != Id) {
            $(Parent).find(".error").html(Title + " تکراری است.");
            IsValid = false;
        }
        else {
            $(Parent).find(".error").html();
        }
    }

    return IsValid;
}

$("body").on("keyup", "input.requierd", function () {
    if ($(this).val() != "") {
        $(this).parents(".form-group").find(".error").html("");
    }
})








