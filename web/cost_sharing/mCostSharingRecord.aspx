<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mCostSharingRecord.aspx.cs" Inherits="web_cost_sharing_mCostSharingRecord" %>

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
                <mu-button  ripple	flat   slot="right"  @click="showMonth">{{month1}}</mu-button>
            </mu-appbar>
            <div v-for="firstObject in firstList">
            <mu-list v-if="firstObject.List.length>0" textline="three-line">
                <mu-sub-header>{{firstObject.header}}</mu-sub-header>
                  <mu-list-item v-for="secondObject in firstObject.List" avatar :ripple="false" button  >
                  <mu-list-item-action>
                  <mu-avatar>
                   <img :src="secondObject.avatar">
                   </mu-avatar>
                 </mu-list-item-action>
                <mu-list-item-content>
              <mu-list-item-title>{{secondObject.userName}}--{{secondObject.department}}</mu-list-item-title>
                <mu-list-item-sub-title>
                     <span style="color: rgba(0, 0, 0, .87)">{{secondObject.firstValue}}</span>  <br/> 
                    {{secondObject.secondValue}}
                  </mu-list-item-sub-title>
                </mu-list-item-content>       
             </mu-list-item>
                <mu-divider></mu-divider>
                </mu-list>
                </div>
            <mu-bottom-sheet :open.sync="show" justify-content="center">
                <mu-flex wrap="wrap" justify-content="center" style="width:100%" >
      <mu-paper :z-depth="1" class="demo-date-picker" style="width:100%"   justify-content="center" >
      <mu-date-picker justify-content="center" style="width:100%"   type="month" :date.sync="month" @change="SelectMonth"></mu-date-picker>
    </mu-paper>
                </mu-flex>
  </mu-bottom-sheet>
    
  
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
            show: false,
            month: new Date(),
            month1:'',
        },
        methods: {
             back: function () {
                history.go(-1);
            },
            SelectMonth: function (date) {
                this.month = date;
                this.show = false;
                this.month1 = dateToMonth(date);
                 submitExpectFlow(date);
            },
            showMonth: function () {
                this.show = true;
            }
        },
        mounted: function () {
           
            submitExpectFlow(new Date());  // 获取单据列表
            this.month1 = dateToMonth(new Date());
        },
        
    });
    function dateToMonth(date) {
        var month = date.getMonth() + 1;
        if (month< 10) {
            month = "0" + month;
        }
        return date.getFullYear() + '-' + month;
    }

    function submitExpectFlow(date) {
        var year = date.getFullYear();
        var month = date.getMonth()+1;
        $.ajax({
            url: 'mCostSharingRecord.aspx',
            data: {
                action: 'GetList',
                year: year,
                month:month,
            },
            type: 'post',
            dataType: 'json',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.firstList = msg.data;
                }
            },
            error: function() {
                vue.$alert("网络失败，请及时联系管理员!", '错误信息');
            }
        })
    }
</script>
</html>

