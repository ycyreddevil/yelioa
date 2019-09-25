using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

public partial class BudgetManag : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "initDatagrid")
            {
                InitDatagrid();
            }
            else if (action == "getBudget")
            {
                getBudget();
            }
            else if (action == "initDepartmentTree")
            {
                InitDepartmentTree();
            }
            else if (action == "updateBudget")
            {
                updateBudget();
            }
            else if (action == "import")
            {
                import();
            }
            else if (action == "getTemplate")
            {
                getTemplate();
            }
            Response.End();
        }
        else
        {
            action = Request.Params["act"];
            if (action == "getTemplate")
            {
                Response.Clear();
                getTemplate();
            }
        }
    }

    private void getTemplate()
    {
        DateTime now = DateTime.Now;
        DateTime last = now.AddMonths(-1);

        string sql = string.Format("SELECT import_budget.Id,department.`name`,import_budget.DepartmentId,import_budget.ParentId,"+
            "import_budget.FeeDetail,0 as budget FROM import_budget " +
            "INNER JOIN department ON import_budget.DepartmentId = department.Id " +
            "where import_budget.CreateTime between '{0}-{1}-1 00:00:00 ' and '{2}-{3}-1 00:00:00' order by import_budget.Id;"
            ,  last.Year, last.Month, now.Year, now.Month);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql,ref msg);
        if(ds == null)
        {
            Response.Write(msg);
            return;
        }
        //string path = Server.MapPath("~/Template");
        //if (!Directory.Exists(path))
        //{
        //    Directory.CreateDirectory(path);
        //}
        //path = path + @"\预算导入模板.xls";
        //if (File.Exists(path))
        //    File.Delete(path);
        List<string> listHeadText = new List<string>();
        listHeadText.Add("Id");
        listHeadText.Add("部门名称");
        listHeadText.Add("部门Id");
        listHeadText.Add("父节点Id");
        listHeadText.Add("费用明细");
        listHeadText.Add("预算金额");

        Response.ContentType = "application/vnd.ms-excel";
        Response.ContentEncoding = Encoding.UTF8;
        Response.Charset = "";
        Response.AppendHeader("Content-Disposition", "attachment;filename="
            + HttpUtility.UrlEncode("预算导入模板.xls", Encoding.UTF8));
        Response.BinaryWrite(ExcelHelperV2_0.Export(ds.Tables[0], "预算导入模板", listHeadText.ToArray()).GetBuffer());
        Response.End();
    }

    private void import()
    {
        DataTable dtExl = ExcelHelperV2_0.Import(Request);
        if(dtExl.Rows.Count == 0)
        {
            Response.Write("导入数据失败：数据为空！");
            return;
        }

        List<int> CheckList = new List<int>();
        foreach(DataRow row in dtExl.Rows)
        {
            int val = Convert.ToInt32(row["Id"]);
            if(CheckList.Contains(val))
            {
                Response.Write("导入数据失败：有重复Id！");
                return;
            }
        }

        DateTime now = DateTime.Now;
        DateTime next = now.AddMonths(1);
        string sql = string.Format("delete from import_budget where CreateTime between " +
            "'{0}-{1}-1 00:00:00 ' and '{2}-{3}-1 00:00:00';",now.Year, now.Month, next.Year, next.Month);
        SqlHelper.Exce(sql);
        int startId = 0;
        sql = "select max(Id) from import_budget";
        object obj = SqlHelper.Scalar(sql);
        if (obj != null)
            startId = Convert.ToInt32(obj)+1;

        DataTable dt = new DataTable();
        dt.Columns.Add("Id");
        dt.Columns.Add("DepartmentId");
        dt.Columns.Add("Budget");
        dt.Columns.Add("FeeDetail");
        dt.Columns.Add("ParentId");
        dt.Columns.Add("CreateTime");

        Dictionary<int, int> dictId = new Dictionary<int, int>();
        foreach(DataRow row in dtExl.Rows)
        {
            DataRow r = dt.NewRow();
            int ParentId = Convert.ToInt32(row["父节点Id"]);
            if (ParentId == -1)
            {
                dictId.Add(Convert.ToInt32(row["Id"]), startId);
                r["ParentId"] = -1;
            }
            else
            {
                r["ParentId"] = dictId[ParentId];
            }
                

            r["Id"] = startId++;
            r["DepartmentId"] = row["部门Id"];
            r["Budget"] = row["预算金额"];
            r["FeeDetail"] = row["费用明细"];
            r["CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            dt.Rows.Add(r);
        }
        sql = SqlHelper.GetInsertString(dt, "import_budget");
        Response.Write(SqlHelper.Exce(sql));
    }

    private void InitDatagrid()
    {
        string sql = string.Format("select * from fee_detail_dict_copy where ParentName is null order by Id");
        DataSet ds = SqlHelper.Find(sql);
        string res = "";
        if(ds!=null&&ds.Tables[0].Rows.Count>0)
        {
            ds.Tables[0].Columns.Add("Budget");
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                row["Budget"] = 0;
            }
            res = JsonHelper.DataTable2Json(ds.Tables[0]);
        }
        Response.Write(res);
    }

    private void getBudget()
    {
        string departmentId = Request.Form["departmentId"];

        int lastMonth, lastYear,nextMonth, nextYear;
        DateTime now = DateTime.Now;
        if(now.Month==1)
        {
            lastMonth = 12;
            lastYear = now.Year - 1;
            nextMonth = now.Month + 1;
            nextYear = now.Year;
        }
        else if (now.Month == 12)
        {
            lastMonth = now.Month - 1;
            lastYear = now.Year;
            nextMonth = 1;
            nextYear = now.Year + 1;
        }
        else 
        {
            lastMonth = now.Month - 1;
            lastYear =now.Year;
            nextMonth = now.Month + 1;
            nextYear = now.Year;
        }

        JObject res = new JObject();
        //本月预算
        string sql = string.Format("SELECT a.* FROM `import_budget` a LEFT JOIN department d on a.DepartmentId=d.Id " +
            "where d.Id={0} and a.CreateTime between '{1}-{2}-1 00:00:00 ' and '{3}-{4}-1 00:00:00' order by a.Id;", departmentId, now.Year, now.Month, nextYear, nextMonth);
        //string sql = string.Format("SELECT a.*, ifnull(sum(yr.fee_amount), 0) UsedAmount FROM `import_budget` a LEFT JOIN department d on a.DepartmentId=d.Id " +
        //    "left join yl_reimburse yr on yr.fee_department like CONCAT('%',d.`name`,'%') and a.FeeDetail = yr.fee_detail and yr.approval_time BETWEEN '{1}-{2}-26 00:00:00 '" +
        //    "AND '{3}-{4}-31 23:59:59' and yr.status = '已审批' where d.Id={0} and " +
        //    "a.CreateTime between '{1}-{2}-1 00:00:00 ' and '{3}-{4}-1 00:00:00' group by a.FeeDetail order by a.Id;", departmentId, lastYear, lastMonth, now.Year, now.Month);
        //上月预算
        sql += string.Format("SELECT a.*,0 as UsedAmount FROM `import_budget` a LEFT JOIN department d on a.DepartmentId=d.Id " +
            "where d.Id={0} and a.CreateTime between '{1}-{2}-1 00:00:00 ' and '{3}-{4}-1 00:00:00' order by a.Id;", departmentId, lastYear, lastMonth, now.Year, now.Month);
        sql += "select max(Id) from import_budget;";
        sql += string.Format("select yr.fee_amount,yr.fee_detail from yl_reimburse yr left join department d on yr.fee_department like CONCAT('%',d.name,'%') " +
            " where d.Id={0} and yr.approval_time BETWEEN '{1}-{2}-1 00:00:00 ' and '{3}-{4}-1 00:00:00' and yr.status = '已审批'"
            , departmentId, now.Year, now.Month, nextYear, nextMonth);
        DataSet ds = SqlHelper.Find(sql);
        if (ds!=null)
        {
            int count =Convert.ToInt32(ds.Tables[2].Rows[0][0])+1;
            DataTable dt = new DataTable();
            res.Add("message", "success");
            if(ds.Tables[0].Rows.Count==0)//本月未提交预算，费用明细从上月取，预算全部赋值0
            {
                if (ds.Tables[1].Rows.Count == 0)
                {
                    ds.Tables[1].Rows.Add(count,departmentId, "费用明细" + count , 0, DateTime.Now, null);
                }
                else
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        row["Budget"] = 0;
                    }
                }
                dt = ds.Tables[1];
            }
            else//本月已提交预算，预算和费用明细从本月取数据
            {
                dt=ds.Tables[0];
                dt.Columns.Add("UsedAmount");
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    DataRow row = dt.Rows[i];
                    string ParentName = "";
                    if (Convert.ToInt32(row["ParentId"]) == -1)
                        ;
                    else
                    {
                        DataRow[] rows = dt.Select("Id=" + row["ParentId"].ToString());
                        if (rows.Length > 0)
                            ParentName = rows[0]["FeeDetail"].ToString();
                    }
                    double UsedAmount = 0;
                    for(int j=ds.Tables[3].Rows.Count-1;j>=0;j--)
                    {
                        DataRow r = ds.Tables[3].Rows[j];
                        string ImFeeDetail = r["fee_detail"].ToString();
                        string YrFeeDetail = row["FeeDetail"].ToString();
                        if (string.IsNullOrEmpty(ParentName))
                        {
                            if (ImFeeDetail.Contains(YrFeeDetail))
                            {
                                try
                                {
                                    UsedAmount += Convert.ToDouble(r["fee_amount"]);
                                }
                                catch { }
                                finally
                                {
                                    //ds.Tables[3].Rows.RemoveAt(j);
                                }
                            }
                        }
                        else
                        {                            
                            if ((ImFeeDetail.Contains(YrFeeDetail)) && ImFeeDetail.Contains(ParentName))
                            {
                                try
                                {
                                    UsedAmount += Convert.ToDouble(r["fee_amount"]);
                                }
                                catch { }
                                finally
                                {
                                    //ds.Tables[3].Rows.RemoveAt(j);
                                }
                            }
                        }
                        
                    }
                    dt.Rows[i]["UsedAmount"] = UsedAmount;
                }
            }
            List<BudgetTreeNode> tree = new BudgetTreeNodeHelper(dt).GetTree();
            string json = JsonConvert.SerializeObject(tree);
            res.Add("data", json);
            res.Add("count", count);//预算提交的Id最大值，作为费用明细Id
        }
        else 
        {
            res.Add("message", "fail");
        }
        Response.Write(res);
    }

    private void InitDepartmentTree()
    {
        string json = "";
        DepartmentTreeHelper departmentTree=new DepartmentTreeHelper();
        json = departmentTree.GetJson();
        
        Response.Write(json);
    }

    private void updateBudget()
    {
        string departmentId = Request.Form["departmentId"];
        string budgetList = Request.Form["budgetList"];
        List<BudgetTreeNode> tree = JsonConvert.DeserializeObject<List<BudgetTreeNode>>(budgetList);

        DataTable dt = new DataTable();
        dt.Columns.Add("Id");
        dt.Columns.Add("DepartmentId");
        dt.Columns.Add("Budget");
        dt.Columns.Add("FeeDetail");
        dt.Columns.Add("ParentId");
        dt.Columns.Add("CreateTime");

        treeToTable(tree,ref dt, departmentId);

        int lastMonth, lastYear, nextMonth, nextYear;
        DateTime now = DateTime.Now;
        if (now.Month == 1)
        {
            lastMonth = 12;
            lastYear = now.Year - 1;
            nextMonth = now.Month + 1;
            nextYear = now.Year;
        }
        else if (now.Month == 12)
        {
            lastMonth = now.Month - 1;
            lastYear = now.Year;
            nextMonth = 1;
            nextYear = now.Year + 1;
        }
        else
        {
            lastMonth = now.Month - 1;
            lastYear = now.Year;
            nextMonth = now.Month + 1;
            nextYear = now.Year;
        }

        string date = now.Year.ToString() + '-' + now.Month.ToString();
        string sql = string.Format("delete from import_budget where DepartmentId='{0}' and CreateTime between " +
            "'{1}-{2}-1 00:00:00 ' and '{3}-{4}-1 00:00:00';", departmentId, now.Year, now.Month, nextYear, nextMonth);
        SqlHelper.Exce(sql);
        sql += "select max(Id) from import_budget";
        try
        {
            int id = Convert.ToInt32(SqlHelper.Scalar(sql))+1;
            if (dt.Rows.Count > 0)
            {
                DataRow[] rows = dt.Select("ParentId = -1");
                for(int j=0;j<rows.Length;j++)
                {
                    var oldId = rows[j]["Id"];
                    rows[j]["Id"] = id++;
                    for(int i=0;i<dt.Rows.Count;i++)
                    {
                        if(dt.Rows[i]["ParentId"].Equals(oldId))
                        {
                            dt.Rows[i]["ParentId"] = rows[j]["Id"];
                            dt.Rows[i]["Id"] = id++;
                        }                        
                    }
                }

                sql = SqlHelper.GetInsertString(dt, "import_budget");
            }
            Response.Write(SqlHelper.Exce(sql));
        }
        catch(Exception ex)
        {
            Response.Write(ex.ToString());
        }
    }

    private void treeToTable(List<BudgetTreeNode> tree,ref DataTable dt,string departmentId)
    {
        foreach(BudgetTreeNode node in tree)
        {
            if (node.children!=null&&node.children.Count > 0)
            {
                treeToTable(node.children, ref dt, departmentId);
            }
            DataRow row = dt.NewRow();
            row["Id"] = node.Id;
            row["FeeDetail"] = node.FeeDetail;
            row["Budget"] = node.Budget;
            row["DepartmentId"] = departmentId;
            row["CreateTime"] = DateTime.Now;
            row["ParentId"] = node.ParentId;
            dt.Rows.Add(row);
        }
    }
}