using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ApplicationManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if(!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if(action=="initDatagrid")
            {
                Response.Write(initDatagrid());
            }
            else if(action=="sure")
            {
                Response.Write(sure());
            }
            Response.End();
        }
    }

    private string initDatagrid()
    {
        return ApplicationManageManage.initDatagrid();
    }
    private string sure()
    {
        string id = Request.Form["id"];
        string isValid = Request.Form["isValid"];
        string application = Request.Form["application"];
        return ApplicationManageManage.sure(id, application, isValid);
    }
}