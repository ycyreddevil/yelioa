using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using Newtonsoft.Json.Linq;

/// <summary>
/// ReimbursementSrv 的摘要说明
/// </summary>
public class ReimbursementSrv
{
    public ReimbursementSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetInfos(string name, string keyword)
    {
        string sql = string.Format("select * from yl_reimburse where name='{0}'", name);
        if (!String.IsNullOrEmpty(keyword))
        {
            sql += " and (remark like '%" + keyword + "%' or fee_detail like '%" + keyword + "%' or name like '%" + keyword+ "%')";
        }
        return SqlHelper.Find(sql);
    }

    public static DataSet getApproverByCode(string code)
    {
        string sql = string.Format("select name,approver from yl_reimburse where id = {0}", code);
        return SqlHelper.Find(sql);
    }

    public static DataSet getApprovalRecordCount(string code)
    {
        string sql = string.Format("select t1.*,t2.userName Name from approval_record t1 left join users t2 on " +
            "t1.approverId = t2.userId where docCode = '{0}' and DocumentTableName = 'yl_reimburse' order by id ", code);
        return SqlHelper.Find(sql);
    }

    public static DataSet getAttachmentByCode(string code)
    {
        string sql = string.Format("select * from yl_reimburse_attachment where docCode = '{0}'", code);
        return SqlHelper.Find(sql);
    }

    public static DataSet checkApprover(string userId, string code)
    {
        string sql = string.Format("select t1.* from (select * from " +
            "yl_reimburse where STATUS <> '草稿') t1 inner join approval_process t2 on t1.id = t2.DocCode " +
            "and t1.`level` = t2.`Level` where " +
            "t2.ApproverId = '{0}' and t1.code = '{1}' and t1.status='审批中'", userId, code);
        return SqlHelper.Find(sql);
    }

    public static DataSet GetDocumnetsInfosByCode(string DocCode,string name)
    {
        //if (HasRightToGetDocumnetsInfosByCode(DocCode))
        //{
        //    string sql = string.Format("select * from yl_reimburse where code='{0}'", DocCode);
        //    return SqlHelper.Find(sql);
        //}
        //else
        //    return null;
        string sql = string.Format("select * from yl_reimburse where (approver like '%{0}%' or name like '%{0}%') and code='{1}'"
            , name,DocCode);
        return SqlHelper.Find(sql);
    }

    /// <summary>
    /// 获取与我有关的单据信息
    /// </summary>
    /// <returns></returns>
    public static DataSet GetDocumnetsInfosRelatedToMe(string userId, string keyword)
    {
        //DataSet res = null;
        //UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
        //WxUserInfo wxUser = new WxUserInfo();
        //JObject docUser = (JObject)wxUser.GetWxUserInfoJsonByUserId(user.wechatUserId);
        //if (docUser != null && docUser["isleader"].ToString() == "1")
        //{
        //    string departmentsName = wxUser.GetWxDepartmentNameByID(docUser["department"].ToString());
        //    string sql = string.Format("select * from yl_reimburse where department in {0}"
        //        + " or fee_department in {0}", departmentsName);
        //    res = SqlHelper.Find(sql);
        //}          

        //return res;

        //string sql = string.Format("select * from yl_reimburse where approver like '%{0}%' and name != '{0}'", name);
        string sql = string.Format("select t1.* from (select * from yl_reimburse where STATUS <> '草稿' and STATUS <> '已审批') t1 " +
            "inner join approval_process t2 on t1.id = t2.DocCode " +
            "and t1.`level`= t2.`Level` where " +
            "t2.ApproverId = '{0}' ", userId);
        if (!String.IsNullOrEmpty(keyword))
        {
            sql += " and (remark like '%" + keyword + "%' or fee_detail like '%" + keyword + "%' or name like '%" + keyword + "%') ";
        }
        sql += " order by t1.lmt";
        return SqlHelper.Find(sql);
    }

    public static DataSet GetDocumnetsInfosToBeSubmitedByMe(string userName, string keyword)
    {
        string sql = string.Format("SELECT * from yl_reimburse where (`status`='草稿' or account_result = '拒绝') and `name`='{0}' ", userName);
        if (!String.IsNullOrEmpty(keyword))
        {
            sql += " and (remark like '%" + keyword + "%' or fee_detail like '%" + keyword + "%' or name like '%" + keyword + "%') ";
        }
        sql += " ORDER BY apply_time desc";
        return SqlHelper.Find(sql);
    }

