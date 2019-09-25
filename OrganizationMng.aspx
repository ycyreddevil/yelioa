<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrganizationMng.aspx.cs" Inherits="OrganizationMng" %>

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
    <script src="Scripts/citiselect.js"></script>
    <script src="Scripts/linq.js"></script>
    <style type="text/css">
        .easyui-textbox {
            width: 360px;
        }
        .easyui-combobox {
            width:110px;
        }
    </style>
</head>
<body>
    <table id="dg" class="easyui-datagrid" title="机构信息">
    </table>
    <div id="dg-tb">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-add'," onclick="OpenDlg('add')">新增机构</a>
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-remove'," onclick="Delete()">删除</a>
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-edit'," onclick="OpenDlg('edit')">编辑</a>
        <input class="easyui-searchbox" data-options="prompt:'机构编码或机构名称的拼音或文字',searcher:doSearch" style="width: 300px">
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-leaveStock" onclick="OpenFbxDialoge();">导入机构信息</a>
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
    <div id="dlg" class="easyui-dialog" title="机构信息" style="width: 600px; height: 600px; display: none"
        data-options="modal:true,closed:true">
        <div id="tt" class="easyui-tabs" data-options="plain:true,justified:true,pill:true,border:false,
        	onSelect:function(title,index){selectTab(title,index);}">
            <div title="基本信息" style="padding: 10px">
                <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
                    <table style="width: 100%;" cellpadding="5">
                        <tr>
                            <td style="width: 10%;">&nbsp;</td>
                            <td style="width: 20%;">&nbsp;</td>
                            <td style="width: 70%;">
                                <input name="Id" id="Id" type="hidden" />
                                <input name="act" id="act" type="hidden" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>行政区划</td>
                            <td>
                                <%--<input id="code" name="code" class="easyui-textbox" data-options="required:true" />--%>
                                <input id="province" name="province" type="text" class="easyui-combobox" />
                                <input id="city" name="city" type="text" class="easyui-combobox" />
                                <%--<input id="county" name="county" type="text" class="easyui-combobox" />--%>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>机构名称</td>
                            <td>
                                <input name="name" class="easyui-textbox" data-options="required:true" />
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>机构等级</td>
                            <td>
                                <select id="rank" class="easyui-combobox" name="rank" style="width:200px;">
                                    <option value="30">三甲</option>
                                    <option value="31">三乙</option>
                                    <option value="32">三级</option>
                                    <option value="20">二甲</option>
                                    <option value="21">二乙</option>
                                    <option value="22">二级</option>
                                    <option value="10">一甲</option>
                                    <option value="11">一乙</option>
                                    <option value="12">一级</option>
                                    <option value="13">其他</option>
                                    <option value="14">药店</option>
                                    <option value="15">商业</option>
                                </select>
                            </td>
                        </tr>
                        
                        <tr>
                            <td>&nbsp;</td>
                            <td>机构类别</td>
                            <td>
                                <select class="easyui-combobox" id="type" name="type" style="width: 360px;" data-options="required:true">
                                    <option value="医院">医院</option>
                                    <option value="经销商">经销商</option>
                                    <option value="供应商">供应商</option>
                                    <option value="配送商">配送商</option>
                                    <option value="其他">其他</option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>机构描述</td>
                            <td>
                                <input name="remark" class="easyui-textbox" style="height: 60px" />
                            </td>

                        </tr>

                    </table>
                </form>
            </div>
           <%-- <div title="转义" style="padding: 10px">
                <table id="dg-alias" title="机构转义"></table>
            </div>--%>
        </div>

    </div>
    <script type="text/javascript">
        var DlgState;
        $(document).ready(function () {
            InitDatagrid();
            dg_Load();
            $.citySelect({ $province: $('#province'), $city: $('#city'), $County: $('#county') });
            $("#province").combobox("setValue", "111")
            $("#city").combobox("setValue", "南昌市")
        });

        $.extend($.fn.combobox.methods, {
            setIndex: function (jq, index) {
                if (!index)
                    index = 0;
                var data = $(jq).combobox('options').data;
                var vf = $(jq).combobox('options').valueField;
                $(jq).combobox('setValue', eval('data[index].' + vf));
            },
            getIndex: function (jq) {
                var index = 0;
                var data = $(jq).combobox('options').data;
                var vf = $(jq).combobox('options').valueField;
                var value = $(jq).combobox('getValue');
                if (data != null && data.length != null) {
                    for (var i = 0; i < data.length; i++) {
                        if (value == eval('data[i].' + vf))
                            index = i;
                    }
                }
               
                return index;
            }
        });


        function InitDatagrid() {
            //主页面表格初始化
            $('#dg').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#dg-tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: 'code', width: 10, align: 'center', title: '编号', sortable: "true" },
                    { field: 'name', width: 30, align: 'center', title: '名称', sortable: "true" },
                    { field: 'rank', width: 10, align: 'center', title: '医院等级', sortable: "true" },
                    { field: 'city', width: 10, align: 'center', title: '地级市', sortable: "true" },
                    { field: 'administrativeArea', width: 10, align: 'center', title: '行政区域', sortable: "true" }
                ]],
                onDblClickRow: function (index, row) { OpenDlg('edit'); }
            });

            //机构转义表初始化
            $('#dg-alias').edatagrid({
                singleSelect: true, fit: false,
                toolbar: [{
                    text: '新增转义',
                    iconCls: 'icon-add',
                    handler: function () {
                        $('#dg-alias').edatagrid('addRow', { row: { code: $('#code').textbox('getValue') } });
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
                fitColumns: true, idField: "Id",
                saveUrl: 'OrganizationMng.aspx?act=saveAliasData',
                updateUrl: 'OrganizationMng.aspx?act=updateAliasData',
                destroyUrl: 'OrganizationMng.aspx?act=destroyAliasData',
                columns: [[
                    {
                        field: 'code', width: 10, align: 'center', title: '机构编号',
                        editor: { type: 'textbox', options: { editable: false } }
                    },
                    {
                        field: 'type', width: 10, align: 'center', title: '数据类型',
                        editor: {
                            type: 'combobox', options: {
                                valueField: 'type',
                                textField: 'type',
                                url: 'LeaveStock.aspx?act=getDataType',
                                method: 'post',
                            }
                        }
                    },
                    {
                        field: 'aliasCode', width: 10, align: 'center', title: '转义编号',
                        editor: { type: 'textbox', options: {} }
                    },
                    {
                        field: 'alias', width: 10, align: 'center', title: '转义名称',
                        editor: { type: 'textbox', options: {} }
                    }
                ]]
            });

            $('#dlg').dialog({
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok', size: 'large',
                    handler: function () {
                        Submit();
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel', size: 'large',
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

        function selectTab(title, index) {
            if (index == 1) {
                dgAliasLoad();
            }
        }

        function dgAliasLoad() {
            var url = 'OrganizationMng.aspx?act=getAliasData&code=' + $('#code').textbox('getValue');
            $('#dg-alias').datagrid({ url: url }).datagrid('load');
        }

        function dg_Load(search) {
            var url = "OrganizationMng.aspx";
            if (search == undefined) {
                search = "";
            }
            var data = {
                act: 'getData', search: search
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
                parent.Loading(false);
            });
        }

        function OpenDlg(state) {
            DlgState = state;
            $('#tt').tabs('select', 0);
            if (state == 'add') {
                // 设置默认值为江西南昌
                $("#province").combobox("setValue", "江西省")
                $("#city").combobox("setValue", "南昌市")
                $('#type').combobox('setValue', '医院');
            }
            else if (state == 'edit') {
                var row = $('#dg').datagrid('getSelected');
                if (row == null) {
                    $.messager.alert("提示", "请先选择一条记录!", "info");
                    return;
                }
                else {
                    $('#fm').form('load', row);
                }
            }
            $('#act').val(state);
            $('#dlg').dialog('open');
            $('#fm').form('disableValidation');
        }

        function Submit() {
            $('#fm').form('submit', {
                url: 'OrganizationMng.aspx',
                onSubmit: function () {
                    var res = $(this).form('enableValidation').form('validate');
                    //if (res) {
                    //    if (DlgState == 'add') {
                    //        var codeValue = $('#code').textbox('getValue');
                    //        var validate = AjaxSync('OrganizationMng.aspx', {
                    //            act: 'checkCode', code: codeValue
                    //        });
                    //        if (validate == "F") {
                    //            $.messager.alert("提示", "机构编码已存在，请重新输入!", 'info');
                    //            $('#code').textbox("setValue", "");
                    //            $('#code').focus();
                    //            res = false;
                    //            return res;
                    //        }
                    //    }       
                    //    parent.Loading(true);
                    //}
                    return res;
                },
                success: function (res) {
                    parent.Loading(false);
                    $.messager.alert({
                        title: '提示', msg: res, icon: 'info',
                        fn: function () {
                            $('#dlg').dialog('close');
                            dg_Load('');
                        }
                    });

                }
            });
        }
        function Delete() {

            var row = $('#dg').datagrid('getSelected');
            if (row == null) {
                $.messager.alert('提示', '请先选择一条记录!', 'info');
            }
            else {
                $.messager.confirm('提示', '确定删除 ' + row.name + ' 的数据?', function (r) {
                    if (r) {
                        var url = "OrganizationMng.aspx";
                        var data = {
                            act: 'del',
                            id: row.Id
                        };
                        parent.Loading(true);
                        $.post(url, data, function (res) {
                            parent.Loading(false);
                            dg_Load('');
                        });
                    }
                });
            }
        }

        function doSearch(value) {
            dg_Load(value);
        }

        function OpenFbxDialoge() {
            $('#dlg-Import').dialog('open');
            $('#fmFile').form('clear');
        }
        var Url = "OrganizationMng.aspx";
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
