
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ICSharpCode.SharpZipLib.LZW;

/// <summary>
/// FormBuilderHelper 的摘要说明
/// </summary>
public class FormBuilderHelper
{
    public FormBuilderHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string saveForm(string formName, string formData, string parameterData, UserInfo userInfo)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(formData) || string.IsNullOrEmpty(parameterData))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string sql = string.Format("insert into wf_form_config (FormName,FormData,ParameterData,UserId,CreateTime) values " +
            "('{0}','{1}','{2}',{3},now())", formName, formData, parameterData, userInfo.userId);

        string msg = SqlHelper.Exce(sql);

        if (msg.Contains("操作成功"))
        {
            // 再创建表单具体内容记录表
            string tableName = "wf_form_" + formName;

            List<JObject> parameterList = JsonHelper.DeserializeJsonToObject<List<JObject>>(parameterData);

            string createSql = "create table " + tableName + " (Id int auto_increment primary key";

            foreach (JObject parameterJObject in parameterList)
            {
                string columnName = parameterJObject["LBL"].ToString();
                string columnType = parameterJObject["TYP"].ToString();

                createSql += ", " + columnName;
                if ("date".Equals(columnType))
                {
                    createSql += " date ";
                }
                else
                {
                    createSql += " varchar(999) ";
                }
                createSql += "COMMENT '" + parameterJObject["LBL"].ToString() + "'";
            }

            createSql += ", DocCode varchar(20) COMMENT '单据编号', userId varchar(20) COMMENT '填表人ID',departmentId varchar(20) COMMENT '填表人部门ID', " +
                "Level tinyint(2) COMMENT '审批级别', Status varchar(20) COMMENT '状态', informer varchar(1000) COMMENT '抄送人',CreateTime datetime COMMENT '创建时间'";
            createSql += ")";
            msg = SqlHelper.Exce(createSql);

            if (msg.Contains("操作成功"))
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
            }
            else
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", "表单内容表创建失败");
            }
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "表单创建失败");
        }

        return res.ToString(); ;
    }

    public static string updateForm(string formName, string formData, string parameterData, string id)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(formData) || string.IsNullOrEmpty(parameterData) || string.IsNullOrEmpty(id))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        //更新配置表
        string sql1 = string.Format("update wf_form_config set FormName='{0}',FormData='{1}',ParameterData='{2}',ModifyTime=now() where id = '{3}'", formName, formData, parameterData, id);

        //更新表结构， 先删除再新增
        string tableName = "wf_form_" + formName;

        string sql2 = string.Format("drop table {0}", tableName);

        List<JObject> parameterList = JsonHelper.DeserializeJsonToObject<List<JObject>>(parameterData);

        string sql3 = "create table " + tableName + " (Id int auto_increment primary key";

        foreach (JObject parameterJObject in parameterList)
        {
            string columnName = parameterJObject["LBL"].ToString();
            string columnType = parameterJObject["TYP"].ToString();

            sql3 += ", " + columnName;
            if ("date".Equals(columnType))
            {
                sql3 += " date ";
            }
            else
            {
                sql3 += " varchar(255) ";
            }
            sql3 += "COMMENT '" + parameterJObject["LBL"].ToString() + "'";
        }

        sql3 += ", DocCode varchar(20) COMMENT '单据编号', userId varchar(20) COMMENT '提交人ID', departmentId varchar(20) COMMENT '填表人部门ID', " +
                "Level tinyint(2) COMMENT '审批级别', Status varchar(20) COMMENT '状态', CreateTime datetime COMMENT '创建时间', informer varchar(1000) COMMENT '抄送人'";
        sql3 += ")";

        List<string> sqlList = new List<string>();

        sqlList.Add(sql1); sqlList.Add(sql2); sqlList.Add(sql3);

        string msg = SqlHelper.Exce(sqlList.ToArray());

        if (msg.Contains("操作成功"))
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "表单创建失败");
        }

        return res.ToString(); ;
    }

    public static string findAllForms()
    {
        JObject res = new JObject();

        string sql = "select * from wf_form_config";

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
            res.Add("data", JsonHelper.DataTable2Json(ds.Tables[0]));
        }
        return res.ToString();
    }

    public static string findDetail(string id, string userId)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(id))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string sql = string.Format("select * from wf_form_config where id = '{0}';", id);
        sql += "select departmentId, userName, department from v_user_department_post where userId = " + userId;

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
            res.Add("ErrMsg", "未找到对应表单");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");

            DataTable dt = ds.Tables[0];

            dt.Columns.Add("userId", typeof(string));
            dt.Columns.Add("departmentId", typeof(string));
            dt.Columns.Add("userName", typeof(string));
            dt.Columns.Add("department", typeof(string));

            dt.Rows[0]["userId"] = userId;
            dt.Rows[0]["departmentId"] = ds.Tables[1].Rows[0][0];
            dt.Rows[0]["userName"] = ds.Tables[1].Rows[0][1];
            dt.Rows[0]["department"] = ds.Tables[1].Rows[0][2];

            res.Add("data", JsonHelper.DataTable2Json(ds.Tables[0]));
        }

        return res.ToString();
    }

    public static string updateRight(string visionJson, string id)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(visionJson))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string sql = string.Format("update wf_form_config set VisibleRange = '{0}' where Id = '{1}'", visionJson, id);

        string msg = SqlHelper.Exce(sql);

        if (msg.Contains("操作成功"))
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "操作成功");
        }

        return res.ToString();
    }

    public static string getHasRight(string id)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(id))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string sql = string.Format("select VisibleRange from wf_form_config where Id = '{0}'", id);

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
            res.Add("ErrMsg", "未找到对应表单");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("data", JsonHelper.DataTable2Json(ds.Tables[0]));
        }

        return res.ToString();
    }

    public static string hasUsedForm(string formName)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(formName))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string tableName = "wf_form_" + formName;

        string sql = string.Format("select Id from {0}", tableName);

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
            res.Add("ErrMsg", "未找到对应表单");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
            res.Add("data", JsonHelper.DataTable2Json(ds.Tables[0]));
        }

        return res.ToString();
    }

    public static string updateFormTypeAndValid(string id, string type, string valid)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(valid))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string sql = string.Format("update wf_form_config set Type = '{0}', Valid = '{1}' where Id = '{2}'", type, valid, id);

        string msg = SqlHelper.Exce(sql);

        if (msg.Contains("操作成功"))
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "表单更新失败");
        }

        return res.ToString(); ;
    }

    public static string findFormByType(string type, UserInfo userInfo)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(type))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string sql = string.Format("select * from wf_form_config where type = '{0}'", type);

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
            res.Add("ErrMsg", "未找到对应表单");
        }
        else
        {
            // 判断该用户是否在表单的可见范围之内
            DataTable dt = ds.Tables[0];
            DataTable newDt = dt.Clone();
            foreach (DataRow dr in dt.Rows)
            {
                Boolean hasDepartmentRight = false;

                string visibleRange = dr["visibleRange"].ToString();

                JObject visibleJObject = JsonHelper.DeserializeJsonToObject<JObject>(visibleRange);

                string departmentRange = visibleJObject["departmentJSON"].ToString();
                string userRange = visibleJObject["userJSON"].ToString();

                // 先判断部门
                List<string> departmentList = JsonHelper.DeserializeJsonToList<string>(departmentRange);

                DataTable allDepartment = RightManage.GetDepartmentIds(userInfo.wechatUserId);

                foreach (DataRow allDepartmentDr in allDepartment.Rows)
                {
                    if (hasDepartmentRight)
                    {
                        continue;
                    }
                    if (departmentList.Contains(allDepartmentDr["departmentId"].ToString()))
                    {
                        hasDepartmentRight = true;

                        newDt.Rows.Add(dr.ItemArray);
                    }
                }

                Boolean hasUserRight = false;
                // 如果部门没权限再判断人员名有无权限
                if (!hasDepartmentRight)
                {
                    if (hasUserRight)
                    {
                        continue;
                    }

                    List<string> userList = JsonHelper.DeserializeJsonToList<string>(userRange);
                    
                    if (userList.Contains(userInfo.userId.ToString()))
                    {
                        hasUserRight = true;
                        newDt.Rows.Add(dr.ItemArray);
                    }
                }
            }
            res.Add("data", JsonHelper.DataTable2Json(dt));
            res.Add("newData", JsonHelper.DataTable2Json(newDt));
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }

        return res.ToString();
    }

    public static string saveFormRecord(string formName, JObject formDataJObject, string processJSON,
        string informerJSON, string docId, UserInfo userInfo, string docCode)
    {
        JObject res = new JObject();

        WxCommon wx = new WxCommon("yeliForm",
           "E26TbitJpOlsniJaKMq6lrNYhiu1bKVtRddflNwIsoE",
           "1000015",
           "");

        if (string.IsNullOrEmpty(formName) || formDataJObject == null)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string sql = "";

        string dataMsg = "";
        JObject msgJObject = new JObject();
        

        if (string.IsNullOrEmpty(docId))
        {
            sql = SqlHelper.GetInsertString(formDataJObject, "wf_form_" + formName);

            dataMsg = SqlHelper.InsertAndGetLastId(sql);

            msgJObject = JsonHelper.DeserializeJsonToObject<JObject>(dataMsg);
        }
        else
        {
            sql = SqlHelper.GetUpdateString(formDataJObject, "wf_form_" + formName, "where id =" + docId);

            dataMsg = SqlHelper.Exce(sql);
        }

        if (dataMsg.Contains("操作成功") || "1".Equals(msgJObject["Success"].ToString()))
        {
            // 插入数据到审批人表中
            List<JObject> processList = JsonHelper.DeserializeJsonToList<JObject>(processJSON);

            string firstApproverUserId = "(";
            if (!processList[0]["level"].ToString().Equals("0"))
            {
                JObject jObject = new JObject
                {
                    {"level", 0},
                    {"name", userInfo.userName},
                    {"userId", userInfo.userId}
                };
                processList.Add(jObject);
            }

            foreach (JObject processJObject in processList)
            {
                processJObject.Add("tableName", formName);
                processJObject.Add("docId", msgJObject.Property("Success") == null ? docId : msgJObject["Id"].ToString());

                processJObject.Remove("name");

                if(processJObject["level"].ToString()=="1")
                {
                    firstApproverUserId += processJObject["userId"].ToString()+",";
                }
            }
            sql = string.Format("delete from wf_approver where TableName='{0}' and DocId='{1}';", formName, docId);
            sql += SqlHelper.GetInsertString(processList, "wf_approver");

            List<string> sqlList = new List<string>();
            //string insertApproverMsg = SqlHelper.Exce(sql);
            sqlList.Add(sql);

            //抄送人userId列
            string informerUserId = "(";

            JObject InformerObject = new JObject();

            if (!string.IsNullOrEmpty(informerJSON))
            {
                DataTable dt = JsonHelper.Json2Dtb(informerJSON);

                foreach (DataRow row in dt.Rows)
                {
                    InformerObject = new JObject();

                    // 插入到抄送人表
                    InformerObject.Add("TableName", formName);
                    InformerObject.Add("DocId", msgJObject.Property("Success") == null ? docId : msgJObject["Id"].ToString());
                    InformerObject.Add("UserId", row[0].ToString());

                    string tempsql = SqlHelper.GetInsertString(InformerObject, "wf_informer");

                    sqlList.Add(tempsql);

                    informerUserId += row[0].ToString() + ",";
                }
            }
            if(string.IsNullOrEmpty(formDataJObject["informer"].ToString()))
            {
                informerUserId = informerUserId.Substring(0, informerUserId.Length - 1);
                if (informerUserId == "")
                {
                    informerUserId = "(-1)";
                }
                else
                {
                    informerUserId += ")";
                }
            }
            else
            {
                foreach (string tempUserId in formDataJObject["informer"].ToString().Split(','))
                {
                    // 插入到抄送人表
                    InformerObject = new JObject();
                    InformerObject.Add("TableName", formName);
                    InformerObject.Add("DocId", msgJObject.Property("Success") == null ? docId : msgJObject["Id"].ToString());
                    InformerObject.Add("UserId", tempUserId);

                    string tempsql = SqlHelper.GetInsertString(InformerObject, "wf_informer");

                    sqlList.Add(tempsql);
                }

                informerUserId += formDataJObject["informer"].ToString()+")";
            }

            // 插入数据到审批记录表
            JObject recordJObject = new JObject();
            recordJObject.Add("tableName", formName);
            recordJObject.Add("docId", msgJObject.Property("Success") == null ? docId : msgJObject["Id"].ToString());
            recordJObject.Add("userId", userInfo.userId);
            recordJObject.Add("approvalTime", DateTime.Now.ToString());
            recordJObject.Add("approvalResult", "已提交");
            recordJObject.Add("approvalOpinion", "");

            sql = SqlHelper.GetInsertString(recordJObject, "wf_record");

            //string insertRecordMsg = SqlHelper.Exce(sql);

            sqlList.Add(sql);

            string totalMsg = SqlHelper.Exce(sqlList.ToArray());

            if (totalMsg.Contains("操作成功"))
            {
                res.Add("ErrMsg", "操作成功");
                res.Add("ErrCode", 0);

                firstApproverUserId = firstApproverUserId.Substring(0, firstApproverUserId.Length - 1);
                firstApproverUserId += ")";

                string userIdToWechatUserIdList = "select wechatUserId from users where userId in " + firstApproverUserId +
                    ";select wechatUserId from users where userId in" + informerUserId;
                DataSet ds = SqlHelper.Find(userIdToWechatUserIdList);
                string firstApproverWechatUserId = "";
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    firstApproverWechatUserId += row[0].ToString() + "|";
                }
                firstApproverWechatUserId = firstApproverWechatUserId.Substring(0, firstApproverWechatUserId.Length - 1);

                // 发送消息给第一级审批人
                wx.SendWxMsg(firstApproverWechatUserId, "审批通知",
                "您有一条单号为" + docCode + "的" + formName + "待您审批，请知悉",
                "http://yelioa.top/mFormListAndDetail.aspx?formName=" + formName + "&docId=" + recordJObject["docId"].ToString() + "&type=toBeApprovedByMe");

                if (ds.Tables[1].Rows.Count > 0)
                {
                    string informerWxUserId = "";
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        informerWxUserId += row[0].ToString() + "|";
                    }

                    informerWxUserId = informerWxUserId.Substring(0, informerWxUserId.Length - 1);

                    // 发送消息给抄送人
                    wx.SendWxMsg(informerWxUserId, "审批通知",
                    "有一条单号为" + docCode + "的" + formName + "与您相关，请知悉",
                    "http://yelioa.top/mFormListAndDetail.aspx?formName=" + formName + "&docId=" + recordJObject["docId"].ToString() + "&type=relatedToMe");
                }


            }
            else
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", "操作成功,审批人保存错误");
            }
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "表单更新失败");
        }

        return res.ToString(); ;
    }

    public static string showDraftData(string id, string formName)
    {
        JObject res = new JObject();

        if (string.IsNullOrEmpty(formName) || string.IsNullOrEmpty(id))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "参数缺少");
            return res.ToString();
        }

        string tableName = "wf_form_" + formName;

        string sql = string.Format("select * from wf_form_config where FormName = '{0}'", formName);

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
            res.Add("ErrMsg", "不存在该表单");
        }
        else
        {
            JArray parameterData = JArray.Parse(ds.Tables[0].Rows[0]["ParameterData"].ToString());
            sql = "";
            foreach(JObject field in parameterData)
            {
                if (field["RELA"] != null)
                {
                    JObject RELA = JObject.Parse(field["RELA"].ToString());
                    JObject RELA1 = JObject.Parse(RELA["RELA1"].ToString());
                    if (RELA1["TABLENM"].ToString() == "部门表")
                    {
                        sql += string.Format("select a.*,department.name {0} from {1} a left join department on a.{2}=department.Id where a.id='{3}';",
                            field["LBL"].ToString()+"1",tableName, field["LBL"].ToString(), id);
                    }
                    else if (RELA1["TABLENM"].ToString() == "用户表")
                    {
                        sql += string.Format("select a.*,users.userName {0} from {1} a left join users on a.{2}=users.userId where a.id='{3}'; ", 
                            field["LBL"].ToString() + "1", tableName, field["LBL"].ToString(), id);
                    }
                    else if (RELA1["TABLENM"].ToString() == "费用明细表")
                    {
                        sql += string.Format("select a.*,fee_detail_dict_copy.name {0} from {1} a left join fee_detail_dict_copy on a.{0}=fee_detail_dict_copy.Id where a.id='{3}'; ",
                            field["LBL"].ToString() + "1", tableName, field["LBL"].ToString(), id);
                    }
                    else if (RELA1["TABLENM"].ToString() == "网点表")
                    {
                        sql += string.Format("select a.*,fee_branch_dict.name {0} from {1} a left join fee_branch_dict on a.{0}=fee_branch_dict.Id where a.id='{3}' ;",
                            field["LBL"].ToString() + "1", tableName, field["LBL"].ToString(), id);
                    }
                }
            }
            if(sql=="")
            {
                sql = string.Format("select * from {0} where id='{1}'", tableName, id);
            }
            else
            {
                sql = sql.Substring(0, sql.Length - 1);
            }
            DataSet set = SqlHelper.Find(sql, ref msg);
            if (set == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else if (set.Tables[0].Rows.Count == 0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "不存在该单据");
            }
            else
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
                if (set.Tables.Count>1)
                {
                   for(int i=1;i<set.Tables.Count;i++)
                    {
                        foreach(DataColumn dc in set.Tables[i].Columns)
                        {
                             if(!set.Tables[0].Columns.Contains(dc.ColumnName))
                            {
                                set.Tables[0].Columns.Add(dc.ColumnName, Type.GetType("System.String"));
                                set.Tables[0].Rows[0][dc.ColumnName] = set.Tables[i].Rows[0][dc.ColumnName];
                            }
                        }
                    }
                }

                // userId和departmentId返回数据编码转成名字
                string userId = set.Tables[0].Rows[0]["userId"].ToString();
                string departmentId = set.Tables[0].Rows[0]["departmentId"].ToString();

                //sql = string.Format("select userName from users where userId = '{0}'", userId);
                set.Tables[0].Rows[0]["userId"] = userId;

                //sql = string.Format("select name from department where id = '{0}'", departmentId);
                set.Tables[0].Rows[0]["departmentId"] = departmentId;

                res.Add("data", JsonHelper.DataTable2Json(set.Tables[0]));
            }
        }

        return res.ToString();
    }
}