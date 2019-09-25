using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Privilege 的摘要说明
/// </summary>
public class Privilege
{
    public Privilege()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static Boolean checkPrivilege(UserInfo user)
    {
        PostHelper postHelper = new PostHelper(user);

        if (("吕正和".Equals(user.userName) || "彭春燕".Equals(user.userName) || "张代俊".Equals(user.userName) 
            || "陈永洪".Equals(user.userName)) || postHelper.ContainPost(201, 74)) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}