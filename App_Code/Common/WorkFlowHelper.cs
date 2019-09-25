using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data;

/// <summary>
/// WorkFlowHelper 的摘要说明
/// </summary>
public class WorkFlowHelper
{
    public WorkFlowHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 获取审批流程配置结果
    /// </summary>
    /// <param name="formId">表单id</param>
    /// <returns></returns>
    public static string GetTreeJson(string formId)
    {
        JObject res = new JObject();
        string sql = string.Format("select * from wf_process where FormId={0}", formId);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", msg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "未找该表单对应流程");
        }
        else
        {
            WorkFlowTreeHelper wfTree = new WorkFlowTreeHelper(ds.Tables[0]);
            if (wfTree.tree == null)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "流程列表加载失败，错误信息：" + wfTree.ErrMsg);
            }
            else
            {
                res.Add("ErrCode", 0);
                res.Add("tree", wfTree.GetJson());
            }
        }
        return res.ToString();
    }

    /// <summary>
    /// 修改或者增加某个流程
    /// </summary>
    /// <param name="DefaultProcessJson">该流程的审批流程</param>
    /// <param name="DefaultInformerJson">该流程的抄送人</param>
    /// <param name="ConditionItem">该流程的条件项</param>
    /// <param name="ConditionJson">该流程的条件</param>
    /// <param name="ConditionName">条件名称</param>
    /// <param name="processId">该流程ID</param>
    /// <returns></returns>
    public static string updateOrAddProcess(string DefaultProcessJson, string DefaultInformerJson,
        string ConditionItem, string ConditionJson, string ConditionName, string processId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(processId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            List<string> sqlList = new List<string>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("DefaultProcessJson", DefaultProcessJson);
            dict.Add("DefaultInformerJson", DefaultInformerJson);
            dict.Add("ConditionItem", ConditionItem);
            dict.Add("ConditionJson", ConditionJson);
            dict.Add("ConditionName", ConditionName);
            sqlList.Add("delete from wf_process where Id=" + processId);
            sqlList.Add(SqlHelper.GetUpdateString(dict, "wf_process", "where id=" + processId));
            SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sqlList.ToString()));
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
        }
        return res.ToString();
    }
    /// <summary>
    /// 保存审批流程配置结果
    /// </summary>
    /// <param name="formId">表单id</param>
    /// <param name="dataJson">审批流程treeData</param>
    /// <returns></returns>
    public static string SaveTree(string formId, string dataJson)
    {
        JObject res = new JObject();
        DataTable dt = JsonHelper.Json2Dtb(dataJson);
        if (string.IsNullOrEmpty(formId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            dt.Columns.Add("FormId", Type.GetType("System.String"));
            foreach (DataRow row in dt.Rows)
            {
                row["FormId"] = formId;
            }
            SqlExceRes r = new SqlExceRes(SqlHelper.Exce(SqlHelper.GetInsertString(dt, "wf_process")));
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
        }
        return res.ToString();
    }

    /// <summary>
    /// 获取表单的所有流程(table形式）
    /// </summary>
    /// <param name="formId">表单ID</param>
    /// <returns></returns>
    public static string getFormProcess(string formId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(formId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string msg = "";
            DataSet ds = SqlHelper.Find("select * from wf_process where formId=" + formId, ref msg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "未找到相关表单的审批流");
            }
            else
            {
                res.Add("Errcode", 0);
                res.Add("ErrMsg", "获取审批流成功");
                res.Add("process", JsonHelper.DataTable2Json(ds.Tables[0]));
            }
        }
        return res.ToString();
    }

    /// <summary>
    /// 获取表单的所有流程 因为存的是id 所以需要转成name来回显(table形式）
    /// </summary>
    /// <param name="formId">表单ID</param>
    /// <returns></returns>
    public static string getFormProcessOther(string formId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(formId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string msg = "";
            DataSet ds = SqlHelper.Find("select * from wf_process where formId=" + formId, ref msg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "未找到相关表单的审批流");
            }
            else
            {
                JArray defaultProcessJsonArray = JArray.Parse(ds.Tables[0].Rows[0]["DefaultProcessJson"].ToString());
                foreach (JObject defaultProcessJsonObject in defaultProcessJsonArray)
                {
                    if (defaultProcessJsonObject.Property("userId") != null && defaultProcessJsonObject.Property("userId").ToString() != "")
                    {
                        string userId = defaultProcessJsonObject["userId"].ToString();

                        string userName = SqlHelper.Find("select userName from users where userId = " + userId).Tables[0].Rows[0][0].ToString();

                        defaultProcessJsonObject.Add("name", userName);
                    }

                    if (defaultProcessJsonObject.Property("departmentId") != null && defaultProcessJsonObject.Property("departmentId").ToString() != "")
                    {
                        string departmentId = defaultProcessJsonObject["departmentId"].ToString();

                        string departmentName = SqlHelper.Find("select name from department where id = " + departmentId).Tables[0].Rows[0][0].ToString();

                        defaultProcessJsonObject.Add("name", departmentName);
                    }
                }

                ds.Tables[0].Rows[0]["DefaultProcessJson"] = defaultProcessJsonArray.ToString();

                res.Add("Errcode", 0);
                res.Add("ErrMsg", "获取审批流成功");
                res.Add("process", JsonHelper.DataTable2Json(ds.Tables[0]));
            }
        }

        return res.ToString();
    }


    /// <summary>
    /// 删除流程（包括其所有子流程）
    /// </summary>
    /// <param name="processId">流程ID</param>
    /// <param name="formId">表单ID</param>
    /// <returns></returns>
    public static string deleteProcess(string processId, string formId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(formId) || string.IsNullOrEmpty(processId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            JObject processJB = JObject.Parse(getFormProcess(formId));
            if (Convert.ToInt32(processJB["ErrCode"]) == 0)
            {
                DataTable dt = JsonHelper.Json2Dtb(processJB["process"]["rows"].ToString());
                WorkFlowTreeHelper WFTP = new WorkFlowTreeHelper(dt);
                WorkFlowTreeNode targetNode = idToNode(WFTP.tree[0], processId);
                ArrayList processIdList = getNodeIdList(targetNode);
                string sql = "delete from wf_process where Id in('";
                foreach (string id in processIdList)
                {
                    sql += id + "','";
                }
                sql = sql.Substring(0, sql.Length - 1);
                sql += ")";
                SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sql));
                if (r.Result == SqlExceRes.ResState.Success)
                {
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");
                }
                else
                {
                    res.Add("ErrCode", 4);
                    res.Add("ErrMsg", r.ExceMsg);
                }
            }
            else res = processJB;
        }
        return res.ToString();
    }
    /// <summary>
    /// 获取指定节点的所有子节点（包括本身）ID
    /// </summary>
    /// <param name="targetNode">目标节点</param>
    /// <returns></returns>
    private static ArrayList getNodeIdList(WorkFlowTreeNode targetNode)
    {
        ArrayList processIdList = new ArrayList();
        if (targetNode.children.Count > 0)
        {
            foreach (WorkFlowTreeNode node in targetNode.children)
            {
                ArrayList list = getNodeIdList(node);
                if (list != null && list.Count > 0)
                {
                    foreach (string id in list)
                    {
                        processIdList.Add(id);
                    }
                }
            }
        }
        processIdList.Add(targetNode.id);
        return processIdList;
    }
    /// <summary>
    /// 根据节点ID获取节点
    /// </summary>
    /// <param name="rootNode">根节点</param>
    /// <param name="processId">流程（节点）ID</param>
    /// <returns></returns>
    private static WorkFlowTreeNode idToNode(WorkFlowTreeNode rootNode, string processId)
    {
        WorkFlowTreeNode TargetNode = null;
        if (processId == rootNode.id.ToString())
        {
            TargetNode = rootNode;
        }
        else
        {
            foreach (WorkFlowTreeNode node in rootNode.children)
            {
                TargetNode = idToNode(node, processId);
                if (TargetNode != null)
                    break;
            }
        }
        return TargetNode;
    }

    /// <summary>
    /// 获取单据审批人
    /// </summary>
    /// <param name="formId">表单ID</param>
    /// <param name="docId">单据ID</param>
    /// <returns></returns>
    public static string GetApprover(string formId, string doc)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(formId) || string.IsNullOrEmpty(doc))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string msg = "";
            DataTable dt = JsonHelper.Json2Dtb(doc);
            JObject docJobject = new JObject();
            foreach (DataRow row in dt.Rows)
            {
                docJobject.Add(row["name"].ToString(), row["value"].ToString());
            }
            string userId = docJobject["userId"].ToString();
            string departmentId = docJobject["departmentId"].ToString();

            string sql = string.Format("select * from wf_form_config where Id='{0}';", formId);
            sql += string.Format("select * from wf_process where FormId={0};", formId);
            sql += string.Format("SELECT * FROM `v_user_department_post` order by Id;");
            sql += string.Format("select *from department");
            DataSet ds = SqlHelper.Find(sql, ref msg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {
                res.Add("Errcode", 3);
                res.Add("ErrMsg", "未找到该表单");
            }
            else if (ds.Tables[1].Rows.Count == 0)
            {
                res.Add("Errcode", 4);
                res.Add("ErrMsg", "未找到相关表单的审批流");
            }
            else
            {
                WorkFlowTreeHelper WFTP = new WorkFlowTreeHelper(ds.Tables[1]);
                if (WFTP.tree == null)
                {
                    res.Add("ErrCode", 5);
                    res.Add("ErrMsg", "流程列表加载失败，错误信息：" + WFTP.ErrMsg);
                }
                else
                {
                    int id = selectProcessNode(WFTP.tree[0], docJobject);
                    DataRow selectedNode = idToNode(id, ds.Tables[1]);

                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "获取审批流成功");
                    res.Add("process", JsonToProcess(selectedNode["DefaultProcessJson"].ToString(), userId, departmentId, ds.Tables[0], docJobject, ds.Tables[2], ds.Tables[3]));
                    res.Add("informer", selectedNode["DefaultInformerJson"].ToString());
                }
            }
        }
        return res.ToString();
    }

    /// <summary>
    /// 将流程Json转换成审批人列表
    /// </summary>
    /// <param name="DefaultProcessJson">流程Json</param>
    /// <param name="wechatUserId">申请人ID</param>
    /// <param name="departmentID">申请人所在部门ID（有多部门的申请人才有此字段，单部门申请人此项为""）</param>
    /// <param name="formConfig">表单定义的相关数据</param>
    /// <param name="doc">单据数据</param>
    ///<param name="approverDT">公司所有人表单</param>
    /// <returns></returns>
    private static string JsonToProcess(string DefaultProcessJson, string userId, string departmentId, DataTable formConfig, JObject doc, DataTable approverDT, DataTable departmentDT)
    {
        JArray DefaultProcess = JArray.Parse(DefaultProcessJson);
        DataTable dt = new DataTable();
        dt.Columns.Add("level", Type.GetType("System.String"));
        dt.Columns.Add("userId", Type.GetType("System.String"));
        dt.Columns.Add("name", Type.GetType("System.String"));
        int level = 0;
        string dptId = departmentId;
        for (int i = 0; i < DefaultProcess.Count; i++)
        {
            if (i == 0)
            {
                foreach (DataRow row in approverDT.Rows)
                {
                    if (userId == row["userId"].ToString())
                    {
                        dt.Rows.Add(level++, row["userId"].ToString(), row["userName"].ToString());
                        if (row["isHead"].ToString() == "1")
                        {
                            foreach (DataRow dw in departmentDT.Rows)
                            {
                                if (dptId == dw["Id"].ToString())
                                {
                                    dptId = dw["parentId"].ToString();
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            if (DefaultProcess[i]["type"].ToString() == "User")//指定人员审批
            {
                foreach (DataRow row in approverDT.Rows)
                {
                    if (DefaultProcess[i]["userId"].ToString() == row["userId"].ToString())
                    {
                        dt.Rows.Add(level++, row["userId"].ToString(), row["userName"].ToString());

                        break;
                    }
                }
            }
            else if (DefaultProcess[i]["type"].ToString() == "OneSuperior")//上级领导审批
            {
                DataTable DT = FindDepartmentHead(approverDT, departmentDT, DefaultProcess[i]["mode"].ToString(), ref dptId, ref level);
                if (DT != null)
                {
                    foreach (DataRow row in DT.Rows)
                    {
                        dt.ImportRow(row);
                    }
                }
            }
            else if (DefaultProcess[i]["type"].ToString() == "SuperiorUntil")//一直往上审批，直至某个部门为止
            {
                JArray groupJArry = JsonHelper.DeserializeJsonToObject<JArray>(DefaultProcess[i]["group"].ToString());
                List<string> group = new List<string>();
                foreach (JObject groupJObject in groupJArry)
                {
                    group.Add(groupJObject["departmentId"].ToString());
                }
                while (!group.Contains(dptId))
                {
                    DataTable DT1 = FindDepartmentHead(approverDT, departmentDT, DefaultProcess[i]["mode"].ToString(), ref dptId, ref level);
                    if (DT1 != null)
                    {
                        foreach (DataRow row in DT1.Rows)
                        {
                            dt.ImportRow(row);
                        }
                    }
                    if (dptId == "0")
                        break;

                }
                DataTable DT = FindDepartmentHead(approverDT, departmentDT, DefaultProcess[i]["mode"].ToString(), ref dptId, ref level);
                if (DT != null)
                {
                    foreach (DataRow row in DT.Rows)
                    {
                        dt.ImportRow(row);
                    }
                }

            }
            else if (DefaultProcess[i]["type"].ToString() == "Association")//关联字段审批
            {
                JArray fields = JArray.Parse(formConfig.Rows[0]["ParameterData"].ToString());
                foreach (JObject fd in fields)
                {
                    if (fd["LBL"].ToString() == DefaultProcess[i]["AssociatedFields"].ToString())
                    {
                        JObject RELA1 = JObject.Parse(JObject.Parse(fd["RELA"].ToString())["RELA1"].ToString());
                        if (RELA1["TABLENM"].ToString() == "部门表")
                        {
                            string dpt1 = doc[fd["LBL"].ToString()].ToString();
                            DataTable DT = FindDepartmentHead(approverDT, departmentDT, DefaultProcess[i]["mode"].ToString(), ref dpt1, ref level);
                            if (DT != null)
                            {
                                foreach (DataRow row in DT.Rows)
                                {
                                    dt.ImportRow(row);
                                }
                            }

                        }
                        else
                        {
                            foreach (DataRow row in approverDT.Rows)
                            {
                                if (doc[fd["LBL"].ToString()].ToString() == row["userId"].ToString())
                                {
                                    dt.Rows.Add(level++, row["userId"].ToString(), row["userName"].ToString());

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (DefaultProcess[i]["type"].ToString() == "Department")//指定部门审批
            {
                string dpt1 = DefaultProcess[i]["departmentId"].ToString();
                DataTable DT = FindDepartmentHead(approverDT, departmentDT, DefaultProcess[i]["mode"].ToString(), ref dpt1, ref level);
                if (DT != null)
                {
                    foreach (DataRow row in DT.Rows)
                    {
                        dt.ImportRow(row);
                    }
                }

            }
        }
        for (int i = dt.Rows.Count - 1; i > 0; i--)
        {
            DataRow[] rows = dt.Select(string.Format("{0}='{1}'", "userId", dt.Rows[i]["userId"]));
            if (rows.Length > 1)
            {
                dt.Rows.RemoveAt(i);
            }
        }
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            dt.Rows[i]["Level"] = i;
        }
        return JsonHelper.Dtb2Json(dt);
    }



    private static DataTable FindDepartmentHead(DataTable dt, DataTable departmentDT, string mode, ref string dptId, ref int level)
    {
        string ID = "";
        DataTable DT = new DataTable();
        DT.Columns.Add("level", Type.GetType("System.String"));
        DT.Columns.Add("userId", Type.GetType("System.String"));
        DT.Columns.Add("name", Type.GetType("System.String"));
        foreach (DataRow row in dt.Rows)
        {
            if (dptId == row["departmentId"].ToString() && row["isHead"].ToString() == "1")
            {
                DT.Rows.Add(level, row["userId"].ToString(), row["userName"].ToString());
                ID = row["departmentParentId"].ToString();

                if (mode == "And")
                {
                    level++;
                }
            }
        }
        if (ID != "")
        {
            dptId = ID;
        }
        else
        {
            foreach (DataRow row in departmentDT.Rows)
            {
                if (dptId == row["Id"].ToString())
                {
                    dptId = row["parentId"].ToString();
                    break;
                }
            }
            if (dptId == "0")
            {
                return null;
            }
            else
            {
                return FindDepartmentHead(dt, departmentDT, mode, ref dptId, ref level);
            }
        }
        if (mode != "And")
        {
            level++;
        }
        return DT;
    }

    /// <summary>
    /// 根据节点ID获取节点
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <param name="dt">所有节点组成的table</param>
    /// <returns></returns>
    private static DataRow idToNode(int id, DataTable dt)
    {
        DataRow selectNode = null;
        foreach (DataRow row in dt.Rows)
        {
            if (Convert.ToInt32(row["id"]) == id)
            {
                selectNode = row;
            }
        }
        return selectNode;
    }

    /// <summary>
    /// 查找与表单单据相符合的流程
    /// </summary>
    /// <param name="rootnode">流程树根节点</param>
    /// <param name="row">单据</param>
    /// <returns></returns>
    private static int selectProcessNode(WorkFlowTreeNode rootnode, JObject doc)
    {
        int id = rootnode.id;
        if (rootnode.ParentId == -1 && rootnode.children.Count == 0)
            return id;
        else
        {
            int flag = 0;
            if (rootnode.ParentId != -1)
            {
                JObject condition = JObject.Parse(rootnode.ConditionJson);
                //数字类型
                if (condition["type"].ToString() == "Number")
                {
                    double number = Convert.ToDouble(doc[rootnode.ConditionItem]);
                    if (condition["max"].ToString() == "" || condition["min"].ToString() == "")
                    {
                        if (condition["operator"].ToString() == "GT" && number > Convert.ToDouble(condition["min"]))
                        {
                            flag = 1;
                        }
                        else if (condition["operator"].ToString() == "LT" && number < Convert.ToDouble(condition["max"]))
                        {
                            flag = 1;
                        }
                        else if (condition["operator"].ToString() == "LTOET" && number <= Convert.ToDouble(condition["max"]))
                        {
                            flag = 1;
                        }
                        else if (condition["operator"].ToString() == "GTOET" && number >= Convert.ToDouble(condition["min"]))
                        {
                            flag = 1;
                        }
                    }
                    else
                    {
                        double min = Convert.ToDouble(condition["min"]);
                        double max = Convert.ToDouble(condition["max"]);
                        if (condition["operator"].ToString() == "LT&GT" && number < max && number > min)
                        {
                            flag = 1;
                        }
                        else if (condition["operator"].ToString() == "LTOET&GT" && number <= max && number > min)
                        {
                            flag = 1;
                        }
                        else if (condition["operator"].ToString() == "LT&GTOET" && number < max && number >= min)
                        {
                            flag = 1;
                        }
                        else if (condition["operator"].ToString() == "LTOET&GTOET" && number <= max && number >= min)
                        {
                            flag = 1;
                        }
                    }
                }
                else
                {//字符串类型
                    string str = Convert.ToString(doc[rootnode.ConditionItem]);
                    DataTable dt = JsonHelper.Json2Dtb(condition["selected"].ToString());
                    foreach (DataRow dw in dt.Rows)
                    {
                        if (str == dw[0].ToString())
                        {
                            flag = 1;
                            break;
                        }
                    }
                }
            }
            else
            {
                flag = 1;
            }
            if (flag == 0)
            {
                id = rootnode.ParentId;
            }
            else if (rootnode.children.Count == 0)
            {
                id = rootnode.id;
            }
            else
            {
                foreach (WorkFlowTreeNode node in rootnode.children)
                {
                    int nodeid = selectProcessNode(node, doc);
                    if (id != nodeid)
                    {
                        id = nodeid;
                        break;
                    }
                }
            }
            return id;
        }
    }



    /// <summary>
    /// 审批单据
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="docId">单据ID</param>
    /// <param name="userId">审批人ID</param>
    /// <param name="result">审批结果（已拒绝、已审批）</param>
    /// <param name="opinion">意见原因</param>
    /// <returns></returns>
    public static string Approving(string tableName, string docId, string userId, string result, string opinion,
        string hospitalCode, string agentCode)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(docId)
            || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(result))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string msg = "";
            WxCommon wx = new WxCommon("yeliForm",
           "E26TbitJpOlsniJaKMq6lrNYhiu1bKVtRddflNwIsoE",
           "1000015",
           "");

            string sql = string.Format("SELECT a.*,b.wechatUserId,b.userName from wf_approver a LEFT JOIN users b on a.UserId=b.userId WHERE" +
                " a.TableName='{0}' AND a.DocId='{1}' order by a.Level;", tableName, docId);
            sql += string.Format("select Level from wf_approver where TableName='{0}' and DocId='{1}' and UserId='{2}';", tableName, docId, userId);
            sql += string.Format("select max(Level) Level from wf_approver where TableName='{0}' and DocId='{1}';", tableName, docId);
            sql += string.Format("select DocCode, Level from wf_form_{0} where Id='{1}' ", tableName, docId);

            DataSet ds = SqlHelper.Find(sql, ref msg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "未找到审批人");
            }
            else if (Int32.Parse(ds.Tables[1].Rows[0][0].ToString()) != Int32.Parse(ds.Tables[3].Rows[0][1].ToString()))
            {
                res.Add("ErrCode", 4);
                res.Add("ErrMsg", "该单据你已审批，请勿重复审批");
            }
            else
            {
                string level = ds.Tables[1].Rows[0][0].ToString();
                int sameLevelNumber = 0;
                string docCode = ds.Tables[3].Rows[0][0].ToString();
                List<string> approverWeChatUserIds = new List<string>();
                Boolean flag = false;

                for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                {
                    approverWeChatUserIds.Add(ds.Tables[0].Rows[i]["wechatUserId"].ToString());
                    if (level == ds.Tables[0].Rows[i]["Level"].ToString())
                    {
                        List<string> sqlList = new List<string>();
                        sqlList.Add(string.Format("insert into wf_record (TableName,DocId,UserId,ApprovalResult," +
                           "ApprovalTime,ApprovalOpinion) values ('{0}','{1}','{2}','{3}','{4}','{5}');",
                           tableName, docId, userId, result, DateTime.Now.ToString(), opinion));

                        JObject message2 = new JObject();

                        JObject message1 = new JObject();
                        JObject MsgOutlet = null;

                        SqlExceRes r;

                        //审批人拒绝
                        if (result == "已拒绝")
                        {
                            sqlList.Add("update wf_form_" + tableName + " set Level = 0" + ", Status='" + result + "' where Id = " + docId + "; ");
                            r = new SqlExceRes(SqlHelper.Exce(sqlList.ToArray()));

                            if (r.Result == SqlExceRes.ResState.Success)
                            {
                                string beforeApproverWeChatUserIds = "";
                                foreach (string approverWxCahtUserId in approverWeChatUserIds)
                                {
                                    beforeApproverWeChatUserIds += approverWxCahtUserId + "|";
                                }
                                //发送审批消息给单据填写人
                                message1 = JObject.Parse(wx.SendWxMsg(ds.Tables[0].Rows[0]["wechatUserId"].ToString(), "审批通知",
                                       "您提交的单号为" + docCode + "的" + tableName + "已被" + ds.Tables[0].Rows[i]["userName"].ToString()
                                       + "拒绝，请知悉",
                                       "http://yelioa.top/mFormListAndDetail.aspx?formName=" + tableName + "&docId=" + docId + "&type=submitedByMe"));

                                //发送审批消息给单据之前的审批人
                                message2 = JObject.Parse(wx.SendWxMsg(beforeApproverWeChatUserIds.Substring(0, beforeApproverWeChatUserIds.Length - 1), "审批通知",
                                           "您审批的单号为" + docCode + "的" + tableName + "已被" + ds.Tables[0].Rows[i]["userName"].ToString()
                                           + "拒绝,请知悉",
                                           "http://yelioa.top/mFormListAndDetail.aspx?formName=" + tableName + "&docId=" + docId + "&type=hasApprovedByMe"));
                            }
                        }

                        //流程结束
                        else if (ds.Tables[1].Rows[0]["Level"].ToString() == ds.Tables[2].Rows[0]["Level"].ToString())
                        {
                            sqlList.Add("update wf_form_" + tableName + " set Level = Level+1" + ", Status='" + result + "' where Id = " + docId + "; ");
                            r = new SqlExceRes(SqlHelper.Exce(sqlList.ToArray()));

                            if (r.Result == SqlExceRes.ResState.Success)
                            {
                                //发送审批消息给单据填写人
                                message1 = JObject.Parse(wx.SendWxMsg(ds.Tables[0].Rows[0]["wechatUserId"].ToString(), "审批通知",
                                       "您提交的单号为" + docCode + "的" + tableName + "已被" + ds.Tables[0].Rows[i]["userName"].ToString()
                                       + "审批通过，审批流程结束，请知悉",
                                       "http://yelioa.top/mFormListAndDetail.aspx?formName=" + tableName + "&docId=" + docId + "&type=submitedByMe"));


                                message2.Add("errcode", 0);

                                ////////////////////////////////////////////////////////////
                                /// YYY 添加 完成网点备案表单后网点信息修改
                                /// /////////////////////////////////////////////////////

                                if (tableName == "网点备案新增申请表")
                                {
                                    insertHospital(tableName, docId, hospitalCode, agentCode);
                                    MsgOutlet = AddOutletBaseInfo(docCode);
                                }
                                else if (tableName == "网点备案变更申请表")
                                {
                                    insertHospital(tableName, docId, "", agentCode);
                                    MsgOutlet = UpdateOutletBaseInfo(docCode);
                                }
                                else if (tableName == "网点价格费用及指标新增申请表" || tableName == "网点价格费用及指标变更申请表")
                                {
                                    MsgOutlet = UpdateOutletCostInfo(docCode, tableName);
                                }
                            }
                        }

                        //审批通过，但流程未结束
                        else
                        {
                            sqlList.Add("update wf_form_" + tableName + " set Level = Level+1" + " where Id = " + docId + "; ");
                            string wechatUserIds = "";
                            for (int j = i + 1; j < ds.Tables[0].Rows.Count; j++)
                            {
                                if ((Convert.ToInt32(level) + 1).ToString() == ds.Tables[0].Rows[j]["Level"].ToString())
                                {
                                    if (approverWeChatUserIds.Contains(ds.Tables[0].Rows[j]["wechatUserId"].ToString()))
                                        flag = true;
                                    wechatUserIds += ds.Tables[0].Rows[j]["wechatUserId"].ToString() + "|";
                                    sameLevelNumber++;
                                }
                            }

                            r = new SqlExceRes(SqlHelper.Exce(sqlList.ToArray()));

                            if (r.Result == SqlExceRes.ResState.Success)
                            {

                                message1 = JObject.Parse(wx.SendWxMsg(ds.Tables[0].Rows[0]["wechatUserId"].ToString(), "审批通知",
                                            "您提交的单号为" + docCode + "的" + tableName + "已被" + ds.Tables[0].Rows[i]["userName"].ToString()
                                            + "审批通过,请知悉",
                                            "http://yelioa.top/mFormListAndDetail.aspx?formName=" + tableName + "&docId=" + docId + "&type=submitedByMe"));



                                message2 = JObject.Parse(wx.SendWxMsg(wechatUserIds, "审批通知",
                                  "您有一条单号为" + docCode + "的" + tableName + "待您审批，请知悉",
                                  "http://yelioa.top/mFormListAndDetail.aspx?formName=" + tableName + "&docId=" + docId + "&type=toBeApprovedByMe"));

                            }
                        }

                        if (flag && r.Result == SqlExceRes.ResState.Success && message1["errcode"].ToString() == "0" && message2["errcode"].ToString() == "0")
                        {
                            i += sameLevelNumber;
                        }
                        else if (r.Result == SqlExceRes.ResState.Success && message1["errcode"].ToString() == "0" && message2["errcode"].ToString() == "0")
                        {
                            if (MsgOutlet == null)
                            {
                                res.Add("ErrCode", 0);
                                res.Add("ErrMsg", "操作成功，发送消息成功");
                                break;
                            }
                            else if (MsgOutlet["ErrCode"].ToString() == "0")
                            {
                                res.Add("ErrCode", 0);
                                res.Add("ErrMsg", "操作成功，发送消息成功，网点信息保存成功");
                                break;
                            }
                            else
                            {
                                res.Add("ErrCode", 6);
                                res.Add("ErrMsg", "操作成功，发送消息成功，网点信息保存失败：" + MsgOutlet["ErrMsg"]);
                                break;
                            }
                        }
                        else if (r.Result == SqlExceRes.ResState.Success)
                        {
                            res.Add("ErrCode", 4);
                            res.Add("ErrMsg", "操作成功，发送消息失败");
                            break;
                        }
                        else
                        {
                            res.Add("ErrCode", 5);
                            res.Add("ErrMsg", "操作失败");
                            break;
                        }
                    }
                }
            }
        }
        return res.ToString();
    }

    private static string insertHospital(string formName, string formId, string hospitalCode, string agentCode)
    {
        var sqlList = new List<string>();

        string sql = ""; int id = 0;

        if (!string.IsNullOrEmpty(hospitalCode))
        {
            DataTable hospitalDs = SqlHelper.Find(string.Format("select clientName from new_client where clientCode = '{0}'", hospitalCode)).Tables[0];

            if (hospitalDs.Rows.Count == 0)
            {
                // 如果填写的是新网点编码
                string exHospitalName = SqlHelper.Find(string.Format("select 医院 from wf_form_{0} where id = {1}", formName, formId)).Tables[0].Rows[0][0].ToString();
                string temp_sql = String.Format("insert into new_client (clientCode, clientName) values ('{0}', '{1}')", hospitalCode, exHospitalName);
                JObject jobject = JsonHelper.DeserializeJsonToObject<JObject>(SqlHelper.InsertAndGetLastId(temp_sql));
                id = int.Parse(jobject["Id"].ToString());
            }

            sql = string.Format("update wf_form_{0} set 医院 = '{1}' where id = {2}", formName, id, formId);
            sqlList.Add(sql);
        }

        if (formName == "网点备案新增申请表")
        {
            // 新增数据 到new_client_product_user表中
            DataRow personDr = SqlHelper.Find(string.Format("select 代表,主管,区域经理,销售负责人,产品 from wf_form_{0} where id = {1}", formName, formId)).Tables[0].Rows[0];

            string productCode = personDr["产品"].ToString();

            foreach (DataColumn column in personDr.Table.Columns)
            {
                sql = string.Format("insert into new_client_product_users (clientCode, productCode, userId) values ('{0}', '{1}', '{2}')", hospitalCode, productCode, personDr[column.ColumnName]);
                sqlList.Add(sql);
            }
        }
        else if (formName == "网点备案变更申请表")
        {
            DataRow personDr = SqlHelper.Find(string.Format("select 变更前代表,代表,变更前主管,主管,变更前区域经理,区域经理," +
                "变更前销售负责人,销售负责人,产品 from wf_form_{0} where id = {1}", formName, formId)).Tables[0].Rows[0];

            foreach (DataColumn column in personDr.Table.Columns)
            {
                if (column.ColumnName.Contains("变更前"))
                {
                    sql = string.Format("delete from new_client_product_users where clientCode = '{0}' and productCode = '{1}' and userId = '{2}'", hospitalCode, personDr["产品"], personDr[column.ColumnName]);
                    sqlList.Add(sql);
                }
                else
                {
                    sql = string.Format("insert into new_client_product_users (clientCode, productCode, userId) values ('{0}', '{1}', '{2}')", hospitalCode, personDr["产品"], personDr[column.ColumnName]);
                    sqlList.Add(sql);
                }
            }
        }

        if (!string.IsNullOrEmpty(agentCode))
        {
            DataTable agentDs = SqlHelper.Find(string.Format("select name from new_agent where code = '{0}'", agentCode)).Tables[0];

            if (agentDs.Rows.Count == 0)
            {
                // 如果填写的是新代理商编码
                string exAgentName = SqlHelper.Find(string.Format("select 代理商名称 from wf_form_{0} where id = {1}", formName, formId)).Tables[0].Rows[0][0].ToString();
                string temp_sql = String.Format("insert into new_agent (code, name) values ('{0}', '{1}')", agentCode, exAgentName);
                JObject jobject = JsonHelper.DeserializeJsonToObject<JObject>(SqlHelper.InsertAndGetLastId(temp_sql));
                id = int.Parse(jobject["Id"].ToString());
            }

            sql = string.Format("update wf_form_{0} set 代理商名称 = '{1}' where id = {2}", formName, id, formId);
            sqlList.Add(sql);
        }

        return SqlHelper.Exce(sqlList.ToArray());
    }

    private static JObject AddOutletBaseInfo(string docCode)
    {
        JObject r = new JObject();
        Dictionary<string, string> dict = new Dictionary<string, string>();
        string sql = string.Format("select * from wf_form_网点备案新增申请表 where DocCode = '{0}'", docCode);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
        {
            r.Add("ErrCode", "1");
            r.Add("ErrMsg", msg);
            return r;
        }

        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["代表"].ToString()))
        {
            dict.Add("代表", ds.Tables[0].Rows[0]["代表"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["主管"].ToString()))
        {
            dict.Add("主管", ds.Tables[0].Rows[0]["主管"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["区域经理"].ToString()))
        {
            dict.Add("区域经理", ds.Tables[0].Rows[0]["区域经理"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["销售负责人"].ToString()))
        {
            dict.Add("销售负责人", ds.Tables[0].Rows[0]["销售负责人"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["医院"].ToString()))
        {
            dict.Add("医院", ds.Tables[0].Rows[0]["医院"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["网点所在部门"].ToString()))
        {
            dict.Add("部门", ds.Tables[0].Rows[0]["网点所在部门"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["产品"].ToString()))
        {
            dict.Add("产品", ds.Tables[0].Rows[0]["产品"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["代理商名称"].ToString()))
        {
            dict.Add("代理商名称", ds.Tables[0].Rows[0]["代理商名称"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["配送点位"].ToString()))
        {
            dict.Add("配送点位", ds.Tables[0].Rows[0]["配送点位"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["医院供货价"].ToString()))
        {
            dict.Add("医院供货价", ds.Tables[0].Rows[0]["医院供货价"].ToString());
        }
        if (dict.ContainsKey("医院供货价") && dict.ContainsKey("配送点位"))
            dict["开票价"] = (float.Parse(dict["医院供货价"].ToString()) - float.Parse(dict["医院供货价"].ToString()) * (1-float.Parse(dict["配送点位"].ToString()))).ToString();

        if (dict.ContainsKey("开票价") && dict.ContainsKey("销售折让"))
            dict["考核价"] = (float.Parse(dict["开票价"].ToString()) - float.Parse(dict["销售折让"].ToString())).ToString();

        dict["CreateTime"] = DateTime.Now.ToString();
        dict["ModifyTime"] = DateTime.Now.ToString();

        sql = SqlHelper.GetInsertString(dict, "new_cost_sharing");
        SqlExceRes res = new SqlExceRes(SqlHelper.Exce(sql));
        if (res.Result == SqlExceRes.ResState.Success)
        {
            r.Add("ErrCode", "0");
            r.Add("ErrMsg", "网点信息新增成功！");
            return r;
        }
        else
        {
            r.Add("ErrCode", "1");
            r.Add("ErrMsg", res.ExceMsg);
            return r;
        }
    }

    private static JObject UpdateOutletBaseInfo(string docCode)
    {
        JObject r = new JObject();
        Dictionary<string, string> dict = new Dictionary<string, string>();
        string sql = string.Format("select * from wf_form_网点备案变更申请表 where DocCode = '{0}'", docCode);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
        {
            r.Add("ErrCode", "1");
            r.Add("ErrMsg", msg);
            return r;
        }

        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["代表"].ToString()) && ds.Tables[0].Rows[0]["代表"].ToString() != "无")
        {
            dict.Add("代表", ds.Tables[0].Rows[0]["代表"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["主管"].ToString()) && ds.Tables[0].Rows[0]["主管"].ToString() != "无")
        {
            dict.Add("主管", ds.Tables[0].Rows[0]["主管"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["区域经理"].ToString()) && ds.Tables[0].Rows[0]["区域经理"].ToString() != "无")
        {
            dict.Add("区域经理", ds.Tables[0].Rows[0]["区域经理"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["销售负责人"].ToString()) && ds.Tables[0].Rows[0]["销售负责人"].ToString() != "无")
        {
            dict.Add("销售负责人", ds.Tables[0].Rows[0]["销售负责人"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["医院"].ToString()) && ds.Tables[0].Rows[0]["医院"].ToString() != "无")
        {
            dict.Add("医院", ds.Tables[0].Rows[0]["医院"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["网点所在部门"].ToString()) && ds.Tables[0].Rows[0]["网点所在部门"].ToString() != "无")
        {
            dict.Add("部门", ds.Tables[0].Rows[0]["网点所在部门"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["产品"].ToString()) && ds.Tables[0].Rows[0]["产品"].ToString() != "无")
        {
            dict.Add("产品", ds.Tables[0].Rows[0]["产品"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["代理商名称"].ToString()) && ds.Tables[0].Rows[0]["代理商名称"].ToString() != "无")
        {
            dict.Add("代理商名称", ds.Tables[0].Rows[0]["代理商名称"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["配送点位"].ToString()) && ds.Tables[0].Rows[0]["配送点位"].ToString() != "无")
        {
            dict.Add("配送点位", ds.Tables[0].Rows[0]["配送点位"].ToString());
        }
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["医院供货价"].ToString()) && ds.Tables[0].Rows[0]["医院供货价"].ToString() != "无")
        {
            dict.Add("医院供货价", ds.Tables[0].Rows[0]["医院供货价"].ToString());
        }
        if (dict.ContainsKey("医院供货价") && dict.ContainsKey("配送点位"))
            dict["开票价"] = (float.Parse(dict["医院供货价"].ToString()) - float.Parse(dict["医院供货价"].ToString()) * (1 - float.Parse(dict["配送点位"].ToString()))).ToString();

        if (dict.ContainsKey("开票价") && dict.ContainsKey("销售折让"))
            dict["考核价"] = (float.Parse(dict["开票价"].ToString()) - float.Parse(dict["销售折让"].ToString())).ToString();

        dict["ModifyTime"] = DateTime.Now.ToString();

        Dictionary<string, string> dictPreChange = new Dictionary<string, string>();

        foreach (DataColumn c in ds.Tables[0].Columns)
        {
            if (c.ColumnName.Equals("变更前网点所在部门"))
                c.ColumnName = "变更前部门";

            if (c.ColumnName.Contains("变更前") && !string.IsNullOrEmpty(ds.Tables[0].Rows[0][c.ColumnName].ToString()) && ds.Tables[0].Rows[0][c.ColumnName].ToString() != "无")
                dictPreChange.Add(c.ColumnName, ds.Tables[0].Rows[0][c.ColumnName].ToString());
        }
        if (dictPreChange.Count > 0)
        {
            string condition = " where ";
            foreach (string key in dictPreChange.Keys)
            {
                condition += string.Format("{1}='{0}' and ", dictPreChange[key], key.Replace("变更前", ""));
            }
            condition = condition.Substring(0, condition.Length - 4);
            sql = string.Format("select Id from new_cost_sharing {0}", condition);
        }
        else
        {
            r.Add("ErrCode", "2");
            r.Add("ErrMsg", "未找到变更前网点信息！");
            return r;
        }

        msg = "";
        ds = SqlHelper.Find(sql, ref msg);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            r.Add("ErrCode", "3");
            r.Add("ErrMsg", "未找到变更前网点信息！\r\n" + msg);
            return r;
        }

        string outlet = "";
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            outlet += string.Format("{0},", row["Id"]);
        }
        outlet = outlet.Substring(0, outlet.Length - 1);

        sql = SqlHelper.GetUpdateString(dict, "new_cost_sharing"
            , string.Format(" where Id in ({0})", outlet));
        SqlExceRes res = new SqlExceRes(SqlHelper.Exce(sql));

        if (res.Result == SqlExceRes.ResState.Success)
        {
            r.Add("ErrCode", "0");
            r.Add("ErrMsg", "网点信息更新成功！");
            return r;
        }
        else
        {
            r.Add("ErrCode", "4");
            r.Add("ErrMsg", res.ExceMsg);
            return r;
        }
    }

    private static JObject UpdateOutletCostInfo(string docCode, string tableName)
    {
        JObject r = new JObject();
        Dictionary<string, string> dict = new Dictionary<string, string>();
        string sql = string.Format("select * from wf_form_{0} where DocCode = '{1}'", tableName, docCode);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
        {
            r.Add("ErrCode", "1");
            r.Add("ErrMsg", msg);
            return r;
        }

        foreach (DataColumn c in ds.Tables[0].Columns)
        {
            if (StringTools.HasChinese(c.ColumnName) && c.ColumnName != "产品" && c.ColumnName != "医院" && c.ColumnName != "代表"
                && c.ColumnName != "主管" && c.ColumnName != "区域经理" && c.ColumnName != "销售负责人" && c.ColumnName != "网点所在部门"
                && c.ColumnName != "代理商名称" && !string.IsNullOrEmpty(ds.Tables[0].Rows[0][c.ColumnName].ToString()))
            {
                dict.Add(c.ColumnName, ds.Tables[0].Rows[0][c.ColumnName].ToString());
            }
        }

        string searchCondition = "";
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["医院"].ToString()) && ds.Tables[0].Rows[0]["医院"].ToString() != "无")
            searchCondition += string.Format(" 医院='{0}' and", ds.Tables[0].Rows[0]["医院"].ToString());
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["产品"].ToString()) && ds.Tables[0].Rows[0]["产品"].ToString() != "无")
            searchCondition += string.Format(" 产品='{0}' and", ds.Tables[0].Rows[0]["产品"].ToString());
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["网点所在部门"].ToString()) && ds.Tables[0].Rows[0]["网点所在部门"].ToString() != "无")
            searchCondition += string.Format(" 部门='{0}' and", ds.Tables[0].Rows[0]["网点所在部门"].ToString());
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["代表"].ToString()) && ds.Tables[0].Rows[0]["代表"].ToString() != "无")
            searchCondition += string.Format(" 代表='{0}' and", ds.Tables[0].Rows[0]["代表"].ToString());
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["主管"].ToString()) && ds.Tables[0].Rows[0]["主管"].ToString() != "无")
            searchCondition += string.Format(" 主管='{0}' and", ds.Tables[0].Rows[0]["主管"].ToString());
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["区域经理"].ToString()) && ds.Tables[0].Rows[0]["区域经理"].ToString() != "无")
            searchCondition += string.Format(" 区域经理='{0}' and", ds.Tables[0].Rows[0]["区域经理"].ToString());
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["销售负责人"].ToString()) && ds.Tables[0].Rows[0]["销售负责人"].ToString() != "无")
            searchCondition += string.Format(" 销售负责人='{0}' and", ds.Tables[0].Rows[0]["销售负责人"].ToString());
        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["代理商名称"].ToString()) && ds.Tables[0].Rows[0]["代理商名称"].ToString() != "无")
            searchCondition += string.Format(" 代理商名称='{0}' and", ds.Tables[0].Rows[0]["代理商名称"].ToString());

        if (string.IsNullOrEmpty(searchCondition))
        {
            r.Add("ErrCode", "2");
            r.Add("ErrMsg", "未找到变更前网点信息！");
            return r;
        }

        sql = string.Format("select Id from new_cost_sharing where {0}", searchCondition.Substring(0, searchCondition.Length - 3));
        msg = "";
        ds = SqlHelper.Find(sql, ref msg);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            r.Add("ErrCode", "2");
            r.Add("ErrMsg", "未找到变更前网点信息！\r\n" + msg);
            return r;
        }

        string outlet = "";
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            outlet += string.Format("{0},", row["Id"]);
        }
        outlet = outlet.Substring(0, outlet.Length - 1);
        
        // 部门数据需要特殊处理
        dict["挑战指标数量1月"] = !dict.ContainsKey("进取指标数量1月") ? "0" : (float.Parse(dict["进取指标数量1月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量2月"] = !dict.ContainsKey("进取指标数量2月") ? "0" : (float.Parse(dict["进取指标数量2月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量3月"] = !dict.ContainsKey("进取指标数量3月") ? "0" : (float.Parse(dict["进取指标数量3月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量4月"] = !dict.ContainsKey("进取指标数量4月") ? "0" : (float.Parse(dict["进取指标数量4月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量5月"] = !dict.ContainsKey("进取指标数量5月") ? "0" : (float.Parse(dict["进取指标数量5月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量6月"] = !dict.ContainsKey("进取指标数量6月") ? "0" : (float.Parse(dict["进取指标数量6月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量7月"] = !dict.ContainsKey("进取指标数量7月") ? "0" : (float.Parse(dict["进取指标数量7月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量8月"] = !dict.ContainsKey("进取指标数量8月") ? "0" : (float.Parse(dict["进取指标数量8月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量9月"] = !dict.ContainsKey("进取指标数量9月") ? "0" : (float.Parse(dict["进取指标数量9月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量10月"] = !dict.ContainsKey("进取指标数量10月") ? "0" : (float.Parse(dict["进取指标数量10月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量11月"] = !dict.ContainsKey("进取指标数量11月") ? "0" : (float.Parse(dict["进取指标数量11月"].ToString()) * 1.4).ToString();
        dict["挑战指标数量12月"] = !dict.ContainsKey("进取指标数量12月") ? "0" : (float.Parse(dict["进取指标数量12月"].ToString()) * 1.4).ToString();

        if (dict.ContainsKey("医院供货价") && dict.ContainsKey("配送点位"))
            dict["开票价"] = (float.Parse(dict["医院供货价"].ToString()) - float.Parse(dict["医院供货价"].ToString()) * (1 - float.Parse(dict["配送点位"].ToString()))).ToString();

        if (dict.ContainsKey("开票价") && dict.ContainsKey("销售折让"))
            dict["考核价"] = (float.Parse(dict["开票价"].ToString()) - float.Parse(dict["销售折让"].ToString())).ToString();

        string cost = "0";

        if (dict.ContainsKey("产品"))
        {
            int productId = int.Parse(dict["产品"].ToString());
            cost = SqlHelper.Find("select cost from new_product where id = " + productId).Tables[0].Rows[0][0].ToString();
        }

        dict["采购成本"] = cost;

        if (dict.ContainsKey("考核价") && dict.ContainsKey("采购成本"))
        {
            dict["毛利"] = (float.Parse(dict["考核价"].ToString()) - float.Parse(dict["采购成本"].ToString())).ToString();
        }
        
        sql = SqlHelper.GetUpdateString(dict, "new_cost_sharing", string.Format(" where Id in ({0})", outlet));
        SqlExceRes res = new SqlExceRes(SqlHelper.Exce(sql));

        if (res.Result == SqlExceRes.ResState.Success)
        {
            r.Add("ErrCode", "0");
            r.Add("ErrMsg", "网点信息更新成功！");
            return r;
        }
        else
        {
            r.Add("ErrCode", "3");
            r.Add("ErrMsg", res.ExceMsg);
            return r;
        }
    }


    /// <summary>
    /// 撤回单据
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="docId">单据ID</param>
    /// <returns></returns>
    public static string BackDocument(string tableName, string docId, string userId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(docId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string sql = string.Format("update wf_form_{0} set Level=0,Status='草稿' where Id='{1}';", tableName, docId);
            sql += string.Format("insert into wf_record(TableName, DocId, UserId, ApprovalResult, " +
                "ApprovalTime) values ('{0}','{1}','{2}','撤回','{3}');", tableName, docId, userId, DateTime.Now.ToString());
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
        }
        return res.ToString();
    }

    public static string showFormFiled(string formId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(formId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }

        string sql = string.Format("select parameterData from wf_form_config where id = '{0}'", formId);

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

            DataTable table = ds.Tables[0];

            string parameterData = table.Rows[0][0].ToString();

            List<JObject> fieldList = JsonHelper.DeserializeJsonToList<JObject>(parameterData);

            JArray jArray = new JArray();

            foreach (JObject jObject in fieldList)
            {
                // 如果字段类型为 数字 单选框 多选框的时候 允许条件审批
                if ("number".Equals(jObject["TYP"].ToString()) || "radio".Equals(jObject["TYP"].ToString()) || "checkbox".Equals(jObject["TYP"].ToString()) || "name".Equals(jObject["TYP"].ToString()))
                {
                    JObject tempJObject = new JObject();

                    if ("name".Equals(jObject["TYP"].ToString()))
                    {
                        tempJObject.Add("json", jObject["RELA"]);
                    }
                    else if ("radio".Equals(jObject["TYP"].ToString()) || "checkbox".Equals(jObject["TYP"].ToString()))
                    {
                        tempJObject.Add("json", jObject["ITMS"]);
                    }

                    tempJObject.Add("name", jObject["LBL"]);
                    tempJObject.Add("type", jObject["TYP"]);

                    jArray.Add(tempJObject);
                }
            }

            res.Add("data", jArray.ToString());
        }

        return res.ToString();
    }



    /// <summary>
    /// 获取待我审批的单据
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="userId">审批人ID</param>
    /// <returns></returns>
    public static JArray GetToBeApprovedByMe(string userId, string tableName, ref JArray result)
    {
        string sql = string.Format("SELECT a.*,c.userName from wf_form_{0} a LEFT JOIN wf_approver b on a.Id=b.DocId left join users c on a.userId=c.userId WHERE b.TableName='{1}' " +
            "AND b.UserId='{2}' and a.Level=b.Level and b.Level>0;", tableName, tableName, userId);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            return result;
        }
        else
        {
            var _temp = JsonHelper.Dtb2JArray(ds.Tables[0]);

            foreach (var _temp_jobject in _temp)
            {
                _temp_jobject["tableName"] = tableName;
                result.Add(_temp_jobject);
            }
        }

        return result;
    }

    /// <summary>
    /// 获取我已审批的单据
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="userId">审批人ID</param>
    /// <returns></returns>
    public static JArray GetHasApprovedByMe(string userId, string tableName, ref JArray result)
    {
        string sql = string.Format("SELECT a.*,c.userName FROM `wf_form_{0}` a LEFT JOIN wf_record b ON a.Id=b.DocId left join users c on a.userId=c.userId WHERE" +
            " b.TableName='{1}' AND b.UserId='{2}' and a.userId!='{3}'", tableName, tableName, userId, userId);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);

        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            return result;
        }
        else
        {
            var _temp = JsonHelper.Dtb2JArray(ds.Tables[0]);

            foreach (var _temp_jobject in _temp)
            {
                _temp_jobject["tableName"] = tableName;
                result.Add(_temp_jobject);
            }
        }

        return result;
    }
    /// <summary>
    /// 获取我提交的单据
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="userId">提交人ID</param>
    /// <returns></returns>
    public static JArray GetSubmitedByMe(string userId, string tableName, ref JArray result)
    {
        string sql = string.Format("SELECT a.*,b.userName from wf_form_{0} a left join users b on a.userId=b.userId  where a.userId='{1}' and Status!='草稿' ", tableName, userId);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            return result;
        }
        else
        {
            var _temp = JsonHelper.Dtb2JArray(ds.Tables[0]);

            foreach (var _temp_jobject in _temp)
            {
                _temp_jobject["tableName"] = tableName;
                result.Add(_temp_jobject);
            }
        }

        return result;
    }

    /// <summary>
    /// 获取草稿
    /// </summary>
    /// <param name="userId">申请人ID</param>
    /// <param name="formName">表单名称</param>
    /// <returns></returns>
    public static JArray GetToBeSubmitedByMe(string userId, string tableName, ref JArray result)
    {
        string sql = string.Format("SELECT a.*,b.userName from wf_form_{0} a left join users b on a.userId=b.userId  where a.userId='{1}' and a.Status='草稿'", tableName, userId);
        var msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            return result;
        }
        else
        {
            var _temp = JsonHelper.Dtb2JArray(ds.Tables[0]);

            foreach (var _temp_jobject in _temp)
            {
                _temp_jobject["tableName"] = tableName;
                result.Add(_temp_jobject);
            }
        }

        return result;
    }

    /// <summary>
    /// 获取表单列表
    /// </summary>
    /// <param name="userId">用户userId</param>
    /// <param name="type">1-待我提交 2-待我审批 3-我已提交 4-我已审批 5-与我相关</param>
    /// <returns></returns>
    public static JArray getFormDataOverall(string userId, int type)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var findAllFormNameSql = string.Format("select formName from wf_form_config where valid = 0");
        string msg = "";
        var ds = SqlHelper.Find(findAllFormNameSql, ref msg);
        if (ds == null)
        {
            return null;
        }

        var result = new JArray();

        foreach (DataRow formNameRow in ds.Tables[0].Rows)
        {
            var tableName = formNameRow[0].ToString();

            if (type == 1)
            {
                // 待我提交列表
                result = GetToBeSubmitedByMe(userId, tableName, ref result);
            }
            else if (type == 2)
            {
                // 待我审批列表
                result = GetToBeApprovedByMe(userId, tableName, ref result);
            }
            else if (type == 3)
            {
                // 我已提交列表
                result = GetSubmitedByMe(userId, tableName, ref result);
            }
            else if (type == 4)
            {
                // 我已审批列表
                result = GetHasApprovedByMe(userId, tableName, ref result);
            }
            else
            {
                // 与我相关列表
                result = GetRelatedToMe(userId, tableName, ref result);
            }
        }

        return result;
    }

    /// <summary>
    /// 获取单据详情
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="docId">单据ID</param>
    /// <returns></returns>
    public static string GetDocumentDetail(string tableName, string docId, string userId, string type)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(docId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string table = "wf_form_" + tableName;
            string sql = string.Format("select * from wf_form_config where FormName = '{0}';", tableName);
            sql += string.Format("select a.*,b.userName from wf_record  a LEFT JOIN  users b on a.UserId=b.userId where TableName='{0}' and DocId='{1}' order by a.ApprovalTime;", tableName, docId);
            sql += string.Format("select a.*,b.userName from wf_approver  a LEFT JOIN  users b on a.UserId=b.userId  where TableName='{0}' and DocId='{1}' order by a.Level;", tableName, docId);
            sql += string.Format("SELECT userName from  users  WHERE userId in (select informer from wf_form_{0} where Id={1})", tableName, docId);
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
                sql = string.Format("SELECT a.*,c.userName  preparerName,d.name departmentName from `wf_form_{0}` a " +
                    " left join users c on a.userId=c.userId left join department d on a.departmentId=d.Id WHERE a.Id={1};", tableName, docId);

                foreach (JObject field in parameterData)
                {
                    if (field["RELA"] != null)
                    {
                        JObject RELA = JObject.Parse(field["RELA"].ToString());
                        JObject RELA1 = JObject.Parse(RELA["RELA1"].ToString());
                        if (RELA1["TABLENM"].ToString() == "部门表")
                        {
                            sql += string.Format("select a.*,department.name {0} from {1} a left join department on a.{2}=department.Id where a.id='{3}';",
                                field["LBL"].ToString() + "1", table, field["LBL"].ToString(), docId);
                        }
                        else if (RELA1["TABLENM"].ToString() == "用户表")
                        {
                            sql += string.Format("select a.*,users.userName {0} from {1} a left join users on a.{2}=users.userId where a.id='{3}'; ",
                                field["LBL"].ToString() + "1", table, field["LBL"].ToString(), docId);
                        }
                        else if (RELA1["TABLENM"].ToString() == "费用明细表")
                        {
                            sql += string.Format("select a.*,fee_detail_dict_copy.name {0} from {1} a left join fee_detail_dict_copy on a.{2}=fee_detail_dict_copy.Id where a.id='{3}'; ",
                                field["LBL"].ToString() + "1", table, field["LBL"].ToString(), docId);
                        }
                        else if (RELA1["TABLENM"].ToString() == "网点表")
                        {
                            sql += string.Format("select a.*,new_client.clientName {0} from {1} a left join new_client on a.{2}=new_client.Id where a.id='{3}' ;",
                                field["LBL"].ToString() + "1", table, field["LBL"].ToString(), docId);
                        }
                        else if (RELA1["TABLENM"].ToString() == "产品表")
                        {
                            sql += string.Format("select a.*,new_product.productName {0} from {1} a left join new_product on a.{2}=new_product.Id where a.id='{3}' ;",
                                field["LBL"].ToString() + "1", table, field["LBL"].ToString(), docId);
                        }
                        else if (RELA1["TABLENM"].ToString() == "项目表")
                        {
                            sql += string.Format("select a.*,yl_project.Code {0} from {1} a left join yl_project on a.{2}=yl_project.Id where a.id='{3}' ;",
                                field["LBL"].ToString() + "1", table, field["LBL"].ToString(), docId);
                        }
                        else if (RELA1["TABLENM"].ToString() == "代理商表")
                        {
                            sql += string.Format("select a.*,new_agent.name {0} from {1} a left join new_agent on a.{2}=new_agent.Id where a.id='{3}' ;",
                                field["LBL"].ToString() + "1", table, field["LBL"].ToString(), docId);
                        }
                    }
                }

                sql = sql.Substring(0, sql.Length - 1);

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
                    string approverUserId = "";
                    foreach (DataRow dw in ds.Tables[2].Rows)
                    {
                        if (dw["Level"].ToString() == set.Tables[0].Rows[0]["Level"].ToString() &&
                            dw["UserId"].ToString() == userId)

                        {
                            approverUserId = dw["UserId"].ToString();
                            break;
                        }
                    }

                    if (approverUserId == "" && type == "toBeApprovedByMe")
                    {
                        res.Add("ErrCode", 4);
                        res.Add("ErrMsg", "该单据已审批");
                    }
                    else
                    {

                        res.Add("ErrCode", 0);
                        res.Add("ErrMsg", "操作成功");
                        if (set.Tables.Count > 1)
                        {
                            for (int i = 1; i < set.Tables.Count; i++)
                            {
                                foreach (DataColumn dc in set.Tables[i].Columns)
                                {
                                    if (!set.Tables[0].Columns.Contains(dc.ColumnName))
                                    {
                                        set.Tables[0].Columns.Add(dc.ColumnName, Type.GetType("System.String"));
                                        set.Tables[0].Rows[0][dc.ColumnName] = set.Tables[i].Rows[0][dc.ColumnName];
                                    }
                                }
                            }
                        }

                        string informer = "";
                        if (ds.Tables[3].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[3].Rows)
                            {
                                informer += row["userName"].ToString() + ",";
                            }

                            informer = informer.Substring(0, informer.Length - 1);
                        }

                        set.Tables[0].Rows[0]["informer"] = informer;
                        res.Add("document", JsonHelper.DataTable2Json(set.Tables[0]));
                        res.Add("record", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[1]));
                        res.Add("approver", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[2]));
                        res.Add("form", JsonHelper.DataTable2Json(ds.Tables[0]));
                    }
                }
            }
        }
        return res.ToString();
    }


    /// <summary>
    /// 保存草稿
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="formData">要保存的数据</param>
    /// <param name="userId">申请人ID</param>
    /// <returns></returns>
    public static string SaveAsToBeSubmitedByMe(string tableName, string formData, string userId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(formData) || string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            JObject data = new JObject();
            data.Add("Level", "0");
            data.Add("SubmitId", userId);
            data.Add("Status", "草稿");
            data.Add("CreateTime", DateTime.Now.ToString());
            List<JObject> dataList = JsonHelper.DeserializeJsonToList<JObject>(formData);
            foreach (JObject DataJObject in dataList)
            {
                data.Add(DataJObject["name"].ToString(), DataJObject["value"].ToString());
            }

            SqlExceRes r = new SqlExceRes(SqlHelper.Exce(SqlHelper.GetInsertString(data, "wf_form_" + tableName)));
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
        }
        return res.ToString();
    }

    /// <summary>
    /// 获取抄送人
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="docId">单据ID</param>
    /// <returns></returns>
    public static string GetInformer(string tableName, string docId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(docId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string sql = string.Format("SELECT * from `wf_informer` WHERE TableName='{0}'and DocId={1}", tableName, docId);
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
                res.Add("ErrMsg", "未找到相关单据抄送人");
            }
            else
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
                res.Add("informer", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]));
            }
        }
        return res.ToString();
    }

    /// <summary>
    /// 获取与我相关的单据
    /// </summary>
    /// <param name="tableName">表单名称</param>
    /// <param name="userId">人员ID</param>
    /// <returns></returns>
    public static JArray GetRelatedToMe(string userId, string tableName, ref JArray result)
    {
        string sql = string.Format("SELECT * from `wf_form_{0}` a left join users b on a.userId=b.userId where Id in(SELECT DocId FROM wf_informer" +
            " WHERE UserId='{1}' and TableName='{2}')", tableName, userId, tableName);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
            return result;
        }
        else
        {
            var _temp = JsonHelper.Dtb2JArray(ds.Tables[0]);

            foreach (var _temp_jobject in _temp)
            {
                _temp_jobject["tableName"] = tableName;
                result.Add(_temp_jobject);
            }
        }
        return result;
    }

    public static string submitProcess(string defaultProcessJson, string conditionItem, string conditionJson, string formId, string processName)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(defaultProcessJson) || string.IsNullOrEmpty(conditionItem)
            || string.IsNullOrEmpty(conditionJson) || string.IsNullOrEmpty(formId) || string.IsNullOrEmpty(processName))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {

            string sql = string.Format("delete from wf_process where FormId='{0}'", formId);

            SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sql));
            if (r.ExceMsg.ToString().Contains("操作成功"))
            {
                JArray defaultProcessList = JsonHelper.DeserializeJsonToObject<JArray>(defaultProcessJson);
                List<string> conditionItemList = JsonHelper.DeserializeJsonToList<string>(conditionItem);
                JArray conditionJsonList = JsonHelper.DeserializeJsonToObject<JArray>(conditionJson);

                JArray realDefaultProcess = new JArray();

                string thisConditionitem = "";
                string thisConditionJson = "";
                string parentId = "-1";
                string beforeParentId = "-1";
                int itemNumber = 0;

                Boolean insertOK = true;
                for (int i = 0; i < conditionItemList.Count; i++)
                {
                    if ("".Equals(conditionItemList[i]))
                    {
                        realDefaultProcess.Add((JObject)defaultProcessList[i - itemNumber]);

                        if (i == conditionItemList.Count - 1)
                        {
                            sql = "";

                            // 通过formId和相应的条件查询 有无重复的
                            sql = string.Format("select ParentId from wf_process where FormId = '{0}' and ConditionItem = '{1}'", formId, thisConditionitem);
                            DataSet ds = SqlHelper.Find(sql);

                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                sql = string.Format("insert into wf_process (ParentId, FormId, DefaultProcessJson, DefaultInformerJson, ConditionItem, " +
                                "ConditionJson, ConditionName) values ('{4}', '{0}', '{1}', '', '{2}', '{3}', '')", formId, realDefaultProcess.ToString(), thisConditionitem, thisConditionJson, parentId);
                            }
                            else
                            {
                                beforeParentId = ds.Tables[0].Rows[0][0].ToString();
                                sql = string.Format("insert into wf_process (ParentId, FormId, DefaultProcessJson, DefaultInformerJson, ConditionItem, " +
                                "ConditionJson, ConditionName) values ('{4}', '{0}', '{1}', '', '{2}', '{3}', '')", formId, realDefaultProcess.ToString(), thisConditionitem, thisConditionJson, beforeParentId);
                            }

                            SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sql));

                            if (msg.Result != SqlExceRes.ResState.Success && insertOK)
                            {
                                insertOK = false;
                            }
                        }
                    }
                    else
                    {
                        itemNumber++;

                        sql = "";

                        // 通过formId和相应的条件查询 有无重复的
                        sql = string.Format("select ParentId from wf_process where FormId = '{0}' and ConditionItem = '{1}'", formId, thisConditionitem);
                        DataSet ds = SqlHelper.Find(sql);

                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            sql = string.Format("insert into wf_process (ParentId, FormId, DefaultProcessJson, DefaultInformerJson, ConditionItem, " +
                            "ConditionJson, ConditionName) values ('{4}', '{0}', '{1}', '', '{2}', '{3}', '')", formId, realDefaultProcess.ToString(), thisConditionitem, thisConditionJson, parentId);
                        }
                        else
                        {
                            beforeParentId = ds.Tables[0].Rows[0][0].ToString();
                            sql = string.Format("insert into wf_process (ParentId, FormId, DefaultProcessJson, DefaultInformerJson, ConditionItem, " +
                            "ConditionJson, ConditionName) values ('{4}', '{0}', '{1}', '', '{2}', '{3}', '')", formId, realDefaultProcess.ToString(), thisConditionitem, thisConditionJson, beforeParentId);
                        }

                        beforeParentId = parentId;

                        string lastIdstring = SqlHelper.InsertAndGetLastId(sql);

                        JObject lastIdJObject = JsonHelper.DeserializeJsonToObject<JObject>(lastIdstring);

                        parentId = lastIdJObject["Id"].ToString();

                        realDefaultProcess.Clear();
                        thisConditionitem = conditionItemList[i];
                        thisConditionJson = conditionJsonList[i].ToString();
                    }
                }

                if (insertOK)
                {
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");
                }
                else
                {
                    res.Add("ErrCode", 2);
                    res.Add("ErrMsg", "操作失败");
                }
            }
            else
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", "操作失败");
            }
        }
        return res.ToString(); ;
    }

    public static string find(string type, string value, string relatedType, string relatedValue)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(type))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少相关参数");
        }
        else
        {
            string sql = "";
            if ("findUsers".Equals(type) && (string.IsNullOrEmpty(relatedType) || string.IsNullOrEmpty(relatedValue)))
            {
                sql += string.Format("SELECT distinct userId value, userName target FROM `v_user_department_post` where  userName like '%{0}%'", value);
            }
            else if ("findDepartments".Equals(type) && (string.IsNullOrEmpty(relatedType) || string.IsNullOrEmpty(relatedValue)))
            {
                sql += string.Format("SELECT Id value, name target from department where name like '%{0}%'", value);
            }
            else if ("findBranches".Equals(type) && (string.IsNullOrEmpty(relatedType) || string.IsNullOrEmpty(relatedValue)))
            {
                sql += string.Format("SELECT fbd.Id value,fbd.`clientName` target from new_client fbd WHERE fbd.`clientName` like '%{0}%'", value);
            }
            else if ("findUsers".Equals(type) && "部门表".Equals(relatedType))
            {
                sql += string.Format("SELECT distinct userId value, userName target FROM `v_user_department_post`" +
                    " where (select name from department where id = {0}) like concat(department,'%') and userName like '%{1}%'", relatedValue, value);
            }
            else if ("findDepartments".Equals(type) && "用户表".Equals(relatedType))
            {
                sql += string.Format("SELECT distinct departmentId value, department target FROM `v_user_department_post` where userId='{0}' and department like '%{1}%'", relatedValue, value);
            }
            else if ("findProducts".Equals(type))
            {
                sql += string.Format("select  jp.Id value, CONCAT(jp.`productName`,'[',ifnull(jp.Specification, ''),'][',ifnull(jp.Unit, ''),']') target from  " +
                    " new_product jp where jp.`productName` LIKE '%{0}%' and jp.ProductType is not null", value);
            }
            else if ("findFeeDetails".Equals(type))
            {
                sql += string.Format("SELECT id value,name target from fee_detail_dict_copy where ParentName is null");
            }
            else if ("findAgent".Equals(type))
            {
                sql += string.Format("select id value, name target from new_agent where name like '%{0}%'", value);
            }
            else if ("findBranches".Equals(type) && "用户表".Equals(relatedType))
            {
                sql += string.Format("SELECT fbd.Id value,fbd.`Name` target   FROM `new_cost_sharing` ncs LEFT JOIN  fee_branch_dict fbd on ncs.HospitalId=fbd.Id " +
                    " where  ncs.SalesId='{0}' and fbd.name like '{1}' ", relatedValue, value);
            }
            else if ("findUsers".Equals(type) && "网点表".Equals(relatedType))
            {
                sql += string.Format("select u.userId value,u.userName target from new_cost_sharing ncs LEFT JOIN users u on ncs.SalesId = u.userId where" +
                    " ncs.HospitalId='{0}' AND u.userName LIKE '%{1}%'", relatedValue, value);
            }
            else if ("findBranches".Equals(type) && "部门表".Equals(relatedType))
            {
                sql += string.Format("SELECT DISTINCT ncs.HospitalId value,fbd.`Name` target from new_cost_sharing ncs LEFT JOIN fee_branch_dict fbd  ON ncs.DepartmentId=fbd.Id" +
                    " WHERE ncs.DepartmentId='{0}' AND fbd.`name` LIKE '%{1}%'", relatedValue, value);
            }
            else if ("findDepartments".Equals(type) && "网点表".Equals(relatedType))
            {
                sql += string.Format("SELECT DISTINCT ncs.DepartmentId value,d.`name` target from new_cost_sharing ncs LEFT JOIN department d  ON ncs.DepartmentId=d.Id " +
                    "WHERE ncs.HospitalId='{0}' AND d.`name` LIKE '%{1}%'", relatedValue, value);
            }
            else if ("findProject".Equals(type))
            {
                sql += string.Format("SELECT id value, code target from yl_project where code like '%{0}%'", value);
            }

            if (sql == "")
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", "关联表单出现错误");
            }
            else
            {
                string msg = "";
                DataSet ds = SqlHelper.Find(sql, ref msg);
                if (ds == null)
                {
                    res.Add("ErrCode", 3);
                    res.Add("ErrMsg", msg);
                }
                else if (ds.Tables[0].Rows.Count == 0)
                {
                    res.Add("ErrCode", 4);
                    res.Add("ErrMsg", "暂无数据");
                }
                else
                {

                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");
                    res.Add("data", JsonHelper.DataTable2Json(ds.Tables[0]));
                }
            }
        }
        return res.ToString();
    }
}

