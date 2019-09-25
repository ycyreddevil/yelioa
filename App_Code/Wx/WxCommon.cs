using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// WxCommon 的摘要说明
/// </summary>
public class WxCommon
{
    public WxCommon(string name ,string Secret)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        AppName = name;
        AppSecret = Secret;
    }

    public WxCommon(string name, string Secret,string agentId,string redirectUri)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        AppName = name;
        AppSecret = Secret;
        AgentId = agentId;
        RedirectUri = redirectUri;
    }

    public string AgentId { get; set; }
    public string AppName { get; set; }
    public string AppSecret { get; set; }
    public string RedirectUri { get; set; }
    public UserInfo User { get; set; }

    private string Corpid = "wx9f620907462561ca";

    public string GetWxToken()
    {
        string WxToken = CookieHelper.GetCookieValueStatic(AppName + "WxToken");
        if (string.IsNullOrEmpty(WxToken))
        {
            WxToken = GetWxTokenFromWx();
        }
        return WxToken;
    }
    public string GetWxTokenFromWx()
    {
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}",
               Corpid, AppSecret);

        string res = HttpHelper.Get(url);
        Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(res);
        if (!dict.Keys.Contains("access_token"))
        {
            return res;
        }
        else
        {
            int expires_in = Convert.ToInt32(dict["expires_in"]);
            CookieHelper.SetCookieStatic(AppName + "WxToken", dict["access_token"].ToString(), DateTime.Now.AddSeconds(expires_in));
            return dict["access_token"].ToString();
        }

    }

    public string SendChatGroupTextMsg(string chatGroupId,string txt)
    {
        string token = GetWxToken();
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/appchat/send?access_token=" + token);
        JObject obj = new JObject();
        obj.Add("chatid", chatGroupId);
        obj.Add("msgtype", "text");
        JObject jText = new JObject();
        jText.Add("content", txt);
        obj.Add("text", jText);
        obj.Add("safe", 0);
        return HttpHelper.Post(url, obj.ToString());
    }

    public string SendChatGroupTextMsg(string chatGroupId, string msgtype, JObject message)
    {
        string token = GetWxToken();
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/appchat/send?access_token=" + token);
        JObject jObject = new JObject();
        jObject.Add("msgtype", msgtype);
        jObject.Add("chatid", chatGroupId);
        jObject.Add(msgtype, message);
        return HttpHelper.Post(url, jObject.ToString());
    }
    public string SendWxMsg(string paraJson)
    {
        string token = GetWxToken();
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + token);

        return HttpHelper.Post(url, paraJson);
    }

    public string SendWxMsg(string ids,string msgtype,JObject message)
    {
        JObject jObject = new JObject();
        jObject.Add("touser", ids);
        jObject.Add("msgtype", msgtype);
        jObject.Add("agentid", AgentId);
        jObject.Add(msgtype, message);
        return SendWxMsg(jObject.ToString());
    }

    public string SendWxMsg(string ids,string title,string description,string url)
    {
        JObject jObject = new JObject();
        jObject.Add("touser", ids);
        jObject.Add("msgtype", "textcard");
        jObject.Add("agentid", AgentId);
        JObject innerJObject = new JObject();
        innerJObject.Add("title", title);
        innerJObject.Add("description", description);
        innerJObject.Add("url", url);
        jObject.Add("textcard", innerJObject);

        return SendWxMsg(jObject.ToString());
    }

    private void GotoGetCode(HttpContext context)
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

    private string GetWxUserId(string code, string token)
    {
        string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}",
            token, code);
        string res = HttpHelper.Get(url);
        Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(res);
        if (!dict.Keys.Contains("UserId"))
        {
            return res;
        }
        else
            return dict["UserId"].ToString();
    }

    private string GetUserInfo(string token, string wechatUserId, ref UserInfo user)
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
        user = UserInfoManage.GetUserInfo(wechatUserId, ref msg);
        if (user != null)
            return "success";
        else
            return msg;
    }

    private string GetDepartmentPostInfo(ref List<DepartmentPost> departmentPost, UserInfo user)
    {
        string msg = "";
        departmentPost = UserInfoManage.GetDepartmentPostList(user);
        if (departmentPost != null && departmentPost.Count > 0)
            return "success";
        else
            return "";
    }

    private string CheckAndGetDepartmentInfo(HttpContext context, UserInfo user)
    {
        List<DepartmentPost> departmentPostList = (List<DepartmentPost>)context.Session["DepartmentPostList"];

        if (departmentPostList == null)
        {
            string res = GetDepartmentPostInfo(ref departmentPostList, user);
            if (departmentPostList == null)
            {
                return res;
            }
        }
        context.Session["DepartmentPostList"] = departmentPostList;
        return "success";
    }

    public string CheckAndGetUserInfo(HttpContext context)
    {
        UserInfo user = (UserInfo)context.Session["user"];

        if (user == null)
        {
            string code = context.Request["code"];
            string state = context.Request["state"];
            string randomString = (string)context.Session["randomString"];
            string WxToken = GetWxToken();
            string UserId = CookieHelper.GetCookieValueStatic("UserId");
            if (string.IsNullOrEmpty(UserId))
            {
                //randomString和state用来防止csrf攻击（跨站请求伪造攻击）
                if (string.IsNullOrEmpty(code)
                    || string.IsNullOrEmpty(randomString)
                    || !string.Equals(state, randomString))
                {
                    GotoGetCode(context);
                    return "";
                }
                else
                {
                    if (string.IsNullOrEmpty(WxToken))
                    {
                        WxToken = GetWxTokenFromWx();
                        if (string.IsNullOrEmpty(WxToken) || WxToken.Contains("errcode"))
                        {
                            return WxToken;
                        }

                    }
                    UserId = GetWxUserId(code, WxToken);
                    if (string.IsNullOrEmpty(UserId) || UserId.Contains("errcode"))
                    {
                        WxToken = GetWxTokenFromWx();
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

        return CheckAndGetDepartmentInfo(context, user);
    }

    private string GetWxTicket()
    {
        string Ticket = CookieHelper.GetCookieValueStatic(AppName + "jsapi_ticket");
        if (string.IsNullOrEmpty(Ticket))
        {
            Ticket = GetWxTicketFromWx();
        }
        return Ticket;
    }
    private string GetWxTicketFromWx()
    {
        string wxtoken = GetWxToken();
        string url= "https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token=" + wxtoken;
        string res = HttpHelper.Get(url);
        Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(res);
        if (!dict.Keys.Contains("ticket"))
        {
            return res;
        }
        else
        {
            int expires_in = Convert.ToInt32(dict["expires_in"]);
            CookieHelper.SetCookieStatic(AppName + "jsapi_ticket", dict["ticket"].ToString(), DateTime.Now.AddSeconds(expires_in));
            return dict["ticket"].ToString();
        }
    }
    public string GetSignature()
    {
        string noncestr = ValideCodeHelper.GetRandomCode(16);
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        string timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();      
        string jsapi_ticket = GetWxTicket();    
        if (jsapi_ticket.Contains("errcode"))
        {
            return jsapi_ticket;
        }
        else
        {
            string string1 = "jsapi_ticket="+ jsapi_ticket + "&noncestr=" + noncestr + "&timestamp=" + timestamp + "&url=" + RedirectUri;
            SHA1 sh = new SHA1CryptoServiceProvider();
            byte[] StrRes = Encoding.Default.GetBytes(string1);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder signature = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                signature.AppendFormat("{0:x2}", iByte);
            }

            JObject jb = new JObject();
            jb.Add("signature", signature.ToString());
            jb.Add("noncestr", noncestr);
            jb.Add("timestamp", timestamp);
            jb.Add("appid", Corpid);
            return jb.ToString();
        }
    }
}