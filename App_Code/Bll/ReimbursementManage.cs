using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using Newtonsoft.Json.Linq;

/// <summary>
/// ReimbursementManage 的摘要说明
/// </summary>
public class ReimbursementManage
{
    public ReimbursementManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetInfos(string name, string keyword, int year, int month)
    {
        return ReimbursementSrv.GetInfos(name, keyword, year, month);
    }

    public static DataSet GetDocumnetsInfosRelatedToMe(string userId, string keyword)
    {
        return ReimbursementSrv.GetDocumnetsInfosRelatedToMe(userId, keyword);
    }

    public static DataSet GetDocumnetsInfosToBeSubmitedByMe(string userName, string keyword)
    {
        return ReimbursementSrv.GetDocumnetsInfosToBeSubmitedByMe(userName, keyword);
    }

    public static DataSet GetDocumnetsInfosByCode(string DocCode, string name)
    {
        return ReimbursementSrv.GetDocumnetsInfosByCode(DocCode, name);
    }

    public static DataSet checkApprover(string code, string userId)
    {
        return ReimbursementSrv.checkApprover(userId, code);
    }

    public static DataTable getApprovalRecordCount(string code)
    {
        DataSet ds = ReimbursementSrv.getApprovalRecordCount(code);
        if (ds == null)
            return null;
        return ds.Tables[0];
    }

    public static DataTable getAttachmentByCode(string docCode)
    {
        DataSet ds = ReimbursementSrv.getAttachmentByCode(docCode);
        if (ds == null)
            return null;
        return ds.Tables[0];
    }

    public static DataTable getApproverByCode(string docCode)
    {
        DataSet ds = ReimbursementSrv.getApproverByCode(docCode);
        if (ds == null)
            return null;
        return ds.Tables[0];
    }

    public static string batchInsertReimburse(Dictionary<string, object> dict)
    {
        string msg = ReimbursementSrv.batchInsertReimburse(dict);

        if (msg.Contains("操作成功"))
        {
            return "成功";
        }
        else
        {
            return "失败";
        }
    }

