<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MemberManage.aspx.cs" Inherits="MemberManage" %>

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
        table input, table select {
            width: 260px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'west',split:true" title="组织架构" style="width: 300px;">
        <div>
             <div style="margin: 10px 0px 10px 0px; text-align: center;">
                <a href="javascript:void(0)" class="easyui-linkbutton c8" <%--style="width: 80%"--%> onclick="dlgDepartOpen('add');"
                    data-options="iconCls:'icon-add'">新增部门</a>               
                <a href="javascript:void(0)" class="easyui-linkbutton c8" onclick="dlgDepartOpen('edit');"
                    data-options="iconCls:'icon-edit'">编辑部门</a>
            </div> 
            <div style="margin: 10px 0px 10px 0px; text-align: center;">
                <a href="javascript:void(0)" class="easyui-linkbutton c8" onclick="UpdateFromWx('department');"
                    data-options="iconCls:'icon-reload'">从企业微信同步部门信息</a>
            </div>
        </div>
        <hr />
        <ul id="tree" class="easyui-tree"></ul>
    </div>
    <div data-options="region:'center',title:'人员信息',iconCls:'icon-man'">
        <table id="dg" class="easyui-datagrid"></table>
        <div id="dg-tb">
            <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-add'," onclick="dlgOpen('add')">新增人员</a>
            <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-remove'," onclick="Delete()">删除</a>
            <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-edit'," onclick="dlgOpen('edit')">编辑</a>
            <input class="easyui-searchbox" data-options="prompt:'人员名字或工号的拼音或文字',searcher:doSearch" style="width: 300px">
            &nbsp;&nbsp;&nbsp;&nbsp;
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-leaveStock" onclick="OpenFbxDialoge();">导入人员信息</a>
            <a href="javascript:void(0)" class="easyui-linkbutton"  onclick="UpdateFromWx('users');">从企业微信同步</a>
            <a href="javascript:void(0)" class="easyui-linkbutton"  onclick="OpenFbxDialoge();">增量同步到企业微信</a>
            <a href="javascript:void(0)" class="easyui-linkbutton"  onclick="OpenFbxDialoge();">覆盖同步到企业微信</a>
        </div>
        <div id='dlg-DepartPost' class="easyui-dialog" title="部门岗位设置" data-options="modal:true,closed:true" style="width:50%;height:100%;">
            <!-- <div class="easyui-layout" style="width:50%;height:100%;">
                <div data-options="region:'west',split:true" title="组织架构" style="width:300px;">
                    <ul id="tree-Department" class="easyui-tree"></ul>
                </div>
                <div data-options="region:'center',split:true" title="部门岗位信息">
                    <table id="dg-Department" class="easyui-datagrid"></table>
                </div>
            </div> -->
            
            
        </div>
        <!-- <div id="mm-DepartPost" class="easyui-menu" style="width:120px;">
        </div> -->
        <div id="dlg" class="easyui-dialog" title="人员信息" style="width: 600px; height: 650px; display: none"
            data-options="iconCls:'icon-man',modal:true,closed:true">
            <form id="fm" <%--style="width:100%;height:100%"--%> class="easyui-form" method="post" data-options="novalidate:true">
                <div id="tt" class="easyui-tabs" data-options="plain:true,justified:true,pill:true,border:false,
        onSelect:function(title,index){}">
                    <div title="基本信息" style="padding: 10px">
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 10%;">&nbsp;</td>
                                <td style="width: 20%;">&nbsp;</td>
                                <td style="width: 60%;">
                                    <input id="userId" name="userId" type="hidden" />
                                    <input id="actMember" name="act" type="hidden" />
                                </td>
                                <td style="width: 10%;">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>姓名</td>
                                <td>
                                    <input id="userName" name="userName" class="easyui-textbox" data-options="required:true" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>手机号</td>
                                <td>
                                    <input id="mobilePhone" name="mobilePhone" class="easyui-textbox" data-options="required:true" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>企业微信账号</td>
                                <td>
                                    <input id="wechatUserId" name="wechatUserId" class="easyui-textbox" data-options="required:true" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>密码</td>
                                <td>
                                    <!-- <input id="passWord" name="passWord" type="password" class="easyui-textbox" data-options="editable:false" /> -->
                                    <a href="javascript:void(0)">恢复默认密码</a>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>性别</td>
                                <td>
                                    <select name="sex" class="easyui-combobox">
                                        <option value="男" selected>男</option>
                                        <option value="女">女</option>
                                    </select>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>出生日期</td>
                                <td>
                                    <input name="birthday" class="easyui-datebox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>入职日期</td>
                                <td>
                                    <input name="hiredate" class="easyui-datebox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>转正日期</td>
                                <td>
                                    <input name="regularEmployeeDate" class="easyui-datebox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>身份证号</td>
                                <td>
                                    <input id="idNumber" name="idNumber" class="easyui-textbox" data-options="required:true" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>工号</td>
                                <td>
                                    <input id="employeeCode" name="employeeCode" class="easyui-textbox" data-options="required:true" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <!-- <tr>
                                <td>&nbsp;</td>
                                <td>部门</td>
                                <td>
                                    <input id="department" name="department" class="easyui-textbox" data-options="required:true,editable:false" />
                                    <a href="javascript:void(0)" class="easyui-linkbutton"  onclick="OpenDepartPostDialoge();">设置部门岗位信息</a>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>岗位</td>
                                <td>
                                    <input id="post" name="post" class="easyui-textbox" data-options="required:true,editable:false"/>
                                </td>
                                <td>&nbsp;</td>
                            </tr> -->
                            <tr>
                                <td>&nbsp;</td>
                                <td>人员状态</td>
                                <td>
                                    <select name="isValid" class="easyui-combobox">
                                        <option value="在职" selected>在职</option>
                                        <option value="离职">离职</option>
                                        <option value="试用">试用</option>
                                        <option value="过渡">过渡</option>
                                    </select>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>备注</td>
                                <td>
                                    <input name="remark" class="easyui-textbox" style="height: 60px" multiline="true" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>

                        </table>
                    </div>
                    <div title="个人信息" style="padding: 10px">
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 10%;">&nbsp;</td>
                                <td style="width: 20%;">&nbsp;</td>
                                <td style="width: 60%;">                                    
                                </td>
                                <td style="width: 10%;">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>毕业学校</td>
                                <td>
                                    <input name="graduationSchool" class="easyui-textbox"/>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>专业</td>
                                <td>
                                    <input name="major" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>学历</td>
                                <td>
                                    <input name="education" class="easyui-textbox"/>
                                </td>
                                <td>&nbsp;</td>
                            </tr>    
                            <tr>
                                <td>&nbsp;</td>
                                <td>私人邮箱</td>
                                <td>
                                    <input name="email" class="easyui-textbox"  />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>企业qq</td>
                                <td>
                                    <input name="enterpriseQQ" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>企业邮箱</td>
                                <td>
                                    <input name="enterpriseEmail" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>家庭住址</td>
                                <td>
                                    <input name="address" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>紧急联络人</td>
                                <td>
                                    <input name="emergencyContact" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>紧急联络人电话</td>
                                <td>
                                    <input name="emergencyContactNumber" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>社保个人编号</td>
                                <td>
                                    <input name="socialSecurityNumber" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>开户银行</td>
                                <td>
                                    <input name="bank" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>银行账户</td>
                                <td>
                                    <input name="bankAccount" class="easyui-textbox" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>

                        </table>
                    </div>
                    <div title="部门岗位设置" style="padding: 10px">
                        <table id="dg-DepartPost" title="部门岗位列表"></table>
                    </div>
                </div>
            </form>
        </div>
        <div id="dlg-Import" class="easyui-dialog" title="人员信息导入" style="width: 800px; height: 60px"
            data-options="iconCls:'icon-import',modal:true,closed:true">
            <form id="fmFile" method="post" enctype="multipart/form-data">
                <input id="fbx" class="easyui-filebox" label="人员信息文件:" labelposition="left"
                    data-options="onChange:function(){uploadFiles('upload');},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				    <input type="hidden" name="act" id="actFbx" />
            </form>
        </div>
    </div>

    <div id="dlg_depart" class="easyui-dialog" title="组织架构" style="width: 460px; height: 240px; display: none"
        data-options="modal:true,closed:true">
        <table style="width: 100%;">
            <tr>
                <td style="width: 10%;">&nbsp;</td>
                <td style="width: 20%;">上级部门</td>
                <td style="width: 60%;">
                    <input id="parentDepart" class="easyui-combotree" style="width: 100%;" />
                </td>
                <td style="width: 10%;">&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>部门名称</td>
                <td>
                    <input id="DepartName" class="easyui-textbox" data-options="required:true" style="width: 100%;" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>部门描述</td>
                <td>
                    <input id="DepartRemark" class="easyui-textbox" style="width: 100%; height: 60px" multiline="true" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>部门负责人</td>
                <td>
                    <input id="departmentLeader" name="departmentLeader" value="">
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        var DlgState, DlgDepartState;
        var company;
        var TreeDataJson;
        var SelectedNode;
        $(document).ready(function () {
            InitDlgs();
            InitDatagrid();
            InitTree();
            TreeLoad();
        });
        function initDepartmentMember() {
            $.ajax({
                url: 'MemberManage.aspx',
                data: {act: 'getDepartmentMember', departmentId: $('#tree').tree('getSelected').id},
                type: 'post',
                dataType:'json',
                success: function(json) {
                    $("#departmentLeader").combobox({
                        data : json,
                        valueField: 'wechatUserId',
                        textField: 'userName'
                    });
                    $("#departmentLeader").combobox({ editable: false}).combobox({panelHeight:'auto'});
                }
            })
        }

        function InitDlgs() {
            $('#dlg').dialog({//用户对话框
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok',
                    handler: function () {
                        Submit();
                    }
                }, {
                    text: '清除',
                    iconCls: 'icon-clear', 
                    handler: function () {
                        var userId;
                        if (DlgState == 'edit') {
                            userId = $('#userId').val();
                        }
                        $('#fm').form('clear');
                        if (DlgState == 'edit') {
                            $('#userId').val(userId);
                        }
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel', 
                    handler: function () {
                        $('#dlg').dialog('close');$('#tt').tabs("select","基本信息");
                    }
                }],
                onOpen: function () {
                    var h = $(window).height() - 60;
                    $(this).dialog('resize', { height: h });
                    $(this).dialog('move', { top: 30 });
                    $('#tt').tabs("select","基本信息");
                }
            });

            $('#dlg_depart').dialog({//组织架构对话框
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok',
                    handler: function () {
                        if (DlgDepartState == 'add') {
                            AddTree();
                        }
                        else if (DlgDepartState == 'edit') {
                            EidtTree();
                        }
                    }
                }, {
                    text: '删除部门',
                    iconCls: 'icon-clear',
                    handler: function () {
                        if (DlgDepartState == 'edit') {
                            DeleteTree();
                        }
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        $('#dlg_depart').dialog('close');
                    }
                }]
            });
        }
        function AddTree() {
            var url = "MemberManage.aspx";
            var np = $('#parentDepart').combotree('tree').tree('getSelected');
            var data = {
                act: 'addTree',
                parentId: np.id,
                parentName: np.text,
                name: $('#DepartName').textbox('getValue'),
                remark: $('#DepartRemark').textbox('getValue')
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                parent.Loading(false);
                $.messager.alert("提示", res, "info", function () {
                    if (res == "新建成功") {
                        TreeLoad();
                        $('#dlg_depart').dialog('close');
                    }
                });
            });
        }

        function EidtTree() {
            var url = "MemberManage.aspx";
            var np = $('#parentDepart').combotree('tree').tree('getSelected');
            var data = {
                act: 'editTree',
                parentId: np.id,
                parentName: np.text,
                name: $('#DepartName').textbox('getValue'),
                remark: $('#DepartRemark').textbox('getValue'),
                id: $('#tree').tree('getSelected').id,
                departmentId: $('#tree').tree('getSelected').id,
                leaderWechatUserId:$("#departmentLeader").combobox("getValue"),
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                parent.Loading(false);
                $.messager.alert("提示", res, "info", function () {
                    if (res == "操作成功") {
                        TreeLoad();
                        $('#dlg_depart').dialog('close');
                    }
                });
            });
        }

        function DeleteTree() {
            var url = "MemberManage.aspx";
            var data = {
                act: 'deletTree',
                id: $('#tree').tree('getSelected').id
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                parent.Loading(false);
                $.messager.alert("提示", res, "info", function () {
                    if (res == "操作成功") {
                        TreeLoad();
                        $('#dlg_depart').dialog('close');
                    }
                });
            });
        }

        function InitTree() {
            $('#tree').tree({
                animate: true, lines: false,
                onClick: function (node) {
                    //$(this).tree('beginEdit', node.target);//点击可编辑
                    SelectedNode = node;
                    dg_Load(node.id);
                },
                formatter: function (node) {
                    var s = node.text;
                    // s += '&nbsp;<span style=\'color:blue\'>(' + node.MemberNumber + ')</span>';
                    return s;
                }
            });

            $('#tree-Department').tree({
                animate: true, lines: false,checkbox:true,cascadeCheck:false,
                onContextMenu: function(e,node){
                    e.preventDefault();
                    $(this).tree('select',node.target);
                    // $(this).tree('check',node.target);
                    $('#mm-DepartPost').menu('show',{
                        left: e.pageX,
                        top: e.pageY
                    });
                },
                onCheck:function(node, checked){
                    if(!checked){

                    }
                }
            });
        }
        function InitDatagrid() {
            $('#dg').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#dg-tb',
                //    [{
                //    text: '新建用户',
                //    iconCls: 'icon-add',
                //    handler: function () { dlgOpen('add'); }
                //}, {
                //    text: '删除',
                //    iconCls: 'icon-remove',
                //    handler: function () { Delete(); }
                //}, {
                //    text: '编辑',
                //    iconCls: 'icon-edit',
                //    handler: function () { dlgOpen('edit'); }
                //}],
                striped: true,               
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[               
                    { field: 'userName', width: 10, align: 'center', title: '姓名' },
                    { field: 'sex', width: 5, align: 'center', title: '性别' },
                    { field: 'mobilePhone', width: 15, align: 'center', title: '手机号' },
                    { field: 'employeeCode', width: 10, align: 'center', title: '工号' },
                    //{field:'idNumber',width:20,align:'center',title:'身份证号'},
                    //{ field: 'company', width: 30, align: 'center', title: '公司' },
                    { field: 'department', width: 30, align: 'center', title: '部门' },
                    { field: 'post', width: 15, align: 'center', title: '岗位' },
                    {
                        field: 'hiredate', width: 15, align: 'center', title: '入职时间',
                        formatter: function (value, row, index) {
                            if (value == null || value == '') {
                                return '';
                            }
                            var dt;
                            if (value instanceof Date) {
                                dt = value;
                            } else {
                                dt = new Date(value);
                            }

                            var res = dt.getFullYear() + '年' + (dt.getMonth() + 1) + '月' + dt.getDate() + '日';
                            return res;
                        }
                    },
                    {
                        field: 'isValid', title: '状态', width: 10, align: 'center',
                        formatter: function (value, row, index) {
                            if (value == '在职') {
                                return '<span style="color: #FFFFFF; background-color: #00CC00"> ' + value + ' </span>';
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #FF0000"> ' + value + ' </span>';
                            }
                        }
                    },
                    { field: 'remark', width: 10, align: 'center', title: '备注' }
                ]],
                onDblClickRow: function () { dlgOpen('edit');}
            });

            $('#dg-DepartPost').edatagrid({
                singleSelect: true, fit: false,striped: true,rownumbers: true,collapsible: false,fitColumns: true,
                toolbar: [{
                    text: '新增',
                    iconCls: 'icon-add',
                    handler: function () {
                        $('#dg-DepartPost').edatagrid('addRow',
                         { row: { wechatUserId: $('#wechatUserId').textbox('getValue') } });
                    }
                }, {
                    text: '删除',
                    iconCls: 'icon-remove',
                    handler: function () {
                        $('#dg-DepartPost').edatagrid('destroyRow');
                    }
                }, {
                    text: '保存',
                    iconCls: 'icon-save',
                    handler: function () {
                        $('#dg-DepartPost').edatagrid('saveRow');
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-undo',
                    handler: function () {
                        $('#dg-DepartPost').edatagrid('cancelRow');
                    }
                }, {
                    text: '刷新',
                    iconCls: 'icon-reload',
                    handler: function () {
                        DgDepartPostLoad();
                    }
                }],
                columns: [[
                    { field: 'department', width: 10, align: 'center', title: '部门',
                        editor:{
                            type:'combotree',
                            options:{
                                required:true,
                                url:'MemberManage.aspx?act=getTree',method:'get'
                            }
                        }
                     },
                    { field: 'postName', width: 10, align: 'center', title: '岗位',
                        editor:{
                            type:'combobox',
                            options:{
                                required:true,valueField:'postId',textField:'postName',
                                url:'MemberManage.aspx?act=getPosts'
                            }
                        }
                     }
                ]],
                idField: "Id",
                saveUrl: 'MemberManage.aspx?act=saveDepartPostData',
                updateUrl: 'MemberManage.aspx?act=updateDepartPostData',
                destroyUrl: 'MemberManage.aspx?act=destroyDepartPostData',
            });
        }

        function TreeLoad(selectedId) {
            var url = "MemberManage.aspx";
            var data = {
                act: 'getTree'
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                parent.Loading(false);
                if (res != "F") {
                    TreeDataJson = res;
                    var datasource = $.parseJSON(res);
                    $('#tree').tree("loadData", datasource);
                    company = $('#tree').tree('getRoot').text;
                    if (selectedId == null) {
                        selectedId = $('#tree').tree('getRoot').id;
                        SelectedNode = $('#tree').tree('getRoot');
                    }
                    dg_Load(selectedId);
                }
            });
        }

        function PostLoad() {
            var url = "MemberManage.aspx";
            var data = {
                act: 'getPosts'
            };
            parent.Loading(true);
            var res = AjaxSync(url, data);
            parent.Loading(false);
            var datasource = $.parseJSON(res);
            $('#post').combobox('loadData', datasource);
            //$.post(url, data, function (res) {
            //    parent.Loading(false);
            //    var datasource = $.parseJSON(res);

            //});
        }

        function dlgOpen(state) {
            DlgState = state;
            // $('#department').combotree("loadData", $.parseJSON(TreeDataJson));

            if (state == 'add') {
                $('#fm').form('reset');
                $('#passWord').textbox('setValue', '888888');
                $('select option:first').prop("selected", 'selected');
            }
            else if (state == 'edit') {
                var row = $('#dg').datagrid('getSelected');
                if (row == null) {
                    $.messager.alert("提示", "请先选择一位人员!", "info");
                    return;
                }
                else {
                    $('#fm').form('load', row);
                    // $('#department').combotree("setValue", row.departmentId);
                }                
                $('#tt').tabs("select",0);
                DgDepartPostLoad();
            }
            //PostLoad();
            $('#dlg').dialog('open');
            $('#fm').form('disableValidation');
        }

        function dlgDepartOpen(state) {
            //var treeData = AjaxSync("MemberManage.aspx", {
            //    act: 'getTree',
            //})
            //var datasource = $.parseJSON(treeData);
            //$('#parentDepart').combotree("loadData", datasource);
            //加载本地数据,加载本地数据是datasource只能给tree用，否则获取不到getselected
            $('#parentDepart').combotree("loadData", $.parseJSON(TreeDataJson));
            DlgDepartState = state;
            if (state == 'add') {
                var n = $('#tree').tree('getSelected');
                if (n == null) {//没有选择任何节点就选择根节点
                    n = $('#parentDepart').combotree('tree').tree('getRoot');
                }
                $('#parentDepart').combotree('setValue', n.id);
                $('#DepartName').textbox('setValue', '');
                $('#DepartName').next('span').find('input').focus();
                $('#DepartRemark').textbox('setValue', '');
            }
            else if (state == 'edit') {
                var n = $('#tree').tree('getSelected');
                if (n == null) {
                    $.messager.alert("提示", "请先选择一个部门!", "info");
                    return;
                }

                var nP = $('#tree').tree('getParent', n.target);
                if (nP == null) {//父节点没有就选择根节点
                    nP = $('#parentDepart').combotree('tree').tree('getRoot');
                }
                $('#parentDepart').combotree('setValue', nP.id);
                $('#DepartName').textbox('setValue', n.text);
                $('#DepartName').next('span').find('input').focus();
                var remark = GetDepartRemark(n.id);
                $('#DepartRemark').textbox('setValue', remark);

                initDepartmentMember();
            }

            $('#dlg_depart').dialog('open');
        }

        function dg_Load(departmentId, searchString) {
            if (searchString == undefined) {
                searchString = "";
            }
            var url = "MemberManage.aspx";
            var data = {
                act: 'getInfos',
                departmentId: departmentId,
                searchString: searchString
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                parent.Loading(false);
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
            });
        }

        function Submit() {
            $('#fm').form('submit', {
                url: 'MemberManage.aspx',
                onSubmit: function () {
                    var res = false;
                    if ($(this).form('enableValidation').form('validate')) {
                        if (CheckInfo()) {
                            parent.Loading(true);
                            $('#actMember').val(DlgState);
                            res = true;
                        }
                    }
                    return res;
                },
                success: function (res) {
                    parent.Loading(false);
                    $.messager.alert({
                        title: '提示', msg: res, icon: 'info',
                        fn: function () {
                            if (res.indexOf('新建成功') >= 0 || res.indexOf('修改成功') >= 0) {
                                $('#dlg').dialog('close');
                                var node = SelectedNode;
                                //dg_Load($('#tree').tree('getSelected').id);
                                TreeLoad(node.id);
                            }
                        }
                    });

                }
            });
        }

        function Delete() {
            var row = $('#dg').datagrid('getSelected');
            if (row == null) {
                $.messager.alert('提示', '请先选择一位人员!', 'info');
            }
            else {
                var url = "MemberManage.aspx";
                var data = {
                    act: 'del',
                    id: row.userId
                };
                $.messager.confirm('提示', '确定删除' + row.userName + '的数据?', function (r) {
                    if (r) {
                        parent.Loading(true);
                        $.post(url, data, function (res) {
                            parent.Loading(false);
                            dg_Load();
                        });
                    }
                });
            }
        }

        function Edit(row) {
            if (row == null) {
                $.messager.alert('提示', '请先选择一行数据!', 'info');
            }
            else {
                DlgState = "edit";
                dlgOpen();
                $('#fm').form('load', row);
            }
        }

        function GetDepartRemark(id) {
            return AjaxSync('MemberManage.aspx', {
                act: 'GetDepartRemark', id: id
            });
        }

        function AjaxSync(url, data) {
            return $.ajax({
                async: false,
                cache: false,
                type: 'post',
                url: url,
                data: data
            }).responseText;
        }

        function CheckInfo() {
            var data = {
                act: 'CheckInfo',
                mobile: $('#mobilePhone').textbox('getValue'),
                code: $('#employeeCode').textbox('getValue'),
                idNumber: $('#idNumber').textbox('getValue'),
                state: DlgState
            };
            var res = AjaxSync('MemberManage.aspx', data);
            if (res == "T") {
                return true;
            }
            else {
                $.messager.alert("提示", res, 'info');
                if (res.indexOf("手机号") >= 0) {
                    $('#mobilePhone').textbox("setValue", "");
                }
                if (res.indexOf("工号") >= 0) {
                    $('#employeeCode').textbox("setValue", "");
                }
                if (res.indexOf("身份证号") >= 0) {
                    $('#idNumber').textbox("setValue", "");
                }
                return false;
            }
        }

        function CheckMobile() {
            var data;
            if (DlgState == 'add') {
                data = { act: 'CheckMobile', mobile: $('#mobilePhone').textbox('getValue'), isEdit: false };
            }
            else {
                data = { act: 'CheckMobile', mobile: $('#mobilePhone').textbox('getValue'), isEdit: true };
            }
            var res = AjaxSync('MemberManage.aspx', data);

            if (res == "F") {
                $.messager.alert("提示", "手机号已存在，请重新输入!", 'info');
                $('#mobilePhone').textbox("setValue", "");
                $('#mobilePhone').focus();
                res = false;
            }
            else
                res = true;
            return res;
        }

        function CheckEmployeeCode() {
            var data;
            if (DlgState == 'add') {
                data = { act: 'CheckEmployeeCode', code: $('#employeeCode').textbox('getValue'), isEdit: false };
            }
            else {
                data = { act: 'CheckEmployeeCode', code: $('#employeeCode').textbox('getValue'), isEdit: true };
            }
            var res = AjaxSync('MemberManage.aspx', data);
            if (res == "F") {
                $.messager.alert("提示", "工号已存在，请重新输入!", 'info');
                $('#employeeCode').textbox("setValue", "");
                $('#employeeCode').focus();
                res = false;
            }
            else
                res = true;
            return res;
        }

        function doSearch(value) {
            var node = $('#tree').tree('getSelected');
            if (node == null) {
                node = $('#tree').tree('getRoot');
            }
            dg_Load(node.id, value);
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
                    url: "MemberManage.aspx",
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

        function UpdateFromWx(type){
            var url = "MemberManage.aspx";
            var data = {
                act: 'UpdateFromWx',type:type
            };
            parent.Loading(true);
            $.post(url,data,function(res){
                $.messager.alert('提示',res,'info');                
                parent.Loading(false);
                TreeLoad();
            });
        }

        function DgDepartPostLoad(){
            var url = 'MemberManage.aspx?act=getSelectedDepartmentId&wechatUserId=' 
                + $('#wechatUserId').textbox('getValue');
            $('#dg-DepartPost').datagrid({ url: url }).datagrid('load');
        }

        function OpenDepartPostDialoge(){
            $('#dlg-DepartPost').dialog('open');
            DgDepartPostLoad();
            // $('#tree-Department').tree("loadData", $.parseJSON(TreeDataJson));
            // var url = "MemberManage.aspx";
            // var data = {
            //     act: 'getSelectedDepartmentId',wechatUserId:$('#wechatUserId').textbox('getValue')
            // };
            // parent.Loading(true);
            // var Ids = $.parseJSON(AjaxSync(url,data));
            // var postList = $.parseJSON(AjaxSync(url,{act:'getPostList'}));
            // parent.Loading(false);

            // $.each(postList,function(i,val){
            //     $('#mm-DepartPost').menu('appendItem', {
            //         text: val.post,
            //         onclick: function(){DepartPostMenuOnClick(val.post)}
            //     });
            // });
            // // $('#mm-DepartPost').menu('appendItem', {
            // //         separator: true
            // //     });
            // //     $('#mm-DepartPost').menu('appendItem', {
            // //         text: val.post,
            // //         onclick: function () { DepartPostMenuOnClick(val.post) }
            // //     });

            // $('#dg-Department').datagrid('loadData',Ids);
            // $.each(Ids.rows,function(i,val){
            //     var node = $('#tree-Department').tree('find',val.departmentId);
            //     // $('#tree-Department').tree('select',node.target);
            //     $('#tree-Department').tree('check',node.target);
            //     $('#tree-Department').tree('expandTo',node.target);
            // });
        }

        function DepartPostMenuOnClick(type){
            var node = $('#tree-Department').tree('getSelected');

        }
    </script>
</body>
</html>
