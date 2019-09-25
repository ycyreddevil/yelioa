using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;

/// <summary>
/// NewBranchRegistrationManage 的摘要说明
/// </summary>
public class NewBranchRegistrationManage
{
    public NewBranchRegistrationManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 判断是新增网点或者审批单据或者审批记录
    /// </summary>
    /// <param name="docCode">单据号</param>
    ///  <param name="userId">人员ID</param>
    ///  <param name="IsRecord">是否只是审批记录</param>
    /// <returns>可见网点备案字段</returns>
    public static string IsAddOrApproveOrRecord(string docCode,string userId,string IsRecord)
    {
        JObject res = new JObject();
        if(string.IsNullOrEmpty(docCode)|| string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else if(docCode=="-1")
        {
            res = GetFieldsOfAddBranch(userId);
        }
        else 
        {
            if (IsRecord == "false")
            {
                res = GetFieldsOfApproveBranch(docCode);
            }
            else
            {
                res = GetFieldsOfRecordBranch(userId, docCode);
            }
        }
        return res.ToString();
    }
    /// <summary>
    /// 新增网点备案时获取字段
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <returns></returns>
    public static JObject GetFieldsOfAddBranch(string userId)
    {
        JObject res = new JObject();
        string ErrMsg = "";
        DataSet ds = NewBranchRegistrationSrv.IsRegionalManager(userId,ref ErrMsg);
        if(ds==null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", ErrMsg);
        }
        else if(ds.Tables[0].Rows.Count==0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "抱歉，您无权限增加网点");
        }
        else
        {
            DataTable empty = new DataTable();
            JArray fieldList = GetFieldList(ds.Tables[1], ds.Tables[2], ds.Tables[3], ds.Tables[4], ds.Tables[5],ds.Tables[6],empty);
            JArray approverList = new JArray();
            res.Add("fieldList", fieldList);
            res.Add("onlyRead", "false");
            res.Add("approverList", approverList);
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        return res;
    }
    /// <summary>
    /// 获取待审批的单据
    /// </summary>
    /// <param name="docCode">单据号</param>
    /// <returns></returns>
    public static JObject GetFieldsOfApproveBranch(string docCode)
    {
        JObject res = new JObject();
        string ErrMsg = "";
        DataSet ds = NewBranchRegistrationSrv.GetApprovalRecordAndApprovalFields(docCode, ref ErrMsg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", ErrMsg);
        }
        else if (ds.Tables[1].Rows.Count == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "抱歉，您无审批内容");
        }
        else
        {
            JArray fieldList = GetFieldList(ds.Tables[1], ds.Tables[2], ds.Tables[3], ds.Tables[4], ds.Tables[5], ds.Tables[6],ds.Tables[7]);           
            res.Add("fieldList", fieldList);
            //已审批人的记录
            JArray approverList = DataTableToJArray(ds.Tables[0]);
            res.Add("approverList", approverList);

            res.Add("onlyRead", "false");
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        return res;
    }
    /// <summary>
    /// 审批/提交记录获取
    /// </summary>
    /// <param name="userId">人员ID</param>
    /// <param name="docCode">单据号</param>
    /// <returns></returns>
    public static JObject GetFieldsOfRecordBranch(string userId,string docCode)
    {
        JObject res = new JObject();
        string ErrMsg = "";
        DataSet ds = NewBranchRegistrationSrv.GetApprovalRecord(userId,docCode, ref ErrMsg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", ErrMsg);
        }
        else if (ds.Tables[1].Rows.Count == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "抱歉，无已审批内容，如有疑问，请及时联系管理员");
        }
        else
        {
            JArray fieldList = GetRecordFieldList(ds.Tables[1], ds.Tables[2], ds.Tables[3], ds.Tables[4], ds.Tables[5]);
            res.Add("fieldList", fieldList);
            //已审批人的记录
            JArray approverList = DataTableToJArray(ds.Tables[0]);
            res.Add("approverList", approverList);

            res.Add("onlyRead", "true");
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        return res;
    }

    /// <summary>
    /// 新增（包含审批）网点备案时获取（转化）字段列表
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static JArray GetFieldList(DataTable fieldDT,DataTable userDT, DataTable productDT,DataTable hospitalDT,DataTable departmentDT,DataTable DPTdt,DataTable detailDT)
    {
        JArray fieldList = new JArray();

        JArray productList = DataTableToJArray(productDT);
        JArray hospitalList = DataTableToJArray(hospitalDT);
        JArray userList = DataTableToJArray(userDT);
        JArray departmentList = DepartmentDataTableToJArray(departmentDT,DPTdt);

        foreach (DataRow row in fieldDT.Rows)
        {
            JObject field = new JObject();
            field.Add("label", row["FieldName"].ToString());
            
            
            field.Add("type", row["FieldType"].ToString());
            
            if(row["FieldType"].ToString()=="select")
            {
                if (row["RelativeTable"].ToString()=="users")
                {
                    field.Add("optionList", userList);
                }
                else if(row["RelativeTable"].ToString() == "jb_product")
                {
                    field.Add("optionList", productList);
                }
                else if (row["RelativeTable"].ToString() == "fee_branch_dict")
                {
                    field.Add("optionList", hospitalList);
                }
                else if (row["RelativeTable"].ToString() == "department")
                {
                    field.Add("optionList", departmentList);
                }
                JObject valueObject = new JObject();
                if(detailDT!=null&&detailDT.Rows.Count>0&& detailDT.Select("FieldName='" + row["FieldName"].ToString() + "'").Length>0)
                {
                        valueObject = IDToName(JArray.Parse(field["optionList"].ToString()), detailDT.Select("FieldName='" + row["FieldName"].ToString() + "'")[0]["NewValue"].ToString());
                }
                else
                {
                    valueObject.Add("Id", "");
                    valueObject.Add("Name", "");
                }
                field.Add("relativeTable", row["RelativeTable"].ToString());
                field.Add("value", valueObject);
            }
            else
            {
                if (detailDT != null && detailDT.Rows.Count > 0 && detailDT.Select("FieldName='" + row["FieldName"].ToString() + "'").Length > 0)
                {
                    field.Add("value", detailDT.Select("FieldName='" + row["FieldName"].ToString() + "'")[0]["NewValue"].ToString());
                 
                }
                else
                {
                    field.Add("value", "");
                }
            }
            fieldList.Add(field);
        }
        return fieldList;
    }
    /// <summary>
    /// 将已审批单据的字段由datatable转换成Jarray
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static JArray GetRecordFieldList(DataTable dt,DataTable userDT,DataTable producrDT,DataTable hospitalDT,DataTable departmentDT)
    {
        JArray fieldList = new JArray();
        foreach (DataRow row in dt.Rows)
        {
            JObject field = new JObject();
            field.Add("label", row["FieldName"].ToString());                    
            field.Add("type", "text");
            
            if(row["FieldType"].ToString()=="select")
            {
                if (row["RelativeTable"].ToString() == "users")
                {
                    field.Add("value", IDToName(userDT, row["NewValue"].ToString()));
                }
                else if (row["RelativeTable"].ToString() == "jb_product")
                {
                    field.Add("value", IDToName(producrDT, row["NewValue"].ToString()));
                }
                else if (row["RelativeTable"].ToString() == "fee_branch_dict")
                {
                    field.Add("value", IDToName(hospitalDT, row["NewValue"].ToString()));
                }
                else if (row["RelativeTable"].ToString() == "department")
                {
                    string name = IDToName(departmentDT, row["NewValue"].ToString());
                    field.Add("value", name.Substring(name.LastIndexOf("/") + 1, name.Length - name.LastIndexOf("/") - 1));
                }
            }
            else
            {
                field.Add("value",  row["NewValue"].ToString());
            }
            fieldList.Add(field);
        }
        return fieldList;
    }
    /// <summary>
    /// datatable转jarray
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static JArray DataTableToJArray(DataTable dt)
    {
        JArray fieldList = new JArray();
        foreach (DataRow row in dt.Rows)
        {
            JObject field = new JObject();
         
            foreach(DataColumn column in dt.Columns)
            {
                field.Add(column.ColumnName.ToString(),row[column.ColumnName.ToString()].ToString());
            }
            fieldList.Add(field);
        }
        return fieldList;
    }
    /// <summary>
    /// 将datatable转为jarray，同时对部门表进行筛选，只保留新增发起人所在的销售线最底层部门
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    private static JArray DepartmentDataTableToJArray(DataTable dt,DataTable DT)
    {
        JArray fieldList = new JArray();

        List<TreeNode> root = new DepartmentTreeHelper(dt).GetTree();
        //获取销售部和直属战区下面的所有最底层的地区
        List<TreeNode> SalesDepartmentList = GetLeafNodes(SearchNodeList(root, "290"));
        SalesDepartmentList.AddRange(GetLeafNodes(SearchNodeList(root, "291")));


        foreach (TreeNode node in SalesDepartmentList)
        {
            if (DT.Select("departmentId=" + node.id.ToString()).Length > 0)//只保留新增发起人所在的销售线最底层部门
            {
                JObject field = new JObject();
                string name = node.text;
                field.Add("Id", node.id);
                field.Add("Name", name.Substring(name.LastIndexOf("/") + 1, name.Length - name.LastIndexOf("/") - 1));
                fieldList.Add(field);
            }
        }
        return fieldList;
    }
    /// <summary>
    /// 获取树里的叶子节点（最底层节点）
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    private static List<TreeNode> GetLeafNodes(TreeNode root)
    {
        List<TreeNode> list = new List<TreeNode>();
        if (root.children.Count == 0)
        {
            list.Add(root);
        }
        else
        {
            foreach (TreeNode node in root.children)
            {
                list.AddRange(GetLeafNodes(node));
            }
        }
        return list;
    }
    /// <summary>
    /// 通过节点ID搜索树里的节点
    /// </summary>
    /// <param name="listNodes"></param>
    /// <param name="id">节点ID</param>
    /// <returns></returns>
    private static TreeNode SearchNodeList(List<TreeNode> listNodes, string id)
    {
        TreeNode res = null;
        foreach (TreeNode node in listNodes)
        {
            if (node.id.ToString() == id)
            {
                return node;
            }
            else if (node.children.Count > 0)
            {
                TreeNode temp = SearchNodeList(node.children, id);
                if (temp != null)
                {
                    res = temp;
                    break;
                }
            }
        }
        return res;
    }


    /// <summary>
    /// 提交单据
    /// </summary>
    /// <param name="docCode">单据号</param>
    /// <param name="fieldList">单据所有字段相关数据</param>
    /// <param name="userId">提交人UserId</param>
    /// <returns></returns>
    public static string Submit(string docCode,JArray fieldList,string userId)
    {
        JObject res = new JObject();
        if(fieldList.Count==0||string.IsNullOrEmpty(docCode))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            List<string> sqlList = new List<string>();
            if (docCode=="-1")//提交单据
            {
                res = LeafDepartmentSubmit(fieldList, userId, 10);
            }
            else//审批单据
            {
                res = DepartmentApprove(fieldList, userId, docCode);
            }
        }
        return res.ToString();
    }

