using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

public partial class index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "checkLogin")
            {
                Response.Write(checkLogin());
            }
            if (action == "getMenu")
            {
                //Response.ContentType = "application/json";
                Response.Write(GetMenu());
            }
            Response.End();
        }            
    }

    private string checkLogin()
    {
        UserInfo user = (UserInfo)Session["user"];
        if (user == null)
        {
            if(CheckCookie()== "T")
            {
                user = (UserInfo)Session["user"];
                //user.passWord = "888888";
                return (JsonHelper.SerializeObject(user));
            }
        }
        else
        {
            //user.passWord = "888888";
            return (JsonHelper.SerializeObject(user));
        }
        return "F";
    }

    private string CheckCookie()
    {
        CookieHelper cookie = new CookieHelper(Context);
        string userName = cookie.GetCookieValue("RememberMe");
        if (!string.IsNullOrEmpty(userName))
        {
            string token = cookie.GetCookieValue("LoginToken");
            if (string.IsNullOrEmpty(token))
            {
                return (userName);
            }
            else
            {
                UserInfo user = new UserInfo();
                user.userName = userName;
                string value = UserInfoManage.CookieLogin(ref user, token);
                if (value == "登录成功")
                {
                    Session["user"] = user;
                    List<DepartmentPost> dpList = UserInfoManage.GetDepartmentPostList(user);
                    Session["DepartmentPostList"] = dpList;
                    return ("T");
                }
                else
                {
                    return (userName);
                }
            }
        }
        else
            return "";
    }





    private string GetMenu()
    {
        UserInfo user = (UserInfo)Session["user"];
        //NameValueCollection data = new NameValueCollection();
        //data.Add("userId", user.wechatUserId);
        //data.Add("act", "getLeftMenu");
        //string res = HttpHelper.Post(YlTokenHelper.GetUrl() + "index.aspx?Token=" + YlTokenHelper.GetToken(), data);
        //return res;
        LeftMenu menu = null;
        if (user != null)
        {
            menu = new LeftMenu(user.wechatUserId);
            return menu.GetJson();
        }
        else
        {
            return null;    
        }
        
    }
}