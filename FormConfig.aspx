<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FormConfig.aspx.cs" Inherits="FormConfig" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>表单配置页</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link href="Scripts/themes/mobile.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
</head>
<body>
    <table id="dg"></table>
    
    <div id="tb" style="padding: 2px 5px;">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-add'," onclick="form_add()">新增表单</a>&nbsp; 
    </div>
    <div id="dlg-preview" class="easyui-dialog" title="表单预览" data-options="iconCls:'icon-save',resizable:true,modal:true,closed:true">
        
    </div>
</body>
<script>
    $(function () {
        initTable();
        initialDialog();
    });

    var initialDialog = function () {
        $('#dlg-preview').dialog({
            onOpen: function () {
                $('#dlg-preview').dialog('resize', { width: 250});
            },
            onClose: function () {
                
            }
        });
    }

    var OpenPreviewDialoge = function (formName) {
        $.ajax({
            url: 'FormConfig.aspx',
            data: { formName: formName, act: 'previewForm' },
            dataType: 'json',
            type: 'post',
            success: function (datas) {
                $('#dlg-preview').dialog('open');
                var formTable = "<table>";
                formTable += "<caption>" + datas[0].formName + "</caption>"
                for (i = 0; i < datas.length; i++) {
                    var data = datas[i];
                    var newEle = "<tr style='margin-bottom:50px'><td>" + data.fieldName + ":</td><td>";
                    if (data.type == 'input') {
                        if (data.isnecessary == '是') {
                            newEle += "<input value='" + data.defaultValue + "' maxLength='" + data.length + "' class='easyui- textbox' data-options='required:true'/>";
                        } else {
                            newEle += "<input value='" + data.defaultValue + "' maxLength='" + data.length + "'/>";
                        }
                    } else if (data.type == 'select') {
                        var options = data.extra.split(",");
                        newEle += "<select class='easyui-combobox'>";
                        for (j = 0; j < options.length; j++) {
                            newEle += "<option>" + options[j] + "</option>";
                        }
                        newEle += "</select>";
                    } else if (data.type == 'checkbox'){
                        var options = data.extra.split(",");
                        for (z = 0; z < options.length; z++) {
                            newEle = "<input type='checkbox' value='"+ options[z] +"'/>"
                        }
                    }
                    newEle += "</td></tr>";
                    formTable += newEle;
                }
                formTable += "</table>";
                $("#dlg-preview").append(formTable);
            },
        })
    }

    var initTable = function () {
        $('#dg').datagrid({
            url: 'FormConfig.aspx',
            queryParams: {
                act: 'getCustomizedForm',
            },
            rownumbers: true,
            collapsible: false,
            singleSelect: true,
            fit: true,
            toolbar: '#tb',
            fitColumns: true,
            columns: [[
                {
                    field: 'formName',
                    title: '表单名称',
                    align: "center",
                    width: '20%',
                },
                {
                    field: 'userName',
                    title: '创建人',
                    align: "center",
                    width: '20%',
                },
                {
                    field: 'lmt',
                    title: '创建时间',
                    align: "center",
                    width: '20%',
                },
                {
                    field: 'operation',
                    title: '操作',
                    align: 'center',
                    formatter: function (value, row, index) {
                        return '<a class="easyui-linkbutton" data-options="iconCls:\'icon-search\'" href="javascript:void(0)" onclick="OpenPreviewDialoge(\'' + row.formName + '\')">预览</a>'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-search\'" href="javascript:void(0)" onclick="EditForm(\'' + row.formName + '\')">编辑</a>'
                            + '<a class="easyui-linkbutton" data-options="iconCls:\'icon-search\'" href="javascript:void(0)" onclick="GiveRights(\'' + row.formName + '\')">赋权</a>';
                    },
                    width: '40%',
                }
            ]],
            onLoadSuccess: function (data) {
                
            }
        })
    }

    var form_add = function () {
        // 显示表单设置页面
        location.href="CustomizedForm.aspx"
    }

    var EditForm = function (formName) {
        // 表单编辑页面
        location.href = "CustomizedForm.aspx?getData=true&&formName="+formName;
    }
</script>
</html>