    /// <summary>
    /// 提交单据
    /// </summary>
    /// <param name="fieldList">单据所有字段相关数据</param>
    /// <param name="userId">提交人UserId</param>
    /// <param name="n">遇到错误时提交次数（两个或以上人员同时提交单据时会报错）</param>
    /// <returns></returns>
    private static JObject LeafDepartmentSubmit(JArray fieldList,string userId,int n)
    {
        JObject res = new JObject();
        if (n > 0)
        {
            
            string docCode = "0";
            string ErrMsg = "";
            DataSet ds = NewBranchRegistrationSrv.GetMaxDocCode(ref ErrMsg);
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
                DocJObject.Add("InsertOrUpdate", "0");//0表示新增
                DocJObject.Add("State", "审批中");
                DocJObject.Add("CreateTime", DateTime.Now);

                string departmentId = SearchDepartment(fieldList);

                JArray approverList = GetNextApprover(ds.Tables[1], 1, departmentId, 0);
                string approverUserId = JArrayToString(approverList, "userId");//审批人userId
                string approverWechatUserId = JArrayToString(approverList, "wechatUserId");//审批人wechatUserId

                DocJObject.Add("ApproverUserId", approverUserId);

                sqlList.Add(SqlHelper.GetInsertString(DocJObject, "cost_sharing_record"));
                sqlList.Add(GetDetailInsertSQL(fieldList, "0", userId, docCode));

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
                    res= LeafDepartmentSubmit(fieldList, userId, n - 1);
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
    /// <summary>
    /// 审批
    /// </summary>
    /// <param name="fieldList">被审批的字段</param>
    /// <param name="userId">审批人userId</param>
    /// <param name="docCode">单据号</param>
    /// <returns></returns>
    private static JObject DepartmentApprove(JArray fieldList, string userId,string docCode)
    {
        JObject res = new JObject();
        string ErrMsg = "";
        DataSet ds = NewBranchRegistrationSrv.GetDoc(docCode, ref ErrMsg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", ErrMsg);
        }
        else
        {
            List<string> sqlList = new List<string>();

            string departmentId = SearchDepartment(fieldList);//从前端传回的字段中里找‘部门’字段
            if(departmentId==null)//如果前端里没有‘部门’字段，从数据库中获取
            {
                departmentId = SearchDepartment(ds.Tables[0]); 
            }

            int level = Convert.ToInt16(ds.Tables[2].Rows[0]["Level"]);
            int n = 0;
            if(level==3)//如果level=3需要往上找2级，所以n=1,具体代码见GetNextApprover方法
            {
                n = 1;
            }
            JArray approverList = GetNextApprover(ds.Tables[1],level+1, departmentId, n);

            string state = "已审批";
            string approverUserId = "";//审批人userId
            string approverWechatUserId = "";//审批人wechatUserId

            if (approverList != null || approverList.Count > 0)//若审批未结束
            {
                approverUserId = JArrayToString(approverList, "userId");
                approverWechatUserId = JArrayToString(approverList, "wechatUserId");
                state = "审批中";
            }

            JObject DocJObject = new JObject();
            DocJObject.Add("ApproverUserId", approverUserId);
            DocJObject.Add("Level", level+1);
            DocJObject.Add("State", state);
            string condition = "where Code= '" + docCode + "'";

            sqlList.Add(SqlHelper.GetUpdateString(DocJObject, "cost_sharing_record",condition));
            sqlList.Add(GetDetailInsertSQL(fieldList, level.ToString(), userId, docCode));
            SqlHelper.Exce(sqlList.ToArray());

            if(approverWechatUserId=="")//审批结束
            {
                DataSet DocDS = NewBranchRegistrationSrv.GetDetail(docCode);
                SqlHelper.Exce(GetInsertSQL(DocDS.Tables[0]));//插入到new_cost_sharing表中
                //发送消息给单据提交人
            }
            else//审批进行中
            {
                //发送消息给单据提交人和下级审批人
            }
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");

        }
        return res;
    }

    private static string GetDetailInsertSQL(JArray fieldList,string level,string userId,string docCode)
    {

        List<JObject> sqlArray = new List<JObject>();
        foreach(JObject jobject in fieldList)
        {
            JObject sqlObject = new JObject();
            sqlObject.Add("FieldName", jobject["label"].ToString());
            
            sqlObject.Add("RegistrationCode", docCode);
            sqlObject.Add("ApproverUserId", userId);
            sqlObject.Add("Level", level);
            sqlObject.Add("CreateTime", DateTime.Now);

            if(jobject["type"].ToString()=="select")
            {
                JObject valueJobject = JObject.Parse(jobject["value"].ToString());
                sqlObject.Add("NewValue", valueJobject["Id"].ToString());
            }
            else
            {
                sqlObject.Add("NewValue", jobject["value"].ToString());
            }
            sqlArray.Add(sqlObject);
        }
        return SqlHelper.GetInsertString(sqlArray, "cost_sharing_detail");
    }
    
    /// <summary>
    /// 创建单据号
    /// </summary>
    /// <param name="docCode">数据库中保存的今天最大单据号，如果今天没有单据，则为0</param>
    /// <returns></returns>
    private static string CreateDocCode(string docCode)
    {
        if(docCode=="0")
        {
            docCode = DateTime.Now.ToString("yyyyMMdd") + "000000";
        }
        return (Convert.ToInt64(docCode)+1).ToString();
    }

    /// <summary>
    /// 在单据中找到部门项
    /// </summary>
    /// <param name="fieldList">单据的项列表</param>
    /// <returns></returns>
    private static string SearchDepartment(JArray fieldList)
    {
        foreach(JObject jobject in fieldList)
        {
            if(jobject["label"].ToString()=="部门")
            {
                JObject valueJobject = JObject.Parse(jobject["value"].ToString());
                return valueJobject["Id"].ToString();
            }
        }
        return null;
    }
    /// <summary>
    /// 在单据中找到部门项
    /// </summary>
    /// <param name="fieldList">单据的项列表</param>
    /// <returns></returns>
    private static string SearchDepartment(DataTable fieldList)
    {
        foreach (DataRow jobject in fieldList.Rows)
        {
            if (jobject["FieldName"].ToString() == "部门")
            {
                return jobject["NewValue"].ToString();
            }
        }
        return null;
    }
    /// <summary>
    /// 获取下一审批人
    /// </summary>
    /// <param name="dt">部门和负责人表</param>
    /// <param name="level">审批人级别</param>
    /// <param name="departmentId">部门ID</param>
    /// <param name="n">第n级审批人</param>
    /// <returns></returns>
    private static JArray GetNextApprover(DataTable dt, int level,string departmentId, int n)
    {
        JArray userIdList = new JArray();
        if (level < 4)
        {
            if(level==2)//第2级审批人--商务部负责人--程丹风
            {
                JObject user = new JObject();
                user.Add("wechatUserId", "D16030170");
                user.Add("userId", "100000225");
                userIdList.Add(user);
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {

                    if (row["departmentId"].ToString() == departmentId)
                    {
                        if (level > n )
                        {
                            return GetNextApprover(dt, level, row["departmentParentId"].ToString(), n + 1);
                        }
                       
                        else
                        {
                            if (level == 1 && departmentId != "303" && departmentId != "304" 
                                && departmentId != "305" && departmentId != "301" && departmentId != "302")//非直属战区,独立大区，跳过大区经理
                            {
                                return GetNextApprover(dt, level, row["departmentParentId"].ToString(), n);
                            }                          
                            else
                            {
                                JObject user = new JObject();
                                user.Add("wechatUserId", row["wechatUserId"].ToString());
                                user.Add("userId", row["userId"].ToString());
                                userIdList.Add(user);
                            }
                        }

                    }

                }
            }
            
        }
        return userIdList;
    }

    
    private static string GetInsertSQL(DataTable dt)
    {
        JObject sqlJobject = new JObject();
        JObject dataJson = new JObject();
        foreach(DataRow row in dt.Rows)
        {
            if(string.IsNullOrEmpty(row["RelativeFieldName"].ToString()))
            {
                dataJson.Add(row["FieldName"].ToString(), row["NewValue"].ToString());
            }
            else
            {
                sqlJobject.Add(row["RelativeFieldName"].ToString(), row["NewValue"].ToString());
            }
        }
        sqlJobject.Add("DataJson", dataJson);
        sqlJobject.Add("CreateTime", DateTime.Now);
        return SqlHelper.GetInsertString(sqlJobject, "new_cost_sharing");
    }
    /// <summary>
    /// 将jarray中所有的jobject的某一共同字段用字符串连接起来,中间用‘|’隔开
    /// </summary>
    /// <param name="jArray">JArray</param>
    /// <param name="field">共同的字段名</param>
    /// <returns></returns>
    private static string JArrayToString(JArray jArray,string field)
    {
        string res = "";
        foreach(JObject jobject in jArray)
        {
            foreach(var ss in jobject)
            {
                if(field==ss.Key.ToString())
                {
                    res += ss.Value.ToString() + "|";
                    break;
                }
            }
        }
        return res.Substring(0, res.Length - 1);
    }
    private static JObject IDToName(JArray jarray,string id)
    {
        foreach(JObject jobject in jarray)
        {
            if(jobject["Id"].ToString()==id)
            {
                return jobject;
            }
        }
        return null;
    }
    private static string IDToName(DataTable dt, string id)
    {
        foreach (DataRow row in dt.Rows)
        {
            if (row["Id"].ToString() == id)
            {
                return row["Name"].ToString();
            }
        }
        return null;
    }
}