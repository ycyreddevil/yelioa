<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mEmailIndex.aspx.cs" Inherits="mEmailIndex" %>

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
    <%--<link ref="stylesheet" href="Scripts/vant/sendEmail/iconfont.css"></link>--%>
    <style>
        #searchSummary{
            background:rgb(242, 242, 242) 
        }

        [v-cloak] {
            display: none;
        }
    </style>
</head>
<body>
    <div id="emailTitle" v-cloak>
        <van-nav-bar title="业力邮箱"
            right-text="新建"
            @click-right="createEmail" >
        </van-nav-bar>
        <div class="van-doc-demo-section demo-pull-refresh">
            <van-pull-refresh v-model="isLoading" @refresh="onRefresh">
                <div>
                    <van-search placeholder="请输入邮件主题，相关人或者正文进行搜索" v-model="value" placeholder="请搜索邮件" show-action="true" @search="onSearch" @cancel="onCancel"/>
                </div>

                <div v-show="isSearch">
                    <van-tabs swipeable @click="clickTab">
                        <%--<van-tab v-for="index in 4" :title="'选项 ' + index">
                        内容 {{ index }}
                        </van-tab>--%>
                        <van-tab title="收件箱">
                            <van-cell id="searchSummary">搜索到与<span style="color:#43CD80">"{{value}}"</span>相关的共<span style="color:#43CD80">{{searchEmailCount}}</span>封邮件</van-cell>
                            <van-cell-group v-for="email in emailList">
                                <van-cell is-link @click="showDetail(email.EmailId)">
                                    <template>
                                        <span class="van-cell-text">
                                            <span v-html="email.userName">
                                                <van-icon v-show="email.RecipientStatus=='未读'" name="check" slot="left"></van-icon>
                                            </span>
                                            <van-tag v-if="email.Attachment != 0" type="success">附件</van-tag>
                                            <span style="float:right">{{email.SendTime}}</span>
                                            <br/><span><strong>主题:</strong>{{email.Subject | substringText}}</span>
                                            <br/><span><strong>正文:</strong>{{email.Text | substringText}}</span>
                                        </span>
                                    </template>
                                </van-cell>
                            </van-cell-group>
                        </van-tab>
                        <van-tab title="发件箱">
                            <van-cell id="searchSummary">搜索到与<span style="color:#43CD80">"{{value}}"</span>相关的共<span style="color:#43CD80">{{searchEmailCount}}</span>封邮件</van-cell>
                            <van-cell-group v-for="email in emailList">
                                <van-cell is-link @click="showDetail(email.EmailId)">
                                    <template>
                                        <span class="van-cell-text">
                                            <span v-html="email.userName">
                                                <van-icon v-show="email.RecipientStatus=='未读'" name="check" slot="left"></van-icon>
                                            </span>
                                            <van-tag v-if="email.Attachment != 0" type="success">附件</van-tag>
                                            <span style="float:right">{{email.SendTime}}</span>
                                            <br/><span><strong>主题:</strong>{{email.Subject | substringText}}</span>
                                            <br/><span><strong>正文:</strong>{{email.Text | substringText}}</span>
                                        </span>
                                    </template>
                                </van-cell>
                            </van-cell-group>
                        </van-tab>
                        <van-tab title="草稿箱">
                            <van-cell id="searchSummary">搜索到与<span style="color:#43CD80">"{{value}}"</span>相关的共<span style="color:#43CD80">{{searchEmailCount}}</span>封邮件</van-cell>
                            <van-cell-group v-for="email in emailList">
                                <van-cell is-link @click="showDetail(email.EmailId)">
                                    <template>
                                        <span class="van-cell-text">
                                            <span v-html="email.userName">
                                                <van-icon v-show="email.RecipientStatus=='未读'" name="check" slot="left"></van-icon>
                                            </span>
                                            <van-tag v-if="email.Attachment != 0" type="success">附件</van-tag>
                                            <span style="float:right">{{email.SendTime}}</span>
                                            <br/><span><strong>主题:</strong>{{email.Subject | substringText}}</span>
                                            <br/><span><strong>正文:</strong>{{email.Text | substringText}}</span>
                                        </span>
                                    </template>
                                </van-cell>
                            </van-cell-group>
                        </van-tab>
                        <van-tab title="垃圾箱">
                            <van-cell id="searchSummary">搜索到与<span style="color:#43CD80">"{{value}}"</span>相关的共<span style="color:#43CD80">{{searchEmailCount}}</span>封邮件</van-cell>
                            <van-cell-group v-for="email in emailList">
                                <van-cell is-link @click="showDetail(email.EmailId)">
                                    <template>
                                        <span class="van-cell-text">
                                            <span v-html="email.userName">
                                                <van-icon v-show="email.RecipientStatus=='未读'" name="check" slot="left"></van-icon>
                                            </span>
                                            <van-tag v-if="email.Attachment != 0" type="success">附件</van-tag>
                                            <span style="float:right">{{email.SendTime}}</span>
                                            <br/><span><strong>主题:</strong>{{email.Subject | substringText}}</span>
                                            <br/><span><strong>正文:</strong>{{email.Text | substringText}}</span>
                                        </span>
                                    </template>
                                </van-cell>
                            </van-cell-group>
                        </van-tab>
                    </van-tabs>
                </div>
                <div v-show="!isSearch" style="height:700px">
                    <van-cell-group >
                        <van-cell title="收件箱" url="mEmailList.aspx?type=receive" v-model="ReceiveNumber" is-link :val="ReceiveNumber">
                            <template slot="icon"><img src="Scripts/email/icon/receiveEmail.png"/>&nbsp;</template>
                        </van-cell>
                        <van-cell title="发件箱" url="mEmailList.aspx?type=send" v-model="SendNumber" icon="wechat" is-link :val="SendNumber">
                            <template slot="icon"><img src="Scripts/email/icon/sendEmail.png"/>&nbsp;</template>
                        </van-cell>
                        <van-cell title="草稿箱" url="mEmailList.aspx?type=draft" v-model="DraftNumber" icon="wechat" is-link :val="DraftNumber">
                            <template slot="icon"><img src="Scripts/email/icon/draftEmail.png"/>&nbsp;</template>
                        </van-cell>
                        <van-cell title="垃圾箱" url="mEmailList.aspx?type=delete" v-model="DeletedNumber" icon="delete" is-link :val="DeletedNumber">
                            <template slot="icon"><img src="Scripts/email/icon/trash.png"/>&nbsp;</template>
                        </van-cell>
                    </van-cell-group>
                    <van-cell-group >
                        <van-cell title="分组信息" url="mEmailGroupSetting.aspx" is-link :val="查看">
                            <template slot="icon"><img src="Scripts/email/icon/group.png"/>&nbsp;</template>
                        </van-cell>
                    </van-cell-group>
                </div>
            </van-pull-refresh>
        </div>
    </div>
