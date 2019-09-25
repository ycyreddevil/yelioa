<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PointManage.aspx.cs" Inherits="PointManage" %>
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
<body>
    <div style="margin: 20px 0;"></div>

    <div id="tbs" class="easyui-tabs" title="积分管理" data-options="fit:true">

        <div title="未审批" style="padding: 20px; display: none; height: auto">
            <div id="tb1" style="padding: 5px; height: auto">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok'" onclick="pass()">通过</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-no'" onclick="reject()">拒绝</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick="$('#dlg-add').dialog('open');">基础积分申请</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-lock'" onclick="$('#dlg-right').dialog('open');">加分权限设置</a>
                <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-import'," onclick="$('#dlg-Import').dialog('open'); $('#fmFile').form('clear');">导入</a>
            </div>
            <table id="dg1" class="easyui-datagrid"
                data-options="rownumbers:'true',fit:'true',striped:'true',url:'PointManage.aspx?data=getdata1',method:'get',selectOnCheck:'true',checkOnSelect:'false',pagination:'true',fitColumns:'true',toolbar:'#tb1'">
                <thead>
                    <tr>
                        <th data-options="field:'status',align:'center',checkbox:true"></th>
                        <th data-options="field:'Id',align:'center',sortable:'true'">编号</th>
                        <th data-options="field:'Proposer',align:'center',sortable:'true'">申请人</th>
                        <th data-options="field:'Target',align:'center',sortable:'true'">被申请人</th>
                        <th data-options="field:'Event',align:'center',width:'100px',sortable:'true'">事件</th>
                        <th data-options="field:'Type',align:'center',sortable:'true'">事件类型</th>
                        <th data-options="field:'EffectiveTime',align:'center',sortable:'true',formatter:formatterdate">发生事件日期</th>
                        <th data-options="field:'Bpoint',align:'center',sortable:'true'">B类积分</th>
                        <th data-options="field:'CreatingTime',align:'center',sortable:'true',formatter:formatterdatetime">申请时间</th>
                        <th data-options="field:'CheckState',align:'center',sortable:'true'">审批状态</th>
                        <th data-options="field:'Opinion',align:'center',sortable:'true'">审批原因</th>
                    </tr>
                </thead>
            </table>
            <div id="dlg1" class="easyui-dialog" title="拒绝原因填写" style="width: 500px; height: 270px; padding: 10px"
                data-options="closed:true,
				buttons: [{
					text:'确定拒绝',
					iconCls:'icon-ok',
					handler:function(){
						surereject();
					}
				},{
					text:'取消',
					handler:function(){
					 $('#dlg1').dialog('close');
					}
				}]
			">
                <div style="margin-bottom: 10px">
                    <input id="textbox1" class="easyui-textbox" label="拒绝原因:" prompt="请填写拒绝原因..." style="width: 100%; height: 80px" name="Event" data-options="multiline:true">
                </div>
            </div>
           <div id="dlg-Import" class="easyui-dialog" title="信息导入" style="width: 800px; height: 60px"
            data-options="iconCls:'icon-import',modal:true,closed:true">
            <form id="fmFile" method="post" enctype="multipart/form-data">
                <input id="fbx" class="easyui-filebox" label="人员信息文件:" labelposition="left"
                    data-options="onChange:function(){uploadFiles('upload');},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				    <input type="hidden" name="act" id="actFbx" />
            </form>
        </div>
            <div id="dlg-right" class="easyui-dialog" title="积分权限设置" style="width:500px;height:300px"data-options="closed:true,
				buttons: [{
					text:'确定',
					iconCls:'icon-ok',
					handler:function(){
						setRight();
					}
				},{
					text:'取消',
					handler:function(){
					 $('#dlg-right').dialog('close');
					}
				}],onClose:function(){
                $('#fmRight').form('clear');
                }
			">
            <form id="fmRight">
                <br /><br />
                <div style="margin-bottom:10px">
                &nbsp;&nbsp;        <input id="user" class="easyui-combobox" label="人员:" prompt="请输入人员名字中文或首字母" style="width:80%" name="userName"/>
                    </div>
                 <div style="margin-bottom:10px">
                &nbsp;&nbsp;        <input class="easyui-numberbox" label="积分:" prompt="请填写积分权限数" style="width:80%" name="PointApply"/>
                    </div>
            </form>

        </div>
           <div id="dlg-add" class="easyui-dialog" title="基础积分申请" style="width:500px;height:300px"data-options="closed:true,
				buttons: [{
					text:'确定',
					iconCls:'icon-ok',
					handler:function(){
						addPoint();
					}
				},{
					text:'取消',
					handler:function(){
					 $('#dlg-add').dialog('close');
					}
				}],onClose:function(){
                $('#fmAdd').form('clear');
               $('#datebox').datebox('setValue',formatterdate(new Date()));
                }
			">
            <form id="fmAdd">
                <br /><br />
                 <div style="margin-bottom:10px">
             &nbsp;&nbsp;   <input id="datebox" class="easyui-datebox" style="width: 80%" required="required"  label="日期:" value="true" editable="false" />
                  </div>
                <div style="margin-bottom:10px">
                &nbsp;&nbsp;        <input id="addUser" class="easyui-combobox" label="人员:" prompt="请输入人员名字中文或首字母" style="width:80%" name="userName"/>
                    </div>
                 <div style="margin-bottom:10px">
                &nbsp;&nbsp;        <input class="easyui-numberbox" label="积分:" prompt="请填写积分数" style="width:80%" name="Bpoint"/>
                    </div>
            </form>

        </div>
     </div>
        <div title="已审批" style="padding: 20px; display: none; height: auto">
            <div id="tb2" style="padding: 5px; height: auto">
                日期：
                <input id="datebox1" class="easyui-datebox" style="width: 100px" required="required" value="true" editable="false" />
                至：<input id="datebox2" class="easyui-datebox" style="width: 100px" required="required" value="true" editable="false" />
                <input id="textbox" class="easyui-textbox" data-options="prompt:'人员名字的拼音或者文字'" style="width: 150px" />
                <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search' " onclick="getdata2()">查询</a>
                <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-undo'" onclick="back()">撤销</a>
            </div>
            <table id="dg2" class="easyui-datagrid"
                data-options="rownumbers:'true',striped:'true',fit:'true',url:'PointManage.aspx?data=getdata2',method:'get',selectOnCheck:'true',checkOnSelect:'false',pagination:'true',fitColumns:'true',toolbar:'#tb2'">

                <thead>
                    <tr>
                        <th data-options="field:'status',align:'center',checkbox:true"></th>
                        <th data-options="field:'Id',align:'center',sortable:'true'">编号</th>
                        <th data-options="field:'Proposer',align:'center',sortable:'true'">申请人</th>
                        <th data-options="field:'Target',align:'center',sortable:'true'">被申请人</th>
                        <th data-options="field:'Event',align:'center',sortable:'true'">事件</th>
                        <th data-options="field:'Type',align:'center',sortable:'true'">事件类型</th>
                        <th data-options="field:'EffectiveTime',align:'center',sortable:'true',formatter:formatterdate">发生事件日期</th>
                        <th data-options="field:'Bpoint',align:'center',sortable:'true'">B类积分</th>
                        <th data-options="field:'CreatingTime',align:'center',sortable:'true',formatter:formatterdatetime">申请时间</th>
                        <th data-options="field:'Auditor',align:'center',sortable:'true'">审批人</th>
                        <th data-options="field:'CheckTime',align:'center',sortable:'true',formatter:formatterdatetime">审批时间</th>
                        <th data-options="field:'CheckState',align:'center',sortable:'true',styler:stateBackground">审批状态</th>
                        <th data-options="field:'Opinion',align:'center',sortable:'true',editor:'text'">审批意见或审批原因</th>
                        <th data-options="field:'State',align:'center',sortable:'true',styler:stateBackground">抽奖状态</th>

                    </tr>
                </thead>
            </table>
            <div id="dlg2" class="easyui-dialog" title="撤销原因填写" style="width: 500px; height: 270px; padding: 10px"
                data-options="closed:true,
				buttons: [{
					text:'确定撤销',
					iconCls:'icon-ok',
					handler:function(){
						sureback();
					}
				},{
					text:'取消',
					handler:function(){
					 $('#dlg2').dialog('close');
					}
				}]
			">
                <div style="margin-bottom: 10px">
                    <input id="textbox2" class="easyui-textbox" label="撤销原因:" prompt="请填写撤销原因..." style="width: 100%; height: 80px" name="Event" data-options="multiline:true" />
                </div>
            </div>
        </div>

        <div title="积分查询" style="padding: 20px; display: none;" data-options="fit:'true'">
            <div id="tb3" style="padding: 5px; height: auto">
                日期：
                <input id="datebox3" class="easyui-datebox" style="width: 100px" required="required" value="true" editable="false" />
                请选择部门：<input id="tree" class="easyui-combotree" multiple style="width: 200px" />
                <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search' " onclick="getdata3()">查询</a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="exportExcel();">导出</a>
            </div>
            <table id="dg3" class="easyui-datagrid"
                data-options="rownumbers:'true',fit:'true',striped:'true',url:'PointManage.aspx?data=getdata3',method:'get',selectOnCheck:'true',checkOnSelect:'true',fitColumns:'true',toolbar:'#tb3'">
                <thead>
                    <tr>
                        <th data-options="field:'Target',align:'center',sortable:'true'">姓名</th>
                        <th data-options="field:'department',align:'center',sortable:'true'">部门</th>
                        <th data-options="field:'month_point',align:'center',sortable:'true'">月度积分</th>
                        <th data-options="field:'month_add_point_times',align:'center',sortable:'true'">月度申请加分次数</th>
                        <th data-options="field:'month_add_point',align:'center',sortable:'true'">月度申请加分</th>
                        <th data-options="field:'month_cut_point_times',align:'center',sortable:'true'">月度申请扣分次数</th>
                        <th data-options="field:'month_cut_point',align:'center',sortable:'true'">月度申请扣分</th>
                        <th data-options="field:'season_point',align:'center',sortable:'true'">季度积分</th>
                        <th data-options="field:'year_point',align:'center',sortable:'true'">年度积分</th>
                        <th data-options="field:'total_point',align:'center',sortable:'true'">总积分</th>
                    </tr>
                </thead>

            </table>

        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#tbs').tabs({
                onUnselect: function (title, index) {
                    if (index == 0) {
                        $('#dlg1').dialog('close');
                        $('#dlg-add').dialog('close');
                        $('#dlg-Import').dialog('close');
                        $('#dlg-right').dialog('close');
                    }
                    else if (index == 1) {
                        $('#dlg2').dialog('close');
                    }
                }
            })
            initgrid1And2();
            getdata1();
            InitTree();
            TreeLoad();
            initCombobox();
        });
        function initgrid1And2() {
            $('#dg2').datagrid('getPager').pagination({
                total: 0,
                pageSize: 50,
                pageNumber: 1,
                pageList: [50, 100, 200],
                beforePageText: '第',//页数文本框前显示的汉字   
                afterPageText: '页    共 {pages} 页',
                displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
                onSelectPage: function (pageNumber, pageSize) {
                    getdata2(pageNumber, pageSize);
                },
                onRefresh: function (pageNumber, pageSize) {
                    getdata2(pageNumber, pageSize);
                }

            });
            $('#dg1').datagrid('getPager').pagination({
                total: 0,
                pageSize: 50,
                pageNumber: 1,
                pageList: [50, 100, 200],
                beforePageText: '第',//页数文本框前显示的汉字   
                afterPageText: '页    共 {pages} 页',
                displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录',
                onSelectPage: function (pageNumber, pageSize) {
                    getdata1(pageNumber, pageSize);
                },
                onRefresh: function (pageNumber, pageSize) {
                    getdata1(pageNumber, pageSize);
                }

            });
        }

        function getdata1(pageNumber, pageSize) {
            if (pageNumber == null)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 50;
            var data = {
                act: "getdata1",
                pagenumber: pageNumber,
                pagesize: pageSize
            }
            getdata(data);

        }

        function getdata2(pageNumber, pageSize) {
            if (pageNumber == null)
                pageNumber = 1;
            if (pageSize == null)
                pageSize = 50;
            var date1 = $('#datebox1').datebox('getValue');
            var date2 = $('#datebox2').datebox('getValue');
            var name = $('#textbox').textbox('getValue');
            var data = {
                act: "getdata2",
                date1: date1,
                date2: date2,
                name: name,
                pagenumber: pageNumber,
                pagesize: pageSize
            }
            getdata(data);

        }

        function getdata3() {
            var date3 = $('#datebox3').datebox('getValue');
            var str = $("#tree").combotree("getValues");
            var departments = JSON.stringify(str);
            if (str == "")
                $.messager.alert('提示', '请选择部门！', 'info');
            else {
                var data = {
                    act: "getdata3",
                    date3: date3,
                    departments: departments
                }
                getdata(data);

            }
        }

        function getdata(data) {
            parent.Loading(true);
            $.post("PointManage.aspx",
                data,
                function (res) {
                    if (res != "") {
                        var datasource = $.parseJSON(res);
                        if (data.act == "getdata1")
                            $('#dg1').datagrid("loadData", datasource);
                        else if (data.act == "getdata2")
                            $('#dg2').datagrid("loadData", datasource);
                        else
                            $('#dg3').datagrid("loadData", datasource);
                        
                    }
                    parent.Loading(false);
                });

        }


        function back() {
            if ($('#dg2').datagrid('getSelections').length == 0)
                $.messager.alert('提示', '请选择要撤销的申请记录！', 'info');
            else
                $('#dlg2').dialog('open');
            //update(act);
        }
        function sureback() {
            var reason = $('#textbox2').textbox('getValue');
            if (reason == "")
                $.messager.alert('提示', '请输入撤销的原因！', 'info');
            else {
                var post = $('#dg2').datagrid('getSelections');
                for (var i = 0; i < post.length; i++) {
                    post[i]['Opinion'] = reason;
                    if (post[i]['wechatUserId'] == null || post[i]['wechatUserId'] == "") {
                        post[i]['wechatUserId'] = "----";
                    }
                }
                update("back", post)
            }
        }
        function pass() {
            if ($('#dg1').datagrid('getSelections').length == 0)
                $.messager.alert('提示', '请选择要通过的申请记录！', 'info');
            else {
                var post = $('#dg1').datagrid('getSelections');
                for (var i = 0; i < post.length; i++) {
                    post[i]['Opinion'] = "通过";
                     if (post[i]['wechatUserId'] == null || post[i]['wechatUserId'] == "") {
                        post[i]['wechatUserId'] = "----";
                    }
                }
                update("pass", post)
            }
        }
        function reject() {
            var act = "reject";
            if ($('#dg1').datagrid('getSelections').length == 0)
                $.messager.alert('提示', '请选择要拒绝的申请记录！', 'info');
            else
                $('#dlg1').dialog('open');
        }
        function surereject() {
            var reason = $('#textbox1').textbox('getValue');
            if (reason == "")
                $.messager.alert('提示', '请输入撤销的原因！', 'info');
            else {
                var post = $('#dg1').datagrid('getSelections');
                for (var i = 0; i < post.length; i++) {
                    post[i]['Opinion'] = reason;
                     if (post[i]['wechatUserId'] == null || post[i]['wechatUserId'] == "") {
                        post[i]['wechatUserId'] = "----";
                    }
                }
                update("reject", post)
            }
        }
        function update(act, post) {
            parent.Loading(true);
            var str = JSON.stringify(post);
            $.post("PointManage.aspx",
                {
                    act: act,
                    data: str
                },
                function (res) {
                    if (res == "操作发布成功！") {
                        getdata1();
                        getdata2();
                        if (act == "reject")
                            $('#dlg1').dialog('close');
                        else if (act == "back")
                            $('#dlg2').dialog('close');
                    }
                    parent.Loading(false);
                    $.messager.alert('提示', res, 'info');
                });
        }

         function getSearched(type) {
               searchStr = $(type).combobox('getText');
               parent.Loading(true);
               $.post('PointManage.aspx', { act: 'getUsers', searchStr: searchStr },
                   function (res) {
                       if (res != "") {
                           var datasource = JSON.parse(res);
                           $(type).combobox('loadData', datasource);
                       }
                       else
                           $.messager.alert('提示', '未找到相关人员，请重新输入！', 'info');
                       parent.Loading(false);
                   });
           }
           function initCombobox() {
               $('#user').combobox({
                   valueField: 'value',                   
                   icons: [{

                       iconCls: 'icon-search',
                       handler: function (e) {
                           getSearched('#user');
                       }
                   }],
                   textField: 'text',
                   panelHeight: 'auto',
                   hasDownArrow: false,
                   formatter: function (row) {
                       var opts = $(this).combobox('options');
                       return row[opts.textField];
                   }
               });
                $('#addUser').combobox({
                   valueField: 'value',                   
                   icons: [{

                       iconCls: 'icon-search',
                       handler: function (e) {
                           getSearched('#addUser');
                       }
                   }],
                   textField: 'text',
                   panelHeight: 'auto',
                   hasDownArrow: false,
                   formatter: function (row) {
                       var opts = $(this).combobox('options');
                       return row[opts.textField];
                   }
               });
        }
        function addPoint() {
            var obj = $('#fmAdd').serializeArray();
            if ($('#addUser').combobox("getText").toString() == null || $('#addUser').combobox("getText").toString() == "")
                $.messager.alert('提示', "请输入人员名字！", 'info');
            else if (obj[1]["value"].toString() == null || obj[1]["value"].toString() == "")
                $.messager.alert('提示', "请输入积分数目！", 'info');
             else {
                var data = JSON.stringify(obj);
                parent.Loading(true);
                $.post('PointManage.aspx', {
                    act: 'addPoint',
                    data: data,
                    date: $('#datebox').datebox('getValue')
                }, function (res) {
                    if (res == "操作成功!") {
                        $('#dlg-add').dialog('close');
                        getdata1();
                    }
                    $.messager.alert('提示', res, 'info');
                  parent.Loading(false);
                });
            }
        }

        function setRight() {
            var obj = $('#fmRight').serializeArray();
            if ($('#user').combobox("getText").toString() == null || $('#user').combobox("getText").toString() == "")
                $.messager.alert('提示', "请输入人员名字！", 'info');
            else if (obj[1]["value"].toString() == null || obj[1]["value"].toString() == "")
                $.messager.alert('提示', "请输入积分数目！", 'info');
             else {
                var data = JSON.stringify(obj);
                parent.Loading(true);
                $.post('PointManage.aspx', {
                    act: 'setRight',
                    data: data
                }, function(res) {
                    $('#dlg-right').dialog('close');
                    $.messager.alert('提示', res, 'info');
                  parent.Loading(false);
                });
            }
        }

        function exportExcel() {
            var rows = $("#dg3").datagrid("getRows");

            if (rows.length == 0) {
                $.messager.alert('提示', '暂无数据，无法导出', 'info');
                return;
            }
            window.location.href = "PointManage.aspx?act=exportExcel";

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
                    url: "PointManage.aspx",
                    success: function (res) {
                        parent.Loading(false);
                        $.messager.alert('提示', res, 'info', function () {
                            $('#dlg-Import').dialog('close');
                            getdata1();
                        });
                    }
                });
            }
        }
       
        function formatterdate(value) {
            var d = new Date(value);
            var year = d.getFullYear();
            var month = (d.getMonth() + 1).toString();
            var day = (d.getDate()).toString();
            if (month.length == 1) {
                month = "0" + month;
            }
            if (day.length == 1) {
                day = "0" + day;
            }
            var date = year + "-" + month + "-" + day;
            return date;
        }

        function formatterdatetime(value) {
            value = value.replace(/T/, " ");
            var dt = new Date(value);
            var year = dt.getFullYear();
            var month = (dt.getMonth() + 1).toString();
            var day = (dt.getDate()).toString();
            var hour = (dt.getHours()).toString();
            var minute = (dt.getMinutes()).toString();
            var second = (dt.getSeconds()).toString();
            if (month.length == 1) {
                month = "0" + month;
            }
            if (day.length == 1) {
                day = "0" + day;
            }
            if (hour.length == 1) {
                hour = "0" + hour;
            }
            if (minute.length == 1) {
                minute = "0" + minute;
            }
            if (second.length == 1) {
                second = "0" + second;
            }
            var dateTime = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;
            return dateTime;
        }

        function stateBackground(value, row, index) {
            if (value == "已审核" || value == "未抽奖")
                return 'background-color:#00FF00;';
            else
                return 'background-color:#FF0000;';
        }


        $(function () {
            $('#datebox').datebox().datebox('calendar').calendar({
                validator: function (date) {
                    return validate(date);
                }
            });
            $('#datebox1').datebox().datebox('calendar').calendar({
                validator: function (date) {
                    return validate(date);
                }
            });
            $('#datebox2').datebox().datebox('calendar').calendar({
                validator: function (date) {
                    return validate(date);
                }
            });
            $('#datebox3').datebox().datebox('calendar').calendar({
                validator: function (date) {
                    return validate(date);
                }
            });
        });
        function validate(date) {
            var now = new Date();
            var d = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            return date <= d;
        }

        function InitTree() {
            $('#tree').combotree({
                animate: true, required: true,
                formatter: function (node) {
                    var s = node.text;
                    return s;
                }
            });

        }
        function TreeLoad() {
            var url = "PointManage.aspx";
            var data = {
                act: 'getTree'
            };
            $.post(url, data, function (res) {
                if (res != "F") {
                    var datasource = $.parseJSON(res);
                    $('#tree').combotree("loadData", datasource);
                }
            });
        }

    </script>
</body>
</html>
