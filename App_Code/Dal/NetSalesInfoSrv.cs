using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// NetSalesInfoSrv 的摘要说明
/// </summary>
public class NetSalesInfoSrv
{
    public NetSalesInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet getInfos(string salesId)
    {
        string sql = string.Format("select c.*, o.name as Hospital, p.name as Product,u.userName as Sales,"
            + "us.userName as Supervisor, um.userName as Manager, ud.userName as Director, "
            + "ns.State as State, ns.DocCode as DocCode, ns.CorrespondingTime as CorrespondingTime "
            + "from cost_sharing as c inner join organization as o on c.HospitalId = o.Id "
            + "inner join products as p on c.ProductId = p.Id "
            + "inner join users as u on c.SalesId = u.userId "
            + "inner join users as us on c.SupervisorId = us.userId "
            + "inner join users as um on c.ManagerId = um.userId "
            + "inner join users as ud on c.DirectorId = ud.userId "
            //+ "inner join department as dd on c.DepartmentId = dd.Id "
            //+ "inner join department as ds on c.SectorId = ds.Id "
            + "left join net_sales as ns on c.HospitalId = ns.HospitalId and c.ProductId = ns.ProductId and c.SalesId = ns.SalesId "
            + "where c.SalesId={0}",salesId);
        return SqlHelper.Find(sql);
    }

    public static string getFlowNumOfReportSales(string hospital, string product,string sales)
    {
        //UserInfo user = (UserInfo)HttpContext.Current.Session["user"];
        string res = "";
        DateTime lastMonth = DateTime.Now.AddMonths(-1);
        string sql = string.Format("select * from v_net_sales where Hospital='{0}' and "
            + "Product='{1}' and Sales='{2}' and "
            + "date_format(CorrespondingTime,'%Y-%m')=date_format('{3}','%Y-%m')"
            ,hospital,product,sales, lastMonth.ToString("yyyy-MM-dd"));
        DataSet ds = SqlHelper.Find(sql, ref res);
        if (ds == null)
        {
            //数据错误，返回错误信息
            return res;
        }
        else if(ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["State"].ToString() != "未提交")
        {
            return "上月纯销已提交，请勿重复提交！";
        }

        //查询flow_statistics表中上个月的当月库存数量
        //DateTime date = DateTime.Now.AddMonths(-1);
        sql = string.Format("select * from flow_statistics where Year={0} and Month={1} "
            + " and Product = '{2}' and Hospital='{3}'", lastMonth.Year, lastMonth.Month, product,hospital);
        List<string> list = new List<string>();
        list.Add(sql);
        //查询上月出库，重新统计流向数据
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

        //sql = string.Format("select c.Product, c.Hospital, ifnull(l.sum,0) as sum,c.* from ({0}) c left join"
        //    + " (select leave_stock.*, SUM(leave_stock.amountSend) as sum"
        //    + " from leave_stock where date_format(leave_stock.date,'%Y-%m')=date_format('{1}','%Y-%m')"
        //    + " group by productCode, terminalClientCode) l"
        //    + " on(c.ProductId = l.ProductId and c.HospitalId = l.terminalClientId)"
        //    + " where Product = '{2}' and Hospital='{3}'"
        //    , view, lastMonth.ToString("yyyy-MM-dd"), product, hospital);
        //list.Add(sql);

        ////查询已经审核入库的纯销
        //sql = string.Format("select * from v_net_sales where Product = '{0}' and Hospital='{1}'"
        //    + " and State='已审核入库'", product, hospital);
        //list.Add(sql);

        
        ds = SqlHelper.Find(list.ToArray(),ref res);
        if (ds == null)
        {
            //数据错误，返回错误信息
            res = "网络出错，请及时联系管理员！";
        }
        //else if (ds.Tables[1].Rows.Count == 0)
        //    res = "未找到相关网点信息！";
        else if (ds.Tables[0].Rows.Count > 0)//flow_statistics表中上个月数据已保存
        {
            res = ds.Tables[0].Rows[0]["StockThisMonth"].ToString();
        }
        else//flow_statistics表中上个月数据未保存，需要临时统计
        {
            //int flowSalse = 0;
            //foreach(DataRow row in ds.Tables[1].Rows)//累加所有相同医院相同产品网点流向数据和上月库存
            //{
            //    flowSalse += Convert.ToInt32(row["sum"]);
            //}
            //int netSalesStocked = 0;
            //foreach (DataRow row in ds.Tables[2].Rows)//累加所有相同医院相同产品网点流向数据和上月库存
            //{
            //    netSalesStocked += Convert.ToInt32(row["NetSales"]);
            //}
            //res = (flowSalse - netSalesStocked).ToString();
            res = "流向数据未归档，请及时联系管理员";
        }
        return res;
    }

