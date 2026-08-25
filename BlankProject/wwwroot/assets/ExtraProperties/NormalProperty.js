//////////////////////////////////////
//   پروپرتی ای که مدیر مقدار ان را وارد میکند.
//    مثل قیمت
/////////////////////////////////////

// Add Normal Property preparation.
var addNormalProperty = function () {
    NormalpropertyModalData.property = new EmptyNormalProperty();
    //if (NormalpropertyModalData.ContentTypes.length > 0)
    //    NormalpropertyModalData.property.ContentTypeId = NormalpropertyModalData.ContentTypes[0].Id;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#normal-property-modal");
    });
};


// Save normal property.
var SaveNormalProperty = function () {
    var IsValid = ValidateForm('#normal-property-modal');
    if (!IsValid) return;

    var Ids = "";
    var SelectedOptions = $(".selectpicker.NormalPropertyCategoryIds option:selected");
    for (var i = 0; i < SelectedOptions.length; i++) {
        if (Ids == "")
            Ids += $(SelectedOptions[i]).attr("value");
        else
            Ids += "," + $(SelectedOptions[i]).attr("value");
    }
    NormalpropertyModalData.property.CategoryIds = Ids;
    $.post("/Admin/Properties/AddOrUpdateNormalProperty",
        NormalpropertyModalData.property,
        function (res) {
            if (res)
                getProperties();

            closeModal("#normal-property-modal");
        }).fail(function () { alert("خطا رخ داده است") });
};


// Edit Normal property.
var EditNormalProperty = function (property) {
    NormalpropertyModalData.property = property;
    if (property.NormalPropertyType == null)
        NormalpropertyModalData.property.NormalPropertyType = NormalPropertyType.SingleLineText;
    if (property.ContentType != null)
        NormalpropertyModalData.Categories = property.ContentType.Categories;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#normal-property-modal");
    });
};


// Delete Normal property Confirm Modal.
var StartDeleteNormalProperty = function (property) {
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
        $.get("/Admin/properties/DeleteNormalProperty/" + property.Id,
            function (res) {
                if (res) {
                    getProperties();
                }
            });
    });
};




// Add Normal Property for Option preparation.
var StartAddNormalPropertyForOption = function (option) {
    var q = new EmptyNormalProperty();
    q.IsDependedProperty = true;
    q.HeadOptionId = option.Id;
    var parentProp = AllProperties.ContentTypeProperties.find(x => x.Id == option.PropertyId);
    if (parentProp == null)
        parentProp = AllProperties.CategoryProperties.find(x => x.Id == option.PropertyId);
    if (parentProp == null)
        parentProp = AllProperties.CustomerProperties.find(x => x.Id == option.PropertyId);
    if (parentProp.ContentTypeId != null)
        q.ContentTypeId = parentProp.ContentTypeId;
    if (parentProp.CategoryId != null)
        q.CategoryId = parentProp.CategoryId;

    q.IsUserProperty = parentProp.IsUserProperty
    NormalpropertyModalData.property = q;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#normal-property-modal");
    });
};



// Edit depended normal property.
var EditDependedNormalProperty = function (property) {
    NormalpropertyModalData.property = property;
    if (property.NormalPropertyType == null)
        NormalpropertyModalData.property.NormalPropertyType = NormalPropertyType.SingleLineText;
    if (property.ContentType != null)
        NormalpropertyModalData.Categories = property.ContentType.Categories;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#normal-property-modal");
    });
};


///////////////////////
//  objects
//////////////////////

//empty normal property model
var EmptyNormalProperty = function () {
    this.Id = null;
    this.Title = null;
    this.UniqueKey = null;
    this.ContentTypeId = null;
    this.CategoryIds = ""; // آیدی دسته هایی که این پروپرتی را دارند به صورت استرینگ که با ویرگول جدا شدند.
    this.Categories = []; // آرایه ای از دسته بندی های این پروپرتی
    this.IsUserProperty = false;
    this.IsCheckableInBasket = false; // در سبد خرید مقایسه شود؟
    this.Order = 9999;
    this.IsDependedProperty = false;
    this.HeadOptionId = null;
    this.PropertyType = PropertyType.Input;
    this.Placeholder = null;
    this.ShowInForm = true;
    this.IsNotNull = false;
    this.IsUnique = false;
    this.MaxCharacter = null;
    this.MinCharacter = null;
    this.MaxValue = null;
    this.MinValue = null;
    this.NormalPropertyType = NormalPropertyType.SingleLineText;
    this.ValidationType = null;
};


