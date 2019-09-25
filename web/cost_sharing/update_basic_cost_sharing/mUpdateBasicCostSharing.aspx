<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mUpdateBasicCostSharing.aspx.cs" Inherits="web_cost_sharing_update_basic_cost_sharing_mUpdateBasicCostSharing" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>网点备案表更新</title>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-message/dist/muse-ui-message.all.css"/>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-loading/dist/muse-ui-loading.all.css"/>
</head>
<body>
   <div id="total">
       <mu-container >
           <mu-appbar style="width: 100%;" color="lightBlue">
               <mu-button icon slot="left" @click="back">
                   <mu-icon value="arrow_back" ></mu-icon>
               </mu-button>
               网点基本信息更新
               <mu-button flat slot="right" @click="submissionOfCostSharingUpdating">提交</mu-button>
           </mu-appbar>
           <mu-row v-loading="loading">
               <mu-col span="12" lg="4" sm="6" v-for="oneColumn in column">
                   <mu-select filterable label-float v-if="oneColumn.RelativeTable != ''" v-model="bindData(detail, oneColumn)" :label="oneColumn.FieldName" full-width @change="(value) => modifyColumnRelativeData(value,oneColumn,jsoncolumnRelativeData(oneColumn.columnRelativeData))">
                       <mu-option v-for="oneRelatetiveData in jsoncolumnRelativeData(oneColumn.columnRelativeData)" :id="oneRelatetiveData.id" :label="oneRelatetiveData.name" :value="oneRelatetiveData.id"></mu-option>
                   </mu-select>
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
            loading: true,
            column: [],
            detail: [],
        },
        methods: {
            modifyColumnRelativeData: function(value, oneColumn,data) {
                modifyColumnRelativeData(value, oneColumn,data);
            },
            back: function() {
                location.href = "../mCostSharingSubmit.aspx";
            },
            submissionOfCostSharingUpdating: function() {
                //todo 提交方法
                submissionOfCostSharingUpdating();
            }
        },
        mounted: function () {
            getFormColumnsAndData();
        },
        computed: {
            bindData() {
                return function(detail, col) {
                    return detail[col['FieldName']+"Id"];
                }
            },
            jsoncolumnRelativeData() {
                return function(str) {
                    return JSON.parse(str);
                }
            }
        }
    });

    function submissionOfCostSharingUpdating() {
        $.ajax({
            url: 'mUpdateBasicCostSharing.aspx',
            data: {
                action: 'submissionOfCostSharingUpdating',
                newCostSharingId: GetQueryString('newCostSharingId'),
                detail: JSON.stringify(vue.detail),
                costSharingRecordId: GetQueryString('costSharingRecordId'),
            },
            dataType: 'json',
            type: 'post',
            success: function(res) {
                if (res.ErrCode != 0) {
                    vue.$alert(res.ErrMsg, '提示').then(() => {

                    });
                } else {
                    vue.$alert('您的修改方案已提交，请耐心等待审核', '提示').then(() => {
                        //todo 页面跳转回列表页
                        location.href = "../mCostSharingSubmit.aspx";
                    });
                }
                vue.loading = false;
            },
            error: function(res) {
                vue.$alert('网络错误，请及时联系管理员', '提示').then(() => {
                    isLoading = false;
                });
            }
        });
    }

    function modifyColumnRelativeData(value, oneColumn, data) {
        vue.detail[oneColumn['FieldName'] + "Id"] = value;
        for (i = 0; i < data.length; i++) {
            if (value == data[i].id) {
                vue.detail[oneColumn['FieldName']] = data[i].name;
            }
        }
    }

    function getFormColumnsAndData() {
        $.ajax({
            url: 'mUpdateBasicCostSharing.aspx',
            data: {
                action: 'getFormColumnsAndData',
                newCostSharingId: GetQueryString('newCostSharingId'),
                costSharingRecordId: GetQueryString('costSharingRecordId'),
            },
            dataType: 'json',
            type: 'post',
            success: function(res) {
                if (res.ErrCode != 0) {
                    vue.$alert(res.ErrMsg, '提示').then(() => {
                        location.href = "../mCostSharingSubmit.aspx";
                    });
                } else {
                    var column = res.column;
                    var detail = res.detail;

                    vue.column = JSON.parse(column);
                    vue.detail = JSON.parse(detail);
                }
                vue.loading = false;
            },
            error: function(res) {
                vue.$alert('网络错误，请及时联系管理员', '提示').then(() => {
                    vue.loading = false;
                });
            }
        });
    }

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
</script>
