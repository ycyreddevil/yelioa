$(function () {
    var tmpl = '<li class="weui-uploader__file" style="background-image:url(#url#)"></li>',
        $gallery = $("#gallery"),
        $galleryImg = $("#galleryImg"),
        $uploaderInput = $("#uploaderInput"),
        $uploaderFiles = $("#uploaderFiles");

    $uploaderInput.on("change", function (e) {
        var src, url = window.URL || window.webkitURL || window.mozURL,
            files = e.target.files;
        var suffix = files[0].name.substring(files[0].name.indexOf("."), files[0].name.length).toLowerCase();
        if (suffix == '.jpg' || suffix == '.jpeg' || suffix == '.png') {
            if (files[0].size > 10 * 1024 * 1024) {
                $.messager.alert('提示', "无法上传超过10m的图片");
            } else {
                for (var i = 0, len = files.length; i < len; ++i) {
                    var file = files[i];
                    if (url) {
                        src = url.createObjectURL(file);
                    } else {
                        src = e.target.result;
                    }
                    $uploaderFiles.append($(tmpl.replace('#url#', src)));
                    Loading(true)
                    //调用ajaxfileupload上传图片
                    $.ajaxFileUpload({
                        url: 'mFinanceReimburse.aspx',
                        type: "post",
                        secureuri: false,
                        fileElementId: "uploaderInput",
                        dataType: "json",
                        data: { act: 'uploadReimburseImage' },
                        success: function (data) {
                            Loading(false)
                            if (data.status == "文件上传成功") {
                                uploadFileUrls.push(data.filePath);
                            }
                            $.messager.alert('提示', data.status);
                        },
                        error: function (data) {
                            Loading(false)
                            $.messager.alert('提示', "上传失败");
                        }
                    })
                }
            }
        }
        else {
            $.messager.alert('提示', "只能上传以jpg，jpeg，png为结尾的图片");
        }
    });
    var index; //第几张图片  
    $uploaderFiles.on("click", "li", function () {
        index = $(this).index();
        $galleryImg.attr("style", this.getAttribute("style"));
        $gallery.fadeIn(100);
    });
    $gallery.on("click", function () {
        $gallery.fadeOut(100);
    });
    //删除图片  
    $(".weui-gallery__del").click(function () {
        $uploaderFiles.find("li").eq(index).remove();
        // 删除相应的路劲
        uploadFileUrls.remove(index);
    });
});  