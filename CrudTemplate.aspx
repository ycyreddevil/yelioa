<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CrudTemplate.aspx.cs" Inherits="CrudTemplate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>CrudTemplate</title>
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

<body class="easyui-layout">
    <!-- <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
        background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog"
        border="false" noheader="true" closed="true" modal="true">
    </div> -->
    <div data-options="region:'east',split:true" title="导入" style="width: 200px;"></div>

    <div data-options="region:'center',title:'表单详细信息'">
        <table id="dg" class="easyui-datagrid"></table>
        <div id="dg-tb" style="padding: 2px 5px;">
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add',"
                onclick="openDlg('add');">新增</a>&nbsp;
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'"
                onclick="openDlg('edit');">修改</a>&nbsp;
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove'"
                onclick="Delete();">删除</a>&nbsp;
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-mini-refresh'"
                onclick="dg_Load();">刷新</a>&nbsp;
        </div>
    </div>
    <div id="dlg" class="easyui-dialog" title="详细信息" data-options="iconCls:'icon-edit',modal:true,closed:true">
        <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
            <input id="actSubmit" name="actSubmit" type="hidden"></input>
            <input id="table" name="table" type="hidden"></input>
            <!-- <input id="Id" name="Id" type="hidden"></input> -->
            <table id="tb" style="width: 100%;"> </table>
        </form>
    </div>
    <script>
        var Url = 'CrudTemplate.aspx';
        var table='';
        $(document).ready(function () {
            InitDg();
            InitWindow();
            InitDlg();
            // $('#btn-edit').linkbutton('disable');
            // SetStartAndEndDate();
        });

        function GetRequestParametre() {
            var url = location.search; //获取url中"?"符后的字串
            var theRequest = new Object();
            if (url.indexOf("?") != -1) {
                var str = url.substr(1);
                strs = str.split("&");
                for (var i = 0; i < strs.length; i++) {
                    theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
                }
            }
            return theRequest;
        }

        function InitWindow() {
            var requestPara = GetRequestParametre();
            table=requestPara['table'];
            dg_Load();
            
        }
        function dg_Load(){
            var data = {
                act: 'init', table: table
            };
            parent.parent.Loading(true);
            $.post(Url, data, function (res) {
                parent.parent.Loading(false);
                var datasource = $.parseJSON(res);
                if (datasource.ErrCode == 0) {
                    var dgData = $.parseJSON(datasource.data)
                    DgLoad(dgData);
                    InitTable(dgData.rows[0]);
                }
                else
                    $.messager.alert('提示', 'code:' + datasource.ErrCode + ',msg:' + datasource.ErrMsg, 'info');
            });
        }

        function InitDg() {
            $('#dg').datagrid({
                fit: true,
                toolbar: '#dg-tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                singleSelect: false,
                fitColumns: false,
                // onDblClickCell: function (index, field, value) {
                //     openDlg('edit');
                // }
            });
        }
        function DgLoad(data) {
            if (data.total == 0) {
                $('#dg').datagrid({ columns: [] }).datagrid("loadData", data);
                return;
            }

            var columns = new Array();
            var obj = data.rows[0];
            // DgImportLength = data.total;
            //columns.push();
            // columns.push({ field: 'code11', checkbox: true, width: 12, align: 'center', title: '复选框', sortable: "true", });
            $.each(obj, function (key, value) {
                // if (!isContainChinese(key))
                //     return true; //Continue

                // else {
                var column = {};
                column["field"] = key;
                column["title"] = key;
                column["width"] = 100;
                column["align"] = 'center';
                column["sortable"] = true;
                columns.push(column);
                // }
            });

            $('#dg').datagrid({
                columns: [columns]
            }).datagrid("loadData", data);
        }

        var isContainChinese = function (s) {
            if (escape(s).indexOf("%u") < 0) {
                // alert("没有包含中文");
                return false;
            } else {
                // alert("包含中文");
                return true;
            }
        };

        var InitDlg = function () {
            $('#dlg').dialog({//用户对话框
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok', size: 'large',
                    handler: function () {
                        Submit();
                    }
                }, {
                    text: '清除',
                    iconCls: 'icon-clear', size: 'large',
                    handler: function () {
                        $('#fm').form('clear');
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        $('#fm').form('clear');
                        $('#dlg').dialog('close');
                    }
                }],
                onOpen: function () {
                    var h = $(window).height() - 60;
                    var w = $(window).height() - 60;
                    $(this).dialog('resize', { height: h, width: w });
                    $(this).dialog('move', { top: 30, left: 30 });
                },
                onClose:function(){
                    dg_Load();
                }
            });
        }

        function Delete() {            
            var selected = $('#dg').datagrid('getSelections');
            if (selected == undefined) {
                return;
            }
            $.messager.confirm('确认', '确定删除所有选中项目?', function(r){
				if (r){
                    // parent.parent.Loading(true);
                    var d={act:'delete',data:JSON.stringify(selected), table: table};
					$.post(Url,d,function(res){
                        // parent.parent.Loading(false);
                        $.messager.alert('提示', res, 'info');
                        dg_Load();
                    });
				}
			});
        }

        function openDlg(type) {
            $('#fm').form('clear');
            if (type == 'edit') {
                if ($('#dg').datagrid('getSelected') == undefined) {
                    return;
                }
                var data = $('#dg').datagrid('getSelections');
                $('#fm').form('load', data[data.length-1]);
            }

            $('#dlg').dialog('open');
        };

        function InitTable(data) {
            var table = $('#tb');
            // $("#tb tr:not(:first)").empty(); //清空table（除了第一行以外）
            table.empty(); //清空table
            //初始化第一行
            {
                var tr = $("<tr></tr>");
                tr.append($('<td style="width: 5%;">&nbsp;</td>'));
                tr.append($('<td style="width: 30%;">&nbsp;</td>'));
                tr.append($('<td style="width: 70%;"></td>'));
                tr.append($('<td style="width: 5%;">&nbsp;</td>'));

                table.append(tr);
            }
            $.each(data, function (key, value) {
                // if (!isContainChinese(key)) {
                //     return true;
                // }
                var tr = $("<tr></tr>");
                tr.append($("<td>&nbsp;</td>"));
                tr.append($("<td>" + key + "</td>"));
                var textbox = $('<td><input class="easyui-textbox" style="width: 100%;" name="' + key + '"/></td>');
                // if (key == '产品' || key == '医院' || key == '代表' || key == '区域经理' || key == '主管'
                //     || key == '销售负责人' || key == '部门') {
                //     textbox = $('<td><input class="easyui-textbox" style="width: 100%;" data-options="editable:false" name="' + key + '"/></td>');
                // }
                tr.append(textbox);
                tr.append($("<td>&nbsp;</td>"));
                table.append(tr);
            });
            $.parser.parse(table);//重新渲染控件

        }


        function Submit() {
            $('#fm').form('submit', {
                url: Url,
                onSubmit: function () {
                    var res = false;
                    if ($(this).form('enableValidation').form('validate')) {
                        // parent.parent.Loading(true);
                        $('#actSubmit').val('save');
                        $('#table').val(table);
                        res = true;
                    }
                    return res;
                },
                success: function (res) {
                    // parent.parent.Loading(false);
                    // $.messager.alert('提示', res, 'info');
                    $.messager.alert({
                        title: '提示', msg: res, icon: 'info',
                        fn: function () {
                            // if (res.indexOf('新建成功') >= 0 || res.indexOf('修改成功') >= 0) {
                            //     $('#dlg').dialog('close');
                            //     dgLoad();
                            // }

                        }
                    });

                }
            });
        }

    </script>
</body>

</html>