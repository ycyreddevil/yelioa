



//////////////////////////////////////////变量
var IsLoadingNow = false;










////////////////////////////////////////////////////////////base-loading.js
//获取浏览器页面可见高度和宽度
var _PageHeight = document.documentElement.clientHeight,
    _PageWidth = document.documentElement.clientWidth;

//计算loading框距离顶部和左部的距离（loading框的宽度为215px，高度为61px）
var _LoadingTop = _PageHeight > 61 ? (_PageHeight - 61) / 2 : 0,
    _LoadingLeft = _PageWidth > 215 ? (_PageWidth - 215) / 2 : 0;

//加载gif地址
var Loadimagerul = "/resources/loadingSmall.gif";

//在页面未加载完毕之前显示的loading Html自定义内容
var _LoadingHtml = '<div id="loadingDiv" style="position:absolute;left:0;width:100%;height:' + _PageHeight
    + 'px;top:0;background:#f3f8ff;opacity:1;filter:alpha(opacity=80);z-index:10000;"><div style="position: absolute; cursor1: wait; left: '
    + _LoadingLeft + 'px; top:' + _LoadingTop +
    'px; width:100px;; height: 61px; line-height: 57px; padding-left: 50px; padding-right: 5px; background: #fff url('
    + Loadimagerul +
    ') no-repeat scroll 5px 12px; border: 2px solid #95B8E7; color: #696969; font-family:\'Microsoft YaHei\';">玩命加载中...</div></div>';
//呈现loading效果
document.write(_LoadingHtml);
//parent.Loading(true);

//监听加载状态改变
document.onreadystatechange = completeLoading;

//加载状态为complete时移除loading效果
function completeLoading() {
    if (document.readyState == "complete") {
        var loadingMask = document.getElementById('loadingDiv');
        loadingMask.parentNode.removeChild(loadingMask);
        //parent.Loading(false);
    }
}

////////////////////////////////////////////////////////////base-loading.js


function AjaxSync(url, data) {
    Loading(true);
    var res = $.ajax({
        async: false,
        cache: false,
        type: 'post',
        url: url,
        data: data
    }).responseText;
    Loading(false);
    return res;
}

//////////////////////////////////////////////顶部导航栏相关函数
function Logout() {
	$.post(
		"login.aspx", //url
		{ //data
			action: "logout"
		},
		function(ret) { //success
			location.href = "login.aspx";
		}
	);
}

function InitControls()
{
    InitMenu();
}

function InitMenu()
{
    var res = AjaxSync("index.aspx", {
        act:'getMenu'
    });
    var menu = JSON.parse(res);
    $.each(menu, function (i, val) {
        var ul = "<ul class='easyui-datalist' lines='true'>";
        $.each(menu[i], function (index, value) {
            ul += '<li><a href="javascript:void(0)" class="LeftItem" onclick="addTab(\''
                + value.Name + '\',\'' + value.WebSite + '\');"><span >' + value.Name + '</span></a></li>';
        });
        ul += "</ul>";
        $("#accordion").accordion('add', {
            title: i,
            content: ul,
            selected: false
        });
    });

    $("#accordion").accordion({
        onSelect: function () {
            $("ul.easyui-datalist").datalist("clearSelections");
        }
    });

    $("#accordion").accordion('select', 0);
}

//////////////////////////////////////////////tab 控制相关函数
function RefreshTab()
{
    var tab = $('#tt').tabs('getSelected');
    if (tab && tab.find('iframe').length > 0) {
        var iframe = tab.find('iframe')[0];
        iframe.src = iframe.src;
    }
}

function addTab(title, url) {
    var index = url.indexOf(".aspx");
    var id = url.substr(0, index);
    if ($('#tt').tabs('exists', title)) {
        var tabSelect = $('#tt').tabs('getSelected');
        if (tabSelect.panel('options').title == title) {
            id = '#' + id;
            $(id).attr('src', $(id).attr('src'));
        }
        else {
            $('#tt').tabs('select', title);
        }
        
    } else {        
        var content = '<iframe scrolling="auto" frameborder="0" id="' + id + '" src="' + url + '" style="width:100%;height:100%;"></iframe>';
        $('#tt').tabs('add', {
            title: title,
            content: content,
            closable: true
        });
    }
}

function removeTab() {
    var tab = $('#tt').tabs('getSelected');
    if (tab) {
        if (tab.panel('options').title != '欢迎') {
            var index = $('#tt').tabs('getTabIndex', tab);
            $('#tt').tabs('close', index);
        }        
    }
}


