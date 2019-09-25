<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mFormIndex.aspx.cs" Inherits="mFormIndex" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>业力表单</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link rel="stylesheet" href="https://unpkg.com/mint-ui/lib/style.css">
    <style>
        [v-cloak] {
            display: none;
        }
    </style>
</head>
<body>
    <div id="vue" v-cloak>
        <mt-header title="业力表单"></mt-header>
        <div style="margin-top:20px">
            <mt-cell id="hr" title="人事类" is-link><mt-spinner id="hr_spinner" color="#26a2ff" style="display: none" type="snake"></mt-spinner></mt-cell>
            <mt-cell id="fn" title="财务类" is-link><mt-spinner id="fn_spinner" color="#26a2ff" style="display: none" type="snake"></mt-spinner></mt-cell>
            <mt-cell id="ad" title="行政类" is-link><mt-spinner id="ad_spinner" color="#26a2ff" style="display: none" type="snake"></mt-spinner></mt-cell>
            <mt-cell id="op" title="运营类" is-link><mt-spinner id="op_spinner" color="#26a2ff" style="display: none" type="snake"></mt-spinner></mt-cell>
            <mt-cell id="rd" title="研发类" is-link><mt-spinner id="rd_spinner" color="#26a2ff" style="display: none" type="snake"></mt-spinner></mt-cell>
            <mt-cell id="sa" title="销售类" is-link><mt-spinner id="sa_spinner" color="#26a2ff" style="display: none" type="snake"></mt-spinner></mt-cell>
            <mt-cell id="ot" title="其他"  is-link><mt-spinner id="ot_spinner" color="#26a2ff" style="display: none" type="snake"></mt-spinner></mt-cell>
        </div>
        <div>
            <mt-popup v-model="popupVisible" position="right">
                <mt-index-list :show-indicator="false">
                    <mt-header :title="popupTitle">
                        <mt-button icon="back" slot="left" @click="backToIndex()"></mt-button>
                    </mt-header>
                    <mt-cell v-for="subForm in subFormList" :title="subForm.FormName" label="">
                        <mt-button size="small" type="default" @click="toSubmit(subForm.Id)">提交</mt-button>
                        <%--<mt-button size="small" type="default" @click="toListAndDetail(subForm.FormName)">查询</mt-button>--%>
                    </mt-cell>

                </mt-index-list>
            </mt-popup>
        </div>
    </div>    
</body>
<script src="Scripts/jquery.min.js"></script>
<script src="Scripts/vue.js"></script>
<script src="https://unpkg.com/mint-ui/lib/index.js"></script>
<script>
    var vue = new Vue({
        el: '#vue',
        data: {
            popupVisible: false,
            popupTitle: '',
            subFormList: [],
            hasRightFormList:[],
            messagebox: '',
        },
        methods: {
            toSubmit: function (id) {
                for (var i = 0; i < this.hasRightFormList.length; i++) {
                    if (id == this.hasRightFormList[i].Id) {
                        location.href = 'mFormDetail.aspx?id=' + id;
                        return;
                    }
                }             
                this.messagebox.alert('抱歉，您暂无此权限!').then(action => {
                  
                });
                
            },
            toListAndDetail: function (formName) {
                location.href = 'mFormListAndDetail.aspx?formName=' + formName;
            },
            backToIndex: function() {
                vue.popupVisible = false;
            }
        },
        mounted: function () {
            this.messagebox = this.$messagebox;
        }
    })

    $("#hr").click(function () {
        //$("#hr_spinner").css("display", "block");
        vue.popupTitle = "人事类";
        showSubFormList('人事');
        //$("#hr_spinner").css("display", "none");
    })
    $("#fn").click(function () {
        vue.popupTitle = "财务类";
        showSubFormList('财务');
    })
    $("#ad").click(function () {
        vue.popupTitle = "行政类";
        showSubFormList('行政');
    })
    $("#op").click(function () {
        vue.popupTitle = "运营类";
        showSubFormList('运营');
    })
    $("#rd").click(function () {
        vue.popupTitle = "研发类";
        showSubFormList('研发');
    })
    $("#sa").click(function () {
        vue.popupTitle = "销售类";
        showSubFormList('销售');
    })
    $("#ot").click(function() {
        vue.popupTitle = "其他";
        showSubFormList('其他');
    });

    function showSubFormList(type) {
        var newType = "";
        if (type == "人事") {
            newType = "hr";
        } else if (type == "财务") {
            newType = "fn";
        } else if (type == "行政") {
            newType = "ad";
        } else if (type == "运营") {
            newType = "op";
        } else if (type == "销售") {
            newType = "sa";
        } else if (type == "研发") {
            newType = "rd";
        } else {
            newType = "ot";
        }
        $("#"+newType+"_spinner").css("display", "block");
        
        $.ajax({
            url: 'mFormIndex.aspx',
            data: { act: 'findFormByType', type: type },
            dataType: 'json',
            type: 'post',
            success: function(data) {
                if (data.ErrCode == 0) {
                    vue.subFormList = JSON.parse(data.data);
                    vue.hasRightFormList = JSON.parse(data.newData);
                    vue.popupVisible = true;
                    $(".mint-popup.mint-popup-right").css("width", "100%").css("height", "100%");
                } else {
                    vue.messagebox.alert('未找到相关表单').then(action => {
                        vue.popupVisible = false;
                    });
                }
                $("#"+newType+"_spinner").css("display", "none");
            }
        });
    }
</script>
</html>
