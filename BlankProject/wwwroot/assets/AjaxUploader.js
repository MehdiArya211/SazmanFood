

var formdata

// use this when content is just files
function Upload(TargetURL, InputFileID, SuccessHandler, FailHandler, ProgressHandler) {
 formdata= new FormData(); //FormData object
    var fileInput = document.getElementById(InputFileID);
    //Iterating through each files selected in fileInput
    for (i = 0; i < fileInput.files.length; i++) {
        //Appending each file to FormData object
        formdata.append(fileInput.files[i].name, fileInput.files[i]);
    }
    sendData(TargetURL, SuccessHandler);
    
}



function sendData(TargetURL, SuccessHandler, FailHandler, ProgressHandler) {
    //Creating an XMLHttpRequest and sending
    var xhr = new XMLHttpRequest();

    xhr.open('POST', TargetURL);
    xhr.send(formdata);
    xhr.upload.onprogress = function (data, ProgressHandler) {
        Progress(data, ProgressHandler);
    }
    xhr.onerror = FailHandler;
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            SuccessHandler(xhr.responseText);
        }
    }
}

function Progress(data,ProgressHandler) {
    pr = (data.loaded / data.total) * 100;
    pr = pr.toString();
    pr = pr.substring(0, 4);
    ProgressHandler(pr)

}
