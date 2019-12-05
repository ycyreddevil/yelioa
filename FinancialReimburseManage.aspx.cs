using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Collections.Specialized;

public partial class FinancialReimburseManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["action"];

        if (action == null)
        {
            action = Request.Params["action"];
        }

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if ("getFinancialApprovalData".Equals(action))
            {
                Response.Write(getFinancialApprovalData());
            }
            else if ("approval".Equals(action))
            {
                Response.Write(approval());
            }
            //else if ("exportExcel".Equals(action))
            //{
            //    Response.Write(exportExcel());
            //}
            else if ("updateActualFee".Equals(action))
            {
                Response.Write(updateActualFee());
            }
            else if (action == "upload")
            {
                Response.Write(uploadFile());
            }
            else if (action == "getDetailData")
            {
                Response.Write(getDetailData());
            }
            //else if (action == "uploadBudget")
            //{
            //    Response.Write(uploadBudgetFile());
            //}
            Response.End();
        }
    }

    //protected string exportExcel()
    //{
    //    string applystarttm = Request.Params["applystarttm"];
    //    string applyendtm = Request.Params["applyendtm"];
    //    string starttm = Request.Params["starttm"];
    //    string endtm = Request.Params["endtm"];
    //    string applyName = Request.Params["apply_name"];
    //    string depart = Request.Params["depart"];
    //    string fee_depart = Request.Params["fee_depart"];
    //    string fee_detail = Request.Params["fee_detail"];
    //    string account_status = Request.Form["account_status"];

    //    DataTable dt = ReimbursementManage.findByCond(applystarttm, applyendtm, starttm, endtm, applyName, depart, fee_depart, fee_detail, account_status, null);

    //    JObject resultJObject = new JObject();

    //    if (dt == null || dt.Columns.Count == 0)
    //    {
    //        resultJObject.Add("msg", "暂无数据导出");
    //    }
    //    else
    //    {
    //        string[] chineseHeaders = new string[] {"序号", "编号", "提交日期", "审批日期", "财务审批日期",
    //                "提交人", "部门", "费用归属部门", "产品", "费用明细", "金额", "实报金额", "状态", "审批人", "财务审批人", "抄送人", "备注", "审批意见", "审批结果"};
    //        ExcelHelperV2_0.ExportByWeb(dt, "财务审批单据信息", "财务审批单据信息.xlsx", chineseHeaders);

    //        resultJObject.Add("msg", "导出成功");
    //    }

    //    return resultJObject.ToString();
    //}
    //private string uploadBudgetFile()
    //{
    //    string res = "读取文件失败！";
    //    DataTable dt = ExcelHelperV2_0.Import(Request);
        
    //    if (dt != null)
    //    {
    //        res=ReimbursementManage.uploadBudgetFile(dt);
    //    }
    //    return res;
    //}

    private string uploadFile()
    {
        string res = "上传成功！";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        UserInfo user = (UserInfo)Session["user"];
        string sql = "";
        if (dt != null)
        {
            ArrayList list = new ArrayList();

            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();

                dict.Add("code", row[0].ToString());
                dict.Add("actual_fee_amount", row[1].ToString());
                //dict.Add("name", row[2].ToString());
                dict.Add("approver", user.userName.ToString());

                list.Add(dict);
            }
            string msg = ReimbursementManage.updateActualFee(list);
        }
        return res;
    }

    protected string getFinancialApprovalData()
    {
        string starttm = Request.Form["starttm"];
        string endtm = Request.Form["endtm"];
        string applystarttm = Request.Form["applystarttm"];
        string applyendtm = Request.Form["applyendtm"];
        string apply_name = Request.Form["apply_name"];
        string depart = Request.Form["depart"];
        string fee_depart = Request.Form["fee_depart"];
        string fee_detail = Request.Form["fee_detail"];
        string account_status = Request.Form["account_status"];
        string status = Request.Form["status"];
        string sortName = Request.Form["sortName"];
        string sortOrder = Request.Form["sortOrder"];

        DataTable dt = ReimbursementManage.findByCond(applystarttm, applyendtm,starttm, endtm, apply_name, depart, fee_depart, fee_detail, account_status, status
            ,sortName,sortOrder);

        if (dt == null)
            return null;

        //DataTable res = dt.Clone();
        //res.Columns["isPrepaid"].DataType = typeof(string);
        //res.Columns["isHasReceipt"].DataType = typeof(string);

        DataTable res = dt.Clone();

        for (int i=0;i<dt.Rows.Count;i++)
        {
            DataRow row = res.NewRow();

            row.ItemArray = dt.Rows[i].ItemArray;
            //foreach(DataColumn c in dt.Columns)
            //{
            //    if(c.ColumnName != "isPrepaid" && c.ColumnName != "isHasReceipt")
            //        row[c.ColumnName] = dt.Rows[i][c.ColumnName];
            //}
            //int val = 0;
            //if (dt.Rows[i]["isPrepaid"] == DBNull.Value)
            //    row["isPrepaid"] = "否"; 
            //else
            //{
            //    val = Convert.ToInt32(dt.Rows[i]["isPrepaid"]);
            //    if (val == 1)
            //    {
            //        row["isPrepaid"] = "是";
            //    }
            //    else
            //        row["isPrepaid"] = "否";
            //}
            //if (dt.Rows[i]["isHasReceipt"] == DBNull.Value)
            //    row["isHasReceipt"] = "是";
            //else
            //{
            //    val = Convert.ToInt32(dt.Rows[i]["isHasReceipt"]);
            //    if (val == 1)
            //    {
            //        row["isHasReceipt"] = "是";
            //    }
            //    else
            //        row["isHasReceipt"] = "否";
            //}

            row["remark"] = SqlHelper.DesDecrypt(dt.Rows[i]["remark"].ToString());

            //row["receiptAmount"] = SqlHelper.Find(string.Format("select sum(amount) from yl_reimburse_detail_relevance where reimburseCode = '{0}'", dt.Rows[i]["code"])).Tables[0].Rows[0][0].ToString();

            res.Rows.Add(row);
        }
        return JsonHelper.DataTable2Json(res);

        //NameValueCollection data = new NameValueCollection();
        //data.Add("starttm", starttm);
        //data.Add("endtm", endtm);
        //data.Add("apply_name", apply_name);
        //data.Add("depart", depart);
        //data.Add("fee_depart", fee_depart);
        //data.Add("fee_detail", fee_detail);
        //data.Add("account_status", account_status);
        //data.Add("status", status);
        //data.Add("act", "getFinancialApprovalData");
        //string res = HttpHelper.Post(YlTokenHelper.GetUrl() + "FinancialReimburseManage.aspx?Token="
        //    + YlTokenHelper.GetToken(), data);

        //return res;
    }

    protected string approval()
    {
        string codes = Request.Form["codes"];
        List<string> codeList = JsonHelper.DeserializeJsonToObject<List<string>>(codes);

        string names = Request.Form["names"];
        List<string> nameList = JsonHelper.DeserializeJsonToObject<List<string>>(names);

        string account_approval_results = Request.Form["account_approval_results"];
        List<string> accountApprovalResultList = JsonHelper.DeserializeJsonToObject<List<string>>(account_approval_results);

        string approvalOption = Request.Form["approvalOption"];
        string approvalResult = Request.Form["approvalResult"];

        UserInfo user = (UserInfo)Session["user"];
        string approvalName = user.userName;

        ArrayList list = new ArrayList();

        DataTable accountRecordDt = new DataTable();

        accountRecordDt.Columns.Add("name", Type.GetType("System.String"));
        accountRecordDt.Columns.Add("code", Type.GetType("System.String"));
        accountRecordDt.Columns.Add("event", Type.GetType("System.String"));
        accountRecordDt.Columns.Add("lmt", Type.GetType("System.String"));

        for (int i = 0; i < codeList.Count; i ++)
        {
            if (accountApprovalResultList[i] == null || "".Equals(accountApprovalResultList[i]))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("code", codeList[i]);
                dict.Add("account_approver", approvalName);
                dict.Add("account_result", approvalResult);
                dict.Add("account_opinion", approvalOption);
                dict.Add("name", nameList[i]);
                dict.Add("account_approval_time", DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString());

                list.Add(dict);

                DataRow accountRecordDr = accountRecordDt.NewRow();

                accountRecordDr["name"] = approvalName;
                accountRecordDr["code"] = codeList[i];
                accountRecordDr["event"] = "财务审批" + approvalResult;
                accountRecordDr["lmt"] = DateTime.Now.ToString();

                accountRecordDt.Rows.Add(accountRecordDr);
            }
        }

        JObject resultJObject = new JObject();

        if (list.Count == 0)
        {
            resultJObject.Add("msg", "所选单据都已审批，请重新选择");
        }
        else
        {
            resultJObject.Add("msg", ReimbursementManage.Approval(list));

            //财务审批的操作记录
            ReimbursementManage.recordAccountEvent(accountRecordDt);
        }

        return resultJObject.ToString();
    }

    protected string updateActualFee()
    {
        string codes = Request.Form["codes"];
        List<string> codeList = JsonHelper.DeserializeJsonToObject<List<string>>(codes);

        string names = Request.Form["names"];
        List<string> nameList = JsonHelper.DeserializeJsonToObject<List<string>>(names);

        string fees = Request.Form["actual_fee_amount"];

        string originFees = Request.Form["originFees"];

        UserInfo user = (UserInfo)Session["user"];
        string approvalName = user.userName;

        List<string> originFeeList = new List<string>();
        if (originFees != null)
        {
            originFeeList = JsonHelper.DeserializeJsonToObject<List<string>>(originFees);
        }

        ArrayList list = new ArrayList();

        for (int i = 0; i < codeList.Count; i++)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("code", codeList[i]);

            if (originFees != null)
            {
                dict.Add("actual_fee_amount", originFeeList[i]);
            }
            else
            {
                dict.Add("actual_fee_amount", fees);
            }

            dict.Add("name", nameList[i]);
            dict.Add("approver", approvalName);

            list.Add(dict);
        }

        string msg = ReimbursementManage.updateActualFee(list);

        JObject resultJObject = new JObject();

        resultJObject.Add("msg", msg);

        return resultJObject.ToString();
    }
    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }

    private string getDetailData()
    {
        string code = Request.Form["code"];

        DataTable dt = SqlHelper.Find(string.Format("select * from yl_reimburse_detail where code like '%{0}%' and status = '同意'", code)).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }
}