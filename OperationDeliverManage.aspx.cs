using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OperationDeliverManage : System.Web.UI.Page
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
            else if ("updateReason".Equals(action))
            {
                Response.Write(updateReason());
            }
            else if ("reject".Equals(action))
            {
                Response.Write(Reject());
            }
            else if ("AddBranch".Equals(action))
            {
                Response.Write(AddBranch());
            }
            else if ("AddProduct".Equals(action))
            {
                Response.Write(AddProduct());
            }
            else if ("updateDeliverCode".Equals(action))
            {
                Response.Write(updateDeliverCode());
            }
            else if ("updateReceiptCode".Equals(action))
            {
                Response.Write(updateReceiptCode());
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
        DataTable dt = OperationDeliverManager.findByCond(starttm, endtm, applyName, hospital, product, isChecked);

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
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

        string msg = OperationDeliverManager.updateActualFee(list);

        JObject jObject = new JObject();
        jObject.Add("msg", msg);
        return jObject.ToString();
    }

    private string updateReason()
    {
        string Ids = Request.Form["codes"];
        List<string> IdList = JsonHelper.DeserializeJsonToObject<List<string>>(Ids);

        string reason = Request.Form["reason"];

        string msg = OperationDeliverManager.updateReason(IdList, reason);

        JObject jObject = new JObject();
        jObject.Add("msg", msg);
        return jObject.ToString();
    }

    private string updateDeliverCode()
    {
        string Ids = Request.Form["codes"];
        List<string> IdList = JsonHelper.DeserializeJsonToObject<List<string>>(Ids);

        string deliverCode = Request.Form["deliverCode"];

        string msg = OperationDeliverManager.updateDeliverCode(IdList, deliverCode);

        JObject jObject = new JObject();
        jObject.Add("msg", msg);
        return jObject.ToString();
    }

    private string updateReceiptCode()
    {
        string Ids = Request.Form["codes"];
        List<string> IdList = JsonHelper.DeserializeJsonToObject<List<string>>(Ids);

        string receiptCode = Request.Form["receiptCode"];

        string msg = OperationDeliverManager.updateReceiptCode(IdList, receiptCode);

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
        string msg = OperationDeliverManager.Reject(IdList, opinion, user.userName);

        JObject jObject = new JObject();
        jObject.Add("msg", msg);
        return jObject.ToString();
    }

    private string AddBranch()
    {
        string name = Request.Form["branchName"];
        string code = Request.Form["branchCode"];

        return OperationDeliverManager.AddBranch(name, code);
    }

    private string AddProduct()
    {
        string name = Request.Form["productName"];
        string code = Request.Form["productCode"];
        string spec = Request.Form["productSpec"];
        string unit = Request.Form["productUnit"];

        return OperationDeliverManager.AddProduct(name, code, spec, unit);
    }
}