public class WorkFlowTreeHelper
{
    public List<WorkFlowTreeNode> tree { get; set; }
    public string ErrMsg { get; set; }
    public WorkFlowTreeHelper(DataTable dt)
    {
        InitTree(dt);
    }

    public List<WorkFlowTreeNode> GetTree()
    {
        return tree;
    }
    public string GetJson()
    {
        if (tree == null)
            return "F";
        else
            return JsonHelper.SerializeObject(tree);
    }

    private void InitTree(DataTable dt)
    {
        tree = new List<WorkFlowTreeNode>();

        DataRow rootRow = FindRootNode(dt);
        if (rootRow == null)
        {
            ErrMsg = "未找到默认流程信息";
            tree = null;
            return;
        }
        WorkFlowTreeNode rootNode = new WorkFlowTreeNode(rootRow);
        rootNode.state = "open";
        tree.Add(rootNode);
        foreach (DataRow row in dt.Rows)
        {
            if (row["Id"].ToString() == rootRow["Id"].ToString())//跳过根节点，避免重复添加
                continue;
            WorkFlowTreeNode node = new WorkFlowTreeNode(row);
            if (node.ParentId == -1)//根节点
            {
                tree.Add(node);
            }
            else
            {
                AddNode(node, dt);
            }
        }
    }

