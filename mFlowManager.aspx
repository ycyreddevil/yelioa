<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mFlowManager.aspx.cs" Inherits="mFlowManager" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <title>历史数据查询</title>
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/datagrid-detailview.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
    <script src="Scripts/weui.min.js"></script>
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <style>
        input[type="date"]::-webkit-calendar-picker-indicator {
           display: none;
        }

        /*----------用来移除叉叉按钮----------*/
        input[type="date"]::-webkit-clear-button{
           display:none;
        }
    </style>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                    background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog"
        border="false" noheader="true" closed="true" modal="true">
    </div>
    <table id="dg" class="easyui-datagrid" title="流向信息"><thead data-options="frozen:true"></thead></table>
    <div id="tb" style="padding: 2px 5px;">
        年：<input class="easyui-textbox" readonly  id="year" value="" style="width: 60px"/><a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'" id="showYear">年份选择</a>
        &nbsp;月：<input class="easyui-textbox" readonly  id="month" value="" style="width: 60px"/><a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'" id="showMonth">月份选择</a>
        <%--<input type="text" id="date" class="easyui-datebox" style="width: 110px" data-options="onSelect:function(date){dateBoxOnselectEvent();}"> --%>  
        <br/>人员或部门：<input id="dg-searchbox" class="easyui-textbox" data-options="prompt:'首字母或文字'" style="width: 100px">
        &nbsp;医院：<input id="dg-searchbox_hospital" class="easyui-textbox" data-options="prompt:'首字母或文字'" style="width: 100px">
        <br/>产品：<input id="dg-searchbox_product" class="easyui-textbox" data-options="prompt:'首字母或文字'" style="width: 100px">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search',"
            onclick="dg_Load($('#dg-searchbox').searchbox('getValue'))">查询</a>&nbsp; 
    </div>
