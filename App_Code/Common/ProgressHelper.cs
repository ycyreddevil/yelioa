using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ProgressHelper 的摘要说明
/// </summary>
public class ProgressHelper
{
    //public ProgressHelper(HttpContext context)
    //{
    //    //
    //    // TODO: 在此处添加构造函数逻辑
    //    //
    //    Context = context;
    //}

    //private HttpContext Context = null;

    public void SetProgress(string auid,int progress, HttpContext context)
    {
        context.Session[auid + "Progress"] = progress;
    }

    public int GetProgress(string auid, HttpContext context)
    {
        string pg = context.Session[auid + "Progress"].ToString();
        if (string.IsNullOrEmpty(pg))
            return 0;
        else
            return Convert.ToInt32(pg);
    }
}