using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MyInfo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "modifyPsw")
            {
                Response.Write(modifyPsw());
            }
            Response.End();
        }
    }

    private string modifyPsw()
    {
        UserInfo user = (UserInfo)Session["user"];
        string oldPsw = Request.Form["oldPsw"];
        string newPsw = Request.Form["newPsw"];

        user.passWord = oldPsw;
        string token = "";
        string res = UserInfoManage.Login(ref user,ref token);//"登录成功"
        if(res == "登录成功")
        {
            user.passWord = newPsw;
            res = UserInfoManage.ModifyPassword(ref user);
            //Session["user"] = user;
            if(res.Contains("操作成功"))
            {
                Session["user"] = user;
                List<DepartmentPost> dpList = UserInfoManage.GetDepartmentPostList(user);
                Session["DepartmentPostList"] = dpList;
                res = "密码修改成功！";
            }
        }
        return res;
    }
}