﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// HttpClient 的摘要说明
/// </summary>
public class HttpClient : WebClient
{
    //
    // TODO: 在此处添加构造函数逻辑
    //

    // Cookie 容器
    private CookieContainer cookieContainer;

    /**/
    /// <summary>
    /// 创建一个新的 WebClient 实例。
    /// </summary>
    public HttpClient()
    {
        this.cookieContainer = new CookieContainer();
    }

    /**/
    /// <summary>
    /// 创建一个新的 WebClient 实例。
    /// </summary>
    /// <param name="cookie">Cookie 容器</param>
    public HttpClient(CookieContainer cookies)
    {
        this.cookieContainer = cookies;
    }

    /**/
    /// <summary>
    /// Cookie 容器
    /// </summary>
    public CookieContainer Cookies
    {
        get { return this.cookieContainer; }
        set { this.cookieContainer = value; }
    }

    /**/
    /// <summary>
    /// 返回带有 Cookie 的 HttpWebRequest。
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest request = base.GetWebRequest(address);
        if (request is HttpWebRequest)
        {
            HttpWebRequest httpRequest = request as HttpWebRequest;
            httpRequest.CookieContainer = cookieContainer;
        }
        return request;
    }
}