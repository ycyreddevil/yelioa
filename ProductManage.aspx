<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProductManage.aspx.cs" Inherits="ProductManage" %>

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
    <script src="Scripts/jquery.edatagrid.js"></script>
    <script src="Scripts/base-loading.js"></script>
    <style type="text/css">
        .easyui-textbox {
            width: 280px;
        }
    </style>
</head>

<body>
    <table id="dg" class="easyui-datagrid" title="产品信息">
    </table>
    <div id="dg-tb">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-add'," onclick="DgToolBarProcess('add')">新增产品</a>
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-remove'," onclick="DgToolBarProcess('del')">删除</a>
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-edit'," onclick="DgToolBarProcess('edit')">编辑</a>
        <input class="easyui-searchbox" data-options="prompt:'编码或名称或供应商的拼音或文字',searcher:doSearch" style="width: 300px">
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-leaveStock" onclick="OpenFbxDialoge();">导入产品信息</a>
    </div>

    <div id="dlg" class="easyui-dialog" title="产品信息管理" style="width: 600px; height: 720px; display: none" data-options="iconCls:'icon-import',modal:true,closed:true">
        <div id="tt" class="easyui-tabs" data-options="plain:true,justified:true,pill:true,border:false,
        onSelect:function(title,index){selectTab(title,index);}">
            <div title="基本信息" style="padding: 10px">
                <form id="fm" <%--style="width:100%;height:100%" --%> class="easyui-form" method="post" data-options="novalidate:true">
                    <table style="width: 100%;" cellpadding="5">
                        <tr>
                            <td style="width: 10%;">&nbsp;</td>
                            <td style="width: 30%;">&nbsp;</td>
                            <td style="width: 60%;">
                                <input id="Id" name="Id" type="hidden" /><input id="act" name="act" type="hidden" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            .
								<td>产品编码</td>
                            <td>
                                <input id="code" name="code" class="easyui-textbox" data-options="required:true" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>产品名称</td>
                            <td>
                                <input name="name" class="easyui-textbox" data-options="required:true" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>产品全名</td>
                            <td>
                                <input name="fullName" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>规格型号</td>
                            <td>
                                <input name="specification" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>保质期(天)</td>
                            <td>
                                <input name="shelfLife" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>存货科目代码</td>
                            <td>
                                <input name="stockAccountCode" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>销售收入科目代码</td>
                            <td>
                                <input name="salesIncomeAccountCode" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>销售成本科目代码</td>
                            <td>
                                <input name="salesCostAccountCode" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>税率（%）</td>
                            <td>
                                <input name="taxRate" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>生产企业许可证号或备案凭证号</td>
                            <td>
                                <input name="manufacturerLicenseNumber" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>储运条件</td>
                            <td>
                                <input name="storageCondition" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>产地</td>
                            <td>
                                <input name="placeOfProduction" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>生产厂家</td>
                            <td>
                                <input name="manufacturer" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>产品注册证号或备案凭证号</td>
                            <td>
                                <input name="productLicenseNumber" class="easyui-textbox" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>产品描述</td>
                            <td>
                                <input name="remark" class="easyui-textbox" style="height: 60px" />
                            </td>

                        </tr>

                    </table>
                </form>
            </div>
            <div title="转义" style="padding: 10px">
                <table id="dg-alias" title="产品转义"></table>
            </div>

        </div>
    </div>
    <div id="dlg-Import" class="easyui-dialog" title="产品信息导入" style="width: 800px; height: 60px"
        data-options="iconCls:'icon-import',modal:true,closed:true">
        <form id="fmFile" method="post" enctype="multipart/form-data">
            <input id="fbx" class="easyui-filebox" label="产品信息文件:" labelposition="left"
                data-options="onChange:function(){uploadFiles('upload');},prompt:'请选择一个xls文件...'"
                style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				<input type="hidden" name="act" id="actFbx" />
        </form>
    </div>

    <script type="text/javascript">
        var DlgState = "";
        var IsLoadingNow = false;
        var Url = "ProductManage.aspx";

        $(document).ready(function () {
            InitDatagrid();
            dg_Load();
        });

        function InitDatagrid() {
            //主页面表格初始化
            $('#dg').datagrid({
                singleSelect: true,
                fit: true,
                toolbar: '#dg-tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [
                    [{
                        field: 'code',
                        width: 10,
                        align: 'center',
                        title: '产品编号'
                    },
                        {
                            field: 'name',
                            width: 20,
                            align: 'center',
                            title: '产品名称'
                        },
                        {
                            field: 'specification',
                            width: 10,
                            align: 'center',
                            title: '规格型号'
                        },
                        {
                            field: 'shelfLife',
                            width: 5,
                            align: 'center',
                            title: '保质期'
                        },
                        {
                            field: 'stockAccountCode',
                            width: 5,
                            align: 'center',
                            title: '存货科目代码'
                        },
                        {
                            field: 'salesIncomeAccountCode',
                            width: 5,
                            align: 'center',
                            title: '销售收入科目代码'
                        },
                        {
                            field: 'salesCostAccountCode',
                            width: 5,
                            align: 'center',
                            title: '销售成本科目代码'
                        },
                        {
                            field: 'taxRate',
                            width: 5,
                            align: 'center',
                            title: '税率（%）'
                        },
                        //{ field: 'manufacturerLicenseNumber', width: 10, align: 'center', title: '生产企业许可证号或备案凭证号' },
                        {
                            field: 'manufacturer',
                            width: 10,
                            align: 'center',
                            title: '生产厂家'
                        },
                        {
                            field: 'productLicenseNumber',
                            width: 10,
                            align: 'center',
                            title: '产品注册证号或备案凭证号'
                        },
                        {
                            field: 'remark',
                            width: 10,
                            align: 'center',
                            title: '产品描述'
                        }
                    ]
                ],
                onDblClickRow: function (index, row) { DgToolBarProcess('edit'); }
            });

            //产品转义表初始化
            $('#dg-alias').edatagrid({
                singleSelect: true,
                fit: false,
                toolbar: [{
                    text: '新增转义',
                    iconCls: 'icon-add',
                    handler: function () {
                        $('#dg-alias').edatagrid('addRow', {
                            row: {
                                productCode: $('#code').textbox('getValue')
                            }
                        });
                    }
                }, {
                    text: '删除',
                    iconCls: 'icon-remove',
                    handler: function () {
                        $('#dg-alias').edatagrid('destroyRow');
                    }
                }, {
                    text: '保存',
                    iconCls: 'icon-save',
                    handler: function () {
                        $('#dg-alias').edatagrid('saveRow');
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-undo',
                    handler: function () {
                        $('#dg-alias').edatagrid('cancelRow');
                    }
                }, {
                    text: '刷新',
                    iconCls: 'icon-reload',
                    handler: function () {
                        dgAliasLoad();
                    }
                }],
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                idField: "Id",
                //url: 'ProductManage.aspx?act=getAliasData&code=' + $('#code').textbox('getValue'),
                saveUrl: 'ProductManage.aspx?act=saveAliasData',
                updateUrl: 'ProductManage.aspx?act=updateAliasData',
                destroyUrl: 'ProductManage.aspx?act=destroyAliasData',
                columns: [
                    [{
                        field: 'productCode',
                        width: 10,
                        align: 'center',
                        title: '产品编号',
                        editor: {
                            type: 'textbox',
                            options: {
                                editable: false
                            }
                        }
                    },
                        {
                            field: 'type',
                            width: 10,
                            align: 'center',
                            title: '数据类型',
                            editor: {
                                type: 'combobox',
                                options: {
                                    valueField: 'type',
                                    textField: 'type',
                                    url: 'LeaveStock.aspx?act=getDataType',
                                    method: 'post',
                                }
                            }
                        },
                        {
                            field: 'aliasCode',
                            width: 10,
                            align: 'center',
                            title: '转义编号',
                            editor: {
                                type: 'textbox',
                                options: {}
                            }
                        },
                        {
                            field: 'alias',
                            width: 10,
                            align: 'center',
                            title: '转义名称',
                            editor: {
                                type: 'textbox',
                                options: {}
                            }
                        }
                    ]
                ]
            });

            //弹出框初始化           

            $('#dlg').dialog({
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok',
                    size: 'large',
                    handler: function () {
                        if (DlgState == 'add') {
                            Submit(DlgState);
                        } else if (DlgState == 'del') {
                            Submit(DlgState);
                        } else if (DlgState == 'edit') {

                            Submit(DlgState);
                        }
                    }
                }, {
                    text: '清除',
                    iconCls: 'icon-clear',
                    size: 'large',
                    handler: function () {
                        $('#fm').form('clear');
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel',
                    size: 'large',
                    handler: function () {
                        $('#dlg').dialog('close');
                    }
                }],
                onOpen: function () {
                    var h = $(window).height() - 60;
                    $(this).dialog('resize', { height: h });
                    $(this).dialog('move', { top: 30 });
                }
            });
        }

        function dg_Load() {
            var url = "ProductManage.aspx";
            var data = {
                act: 'getData',
            };
            Loading(true);
            $.post(url, data, function (res) {
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
                Loading(false);
            });
        }

        function selectTab(title, index) {
            if (index == 1) {
                dgAliasLoad();
                //$('#dg-alias').datagrid('load');
            }
        }

        function dgAliasLoad() {
            var url = 'ProductManage.aspx?act=getAliasData&code=' + $('#code').textbox('getValue');
            $('#dg-alias').datagrid({
                url: url
            }).datagrid('load');
        }

        function Loading(OnOff) {
            parent.Loading(OnOff);
        }

        function Submit(action) {
            var url;
            var id = $('#Id').val();
            if (DlgState == 'add') {
                url = 'ProductManage.aspx?act=' + action;
            } else if (DlgState == 'edit') {
                url = 'ProductManage.aspx?act=' + action + '&Id=' + id;
            }
            $('#fm').form('submit', {
                url: url,
                title: '提示',
                text: '数据处理中，请稍后....',
                onSubmit: function () {
                    var res = $(this).form('enableValidation').form('validate');
                    var codeValue = $('#code').textbox('getValue');
                    var validate = $.ajax({
                        async: false,
                        cache: false,
                        type: 'post',
                        url: 'ProductManage.aspx',
                        data: {
                            act: 'validate',
                            code: codeValue,
                            DlgState: DlgState
                        }
                    }).responseText;
                    if ((DlgState == 'add' && validate.length > 0) || (DlgState == 'edit' && validate != id)) {
                        $.messager.alert("提示", "产品编码已存在，请重新输入产品编码!", 'info');
                        $('#code').textbox("setValue", "");
                        $('#code').focus();
                        res = false;
                    } 
                    if (res) {
                        Loading(true);
                    }
                    return res;
                },
                success: function (res) {
                    Loading(false);
                    $('#dlg').dialog('close');
                    $.messager.alert({
                        title: '提示',
                        msg: res,
                        icon: 'info',
                        fn: function () {
                            dg_Load();                            
                        }
                    });

                }
            });
        }

        function validate() {

        }

        function ShowEdit(row) {
            if (row == null) {
                $.messager.alert('提示', '请先选择一行数据!', 'info');
            } else {
                DlgState = "edit";
                $('#fm').form('clear');
                $('#dlg').dialog('open');
                $('#fm').form('load', row);
                $('#tt').tabs('select', 0);
            }
        }

        function ShowDelete(row) {
            if (row == null) {
                $.messager.alert('提示', '请先选择一行数据!', 'info');
            } else {
                var url = "ProductManage.aspx";
                var data = {
                    act: 'del',
                    id: row.Id
                };
                $.messager.confirm('My Title', '确定删除' + row.name + '?', function (r) {
                    if (r) {
                        $.post(url, data, function (res) {
                            dg_Load();
                        });
                    }
                });
            }
        }

        function DgToolBarProcess(type) {
            DlgState = type;
            if (type == 'del') {
                var row = $('#dg').datagrid('getSelected');
                ShowDelete(row);
            } else {
                if (type == 'add') {
                    $('#fm').form('disableValidation');
                    $('#fm').form('clear');
                    $('#dlg').dialog('open');
                    $('#tt').tabs('select', 0);
                } else if (type == 'edit') {
                    var row = $('#dg').datagrid('getSelected');
                    ShowEdit(row);
                }
            }
        }

        function doSearch(value) {
            var url = "ProductManage.aspx";
            var data = {
                act: 'getData',
                search: value
            };
            Loading(true);
            $.post(url, data, function (res) {
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
                Loading(false);
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
    </script>
</body>

</html>
