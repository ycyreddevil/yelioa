using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FormProcess : System.Web.UI.Page
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
            else if (action == "getTree")
            {
                Response.Write(getTree());
            }
            else if (action == "getDepartmentTree")
            {
                Response.Write(getDepartmentTree());
            }
            else if (action == "showFormFiled")
            {
                Response.Write(showFormFiled());
            }
            else if (action == "findRemoteData")
            {
                Response.Write(findRemoteData());
            }
            else if (action == "submitProcess")
            {
                Response.Write(submitProcess());
            }
            else if (action == "showProcessData")
            {
                Response.Write(showProcessData());
            }
            else if (action == "getProcessTree")
            {
                Response.Write(getProcessTree());
            }
            Response.End();
        }
    }
    private string initDatagrid()
    {
        return FormBuilderHelper.findAllForms();
    }
    private string getTree()
    {
        UserInfo user = (UserInfo)Session["user"];
        DataSet ds = UserInfoManage.getTree(user.companyId.ToString());
        if (ds == null)
        {
            return "F";
        }
        UserTreeHelper tree = new UserTreeHelper(ds.Tables[0]);
        string json = tree.GetJson();
        return json;
    }
    private string getDepartmentTree()
    {
        UserInfo user = (UserInfo)Session["user"];
        DataSet ds = UserInfoManage.getTree(user.companyId.ToString());
        if (ds == null)
        {
            return "F";
        }
        DepartmentTreeHelper tree = new DepartmentTreeHelper(ds.Tables[0]);
        string json = tree.GetJson();
        return json;
    }

    private string showFormFiled()
    {
        string formId = Request.Form["formId"];

        return WorkFlowHelper.showFormFiled(formId);
    }

    private string findRemoteData()
    {
        string type = Request.Form["type"];
        string value = Request.Form["value"];

        DataTable dt = null;

        if ("用户表".Equals(type))
        {
            dt = MobileReimburseManage.findInformer(value);
        }
        else if ("部门表".Equals(type))
        {
            dt = MobileReimburseManage.findFeeDepartment("");
        }
        else if ("产品表".Equals(type))
        {
            dt = MobileReimburseManage.findProduct(value, null, "");
        }
        else if ("费用明细表".Equals(type))
        {
            dt = MobileReimburseManage.findParentFeeDetail("","");
        }
        else if ("网点表".Equals(type))
        {
            dt = MobileReimburseManage.findBranch(value, null, "");
        }

        return JsonHelper.DataTable2Json(dt);
    }

    private string submitProcess()
    {
        string defaultProcessJson = Request.Form["DefaultProcessJson"];
        string conditionItem = Request.Form["ConditionItem"];
        string conditionJson = Request.Form["ConditionJson"];
        string formId = Request.Form["FormId"];
        string processName = Request.Form["ProcessName"];

        return WorkFlowHelper.submitProcess(defaultProcessJson, conditionItem, conditionJson, formId, processName);
    }

    private string showProcessData()
    {
        string formId = Request.Form["formId"];

        return WorkFlowHelper.getFormProcessOther(formId);
    }

    private string getProcessTree()
    {
        string formId = Request.Form["formId"];
        return WorkFlowHelper.GetTreeJson(formId);
    }
}