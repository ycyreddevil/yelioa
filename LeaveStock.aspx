<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LeaveStock.aspx.cs" Inherits="LeaveStock" %>

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
</head>
<body>
    <table id="dg" class="easyui-datagrid" title="出库信息"><thead data-options="frozen:true"></thead>
    </table>
    <div id="tb" style="padding: 2px 5px;">
        数据类型:
        <select id="productType" class="easyui-combobox" style="width:110px;" data-options="editable:false,valueField:'type',
                                    textField:'type',
                                    url:'LeaveStock.aspx?act=getDataType&needAll=true',
                                    method:'post'">
        </select>
        从:
        <input id="dateFrom" class="easyui-datebox" style="width: 110px" data-options="onSelect:function(){dg_Load();}">
        到:
        <input id="dateTo" class="easyui-datebox" style="width: 110px" data-options="onSelect:function(){dg_Load();}">
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-remove" onclick="DeleteRows();">删除</a>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-leaveStock" onclick="OpenDialoge();">导入出库信息</a>
    </div>
    <div id="dlg-Export" class="easyui-dialog" title="导入出库数据" data-options="iconCls:'icon-leaveStock',modal:true,closed:true">
        <table id="dg-Import" class="easyui-datagrid">
        </table>
        <div id="tb-Export">
            <form id="fm" method="post" enctype="multipart/form-data">
                <input id="fbx" class="easyui-filebox" label="出库信息文件:" labelposition="left" 
                    data-options="onChange:function(){uploadFiles('showFile');},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1" >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <select class="easyui-combobox" name="type" id="type" style="width:200px;" label="文件类型:" data-options="
                                    valueField:'type',
                                    textField:'type',
                                    url:'LeaveStock.aspx?act=getDataType',
                                    method:'post',                    
                                ">
                </select>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-edit" onclick="OpenAndFillAliasDialog();">设置转义</a>
                <input type="hidden" name="act" id="act"/>
                <input type="hidden" name="dataType" id="dataType"/>
            </form>
        </div>
    </div>
    <div id="dlg-Alias" class="easyui-dialog" title="产品及机构转义设置" data-options="modal:true,closed:true"
        style="width:400px">
        <table style="width: 100%;">
            <tr>
                <td colspan="4" style="text-align:center">产品转义</td>
            </tr>
            <tr>
                <td style="width:10%">&nbsp;</td>
                <td style="width:40%;text-align:right">产品转义编码</td>
                <td style="width:40%;text-align:left">
                    <input class="easyui-textbox" id="p_alias_code"/>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td style="width:10%">&nbsp;</td>
                <td style="width:40%;text-align:right">产品转义名称</td>
                <td style="width:40%;text-align:left">
                    <input class="easyui-textbox" id="p_alias_name"/>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td style="width:10%">&nbsp;</td>
                <td style="width:40%;text-align:right">产品转义规格</td>
                <td style="width:40%;text-align:left">
                    <input class="easyui-textbox" id="p_alias_Specification"/>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td style="width:10%">&nbsp;</td>
                <td style="width:40%;text-align:right">本系统产品编码</td>
                <td style="width:40%;text-align:left">
                    <input class="easyui-combobox" id="productCode" data-options="valueField:'id',textField:'text'
                        , panelHeight:'auto',prompt:'输入拼音首字母后自动搜索'"/>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="4" style="text-align:center">终端客户转义</td>
            </tr>
            <tr>
                <td style="width:10%">&nbsp;</td>
                <td style="width:40%;text-align:right">机构转义编码</td>
                <td style="width:40%;text-align:left">
                    <input class="easyui-textbox" id="o_alias_code"/>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td style="width:10%">&nbsp;</td>
                <td style="width:40%;text-align:right">机构转义名称</td>
                <td style="width:40%;text-align:left">
                    <input class="easyui-textbox" id="o_alias_name"/>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
            <tr>
                <td style="width:10%">&nbsp;</td>
                <td style="width:40%;text-align:right">本系统机构编码</td>
                <td style="width:40%;text-align:left">
                    <input class="easyui-combobox" id="organizationCode" data-options="valueField:'id',textField:'text'
                        , panelHeight:'auto',prompt:'输入拼音首字母后自动搜索'"/>
                </td>
                <td style="width:10%">&nbsp;</td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        var Url = 'LeaveStock.aspx';
        var UploadIndex = -1;
        var AliasOfDataType;

        $(document).ready(function () {
            InitDatagrid();
            dg_Load();
            changeType();
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

        function InitDatagrid() {
            //主页面表格初始化
            $('#dg').datagrid({
                singleSelect: false, fit: true,
                toolbar: '#tb' ,
                striped: true,
                rownumbers: true,
                collapsible: false,
                showFooter: true,
                fitColumns: true,
                columns: [[
                    {
                        field: 'date', width: 12, align: 'center', title: '日期', sortable: "true",
                        formatter: function (value, row, index) { return DateFormatter(value, row, index); }
                    },
                    { field: 'type', width: 10, align: 'center', title: '数据类型' , sortable: "true"},
                    //{ field: 'productCode', width: 10, align: 'center', title: '产品编码', sortable: "true" },
                    { field: 'productName', width: 20, align: 'center', title: '产品名称', sortable: "true" },
                    { field: 'accountUnit', width: 20, align: 'center', title: '结算单位' },
                    { field: 'terminalClient', width: 20, align: 'center', title: '终端客户' },
                    // { field: 'deliveryWarehouse', width: 10, align: 'center', title: '发货仓库' },
                    { field: 'specification', width: 20, align: 'center', title: '规格型号' },
                    { field: 'batchNumber', width: 10, align: 'center', title: '批号' },
                    { field: 'unit', width: 5, align: 'center', title: '单位' },
                    {
                        field: 'dateValidity', width: 12, align: 'center', title: '有效期至',
                        formatter: function (value, row, index) { return DateFormatter(value, row, index); }
                    },
                    { field: 'amountSend', width: 10, align: 'center', title: '实发数量' },
                    { field: 'shipper', width: 10, align: 'center', title: '发货人' },
                    { field: 'salesman', width: 10, align: 'center', title: '业务员' },
                    { field: 'salesmanDepartment', width: 20, align: 'center', title: '业务员部门' },
                    { field: 'recipientsDepartment', width: 20, align: 'center', title: '领用部门' }
                ]],
                idField: "Id",
                    sortName: 'date',
                    sortOrder: 'asc',
                    onSortColumn: function (sort, order) {
                        dg_Load("",sort, order);
                    }
            });
            var date = new Date();
            var dateFrom = date.getFullYear() + "-" + (date.getMonth() + 1) + "-01";
            var dateTo = getLastMonthDay(date);
            $("#dateFrom").datebox('setValue', dateFrom);
            $("#dateTo").datebox('setValue', dateTo);

            //Dialog表格初始化

            $('#dg-Import').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#tb-Export',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: false,
                columns: [[]],
                onSelect: function (index, row) {
                    if (UploadIndex == index) {
                        var jsonStr = JSON.stringify(row);
                        var data = {
                            act: 'import', json: jsonStr,
                            type: $('#type').combobox('getValue')
                        };
                        $.post(Url, data, function (res) {
                            var newRow = $.parseJSON(res);
                            $('#dg-Import').datagrid('deleteRow', index);
                            $('#dg-Import').datagrid('insertRow', {
                                index: index,	// index start with 0
                                row: newRow
                            });
                            if (index < (DgImportLength - 1)) {
                                UploadIndex = index + 1;
                                $('#dg-Import').datagrid('selectRow', UploadIndex);
                            }
                            else {//最后一行
                                UploadIndex = -1;
                                //$('#dlg-Import').dialog('close');
                                //dgLoad();
                            }
                        });
                    }
                },
                onDblClickRow: function () { OpenAndFillAliasDialog(); }
            });

            $('#type').combobox({
                onChange: function (newValue, oldValue) {
                    if (!newValue)
                        return;
                    var res = AjaxSync(Url, { act: 'getTemplate', type: newValue });
                    AliasOfDataType = $.parseJSON(res);
                }
            });

            $('#productCode').combobox({
                onChange: function (newValue, oldValue) {
                    if (!newValue)
                        return;
                    var reg = new RegExp("[\\u4E00-\\u9FFF]+", "g");
                    if (reg.test(newValue))//忽略有汉字的情况
                        return;
                    reg = /^[A-Za-z]*$/;　　//正整数 
                    if (!reg.test(newValue))//忽略正整数的情况
                        return;
                    $.post(Url, { act: 'searchProduct', searchString: newValue }, function (res) {
                        //$(this).combobox('showPanel');
                        var datasource = $.parseJSON(res);
                        $('#productCode').combobox('loadData', datasource);
                    });
                }
            });

            $('#organizationCode').combobox({
                onChange: function (newValue, oldValue) {
                    if (!newValue)
                        return;
                    var reg = new RegExp("[\\u4E00-\\u9FFF]+", "g");
                    if (reg.test(newValue))//忽略有汉字的情况
                        return;
                    reg = /^[A-Za-z]*$/;;　　//正整数 
                    if (!reg.test(newValue))//忽略正整数的情况
                        return;
                    $.post(Url, { act: 'searchOrganization', searchString: newValue }, function (res) {
                        //$(this).combobox('showPanel');
                        var datasource = $.parseJSON(res);
                        $('#organizationCode').combobox('loadData', datasource);
                    });
                }
            });

            $('#dlg-Export').dialog({
                buttons: [{
                    text: '导入',
                    iconCls: 'icon-leaveStock', size: 'large',
                    handler: function () {
                        //uploadFiles('import');
                        //开始逐条上传
                        UploadIndex = 0;
                        $('#dg-Import').datagrid('selectRow', UploadIndex);                        
                    }
                }, {
                    text: '转义',
                    iconCls: 'icon-reload', size: 'large',
                    handler: function () {
                        OpenAndFillAliasDialog();                        
                    }
                }, {
                    text: '关闭',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        $('#dlg-Export').dialog('close');
                    }
                }],
                onOpen: function () {
                    var w = $(window).width() - 80;
                    var h = $(window).height() - 80;
                    $('#dlg-Export').dialog('resize', { width: w, height: h });
                    $('#dlg-Export').dialog('move', { left: 40, top: 40 });
                    $('#fm').form('clear');
                    var data = $('#type').combobox('getData');
                    $('#type').combobox('select', data[0].value);
                    $('#dg-Import').datagrid({
                        columns: [[]]
                    });
                },
                onClose: function () {
                    dg_Load();
                }
            }); 

            $('#dlg-Alias').dialog({
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok', size: 'large',
                    handler: function () {
                        var data = {
                            act:'saveAlias',
                            type:$('#type').combobox('getValue'),
                            productCode:$('#productCode').combobox('getValue'),
                            pAliasName:$('#p_alias_name').textbox('getValue'),
                            pAlisaSpecification:$('#p_alias_Specification').textbox('getValue'),
                            organizationCode:$('#organizationCode').combobox('getValue'),
                            oAliasName:$('#o_alias_name').textbox('getValue')
                            };
                        var res = AjaxSync(Url,data);
                        $.messager.alert('提示',res,'info',function(){
                            $('#dlg-Alias').dialog('close');
                        });
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        $('#dlg-Alias').dialog('close');
                    }
                }],
                onOpen: function () {
                    var h = $(window).height() - 60;
                    $(this).dialog('resize', { height: h });
                    $(this).dialog('move', { top: 30 });
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
        function dg_Load(searchString, sort, order, type) {
            if (type == undefined) {
                type = $("#productType").combobox("getValue");
            }
            if (type == "全部") {
                type = "";
            }
            
            if (searchString == undefined) {
                    searchString = "";
                }
                if (sort == undefined) {
                    sort = 'date'; order = 'asc';
                }
            var data = {
                act: 'getData',
                dateStart: $('#dateFrom').datebox('getValue'),
                dateEnd: $('#dateTo').datebox('getValue'),
                searchString: searchString, sort: sort, order: order,
                type: type
            };
            parent.Loading(true);
            $.post(Url, data, function (res) {
                if (res != 'F' && res != "") {
                    var datasource = $.parseJSON(res);
                    $('#dg').datagrid("loadData", datasource);
                    //var combData = $('#productType').combobox('getData');
                    //var array = { "type": "全部" };
                    //var newCombData = new Array();
                    //newCombData.push(array);
                    //$.each(combData, function (i,val) {
                    //    newCombData.push(combData[i]);
                    //})
                    //$('#productType').combobox('loadData', newCombData)
                }
                parent.Loading(false);
            });
        }
        function OpenAndFillAliasDialog()
        {
            var type = $('#type').combobox('getValue');
            if(!type){
                return;
            }
                
            if (!AliasOfDataType){
                return;
            }
            $('#productCode').combobox('clear');     
            $('#organizationCode').combobox('clear');           
            
            var row = $('#dg-Import').datagrid('getSelected');
            if (AliasOfDataType['productCode'])
                $('#p_alias_code').textbox('setValue', row[AliasOfDataType['productCode']]);
            if (AliasOfDataType['productName'])
                $('#p_alias_name').textbox('setValue', row[AliasOfDataType['productName']]);
            if (AliasOfDataType['specification'])
                $('#p_alias_Specification').textbox('setValue', row[AliasOfDataType['specification']]);
            if (AliasOfDataType['terminalClient'])
                $('#o_alias_name').textbox('setValue', row[AliasOfDataType['terminalClient']]);
            $('#dlg-Alias').dialog('open');
        }

        function DateFormatter(value, row, index) {
            if (value == null || value == '') {
                return '';
            }
            if(value == '合计')
                return value;
            var dt;
            if (value instanceof Date) {
                dt = value;
            } else {
                dt = new Date(value);
            }

            var res = dt.getFullYear() + '年' + (dt.getMonth() + 1) + '月' + dt.getDate() + '日';
            return res;
        }

        function DatagridClear(dgId) {
            $('#' + dgId).datagrid('loadData', { total: 0, rows: [] });
        }

        function OpenDialoge() {
            DatagridClear('dg-Import');
            $('#fbx').filebox('setValue', '');
            $('#dlg-Export').dialog('open');
        }

        function DgImportLoad(data) {
            var columns = new Array();
            var frozenColumns = new Array();
            var obj = data.rows[0];
            DgImportLength = data.total;
            $.each(obj, function (key, value) {
                if (key == '状态') {
                    frozenColumns.push({
                        field: '状态', title: '状态', width: 150, align: 'center', frozen: true,
                        formatter: function (value, row, index) {
                            if (value == '已导入') {
                                return '<span style="color: #FFFFFF; background-color: #00CC00">已导入</span>';
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #FF0000">' + value + '</span>';
                            }
                        }
                    });
                }
                else {
                    var column = {};
                    column["field"] = key;
                    column["title"] = key;
                    column["width"] = 100;
                    column["align"] = 'center';
                    column["sortable"] = true;
                    columns.push(column);
                }
            });
            $('#dg-Import').datagrid({
                columns: [columns],
                frozenColumns: [frozenColumns]
            }).datagrid("loadData", data);
        }

        function uploadFiles(type) {
            
            var fileName = $('#fbx').filebox('getValue');
            if (fileName == "") {
                return;
            }
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            }
            else {
                $('#act').val(type);
                $('#dataType').val($('#type').combobox('getValue'));
                parent.Loading(true);
                $('#fm').form('submit', {
                    url: Url,
                    success: function (res) {
                        parent.Loading(false);
                        if (res.indexOf("error:") == 0) {
                            $.messager.alert('错误提示', res.substring("error:".length), 'error');
                        }
                        else {
                            try {
                                var datasource = $.parseJSON(res);
                                DgImportLoad(datasource);
                            }
                            catch (e) {
                                $.messager.alert('错误提示', res + '\r\n' + e, 'error');
                            }
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

        var changeType = function () {
            $("#productType").combobox({
                onSelect: function (n,o) {
                    dg_Load(null,null,null,n.type);
                }
            })
        }
    </script>
</body>
</html>
