<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mSendEmail.aspx.cs" Inherits="mSendEmail" %>

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
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <script src="Scripts/weui.min.js"></script>
    <script src="Scripts/weui-fileUpload.js"></script>
    <script src="Scripts/ajaxfileupload.js"></script>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;"
        class="easyui-dialog" border="false"
        noheader="true" closed="true" modal="true">
    </div>
    <div id="p1" class="easyui-navpanel" style="position: relative; padding: 20px">
        <header>
            <div class="m-toolbar">
                <span class="m-title">写邮件</span>
                <div class="m-right">
                    <a id="submit" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="submit()" style="width: 60px">发送</a>
                </div>
                <div class="m-left">
                    <a id="Sendback" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="Sendback()" style="width: 60px">返回</a>
                </div>
            </div>
        </header>
        <ul class="m-list">
            <li>收件人:<a id="Target" name="Target" style="text-align: right; width: 60%; float: right" href="javascript:void(0)" onclick="openit()">请选择</a></li>
        </ul>

        <div class="weui-cells weui-cells_form">
            <div class="weui-cells__title">主题:</div>
            <div class="weui-cells">
                <div class="weui-cell">
                    <div class="weui-cell__bd">
                        <input id="subject" class="weui-input" type="text" placeholder="请输入主题">
                    </div>
                </div>
            </div>
            <div class="weui-cells__title">正文:</div>
            <div class="weui-cells weui-cells_form">
                <div class="weui-cell">
                    <div class="weui-cell__bd">
                        <textarea class="weui-textarea" id="body" onkeyup="wordStatic(this);" placeholder="请输入正文" rows="12"></textarea>
                        <div class="weui-textarea-counter"><span id="num">0</span>/1000</div>
                    </div>
                </div>
            </div>
            <div class="weui-cell">
                <div class="weui-cell__bd">
                    <div class="weui-uploader">
                        <div class="weui-uploader__hd">
                            <p class="weui-uploader__title" style="font-size: 10px">附件:</p>
                        </div>
                        <div class="weui-uploader__bd">
                            <ul class="weui-uploader__files" id="uploaderFiles">
                            </ul>
                            <div class="weui-uploader__input-box">
                                <input id="uploaderInput" name="uploaderInput" class="weui-uploader__input zjxfjs_file" type="file" accept="*/*" multiple="">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div>
        <div class="weui-mask" id="iosMask" style="opacity: 1; display: none"></div>
        <div class="weui-actionsheet" id="iosActionsheet">
            <%--<div class="weui-actionsheet__title">
                <p class="weui-actionsheet__title-text">{{}}</p>
            </div>--%>
            <div class="weui-actionsheet__menu">
                <div class="weui-actionsheet__cell" onclick="deleteAttachment()">删除该附件</div>
            </div>
            <div class="weui-actionsheet__action">
                <div class="weui-actionsheet__cell" id="iosActionsheetCancel">取消</div>
            </div>
        </div>
    </div>
    <div id="p2" class="easyui-navpanel" data-options="footer:'#footer'">
        <header>
            <div class="m-toolbar">
                <span id="p2-title" class="m-title">联系人</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" onclick="confirm();$.mobile.go('#p1');">返回</a>
                </div>
                <div class="m-right">
                    <a id="confirm" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="confirm();$.mobile.go('#p1');" style="width: 60px">确定</a>
                </div>
            </div>
        </header>
        <div>
            <div id="userTree" class="tree"></div>
        </div>

        <div style="padding: 20px 40px" id="footer">
            <a id="targetFooter" href="#" class="easyui-linkbutton" data-options="iconAlign:'right',size:'small',disabled:true">收件人:</a>
        </div>
    </div>
    <script type="text/javascript">
        var url = 'mSendEmail.aspx';
        var Target = new Array();
        var uploadFileUrls = new Array();
        var chooseIndex = 0;
        $(document).ready(function () {
            initUserTree();
            $("#search").textbox({
                icons: [{
                    iconCls: 'icon-search',
                    handler: function (e) {
                        var name = $("#search").textbox("getValue");
                        findTarget(name);
                    }
                }],
            });
            LimitedWordsNum();

            //$('#search').textbox('textbox').keydown(function (e) {
            //    if (e.keyCode == 13) {
            //        var name = $("#search").textbox("getValue");
            //        findTarget(name);
            //    }
            //});

            var replyData = GetQueryString('replyData');
            var forwardData = GetQueryString('forwardData');
            var originId = GetQueryString('originId');
            if (replyData != "" && replyData != null) {
                showOriginData(originId, "reply");
            } else if (forwardData != "" && forwardData != null) {
                showOriginData(originId, "forward");
            } else {
                showOriginData(originId, "draft");
            }
            var emailId = GetQueryString('emailId');
            //禁止浏览器返回按钮
            history.pushState(null, null, document.URL);
            window.addEventListener('popstate', function () {
                history.pushState(null, null, document.URL);
            });

            $("#Sendback").click(function () {
                weui.confirm('是否要将此封保存草稿', {
                    buttons: [{
                        label: '是',
                        type: 'default',
                        onClick: function () {
                            saveDraft();
                        }
                    }, {
                        label: '否',
                        type: 'default',
                        onClick: function () {
                            // 删除草稿箱中的此邮件
                            if (originId != "" && originId != null) {
                                deleteDraft(originId);
                            }
                            else {
                                deleteDraft(emailId);
                            }
                            location.href = "mEmailIndex.aspx";
                        }
                    }, {
                        label: '取消',
                        type: 'primary',
                        onClick: function () {
                                // 删除草稿箱中的此邮件
                            
                        }
                    }]
                });
            })
        });

        var $iosActionsheet = $('#iosActionsheet');
        var $iosMask = $('#iosMask');

        function hideActionSheet() {
            $iosActionsheet.removeClass('weui-actionsheet_toggle');
            $iosMask.fadeOut(200);
        }

        $iosMask.on('click', hideActionSheet);
        $('#iosActionsheetCancel').on('click', hideActionSheet);
        $("#uploaderFiles").on("click", '#showIOSActionSheet', function () {
            chooseIndex = $(this).index();
            $iosActionsheet.addClass('weui-actionsheet_toggle');
            $iosMask.fadeIn(200);
        });

        function saveDraft() {
            var emailId = GetQueryString('emailId');
            if (emailId == "" || emailId == null)
                emailId=GetQueryString('originId');
            // 保存草稿
            $.ajax({
                url: 'mSendEmail.aspx',
                data: {
                    act: 'saveDraft',
                    emailId: emailId,
                    subject: $("#subject").val(),
                    text: $("#body").val(),
                    recipients: JSON.stringify(Target),
                    uploadFiles: JSON.stringify(uploadFileUrls),
                },
                dataType: 'text',
                type: 'post',
                success: function (msg) {
                    msg = JSON.parse(msg);
                    if (msg.ErrCode != 0) {
                        weui.confirm(msg.ErrMsg, {
                            buttons: [{
                                label: '确定',
                                type: 'primary',
                                onClick: function () {
                                    location.href = "mEmailIndex.aspx";
                                }
                            }]
                        })
                    }
                    else {
                        location.href = "mEmailIndex.aspx";
                    }
                }
            });
        }

        function submit() {
            var emailId = GetQueryString('emailId');
            // 先保存草稿
            $.ajax({
                url: 'mSendEmail.aspx',
                data: {
                    act: 'saveDraft',
                    emailId: emailId,
                    subject: $("#subject").val(),
                    text: $("#body").val(),
                    recipients: JSON.stringify(Target),
                    uploadFiles: JSON.stringify(uploadFileUrls),
                },
                dataType: 'text',
                type: 'post',
                success: function (msg) {
                    msg = JSON.parse(msg);
                    if (msg.ErrCode == 0) {
                        // 再发送
                        $.ajax({
                            url: 'mSendEmail.aspx',
                            data: {
                                act: 'sendEmail',
                                emailId: emailId,                               
                            },
                            dataType: 'json',
                            type: 'post',
                            success: function (msg2) {
                                if (msg2.errcode == 0) {
                                    weui.alert("发送成功", function () {
                                        location.href = "mEmailIndex.aspx";
                                    })

                                } else {
                                    weui.alert("网络超时，请稍后重试");
                                }
                            },
                            error: function () {
                                weui.alert("网络超时，请稍后重试");
                            }
                        })
                    } else {
                        weui.alert("网络超时，请稍后重试");
                    }
                },
                error: function (msg) {
                    weui.alert("网络超时，请稍后重试");
                }
            })
        }

        function deleteDraft(emailId) {
            $.ajax({
                url: 'mSendEmail.aspx',
                data: {
                    act: 'deleteDraft',
                    emailId: emailId,
                },
                dataType: 'json',
                type: 'post',
                success: function (msg) {
                    //删除成功
                }
            })
        }

        function wordStatic(input) {
            // 获取要显示已经输入字数文本框对象  
            var content = document.getElementById('num');
            if (content && input) {
                // 获取输入框输入内容长度并更新到界面  
                var value = input.value;
                // 将换行符不计算为单词数  
                value = value.replace(/\n|\r/gi, "");
                // 更新计数  
                content.innerText = value.length;
            }
        }
        //限制输入字数
        function LimitedWordsNum() {
            $('#body').keydown(function () {
                var curLength = $('#body').val().length;
                if (curLength >= 1000) {
                    var num = $('#body').val().substr(0, 1000);
                    $('#body').val(num);
                    weui.alert('超字数限制，多出的字符将被截断！');
                } else {
                    $('.textareaLength').text(curLength);
                }
            })
        }


        function openit() {
            $.mobile.go('#p2');
        }

        function initUserTree() {
            $('#userTree').tree({
                animate: true, lines: false,
                onClick: function (node) {
                    var flag = 0;
                    $.each(Target, function (i, v) {
                        if (v.wechatUserId == node.id) {
                            Target.splice(i, 1);
                            flag = 1;
                            return false;
                        }
                    });
                    $.each($("#footer").children("a"), function (i, v) {
                        if (i != 0 && ($(v).text().replace(/^\s+|\s+$/g, "") == node.text || $(v).text() == node.text)) {
                            $(v).remove();
                            return;
                        }
                    });
                    if (flag == 0) {
                        Target.push({
                            name: node.text,
                            wechatUserId: node.id
                        })
                        var html = '<a style="margin-left:10px" href="javascript:void(0)"  onclick="remove(this,\'' + node.id + '\')"  class="easyui-linkbutton"  data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'small\'">';
                        html += node.text;
                        html += "</a >";
                        $("#footer").append(html);
                        $.parser.parse($("#targetFooter").parent());
                    }

                },
                formatter: function (node) {
                    return node.text;
                }
            });
            parent.Loading(true);
            $.post(url, { act: 'initUserTree' },
                function (res) {
                    if (res != "") {
                        data = JSON.parse(res);
                        $('#userTree').tree('loadData', data);
                        $.each(Target, function (i, v) {
                            var html = "";
                            $.each(data, function (j, node) {
                                if (node.value == v.wechatUserId) {
                                    html = '<a style="margin-left:10px" href="javascript:void(0)"  onclick="remove(this,\'' + node.value + '\')"  class="easyui-linkbutton"  data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'small\'">';
                                    html += node.text;
                                    html += "</a >";
                                    $("#footer").append(html);
                                    //$('#dl').datalist('selectRow', j);
                                    $.parser.parse($("#p2"));

                                }
                            })
                        });
                    }
                    parent.Loading(false);
                });
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
                                    })
                                    var html = '<a style="margin-left:10px" href="javascript:void(0)"  onclick="remove(this,\'' + row.value + '\',\'' + index + '\')"  class="easyui-linkbutton"  data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'small\'">';
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
                                        html = '<a style="margin-left:10px" href="javascript:void(0)"  onclick="remove(this,\'' + row.value + '\',\'' + j + '\')"  class="easyui-linkbutton"  data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'small\'">';
                                        html += row.target;
                                        html += "</a >";
                                        $("#footer").append(html);
                                        $('#dl').datalist('selectRow', j);
                                        $.parser.parse($("#p2"));

                                    }
                                })

                            }
                        });
                    }
                });
        }
        var confirm = function () {
            var html = "";
            $.each(Target, function (j, k) {
                html += k.name + ",";
            })
            html = html.substring(0, html.length - 1);
            if (html != "" && html)
                $('#Target').html(html);

        }
        function remove(a, value, index) {
            $(a).remove();
            //$('#dl').datalist('unselectRow', index);
            deleteTarget(value);
        }
        function deleteTarget(value) {
            $.each(Target, function (i, v) {
                if (v.wechatUserId == value) {
                    Target.splice(i, 1);
                    return false;
                }
            });
        }
        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }

        function showOriginData(id, type) {
            if (type == "draft") {
                $.ajax({
                    url: 'mEmailDetail.aspx',
                    type: 'post',
                    dataType: 'json',
                    data: { act: 'loadDetail', id: id },
                    success: function (data) {
                        var email = JSON.parse(data.Email);
                        $("#subject").val(email[0].Subject);
                        $("#body").val(email[0].Text);
                        var attachment = JSON.parse(data.Attachment).rows;
                        if (attachment != null && attachment != '') {
                            for (i = 0; i < attachment.length; i++) {
                                uploadFileUrls.push(attachment[i]);
                                var suffix = attachment[i].FileName.substring(attachment[i].FileName.indexOf("."), attachment[i].FileName.length);
                                var tmpl = '<li id="showIOSActionSheet" class="weui-uploader__file" style="background-image:url(#url#)"></li>';
                                if (suffix == ".docx" || suffix == ".doc" || suffix == ".txt") {
                                    $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/word.png")));
                                } else if (suffix == ".xls" || suffix == ".xlsx") {
                                    $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/excel.png")));
                                } else if (suffix == ".pdf") {
                                    $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/PDF.png")));
                                } else if (suffix == ".ppt" || suffix == ".pptx") {
                                    $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/ppt.png")));
                                } else if (suffix == ".jpg" || suffix == ".png" || suffix == ".jpeg" || suffix == ".ico" || suffix == ".gif" || suffix == ".bmp") {
                                    $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/pic.png")));
                                } else if (suffix == ".rar") {
                                    $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/RAR.png")));
                                } else {
                                    $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/document.png")));
                                }
                            }                            
                        }
                        var Recipient = JSON.parse(data.Recipient).rows;
                        if (Recipient != null && Recipient != '') {
                            for (i = 0; i < Recipient.length; i++) {
                                Target.push({
                                    name: Recipient[i].ReceiveName,
                                    wechatUserId: Recipient[i].UserId
                                });
                                var html = '<a style="margin-left:10px" href="javascript:void(0)"  onclick="remove(this,\'' + Recipient[i].UserId + '\',\'0\')"  class="easyui-linkbutton"  data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'small\'">';
                                html += Recipient[i].ReceiveName;
                                html += "</a >";
                                $("#footer").append(html);
                                $.parser.parse($("#targetFooter").parent());
                            }
                             confirm();
                        }
                    }
                })
            } else {
                $.ajax({
                    url: 'mEmailDetail.aspx',
                    type: 'post',
                    dataType: 'json',
                    data: { act: 'replyOrForwordEmail', id: id, type: type },
                    success: function (data) {
                        if (data.ErrCode == "0") {
                            $("#body").val(data.primaryData);
                            $("#subject").val(data.Subject);

                            if (type == "forward") {
                                var attachment = JSON.parse(data.Attachment).rows;
                                if (attachment != null && attachment != '') {
                                    for (i = 0; i < attachment.length; i++) {
                                        uploadFileUrls.push(attachment[i]);
                                        var suffix = attachment[i].FileName.substring(attachment[i].FileName.indexOf("."), attachment[i].FileName.length);
                                        var tmpl = '<li id="showIOSActionSheet" class="weui-uploader__file" style="background-image:url(#url#)"></li>';
                                        if (suffix == ".docx" || suffix == ".doc" || suffix == ".txt") {
                                            $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/word.png")));
                                        } else if (suffix == ".xls" || suffix == ".xlsx") {
                                            $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/excel.png")));
                                        } else if (suffix == ".pdf") {
                                            $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/PDF.png")));
                                        } else if (suffix == ".ppt" || suffix == ".pptx") {
                                            $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/ppt.png")));
                                        } else if (suffix == ".jpg" || suffix == ".png" || suffix == ".jpeg" || suffix == ".ico" || suffix == ".gif" || suffix == ".bmp") {
                                            $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/pic.png")));
                                        } else if (suffix == ".rar") {
                                            $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/RAR.png")));
                                        } else {
                                            $("#uploaderFiles").append($(tmpl.replace('#url#', "Scripts/email/icon/document.png")));
                                        }
                                    }
                                }
                            }

                            if (type == "reply") {
                                Target.push({
                                    name: data.toReplyUserName,
                                    wechatUserId: data.toReplyUserId
                                });
                                var html = '<a style="margin-left:10px" href="javascript:void(0)"  onclick="remove(this,\'' + data.toReplyUserId + '\',\'0\')"  class="easyui-linkbutton"  data-options="iconCls:\'icon-clear\',iconAlign:\'right\',size:\'small\'">';
                                html += data.toReplyUserName;
                                html += "</a >";
                                $("#footer").append(html);
                                $.parser.parse($("#targetFooter").parent());
                                confirm();
                            }
                        }
                    }
                })
            }
        }

        function deleteAttachment() {
            uploadFileUrls.splice(chooseIndex, 1);
            $("#uploaderFiles li:eq(" + chooseIndex + ")").remove()
            hideActionSheet();
        }
    </script>
</body>
</html>
