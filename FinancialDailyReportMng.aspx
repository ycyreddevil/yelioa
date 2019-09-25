<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FinancialDailyReportMng.aspx.cs" Inherits="FinancialDailyReportMng" %>

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
        <script src="Scripts/pcCommon.js"></script>
    </head>

    <body>
        <div>

        </div>
        <div id="dlg-Import" class="easyui-dialog" title="批量信息导入" data-options="iconCls:'icon-leaveStock',modal:true,closed:true">
            <table id="dg-Import" class="easyui-datagrid">
            </table>
            <div id="tb-Import">
                <form id="fmFile" method="post" enctype="multipart/form-data">
                    <input id="fbx" class="easyui-filebox" label="信息文件:" labelposition="left" data-options="onChange:function(){uploadFiles('showFile');},prompt:'请选择一个xls或xlsx文件...'"
                        style="width: 50%" buttontext="请选择文件" name="file1" accept='application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel'>
                    <input type="hidden" name="act" id="actFbx" />
                </form>
            </div>
        </div>
        <script type="text/javascript">
            var Url = "FinancialDailyReportMng.aspx";
            $(document).ready(function () {
                InitTable();
                InitDlgs();
                dgLoad();
            });
            function InitDlgs() {
                $('#dlg-Import').dialog({
                    buttons: [{
                        text: '导入',
                        iconCls: 'icon-leaveStock', size: 'large',
                        handler: function () {
                            uploadRowData(Url, 'dg-Import', 0);
                        }
                    }, {
                        text: '关闭',
                        iconCls: 'icon-cancel', size: 'large',
                        handler: function () {
                            $('#dlg-Import').dialog('close');
                        }
                    }],
                    onOpen: function () {
                        onOpenEventHandler('dlg-Import');
                        $('#dg-Import').datagrid({
                            columns: [[]]
                        });
                    },
                    onClose: function () {
                        dgLoad();
                    }
                });
            }

            function InitTable() {
                $('#dg').edatagrid({
                    singleSelect: true, fit: true, striped: true, rownumbers: true, collapsible: false, fitColumns: false,
                    toolbar: [
                        //     {
                        //     text: '新增',
                        //     iconCls: 'icon-add',
                        //     handler: function () {
                        //         $('#dg').edatagrid('addRow',
                        //             { row: { wechatUserId: $('#wechatUserId').textbox('getValue') } });
                        //     }
                        // }, {
                        //     text: '删除',
                        //     iconCls: 'icon-remove',
                        //     handler: function () {
                        //         $('#dg').edatagrid('destroyRow');
                        //     }
                        // }, 
                        // {
                        //     text: '保存',
                        //     iconCls: 'icon-save',
                        //     handler: function () {
                        //         $('#dg').edatagrid('saveRow');
                        //     }
                        // }, {
                        //     text: '取消',
                        //     iconCls: 'icon-undo',
                        //     handler: function () {
                        //         $('#dg').edatagrid('cancelRow');
                        //     }
                        // }, {
                        //     text: '刷新',
                        //     iconCls: 'icon-reload',
                        //     handler: function () {
                        //         dgLoad();
                        //     }
                        // }, '-', 
                        {
                            text: '导入批量信息',
                            iconCls: 'icon-leaveStock',
                            handler: function () {
                                OpenFbxDialoge();
                            }
                        }],
                    frozenColumns: [[
                        { field: 'Hospital', width: 200, align: 'center', title: '医院', sortable: "true" },
                        { field: 'Product', width: 160, align: 'center', title: '产品', sortable: "true" },
                        { field: 'Sector', width: 50, align: 'center', title: '区域', sortable: "true" },
                        { field: 'Sales', width: 50, align: 'center', title: '业务员', sortable: "true" }
                    ]],
                    columns: [[
                        { field: 'YearTask', width: 80, align: 'center', title: '当年指标', sortable: "true" },
                        { field: 'MonthTask1', width: 80, align: 'center', title: '1月指标', sortable: "true" },
                        { field: 'MonthTask2', width: 80, align: 'center', title: '2月指标', sortable: "true" },
                        { field: 'MonthTask3', width: 80, align: 'center', title: '3月指标', sortable: "true" },
                        { field: 'MonthTask4', width: 80, align: 'center', title: '4月指标', sortable: "true" },
                        { field: 'MonthTask5', width: 80, align: 'center', title: '5月指标', sortable: "true" },
                        { field: 'MonthTask6', width: 80, align: 'center', title: '6月指标', sortable: "true" },
                        { field: 'MonthTask7', width: 80, align: 'center', title: '7月指标', sortable: "true" },
                        { field: 'MonthTask8', width: 80, align: 'center', title: '8月指标', sortable: "true" },
                        { field: 'MonthTask9', width: 80, align: 'center', title: '9月指标', sortable: "true" },
                        { field: 'MonthTask10', width: 80, align: 'center', title: '10月指标', sortable: "true" },
                        { field: 'MonthTask11', width: 80, align: 'center', title: '11月指标', sortable: "true" },
                        { field: 'MonthTask12', width: 80, align: 'center', title: '12月指标', sortable: "true" },
                        { field: 'ExaminePrice', width: 80, align: 'center', title: '考核价', sortable: "true" },
                        // { field: 'TotalCost', width: 80, align: 'center', title: '费用合计', sortable: "true" },
                        // { field: 'OperatingProfit', width: 80, align: 'center', title: '营业利润', sortable: "true" },
                        // { field: 'OperatingProfitRatio', width: 80, align: 'center', title: '营业利润率', sortable: "true", formatter: RatioFormatter },
                        // { field: 'OperatingProfitForYear', width: 80, align: 'center', title: '当年营业利润', sortable: "true" },
                        // { field: 'NetProfit', width: 80, align: 'center', title: '净利润', sortable: "true" },                        
                        { field: 'HospitalSupplyPrice', width: 80, align: 'center', title: '医院供货价', sortable: "true" },
                        { field: 'InvoicePrice', width: 80, align: 'center', title: '公司开票价' },
                        { field: 'SalesAllowances', width: 80, align: 'center', title: '销售折让' },
                        { field: 'PurchasingCost', width: 80, align: 'center', title: '采购成本' },
                        { field: 'GrossProfit', width: 80, align: 'center', title: '毛利' },
                        // { field: 'TotalCost', width: 80, align: 'center', title: '费用合计' },
                        // { field: 'OperatingProfit', width: 80, align: 'center', title: '运营利润' },
                        { field: 'TaxCost', width: 80, align: 'center', title: '税金' },
                        // { field: 'TaxRatio', width: 80, align: 'center', title: '税金比率', formatter: RatioFormatter },
                        { field: 'FixedAssetsCost', width: 80, align: 'center', title: '固定资产分摊' },
                        // { field: 'FixedAssetsRatio', width: 80, align: 'center', title: '固定资产分摊比率', formatter: RatioFormatter },
                        { field: 'FinancialCost', width: 80, align: 'center', title: '财务费用' },
                        // { field: 'FinancialRatio', width: 80, align: 'center', title: '财务费用比率', formatter: RatioFormatter },
                        { field: 'RdCost', width: 80, align: 'center', title: '研发费用' },
                        // { field: 'RdRatio', width: 80, align: 'center', title: '研发费用比率', formatter: RatioFormatter },
                        { field: 'HeadOfficeManageCost', width: 80, align: 'center', title: '总部管理费用' },
                        // { field: 'HeadOfficeManageRatio', width: 80, align: 'center', title: '总部管理费用比率', formatter: RatioFormatter },
                        { field: 'WageSocialSecurityCost', width: 80, align: 'center', title: '工资社保' },
                        // { field: 'WageSocialSecurityRatio', width: 80, align: 'center', title: '工资社保金额比率', formatter: RatioFormatter },
                        { field: 'BusinessCost', width: 80, align: 'center', title: '商务费用' },
                        // { field: 'BusinessRatio', width: 80, align: 'center', title: '商务费用金额比率', formatter: RatioFormatter },
                        { field: 'DevelopmentCost', width: 80, align: 'center', title: '开发费用' },
                        // { field: 'DevelopmentRatio', width: 80, align: 'center', title: '开发费用比率', formatter: RatioFormatter },
                        { field: 'SalesDirectorCost', width: 80, align: 'center', title: '销售总监费用' },
                        // { field: 'SalesDirectorRatio', width: 80, align: 'center', title: '销售总监费用比率', formatter: RatioFormatter },
                        { field: 'ProductDevelopmentFundCost', width: 80, align: 'center', title: '产品发展基金' },
                        // { field: 'ProductDevelopmentFundRatio', width: 80, align: 'center', title: '产品发展基金比率', formatter: RatioFormatter },
                        { field: 'MarketCost', width: 80, align: 'center', title: '市场学术费' },
                        // { field: 'MarketRatio', width: 80, align: 'center', title: '市场学术费比率', formatter: RatioFormatter },
                        { field: 'MarketReadjustmentCost', width: 80, align: 'center', title: '市场调节基金' },
                        //{ field: 'MarketReadjustmentRatio', width: 80, align: 'center', title: '市场调节基金比率', formatter: RatioFormatter },
                        { field: 'ExperimenterBonusCost', width: 80, align: 'center', title: '实验员奖金' },
                        // { field: 'ExperimenterBonusRatio', width: 80, align: 'center', title: '实验员人员奖金比率', formatter: RatioFormatter },
                        { field: 'PmsCost', width: 80, align: 'center', title: 'PMS' },
                        //{ field: 'PmsRatio', width: 80, align: 'center', title: 'PMS比率', formatter: RatioFormatter },
                        { field: 'GuestMaintenanceCost', width: 80, align: 'center', title: '日常客情维护费' },
                        //{ field: 'GuestMaintenanceRatio', width: 80, align: 'center', title: '日常客情维护费比率', formatter: RatioFormatter },
                        { field: 'TfCost', width: 80, align: 'center', title: '实验费(TF)' },
                        // { field: 'TfRatio', width: 80, align: 'center', title: '实验费(TF)比率', formatter: RatioFormatter },
                        { field: 'RegionalCenterCost', width: 80, align: 'center', title: '区域中心费用' },
                        // { field: 'RegionalCenterRatio', width: 80, align: 'center', title: '区域中心费用比率', formatter: RatioFormatter },
                        { field: 'RegionalCenterVipCost', width: 80, align: 'center', title: '区域中心费用VIP' }
                    ]],
                    idField: "Id",
                    sortName: 'Hospital',
                    sortOrder: 'asc',
                    onSortColumn: function (sort, order) {
                        dgLoad("", sort, order);
                    }
                    // saveUrl: 'MemberManage.aspx?act=saveDepartPostData',
                    // updateUrl: 'MemberManage.aspx?act=updateDepartPostData',
                    // destroyUrl: 'MemberManage.aspx?act=destroyDepartPostData',
                });

                $('#dg-Import').datagrid({
                    singleSelect: true, fit: true,
                    toolbar: '#tb-Import',
                    striped: true,
                    rownumbers: true,
                    collapsible: false,
                    fitColumns: false,
                    columns: [[]]
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

            function DgImportLoad(data) {
                var columns = new Array();
                var frozenColumns = new Array();
                var obj = data.rows[0];
                DgImportLength = data.total;
                //columns.push();
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

            function dgLoad(searchString, sort, order) {
                if (searchString == undefined) {
                    searchString = "";
                }
                if (sort == undefined) {
                    sort = 'Hospital'; order = 'asc';
                }
                var data = {
                    act: 'getInfos',
                    searchString: searchString, sort: sort, order: order
                };
                var res = AjaxSync(Url, data);
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
            }

            function OpenFbxDialoge() {
                $('#dg-Import').datagrid('loadData', { total: 0, rows: [] });
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
        </script>
    </body>

    </html>