using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;

/// <summary>
/// CostSharingDeleteManage 的摘要说明
/// </summary>
public class CostSharingDeleteManage
{
    public CostSharingDeleteManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    /// <summary>
    /// 获取单据所有项，判断是提交还是审批，或者查询审批/提交记录
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <param name="docId">new_cost_sharing的单据ID</param>
    /// <param name="docCode">cost_sharing_record的单据Code</param>
    /// <param name="IsRecord">是否只是记录（true，false）</param>
    /// <returns></returns>
    public static string GetList(string userId,string docId,string docCode,string IsRecord)
    {
        JObject res = new JObject();
        if(string.IsNullOrEmpty(userId)||string.IsNullOrEmpty(docId) || string.IsNullOrEmpty(docCode) || string.IsNullOrEmpty(IsRecord))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else if(docCode=="-1")
        {
            res = GetSubmitDoc(docId);
        }
        else if(IsRecord=="false")
        {
            res = GetApproveDoc(docCode, userId);
        }
        else
        {
            res = GetRecord(docCode, userId);
        }
        return res.ToString();
    }

    public static string SubmitOrApprove(string  userId,string docCode,string docId,JArray jArray)
    {
        JObject res = new JObject();
        if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(docCode) ||jArray==null||jArray.Count==0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else if (docCode == "-1")
        {
            res = SubmitDoc(userId,docId,jArray,10);
        }
        else
        {
            res = Approve(userId,jArray,docCode);
        }
        return res.ToString();
    }


    /*以下是获取数据的相关方法*/


    /// <summary>
    /// 获取提交时单据所有项
    /// </summary>
    /// <param name="docId">new_cost_sharing的单据ID</param>
    /// <returns></returns>
    private static JObject GetSubmitDoc(string docId)
    {

        JObject res = new JObject();
        string ErrMsg = "";
        DataSet ds = CostSharingDeleteSrv.GetList(docId, ref ErrMsg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", ErrMsg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "不存在该单据");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("data", DataTableToJArray(ds.Tables[0],ds.Tables[1]));
            res.Add("approver",new JArray());
            res.Add("IsRecord", "false");
        }
        return res;
    }
    /// <summary>
    /// 提交单据时所有项由datatable转为jarray
    /// </summary>
    /// <param name="docDT">new_cost_sharing的一条单据</param>
    /// <param name="fieldDT">提交单据所有项（cost_sharing_field_level中取出）</param>
    /// <returns></returns>
    private static JArray DataTableToJArray(DataTable docDT,DataTable fieldDT)
    {
        JArray jarray = new JArray();
        foreach(DataRow row in fieldDT.Rows)
        {
            if(docDT.Columns.Contains( row["RelativeFieldName"].ToString()))
            {
                jarray.Add(CreateJobject(row["FieldName"].ToString(), docDT.Rows[0][row["RelativeFieldName"].ToString()].ToString(), "disable"));
            }
            else
            {
                JObject dataJson = JObject.Parse(docDT.Rows[0]["DataJson"].ToString());
                if(dataJson.Property(row["FieldName"].ToString()) != null)
                {
                    jarray.Add(CreateJobject(row["FieldName"].ToString(), dataJson[row["FieldName"].ToString()].ToString(), row["FieldType"].ToString()));
                }
                else
                {
                    jarray.Add(CreateJobject(row["FieldName"].ToString(), "", row["FieldType"].ToString()));
                }
            }
        }
        return jarray;
    }
    /// <summary>
    /// 审批或查记录时单据所有项由datatable转为jarray
    /// </summary>
    /// <param name="docDT">来自于cost_sharing_detail左关联cost_sharing_field_level</param>
    /// <returns></returns>
    private static JArray DataTableToJArray(DataTable docDT)
    {
        JArray jarray = new JArray();
        foreach (DataRow row in docDT.Rows)
        {
            jarray.Add(CreateJobject(row["FieldName"].ToString(), row["NewValue"].ToString(), row["FieldType"].ToString()));
        }
        return jarray;
    }