</body>
<script>
    var year = new Date().getFullYear();
    var month = new Date().getMonth() + 1;
    //var day = new Date().getDate();

    $("#showYear").click(function () {
        weui.picker([
            {
                label: '2017',
                value: 2017
            },
            {
                label: '2018',
                value: 2018,
            },
            {
                label: '2019',
                value: 2019,
            },
            {
                label: '2020',
                value: 2020,
            }
            ],
            {
                className: 'custom-classname',
                container: 'body',
                defaultValue: [1],
                onChange: function (result) {
                },
                onConfirm: function (result) {
                    year = result[0].value;
                    $('#year').textbox("setValue", result[0].value);
                },
                id: 'showYear'
            });
    })

    $("#showMonth").click(function () {
        var dayArray = new Array();
        for (i = 1; i <= 12; i++) {
            var days = {};
            days["label"] = '' + i;
            days["value"] = i;
            dayArray.push(days);
        }
        weui.picker(dayArray,
            {
                className: 'custom-classname',
                container: 'body',
                defaultValue: [month],
                onChange: function (result) {
                },
                onConfirm: function (result) {
                    month = result[0].value;
                    $('#month').textbox("setValue", result[0].value);
                },
                id: 'showMonth'
            });
    })

    function RatioFormatter(value, row, index) {
        if (row.IsFooter)
            return;
        if (value == undefined || value == null)
            value = 0;
        return value + '%';
    }

    $(document).ready(function () {
        $('#year').textbox("setValue", year);
        $('#month').textbox("setValue", month);
        initDate();
    })

    //function dateBoxOnselectEvent() {
    //    dg_Load();
    //}

    //$("#date").on("change", function () {
    //    var this_time = $('#date').val();
    //    var this_datetime = new Date(this_time);

    //    year = this_datetime.getFullYear();
    //    month = this_datetime.getMonth() + 1; 
    //    dg_Load();
    //})

    function dg_Load(searchStr, sort, order) {
        var hospital = $('#dg-searchbox_hospital').searchbox('getValue');
        var product = $('#dg-searchbox_product').searchbox('getValue');
        if (sort == undefined) {
            sort = 'Hospital'; order = 'asc';
        }
        if (searchStr == undefined) {
            searchStr = "";
        }
        var data = {
            act: 'getDataList',
            year: year,
            month: month,
            sort: sort, order: order, searchString: searchStr, hospital: hospital, product:product
        };
        //var columns = new Array();
        //var obj = data.rows[0];
        //var dict = AjaxSync(Url, { act: 'getDict' });

        Loading(true);
        $.post('mFlowManager.aspx', data, function (res) {
            if (res != "") {
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
            }
            Loading(false);
        });
    }

    var initDate = function () {
        $('#dg').datagrid({
            view: detailview,
            singleSelect: true,
            fit: true,
            toolbar: '#tb',
            url: 'mFlowManager.aspx',
            queryParams: {
                act: 'getDataList',
                year: year,
                month: month,
            },
            striped: true,
            rownumbers: true,
            collapsible: false,
            fitColumns: true,
            showFooter: true,
            columns: [[
                { field: 'Hospital', width: 200, align: 'left', title: '医院', sortable: "true" },
                { field: 'Product', width: 160, align: 'left', title: '产品', sortable: "true" },
                { field: 'FlowSales', width: 60, align: 'right', title: '流向', sortable: "true" },
                { field: 'Id', align: 'center', hidden:'true'}
            ]],
            sortName: 'Hospital',
            sortOrder: 'asc',
            onSortColumn: function (sort, order) {
                dg_Load("",sort, order);
            },
            rowStyler: function (index, row) {
                if (row.IsFooter) {
                    return 'background-color:#6293BB;color:#fff;font-weight:bold;';
                }
            },
            detailFormatter: function (index, row) {
                return '<div class="ddv" id="ddv'+ index +'" style="padding:5px 0"></div>';
            },
            onLoadSuccess: function (data) {
                if (data.total == 0) {
                    $(this).datagrid('appendRow', { Hospital: '<div style="text-align:center;color:red">没有相关记录！</div>' }).datagrid('mergeCells', { index: 0, field: 'Hospital', colspan: 3 });  
                }
            },
            onExpandRow: function (index, row) {
                console.log(index)
                var ddv = $(this).datagrid('getRowDetail', index).find('div.ddv');
                ddv.panel({
                    border: false,
                    cache: false,
                    href: 'mFlowManager.aspx?act=getDetailData&id='+row.Id,
                    onLoad: function (res) {
                        var data = $.parseJSON(res)
                        var datasource = $.parseJSON(data.data);
                        var isPrivilege = data.privilege;
                        // $('#dg').datagrid('fixDetailRowHeight', index);
                        // $('#dg').datagrid('fixRowHeight', index);
                        var subTable = '<table>' +
                            '<tr>' +
                            '<td style="border:0"><span style="font-weight:bold">区域</span>: ' + datasource[0].Sector + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">业务员</span>: ' + datasource[0].Sales + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">流向金额</span>: ' + datasource[0].FlowSalesMoney + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">纯销金额</span>: ' + datasource[0].NetSalesMoney + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">纯销</span>: ' + datasource[0].NetSales + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">上月库存</span>: ' + datasource[0].StockLastMonth + '</td>' +
                            '</tr>';
                        if (isPrivilege == true) {
                            subTable +=  '<tr>' +
                            '<td style="border:0"><span style="font-weight:bold">当月库存</span>: ' + datasource[0].StockThisMonth + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">考核价</span>: ' + datasource[0].ExaminePrice + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">费用合计</span>: ' + datasource[0].TotalCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">营业利润</span>: ' + datasource[0].OperatingProfit + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">营业利润率</span>: ' + datasource[0].OperatingProfitRatio + '%</td>' +
                            '<td style="border:0"><span style="font-weight:bold">当年营业利润</span>: ' + datasource[0].OperatingProfitForYear + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">净利润</span>: ' + datasource[0].NetProfit + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">当年指标</span>: ' + datasource[0].QuotaForYear + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">医院供货价</span>: ' + datasource[0].HospitalSupplyPrice + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">公司开票价</span>: ' + datasource[0].InvoicePrice + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">销售折让</span>: ' + datasource[0].SalesAllowances + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">采购成本</span>: ' + datasource[0].PurchasingCost + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">运营利润</span>: ' + datasource[0].OperatingProfit + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">毛利</span>: ' + datasource[0].GrossProfit + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">税金</span>: ' + datasource[0].TaxCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">税金比率</span>: ' + datasource[0].TaxRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">固定资产分摊</span>: ' + datasource[0].FixedAssetsCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].FixedAssetsRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">财务费用</span>: ' + datasource[0].FinancialCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].FinancialRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">研发费用</span>: ' + datasource[0].RdCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].RdRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">总部管理费用</span>: ' + datasource[0].HeadOfficeManageCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].HeadOfficeManageRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">工资社保</span>: ' + datasource[0].WageSocialSecurityCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].WageSocialSecurityRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">商务费用</span>: ' + datasource[0].BusinessCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].BusinessRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">开发费用</span>: ' + datasource[0].DevelopmentCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].DevelopmentRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">销售总监费用</span>: ' + datasource[0].SalesDirectorCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">市场调节基金</span>: ' + datasource[0].MarketReadjustmentCost + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">产品发展基金</span>: ' + datasource[0].ProductDevelopmentFundCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].ProductDevelopmentFundRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">市场学术费</span>: ' + datasource[0].MarketCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].MarketRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">实验员奖金</span>: ' + datasource[0].ExperimenterBonusCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].ExperimenterBonusRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">PMS</span>: ' + datasource[0].PmsCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">日常客情维护费</span>: ' + datasource[0].GuestMaintenanceCost + '</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">实验费(TF)</span>: ' + datasource[0].TfCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].TfRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">区域中心费用</span>: ' + datasource[0].RegionalCenterCost + '</td>' +
                            '<td style="border:0"><span style="font-weight:bold">比率</span>: ' + datasource[0].RegionalCenterRatio + '%</td>' +
                            '</tr><tr>' +
                            '<td style="border:0"><span style="font-weight:bold">区域中心费用VIP</span>: ' + datasource[0].RegionalCenterVipCost + '</td>' +
                            '</tr>' +
                            '</table >';
                        } else {
                            subTable += '</table >';
                        }
                            
                        $("#ddv"+index).html(subTable);
                        $('#dg').datagrid('fixDetailRowHeight', index);
                        $('#dg').datagrid('fixRowHeight', index);
                    },
                    onLoadSuccess:function(data){
                        // $('#dg').datagrid('fixDetailRowHeight',index);
                        // $('#dg').datagrid('fixRowHeight', index);
                    }
                });
                // $('#dg').datagrid('fixDetailRowHeight', index);
                // $('#dg').datagrid('fixRowHeight', index);
            }
        });
    }
</script>

</html>