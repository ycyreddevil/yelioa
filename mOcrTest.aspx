<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mOcrTest.aspx.cs" Inherits="mOcrTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link rel="stylesheet" href="Scripts/themes/mobile.css">
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/jquery.easyui.mobile.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
</head>

<body>
    <div id="loading"
        style="background-position: center center; width: 110px; height: 110px; background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;"
        class="easyui-dialog" border="false" noheader="true" closed="true" modal="true">
    </div>
    <div class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span id="p4-title" class="m-title">提示</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                        onclick="$.mobile.back()">返回</a>
                </div>
                <div class="m-right">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                        onclick="uploadImage();">确定</a>
                </div>
            </div>
        </header>
        <div>
            <input id="fpdm" class="easyui-textbox" data-options="label:'发票代码',labelPosition:'top'" style="width: 100%">
            <input id="fphm" class="easyui-textbox" data-options="label:'发票号码',labelPosition:'top'" style="width: 100%">
            <input id="uploaderInput" type="file" accept="image/*">
            <input data-options="multiline:true" id="remark" class="easyui-textbox" style="width: 100%;height: 360px">
        </div>

    </div>

    <script type="text/javascript">
        var Url = "mOcrTest.aspx";
        $(document).ready(function () {

        });

        function uploadImage() {
            var file = $("#uploaderInput").get(0).files[0];
            if (file == undefined) {
                var data = {
                    act: 'uploadImage',
                    fpdm: $('#fpdm').textbox('getText'),
                    fphm: $('#fphm').textbox('getText')
                };
                parent.Loading(true);
                $.post(Url, data, function (resJson) {
                    parent.Loading(false);
                    $("#remark").textbox('setText', resJson);
                    var res = $.parseJSON(resJson);
                });
            }
            else {
                if (file != undefined && file.size > 4 * 1024 * 1024) {
                    $.messager.alert('提示', "无法上传超过4m的图片");
                }
                else {
                    var reader = new FileReader();
                    reader.readAsDataURL(file);
                    reader.onloadend = function () {
                        // $("#Image1").attr("src", reader.result);
                        var data = {
                            act: 'uploadImage',
                            imageBase: reader.result.replace("data:image/jpeg;base64,", ""),
                            fpdm: $('#fpdm').textbox('getText'),
                            fphm: $('#fphm').textbox('getText')
                        };
                        parent.Loading(true);
                        $.post(Url, data, function (resJson) {
                            parent.Loading(false);
                            $("#remark").textbox('setText', resJson);
                            var res = $.parseJSON(resJson);
                        });
                    }
                }

            }

        }
    </script>
</body>

</html>