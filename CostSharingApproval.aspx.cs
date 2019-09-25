using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CostSharingApproval : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Params["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "GetFormInfo")
            {
                //string name = Request.Params["name"];
                //Response.ContentType = "application/json";
                Response.Write(GetFormInfo());
            }
            else if (action == "getTableInfo")
            {
                //string name = Request.Params["name"];
                //Response.ContentType = "application/json";
                Response.Write(GetTableInfo());
            }
            else if (action == "submit")
            {
                //string name = Request.Params["name"];
                //Response.ContentType = "application/json";
                Response.Write(Submit());
            }
            Response.End();
        }
    }

    private string Submit()
    {
        JObject res = new JObject();
        string msg = "";
        string sql = "SELECT * FROM `v_outlet1` LIMIT 1";
        DataSet ds = SqlHelper.Find(sql,ref msg);
        if(ds !=null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataColumn c in ds.Tables[0].Columns)
            {
                if (c.ColumnName == "产品" || c.ColumnName == "医院" || c.ColumnName == "代表" || c.ColumnName == "区域经理" || c.ColumnName == "主管" ||
                    c.ColumnName == "销售负责人" || c.ColumnName == "部门" || !StringTools.HasChinese(c.ColumnName))
                    continue;
                string val = Request.Form[c.ColumnName].ToString();
                dict.Add(c.ColumnName, val);
            }
            sql = SqlHelper.GetUpdateString(dict, "new_cost_sharing", string.Format(" where Id={0}", Request.Form["Id"]));
            SqlExceRes r = new SqlExceRes(SqlHelper.Exce(sql));
            if(r.Result == SqlExceRes.ResState.Success)
            {
                res.Add("ErrCode", 0);
                res.Add("ErrMsg", "操作成功！");
            }
            else
            {
                res.Add("ErrCode", 2);
                res.Add("ErrMsg", r.ExceMsg);
            }
        }
        else
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", msg);
        }
        return res.ToString();
    }

    private string GetTableInfo()
    {
        string Id = Request.Params["id"];
        string sql = "select * from v_outlet1 where Id=" + Id;
        DataSet ds = SqlHelper.Find(sql);
        if (ds == null || ds.Tables[0].Rows.Count == 0)
            return null;
        else
            return JsonHelper.SerializeObject(ds.Tables[0].Rows[0]);
    }

    private string GetFormInfo()
    {
        string Id = Request.Params["id"];
        string Name = Request.Params["name"];
        string client = Request.Params["client"];
        string product = Request.Params["product"];
        DateTime start = Convert.ToDateTime(Request.Params["start"]);
        DateTime end = Convert.ToDateTime(Request.Params["end"]);

        JObject obj = new JObject();
        if (HasRight(Id) || Id == "0")
            obj.Add("hasRight", 1);
        else
        {
            obj.Add("hasRight", 0);
            obj.Add("data", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(new DataTable()));
            return obj.ToString();
        }
        if(Id != "0")
        {
            //int monthDay = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, monthDay);
            obj.Add("data", SearchFormInfo(start, end, Name, client,product));
        }
        else
        {
            obj.Add("data", GetOutletInfo());
        }

        return obj.ToString();
    }

    private string GetOutletInfo()
    {
        string sql = "select * from v_outlet1 order by Id";
        DataSet ds = SqlHelper.Find(sql);
        if (ds == null)
            return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(new DataTable());
        else
            return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]);
    }

    private string SearchFormInfo(DateTime start, DateTime end, string formName, string client, string product)
    {
        string sql = string.Format("select * from v_wf_form_{0} where Status='已审批'", formName);
        if(!string.IsNullOrEmpty(client))
        {
            if(formName.Contains("备案变更"))
                sql += string.Format(" and (医院 like '%{0}%' or 变更前医院 like '%{1}%')",client,client);
            else
                sql += string.Format(" and (医院 like '%{0}%')", client);
        }
        if (!string.IsNullOrEmpty(product))
        {
            if (formName.Contains("备案变更"))
                sql += string.Format(" and (产品 like '%{0}%' or 变更前产品 like '%{1}%')", product, product);
            else
                sql += string.Format(" and (产品 like '%{0}%')", product);
        }
        DataSet ds = SqlHelper.Find(sql);

        if (ds == null)
            return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(new DataTable());

        DataTable dt = ds.Tables[0];
        dt.Columns["DocCode"].SetOrdinal(0);
        dt.Columns["DocCode"].ColumnName = "单据编号";
        dt.Columns["CreateTime"].SetOrdinal(2);
        dt.Columns["CreateTime"].ColumnName = "提交时间";
        dt.Columns["Status"].SetOrdinal(3);
        dt.Columns["Status"].ColumnName = "审批状态";
        return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
    }

    private string SearchFormInfo(DateTime start, DateTime end, string formName, string submitter, string department, string result)
    {
        string sql = string.Format("select users.userName as 提交人,department.name as 提交部门, wf.* from v_wf_form_{0} as wf"
            + " INNER JOIN users ON wf.userId = users.userId"
            + " INNER JOIN department ON wf.departmentId = department.Id where Status = '已审批' and wf.CreateTime between '{1} 00:00:00' and"
            + " '{2} 23:59:59' ", formName, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
        if (!string.IsNullOrEmpty(submitter))
        {
            string tempStr = string.Format("select userId from users where userName like '%{0}%'", submitter);
            DataSet tempDs = SqlHelper.Find(tempStr);
            if (tempDs != null)
            {
                string val = "";
                foreach (DataRow r in tempDs.Tables[0].Rows)
                {
                    val += string.Format("{0},", r["userId"].ToString());
                }
                if (!string.IsNullOrEmpty(val))
                {
                    val = val.Substring(0, val.Length - 1);
                    sql += string.Format("and wf.userId in ({0}) ", val.ToString());
                }
            }
        }
        if (!string.IsNullOrEmpty(department))
        {
            string tempStr = string.Format("select Id from department where name like '%{0}%'", department);
            DataSet tempDs = SqlHelper.Find(tempStr);
            if (tempDs != null)
            {
                string val = "";
                foreach (DataRow r in tempDs.Tables[0].Rows)
                {
                    val += string.Format("{0},", r["Id"].ToString());
                }
                if (!string.IsNullOrEmpty(val))
                {
                    val = val.Substring(0, val.Length - 1);
                    sql += string.Format("and wf.departmentId in ({0}) ", val.ToString());
                }
            }
        }
        if (result != "全部")
        {
            sql += string.Format("and wf.Status='{0}' ", result);
        }
        DataSet ds = SqlHelper.Find(sql);

        if (ds == null)
            return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(new DataTable());

        DataTable dt = ds.Tables[0];
        dt.Columns["DocCode"].SetOrdinal(0);
        dt.Columns["DocCode"].ColumnName = "单据编号";
        dt.Columns["CreateTime"].SetOrdinal(2);
        dt.Columns["CreateTime"].ColumnName = "提交时间";
        dt.Columns["Status"].SetOrdinal(3);
        dt.Columns["Status"].ColumnName = "审批状态";
        return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
    }

    private bool HasRight(string id)
    {
        bool res = false;
        UserInfo user = (UserInfo)Session["user"];
        string sql = "select ManageRange from wf_form_config where Id=" + id;
        object ManageRange = SqlHelper.Scalar(sql);
        if ((Object.Equals(ManageRange, null)) || (Object.Equals(ManageRange, System.DBNull.Value)))
            return res;
        JObject obj = JObject.Parse(ManageRange.ToString());

        List<string> userList = JsonHelper.DeserializeJsonToList<string>(obj["userJSON"].ToString());

        if (userList.Contains(user.userId.ToString()))
        {
            return true;
        }

        List<string> departList = JsonHelper.DeserializeJsonToList<string>(obj["departmentJSON"].ToString());
        if (departList.Count == 0)
            return res;

        DataTable allDepartment = RightManage.GetDepartmentIds(user.wechatUserId);
        foreach (DataRow row in allDepartment.Rows)
        {
            if (departList.Contains(row["departmentId"].ToString()))
            {
                res = true;
                break;
            }
        }

        return res;
    }
}