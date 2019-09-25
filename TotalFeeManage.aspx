<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TotalFeeManage.aspx.cs" Inherits="TotalFeeManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>移动报销财务审批数据</title>
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

<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
        background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog"
        border="false" noheader="true" closed="true" modal="true">
    </div>
    <div class="easyui-accordion" style="width:100%;height:800px">
        <div title="费用汇总表" data-options="iconCls:'icon-ok'" style="overflow:auto;padding:10px;">
            <table id="dg" class="easyui-datagrid">
                <thead data-options="frozen:true"></thead>
            </table>
        </div>
        <div title="税额计算表" data-options="iconCls:'icon-help'" style="overflow:auto;padding:10px;">
            <table id="dg1" class="easyui-datagrid">
                <thead data-options="frozen:true"></thead>
            </table>
        </div>
    </div>

    <%--<div>
        <table id="dg1" class="easyui-datagrid" title="税额计算表">
            <thead data-options="frozen:true"></thead>
        </table>
    </div>--%>
    <div id="tb" style="padding: 2px 5px;">
        月份查询:
        <input id="date" type="text" class="easyui-datebox">
        汇总表类型:
        <select id="type" class="easyui-combobox" style="width:110px;" data-options="editable:false,panelHeight:'auto'">
            <option value="1">业力职能</option>
            <option value="2">业力销售</option>
            <option value="3">中申职能</option>
            <option value="4">中申销售</option>
            <option value="5">东森销售</option>
            <option value="6">东森职能</option>
            <option value="7">业力科技</option>
            <option value="8">老康科技</option>
            <option value="9">天津</option>
            <option value="10">业力医学</option>
            <option value="11">傲沐科技</option>
        </select>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search',"
            onclick="dg_search();">查询</a>&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-print'"
            onclick="exportExcel();">导出</a>&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'"
            onclick="importVoucher();">凭证导入</a>
    </div>
</body>

<script>
    var d = new Date();
    $(function () {
        dateboxShowMonth('date');
        var firstDate = d.getFullYear() + "-" + (d.getMonth() + 1) + "-1";
        $("#date").datebox('setValue', firstDate);
        initDatagrid();
    })

    function dateboxShowMonth(id) {
        var db = $('#' + id);
        db.datebox({
            onShowPanel: function () {//显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层
                span.trigger('click'); //触发click事件弹出月份层
                if (!tds) setTimeout(function () {//延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔
                    tds = p.find('div.calendar-menu-month-inner td');
                    tds.click(function (e) {
                        e.stopPropagation(); //禁止冒泡执行easyui给月份绑定的事件
                        var year = /\d{4}/.exec(span.html())[0]//得到年份
                            , month = parseInt($(this).attr('abbr'), 10); //月份，这里不需要+1
                        db.datebox('hidePanel')//隐藏日期对象
                            .datebox('setValue', year + '-' + month); //设置日期的值
                    });
                }, 0);
                yearIpt.unbind();//解绑年份输入框中任何事件
            },
            parser: function (s) {
                if (!s) return new Date();
                var arr = s.split('-');
                return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
            },
            formatter: function (d) {
                var yearstr = d.getFullYear();
                var month = d.getMonth() + 1;
                var monthstr = month < 10 ? "0" + month : month;
                return yearstr + '-' + monthstr;
            }
        });
        var p = db.datebox('panel'), //日期选择对象
            tds = false, //日期选择对象中月份
            yearIpt = p.find('input.calendar-menu-year'),//年份输入框
            span = p.find('span.calendar-text'); //显示月份层的触发控件    
    }

    function dg_search() {
        initDatagrid()
    }

    function initDatagrid() {
        parent.Loading(true);
        $('#dg').datagrid({
            singleSelect: true,
            fit: true,
            toolbar: '#tb',
            striped: true,
            rownumbers: true,
            collapsible: false,
            fitColumns: true
        });

        $('#dg1').datagrid({
            singleSelect: true,
            fit: true,
            striped: true,
            rownumbers: true,
            collapsible: false,
            fitColumns: true
        });

        $.post('TotalFeeManage.aspx', {
            action: 'getTotalFeeDatagrid',
            type: $("#type").val(),
            startTm: $('#date').datebox('getValue') + '-06',
            endTm: d.getFullYear() + "-" + (d.getMonth() + 2) + '-05'
        }, function (res) {
            var data = $.parseJSON(res);
            if (data.length == 0) {
                $('#dg').datagrid({ columns: [] }).datagrid("loadData", data);
                return;
            }
            var columns = new Array();
            // 获取所有的key
            var keyList = Object.keys(data[0])
            $.each(keyList, (key, value) => {
                var column = {};
                column["field"] = value;
                column["title"] = value;
                column["width"] = 100;
                column["align"] = 'center';
                column["sortable"] = true;
                columns.push(column);
            });
            $('#dg').datagrid({
                columns: [columns]
            }).datagrid("loadData", data);

            $.post('TotalFeeManage.aspx', {
                action: 'getTotalTaxDatagrid',
                type: $("#type").val(),
                startTm: $('#date').datebox('getValue') + '-01',
                endTm: d.getFullYear() + "-" + (d.getMonth() + 1) + '-01'
            }, function (res) {
                parent.Loading(false);
                data = $.parseJSON(res);
                if (data.length == 0) {
                    $('#dg1').datagrid({ columns: [] }).datagrid("loadData", data);
                    return;
                }
                var columns = new Array();
                // 获取所有的key
                var keyList = Object.keys(data[0])
                $.each(keyList, (key, value) => {
                    var column = {};
                    column["field"] = value;
                    column["title"] = value;
                    column["width"] = 100;
                    column["align"] = 'center';
                    column["sortable"] = true;
                    columns.push(column);
                });
                $('#dg1').datagrid({
                    columns: [columns]
                }).datagrid("loadData", data);


            });
        });
    }

    function exportExcel() {
        parent.Loading(true);
        $.post('TotalFeeManage.aspx', {
            action: 'exportExcel',
            date:$('#date').datebox('getValue') + '-01',
            type: $("#type").val(),
            dataTotal: JSON.stringify($('#dg').datagrid('getData').rows),
            dataFax: JSON.stringify($('#dg1').datagrid('getData').rows)
        }, res => {
            parent.Loading(false);
            response = $.parseJSON(res);
            if (response.ErrCode == 0) {
                window.location.href = 'ExportExcelHelper.aspx?fileName=' + response.FileName + '.xls&fileCode=' + response.FileCode;
            } else {
                $.messager.alert('提示', res, 'info');
            }
        })
    }
</script>

</html>