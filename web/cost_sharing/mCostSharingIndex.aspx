<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mCostSharingIndex.aspx.cs" Inherits="web_cost_sharing_mCostSharingIndex" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>网点备案管理系统</title>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-message/dist/muse-ui-message.all.css"/>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-loading/dist/muse-ui-loading.all.css"/>
</head>
<body>
    <div id="total">
        <mu-container>
            <mu-bottom-nav>
                <mu-bottom-nav-item ripple title="网点列表" icon="restore" href="mCostSharingSubmit.aspx"></mu-bottom-nav-item>
                <mu-bottom-nav-item ripple title="待我审批" icon="favorite" href="#"></mu-bottom-nav-item>
                <mu-bottom-nav-item ripple title="提交记录" icon="location_on" href="#"></mu-bottom-nav-item>
            </mu-bottom-nav>
        </mu-container>
    </div>
</body>
</html>
<script src="/Scripts/jquery.min.js"></script>
<script src="/Scripts/vue.js"></script>
<script src="https://unpkg.com/muse-ui@3.0.0/dist/muse-ui.js"></script>
<script src="https://unpkg.com/muse-ui-message/dist/muse-ui-message.js"></script>
<script src="https://unpkg.com/muse-ui-loading/dist/muse-ui-loading.js"></script>
<script>
    var vue = new Vue({
        el: '#total',
    })
</script>
