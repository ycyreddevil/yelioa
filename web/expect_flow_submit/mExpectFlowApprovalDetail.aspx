<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mExpectFlowApprovalDetail.aspx.cs" Inherits="web_expect_flow_submit_mExpectFlowApprovalDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>预计流向审批详情页面</title>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-message/dist/muse-ui-message.all.css"/>
    <style>
        .mu-demo-form {
            width: 100%;
        }
        .demo-step-content {
            margin: 0  16px;
        }
        .demo-step-button {
            margin-top: 12px;
            margin-right: 12px;
        }
    </style>
</head>
<body>
    <div id="total">
        <mu-container>
            <mu-appbar style="width: 100%;" color="lightBlue">
                <mu-button icon slot="left" @click="back">
                    <mu-icon value="arrow_back"></mu-icon>
                </mu-button>
                预计流向审批
                <mu-button flat slot="right" @click="submitExpectFlow">提交</mu-button>
            </mu-appbar>
            <mu-row>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field label="医院" v-model="HospitalName" disabled full-width></mu-text-field>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field label="产品" v-model="ProductName" disabled full-width></mu-text-field>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field label="预计流向" v-model="ExpectFlow" type="number" placeholder="请填写预计流向" full-width></mu-text-field>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field multi-line :rows="3" :rows-max="6" label="备注" v-model="Remark" placeholder="请填写备注信息" full-width></mu-text-field>
                </mu-col>
            </mu-row>
            <div class="demo-vsteper-container">
                <mu-stepper :active-step="vactiveStep" orientation="vertical" :linear="false">
                    <mu-step v-for="(approvalRecord,index) in approvalRecordList">
                        <mu-step-button @click="changeStep(index)">
                            {{approvalRecord.userName}}
                        </mu-step-button>
                        <mu-step-content>
                            <p>
                                <b>初始值</b>:{{approvalRecord.OriginValue}}<br/>
                                <b>修改值</b>:{{approvalRecord.ModifiedValue}}<br/> 
                                <b>修改时间</b>:{{approvalRecord.CreateTime}}<br/>
                            </p>
                        </mu-step-content>
                    </mu-step>
                </mu-stepper>
            </div>
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
            HospitalId: '',
            HospitalName:'',
            ProductId: '',
            ProductName: '',
            OriginExpectFlow: 0,
            SupervisorId:'',
            ExpectFlow: 0,
            Remark: '',
            DepartmentId: '',
            approvalRecordList: [],
            vactiveStep: 0,
        },
        methods: {
            submitExpectFlow: function() {
                submitExpectFlow(this.HospitalId, this.ProductId, this.ExpectFlow, this.Remark, this.OriginExpectFlow, this.DepartmentId, this.SupervisorId);
            },
            changeStep (index) {
                this.vactiveStep = index;
            },
            back: function() {
                history.go(-1);
            }
        },
        mounted: function () {
            var docCode = GetQueryString('docCode');
            getApprovalDataDetail(docCode);  // 获取单据详情
        },
    });

    function submitExpectFlow(hospitalId,productId,expectFlow,remark,originExpectFlow,departmentId,SupervisorId) {
        $.ajax({
            url: 'mExpectFlowApprovalDetail.aspx',
            data: {
                action: 'submitExpectFlow',
                HospitalId: hospitalId,
                ProductId: productId,
                ExpectFlow: expectFlow,
                Remark: remark,
                OriginExpectFlow: originExpectFlow,
                DepartmentId: departmentId,
                SupervisorId: SupervisorId,
            },
            type: 'post',
            dataType: 'json',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.$alert("操作成功",'提示').then(() => {
                        location.href = "mExpectFlowApprovalIndex.aspx";
                    });
                }
            },
            error: function() {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        })
    }

    function getApprovalDataDetail(docCode) {
        $.ajax({
            url:'mExpectFlowApprovalDetail.aspx',
            data: {
                action: 'getApprovalDataDetail',
                docCode: docCode,
            },
            dataType: 'json',
            type: 'post',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    var data = JSON.parse(msg.approvalDetailData)[0];
                    vue.HospitalId = data.HospitalId;
                    vue.HospitalName = data.hospitalName;
                    vue.ProductId = data.ProductId;
                    vue.ProductName = data.productName;
                    vue.OriginExpectFlow = data.ExpectFlow;
                    vue.ExpectFlow = data.ExpectFlow;
                    vue.DepartmentId = data.ApproverDepartmentId;
                    vue.SupervisorId = data.SupervisorId;

                    vue.approvalRecordList = JSON.parse(msg.approvalDetailDataRecord);
                }
            },
            error: function() {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        })
    }

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
</script>
</html>
