<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mExpectFlowSubmit.aspx.cs" Inherits="web_expect_flow_submit_mExpectFlowSubmit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>预计流向填报页面</title>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-message/dist/muse-ui-message.all.css"/>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-loading/dist/muse-ui-loading.all.css"/>
    <style>
        .mu-demo-form {
            width: 100%;
            max-width: 460px;
        }
        .demo-loading-wrap {
            height: 300px;
            position: relative;
        }
        .demo-loading-wrap.mu-button {
            margin: 6px 8px;
        }
    </style>
</head>
<body>
    <div id="total">
        <mu-container v-loading="loading">
            <mu-appbar style="width: 100%;" color="lightBlue">
                <mu-button icon slot="left" @click="back">
                    <mu-icon value="arrow_back" ></mu-icon>
                </mu-button>
                预计流向上报
                <mu-button flat slot="right" @click="submitExpectFlow">提交</mu-button>
            </mu-appbar>
            <mu-row>
                <mu-col span="12" lg="4" sm="6">
                    <mu-select label="医院" v-model="HospitalId" full-width @change="relativeProduct">
                        <mu-option v-for="onebranch in relatedHospital" :key="onebranch.hospitalId" :label="onebranch.hospitalName" :value="onebranch.hospitalId"></mu-option>
                    </mu-select>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-select label="产品" v-model="ProductId" full-width>
                        <mu-option v-for="onebranch in relatedProduct" :key="onebranch.productId" :label="onebranch.productName" :value="onebranch.productId"></mu-option>
                    </mu-select>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field label="预计流向" v-model="ExpectFlow" type="number" placeholder="请填写预计流向" full-width></mu-text-field>
                </mu-col>
                <mu-col span="12" lg="4" sm="6">
                    <mu-text-field multi-line :rows="3" :rows-max="6" label="备注" v-model="Remark" placeholder="请填写备注信息" full-width></mu-text-field>
                </mu-col>
            </mu-row>
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
        data: {
            relatedHospital: [],
            relatedProduct: [],
            HospitalId: '',
            ProductId: '',
            ExpectFlow: 0,
            Remark: '',
            loading: false,
        },
        methods: {
            submitExpectFlow: function() {
                submitExpectFlow(this.HospitalId, this.ProductId, this.ExpectFlow, this.Remark);
            },
            back: function() {
                history.go(-1);
            },
            relativeProduct: function(hospitalId) {
                getRelativeProduct(hospitalId);
            }
        },
        mounted: function () {
            getRelatedBranch();
        }
    });

    function getRelatedBranch() {
        $.ajax({
            url: 'mExpectFlowSubmit.aspx',
            data: { action: 'queryRelatedBranch'},
            type: 'post',
            dataType: 'json',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg,'提示').then(() => {
                        location.href = "mExpectFlowApprovalIndex.aspx";
                    });
                } else {
                    vue.relatedHospital = JSON.parse(msg.RelatedHospital);
//                    vue.relatedProduct = JSON.parse(msg.RelatedProduct);
                }
            },
            error: function() {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        });
    }

    function getRelativeProduct(hospitalId) {
        $.ajax({
            url: 'mExpectFlowSubmit.aspx',
            data: { action: 'getRelativeProduct', hospitalId: hospitalId},
            type: 'post',
            dataType: 'json',
            success: function (msg) {
                vue.ProductId = "";
                if (msg.ErrCode != 0) {
                    vue.$alert('暂无对应的产品','提示');
                } else {
                    vue.relatedProduct = JSON.parse(msg.RelatedProduct);
                }
            },
            error: function() {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        });
    }

    function submitExpectFlow(hospitalId, productId, expectFlow, remark) {
        if (hospitalId == "" || productId == "" || expectFlow == "") {
            vue.$alert("医院，产品，预计流向不能为空", '错误信息');
        } else {
            vue.loading = true;
            $.ajax({
                url: 'mExpectFlowSubmit.aspx',
                data: {
                    action: 'submitExpectFlow',
                    HospitalId: hospitalId,
                    ProductId: productId,
                    ExpectFlow: expectFlow,
                    Remark: remark,
                },
                type: 'post',
                dataType: 'json',
                success: function (msg) {
                    vue.loading = false;
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
    }
</script>