// Normal Property Modal Data.
var NormalpropertyModalData = new Vue({
    el: "#normal-property-modal-body",
    data: {
        property: new EmptyNormalProperty(),
        ContentTypes: [],
        Categories: [],
        NormalPropertyTypes: [],
        ValidationTypes: []
    },
    mounted: function () {
        axios.get("/Admin/ContentTypes/GetAllContentTypes")
                .then(response => { this.ContentTypes = response.data });

        axios.get("/Admin/Properties/GetAllNormalPropertyTypes")
                .then(response => {
                    this.NormalPropertyTypes = response.data;
                });

        axios.get("/Admin/Properties/GetAllValidationTypes")
                .then(response => { this.ValidationTypes = response.data })
    },
    methods: {
        NormalPropertyTypeHasValidation: function (NormalPropertyType) {
            var SingleLineText;
            var MultiLineText;
            for (var i = 0; i < this.NormalPropertyTypes.length ; i++) {
                if (this.NormalPropertyTypes[i].Key == "SingleLineText") {
                    SingleLineText = this.NormalPropertyTypes[i].Id;
                }
                if (this.NormalPropertyTypes[i].Key == "MultiLineText") {
                    MultiLineText = this.NormalPropertyTypes[i].Id;
                }
            }
            if (NormalPropertyType == SingleLineText
                || NormalPropertyType == MultiLineText)
                return "show";
            else
                return "hide";
        },
        NormalPropertyTypeIsNumeric: function (NormalPropertyType) {
            var Numeric;
            for (var i = 0; i < this.NormalPropertyTypes.length ; i++) {
                if (this.NormalPropertyTypes[i].Key == "Numeric") {
                    Numeric = this.NormalPropertyTypes[i].Id;
                }
            }
            if (NormalPropertyType == Numeric)
                return "show";
            else
                return "hide";
        },
        NormalPropertyHasCategory: function (CategoryId) {
            var IsExist = false;
            for (var i = 0; i < this.property.Categories.length ; i++) {
                if (this.property.Categories[i].Id == CategoryId) {
                    IsExist = true;
                }
            }
            return IsExist;
        }
    }
});




/////////////////////////////////////////////
// Controll JQuery
/////////////////////////////////////////////


//انتخاب اینکه پروپرتی برای کاربران است یا محتوا
$(".selectpicker.IsUserProperty-dropdown").on("change", function () {
    if ($(this).val() == "0") {
        NormalpropertyModalData.property.IsUserProperty = false;
    }
    else if ($(this).val() == "1") {
        NormalpropertyModalData.property.IsUserProperty = true;
    }
    NormalpropertyModalData.property.ContentTypeId = null;
    NormalpropertyModalData.property.CategoryId = null;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
    });
});



//لود کردن دسته بندی های مربوط به یک نوع محتوا
$(".selectpicker.ContentType").on("change", function () {
    var val = $(this).val();
    if (val != "" && val != null) {
        for (var i = 0; i < NormalpropertyModalData.ContentTypes.length; i++)
            if (parseInt(val, 10) == NormalpropertyModalData.ContentTypes[i].Id)
                NormalpropertyModalData.Categories = NormalpropertyModalData.ContentTypes[i].Categories;
    }
    else
        NormalpropertyModalData.property.ContentTypeId = null;

    NormalpropertyModalData.property.CategoryId = null;
    Vue.nextTick(function () {
        $('.selectpicker.NormalPropertyCategoryIds').selectpicker('refresh');
    });
});



//تغییر ولیدیشن
$(".selectpicker.NormalPropertyType").on("change", function () {
    NormalpropertyModalData.property.ValidationType = null;
    NormalpropertyModalData.property.MinValue = null;
    NormalpropertyModalData.property.MaxValue = null;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
    });
});












