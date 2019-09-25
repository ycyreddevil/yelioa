using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data;


public class EmailHelper
{
    /// <summary>
    /// 创建邮件
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <param name="ReplyEmailId"></param>
    /// <returns></returns>
    public static string CreateEmail(string userId, string ReplyEmailId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Sender", userId);
        dict.Add("Status", "草稿");
        dict.Add("LMT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict.Add("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict.Add("ReplyFrom", ReplyEmailId);
        string sql = SqlHelper.GetInsertIgnoreString(dict, "yl_email");
        string id = SqlHelper.InsertAndGetLastId(sql);

        JObject obj = JObject.Parse(id);
        if (obj["Success"].ToString() == "1")
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("Id", obj["Id"].ToString());
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", obj["message"].ToString());
        }
        return res.ToString();
    }

    /// <summary>
    /// 保存草稿
    /// </summary>
    /// <param name="emailId">邮件ID</param>
    /// <param name="subject">邮件主题</param>
    /// <param name="text">邮件正文</param>
    /// <param name="recipients">收件人</param>
    /// <returns></returns>
    public static string SaveDraft(string emailId, string subject, string text
        , string[] recipients)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(emailId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        List<string> sqlList = new List<string>();
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Subject", subject);
        dict.Add("Text", text);
        dict.Add("LMT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sqlList.Add(SqlHelper.GetUpdateString(dict, "yl_email", string.Format(" where Id={0}", emailId)));
        if (recipients.Length > 0)
        {
            sqlList.Add(string.Format("delete from yl_email_recipient where EmailId={0}", emailId));
            ArrayList list = new ArrayList();

            foreach (string recpient in recipients)
            {
                if (string.IsNullOrEmpty(recpient))
                    continue;
                if (recpient.Length <= 5)
                {
                    DataSet ds = UserInfoSrv.getInfos("1", recpient);                  
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                Boolean flag = true;
                                foreach(Dictionary<string,string> dc in list)
                                {
                                    if(dc["UserId"].ToString()==row["userId"].ToString())
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    dict = new Dictionary<string, string>();
                                    dict.Add("EmailId", emailId);
                                    dict.Add("UserId", row["userId"].ToString());
                                    list.Add(dict);
                                    sqlList.Add(string.Format("insert into `yl_email_recipient` (userid,emailId) values" +
                                        " ((select t.wechatUserId from users t where userId = '{0}'),'{1}');", row["userId"].ToString(), emailId));
                                }
                            }
                        }
                    }
                    else if(ds==null)
                    {
                        res.Add("ErrCode", 1);
                        res.Add("ErrMsg","连接数据库出现问题");
                        return res.ToString();
                    }
                }
                else if(recpient.Length <= 8)
                {
                    int id = (Convert.ToInt32(recpient) - 1000000) / 1000;
                    DataSet ds = SqlHelper.Find("select GroupMember from yl_email_group where Id="+id);
                    JArray GroupJarray = JArray.Parse(ds.Tables[0].Rows[0][0].ToString());
                    foreach(JObject jobject in GroupJarray)
                    {
                        Boolean flag = true;
                        foreach (Dictionary<string, string> dc in list)
                        {
                            if (dc["UserId"].ToString() == jobject["UserId"].ToString())
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            dict = new Dictionary<string, string>();
                            dict.Add("EmailId", emailId);
                            dict.Add("UserId", jobject["UserId"].ToString());
                            list.Add(dict);
                            sqlList.Add(string.Format("insert into `yl_email_recipient` (userid,emailId) values" +
                                           " ((select t.wechatUserId from users t where userId = '{0}'),'{1}');", jobject["UserId"].ToString(), emailId));
                        }
                    }
                }
                else
                {
                    Boolean flag = true;
                    foreach (Dictionary<string, string> dc in list)
                    {
                        if (dc["UserId"].ToString() == recpient)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        dict = new Dictionary<string, string>();
                        dict.Add("EmailId", emailId);
                        dict.Add("UserId", recpient);
                        list.Add(dict);
                        sqlList.Add(string.Format("insert into `yl_email_recipient` (userid,emailId) values" +
                                       " ((select t.wechatUserId from users t where userId = '{0}'),'{1}');", recpient, emailId));
                    }
                }
            }                  
        }
        SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sqlList.ToArray()));
        if (r.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", "0");
            res.Add("ErrMsg", "操作成功");
          
           
        }
        else
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", r.ExceMsg);
        }
        return res.ToString();
    }

    /// <summary>
    /// 添加附件
    /// </summary>
    /// <param name="emailId">邮件ID</param>
    /// <param name="fileName">文件名称</param>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    public static string InsertAttachment(string emailId, string fileName, string filePath)
    {
        string fileCode = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + ValideCodeHelper.GetRandomCode(128);
        JObject res = new JObject();
        if (string.IsNullOrEmpty(emailId) || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(filePath))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        List<string> listSql = new List<string>();
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("EmailId", emailId);
        dict.Add("FileName", fileName);
        dict.Add("FilePath", filePath);
        dict.Add("FileCode", fileCode);
        dict.Add("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict["FilePath"] = dict["FilePath"].Replace("\\", "\\\\");
        listSql.Add(SqlHelper.GetInsertString(dict, "yl_email_attachment"));
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("Attachment", "1");
        listSql.Add(SqlHelper.GetUpdateString(dict, "yl_email", "where emailId=" + emailId));
        SqlExceRes r = new SqlExceRes(SqlHelper.Exce(listSql.ToArray()));
        if (r.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("FileCode", fileCode);
        }
        else
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", r.ExceMsg);
        }

        return res.ToString();
    }

    /// <summary>
    /// 打开收件箱
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <returns></returns>
    public static string ReceiveEmail(string userId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        string sql = string.Format("SELECT vm.*, u.userName,u.avatar FROM v_email as vm LEFT JOIN users as u ON vm.Sender = u.wechatUserId WHERE"+
                   " vm.UserId = '{0}' AND vm.RecipientStatus IN ('已读', '未读') ORDER BY vm.SendTime desc,vm.RecipientStatus", userId);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "未找到邮件");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("data", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]));
        }
        return res.ToString();
    }

    /// <summary>
    /// 打开草稿箱或垃圾箱或发件箱
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <param name="operation">草稿箱（草稿）、垃圾箱（已删除）、发件箱（已发送）</param>
    /// <returns></returns>
    public static string GetDraftAndSpamList(string userId, string operation)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId) || (operation != "草稿" && operation != "已删除"&& operation != "已发送"))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少或错误");
            return res.ToString();
        }
        string sql = string.Format(" (SELECT ye.*,ye.id EmailId, u.avatar,u.userName,u.wechatUserId as UserId FROM yl_email AS ye" +
                                   " LEFT JOIN users AS u ON ye.Sender = u.wechatUserId WHERE" +
                                   " ye.Sender = '{0}' AND ye.STATUS = '{1}' GROUP BY ye.Id ORDER BY ye.LMT desc)", userId, operation);
        if (operation=="已删除")
        {
            sql += string.Format("UNION ALL(SELECT ye.*, ye.id EmailId, GROUP_CONCAT(u.userName) as userName, GROUP_CONCAT(yer.UserId) as UserId FROM yl_email AS ye" +
                          " LEFT JOIN yl_email_recipient AS yer ON ye.Id = yer.EmailId LEFT JOIN users AS u ON yer.UserId = u.wechatUserId WHERE" +
                          " yer.UserId = '{0}' AND yer.STATUS = '{1}' GROUP BY ye.Id ORDER BY ye.LMT desc)", userId, operation);
             
        }
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "未找到邮件");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("data", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]));
        }
        return res.ToString();
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="emailId">邮件ID</param>
    /// <returns></returns>
    public static string SendEmail(string emailId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(emailId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }        
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Status", "已发送");
        dict.Add("SendTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        
       
        SqlExceRes r = new SqlExceRes(SqlHelper.Exce(SqlHelper.GetUpdateString(dict, "yl_email", string.Format(" where Id={0}", emailId))));
        if (r.Result == SqlExceRes.ResState.Success)
        {
            string msg = "";
            List<string> sqlList = new List<string>();
            sqlList.Add(string.Format("select ye.*,u.userName as SendName from yl_email as ye LEFT JOIN users as u on ye.Sender=u.wechatUserId where Id={0}", emailId));
            sqlList.Add(string.Format("select UserId  from yl_email_recipient where EmailId={0}", emailId));
            DataSet ds = SqlHelper.Find(sqlList.ToArray(), ref msg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {
                res.Add("ErrCode", 4);
                res.Add("ErrMsg", "不存在该邮件！");
            }
            else if (ds.Tables[1].Rows.Count == 0)
            {
                res.Add("ErrCode", 4);
                res.Add("ErrMsg", "没有设置收件人！");
            }
            else
            {           
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "发送成功");
                res.Add("SendName", ds.Tables[0].Rows[0]["SendName"].ToString());
                res.Add("Subject", ds.Tables[0].Rows[0]["Subject"].ToString());
                string recipientId = "";
                foreach(DataRow row in ds.Tables[1].Rows)
                {
                    recipientId += row["UserId"].ToString() + "|";
                }
                recipientId = recipientId.Substring(0, recipientId.Length - 1);
                res.Add("recipientId", recipientId);
            }
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", r.ExceMsg);
        }
        return res.ToString();
    }

    /// <summary>
    /// 创建分组
    /// </summary>
    /// <param name="userId">创建者id</param>
    /// <param name="membersString">分组成员id集合string</param>
    /// <param name="groupName">分组名称</param>
    /// <returns></returns>
    public static string CreateGroup(string userId,string membersString,string groupName)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId)|| string.IsNullOrEmpty(membersString) || string.IsNullOrEmpty(groupName))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少或为空");
            return res.ToString();
        }
        //string[] members = membersString.Split(',');
        //foreach(string member in members)
        //{

        //}
        
        string sql = string.Format("select GroupName from yl_email_group where GroupName='{0}'", groupName);
        object resTemp = SqlHelper.Scalar(sql);
        if(Object.Equals(resTemp, null))
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("UserId", userId);
            dict.Add("GroupName", groupName);
            dict.Add("GroupMember", membersString);
            sql = SqlHelper.GetInsertString(dict, "yl_email_group");
            SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sql));
            if(r.Result == SqlExceRes.ResState.Success)
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "分组创建成功");
            }
            else
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", r.ExceMsg);
            }
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "分组名称已存在");
        }

        return res.ToString();
    }

    /// <summary>
    /// 保存分组
    /// </summary>
    /// <param name="userId">创建者id</param>
    /// <param name="membersString">分组成员id集合string</param>
    /// <param name="groupName">分组名称</param>
    /// <returns></returns>
    public static string SaveGroup(string userId, string membersString, string groupName,string groupId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(membersString) || string.IsNullOrEmpty(groupName)
            || string.IsNullOrEmpty(groupId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少或为空");
            return res.ToString();
        }

        string sql = string.Format("select GroupName from yl_email_group where Id={0}", groupId);
        object resTemp = SqlHelper.Scalar(sql);
        if (!Object.Equals(resTemp, null))
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("UserId", userId);
            dict.Add("GroupName", groupName);
            dict.Add("GroupMember", membersString);
            sql = SqlHelper.GetUpdateString(dict, "yl_email_group",
                string.Format(" where Id={0}", groupId));
            SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sql));
            if (r.Result == SqlExceRes.ResState.Success)
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "分组保存成功");
            }
            else
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", r.ExceMsg);
            }
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "分组名称不存在");
        }
        return res.ToString();
    }


    /// <summary>
    /// 获取用户分组信息,用于获取分组列表
    /// </summary>
    /// <param name="userId">创建者id</param>
    /// <returns></returns>
    public static DataTable GetGroupDataTableInfo(string userId)
    {
        string sql = string.Format("select * from yl_email_group where UserId={0}", userId);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if(ds==null)
        { return null; }
        else
        {
            return ds.Tables[0];
        }
    }

    /// <summary>
    /// 获取用户分组信息
    /// </summary>
    /// <param name="userId">创建者id</param>
    /// <returns></returns>
    public static string GetGroupInfo(string userId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId) )
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少或为空");
            return res.ToString();
        }

        string sql = string.Format("select * from yl_email_group where UserId={0}", userId);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql,ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "未找到分组");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("Data", JsonHelper.DataTable2Json(ds.Tables[0]));
        }
        return res.ToString();
    }

    /// <summary>
    /// 删除分组信息
    /// </summary>
    /// <param name="userId">创建者id</param>
    /// <param name="groupName">分组名称</param>
    /// <returns></returns>
    public static string DeleteGroup(string userId, string groupName)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(groupName))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少或为空");
            return res.ToString();
        }

        string sql = string.Format("delete from yl_email_group where UserId='{0}' and GroupName='{1}'"
            , userId, groupName);
        SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sql));
        if (r.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "删除成功");
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", r.ExceMsg);
        }
        return res.ToString();
    }


    /// <summary>
    /// 邮件数量统计
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <returns></returns>
    public static string EmailStatistique(string userId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        List<string> listSql = new List<string>();
        listSql.Add(string.Format("select count(*) from yl_email where Sender = '{0}' and Status='已发送'"
            , userId));
        listSql.Add(string.Format("select count(*) from yl_email where Sender = '{0}' and Status='草稿'"
            , userId));
        listSql.Add(string.Format("select count(*) from yl_email where Sender = '{0}' and Status='已删除'"
            , userId));
        listSql.Add(string.Format("select count(*) from yl_email_recipient where UserId = '{0}' and Status='已删除'"
           , userId));
        listSql.Add(string.Format("select count(*) from v_email where UserId = '{0}' and RecipientStatus IN ('已读', '未读')"
            , userId));
        string[] r = SqlHelper.Scalar(listSql.ToArray());
        res.Add("ErrCode", 0);
        res.Add("ErrMsg", "操作成功");
        res.Add("DraftNumber", r[1]);
        res.Add("SendNumber", r[0]);
        res.Add("DeletedNumber", (Convert.ToInt32(r[2])+ Convert.ToInt32(r[3])).ToString());
        res.Add("ReceiveNumber", r[4]);
        return res.ToString();
    }

    /// <summary>
    /// 阅读邮件
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <param name="emailId">邮件ID</param>
    /// <returns></returns>
    public static string ReadEmail(string userId, string emailId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(emailId) || string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        List<string> sqlList = new List<string>();
        sqlList.Add(string.Format("select ye.*,u.userName as SendName from yl_email as ye LEFT JOIN users as u on ye.Sender=u.wechatUserId where Id={0}", emailId));
        sqlList.Add(string.Format("select yer.*,u.userName as ReceiveName from yl_email_recipient as yer LEFT JOIN users as u on yer.UserId=u.wechatUserId where EmailId={0}", emailId));
        sqlList.Add(string.Format("select * from yl_email_attachment where EmailId={0}", emailId));
        string msg = "";
        DataSet ds = SqlHelper.Find(sqlList.ToArray(), ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Rows[0]["Text"] = ds.Tables[0].Rows[0]["Text"].ToString().Replace("\n", @"<br/>");
            }
            res.Add("Email", JsonHelper.SerializeObject(ds.Tables[0]));
            res.Add("Recipient", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[1]));
            res.Add("Attachment", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[2]));


            if (ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["Status"].ToString() == "未读")
            {
                sqlList.Clear();
                sqlList.Add(string.Format("UPDATE yl_email_recipient SET Status = '已读', ReadTime='{0}'"
                    + " WHERE EmailId = {1} and UserId='{2}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    , emailId, userId));
                SqlHelper.Exce(sqlList.ToArray());
            }

        }

        return res.ToString();
    }

    /// <summary>
    /// 标记未读为已读，或标记已读为未读
    /// </summary>
    /// <param name="emailId">邮件ID</param>
    /// <param name="status">已读、未读</param>
    /// <returns></returns>
    public static string ChangeStatusToRead(string emailId,string status)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(emailId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        if (status == "未读")
        {
            dict.Add("Status", "已读");
        }
        else if(status=="已读")
        {
            dict.Add("Status", "未读");
        }
        string sql=SqlHelper.GetUpdateString(dict, "yl_email_recipient", "where emailId="+emailId);
        SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sql));
        if (r.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", r.ExceMsg);
        }
        return res.ToString();
    }

    /// <summary>
    /// 阅读草稿或者垃圾箱邮件
    /// </summary>
    /// <param name="emailId">邮件ID</param>
    /// <returns></returns>
    public static string ReadDraftAndSpam(string emailId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(emailId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        List<string> sqlList = new List<string>();
        sqlList.Add(string.Format("select ye.*,u.userName as SendName from yl_email as ye LEFT JOIN users as u on ye.Sender=u.wechatUserId where Id=={0}", emailId));
        sqlList.Add(string.Format("select yer.*,u.userName as ReceiveName from yl_email_recipient as yer LEFT JOIN users as u on yer.UserId=u.wechatUserId where EmailId={0}", emailId));
        sqlList.Add(string.Format("select * from yl_email_attachment where EmailId={0}", emailId));
        string msg = "";
        DataSet ds = SqlHelper.Find(sqlList.ToArray(), ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("Email", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]));
            res.Add("Recipient", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[1]));
            res.Add("Attachment", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[2]));
        }

        return res.ToString();
    }
    /// <summary>
    /// 删除邮件或者将邮件丢入垃圾箱
    /// </summary>
    /// <param name="emailIds">要被操作的邮件ID表</param>
    /// <param name="isDeleteEntirely">true表示删除，false表示丢入垃圾箱</param>
    /// <param name="ReceiveOrSender">将要操作的邮件是收件箱的还是发件箱（此处发件箱包括草稿箱,为垃圾箱时isDeleteEntirely一定为true）的</param>
    /// <returns></returns>
    public static string DeleteEmail(List<string> emailIds, bool isDeleteEntirely,string ReceiveOrSender)
    {
        JObject res = new JObject();
        if (emailIds.Count==0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        
        Dictionary<string, string> dict = new Dictionary<string, string>();
        if (isDeleteEntirely || "delete".Equals(ReceiveOrSender))
        {
            dict.Add("Status", "彻底删除");
        }
        else
        {
            dict.Add("Status", "已删除");
        }
        List<string> sqlList = new List<string>();
        if (ReceiveOrSender=="receive"|| ReceiveOrSender == "delete")
        {
            string condition = "where emailId in('";
            foreach (string emaild in emailIds)
            {
                condition += emaild + "','";
            }
            condition = condition.Substring(0, condition.Length - 2);
            condition += ")";
            //sqlList.Add(string.Format("delete from yl_email where Id={0}", emailId));
            //sqlList.Add(string.Format("delete from yl_email_recipient where EmailId={0}", emailId));
            //sqlList.Add(string.Format("delete from yl_email_attachment where EmailId={0}", emailId));
            sqlList.Add(SqlHelper.GetUpdateString(dict, "yl_email_recipient", condition));
        }
        else
        {
            dict["Status"] = "彻底删除";
            string condition = "where Id in('";
            foreach (string emaild in emailIds)
            {
                condition += emaild + "','";
            }
            condition = condition.Substring(0, condition.Length - 2);
            condition += ")";
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            //dict.Add("Status", "已删除");
            sqlList.Add(SqlHelper.GetUpdateString(dict, "yl_email", condition));
        }
        SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sqlList.ToArray()));
        if (r.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "删除成功");
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", r.ExceMsg);
        }
        return res.ToString();
    }

    /// <summary>
    /// 搜索邮件
    /// </summary>
    /// <param name="keyword">关键词</param>
    /// <param name="userId">用户ID</param>
    /// <param name="state">已接收、已发送、草稿、垃圾箱（已删除）</param>
    /// <returns></returns>
    public static string SearchEmail(string keyword,string userId,string state)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        if (keyword == null)
            keyword = "";
        string sql = "";
        if(state=="已接收")
        {
            sql += "select vm.*,u.userName from v_email as vm LEFT JOIN users as u on vm.Sender=u.wechatUserId" +
                " where vm.recipientStatus in('已读','未读') and  vm.UserId='" + userId+"' and (subject like '%"+ keyword +
                "%' OR u.userName LIKE '%"+ keyword + "%' OR vm.Text LIKE '%"+ keyword + "%') ORDER BY LMT desc";
        }
        else 
        {
            sql += string.Format("(SELECT ye.*, GROUP_CONCAT(DISTINCT u.userName) as userName, GROUP_CONCAT(DISTINCT yer.UserId) as UserId FROM" +
                                 " yl_email AS ye LEFT JOIN yl_email_recipient AS yer ON ye.Id = yer.EmailId LEFT JOIN " +
                                 " users AS u ON yer.UserId = u.wechatUserId WHERE ye.Id IN (SELECT ye.Id FROM yl_email AS ye " +
                                 " LEFT JOIN yl_email_recipient AS yer ON ye.Id = yer.EmailId LEFT JOIN users AS u " +
                                 " ON yer.UserId = u.wechatUserId WHERE ye.Sender = '{0}' AND ye.STATUS = '{1}' and " +
                                 "(ye.subject like '%{2}%' OR u.userName LIKE '%{3}%' OR ye.Text LIKE '%{4}%') " +
                                 " GROUP BY ye.Id ORDER BY ye.LMT desc))", userId, state, keyword, keyword, keyword);
            if (state == "已删除")
            {
                sql += string.Format("UNION ALL (SELECT ye.*, u.userName,u.wechatUserId as UserId FROM yl_email AS ye" +
                                     " LEFT JOIN yl_email_recipient AS yer ON ye.Id = yer.EmailId LEFT JOIN users AS u " +
                                     " ON ye.Sender = u.wechatUserId WHERE yer.UserId = '{0}' " +
                                     " and yer.STATUS = '{1}' AND (ye.subject like '%{2}%' OR u.userName LIKE '%{3}%' OR ye.Text LIKE '%4%')" +
                                     " GROUP BY ye.Id ORDER BY ye.LMT desc)", userId, state, keyword, keyword, keyword);
            }
        }
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else if(ds.Tables[0].Rows.Count == 0||(ds.Tables[0].Rows.Count==1&&string.IsNullOrEmpty( ds.Tables[0].Rows[0][0].ToString())))
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "未找到邮件");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("data", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]));
        }
        return res.ToString();
    }

  /// <summary>
  /// 回复或者转发邮件
  /// </summary>
  /// <param name="userId">人员ID</param>
  /// <param name="emailId">待转发的邮件ID</param>
  /// <param name="state">回复、转发</param>
  /// <returns></returns>
    public static string ReplyOrForwordEmail(string userId, string emailId,string state)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(emailId) || string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }
        List<string> sqlList = new List<string>();
        sqlList.Add(string.Format("select ye.*,u.userName as SendName,u.userId from yl_email as ye LEFT JOIN users as u on ye.Sender=u.wechatUserId where Id={0}", emailId));
        sqlList.Add(string.Format("select yer.*,u.userName as ReceiveName from yl_email_recipient as yer LEFT JOIN users as u on yer.UserId=u.wechatUserId where EmailId={0}", emailId));
        sqlList.Add(string.Format("select * from yl_email_attachment where EmailId={0}", emailId));
        string msg = "";
        DataSet ds = SqlHelper.Find(sqlList.ToArray(), ref msg);
        string primaryData = "";
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            if (state == "回复")
            {
                res.Add("Subject", "回复：" + ds.Tables[0].Rows[0]["Subject"].ToString());
                res.Add("toReplyUserId", ds.Tables[0].Rows[0]["userId"].ToString());
                res.Add("toReplyUserName", ds.Tables[0].Rows[0]["SendName"].ToString());
                string replyEmailId = CreateEmail(userId, emailId);
                res.Add("replyEmailId", replyEmailId);
            }
            else if(state=="转发")
            {
                res.Add("Subject", "转发：" + ds.Tables[0].Rows[0]["Subject"].ToString());
                string forwordEmailId = CreateEmail(userId, emailId);
                res.Add("forwordEmailId", forwordEmailId);
                res.Add("Attachment", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[2]));
            }
            primaryData += "\r\n\r\n\r\n\r\n\r\n\r\n-----------原始数据-----------\r\n";
            primaryData += "发件人：" + ds.Tables[0].Rows[0]["SendName"] + "\r\n";
            primaryData += "收件人：";
            if (ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                    primaryData += row["ReceiveName"] + ",";
                primaryData = primaryData.Substring(0, primaryData.Length - 1);
                primaryData += "\r\n";
            }
            primaryData += "主题：" + ds.Tables[0].Rows[0]["Subject"] + "\r\n";
            primaryData += "发送时间：" + ds.Tables[0].Rows[0]["SendTime"] + "\r\n";
            primaryData += "正文：" + ds.Tables[0].Rows[0]["Text"] + "\r\n";            
            if (ds.Tables[2].Rows.Count > 0)
            {
                primaryData += "附件：";
                foreach (DataRow row in ds.Tables[2].Rows)
                    primaryData += row["FileName"] + ",";
                primaryData = primaryData.Substring(0, primaryData.Length - 1);
            }
            res.Add("primaryData", primaryData);
        }
        return res.ToString();
    }

    public static string deleteDraft(string emailId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(emailId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string sql = string.Format("delete from yl_email where id = '{0}';" +
            "delete from yl_email_attachment where EmailId = '{1}';" +
            "delete from yl_email_recipient where EmailId = '{2}'", emailId, emailId, emailId);

        string msg = SqlHelper.Exce(sql);

        if (msg.Contains("操作成功"))
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "操作失败");
        }

        return res.ToString();
    }

    ///// <summary>
    ///// 转发邮件
    ///// </summary>
    ///// <param name="userId">人员ID</param>
    ///// <param name="emailId">待转发的邮件ID</param>
    ///// <param name="recipients">想要转发给的对象ID</param>
    ///// <returns></returns>
    //public static string ForwardEmail(string userId, string emailId, string[] recipients)
    //{
    //    JObject res = new JObject();
    //    if (string.IsNullOrEmpty(emailId) || string.IsNullOrEmpty(userId) || recipients.Length == 0)
    //    {
    //        res.Add("ErrCode", 1);
    //        res.Add("ErrMsg", "参数缺少");
    //        return res.ToString();
    //    }
    //    List<string> sqlList = new List<string>();
    //    sqlList.Add(string.Format("select ye.*,u.userName as SendName from yl_email as ye LEFT JOIN users as u on ye.Sender=u.wechatUserId where Id={0}", emailId));
    //    sqlList.Add(string.Format("select yer.*,u.userName as ReceiveName from yl_email_recipient as yer LEFT JOIN users as u on yer.UserId=u.wechatUserId where EmailId={0}", emailId));
    //    sqlList.Add(string.Format("select * from yl_email_attachment where EmailId={0}", emailId));
    //    string msg = "";
    //    DataSet ds = SqlHelper.Find(sqlList.ToArray(), ref msg);
    //    if (ds == null)
    //    {
    //        res.Add("ErrCode", 2);
    //        res.Add("ErrMsg", msg);
    //    }
    //    else
    //    {
    //        JObject creatRes = JObject.Parse(CreateEmail(userId, "0"));
    //        if (creatRes["ErrCode"].ToString() == "0")
    //        {
    //            JObject saveDraftRes = JObject.Parse(SaveDraft(creatRes["Id"].ToString(),
    //                ds.Tables[0].Rows[0]["Subject"].ToString(), ds.Tables[0].Rows[0]["Text"].ToString(), recipients));
    //            if (saveDraftRes["ErrCode"].ToString() == "0")
    //            {
    //                if (ds.Tables[2].Rows.Count > 0)
    //                {
    //                    foreach (DataRow row in ds.Tables[2].Rows)
    //                    {
    //                        JObject insertAttachmentRes = JObject.Parse(InsertAttachment(creatRes["Id"].ToString(),
    //                            row["fileName"].ToString(), row["filePath"].ToString()));
    //                        if (insertAttachmentRes["ErrCode"].ToString() != "0")
    //                            return insertAttachmentRes.ToString();
    //                    }
    //                }
    //                res = JObject.Parse(SendEmail(creatRes["Id"].ToString()));
    //            }
    //            else
    //                res = saveDraftRes;
    //        }
    //        else
    //            res = creatRes;
    //    }
    //    return res.ToString();
    //}
}
