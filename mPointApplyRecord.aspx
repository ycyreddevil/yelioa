<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mPointApplyRecord.aspx.cs" Inherits="mPointApplyRecord" %>

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
        <script src="Scripts/weui.min.js"></script>
        <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    </head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog" border="false"
                noheader="true" closed="true" modal="true">
    </div>
    
<div id="p1" class="easyui-navpanel">
            <header>
                <div class="m-toolbar">
                    <span class="m-title">积分记录</span>
                </div>
                <input id="db" class="easyui-datebox" style="width:40%"/>&nbsp;&nbsp;&nbsp;&nbsp;<input id="textbox1" class="easyui-textbox" label="共计:" style="width:40%" data-options="editable:false">
            </header>   
    <div id="tt" class="easyui-tabs" data-options="tabHeight:40,fit:true,tabPosition:'bottom',border:false,pill:true,narrow:true,justified:true">
             <div id="preview1">    
                    <div class="panel-header tt-inner">
                        与我相关的
                    </div>
               </div>
              <div id="preview2">  
                    <div class="panel-header tt-inner">
                         我提交的
                    </div>
                </div>
     </div>        
 </div>
    <script type="text/javascript">
        var p = 0;
        var myPointSum = 0;
        var myApplySum = 0;
        $(document).ready(function () { 
            initdatebox();
            $('#db').datebox('setValue', formatterdate(new Date()));
            initTBS();          
        });
        function initTBS() {
               $('#tt').tabs({
               onSelect: function (title, index) {                  
                    if (index == 0) {
                        $('#textbox1').textbox('setValue',  myPointSum);
                    }
                    else if (index == 1) {
                        $('#textbox1').textbox('setValue', myApplySum);
                    }
                }
            })
            
        }
        function initdatebox() {
            $('#db').datebox().datebox('calendar').calendar({
                validator: function (date) {
                    var now = new Date();
                    var d = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                    return date <= d;
                }
            });
            $('#db').datebox({
                editable: false,
                onChange: function (date) {  
                    $('#preview1').empty();
                    $('#preview2').empty();
                    initdatagrid1(date);
                    initdatagrid2(date);
                },
                onShowPanel: function () { //显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层
                    span.trigger('click'); //触发click事件弹出月份层
                    if (!tds) setTimeout(function () { //延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔
                        tds = p.find('div.calendar-menu-month-inner td');
                        tds.click(function (e) {
                            e.stopPropagation(); //禁止冒泡执行easyui给月份绑定的事件
                            var year = /\d{4}/.exec(span.html())[0] //得到年份
                                ,
                                month = parseInt($(this).attr('abbr'), 10); //月份，这里不需要+1
                            $('#db').datebox('hidePanel') //隐藏日期对象
                                .datebox('setValue', year + '-' + month); //设置日期的值
                        });
                    }, 0);
                    yearIpt.unbind(); //解绑年份输入框中任何事件
                },
               

                parser: function (s) {
                    if (!s) return new Date();
                    var arr = s.split('-');
                    return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
                },
                formatter: function (d) {
                    return d.getFullYear() + '-' + (d.getMonth() + 1);
                }
            });
            var p = $('#db').datebox('panel'), //日期选择对象
                tds = false, //日期选择对象中月份
                yearIpt = p.find('input.calendar-menu-year'), //年份输入框
                span = p.find('span.calendar-text'); //显示月份层的触发控件
            
        }


        function initdatagrid1(date) {
            Loading(true);
            $.post('mPointApplyRecord.aspx', { act: 'initdatagrid1',date:date }, function (res) {
                if (res != "") {
                    var tempData = $.parseJSON(res);
                    myApplySum = 0;
                    for (i = 0; i < tempData.length; i++) {
                        var html = "";
                        html += '<div class="weui-form-preview">';
                        html += '<div class="weui-form-preview__hd">';
                        html += '<label class="weui-form-preview__label">被申请人:</label>';
                        html += '<em class="weui-form-preview__value">' + tempData[i].Target + '</em>';
                        html += '</div>';
                        html += '<div class="weui-form-preview__bd">';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">事件:</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Event + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">事件日期:</label>';
                        html += '<span class="weui-form-preview__value">' + formatterdate(tempData[i].EffectiveTime) + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">事件类型:</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Type + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">B积分:</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Bpoint + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">审批状态:</label>';
                        html += '<span class="weui-form-preview__value" style="color:red">' + tempData[i].CheckState + '</span>';
                        html += '</p>';
                        html += '</div>';
                        html += '</div>';

                        if (tempData[i].CheckState != "已拒绝") {
                            myApplySum += parseInt(tempData[i].Bpoint);
                        }

                        $("#preview2").append(html);
                    }
                   
                } else {
                    p = 1;
                   
                }
                Loading(false);
            });
            
        }

        function initdatagrid2(date) {
            Loading(true);
            $.post('mPointApplyRecord.aspx', { act: 'initdatagrid2', date: date }, function (res) {
                if (res != "") {
                    var tempData = $.parseJSON(res);
                    myPointSum = 0;
                    for (i = 0; i < tempData.length; i++) {
                        var html = "";
                        html += '<div class="weui-form-preview">';
                        html += '<div class="weui-form-preview__hd">';
                        html += '<label class="weui-form-preview__label">申请人:</label>';
                        html += '<em class="weui-form-preview__value">' + tempData[i].Proposer  + '</em>';
                        html += '</div>';
                        html += '<div class="weui-form-preview__bd">';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">事件:</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Event + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">事件日期:</label>';
                        html += '<span class="weui-form-preview__value">' + formatterdate(tempData[i].EffectiveTime) + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">事件类型:</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Type + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">B积分:</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Bpoint + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">审批状态:</label>';
                        html += '<span class="weui-form-preview__value" style="color:red">' + tempData[i].CheckState + '</span>';
                        html += '</p>';
                        html += '</div>';
                        html += '</div>';

                       if (tempData[i].CheckState != "已拒绝") {
                            myPointSum += parseInt(tempData[i].Bpoint);
                        }
                        $("#preview1").append(html);
                    }
                     if ($('#tt').tabs('getTabIndex', $('#tt').tabs('getSelected')) == 0) {
                         $('#textbox1').textbox('setValue',  myPointSum);
                    }
                } else {
                    if (p == 0)
                        p = 2;
                    else
                        p = 3;
                }
                if (p == 1) {
                    $.messager.alert('提示', '暂无我提交的数据', 'info');
                }
                else if (p == 2) {
                    $.messager.alert('提示', '暂无与我相关的数据', 'info');
                }
                else if (p == 3) {
                    $.messager.alert('提示', '暂无数据', 'info');
                }
                p = 0;
                Loading(false);
            });

        }


        function formatterdate(value) {
            var d = new Date(value);
            var year = d.getFullYear();
            var month = (d.getMonth() + 1).toString();
            var day = (d.getDate()).toString();
            if (month.length == 1) {
                month = "0" + month;
            }
            if (day.length == 1) {
                day = "0" + day;
            }
            var date = year + "-" + month + "-" + day;
            return date;
        }
    </script>
</body>
</html>
