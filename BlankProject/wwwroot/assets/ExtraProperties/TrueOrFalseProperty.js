//////////////////////////////////////
//   پروپرتی ای که مقدار بولین میگیرد.
//    مثل موجود بودن
/////////////////////////////////////

// Add TrueOrFalse Property preparation.
var addTrueOrFalseProperty = function () {
    TrueOrFalsePropertyModalData.property = new EmptyTrueOrFalseProperty();
    //if (TrueOrFalsePropertyModalData.ContentTypes.length > 0)
    //    TrueOrFalsePropertyModalData.property.ContentTypeId = TrueOrFalsePropertyModalData.ContentTypes[0].Id;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#TrueOrFalse-property-modal");
    });
};


// Save TrueOrFalse property.
var SaveTrueOrFalseProperty = function () {
    var IsValid = ValidateForm('#TrueOrFalse-property-modal');
    if (!IsValid) return;

    var Ids = "";
    var SelectedOptions = $(".selectpicker.TrueOrFalsePropertyCategoryIds option:selected");
    for (var i = 0; i < SelectedOptions.length; i++) {
        if (Ids == "")
            Ids += $(SelectedOptions[i]).attr("value");
        else
            Ids += "," + $(SelectedOptions[i]).attr("value");
    }
    TrueOrFalsePropertyModalData.property.CategoryIds = Ids;
    $.post("/Admin/Properties/AddOrUpdateTrueOrFalseProperty",
        TrueOrFalsePropertyModalData.property,
        function (res) {
            if (res)
                getProperties();

            closeModal("#TrueOrFalse-property-modal");
        });
};


// Edit TrueOrFalse property.
var EditTrueOrFalseProperty = function (property) {
    TrueOrFalsePropertyModalData.property = property;
    if (property.ContentType != null)
        TrueOrFalsePropertyModalData.Categories = property.ContentType.Categories;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#TrueOrFalse-property-modal");
    });
};


// Delete TrueOrFalse property Confirm Modal.
var StartDeleteTrueOrFalseProperty = function (property) {
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
        $.get("/Admin/properties/DeleteTrueOrFalseProperty/" + property.Id,
            function (res) {
                if (res) {
                    getProperties();
                }
            });
    });
};




// Add TrueOrFalse Property for Option preparation.
var StartAddTrueOrFalsePropertyForOption = function (option) {
    var q = new EmptyTrueOrFalseProperty();
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
    TrueOrFalsePropertyModalData.property = q;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#TrueOrFalse-property-modal");
    });
};



// Edit depended TrueOrFalse property.
var EditDependedTrueOrFalseProperty = function (property) {
    TrueOrFalsePropertyModalData.property = property;
    if (property.ContentType != null)
        TrueOrFalsePropertyModalData.Categories = property.ContentType.Categories;
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
        openModal("#TrueOrFalse-property-modal");
    });
};


///////////////////////
//  objects
//////////////////////

//empty TrueOrFalse property model
var EmptyTrueOrFalseProperty = function () {
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
    this.PropertyType = PropertyType.TrueOrFalse;
};


// TrueOrFalse Property Modal Data.
var TrueOrFalsePropertyModalData = new Vue({
    el: "#TrueOrFalse-property-modal-body",
    data: {
        property: new EmptyTrueOrFalseProperty(),
        ContentTypes: [],
        Categories: []
    },
    mounted: function () {
        axios.get("/Admin/ContentTypes/GetAllContentTypes")
                .then(response => { this.ContentTypes = response.data });

    },
    methods: {
        TrueOrFalsePropertyHasCategory: function (CategoryId) {
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
        TrueOrFalsePropertyModalData.property.IsUserProperty = false;
    }
    else if ($(this).val() == "1") {
        TrueOrFalsePropertyModalData.property.IsUserProperty = true;
    }
    TrueOrFalsePropertyModalData.property.ContentTypeId = null;
    TrueOrFalsePropertyModalData.property.CategoryIds = "";
    Vue.nextTick(function () {
        $('.selectpicker').selectpicker('refresh');
    });
});



//لود کردن دسته بندی های مربوط به یک نوع محتوا
$(".selectpicker.ContentType").on("change", function () {
    var val = $(this).val();
    if (val != "" && val != null) {
        for (var i = 0; i < TrueOrFalsePropertyModalData.ContentTypes.length; i++)
            if (parseInt(val, 10) == TrueOrFalsePropertyModalData.ContentTypes[i].Id)
                TrueOrFalsePropertyModalData.Categories = TrueOrFalsePropertyModalData.ContentTypes[i].Categories;
    }
    else
        TrueOrFalsePropertyModalData.property.ContentTypeId = null;

    TrueOrFalsePropertyModalData.property.CategoryId = null;
    Vue.nextTick(function () {
        $('.selectpicker.TrueOrFalsePropertyCategoryIds').selectpicker('refresh');
    });
});










