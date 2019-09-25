using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

/// <summary>
/// YlTokenHelper 的摘要说明
/// </summary>
public class YlTokenHelper
{
    public YlTokenHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string  GetUrl()
    {
        return "http://yelioa.51vip.biz:13319/";
        //return "http://localhost:50219/";
    }

    public static string GetToken()
    {
        CookieHelper cookie = new CookieHelper(HttpContext.Current);
        string token = cookie.GetCookieValue("LoginToken");
        if (string.IsNullOrEmpty(token))
            token = token;
        return token;
    }
}