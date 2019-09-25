<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mPointDetail.aspx.cs" Inherits="mPointDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>积分申请详情</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <script src="Scripts/weui.min.js"></script>
    <script src="Scripts/jquery.min.js"></script>
    <style>
        #aaaa{
            font-weight: 400;
            font-size: 17px;
            width: auto;
            overflow: hidden;
            text-overflow: ellipsis;
           color:blue;
        }
        .weui-gallery__img {
    position: absolute;
    top: 0;
    right: 0;
    bottom: 60px;
    left: 0;
    background: 50% no-repeat;
    background-size: contain;
}
    </style>
   
</head>
<body>

<div id="total">
    <div class="weui-panel weui-panel_access">
    <div class="page__hd">
        <h1 class="page__title">积分申请</h1>
    </div>
    <div class="page__bd">
        <article class="weui-article">
            <h1>积分申请详情</h1>
            <section>
                <h2 class="title" id="aaaa">事件：{{list.Event}}</h2>
                <p>申请时间：{{list.CreatingTime}}</p>
            </section>
        </article>





            <div class="weui-panel__hd">人员信息</div>
            <div class="weui-panel__bd" id="person">
                <a href="javascript:void(0);" class="weui-media-box weui-media-box_appmsg" >
                    <div class="weui-media-box__hd">
                        <img class="weui-media-box__thumb" :src="list.proposerAvatar" alt="">
                    </div>
                    <div class="weui-media-box__bd">
                        <h4 class="weui-media-box__title">申请人：{{list.Proposer}}</h4>
                         <p class="weui-media-box__desc">{{list.proposerDepartment}}</p>
                    </div>
                </a>
                <a href="javascript:void(0);" class="weui-media-box weui-media-box_appmsg"  v-for="row in form">
                    <div class="weui-media-box__hd">
                        <img class="weui-media-box__thumb":src="row.targetAvatar" alt="">
                    </div>
                    <div class="weui-media-box__bd">
                        <h4 class="weui-media-box__title">被申请人：{{row.Target}}</h4>
                        <p class="weui-media-box__desc">{{row.targetDepartment}}</p>
                        <p class="weui-media-box__desc">状态：{{row.CheckState}}&nbsp&nbsp B积分：{{row.Bpoint}}</p>
                    </div>
                </a>
                
            </div>
        


        
    </div>
</div>



    



         <div class="js_dialog" id="dlg" style="display: none;">
             <div class="weui-mask"></div> 
            <div class="weui-dialog">
                <div class="weui-dialog__bd">{{message}}</div> 
                <div class="weui-dialog__ft"> 
                     <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_primary" onclick="location.href='mPointApplyRecord.aspx'">确定</a>
                </div>
            </div> 
        </div> 

    

    <div class="weui-gallery" id="gg" style="display: none;">
        <span class="weui-gallery__img" id="galleryImg" ></span>
    </div>






 </div>
</body>
</html>

<script src="Scripts/jquery.min.js"></script>
<script src="Scripts/vue.js"></script>
<script>
    var vue = new Vue({
        el: '#total',
        data: {
            form: [],
            message: '',
            list: [],
            picture:''
        },

        mounted: function () {

            initForm();
            $(function () {

                $('#person').on("click", "img", function () {
                    $("#galleryImg").css("background", "url(" + this.getAttribute('src') + ") no-repeat center center");
                    $("#galleryImg").css("background-size", "contain");
                     $("#gg").fadeIn(100);
                });
                $("#gg").on("click", function () {
                     $("#gg").fadeOut(100);
                });
            });
        }
    });
    function initForm() {
        var ids = GetQueryString('ids');
       $.post('mPointDetail.aspx', { act: 'initForm', ids: ids },
            function (res) {
                var data = JSON.parse(res);
                if (data.ErrCode != 0) {
                    vue.message = data.ErrMsg;
                    $('#dlg').fadeIn(200);
                }
                vue.form = JSON.parse(data.document);
                vue.list=JSON.parse(data.document)[0];
            })
    }

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }




</script>