    /// <summary>
    /// 获取待审批的单据所有项
    /// </summary>
    /// <param name="docCode">单据Code，cost_sharing_record中Code</param>
    /// <param name="userId">人员ID</param>
    /// <returns></returns>
    private static JObject GetApproveDoc(string docCode,string userId)
    {
        JObject res = new JObject();
        string ErrMsg = "";
        DataSet ds = CostSharingDeleteSrv.GetDoc(docCode, ref ErrMsg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", ErrMsg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "不存在该单据");
        }
        else if(ds.Tables[0].Rows[0]["ApproverUserId"].ToString()!=userId)
        {
            res.Add("ErrCode", 4);
            res.Add("ErrMsg", "无审批权限或该单据已被审批");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("data", DataTableToJArray(ds.Tables[1]));
            res.Add("approver", GetApproverList(ds.Tables[2]));
            res.Add("IsRecord", "false");
        }
        return res;
    }
    /// <summary>
    /// 获取审批记录
    /// </summary>
    /// <param name="docCode">单据Code，cost_sharing_record中Code</param>
    /// <param name="userId">人员ID</param>
    /// <returns></returns>
    private static JObject GetRecord(string docCode, string userId)
    {
        JObject res = new JObject();
        string ErrMsg = "";
        DataSet ds = CostSharingDeleteSrv.GetRecord(docCode,userId,ref ErrMsg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", ErrMsg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "不存在该单据");
        }
        else if (!ds.Tables[0].Rows[0]["ApproverUserId"].ToString().Contains( userId))
        {
            res.Add("ErrCode", 4);
            res.Add("ErrMsg", "无审批权限或该单据已被审批");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("data", DataTableToJArray(ds.Tables[0]));
            res.Add("approver", GetApproverList(ds.Tables[1]));
            res.Add("IsRecord", "true");
        }
        return res;
    }
    /// <summary>
    /// 将已审批的人员列表由datatable转为jarray
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static JArray GetApproverList(DataTable dt)
    {
        JArray jarray = new JArray();
        foreach(DataRow row in dt.Rows)
        {
            JObject jobject = new JObject();
            string department = row["departmentName"].ToString();
            jobject.Add("userName", row["userName"].ToString());
            jobject.Add("avatar", row["avatar"].ToString());
            jobject.Add("department", department);//截取最后一个‘/’后面的部门名称
            jarray.Add(jobject);
        }
        return jarray;
    }
    /// <summary>
    /// 将单据某一项格式进行规范化
    /// </summary>
    /// <param name="label">标签</param>
    /// <param name="value">值</param>
    /// <param name="type">类型（number,text,select等）</param>
    /// <returns></returns>
    private static JObject CreateJobject(string label,string value,string type)
    {
        JObject jobject = new JObject();
        jobject.Add("label", label);
        jobject.Add("value", value);
        jobject.Add("type", type);
        return jobject;
    }

    /*以下是提交或审批的相关方法*/

    private static JObject SubmitDoc(string userId,string docId,JArray jarray,int n)
    {
        JObject res = new JObject();
        if (n > 0)
        {

            string docCode = "0";
            string ErrMsg = "";
            DataSet ds = CostSharingDeleteSrv.GetMaxDocCode(ref ErrMsg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", ErrMsg);
            }
            else
            {
                List<string> sqlList = new List<string>();

                docCode = CreateDocCode(ds.Tables[0].Rows[0][0].ToString());

                JObject DocJObject = new JObject();
                DocJObject.Add("Code", docCode);//单据号
                DocJObject.Add("Level", "1");//下一审批级别
                DocJObject.Add("SubmitterUserId", userId);//提交人userId
                DocJObject.Add("InsertOrUpdate", "3");//3表示丢失
                DocJObject.Add("State", "审批中");
                DocJObject.Add("NewCostSharingId", docId);//new_cost_sharing的Id
                DocJObject.Add("CreateTime", DateTime.Now);
                DocJObject.Add("ApproverUserId", "100000142");//公司总经理--吕正和

                sqlList.Add(SqlHelper.GetInsertString(DocJObject, "cost_sharing_record"));
                sqlList.Add(GetDetailInsertSQL(jarray, "0", userId, docCode));

                SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sqlList.ToArray()));
                if (msg.Result == 0)
                {
                    //发信息给提交者还有下级审批人
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");
                }
                else//错误，等待0.2秒重新提交
                {
                    Thread.Sleep(200);
                    res = SubmitDoc( userId, docId,  jarray,  n-1);
                }

            }
        }
        else
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "网络超时，请重新提交！");
        }
        return res;
    }
    private static JObject Approve(string userId, JArray jarray, string docCode)
    {
        JObject res = new JObject();


        List<string> sqlList = new List<string>();
        string level = "0";


        JObject DocJObject = new JObject();
        if (userId == "100000142")
        {
            level = "1";
            DocJObject.Add("Level", "2");//下一审批级别
            DocJObject.Add("ApproverUserId", "100000225");//企管部--程丹凤
        }
        else
        {
            level = "2";
            DocJObject.Add("Level", "3");//下一审批级别
            DocJObject.Add("State", "已审批");
            DocJObject.Add("ApproverUserId", "");
        }

        sqlList.Add(SqlHelper.GetUpdateString(DocJObject, "cost_sharing_record", "where Code='" + docCode + "'"));
        sqlList.Add(GetDetailInsertSQL(jarray, level, userId, docCode));

        SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sqlList.ToArray()));
        if (msg.Result == 0)
        {
            //发信息给提交者还有下级审批人
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", msg.ExceMsg);
        }

        return res;
    }

    /// <summary>
    /// 创建单据号
    /// </summary>
    /// <param name="docCode">数据库中保存的今天最大单据号，如果今天没有单据，则为0</param>
    /// <returns></returns>
    private static string CreateDocCode(string docCode)
    {
        if (docCode == "0")
        {
            docCode = DateTime.Now.ToString("yyyyMMdd") + "000000";
        }
        return (Convert.ToInt64(docCode) + 1).ToString();
    }
    private static string GetDetailInsertSQL(JArray fieldList, string level, string userId, string docCode)
    {

        List<JObject> sqlArray = new List<JObject>();
        foreach (JObject jobject in fieldList)
        {
            JObject sqlObject = new JObject();
            sqlObject.Add("FieldName", jobject["label"].ToString());

            sqlObject.Add("RegistrationCode", docCode);
            sqlObject.Add("ApproverUserId", userId);
            sqlObject.Add("Level", level);
            sqlObject.Add("CreateTime", DateTime.Now);
            sqlObject.Add("NewValue", jobject["value"].ToString());

            sqlArray.Add(sqlObject);
        }
        return SqlHelper.GetInsertString(sqlArray, "cost_sharing_detail");
    }
}