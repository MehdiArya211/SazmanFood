var uploadTemplate = '<div class="col-md-4 col-sm-4">'
                   + '<div class="fileinput text-center fileinput-new" data-provides="fileinput">'
                       + '<div class="fileinput-new thumbnail">'
                           + '<img src="/areas/admin/assets/img/image_placeholder.jpg" alt="...">'
                       + '</div>'
                       + '<div class="fileinput-preview fileinput-exists thumbnail" style=""></div>'
                       + '<div class="form-group label-floating is-empty">'
                           + '<label class="control-label">عنوان</label>'
                           + '<input class="form-control" name="Title0" type="text">'
                           + '<span class="material-input"></span>'
                       + '</div>'
                       + '<div>'
                           + '<span class="btn btn-rose btn-round btn-file">'
                               + '<span class="fileinput-new">تصویر را انتخاب کنید</span>'
                               + '<span class="fileinput-exists">تغییر</span>'
                               + '<input value="" name="title0" type="text">'
                               + '<input name="file0" type="file">'
                               + '<div class="ripple-container"></div>'
                           + '</span>'
                           + '<a href="#pablo" class="btn btn-danger btn-round fileinput-exists" data-dismiss="fileinput"><i class="fa fa-times"></i> حذف<div class="ripple-container"><div class="ripple ripple-on ripple-out" style="left: 43.75px; top: 30.9px; background-color: rgb(255, 255, 255); transform: scale(15.5458);"></div></div></a>'
                       + '</div>'
                   + '</div>'
               + '</div>';

var i = 0;
$("#add-new-upload").click(function () {
    uploadTemplate = replaceAll(uploadTemplate, "file" + i, "file" + (i + 1))
    uploadTemplate = replaceAll(uploadTemplate , "Title" + i , "Title" + (i + 1))

    $(".upload-place").append(uploadTemplate);
    $("#filesCount").val(i + 1)
    i++;
})

var bar = $('.progress-bar');
var percent = $('.progress-bar');
var status = $('#status');

$('#files-form').ajaxForm({
    beforeSend: function () {
        // status.empty();
        var percentVal = '0%';
        bar.width(percentVal)
        percent.html(percentVal);
    },
    uploadProgress: function (event, position, total, percentComplete) {
        var percentVal = percentComplete + '%';
        bar.width(percentVal)
        percent.html(percentVal);
    },
    success: function (res) {
        var percentVal = '100%';
        bar.width(percentVal)
        percent.html(percentVal);
        $(".upload-place").empty();
        if (i != 0) {
            uploadTemplate = replaceAll(uploadTemplate, "file" + i, "file0")
            uploadTemplate = replaceAll(uploadTemplate, "Title" + i, "Title0")
            $("#filesCount").val("0");
            i = 0;
        }
        $(".upload-place").append(uploadTemplate);
        RenderPictures(res);

    },
    complete: function (xhr) {
       // status.html(xhr.responseText);
    }
});

// Replace All function
function replaceAll(word, key, replaceValue) {
    while (word.indexOf(key) > -1) {
        word = word.replace(key, replaceValue)
    }
    return word;
}


//MakeFileElement (Card)
function NewCard(obj) {
    var temp = ' <div class="col s6 m3 l1 item">'
                     + '<div class="card">'
                            + '<div class="card-image">'
                                + '<img class="circle responsive-img" style="height:60px;" src="' + obj.Pic + '" />'
                            + '</div>'
                            + '<div class="card-content">'
                                + '<p class="truncate">'
                                     + obj.Title
                                + '</p>'
                                + '<input onchange="ToggleSelectFile(this)" type="checkbox" id="' + obj.Id + '" name=' + obj.Id + '" />'
                                + '<label for="' + obj.Id + '">انتخاب</label>'
                            + '</div>'
                    + '</div>'
                + '</div>'
    return temp;
}

