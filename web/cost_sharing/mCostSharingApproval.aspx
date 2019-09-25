<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mCostSharingApproval.aspx.cs" Inherits="web_cost_sharing_mCostSharingApproval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>待我审批</title>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-message/dist/muse-ui-message.all.css"/>
    <link rel="stylesheet" href="https://unpkg.com/muse-ui-loading/dist/muse-ui-loading.all.css"/>
</head>
<body>
    <div id="total">
        <mu-container v-loading="loading">
            <mu-appbar style="width: 100%;" color="lightBlue">
                <mu-button ref="button" icon slot="left" @click="showMenu">
                    <mu-icon value="menu"></mu-icon>
                </mu-button>
                待我审批
                <%-- <mu-button flat slot="right" @click="insertCostSharing">新增</mu-button> --%>
            </mu-appbar>
            <mu-list toggle-nested >
                <mu-sub-header>新增</mu-sub-header>
                <mu-list-item v-for="ocs in addList" button :ripple="false" nested :open="openDropDown1 === 'ocs.代表'" @toggle-nested="openDropDown1 = arguments[0] ? '{{ocs.代表}}' : ''">
                    <mu-list-item-action>
                        <mu-avatar>
                            <img :src="ocs.avatar">
                        </mu-avatar>
                    </mu-list-item-action>
                    <mu-list-item-title>{{ocs.代表}}</mu-list-item-title>
                    <mu-list-item-action>
                        <mu-icon class="toggle-icon" size="24" value="keyboard_arrow_down"></mu-icon>
                    </mu-list-item-action>
                    <mu-list-item v-for="oocs in ocs.网点信息" button :ripple="false" slot="nested" >
                        <mu-list-item-sub-title>
                            <span v-html="oocs.info.replace('|', '<br/>')"></span>
                        </mu-list-item-sub-title>
                        <mu-list-item-action>
                            <mu-button color="primary" @click="approval('add', oocd.id)">审批</mu-button>
                        </mu-list-item-action>
                    </mu-list-item>
                    
                </mu-list-item>
            </mu-list>
            <mu-divider></mu-divider>
            <mu-list toggle-nested>
                <mu-sub-header>更新1</mu-sub-header>
                <mu-list-item v-for="ocs in update1List" button :ripple="false" nested :open="openDropDown2 === 'ocs.代表'" @toggle-nested="openDropDown2 = arguments[0] ? '{{ocs.代表}}' : ''">
                    <mu-list-item-action>
                        <mu-avatar>
                            <img :src="ocs.avatar">
                        </mu-avatar>
                    </mu-list-item-action>
                    <mu-list-item-title>{{ocs.代表}}</mu-list-item-title>
                    <mu-list-item-action>
                        <mu-icon class="toggle-icon" size="24" value="keyboard_arrow_down"></mu-icon>
                    </mu-list-item-action>
                    <mu-list-item v-for="oocs in ocs.网点信息" button :ripple="false" slot="nested" >
                        <mu-list-item-sub-title>
                            <span v-html="oocs.info.replace('|', '<br/>')"></span>
                        </mu-list-item-sub-title>
                        <mu-list-item-action>
                            <mu-button color="primary" @click="approval('update',oocs.id)">审批</mu-button>
                        </mu-list-item-action>
                    </mu-list-item>
                    
                </mu-list-item>
            </mu-list>
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
            loading: false,
            openDropDown1: '',
            openDropDown2: '',
            addList: [],
            update1List: [],
        },
        methods: {
            showMenu: function() {

            },
            approval: function(type, code) {
                if (type == 'add') {

                } else if (type == 'update') {
                    location.href = 'update_basic_cost_sharing/mUpdateBasicCostSharing.aspx?action=getFormColumnsAndData' +
                        '&&type=1&&costSharingRecordId='+code;
                }
            }
        },
        mounted: function() {
            queryRelativeCostSharing();
        }
    })

    function queryRelativeCostSharing() {
        $.ajax({
            url: 'mCostSharingApproval.aspx',
            data: {
                action:'queryRelativeCostSharing'
            },
            dataType: 'json',
            type: 'post',
            success: function(res) {
                if (res.ErrCode != 0) {
                    vue.$alert(res.ErrMsg, '提示').then(() => {

                    });
                } else {
                    vue.addList = res.add;
                    vue.update1List = res.update1;
                    vue.update2List = res.update2;
                    vue.deleteList = res.delete;
                }
            },
            error: function(res) {
                vue.$alert('网络错误，请及时联系管理员', '提示').then(() => {
                    vue.loading = false;
                });
            }
        })
    }
</script>
            