    public static Dictionary<String, String> SaveNetSales(string hospital, string product, string sales, string netSalesNumber, string docCode, string time)
    {
        List<string> list = new List<string>();
        string sql = string.Format("select Id from organization where name = '{0}'", hospital);
        list.Add(sql);
        sql = string.Format("select Id from products where name = '{0}'", product);
        list.Add(sql);
        sql = string.Format("select userId from users where userName = '{0}'", sales);
        list.Add(sql);
        string[] Ids = SqlHelper.Scalar(list.ToArray());
        Dictionary<string, string> dict = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(Ids[0]))
        {
            dict.Add("returnMsg", "未找到医院信息");
            return dict;
        }
        else if (string.IsNullOrEmpty(Ids[1]))
        {
            dict.Add("returnMsg", "未找到产品信息");
            return dict;
        }
        else if (string.IsNullOrEmpty(Ids[2]))
        {
            dict.Add("returnMsg", "未找到业务员信息");
            return dict;
        }

        list.Clear();
        sql = string.Format("select ManagerId from cost_sharing where HospitalId = {0} "
            + "and ProductId = {1} and SalesId={2}", Ids[0], Ids[1], Ids[2]);
        String ManagerId = "";
        object obj = SqlHelper.Scalar(sql);
        if (obj != null)
        {
            ManagerId = obj.ToString();
        }
        else
        {
            dict.Add("returnMsg", "未找到网点销售经理信息");
            return dict;
        }

        SqlExceRes res = null;
        
        // 判断是否之前有相同的单据，有则更新，没有则新增
        if (docCode == null || "".Equals(docCode))
        {
            docCode = GenerateDocCode.getDocCode();
            dict.Add("HospitalId", Ids[0]);
            dict.Add("ProductId", Ids[1]);
            dict.Add("SalesId", Ids[2]);
            dict.Add("NetSalesNumber", netSalesNumber.ToString());
            //dict.Add("ApproverId", ManagerId);
            //DateTime date = DateTime.Now.AddMonths(-1);
            dict.Add("CorrespondingTime", time);
            dict.Add("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            dict.Add("State", "未提交");
            dict.Add("Editable", "1");
            dict.Add("DocCode", docCode);

            String insertSql = SqlHelper.GetInsertIgnoreString(dict, "net_sales");
            res = new SqlExceRes(SqlHelper.Exce(insertSql));
        }
        else
        {
            // 根据docCode来更新纯销数量
            String updateSql = String.Format("update net_sales set NetSalesNumber = {0}, CorrespondingTime = '{1}' where DocCode = {2}", netSalesNumber.ToString(), time, docCode);
            res = new SqlExceRes(SqlHelper.Exce(updateSql));
        }

        dict.Clear();
        dict.Add("docCode", docCode);
        dict.Add("returnMsg", res.GetResultString("success", "单据已保存，请勿重复保存", "ErrorMsg:"));
        return dict;
    }