    private void AddNode(WorkFlowTreeNode node, DataTable dt)
    {
        WorkFlowTreeNode parent = FindNodeById(tree, node.ParentId);
        if (parent == null)
        {
            DataRow row = FindRowById(dt, node.ParentId);
            if (row == null)
                return;
            parent = new WorkFlowTreeNode(row);
            parent.children.Add(node);
            AddNode(parent, dt);
        }
        else
        {
            if (!ContainsNode(parent.children, node.id))
                parent.children.Add(node);
        }
    }

    private bool ContainsNode(List<WorkFlowTreeNode> list, int id)
    {
        bool res = false;
        foreach (WorkFlowTreeNode node in list)
        {
            if (node.id == id)
            {
                res = true;
                break;
            }
        }
        return res;
    }

    private WorkFlowTreeNode FindNodeById(List<WorkFlowTreeNode> listNodes, int id)
    {
        WorkFlowTreeNode res = null;
        foreach (WorkFlowTreeNode node in listNodes)
        {
            if (node.id == id)
            {
                return node;
            }
            else if (node.children.Count > 0)
            {
                WorkFlowTreeNode temp = FindNodeById(node.children, id);
                if (temp != null)
                {
                    res = temp;
                    break;
                }
            }
        }
        return res;
    }

    private DataRow FindRowById(DataTable dt, int id)
    {
        foreach (DataRow row in dt.Rows)
        {
            if (Convert.ToInt32(row["Id"]) == id)
            {
                return row;
            }
        }
        return null;
    }

