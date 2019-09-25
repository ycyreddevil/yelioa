<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mCostSharingDelete.aspx.cs" Inherits="web_cost_sharing_delete_cost_sharing_mCostSharingDelete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>网点备案审批记录列表</title>
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
        

.demo-date-picker {
  margin: 8px;
}
</style>
</head>
<body>
    <div id="total">
        <mu-container>
            <mu-appbar fill style="width: 100%;" color="lightBlue">
                <mu-button icon slot="left" @click="back">
                    <mu-icon value="arrow_back"></mu-icon>
                </mu-button>
                网点备案审批记录列表 
                <mu-button  ripple	flat   slot="right"  @click="Submit">提交</mu-button>
            </mu-appbar>

            <mu-row >
                <mu-col v-for="field in firstList" v-if="field.type=='number'" span="12" lg="4" sm="6">
                    <mu-text-field type="number" :label="field.label" v-model="field.value"  full-width label-float></mu-text-field>
                </mu-col>
                <mu-col v-for="field in firstList" v-if="field.type=='textarea'" span="12" lg="4" sm="6">
                    <mu-text-field v-model="field.value" :label="field.label" multi-line :rows="3" :rows-max="6"></mu-text-field><br/>
                </mu-col>
            </mu-row >

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
                     <span style="color: rgba(0, 0, 0, .87)">{{approver.department}}</span>   
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
    
    var thisMonth;
    var vue = new Vue({
        el: '#total',
        data: {
            firstList: [],
            approverList: [],
            onlyRead: false,
            docId: '',
            docCode:'',
        },
        methods: {
             back: function () {
                history.go(-1);
            },
            Submit: function () {
                Submit(this.firstList);
            },
        },
        mounted: function () {
            this.docId = GetQueryString("docId");
            this.docCode = GetQueryString("docCode");
            var IsRecord = GetQueryString("IsRecord");
            GetList(this.docId, this.docCode, IsRecord);  // 获取单据列表     
            
        },
        
    });
    function GetList(docId, docCode, IsRecord) {
        $.ajax({
            url: 'mCostSharingDelete.aspx',
            data: {
                action: 'GetList',
                docId: docId,
                docCode: docCode,
                IsRecord: IsRecord,
            },
            type: 'post',
            dataType: 'json',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.firstList = msg.data;
                    vue.approverList = msg.approver;
                }
            },
            error: function() {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        })
    }
        function Submit(firstList) {
        $.ajax({
            url: 'mCostSharingDelete.aspx',
            data: {
                action: 'submitData',
                firstList: JSON.stringify(firstList),
                docCode: vue.docCode,
                docId: vue.docId,
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
     function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
</script>
</html>

