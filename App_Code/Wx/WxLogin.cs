using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// WxTest 的摘要说明
/// </summary>
public class WxLogin
{
    public WxLogin()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    private string Corpid = "wx9f620907462561ca";
    private string AppSecret = "szkf9xLuUZBI8hVEmtrVSS-A4EZSDhFbvFi_-niJOxI";
    private string AgentId = "1000007";
    private string AppName = "user";
    private string RedirectUri = "Http://yelioa.top/login.aspx";
    public UserInfo User { get; set; }

    public string GetWxToken()
    {
        string WxToken = CookieHelper.GetCookieValueStatic(AppName + "WxToken");
        if (string.IsNullOrEmpty(WxToken))
        {
            WxToken = GetWxTokenFromWx();
        }
        return WxToken;
    }
    //   public string GetState(HttpContext context)
    //  {
    //       string randomString = ValideCodeHelper.GetRandomCode(16);
    //      context.Session["randomString"] = randomString;
    //       return randomString;
    //   }

    public void GotoGetCode(HttpContext context)
    {
        string randomString = ValideCodeHelper.GetRandomCode(16);
        context.Session["randomString"] = randomString;
        string urlForGettingCode = string.Format("https://open.work.weixin.qq.com/wwopen/sso/qrConnect?appid={0}&agentid={1}&redirect_uri={2}&state={3}" 
, Corpid, AgentId, HttpUtility.UrlEncode(RedirectUri),randomString);
        context.Response.Redirect(urlForGettingCode, false);
    }

    private string GetWxTokenFromWx()
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
            CookieHelper.SetCookieStatic(AppSecret + "WxToken", dict["access_token"].ToString(), DateTime.Now.AddSeconds(expires_in));
            return dict["access_token"].ToString();
        }

    }

    public string GetUserInfo(string wechatUserId, ref UserInfo user)
    {
        string msg = "";
        user = UserInfoManage.GetUserInfo(wechatUserId, ref msg);
        if (user != null)
            return "success";
        else
            return msg;
    }

    public string CheckAndGetUserInfo(HttpContext context,string code,string state)
    {
        UserInfo user = new UserInfo();
        //user = null;
        //GotoGetCode(context);
        //if (user == null)
        //{
            string randomString = (string)context.Session["randomString"];
            string WxToken = GetWxToken();
            string UserId = "";
            //         string UserId = CookieHelper.GetCookieValueStatic("UserId");
            //         if (string.IsNullOrEmpty(UserId))
            //         {
            //randomString和state用来防止csrf攻击（跨站请求伪造攻击）
            if (string.IsNullOrEmpty(code)
                || string.IsNullOrEmpty(randomString)
                || !string.Equals(state, randomString)
            )
            {
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
                   
            string res = GetUserInfo(UserId, ref user);
                if (user == null)
                {
                    return res;
                }
            CookieHelper.SetCookieStatic("RememberMe", user.userName, DateTime.Now.AddDays(7.0));
            CookieHelper.SetCookieStatic("LoginToken", state, DateTime.Now.AddDays(7.0));
            string sql = "delete from login_info where UserId = " + user.userId + "\r\n;";
            JObject obj = new JObject();
            obj.Add("UserId", user.userId);
            obj.Add("Token", state);
            obj.Add("LoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            string ip = getIp();
            obj.Add("IpAddress", ip);
            sql += SqlHelper.GetInsertString(obj, "login_info");
            SqlHelper.Exce(sql);
        }
        
            context.Session["user"] = user;
            List<DepartmentPost> dpList = UserInfoManage.GetDepartmentPostList(user);
            context.Session["DepartmentPostList"] = dpList;
            User = user;
            return "success";
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



    private static string getIp()
    {
        if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
        else
            return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
    }
}