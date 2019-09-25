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
    ') no-repeat scroll 5px 12px; border: 2px solid #95B8E7; color: #696969; font-family:\'Microsoft YaHei\';">Loading...</div></div>';

//if (self == top) {
//    location.href = 'login.aspx';
//}
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

var IsLoadingNow = false;

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

function Loading(OnOff) {
    if (!IsLoadingNow && OnOff) {
        $('#loading').dialog('open').dialog('center');
        IsLoadingNow = OnOff;
    }
    else if (IsLoadingNow && !OnOff) {
        $('#loading').dialog('close');
        IsLoadingNow = OnOff;
    }
    else {
    }

}

function is_weixn() {
    var ua = navigator.userAgent.toLowerCase();
    if (ua.match(/MicroMessenger/i) != "micromessenger") {
        location.href = 'noAuthority.aspx';
    }
}

function contains(arr, obj) {
    var i = arr.length;
    while (i--) {
        if (arr[i] === obj) {
            return true;
        }
    }
    return false;
}

function indexOf(arr, item) {
    var index = -1;
    for (var i = 0; i < arr.length; i++) {
        if (arr[i] === item)
            index = i;
    }
    return index;
}

