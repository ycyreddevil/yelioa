using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;

public partial class OtherFinanceReimburseManage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if (action == "initDatagrid")
            {
                Response.Write(initDatagrid());
            }
            else if (action == "insertOrUpdateOtherFee")
            {
                Response.Write(insertOrUpdateOtherFee());
            }
            else if (action == "getTree")
            {
                Response.Write(getTree());
            }
            else if (action == "importWage")
            {
                Response.Write(importWage());
            }
            else if (action == "importFlow")
            {
                Response.Write(importFlow());
            }

            Response.End();
        }
    }

    private string initDatagrid()
    {
        string year = Request.Form["year"];
        string month = Request.Form["month"];

        string sql = string.Format("select t1.*, t3.UserName from yl_reimburse_other t1" +
            " left join users t3 on t1.userId = t3.userId ");

        if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month))
        {
            sql += " where year = '" + year + "' and month = '" + month + "'";
        }
        else if (string.IsNullOrEmpty(year))
        {
            sql += " where year = '" + year + "'"; 
        }
        else if (string.IsNullOrEmpty(month))
        {
            sql += " where month = '" + month + "'";
        }
        else
        {
            sql += " where year = '" + DateTime.Now.Year + "' and month = '" + (DateTime.Now.Month + 1) + "'";
        }

        var dt = SqlHelper.Find(sql).Tables[0];

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string insertOrUpdateOtherFee()
    {
        var id = Request.Form["id"].ToString();
        var year = Request.Form["Year"];
        var month = Request.Form["Month"];
        var feeAmount = Request.Form["FeeAmount"];
        var budget = Request.Form["Budget"];
        var feeName = Request.Form["FeeName"];
        var reportDepartmentName = Request.Form["ReportDepartmentName"];
        var user = (UserInfo)Session["user"];

        // 更新
        string sql = string.Format("update yl_reimburse_other set reportDepartmentName = '{0}', feeName = '{1}', feeAmount = {2}, budget = {7}, year = '{3}'," +
            " month = '{4}', userId = '{5}', createTime = now() where id = {6}", reportDepartmentName, feeName, feeAmount, year, month, user.userId, id, budget);

        // 新增
        if (string.IsNullOrEmpty(id))
        {
            sql = string.Format("insert into yl_reimburse_other (reportDepartmentName, feeName, feeAmount, year, month, userId, createTime)" +
            " values ({0}, '{1}', {2}, {3}, '{4}', '{5}', '{6}', now())", reportDepartmentName, feeName, feeAmount, year, month, user.userId);
        }

        var msg = SqlHelper.Exce(sql);

        var result = new JObject
        {
            { "code", 200 },
            { "msg", "操作成功" }
        };

        if (!msg.Contains("操作成功"))
        {
            result["code"] = 500;
            result["msg"] = "操作失败";
        }

        return result.ToString();
    }

    public string getTree()
    {
        var user = (UserInfo)Session["user"];
        var ds = UserInfoManage.getTree(user.companyId.ToString());
        if (ds == null)
        {
            return "F";
        }
        DepartmentTreeHelper tree = new DepartmentTreeHelper(ds.Tables[0]);
        string json = tree.GetJson();
        return json;
    }

    private string importWage()
    {
        string year = Request.Form["wage-year"];
        string month = Request.Form["wage-month"];

        JObject res = new JObject();

        DataTable dtExl = ExcelHelperV2_0.Import(Request, 1);
        if (dtExl.Rows.Count == 0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "导入数据失败：数据为空！");
            return res.ToString();
        }        

        UserInfo user = (UserInfo)Session["user"];

        DataTable dt = new DataTable();        
        dt.Columns.Add("ReportDepartmentName");
        dt.Columns.Add("FeeName");
        dt.Columns.Add("FeeAmount");
        dt.Columns.Add("Budget");
        dt.Columns.Add("Year");
        dt.Columns.Add("Month");
        dt.Columns.Add("CreateTime");
        dt.Columns.Add("UserId");

        foreach (DataRow iRow in dtExl.Rows)
        {
            DataRow row = dt.NewRow();
            string depart = iRow["部门"].ToString();

            if (depart == "部门")
                continue;

            // 判断部门是否存在
            DataTable tempDt = SqlHelper.Find(string.Format("select 1 from report_department where name = '{0}'", depart)).Tables[0];

            if (tempDt.Rows.Count == 0)
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "'" + depart + "'不存在，请确定该部门是否正确");

                return res.ToString();
            }

            row["ReportDepartmentName"] = depart;
            row["FeeName"] = iRow["费用科目"].ToString();
            row["Year"] = year;
            row["Month"] = month;
            row["CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            row["UserId"] = user.userId;
            row["FeeAmount"] = Convert.ToDouble(iRow["金额"].ToString());
            row["Budget"] = Convert.ToDouble(iRow["预算"].ToString());

            dt.Rows.Add(row);
        }

        //string sql = string.Format("delete from yl_reimburse_other where year = '{0}' " +
        //    " and month='{1}'\r\n;", year, month);
        string sql = SqlHelper.GetInsertString(dt, "yl_reimburse_other");
        SqlExceRes r = new SqlExceRes( SqlHelper.Exce(sql));
        if(r.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", r.ExceMsg);            
        }
        else
        {
            res.Add("ErrCode", 4);
            res.Add("ErrMsg", r.ExceMsg);
        }
        return res.ToString();
    }

    private string importFlow()
    {
        string year = Request.Form["wage-year"];
        string month = Request.Form["wage-month"];

        JObject res = new JObject();

        DataTable dtExl = ExcelHelperV2_0.Import(Request, 1);
        if (dtExl.Rows.Count == 0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "导入数据失败：数据为空！");
            return res.ToString();
        }

        UserInfo user = (UserInfo)Session["user"];

        DataTable dt = new DataTable();
        dt.Columns.Add("ReportDepartmentName");
        dt.Columns.Add("FeeName");
        dt.Columns.Add("FeeAmount");
        dt.Columns.Add("Budget");
        dt.Columns.Add("Year");
        dt.Columns.Add("Month");
        dt.Columns.Add("CreateTime");
        dt.Columns.Add("UserId");

        foreach (DataRow iRow in dtExl.Rows)
        {
            DataRow row = dt.NewRow();
            string depart = iRow["部门"].ToString();

            if (depart == "部门")
                continue;

            // 判断部门是否存在
            DataTable tempDt = SqlHelper.Find(string.Format("select 1 from report_department where name = '{0}'", depart)).Tables[0];

            if (tempDt.Rows.Count == 0)
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "'" + depart + "'不存在，请确定该部门是否正确");

                return res.ToString();
            }

            double flow = double.Parse(iRow["流向"].ToString());
            double netSales = double.Parse(iRow["纯销"].ToString());
            double grossProfit = double.Parse(iRow["毛利"].ToString());

            row["ReportDepartmentName"] = depart;
            row["FeeName"] = "流向";
            row["Year"] = year;
            row["Month"] = month;
            row["CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            row["UserId"] = user.userId;
            row["FeeAmount"] = flow;
            row["Budget"] = 0;

            dt.Rows.Add(row);

            row = dt.NewRow();

            row["ReportDepartmentName"] = depart;
            row["FeeName"] = "纯销";
            row["Year"] = year;
            row["Month"] = month;
            row["CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            row["UserId"] = user.userId;
            row["FeeAmount"] = netSales;
            row["Budget"] = 0;

            dt.Rows.Add(row);

            row = dt.NewRow();

            row["ReportDepartmentName"] = depart;
            row["FeeName"] = "毛利";
            row["Year"] = year;
            row["Month"] = month;
            row["CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            row["UserId"] = user.userId;
            row["FeeAmount"] = grossProfit;
            row["Budget"] = 0;

            dt.Rows.Add(row);
        }

        //string sql = string.Format("delete from yl_reimburse_other where year = '{0}' " +
        //    " and month='{1}'\r\n;", year, month);
        string sql = SqlHelper.GetInsertString(dt, "yl_reimburse_other");
        SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sql));
        if (r.Result == SqlExceRes.ResState.Success)
        {
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", r.ExceMsg);
        }
        else
        {
            res.Add("ErrCode", 4);
            res.Add("ErrMsg", r.ExceMsg);
        }
        return res.ToString();
    }

    //private string GetDepartmentIdForWage(string company, string centre, string depart, string subDepart, string warZone, string region, DataTable dt)
    //{
    //    DataRow[] rows = null;
    //    if (centre == "营销中心")
    //    {
    //        //string condition = string.Format("'%{0}%{1}%{2}%{3}%{4}%'", centre, depart, subDepart, warZone, region);
    //        string condition = "'%" + centre;
    //        if (!string.IsNullOrEmpty(depart))
    //        {
    //            condition += "%" + depart;
    //            if (!string.IsNullOrEmpty(subDepart))
    //            {
    //                condition += "%" + subDepart;
    //                if (!string.IsNullOrEmpty(warZone))
    //                {
    //                    condition += "%" + warZone;
    //                    if (!string.IsNullOrEmpty(region))
    //                    {
    //                        condition += "%" + region;
    //                    }
    //                }
    //            }
    //        }
    //        condition += "'";
    //        condition = string.Format(@"name like {0}", condition);
    //        rows = dt.Select(condition);
    //    }
    //    else if (centre == "制造中心")
    //    {
    //        string condition = "'%" + centre;
    //        if (!string.IsNullOrEmpty(company))
    //        {
    //            condition += "%" + company;
    //        }
    //        condition += "%'";

    //        rows = dt.Select(string.Format(@"name like {0} and name not LIKE '%/%/%/%'", condition));
    //    }
    //    else if (centre == "人力行政中心")
    //    {
    //        string condition = "'%" + centre;
    //        if (!string.IsNullOrEmpty(depart))
    //        {
    //            condition += "%" + depart;
    //        }
    //        condition += "%'";

    //        rows = dt.Select(string.Format(@"name like {0}", condition));
    //    }
    //    else if (centre == "综合支持中心")
    //    {
    //        string condition = "'%" + centre;
    //        if (!string.IsNullOrEmpty(depart))
    //        {
    //            condition += "%" + depart;
    //        }
    //        condition += "%'";

    //        rows = dt.Select(string.Format(@"name like {0}", condition));
    //    }
    //    else if (centre == "总经办" || centre == "顾问/兼职/实习")
    //    {
    //        rows = dt.Select("name = '东森家园'");
    //    }
    //    if (rows == null || rows.Length == 0)
    //        return null;
    //    else
    //        return rows[0]["Id"].ToString();
    //}

    private string GetDepartmentIdForWage(string company, string centre, string depart, string subDepart, string warZone, string region, DataTable dt)
    {
        foreach (DataRow row in dt.Rows)
        {
            string name = row["name"].ToString();
            if (centre == "总经办" || centre == "顾问/兼职/实习")
            {
                return "1";
            }
            else if (centre == "营销中心")
            {
                if (depart == "销售部")
                {
                    if (!string.IsNullOrEmpty(region))
                    {
                        if (name.Contains(region))
                            return row["Id"].ToString();
                    }
                    else
                    {
                        int count = Regex.Matches(name, "/").Count;
                        if (count == 4 && name.Contains(warZone) && name.Contains(subDepart))
                            return row["Id"].ToString();
                    }
                }
                else if (depart == "市场部")
                {
                    if (string.IsNullOrEmpty(subDepart))
                    {
                        return "292";
                    }
                    else
                    {
                        if (name.Contains(subDepart) && name.Contains(depart))
                            return row["Id"].ToString();
                    }
                }
                else if (depart == "销售办公室")
                {
                    if (string.IsNullOrEmpty(subDepart))
                    {
                        return "508";
                    }
                    else if (subDepart == "计划组")
                    {
                        return "512";
                    }
                    else if (subDepart == "企划组")
                    {
                        return "490";
                    }
                }
            }
            else if (centre == "制造中心")
            {
                int count = Regex.Matches(name, "/").Count;
                if(company == "中申" || company == "老康")
                {
                    if (count == 3 && name.Contains(company))
                        return row["Id"].ToString();
                }
                else
                {
                    if (count == 2 && name.Contains(company))
                        return row["Id"].ToString();
                }               
            }
            else if (centre == "人力行政中心")
            {
                int count = Regex.Matches(name, "/").Count;
                if (count == 2 && name.Contains(depart))
                    return row["Id"].ToString();
            }
            else if (centre == "综合支持中心")
            {
                int count = Regex.Matches(name, "/").Count;
                if (count == 2 && name.Contains(depart))
                    return row["Id"].ToString();
            }
        }

        return null;
    }
}