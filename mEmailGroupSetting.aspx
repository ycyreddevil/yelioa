<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mEmailGroupSetting.aspx.cs" Inherits="mEmailGroupSetting" %>

    <!DOCTYPE html>

    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title></title>
        <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
        <meta name="description" content="" />
        <link rel="stylesheet" href="https://unpkg.com/mint-ui/lib/style.css">
        <style type="text/css">
            userCell {
                width: 100%
            }
        </style>
    </head>

    <body>

        <div id="vue">

            <div>
                <mt-header fixed title="自定义分组">
                    <mt-button icon="back" @click="backToEmailIndex()" slot="left">返回</mt-button>
                    <mt-button @click="createGroup()" slot="right">新建</mt-button>
                </mt-header>
                <mt-field></mt-field>
                <mt-index-list :show-indicator="false">
                    <mt-cell v-for="group in groupList" :title="group.GroupName">
                        <mt-button @click="modifyGroup(group)">更改</mt-button>&nbsp;
                        <mt-button @click="deleteGroup(group)">删除</mt-button>
                    </mt-cell>
                </mt-index-list>
            </div>

            <div>
                <mt-popup v-model="popupVisible" position="right" style="width: 100%;height: 100%;">
                    <mt-header :title="popupTitle">
                        <mt-button icon="back" @click="backToIndex()" slot="left">返回</mt-button>
                        <mt-button @click="submit()" slot="right">确定</mt-button>
                    </mt-header>
                    <!-- <mt-field></mt-field> -->
                    <mt-field label="分组名" placeholder="请输入分组名称" v-model="groupName"></mt-field>
                    <div id="tbx_members" >
                        <mt-field label="成员" v-model="memberString" :readonly="true" type="textarea" rows="4"></mt-field>
                    </div>
                    
                    <mt-index-list :height="userListHeight" @onload.native="autoUserListHeight()" >
                        <mt-index-section v-for="(userGroup,indexGroup) in userList" :index="userGroup.Index">
                            <mt-cell name="userCell" v-for="(user,indexUser) in userGroup.Users" :title="user.UserName" v-on:click.native="clickUser(indexGroup,indexUser)">
                                <input type="checkbox" style="margin-left: -60px" v-model="user.Checked"></input>
                                <img slot="icon" :src="user.Avatar" width="24" height="24">
                            </mt-cell>
                        </mt-index-section>
                    </mt-index-list>
                </mt-popup>
            </div>

        </div>
    </body>
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/vue.js"></script>
    <script src="https://unpkg.com/mint-ui/lib/index.js"></script>
    <!-- <script src="Scripts/mobileCommon.js"></script> -->
    <script type="text/javascript">
        var url = 'mEmailGroupSetting.aspx';
        var vue = new Vue({
            el: '#vue',
            data: {
                popupVisible: false,
                popupTitle: '',
                type: 'add',
                userList: [],
                hasRightFormList: [],
                MessageBox: '',
                groupName: '',
                groupId: '',
                members: [],
                memberString: '',
                groupList: [],
                userListHeight: 0
            },
            methods: {
                backToEmailIndex: function () {
                    location.href = "mEmailIndex.aspx";
                },
                backToIndex: function () {
                    this.popupVisible = false;
                },
                submit: function () {
                    if (this.groupName == '') {
                        this.MessageBox.alert('分组名不能为空！');
                        return;
                    }
                    if (this.members.length == 0) {
                        this.MessageBox.alert('请选择分组成员！');
                        return;
                    }
                    for (var i = 0; i < this.members.length; i++) {
                        delete this.members[i].Checked;
                        delete this.members[i].Avatar;
                    }
                    data = {
                        act: 'Submit', type: this.type,
                        members: JSON.stringify(this.members),
                        groupName: this.groupName,
                        groupId: this.groupId
                    };
                    // this.$indicator.open({
                    //     text: '加载中...',
                    //     spinnerType: 'fading-circle'
                    // });
                    // setTimeout(function(){
                    //     $.post(url,data,function(res){
                    //     vue.$indicator.close();
                    //     var dataRes = JSON.parse(res);
                    //     vue.MessageBox.alert(dataRes.ErrMsg).then(action => {
                    //         if(dataRes.ErrCode == 0){
                    //             vue.popupVisible = false;
                    //             loadGroupInfo();
                    //         }
                    //     });
                    // });
                    // },2000);
                    SubmitFunc();
                },
                createGroup: function () {
                    this.popupVisible = true;
                    this.popupTitle = "新建分组";
                    loadUserList();
                    this.memberString = '';
                    this.members = [];
                    this.groupName = '';
                    
                    // $(".mint-popup.mint-popup-right").css("width", "100%").css("height", "100%");
                },
                modifyGroup: function (group) {
                    this.popupVisible = true;
                    this.popupTitle = "分组名：" + group.GroupName;
                    this.groupName = group.GroupName;
                    this.groupId = group.Id;
                    this.type = 'modify';
                    loadUserList();
                    this.memberString = '';
                    this.members = JSON.parse(group.GroupMember);
                    GenerateMemberString();
                    var tempMembers = JSON.parse(group.GroupMember);
                    if (tempMembers.length > 0) {
                        for (var i = 0; i < this.userList.length; i++) {
                            var len = this.userList[i].Users.length;
                            for (var j = 0; j < len; j++) {
                                if (tempMembers.length == 0)
                                    return;
                                for (var k = tempMembers.length - 1; k >= 0; k--) {
                                    if (this.userList[i].Users[j].UserId == tempMembers[k].UserId) {
                                        this.userList[i].Users[j].Checked = true;
                                        tempMembers.splice(k, 1);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                },
                deleteGroup: function (group) {
                    this.MessageBox.confirm('确定删除该分组?', '提示').then(action => {
                        var data = { act: 'DeleteGroup', groupName: group.GroupName };
                        var res = JSON.parse(AjaxSync(url, data));
                        this.MessageBox.alert(res.ErrMsg).then(action => {
                            loadGroupInfo();
                        });
                    });
                },
                clickUser: function (indexGroup, indexUser) {
                    var index = $.inArray(this.userList[indexGroup].Users[indexUser], this.members)
                    var needRebuiltMemberString = false;
                    if (!this.userList[indexGroup].Users[indexUser].Checked) {
                        this.userList[indexGroup].Users[indexUser].Checked = true;
                        if (index < 0) {
                            this.members.push(this.userList[indexGroup].Users[indexUser]);
                            needRebuiltMemberString = true;
                        }
                    }
                    else {
                        this.userList[indexGroup].Users[indexUser].Checked = false;
                        if (index >= 0) {
                            this.members.splice(index, 1);
                            needRebuiltMemberString = true;
                        }
                    }
                    if (needRebuiltMemberString) {
                        GenerateMemberString();
                    }
                },
                autoUserListHeight: function () {
                    var tbxlocationY = $("#tbx_members").offset().top;
                    var tbxHeight = $("#tbx_members").height();
                    var windowsHeight = $(window).height();
                    this.userListHeight = windowsHeight - tbxlocationY - tbxHeight;
                }

            },
            mounted: function () {
                // InitUserCellClickEvent();
                // loadGroupInfo();
                this.MessageBox = this.$messagebox;
                
                // this.Indicator = this.$indicator;
            },
            created: function () {
                // loadUserList();
                // autoUserListHeight();
                
            },
            beforeCreate: function () {
                // InitUserCellClickEvent();

            },
            updated: function () {
                // $("a[name='userCell']").click(function () {
                //     $(this).find('checkbox').attr('checked','checked');
                //     alert('111');
                // });
                this.autoUserListHeight();
            }
        });

        function autoUserListHeight() {
            var tbxlocationY = $("#tbx_members").offset().top;
            var tbxHeight = $("#tbx_members").height();
            var windowsHeight = $(window).height();
            vue.userListHeight = windowsHeight - tbxlocationY - tbxHeight;
        }

        function SubmitFunc() {
            setTimeout(function () {

            }, 2000);
            $.post(url, data, function (res) {
                var dataRes = JSON.parse(res);
                vue.MessageBox.alert(dataRes.ErrMsg).then(action => {
                    if (dataRes.ErrCode == 0) {
                        vue.popupVisible = false;
                        loadGroupInfo();
                    }
                });
            });
        }

        loadGroupInfo();
        // autoUserListHeight();

        function loadGroupInfo() {
            var data = { act: "GetGroupList" };
            var res = AjaxSync(url, data);
            var dataSrc = JSON.parse(res);
            vue.groupList = dataSrc;
            // $.post(url, data, function (res) {
            //     vue.groupList = JSON.parse(res).rows;
            // });
        };

        function GenerateMemberString() {
            vue.memberString = '';
            // $.each(this.members, function (i, val) {
            //     this.memberString += val.Name + ','
            // });
            if (vue.members == undefined || vue.members.length == 0) {
                return;
            }
            for (var i = 0; i < vue.members.length; i++) {
                vue.memberString += (vue.members[i].UserName + ',');
            }
            vue.memberString = vue.memberString.substr(0, vue.memberString.length - 1);
        }

        function AjaxSync(url, data) {
            var res = $.ajax({
                async: false,
                cache: false,
                type: 'post',
                url: url,
                data: data
            }).responseText;
            return res;
        }

        function loadUserList() {
            var data = { act: "getUsers" };
            var res = AjaxSync(url, data);
            vue.userList = JSON.parse(res);
            // $.post(url, data, function (res) {
            //     vue.userList = JSON.parse(res);
            // });
        }
    </script>


    </html>