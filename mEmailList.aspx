<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mEmailList.aspx.cs" Inherits="mEmailList" %>

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
        .van-swipe-cell__right{
            color: #FFFFFF;
            font-size: 15px;
            width: 65px;
            height: 100%;
            display: inline-block;
            text-align: center;
            line-height: 95px;
            background-color: #F44;
        }
        .van-swipe-cell__left{
            color: #FFFFFF;
            font-size: 15px;
            width: 65px;
            height: 100%;
            display: inline-block;
            text-align: center;
            line-height: 95px;
            background-color: #00BFFF;
        }
        .van-card__thumb img {
            margin-left: -30px;
            border-radius: 45px;
            border: none;
            max-width: 100%;
            max-height: 88%;
        }
        [v-cloak] {
            display: none;
        }
    </style>
</head>
<body>
    <div id="emailTitle" v-cloak>
        <van-nav-bar title="业力邮箱"
            left-text="返回"
            right-text="新建"
            left-arrow
            @click-left="backToIndex"
            @click-right="createEmail">
        </van-nav-bar>
        <div>
            <van-search placeholder="请输入邮件主题，相关人或者正文进行搜索" v-model="value" @search="onSearch"/>
        </div>
        <div>
            <van-swipe-cell :right-width="65" :left-width="leftWidth" v-for="email in emailList" :key="email.EmailId">
                <%--<span slot="left" v-on:click="markAsReadOrUnread(email.EmailId, email.RecipientStatus)">标记</span>--%>
                <van-cell-group>
                    <van-cell is-link @click="showDetail(email.EmailId)">
                        <template>
                            <span class="van-cell-text">
                                <span>
                                    <van-icon v-show="email.RecipientStatus=='未读'" name="check" slot="left"></van-icon>
                                    <van-icon v-show="email.RecipientStatus=='已读'" name="checked" slot="left"></van-icon>
                                    <b>{{email.userName}}</b>
                                </span>
                                <van-tag v-if="email.Attachment != 0" type="success">附件</van-tag>
                                <span style="float:right"><i>{{email.SendTime}}</i></span>
                                <br/><span><strong>主题:</strong>{{email.Subject | substringText}}</span>
                                <br/><span><strong>正文:</strong>{{email.Text | substringText}}</span>
                            </span>
                        </template>
                    </van-cell>
                </van-cell-group>
                <span slot="left" @click="markAsReadOrUnread(email.EmailId, email.RecipientStatus)">标记</span>
                <span slot="right" @click="deleteEmail(email.EmailId)">删除</span>
            </van-swipe-cell>
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
            emailList: [],
            loading: false,
            finished: false,
            dialog: '',
            leftWidth: 0,
            value: '',
        },
        methods: {
            backToIndex: function () {
                window.history.back();
            },
            onSearch: function () {
                search(this.value, this.type);
            },
            createEmail: function () {
                createEmail();
            },
            deleteEmail: function (id) {
                this.$dialog.confirm({
                    message: '确定删除吗?'
                }).then(() => {
                    deleteEmailList(id, vm.type);
                }).catch(() => {
                    
                });
            },
            showDetail: function (id) {
                if (this.type == 'draft') {
                    location.href = "mSendEmail.aspx?originId=" + id;
                } else {
                    location.href = "mEmailDetail.aspx?id=" + id + "&type=" + vm.type;
                }
            },
            markAsReadOrUnread: function (id, status) {
                var message = "";
                if (status == "未读") {
                    message = "'确定标记为已读吗?"
                } else {
                    message = "'确定标记为未读吗?"
                }

                this.$dialog.confirm({
                    message: message
                }).then(() => {
                    markAsReadOrUnread(id, status);
                    }).catch(() => {
                });
            }
        },
        mounted: function () {
            var type = GetQueryString('type');
            getEmailList(type);

            this.dialog = this.$dialog;

            if (type == 'receive') {
                this.leftWidth = 65;
            }

            $(".van-cell .van-cell--clickable .van-hairline").css("background-color","#fafafa");
        },
    })

    Vue.filter('substringText', function (text) {
        if (text.length > 20)
            return text.substr(0, 20) + "...";
        else
            return text;
    })

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    function getEmailList(type) {
        var action = ""; var operation = "";
        if (type == "receive") {
            action = "getEmailList";
        } else {
            action = "getDraftAndSpamList";
            operation = type;
        }
        $.ajax({
            url: 'mEmailList.aspx',
            type: 'post',
            dataType: 'json',
            data: { act: action, operation: operation},
            success: function (msg) {
                vm.emailList = JSON.parse(msg.data).rows;
                vm.type = type;
            },
            error: function () {
                vm.dialog.alert({
                    message: '网络错误，请稍后重试！'
                }).then(() => {
                });
            }
        })
    }

    function deleteEmailList(id, type) {
        $.ajax({
            url: 'mEmailList.aspx',
            type: 'post',
            dataType: 'json',
            data: { act: 'deleteEmailList', ids: id, type: type},
            success: function (msg) {
                vm.dialog.alert({
                    message: '删除成功'
                }).then(() => {
                    location.reload();
                });
            }
        })
    }

    function markAsReadOrUnread(id,status) {
        $.ajax({
            url: 'mEmailList.aspx',
            type: 'post',
            dataType: 'json',
            data: { act: 'markAsReadOrUnread', id: id, status: status },
            success: function (msg) {
                 vm.dialog.alert({
                    message: '标记成功'
                }).then(() => {
                    location.reload();
                });
            }
        })
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

    function search(value, type) {
        $.ajax({
            url: 'mEmailIndex.aspx',
            type: 'post',
            dataType: 'json',
            data: { act: 'searchEmail', type: type, value: value},
            success: function (data) {
                vm.emailList = [];
                vm.searchEmailCount = 0;
                if (data.ErrCode == 0) {
                    vm.emailList = JSON.parse(data.data).rows;
                    vm.searchEmailCount = JSON.parse(data.data).total;
                } else if (data.ErrCode == 3) {
                    Dialog.alert({
                        message: '未找到相关邮件！'
                    }).then(() => {
                    });
                } else {
                    Dialog.alert({
                        message: '网络错误，请稍后重试！'
                    }).then(() => {
                    });
                }
                
            },
            error: function (data) {

            }
        })
    }
</script>
</html>
