using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// WxNetSalesHelper 的摘要说明
/// </summary>
public class WxNetSalesHelper
{
    public WxNetSalesHelper(string url)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        RedirectUri = url;
        Initi();
    }

    public WxNetSalesHelper(string appSecret, string thisAppName, string agentId)
    {
        NetSalesAppSecret = appSecret;//"PyO4Il3bIxyuFquBAGrrr76GVcUbIN5NPpxNGAja-4U";
        ThisAppName = thisAppName;//"NetSales";
        AgentId = agentId;
        //
        // TODO: 在此处添加构造函数逻辑
        //
        Initi();
    }

    private void Initi()
    {
        wxHelper = new WxCommon(ThisAppName, NetSalesAppSecret);
    }

    WxCommon wxHelper = null;

    private string Corpid = "wx9f620907462561ca";
    private string NetSalesAppSecret = "";//"PyO4Il3bIxyuFquBAGrrr76GVcUbIN5NPpxNGAja-4U";
    private string ThisAppName = "";//"NetSales";
    private string AgentId = "";//"1000002";

    public string RedirectUri { get; set; }
    public UserInfo User { get; set; }

    //public string GetWxToken(string AppName)
    //{
    //    string WxToken = CookieHelper.GetCookieValue(AppName+"WxToken");
    //    if(string.IsNullOrEmpty(WxToken))
    //    {
    //        WxToken = GetWxTokenFromWx(AppName);
    //    }
    //    return WxToken;
    //}
    //public   string GetWxTokenFromWx(string AppName)
    //{
    //    string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}",
    //           Corpid , NetSalesAppSecret);

    //    string res = HttpHelper.Get(url);
    //    Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(res);
    //    if (!dict.Keys.Contains("access_token") )
    //    {            
    //        return res;
    //    }
    //    else
    //    {
    //        int expires_in = Convert.ToInt32(dict["expires_in"]);
    //        CookieHelper.SetCookie(AppName + "WxToken", dict["access_token"].ToString(), DateTime.Now.AddSeconds(expires_in));
    //        return dict["access_token"].ToString();
    //    }
            
    //}

    public   string GetWxUserId(string code,string token)
    {
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}",
            token, code);
        string res = HttpHelper.Get(url);
        Dictionary<string, object>  dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(res);
        if (!dict.Keys.Contains("UserId"))
        {
            return res;
        }
        else
            return dict["UserId"].ToString();
    }

    public  string GetUserInfo(string token, string wechatUserId,ref UserInfo user)
    {
        //string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}",
        //   token, userId);
        //string res = HttpHelper.Get(url);
        //Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(res);

        //if (!dict.Keys.Contains("userid"))//获取用户信息失败
        //{
        //    return res;
        //}
        string msg = "";
        user = UserInfoManage.GetUserInfo(wechatUserId,ref msg);
        if (user != null)
            return "success";
        else
            return msg;
    }

    private   void GotoGetCode(HttpContext context)
    {
        string randomString = ValideCodeHelper.GetRandomCode(16);
        context.Session["randomString"] = randomString;
        //
        //string urlForGettingCode = string.Format(@"https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid={0}" +
        //        "&agentid={1}&redirect_uri={2}&state={3}",
        //        Corpid, AgentId, HttpUtility.UrlEncode(RedirectUri), randomString);
        string urlForGettingCode = string.Format(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}"
            + "&redirect_uri={1}&response_type=code&scope=snsapi_base&agentid={2}&state={3}#wechat_redirect"
            , Corpid, HttpUtility.UrlEncode(RedirectUri), AgentId, randomString);
        context.Response.Redirect(urlForGettingCode, false);
    }

    public  string CheckAndGetUserInfo(HttpContext context)
    {
        UserInfo user = (UserInfo)context.Session["user"];
        if (user == null)
        {
            string code = context.Request["code"];
            string state = context.Request["state"];
            string randomString = (string)context.Session["randomString"];
            string WxToken = wxHelper.GetWxToken();
            string UserId = CookieHelper.GetCookieValueStatic("UserId");
            if (string.IsNullOrEmpty(UserId))
            {
                //randomString和state用来防止csrf攻击（跨站请求伪造攻击）
                if (string.IsNullOrEmpty(code) 
                    || string.IsNullOrEmpty(randomString)
                    || !string.Equals(state,randomString))
                {
                    GotoGetCode(context);
                    return "";
                }
                else
                {
                    if (string.IsNullOrEmpty(WxToken))
                    {
                        WxToken = wxHelper.GetWxTokenFromWx();
                        if (string.IsNullOrEmpty(WxToken) || WxToken.Contains("errcode"))
                        {
                            return WxToken;
                        }
                        
                    }
                    UserId = GetWxUserId(code, WxToken);
                    if (string.IsNullOrEmpty(UserId) || UserId.Contains("errcode"))
                    {
                        WxToken = wxHelper.GetWxTokenFromWx();
                        if (string.IsNullOrEmpty(WxToken) || WxToken.Contains("errcode"))
                        {
                            return WxToken;
                        }
                        UserId = GetWxUserId(code, WxToken);
                        if (string.IsNullOrEmpty(UserId) || UserId.Contains("errcode"))
                        {
                            return UserId;
                        }
                    }
                    CookieHelper.SetCookieStatic("UserId", UserId, DateTime.Now.AddDays(7.0));
                }
            }

            string res = GetUserInfo(WxToken, UserId, ref user);
            if (user == null)
            {
                return res;
            }
        }
        context.Session["user"] = user;
        User = user;
        return "success";
    }

    /// <summary>
    /// 应用支持推送文本、图片、视频、文件、图文等类型。
    /// 请求方式：POST（HTTPS）
    /// 请求地址： https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=ACCESS_TOKEN
    /// 返回示例：
    /// {
    //  "errcode" : 0,
    //  "errmsg" : "ok",
    //  "invaliduser" : "userid1|userid2", // 不区分大小写，返回的列表都统一转为小写
    //  "invalidparty" : "partyid1|partyid2",
    //  "invalidtag":"tagid1|tagid2"
    //}
    ///文本卡片消息
    ///请求示例：
    ///{
    //   "touser" : "UserID1|UserID2|UserID3",
    //   "toparty" : "PartyID1 | PartyID2",
    //   "totag" : "TagID1 | TagID2",
    //   "msgtype" : "textcard",
    //   "agentid" : 1,
    //   "textcard" : {
    //            "title" : "领奖通知",
    //            "description" : "<div class=\"gray\">2016年9月26日</div> <div class=\"normal\">恭喜你抽中iPhone 7一台，领奖码：xxxx</div><div class=\"highlight\">请于2016年10月10日前联系行政同事领取</div>",
    //            "url" : "URL",
    //            "btntxt":"更多"
    //   }
    //}
    ///参数	是否必须	说明
    //touser 否   成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为 @all，则向关注该企业应用的全部成员发送
    //toparty 否 部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
    //totag 否   标签ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为 @all时忽略本参数
    //msgtype 是   消息类型，此时固定为：textcard
    //agentid 是 企业应用的id，整型。可在应用的设置页面查看
    //title   是 标题，不超过128个字节，超过会自动截断
    //description 是 描述，不超过512个字节，超过会自动截断
    //url 是 点击后跳转的链接。
    //btntxt 否   按钮文字。 默认为“详情”， 不超过4个文字，超过自动截断。
    /// </summary>
    /// <param name="paraJson"></param>
    /// <returns></returns>
    public string SendWxMsg(string paraJson)
    {
        string token = wxHelper.GetWxToken();
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + token);
        
        return HttpHelper.Post(url,paraJson);
    }

    public string GetJsonAndSendWxMsg(string ids, string description, string url, string agentId)
    {
        JObject jObject = new JObject();
        jObject.Add("touser", ids);
        jObject.Add("msgtype", "textcard");
        jObject.Add("agentid", agentId);
        JObject innerJObject = new JObject();
        innerJObject.Add("title", "审批通知");
        innerJObject.Add("description", description);
        innerJObject.Add("url", url);
        jObject.Add("textcard", innerJObject);

        return SendWxMsg(jObject.ToString());
    }
}