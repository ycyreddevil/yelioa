<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PostManage.aspx.cs" Inherits="PostManage" %>

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
    <script src="Scripts/pcCommon.js"></script>
</head>
<body class="easyui-layout">
    <div data-options="region:'west',split:true" title="岗位列表" style="width: 300px;">
        <table id="dg" class="easyui-datagrid" style="width:100%">
        </table>
    </div>
    <div data-options="region:'center',title:'岗位相关信息及权限设置'">
        <div id="divFm" class="easyui-panel" style="height: 100%; width:100%" data-options="
                    onCollapse: function(){FormToggle();}">
            <div class="easyui-layout" data-options="fit:true">
                <div data-options="region:'north'" title="岗位人员信息" style="height: 240px;padding:20px 30px">
                    <form id="fm" method="post" data-options="novalidate:true">
                        <div style="margin-bottom:20px">
                                <input type="hidden" name="postId" id="postId"/>
                            <div>岗位名称:</div>
                            <div>
                                <input id="postName" name="postName" class="easyui-textbox" required="true" style="width:100%">
                            </div>
                        </div>
                        <div style="margin-bottom:20px">
                            <div>备注:</div>
                            <div>
                                <input name="remark" class="easyui-textbox" style="width:100%;height:50px" multiline="true">
                            </div>
                        </div>                      
                    </form>
            
                    <div style="text-align:right">
                        <a href="javascript:void(0)" class="easyui-linkbutton c6" iconCls="icon-ok" onclick="save()" style="width:90px">保存</a>
                    </div>
                </div>
                <div data-options="region:'center',title:'岗位权限管理'">
                    <table id="dg-right" class="easyui-datagrid" style="width:100%;height: 100%">
                    </table>
                </div>
            </div>
        
        
        </div>
        
    </div>
    <div data-options="region:'east',split:true,iconCls:'icon-man'" title="岗位人员信息" style="width: 300px;">
            <table id="dg-member" class="easyui-datagrid" style="width:100%;height: 100%">
                </table>
    </div>
    <div id="dlg" class="easyui-dialog" style="width:800px" data-options="
				resizable:false,title:'岗位信息',
				modal:true,
		        closed:true,buttons: '#dlg-buttons',
		        onResize:function(){
		                $(this).dialog('center');
		            }">
        
    </div>
    <div id="dlg-buttons">
        <a href="javascript:void(0)" class="easyui-linkbutton c6" iconCls="icon-ok" onclick="save()" style="width:90px">保存</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" iconCls="icon-cancel" onclick="javascript:$('#dlg').dialog('close')"
             style="width:90px">取消</a>
    </div>
    <script type="text/javascript">
        var toolbar = [{
            text: '新建',
            iconCls: 'icon-add',
            handler: function () {
                EnableForm('add');
            }
        }, {
            text: '删除',
            iconCls: 'icon-remove',
            handler: function () {
                var row = $('#dg').datagrid('getSelected');
                Delete(row);
            }
        
        //}, '-', {
        //    text: '编辑',
        //    iconCls: 'icon-edit',
        //    handler: function () {
        //        var row = $('#dg').datagrid('getSelected');
        //        Edit(row);
        //    }
        }];
        var DlgState;
        var FormNeedToggle = false;
        var FormIsExpand = false;
        var Url = "PostManage.aspx";

        $(document).ready(function () {
            InitDatagrid();
            //$('#dlg').dialog('close');
            dg_Load();
            //TbLoad('副总经理');
        });

        

        function DgRightLoad(post) {
            var res = AjaxSync("PostManage.aspx", { act: 'TbLoad', post: post });
            var datasource = $.parseJSON(res);
            $('#dg-right').datagrid("loadData", datasource[0]);
            $('#dg-member').datagrid("loadData", datasource[1]);
        }

        function dg_Load() {
            var url = "PostManage.aspx";
            var data = {
                act: 'getPosts'
            };
            parent.Loading(true);
            $.post(url, data, function (res) {                
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
                parent.Loading(false);
            });
            FormExpand(false);
        }
        function InitDatagrid() {
            $('#dg').datagrid({
                singleSelect: true, fit: true,
                toolbar: toolbar,
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    //{ field: 'postId', width: 20, align: 'center', title: '编号' },
                    { field: 'postName', width: 20, align: 'center', title: '岗位名称' },
                    { field: 'counts', width: 10, align: 'center', title: '人数' },
                    { field: 'remark', width: 20, align: 'center', title: '备注' }
                ]],
                onSelect: function (index, row) {
                    EnableForm('edit');
                }
            });
            $('#dg-member').datagrid({
                singleSelect: true, 
                fit: true,
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    //{ field: 'postId', width: 20, align: 'center', title: '编号' },
                    { field: 'userName', width: 20, align: 'center', title: '人员名称' },
                    { field: 'department', width: 20, align: 'center', title: '部门' }
                ]]
            });
            $('#dg-right').datagrid({
                singleSelect: false, fit: true,
                //toolbar: toolbar,
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: 'typeName', width: 10, align: 'center', title: '分类' },
                    { field: 'pageName', width: 15, align: 'center', title: '页面' },
                    {
                        field: 'hasRight', checkbox: true, title: '权限',
                        formatter: function (value, row, index) {
                            if (row['hasRight'] == "1")
                                $('#dg-right').datagrid('checkRow', index);
                        }
                    }
                ]],
                onLoadSuccess: function (data) {
                    if (data) {
                        $.each(data.rows, function (index, item) {
                            if (item['hasRight'] == "1")
                                $('#dg-right').datagrid('checkRow', index);
                        });
                    }
                }
            });
        }

        function FormToggle() {
            if (FormNeedToggle) {
                FormNeedToggle = false;
                FormExpand(true);
            }
        }

        function FormExpand(OnOff) {
            if (OnOff) {
                $("#divFm").panel("expand", true);
                FormIsExpand = true;
            }
            else {
                FormIsExpand = false;
                $('#divFm').panel('collapse', true);
            }
        }

        function EnableForm(type) {
            var row = $('#dg').datagrid('getSelected');
            if (type == 'edit' && !row) {
                $.messager.alert({ title: '提示', msg: '请先选中一行数据', icon: 'info' });
                return;
            }
            DlgState = type;
            if (FormIsExpand) {
                FormNeedToggle = true;
                FormExpand(false);
            }
            else {
                FormExpand(true);
            }
            $('#fm').form('disableValidation');
            if (type == 'add') {
                $('#fm').form('clear');
                DgRightLoad();
            }
            else {
                $('#fm').form('load', row);
                DgRightLoad(row.postName);
            }
            
        }

        function GetFormData(formName) {
            var arr = $('#' + formName).serializeArray();
            if (arr.length == 0) {
                return undefined;
            }
            var obj = new Object;
            $.each(arr, function (k, v) {
                obj[v.name] = v.value;
            });
            return obj;
        }

        function GetRgihtsData() {
            var rows = $('#dg-right').datagrid('getRows');
            var checkedItems = $('#dg-right').datagrid('getChecked');
            if (rows.length == 0) {
                return undefined;
            }
            var obj = new Object;
            $.each(rows, function (k, v) {
                var hasFinded = false;
                $.each(checkedItems, function (i, vchecked) {
                    if (vchecked['pageName'] == v['pageName'] && vchecked['typeName'] == v['typeName']) {
                        hasFinded = true;
                        return true;
                    }
                });
                if (hasFinded)
                    rows[k]['hasRight'] = "1";
                else
                    rows[k]['hasRight'] = "0";
            });
            
            return rows;
        }

        function save()
        {
            if (!$('#fm').form('enableValidation').form('validate')) {
                return;
            }
            var data = {
                act:'save',
                state: DlgState,
                formData: JSON.stringify(GetFormData('fm')),
                rightsData: JSON.stringify(GetRgihtsData())
            };
            var res = AjaxSync(Url, data);
            $.messager.alert({
                title: '提示', msg: res, icon: 'info',
                fn: function () {
                    dg_Load();
                }
            });

            //$('#fm').form('submit', {
            //    url:Url,
            //    onSubmit: function () {
            //        var res = $(this).form('enableValidation').form('validate');                  
            //        if (res) {
            //            parent.Loading(true);
            //        }
            //        return res;
            //    },
            //    success: function (res) {                    
            //        parent.Loading(false);
            //        $.messager.alert({
            //            title: '提示', msg: res, icon: 'info',
            //            fn: function () {
            //                if (res.indexOf('新建成功') >= 0)
            //                {
            //                    $('#dlg').dialog('close');
            //                    dg_Load();
            //                }
            //            }
            //        });

            //    }
            //});
        }

        function Delete(row)
        {
            if (row == null) {
                $.messager.alert('提示', '请先选择一行数据!', 'info');
            }
            else {
                var url = "PostManage.aspx";
                var data = {
                    act: 'del',
                    id: row.postId,
                    postName:row.postName
                };
                $.messager.confirm('My Title', '确定删除' + row.postName + '?', function (r) {
                    if (r) {
                        $.post(url, data, function (res) {
                            if (res.indexOf('操作成功') > -1)
                            {
                                dg_Load();
                            }
                            else {
                                $.messager.alert('提示', res, 'info');
                            }
                        });
                    }
                });
            }
        }

        function Edit(row)
        {
            if (row == null) {
                $.messager.alert('提示', '请先选择一行数据!', 'info');
            }
            else {
                DlgState = "edit";
                dlgOpen();
                $('#fm').form('load', row);
                //TbLoad(row.postName);
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

        function dlgOpen() {
            $('#dlg').dialog('open');
            $('#fm').form('disableValidation');
            $('#fm').form('clear');
        }

        function TbLoad(post)
        {
            var res = AjaxSync("PostManage.aspx", { act: 'TbLoad', post: post });
            if (res == null)
            {
                return;
            }
            var table = $('#tb_right');
            var data = $.parseJSON(res);
            $.each(data, function (i, value) {
                var tr = $("<tr></tr>");
                tr.append($("<td>" + i + "</td>"));
                var tb = "<tb>";
                $.each(value, function (index, val) {
                    var text = '<p><input type="checkbox" name="pageName'+index+'" value="' + val.PageName + '"';
                    if (val.HasRigth == "1")
                    {
                        text += ' checked="true" /></p>';
                    }
                    else {
                        text += '/></p>';
                    }
                    tb += text;
                });
                tb += "</td>";
                tr.append($(tb));
                table.append(tr);
            });
            $.parser.parse(table);//重新渲染控件
        }
    </script>
</body>
</html>
