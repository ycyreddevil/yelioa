//��ȡ�����ҳ��ɼ��߶ȺͿ��
var _PageHeight = document.documentElement.clientHeight,
    _PageWidth = document.documentElement.clientWidth;

//����loading����붥�����󲿵ľ��루loading��Ŀ��Ϊ215px���߶�Ϊ61px��
var _LoadingTop = _PageHeight > 61 ? (_PageHeight - 61) / 2 : 0,
    _LoadingLeft = _PageWidth > 215 ? (_PageWidth - 215) / 2 : 0;

//����gif��ַ
var Loadimagerul = "/resources/loadingSmall.gif";

//��ҳ��δ�������֮ǰ��ʾ��loading Html�Զ�������
var _LoadingHtml = '<div id="loadingDiv" style="position:absolute;left:0;width:100%;height:' + _PageHeight
    + 'px;top:0;background:#f3f8ff;opacity:1;filter:alpha(opacity=80);z-index:10000;"><div style="position: absolute; cursor1: wait; left: '
    + _LoadingLeft + 'px; top:' + _LoadingTop +
    'px; width:100px;; height: 61px; line-height: 57px; padding-left: 50px; padding-right: 5px; background: #fff url('
    + Loadimagerul +
    ') no-repeat scroll 5px 12px; border: 2px solid #95B8E7; color: #696969; font-family:\'Microsoft YaHei\';">Loading...</div></div>';

//if (self == top) {
//    location.href = 'login.aspx';
//}
//����loadingЧ��
document.write(_LoadingHtml);
//parent.Loading(true);

//��������״̬�ı�
document.onreadystatechange = completeLoading;

//����״̬Ϊcompleteʱ�Ƴ�loadingЧ��
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

