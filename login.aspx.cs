using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;


public partial class login : System.Web.UI.Page
{
   
    protected void Page_Load(object sender, EventArgs e)
    {
        string url= Request.Url.ToString();
        if (url.Contains("code") && url.Contains("state"))
        {
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];
            WxLogin(code,state);
        }
        else if(url.Contains("act=wxlogin"))
        {
            WxLogin act = new WxLogin();
            HttpContext context = HttpContext.Current;
            act.GotoGetCode(context);
        }
        string action = Request.Form["action"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "init")
            {
                //Init();
            }
            else if (action == "login")
            {
                Login();
            }
            else if (action == "check")
            {
                CheckCookie();
            }
            else if (action == "logout")
            {
                LogOut();
            }
            else if (action == "getValideCode")
            {
                GetValideCode();
            }
        //    else if (action == "gotogetcode")
        //    {
        //        string randomString = ValideCodeHelper.GetRandomCode(16);
        //        Session["randomString"] = randomString;
        //        string urlForGettingCode = string.Format("https://www.baidu.com?appid={0}&agentid={1}&redirect_uri={2}&state={3}"
        //, Corpid, AgentId, HttpUtility.UrlEncode(RedirectUri), randomString);
        //        Response.Redirect("http://www.baidu.com");
        //    }
            //     else if(action=="wxlogin")
            //     {
            //           WxLogin();
            //       }
            Response.End();
        }
        else
        {
            //if (UserInfoManage.IsLogined())
            //{
            //    Response.Redirect("~/index.aspx");
            //}
        }
    }

    private void GetValideCode()
    {
        string vc = ValideCodeHelper.staticCreateVerifyCode(4);
        //ImageButton1.ImageUrl = "~/ValideCode.aspx?code=" + vCode.Value;
        Response.Write(vc);
    }

    private void LogOut()
    {
        //删除Session
        Session.Clear();
        //删除Cookie
        CookieHelper.ClearCookieStatic("LoginToken");
        //CookieHelper.ClearCookie("UserId");
    }

    private void Login()
    {
        string userName = Request.Form["user"];
        string psw = Request.Form["psw"];
        string remerberMe = Request.Form["remerberMe"];
        UserInfo user = new UserInfo();
        user.userName = userName;
        user.passWord = psw;
        user.mobilePhone = userName;
        string token = "";
        string value = UserInfoManage.Login(ref user,ref token);

        CookieHelper cookie = new CookieHelper(Context);
        
        if (value == "登录成功")
        {
            Response.Write("T");
            Session["user"] = user;
            List<DepartmentPost> dpList = UserInfoManage.GetDepartmentPostList(user);
            Session["DepartmentPostList"] = dpList;
            //把用户名存入cookie
            cookie.ClearCookie("RememberMe");
            cookie.SetCookie("RememberMe",user.userName , DateTime.Now.AddDays(7));

            if (remerberMe== "true")//把token存入cookie
            {
                cookie.ClearCookie("LoginToken");
                cookie.SetCookie("LoginToken", token, DateTime.Now.AddDays(7));
            }
        }
        else
        {
            Response.Write(value);
        }
    }
    

    private void CheckCookie()
    {
        UserInfo user = (UserInfo)Session["user"];
        if(user != null)
        {
            Response.Write("T");
            return;
        }
            
        CookieHelper cookie = new CookieHelper(Context);
        string userName = cookie.GetCookieValue("RememberMe");
        if (!string.IsNullOrEmpty(userName))
        {
            string token = cookie.GetCookieValue("LoginToken");
            if (string.IsNullOrEmpty(token))
            {
                Response.Write(userName);
            }
            else
            {
                user = new UserInfo();
                user.userName = userName;
                string value = UserInfoManage.CookieLogin(ref user,token);
                if (value == "登录成功")
                {
                    Session["user"] = user;
                    List<DepartmentPost> dpList = UserInfoManage.GetDepartmentPostList(user);
                    Session["DepartmentPostList"] = dpList;
                    Response.Write("T");
                }
                else
                {
                    Response.Write(userName);
                }
            }            
        }
        else
            Response.Write("");
    }


    private void WxLogin(string code,string state)
    {
        HttpContext context = HttpContext.Current;
        WxLogin test = new WxLogin();
        string res=test.CheckAndGetUserInfo(context,code,state);
        if (res == "success")
            Response.Redirect("index.aspx", false);
        else
            Response.Write("<script> $.messager.alert('登录错误', res, 'error');<script/>");
    }
}
