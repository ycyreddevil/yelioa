$(function () {
    var tmpl = '<li id="showIOSActionSheet" class="weui-uploader__file" style="background-image:url(#url#)"></li>',
        $uploaderInput = $("#uploaderInput"),
        $uploaderFiles = $("#uploaderFiles");

    $uploaderInput.on("change", function (e) {
        var src, url = window.URL || window.webkitURL || window.mozURL,
            files = e.target.files;
        var suffix = files[0].name.substring(files[0].name.indexOf("."), files[0].name.length);
        if (files[0].size > 20 * 1024 * 1024) {
            $.messager.alert('提示', "无法上传超过20m的文件");
        } else {
            for (var i = 0, len = files.length; i < len; ++i) {
                var file = files[i];
                if (url) {
                    src = url.createObjectURL(file);
                } else {
                    src = e.target.result;
                }
                var suffix = files[0].name.substring(files[0].name.indexOf("."), files[0].name.length);
                if (suffix == ".docx" || suffix == ".doc" || suffix == ".txt") {
                    $uploaderFiles.append($(tmpl.replace('#url#', "Scripts/email/icon/word.png")));
                } else if (suffix == ".xls" || suffix == ".xlsx") {
                    $uploaderFiles.append($(tmpl.replace('#url#', "Scripts/email/icon/excel.png")));
                } else if (suffix == ".pdf") {
                    $uploaderFiles.append($(tmpl.replace('#url#', "Scripts/email/icon/PDF.png")));
                } else if (suffix == ".ppt" || suffix == ".pptx") {
                    $uploaderFiles.append($(tmpl.replace('#url#', "Scripts/email/icon/ppt.png")));
                } else if (suffix == ".jpg" || suffix == ".png" || suffix == ".jpeg" || suffix == ".ico" || suffix == ".gif" || suffix == ".bmp") {
                    $uploaderFiles.append($(tmpl.replace('#url#', "Scripts/email/icon/pic.png")));
                } else if (suffix == ".rar") {
                    $uploaderFiles.append($(tmpl.replace('#url#', "Scripts/email/icon/RAR.png")));
                } else {
                    $uploaderFiles.append($(tmpl.replace('#url#', "Scripts/email/icon/document.png")));
                }

                //$uploaderFiles.append($(tmpl.replace('#url#', src)));
                Loading(true)
                //调用ajaxfileupload上传图片
                $.ajaxFileUpload({
                    url: 'mSendEmail.aspx',
                    type: "post",
                    secureuri: false,
                    fileElementId: "uploaderInput",
                    dataType: "json",
                    data: { act: 'uploadAttachment', emailId: GetQueryString('emailId')},
                    success: function (data) {
                        Loading(false)
                        if (data.status == "文件上传成功") {
                            uploadFileUrls.push({ FilePath: data.filePath, FileName: data.fileName });
                        }
                        $.messager.alert('提示', data.status);
                    },
                    error: function (data, status, e) {
                        console.log(e)
                        Loading(false)
                        $.messager.alert('提示', "上传失败");
                    }
                })
            }
        }        
    });
});  