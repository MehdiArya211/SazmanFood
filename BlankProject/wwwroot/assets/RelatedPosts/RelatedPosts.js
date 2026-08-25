TotalCount = 0;

var MainPreLoader = '<div class="preloader text-center" style="padding:50px 0px 30px">'
                    + '<div class="md-preloader"><svg xmlns="http://www.w3.org/2000/svg" version="1.1" height="50" width="50" viewbox="0 0 75 75"><circle cx="37.5" cy="37.5" r="33.5" stroke-width="8" /></svg></div>'
                    + '<div>در حال بارگذاری ...</div>'
                    + '</div>'

/////////////////////////////
// initial load
/////////////////////////////
$(document).ready(function () {
    SearchPosts();
})



/////////////////////////////
// search Posts
/////////////////////////////
$(".srch-inp").on("keyup", function () {
    SearchPosts();
})


$(".srch-type").on("change", function () {
    SearchPosts();
})




$(".RelationTypes").on("change", function () {
    SearchPosts();
})


function SearchPosts() {
    CurrentItemsCount = 0;
    $("#Posts").empty();
    $("#Posts").append(MainPreLoader);
    $.ajax({
        url: "/Posts/SearchRelations/" + $("#MainPostId").val(),
        method: "POST",
        data: "{'Text' : '" + $(".srch-inp").val() + "', 'SearchTypeId' : '" + $(".srch-type").val() + "', 'RelationTypeId' :'" + $(".RelationTypes").val() + "', 'CurrentItemsCount' : '" + CurrentItemsCount + "' , 'TypeId' : '" +$("#TypeId").val() +"' }",
    contentType: "application/json",
    success: function (res, status, xhr) {
        TotalCount = xhr.getResponseHeader("TotalCount");
        if (TotalCount > 0)
            ShowNotification(TotalCount + " مورد یافت شد. ", "green");
        else
            ShowNotification(" موردی یافت نشد.", "red");

        RenderPictures(res);
    },
    fail: function (err) {
        swal({
            title: 'خطا !',
            text: 'جستجو دچار خطا شده است! ',
            type: 'error',
            confirmButtonClass: "btn btn-danger",
            confirmButtonText: "باشه",
            buttonsStyling: false
        })
    }
});

}


function RenderPictures(data) {
    $("#Posts").empty()
    for (var i = 0; i < data.length; i++) {
        $("#Posts").append(NewItem(data[i]));
    }
}




/////////////////////////////
// ChangeRelation post
/////////////////////////////

//select current post as parent
$('body').delegate('.IsRelated', 'click', function () {
    var ClickedItem = $(this);
    AddOrRemoveRelation(true, ClickedItem);
});


//select current post as related
$('body').delegate('.IsParent', 'click', function () {
    var ClickedItem = $(this);
    AddOrRemoveRelation(false, ClickedItem);
});



// clicked for add relation or remove it
function AddOrRemoveRelation(IsRelated, ClickedItem) {
    $(".modal input[type='checkbox']").prop("checked", false);
    $(".modal .Order").val('');
    $(".modal #IsRelated").val(IsRelated);
    $(".modal #RelatedPostId").val(ClickedItem.attr("data-id"));

    RelIds = ClickedItem.attr("data-Ids").split('/');
    if (RelIds != "") {
        for (var i = 0; i < RelIds.length; i++) {
            var st = RelIds[i];
            var id = st.split('-')[0];
            var order = st.split('-')[1];
            $("#RelType" + id).prop("checked", true);
            $("#Order" + id).val(order);
        }
    }
    $(".modal").modal("show");
}



$(".update-relation").on("click", function () {
    $(".Order").find("+ span").html("");
    var IsValid = true;
    var RelTypes = $("input[name='RelType']:checked");
    var RelTypeIds = "";
    for (var i = 0; i < RelTypes.length; i++) {
        if (RelTypeIds != "")
            RelTypeIds += "/";
        TypeId = $(RelTypes[i]).val();
        RelTypeIds += $(RelTypes[i]).val() + "-" + $("#Order" + TypeId).val();
        var Order = $("#Order" + TypeId);
        if (Order.val() === "") {
            Order.find("+ span").html("اولویت الزامی است");
            IsValid = false;
        }
        else {
            if (!IsNumber(Order.val())) {
                Order.find("+ span").html("اولویت باید عدد باشد!");
                IsValid = false;
            }
        }
    }
    if (IsValid) {
        ChangeRelation(RelTypeIds);
    }
    else
        return false;
});




//main function to change relation
function ChangeRelation(RelTypeIds) {
    var RelatedPostId = $("#RelatedPostId").val();
    var IsRelated = $("#IsRelated").val();
    var item;
    if (IsRelated)
        item = $(".item" + RelatedPostId).find("label.IsRelated");
    else
        item = $(".item" + RelatedPostId).find("label.IsParent");

    $(".modal").modal("hide");
    item.parents(".card").find(" .fix-preloader").css({ "display": "block" });
    var data = {
        RelatedPostId: RelatedPostId,
        IsRelated: IsRelated,
        RelationTypeIds: RelTypeIds
    }
    $.get("/Posts/ChangeRelation/" + $("#MainPostId").val(), data, function (res) {
        item.parents(".card").find(" .fix-preloader").css({ "display": "none" });
        swal("ثبت شد", "تغییرات ذخیره شد", "success");
        SearchPosts();
    }).fail(function () {
        item.parents(".card").find(" .fix-preloader").css({ "display": "none" });
        $(".modal").modal("hide");

        swal({
            title: 'خطا !',
            text: 'افزودن ارتباط با خطا مواجه شده است. لطفا مجددا امتحان کنید. ',
            type: 'error',
            confirmButtonClass: "btn btn-success",
            confirmButtonText: "باشه",
            buttonsStyling: false
        })
    });

}




