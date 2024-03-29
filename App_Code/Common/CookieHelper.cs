﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;

/// <summary>
/// CookieHelper 的摘要说明
/// </summary>
public class CookieHelper
{
    private HttpContext Context = null;
    public CookieHelper(HttpContext context)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        Context = context;
    }



    /// <summary>  
    /// 清除指定Cookie  
    /// </summary>  
    /// <param name="cookiename">cookiename</param>  
    public void ClearCookie(string cookiename)
    {
        HttpCookie cookie = Context.Request.Cookies[cookiename];
        if (cookie != null)
        {
            cookie.Expires = DateTime.Now.AddYears(-3);
            Context.Response.Cookies.Add(cookie);
        }
    }
    /// <summary>  
    /// 获取指定Cookie值  
    /// </summary>  
    /// <param name="cookiename">cookiename</param>  
    /// <returns></returns>  
    public string GetCookieValue(string cookiename)
    {
        HttpCookie cookie = Context.Request.Cookies[cookiename];
        string str = string.Empty;
        if (cookie != null)
        {
            str = HttpUtility.UrlDecode(cookie.Value, Encoding.GetEncoding("UTF-8"));
        }
        return str;
    }
    /// <summary>  
    /// 添加一个Cookie（24小时过期）  
    /// </summary>  
    /// <param name="cookiename"></param>  
    /// <param name="cookievalue"></param>  
    public void SetCookie(string cookiename, string cookievalue)
    {
        SetCookie(cookiename, cookievalue, DateTime.Now.AddDays(1.0));
    }
    /// <summary>  
    /// 添加一个Cookie  
    /// </summary>  
    /// <param name="cookiename">cookie名</param>  
    /// <param name="cookievalue">cookie值</param>  
    /// <param name="expires">过期时间 DateTime</param>  
    public void SetCookie(string cookiename, string cookievalue, DateTime expires)
    {
        HttpCookie cookie = new HttpCookie(cookiename)
        {
            Value = HttpUtility.UrlEncode(cookievalue, Encoding.GetEncoding("UTF-8")),
            Expires = expires
        };
        Context.Response.Cookies.Add(cookie);
    }



    /// <summary>  
    /// 清除指定Cookie  
    /// </summary>  
    /// <param name="cookiename">cookiename</param>  
    public static void ClearCookieStatic(string cookiename)
    {
        HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
        if (cookie != null)
        {
            cookie.Expires = DateTime.Now.AddYears(-3);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
    /// <summary>  
    /// 获取指定Cookie值  
    /// </summary>  
    /// <param name="cookiename">cookiename</param>  
    /// <returns></returns>  
    public static string GetCookieValueStatic(string cookiename)
    {
        HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
        string str = string.Empty;
        if (cookie != null)
        {
            str = HttpUtility.UrlDecode(cookie.Value, Encoding.GetEncoding("UTF-8"));
        }
        return str;
    }
    /// <summary>  
    /// 添加一个Cookie（24小时过期）  
    /// </summary>  
    /// <param name="cookiename"></param>  
    /// <param name="cookievalue"></param>  
    public static void SetCookieStatic(string cookiename, string cookievalue)
    {
        SetCookieStatic(cookiename, cookievalue, DateTime.Now.AddDays(1.0));
    }
    /// <summary>  
    /// 添加一个Cookie  
    /// </summary>  
    /// <param name="cookiename">cookie名</param>  
    /// <param name="cookievalue">cookie值</param>  
    /// <param name="expires">过期时间 DateTime</param>  
    public static void SetCookieStatic(string cookiename, string cookievalue, DateTime expires)
    {
        HttpCookie cookie = new HttpCookie(cookiename)
        {
            Value = HttpUtility.UrlEncode(cookievalue, Encoding.GetEncoding("UTF-8")),
            Expires = expires
        };
        HttpContext.Current.Response.Cookies.Add(cookie);
    }
}