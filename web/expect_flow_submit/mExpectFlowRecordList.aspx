<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mExpectFlowRecordList.aspx.cs" Inherits="web_expect_flow_submit_mExpectFlowRecordList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>预计流向操作记录列表页</title>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-message/dist/muse-ui-message.all.css"/>
</head>
<body>
    <div id="total">
        <mu-paper :z-depth="1" class="demo-list-wrap">
            <mu-appbar color="lightBlue">
                <mu-button icon slot="left" @click="back">
                    <mu-icon value="arrow_back"></mu-icon>
                </mu-button>
                预计流向操作记录列表
                <mu-button icon slot="right">
                    <mu-icon value="view_quilt"></mu-icon>
                </mu-button>
            </mu-appbar>
            <mu-list textline="two-line">
                <mu-list-item v-for="approvalRecord in approvalRecordList" avatar button :ripple="false">
                    <mu-list-item-action>
                        <mu-avatar>
                            <img :src="approvalRecord.avatar">
                        </mu-avatar>
                    </mu-list-item-action>
                    <mu-list-item-content>
                        <mu-list-item-title>提交人: {{approvalRecord.salesName}}</mu-list-item-title>
                        <mu-list-item-sub-title>{{approvalRecord.CreateTime}}</mu-list-item-sub-title>
                    </mu-list-item-content>
                    <mu-list-item-action>
                        <mu-button icon @click="queryDetail(approvalRecord.DocCode)">
                            <mu-icon value="info"></mu-icon>
                        </mu-button>
                    </mu-list-item-action>
                </mu-list-item>
            </mu-list>
        </mu-paper>
    </div>
</body>
<script src="/Scripts/jquery.min.js"></script>
<script src="/Scripts/vue.js"></script>
<script src="https://unpkg.com/muse-ui/dist/muse-ui.js"></script>
<script src="https://unpkg.com/element-ui/lib/index.js"></script>
<script src="https://unpkg.com/muse-ui-message/dist/muse-ui-message.js"></script>
<script>
    var vue = new Vue({
        el: '#total',
        data: {
            approvalRecordList:[],
        },
        methods: {
            queryDetail: function (docCode) {
                location.href = "mExpectFlowRecordDetail.aspx?docCode=" + docCode;
            },
            back: function() {
                history.go(-1);
            }
        },
        mounted: function() {
            getApprovalRecord();
        }
    })

    function getApprovalRecord() {
        $.ajax({
            url: 'mExpectFlowRecordList.aspx',
            data: {
                action:'getExpectFlowRecord'
            },
            dataType: 'json',
            type: 'post',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg,'提示').then(() => {
                        location.href = "mExpectFlowApprovalIndex.aspx";
                    });
                } else {
                    vue.approvalRecordList = JSON.parse(msg.approvalRecord);
                }
            },
            error: function(msg) {

            }
        })
    }
</script>
</html>
