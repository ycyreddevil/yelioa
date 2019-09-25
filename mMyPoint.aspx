<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mMyPoint.aspx.cs" Inherits="mMyPoint" %>

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
        <script src="Scripts/weui.min.js"></script>
        <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    </head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog" border="false"
                noheader="true" closed="true" modal="true">
            </div>
    <header>
                <div class="m-toolbar">
                    <span class="m-title" style="font-size:25px;color:darkorchid">我的积分</span>
                </div>
            </header>
        <div id="preview"></div>
     <script type="text/javascript">
         var url = "mMyPoint.aspx";
         $(document).ready(function () {    
                 Loading(true);
                 $.post('mMyPoint.aspx', { act: 'getmypoint' }, function (res) {
                     if (res != "") {
                         var tempData = $.parseJSON(res);                       
                             var html = "<br><br>";
                             html += '<div class="weui-form-preview">';
                             html += '<div class="weui-form-preview__hd">';
                             html += '<label class="weui-form-preview__label">姓名:</label>';
                             html += '<em class="weui-form-preview__value">' + tempData[0].Target + '</em>';
                             html += '</div>';
                             html += '<div class="weui-form-preview__bd">';
                             html += '<p>';
                             html += '<label class="weui-form-preview__label">月度积分:</label>';
                             html += '<span class="weui-form-preview__value">' + tempData[0].month_point + '</span>';
                             html += '</p>';
                             html += '<p>';
                             html += '<label class="weui-form-preview__label">季度积分:</label>';
                             html += '<span class="weui-form-preview__value">' + tempData[0].season_point + '</span>';
                             html += '</p>';
                             html += '<p>';
                             html += '<label class="weui-form-preview__label">年度积分:</label>';
                             html += '<span class="weui-form-preview__value">' + tempData[0].year_point + '</span>';
                             html += '</p>';
                             html += '<p>';
                             html += '<label class="weui-form-preview__label">总积分:</label>';
                             html += '<span class="weui-form-preview__value">' + tempData[0].total_point + '</span>';
                             html += '</p>';
               
                             html += '</div>';
                             html += '</div>';


                             $("#preview").append(html);                        
                     } else {
                         $.messager.alert('提示', '暂无数据', 'info');
                     }
                     Loading(false);
                 });
         });
     </script>
</body>
</html>
