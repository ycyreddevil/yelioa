using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// SalesLineBudgetApplicationManage 的摘要说明
/// </summary>
public class SalesLineBudgetApplicationManage
{
    public SalesLineBudgetApplicationManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string GetUserDepartmentList(string userId,List<TreeNode> DepartmentTree)
    {
        JObject res = new JObject();
        if (string.IsNullOrEmpty(userId))
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "缺少参数");
        }
        else
        {
            string msg = "";
            DataSet ds = SalesLineBudgetApplicationSrv.GetUserDepartmentList(userId, ref msg);
            if (ds == null)
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", msg);
            }
            else if (ds.Tables[0].Rows.Count == 0)
            {
                res.Add("ErrCode", 3);
                res.Add("ErrMsg", "非法用户");
            }
            else
            {
                List<string> DepartmentList = new List<string>();
                List<string> departmentList = FindDepartmentList(DepartmentTree[0], "291",ref DepartmentList);//销售部及其所有子部门
                departmentList.AddRange(FindDepartmentList(DepartmentTree[0], "290", ref DepartmentList));//直属战区及其所有子部门
                departmentList.Add("284");//营销中心
                DataTable departmentTable = ds.Tables[0];
                departmentTable.Columns.Add("isLeaf", Type.GetType("System.String"));
                for(int i=departmentTable.Rows.Count-1;i>=0;i--)
                {
                    if(!departmentList.Contains(departmentTable.Rows[i]["DepartmentId"].ToString()))
                    {
                        departmentTable.Rows.RemoveAt(i);
                        continue;
                    }
                    if(ds.Tables[1].Select("parentId="+ departmentTable.Rows[i]["DepartmentId"].ToString()).Length==0)
                    {
                        departmentTable.Rows[i]["isLeaf"] = "1";
                    }
                    else
                    {
                        departmentTable.Rows[i]["isLeaf"] = "0";
                    }
                }
                if(departmentTable.Rows.Count==0)
                {
                    res.Add("ErrCode", 4);
                    res.Add("ErrMsg", "用户无访问权限");
                }
                else
                {
                    res.Add("ErrCode", 0);
                    res.Add("ErrMsg", "操作成功");
                    res.Add("Department", JsonHelper.DataTable2Json(departmentTable));
                    if(departmentTable.Rows[0]["isLeaf"].ToString()=="1")
                    {
                       JObject temp= GetNetBelongToMe(departmentTable.Rows[0]["DepartmentId"].ToString());
                        if(temp["ErrCode"].ToString()=="0")
                        {
                            res.Add("Document", temp["Document"]);
                        }
                    }
                }
            }
        }
        return res.ToString();
    }
    private static List<string> FindDepartmentList(TreeNode DepartmentTree,string departmentId,ref List<string> DepartmentList)
    {
        
        foreach (TreeNode node in DepartmentTree.children)
        {
            if(node.id.ToString()==departmentId)
            {
                DepartmentList.Add(node.id.ToString());
                FindDepartmentList(node.children, ref DepartmentList);
            }
            else
            {
                FindDepartmentList(node, departmentId,ref DepartmentList);
            }
        }
        return DepartmentList;
    }
    private static void FindDepartmentList(List<TreeNode> DepartmentTree,ref List<string> DepartmentList)
    {
        foreach (TreeNode node in DepartmentTree)
        {
            if (node.children.Count > 0)
            {
                FindDepartmentList(node.children, ref DepartmentList);
            }
            DepartmentList.Add(node.id.ToString());
        }
    }
    //public static string initForm(string userId)
    //{
    //    JObject res = new JObject();
    //    if(string.IsNullOrEmpty(userId))
    //    {
    //        res.Add("ErrCode", 1);
    //        res.Add("ErrMsg", "缺少参数");    
    //    }
    //    else
    //    {
    //        string ErrMsg = "";
    //        DataSet ds = SalesLineBudgetApplicationSrv.GetUserMsg(userId, ref ErrMsg);
    //        if(ds==null)
    //        {
    //            res.Add("ErrCode", 2);
    //            res.Add("ErrMsg", ErrMsg);
    //        }
    //        else if(ds.Tables[0].Rows.Count==0)
    //        {
    //            res.Add("ErrCode", 3);
    //            res.Add("ErrMsg", "非法用户");
    //        }
    //        else
    //        {
    //            if(ds.Tables[0].Rows[0]["isHead"].ToString()=="0")
    //            {
    //                res = GetNetBelongToMe(userId);
    //            }
    //        }
    //    }
    //    return res.ToString();
    //}

    public static JObject GetNetBelongToMe(string departmentId)
    {
        JObject res = new JObject();
        string msg = "";
        DataSet ds = SalesLineBudgetApplicationSrv.GetNetBelongToMe(departmentId, ref  msg);
        if(ds==null)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {

            res.Add("ErrCode", 4);
            res.Add("ErrMsg", "该成员无负责网点");
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "操作成功");


            double firstAvailableQuota = 0;
            JArray departmentJarray = new JArray();
            JObject departmentObject = new JObject();
            departmentObject.Add("id", departmentId);
            departmentObject.Add("name", FindAndFilterDepartmentNameById(ds.Tables[3], departmentId));
            departmentObject.Add("avatar", "resources/医院.jpg");
            JArray HospitalJarray = new JArray();
            int id = 10000001;
            foreach (DataRow dw1 in ds.Tables[0].Rows)
            {
                
                double secondAvailableQuota = 0;
                JObject hospital = new JObject();//网点，由医院和产品确定

                hospital.Add("firstId", dw1["HospitalId"].ToString());//医院Id
                hospital.Add("secondId", dw1["ProDuctId"].ToString());//产品Id
                hospital.Add("firstName", dw1["HospitalName"].ToString());//医院名称
                hospital.Add("secondName", dw1["ProDuctName"].ToString());//产品名称（包含规格和型号）
                hospital.Add("id",id++);



                hospital.Add("avatar", "resources/医院.jpg");
                JObject dataJson = JObject.Parse(dw1["DataJson"].ToString());//包含各父费用明细的费用率及上月流向

                JArray parentDetailData = new JArray();//父明细列表
                foreach(DataRow dw2 in ds.Tables[1].Rows)
                {
                    JObject parentDetail = new JObject();//父明细
                    double thirdAvailableQuota = Convert.ToDouble(dataJson[dw2["Name"].ToString()])*Convert.ToDouble(dataJson["当月流向数"]);
                    secondAvailableQuota += thirdAvailableQuota;

                    parentDetail.Add("id", dw2["FeeDetailId"].ToString());//父明细Id
                    parentDetail.Add("name", dw2["Name"].ToString());//父明细名称
                    //parentDetail.Add("Rate", dataJson[dw2["Name"].ToString()].ToString());//父明细费用率

                    parentDetail.Add("availableQuota", thirdAvailableQuota);//实际可用额度
                    parentDetail.Add("plannedCost", 0);//计划费用

                    JArray DetailData = new JArray();

                    foreach(DataRow dw3 in ds.Tables[2].Rows)
                    {
                        if (dw3["ParentName"].ToString() == dw2["Name"].ToString())//该父明细下的子明细
                        {
                            JObject Detail = new JObject();//子明细

                            Detail.Add("id", dw3["id"].ToString());//子明细Id
                            Detail.Add("name", dw3["name"].ToString());//子明细名称

                            Detail.Add("plannedCost", 0);//计划费用

                            DetailData.Add(Detail);
                        }
                    }
                    parentDetail.Add("children", DetailData);
                    parentDetailData.Add(parentDetail);
                }
                hospital.Add("plannedCost", 0);//计划费用
                hospital.Add("availableQuota", secondAvailableQuota);//实际可用额度
                hospital.Add("children", parentDetailData);
                HospitalJarray.Add(hospital);
                firstAvailableQuota += secondAvailableQuota;
            }
            departmentObject.Add("availableQuota", firstAvailableQuota);//实际可用额度
            departmentObject.Add("plannedCost", 0);//计划费用
            departmentObject.Add("netChildren", HospitalJarray);
            departmentObject.Add("children", CalculateTheSumOfExpensesOfHospitalsInThisDepartment(departmentObject, ds.Tables[1], ds.Tables[2]));
            departmentJarray.Add(departmentObject);

            res.Add("Document", departmentJarray);
        }
        return res;
    }

    private static JArray CalculateTheSumOfExpensesOfHospitalsInThisDepartment(JObject departmentObject,DataTable parentdt,DataTable dt)
    {
        JArray feeDetailJarray = new JArray();
        foreach(DataRow row in parentdt.Rows)
        {
            JObject parentDetail = new JObject();
            parentDetail.Add("id", row["FeeDetailId"].ToString());//父明细Id
            parentDetail.Add("name", row["Name"].ToString());//父明细名称
            double availableQuota = 0;
            foreach (JObject firstTemp in departmentObject["netChildren"])
            {
                foreach(JObject secondTemp in firstTemp["children"])
                {
                    if(secondTemp["id"].ToString()== row["FeeDetailId"].ToString())
                    {
                        availableQuota += Convert.ToDouble(secondTemp["availableQuota"]);
                        break;
                    }
                }
                
            }

            parentDetail.Add("availableQuota", availableQuota);//实际可用额度
            parentDetail.Add("plannedCost", 0);//计划费用

            JArray DetailData = new JArray();

            foreach (DataRow dw in dt.Rows)
            {
                if (dw["ParentName"].ToString() == row["Name"].ToString())//该父明细下的子明细
                {
                    JObject Detail = new JObject();//子明细

                    Detail.Add("id", dw["id"].ToString());//子明细Id
                    Detail.Add("name", dw["name"].ToString());//子明细名称

                    Detail.Add("plannedCost", 0);//计划费用

                    DetailData.Add(Detail);
                }
            }
            parentDetail.Add("children", DetailData);
            feeDetailJarray.Add(parentDetail);
        }
        return feeDetailJarray;
    }

    private static string FindAndFilterDepartmentNameById(DataTable dt,string id)
    {
        foreach(DataRow row in dt.Rows)
        {
            if(row["Id"].ToString()==id)
            {
                return row["Name"].ToString().Substring(row["Name"].ToString().LastIndexOf("/") + 1, row["Name"].ToString().Length - row["Name"].ToString().LastIndexOf("/") - 1);
            }
        }
        return null;
    }
}