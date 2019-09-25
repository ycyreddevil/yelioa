<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewBranchRegistration.aspx.cs" Inherits="NewBranchRegistration" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
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
        .demo-list-wrap {
            width: 100%;
            max-width: 360px;
            overflow: hidden;
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
                网点新增
                <mu-button flat slot="right" v-if="onlyRead"  @click="submitExpectFlow">提交</mu-button>
                <mu-button disabled  flat slot="right" v-if="!onlyRead"  @click="submitExpectFlow">提交</mu-button>
            </mu-appbar>

            <mu-row v-if="fieldList.length>0">
                <mu-col v-for="field in fieldList" v-if="field.type=='number'" span="12" lg="4" sm="6">
                    <mu-text-field type="number" :label="field.label" v-model="field.value"  full-width label-float></mu-text-field>
                </mu-col>
                <mu-col v-for="field in fieldList"  v-if="field.type=='text'" span="12" lg="4" sm="6">
                    <mu-text-field :label="field.label" v-model="field.value" :disabled="onlyRead" full-width label-float></mu-text-field>
                </mu-col>
                <mu-col  v-for="field in fieldList" v-if="field.type=='select'" span="12" lg="4" sm="6">
                   <mu-select :label="field.label" filterable label-float v-model="field.value.Id" full-width @change="(value) => modify(value,field.value,field.optionList)">
                       <mu-option v-for="option in field.optionList"  :label="option.Name" :value="option.Id"></mu-option>
                   </mu-select>
                </mu-col>
                  <mu-col v-for="field in fieldList"  v-if="field.type=='date'" span="12" lg="4" sm="6">
                      <mu-date-input   v-model="field.value" :label="field.label"  type="date" container="bottomSheet" label-float full-width></mu-date-input>
                </mu-col>
            </mu-row>

            
            <mu-list v-if="approverList.length>0" textline="three-line">
                <mu-sub-header>提交审批记录</mu-sub-header>
                 <mu-divider></mu-divider>
                  <mu-list-item v-for="approver in approverList" avatar :ripple="false" button>
                  <mu-list-item-action>
                  <mu-avatar>
                   <img :src="approver.avatar">
                   </mu-avatar>
                 </mu-list-item-action>
                <mu-list-item-content>
              <mu-list-item-title>{{approver.userName}}</mu-list-item-title>
                <mu-list-item-sub-title>
                     <span style="color: rgba(0, 0, 0, .87)">{{approver.departmentName|myfilter}}</span>   
                  </mu-list-item-sub-title>
                </mu-list-item-content>
             </mu-list-item>
                </mu-list>
        </mu-container>
    </div>
</body>
<script src="/Scripts/jquery.min.js"></script>
<script src="/Scripts/vue.js"></script>
<script src="https://unpkg.com/muse-ui/dist/muse-ui.js"></script>
<script src="https://unpkg.com/muse-ui-message/dist/muse-ui-message.js"></script>
<script>
    var vue = new Vue({
        el: '#total',
        data: {
            approverList: [],
            fieldList: [],
            onlyRead: '',
            docId: '-1'
        },
        methods: {
            submitExpectFlow: function () {
                submitExpectFlow(this.fieldList);
            },
            ApproverChange(level) {
                this.vactiveStep = index;
            },
            back: function () {
                history.go(-1);
            },
            modify: function (value1, value2, value3) {
                for(i = 0; i < value3.length; i++)
                {
                    if (value1 == value3[i].Id) {
                        value2.Name = value3[i].Name;
                        break;
                    }
                }
            }
        },
        mounted: function () {
            var docCode = GetQueryString('docCode');
            var IsRecord = GetQueryString('IsRecord');
            this.docCode = docCode;
            getApprovalDataDetail(docCode,IsRecord);  // 获取单据详情
        },
        filters: {
            myfilter: function (value) {
                var res = "";
                var strList = value.split(",");
                for (var i = 0; i < strList.length; i++) {
                    res += strList[i].substr(strList[i].lastIndexOf("/")+1,strList[i].length-strList[i].lastIndexOf("/")-1)+","
                }
                return res.substr(0, res.length - 1);
            }            
        }
    });


    function submitExpectFlow(fieldList) {
        $.ajax({
            url: 'NewBranchRegistration.aspx',
            data: {
                action: 'submitData',
                fieldList: JSON.stringify(fieldList),
                docCode: vue.docCode,
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

    function getApprovalDataDetail(docCode,IsRecord) {
        $.ajax({
            url:'NewBranchRegistration.aspx',
            data: {
                action: 'getApprovalDataDetail',
                docCode: docCode,
                IsRecord:IsRecord,
            },
            dataType: 'json',
            type: 'post',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    
                    vue.fieldList = msg.fieldList;
                    vue.onlyRead = msg.onlyRead;
                    vue.approverList =msg.approverList;
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