/////////////////////////////
// load on scroll
/////////////////////////////

//for mini sidebar that page get scroll
$(window).scroll(function () {
    if ($("body").hasClass("sidebar-mini")) {
        if ($(window).scrollTop() == $(document).height() - $(window).height()) {
            GetData();
        }
    }
});

//for standard sidebar
$(".main-panel").on("scroll", function (e) {
    var $o = $(e.currentTarget);
    if ($o[0].scrollHeight - $o.scrollTop() <= $o.outerHeight()) {
        GetData();
    }
});

function GetData() {
    var items = $("#Posts > .item");
    CurrentItemsCount = items.length;
    if (CurrentItemsCount >= TotalCount || $("#Posts > .preloader").length > 0)
        return;

    $("#Posts").append(MainPreLoader);
    $.ajax({
        url: "/Posts/SearchRelations/"+ $("#MainPostId").val(),
        method: "POST",
    data: "{'Text' : '" + $(".srch-inp").val() + "', 'SearchTypeId' : '" + $(".srch-type").val() + "', 'RelationTypeId' :'" + $(".RelationTypes").val() + "', 'CurrentItemsCount' : '" + CurrentItemsCount + "' , 'TypeId' : '" +$("#TypeId").val() +"' }",
    contentType: "application/json",
    success: function (res, status, xhr) {
        TotalCount = xhr.getResponseHeader("TotalCount");
        $("#Posts > .preloader").remove();
        AppendPictures(res);
    },
    fail: function (err) {
        append("failed")
        swal({
            title: 'خطا !',
            text: 'خواندن اطلاعات دچار خطا شده است! لطفا مجددا تلاش کنید. ',
            type: 'error',
            confirmButtonClass: "btn btn-danger",
            confirmButtonText: "باشه",
            buttonsStyling: false
        }, function () { GetData(); });//end of swal
    }
});
}


function AppendPictures(data) {
    for (var i = 0; i < data.length; i++) {
        $("#Posts").append(NewItem(data[i]));
    }
}


/////////////////////////////
// Create New Item
/////////////////////////////

var preloader = '<div class="fix-preloader text-center">'
                + '<div class="md-preloader"><svg xmlns="http://www.w3.org/2000/svg" version="1.1" height="30" width="30" viewbox="0 0 75 75"><circle cx="37.5" cy="37.5" r="33.5" stroke-width="8" /></svg></div>'
                + '<div>لطفا صبر کنید ...</div>'
                + '</div>'

var ThumbFolderAddress = "/Uploads/Pictures/Thumbs/";
var MediumFolderAddress = "/Uploads/Pictures/Mediums/";
var LargeFolderAddress = "/Uploads/Pictures/Larges/";


function NewItem(data) {
    var MinTitle = data.Title;
    var PicName = ThumbFolderAddress + data.MainPicName;
    var IsRelated = "checked";
    var IsParent = "checked";
    var RelatedIds = '';
    var ParentIds = '';

    if (data.Title.length > 50)
        MinTitle = data.Title.substring(0, 50) + "...";

    if (data.MainPicName == "")
        PicName = "/areas/admin/assets/img/image_placeholder.jpg";

    if (!data.IsRelated)
        IsRelated = "unchecked";

    if (!data.IsParent)
        IsParent = "unchecked";

    for (var i = 0; i < data.RelatedTypes.length; i++) {
        if (RelatedIds != "")
            RelatedIds += "/";
        RelatedIds += data.RelatedTypes[i].Id + "-" + data.RelatedTypes[i].Order
    }

    for (var i = 0; i < data.ParentTypes.length; i++) {
        if (ParentIds != "")
            ParentIds += "/";
        ParentIds += data.ParentTypes[i].Id + "-" + data.ParentTypes[i].Order
    }


    return '<div class="col-lg-3 col-md-4 col-sm-6 item item' + data.Id + '">'
        + '      <div class="card">'
        + '           <div class="card-image">'
        + '                <img class="img" src="' + PicName + '">'
        + '           </div>'
        + '          <div class="card-content text-center">'
        + '                <div class="card-title row" title="' + data.Title + '">' + MinTitle + '</div>'
        + '<div class="row">'
        + '        <div class="col-md-12">'
        + '            <div class="checkbox">'
        + '                <label class="IsRelated ' + IsRelated + '" data-id="' + data.Id + '" data-Ids="' + RelatedIds + '" type="checkbox" name="IsRelated' + data.Id + '">'
        + '                    <span class="' + IsRelated + '"> </span>  افزودن این پست به پست های مرتبط پست مرجع'
        + '                </label>'
        + '            </div>'
        + '        </div>'
        + '        <div class="col-md-12">'
        + '            <div class="checkbox" style="margin:11px auto">'
        + '                <label class="IsParent ' + IsParent + '" data-id="' + data.Id + '" data-Ids="' + ParentIds + '" type="checkbox" name="IsParent' + data.Id + '">'
        + '                    <span class="' + IsParent + '" ></span> افزودن پست مرجع به پست های مرتبط این پست '
        + '                </label>'
        + '            </div>'
        + '        </div>'
        + ' </div>'

        + preloader

        + '          </div>'
        + '       </div>'
        + '</div>'
}




/////////////////////////////
// show notification
/////////////////////////////
function ShowNotification(Text, Color) {
    $(".notification-text").html(Text);
    $(".notification").css({ "background": Color });
    $(".notification-icon").css({ "color": Color, "border-color": Color });
    $(".notification").addClass("show-top");
}

function HideNotification(Text, Color) {
    $(".notification").removeClass("show")
}


/////////////////////////////
// Is Number?
/////////////////////////////
function IsNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

