<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mUndeliverReport.aspx.cs" Inherits="mUndeliverReport" %>

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
    <div id="p1" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span class="m-title" >未发货表单</span>
            </div>       
        </header>
        <div id="tb" style="padding:5px;height:auto">
            请选择日期:<input id="db" type="month" />
        </div>
        <table id="dg" class="easyui-datagrid" 
            data-options="rownumbers:'true',fit:'true',striped:'true',fitColumns:'true',nowrap: false,toolbar:'#tb'">        
            <thead>
		        <tr>	
                    <th data-options="field:'UserName',align:'center'">申请人</th> 
			        <th data-options="field:'HospitalName',align:'center'">收货单位</th>                
			        <th data-options="field:'ProductName',align:'center'">产品名称</th>
                    <th data-options="field:'Specification',align:'center'">产品规格</th>
                    <th data-options="field:'UndeliverNumber',align:'center'">未发货数量</th>
                    <th data-options="field:'Unit',align:'center'">产品单位</th>
                    <th data-options="field:'Remark',align:'center'">备注</th>
                    <th data-options="field:'LMT',align:'center'">提交时间</th>
                    <th data-options="field:'Reason',align:'center'">未发货原因</th>
		        </tr>
	        </thead>
	    </table>
    </div>
    <script type="text/javascript">
        var year = new Date().getFullYear();
        var month = new Date().getMonth() + 1;
        var day = new Date().getDate();
        var dateStr;
        if (month.toString().length == 1) {
            dateStr = year + "-" + "0" + month;
        } else {
            dateStr = year + "-" + month;
        }
        $('#db').on("change", function () {
            $('#dg').datagrid('loadData', { total: 0, rows: [] });
            initdatagrid($('#db').val());
        })
        $(document).ready(function () {
            $("#db").val(dateStr)
            initdatagrid(dateStr);
        });
       

        function initdatagrid(date) {
            Loading(true);
            $.post('mUndeliverReport.aspx', { act: 'UndeliverReport',date:date}, function (res) {
                if (res != "") {
                    var tempData = $.parseJSON(res);
                    $('#dg').datagrid('loadData', tempData);
                }
                Loading(false);
            });
        }

    </script>
</body>
</html>
