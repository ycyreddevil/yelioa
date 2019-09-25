<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>业力企业办公平台主页</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <!--<link href="http://g.alicdn.com/sj/dpl/1.5.1/css/sui.min.css" rel="stylesheet">-->
    <!--<link rel="stylesheet" href="http://cdn.static.runoob.com/libs/bootstrap/3.3.7/css/bootstrap.min.css">-->
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/jquery.json-2.4.min.js"></script>
    <script src="Scripts/index.js"></script>
    <%--<script src="Scripts/base-loading.js"></script>--%>
    <style type="text/css">
        .LeftItem{
            text-transform: none; text-decoration: none;
        }
    </style>
</head>
<body class="easyui-layout" style="width: 100%; height: 100%" fit="true" scroll="no">
    <div data-options="region:'north',border:false" style="height: 66px; background-color: #6388B8;">
        <table style="width: 100%; height: 100%">
            <tr>
                <td style="width: 20px">&nbsp;</td>
                <td style="width: 300px"><a style="text-transform: none; text-decoration: none;" href="index.aspx">
                    <span style="color: #FFFFFF; font-size: xx-large">企业办公平台</span>
                </a></td>
                <td id="tdTopBlank">&nbsp;</td>
                <td style="text-align:right">
                    <a href="javascript:void(0)" class="easyui-menubutton c8" data-options="menu:'#mm1',iconCls:'icon-man',plain:true">个人信息</a>

                </td>
            </tr>
        </table>
        <div id="mm1" style="width: 150px;">
            <div><a href="MyInfo.aspx">个人设置</a></div>
            <div><a href="javascript:void(0)" onclick="Logout()">退出</a></div>
        </div>
    </div>
    <div data-options="region:'west',split:true,collapsible:true" style="width: 266px; height: 100%; background-color: #F5F5F5;" title="导航菜单">
        <div id="accordion" class="easyui-accordion" data-options="multiple:false" style="background-color: #F5F5F5; text-align: center;">
            <%--<div title="进销存数据管理">
                <ul class="easyui-datalist" lines="true">
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('入库数据','EnterStock.aspx');">
                            <span >入库数据</span>
                        </a> 
                    </li>
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('出库数据','LeaveStock.aspx');">
                            <span >出库数据</span>
                        </a> 
                    </li>
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('网点管理','CostSharingManage.aspx');">
                            <span >网点管理</span>
                        </a> 
                    </li>
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('流向统计','FlowManage.aspx');">
                            <span >流向统计</span>
                        </a> 
                    </li>
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('出库模板','LeavStkTemplate.aspx');">
                            <span >出库模板</span>
                        </a> 
                    </li>
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('产品管理','ProductManage.aspx');">
                            <span >产品管理</span>
                        </a> 
                    </li>
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('机构管理','OrganizationMng.aspx');">
                            <span >机构管理</span>
                        </a> 
                    </li>
                </ul>
            </div>
            <div title="财务数据管理">
            </div>
            <div title="联系人">
                <ul class="easyui-datalist" lines="true">
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('通讯录','memberSearch.aspx');">
                            <span >通讯录</span>
                        </a>                        
                    </li>
                </ul>
            </div>
            <div title="系统管理">
                <ul class="easyui-datalist" lines="true">
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('人员架构管理','MemberManage.aspx');">
                            <span >人员架构管理</span>
                        </a>  
                    </li>
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('岗位管理','PostManage.aspx');">
                            <span >岗位管理</span>
                        </a>
                    </li>
                </ul>
            </div>
            <div title="我的信息">
                <ul class="easyui-datalist" lines="true">
                    <li>
                        <a href="javascript:void(0)" class="LeftItem" onclick="addTab('个人设置','MyInfo.aspx');">
                            <span >个人设置</span>
                        </a>  
                    </li>
                </ul>
            </div>--%>
        </div>
    </div>
    <%--<div data-options="region:'east',split:false"style="width: 100px;"></div>--%>
    <div data-options="region:'center',title:'主窗口',iconCls:'icon-ok'" style="height: 100%;">
        <div id="tt" class="easyui-tabs" style="width: 100%; height: 100%;" data-options="tools:'#tab-tools'">
            <div title="欢迎" style="padding: 10px;text-align:center">
                <h1>欢迎使用业力企业办公平台</h1>
            </div>
        </div>
        <div id="tab-tools">
            <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-add'" onclick="addTab('登录','login.aspx')"></a>--%>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-reload'" onclick="RefreshTab()"></a>
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-cancel'" onclick="removeTab()"></a>
        </div>
    </div>
    <div data-options="region:'south',split:true" style="height: 30px;text-align:center; vertical-align: middle;">
        <span style="text-align:center; vertical-align: middle">网站备案号:赣ICP备16002879号-2</span>
    </div>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
        background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" 
        class="easyui-window" border="false" noheader="true" closed="true" modal="true">
    </div>
    <script type="text/javascript">
        var userInfo;
        $(document).ready(function () {
            checkLogin();
            InitControls();
        });
        function checkLogin()
        {
            var checkLogin = $.ajax({
                async: false,
                cache: false,
                type: 'post',
                url: 'index.aspx',
                data: {
                    act: 'checkLogin'
                }
            }).responseText;
            if (checkLogin == "F") {
                location.href = 'login.aspx';
            }
            else {
                //userInfo = $.parseJSON(checkLogin);
            }
        }

        function GetUserInfo() {
            return userInfo;
        }

        function Loading(OnOff) {
            checkLogin();
            if (!IsLoadingNow && OnOff) {
                $('#loading').window('open');
                IsLoadingNow = OnOff;
            }
            else if (IsLoadingNow && !OnOff) {
                $('#loading').window('close');
                IsLoadingNow = OnOff;
            }
            else {
            }

        }
    </script>
</body>

</html>
