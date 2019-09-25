using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FormIndex : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "initDatagrid")
            {
                Response.Write(initDatagrid());
            }
            else if (action == "initUserTree")
            {
                Response.Write(InitUserTree());
            }
            else if (action == "findDetail")
            {
                Response.Write(findDetail());
            }
            else if (action == "updateRight")
            {
                Response.Write(updateRight());
            }
            else if (action == "getHasRight")
            {
                Response.Write(getHasRight());
            }
            else if (action == "updateFormTypeAndValid")
            {
                Response.Write(updateFormTypeAndValid());
            }

            Response.End();
        }
    }

    private string initDatagrid()
    {
        return FormBuilderHelper.findAllForms(); 
    }

    private string InitUserTree()
    {
        DataSet ds = UserInfoManage.getTree(GetUserInfo().companyId.ToString());
        string json = "";
        UserTreeHelper users;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            users = new UserTreeHelper(ds.Tables[0]);
            json = users.GetJson();
        }

        return json;
    }

    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }

    private string findDetail()
    {
        string id = Request.Form["id"].ToString();
        UserInfo userInfo = (UserInfo)Session["user"];
        return FormBuilderHelper.findDetail(id, userInfo.userId.ToString());
    }

    private string updateRight()
    {
        string visionJson = Request.Form["visionJson"].ToString();
        string id = Request.Form["id"];

        return FormBuilderHelper.updateRight(visionJson, id);
    }

    private string getHasRight()
    {
        string id = Request.Form["id"];
        return FormBuilderHelper.getHasRight(id); ;
    }

    private string updateFormTypeAndValid()
    {
        string id = Request.Form["id"];
        string type = Request.Form["type"];
        string valid = Request.Form["valid"];

        return FormBuilderHelper.updateFormTypeAndValid(id, type, valid);
    }
}