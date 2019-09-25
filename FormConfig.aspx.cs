using System;
using System.Data;

public partial class FormConfig : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Params["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if ("getCustomizedForm".Equals(action))
            {
                Response.Write(getCustomizedForm());
            }
            else if ("previewForm".Equals(action))
            {
                Response.Write(previewForm());
            }
            Response.End();
        }
    }

    private string getCustomizedForm()
    {
        DataTable dt = CustomizedFormManage.getCustomizedForm();

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string previewForm()
    {
        string formName = Request.Params["formName"];
        DataTable dt = CustomizedFormManage.getFormData(formName);
        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }
}