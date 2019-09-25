using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// PostHelper 的摘要说明
/// </summary>
public class PostHelper
{
    public PostHelper(UserInfo user)
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
        if (user == null)
            return;
        dpList = (List<DepartmentPost>)HttpContext.Current.Session["DepartmentPostList"];
        if(dpList==null)
            dpList = UserInfoManage.GetDepartmentPostList(user);
        HttpContext.Current.Session["DepartmentPostList"] = dpList;
    }

    public List<DepartmentPost> dpList { get; set; }

    public bool ContainPost(int departmentID,int postId)
    {
        bool res = false;
        foreach(DepartmentPost dp in dpList)
        {
            if(dp.departmentId == departmentID && dp.postId == postId)
            {
                res = true;
                break;
            }
        }
        return res;
    }

}