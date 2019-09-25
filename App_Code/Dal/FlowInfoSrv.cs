using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data;

/// <summary>
/// FlowInfoSrv 的摘要说明
/// </summary>
public class FlowInfoSrv
{
    public FlowInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetInfo(DateTime date)
    {
        //  查询出库数据和cost_sharing网点数据，并计算出流向数据
        //string view = string.Format("select c.*, o.name as Hospital, p.name as Product,u.userName as Sales,"
        //    + "us.userName as Supervisor, um.userName as Manager, ud.userName as Director"
        //    + " from cost_sharing as c inner join organization as o on c.HospitalId = o.Id "
        //    + "inner join products as p on c.ProductId = p.Id "
        //    + "inner join users as u on c.SalesId = u.userId "
        //    + "inner join users as us on c.SupervisorId = us.userId "
        //    + "inner join users as um on c.ManagerId = um.userId "
        //    + "inner join users as ud on c.DirectorId = ud.userId "
        //    //+ "inner join department as dd on c.DepartmentId = dd.Id "
        //    //+ "inner join department as ds on c.SectorId = ds.Id"
        //    + " where c.SalesId= (select Min(SalesId) from cost_sharing"
        //    + " where c.ProductId = cost_sharing.ProductId and "
        //    + "c.HospitalId = cost_sharing.HospitalId)");

        //string sql = string.Format("select c.Product, c.Hospital, ifnull(l.sum,0) as sum,c.* from ({0}) c right join"
        //    + " (select leave_stock.*, SUM(leave_stock.amountSend) as sum"
        //    + " from leave_stock where date_format(leave_stock.date,'%Y-%m')=date_format('{1}','%Y-%m')"
        //    + " group by productCode, terminalClientCode) l"
        //    + " on(c.ProductId = l.ProductId and c.HospitalId = l.terminalClientId)"
        //    , view,date.ToString("yyyy-MM-dd"));
        string sql = "select * from v_outlet";
        List<string> list = new List<string>();
        list.Add(sql);
        
        //查询flow_statistics表数据
        sql = string.Format("select * from flow_statistics where Year={0} and Month={1}"
            ,date.Year,date.Month);
        list.Add(sql);
        //查询flow_statistics上月所有网点当月库存
        DateTime dateLastMonth = date.AddMonths(-1);
        sql = string.Format("select * from flow_statistics where Year={0} and Month={1}", dateLastMonth.Year, dateLastMonth.Month);
        list.Add(sql);
        //查询出库数据
        sql = string.Format("select * from leave_stock where date_format(leave_stock.date,'%Y-%m')=date_format('{0}','%Y-%m')"
            , date.ToString("yyyy-MM-dd"));
        list.Add(sql);
        return SqlHelper.Find(list.ToArray());
    }

    public static DataSet GetMobileInfo(int year, int month)
    {
        UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
        if (user == null)
            return null;
        PostHelper postHelper = new PostHelper(user);
        if(postHelper.dpList == null)
            return null;
        //集团本部的运营数据管理员有权限查看所有流向数据
        string sql = "";
        if (Privilege.checkPrivilege(user))
        {
            sql = string.Format("select * from flow_statistics where Year={0} and Month={1} "
            , year, month);
        }
        else
        {
            sql = string.Format("select * from flow_statistics where Year={0} and Month={1} and "
            + "(Sales='{2}' or Supervisor='{3}' or Manager='{4}' or Director='{5}')"
            , year, month,user.userName, user.userName, user.userName, user.userName);
        }
        
        return SqlHelper.Find(sql);
    }

    public static DataSet GetMobileSimilarInfo(string productCode, string hospitalCode, string sales, int year, int month)
    {
        string sql = string.Format("select * from flow_statistics where Year={0} and Month={1} and " +
            "ProductCode = {2} and HospitalCode = {3} and Sales != '{4}'", year, month, productCode, hospitalCode, sales);
        return SqlHelper.Find(sql);
    }

    public static DataSet GetMobileDetail(string id)
    {
        string sql = string.Format("select * from flow_statistics where id = {0}", id);
        return SqlHelper.Find(sql);
    }