    private DataRow FindRootNode(DataTable dt)
    {
        foreach (DataRow row in dt.Rows)
        {
            if (Convert.ToInt32(row["parentId"]) == -1)
                return row;
        }
        return null;
    }
}

public class WorkFlowTreeNode
{
    //前端tree属性
    public int id { get; set; }  //节点的id值  
    public string text { get; set; }  //节点显示的名称  
    public string state { get; set; }//节点的状态  
    //public bool Checked { get; set; }
    public List<WorkFlowTreeNode> children { get; set; }  //集合属性，可以保存子节点  
    public string iconCls { get; set; }

    //附加属性
    public string DefaultProcessJson { get; set; }
    public string DefaultInformerJson { get; set; }
    public string ConditionItem { get; set; }
    public string ConditionJson { get; set; }
    public int ParentId { get; set; }

    public WorkFlowTreeNode(DataRow row)
    {
        if (row == null)
            return;
        id = Convert.ToInt32(row["Id"]);
        DefaultProcessJson = row["DefaultProcessJson"].ToString();
        DefaultInformerJson = row["DefaultInformerJson"].ToString();
        ConditionItem = row["ConditionItem"].ToString();
        ConditionJson = row["ConditionJson"].ToString();
        ParentId = Convert.ToInt32(row["ParentId"]);
        if (Convert.ToInt32(row["parentId"]) == -1)
            text = "默认审批流程";
        else
        {
            text = row["ConditionName"].ToString();
        }
        state = "closed";
        //Checked = false;
        children = new List<WorkFlowTreeNode>();
        iconCls = "";
    }
}