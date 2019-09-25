<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mNetSalesUpload.aspx.cs" Inherits="mNetSalesUpload" %>

    <!DOCTYPE html>

    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title></title>
        <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
        <meta name="description" content="" />
        <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
        <link rel="stylesheet" href="Scripts/themes/mobile.css">
        <link href="Scripts/themes/icon.css" rel="stylesheet" />
        <link href="Scripts/themes/color.css" rel="stylesheet" />
        <script src="Scripts/jquery.min.js"></script>
        <script src="Scripts/jquery.easyui.min.js"></script>
        <script src="Scripts/jquery.easyui.mobile.js"></script>
        <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
        <script src="Scripts/mobileCommon.js"></script>
        <style>
            #linkbuttons a{
                margin:10px 5px 15px 20px;
            }
        </style>
    </head>
        <body>
            <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog" border="false"
                noheader="true" closed="true" modal="true">
            </div>
            <div id="p1" class="easyui-navpanel" >
                <header>
                    <div class="m-toolbar">
                        <span class="m-title">纯销上报网点列表</span>
                    </div>
                </header>
                <div id="dl" data-options="
                fit: true,
                border: false,
                lines: true
                ">
                </div>
            </div>
            <div id="p2" class="easyui-navpanel">
                <header>
                    <div class="m-toolbar">
                        <span id="p2-title" class="m-title">纯销上报</span>
                        <div class="m-left">
                            <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" onclick="$.mobile.back();Datalist_Load();">返回</a>
                        </div>
                        <div class="m-right">
                            <a href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" 
                            onclick='$("#netSalesNum").textbox("setValue", "");' style="width:60px">清空</a>
                        </div>
                    </div>
                </header>
                <form id="ff" method="post">
                    <input id="act" name="act" value="" type="hidden"/>
                    <input id="docCode" name="docCode" value="" type="hidden"/>
                    <div style="margin-bottom:10px; margin-top:10px; text-align:center">
                        <input id="time" type="text" class="easyui-textbox" required="required" label="对应时间:" prompt="对应时间" style="width:95%" name='time' data-options="editable:false">                    
                    </div>
                    <div style="margin-bottom:10px; margin-top:10px; text-align:center">
                        <input id="group" type="text" class="easyui-textbox" label="医院:" prompt="医院名称" style="width:95%" name='group' data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px; text-align:center">
                        <input id="item" class="easyui-textbox" label="产品:" prompt="产品名称" data-options="editable:false" style="width:95%" name="item">
                    </div>
                    <div style="margin-bottom:10px; text-align:center">
                        <input id="flowNum" class="easyui-numberbox" label="流向数量:" prompt="流向数量" style="width:95%" name="flowSales" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px; text-align:center">
                        <input id="netSalesNum" class="easyui-numberbox" label="纯销数量:" prompt="纯销数量" style="width:95%" 
                            name="netSales" data-options="required:true" validType="compare_NetsaleAndFlow['#flowNum']">
                    </div>
                </form>
                <div style="margin:50px 0 10px;text-align:center" id="linkbuttons">
                    <a id="submit" href="javascript:void(0)" class="easyui-linkbutton" style="width:60px;height:30px" onclick="submit()">提交</a>
                    <a id="save" href="javascript:void(0)" class="easyui-linkbutton" style="width:60px;height:30px" onclick="save()">保存</a>
                    <a href="javascript:void(0)" class="easyui-linkbutton" style="width:60px;height:30px" onclick="$.mobile.back();Datalist_Load();">取消</a>
                </div>
                
            </div>

            <script type="text/javascript">
                // 自定义比较方法
                $.extend($.fn.validatebox.defaults.rules, {
                    compare_NetsaleAndFlow: {
                        validator: function (value, param) {
                            return parseInt(value) < parseInt($(param[0]).val());
                        },
                        message: '纯销数量不能大于流向数量'
                    }
                });

                $(document).ready(function () {
                    InitDataList();
                    Datalist_Load();
                    //$('#time').datebox({
                    //    onShowPanel: function () {//显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层
                    //        span.trigger('click'); //触发click事件弹出月份层
                    //        if (!tds) setTimeout(function () {//延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔
                    //            tds = p.find('div.calendar-menu-month-inner td');
                    //            tds.click(function (e) {
                    //                e.stopPropagation(); //禁止冒泡执行easyui给月份绑定的事件
                    //                var year = /\d{4}/.exec(span.html())[0]//得到年份
                    //                    , month = parseInt($(this).attr('abbr'), 10); //月份，这里不需要+1
                    //                $('#time').datebox('hidePanel')//隐藏日期对象
                    //                    .datebox('setValue', year + '-' + month); //设置日期的值
                    //            });
                    //        }, 0);
                    //        yearIpt.unbind();//解绑年份输入框中任何事件
                    //    },
                    //    parser: function (s) {
                    //        if (!s) return new Date();
                    //        var arr = s.split('-');
                    //        return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
                    //    },
                    //    formatter: function (d) {
                    //        var a = parseInt(d.getMonth()) < parseInt('9') ? '0' + parseInt(d.getMonth() + 1) : d.getMonth() + 1;
                    //        return d.getFullYear() + '-' + a;
                    //    }
                    //});
                    //var p = $('#time').datebox('panel'), //日期选择对象
                    //    tds = false, //日期选择对象中月份
                    //    yearIpt = p.find('input.calendar-menu-year'),//年份输入框
                    //    span = p.find('span.calendar-text'); //显示月份层的触发控件
                });

                function InitDataList() {
                    $('#dl').datalist({
                        // data: data,
                        textField: 'item',
                        groupField: 'group',
                        textFormatter: function (value, row) {
                            var val;
                            if (row.status == '') {
                                val = '<a href="javascript:void(0)" class="datalist-link" >' + value 
                                    + ' <span style="background-color: #72BC11;color: #FFFFFF">未上报</span></a>';
                            } else if(row.status == '未提交') {
                                val = '<a href="javascript:void(0)" class="datalist-link">' + value 
                                    + ' <span style="background-color: #808080;color: #FFFFFF">未提交</span></a>';
                            }
                            else{
                                val = '<a href="javascript:void(0)" class="datalist-link" >' + value 
                                    + ' <span style="background-color: #FF0000;color: #FFFFFF">已上报</span></a>';
                            }
                            return val;
                        },
                        onClickRow: function (index, row) {
                            $('#ff').form('reset');
                            if (row.status == '' || row.status == '未提交') {
                                $.mobile.go('#p2');
                            } else {
                                $.messager.alert('警告', '上月纯销已提交，请勿重复提交');
                                return;
                            }
                            $("#group").textbox("setValue", row.group);;
                            $("#item").textbox("setValue", row.item);
                            $("#docCode").val(row.docCode);
                            //$("#time").textbox('setValue', row.time);
                            // 在当月的10号之前上报的为上个月 超过10号为这个月 不能修改
                            var myDate = new Date();
                            var todaydate = myDate.getDate();
                            var year = myDate.getFullYear();
                            var month = myDate.getMonth();
                            if (todaydate < 10) {
                                $("#time").textbox("setValue", year + "-" + month);
                            } else {
                                $("#time").textbox("setValue", year + "-" + (month + 1));
                            }
                            Loading(true);
                            // 获取流向数量和纯销数据
                            $.post('mNetSalesUpload.aspx',
                                { act: 'getFlowNumOfReportSales', group: $("#group").val(), item: $("#item").val() },
                                function (res) {
                                    var datasource = $.parseJSON(res);
                                    $("#netSalesNum").textbox("setValue", datasource.netSalesNum);
                                    if (datasource.errorMsg == 'success') {
                                        $("#flowNum").textbox("setValue", datasource.FlowNum);
                                    } else {
                                        // 如果流向数据没有取到，则禁用提交和保存按钮
                                        //$("#save").linkbutton("disable");
                                        //$("#submit").linkbutton("disable");
                                        $.messager.alert('警告', datasource.errorMsg);
                                    }
                                    Loading(false);
                                }
                            );
                        }
                    })
                }

                function Datalist_Load() {
                    Loading(true);
                    $.post('mNetSalesUpload.aspx', { act: 'getDatalist'}, function (res) {
                        //$.messager.alert(res);
                        var datasource = $.parseJSON(res);
                        $('#dl').datalist('loadData', datasource);
                        Loading(false);
                    });
                }

                function save() {
                    Loading(true);
                    $("#act").val("saveNetSalesNumOfReportSales");
                    $('#ff').form('submit', {
                        url: 'mNetSalesUpload.aspx',
                        onSubmit: function () {
                            return $(this).form('enableValidation').form('validate');
                        },
                        success: function (data) {
                            Loading(false);
                            var reg = /^[0-9]+.?[0-9]*$/;
                            if (reg.test(data) || data == "") {
                                $("#docCode").val(data);
                                $.messager.alert('提示', '保存成功');
                            }
                            else{
                                $.messager.alert('警告', data);
                            }
                        }
                    });
                }

                function submit() {
                    Loading(true);
                    $("#act").val("saveNetSalesNumOfReportSales");
                    $('#ff').form('submit', {
                        url: 'mNetSalesUpload.aspx',
                        onSubmit: function () {
                            return $(this).form('enableValidation').form('validate');
                        },
                        success: function (data) {
                            var reg = /^[0-9]+.?[0-9]*$/;
                            if (reg.test(data) || data == "") {
                                $("#docCode").val(data);
                                //$.messager.alert('提示', '保存成功');
                                $("#act").val("submitNetSalesNumOfReportSales");
                                $('#ff').form('submit', {
                                    url: 'mNetSalesUpload.aspx',
                                    onSubmit: function () {
                                        return $(this).form('enableValidation').form('validate');
                                    },
                                    success: function (data) {
                                        Loading(false);
                                        if (data.toString().indexOf("操作成功") > 0) {
                                            $.messager.alert('提示', '提交成功', 'info', function () {
                                                $.mobile.back();
                                                Datalist_Load();
                                            });
                                        } else {
                                            $.messager.alert('警告', data);
                                        }
                                    }
                                });
                            }
                            else {
                                $.messager.alert('警告', data);
                            }
                        }
                    });
                    
                }
            </script>
        </body>

    </html>