    public static string ArchiveData(DateTime date, DataTable data)
    {
        List<string> list = new List<string>();
        string sql = string.Format("delete from flow_statistics where Year={0} and Month={1}", date.Year, date.Month);
        list.Add(sql);
        sql = SqlHelper.GetInsertString(data, "flow_statistics");
        list.Add(sql);
        //foreach(DataRow row in data.Rows)
        //{
        //    sql = SqlHelper.GetInsertIgnoreString(row, "flow_statistics");
        //    list.Add(sql);
        //}
        //List<Dictionary<string, string>> dataList = (List<Dictionary<string, string>>)data;
        //foreach(Dictionary<string, string> dict in dataList)
        //{
        //    sql = SqlHelper.GetInsertIgnoreString(dict, "flow_statistics");
        //    list.Add(sql);
        //}
        return SqlHelper.Exce(list.ToArray());
    }

    public static string ImportInfos(Dictionary<string, string> dict)
    {
        string res = "";
        if (dict.Count == 0)
        {
            return res;
        }
        //string[] field = new string[] { dict["HospitalId"], dict["ProductId"]+','+dict["Specification"], dict["SalesId"]
        //    ,dict["SupervisorId"], dict["ManagerId"], dict["DirectorId"], };
        //string[] tables = new string[] { "organization", "products", "users", "users", "users", "users" };
        //string[] strs = GetId(field, tables);
        //string HospitalId = strs[0];
        //if (string.IsNullOrEmpty(HospitalId))
        //{
        //    return "医院未找到！";
        //}
        //string ProductId = strs[1];
        //if (string.IsNullOrEmpty(ProductId))
        //{
        //    return "产品未找到！";
        //}
        //string SalesId = strs[2];
        //if (string.IsNullOrEmpty(SalesId) || string.IsNullOrEmpty(strs[3]) || string.IsNullOrEmpty(strs[4]) || string.IsNullOrEmpty(strs[5]))
        //{
        //    return "人员未找到！";
        //}
        ////if (string.IsNullOrEmpty(strs[3]) || string.IsNullOrEmpty(strs[4]))
        ////{
        ////    return "部门未找到！";
        ////}
        //dict = Common.ChangeDictionaryValue(dict, "HospitalId", strs[0]);
        //dict = Common.ChangeDictionaryValue(dict, "ProductId", strs[1]);
        //dict = Common.ChangeDictionaryValue(dict, "SalesId", strs[2]);
        ////dict = Common.ChangeDictionaryValue(dict, "DepartmentId", strs[3]);
        ////dict = Common.ChangeDictionaryValue(dict, "SectorId", strs[4]);
        //dict = Common.ChangeDictionaryValue(dict, "SupervisorId", strs[3]);
        //dict = Common.ChangeDictionaryValue(dict, "ManagerId", strs[4]);
        //dict = Common.ChangeDictionaryValue(dict, "DirectorId", strs[5]);
        //string sql = string.Format("select Id from flow_statistics where Hospital='{0}' and Product='{1}' and Sales='{2}' "+
        //    "and Year={3} and Month={4}", dict["Hospital"], dict["Product"], dict["Sales"], dict["Year"], dict["Month"]);
        //object id = SqlHelper.Scalar(sql);
        //if (id != null)//有重复
        //{
        //    res = ImportUpdateInfos(dict, id.ToString());
        //}
        //else
        {
            res = ImportInsertInfos(dict);
        }
        return res;
    }

    public static string GetId(string field, string table)
    {
        string sql = "";
        if (table == "users")
        {
            sql = string.Format("select userId from users where userName = '{0}'", field);
        }
        else
        {
            sql = string.Format("select Id from {0} where name = '{1}'", table, field);
        }
        object res = SqlHelper.Scalar(sql);
        if (res == null)
            return null;
        else
            return res.ToString();
    }

