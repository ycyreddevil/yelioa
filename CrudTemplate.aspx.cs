using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Data;

public partial class CrudTemplate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];

        if (string.IsNullOrEmpty(action))
        {
            action = Request.Params["actSubmit"];
            if (!string.IsNullOrEmpty(action))
            {
                Response.Clear();
                if ("save".Equals(action))
                {
                    Response.Write(save());
                }
                Response.End();
            }
        }
        else
        {
            Response.Clear();

            if ("init".Equals(action))
            {
                Response.Write(init());
            }
            else if ("delete".Equals(action))
            {
                Response.Write(delete());
            }
            //else if ("accountApproval".Equals(action))
            //{
            //    Response.Write(accountApproval());
            //}
            //else if ("exportVoucher".Equals(action))
            //{
            //    Response.Write(exportVoucher());
            //}

            Response.End();
        }
    }
    private string save()
    {
        JObject res = new JObject();
        string table = Request.Params["table"];
        string Id = Request.Params["Id"];
        try {
            string sql = string.Format("SELECT * FROM {0} LIMIT 1", table);
            string msg = "";
            DataSet ds = SqlHelper.Find(sql, ref msg);
            if (ds == null)
            {
                res.Add("ErrCode", 500);
                res.Add("ErrMsg", msg);
            }
            else
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (DataColumn c in ds.Tables[0].Columns)
                {
                    if (c.ColumnName == "Id")
                        continue;
                    dict.Add(c.ColumnName, Request.Params[c.ColumnName]);
                }
                if(!StringTools.IsInt(Id))//add
                {
                    sql = SqlHelper.GetInsertString(dict, table);
                }
                else//edit
                {
                    string condition = string.Format(" where Id={0}", Id);
                    sql = SqlHelper.GetUpdateString(dict, table, condition);
                }
                
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", SqlHelper.Exce(sql));
            }
        }
        catch(Exception ex)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", ex.ToString());
        }

        

        return res.ToString();
    }
    //private string add()
    //{
    //    JObject res = new JObject();
    //    string table = Request.Params["table"];

    //    try
    //    {
    //        string sql = string.Format("SELECT * FROM {0} LIMIT 1", table);
    //        string msg = "";
    //        DataSet ds = SqlHelper.Find(sql, ref msg);
    //        if (ds == null)
    //        {
    //            res.Add("ErrCode", 500);
    //            res.Add("ErrMsg", msg);
    //        }
    //        else
    //        {
    //            Dictionary<string, string> dict = new Dictionary<string, string>();
    //            foreach (DataColumn c in ds.Tables[0].Columns)
    //            {
    //                if (c.ColumnName == "Id")
    //                    continue;
    //                dict.Add(c.ColumnName, Request.Params[c.ColumnName]);
    //            }
    //            sql = SqlHelper.GetInsertString(dict, table);
    //            res.Add("ErrCode", 0);
    //            res.Add("ErrMsg", SqlHelper.Exce(sql));
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        res.Add("ErrCode", 1);
    //        res.Add("ErrMsg", ex.ToString());
    //    }

        
    //    return res.ToString();
    //}

    private string delete()
    {
        JObject res = new JObject();
        string table = Request.Form["table"];
        string data = Request.Form["data"];
        string condition = "";
        DataTable dt = JsonHelper.Json2Dtb(data);
        if(dt.Rows.Count>0)
        {
            foreach (DataRow row in dt.Rows)
            {
                condition += row[0].ToString() + ",";
            }
            condition = condition.Substring(0, condition.Length - 1);
            string sql = string.Format("delete from {0} where Id in ({1})", table, condition);
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", SqlHelper.Exce(sql));
        }
        else
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "未找到选中数据！");
        }
        return res.ToString();
    }

    private string init()
    {
        JObject res = new JObject();
        string table = Request.Form["table"];
        string msg = "";
        string sql = string.Format("select * from {0}", table);
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if(ds==null)
        {
            res.Add("ErrCode", 500);
            res.Add("ErrMsg", msg);
        }
        else
        {
            res.Add("ErrCode", 0);
            res.Add("data", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]));
        }
        return res.ToString();
    }
}