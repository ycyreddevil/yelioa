using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FormBuilder : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "saveForm")
            {
                Response.Write(saveForm());
            }
            else if (action == "updateForm")
            {
                Response.Write(updateForm());
            }
            else if (action == "hasUsedForm")
            {
                Response.Write(hasUsedForm());
            }
           
            Response.End();
        }
    }

    private string saveForm()
    {
        string formName = Request.Form["formName"];
        string formData = Request.Form["formData"];
        string parameterData = Request.Form["parameterData"];

        UserInfo userInfo = (UserInfo)Session["user"];

        return FormBuilderHelper.saveForm(formName, formData, parameterData, userInfo);
    }

    private string updateForm()
    {
        string formName = Request.Form["formName"];
        string formData = Request.Form["formData"];
        string parameterData = Request.Form["parameterData"];
        string id = Request.Form["id"];

        UserInfo userInfo = (UserInfo)Session["user"];

        return FormBuilderHelper.updateForm(formName, formData, parameterData, id);
    }

    private string hasUsedForm()
    {
        string formName = Request.Form["formName"];

        return FormBuilderHelper.hasUsedForm(formName);
    }
}