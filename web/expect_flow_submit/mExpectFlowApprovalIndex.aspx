<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mExpectFlowApprovalIndex.aspx.cs" Inherits="web_expect_flow_submit_mExpectFlowApprovalIndex" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>预计流向首页</title>
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
                预计流向首页
            </mu-appbar>
            <mu-list>
                <mu-list-item button :ripple="false" href="mExpectFlowSubmit.aspx">
                    <mu-list-item-action>
                        <mu-icon value="send"></mu-icon>
                    </mu-list-item-action>
                    <mu-list-item-title>提交</mu-list-item-title>
                </mu-list-item>
                <mu-list-item button :ripple="false" href="mExpectFlowApprovalList.aspx">
                    <mu-list-item-action>
                        <mu-icon value="inbox"></mu-icon>
                    </mu-list-item-action>
                    <mu-list-item-title>待我审批</mu-list-item-title>
                </mu-list-item>
                <mu-list-item button :ripple="false" href="mExpectFlowRecordList.aspx">
                    <mu-list-item-action>
                        <mu-icon value="drafts"></mu-icon>
                    </mu-list-item-action>
                    <mu-list-item-title>审批记录</mu-list-item-title>
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
        methods: {
            back: function() {
                history.go(-1);
            }
        },
    })
</script>
</html>
