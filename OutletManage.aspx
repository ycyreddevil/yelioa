<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OutletManage.aspx.cs" Inherits="OutletManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>网点管理</title>
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
    <script src="Scripts/jquery.json-2.4.min.js"></script>
</head>

<body>
    <table id="dg" class="easyui-datagrid"></table>
    <div id="dg-tb">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-add',"
            onclick="dlgOpen('add')">新增</a>
        <%--<a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-remove'," onclick="Delete()">停用</a>--%>
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-edit',"
            onclick="dlgOpen('edit')">编辑</a>
        <input id="dg-searchbox" class="easyui-searchbox" data-options="prompt:'人员或部门或医院或产品的拼音或文字',searcher:doSearch"
            style="width: 300px">
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-leaveStock"
            onclick="OpenFbxDialoge();">导入批量信息</a>
    </div>
    <div id="dlg-budget-Import" class="easyui-dialog" title="预算导入" style="width: 800px; height: 80px"
        data-options="iconCls:'icon-import',modal:true,closed:true">
        <form id="fmFile" method="post" enctype="multipart/form-data">
            <input id="fbx" class="easyui-filebox" label="预算文件:" labelposition="left"
                data-options="prompt:'请选择一个xls文件...'" style="width: 50%" buttontext="请选择文件"
                accept='application/vnd.ms-excel' name="file">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <input type="hidden" name="act" id="actFbx" />
            <input type="hidden" name="departmentId" id="departmentIdFbx" />
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="ImportExcel()">提交</a>
        </form>
    </div>

    <script type="text/javascript">
        var Url = "OutletManage.aspx";
        var SearchFormType = "";
        var ValueSearched, dlgType, TreeDataJson, UploadIndex = -1, DgImportLength;
        var TotalImportNumber, ImportedNumber;


        $(document).ready(function () {
            InitTable();
        });

        function dgLoad() {

            var data = {
                act: 'getInfos'
            };
            parent.Loading(true);
            $.post(Url, data, function (res) {
                var datasource = $.parseJSON(res);
                parent.Loading(false);
                $('#dg').datagrid("loadData", datasource);
            });
        }

        function ImportExcel() {
            var fileName = $('#fbx').filebox('getValue');
            if (fileName == "") {
                return;
            }
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            } else {
                $('#actFbx').val('import');
                $('#departmentIdFbx').val(departmentId);
                parent.Loading(true);
                $('#fmFile').form('submit', {
                    url: url,
                    success: function (res) {
                        parent.Loading(false);
                        $.messager.alert('提示', res);
                    }
                });
            }
        }

        function InitTable() {
            $('#dg').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#dg-tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: 'Hospital', width: 20, align: 'center', title: '医院', sortable: "true" },
                    { field: 'Product', width: 20, align: 'center', title: '产品', sortable: "true" },
                    { field: 'Department', width: 10, align: 'center', title: '部门', sortable: "true" },
                    { field: 'Sector', width: 10, align: 'center', title: '区域', sortable: "true" },
                    { field: 'Sales', width: 10, align: 'center', title: '业务员', sortable: "true" },
                    { field: 'Supervisor', width: 10, align: 'center', title: '主管', sortable: "true" },
                    { field: 'Manager', width: 10, align: 'center', title: '经理', sortable: "true" },
                    { field: 'Director', width: 10, align: 'center', title: '总监', sortable: "true" }
                ]],
                onDblClickRow: function () { dlgOpen('edit'); },
                sortName: 'Hospital',
                sortOrder: 'asc',
                onSortColumn: function (sort, order) {
                    dgLoad($('#dg-searchbox').searchbox('getValue'), sort, order);
                }
            });
        }
    </script>
</body>

</html>