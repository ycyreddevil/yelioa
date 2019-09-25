<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mEmailDetail.aspx.cs" Inherits="mEmailDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>业力邮箱</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link rel="stylesheet" href="Scripts/themes/mobile.css">
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://unpkg.com/vant/lib/index.css">
    <style>
        .van-panel__content{padding:20px}
        .van-cell__title{width:185px}
        .van-cell__label{width:320px}
        [v-cloak] {
            display: none;
        }
    </style>
</head>
<body>
    <div id="emailTitle" v-cloak>
        <van-nav-bar :title="subject"
            left-text="返回"
            right-text="新建"
            left-arrow
            @click-left="backToList"
            @click-right="createEmail">
        </van-nav-bar>
        <div>
            <van-panel :title="sender" :desc="receiver" :status="formateDate">
                <div style="padding:20px; font-family:'Microsoft YaHei'" id="emailText" v-html="text">
                    
                </div>
                <div>
                    <van-cell-group v-for="attachment in attachments"> 
                        <van-cell :title="attachment.FileName" is-link :url="attachment.FilePath" />
                        <template slot="icon">
                            <img v-if="attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.pdf'" src="Scripts/email/icon/PDF.png" />
                            <img v-else-if="attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.jpg' 
                                || attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.jpeg' 
                                || attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.png'" 
                                src="Scripts/email/icon/pic.png" />
                            <img v-else-if="attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.docx' 
                                || attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.doc'" src="Scripts/email/icon/word.png" />
                            <img v-else-if="attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.pptx' 
                                || attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.ppt'" src="Scripts/email/icon/ppt.png" />
                            <img v-else-if="attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.xls' 
                                || attachment.FilePath.substring(attachment.FilePath.lastIndexOf('.'), attachment.FilePath.length) == '.xlsx'" src="Scripts/email/icon/excel.png" />
                            <img v-else src="Scripts/email/icon/document.png"/>
                        </template>
                    </van-cell-group>
                </div>
                <div slot="footer">
                    <van-button type="default" @click="replyEmail(emailId)">回复</van-button>
                    <%--<van-popup v-model="popupReplay" position="bottom" >
                        <van-cell-group>
                            <van-field v-model="replyMsg" placeholder="回复">
                                <van-button slot="button" size="small" type="primary" @click="reply">发送</van-button>
                            </van-field>
                        </van-cell-group>
                    </van-popup>--%>
                    <van-button type="primary" @click="forwardEmail(emailId)">转发</van-button>
                    <van-button type="danger" @click="deleteEmail(emailId)">删除</van-button>
                </div>
            </van-panel>
        </div>
    </div>
</body>
<script src="Scripts/jquery.min.js"></script>
<script src="Scripts/mobileCommon.js"></script>
<script src="Scripts/vue.js"></script>
<script src="https://unpkg.com/vant/lib/vant.min.js"></script>
<script>
    var vm = new Vue({
        el: '#emailTitle',
        data: {
            type: '',
            emailId: '',
            subject: '',
            text: '',
            time: '',
            sender: '',
            receiver: '',
            fullReceiver: '',
            shortReceiver: '',
            attachments: [],
            show: false,
            popupReplay: false,
            replyMsg: '',
            dialog: '',
            isShowAllName: false,
        },
        methods: {
            backToList: function () {
                window.history.back();
            },
            createEmail: function () {
                createEmail();
            },
            replyEmail: function (id) {
                replyEmail(id);
            },
            forwardEmail: function (id) {
                forwardEmail(id);
            },
            deleteEmail: function (id) {
                this.$dialog.confirm({
                    message: '确定删除吗?'
                }).then(() => {
                    deleteEmailList(id, vm.type);
                }).catch(() => {

                });
            },
        },
        mounted: function () {
            var id = GetQueryString('id');
            var type = GetQueryString('type');
            loadDetail(id, type);

            this.dialog = this.$dialog
            setTimeout('$(".van-cell__label").append("<a href=\'javascript:void(0)\' onclick=\'showOrHide()\'>显示</a>")',500)
            //$(".van-cell__label").append("<a id='nameList'>展示</a>");
        },
        computed: {
            formateDate() {
                return this.time.toString().replace("T", " ").substring(0,16);
            },
        }
    })

    Vue.filter('newLineText', function (text) {
        return text.replace("\r\n", "<br/>")
    })

    function showOrHide() {
        vm.isShowAllName = !vm.isShowAllName;
        if (vm.isShowAllName) {
            vm.receiver = vm.fullReceiver;
            setTimeout('$(".van-cell__label").append("<a href=\'javascript:void(0)\' onclick=\'showOrHide()\'>隐藏</a>")', 500)
        } else {
            vm.receiver = vm.shortReceiver;
            setTimeout('$(".van-cell__label").append("<a href=\'javascript:void(0)\' onclick=\'showOrHide()\'>显示</a>")', 500)
        }
    }

    function loadDetail(id, type) {
        $.ajax({
            url: 'mEmailDetail.aspx',
            data: {act: 'loadDetail', id:id},
            type: 'post',
            dataType: 'json',
            success: function (data) {
                var email = JSON.parse(data.Email)[0];
                var recipient = JSON.parse(data.Recipient).rows;
                var attachment = JSON.parse(data.Attachment).rows;

                vm.type = type;
                vm.emailId = email.Id;
                vm.subject = email.Subject;
                vm.text = email.Text;
                vm.time = email.SendTime;
                vm.sender = '发件人:' + email.SendName;

                $("#emailText").val(email.Text);

                var receiver = "收件人:";
                for (i = 0; i < recipient.length; i++) {
                    receiver += recipient[i].ReceiveName + ",";
                }
                vm.shortReceiver = receiver.substring(0, receiver.indexOf(",", 22)) + "...";
                vm.fullReceiver = receiver;
                vm.receiver = vm.shortReceiver
                vm.attachments = attachment;
            },
            error: function () {

            }
        })
    }

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    function createEmail() {
        $.ajax({
            url: 'mEmailIndex.aspx',
            type: 'post',
            dataType: 'json',
            data: { act: 'createEmail' },
            success: function (msg) {
                var emailId = msg.Id;
                location.href = "mSendEmail.aspx?emailId=" + emailId;
            }
        })
    }

    function replyEmail(id) {
        $.ajax({
            url: 'mEmailIndex.aspx',
            type: 'post',
            dataType: 'json',
            data: { act: 'createEmail'},
            success: function (msg) {
                var emailId = msg.Id;
                location.href = "mSendEmail.aspx?emailId=" + emailId + "&replyData=true&originId="+id;
            }
        })
    }

    function forwardEmail(id) {
        $.ajax({
            url: 'mEmailIndex.aspx',
            type: 'post',
            dataType: 'json',
            data: { act: 'createEmail'},
            success: function (msg) {
                var emailId = msg.Id;
                location.href = "mSendEmail.aspx?emailId=" + emailId + "&forwardData=true&originId=" + id;
            }
        })
    }

    function deleteEmailList(id, type) {
        $.ajax({
            url: 'mEmailList.aspx',
            type: 'post',
            dataType: 'json',
            data: { act: 'deleteEmailList', ids: id, type: type },
            success: function (msg) {
                vm.dialog.alert({
                    message: '删除成功'
                }).then(() => {
                    window.history.back();
                });
            }
        })
    }
</script>
</html>
