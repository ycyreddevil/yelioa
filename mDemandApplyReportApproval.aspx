<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mDemandApplyReportApproval.aspx.cs" Inherits="mDemandApplyReportApproval" %>

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
                background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog"
            border="false" noheader="true" closed="true" modal="true">
        </div>
        <div id="p1" class="easyui-navpanel">
            <header>
                <div class="m-toolbar">
                    <span class="m-title">货需申请表单</span>
                </div>
            </header> 
           <div id="tt" class="easyui-tabs" data-options="tabHeight:40,fit:true,tabPosition:'bottom',border:false,pill:true,narrow:true,justified:true">
            <div id="preview1">
                   <div class="panel-header tt-inner">
                        我提交的
                    </div>
            </div>
                <div id="preview2">
                    <div class="panel-header tt-inner">
                        待我审批的
                    </div>
                </div>
              </div>
         </div>
        <div id="p2" class="easyui-navpanel" style="position:relative;padding:20px">
            <header>
                <div class="m-toolbar">
                    <span class="m-title">货需申请明细</span>
                </div>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" style="width:50px" onclick="$.mobile.back()">返回</a>
                </div>
            </header>
            <div>
                <form id="fm1" class="easyui-form" method="post" data-options="novalidate:true">
                    <input class="easyui-textbox" type="hidden" id="docCode1" name="Id" value=""/>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="申请人:" prompt="申请人" style="width:100%" name="UserName" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="收货单位:" prompt="收货单位" style="width:100%" name="HospitalName" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="产品名称:" prompt="产品名称" style="width:100%" name="ProductName" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="产品规格:" prompt="产品规格" style="width:100%" name="Specification" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="单位:" prompt="单位" style="width:100%" name="Unit" data-options="editable:false">
                    </div>
                   <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="货需申请:" prompt="货需申请" style="width:100%" name="DeliverNumber" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="本月终端库存:" prompt="本月终端库存" style="width:100%" name="Stock" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="本月纯销:" prompt="本月纯销" style="width:100%" name="NetSales" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="备注:" prompt="备注" style="width:100%;height:100px" name="Remark" data-options="editable:false,multiline:true">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="更新日期:" prompt="更新日期" style="width:100%" name="LMT" data-options="editable:false">
                    </div>
                </form>
            </div>
        </div>
        <div id="p3" class="easyui-navpanel" style="position:relative;padding:20px">
            <header>
                <div class="m-toolbar">
                    <span class="m-title">货需申请明细</span>
                </div>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" style="width:50px" onclick="$.mobile.back()">返回</a>
                </div>
            </header>
            <div>
                <form id="fm2" class="easyui-form" >
                    <input class="easyui-textbox"  type="hidden" id="docCode2" value="" name="Id"/>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="申请人:" prompt="申请人" style="width:100%" name="UserName" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="收货单位:" prompt="收货单位" style="width:100%" name="HospitalName" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="产品名称:" prompt="产品名称" style="width:100%" name="ProductName" data-options="editable:false">
                    </div>
                     <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="产品规格:" prompt="产品规格" style="width:100%" name="Specification" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="单位:" prompt="单位" style="width:100%" name="Unit" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="货需申请:" prompt="货需申请" style="width:100%" name="DeliverNumber" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="本月终端库存:" prompt="本月终端库存" style="width:100%" name="Stock" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="本月纯销:" prompt="本月纯销" style="width:100%" name="NetSales" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="备注:" prompt="备注" style="width:100%;height:100px" name="Remark" data-options="editable:false,multiline:true">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="更新日期:" prompt="更新日期" style="width:100%" name="LMT" data-options="editable:false">
                    </div>
                </form>
            </div>
            <footer>
                <div class="m-buttongroup m-buttongroup-justified" style="width:100%">
                    &nbsp;
                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-ok',size:'large',iconAlign:'top',plain:true" onclick="approval('同意')">通过</a>&nbsp;
                    <a href="javascript:void(0)"   class="easyui-linkbutton" data-options="iconCls:'icon-no',size:'large',iconAlign:'top',plain:true" onclick="approval('不同意')">拒绝</a>&nbsp;
                    <a href="javascript:void(0)"  class="easyui-linkbutton" data-options="iconCls:'icon-back',size:'large',iconAlign:'top',plain:true" onclick="$.mobile.back()">返回</a>&nbsp;
                </div>
            </footer>
        </div>
    <script type="text/javascript">
        var url = "mDemandApplyReportApproval.aspx";
        $(document).ready(function () {
            var code = GetQueryString('docCode');
            if (code == null) {
                myapply();
                pending();
            }
            else {
                ShowDocumentByCode(code);
            }   
            
        });

        $("#tt").tabs({
            selected: <%=Get()%>,
            onSelect: function (title, index) {
                if (index == 0) {
                    myapply();
                } else {
                    pending();
                }
            },
        })

        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }

        function ShowDocumentByCode(code) {
            var res = AjaxSync('mDemandApplyReportApproval.aspx', { act: 'getDocument', docCode: code });
            try {
                var data = $.parseJSON(res)[0];
                if ('<%=Get()%>' == '0') {
                    $.mobile.go('#p2');
                    $('#fm1').form('load', data);
                } else {
                    $.mobile.go('#p3');
                    $('#fm2').form('load', data);
                }
            } catch (error) {
                $.messager.alert('提示', res);
            }

        }

        function myapply() {
            $.post(url, {act:'myapply'}, function (res) {
                if (res != "") {
                    var tempData = $.parseJSON(res);
                    $("#preview1").empty();
                    for (i = 0; i < tempData.length; i++) {
                        var html = "";
                        html += '<div class="weui-form-preview">';
                        html += ' <div class="weui-form-preview__hd">';
                        html += '<label class="weui-form-preview__label">收货单位</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].HospitalName + '</span>';
                        html += '</div>';
                        html += '<div class="weui-form-preview__bd">';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品名称</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].ProductName + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品规格</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Specification + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">单位</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Unit + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品数量</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].DeliverNumber + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">备注</label>';
                        html += '<span class="weui-form-preview__value">' + (tempData[i].Remark ==""? "无" : tempData[i].Remark) + '</span>';
                        html += '</p>';
                        html += '</div>';
                        html += '<div class="weui-form-preview__ft">';
                        html += "<a class='weui-form-preview__btn weui-form-preview__btn_primary' href='javascript:' onClick='ShowDetail1(" + JSON.stringify(tempData[i]) + ")'>查看更多</a>";
                        html += '</div>';
                        html += '</div>';

                        $("#preview1").append(html);
                    }
                } else {
                    $.messager.alert('提示', '暂无数据', 'info');
                }
               
            });
        }

        function ShowDetail1(data) {
            $.mobile.go('#p2');
            $('#fm1').form('load', data);

        }
        function pending() {
            $.post(url, { act: 'pending'}, function (res) {
                if (res != "") {
                    var tempData = $.parseJSON(res);
                    $("#preview2").empty();
                    for (i = 0; i < tempData.length; i++) {
                        var html = "";
                        html += '<div class="weui-form-preview">';
                        html += ' <div class="weui-form-preview__hd">';
                        html += '<label class="weui-form-preview__label">收货单位</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].HospitalName + '</span>';
                        html += '</div>';
                        html += '<div class="weui-form-preview__bd">';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品名称</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].ProductName + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品规格</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Specification + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">单位</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].Unit + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">产品数量</label>';
                        html += '<span class="weui-form-preview__value">' + tempData[i].DeliverNumber + '</span>';
                        html += '</p>';
                        html += '<p>';
                        html += '<label class="weui-form-preview__label">备注</label>';
                        html += '<span class="weui-form-preview__value">' + (tempData[i].Remark == "" ? "无" : tempData[i].Remark) + '</span>';
                        html += '</p>';
                        html += '</div>';
                        html += '<div class="weui-form-preview__ft">';
                        html += "<a class='weui-form-preview__btn weui-form-preview__btn_primary' href='javascript:' onClick='ShowDetail2(" + JSON.stringify(tempData[i]) + ")'>查看更多</a>";
                        html += '</div>';
                        html += '</div>';

                        $("#preview2").append(html);
                    }
                } else {
                    $.messager.alert('提示', '暂无数据', 'info');
                }
                
            });
        }

        function ShowDetail2(data) {
            console.log(data)
            $.mobile.go('#p3');
            $('#fm2').form('load', data);
        }

        function approval(res) {
            Loading(true);
            $.ajax({
                url: 'mDemandApplyReportAppRoval.aspx',
                data: { act: 'approval', docCode: $("#docCode2").textbox("getValue"), approvalResult: res },
                type: 'post',
                dataType: 'json',
                success: function (msg) {
                    Loading(false)
                    $.messager.alert('提示', '审批结果:'+res, 'info', function () { location.href = 'http://yelioa.top//mDemandApplyReportAppRoval.aspx?type=1' });
                },
                error: function (msg) {
                    $.messager.alert('提示', '审批失败，请重试', 'info');
                    
                    $.mobile.back();
                }
            })
        }
  </script>
</body>
</html>
