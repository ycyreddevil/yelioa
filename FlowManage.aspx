<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FlowManage.aspx.cs" Inherits="FlowManage" %>

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
    <table id="dg" class="easyui-datagrid" title="流向信息"><thead data-options="frozen:true"></thead>
    </table>
    <div id="tb" style="padding: 2px 5px;">
        请选择月份:
        <input type="text" id="date" class="easyui-datebox" style="width: 110px" data-options="onSelect:function(date){dateBoxOnselectEvent();}">   
        &nbsp;&nbsp;
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'," onclick="dg_Load()">查询</a>&nbsp; 
        <a id='btn-Archive' class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-archive16'," onclick="ArchiveData()">流向数据归档</a>&nbsp; 
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-leaveStock'," onclick="OpenFbxDialoge()">文件导入流向</a> 
        <div id='div-state'></div>
    </div>
    <div id="dlg-Import" class="easyui-dialog" title="批量信息导入" data-options="iconCls:'icon-leaveStock',modal:true,closed:true">
        <table id="dg-Import" class="easyui-datagrid">
        </table>
        <div id="tb-Import">
                <div>
                    请选择年月份:
                    <input type="text" id="date-import" name="date-import" class="easyui-datebox" style="width: 110px" data-options="required:true">
                </div>
            <form id="fmFile" method="post" enctype="multipart/form-data">
                
                <div>
                    <input id="fbx" class="easyui-filebox" label="信息文件:" labelposition="left" 
                    data-options="onChange:function(){uploadFiles('showFile');},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1" >
                </div>
                
                <input type="hidden" name="act" id="actFbx"/>
            </form>
        </div>
    </div>
    <script type="text/javascript">
        var Url = 'FlowManage.aspx';
        var UploadIndex=-1;
        $(document).ready(function () {
            InitDatagrid();
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

        function InitDateBox()
        {
            //$('#date').datebox({  
            //    onShowPanel : function() {// 显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层  
            //        span.trigger('click'); // 触发click事件弹出月份层  
            //        if (!tds)  
            //            setTimeout(function() {// 延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔  
            //                tds = p.find('div.calendar-menu-month-inner td');  
            //                tds.click(function(e) {  
            //                    e.stopPropagation(); // 禁止冒泡执行easyui给月份绑定的事件  
            //                    var year = /\d{4}/.exec(span.html())[0]// 得到年份  
            //                    , month = parseInt($(this).attr('abbr'), 10) + 1; // 月份  
            //                    $('#date').datebox('hidePanel')// 隐藏日期对象  
            //                    .datebox('setValue', year + '-' + month); // 设置日期的值  
            //                });  
            //            }, 0);  
            //    },  
            //    parser : function(s) {// 配置parser，返回选择的日期  
            //        if (!s)  
            //            return new Date();  
            //        var arr = s.split('-');  
            //        return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);  
            //    },  
            //    formatter : function(d) {  
            //        if (d.getMonth() == 0) {  
            //            return d.getFullYear()-1 + '-' + 12;  
            //        } else {  
            //            return d.getFullYear() + '-' + d.getMonth();  
            //        }  
            //    }// 配置formatter，只返回年月                  
            //});  
            //var p = $('#date').datebox('panel'), // 日期选择对象  
            //tds = false, // 日期选择对象中月份  
            //span = p.find('span.calendar-text'); // 显示月份层的触发控件    
            //$('#date').datebox({
            //    onSelect: function (date) {
            //        alert(date.getFullYear() + ":" + (date.getMonth() + 1));
            //    }
            //});
            $('#date').datebox({
                //onShowPanel: function () {// 显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层    
                //    span.trigger('click'); // 触发click事件弹出月份层    
                //    if (!tds)
                //        setTimeout(function () {// 延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔    
                //            tds = p.find('div.calendar-menu-month-inner td');
                //            tds.click(function (e) {
                //                e.stopPropagation(); // 禁止冒泡执行easyui给月份绑定的事件    
                //                var year = /\d{4}/.exec(span.html())[0]// 得到年份    
                //                , month = parseInt($(this).attr('abbr'), 10) + 1; // 月份    
                //                $('#date').datebox('hidePanel')// 隐藏日期对象    
                //                .datebox('setValue', year + '-' + month); // 设置日期的值    
                //            });
                //        }, 0);
                //},
                parser: function (s) {// 配置parser，返回选择的日期    
                    if (!s)
                        return new Date();
                    var arr = s.split('-');
                    return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
                },
                formatter: function (d) {
                    var month = d.getMonth();
                    if (month <= 10) {
                        month = "0" + month;
                    }
                    if (d.getMonth() == 0) {
                        return d.getFullYear() - 1 + '-' + 12;
                    } else {
                        return d.getFullYear() + '-' + month;
                    }
                }// 配置formatter，只返回年月    
            });
            $('#date-import').datebox({
                formatter: function (d) {
                    var month = d.getMonth();
                    if (month <= 10) {
                        month = "0" + month;
                    }
                    if (d.getMonth() == 0) {
                        return d.getFullYear() - 1 + '-' + 12;
                    } else {
                        return d.getFullYear() + '-' + month;
                    }
                }// 配置formatter，只返回年月    
            });
            //var p = $('#date').datebox('panel'), // 日期选择对象    
            //tds = false, // 日期选择对象中月份    
            //span = p.find('span.calendar-text'); // 显示月份层的触发控件    
        }

        function dateBoxOnselectEvent()
        {
            dg_Load();
        }

        function InitDatagrid() {
            //主页面表格初始化
            //InitDateBox();
            var d = new Date();
            var nowtime = d.toLocaleString();
            $('#date').datebox('setValue',nowtime);
            $('#dg').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: false,
                showFooter: true,
                frozenColumns: [[
                    { field: 'Hospital', width: 200, align: 'center', title: '医院', sortable: "true" },
                    { field: 'Product', width: 160, align: 'center', title: '产品', sortable: "true" },
                    { field: 'Sector', width: 50, align: 'center', title: '区域', sortable: "true" },
                    { field: 'Sales', width: 50, align: 'center', title: '业务员', sortable: "true" },
                    { field: 'FlowSalesMoney', width: 50, align: 'center', title: '流向金额' },
                    { field: 'FlowSales', width: 60, align: 'center', title: '流向', sortable: "true" },
                    { field: 'NetSalesMoney', width: 60, align: 'center', title: '纯销金额', sortable: "true" },
                    { field: 'NetSales', width: 60, align: 'center', title: '纯销', sortable: "true" },
                    { field: 'StockLastMonth', width: 60, align: 'center', title: '上月库存', sortable: "true" },
                    { field: 'StockThisMonth', width: 60, align: 'center', title: '当月库存', sortable: "true" }
                ]],
                columns: [[       
                    { field: 'ExaminePrice', width: 60, align: 'center', title: '考核价', sortable: "true" },    
                    { field: 'TotalCost', width: 60, align: 'center', title: '费用合计', sortable: "true" },  
                    { field: 'OperatingProfit', width: 60, align: 'center', title: '营业利润', sortable: "true" },  
                    { field: 'OperatingProfitRatio', width: 60, align: 'center', title: '营业利润率', sortable: "true" , formatter: RatioFormatter},  
                    { field: 'OperatingProfitForYear', width: 60, align: 'center', title: '当年营业利润', sortable: "true" },   
                    { field: 'NetProfit', width: 60, align: 'center', title: '净利润', sortable: "true" },  
                    { field: 'QuotaForYear', width: 60, align: 'center', title: '当年指标', sortable: "true" },          
                    { field: 'HospitalSupplyPrice', width: 60, align: 'center', title: '医院供货价', sortable: "true" },
                    { field: 'InvoicePrice', width: 60, align: 'center', title: '公司开票价' },
                    { field: 'SalesAllowances', width: 60, align: 'center', title: '销售折让' },                    
                    { field: 'PurchasingCost', width: 60, align: 'center', title: '采购成本' },
                    { field: 'GrossProfit', width: 60, align: 'center', title: '毛利' },
                    { field: 'TotalCost', width: 60, align: 'center', title: '费用合计' },
                    { field: 'OperatingProfit', width: 60, align: 'center', title: '运营利润' },
                    { field: 'TaxCost', width: 60, align: 'center', title: '税金' },
                    { field: 'TaxRatio', width: 60, align: 'center', title: '税金比率', formatter: RatioFormatter },
                    { field: 'FixedAssetsCost', width: 60, align: 'center', title: '固定资产分摊' },
                    { field: 'FixedAssetsRatio', width: 80, align: 'center', title: '固定资产分摊比率', formatter: RatioFormatter },
                    { field: 'FinancialCost', width: 60, align: 'center', title: '财务费用' },
                    { field: 'FinancialRatio', width: 80, align: 'center', title: '财务费用比率', formatter: RatioFormatter },
                    { field: 'RdCost', width: 60, align: 'center', title: '研发费用' },
                    { field: 'RdRatio', width: 60, align: 'center', title: '研发费用比率', formatter: RatioFormatter },
                    { field: 'HeadOfficeManageCost', width: 60, align: 'center', title: '总部管理费用' },
                    { field: 'HeadOfficeManageRatio', width: 80, align: 'center', title: '总部管理费用比率', formatter: RatioFormatter },
                    { field: 'WageSocialSecurityCost', width: 60, align: 'center', title: '工资社保' },
                    { field: 'WageSocialSecurityRatio', width: 80, align: 'center', title: '工资社保金额比率', formatter: RatioFormatter },
                    { field: 'BusinessCost', width: 60, align: 'center', title: '商务费用' },
                    { field: 'BusinessRatio', width: 80, align: 'center', title: '商务费用金额比率', formatter: RatioFormatter },
                    { field: 'DevelopmentCost', width: 60, align: 'center', title: '开发费用' },
                    { field: 'DevelopmentRatio', width: 60, align: 'center', title: '开发费用比率', formatter: RatioFormatter },
                    { field: 'SalesDirectorCost', width: 60, align: 'center', title: '销售总监费用' },
                    // { field: 'SalesDirectorRatio', width: 80, align: 'center', title: '销售总监费用比率', formatter: RatioFormatter },
                    { field: 'ProductDevelopmentFundCost', width: 60, align: 'center', title: '产品发展基金' },
                    { field: 'ProductDevelopmentFundRatio', width: 80, align: 'center', title: '产品发展基金比率', formatter: RatioFormatter },
                    { field: 'MarketCost', width: 60, align: 'center', title: '市场学术费' },
                    { field: 'MarketRatio', width: 80, align: 'center', title: '市场学术费比率', formatter: RatioFormatter },
                    { field: 'MarketReadjustmentCost', width: 60, align: 'center', title: '市场调节基金' },
                    //{ field: 'MarketReadjustmentRatio', width: 80, align: 'center', title: '市场调节基金比率', formatter: RatioFormatter },
                    { field: 'ExperimenterBonusCost', width: 60, align: 'center', title: '实验员奖金' },
                    { field: 'ExperimenterBonusRatio', width: 80, align: 'center', title: '实验员人员奖金比率', formatter: RatioFormatter },
                    { field: 'PmsCost', width: 60, align: 'center', title: 'PMS' },
                    //{ field: 'PmsRatio', width: 60, align: 'center', title: 'PMS比率', formatter: RatioFormatter },
                    { field: 'GuestMaintenanceCost', width: 60, align: 'center', title: '日常客情维护费' },
                    //{ field: 'GuestMaintenanceRatio', width: 80, align: 'center', title: '日常客情维护费比率', formatter: RatioFormatter },
                    { field: 'TfCost', width: 60, align: 'center', title: '实验费(TF)' },
                    { field: 'TfRatio', width: 60, align: 'center', title: '实验费(TF)比率', formatter: RatioFormatter },
                    { field: 'RegionalCenterCost', width: 60, align: 'center', title: '区域中心费用' },
                    { field: 'RegionalCenterRatio', width: 80, align: 'center', title: '区域中心费用比率', formatter: RatioFormatter },
                    { field: 'RegionalCenterVipCost', width: 80, align: 'center', title: '区域中心费用VIP' },
                ]],
                sortName: 'Hospital',
                sortOrder: 'asc',
                onSortColumn: function (sort, order) {
                    dg_sort( sort, order);
                },
                rowStyler: function(index,row){
                    if(row.IsFooter){
                        return 'background-color:#6293BB;color:#fff;font-weight:bold;';
                    }
                }
            });

            $('#dg-Import').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#tb-Import',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: false,
                columns: [[]],
                onSelect: function (index, row) {                    
                    if (UploadIndex == index) {
                        var jsonStr = JSON.stringify(row);
                        var data = { act: 'import', json: jsonStr,dateString:$('#date-import').datebox('getValue') };
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
                }
            });     

            $('#dlg-Import').dialog({
                buttons: [{
                    text: '导入',
                    iconCls: 'icon-leaveStock', size: 'large',
                    handler: function () {
                        UploadIndex = 0;
                        $('#dg-Import').datagrid('selectRow', UploadIndex);                         
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
                    $('#dg-Import').datagrid({
                        columns: [[]]
                    });
                },
                onClose: function () {
                    dg_Load();
                }
            });
        }

        function RatioFormatter(value, row, index) {
            if(row.IsFooter)
                return;
            if(value == undefined || value == null)
                value=0;
            return value + '%';
        }

        function getLastMonthDay(date) {
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            //var   firstdate = year + '-' + month + '-01';  
            var day = new Date(year, month, 0);
            var lastdate = year + '-' + month + '-' + day.getDate();//获取当月最后一天日期    
            return lastdate;
        }

        function dg_sort(sort, order){
            if (sort == undefined) {
                sort = 'Hospital'; order = 'asc';
            }
            var data = {
                act: 'dataGridSort',
                data: JSON.stringify($('#dg').datagrid('getData').rows),
                sort: sort, order: order
            };
            var res =AjaxSync(Url,data);
            var datasource = $.parseJSON(res);         
            $('#dg').datagrid("loadData", (datasource));
        }

        function dg_Load(sort, order) {
            if (sort == undefined) {
                sort = 'Hospital'; order = 'asc';
            }
            var data = {
                act: 'getInfos',
                date: $('#date').datebox('getValue'),
                sort: sort, order: order
            };
            //var columns = new Array();
            //var obj = data.rows[0];
            //var dict = AjaxSync(Url, { act: 'getDict' });

            parent.Loading(true);
            $.post(Url, data, function (res) {
                if (res != 'F' && res != "") {
                    var datasource = $.parseJSON(res);               
                    if(datasource.DataIsArchived){
                        $('#div-state').html('<span style="color: #FFFFFF; background-color: #FF0000">当月流向数据已归档</span>');
                        $('#btn-Archive').linkbutton('disable');
                    }     
                    else{
                        $('#div-state').html('<span style="color: #FFFFFF; background-color: #00CC00">当月流向数据未归档</span>');
                        $('#btn-Archive').linkbutton('enable');
                    }
                    $('#dg').datagrid("loadData", (datasource.Data));
                }
                parent.Loading(false);
            });
        }

        function ArchiveData(){
            var json = JSON.stringify($('#dg').datagrid('getData').rows);
            var data = {
                act: 'archive',
                date: $('#date').datebox('getValue'),
                dataJson:json
            };
            parent.Loading(true);
            $.post(Url,data,function(res){
                $.messager.alert('提示',res,'info');
                parent.Loading(false);
            });
        }

        function OpenFbxDialoge() {
            DatagridClear('dg-Import');
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
                        try {
                            var datasource = $.parseJSON(res);
                            DgImportLoad(datasource);
                        }
                        catch (e) {
                            $.messager.alert('错误提示', res + '\r\n' + e, 'error');
                        }
                    }
                });
            }
        }

        function DatagridClear(dgId) {
            $('#' + dgId).datagrid('loadData', { total: 0, rows: [] });
        }

        function DgImportLoad(data) {
            var columns = new Array();
            var frozenColumns = new Array();
            var obj = data.rows[0];
            DgImportLength = data.total;
            //columns.push();
            $.each(obj, function (key, value) {
                if (key == '状态') {
                    frozenColumns.push({
                        field: '状态', title: '状态', width: 150, align: 'center',frozen:true,
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
    </script>
</body>
</html>
