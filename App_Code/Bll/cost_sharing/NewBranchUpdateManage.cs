using System;
using System.Collections.Generic;
using System.Data;
using Dal.cost_sharing;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Dml;

namespace Bll.cost_sharing
{
    /// <summary>
    /// NewBranchUpdateManage 的摘要说明
    /// </summary>
    public class NewBranchUpdateManage
    {
        public NewBranchUpdateManage()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public static string getFormColumnsAndDataForApproval(JObject jObject, int level)
        {
            string costSharingRecordId = jObject["costSharingRecordId"].ToString();

            JObject res = new JObject();

            if (costSharingRecordId == null || "".Equals(costSharingRecordId))
            {
                res.Add("ErrCode", 1);
                res.Add("ErrMsg", "缺少参数");

                return res.ToString();
            }

            DataSet ds = NewBranchUpdateSrv.getFormColumnsAndDataForApproval(costSharingRecordId, level);

            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                res.Add("ErrCode", 1);
                res.Add("ErrMsg", "暂无数据");

                return res.ToString();
            }

            DataTable dt = ds.Tables[0];

            res.Add("ErrCode", 0);

            JObject detailJObject = new JObject();

            dt.Columns.Add("columnRelativeData", typeof(string));

            foreach (DataRow dr in dt.Rows)
            {
                // 表单列中加入对应的表内容基础信息
                string tableName = dr["RelativeTable"].ToString();
                DataTable columnRelativeDataTable = NewBranchUpdateSrv.showColumnRelativeData(tableName);
                if (columnRelativeDataTable != null && columnRelativeDataTable.Rows.Count > 0)
                {
                    dr["columnRelativeData"] = JsonHelper.DataTable2Json(columnRelativeDataTable);
                }

                string fieldName = dr["FieldName"].ToString();
                string fieldValueId = dr["NewValue"].ToString();
                string fieldValue = dr["OldValue"].ToString();
                string relativeFieldName = dr["RelativeFieldName"].ToString();

                detailJObject.Add(fieldName, fieldValue);
                detailJObject.Add(fieldName + "Id", fieldValueId);
                detailJObject.Add(fieldName + "relativeFieldName", relativeFieldName);
            }

            res.Add("detail", detailJObject.ToString());
            res.Add("column", JsonHelper.DataTable2Json(dt));

            return res.ToString();
        }

        public static string getFormColumnsAndDataForSubmit(JObject jObject, UserInfo userInfo)
        {
            string newCostSharingId = jObject["newCostSharingId"].ToString();
            
            JObject res = new JObject();

            if (newCostSharingId == null || "".Equals(newCostSharingId))
            {
                res.Add("ErrCode", 1);
                res.Add("ErrMsg", "缺少参数");

                return res.ToString();
            }

            DataSet ds = NewBranchUpdateSrv.getFormColumnsAndDataForSubmit(newCostSharingId, userInfo);

            if (ds == null)
            {
                res.Add("ErrCode", 1);
                res.Add("ErrMsg", "数据库查询错误");

                return res.ToString();
            }

            DataTable isUpdateFinishedTable = ds.Tables[0];
            DataTable columnDataTable = ds.Tables[1];
            DataTable contentDataTable = ds.Tables[2];

            if (isUpdateFinishedTable != null && isUpdateFinishedTable.Rows.Count > 0)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", "该网点存在未结束的更新流程，请勿重复更新");

                return res.ToString();
            }

