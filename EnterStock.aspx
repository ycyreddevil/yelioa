<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EnterStock.aspx.cs" Inherits="EnterStock" %>

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
    <style type="text/css">
        div .btn-separator {
            float: left;
            height: 24px;
            border-left: 1px solid LightGrey;
            border-right: 0px solid LightGrey;
            margin: 1px 1px;
        }
    </style>
</head>
<body>
    <table id="dg" class="easyui-datagrid" title="入库信息">
    </table>
    <div id="tb" style="padding: 2px 5px;">
        从:
        <input id="dateFrom" class="easyui-datebox" style="width: 110px">
        到:
        <input id="dateTo" class="easyui-datebox" style="width: 110px">
        <%--Language: 
        <select class="easyui-combobox" panelHeight="auto" style="width:100px">
            <option value="java">Java</option>
            <option value="c">C</option>
            <option value="basic">Basic</option>
            <option value="perl">Perl</option>
            <option value="python">Python</option>
        </select>--%>
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-search" onclick="dg_Load();">查询</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-remove" onclick="DeleteRows();">删除</a>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-import" onclick="OpenDialoge();">导入入库信息</a>
    </div>
    <div id="dlg-Import" class="easyui-dialog" title="导入入库数据" <%--style="width: 90%; height: 90%"--%>
        data-options="iconCls:'icon-import',modal:true,closed:true">
        <table id="dg-Import" class="easyui-datagrid">
        </table>
        <div id="tb-Import">
            <form id="fm" method="post" enctype="multipart/form-data">
                <input id="fbx" class="easyui-filebox" label="入库信息文件:" labelposition="left" 
                    data-options="onChange:function(){ShowFile();},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1">
                <input type="hidden" name="act" id="act"/>
            </form>
        </div>
    </div>
    <script type="text/javascript">
        var Url = "EnterStock.aspx";

        $(document).ready(function () {
            InitDatagrid();
            dg_Load();
        });

        function InitDatagrid() {
            //主页面表格初始化
            $('#dg').datagrid({
                singleSelect: false, fit: true,
                toolbar: '#tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: 'documentNumber', width: 10, align: 'center', title: '单据编号', sortable:"true"},
                    { field: 'Id', width: 10, align: 'center', title: '产品编号', sortable:"true"},
                    { field: 'productName', width: 20, align: 'center', title: '产品名称', sortable: "true" },
                    {
                        field: 'date', width: 12, align: 'center', title: '入库日期', sortable: "true",
                        formatter: function (value, row, index) { return DateFormatter(value, row, index); }
                    },
                    { field: 'manufacturer', width: 20, align: 'center', title: '供应商' },
                    { field: 'specification', width: 20, align: 'center', title: '规格型号' },
                    { field: 'batchNumber', width: 10, align: 'center', title: '批号' },
                    { field: 'documentCreaterName', width: 10, align: 'center', title: '制单人' },
                    { field: 'amountReceivable', width: 10, align: 'center', title: '应收数量' },
                    { field: 'amountReceived', width: 10, align: 'center', title: '实收数量' },
                    { field: 'unit', width: 5, align: 'center', title: '单位' },
                    { field: 'price', width: 5, align: 'center', title: '单价', sortable: "true" },
                    { field: 'sumOfMonye', width: 10, align: 'center', title: '金额', sortable: "true" }
                ]]
            });
            var date = new Date();
            var dateFrom = date.getFullYear() + "-" + (date.getMonth() + 1) + "-01";
            var dateTo = getLastMonthDay(date);
            $("#dateFrom").datebox('setValue', dateFrom);
            $("#dateTo").datebox('setValue', dateTo);

            //Dialog表格初始化

            $('#dg-Import').datagrid({
                singleSelect: true,fit:true,
                toolbar: '#tb-Import',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: '单据编号', width: 10, align: 'center', title: '单据编号' },
                    { field: '物料代码', width: 10, align: 'center', title: '产品编号' },
                    { field: '物料名称', width: 20, align: 'center', title: '产品名称' },
                    {
                        field: '日期', width: 12, align: 'center', title: '入库日期',
                        formatter: function (value, row, index) { return DateFormatter(value, row, index); }
                    },
                    { field: '供应商', width: 20, align: 'center', title: '供应商' },
                    { field: '规格型号', width: 15, align: 'center', title: '规格型号' },
                    { field: '批号', width: 10, align: 'center', title: '批号' },
                    { field: '制单', width: 8, align: 'center', title: '制单人' },
                    { field: '应收数量', width: 8, align: 'center', title: '应收数量' },
                    { field: '实收数量', width: 8, align: 'center', title: '实收数量' },
                    { field: '单位', width: 8, align: 'center', title: '单位' },
                    { field: '单价', width: 5, align: 'center', title: '单价' },
                    { field: '金额', width: 10, align: 'center', title: '金额' },
                    {
                        field: '状态', width: 10, align: 'center', title: '状态',
                        formatter: function (value, row, index) {
                            if (value == '操作成功') {
                                return '<span style="color: #FFFFFF; background-color: #00CC00">已导入</span>';
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #FF0000">' + value + '</span>';
                            }
                        }
                    }
                ]]
            });

            $('#dlg-Import').dialog({
                buttons: [{
                //    text: '确定',
                //    iconCls: 'icon-ok', size: 'large',
                //    handler: function () {
                //        //$('#dlg-Import').dialog('close');
                //        //dg_Load();
                //        updateData();
                //    }
                //}, {
                    text: '导入',
                    iconCls: 'icon-import', size: 'large',
                    handler: function () {
                        uploadFiles();
                    }
                }, {
                    text: '关闭',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        $('#dlg-Import').dialog('close');                        
                    }
                }],
                onOpen: function () {
                    var w = $(window).width() - 80;
                    var h = $(window).height() - 80;
                    $('#dlg-Import').dialog('resize', { width: w, height: h });
                    $('#dlg-Import').dialog('move', { left: 40, top: 40 });
                    $('#fm').form('clear');
                },
                onClose: function () {
                    dg_Load();
                }
            });
        }
        function getLastMonthDay(date) {
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            //var   firstdate = year + '-' + month + '-01';  
            var day = new Date(year, month, 0);
            var lastdate = year + '-' + month + '-' + day.getDate();//获取当月最后一天日期    
            return lastdate;
        }
        function dg_Load() {
            var url = "EnterStock.aspx";
            var data = {
                act: 'getData',
                dateStart: $('#dateFrom').datebox('getValue'),
                dateEnd: $('#dateTo').datebox('getValue')
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                if (res != 'F')
                {
                    var datasource = $.parseJSON(res);
                    $('#dg').datagrid("loadData", datasource);
                }                
                parent.Loading(false);
            });
        }
        function DateFormatter(value, row, index)
        {
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

        function DatagridClear(dgId)
        {
            $('#'+dgId).datagrid('loadData', { total: 0, rows: [] });
        }

        function updateData()
        {
            var dt = $('#dg-Import').datagrid('getData');
            var len = dt.total;
            var jsonData = JSON.stringify(dt);
            var url = "EnterStock.aspx";
            var datas = {
                act:'import',
                data: jsonData
            };
            $.post(url, datas, function (res) {
                if(res!="")
                {
                    var datasource = $.parseJSON(res);
                    $('#dg-Import').datagrid("loadData", datasource);
                }
            });
        }

        function CreateDataTable(data)
        {
            var len = data.total;
            var res = new Array();
            for (var i = 0; i < len; i++) {

            }
        }

        function OpenDialoge()
        {
            DatagridClear('dg-Import');
            $('#fbx').filebox('setValue', '');
            $('#dlg-Import').dialog('open');
        }

        function ShowFile() {
            var fileName = $('#fbx').filebox('getValue');
            if (fileName == "")
            {
                return;
            }
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            }
            else {
                $('#act').val('showFile');
                parent.Loading(true);
                $('#fm').form('submit', {
                    url: 'EnterStock.aspx',
                    success: function (res) {
                        parent.Loading(false);
                        if (res == "F") {
                            $.messager.alert('错误提示', '文件读取失败', 'error');
                        }
                        else {
                            try {
                                var datasource = $.parseJSON(res);
                                $('#dg-Import').datagrid("loadData", datasource);
                            }
                            catch (e) {
                                $.messager.alert('错误提示', res + '\r\n' + e, 'error');
                            }
                        }
                    }
                });
            }
        }

        function uploadFiles() {
            var fileName = $('#fbx').filebox('getValue');
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            }
            else {
                parent.Loading(true);
                $('#act').val('upload');
                $('#fm').form('submit', {
                    url: 'EnterStock.aspx',
                    success: function (res) {
                        parent.Loading(false);
                        try{
                            var datasource = $.parseJSON(res);
                            $('#dg-Import').datagrid("loadData", datasource);
                        }
                        catch(e)
                        {
                            $.messager.alert('错误提示', res + '\r\n' + e, 'error');
                        }                        
                    }
                });
            }

        }

        function DeleteRows() {
            var rows = $('#dg').datagrid("getSelections");
            if (rows.length == 0) {
                $.messager.alert('提示', '请先选择一行数据!', 'info');
            }
            $.messager.confirm('提示', '确定删除所选行数据?', function (r) {
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
    </script>
</body>
</html>
