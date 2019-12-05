using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// CostSharingInfoSrv 的摘要说明
/// </summary>
public class CostSharingInfoSrv
{
    public CostSharingInfoSrv()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static string GetId(string field,string table)
    {
        string sql = "";
        if (table == "users")
        {
            sql = string.Format("select userId from users where userName = '{0}'", field);
        }
        else
        {
            sql = string.Format("select Id from {0} where name = '{1}'", table,field);
        }
        object res = SqlHelper.Scalar(sql);
        if (res == null)
            return null;
        else
            return res.ToString();
    }

    public static string [] GetId(string[] field,string [] table)
    {
        if (field.Length != table.Length)
            return null;
        if (field.Length == 0 || table.Length == 0)
            return null;
        
        int len = field.Length;
        string[] sql = new string[len+1];
        for(int i=0;i<len;i++)
        {
            sql[i] = "";
            if (table[i] == "users")
            {
                sql[i] = string.Format("select userId from users where userName = '{0}'", field[i]);
            }
            //else if(table[i] == "products")
            //{
            //    string[] vals = field[i].Split(',');
            //    if(vals.Length <2 || string.IsNullOrEmpty(vals[1]))
            //        sql[i] = string.Format("select Id from products where name = '{0}' and specification='无'", vals[0]);
            //    else
            //        sql[i] = string.Format("select Id from products where name = '{0}' and specification='{1}'"
            //            , vals[0], vals[1]);
            //}
            else if (table[i] == "products" || table[i] == "organization")
            {
                sql[i] = string.Format("select Id from {0} where code = '{1}'", table[i], field[i]);
            }
            else
            {
                sql[i] = string.Format("select Id from {0} where name = '{1}'", table[i], field[i]);
            }
        }
        sql[len] = string.Format("select specification from products where code = '{0}'",field[1]);
        return SqlHelper.Scalar(sql);
    }

    public static DataSet getInfos()
    {
        //string sql = string.Format("select c.*, o.name as Hospital, p.name as Product,u.userName as Sales,"
        //    + "us.userName as Supervisor, um.userName as Manager, ud.userName as Director, "
        //    + "case when ns.DocCode is null then '未上报' else '已上报' end as Status, ns_other.DocCode as docCode "
        //    + "from cost_sharing as c inner join organization as o on c.HospitalId = o.Id "
        //    + "inner join products as p on c.ProductId = p.Id "
        //    + "inner join users as u on c.SalesId = u.userId "
        //    + "inner join users as us on c.SupervisorId = us.userId "
        //    + "inner join users as um on c.ManagerId = um.userId "
        //    + "inner join users as ud on c.DirectorId = ud.userId "
        //    //+ "inner join department as dd on c.DepartmentId = dd.Id "
        //    //+ "inner join department as ds on c.SectorId = ds.Id "
        //    + "left join net_sales as ns on ns.Editable <> 1 and c.HospitalId = ns.HospitalId and c.ProductId = ns.ProductId and c.SalesId = ns.SalesId "
        //    + "left join net_sales as ns_other on c.HospitalId = ns_other.HospitalId and c.ProductId = ns_other.ProductId and c.SalesId = ns_other.SalesId");

        string sql = "select * from v_outlet";
        return SqlHelper.Find(sql);
    }

    public static DataSet getInfos(string sort, string order)
    {
        string sql = string.Format("select c.*, o.name as Hospital, p.name as Product,u.userName as Sales,"
            + "us.userName as Supervisor, um.userName as Manager, ud.userName as Director, c.Department as Department, c.Sector as Sector"
            + " from cost_sharing as c inner join organization as o on c.HospitalId = o.Id "
            + "inner join products as p on c.ProductId = p.Id "
            + "inner join users as u on c.SalesId = u.userId "
            + "inner join users as us on c.SupervisorId = us.userId "
            + "inner join users as um on c.ManagerId = um.userId "
            + "inner join users as ud on c.DirectorId = ud.userId ");
            //+ "inner join department as dd on c.DepartmentId = dd.Id "
            //+ "inner join department as ds on c.SectorId = ds.Id");
        if(!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(order))
        {
            sql += string.Format(" order by {0} {1}", sort, order);
        }
        return SqlHelper.Find(sql);
    }

    /// <summary>
    /// 去除名字当中的“（兼）”，例如name=王化勤（兼）
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string CheckUserName(string name)
    {
        if (name.Contains("兼") && name.Length > 3)
            name = name.Substring(0, name.Length - 3);

        return name;
    }

