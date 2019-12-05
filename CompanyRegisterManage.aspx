<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CompanyRegisterManage.aspx.cs" Inherits="CompanyRegisterManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>公司税号管理</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/pcCommon.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/base-loading.js"></script>
</head>

<body>
    <div id="dlg" class="easyui-dialog" title="详细信息"
        data-options="iconCls:'icon-edit',modal:true,closed:true,onOpen:function(){onOpenEventHandler('dlg')}">
        <!-- <div id="dlg-url" class="easyui-panel" style="width: 100%;height: 100%;"></div> -->
    </div>
</body>
<script>
    $(function () {
        // $('#dlg').dialog('refresh','CrudTemplate.aspx?table=fee_type_name');
        $("#dlg").dialog({
            content: "<iframe scrolling='no' frameborder='0' src='CrudTemplate.aspx?table=company_register_num' style='width:100%; height:100%;'></iframe>"
        });
        // $('#dlg-url').panel({href:'CrudTemplate.aspx?table=fee_type_name'});
        $('#dlg').dialog('open');
    })
</script>
</html>
