using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Authorization : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if(!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if(action== "initDatagrid")
            {
                InitDatagrid();
            }
            else if(action=="getHasRight")
            {
                GetHasRight();
            }
            else if(action=="initUserTree")
            {
                InitUserTree();
            }
            else if(action=="updateRight")
            {
                UpdateRight();
            }
            else if(action=="getSearched")
            {
                GetSearched();
            }
            Response.End();
        }
    }



    private void InitDatagrid()
    {
        Response.Write(RightManage.InitDatagrid());
    }


    private void GetHasRight()
    {
        string id = Request.Form["id"];
        Response.Write(RightManage.GetHasRight(id));
    }

    private void InitUserTree()
    {
        DataSet ds = UserInfoManage.getTree(GetUserInfo().companyId.ToString());
        string json = "";
        UserTreeHelper users;
        if (ds != null&&ds.Tables[0].Rows.Count>0)
        {
             users= new UserTreeHelper(ds.Tables[0]);
             json = users.GetJson();
        }
        Response.Write(json);       
    }


  
    private void UpdateRight()
    {
        string id =Request.Form["id"];
        string departmentIds = Request.Form["departmentIds"];
        string UserIds = Request.Form["UserIds"];
        string res = RightManage.UpdateRight(id, departmentIds, UserIds);
        Response.Write(res);
    }

    private void GetSearched()
    {
        string searchStr = Request.Form["searchStr"];
        Response.Write(RightManage.GetSearched(searchStr));
    }
    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }

}