    public static string NewImportInfos(Dictionary<string, string> dict, string index)
    {
        string res = "";
        if (dict.Count == 0)
        {
            return res;
        }
        string sql = "";
        //上传第一行数据时，清空原表
        if(index.Equals("0"))
        {
            sql = "delete from new_client_product_users";
            SqlHelper.Exce(sql);
        }
        List<string> listSql = new List<string>();
        listSql.Add( string.Format("select * from new_client where ClientCode='{0}'", dict["网点代码"]));
        listSql.Add(string.Format("select * from new_product where ProductCode='{0}'", dict["产品代码"]));
        string users = "'" + CheckUserName(dict["渠道代表"]) + "',";
        users += "'" + CheckUserName(dict["省级负责人"]) + "',";
        users += "'" + CheckUserName(dict["地区级负责人"]) + "',";
        users += "'" + CheckUserName(dict["大区级负责人"]) + "',";
        users += "'" + CheckUserName(dict["渠道经理"]) + "',";
        users += "'" + CheckUserName(dict["渠道负责人"]) + "',";
        users += "'" + CheckUserName(dict["代表"]) + "',";
        users += "'" + CheckUserName(dict["主管"]) + "',";
        users += "'" + CheckUserName(dict["区域经理"]) + "',";
        users += "'" + CheckUserName(dict["大区经理"]) + "',";
        users += "'" + CheckUserName(dict["销售负责人"]) + "'";
        listSql.Add(string.Format("select userId from users where userName in ({0})", users));
        listSql.Add(string.Format("select * from new_client_product_users where ProductCode='{0}' and ClientCode='{0}'"
            , dict["产品代码"], dict["网点代码"]));
        listSql.Add(string.Format("select * from new_agent where code='{0}'", dict["代理商编码"]));
        DataSet ds = SqlHelper.Find(listSql.ToArray());
        if(ds == null)
        {
            return "查询字段出错！";
        }
        //网点未找到，需要插入网点数据
        if (ds.Tables[0].Rows.Count == 0)
        {
            sql = string.Format("insert into new_client (ClientCode,ClientName) values ('{0}','{1}')"
                , dict["网点代码"], dict["医院"]);
            SqlHelper.Exce(sql);
        }
        else
        {
            if(!ds.Tables[0].Rows[0]["ClientName"].Equals(dict["医院"]))
            {
                sql = string.Format("update new_client set ClientName='{0}' where ClientCode='{1}'"
                , dict["医院"], dict["网点代码"]);
                SqlHelper.Exce(sql);
            }            
        }

        //产品未找到，需要插入产品数据
        if (ds.Tables[1].Rows.Count == 0)
        {
            sql = string.Format("insert into new_product (ProductCode,ProductName,Specification,Unit) values "
                +"('{0}','{1}','{2}','{3}')"
                , dict["产品代码"], dict["产品"], dict["规格型号"], dict["单位"]);
            SqlHelper.Exce(sql);
        }
        else
        {
            if (!ds.Tables[1].Rows[0]["ProductName"].Equals(dict["产品"]) || !ds.Tables[1].Rows[0]["Specification"].Equals(dict["规格型号"]))
            {
                sql = string.Format("update new_product set ProductName='{0}',Specification='{1}',Unit='{2}' where ProductCode='{3}'"
                ,  dict["产品"], dict["规格型号"], dict["单位"], dict["产品代码"]);
                SqlHelper.Exce(sql);
            }
        }

        //代理商未找到，需要插入代理商数据
        if (ds.Tables[4].Rows.Count == 0)
        {
            sql = string.Format("insert into new_agent (code,name) values ('{0}','{1}')", dict["代理商编码"], dict["代理商名称"]);
            SqlHelper.Exce(sql);
        }
        else
        {
            if (!ds.Tables[4].Rows[0]["Name"].Equals(dict["代理商名称"]))
            {
                sql = string.Format("update new_agent set name='{0}' where code='{1}'"
                , dict["代理商名称"], dict["代理商编码"]);
                SqlHelper.Exce(sql);
            }
        }

        if (ds.Tables[2].Rows.Count == 0)
        {
            return "人员未找到！";
        }

        listSql.Clear();

        sql = string.Format("select id from department where name like '%{0}%' and name like '%{1}%' and name like '%{2}%'", dict["区域（新）"], dict["操作模式"], dict["省份（新）"]);
        DataTable dt = SqlHelper.Find(sql).Tables[0];

        if (dt.Rows.Count == 0) {
            sql = string.Format("select id from department where name like '%{0}%' and name like '%{1}%'", dict["区域（新）"], dict["操作模式"]);
            dt = SqlHelper.Find(sql).Tables[0];
        }

        foreach (DataRow row in ds.Tables[2].Rows)
        {
            DataRow[] rows = ds.Tables[3].Select(string.Format("UserId = {0}", row["userId"].ToString()));
            if (rows.Length > 0)
                continue;
            sql = string.Format("insert into new_client_product_users (ProductCode,ClientCode,UserId,AssessmentPrice,DepartmentId,AgentCode) values ('{0}','{1}','{2}','{3}', '{4}', '{5}')"
                , dict["产品代码"], dict["网点代码"],row["userId"].ToString(), dict["考核价"], dt.Rows[0][0].ToString(), dict["代理商编码"]);
            listSql.Add(sql);
        }
        return SqlHelper.Exce(listSql.ToArray());
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
        string[] field = new string[] { dict["HospitalCode"], dict["ProductCode"], dict["SalesId"]
            ,dict["SupervisorId"], dict["ManagerId"], dict["DirectorId"], };
        string [] tables = new string[] { "organization", "products", "users", "users", "users", "users"};
        string[] strs = GetId(field, tables);
        string HospitalId = strs[0];
        if (string.IsNullOrEmpty(HospitalId))
        {
            return "医院未找到！";
        }
        string ProductId = strs[1];
        if ( string.IsNullOrEmpty(ProductId) )
        {
            return "产品未找到！";
        }
        string SalesId = strs[2];
        if( string.IsNullOrEmpty(SalesId)|| string.IsNullOrEmpty(strs[3]) 
            || string.IsNullOrEmpty(strs[4]) || string.IsNullOrEmpty(strs[5]))
        {
            return "人员未找到！";
        }
        dict = Common.ChangeDictionaryValue(dict, "HospitalId", strs[0]);
        dict = Common.ChangeDictionaryValue(dict, "ProductId", strs[1]);
        dict = Common.ChangeDictionaryValue(dict, "SalesId", strs[2]);
        //dict = Common.ChangeDictionaryValue(dict, "DepartmentId", strs[3]);
        //dict = Common.ChangeDictionaryValue(dict, "SectorId", strs[4]);
        dict = Common.ChangeDictionaryValue(dict, "SupervisorId", strs[3]);
        dict = Common.ChangeDictionaryValue(dict, "ManagerId", strs[4]);
        dict = Common.ChangeDictionaryValue(dict, "DirectorId", strs[5]);
        dict = Common.ChangeDictionaryValue(dict, "Specification", strs[6]);
        string sql = string.Format("select * from cost_sharing where HospitalId={0} and ProductId={1} and SalesId={2} "
            + "and ManagerId={3} and HospitalSupplyPrice={4} and HospitalDepartment='{5}'"
            , HospitalId, ProductId, SalesId, dict["ManagerId"], dict["HospitalSupplyPrice"],dict["HospitalDepartment"]);
        DataSet ds = SqlHelper.Find(sql);
        if (ds == null)
            res = "查询字段出错！";
        else
        {
            Dictionary<string, string> dictData = new Dictionary<string, string>();
            foreach(string key in dict.Keys)
            {
                if(ds.Tables[0].Columns.Contains(key))
                {
                    dictData.Add(key, dict[key]);
                }
            }
            if (ds.Tables[0].Rows.Count > 0)//有重复
            {
                res = ImportUpdateInfos(dictData, ds.Tables[0].Rows[0]["Id"].ToString());
            }
            else
            {
                res = ImportInsertInfos(dictData);
            }
        }
        
        return res;
    }

