<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>业力企业办公平台登录</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <%--<script src="Scripts/base-loading.js"></script>--%>
    <style type="text/css">
        .backgroundImage {
            width: 100%;
            height: 100%;
            background-position: center center;
            position: absolute;
            top: 0px;
            left: 0px;
            background-image: url('../resources/login.png');
            background-repeat: no-repeat;
            background-size: cover;
        }

        #login {
            position: absolute;
            top: 60%;
            left: 50%;
            margin: -150px 0 0 -150px;
            width: 300px;
            height: 300px;
        }

            #login h1 {
                color: #fff;
                text-shadow: 0 0 10px;
                letter-spacing: 1px;
                text-align: center;
            }

        input {
            width: 280px;
            height: 30px;
            /*margin-bottom: 50px;
            outline: none;
            padding: 10px;
            font-size: 13px;
            color: #fff;
            text-shadow: 1px 1px 1px;
            border-top: 1px solid #312E3D;
            border-left: 1px solid #312E3D;
            border-right: 1px solid #312E3D;
            border-bottom: 1px solid #56536A;
            border-radius: 4px;
            background-color: #2D2D3F;*/
        }
    </style>
</head>

<body style="width: 100%; height: 100%;">
    <form id="form1" runat="server" style="width: 100%; height: 100%;">
        <div class="backgroundImage">
            <input id="vCode" type="hidden" />
            <div id="login">
                <h1>业力企业办公平台</h1>
                <table style="width: 100%;">
                    <tr style="height: 45px">
                        <td colspan="2">
                            <input id="Text1" class="easyui-textbox easyui-validatebox" data-options="
									required:true,
                                    prompt: '请输入用户姓名或手机号',
                                    iconCls:'icon-man',
                                    iconWidth:30
                                    " />
                        </td>
                    </tr>
                    <tr style="height: 45px">
                        <td colspan="2">
                            <input id="Password1" class="easyui-validatebox easyui-passwordbox" data-options="
									required:true,
									prompt:'请输入6-16位密码!',
									validType:'length[6,16]'
									" />
                        </td>
                    </tr>
                    <tr style="height: 45px">
                        <td>
                            <input type="text" class="easyui-textbox" id="vc" style="width: 200px" data-options="
                                prompt: '请输入验证码',
                                " />
                        </td>
                        <td>
                            <img id="imgVc" style="width: 80px; height: 35px" src="" />
                        </td>
                    </tr>
                    <tr style="height: 45px">
                        <td style="width: 100px;">
                            <span style="color: #fff; text-shadow: 0 0 10px; letter-spacing: 1px; text-align: center;">自动登录(7天)：</span>
                        </td>
                        <td>
                            <input id="Checkbox1" class="easyui-switchbutton" data-options="onText:'是',offText:'否',checked:true"></td>

                    </tr>
                    <tr style="height: 45px">
                        <td colspan="2">
                            <a class="easyui-linkbutton c6" style="width: 290px; height: 40px; font-size: large;" data-options="size:'large'" onclick="login();">登 录</a>
                        </td>
                     </tr>  
                    <tr style="height: 45px">
                        <td colspan="2">
                        <a class="easyui-linkbutton " style="width: 290px; height: 40px; font-size: large;background-image:url('../resources/login_blue_big.png');"  data-options="size:'large'" onclick="wxlogin();"></a>
                         </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id='Loading' style="position: absolute; z-index: 1000; top: 0px; left: 0px; width: 100%; height: 100%; background: gray; text-align: center;">
            <h1 style="top: 48%; position: relative; left: 0px;">
                <font color="#15428B">登录中···</font>
            </h1>
        </div>
    </form>

    <script type="text/javascript">
        $(document).ready(function () {
            //IsMobile();
            $("#Loading").hide();
            check();
            //init();
            getVC();
            $("#imgVc").click(function () {
                getVC();
            });
        });

        $(document).keydown(function(event){
            if(event.keyCode == 13){
                login();
            }
        });

        function getCookie(name) {
            var cookies = document.cookie.split(';');
            if (!cookies.length) return '';
            for (var i = 0; i < cookies.length; i++) {
                var pair = cookies[i].split('=');
                if ($.trim(pair[0]) == name) {
                    return $.trim(pair[1]);
                }
            }
            return '';
        }
        //弹出加载层
        function showLoading() {
            $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
            $("<div class=\"datagrid-mask-msg\"></div>").html("登录中。。。").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
        }

        //取消加载层  
        function hideLoading() {
            $(".datagrid-mask").remove();
            $(".datagrid-mask-msg").remove();
        }
        function login() {
            if ($("#vc").val().toUpperCase() != $("#vCode").val()) {
                getVC();
                $.messager.alert('登录错误', '验证码错误，请输入正确的验证码!', 'error');                
                $("#vc").val("");
                $("#vc").focus();
                return;
            }
            var userName = $("#Text1").textbox('getText');
            var psw = $("#Password1").val();
            var remerberMe = $("#Checkbox1").switchbutton("options").checked;
            if (!$("#Password1").validatebox("isValid")) {
                $.messager.alert('登录错误', '密码需要在6-16位之间!', 'error');
                return;
            }
            if (userName == null || userName == "") {
                $.messager.alert('登录错误', '用户名不能为空!', 'error');
                return;
            }
            //$("#Loading").show();
            showLoading();
            $.post(
                "login.aspx", //url
                { //data
                    action: "login",
                    user: userName,
                    psw: psw,
                    remerberMe: remerberMe
                },
                function (ret) { //success
                    //$("#Loading").hide();                    
                    if (ret == "T") {
                        //$.messager.show({
                        //    msg: '登录成功！',
                        //    timeout: 5000,
                        //    showType: 'slide'
                        //});
                        location.href = "index.aspx";
                    } else {
                        $.messager.alert('登录错误', ret, 'error');
                    }
                    hideLoading();
                }
            );
        }

      

        //微信登陆;
        function wxlogin() {
         


            location.href = "login.aspx?act=wxlogin";
        }

        function check() {
            // var username = getCookie("RememberMe");
            // if (username != null) {
            //     $("#Text1").textbox('setText', username);
            // }
            showLoading();
            $.post(
                "login.aspx", //url
                { //data
                    action: "check"
                },
                function (ret) { //success
                    
                    if (ret != "") {
                        if (ret == "T") {
                            location.href = "index.aspx";
                        } else {
                            $("#Text1").textbox('setText',ret);
                        }
                    }
                    //$.messager.alert('登录错误', ret, 'error');
                    hideLoading();
                }
            );
        }

        function IsMobile() {
            $.post(
                "ashx/IsMobileHandler.ashx",
                null,
                function (res) {
                    if (res == "T") {
                        location.href = "mindex.aspx";
                    }
                }
            );
        }

        function getVC() {
            $.post(
                "login.aspx", //url
                { //data
                    action: "getValideCode"
                },
                function (ret) { //success
                    if (ret != "") {
                        $("#vCode").val(ret);
                        $("#imgVc").attr("src", "ValideCode.aspx?code='" + ret + "'");
                    }
                }
            );
        }

        function init() {
            $.post(
                "login.aspx", //url
                { //data
                    action: "getValideCode"
                },
                function (ret) { //success
                    if (ret != "") {
                        $("#Text1").textbox("setValue", ret);
                        $("#Checkbox1").switchbutton("check");
                    }
                }
            );
        }
    </script>
</body>

</html>
