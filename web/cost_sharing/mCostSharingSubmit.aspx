<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mCostSharingSubmit.aspx.cs" Inherits="web_cost_sharing_mCostSharingSubmit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>网点列表</title>
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
                    <mu-icon value="menu" ></mu-icon>
                </mu-button>
                网点列表
                <mu-button flat slot="right" @click="insertCostSharing">新增</mu-button>
            </mu-appbar>
            <mu-list toggle-nested >
                <mu-list-item v-for="ocs in relativeCostSharing" button :ripple="false" nested :open="openDropDown === 'ocs.代表'" @toggle-nested="openDropDown = arguments[0] ? '{{ocs.代表}}' : ''">
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
                            <mu-flex justify-content="center">
                                <mu-button icon @click="openBotttomSheet(oocs.costSharingId)">
                                    <mu-icon value="more_vert"></mu-icon>
                                </mu-button>
                            </mu-flex>
                            <mu-bottom-sheet :open.sync="openBottomSheet">
                                <mu-list @item-click="closeBottomSheet">
                                    <mu-list-item button>
                                        <mu-list-item-action>
                                            <mu-icon value="delete_forever" color="red"></mu-icon>
                                        </mu-list-item-action>
                                        <mu-list-item-title>丢失</mu-list-item-title>
                                    </mu-list-item>
                                    <mu-list-item button :href="'update_basic_cost_sharing/mUpdateBasicCostSharing.aspx?newCostSharingId='+costSharingId">
                                        <mu-list-item-action>
                                            <mu-icon value="build" color="orange"></mu-icon>
                                        </mu-list-item-action>
                                        <mu-list-item-title>更新基本信息</mu-list-item-title>
                                    </mu-list-item>
                                    <mu-list-item button>
                                        <mu-list-item-action>
                                            <mu-icon value="build" color="blue"></mu-icon>
                                        </mu-list-item-action>
                                        <mu-list-item-title>更新费用比例</mu-list-item-title>
                                    </mu-list-item>
                                </mu-list>
                            </mu-bottom-sheet>
                        </mu-list-item-action>
                    </mu-list-item>
                    
                </mu-list-item>
            </mu-list>
            <mu-popover cover :open.sync="open" :trigger="trigger">
                <mu-list>
                    <mu-list-item button href="mCostSharingSubmit.aspx">
                        <mu-list-item-title>网点列表</mu-list-item-title>
                    </mu-list-item>
                    <mu-list-item button href="#">
                        <mu-list-item-title>待我审批</mu-list-item-title>
                    </mu-list-item>
                    <mu-list-item button href="#">
                        <mu-list-item-title>提交记录</mu-list-item-title>
                    </mu-list-item>
                </mu-list>
            </mu-popover>
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
            relativeCostSharing: [],
            openDropDown: '',
            openBottomSheet: false,
            loading: true,
            open: false,
            trigger: null,
            costSharingId:0,
        },
        methods: {
            showMenu: function() {
                this.open = !this.open;
            },
            closeBottomSheet () {
                this.openBottomSheet = false;
            },
            openBotttomSheet (costSharingId) {
                this.openBottomSheet = true;
                this.costSharingId = costSharingId;
            },
            insertCostSharing() {
                // todo 跳转新增网点
            }
        },
        mounted: function () {
            this.trigger = this.$refs.button.$el;
            getRelativeCostSharing();
        },
    });

    Vue.filter('addBr', function (info) {
        return info.replace('|', '<br/>');
    });

    function getRelativeCostSharing() {
        $.ajax({
            url: 'mCostSharingSubmit.aspx',
            data: {
                action: 'queryRelativeCostSharing',
            },
            dataType: 'json',
            type: 'post',
            success: function(res) {
                if (res.ErrCode != 0) {
                    vue.$alert(res.ErrMsg, '提示').then(() => {

                    });
                } else {
                    vue.relativeCostSharing = res.RelativeCostSharing;
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
</script>
