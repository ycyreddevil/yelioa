<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LeavStkTemplate.aspx.cs" Inherits="LeavStkTemplate" %>

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
    <script src="Scripts/jquery.json-2.4.min.js"></script>
    <style type="text/css">
        table input {
            width: 260px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'west',split:true" title="模板列表" style="width: 300px;">
        <div>
            <div style="margin: 10px 0px 10px 0px; text-align: center;">
                <a href="javascript:void(0)" class="easyui-linkbutton c8" onclick="Add();"
                    data-options="iconCls:'icon-add',size:'large'">新增模板</a>
                <%--<a href="javascript:void(0)" class="easyui-linkbutton c8" onclick="Delete();"
                    data-options="iconCls:'icon-remove',size:'large'">删除模板</a>--%>
            </div>
        </div>
        <hr />
        <ul id="datalist" class="easyui-datalist" style="width: 400px;" data-options="valueField:'type',textField:'type',striped:true,lines:true,
            onSelect: function(index,row){
                Edit(row.type);
            }">
        </ul>
    </div>
    <div data-options="region:'center',title:'出库数据模板信息',iconCls:'icon-man'">
        <div id="divFm" class="easyui-panel" style="height: 100%; padding: 5px;" data-options="
            onCollapse: function(){FormToggle();}">
            <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
                <table id="tb" style="width: 100%;">
                    <tr>
                        <td style="width: 5%;">&nbsp;</td>
                        <td style="width: 15%;">&nbsp;</td>
                        <td style="width: 75%;">
                            <%--<input id="Id" name="Id" type="hidden" />--%>
                            <input id="act" name="act" type="hidden" />
                        </td>
                        <td style="width: 5%;">&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>出库数据类型</td>
                        <td>
                            <input id="type" name="type" class="easyui-textbox" data-options="required:true" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>

                </table>
            </form>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 5%;">&nbsp;</td>
                    <td style="width: 95%;">&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <div style="text-align: left;">
                            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="submitForm()" style="width: 80px"
                                data-options="iconCls:'icon-ok',size:'large'">提交</a>
                            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="FormExpand(false);;" style="width: 80px"
                                data-options="iconCls:'icon-cancel',size:'large'">取消</a>
                            <a href="javascript:void(0)" class="easyui-linkbutton c5" onclick="Delete()" style="width: 120px"
                                data-options="iconCls:'icon-remove',size:'large'">删除此模板</a>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <script type="text/javascript">
        var Url = "LeavStkTemplate.aspx";
        var SubState;
        var FormNeedToggle = false;
        var FormIsExpand = false;
        
        $(document).ready(function () {
            InitTable();
            DlistLoad();
        });

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
        function InitTable() {
            var table = $('#tb');
            var res = AjaxSync(Url, { act: 'getTable' });
            var data = $.parseJSON(res);
            $.each(data.rows, function (i, value) {
                var tr = $("<tr></tr>");
                tr.append($("<td>&nbsp;</td>"));
                tr.append($("<td>" + value.name + "</td>"));
                var textbox = $('<td><input class="easyui-textbox" name="' + value.field + '"/></td>');
                tr.append(textbox);
                tr.append($("<td>&nbsp;</td>"));
                table.append(tr);
            });
            $.parser.parse(table);//重新渲染控件
        }

        function FormToggle()
        {
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
            if (FormIsExpand) {
                FormNeedToggle = true;
                FormExpand(false);
            }
            else {
                FormExpand(true);
            }
            $('#fm').form('disableValidation');
            if (SubState == 'add') {
                $('#fm').form('clear');
            }
            else {
                var res = AjaxSync(Url, { act: 'getFormDetail', dataType: type });
                var data = $.parseJSON(res);
                $('#fm').form('load', data);
            }
        }

        function DlistLoad() {
            var res = AjaxSync(Url, { act: 'getDatalist' });
            if (res != "") {
                var data = $.parseJSON(res);
                $('#datalist').datalist("loadData", data);
            }
            FormExpand(false);
        }

        function Add() {
            SubState = 'add';
            EnableForm();
        }

        function Edit(type) {
            SubState = 'edit';
            EnableForm(type);
        }

        function Delete(type) {
            $.messager.confirm('提示', '确定删除 ' + type + ' 的数据?', function (r) {
                if (r) {
                    SubState = 'delete';
                    submitForm();
                }
            });            
        }

        function submitForm() {
            $('#act').val(SubState);
            $('#fm').form('submit', {
                url: Url,
                onSubmit: function () {
                    var res = false;
                    if ($(this).form('enableValidation').form('validate')) {
                        if (CheckInfo()) {
                            parent.Loading(true);
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
                            DlistLoad();
                        }
                    });

                }
            });
        }

        function CheckInfo() {
            if (SubState == 'delete')
            {
                return true;
            }
            var data = {
                act: 'CheckInfo',
                dataType: $('#type').textbox('getValue'),
                state: SubState
            };
            var res = AjaxSync(Url, data);
            if (res == "T") {
                return true;
            }
            else {
                $.messager.alert("提示", res, 'info');
                $('#type').textbox("setValue", "");
                return false;
            }
        }
    </script>
</body>
</html>