    public static string[] GetId(string[] field, string[] table)
    {
        if (field.Length != table.Length)
            return null;
        if (field.Length == 0 || table.Length == 0)
            return null;

        int len = field.Length;
        string[] sql = new string[len];
        for (int i = 0; i < len; i++)
        {
            sql[i] = "";
            if (table[i] == "users")
            {
                sql[i] = string.Format("select userId from users where userName = '{0}'", field[i]);
            }
            else if (table[i] == "products")
            {
                string[] vals = field[i].Split(',');
                if (string.IsNullOrEmpty(vals[1]))
                    sql[i] = string.Format("select Id from products where name = '{0}'", vals[0]);
                else
                    sql[i] = string.Format("select Id from products where name = '{0}' and specification='{1}'"
                        , vals[0], vals[1]);
            }
            else
            {
                sql[i] = string.Format("select Id from {0} where name = '{1}'", table[i], field[i]);
            }
        }
        return SqlHelper.Scalar(sql);
    }

    public static string ImportUpdateInfos(Dictionary<string, string> dict, string id)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        //string[] baseInfo = { "HospitalId", "ProductId", "SalesId", "SupervisorId", "ManagerId", "DirectorId", "Department"
        //        ,"Sector","Specification" };
        string sql = string.Format("Update flow_statistics set ");
        foreach (string key in dict.Keys)
        {
            //不包含中文列名及excel缺省列名ColumnXXX
            if (!StringTools.HasChinese(key) && !key.Contains("Column"))
            {
                string value = "";
                if (key.Contains("Department") || key.Contains("Sector") ||
                    key.Contains("Supervisor") || key.Contains("Specification") ||
                    key.Contains("Manager") || key.Contains("Director") ||
                    key.Contains("Hospital") ||
                    key.Contains("Product") || key.Contains("Sales"))
                {
                    value = dict[key];
                }
                else
                {
                    double val = 0.0;
                    try
                    {
                        val = Convert.ToDouble(dict[key]);
                    }
                    catch
                    {
                        val = 0.0;
                    }
                    if (key.Contains("Ratio"))
                    {
                        value = (val * 100).ToString();
                    }
                    else
                    {
                        value = val.ToString();
                    }
                }
               
                    
                if (!string.IsNullOrEmpty(value))
                {
                    sql += string.Format("{0}='{1}', ", (key), value);
                }
                else
                {
                    //return "人员未找到！";
                }
            }

        }
        sql = sql.Substring(0, sql.Length - 2);
        //sql += string.Format("HospitalId = {0},ProductId={1},SalesId={2}", HospitalId, ProductId, SalesId);
        sql += string.Format(" where Id={0}", id);
        return SqlHelper.Exce(sql);
    }

    public static string ImportInsertInfos(Dictionary<string, string> dict)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string fileds = "";
        string values = "";
        foreach (string key in dict.Keys)
        {
            //不包含中文列名及excel缺省列名ColumnXXX
            if (!StringTools.HasChinese(key) && !key.Contains("Column"))
            {
                string value = "";
                if (key.Contains("Department") || key.Contains("Sector") ||
                    key.Contains("Supervisor") || key.Contains("Specification") ||
                    key.Contains("Manager") || key.Contains("Director") ||
                    key.Contains("Hospital") ||
                    key.Contains("Product") || key.Contains("Sales"))
                {
                    value = dict[key];
                }
                else
                {
                    double val = 0.0;
                    try
                    {
                        val = Convert.ToDouble(dict[key]);
                    }
                    catch
                    {
                        val = 0.0;
                    }
                    if (key.Contains("Ratio"))
                    {
                        value = (val * 100).ToString();
                    }
                    else
                    {
                        value = val.ToString();
                    }
                }
                if (!string.IsNullOrEmpty(value))
                {
                    fileds += string.Format("{0},", (key));
                    values += string.Format("'{0}',", value);
                }
                else
                {
                    //return "人员未找到！";
                }
            }
        }
        if (!string.IsNullOrEmpty(fileds))
        {
            fileds = fileds.Substring(0, fileds.Length - 1);
            values = values.Substring(0, values.Length - 1);
            //fileds += string.Format("HospitalId, ProductId, SalesId");
            //values += string.Format("{0},{1},{2}", HospitalId, ProductId, SalesId);
        }
        else
        {
            return "";
        }

        string sql = string.Format("Insert into {0} ({1}) values ({2}) ", "flow_statistics", fileds, values);
        return SqlHelper.Exce(sql);
    }
}