            if (columnDataTable.Rows.Count == 0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "暂无对应列数据");
            }
            else if (contentDataTable.Rows.Count == 0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "暂无对应网点数据");
            }
            else
            {
                res.Add("ErrCode", 0);

                //res.Add("column", JsonHelper.DataTable2Json(columnDataTable));  // 表单列
                columnDataTable.Columns.Add("columnRelativeData", typeof(string));

                JObject columnDataJObjet = new JObject();

                // 从json中提取回显数据
                foreach (DataRow dr in columnDataTable.Rows)
                {
                    // 表单列中加入对应的表内容基础信息
                    string tableName = dr["RelativeTable"].ToString();
                    DataTable columnRelativeDataTable = NewBranchUpdateSrv.showColumnRelativeData(tableName);
                    if (columnRelativeDataTable != null && columnRelativeDataTable.Rows.Count > 0)
                    {
                        dr["columnRelativeData"] = JsonHelper.DataTable2Json(columnRelativeDataTable);
                    }

                    string columnName = dr["FieldName"].ToString();
                    
                    if (contentDataTable.Columns.Contains(columnName))
                    {
                        // 如果包含 则是在new_cost_sharing表中除datajson外的数据
                        string columnData = contentDataTable.Rows[0][columnName].ToString();

                        columnDataJObjet.Add(columnName, columnData);
                    }
                    else
                    {
                        // 如果不包含，则是dataJson的数据
                        string columnData = contentDataTable.Rows[0]["DataJson"].ToString();
                        JObject tempJObject = JsonHelper.DeserializeJsonToObject<JObject>(columnData);
                        if (tempJObject.Property(columnName) == null || "".Equals(tempJObject.Property(columnName).ToString()))
                        {
                            columnDataJObjet.Add(columnName, "");
                        }
                        else
                        {
                            columnDataJObjet.Add(columnName, tempJObject[columnName]);
                        }
                    }

                    string fieldType = dr["FieldType"].ToString();

                    if ("select".Equals(fieldType) && !"".Equals(columnDataJObjet[columnName].ToString()))
                    {
                        // 找到id字段
                        string relativeFieldName = dr["RelativeFieldName"].ToString();
                        if ("".Equals(relativeFieldName))
                        {
                            // 主要是处理开发人字段
                            string relativeTable = dr["RelativeTable"].ToString();
                            string tempSql = string.Format("select userid from {0} where username = '{1}'",
                                relativeTable, columnDataJObjet[columnName]);
                            try
                            {
                                columnDataJObjet.Add(columnName + "Id", SqlHelper.Find(tempSql).Tables[0].Rows[0][0].ToString());
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                columnDataJObjet.Add(columnName + "Id", 0);
                            }
                        }
                        else
                        {
                            string columnDataId = contentDataTable.Rows[0][relativeFieldName].ToString();
                            columnDataJObjet.Add(columnName + "Id", columnDataId);
                        }
                    }
                    else
                    {
                        columnDataJObjet.Add(columnName + "Id", 0);
                    }
                }

                res.Add("column", JsonHelper.DataTable2Json(columnDataTable));  // 表单列
                res.Add("detail", columnDataJObjet.ToString()); // 回显详情内容
            }

            return res.ToString();
        }

        public static string approvalOfCostSharingUpdating(JObject jObject, UserInfo userInfo)
        {
            JObject costSharingRecordJObject = new JObject();

            costSharingRecordJObject.Add("Code", jObject["costSharingRecordId"]);

            string costSharingRecordId = jObject["costSharingRecordId"].ToString();

            DataSet ds = NewBranchUpdateSrv.getCostSharingInfoById(costSharingRecordId, userInfo);

            if (ds == null)
            {
                JObject tempJObject = new JObject();
                tempJObject.Add("ErrCode", 3);
                tempJObject.Add("ErrMsg", "数据库查询错误");

                return tempJObject.ToString();
            }

            costSharingRecordJObject.Add("NewCostSharingId", ds.Tables[0].Rows[0]["newCostSharingId"].ToString());
            costSharingRecordJObject.Add("SubmitterUserId", userInfo.userId.ToString());
            costSharingRecordJObject.Add("InsertOrUpdate", 1);
            costSharingRecordJObject.Add("State", "审批中");
            costSharingRecordJObject.Add("Level", Int32.Parse(ds.Tables[0].Rows[0]["level"].ToString())+1);
            costSharingRecordJObject.Add("CreateTime", DateTime.Now);

            // 获取下级审批人
            costSharingRecordJObject.Add("ApproverUserId", ds.Tables[1].Rows[0]["userId"].ToString());

            SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(SqlHelper.GetUpdateString(costSharingRecordJObject, "cost_sharing_record", "where code = " + jObject["costSharingRecordId"])));

            if (!msg.Result.Equals(SqlExceRes.ResState.Success))
            {
                JObject tempJObject = new JObject();
                tempJObject.Add("ErrCode", 3);
                tempJObject.Add("ErrMsg", "数据库查询错误");

                return tempJObject.ToString();
            }

            // 获取页面上修改后的所有值
            JObject detailJObject = JsonHelper.DeserializeJsonToObject<JObject>(jObject["detail"].ToString());

            JObject detailJObjectForInsert = new JObject();

            detailJObjectForInsert.Add("RegistrationCode", jObject["costSharingRecordId"]);
            detailJObjectForInsert.Add("Level", Int32.Parse(ds.Tables[0].Rows[0]["level"].ToString()));
            detailJObjectForInsert.Add("ApproverUserId", userInfo.userId.ToString());
            detailJObjectForInsert.Add("CreateTime", DateTime.Now);

            List<string> detailJObjectForInsertSqls = new List<string>();

            foreach (var key in detailJObject)
            {
                if (key.Key.Contains("Id"))
                {
                    continue;
                }

                detailJObjectForInsert.Remove("FieldName");
                detailJObjectForInsert.Add("FieldName", key.Key);

                detailJObjectForInsert.Remove("NewValue");
                detailJObjectForInsert.Add("NewValue", detailJObject[key.Key + "Id"]);

                detailJObjectForInsert.Remove("OldValue");
                detailJObjectForInsert.Add("OldValue", detailJObject[key.Key]);

                string sql = SqlHelper.GetInsertString(detailJObjectForInsert, "cost_sharing_detail");

                detailJObjectForInsertSqls.Add(sql);
            }

            SqlExceRes res = new SqlExceRes(SqlHelper.Exce(detailJObjectForInsertSqls.ToArray()));

            JObject resJObject = new JObject();

            if (res.Result == SqlExceRes.ResState.Success)
            {
                // TODO 如果审批流程结束，则newCostSharing表中的数据进行更新
                JObject newCostSharingJObject = new JObject();
                JObject innerJObject = new JObject();
                foreach (var key in detailJObject)
                {
                    var relativeFieldName = detailJObject[key.Key + "relativeFieldName"].ToString();
                    if (relativeFieldName == null || "".Equals(relativeFieldName))
                    {
                        innerJObject.Add(key.Key, detailJObject[key.Key + "Id"]);
                    }
                    else
                    {
                        newCostSharingJObject.Add(relativeFieldName, detailJObject[key.Key + "Id"]);
                    }
                }
                
                newCostSharingJObject.Add("DataJson", innerJObject);
                SqlExceRes tempRes = new SqlExceRes(SqlHelper.Exce(SqlHelper.GetUpdateString(newCostSharingJObject, "newCostSharing", "where id = " + ds.Tables[0].Rows[0]["newCostSharingId"])));SqlHelper.Exce(SqlHelper.GetUpdateString(newCostSharingJObject, "newCostSharing", "where id = " + ds.Tables[0].Rows[0]["newCostSharingId"]));

                if (tempRes.Result == SqlExceRes.ResState.Success)
                {
                    // 插入成功
                    resJObject.Add("ErrCode", 0);
                    resJObject.Add("ErrMsg", "修改成功");
                }
                else
                {
                    resJObject.Add("ErrCode", 2);
                    resJObject.Add("ErrMsg", "修改失败");
                }
            }
            else
            {
                // 插入失败
                resJObject.Add("ErrCode", 3);
                resJObject.Add("ErrMsg", "修改失败");
            }

            return resJObject.ToString();
        }

        public static string submissionOfCostSharingUpdating(JObject jObject, UserInfo userInfo)
        {
            JObject costSharingRecordJObject = new JObject();

            string code = GenerateDocCode.getUpdateCostSharingCode();

            costSharingRecordJObject.Add("Code", code);
            costSharingRecordJObject.Add("NewCostSharingId", jObject["newCostSharingId"]);
            costSharingRecordJObject.Add("SubmitterUserId", userInfo.userId.ToString());
            costSharingRecordJObject.Add("InsertOrUpdate", 1);
            costSharingRecordJObject.Add("State", "审批中");
            costSharingRecordJObject.Add("Level", 1);
            costSharingRecordJObject.Add("CreateTime", DateTime.Now);
    
            // 获取下级审批人
            string findApproverUserIdSql = string.Format("(select case when ((select directorId from new_cost_sharing where id = {0}) " +
             "is not null and (select directorId from new_cost_sharing where id = {0}) != '100000145' " +
             "and (select managerId from new_cost_sharing where id = {0}) != '100000154' " +
             "and (select managerId from new_cost_sharing where id = {0}) != '100000180') " +
             "then (SELECT DirectorId FROM `new_cost_sharing` where id = {0}) " +
             "else (SELECT managerId FROM `new_cost_sharing` where id = {0}) end ApproverUserId)", jObject["newCostSharingId"]);
            costSharingRecordJObject.Add("ApproverUserId", findApproverUserIdSql);

            List<string> withoutQuoteColumn = new List<string>();
            withoutQuoteColumn.Add("ApproverUserId");

            JObject InsertAndGetLastIdJObject = JsonHelper.DeserializeJsonToObject<JObject>(SqlHelper.InsertAndGetLastId(
                SqlHelper.GetInsertStringWithoutQuotes(costSharingRecordJObject, "cost_sharing_record", withoutQuoteColumn)));

            JObject resJObject = new JObject();

            // 找到刚插入的记录的id
            if ("1".Equals(InsertAndGetLastIdJObject["Success"].ToString()) || "-1".Equals(InsertAndGetLastIdJObject["Id"].ToString()))
            {
                resJObject.Add("ErrCode", 3);
                resJObject.Add("ErrMsg", "数据库插入失败");

                return resJObject.ToString();
            }

            string lastId = InsertAndGetLastIdJObject["Id"].ToString();

            // 获取页面上修改后的所有值
            JObject detailJObject = JsonHelper.DeserializeJsonToObject<JObject>(jObject["detail"].ToString());

            JObject detailJObjectForInsert = new JObject();

            detailJObjectForInsert.Add("RegistrationCode", code);
            detailJObjectForInsert.Add("Level", 0);
            detailJObjectForInsert.Add("ApproverUserId", userInfo.userId.ToString());
            detailJObjectForInsert.Add("CreateTime", DateTime.Now);

            List<string> detailJObjectForInsertSqls = new List<string>();

            foreach (var key in detailJObject)
            {
                if (key.Key.Contains("Id"))
                {
                    continue;
                }

                detailJObjectForInsert.Remove("FieldName");
                detailJObjectForInsert.Add("FieldName", key.Key);

                detailJObjectForInsert.Remove("NewValue");
                detailJObjectForInsert.Add("NewValue", detailJObject[key.Key+"Id"]);

                detailJObjectForInsert.Remove("OldValue");
                detailJObjectForInsert.Add("OldValue", detailJObject[key.Key]);

                string sql = SqlHelper.GetInsertString(detailJObjectForInsert, "cost_sharing_detail");

                detailJObjectForInsertSqls.Add(sql);
            }

            SqlExceRes res = new SqlExceRes(SqlHelper.Exce(detailJObjectForInsertSqls.ToArray()));

            if (res.Result == SqlExceRes.ResState.Success)
            {
                // 插入成功
                resJObject.Add("ErrCode", 0);
                resJObject.Add("ErrMsg", "修改成功");
            }
            else
            {
                // 插入失败
                resJObject.Add("ErrCode", 3);
                resJObject.Add("ErrMsg", "修改失败");
            }

            return resJObject.ToString();
        }
    }
}