    public static String getNetSalesNum(string hospital, string product, string sales)
    {
        List<string> list = new List<string>();
        string sql = string.Format("select Id from organization where name = '{0}'", hospital);
        list.Add(sql);
        sql = string.Format("select Id from products where name = '{0}'", product);
        list.Add(sql);
        sql = string.Format("select userId from users where userName = '{0}'", sales);
        list.Add(sql);
        string[] Ids = SqlHelper.Scalar(list.ToArray());
        if (string.IsNullOrEmpty(Ids[0]))
        {
            return "未找到医院信息";
        }
        else if (string.IsNullOrEmpty(Ids[1]))
        {
            return "未找到产品信息";
        }
        else if (string.IsNullOrEmpty(Ids[2]))
        {
            return "未找到网点业务员信息";
        }
        sql = String.Format("select NetSalesNumber from net_sales where HospitalId = {0} and ProductId = {1} and SalesId = {2}",
            Ids[0], Ids[1], Ids[2]);
        DataSet ds = SqlHelper.Find(sql);
        String netSalesNum = "";
        if (ds.Tables[0].Rows.Count > 0)
        {
            netSalesNum = ds.Tables[0].Rows[0][0].ToString();
        }
        else
        {
            netSalesNum = "0";
        }
        
        return netSalesNum;
    }

    public static String updateNetSalesAndStockAfterApproval(String docCode)
    {
        String sql = String.Format("select Hospital, Product, Sales, NetSalesNumber, CorrespondingTime from " +
            "v_net_sales where docCode = '{0}'", docCode);

        DataSet ds = SqlHelper.Find(sql);

        if (ds == null)
            return null;

        string netSalesNum = ds.Tables[0].Rows[0]["NetSalesNumber"].ToString();
        string hospital = ds.Tables[0].Rows[0]["Hospital"].ToString();
        string product = ds.Tables[0].Rows[0]["Product"].ToString();
        string sales = ds.Tables[0].Rows[0]["Sales"].ToString();
        string correspondingTime = ds.Tables[0].Rows[0]["CorrespondingTime"].ToString();

        string[] time = correspondingTime.Split(new char[1] {'-'});
        int year = Int32.Parse(time[0]);
        String monthStr = time[1];

        if (monthStr.Length == 1)
            monthStr = monthStr.Substring(1,1);

        int month = Int32.Parse(monthStr)-1;

        string updateNetSaleSql = string.Format("update flow_statistics set NetSales = {0} where Hospital = '{1}' and Product = '{2}' and sales = '{3}'" +
            " and Year = {4} and Month = {5}", netSalesNum, hospital, product, sales, year, month);

        SqlExceRes res1 = new SqlExceRes(SqlHelper.Exce(updateNetSaleSql));
        string netSalesRes = res1.GetResultString("更新纯销成功", "更新纯销失败", "更新纯销失败");

        string queryStockSql = string.Format("select * from flow_statistics where Hospital = '{0}' and Product = '{1}' and sales = '{2}'" +
            " and Year = {3} and Month = {4}", hospital, product, sales, year, month);

        ds = SqlHelper.Find(queryStockSql);

        if (ds == null)
            return null;

        int stockLastMonth = Int32.Parse(ds.Tables[0].Rows[0]["StockLastMonth"].ToString());
        int flowSales = Int32.Parse(ds.Tables[0].Rows[0]["FlowSales"].ToString());
        int NetSales = Int32.Parse(ds.Tables[0].Rows[0]["NetSales"].ToString());
        int stockThisMonth = stockLastMonth + flowSales - NetSales;
        string updateStockSql = string.Format("update flow_statistics set StockThisMonth = {0} where Hospital = '{1}' and Product = '{2}' and sales = '{3}'" +
            " and Year = {4} and Month = {5}", stockThisMonth, hospital, product, sales, year, month);
        //List<string> list = new List<string>();
        //list.Add(updateNetSaleSql);
        //list.Add(updateStockSql);
        SqlExceRes res2 = new SqlExceRes(SqlHelper.Exce(updateStockSql));
        string stockRes = res1.GetResultString("更新库存成功", "更新库存失败", "更新库存失败");

        if ("更新纯销成功".Equals(netSalesRes) && "更新库存成功".Equals(stockRes))
        {
            return "更新成功";
        }
        else
        {
            return "更新失败";
        }
    }
}