    public static DataTable findByCond(string applystarttm, string applyendtm, string starttm, string endtm, string applyName, string depart, string fee_depart, string fee_detail
        , string account_status, string status,string sortName,string  sortOrder)
    {
        DataSet ds = ReimbursementSrv.findByCond(null,applystarttm, applyendtm, starttm, endtm, applyName, depart, fee_depart, fee_detail, account_status, status, sortName, sortOrder);

        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    public static DataTable findByCode(string code)
    {
        DataSet ds = ReimbursementSrv.findByCond(code,null,null, null, null, null, null, null, null, null, null, null, null);

        if (ds == null)
            return null;

        return ds.Tables[0];
    }

    /// <summary>
    /// 财务单据审批
    /// </summary>
    /// <param name="list">
    /// list元素为Dictionary<string,string>,其中key包含：
    /// 单据编号，code
    /// 审批人，approver
    /// 审批结果，ApprovalResult
    /// 审批意见,ApprovalOpinions
    /// 审批时间，approval_time
    /// 提交人, name
    /// </param>
    /// <returns>
    /// 
    /// </returns>
    public static string Approval(ArrayList list)
    {
        // 记录code 因为下一步不更新code会把code删除
        ArrayList codeList = new ArrayList();
        foreach (Dictionary<string, string> dict in list)
        {
            if (dict != null)
            {
                codeList.Add(dict["code"].ToString());
            }
            else
            {
                codeList.Add("");
            }
        }

        string res = ReimbursementSrv.Approval(list);
        string[] msgs = res.Split(';');
        DataTable dtUser = ReimbursementSrv.GetUserNameAndWxUserId();
        WxCommon wx = new WxCommon("mMobileReimbursement",
            "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
            "1000006",
            "");
        for (int i = 0; i < msgs.Length - 1; i++)
        {
            SqlExceRes sqlRes = new SqlExceRes(msgs[i]);
            if (sqlRes.Result == SqlExceRes.ResState.Success)
            {
                Dictionary<string, string> dict = (Dictionary<string, string>)list[i];
                string WxUserId = "";
                foreach (DataRow row in dtUser.Rows)
                {
                    if (row["userName"].ToString() == dict["name"])
                    {
                        WxUserId = row["wechatUserId"].ToString();
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(WxUserId))
                {
                    // 发送审批的消息给提交者
                    wx.SendWxMsg(WxUserId, "审批通知", "您编号为:" + codeList[i] + "的审批单据已被财务审批,审批人为:"
                        + dict["account_approver"] + "，结果为:" + dict["account_result"] + ",意见为:"+ dict["account_opinion"] +"，请知悉"
                        , "http://yelioa.top/mMySubmittedReimburse.aspx?docCode=" + codeList[i]);
                }

                // 审批拒绝的需要把关联的发票一起拒绝
                if ("拒绝" == dict["account_result"].ToString())
                {
                    string sql = string.Format("update yl_reimburse_detail set status = '拒绝', opinion = '关联的移动报销单据被拒绝' where code like '%{0}%'", codeList[i]);

                    // 删除关联差旅申请
                    sql += string.Format("delete from wf_form_差旅申请 where reimburseCode = '{0}';", codeList[i]);

                    // 删除关联借款单 并把借款单的金额还原
                    DataTable dt = SqlHelper.Find(string.Format("select * from yl_reimburse_loan where ReimburseCode = '{0}'", codeList[i])).Tables[0];

                    sql += string.Format("delete from yl_reimburse_loan where ReimburseCode = '{0}';", codeList[i]);

                    foreach (DataRow dr in dt.Rows)
                    {
                        decimal amount = Decimal.Parse(dr["amount"].ToString());
                        string tempCode = dr["docCode"].ToString();

                        sql += string.Format("update wf_form_借款单 set remainAmount = remainAmount + {0} where docCode = '{1}';", amount, tempCode);
                    }

                    SqlHelper.Exce(sql);
                }
            }
        }
        return res;
    }

    public static string updateActualFee(ArrayList list)
    {
        ArrayList codeList = new ArrayList();
        ArrayList actualFeeList = new ArrayList();

        foreach (Dictionary<string, string> dict in list)
        {
            if (dict != null)
            {
                codeList.Add(dict["code"].ToString());
                actualFeeList.Add(dict["actual_fee_amount"].ToString());
            }
            else
            {
                codeList.Add("");
                actualFeeList.Add("");
            }
        }

        string res = ReimbursementSrv.updateActualFee(list);

        string[] msgs = res.Split(';');
        //DataTable dtUser = ReimbursementSrv.GetUserNameAndWxUserId();
        //WxCommon wx = new WxCommon("mMobileReimbursement",
        //    "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
        //    "1000006",
        //    "");
        //for (int i = 0; i < msgs.Length - 1; i++)
        //{
        //    SqlExceRes sqlRes = new SqlExceRes(msgs[i]);
        //    if (sqlRes.Result == SqlExceRes.ResState.Success)
        //    {
        //        //Dictionary<string, string> dict = (Dictionary<string, string>)list[i];
        //        //string WxUserId = SqlHelper.Find("select wechatUserId from users where userName = '" + dict["name"] + "'").Tables[0].Rows[0][0].ToString();
        //        //foreach (DataRow row in dtUser.Rows)
        //        //{
        //        //    if (row["userName"].ToString() == dict["name"])
        //        //    {
        //        //        WxUserId = row["wechatUserId"].ToString();
        //        //        break;
        //        //    }
        //        //}
        //        //if (!string.IsNullOrEmpty(WxUserId))
        //        //{
        //        //    // 发送审批的消息给提交者
        //        //    wx.SendWxMsg(WxUserId, "审批通知", "您编号为:" + codeList[i] + "的审批单据已被财务进行金额复审,审批人为:"
        //        //        + dict["approver"] + "，财务已付款" + actualFeeList[i] + "元，请知悉"
        //        //        , "http://yelioa.top/mMySubmittedReimburse.aspx?docCode=" + codeList[i]);
        //        //}
        //    }
        //}

        return res;
    }

    public static string recordAccountEvent(DataTable dt)
    {
        return ReimbursementSrv.recordAccountEvent(dt);
    }

    public static string cancel(string code, UserInfo userInfo)
    {
        return MobileReimburseSrv.cancel(code, userInfo);
    }

    public static JObject IsOverBudget(string department, string feeDetail, double fee_amount,DateTime submitTime)
    {
        int nextMonth, nextYear;
        DateTime now = submitTime;
        if (now.Month == 12)
        {
            nextMonth = 1;
            nextYear = now.Year + 1;
        }
        else
        {
            nextMonth = now.Month + 1;
            nextYear = now.Year;
        }

        JObject res = new JObject();
        string sql = string.Format("select FindParentDepartment((select Id from department where name='{0}'));", department);

        string detail = feeDetail.Contains("-") ? feeDetail.Substring(feeDetail.LastIndexOf("-") + 1, feeDetail.Length - feeDetail.LastIndexOf("-") - 1) : feeDetail;
        DataSet ds = SqlHelper.Find(sql);

        if (detail.Contains("研发费用"))
        {
            var parentFeeDetail = feeDetail.Substring(0, feeDetail.LastIndexOf("-"));
            // 研发部项目预算
            var parentFeeDetailId = SqlHelper.Find(string.Format("select id from import_budget where feeDetail = '{0}'" +
                " and CreateTime between '{1}-{2}-1 00:00:00 ' and '{3}-{4}-1 00:00:00'"
                , parentFeeDetail, now.Year, now.Month, nextYear, nextMonth)).Tables[0].Rows[0][0].ToString();
            sql = string.Format("SELECT * FROM import_budget  WHERE DepartmentId IN ({0}) and CreateTime between '{1}-{2}-1 00:00:00 '" +
                " and '{3}-{4}-1 00:00:00' and FeeDetail='{5}' and ParentId = {6} ;", ds.Tables[0].Rows[0][0].ToString(), now.Year, now.Month,
                nextYear, nextMonth, detail, int.Parse(parentFeeDetailId));
        }
        else
        {
            sql = string.Format("SELECT * FROM import_budget  WHERE DepartmentId IN ({0}) and CreateTime between '{1}-{2}-1 00:00:00 '" +
                " and '{3}-{4}-1 00:00:00' and FeeDetail='{5}' ;", ds.Tables[0].Rows[0][0].ToString(), now.Year, now.Month, nextYear, nextMonth, detail);
        }

        DataSet secondDS = SqlHelper.Find(sql);
        if (secondDS.Tables[0].Rows.Count == 0)
        {
            res.Add("budget", 0);
            res.Add("hasApprove", 0);
            res.Add("apply", fee_amount);
        }
        else
        {
            sql = string.Format("SELECT ifnull(sum(fee_amount),0) FROM `yl_reimburse` WHERE fee_department LIKE " +
                        "CONCAT((SELECT NAME FROM department WHERE Id = {0} ), '%')" +
                        "AND fee_detail = '{1}' and status='已审批' and (account_result is null or account_result = '同意') and isOverBudget = 0 and approval_time between '{2}-{3}-1 00:00:00 ' and '{4}-{5}-1 00:00:00' ;", secondDS.Tables[0].Rows[0]["DepartmentId"].ToString(),
                         feeDetail, now.Year, now.Month, nextYear, nextMonth);
            sql+= string.Format("SELECT count(*) FROM `yl_reimburse` WHERE fee_department LIKE " +
                        "CONCAT((SELECT NAME FROM department WHERE Id = {0} ), '%')" +
                        "AND fee_detail = '{1}' and status='已审批' and (account_result is null or account_result = '同意') and isOverBudget = 0 and approval_time between '{2}-{3}-1 00:00:00 ' and '{4}-{5}-1 00:00:00' ;", secondDS.Tables[0].Rows[0]["DepartmentId"].ToString(),
                         feeDetail, now.Year, now.Month, nextYear, nextMonth);

            DataSet thirdDS = SqlHelper.Find(sql);
            double budget = Convert.ToDouble(secondDS.Tables[0].Rows[0]["Budget"].ToString());
            double cost = Convert.ToDouble(thirdDS.Tables[0].Rows[0][0].ToString());
            res.Add("budget", budget);
            res.Add("hasApprove", cost);
            res.Add("apply", fee_amount);
            res.Add("count", Convert.ToInt32(thirdDS.Tables[1].Rows[0][0].ToString()));
        }

        return res;
    }

    ///// <summary>
    ///// 导入预算，存入数据库
    ///// </summary>
    ///// <param name="dt"></param>
    ///// <returns></returns>
    //public static string uploadBudgetFile(DataTable dt)
    //{
    //    DataSet ds = MobileReimburseSrv.GetDepartmentListAndFeeDetailList();
    //    List<JObject> sqlArray = new List<JObject>();
    //    for(var i = 0; i < dt.Rows.Count; i++)
    //    {
    //        DataRow[] rows = ds.Tables[0].Select("Name='" + dt.Rows[i][1].ToString() + "'");
    //        string detailId = rows[0]["Id"].ToString();
    //        for(var j = 2; j < dt.Columns.Count; j++)
    //        {
    //            string departmentId = "";
    //            JObject sqlString = new JObject();
    //            double budget = currencyTransform(dt.Rows[i][j].ToString());
    //            if (budget == 0)//无预算，不存入数据库
    //            {
    //                break;
    //            }
    //            sqlString.Add("FeeDetailId", detailId);
    //            switch (j)
    //            {
    //                case 2:
    //                    departmentId = "296";//赣北战区
    //                    break;
    //                case 3:
    //                    departmentId = "301";//赣南战区
    //                    break;
    //                case 4:
    //                    departmentId = "300";//北中国
    //                    break;
    //                case 5:
    //                    departmentId = "298";//南中国
    //                    break;
    //                case 6:
    //                    departmentId = "292";//市场部
    //                    break;
    //                default:
    //                    break;
    //            }
    //            sqlString.Add("DepartmentId", departmentId);
    //            sqlString.Add("Budget", budget);
    //            sqlString.Add("CreateTime",DateTime.Now);

    //            sqlArray.Add(sqlString);
    //        }
    //    }

    //    return SqlHelper.Exce(SqlHelper.GetInsertString(sqlArray,"import_budget"));
    //} 
    ///// <summary>
    ///// 将预算万元单位转换为元
    ///// </summary>
    ///// <param name="number"></param>
    ///// <returns></returns>
    //private static double currencyTransform(string number)
    //{
    //    if(string.IsNullOrEmpty(number))
    //    {
    //        number = "0";
    //    }
    //    return Convert.ToDouble(number) * 10000;
    //}

}