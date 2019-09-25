<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mPointApply.aspx.cs" Inherits="mPointApply" %>

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
            .m-list .textbox.easyui-fluid{
                border:none;
            }
             #_easyui_textbox_input2 {
                 text-align: right
             }
             #_easyui_textbox_input3{
                text-align:right
            }
            input[type=date]::-webkit-inner-spin-button { visibility: hidden; } 
            input[type="date"]::-webkit-clear-button{
               display:none;
            }
        </style>
       
    </head>
<body>
     <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog" border="false"
                noheader="true" closed="true" modal="true">
    </div>
    <div class="easyui-navpanel" style="position: relative; padding: 20px">
        <header style="margin-bottom: 10px">
            <div class="m-toolbar">
                <span class="m-title">积分申请</span>
            </div>
           
        </header>
        <div>
            <ul class="m-list" id="pointApply">
                <li>日期:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="CreatingTime" type="date" name="date" style="width: 40%; float: right; border: 0px" />
                </a>
                </li>
                <li>事件:
                <br />
                    <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                        <input data-options="multiline:true" id="Event" class="easyui-textbox" name="Event" style="width: 90%; float: right; border: 0px; height: 150px" />
                    </a>
                </li>
                <li>类型:<a id="Type" style="text-align: right; width: 80%; float: right" href="javascript:void(0)" onclick="openit('类型')">奖分</a></li>
                <br /> 
                <HR style="border:3px double #987cb9" width="100%" color=#987cb9 SIZE=3> <br />
                <li>B积分:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="Bpoint" class="easyui-numberbox" name="Bpoint" style="width: 40%;text-align:right; float: right; border: 0px" />
                </a>
                </li>
                <li>被申请人:<a id="Target" name="Target" style="text-align: right; width: 60%; float: right" href="javascript:void(0)" onclick="openit('被申请人',this)">请选择</a></li>
                <br /> <HR style="border:3px double #987cb9" width="100%" color=#987cb9 SIZE=3> <br />
            </ul>
           <div style="text-align:center;margin-top:30px">
                <a href="#" class="easyui-linkbutton c4" onclick="addPoint()" style="width:100%;height:40px" data-options="iconCls:'icon-add'"><span style="font-size:16px">添加更多积分项</span></a>
            </div>
             <div style="text-align:center;margin-top:30px">
                <a href="#" class="easyui-linkbutton c1" onclick="applypoint()" style="width:100%;height:40px"><span style="font-size:16px">提交</span></a>
            </div>
        </div>
    </div>

    <div id="p2" class="easyui-navpanel" data-options="footer:'#footer'">
        <header>
            <div class="m-toolbar">
                <span id="p2-title" class="m-title">选项</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" onclick="confirmInformer()">返回</a>
                </div>
                <div class="m-right">
                    <a href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="confirmInformer()">确定</a>
                </div>
            </div>
        </header>
        <div>
            <input id="search" class="easyui-textbox" style="width: 100%;">
            <ul class="m-list" id="detailList">
            </ul>
            <div id="dl" style="height: 500px" data-options="
                border: false,
                lines: true,
                singleSelect: false
                ">
            </div>
        </div>
        <div style="padding: 20px 40px" id="footer">
            <a id="targetFooter" href="#" class="easyui-linkbutton" data-options="iconAlign:'right',size:'small',disabled:true">被申请人:</a>
        </div>
    </div>
     


    <script type="text/javascript">
        var url = "mPointApply.aspx";
        var Target = new Array();
        var Number;
        $(document).ready(function () {
            $("#CreatingTime").val(formatterDate(new Date()));
            init(); 
        });

        function init() {
            $("#search").textbox({
                icons: [{
                    iconCls: 'icon-search',
                    handler: function (e) {
                        var name = $("#search").textbox("getValue");
                        findTarget(name);
                    }
                }],
            });
            $('#dl').datalist({
                textField: 'target',
                valueField: 'value'
            })
        }

        function openit(type, number) { 
            Number = number;
            $("#detailList").empty();
            $("#dl").datalist('loadData', { total: 0, rows: [] });
            $.each($("#footer").children("a"), function (i, v) {
                             if (i != 0) {
                                 $(v).remove();
                             }
                         })
            if (type == "类型") {               
                $("#search").textbox('disable'); 
                
                var html = "";
                html += '<li><a href="javascript:void(0)" onclick="setValue(\'奖分\')">奖分 </a></li>';
                html += '<li><a href="javascript:void(0)" onclick="setValue(\'扣分\')">扣分 </a></li>';
                //html += '<li><a href="javascript:void(0)" onclick="setValue(\'非工作类奖分\')">非工作类奖分 </a></li>';
                //html += '<li><a href="javascript:void(0)" onclick="setValue(\'非工作类扣分\')">非工作类扣分 </a></li>';
                $("#detailList").append(html);
            }
            else {
                $("#search").textbox('enable');               
                findTarget("");  
                
            }
            $.mobile.go('#p2');
        }
        function findTarget(name) {
            $.post(url, { act: "findTarget", name: name },
                function (res) {
                    if (res != "") {
                        $('#dl').datalist({
                            textField: 'target',
                            valueField: 'value',
                            onClickRow: function (index, row) {
                                var flag = 0;
                                $.each(Target, function (i, v) {
                                    if (v.wechatUserId == row.value) {
                                        Target.splice(i, 1);
                                        flag = 1;
                                        return false;
                                    }
                                });
                                $.each($("#footer").children("a"), function (i, v) {
                                    if (i != 0 && ($(v).text().replace(/^\s+|\s+$/g, "") == row.target || $(v).text() == row.target)) {
                                        $(v).remove();
                                        return;
                                    }
                                });
                                if (flag == 0) {
                                    Target.push({
                                        name: row.target,
                                        wechatUserId: row.value,
                                        number: Number
                                    })
                                    var html = '<a style="margin-left:10px" href="javascript:void(0)"  onclick="removeInformer(this,\'' + row.value + '\',\'' + index + '\')"  class="easyui-linkbutton"  data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'small\'">';
                                    html += row.target;
                                    html += "</a >";
                                    $("#footer").append(html);
                                    $.parser.parse($("#targetFooter").parent());
                                }
                            }
                        });
                        var msg = JSON.parse(res);
                        $('#dl').datalist('loadData', { total: msg.length, rows: msg });
                        $.each(Target, function (i, v) {
                            if (v.number == Number) {
                                var html = "";
                                $.each(msg, function (j, row) {
                                    if (row.value == v.wechatUserId) {
                                        html = '<a style="margin-left:10px" href="javascript:void(0)"  onclick="removeInformer(this,\'' + row.value + '\',\'' + j + '\')"  class="easyui-linkbutton"  data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'small\'">';
                                        html += row.target;
                                        html += "</a >";
                                        $("#footer").append(html);
                                        $.parser.parse($("#targetFooter").parent());
                                        $('#dl').datalist('selectRow', j);
                                    }
                                })

                            }
                        });
                    }
                });
        }
         var setValue = function (data) {           
                 $("#Type").html(data);      
                 $.mobile.back();
        }
        var confirmInformer = function () {
            $.each($("a[name='Target']"), function (i, v) {
                var html = "";
                $.each(Target, function (j, k) {
                    if (k.number == v)
                        html += k.name + ",";
                });
                html = html.substring(0, html.length - 1);
                if (html != "")
                    $(v).html(html);
            })
            $.mobile.back();
         }    
        function removeInformer(a, value, index) {                        
             $(a).remove();
             $('#dl').datalist('unselectRow', index);
             deleteTarget(value);
        }

        function deleteTarget(value) {
            $.each(Target, function (i, v) {
                if (v.wechatUserId ==value) {
                    Target.splice(i, 1);                  
                    return false;
                }
            });
        }
       function  formatterDate(date) {
            var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
            var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : "0"
                + (date.getMonth() + 1);
           
            return date.getFullYear() + '-' + month + '-' + day;
        }
        function addPoint() {
            var html = '<li>B积分:';
            html += '<a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">';
            html += '<input class="easyui-numberbox" name="Bpoint" style="width: 40%;text-align:right;float: right; border: 0px" /></a> </li>';
            html += '<li>被申请人:<a name="Target" style="text-align: right; width: 60%; float: right" href="javascript:void(0)" onclick="openit(\'被申请人\',this)">请选择</a></li>';
            html += ' <div style="text-align:center;margin-top:30px">';
            html += ' <a href="#" name="delete" class="easyui-linkbutton c5" onclick="deletePoint(this)"  style="width:100%;height:40px"><span style="font-size:16px">删除</span></a></div>';
            html += ' <br /> <HR style="border:3 double #987cb9" width="100%" color=#987cb9 SIZE=3> <br />';
            $('#pointApply').append(html);            
            $.parser.parse($('#pointApply'));
        }
        function deletePoint(target) {
            $(target).parent().prev().prev().remove();
            var number = $(target).parent().prev();
            for (var i = Target.length - 1; i >= 0; i--) {
                if (Target[i].number == $(number).children("a")[0])
                    Target.splice(i, 1);
             }
            $(number).remove();          
            $(target).parent().next().remove();
            $(target).parent().next().remove();
            $(target).parent().next().remove();
            $(target).parent().remove();

        }

        function applypoint() {
            var date = $('#CreatingTime').val();
            var event = $('#Event').textbox("getValue");
            var type = $('#Type').html();
            var flag = 1;
            var arr1 = new Array();
            var arr = new Array();
            $.each($("input[name='Bpoint']"), function (i, v) {   
                if ($(v).val() == null || $(v).val() == "" || $(v).val() == "0") {
                    $.messager.alert('提示', "积分数目不能为0或空，请重新输入！", 'info');
                    flag = 0;                   
                    return false;
                }
                else if ($(v).val()<0&&type=="奖分"){
                    $.messager.alert('提示', "奖分，B积分应为正，不必输入负号'-'，请重新输入！", 'info');
                    flag = 0;                   
                    return false;
                }
                else if ($(v).val()>0&&type=="扣分"){
                    $.messager.alert('提示', "扣分，B积分应为负，请输入负号'-'，请重新输入！", 'info');
                    flag = 0;                   
                    return false;
                }
                else {
                    arr1.push($(v).val());
                }
            })
            $.each($("a[name='Target']"), function (i, v) {
                if ($(v).val() == null || $(v).val() == "请选择" ) {
                    $.messager.alert('提示', "请输入人员名字！", 'info');
                    flag = 0;
                    return false;
                }
                else {
                    $.each(Target, function (j, k) {
                        if ($("a[name='Target']")[i] == k.number)
                            arr.push({
                                Target: k.name,
                                wechatUserId: k.wechatUserId,
                                Bpoint:arr1[i]
                            }) 
                    })
                    
                }
            })
            if (event == null || event == "")
                $.messager.alert('提示', "请输入事件！", 'info');
            else if (flag == 0) {
                return;
            }
            else {
                Loading(true);
                $.post(url, {
                    act: 'applypoint',
                    date: date,
                    event: event,
                    type: type,
                    targets: JSON.stringify(arr),
                }, function (res) {
                    Loading(false);
                    if (res == "提交发布成功！") {
                        $.messager.confirm('提示', res + "是否继续提交新的申请？", function (r) {
                            if (r) {
                                location.reload();
                            }
                            else {
                                window.location.href = "http://yelioa.top/mPointApplyRecord.aspx";
                            }

                        });
                    }
                    else
                        $.messager.alert('提示', res, 'info');
                });
            }
        }

    </script>

</body>
</html>
