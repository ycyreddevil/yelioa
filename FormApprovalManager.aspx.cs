using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using Newtonsoft.Json.Linq;

public partial class FormApprovalManager : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //WxCommon wx = new WxCommon("mPointApply",
        //  "i0SFeOuq3eonsbAWYfmAnrB0k_4K5d3Ub7Y6Z-KkYrc",
        //  "1000008",
        //  "http://yelioa.top/mEmailGroupSetting.aspx");
        //UserInfo user = new UserInfo();
        //string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        //if (res != "success")
        //{
        //    Response.Clear();
        //    Response.Write("<script language='javascript'>alert('" + res + "')</script>");
        //    Response.End();
        //    return;
        //}
        string action = Request.Params["act"];
        if (!string.IsNullOrEmpty(action)) {
            Response.Clear();
            if (action == "GetFormApprovalList")
            {
                Response.ContentType = "application/json";
                Response.Write(GetFormApprovalList());
            }
            else if (action == "GetFormListByName")
            {
                string name = Request.Params["name"];
                Response.ContentType = "application/json";
                Response.Write(GetFormListByName(name)); 
            }
            else if (action == "GetFormInfo")
            {
                //string name = Request.Params["name"];
                //Response.ContentType = "application/json";
                Response.Write(GetFormInfo());
            }
            else if (action == "SearchFormInfo")
            {
                //string name = Request.Params["name"];
                //Response.ContentType = "application/json";
                Response.Write(SearchFormInfo());
            }
            Response.End();
        }
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
        foreach(DataRow row in allDepartment.Rows)
        {
            if(departList.Contains(row["departmentId"].ToString()))
            {
                res = true;
                break;
            }
        }

        return res;
    }

    private string GetFormInfo()
    {
        string Id = Request.Params["id"];
        string Name = Request.Params["name"];

        JObject obj = new JObject();
        if(HasRight(Id))
            obj.Add("hasRight", 1);
        else
        {
            obj.Add("hasRight", 0);
            obj.Add("data", JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(new DataTable()));
            return obj.ToString();
        }

        int monthDay = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, monthDay);
        obj.Add("data", SearchFormInfo(start, end, Name,null,null, "全部"));

        return obj.ToString();
    }

    private string SearchFormInfo()
    {
        DateTime start = Convert.ToDateTime(Request.Params["start"]);
        DateTime end = Convert.ToDateTime(Request.Params["end"]);
        string submitter = Request.Params["submitter"];
        string department = Request.Params["department"];
        string result = Request.Params["result"];
        string Name = Request.Params["name"];

        return SearchFormInfo(start, end, Name, submitter, department, result);
    }

    private string SearchFormInfo(DateTime start, DateTime end, string formName,string submitter,string department,string result)
    {
        string sql = string.Format("select users.userName as 提交人,department.name as 提交部门, wf.* from wf_form_{0} as wf"
            + " INNER JOIN users ON wf.userId = users.userId"
            + " INNER JOIN department ON wf.departmentId = department.Id where wf.CreateTime between '{1} 00:00:00' and"
            + " '{2} 23:59:59' ", formName, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
        if(!string.IsNullOrEmpty(submitter))
        {
            string tempStr = string.Format("select userId from users where userName like '%{0}%'", submitter);
            DataSet tempDs = SqlHelper.Find(tempStr);
            if(tempDs != null)
            {
                string val = "";
                foreach(DataRow r in tempDs.Tables[0].Rows)
                {
                    val += string.Format("{0},", r["userId"].ToString());
                }
                if(!string.IsNullOrEmpty(val))
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

        // 新增审批时间
        dt.Columns.Add("审批时间", Type.GetType("System.String"));
        dt.Columns["审批时间"].SetOrdinal(4);
        foreach (DataRow dr in dt.Rows)
        {
            if ("审批中" == dr["审批状态"].ToString())
                dr["审批时间"] = "";
            else
            {
                sql = string.Format("select t2.approvalTime from wf_form_{0} t1 left join wf_record t2 on t1.id = t2.docId " +
                    "where t2.tableName = '{0}' and t1.id = '{1}' order by t2.approvalTime limit 1", formName, dr["Id"].ToString());

                dr["审批时间"] = SqlHelper.Find(sql).Tables[0].Rows[0][0].ToString();
            }
        }

        return JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
    }

    private string GetFormApprovalList()
    {
        string json = "";
        ArrayList list = FormApprovalManage.GetFormApprovalList();
        json = JsonHelper.SerializeObject(list);
        //DataTable dt = FormApprovalManage.GetFormApprovalList();
        //List<TreeNode> list = new List<TreeNode>();
        //TreeNode root = new TreeNode("表单", 1);

        //foreach(DataRow row in dt.Rows)
        //{
        //    string txt = row["FormName"].ToString();
        //    int id = Convert.ToInt32(row["Id"]);
        //    TreeNode node = new TreeNode(txt, id);
        //    root.children.Add(node);
        //}
        //list.Add(root);
        //json = JsonHelper.SerializeObject(list);
        return json;
    }

    private string GetFormListByName(string name)
    {
        string json = "";
        ArrayList list = FormApprovalManage.GetFormListByName(name);
        json = JsonHelper.SerializeObject(list);
        return json;
    }
}