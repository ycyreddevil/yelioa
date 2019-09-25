<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mDeliverRelated.aspx.cs" Inherits="mDeliverRelated" %>

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
   <div id="p1" class="easyui-navpanel">
            <header>
                <div class="m-toolbar">
                    <span class="m-title">发货申请表单</span>
                </div>
            </header> 
            <div id="preview">
            </div>        
    </div>
     <div id="p2" class="easyui-navpanel" style="position:relative;padding:20px">
            <header>
                <div class="m-toolbar">
                    <span class="m-title">发货申请明细</span>
                </div>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" style="width:50px" onclick="$.mobile.back()">返回</a>
                </div>
            </header>
            <div>
                <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
                    <input class="easyui-textbox" type="hidden" id="docCode1" name="Id" value=""/>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="申请人:" prompt="申请人" style="width:100%" name="UserName" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="发货方式:" prompt="发货方式" style="width:100%" name="DeliverStyle" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="收货单位:" prompt="收货单位" style="width:100%" name="hospitalName" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="产品名称:" prompt="产品名称" style="width:100%" name="productName" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="产品规格:" prompt="产品规格" style="width:100%" name="spec" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="单位:" prompt="单位" style="width:100%" name="unit" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="发货数量:" prompt="发货数量" style="width:100%" name="DeliverNumber" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="现有库存:" prompt="现有库存" style="width:100%" name="Stock" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="上月纯销:" prompt="上月纯销" style="width:100%" name="NetSales" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="进货周期:" prompt="进货周期" style="width:100%" name="Period" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="实发数量:" prompt="实发数量" style="width:100%" name="ApprovalNumber" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="备注:" prompt="备注" style="width:100%;height:100px" name="Remark" data-options="editable:false,multiline:true">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="更新日期:" prompt="更新日期" style="width:100%" name="LMT" data-options="editable:false">
                    </div>
                </form>
            </div>
        </div>
    <script type="text/javascript">
        var url = "mDeliverRelated.aspx";
        $(document).ready(function () {
            getData();
        });
        function getData() {
            $.post(url, { act: 'getData' }, function (res) {
                if (res != "") {
                    var tempData = $.parseJSON(res);
                    $("#preview").empty();
                    for (i = 0; i < tempData.length; i++) {
                        var html = "";
                        html += '<div class="weui-form-preview">';
                        html += ' <div class="weui-form-preview__hd">';
                        html += '<label class="weui-form-preview__label">收货单位</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].hospitalName + '</span>';
                        html += '</div>';
                        html += '<div class="weui-form-preview__bd">';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品名称</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].productName + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品规格</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].spec + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">单位</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].unit + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品数量</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].DeliverNumber + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">备注</label>';
                        html += '<span class="weui-form-preview__value">' + (tempData[i].Remark == "" ? "无" : tempData[i].Remark) + '</span>';
                        html += '</p>';
                        html += '</div>';
                        html += '<div class="weui-form-preview__ft">';
                        html += "<a class='weui-form-preview__btn weui-form-preview__btn_primary' href='javascript:' onClick='ShowDetail(" + JSON.stringify(tempData[i]) + ")'>查看更多</a>";
                        html += '</div>';
                        html += '</div>';

                        $("#preview").append(html);
                    }
                } else {
                    $.messager.alert('提示', '暂无数据', 'info');
                }

            });
        }
        function ShowDetail(data) {
            $.mobile.go('#p2');
            switch (data.DeliverStyle) {
                case '0':
                    data.DeliverStyle = "发公司直营网点";
                    break;
                case '1':
                    data.DeliverStyle = "发商业单位";
                    break;
                case '2':
                    data.DeliverStyle = "发代理商";
                    break;
                case '3':
                    data.DeliverStyle = "外购";
                    break;
                case '4':
                    data.DeliverStyle = "借货";
                    break;
                case '5':
                    data.DeliverStyle = "赠品/样品";
                    break;
            }
            $('#fm').form('load', data);
        }
    </script>
</body>
</html>