    public static bool HasRightToGetDocumnetsInfosByCode(string DocCode)
    {
        bool res = false;

        UserInfo user = (UserInfo)HttpContext.Current.Session["user"];

        string sql = string.Format("select name from yl_reimburse where code='{0}'", DocCode);
        object docName = SqlHelper.Scalar(sql);
        if (docName == null || docName.ToString() == user.userName)
            return true;


        WxUserInfo wxUser = new WxUserInfo();
        JObject docUser = (JObject)wxUser.GetWxUserInfoJsonByUserId(user.wechatUserId);
        if (docUser != null && docUser["isleader"].ToString() == "1")
        {
            string str = docUser["department"].ToString().Replace("[", "").Replace("]", "").Replace("\r\n", "");
            string[] departs = str.Split(',');
            List<string> list = new List<string>();
            foreach (string dep in departs)
            {
                JArray jarr = (JArray)wxUser.GetWxUserInfoJsonByDepartmentId(dep);
                foreach (JObject jobj in jarr)
                {
                    //list.Add(jobj["name"].ToString());
                    if (jobj["name"].ToString() == docName.ToString())
                    {
                        return true;
                    }
                }
            }
        }
        return res;
    }

    public static DataSet findByCond(string code,string applystarttm,string applyendtm, string starttm, string endtm, string applyName, string depart, string fee_depart, 
        string fee_detail, string account_status, string status)
    {
        string sql = "select * from yl_reimburse where 1=1 ";

        if (code != null)
        {
            sql += " and code = " + code;
        }
        if (applystarttm != null && applyendtm != null && applystarttm != "" && applyendtm != "")
        {
            sql += " and LMT between '" + applystarttm + "' and '"+ applyendtm + "'";
        }
        if (starttm != null && endtm != null && starttm != "" && endtm != "")
        {
            sql += " and approval_time between '" + starttm + "' and '" + endtm + "'";
        }
        if (applyName != null && !"".Equals(applyName))
        {
            sql += " and name like '%" + applyName + "%'";
        }
        if (depart != null && !"".Equals(depart))
        {
            sql += " and department like '%" + depart + "%'";
        }
        if (fee_depart != null && !"".Equals(fee_depart))
        {
            sql += " and fee_department like '%" + fee_depart + "%'";
        }
        if (fee_detail != null && !"".Equals(fee_detail))
        {
            sql += " and fee_detail like '%" + fee_detail + "%'";
        }
        if (account_status != null && !"".Equals(account_status) && !"null".Equals(account_status))
        {
            sql += " and account_result = '" + account_status + "'";
        }
        else if ("null".Equals(account_status))
        {
            sql += " and account_result is null";
        }
        if (status != null && !"".Equals(status))
        {
            sql += " and ApprovalResult = '" + status + "'";
        }

        return SqlHelper.Find(sql);
    }

    public static string batchInsertReimburse(Dictionary<string, object> dict)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string fileds = "";
        string values = "";
        foreach (string key in dict.Keys)
        {
            string value = dict[key].ToString();

            fileds += string.Format("{0},", (key));
            values += string.Format("'{0}',", value);
        }
        if (!string.IsNullOrEmpty(fileds))
        {
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            //fileds += string.Format("HospitalId, ProductId, SalesId");
            //values += string.Format("{0},{1},{2}", HospitalId, ProductId, SalesId);
        }
        else
        {
            return "";
        }

        string sql = string.Format("Insert into {0} ({1}) values ({2}) ", "yl_reimburse", fileds, values);
        return SqlHelper.Exce(sql);
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
    /// 提交人, 
    /// </param>
    /// <returns>
    /// 返回结果json消息:
    /// {
    ///     {
    ///         'Index':0,
    ///         'ErrorMsg':"success" 或 "相关错误信息"
    ///     }，{    
    ///         'Index':1,
    ///         'ErrorMsg':"success" 或 "相关错误信息"
    ///     }，{
    ///     。。。 。。。
    ///         'Index':n,
    ///         'ErrorMsg':"success" 或 "相关错误信息"
    ///     }
    /// }
    /// 其中n为list元素个数
    /// </returns>
    public static string Approval(ArrayList list)
    {
        List<string> conditionList = new List<string>();
        for (int i = 0; i < list.Count; i++)
        {
            string condition = " where code = ";
            Dictionary<string, string> dict = (Dictionary<string, string>)list[i];
            condition += dict["code"];
            dict.Remove("code");
            conditionList.Add(condition);
        }
        string sql = SqlHelper.GetUpdateString(list, "yl_reimburse", conditionList);
        return SqlHelper.Exce(sql);
    }

    public static DataTable GetUserNameAndWxUserId()
    {
        string sql = "select userName,wechatUserId from users";
        DataSet ds = SqlHelper.Find(sql);
        if (ds == null)
            return null;
        else
            return ds.Tables[0];
    }

    public static string updateActualFee(ArrayList list)
    {
        List<string> conditionList = new List<string>();
        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, string> dict = (Dictionary<string, string>)list[i];
            string condition = string.Format(" where code = '{0}'", dict["code"]);
            dict.Remove("code");
            
            conditionList.Add(condition);
        }
        string sql = SqlHelper.GetUpdateString(list, "yl_reimburse", conditionList);
        return SqlHelper.Exce(sql);
    }

    public static string recordAccountEvent(DataTable dt)
    {
        string sql = SqlHelper.GetInsertString(dt, "account_record");
        return SqlHelper.Exce(sql);
    }
}