using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;

public partial class mNetSalesApproval : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxNetSalesHelper wx = new WxNetSalesHelper("http://yelioa.top/mNetSalesApproval.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        String action = Request.Params["act"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getDatalist")
            {
                Response.Write(getDataList());
            }
            else if (action == "getProcessInfo")
            {
                Response.Write(getProcessInfo());
            }
            else if (action == "getProcessStage")
            {
                Response.Write(getProcessStage());
            }
            else if (action == "getDetails")
            {
                Response.Write(getDetails());
            }
            else if (action == "getApprovalRecord")
            {
                Response.Write(getApprovalRecord());
            }
            else if (action == "beginApproval")
            {
                Response.Write(beginApproval());
            }
            else if (action == "getSubDataList")
            {
                Response.Write(getSubDataList());
            }
            else if (action == "returnDocument")
            {
                Response.Write(returnDocument());
            }
            Response.End();
        }
    }

    /*加载数据列表*/
    private String getDataList()
    {
        UserInfo user = (UserInfo)Session["user"];
        String type = Request.Params["type"];
        DataTable dt = null;
        String res = "";

        // 查询我提交的
        if ("mine".Equals(type))
        {
            dt = NetSalesApprovalInfoManage.getListOfCommitBySelf(user.userName);
        }
        else // 查询待我审批的
        {
            dt = NetSalesApprovalInfoManage.getListOfCommitByOthers(user.userId.ToString());
        }

        if (dt != null)
        {
            dt = PinYinHelper.SortByPinYin(dt, "Hospital", "asc");
            ArrayList list = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("group", row["Hospital"].ToString());
                dict.Add("item", row["Product"].ToString());
                dict.Add("NetSalesNum", row["NetSalesNumber"].ToString());
                dict.Add("sales", user.userName);

                // 查询流向数据
                string flowNumStr = NetSalesInfoManage.getFlowNumOfReportSales(row["Hospital"].ToString(),
                    row["Product"].ToString(), user.userName);
                if (StringTools.IsInt(flowNumStr))
                {
                    dict.Add("flowNum", flowNumStr);
                }
                else//返回错误信息
                {
                    dict.Add("flowNum", "0");
                }
                list.Add(dict);
            }

            res = JsonHelper.SerializeObject(list);
        }
        return res;
    }

    /*加载流程信息*/
    private String getProcessInfo()
    {
        string res = "";
        string docCode = Request.Params["docCode"];

        // 先查询流程是否结束
        DataTable dt = NetSalesApprovalInfoManage.getDetails(docCode);
        if (dt != null && dt.Rows.Count > 0)
        {
            string state = dt.Rows[0]["State"].ToString();
            int level = Int32.Parse(dt.Rows[0]["Level"].ToString());

            if ("审批中".Equals(state))
            {
                dt = NetSalesApprovalInfoManage.getProcessInfo(docCode);
            }
            else
            {
                dt = NetSalesApprovalInfoManage.getProcessInfoOnFinalLevel(docCode, level);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                ArrayList list = new ArrayList();
                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("userName", row["userNames"].ToString());
                    list.Add(dict);
                }
                res = JsonHelper.SerializeObject(list);
            }
            else
            {
                return "加载流程信息失败，请联系管理员";
            }
        }
        else
        {
            return "加载流程信息失败，请联系管理员"; 
        }

        //DataTable dt = NetSalesApprovalInfoManage.getDetails(docCode);
        
        //UserInfo user = (UserInfo)Session["user"];
        //String res = "";
        //if (dt != null && dt.Rows.Count > 0)
        //{
        //    ArrayList list = new ArrayList();
        //    Dictionary<string, string> dict = new Dictionary<string, string>();

        //    string HospitalId = dt.Rows[0]["HospitalId"].ToString();
        //    string ProductId = dt.Rows[0]["ProductId"].ToString();
        //    string salesId = dt.Rows[0]["SalesId"].ToString();

        //    dt = NetSalesApprovalInfoManage.getProcessInfo(HospitalId, ProductId, salesId);

        //    if (dt != null)
        //    {
        //        string userName = dt.Rows[0]["userName"].ToString();
        //        dict.Add("level", "1");
        //        dict.Add("userName", userName);
        //        list.Add(dict);
        //        dt = NetSalesApprovalInfoManage.getProcessInfo();
        //        dict = new Dictionary<string, string>();
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            // TODO 同级别的人名用逗号拼接
        //            dict.Add("level", row["level"].ToString());
        //            Boolean flag = true;
        //            foreach (Dictionary<string, string> tempDict in list)
        //            {
        //                if (tempDict["level"].ToString() == row["level"].ToString())
        //                {
        //                    tempDict["userName"] = tempDict["userName"].ToString() + "," + row["userName"].ToString();
        //                    flag = !flag;
        //                    break;
        //                }
        //            }
        //            if (flag)
        //            {
        //                dict.Add("userName", row["userName"].ToString());
        //                dict.Add("postName", row["postName"].ToString());
        //                dict.Add("name", row["name"].ToString());
        //                list.Add(dict);
        //            }
        //        }
        //        res = JsonHelper.SerializeObject(list);
        //    }
        //    else
        //    {
        //        return "加载流程信息失败，请联系管理员";
        //    }
        //}
        //else
        //{
        //    return "加载流程信息失败，请联系管理员"; 
        //}
        return res;
    }

    // 加载进行到哪一步啦
    private String getProcessStage()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();

        // TODO 流程为空的时候需要判断
        String docCode = Request.Form["docCode"].ToString();
        DataTable dt = NetSalesApprovalInfoManage.getTotalProcessNum();
        String total_process = dt.Rows[0]["total_process"].ToString();
        dict.Add("total_process", total_process);

        DataTable dt2 = NetSalesApprovalInfoManage.getTotalRecordNum(docCode);
        String total_record = dt2.Rows[0]["total_record"].ToString();
        dict.Add("total_record", total_record);

        return JsonHelper.SerializeObject(dict);
    }

    // 加载单据详情
    private String getDetails()
    {
        String docCode = Request.Form["docCode"].ToString();
        DataTable dt = NetSalesApprovalInfoManage.getDetails(docCode);
        String res = "";
        if (dt != null)
        {
            ArrayList list = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("hospital", row["Hospital"].ToString());
                dict.Add("product", row["Product"].ToString());
                dict.Add("sales", row["Sales"].ToString());
                dict.Add("netSalesNum", row["NetSalesNumber"].ToString());
                dict.Add("CreateTime", row["CreateTime"].ToString());
                list.Add(dict);
            }
            res = JsonHelper.SerializeObject(list);
        }
        return res;
    }

    // 加载审批流程
    private String getApprovalRecord()
    {
        String docCode = Request.Form["docCode"].ToString();
        DataTable dt = NetSalesApprovalInfoManage.getApprovalRecord(docCode);
        String res = "";
        if (dt != null)
        {
            ArrayList list = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("time", row["time"].ToString());
                dict.Add("ApprovalOpinions", row["ApprovalOpinions"].ToString());
                dict.Add("ApprovalResult", row["ApprovalResult"].ToString());
                dict.Add("userName", row["userName"].ToString());
                list.Add(dict);
            }
            res = JsonHelper.SerializeObject(list);
        }
        return res;
    }

    // 开始审批
    private String beginApproval()
    {
        UserInfo user = (UserInfo)Session["user"];
        String approvalResult = Request.Form["ApprovalResult"].ToString();
        String approvalOpinions = Request.Form["ApprovalOpinions"].ToString();
        String docCodes = Request.Form["docCodes[]"] != null ? 
            Request.Form["docCodes[]"].ToString() : Request.Form["docCodes"].ToString();
        String[] docCodeArray = docCodes.Split(',');
        String netSalesNum = Request.Form["netSalesNum"].ToString();
        if (docCodeArray.Length < 1)
        {
            return "单据为空，无法进行审批";
        }
        string msg = "";
        for (int i = 0; i < docCodeArray.Length; i++)
        {
            String docCode = docCodeArray[i];
            String returnMsg = ApprovalFlowManage.ApproveDocument("net_sales", docCode, user, approvalResult, approvalOpinions,
                "http://yelioa.top/mNetSalesApproval.aspx?type=mine", "http://yelioa.top/mNetSalesApproval.aspx?type=his", "http://yelioa.top/mNetSalesUpload.aspx",
                "PyO4Il3bIxyuFquBAGrrr76GVcUbIN5NPpxNGAja-4U","netSales", "1000002");
            if (i != 0)
                msg += ",";
            if (returnMsg.Contains("出错"))
            {
                msg += ("单号为" + docCode + "的单据审批出错");
            }
            else if(returnMsg.Contains("结束"))
            {
                // 审批流程结束后，需要把纯销数据更新到flow_statistic表中
                string tempMsg = NetSalesInfoSrv.updateNetSalesAndStockAfterApproval(docCode);

                if ("更新成功".Equals(tempMsg))
                {
                    msg += ("单号为" + docCode + "的单据审批流程结束");
                }
                else
                {
                    msg += ("单号为" + docCode + "的单据审批流程结束失败！");
                }
            }
            else if ("当前用户无审批权限！".Equals(returnMsg))
            {
                msg += ("单号为" + docCode + "的单据已被其他人审批，请知悉！");
            }
            else
            {
                msg += ("单号为" + docCode + "的单据审批成功");
            }
        }
        return msg;
    }

    // 加载子表格数据
    private String getSubDataList()
    {
        String hospital = Request.Params["hospital"];
        String product = Request.Params["product"];
        UserInfo user = (UserInfo)Session["user"];
        String type = Request.Params["type"];
        DataTable dt = NetSalesApprovalInfoManage.getSubDataList(hospital, product, user.userName, user.userId.ToString(), type);
        String res = "";
        if (dt != null)
        {
            ArrayList list = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("docCode", row["DocCode"].ToString());
                dict.Add("createTime", row["CreateTime"].ToString());
                dict.Add("sales", row["Sales"].ToString());
                dict.Add("state", row["State"].ToString());
                dict.Add("netSalesNum", row["NetSalesNumber"].ToString());
                dict.Add("hospital", row["Hospital"].ToString());
                dict.Add("product", row["Product"].ToString());
                // 查询流向数据
                string flowNumStr = NetSalesInfoManage.getFlowNumOfReportSales(row["Hospital"].ToString(),
                    row["Product"].ToString(), user.userName);
                if (StringTools.IsInt(flowNumStr))
                {
                    dict.Add("flowNum", flowNumStr);
                }
                else//返回错误信息
                {
                    dict.Add("flowNum", "0");
                }
                list.Add(dict);
            }
            res = JsonHelper.SerializeObject(list);
        }
        return res;
    }

    // 单据退回
    public String returnDocument()
    {
        String docCode = Request.Params["docCode"];
        UserInfo user = (UserInfo)Session["user"];
        return ApprovalFlowManage.returnDocument("net_sales", docCode, user);
    }
}