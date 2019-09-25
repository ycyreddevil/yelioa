using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mDeliverApplyReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mSalesData",
                    "vvsnJs9JYf8AisLWOE4idJbdR1QGc7roIcUtN6P2Lhc",
                    "1000009",
                    "http://yelioa.top/mDeliverApplyReport.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        //if (Common.GetApplicationValid("mDeliverApplyReport.aspx") == "0")
        //{
        //    Response.Clear();
        //    Response.Write("<script language='javascript'>location.href='Default.aspx';</script>");
        //    Response.End();
        //    return;
        //}

        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "insertDeliverApplyReport")
            {
                Response.Write(insertDeliverApplyReport());
            }
            else if (action == "getProcessInfo")
            {
                Response.Write(getProcessInfo());
            }
            else if (action == "findDeliverType")
            {
                Response.Write(findDeliverType());
            }
            else
            {
                Response.Write(find(action));
            }
            Response.End();
        }
    }
    private string findDeliverType()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("value", Type.GetType("System.String"));
        dt.Columns.Add("target", Type.GetType("System.String"));
        dt.Rows.Add("0", "配送发货");
        dt.Rows.Add("1", "代理商发货");
        dt.Rows.Add("2", "收现发货");
        dt.Rows.Add("3", "直营发货");
        dt.Rows.Add("4", "借货发货");
        dt.Rows.Add("5", "赠品发货");
        dt.Rows.Add("6", "研发发货");
        dt.Rows.Add("7", "预演/投放发货");
        dt.Rows.Add("7", "合富发货");
        return JsonHelper.DataTable2Json(dt);
    }
    private string find(string action)
    {
        string q = Request.Form["q"];
        UserInfo user = (UserInfo)Session["user"];
        return DeliverApplyReportManage.find(action, q, user.userId.ToString());
    }
    private string insertDeliverApplyReport()
    {
        UserInfo user = (UserInfo)Session["user"];

        string deliverType = Request.Form["deliverType"];
        string hospitalName = Request.Form["hospitalName"];
        string productName = Request.Form["productName"];
        string agentName = Request.Form["agentName"];
        string spec = Request.Form["spec"];
        string unit = Request.Form["unit"];
        string applyNumber = Request.Form["applyNumber"];
        string stock = Request.Form["stock"];
        string netSales = Request.Form["netSales"];
        string period = Request.Form["period"];
        string remark = Request.Form["remark"];
        string approverIds = Request.Form["approverIds"];
        string informer = Request.Form["chooseInformerId"];
        string uploadFileUrls = Request.Form["uploadFileUrls"];
        string isStockReceiptTogether = Request.Form["isStockReceiptTogether"];
        string deliverAddress = Request.Form["deliverAddress"];
        string deliverName = Request.Form["deliverName"];
        string deliverPhone = Request.Form["deliverPhone"];

        List<string> approverList = JsonHelper.DeserializeJsonToList<string>(approverIds);
        List<string> informerList = JsonHelper.DeserializeJsonToList<string>(informer);
        List<string> uploadFileUrlList = JsonHelper.DeserializeJsonToList<string>(uploadFileUrls);

        WxUserInfo wxUserInfo = new WxUserInfo();

        string msg = DeliverApplyReportManage.insertDeliverApplyReport(deliverType, hospitalName,
            productName, agentName, spec, unit, applyNumber, remark, user, wxUserInfo.WechartUserIdToUserId(approverList),
            informerList, uploadFileUrlList, stock, netSales, period, isStockReceiptTogether, deliverAddress, deliverName, deliverPhone);

        JObject jObject = new JObject();
        jObject.Add("msg", msg);
        return jObject.ToString();
    }

    private string getProcessInfo()
    {
        UserInfo user = (UserInfo)Session["user"];

        WxUserInfo wxUserInfo = new WxUserInfo();

        //        DepartmentPost DepartmentPostList = getDepartmentPost();
        string departmentId = SqlHelper.Find("select departmentId from user_department_post where wechatUserId = '" + user.wechatUserId + "'").
            Tables[0].Rows[0][0].ToString();

        JArray approvers = new JArray();

        JObject self = new JObject();
        self.Add("name", user.userName);
        self.Add("userId", user.wechatUserId);

        approvers.Add(self);

        // 改成从数据库中把上下级给找出来
        string sql = string.Format("select * from user_department_post where wechatUserId = '{0}' and departmentId = '{1}'",
            user.wechatUserId, departmentId);

        string isHead = SqlHelper.Find(sql).Tables[0].Rows[0]["isHead"].ToString();

        string leader = "";
        string leaderSql = "";
        string leaderUserId = "";

        JObject approver1 = new JObject();

        // 如果不是领导 则要多审批一级 是领导 就直接审批一级
        if ("0".Equals(isHead))
        {
            leaderSql = string.Format("SELECT t1.*,t2.userName,t2.userId userId1, t3.`name` FROM `user_department_post`" +
            " t1 left join users t2 on t1.wechatUserId = t2.wechatUserId left join department t3 on t1.departmentId = t3.Id where departmentId = '{0}' and isHead = 1", departmentId);

            DataTable dt = SqlHelper.Find(leaderSql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                leader = dt.Rows[0]["userName"].ToString();
                leaderUserId = dt.Rows[0]["wechatUserId"].ToString();

                approver1.Add("name", leader);
                approver1.Add("userId", leaderUserId);

                approvers.Add(approver1);
            }
            else
            {
                string parentDepartmentId = SqlHelper.Find("select parentId from department where id = '" + departmentId + "'").Tables[0].Rows[0][0].ToString();

                leaderSql = string.Format("SELECT t1.*,t2.userName,t2.userId userId1, t3.`name` FROM `user_department_post`" +
                " t1 left join users t2 on t1.wechatUserId = t2.wechatUserId left join department t3 on t1.departmentId = t3.id where departmentId = '{0}' and isHead = 1", parentDepartmentId);

                DataTable dt1 = SqlHelper.Find(leaderSql).Tables[0];

                while (dt1.Rows.Count == 0)
                {
                    parentDepartmentId = SqlHelper.Find("select parentId from department where id = '" + parentDepartmentId + "'").Tables[0].Rows[0][0].ToString();
                    leaderSql = string.Format("SELECT t1.*,t2.userName,t2.userId userId1, t3.`name` FROM `user_department_post`" +
                                              " t1 left join users t2 on t1.wechatUserId = t2.wechatUserId left join department t3 on t1.departmentId = t3.id where departmentId = '{0}' and isHead = 1", parentDepartmentId);
                    dt1 = SqlHelper.Find(leaderSql).Tables[0];
                }

                leader = dt1.Rows[0]["userName"].ToString();
                leaderUserId = dt1.Rows[0]["wechatUserId"].ToString();

                approver1 = new JObject();
                approver1.Add("name", leader);
                approver1.Add("userId", leaderUserId);

                approvers.Add(approver1);
            }
        }
        else
        {
            string parentDepartmentId = SqlHelper.Find("select parentId from department where id = '" + departmentId + "'").Tables[0].Rows[0][0].ToString();

            leaderSql = string.Format("SELECT t1.*,t2.userName,t2.userId userId1, t3.`name` FROM `user_department_post`" +
            " t1 left join users t2 on t1.wechatUserId = t2.wechatUserId left join department t3 on t1.departmentId = t3.id where departmentId = '{0}' and isHead = 1", parentDepartmentId);

            DataTable dt1 = SqlHelper.Find(leaderSql).Tables[0];

            while (dt1.Rows.Count == 0)
            {
                parentDepartmentId = SqlHelper.Find("select parentId from department where id = '" + parentDepartmentId + "'").Tables[0].Rows[0][0].ToString();
                leaderSql = string.Format("SELECT t1.*,t2.userName,t2.userId userId1, t3.`name` FROM `user_department_post`" +
                                          " t1 left join users t2 on t1.wechatUserId = t2.wechatUserId left join department t3 on t1.departmentId = t3.id where departmentId = '{0}' and isHead = 1", parentDepartmentId);
                dt1 = SqlHelper.Find(leaderSql).Tables[0];
            }

            leader = dt1.Rows[0]["userName"].ToString();
            leaderUserId = dt1.Rows[0]["wechatUserId"].ToString();

            approver1 = new JObject();
            approver1.Add("name", leader);
            approver1.Add("userId", leaderUserId);

            approvers.Add(approver1);
        }

        return approvers.ToString();
    }

    private DepartmentPost getDepartmentPost()
    {
        return ((List<DepartmentPost>)Session["DepartmentPostList"])[0];
    }
}