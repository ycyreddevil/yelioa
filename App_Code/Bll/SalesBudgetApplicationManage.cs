using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

/// <summary>
/// SalesBudgetApplicationManage 的摘要说明
/// </summary>
public class SalesBudgetApplicationManage
{
    public SalesBudgetApplicationManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /*下面为分配预算的相关方法*/

    /// <summary>
    /// 获取成员第一次需要分配的单据
    /// </summary>
    /// <param name="userId"><成员ID/param>
    /// <returns></returns>
    public static string GetDoc(string userId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            string errMsg = "";
            DataSet ds = SalesBudgetApplicationSrv.GetDoc(userId, ref errMsg);
            if(ds==null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else 
            {
                for(int i= ds.Tables[0].Rows.Count-1;i>=0;i--)//排除地区（最底层部门）
                {
                    if(ds.Tables[1].Select("ParentId="+ ds.Tables[0].Rows[i]["DepartmentId"].ToString()).Length==0)
                    {
                        ds.Tables[0].Rows.RemoveAt(i);
                    }
                }
                if (ds.Tables[0].Rows.Count == 0)
                {
                    res.Add("ErrCode", 3);
                    res.Add("ErrMsg", "暂无数据");
                }
                else
                {
                    //DataTable dt = new DataTable();
                    //dt.Columns.Add("DepartmentId", Type.GetType("System.String"));
                    //dt.Columns.Add("DepartmentName", Type.GetType("System.String"));
                    //dt.Columns.Add("DocNo", Type.GetType("System.String"));
                    //IntegrationCostDetails(ds.Tables[0], ref dt);
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");
                    res.Add("Doc", JsonHelper.DataTable2Json(ds.Tables[0]));
                }
            }
            
        }
        return res.ToString();
    }
    /// <summary>
    /// 获取第一次需要分配的子部门
    /// </summary>
    /// <param name="departmentId">父部门ID</param>
    /// <returns></returns>
    public static string GetChildrenDepartment(string departmentId,string BudgetLimitType)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(departmentId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            Boolean flag = false;
            DataTable dt = new DataTable();
            string errMsg = "";
            DataSet ds = SalesBudgetApplicationSrv.GetChildrenDepartment(departmentId, ref errMsg);
            if (departmentId != "292" && departmentId != "293")
            {
                if (ds == null)
                {
                    res.Add("ErrCode", 2);
                    res.Add("ErrMsg", errMsg);
                }
                else if (ds.Tables[0].Rows.Count == 0)
                {
                    res.Add("ErrCode", 3);
                    res.Add("ErrMsg", "暂无数据");
                }
                else
                {
                    flag = true;
                    dt = ds.Tables[0].Copy();

                }
            }
            else//市场部和运营部分配市场学术费、商务费
            {
                flag = true;
                dt.Columns.Add("Id", Type.GetType("System.String"));
                dt.Columns.Add("name", Type.GetType("System.String"));
                dt.Rows.Add("290", "东森家园/集团营销中心/直属战区");
                dt.Rows.Add("291", "东森家园/集团营销中心/销售部");               
            }
            if(flag)
            {
                JArray Headers = new JArray();
                JArray empty = new JArray();
                JArray departmentJarray = new JArray();

                JObject headDepartment = GeneratingObject("部门名称", "subDepartmentName", empty);
                JObject headfeeDetail = GeneratingObject(BudgetLimitType, BudgetLimitType, empty);
                Headers.Add(headDepartment);
                Headers.Add(headfeeDetail);
                foreach (DataRow row in dt.Rows)
                {
                    JObject departmentJobject = new JObject();
                    departmentJobject.Add("subDepartmentId", row["Id"].ToString());
                    departmentJobject.Add("subDepartmentName", row["name"].ToString());

                    departmentJobject.Add(BudgetLimitType, "0");//该费用明细的分配值，由于未分配，所以为0
                    departmentJarray.Add(departmentJobject);
                }
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
                res.Add("Doc", departmentJarray);
                res.Add("Headers", Headers);
                res.Add("FirstOrSecondDistribute", "0");
            }
        }
        return res.ToString();
    }

    /// <summary>
    /// 第一次分配预算
    /// </summary>
    /// <param name="jarray">分配详情</param>
    /// <param name="BudgetLimitType">分配的类型（费用明细）</param>
    /// <param name="userId">分配人ID</param>
    /// <param name="docId">被分配的单据号</param>
    /// <returns></returns>
    public static string Distribute(JArray jarray,string BudgetLimitType,string userId,string docId)
    {
        JObject res = new JObject();
        if (jarray == null || jarray.Count == 0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "无数据传入");
        }
        else
        {           
            res = Distribute(jarray, BudgetLimitType, userId, docId, 10);//尝试进行十次插入数据库
        }
        return res.ToString();
    }

    /*下面为提交预算的相关方法*/

   /// <summary>
   /// 获取待提交的单据
   /// </summary>
   /// <param name="userId"></param>
   /// <returns></returns>
    public static string GetFormData(string userId)
    {
        DateTime now = DateTime.Now;
        JObject res = new JObject();

        if(Convert.ToInt32(now.Day)<1|| Convert.ToInt32(now.Day) > 31)
        {
            res.Add("ErrCode", 4);
            res.Add("ErrMsg", "暂未开放，开放时间：每月26，27号");
        }
        else if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            string errMsg = "";
            DataSet ds = SalesBudgetApplicationSrv.GetUserDepartment(userId,ref errMsg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "暂无数据");
            }
            else
            {
                DepartmentTreeHelper dth = new DepartmentTreeHelper(ds.Tables[1]);
                List<TreeNode> nodeList = dth.GetTree();

                DataTable doc = null;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string departmentIds = "";
                    TreeNode node;

                    if (userId == "100000142" && (row["departmentId"].ToString() == "292" || row["departmentId"].ToString() == "1"))
                    {
                        node = SearchNodeList(nodeList, "-1");

                    }
                    else if (row["departmentId"].ToString() == "292" || row["departmentId"].ToString() == "293")//市场部、运营部
                    {
                        node = SearchNodeList(nodeList, "284");
                    }
                    else
                    {
                        node = SearchNodeList(nodeList, row["departmentId"].ToString());
                    }

                    if (node != null)
                    {
                        List<TreeNode> list = GetLeafNodes(node);

                        foreach (TreeNode treenode in list)
                        {
                            departmentIds += treenode.id.ToString() + ",";
                        }


                        departmentIds = departmentIds.Substring(0, departmentIds.Length - 1);

                        DataSet temp = SalesBudgetApplicationSrv.GetLeafSubmitDoc(departmentIds);

                        if (temp == null)
                        {
                            res.Add("ErrCode", 3);
                            res.Add("ErrMsg", "暂无数据");

                            return res.ToString();
                        }
                        temp.Tables[0].Columns.Add("SelfDepartmentId", Type.GetType("System.String"));
                        foreach (DataRow dw in temp.Tables[0].Rows)
                        {
                            dw["SelfDepartmentId"] = row["departmentId"].ToString();
                        }


                        if (doc == null)
                        {
                            doc = temp.Tables[0];
                        }
                        else
                        {
                            doc = InsertRowsIntoTable(temp.Tables[0], doc);
                        }
                    }
                }
              
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
                res.Add("Doc", JsonHelper.DataTable2Json(doc));
            }
        }
        return res.ToString();
    }

    /// <summary>
    /// 获取地区下面所有网点的预算申请数据详情
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns></returns>
    public static string GetDetail(string departmentId, string selfDepartmentId, string userId, string docNo)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(departmentId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            // 判断是否重复提交
            Boolean readOnly = SalesBudgetApplicationSrv.isSubmitDuplicate(selfDepartmentId, userId);

            string errMsg = "";
            DataSet ds = SalesBudgetApplicationSrv.IsLeafDepartmentWhenSubmit(departmentId, ref errMsg);//仅作为判断是否是地区
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else if (ds.Tables[0].Rows.Count == 0)//地区经理提交
            {
                res = GetLeafSubmitDetail(selfDepartmentId, departmentId,"-1", userId, readOnly);
            }
            else//地区以上的部门
            {
                res = GetLeafSubmitDetail(selfDepartmentId,departmentId, ds.Tables[0].Rows[0][0].ToString(), userId, readOnly);
            }

            res.Add("readOnly", readOnly);

            // 获取审批记录
            if (docNo == "undefined" || docNo == "")
            {
                // 如果是地区经理的话 则docNo为null 需要重新找出docNo
                ds = SalesBudgetApplicationSrv.findDistrictDocNo(departmentId, userId, ref errMsg);
                if (ds == null)
                {
                    res.Remove("ErrCode");
                    res.Add("ErrCode", 2);
                    res.Remove("ErrMsg");
                    res.Add("ErrMsg", errMsg);
                    return res.ToString();
                }

                if (ds.Tables[0].Rows.Count == 0)
                    docNo = "undefined";
                else
                    docNo = ds.Tables[0].Rows[0][0].ToString();
            }

            ds = SalesBudgetApplicationSrv.getDocSubmitRecord(docNo);
            res.Add("recordData", JsonHelper.DataTable2Json(ds.Tables[0]));
        }
        return res.ToString();
        
    }

    /// <summary>
    /// 提交或审批预算申请
    /// </summary>
    /// <param name="jarray">某地区的所有网点预算详情</param>
    /// <param name="DocNo">单据号</param>
    /// <param name="departmentId">审批人或提交人部门ID</param>
    /// <param name="userId">审批人或提交人ID</param>
    /// <returns></returns>
    public static string Submit(JArray jarray, string DocNo,string departmentId,string userId, string userName)
    {
        JObject res = new JObject();
        if (jarray == null || jarray.Count == 0|| string.IsNullOrEmpty(departmentId) || string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            if(string.IsNullOrEmpty(DocNo))
            {
                res=LeafDepartmentSubmit(jarray, departmentId, userId, 10, userName);//地区提交
            }
            else
            {
                res = DepartmentSubmit(jarray,DocNo, departmentId, userId, userName);//审批
            }
        }
        return res.ToString();
    }


    /*下面为第二次分配预算的相关方法*/

    /// <summary>
    /// 获取成员第二次需要分配的单据
    /// </summary>
    /// <param name="userId"><成员ID/param>
    /// <returns></returns>
    public static string GetSecondDoc(string userId)
    {
        DateTime now = DateTime.Now;
       
        JObject res = new JObject();

        if ( Convert.ToInt32(now.Day) >31)
        {
            res.Add("ErrCode", 4);
            res.Add("ErrMsg", "暂未开放，开放时间：每月28号至每月最后一天");
        }
        else if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            string errMsg = "";
            DataSet ds = SalesBudgetApplicationSrv.GetSecondDoc(userId, ref errMsg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {


                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "暂无数据");
            }
            else
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
                res.Add("Doc", JsonHelper.DataTable2Json(ds.Tables[0]));
            }
        }
        return res.ToString();
    }
    public static string GetDistributeChildren(string userId, string BudgetLimitType,string departmentId,string isFinished)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId)|| string.IsNullOrEmpty(BudgetLimitType) || string.IsNullOrEmpty(departmentId) || string.IsNullOrEmpty(isFinished))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else if(isFinished=="0")
        {
            res = JObject.Parse(GetSecondChildrenDepartment(departmentId, BudgetLimitType));
        }
        else
        {
            res = JObject.Parse(GetChildrenDepartmentHistory(userId, BudgetLimitType));
        }
        return res.ToString();
    }
    public static string GetChildrenDepartmentHistory(string userId, string BudgetLimitType)
    {

        JObject res = new JObject();
        res.Add("ErrCode", 0);
        res.Add("ErrMsg", "操作成功");

        DataTable departmentDT = new DataTable();
        DataTable branchDT = new DataTable();
        string errMsg = "";
        DataSet ds = SalesBudgetApplicationSrv.GetSecondChildrenDepartment(userId, BudgetLimitType, ref errMsg);
        if (ds == null)
        {
            res["ErrCode"] = 2;
            res["ErrMsg"] = errMsg;
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            branchDT = SalesBudgetApplicationSrv.GetBranchHistory(userId,BudgetLimitType).Tables[0];
        }
        else
        {
            departmentDT = ds.Tables[0];
        }
        //拼接数据
        if (res["ErrCode"].ToString() == "0")
        {
            JArray jArray = new JArray();
            JArray Headers = new JArray();
            if (branchDT.Rows.Count == 0)
            {

                JArray empty = new JArray();
              
                JObject headDepartment = GeneratingObject("部门名称", "subDepartmentName", empty);

                Headers.Add(headDepartment);

                foreach (DataRow row in departmentDT.Rows)
                {
                    JObject departmentJobject = new JObject();
                    departmentJobject.Add("subDepartmentId", row["DepartmentId"].ToString());//部门Id
                    departmentJobject.Add("subDepartmentName", row["name"].ToString());//部门名称
                    departmentJobject.Add(BudgetLimitType, row["BudgetLimit"].ToString());//该费用明细的分配值

                    DataTable feeDetailDT = SalesBudgetApplicationSrv.GetFeeDetail(row["DepartmentId"].ToString(), BudgetLimitType).Tables[0];

                    departmentJobject.Add(BudgetLimitType + "SubmitAmount", feeDetailDT.Rows.Count > 0 ? feeDetailDT.Rows[0][0].ToString() : "0");//该部门上报的该费用明细预算，在数据中标签为 该费用明细名字+SubmitAmount

                   
                    jArray.Add(departmentJobject);
                }

                JObject headFeeDetail = GeneratingObject(BudgetLimitType, BudgetLimitType, empty);
                Headers.Add(headFeeDetail);
            }

            else//地区分配到网点
            {
                JArray empty = new JArray();
                JObject headProduct = GeneratingObject("产品名称", "ProductName", empty);
                JObject headHospital = GeneratingObject("医院名称", "HospitalName", empty);
                JObject headSales = GeneratingObject("销售员", "SalesName", empty);
                
                Headers.Add(headProduct);
                Headers.Add(headHospital);
                Headers.Add(headSales);

                foreach (DataRow row in branchDT.Rows)
                {
                    JObject branchJobject = new JObject();
                    branchJobject.Add("ProductId", row["ProductId"].ToString());
                    branchJobject.Add("ProductName", row["ProductName"].ToString());
                    branchJobject.Add("HospitalId", row["HospitalId"].ToString());
                    branchJobject.Add("HospitalName", row["HospitalName"].ToString());
                    branchJobject.Add("SupervisorId", row["SupervisorId"].ToString());
                    branchJobject.Add("SalesName", row["SalesName"].ToString());

                    DataSet feeDetailDS = SalesBudgetApplicationSrv.GetFeeDetail(row["ProductId"].ToString(), row["HospitalId"].ToString(), row["SupervisorId"].ToString(), BudgetLimitType);


                    branchJobject.Add(BudgetLimitType, row["Budget"].ToString());//该费用明细的分配值，由于未分配，所以为0
                    branchJobject.Add(BudgetLimitType + "Id", feeDetailDS.Tables[1].Rows[0][0].ToString());//该费用明细的Id,在数据中标签为 该费用明细名字+Id
                    branchJobject.Add(BudgetLimitType + "SubmitAmount", feeDetailDS.Tables[0].Rows.Count > 0 ? feeDetailDS.Tables[0].Rows[0][0].ToString() : "0");//该网点上报的该费用明细预算，在数据中标签为 该费用明细名字+SubmitAmount

                   
                    
                    jArray.Add(branchJobject);
                }
                JObject headFeeDetail = GeneratingObject(BudgetLimitType, BudgetLimitType, empty);
                Headers.Add(headFeeDetail);
            }
           
            res.Add("Doc", jArray);
            res.Add("Headers", Headers);
        }
        return res.ToString();
    }
    /// <summary>
    /// 获取第二次需要分配的子部门或网点
    /// </summary>
    /// <param name="departmentId">部门ID</param>
    /// <returns></returns>
    public static string GetSecondChildrenDepartment(string departmentId, string BudgetLimitType)
    {
        JObject res = new JObject();

        res.Add("ErrCode", 0);
        res.Add("ErrMsg", "操作成功");
        DataTable departmentDT = new DataTable();
        DataTable branchDT = new DataTable();
        departmentDT.Columns.Add("Id", Type.GetType("System.String"));
        departmentDT.Columns.Add("name", Type.GetType("System.String"));
        if (departmentId == "284" && BudgetLimitType == "市场学术费")//营销中心分配市场学术费
        {
            departmentDT.Rows.Add("292", "东森家园/集团营销中心/市场部");
        }
        else if (departmentId == "284" && BudgetLimitType == "商务费用金额")//营销中心分配商务费用金额
        {
            departmentDT.Rows.Add("293", "东森家园/集团营销中心/运营部");
        }
        else if (departmentId == "293" || departmentId == "292" || departmentId == "284")//营销中心分配其他费用、市场部分配市场学术费、运营部分配商务费用金额
        {
            departmentDT.Rows.Add("290", "东森家园/集团营销中心/直属战区");
            departmentDT.Rows.Add("291", "东森家园/集团营销中心/销售部");
        }
        else
        {
            string errMsg = "";
            DataSet ds = SalesBudgetApplicationSrv.GetSecondChildrenDepartment(departmentId, ref errMsg);
            if (ds == null)
            {
                res["ErrCode"] = 2;
                res["ErrMsg"] = errMsg;
            }
            else
            {

                departmentDT = ds.Tables[0];
            }
        }

        //拼接数据
        if (res["ErrCode"].ToString() == "0")
        {
            JArray jArray = new JArray();
            JArray Headers = new JArray();

            if (departmentDT.Rows.Count == 0)//地区分配到网点
            {
                branchDT = SalesBudgetApplicationSrv.GetBranch(departmentId).Tables[0];
            }
            if (branchDT.Rows.Count == 0)
            {

                JArray empty = new JArray();
                JArray headerArray = new JArray();
                JObject headDepartment = GeneratingObject("部门名称", "subDepartmentName", empty);

                Headers.Add(headDepartment);

                foreach (DataRow row in departmentDT.Rows)
                {

                    JObject departmentJobject = new JObject();
                    departmentJobject.Add("subDepartmentId", row["Id"].ToString());//部门Id
                    departmentJobject.Add("subDepartmentName", row["name"].ToString());//部门名称

                    DataTable feeDetailDT = SalesBudgetApplicationSrv.GetFeeDetail(row["Id"].ToString(), BudgetLimitType).Tables[0];


                    departmentJobject.Add(BudgetLimitType, "0");//该费用明细的分配值，由于未分配，所以为0
                    //departmentJobject.Add(BudgetLimitType + "Id", dw["FeeDetailId"].ToString());//该费用明细的Id,在数据中标签为 该费用明细名字+Id
                    departmentJobject.Add(BudgetLimitType + "SubmitAmount", feeDetailDT.Rows.Count>0?feeDetailDT.Rows[0][0].ToString():"0");//该部门上报的该费用明细预算，在数据中标签为 该费用明细名字+SubmitAmount

                    jArray.Add(departmentJobject);
                }

                JObject headFeeDetail = GeneratingObject(BudgetLimitType, BudgetLimitType, empty);
                Headers.Add(headFeeDetail);
            }

            else//地区分配到网点
            {
                JArray empty = new JArray();
                JObject headProduct = GeneratingObject("产品名称", "ProductName", empty);
                JObject headHospital = GeneratingObject("医院名称", "HospitalName", empty);
                JObject headSales = GeneratingObject("销售员", "SalesName", empty);
               
                Headers.Add(headProduct);
                Headers.Add(headHospital);
                Headers.Add(headSales);

                foreach (DataRow row in branchDT.Rows)
                {
                    JObject branchJobject = new JObject();
                    branchJobject.Add("ProductId", row["ProductId"].ToString());
                    branchJobject.Add("ProductName", row["ProductName"].ToString());
                    branchJobject.Add("HospitalId", row["HospitalId"].ToString());
                    branchJobject.Add("HospitalName", row["HospitalName"].ToString());
                    branchJobject.Add("SupervisorId", row["SupervisorId"].ToString());
                    branchJobject.Add("SalesName", row["userName"].ToString());

                    DataSet feeDetailDS = SalesBudgetApplicationSrv.GetFeeDetail(row["ProductId"].ToString(), row["HospitalId"].ToString(), row["SupervisorId"].ToString(), BudgetLimitType);
                    

                    branchJobject.Add(BudgetLimitType, "0");//该费用明细的分配值，由于未分配，所以为0
                    branchJobject.Add(BudgetLimitType + "Id", feeDetailDS.Tables[1].Rows[0][0].ToString());//该费用明细的Id,在数据中标签为 该费用明细名字+Id
                    branchJobject.Add(BudgetLimitType + "SubmitAmount", feeDetailDS.Tables[0].Rows.Count > 0 ? feeDetailDS.Tables[0].Rows[0][0].ToString() : "0");//该网点上报的该费用明细预算，在数据中标签为 该费用明细名字+SubmitAmount

                  

                    jArray.Add(branchJobject);
                }
                JObject headFeeDetail = GeneratingObject(BudgetLimitType, BudgetLimitType, empty);
                Headers.Add(headFeeDetail);
            }
            res.Add("Doc", jArray);
            res.Add("Headers", Headers);
        }
        return res.ToString();
    }

    /// <summary>
    /// 第二次分配预算
    /// </summary>
    /// <param name="jarray">分配详情</param>
    /// <param name="BudgetLimitType">分配的类型（费用明细）</param>
    /// <param name="userId">分配人ID</param>
    /// <param name="docId">被分配的单据号</param>
    /// <returns></returns>
    public static string SecondDistrbute(JArray jarray, string BudgetLimitType, UserInfo userInfo, string docId)
    {
        JObject res = new JObject();
        if (jarray == null || jarray.Count == 0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "无数据传入");
        }
        else
        {
            JObject temp = JObject.Parse(jarray[0].ToString());
            if (temp.Property("subDepartmentId") != null)
            {
                res = SecondDistributeToDepartment(jarray, BudgetLimitType, userInfo, docId, 10);//尝试进行十次插入数据库
            }
           else
            {
                res=SecondDistributeToBranch(jarray, BudgetLimitType, userInfo, docId);
            }
        }
        return res.ToString();
    }

    /*本类相关私用方法*/

    ///// <summary>
    ///// 将某个部门需要进行第一次分配的费用明细合并成一行
    ///// </summary>
    ///// <param name="Original">原datatable</param>
    ///// <param name="Generated">合并生成的datatable</param>
    //private static void IntegrationCostDetails(DataTable Original,ref DataTable Generated)
    //{
    //    foreach(DataRow row in Original.Rows)
    //    {
    //        if (Generated.Select("DepartmentId=" + row["DepartmentId"].ToString()).Length==0)
    //        {
    //            DataRow dw = Generated.NewRow();

    //            dw["DepartmentId"] = row["DepartmentId"].ToString();
    //            dw["DepartmentName"] = row["department"].ToString();
    //            dw["DocNo"] = row["DocNo"].ToString();
    //            Generated.Rows.Add(dw);
    //        }
    //        if (!Generated.Columns.Contains(row["BudgetLimitType"].ToString()))
    //        {
    //            Generated.Columns.Add(row["BudgetLimitType"].ToString(), Type.GetType("System.String"));
    //        }
    //        foreach (DataRow dw in Generated.Rows)
    //        {
    //            if (dw["DepartmentId"].ToString() == row["DepartmentId"].ToString())
    //            {
    //                dw["DocNo"] +=","+ row["DocNo"].ToString();
    //                dw[row["BudgetLimitType"].ToString()] = row["BudgetLimit"].ToString();
    //            }
    //        }
    //    }
    //}

    private static JObject GeneratingObject(string label,string prop,JArray children)
    {
        JObject jObject = new JObject();
        jObject.Add("label", label);
        jObject.Add("prop", prop);
        jObject.Add("children", children);
        return jObject;
    }

    /// <summary>
    /// 第一次分配的具体方法
    /// </summary>
    /// <param name="jarray">分配方案详情</param>
    /// <param name="BudgetLimitType">父费用明细</param>
    /// <param name="userId">操作人员userId</param>
    /// <param name="docId">被分配的单据ID</param>
    /// <param name="n">插入次数</param>
    /// <returns></returns>
    private static JObject Distribute(JArray jarray, string BudgetLimitType, string userId, string docId, int n)
    {
        JObject res = new JObject();
        if (n == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "当前用户过多，链接超时，请重新操作");
        }
        else
        {
            string errMsg = "";
            DataSet ds = SalesBudgetApplicationSrv.GetMaxDocNo( ref errMsg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else
            {
                DateTime today = DateTime.Now;
                long Max = Convert.ToInt64(ds.Tables[0].Rows[0][0]);
                JArray newJarray = new JArray();
                if (Max == 0)
                {
                    Max = Convert.ToInt64(today.ToString("yyyyMMdd") + "000000");
                }
                foreach (JObject jobject in jarray)
                {
                    JObject newjobject = new JObject();
                    newjobject.Add("DepartmentId", jobject["subDepartmentId"].ToString());
                    newjobject.Add("BudgetLimit", jobject[BudgetLimitType].ToString());
                    newjobject.Add("BudgetLimitType", BudgetLimitType);
                    newjobject.Add("UserId", userId);
                    newjobject.Add("IsFinished", "0");
                    newjobject.Add("FirstOrSecondDistribute", "0");
                    newjobject.Add("CreateTime", today);
                    newjobject.Add("DocNo", ++Max);

                    newJarray.Add(newjobject);
                }

                string sql = SqlHelper.GetInsertString(JsonHelper.DeserializeJsonToList<JObject>(newJarray.ToString()), "yl_budget_distribute");
                SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sql));
                if (msg.Result == SqlExceRes.ResState.Success)
                {
                    sql = string.Format("update yl_budget_distribute set IsFinished=1 where DocNo in ({0});", docId);
                    string temp=SqlHelper.Exce(sql);
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");
                }
                else if (msg.Result == SqlExceRes.ResState.Error)
                {
                    Thread.Sleep(100);//等待0.1s后再进行插入数据库
                    return Distribute(jarray, BudgetLimitType, userId, docId, n - 1);
                }
            }
        }
        return res;
    }

    /// <summary>
    /// 第二次分配到部门的具体方法
    /// </summary>
    /// <param name="jarray">分配方案详情</param>
    /// <param name="BudgetLimitType">父费用明细</param>
    /// <param name="userId">操作人员userId</param>
    /// <param name="docId">被分配的单据ID</param>
    /// <param name="n">插入次数</param>
    /// <returns></returns>
    private static JObject SecondDistributeToDepartment(JArray jarray, string BudgetLimitType, UserInfo userInfo, string docId, int n)
    {
        JObject res = new JObject();
        if (n == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "当前用户过多，链接超时，请重新操作");
        }
        else
        {
            string errMsg = "";
            DataSet ds = SalesBudgetApplicationSrv.GetMaxDocNo( ref errMsg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else
            {
                DateTime today = DateTime.Now;
                long Max = Convert.ToInt64(ds.Tables[0].Rows[0][0]);
                JArray newJarray = new JArray();                
                if (Max == 0)
                {
                    Max = Convert.ToInt64(today.ToString("yyyyMMdd") + "000000");
                }
                foreach (JObject jobject in jarray)
                {
                    Max++;
                    JObject newjobject = new JObject();
             
                    newjobject.Add("BudgetLimit", jobject[BudgetLimitType].ToString());

                    newjobject.Add("DepartmentId", jobject["subDepartmentId"].ToString());
                    
                    newjobject.Add("BudgetLimitType", BudgetLimitType);
                    newjobject.Add("UserId", userInfo.userId.ToString());
                    newjobject.Add("IsFinished", "0");
                    
                    newjobject.Add("CreateTime", today);
                    newjobject.Add("DocNo", Max);

                    newJarray.Add(newjobject);
                }

                string sql = SqlHelper.GetInsertString(JsonHelper.DeserializeJsonToList<JObject>(newJarray.ToString()), "yl_budget_distribute");               
                SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sql));
                if (msg.Result == SqlExceRes.ResState.Success)
                {
                    sql = string.Format("update yl_budget_distribute set IsFinished=1 where DocNo in ({0});", docId);
                    string temp = SqlHelper.Exce(sql);

                    // 发消息给分配人
                    foreach (JObject jObject in jarray)
                    {
                        string departmentId = jObject["subDepartmentId"].ToString();
                        ds = SqlHelper.Find(String.Format(
                            "select wechatUserId from user_department_post where departmentId = '{0}'", departmentId));
                        string supervisorWechatUserId = "";
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            supervisorWechatUserId += dr["wechatUserId"] + "|";
                        }

                        WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper("I-0W2FLXN_BKL-mORN8hxwzDzYCOUzOIvi_JUrtGCTU", "业力费用管控", "1000016");

                        // 发消息给下级审批人
                        wxNetSalesHelper.GetJsonAndSendWxMsg(supervisorWechatUserId, "请及时审批 提交人为:" + userInfo.userName + "的单据,谢谢!", "http://yelioa.top/web/budget_distribute/BudgetDistributeList.aspx", "1000016");
                    }

                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");
                }
                else if (msg.Result == SqlExceRes.ResState.Error)
                {
                    Thread.Sleep(100);//等待0.1s后再进行插入数据库
                    return SecondDistributeToDepartment(jarray, BudgetLimitType, userInfo, docId, n - 1);
                }
            }
        }
        return res;
    }

    /// <summary>
    /// 第二次分配到网点的具体方法
    /// </summary>
    /// <param name="jarray">分配方案详情</param>
    /// <param name="BudgetLimitType">父费用明细</param>
    /// <param name="userId">操作人员userId</param>
    /// <param name="docId">被分配的单据ID</param>
    /// <returns></returns>
    private static JObject SecondDistributeToBranch(JArray jarray, string BudgetLimitType, UserInfo userInfo, string docId)
    {
        JObject res = new JObject();

        DateTime today = DateTime.Now;
        JArray feeDetailJarray = new JArray();
        JArray branchJarray = new JArray();
        string errMsg = "";

        DataSet ds = SalesBudgetApplicationSrv.GetFeeDetail("-1",ref errMsg);

        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", errMsg);
        }
        else
        {
            foreach (JObject jobject in jarray)
            {
                JObject branchJobject = new JObject();
                branchJobject.Add("ProductId", jobject["ProductId"].ToString());
                branchJobject.Add("HospitalId", jobject["HospitalId"].ToString());
                branchJobject.Add("SupervisorId", jobject["SupervisorId"].ToString());
                branchJobject.Add("CreateTime", today);

                branchJarray.Add(branchJobject);

                string HospitalRecordId = string.Format("(SELECT Id FROM yl_sales_hospital_budget_record where HospitalId='{0}' and ProductId='{1}'" +
                    "  and  SupervisorId='{2}' and  DepartmentRecordDoc is NULL AND CreateTime='{3}')",
                    jobject["HospitalId"].ToString(), jobject["ProductId"].ToString(), jobject["SupervisorId"].ToString(), today);


                JObject feeDetailJobject = new JObject();

                feeDetailJobject.Add("FeeDetailId", jobject[BudgetLimitType + "Id"].ToString());
                feeDetailJobject.Add("Budget", jobject[BudgetLimitType].ToString());
                feeDetailJobject.Add("HospitalRecordId", HospitalRecordId);
                feeDetailJobject.Add("supervisorId", userInfo.userId);
                feeDetailJarray.Add(feeDetailJobject);

            }

            string sql = SqlHelper.GetInsertString(JsonHelper.DeserializeJsonToList<JObject>(branchJarray.ToString()), "yl_sales_hospital_budget_record");
            sql += SqlHelper.GetInsertStringForDistribute(JsonHelper.DeserializeJsonToList<JObject>(feeDetailJarray.ToString()), "yl_budget_detail",1000);
            SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sql));
            if (msg.Result == SqlExceRes.ResState.Success)
            {
                sql = string.Format("update yl_budget_distribute set IsFinished=1 where DocNo in ({0});", docId);
                string temp = SqlHelper.Exce(sql);
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");
            }
            else 
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "提交失败，请重新操作");
            }
        }
        return res;
    }


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

    private static DataTable InsertRowsIntoTable(DataTable source,DataTable destination)
    {
        foreach(DataRow row in source.Rows)
        {
            destination.ImportRow(row);
        }
        return destination;
    }

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
    /// 提交预算时所需要的地区所有网点详细预算数据
    /// </summary>
    /// <param name="departmentId">地区部门ID</param>
    /// <param name="docCode">地区部门提交的单据DocCode</param>
    /// <returns></returns>
    private static JObject GetLeafSubmitDetail(string selfDepartmentId, string departmentId,string docCode,string userId, Boolean readOnly)
    {
        JObject res = new JObject();
        DataSet ds = SalesBudgetApplicationSrv.GetDistributeDoc(selfDepartmentId,departmentId, docCode, userId, readOnly);

        JArray headers = new JArray();
        JArray branchArray = new JArray();
        JArray empty = new JArray();
        if (docCode != "-1")
        {
            for (int i = ds.Tables[1].Rows.Count - 1; i >= 0; i--)
            {
                if (ds.Tables[2].Select("FeeDetailId=" + ds.Tables[1].Rows[i]["Id"].ToString()).Length == 0)
                {
                    ds.Tables[1].Rows.RemoveAt(i);
                }
            }
            for (int i = ds.Tables[0].Rows.Count - 1; i >= 0; i--)
            {
                if (ds.Tables[1].Select("ParentName='" + ds.Tables[0].Rows[i]["FeeDetailName"].ToString() + "'").Length == 0
                    && ds.Tables[1].Select("Name='" + ds.Tables[0].Rows[i]["FeeDetailName"].ToString() + "'").Length == 0)
                {
                    ds.Tables[0].Rows.RemoveAt(i);
                }
            }
        }

        headers.Add(GeneratingObject( "医院", "HospitalName", empty));
        headers.Add(GeneratingObject("产品", "ProductName", empty));
        headers.Add(GeneratingObject("负责人", "SalesName", empty));
        
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            JObject branchObject = new JObject();

            branchObject.Add("ProductId", row["ProductId"].ToString());
            branchObject.Add("HospitalId", row["HospitalId"].ToString());
            branchObject.Add("SupervisorId", row["SupervisorId"].ToString());

            int index = JarrayContainJobject(branchArray, branchObject);
            if (index>-1)//网点存在
            {
                JObject temp = (JObject)branchArray[index];
                AddFeeDetailToBranch(ds.Tables[1],ds.Tables[2],  row["FeeDetailName"].ToString(), row["LimitNumber"].ToString(), ref temp);
                branchArray[index] = temp;
                if(index==0)//添加费用明细表头
                {
                    headers.Add(GeneratingObject(row["FeeDetailName"].ToString(), "", AddHeader(ds.Tables[1], row["FeeDetailName"].ToString())));
                }
            }
            else//新建网点
            {
                branchObject.Add("ProductName", row["ProductName"].ToString());
                branchObject.Add("HospitalName", row["HospitalName"].ToString());
                branchObject.Add("SalesName", row["SalesName"].ToString());

                JObject temp = GeneratingObject(row["FeeDetailName"].ToString(), "", AddHeader(ds.Tables[1], row["FeeDetailName"].ToString()));
                if (headers.Children().Count()==3)
                {
                    headers.Add(temp);
                }

                AddFeeDetailToBranch(ds.Tables[1], ds.Tables[2], row["FeeDetailName"].ToString(), row["LimitNumber"].ToString(), ref branchObject);

                branchArray.Add(branchObject);
            }
        }
        res.Add("ErrCode", 0);
        res.Add("ErrMsg", "操作成功");
        res.Add("Headers", headers);
        res.Add("Doc", branchArray);
        return res;
    }


    /// <summary>
    /// 添加某父费用明细的子费用明细表头
    /// </summary>
    /// <param name="dt">费用明细表</param>
    /// <param name="feeDetail">父费用明细名称</param>
    /// <returns></returns>
    private static JArray AddHeader(DataTable dt,string feeDetail)
    {
        
        JArray jarray = new JArray();
        foreach (DataRow row in dt.Rows)
        {
            if (row["ParentName"].ToString() == feeDetail || (row["ParentName"].ToString() == ""&&row["Name"].ToString() == feeDetail&& dt.Select("ParentName='" + feeDetail + "'").Length == 0))
            {
                JObject jObject = new JObject();
                jObject.Add("label", row["Name"].ToString());
                jObject.Add("prop", feeDetail+row["Name"].ToString());
                jarray.Add(jObject);
            }
        }
        JObject temp = new JObject();
        temp.Add("label", "可用额度");
        temp.Add("prop", feeDetail + "可用额度");
        jarray.Add(temp);
        return jarray;
    }

    /// <summary>
    /// 判断某个JObject是否存在于JArray中，只要求相似，即JObject中的所有数据在JArray的某一个child中都存在，但child中的数据JObject不一定存在
    /// </summary>
    /// <param name="jarray"></param>
    /// <param name="jobject"></param>
    /// <returns>JObject在JArray相似的child的index</returns>
    private static int JarrayContainJobject(JArray jarray,JObject jobject)
    {
        for(int i=0;i<jarray.Count; i++)
        {
            int n = 1;
            JObject temp = (JObject)jarray[i];

            foreach (var ss in jobject)
            {
                if (temp[ss.Key].ToString() != ss.Value.ToString())
                {
                    break;
                }
                else if(jobject.Count==n)
                {
                    return i;
                }
                else
                {
                    n++;
                }
            }
        }
        return -1;
    }

    /// <summary>
    /// 将某父费用明细的子费用明细信息加入至网点中
    /// </summary>
    /// <param name="feeDetailDT">费用明细表</param>
    /// <param name="dt">提交的费用明细预算表</param>
    /// <param name="feeDetail">父费用明细</param>
    /// <param name="limitNumber">父费用明细的可用额度</param>
    /// <param name="jobject">网点 实体object</param>
    private static void AddFeeDetailToBranch(DataTable feeDetailDT,DataTable dt, string feeDetail, string limitNumber,ref JObject jobject)
    {
        jobject.Add(feeDetail + "可用额度", limitNumber);
        double sum = 0;
        foreach (DataRow row in feeDetailDT.Rows)
        {
            if(row["ParentName"].ToString()==feeDetail|| (row["ParentName"].ToString()==""&& row["Name"].ToString() == feeDetail && feeDetailDT.Select("ParentName='" + feeDetail + "'").Length == 0))
            {
                
                jobject.Add(feeDetail+row["Name"].ToString(), "0");
                jobject.Add(feeDetail+row["Name"].ToString()+ "Original", "0");
                jobject.Add(feeDetail + row["Name"].ToString() + "Id", row["Id"].ToString());
                foreach (DataRow dw in dt.Rows)
                {

                    if (dt.Columns.Contains("FeeDetailId"))
                    {
                        if (dw["FeeDetailId"].ToString() == row["Id"].ToString() && dw["HospitalId"].ToString() == jobject["HospitalId"].ToString() &&
                           dw["SupervisorId"].ToString() == jobject["SupervisorId"].ToString() && dw["ProductId"].ToString() == jobject["ProductId"].ToString())
                        {
                            jobject[feeDetail + row["Name"].ToString()] = dw["Budget"].ToString();
                            jobject[feeDetail + row["Name"].ToString() + "Original"] = dw["Budget"].ToString();
                            sum += StringTools.StringToDouble(dw["Budget"].ToString());
                        }
                    }
                    else
                    {
                        if (dw["FeeDetailName"].ToString() ==feeDetail && dw["HospitalId"].ToString() == jobject["HospitalId"].ToString() &&
                           dw["SupervisorId"].ToString() == jobject["SupervisorId"].ToString() && dw["ProductId"].ToString() == jobject["ProductId"].ToString())
                        {
                            jobject[feeDetail + row["Name"].ToString()] = "0";
                            jobject[feeDetail + row["Name"].ToString() + "Original"] ="0";
                            sum += StringTools.StringToDouble(dw["LimitNumber"].ToString());
                        }
                    }
                }
            }
        }
        jobject.Add(feeDetail + "上报预算", sum);
    }

    /// <summary>
    /// 地区提交预算
    /// </summary>
    /// <param name="jarray">网点JArray</param>
    /// <param name="departmentId">地区部门ID</param>
    /// <param name="userId">地区部门负责人ID</param>
    /// <param name="n">尝试插入数据库的次数</param>
    /// <returns></returns>
    private static JObject LeafDepartmentSubmit(JArray jarray,string departmentId, string userId,int n, string userName)
    {
        JObject res = new JObject();
        string errMsg = "";
        if (n == 0)
        {
            res.Add("ErrCode", 3);
            res.Add("ErrMsg", "当前用户过多，链接超时，请重新操作");
        }
        else
        {
            DataSet ds = SalesBudgetApplicationSrv.GetSubmitMaxDocNo(departmentId, ref errMsg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else
            {
                DateTime today = DateTime.Now;
                long DocMax = Convert.ToInt64(ds.Tables[0].Rows[0][0]) + 1;//提交预算部门表最大DocNo
                int idMax = Convert.ToInt32(ds.Tables[1].Rows[0][0]);//网点预算表的最大ID
                if (DocMax == 1)
                {
                    DocMax = Convert.ToInt64(today.ToString("yyyyMMdd") + "000001");
                }

                JObject departmentObject = new JObject();//插入提交预算部门表

                departmentObject.Add("DocNo", DocMax);
                departmentObject.Add("DepartmentId", departmentId);
                departmentObject.Add("SubmitterId", userId);
                departmentObject.Add("CreateTime", today);

                string sql = SqlHelper.GetInsertString(departmentObject, "yl_sales_department_budget_record");

                List<JObject> branchList = new List<JObject>();
                List<JObject> feeDetailList = new List<JObject>();
                List<JObject> RecordList = new List<JObject>();

                foreach (JObject jobject in jarray)
                {
                    JObject branchObject = new JObject();

                    branchObject.Add("ProductId", jobject["ProductId"].ToString());//插入提交预算网点表
                    branchObject.Add("HospitalId", jobject["HospitalId"].ToString());
                    branchObject.Add("SupervisorId", jobject["SupervisorId"].ToString());
                    branchObject.Add("DepartmentRecordDoc", DocMax);
                    branchObject.Add("CreateTime", today);
                    branchObject.Add("Id", ++idMax);

                    branchList.Add(branchObject);

                    foreach (var ss in jobject)
                    {
                        string ParentName = GetParentFeeDetail(ds.Tables[2], ss.Key.ToString());
                        if (ParentName != null)
                        {
                            JObject feeDetailObject = new JObject();//插入提交预算明细表

                            feeDetailObject.Add("Budget", ss.Value);
                            feeDetailObject.Add("HospitalRecordId", idMax);
                            feeDetailObject.Add("FeeDetailId", jobject[ss.Key.ToString() + "Id"]);
                            feeDetailObject.Add("ApproverUserId", ds.Tables[3].Rows[0]["userId"].ToString());//下一级审批人
                            feeDetailObject.Add("ApprovalDepartmentId", ds.Tables[3].Rows[0]["departmentId"].ToString());//下一级审批人所在部门

                            feeDetailList.Add(feeDetailObject);

                            JObject recordObject = new JObject();//插入提交预算操作记录表

                            recordObject.Add("ProductId", jobject["ProductId"].ToString());
                            recordObject.Add("HospitalId", jobject["HospitalId"].ToString());
                            recordObject.Add("SupervisorId", jobject["SupervisorId"].ToString());
                            recordObject.Add("DepartmentDocNo", DocMax);
                            recordObject.Add("CreateTime", today);
                            recordObject.Add("ApproverUserId", userId);
                            recordObject.Add("ApproverDepartmentId", departmentId);
                            recordObject.Add("FeeDetailId", jobject[ss.Key.ToString() + "Id"]);
                            recordObject.Add("OriginValue", "0");//原始值
                            recordObject.Add("ModifiedValue", ss.Value);//新值

                            RecordList.Add(recordObject);
                        }
                    }
                }
                sql +=  SqlHelper.GetInsertString(branchList, "yl_sales_hospital_budget_record");
                sql +=  SqlHelper.GetInsertString(feeDetailList, "yl_budget_detail");
                sql +=  SqlHelper.GetInsertString(RecordList, "budget_submit_record");

                SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sql));
                if (msg.Result == SqlExceRes.ResState.Success)
                {
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");

                    WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper("I-0W2FLXN_BKL-mORN8hxwzDzYCOUzOIvi_JUrtGCTU", "业力费用管控", "1000016");

                    // 发消息给下级审批人
                    wxNetSalesHelper.GetJsonAndSendWxMsg(ds.Tables[3].Rows[0]["userId"].ToString(), "请及时审批 提交人为:" + userName + "的单据,谢谢!", "http://yelioa.top/web/budget_submit/BudgetSubmitDetail.aspx", "1000016");
                    //发消息给提交人
                    wxNetSalesHelper.GetJsonAndSendWxMsg(userId, "您的单据已提交,请在电脑端及时查看，谢谢!", "http://yelioa.top/web/budget_submit/BudgetSubmitDetail.aspx", "1000016");
                }
                else if (msg.Result == SqlExceRes.ResState.Error)
                {
                    Thread.Sleep(100);//等待0.1s后再进行插入数据库
                    return LeafDepartmentSubmit(jarray, departmentId, userId, n - 1, userName);
                }
            }
        }
        return res;
    }

    /// <summary>
    /// 地区以上提交预算
    /// </summary>
    /// <param name="jarray">网点JArray</param>
    /// <param name="DocNo">地区提交的单据号</param>
    /// <param name="departmentId">审批人部门ID</param>
    /// <param name="userId">审批人ID</param>
    /// <returns></returns>
    private static JObject DepartmentSubmit(JArray jarray, string DocNo, string departmentId, string userId, string userName)
    {
        JObject res = new JObject();
        string errMsg = "";
       
        WxUserInfo wxUserInfo = new WxUserInfo();
     
        DataSet ds = SalesBudgetApplicationSrv.GetFeeDetail(departmentId,ref errMsg);
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", errMsg);
        }
        else
        {
            string sql = "";
            string SubmitterId = "";
            string superivsorIdList = "";
            string feeDetailList = "";

            DateTime today = DateTime.Now;

            List<JObject> RecordList = new List<JObject>();
            int IsOverBudget = 0;//超预算
            
            foreach (JObject jobject in jarray)
            {
                foreach (var ss in jobject)
                {
                    string ParentName = GetParentFeeDetail(ds.Tables[0], ss.Key.ToString());
                    if (ParentName != null)
                    {
                        //获取下个审批人
                        JObject feeDetailObject = GetNextHeader(userId, ds.Tables[1], ParentName, departmentId, ds.Tables[2].Rows[0]["userId"].ToString(), ds.Tables[2].Rows[0]["departmentId"].ToString());
                        feeDetailObject.Add("Budget", ss.Value);
                        if (ParentName == "市场学术费")
                        {
                            if (StringTools.StringToDouble(jobject[ParentName + "可用额度"].ToString()) >= StringTools.StringToDouble(jobject[ParentName + "上报预算"].ToString()) && departmentId == "292")
                            {
                                feeDetailObject["ApproverUserId"] = null;
                                feeDetailObject["ApprovalDepartmentId"] = null;
                            }
                            else if(StringTools.StringToDouble(jobject[ParentName + "可用额度"].ToString()) >=StringTools.StringToDouble(jobject[ParentName + "上报预算"].ToString()) && ds.Tables[2].Rows[0]["departmentId"].ToString()=="291")
                            {
                                feeDetailObject["ApproverUserId"] = "100000150";
                                feeDetailObject["ApprovalDepartmentId"] ="292";
                            }
                        }
                        else if (ParentName == "商务费用金额")
                        {
                           if (StringTools.StringToDouble(jobject[ParentName + "可用额度"].ToString()) >= StringTools.StringToDouble(jobject[ParentName + "上报预算"].ToString()) && departmentId == "293")
                            {
                                feeDetailObject["ApproverUserId"] = null;
                                feeDetailObject["ApprovalDepartmentId"] = null;
                            }
                            else if (StringTools.StringToDouble(jobject[ParentName + "可用额度"].ToString()) >= StringTools.StringToDouble(jobject[ParentName + "上报预算"].ToString()) && ds.Tables[2].Rows[0]["departmentId"].ToString() == "291")
                            {
                                feeDetailObject["ApproverUserId"] = "100000225";
                                feeDetailObject["ApprovalDepartmentId"] = "293";
                            }
                        }
                        else if(!string.IsNullOrEmpty(feeDetailObject["ApproverUserId"].ToString()))
                        {
                            if (IsOverBudget < 2)
                            {
                              
                                if (StringTools.StringToDouble(jobject[ParentName + "可用额度"].ToString()) >= StringTools.StringToDouble(jobject[ParentName + "上报预算"].ToString()) && ds.Tables[1].Select("userId=" + feeDetailObject["ApproverUserId"].ToString()).Length == 0 &&
                                    (departmentId == "290" || ds.Tables[2].Rows[0]["departmentId"].ToString() == "291"))//判断是否超预算且下一级审批是否是销售总监
                                {
                                    IsOverBudget = 1;
                                }
                                else if (StringTools.StringToDouble(jobject[ParentName + "可用额度"].ToString()) < StringTools.StringToDouble(jobject[ParentName + "上报预算"].ToString()))
                                {
                                    IsOverBudget = 2;
                                }
                            }
                        }
                        if (!feeDetailList.Contains(ParentName))
                        {
                            feeDetailList += "'" + ParentName + "',";
                        }

                        if (feeDetailObject["SupervisorId"] != null && !superivsorIdList.Contains(feeDetailObject["SupervisorId"].ToString()))
                        {
                            superivsorIdList += wxUserInfo.userIdToWechatUserId(feeDetailObject["SupervisorId"].ToString()) + "|";
                        }

                        string condition = string.Format("where HospitalRecordId=(SELECT Id from yl_sales_hospital_budget_record where DepartmentRecordDoc='{0}'" +
                            " and SupervisorId={1} and ProductId={2} and HospitalId={3}) and feeDetailId='{4}'", DocNo, jobject["SupervisorId"].ToString(),
                            jobject["ProductId"].ToString(), jobject["HospitalId"].ToString(), jobject[ss.Key.ToString() + "Id"].ToString());

                        sql += SqlHelper.GetUpdateString(feeDetailObject, "yl_budget_detail", condition);

                        JObject recordObject = new JObject();//插入提交预算操作记录表

                        recordObject.Add("ProductId", jobject["ProductId"].ToString());
                        recordObject.Add("HospitalId", jobject["HospitalId"].ToString());
                        recordObject.Add("SupervisorId", jobject["SupervisorId"].ToString());
                        recordObject.Add("DepartmentDocNo", DocNo);
                        recordObject.Add("CreateTime", today);
                        recordObject.Add("ApproverUserId", userId);
                        recordObject.Add("ApproverDepartmentId", departmentId);
                        recordObject.Add("FeeDetailId", jobject[ss.Key.ToString() + "Id"]);
                        recordObject.Add("OriginValue", jobject[ss.Key.ToString() + "Original"]);//原始值
                        recordObject.Add("ModifiedValue", ss.Value);//新值

                        RecordList.Add(recordObject);
                    }
                }
            }
            if (IsOverBudget == 1)//没超预算则将该单据审批人全部重新设置为null，审批到此结束
            {
                sql += string.Format(";update yl_budget_detail SET ApproverUserId=0,ApprovalDepartmentId=0 where HospitalRecordId in " +
                    "(SELECT Id FROM yl_sales_hospital_budget_record where DepartmentRecordDoc='{0}') and FeeDetailId not in " +
                                     "(select id from fee_detail_dict_copy where parentName='市场学术费' or parentName='商务费用金额');", DocNo);

            }

            sql += SqlHelper.GetInsertString(RecordList, "budget_submit_record");

            SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sql));
            if (msg.Result == SqlExceRes.ResState.Success)
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功");

                SubmitterId = InsertTheFirstToDistribute(DocNo, feeDetailList.Substring(0, feeDetailList.Length - 1), 10);

                WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper("I-0W2FLXN_BKL-mORN8hxwzDzYCOUzOIvi_JUrtGCTU", "业力费用管控", "1000016");

                //                if ((IsOverBudget == 0 || IsOverBudget == 2) && superivsorIdList.Length > 0)
                //                {
                // 发消息给下级审批人
                try
                {
                    wxNetSalesHelper.GetJsonAndSendWxMsg(superivsorIdList, "请在电脑端及时审批 提交人为:" + userName + "的单据,谢谢!", "http://yelioa.top/web/budget_submit/BudgetSubmitDetail.aspx", "1000016");
                    //发消息给修改人
                    wxNetSalesHelper.GetJsonAndSendWxMsg(wxUserInfo.userIdToWechatUserId(userId), "您的单据已提交,请在电脑端及时查看，谢谢!", "http://yelioa.top/web/budget_submit/BudgetSubmitDetail.aspx", "1000016");
                    // 发消息给提交人
                    wxNetSalesHelper.GetJsonAndSendWxMsg(wxUserInfo.userIdToWechatUserId(SubmitterId), "您的单据已被审批,请在电脑端及时查看，谢谢!", "http://yelioa.top/web/budget_submit/BudgetSubmitDetail.aspx", "1000016");
                }
                catch(Exception e)
                {
                    LogHelper.WriteLog(LogFile.Trace, "发送消息人员列表为空，具体报错信息为:" + e.Message);
                }
            }

            else if (msg.Result == SqlExceRes.ResState.Error)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", msg.ExceMsg);
            }
        }
        return res;
    }

    /// <summary>
    /// 获取下一个审批人
    /// </summary>
    /// <param name="userId">审批人ID</param>
    /// <param name="dt">审批部门的所有负责人</param>
    /// <param name="parentName">父费用明细</param>
    /// <param name="departmentId">审批部门</param>
    /// <param name="parentDepartmentHeaderId">审批部门的父部门第一个审批人ID</param>
    /// <returns></returns>
    private static JObject GetNextHeader(string userId,DataTable dt,string parentName,string departmentId,string parentDepartmentHeaderId,string parentDepartmentId)
    {
        JObject res = new JObject();
        if(departmentId=="284")
        {
            res.Add("ApproverUserId", null);
            res.Add("ApprovalDepartmentId",null);
            return res;
        }
        for(int i=0;i<dt.Rows.Count;i++)
        {
            if(dt.Rows[i]["userId"].ToString()==userId&&i<dt.Rows.Count-1&&userId!= "100000142")
            {
                res.Add("ApproverUserId", dt.Rows[i + 1]["userId"].ToString());
                res.Add("ApprovalDepartmentId", dt.Rows[i + 1]["departmentId"].ToString());
                return res;
            }
        }
        if(parentName=="市场学术费"&&(departmentId=="291"|| departmentId=="290"))
        {
            //市场部负责人--黄礼君
            res.Add("ApproverUserId", "100000150");
            res.Add("ApprovalDepartmentId", "292");
            return res;            
        }
        if (parentName == "商务费用金额" && (departmentId == "291" || departmentId == "290"))
        {
            //运营部负责人--程丹凤
            res.Add("ApproverUserId", "100000225");
            res.Add("ApprovalDepartmentId", "293");
            return res;
        }
        res.Add("ApproverUserId", parentDepartmentHeaderId);
        res.Add("ApprovalDepartmentId", parentDepartmentId);
        return res;
       
    }

    private static string InsertTheFirstToDistribute(string departmentDocNo,string feeDetailList,int n)
    {
        DateTime today = DateTime.Now;
        string sql = "";
        DataSet ds = SalesBudgetApplicationSrv.InsertTheFirstToDistribute(departmentDocNo,feeDetailList);
        long Max = Convert.ToInt64(ds.Tables[2].Rows[0][0]) + 1;
        if (Max == 1)
        {
            Max = Convert.ToInt64(today.ToString("yyyyMMdd") + "000001");
        }
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            int count = ds.Tables[1].Select("BudgetLimitType='" + row["feeDetail"].ToString() + "'").Length;
            if (count>0)
            {
                DataRow dw = ds.Tables[1].Select("BudgetLimitType='" + row["feeDetail"].ToString() + "'")[0];

                Dictionary<string, string> updatesql = new Dictionary<string, string>();
                double budgetLimit = StringTools.StringToDouble(dw["BudgetLimit"].ToString()) + StringTools.StringToDouble(row["Budget"].ToString());
                updatesql.Add("BudgetLimit", budgetLimit.ToString());

                string condition = string.Format("Id={0}", dw["Id"].ToString());

                sql += SqlHelper.GetUpdateString(updatesql, "yl_budget_distribute", condition)+";";
            }
            else
            {
                
                Dictionary<string, string> insertsql = new Dictionary<string, string>();

                insertsql.Add("BudgetLimitType", row["feeDetail"].ToString());//费用明细
                insertsql.Add("DocNo", Max++.ToString());//单据号
                insertsql.Add("BudgetLimit", row["Budget"].ToString());//可分配预算
                insertsql.Add("CreateTime", today.ToString());//创建时间
                insertsql.Add("DepartmentId", "284");//分配预算的部门ID
                insertsql.Add("IsFinished", "0");//是否已经分配预算，0表示还没有，1表示已经分配

                sql += SqlHelper.GetInsertString(insertsql, "yl_budget_distribute");
            }
        }
        if(sql!="")
        {
            sql = sql.Substring(0, sql.Length - 1);
            SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sql));
            if (msg.Result == SqlExceRes.ResState.Success)
            {                
                return ds.Tables[3].Rows[0]["SubmitterId"].ToString();
            }
            else if (msg.Result == SqlExceRes.ResState.Error)
            {
                Thread.Sleep(100);//等待0.1s后再进行插入数据库
                return InsertTheFirstToDistribute(departmentDocNo, feeDetailList, n - 1);
            }
        }
        return ds.Tables[3].Rows[0]["SubmitterId"].ToString();
    }

    private static string GetParentFeeDetail(DataTable dt,string key)
    {
        foreach(DataRow row in dt.Rows)
        {
            if(key==row["ParentName"].ToString()+row["Name"].ToString() )
            {
                return row["ParentName"].ToString();
            }
            else if (key == row["Name"].ToString() + row["Name"].ToString())
            {
                return row["Name"].ToString();
            }
        }
        return null;
    }

    public static string GetParnetDepartmentHeader(int departmentId)
    {
        JObject res = new JObject();
        string errMsg = "";
        DataSet ds = SalesBudgetApplicationSrv.GetParnetDepartmentHeader(departmentId, ref errMsg);//仅作为判断是否是地区
        if (ds == null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", errMsg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "无上级部门或上级部门无负责人");
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "操作成功");
            res.Add("doc", JsonHelper.DataTable2Json(ds.Tables[0]));
        }
        return res.ToString();
    }

    /// <summary>
    /// 获取该成员负责的网点
    /// </summary>
    /// <param name="userId">成员ID</param>
    /// <returns></returns>
    public static string getRelatedBranch(string userId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            string errMsg = "";

            DataSet ds = SalesBudgetApplicationSrv.getRelatedBranch(userId, ref errMsg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else
            {
                if (ds.Tables[0].Rows.Count == 0 || ds.Tables[1].Rows.Count == 0)
                {
                    res.Add("ErrCode", 3);
                    res.Add("ErrMsg", "暂无数据");
                }
                else
                {
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");

                    DataTable hospitalDt = ds.Tables[0];
                    DataTable departmentDt = ds.Tables[1];

                    res.Add("RelatedHospital", JsonHelper.DataTable2Json(hospitalDt));
                    res.Add("RelatedDepartment", JsonHelper.DataTable2Json(departmentDt));
                }
            }
        }

        return res.ToString();
    }

    public static string getRelatedBranchProduct(string userId, string hospitalId)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(hospitalId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            string errMsg = "";

            DataSet ds = SalesBudgetApplicationSrv.getRelatedBranchProduct(userId, hospitalId, ref errMsg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", errMsg);
            }
            else
            {
                if (ds.Tables[0].Rows.Count == 0)
                {
                    res.Add("ErrCode", 3);
                    res.Add("ErrMsg", "暂无数据");
                }
                else
                {
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");

                    DataTable productDt = ds.Tables[0];

                    res.Add("RelatedProduct", JsonHelper.DataTable2Json(productDt));
                }
            }
        }

        return res.ToString();
    }

    public static string submitExpectFlow(JObject jObject, UserInfo userInfo, List<DepartmentPost> departmentList)
    {
        jObject.Remove("action");

        jObject.Add("DocCode", GenerateDocCode.getExpectFlowCode());

        if (jObject["SupervisorId"] == null || "".Equals(jObject["SupervisorId"]))
        {
            jObject.Add("SupervisorId", userInfo.userId.ToString());
        }

        jObject.Add("isFinished", 0);

        jObject.Add("CreateTime", DateTime.Now.ToString());

        string errorMsg = "";   JObject res = new JObject();

        // 判断数据库是否存在该网点的预计流向
        DataSet determineInsertOrUpdateRecordDs = SalesBudgetApplicationSrv.determineInsertOrUpdateRecord(jObject["HospitalId"].ToString(), 
            jObject["ProductId"].ToString(), jObject["SupervisorId"].ToString(), ref errorMsg, userInfo);

        if (determineInsertOrUpdateRecordDs != null && determineInsertOrUpdateRecordDs.Tables[0].Rows.Count > 0)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "每个月每个网点只能提交一次预计流向");

            return res.ToString();
        }

        List<string> sqlList = new List<string>(); // 存放新增或更新的sql的集合
        JObject tempJObject = new JObject();    // 用来新增或更新操作的jObject
        string recordDocCode = "";
        string superviosrWechatUserIds = "";   // 处理发消息给下级审批人的字符串
        WxUserInfo wxUserInfo = new WxUserInfo();

        string thirdDepartment = determineInsertOrUpdateRecordDs.Tables[1].Rows[0][0].ToString();

            // 如果不存在，说明是第一次提交，则预计流向单据表是新增
        determineInsertOrUpdateRecordDs = SalesBudgetApplicationSrv.determineInsertOrUpdateRecord(jObject["HospitalId"].ToString(), jObject["ProductId"].ToString(),
            jObject["SupervisorId"].ToString(), ref errorMsg);
        if (determineInsertOrUpdateRecordDs.Tables[0].Rows.Count == 0)
        {
            // 找到提交人上级部门 第一次提交 业务员都只有一个部门 所以只需要找出来取table[0]
            int selfDepartmentId = Int32.Parse(thirdDepartment);
            int parentDepartmentId = Int32.Parse(MobileReimburseSrv.findParentIdById(selfDepartmentId).Tables[0].Rows[0][0].ToString());
            jObject.Add("ApproverDepartmentId", parentDepartmentId);
            // 找到部门的负责人
            DataSet ds = SalesBudgetApplicationSrv.GetParnetDepartmentHeader(selfDepartmentId, ref errorMsg);

            if (ds.Tables[0].Rows.Count == 0)
            {
                parentDepartmentId = Int32.Parse(MobileReimburseSrv.findParentIdById(parentDepartmentId).Tables[0].Rows[0][0].ToString());
                ds = SalesBudgetApplicationSrv.GetSelfDepartmentHeader(selfDepartmentId, ref errorMsg);
                jObject.Remove("ApproverDepartmentId");
                jObject.Add("ApproverDepartmentId", parentDepartmentId);
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                string approverUserId = "";

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    // 处理有多个上级审批人的情况
                    approverUserId += dr["userId"] + ",";
                    superviosrWechatUserIds += wxUserInfo.userIdToWechatUserId(dr["userId"].ToString()) + "|";
                }

                approverUserId = approverUserId.Substring(0, approverUserId.Length - 1);
                jObject.Add("ApproverUserId", approverUserId);
            }
            else
            {
                // 没找到上级部门负责人
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", "上级部门在组织架构中不存在负责人，请及时联系人事专员予以确认。");

                return res.ToString();
            }

            // 先插数据到单据表
            sqlList.Add(SqlHelper.GetInsertString(jObject, "expect_flow_submit"));
            recordDocCode = jObject["DocCode"].ToString();
        }
        else
        {
            // 判断审批人的个数是否和记录表的记录对应上，否则不修改单据表
            string approverUserIds = determineInsertOrUpdateRecordDs.Tables[0].Rows[0][1].ToString();
            int num_of_approver = approverUserIds.Split(',').Length;

            string findDocCode = determineInsertOrUpdateRecordDs.Tables[0].Rows[0][0].ToString();

            DataSet numRecordDs = SalesBudgetApplicationSrv.checkNumOfRecord(approverUserIds, findDocCode);

            int num_of_record = numRecordDs.Tables[0].Rows.Count;

            // 如果存在，则说明是上级审批的过程，则预计流向单据表更新 更新预计流向，上级审批人，上级部门
            tempJObject.Add("ExpectFlow", jObject["ExpectFlow"]);

            if (num_of_approver == num_of_record + 1)
            {
                // 找到上级部门，由于可能存在修改人在多个部门任职，所以这里的本部门需要取从页面上传回来的
                int selfDepartmentId = Int32.Parse(jObject["DepartmentId"].ToString());
                int parentDepartmentId = Int32.Parse(MobileReimburseSrv.findParentIdById(selfDepartmentId).Tables[0].Rows[0][0].ToString());

                //// 需要判断用户在此部门是否是负责人
                //int isHead = 0;
                //foreach (DepartmentPost departmentPost in departmentList)
                //{
                //    if (departmentPost.departmentId == selfDepartmentId && departmentPost.isHead == 1)
                //        isHead = 1;
                //}

                //DataSet ds = null;
                //if (isHead == 1)
                //{
                // 如果是负责人 找到提交人上级部门的负责人
                DataSet ds = SalesBudgetApplicationSrv.GetParnetDepartmentHeader(selfDepartmentId, ref errorMsg);
                    tempJObject.Add("ApproverDepartmentId", parentDepartmentId);
                //}
                //else
                //{
                //    // 如果是非负责人 找到提交人上级部门的负责人
                //    ds = SalesBudgetApplicationSrv.GetSelfDepartmentHeader(selfDepartmentId, ref errorMsg);
                //    tempJObject.Add("ApproverDepartmentId", selfDepartmentId);
                //}

                // 如果找不到就再向上招一级
                if (ds.Tables[0].Rows.Count == 0)
                {
                    parentDepartmentId = Int32.Parse(MobileReimburseSrv.findParentIdById(parentDepartmentId).Tables[0].Rows[0][0].ToString());
                    ds = SalesBudgetApplicationSrv.GetSelfDepartmentHeader(selfDepartmentId, ref errorMsg);
                    tempJObject.Remove("ApproverDepartmentId");
                    tempJObject.Add("ApproverDepartmentId", parentDepartmentId);
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string approverUserId = "";

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        // 处理有多个上级审批人的情况
                        approverUserId += dr["userId"] + ",";
                        superviosrWechatUserIds += wxUserInfo.userIdToWechatUserId(dr["userId"].ToString()) + "|";
                    }

                    approverUserId = approverUserId.Substring(0, approverUserId.Length - 1);

                    // 如果上级审批人是战区总监 则说明审批结束
                    //if (tempJObject["ApproverDepartmentId"].ToString()== "290" || tempJObject["ApproverDepartmentId"].ToString() == "291")
                    //{
                        tempJObject.Add("IsFinished", 1);
                        InsertAvailableCredit(jObject["ProductId"].ToString(),jObject["HospitalId"].ToString(), jObject["SupervisorId"].ToString(),
                            jObject["ExpectFlow"].ToString(), DateTime.Now);
                    

                    tempJObject.Add("ApproverUserId", approverUserId);
                }
                else
                {
                    // 没找到上级部门负责人
                    

                    res.Add("ErrCode", 2);
                    res.Add("ErrMsg", "上级部门在组织架构中不存在负责人，请及时联系人事专员予以确认。");

                    return res.ToString();
                }
            }

            sqlList.Add(SqlHelper.GetUpdateString(tempJObject, "expect_flow_submit", "where docCode = " + findDocCode));

            recordDocCode = findDocCode;
        }

        //插数据到记录表 因为不管是地区还是大区还是战区 记录表都是新增
        tempJObject = new JObject();
        tempJObject.Add("OriginValue", jObject["OriginExpectFlow"]);
        tempJObject.Add("ModifiedValue", StringTools.StringToDouble(jObject["ExpectFlow"].ToString()));
        tempJObject.Add("ModifiedUserId", userInfo.userId.ToString());
        tempJObject.Add("DocCode", recordDocCode);
        tempJObject.Add("Remark", jObject["Remark"]);
        tempJObject.Add("CreateTime", jObject["CreateTime"]);

        sqlList.Add(SqlHelper.GetInsertString(tempJObject, "expect_flow_submit_record"));

        SqlExceRes sqlExce = new SqlExceRes(SqlHelper.Exce(sqlList.ToArray()));

        if (sqlExce.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");

            //WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper("I-0W2FLXN_BKL-mORN8hxwzDzYCOUzOIvi_JUrtGCTU", "业力费用管控", "1000016");
            //// 发消息给提交人
            //wxNetSalesHelper.GetJsonAndSendWxMsg(userInfo.wechatUserId, "你的单据已提交,谢谢!", "http://yelioa.top/web/expect_flow_submit/mExpectFlowRecordList.aspx", "1000016");
            //// 发消息给下级审批人
            //try
            //{
            //    superviosrWechatUserIds = superviosrWechatUserIds.Substring(0, superviosrWechatUserIds.Length - 1);
            //    wxNetSalesHelper.GetJsonAndSendWxMsg(superviosrWechatUserIds, "请及时审批 提交人为:" + userInfo.userName + "的单据,谢谢!", "http://yelioa.top/web/expect_flow_submit/mExpectFlowApprovalList.aspx", "1000016");
            //}
            //catch (Exception e)
            //{
            //    LogHelper.WriteLog(LogFile.Trace, "发消息人员列表为空");
            //}
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "操作失败");
        }

        return res.ToString();
    }

    /// <summary>
    /// 计算可用额度
    /// </summary>
    /// <param name="productId">产品ID</param>
    /// <param name="hospitalId">医院ID</param>
    /// <param name="SupervisorId">销售代表ID</param>
    /// <param name="nextMonthFlow">下月预计流向</param>
    /// <param name="date">时间</param>
    /// <returns></returns>
    public static JObject InsertAvailableCredit(string productId, string hospitalId, string SupervisorId,string nextMonthFlow,DateTime date)
    {
        JObject res = new JObject();
        DataSet ds = SalesBudgetApplicationSrv.GetExpectedFlowThisMonth(productId, hospitalId, SupervisorId,date);
        JObject data = JObject.Parse(ds.Tables[1].Rows[0]["DataJson"].ToString());
        List<string> departmentIdList = new List<string>();
        departmentIdList.Add(ds.Tables[1].Rows[0]["FirstDepartmentId"].ToString());//战区（如果大区是独立大区，则此处是销售部）
        departmentIdList.Add(ds.Tables[1].Rows[0]["SecondDepartmentId"].ToString());//大区
        departmentIdList.Add(ds.Tables[1].Rows[0]["ThirdDepartmentId"].ToString());//地区
        
        if(ds.Tables[1].Rows[0]["FirstDepartmentId"].ToString()!="291"&& ds.Tables[1].Rows[0]["FirstDepartmentId"].ToString() != "290")
        {
            departmentIdList.Add("291");//销售部
        }
        departmentIdList.Add("284");//营销中心
        DataSet ReimbursementDS = SalesBudgetApplicationSrv.GetMobileReimbursementSynthesis(departmentIdList);
        string sqls = "";
        string ExpectFlow = "0";//本月预计流向
        if (ds.Tables[0].Rows.Count!=0)
        {
            ExpectFlow = ds.Tables[0].Rows[0][0].ToString();
        }
        foreach (var ss in data)
        {
            if(ds.Tables[3].Select("Name='"+ss.Key+"'").Length>0)
            {
                JObject temp = new JObject();
                temp.Add("ProductId", productId);
                temp.Add("HospitalId", hospitalId);
                temp.Add("SupervisorId", SupervisorId);
                temp.Add("FeeDetailName", ss.Key);
                temp.Add("CreateTime", DateTime.Now);

                double sum = (StringTools.StringToDouble(nextMonthFlow)+StringTools.StringToDouble(ExpectFlow))* StringTools.StringToDouble(ss.Value.ToString());
                DataRow[] rows = ds.Tables[2].Select("fee_detail like '" + ss.Key + "%'");

                foreach(DataRow row in rows)
                {
                    string feeAmount = row["feeAmount"].ToString();//移动报销金额
                    if (string.IsNullOrEmpty(feeAmount))
                    {
                        feeAmount = "0";
                    }
                    sum -= StringTools.StringToDouble(feeAmount);
                }

                foreach(DataRow row in ReimbursementDS.Tables[0].Rows)
                {
                    string feeAmount = row["feeAmount"].ToString();//移动报销金额
                    if (string.IsNullOrEmpty(feeAmount))
                    {
                        feeAmount = "0";
                    }
                    sum -= GetDistributionRatio(row["departmentId"].ToString(), productId, hospitalId, SupervisorId, ds.Tables[4], ds.Tables[5], ss.Key.ToString(),
                               ExpectFlow, ss.Value.ToString()) * StringTools.StringToDouble(feeAmount);
                }

                temp.Add("LimitNumber", sum);
                sqls += SqlHelper.GetInsertString(temp, "branch_available_credit");
            }
        }
        sqls = sqls.Substring(0, sqls.Length - 1);
        SqlExceRes msg = new SqlExceRes(SqlHelper.Exce(sqls));
        if (msg.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");
        }
        else if (msg.Result == SqlExceRes.ResState.Error)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", msg.ExceMsg);
        }
        return res;
    }

    /// <summary>
    /// 获取某个网点的某个上级部门的某个费用明细综合费用占比
    /// </summary>
    /// <param name="departmentId">部门ID</param>
    /// <param name="productId">产品ID</param>
    /// <param name="hospitalId">医院ID</param>
    /// <param name="SupervisorId">销售代表ID</param>
    /// <param name="newCostSharingDT">网点费用明细率表</param>
    /// <param name="expectFlowDT">本月预计流向表</param>
    /// <param name="feeDetail">费用明细名称</param>
    /// <param name="expectFlow">该网点的预计流向</param>
    /// <param name="ratio">该网点该费用明细的费用率</param>
    /// <returns></returns>
    public static double GetDistributionRatio(string departmentId,string productId,string hospitalId,string SupervisorId,DataTable newCostSharingDT,
        DataTable expectFlowDT,string feeDetail,string expectFlow,string ratio)
    {
        List<string> departmentIdList = new List<string>();
        if(departmentId=="291")//销售部
        {
            departmentIdList.Add("296");
            departmentIdList.Add("297");
            departmentIdList.Add("298");
            departmentIdList.Add("299");
            departmentIdList.Add("300");
            departmentIdList.Add("301");
            departmentIdList.Add("302");            
        }
        else
        {
            departmentIdList.Add(departmentId);
        }

        double sum = 0;

        foreach(DataRow row in newCostSharingDT.Rows)
        {
            if(departmentIdList.Contains(row["FirstDepartmentId"].ToString())|| departmentIdList.Contains(row["SecondDepartmentId"].ToString())||
                departmentIdList.Contains(row["ThirdDepartmentId"].ToString()))//找出该部门下的网点
            {
                JObject dataJson = JObject.Parse(row["DataJson"].ToString());
                double flow = 0;
                foreach(DataRow dw in expectFlowDT.Rows)
                {
                    if(dw["ProductId"].ToString()== row["ProductId"].ToString()|| dw["HospitalId"].ToString() == row["HospitalId"].ToString()||
                        dw["SupervisorId"].ToString() == row["SupervisorId"].ToString())
                    {
                        flow = StringTools.StringToDouble(dw["ExpectFlow"].ToString());
                    }
                }
                sum += flow * StringTools.StringToDouble(dataJson[feeDetail].ToString());
            }
        }
        if (sum > 0)
        {
            return (StringTools.StringToDouble(ratio) * StringTools.StringToDouble(expectFlow)) / sum;
        }
        else
        {
            return sum;
        }
    }

    //查询待我审批单据列表
    public static string getApprovalListData(UserInfo userInfo)
    {
        string userId = userInfo.userId.ToString();

        string errorMsg = "";   JObject resJObject = new JObject();

        DataSet ds = SalesBudgetApplicationSrv.getApprovalListData(userId, ref errorMsg);

        if (ds == null)
        {
            resJObject.Add("ErrCode", 2);
            resJObject.Add("ErrMsg", errorMsg);
        }
        else
        {
//            for (int i = ds.Tables[0].Rows.Count - 1; i >= 0; i--)
//            {
//                if (ds.Tables[0].Rows[i]["lastApproverId"].ToString().Equals(userInfo.userId.ToString()))
//                {
//                    ds.Tables[0].Rows.RemoveAt(i);
//                }
//            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                resJObject.Add("ErrCode", 3);
                resJObject.Add("ErrMsg", "暂无数据");
            }
            else
            {
                resJObject.Add("ErrCode", 0);
                resJObject.Add("ErrMsg", "操作成功");

                resJObject.Add("approvalListData", JsonHelper.DataTable2Json(ds.Tables[0]));
            }
        }

        return resJObject.ToString();
    }

    public static string getApprovalDetailData(string docCode)
    {
        string errorMsg = ""; JObject resJObject = new JObject();

        DataSet ds = SalesBudgetApplicationSrv.getApprovalDetailData(docCode, ref errorMsg);

        if (ds == null)
        {
            resJObject.Add("ErrCode", 2);
            resJObject.Add("ErrMsg", errorMsg);
        }
        else
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                resJObject.Add("ErrCode", 3);
                resJObject.Add("ErrMsg", "暂无数据");
            }
            else
            {
                resJObject.Add("ErrCode", 0);
                resJObject.Add("ErrMsg", "操作成功");

                resJObject.Add("approvalDetailData", JsonHelper.DataTable2Json(ds.Tables[0]));
                resJObject.Add("approvalDetailDataRecord", JsonHelper.DataTable2Json(ds.Tables[1]));
            }
        }

        return resJObject.ToString();
    }

    public static string getExpectFlowRecord(UserInfo userInfo)
    {
        string errorMsg = ""; JObject resJObject = new JObject();

        DataSet ds = SalesBudgetApplicationSrv.getExpectFlowRecord(userInfo.userId.ToString(), ref errorMsg);

        if (ds == null)
        {
            resJObject.Add("ErrCode", 2);
            resJObject.Add("ErrMsg", errorMsg);
        }
        else
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                resJObject.Add("ErrCode", 3);
                resJObject.Add("ErrMsg", "暂无数据");
            }
            else
            {
                resJObject.Add("ErrCode", 0);
                resJObject.Add("ErrMsg", "操作成功");

                resJObject.Add("approvalRecord", JsonHelper.DataTable2Json(ds.Tables[0]));
            }
        }

        return resJObject.ToString();
    }

    public static string getExpectFlowRecordDetail(string docCode, string userId)
    {
        string errorMsg = ""; JObject resJObject = new JObject();

        DataSet ds = SalesBudgetApplicationSrv.getRecordDetailData(docCode, ref errorMsg, userId);

        if (ds == null)
        {
            resJObject.Add("ErrCode", 2);
            resJObject.Add("ErrMsg", errorMsg);
        }
        else
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                resJObject.Add("ErrCode", 3);
                resJObject.Add("ErrMsg", "暂无数据");
            }
            else
            {
                resJObject.Add("ErrCode", 0);
                resJObject.Add("ErrMsg", "操作成功");

                resJObject.Add("approvalRecordDetail", JsonHelper.DataTable2Json(ds.Tables[0]));
            }
        }

        return resJObject.ToString();
        
    }
}