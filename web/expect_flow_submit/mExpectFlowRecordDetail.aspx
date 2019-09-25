<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mExpectFlowRecordDetail.aspx.cs" Inherits="web_expect_flow_submit_mExpectFlowRecordDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>预计流向操作记录详情页</title>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-message/dist/muse-ui-message.all.css"/>
</head>
<body>
    <div id="total">
        <mu-container>
            <mu-appbar style="width: 100%;" color="lightBlue">
                <mu-button icon slot="left" @click="back">
                    <mu-icon value="arrow_back"></mu-icon>
                </mu-button>
                预计流向审批记录
                <%--<mu-button flat slot="right" @click="submitExpectFlow">提交</mu-button>--%>
            </mu-appbar>
            <mu-row>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field label="医院" v-model="HospitalName" disabled full-width></mu-text-field>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field label="产品" v-model="ProductName" disabled full-width></mu-text-field>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field label="预计流向初始值" v-model="OriginFlow" type="number" disabled full-width></mu-text-field>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field label="预计流向修改值" v-model="ExpectFlow" type="number" disabled full-width></mu-text-field>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field multi-line :rows="3" :rows-max="6" label="备注" v-model="Remark" disabled full-width></mu-text-field>
                </mu-col>
            </mu-row>
        </mu-container>
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
            HospitalName: '',
            ProductName: '',
            OriginFlow: '',
            ExpectFlow: '',
            Remark: '',
        },
        methods: {
            back: function() {
                history.go(-1);
            }
        },
        mounted: function() {
            var docCode = GetQueryString('docCode');
            getRecordDetail(docCode);
        }
    });

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    function getRecordDetail(docCode) {
        $.ajax({
            url:'mExpectFlowRecordDetail.aspx',
            data: {
                action: 'getRecordDataDetail',
                docCode: docCode,
            },
            dataType: 'json',
            type: 'post',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    var data = JSON.parse(msg.approvalRecordDetail)[0];
                    vue.HospitalName = data.hospitalName;
                    vue.ProductName = data.productName;
                    vue.OriginFlow = data.OriginValue;
                    vue.ExpectFlow = data.ModifiedValue;
                    vue.Remark = data.Remark;
                }
            },
            error: function() {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        })
    }
</script>
</html>