</body>
<script src="Scripts/jquery.min.js"></script>
<script src="Scripts/mobileCommon.js"></script>
<script src="Scripts/vue.js"></script>
<script src="https://unpkg.com/vant/lib/vant.min.js"></script>
<%--<script src="Scripts/vant/sendEmail/iconfont.js"></script>--%>
<script>
    var vm = new Vue({
        el: '#emailTitle',
        data: {
            value: "",
            ReceiveNumber: 0,
            DraftNumber: 0,
            SendNumber: 0,
            DeletedNumber: 0,
            isSearch: false,
            emailList: [],
            searchEmailCount: 0,
            isLoading: false
        },
        methods: {
            createEmail: function () {
                createEmail();
            },
            onSearch: function () {
                this.isSearch = true;
                search(this.value, 'receive');
            },
            onCancel: function () {
                if (this.isSearch) {
                    this.isSearch = false;
                }
            },
            clickTab: function (index, title) {
                if (index == 0) {
                    search(this.value, 'receive');
                } else if (index == 1) {
                    search(this.value, 'send');
                } else if (index == 2) {
                    search(this.value, 'draft');
                } else {
                    search(this.value, 'trash');
                }
            },
            showDetail: function (id) {
                location.href = "mEmailDetail.aspx?id=" + id + "&type=" + vm.type;
            },
            onRefresh: function () {
                this.isLoading = false;
                location.reload();
            }
        },
        mounted: function () {
            getAllNumber();
        },
        //computed: {
        //    highlightName(aa,index) {
        //        console.log(index)
        //        var titleString = this.emailList[i].userName;

        //        if (this.value != '' && titleString.indexOf(this.value) != -1) {
        //            // 高亮替换v-html值
        //            let replaceString = '<span style="color: red">' + this.value + '</span>';
        //            // 开始替换
        //            titleString = titleString.replace(this.value, replaceString);
        //        }
        //        return titleString;
        //    }
        //}
    })

    Vue.filter('substringText', function (text) {
        if (text.length > 20)
            return text.substr(0, 20) + "...";
        else
            return text;
    })

    function loadGroupInfo(){

    }

    function getAllNumber() {
        $.ajax({
            url: 'mEmailIndex.aspx',
            type: 'post',
            dataType: 'json',
            data: { act:'getAllNumbers'},
            success: function (msg) {
                vm.DraftNumber = msg.DraftNumber;
                vm.SendNumber = msg.SendNumber;
                vm.DeletedNumber = msg.DeletedNumber;
                vm.ReceiveNumber = msg.ReceiveNumber;
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
