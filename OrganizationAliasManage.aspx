<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrganizationAliasManage.aspx.cs" Inherits="OrganizationAliasManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/base-loading.js"></script>
</head>
<body>
    <table id="dg" class="easyui-datagrid" title="机构信息"></table>
    <div id="dg-tb">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-remove'," onclick="DeleteRows();">删除</a>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-leaveStock" onclick="OpenFbxDialoge();">导入机构转义信息</a>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a href="Template/机构转义导入模板.xls" >下载机构转义导入模板</a>
    </div>
    <div id="dlg-Import" class="easyui-dialog" title="机构信息导入" style="width: 800px; height: 60px"
        data-options="iconCls:'icon-import',modal:true,closed:true">
        <form id="fmFile" method="post" enctype="multipart/form-data">
            <input id="fbx" class="easyui-filebox" label="机构信息文件:" labelposition="left"
                data-options="onChange:function(){uploadFiles('upload');},prompt:'请选择一个xls文件...'"
                style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<input type="hidden" name="act" id="actFbx" />
        </form>
    </div>
    <script type="text/javascript">
        var Url = "OrganizationAliasManage.aspx";

        $(document).ready(function () {
            InitDatagrid();
            dg_Load();
        });

        function InitDatagrid() {
            //主页面表格初始化
            $('#dg').datagrid({
                singleSelect: false,
                fit: true,
                toolbar:'#dg-tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: 'code', width: 5, align: 'center', title: '机构编号', sortable: "true" },
                    { field: 'name', width: 15, align: 'center', title: '机构名称' },
                    { field: 'fullName', width: 20, align: 'center', title: '机构全名' },
                    { field: 'type', width: 5, align: 'center', title: '数据类型', sortable: "true" },
                    { field: 'aliasCode', width: 5, align: 'center', title: '转义编号' },
                    { field: 'alias', width: 10, align: 'center', title: '转义名称' }
                ]],
                sortName: 'code',
                sortOrder: 'asc',
                onSortColumn: function (sort, order) {
                    dg_Load(sort, order);
                }
            });
        }

        function dg_Load(sort, order) {
            var data = { act: 'getData', sort: sort, order: order };
            var res = AjaxSync(Url, data);
            var data = $.parseJSON(res);
            $('#dg').datagrid("loadData", data);
        }


        function DeleteRows() {
            var rows = $('#dg').datagrid("getSelections");
            if (rows.length == 0) {
                $.messager.alert('提示', '请先选择一行数据!', 'info');
            }
            $.messager.confirm('My Title', '确定删除所选行数据?', function (r) {
                if (r) {
                    var ids = "";
                    $.each(rows, function (index, value) {
                        ids += value.Id + ',';
                    });
                    var data = { act: 'delete', ids: ids };
                    parent.Loading(true);
                    $.post(Url, data, function (res) {
                        parent.Loading(false);
                        dg_Load();
                    });
                }
            });            
        }

        function OpenFbxDialoge() {
            $('#dlg-Import').dialog('open');
            $('#fmFile').form('clear');
        }

        function uploadFiles(type) {
            var fileName = $('#fbx').filebox('getValue');
            if (fileName == "") {
                return;
            }
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            } else {
                $('#actFbx').val(type);
                parent.Loading(true);
                $('#fmFile').form('submit', {
                    url: Url,
                    success: function (res) {
                        parent.Loading(false);
                        $.messager.alert('提示', res, 'info', function () {
                            $('#dlg-Import').dialog('close');
                            dg_Load();
                        });
                    }
                });
            }
        }

        function AjaxSync(url, data) {
            parent.Loading(true);
            var res = $.ajax({
                async: false,
                cache: false,
                type: 'post',
                url: url,
                data: data
            }).responseText;
            parent.Loading(false);
            return res;
        }

    </script>
</body>
</html>