    public static string ImportUpdateInfos(Dictionary<string, string> dict,string id)
    {
        if (dict.Count == 0)
        {
            return "";
        }
        string sql = string.Format("Update cost_sharing set ");
        foreach (string key in dict.Keys)
        {
            //if (!StringTools.HasChinese(key) && !key.Contains("Column"))
            {
                string value = "";
                if (key.Contains("Department") || key.Contains("Sector")
                    || key.Contains("SupervisorId")|| key.Contains("Specification")
                    || key.Contains("ManagerId") || key.Contains("DirectorId")
                    || key.Contains("HospitalId") || key.Contains("HospitalDepartment")
                    || key.Contains("ProductId") || key.Contains("SalesId"))
                {
                    value = dict[key];
                }
                else
                {
                    try
                    {
                        double val = Convert.ToDouble(dict[key]);
                        if (key.Contains("Ratio"))
                            value = (val * 100).ToString();
                        else
                            value = val.ToString();
                    }
                    catch
                    {
                        value="0";
                    }
                }
                //if (!string.IsNullOrEmpty(value))
                {
                    sql += string.Format("{0}='{1}', ", (key), value);
                }
                //else
                //{
                //    return "人员未找到！";
                //}
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
        string values = "" ;
        foreach (string key in dict.Keys)
        {
            //if (!StringTools.HasChinese(key) && !key.Contains("Column"))
            {
                string value = "";
                if (key.Contains("Department") || key.Contains("Sector")
                    || key.Contains("SupervisorId") || key.Contains("Specification")
                    || key.Contains("ManagerId") || key.Contains("DirectorId")
                    || key.Contains("HospitalId") || key.Contains("HospitalDepartment")
                    || key.Contains("ProductId") || key.Contains("SalesId"))
                {
                    value = dict[key];
                }
                else
                {
                    try
                    {
                        double val = Convert.ToDouble(dict[key]);
                        if (key.Contains("Ratio"))
                            value = (val * 100).ToString();
                        else
                            value = val.ToString();
                    }
                    catch
                    {
                        value = "0";
                    }
                }
                //if (!string.IsNullOrEmpty(value))
                {
                    fileds += string.Format("{0},", (key));
                    values += string.Format("'{0}',", value);
                }
                //else
                //{
                //    return "人员未找到！";
                //}
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

        string sql = string.Format("Insert into {0} ({1}) values ({2}) ", "cost_sharing", fileds, values);
        return SqlHelper.Exce(sql);
    }

    public static string InsertInfos(DataTable dt)
    {
        if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count == 0)
        {
            return "";
        }
        string sql = "";

        foreach (DataRow row in dt.Rows)
        {
            string fileds = "";
            string values = "";
            foreach (DataColumn clm in dt.Columns)
            {
                
                string value="";
                if(clm.ColumnName.Contains("HospitalId"))
                {
                    string val = row[clm.ColumnName].ToString();
                    value = GetId(val, "organization");
                }
                else if (clm.ColumnName.Contains("ProductId"))
                {
                    string val = row[clm.ColumnName].ToString();
                    value = GetId(val, "products");
                }
                else if (clm.ColumnName.Contains("DepartmentId")|| clm.ColumnName.Contains("SectorId"))
                {
                    //string val = row[clm.ColumnName].ToString();
                    //value = GetId(val, "department");
                    value = row[clm.ColumnName].ToString();
                }
                else if (clm.ColumnName.Contains("SalesId") || clm.ColumnName.Contains("SupervisorId")
                    || clm.ColumnName.Contains("ManagerId") || clm.ColumnName.Contains("DirectorId"))
                {
                    string val = row[clm.ColumnName].ToString();
                    value = GetId(val, "users");
                }
                else if(clm.ColumnName.Contains("Ratio"))
                {
                    string tempStr = row[clm.ColumnName].ToString();
                    try
                    {
                        double val = Convert.ToDouble(tempStr);
                        value = (val * 100).ToString();
                    }
                    catch
                    {
                        value = "0";
                    }                    
                }

                if (!string.IsNullOrEmpty(value))
                {
                    fileds += string.Format("{0},", clm.ColumnName);
                    values += string.Format("'{0}',", value);
                }
                
            }
            if (dt.Columns.Count > 0)
            {
                fileds = fileds.Substring(0, fileds.Length - 1);
                values = values.Substring(0, values.Length - 1);
                sql += string.Format("Insert into {0} ({1}) values ({2}) \r\n;", "cost_sharing", fileds, values);
            }
        }

        return SqlHelper.Exce(sql);
    }

    public static string InsertInfos(Dictionary<string, string> dict)
    {
        string sql = SqlHelper.GetInsertIgnoreString(dict, "cost_sharing");
        return SqlHelper.Exce(sql);
    }

    public static string UpdateInfos(Dictionary<string, string> dict,string id)
    {
        string sql = SqlHelper.GetUpdateString(dict, "cost_sharing"," where Id=" + id);
        return SqlHelper.Exce(sql);
    }
}