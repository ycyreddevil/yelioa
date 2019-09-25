using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OperationDemandManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["action"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if ("getData".Equals(action))
            {
                Response.Write(getData());
            }
            else if ("updateActualFee".Equals(action))
            {
                Response.Write(updateActualFee());
            }
            else if ("reject".Equals(action))
            {
                Response.Write(Reject());
            }
            Response.End();
        }
    }

    private string getData()
    {
        string starttm = Request.Form["starttm"];
        string endtm = Request.Form["endtm"];
        string applyName = Request.Form["apply_name"];
        string hospital = Request.Form["hospital"];
        string product = Request.Form["product"];
        string isChecked = Request.Form["isChecked"];

        return DemandApplyReportManage.GetDocument(starttm, endtm, applyName, hospital, product, isChecked);
    }
    private string updateActualFee()
    {
        string Ids = Request.Form["codes"];
        List<string> IdList = JsonHelper.DeserializeJsonToObject<List<string>>(Ids);

        string actual_fee_amounts = Request.Form["actual_fee_amount"];
        List<string> actual_fee_amountsList = JsonHelper.DeserializeJsonToObject<List<string>>(actual_fee_amounts);

        string submitters = Request.Form["submitters"];
        List<string> submittersList = JsonHelper.DeserializeJsonToObject<List<string>>(submitters);

        UserInfo userInfo = (UserInfo)Session["user"];

        ArrayList list = new ArrayList();

        for (int i = 0; i < IdList.Count; i++)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("Id", IdList[i]);
            dict.Add("ApprovalNumber", actual_fee_amountsList[i]);
            dict.Add("ApprovalName", submittersList[i]);

            list.Add(dict);
        }

        string msg = DemandApplyReportManage.updateActualFee(list);

        JObject jObject = new JObject();
        jObject.Add("msg", msg);
        return jObject.ToString();
    }

    private string Reject()
    {
        UserInfo user = (UserInfo)Session["user"];
        string opinion = Request.Form["opinion"];
        string Ids = Request.Form["Ids"];
        List<string> IdList = JsonHelper.DeserializeJsonToObject<List<string>>(Ids);
        string msg = DemandApplyReportManage.Reject(IdList, opinion, user.userName);

        JObject jObject = new JObject();
        jObject.Add("msg", msg);
        return jObject.ToString();
    }
}