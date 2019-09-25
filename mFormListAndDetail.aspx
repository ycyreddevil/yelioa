<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mFormListAndDetail.aspx.cs" Inherits="mFormListAndDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title> 
    <script src="Scripts/weui.min.js"></script>
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <script src="Scripts/weui.min.js"></script>
    <script src="Scripts/jquery.min.js"></script>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link rel="stylesheet" href="Scripts/themes/mobile.css" />
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/jquery.easyui.mobile.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
</head>
<body>
    <div id="loadingToast" style="display: none;">
        <div class="weui-mask_transparent"></div>
        <div class="weui-toast">
            <i class="weui-loading weui-icon_toast"></i>
            <p class="weui-toast__content">加载中</p>
        </div>
    </div>

    <div class="js_dialog" id="Dialog2" style="display: none;">
        <div class="weui-mask"></div>
        <div class="weui-dialog">
            <div class="weui-dialog__bd" id="capion"></div>
            <div class="weui-dialog__ft">
                <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_primary" onclick="$('#Dialog2').fadeOut(200);GetData();$.mobile.go('#p1');">确定</a>
            </div>
        </div>
    </div>

    <div class="js_dialog" id="Dialog4" style="display: none;">
        <div class="weui-mask"></div>
        <div class="weui-dialog">
            <div class="weui-dialog__bd" id="capion4"></div>
            <div class="weui-dialog__ft">
                <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_primary" onclick="$('#Dialog4').fadeOut(200);GetData();GetList('toBeApprovedByMe')">确定</a>
            </div>
        </div>
    </div>

    <div class="js_dialog" id="Dialog5" style="display: none;">
        <div class="weui-mask"></div>
        <div class="weui-dialog">
            <div class="weui-dialog__bd" id="capion5"></div>
            <div class="weui-dialog__ft">
                <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_primary" onclick="$('#Dialog5').fadeOut(200);">确定</a>
            </div>
        </div>
    </div>


    <div id="p1" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span class="m-title" id="title1">单据查询</span>
            </div>
        </header>
        <div class="weui-cells__title"></div>
        <div class="weui-cells">
            <a class="weui-cell weui-cell_access" href="javascript:;" onclick="GetList('toBeSubmitedByMe')">
                <div class="weui-cell__bd">
                    <p>待我提交的</p>
                </div>
                <div class="weui-cell__ft" id="toBeSubmitedByMe">
                </div>
            </a>
            <a class="weui-cell weui-cell_access" href="javascript:;" onclick="GetList('toBeApprovedByMe')">
                <div class="weui-cell__bd">
                    <p>待我审批的</p>
                </div>
                <div class="weui-cell__ft" id="toBeApprovedByMe">
                </div>
            </a>
            <a class="weui-cell weui-cell_access" href="javascript:;" onclick="GetList('submitedByMe')">
                <div class="weui-cell__bd">
                    <p>我已提交的</p>
                </div>
                <div class="weui-cell__ft" id="submitedByMe">
                </div>
            </a>
            <a class="weui-cell weui-cell_access" href="javascript:;" onclick="GetList('hasApprovedByMe')">
                <div class="weui-cell__bd">
                    <p>我已审批的</p>
                </div>
                <div class="weui-cell__ft" id="hasApprovedByMe">
                </div>
            </a>
            <a class="weui-cell weui-cell_access" href="javascript:;" onclick="GetList('relatedToMe')">
                <div class="weui-cell__bd">
                    <p>与我相关的</p>
                </div>
                <div class="weui-cell__ft" id="relatedToMe">
                </div>
            </a>
        </div>
        <div class="js_dialog" id="Dialog1" style="display: none;">
            <div class="weui-mask"></div>
            <div class="weui-dialog">
                <div class="weui-dialog__bd">网络出现错误，请重试!</div>
                <div class="weui-dialog__ft">
                    <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_primary" onclick="window.history.back();">知道了</a>
                </div>
            </div>
        </div>
    </div>
    <div id="p2" class="easyui-navpanel" style="position: relative; padding: 20px;">
        <header>
            <div class="m-toolbar">
                <span class="m-title" id="title2">单据列表</span>
            </div>
            <div class="m-left">
                <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" style="width: 50px" onclick=" $.mobile.go('#p1');">返回</a>
            </div>
        </header>
        <div id="preview1">
        </div>
    </div>


    <div id="p3" class="easyui-navpanel" style="position: relative; padding: 20px" data-options="footer:'#footer'">
        <header>
            <div class="m-toolbar">
                <span class="m-title" id="title3">单据详情</span>
            </div>
            <div class="m-left">
                <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" style="width: 50px" onclick=" $.mobile.go('#p2');">返回</a>
            </div>
            <div class="m-right">
                <a href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" style="width: 50px" onclick="window.print()">打印</a>
            </div>
        </header>

        <div id="detail"></div>

        <div class="weui-gallery" id="gallery">
            <span class="weui-gallery__img" id="galleryImg"></span>
        </div>


        <div style="padding: 20px 40px" id="footer">
        </div>

        <div class="js_dialog" id="Dialog3" style="display: none;">
            <div class="weui-mask"></div>
            <div class="weui-dialog">
                <div class="weui-dialog__hd"><strong class="weui-dialog__title">审批</strong></div>
                <div class="weui-dialog__bd">
                    <div class="weui-cells weui-cells_form">
                        <div class="weui-cell">
                            <div class="weui-cell__bd">
                                <textarea id="opinion" class="weui-textarea" placeholder="请输入意见/原因" rows="4"></textarea>
                            </div
                             
                        </div>
                        
                    </div>
                </div>
                <div class="weui-dialog__bd" id="hospitalCodeShow">
                    <div class="weui-cells weui-cells_form">
                        <div class="weui-cell">
                            <div class="weui-cell__bd">
                                <textarea id="hospitalCode" class="weui-textarea" placeholder="请填写网点编码" rows="1"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="weui-dialog__bd" id="agentCodeShow">
                    <div class="weui-cells weui-cells_form">
                        <div class="weui-cell">
                            <div class="weui-cell__bd">
                                <textarea id="agentCode" class="weui-textarea" placeholder="请填写代理商编码" rows="1"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="weui-dialog__ft">
                    <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_default" onclick="$('#Dialog3').fadeOut(200)">取消</a>
                    <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_warn" onclick="Approve('已拒绝')">拒绝</a>
                    <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_primary" onclick="Approve('已审批')">通过</a>
                </div>
            </div>
        </div>

        <div class="js_dialog" id="Dialog" style="display: none;">
            <div class="weui-mask"></div>
            <div class="weui-dialog">
                <div class="weui-dialog__hd"><strong class="weui-dialog__title">撤销</strong></div>
                <div class="weui-dialog__bd">
                    确定撤销吗？
                </div>
                <div class="weui-dialog__ft">
                    <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_default" onclick="$('#Dialog').fadeOut(200)">取消</a>
                    <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_primary" onclick="SureBack();">确定</a>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var url = "mFormListAndDetail.aspx";
        var image = new Array();
        var file = new Array();
        var ID = "";
        var docID = "";
        var formName = "";
        var toBeSubmitedByMe, toBeApprovedByMe, submitedByMe, hasApprovedByMe, relateToMe;
        $(document).ready(function () {
            docID = GetQueryString('docId');
            if (docID == null) {
                GetData();
            }
            else {
                var type = GetQueryString('type');
                var url = decodeURI(location.href);

                var tmp1 = url.split("?")[1];
                var tmp2 = tmp1.split("&")[0];
                var tmp3 = tmp2.split("=")[1];
                formName = tmp3.replace("#", "");
                //GetData();
                //GetList(type);
                ShowDetail(docID, formName, type);
                $('#title1').html(formName);
                // 网点备案新增申请表并且是审批人是洪秀秀时，填写三个编码
                if ((formName == '网点备案新增申请表') && '<%= userInfo.userName %>' == "洪秀秀") {
                    $('#hospitalCodeShow').css('display', 'block');
                    $('#agentCodeShow').css('display', 'block');
                } else if ((formName == '网点备案变更申请表') && '<%= userInfo.userName %>' == "洪秀秀") {
                    $('#hospitalCodeShow').css('display', 'none');
                    $('#agentCodeShow').css('display', 'block');
                } else {
                    $('#hospitalCodeShow').css('display', 'none');
                    $('#agentCodeShow').css('display', 'none');
                }
            }
        });

        function reload() {
            location.href = 'mFormListAndDetail.aspx?formName=' + formName + '&type=toBeApprovedByMe';
        }

        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }

        function GetData() {
            $('#loadingToast').fadeIn(100);
            $.ajaxSettings.async = false;
            $.post(url, { act: 'getData' },
                function (res) {
                    $('#loadingToast').fadeOut(100);
                    var datasource = JSON.parse(res);
                    toBeSubmitedByMe = datasource.toBeSubmitedByMe;
                    toBeApprovedByMe = datasource.toBeApprovedByMe;
                    submitedByMe = datasource.submitedByMe;
                    hasApprovedByMe = datasource.hasApprovedByMe;
                    relatedToMe = datasource.relatedToMe;

                    $('#toBeSubmitedByMe').html(toBeSubmitedByMe.length);
                    $('#toBeApprovedByMe').html(toBeApprovedByMe.length);
                    $('#submitedByMe').html(submitedByMe.length);
                    $('#hasApprovedByMe').html(hasApprovedByMe.length);
                    $('#relatedToMe').html(relatedToMe.length);
                })
        }
        function GetList(type) {
            $.mobile.go('#p2');
            $('#preview1').empty();
            if ($('#' + type).html() == "0") {
                $('#capion').html("该项暂无数据!");
                $('#Dialog2').fadeIn(200);
                return;
            }
            var data; var title2;
            if (type == "toBeSubmitedByMe") {
                data = toBeSubmitedByMe;
                title2 = "待我提交的";
            } else if (type == "toBeApprovedByMe") {
                data = toBeApprovedByMe;
                title2 = "待我审批的";
            } else if (type == "submitedByMe") {
                data = submitedByMe;
                title2 = "我已提交的";
            } else if (type == "hasApprovedByMe") {
                data = hasApprovedByMe;
                title2 = "我已审批的";
            } else if (type == "relatedToMe") {
                data = relatedToMe;
                title2 = "和我相关的";
            }
            $('#title2').html(title2);
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var html = "";
                    html += '<div class="weui-form-preview">';
                    html += '<div class="weui-form-preview__hd">';
                    html += '<label class="weui-form-preview__label">表单名称</label>';
                    html += '<span class="weui-form-preview__value">' + data[i].tableName + '</span>';
                    html += '</div>';

                    html += '<div class="weui-form-preview__bd">';
                    html += '<div class="weui-form-preview__item">';
                    html += '<label class="weui-form-preview__label">单据编号</label>';
                    html += '<span class="weui-form-preview__value" align="right">' + data[i].DocCode + '</span>';
                    html += '</div>';

                    html += '<div class="weui-form-preview__item">';
                    html += '<label class="weui-form-preview__label">生成时间</label>';
                    html += '<span class="weui-form-preview__value" align="right">' + data[i].CreateTime + '</span>';
                    html += '</div>';

                    html += '<div class="weui-form-preview__item">';
                    html += '<label class="weui-form-preview__label">填表人</label>';
                    html += '<span class="weui-form-preview__value" align="right">' + data[i].userName + '</span>';
                    html += '</div>';

                    html += '<div class="weui-form-preview__item">';
                    html += '<label class="weui-form-preview__label">状态</label>';
                    html += '<span class="weui-form-preview__value" align="right">' + data[i].Status + '</span>';
                    html += '</div>';
                    html += '</div>';

                    html += '<div class="weui-form-preview__ft">';
                    html += "<a class='weui-form-preview__btn weui-form-preview__btn_primary' href='javascript:' onClick='ShowDetail(" + data[i].Id + ",\"" + data[i].tableName + "\",\"" + type + "\")'>查看更多</a>";
                    html += '</div>';
                    html += '</div><br>';

                    $("#preview1").append(html);
                }
            }
            else {
            }
        }

        function ShowDetail(docId, tableName, type) {
            docID = docId;
            $.mobile.go('#p3');
            $('#title3').html(tableName);
            $('#detail').empty();
            $('#footer').empty();
            $('#loadingToast').fadeIn(100);
            formName = tableName
            // 网点备案新增申请表或 网点备案更新申请表 并且是审批人是洪秀秀时，填写三个编码
            if ((formName == '网点备案新增申请表') && '<%= userInfo.userName %>' == "洪秀秀") {
                $('#hospitalCodeShow').css('display', 'block');
                $('#agentCodeShow').css('display', 'block');
            } else if ((formName == '网点备案变更申请表') && '<%= userInfo.userName %>' == "洪秀秀") {
                $('#hospitalCodeShow').css('display', 'none');
                $('#agentCodeShow').css('display', 'block');
            } else {
                $('#hospitalCodeShow').css('display', 'none');
                $('#agentCodeShow').css('display', 'none');
            }
            $.post(url, { act: 'getDetail', formName: tableName, docId: docId, type: type },
                function (res) {
                    $('#loadingToast').fadeOut(100);
                    var data = JSON.parse(res);
                    if (data.ErrCode == 0) {
                        var document = JSON.parse(data.document);
                        var record = JSON.parse(data.record).rows;
                        var approver = JSON.parse(data.approver).rows;
                        var formData = JSON.parse(data.form);
                        var form = JSON.parse(formData[0]["ParameterData"]);

                        ID = formData[0]["Id"];
                        var picture = ""; var file = "";

                        var html = '<form id="fm" class="easyui-form" method="post" data-options="novalidate:true">';
                        html += ' <div style="margin-bottom:10px">';
                        html += '<input class="easyui-textbox" label="单据编号:" prompt="单据编号" style="width:100%" name="DocCode" data-options="editable:false">';
                        html += '</div>';
                        html += ' <div style="margin-bottom:10px">';
                        html += '<input class="easyui-textbox" label="填表人:" prompt="填表人" style="width:100%" name="preparerName" data-options="editable:false">';
                        html += '</div>';
                        html += ' <div style="margin-bottom:10px">';
                        html += '<input class="easyui-textbox" label="填表人部门:" prompt="填表人部门" style="width:100%" name="departmentName" data-options="editable:false">';
                        html += '</div>';
                        html += ' <div style="margin-bottom:10px">';
                        html += '<input class="easyui-textbox" label="生成时间:" prompt="生成时间" style="width:100%" name="CreateTime" data-options="editable:false">';
                        html += '</div>';
                        html += ' <div style="margin-bottom:10px">';
                        html += '<input class="easyui-textbox" label="状态:" prompt="状态" style="width:100%" name="Status" data-options="editable:false">';
                        html += '</div>';

                        for (i = 0; i < form.length; i++) {

                            if (form[i].TYP == "textarea") {
                                html += ' <div style="margin-bottom:10px">';
                                html += '<input class="easyui-textbox" label="' + form[i].LBL + ':" prompt="' + form[i].LBL + '" style="width:100%;height:200px" name="' + form[i].LBL + '" data-options="editable:false,multiline:true">';
                                html += '</div>';
                            }
                            else if (form[i].TYP == "name") {
                                html += ' <div style="margin-bottom:10px">';
                                html += '<input class="easyui-textbox" label="' + form[i].LBL + ':" prompt="' + form[i].LBL + '" style="width:100%" name="' + form[i].LBL + '1" data-options="editable:false">';
                                html += '</div>';
                            }
                            else if (form[i].TYP != "image" && form[i].TYP != "file") {
                                html += ' <div style="margin-bottom:10px">';
                                html += '<input class="easyui-textbox" label="' + form[i].LBL + ':" prompt="' + form[i].LBL + '" style="width:100%" name="' + form[i].LBL + '" data-options="editable:false">';
                                html += '</div>';
                            }
                            else if (form[i].TYP == "image") {
                                image.push(form[i].LBL);
                                var strs = JSON.parse(document[0][form[i].LBL]);
                                picture += '<div class="weui-uploader">';
                                picture += '<div class="weui-uploader__hd">';
                                picture += '<p class="weui-uploader__title">' + form[i].LBL + ':</p> ';
                                picture += '</div>';
                                picture += '<div class="weui-uploader__bd">';
                                picture += '<ul class="weui-uploader__files" id="' + form[i].LBL + '" >';
                                if (strs != null && strs != "") {
                                    for (var j = 0; j < strs.length; j++) {
                                        picture += '<li class="weui-uploader__file" style="background-image:url(' + strs[j] + ')"></li>';
                                    }
                                }
                                else
                                    picture += '<li class="weui-uploader__file" style="background-image:url(resources/detail.png)"></li>';
                                picture += '</ul>';
                                picture += '</div>';
                                picture += '</div>';
                            }
                            else if (form[i].TYP == "file") {
                                file += ' <div style="margin-bottom:10px">' + form[i].LBL + ":";
                                var strs = JSON.parse(document[0][form[i].LBL]);
                                if (strs != null && strs != "") {
                                    for (var j = 0; j < strs.length; j++) {
                                        if (j == 0) {
                                            file += '<a href="' + strs[j] + '" style="margin-left:30px">' + strs[j].substr(strs[j].lastIndexOf('/') + 1, strs[j].length) + '</a><br/>';
                                        } else {
                                            file += '<a href="' + strs[j] + '" style="margin-left:80px">' + strs[j].substr(strs[j].lastIndexOf('/') + 1, strs[j].length) + '</a><br/>';
                                        }
                                    }
                                }
                                file += '</div>';
                            }
                        }
                        html += ' <div style="margin-bottom:10px">';
                        html += '<input class="easyui-textbox" label="抄送人:" prompt="抄送人" style="width:100%" name="informer" data-options="editable:false">';
                        html += '</div>';
                        html += '</div>';

                        $("#detail").append(html);
                        $.parser.parse("#p3");
                        if (picture != "") {
                            picture += '<br>';
                            $("#detail").append(picture);

                            $(function () {
                                for (var i = 0; i < image.length; i++) {
                                    $("#" + image[i]).on("click", "li", function () {
                                        $('#galleryImg').attr("style", this.getAttribute("style"));
                                        $("#gallery").fadeIn(100);
                                    });
                                }
                                $("#gallery").on("click", function () {
                                    $("#gallery").fadeOut(100);
                                });
                            });
                        }

                        if (file != "") {
                            $("#detail").append(file);

                        }

                        $.parser.parse("#p3");

                        $('#fm').form("load", document[0]);

                        if (document[0]["Status"] != "草稿") {
                            var str = "";
                            $.each(record, function (i, v) {
                                str += v.userName + " " + v.ApprovalTime.substr(0, 10) + v.ApprovalTime.substr(v.ApprovalTime.length - 9, 9) + " " + v.ApprovalResult + " " + v.ApprovalOpinion + "\r\n";
                            })
                            html = ' <div style="margin-bottom:10px">';
                            html += '<input class="easyui-textbox" label="操作记录:" prompt="操作记录" style="width:100%;height:200px" id="record" data-options="editable:false,multiline:true">';
                            html += '</div>';
                            $("#detail").children("div").eq(-1).before(html);
                            $.parser.parse("#p3");
                            $('#record').textbox("setValue", str);

                            html = "审批流程：";
                            for (var i = 0; i < approver.length; i++) {
                                if (document[0]["Level"] > approver[i].Level) {
                                    html += '<a href="#" class="easyui-linkbutton c1" data-options="iconAlign:\'top\',size:\'small\'">';
                                } else {
                                    html += '<a href="#" class="easyui-linkbutton c2" data-options="iconAlign:\'top\',size:\'small\'">';
                                }
                                html += approver[i].userName;
                                html += "</a >";
                                if (i < approver.length - 1) {
                                    html += "<span>-></span>";
                                }
                            }
                            $("#footer").append(html);
                            $.parser.parse("#p3");
                        }

                        var maxLevel = 0;
                        for (var i = 0; i < approver.length; i++) {
                            if (approver[i].Level > maxLevel)
                                maxLevel = approver[i].Level;
                        }
                        html = "";
                        if (type == "toBeApprovedByMe") {
                            html = '<a href="javascript:;" class="weui-btn weui-btn_primary" onclick="$(\'#Dialog3\').fadeIn(200);">审    批</a>';
                        }

                        if (type == "toBeSubmitedByMe") {
                            html = '<a href="javascript:;" class="weui-btn weui-btn_primary" onclick="Submit()">编    辑</a>';
                        }

                        if (type == "submitedByMe" && maxLevel > document[0].Level) {
                            html = '<a href="javascript:;" class="weui-btn weui-btn_warn" onclick="$(\'#Dialog\').fadeIn(200);">撤    销</a>';
                        }

                        if (type == "submitedByMe" && document[0].Status == '已拒绝') {
                            html = '<a href="javascript:;" class="weui-btn weui-btn_primary" onclick="Submit()">重    新    提    交</a>';
                        }


                        $("#detail").append(html);
                        $.parser.parse("#p3");
                    }
                    else if (data.ErrCode == 4) {
                        $('#capion').html(data.ErrMsg);
                        $('#Dialog2').fadeIn(200);

                        location.href = "mFormListAndDetail.aspx?formName=" + formName;
                    }
                    else {
                        $('#capion').html(data.ErrMsg);
                        $('#Dialog2').fadeIn(200);
                    }
                })
        }

        function Submit() {
            location.href = "mFormDetail.aspx?formName=" + formName + "&docId=" + docID + "&id=" + ID;
        }

        function SureBack() {
            $('#Dialog').fadeOut(200);
            $('#loadingToast').fadeIn(100);
            $.post(url, { act: "back", formName: formName, docId: docID },
                function (res) {
                    $('#loadingToast').fadeOut(100);
                    var data = JSON.parse(res);
                    if (data.ErrCode == 0) {
                        $('#capion4').html(data.ErrMsg);
                        $('#Dialog4').fadeIn(200);
                    }
                    else {
                        $('#capion5').html(data.ErrMsg);
                        $('#Dialog5').fadeIn(200);
                    }
                })
        }

        function Approve(result) {
            $('#Dialog3').fadeOut(200);
            $('#loadingToast').fadeIn(100);

            if ((formName == '网点备案新增申请表' || formName == '网点备案变更申请表') && '<%= userInfo.userName %>' == "洪秀秀"
                && $('#hospitalCode').val() == '' && $('#productCode').val() == '') {
                $('#loadingToast').fadeOut(100);
                alert('请填写网点和产品编码');
                return;
            }

            if (result == "已审批" || ($('#opinion').val() != "" && $('#opinion').val() != null)) {
                // 审批方法
                var approval = new Promise((resolve, reject) => {
                    $.ajax({
                        url: url,
                        type: 'POST',
                        dataType: "json",
                        data: {
                            act: "approve", formName: formName, docId: docID, result: result, opinion: $('#opinion').val(),
                            hospitalCode: $("#hospitalCode").val(),
                            agentCode: $("#agentCode").val()
                        },
                        success: function (resp) {
                            resolve(resp);
                        }
                    });
                })

                approval.then(res => {
                    $('#loadingToast').fadeOut(100);
                    if (res.ErrCode == 0 || res.ErrCode == 2) {
                        $('#capion4').html(res.ErrMsg);
                        $('#Dialog4').fadeIn(200);
                    }
                    else {
                        $('#capion5').html(res.ErrMsg);
                        $('#Dialog5').fadeIn(200);
                    }
                })
            } else {
                $('#loadingToast').fadeOut(100);
                $('#capion5').html("请填写原因/意见！");
                $('#Dialog5').fadeIn(200);
            }
            $('#opinion').val("");
            $('#').fadeOut(100);
        }
    </script>
</body>
</html>
