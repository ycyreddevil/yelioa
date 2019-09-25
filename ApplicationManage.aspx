<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ApplicationManage.aspx.cs" Inherits="ApplicationManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/base-loading.js"></script>
    <script src="Scripts/pcCommon.js"></script>
    <title></title>
</head>
<body>
    <table class="easyui-datagrid" id="dg" data-options="toolbar:'#tb'">
        <thead>
		<tr>
			<th data-options="field:'Id',width:100",align:'center'></th>
			<th data-options="field:'Application',width:200",align:'center'>应用页面</th>
			<th data-options="field:'IsValid',width:100,align:'center'">是否禁用</th>
		</tr>
    </thead>
   </table>
    <div id="tb">
        <a href="#" class="easyui-linkbutton" iconCls="icon-add"  onclick="$('#dlg').dialog('open')">添加</a>
         <a href="#" class="easyui-linkbutton" iconCls="icon-edit"  onclick="edit()">编辑</a>
    </div>
    <div id="dlg" class="easyui-dialog" style="width:250px" data-options="closed:true,buttons: [{
					text:'确定',
					iconCls:'icon-ok',
					handler:function(){
						sure();
					}
				},{
					text:'取消',
					handler:function(){
						$('#dlg').dialog('close');
					}
				}]">
        应用页面：<input id="Application" class="easyui-textbox" type="text" name="Application" data-options="required:true" />
        是否禁用：<select id="Isvalid" class="easyui-combobox" name="Isvalid">
                     <option value="启用" selected="selected">启用</option>
                     <option value="禁用">禁用</option>
                  </select>
    </div>
    <script type="text/javascript">
        var id = "";
        $(document).ready(function () {
               initDatagrid();
        });
        function initDatagrid() {
            $.post('ApplicationManage.aspx', { act: 'initDatagrid' }, function (res) {
                var data = JSON.parse(res);
                if (data.ErrCode == "0") {
                    var document = JSON.parse(data.document);
                    if (document.length == 0) {
                        alert("暂无数据");
                    }
                    else {
                        $('#dg').datagrid('loadData', document);
                    }
                }
                else {
                    alert(data.ErrMsg);
                }
            });
        }
        function sure() {
            var application = $('#Application').textbox('getValue');
            var isValid = $('#Isvalid').textbox('getValue');
            if (application == "" || application == null) {
                alert("应用网页不能为空");
            } else {
                $.post('ApplicationManage.aspx', { act: 'sure', id: id, application: application, IsValid: isValid }, function (res) {
                    var data = JSON.parse(res);
                    alert(data.ErrMsg);
                    $('#dlg').dialog('close');
                });
            }
        }
        function edit() {
            var row = $('#dg').datagrid('selected');
            if (row == null) {
                alert("请选择要编辑的行");
            }
            else {
                id = row["Id"];
                $('#Application').textbox('setValue', row["Application"]);
                $('#Isvalid').textbox('setValue', row["Isvalid"]);
                $('#dlg').dialog('open');
            }
        }
    </script>
</body>
</html>
