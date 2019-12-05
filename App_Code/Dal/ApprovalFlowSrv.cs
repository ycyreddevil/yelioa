using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using Newtonsoft.Json.Linq;

/// <summary>
/// ApprovalFlowSrv 的摘要说明
/// </summary>
public class ApprovalFlowSrv
{
    public ApprovalFlowSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static readonly string ErrorMsg = "ErrorMsg";

    public static string CreateDocument(Dictionary<string, string> dict)
    {
        string table = dict["DocumentTableName"];
        dict.Remove("DocumentTableName");
        string sql = SqlHelper.GetInsertIgnoreString(dict, table);
        return SqlHelper.Exce(sql);
    }

    /// <summary>
    /// 提交表单，保存审批记录信息，审批Level = 0
    /// </summary>
    /// <param name="table"></param>
    /// <param name="docCode"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static string SubmitDocumentSaveRecord(UserInfo user, DataSet dsDocInfo, ref Dictionary<string, string> dict)
    {
        DataTable DtAppProcess = dsDocInfo.Tables[1];
        string table = DtAppProcess.Rows[0]["DocumentTableName"].ToString();
        string docCode = "";
        if (table == "net_sales")
        {
            docCode = dsDocInfo.Tables[3].Rows[0]["DocCode"].ToString();
        }
        else
        {
            docCode = dsDocInfo.Tables[3].Rows[0]["id"].ToString();
        }
        //Dictionary<string, string> dict = new Dictionary<string, string>();
        dict = new Dictionary<string, string>();
        dict.Add("DocumentTableName", table);
        dict.Add("DocCode", docCode);
        dict.Add("Level", "0");
        dict.Add("Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict.Add("ApproverId", user.userId.ToString());
        dict.Add("SubmitterId", user.userId.ToString());
        dict.Add("ApprovalResult", "单据提交");
        string sql = SqlHelper.GetInsertIgnoreString(dict, "approval_record");
        return SqlHelper.Exce(sql);
    }

    // 新增一条撤回记录到记录表
    public static String returnDocument(UserInfo user, DataSet dsDocInfo)
    {
        DataTable DtAppProcess = dsDocInfo.Tables[1];
        string table = DtAppProcess.Rows[0]["DocumentTableName"].ToString();
        string docCode = dsDocInfo.Tables[3].Rows[0]["DocCode"].ToString();
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("DocumentTableName", table);
        dict.Add("DocCode", docCode);
        dict.Add("Level", "0");
        dict.Add("Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict.Add("ApproverId", user.userId.ToString());
        dict.Add("SubmitterId", user.userId.ToString());
        dict.Add("ApprovalResult", "单据撤回");
        string sql = SqlHelper.GetInsertIgnoreString(dict, "approval_record");
        return SqlHelper.Exce(sql);
    }

    /// <summary>
    /// 更新单据状态及审批级别
    /// </summary>
    /// <param name="table"></param>
    /// <param name="docCode"></param>
    /// <param name="state"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static String UpdateDocStatus(String table, String docCode, string state,int level)
    {
        String sql = "";
        if (table == "deliver_apply_report" || table == "demand_apply_report")
            sql = string.Format("update {0} set Status='{1}',Level={2}  "
                + "where id = {3}", table, state, level, docCode);
        else if (table == "yl_reimburse")
            sql = string.Format("update yl_reimburse set Status='{0}',Level={1}  "
                + "where id = {2}", state, level, docCode);
        else if (table == "wages" || table == "outer_wages" || table == "tax")
            sql = string.Format("update {0} set Status='{1}',Level={2}  "
                + "where DocCode = {3}", table, state, level, docCode);
        else if (state == "未提交")
            sql = string.Format("update {0} set Editable = 0,State='{1}',Level={2},Editable=1  "
                + "where DocCode = {3}", table, state, level, docCode);
        else
            sql = string.Format("update {0} set Editable = 0,State='{1}',Level={2},Editable=0  "
                + "where DocCode = {3}", table, state, level, docCode);        
        return SqlHelper.Exce(sql);
    }

    /// <summary>
    /// 获取审批表单基本信息
    /// DataTable DtAppApprover = dsDocInfo.Tables[0];
    /// DataTable DtAppProcess = dsDocInfo.Tables[1];
    /// DataTable DtAppRecord = dsDocInfo.Tables[2];
    /// DataTable DtDocument = dsDocInfo.Tables[3];
    /// </summary>
    /// <param name="table"></param>
    /// <param name="docCode"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static DataSet GetDocumentInfo(string table,string docCode,ref string msg)
    {
        List<string> list = new List<string>();
        list.Add(string.Format("select * from approval_approver where DocumentTableName='{0}' "
            + "and DocCode='{1}' order by Level", table, docCode));

        if (table == "wages" || table == "outer_wages" || table == "tax")
        {
            list.Add(string.Format("select a.*,users.wechatUserId from approval_process  a LEFT JOIN users ON" +
            " a.ApproverId=users.userId where a.DocumentTableName='{0}' and a.DocCode='0'"
            + " order by a.Level asc", table));
        }
        else
        {
            list.Add(string.Format("select a.*,users.wechatUserId from approval_process  a LEFT JOIN users ON" +
            " a.ApproverId=users.userId where a.DocumentTableName='{0}' and a.DocCode='{1}'"
            + " order by a.Level asc", table, docCode));
        }
        
        list.Add(string.Format("select * from approval_record where DocumentTableName='{0}' "
            + "and DocCode='{1}' order by Time asc", table, docCode));
        if (table == "net_sales" || table == "wages" || table == "outer_wages" || table == "tax")
        {
            list.Add(string.Format("select * from {0} where DocCode='{1}'", table, docCode));
        }
        else
        {
            list.Add(string.Format("select * from {0} where id='{1}'", table, docCode));
        }

        // 查询是否已提交
        list.Add(string.Format("select * from approval_record where DocumentTableName='{0}' "
            + "and DocCode='{1}' and Level > 0 order by Time asc;", table, docCode));

        list.Add(string.Format("SELECT a.*,users.wechatUserId FROM `approval_informer` a LEFT JOIN users ON" +
            " a.InformerUserId=users.userId  where a.TableName='{0}' AND a.DocCode='{1}'", table, docCode));
        DataSet ds = SqlHelper.Find(list.ToArray(),ref msg);
        if (ds == null)
            return null;
        //else if (ds.Tables[0].Rows.Count == 0 && ds.Tables[2].Rows.Count > 0)
        //{
        //    msg = "审批流程已结束！";
        //    return null;
        //}
        else if(ds.Tables[1].Rows.Count == 0)
        {
            msg = "未找到相关单据审批流程,请先设置审批流程！";
            return null;
        }
        //else if (ds.Tables[0].Rows.Count == 0 && ds.Tables[2].Rows.Count == 0)
        //{
        //    msg = "审批单据未提交，请先提交再审批！";
        //    return null;
        //}
        //else if (ds.Tables[0].Rows.Count > 0 )
        //{
        //    DataTable DtAppProcess = ds.Tables[1];
        //    DataTable DtAppApprover = ds.Tables[0];
        //    int level = Convert.ToInt32(DtAppApprover.Rows[0]["Level"]);
        //    int maxLevel = Convert.ToInt32(DtAppProcess.Rows[DtAppProcess.Rows.Count - 1]["Level"]);
        //    if (level > maxLevel)//审批到最后一级，审批流程结束
        //    {
        //        msg = "审批流程已结束！";
        //        return null;
        //    }
        //}
        else if (ds.Tables[3].Rows.Count == 0)
        {
            msg = "未找到相关单据信息";
            return null;
        }
        return ds;
    }



    /// <summary>
    /// 获取当前审批人ID
    /// </summary>
    /// <param name="user"></param>
    /// <param name="dsDocInfo"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static ArrayList GetCurrentApproverId(UserInfo user, DataSet dsDocInfo, ref string msg)
    {
        ArrayList list = new ArrayList();
        //string ErrorMsg = "ErrorMsg";

        DataTable DtAppApprover = dsDocInfo.Tables[0];
        DataTable DtAppProcess = dsDocInfo.Tables[1];
        DataTable DtAppRecord = dsDocInfo.Tables[2];
        DataTable DtDocument = dsDocInfo.Tables[3];

        string table = DtAppProcess.Rows[0]["DocumentTableName"].ToString();
        int level = 0;
        if (DtAppApprover.Rows.Count == 0)//单据提交
        {
            level = 1;
        }
        else//单据审批
        {
            level = Convert.ToInt32(DtAppApprover.Rows[0]["Level"])+1;
        }


        foreach (DataRow row in DtAppProcess.Rows)
        {
            try
            {
                if (Convert.ToInt32(row["Level"]) == level)
                {
                    //首先查找是否直接有审批人ID，如果没有则通过部门岗位查找
                    if(!row["ApproverId"].Equals(DBNull.Value) && Convert.ToInt32(row["ApproverId"]) != 0)
                    {
                        msg = "success";
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("ApproverId", row["ApproverId"].ToString());
                        dict.Add("Level", level.ToString());
                        list.Add(dict);
                        return list;
                    }
                    ///////////////////////纯销上报需要特殊处理，从网点数据里面寻找相应审批人-START
                    else if (table == "net_sales" && level==1)
                    {
                        string sql = string.Format("select ManagerId from cost_sharing where HospitalId = {0} "
                            + "and ProductId = {1} and SalesId={2}", DtDocument.Rows[0]["HospitalId"]
                            , DtDocument.Rows[0]["ProductId"], DtDocument.Rows[0]["SalesId"]);
                        object obj = SqlHelper.Scalar(sql);
                        if(obj == null)
                        {
                            msg = "审批流程当中未找到相关审批人";
                            return null;
                        }
                        msg = "success";
                        //// 更新审批流程中level=1的审核人
                        //sql = string.Format("update approval_process set ApproverId = {0} where DocCode = '{1}' and level = 1", obj.ToString(), row["DocCode"].ToString());
                        //SqlHelper.Exce(sql);

                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("ApproverId", obj.ToString());
                        dict.Add("Level", level.ToString());
                        list.Add(dict);
                        return list;
                    }                  
                    ///////////////////////纯销上报需要特殊处理，从网点数据里面寻找相应审批人-END
                    else
                    {
                        ArrayList idList = GetUserIdFromDepartmentAndPost(row["ApproverDepartmentId"].ToString()
                            , row["ApproverPostId"].ToString(), ref msg);
                        if (idList == null)
                            return null;
                        else
                        {
                            foreach(object id in idList)
                            {
                                Dictionary<string, string> dict = new Dictionary<string, string>();
                                dict.Add("ApproverId", id.ToString());
                                dict.Add("Level", level.ToString());
                                list.Add(dict);
                            }
                            return list;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                msg = ex.ToString();
                return null;
            }
        }
        msg = "审批流程当中未找到相关级别";
        return null;
    }

    public static ArrayList GetUserIdFromDepartmentAndPost(string departmentId, string postId, ref string msg)
    {
        ArrayList list = new ArrayList();
        msg = "";
        string sql = string.Format("select us.userId from users us left join user_department_post udp " +
            "on us.wechatUserId = udp.wechatUserId where udp.departmentId={0} and udp.postId={1}"
            , departmentId, postId);
        DataSet res = SqlHelper.Find(sql, ref msg);
        if (res == null)
        {
            return null;
        }            
        else if (res.Tables[0].Rows.Count == 0)
        {
            msg = "未找到相关审批人信息";
            return null;
        }
        else
        {
            foreach(DataRow row in res.Tables[0].Rows)
            {
                list.Add(row["userId"].ToString());
            }
        }
        msg = "success";
        return list;
    }


    public static string SaveCurrentApproverInfoForAgreement( UserInfo user,DataSet dsDocInfo,int isReturned)
    {
        string res = "";
        ArrayList listApproverId = GetCurrentApproverId( user, dsDocInfo,ref res);
        if (listApproverId == null)//审批流程结束或出错，返回错误信息
            return res;        

        DataTable DtAppProcess = dsDocInfo.Tables[1];
        string table = DtAppProcess.Rows[0]["DocumentTableName"].ToString();
        string docCode = dsDocInfo.Tables[3].Rows[0]["DocCode"].ToString();

        ArrayList listSqlCmd = new ArrayList();
        string sql = string.Format("delete from approval_approver where DocumentTableName='{0}' and DocCode='{1}'", table, docCode);
        listSqlCmd.Add(sql);
        foreach (Dictionary<string, string> dict in listApproverId)
        {
            //string sql = SqlHelper.GetUpdateString(dict, "approval_approver"
            //    , string.Format(" where DocumentTableName='{0}' and DocCode='{1}'", table, docCode));
            
            if (isReturned == 0)
            {
                dict.Add("DocumentTableName", table);
                dict.Add("DocCode", docCode);
                sql = SqlHelper.GetInsertIgnoreString(dict, "approval_approver");
                listSqlCmd.Add(sql);
            }
        }
        string[] strs = (string[])listSqlCmd.ToArray(typeof(string));
        return SqlHelper.Exce(strs);
    }

    public static string SaveCurrentApproverInfoForDisagreement(string table, string docCode)
    {
        string sql = string.Format("delete from approval_approver where DocumentTableName='{0}' and DocCode='{1}'", table, docCode);
        List<string> list = new List<string>();
        list.Add(sql);
        sql = string.Format("update {0} set Editable = 1,State='未提交' where DocCode = {1}", table, docCode);
        list.Add(sql);
        return SqlHelper.Exce(list.ToArray());
    }

    public static string ClearCurrentApproverInfo(string table, string docCode)
    {
        string sql = string.Format("delete from approval_approver where DocumentTableName='{0}' and DocCode='{1}'"
            , table, docCode);
        return SqlHelper.Exce(sql);
    }

    public static string ClearCurrentProcessInfo(string table, string docCode)
    {
        string sql = string.Format("delete from approval_process where DocumentTableName='{0}' and DocCode='{1}'"
            , table, docCode);
        return SqlHelper.Exce(sql);
    }

    public static string UpdateNetSaleEditable(string docCode)
    {
        string sql = string.Format("update net_sales set State = '审批结束', Editable = 2 where DocCode = '{0}'"
            , docCode);
        return SqlHelper.Exce(sql);
    }

    /// <summary>
    /// 判断当前用户是否有审批权限，如果有，顺便更新当前审批级别
    /// </summary>
    /// <param name="user"></param>
    /// <param name="dsDocInfo"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static bool HasRightToApprove(string userId, DataSet dsDocInfo,int level)
    {
        foreach(DataRow row in dsDocInfo.Tables[0].Rows)
        {
            if(Convert.ToString(row["ApproverId"]) == userId
                && Convert.ToInt32(row["Level"]) == level)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 保存审批记录
    /// </summary>
    /// <param name="user"></param>
    /// <param name="dsDocInfo"></param>
    /// <param name="ApprovalResult">审批结果</param>
    /// <param name="ApprovalOpinions">审批意见</param>
    /// <returns></returns>
    public static string ApproveDocumentSaveRecord(string userId, DataSet dsDocInfo
        ,string ApprovalResult,string ApprovalOpinions,int level)
    {
        DataTable DtAppProcess = dsDocInfo.Tables[1];
        DataTable DtAppRecord = dsDocInfo.Tables[2];
        string table = DtAppProcess.Rows[0]["DocumentTableName"].ToString();
        string docCode = "";

        if (dsDocInfo.Tables[3].Columns.Contains("DocCode"))
            docCode = dsDocInfo.Tables[3].Rows[0]["DocCode"].ToString();
        else
            docCode = dsDocInfo.Tables[3].Rows[0]["Id"].ToString();

        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("DocumentTableName", table);
        dict.Add("DocCode", docCode);
        dict.Add("Level", (level).ToString());
        dict.Add("Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict.Add("ApproverId", userId);
        string SubmitterId = DtAppRecord.Rows[0]["SubmitterId"].ToString();
        dict.Add("SubmitterId", SubmitterId);
        dict.Add("ApprovalResult", ApprovalResult);
        dict.Add("ApprovalOpinions", ApprovalOpinions);
        string sql = SqlHelper.GetInsertIgnoreString(dict, "approval_record");
        return SqlHelper.Exce(sql);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////Version 2.0//////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 在提交单据或保存草稿时，计算并保存该单据所有审批人员信息
    /// </summary>
    /// <param name="user"></param>
    /// <param name="dsDocInfo"></param>
    /// <returns>返回错误代码</returns>
    public static string SaveAllApproverInfo(UserInfo user, DataSet dsDocInfo , Dictionary<string, string> dictNewRecd
        , ref JArray listApproverInfo)
    {
        DataTable DtAppApprover = dsDocInfo.Tables[0];
        DataTable DtAppProcess = dsDocInfo.Tables[1];
        //DataTable DtAppRecord = dsDocInfo.Tables[2];
        DataTable DtDocument = dsDocInfo.Tables[3];

        string table = dictNewRecd["DocumentTableName"].ToString();
        string docCode = dictNewRecd["DocCode"].ToString();

        ArrayList listSqlCmd = new ArrayList();
        //清除该单据所有审批人员信息
        string sql = string.Format("delete from approval_approver where DocumentTableName='{0}' and DocCode='{1}'"
            , table, docCode);
        listSqlCmd.Add(sql);

        string msg = "";
        listApproverInfo = GetAllApproverId(user, dsDocInfo, dictNewRecd, ref msg);
        if(listApproverInfo == null)
        {
            return msg;
        }

        foreach(JObject obj in listApproverInfo)
        {
            sql = SqlHelper.GetInsertString(obj, "approval_approver");
            listSqlCmd.Add(sql);
        }

        return SqlHelper.Exce((string[])listSqlCmd.ToArray(typeof(string)));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="dsDocInfo"></param>
    /// <param name="dictNewRecd">最新的审批记录信息</param>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static JArray GetAllApproverId(UserInfo user, DataSet dsDocInfo, Dictionary<string, string> dictNewRecd, ref string msg)
    {
        DataTable DtAppApprover = dsDocInfo.Tables[0];
        DataTable DtAppProcess = dsDocInfo.Tables[1];
        //DataTable DtAppRecord = dsDocInfo.Tables[2];
        DataTable DtDocument = dsDocInfo.Tables[3];

        string table = dictNewRecd["DocumentTableName"].ToString();
        string docCode = dictNewRecd["DocCode"].ToString();

        JArray jList = new JArray();
        int level = 0;

        //保存其他审批人信息
        foreach (DataRow row in DtAppProcess.Rows)
        {            
            try
            {
                level = Convert.ToInt32(row["Level"]);
                //首先查找是否直接有审批人ID，如果没有则通过部门岗位查找
                if (!row["ApproverId"].Equals(DBNull.Value) && Convert.ToInt32(row["ApproverId"]) != 0)
                {
                    msg = "success";
                    JObject jObj = new JObject();
                    jObj.Add("ApproverId", row["ApproverId"].ToString());
                    jObj.Add("Level", level.ToString());
                    jObj.Add("DocumentTableName", table);
                    jObj.Add("DocCode", docCode);
                    jList.Add(jObj);
                }
                ///////////////////////纯销上报需要特殊处理，从网点数据里面寻找相应审批人-START
                else if (table == "net_sales" && level == 1)
                {
                    string sql = string.Format("select ManagerId from cost_sharing where HospitalId = {0} "
                        + "and ProductId = {1} and SalesId={2}", DtDocument.Rows[0]["HospitalId"]
                        , DtDocument.Rows[0]["ProductId"], DtDocument.Rows[0]["SalesId"]);
                    object obj = SqlHelper.Scalar(sql);
                    if (obj == null)
                    {
                        msg = "审批流程当中未找到相关审批人";
                        return null;
                    }
                    else
                    {
                        msg = "success";
                        JObject jObj = new JObject();
                        jObj.Add("ApproverId", obj.ToString());
                        jObj.Add("Level", level.ToString());
                        jObj.Add("DocumentTableName", table);
                        jObj.Add("DocCode", docCode);
                        jList.Add(jObj);
                    }                    
                }
                ///////////////////////纯销上报需要特殊处理，从网点数据里面寻找相应审批人-END
                else
                {
                    ArrayList idList = GetUserIdFromDepartmentAndPost(row["ApproverDepartmentId"].ToString()
                        , row["ApproverPostId"].ToString(), ref msg);
                    if (idList == null)
                        return null;
                    else
                    {
                        foreach (object id in idList)
                        {
                            JObject jObj = new JObject();
                            jObj.Add("ApproverId", id.ToString());
                            jObj.Add("Level", level.ToString());
                            jObj.Add("DocumentTableName", table);
                            jObj.Add("DocCode", docCode);
                            jList.Add(jObj);
                        }
                        msg = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return null;
            }
        }
        msg = "审批流程当中未找到相关级别";
        return jList;
    }

    public static DataSet GetUserInfoByUserId(int [] userId)
    {
        List<string> listSql = new List<string>();
        foreach(int id in userId)
        {
            string sql = string.Format("select * from users where userId={0}", id);
            listSql.Add(sql);
        }
        return SqlHelper.Find(listSql.ToArray());
    }

    public static int[] GetApproverIddByLevel(int level, JArray listApproverInfo)
    {
        List<int> list = new List<int>();
        foreach(var obj in listApproverInfo)
        {
            if(Convert.ToInt32(obj["Level"]) == level)
            {
                list.Add(Convert.ToInt32(obj["ApproverId"]));
            }
        }
        return list.ToArray();
    }

    public static int[] GetApproverIddByLevel(int level, DataTable dt)
    {
        List<int> list = new List<int>();
        foreach(DataRow row in dt.Rows)
        {
            if (Convert.ToInt32(row["Level"]) == level)
            {
                list.Add(Convert.ToInt32(row["ApproverId"]));
            }
        }
        return list.ToArray();
    }

    public static string AddDeliverApproveTime(string id)
    {
        string sql = "UPDATE  `deliver_apply_report` SET ApproveTime=NOW() WHERE id='"+id+"'";       
        return  SqlHelper.Exce(sql);
    }

    public static string AddDemandApproveTime(string id)
    {
        string sql = "UPDATE `demand_apply_report` SET ApproveTime=NOW() WHERE id='" + id + "'";
        return SqlHelper.Exce(sql);
    }
}