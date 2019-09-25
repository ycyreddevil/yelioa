using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mPointDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "initForm")
            {
                Response.Write(initForm());
            }
            Response.End();
        }
    }

    private string initForm()
    {
        string ids = Request.Form["ids"];
        JObject res = new JObject();
        string sql = string.Format("SELECT a.*,GROUP_CONCAT(distinct b.department) targetDepartment,b.avatar targetAvatar,GROUP_CONCAT(distinct c.department) proposerDepartment,c.avatar proposerAvatar" +
            " FROM `accumulate_points` a  LEFT JOIN v_user_department_post b on a.Target=b.userName LEFT JOIN v_user_department_post c on a.Proposer=c.userName " +
            "where a.Id in ({0}) GROUP BY a.Target,a.Proposer", ids);
        string msg = "";
        DataSet ds = SqlHelper.Find(sql,ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", msg);
        }
        else if (ds.Tables[0].Rows.Count == 0)
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", "未找到相关单据");
        }
        else
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if(row["CheckState"].ToString()=="未审核")
                {
                    row["CheckState"] = "已申请";
                }
                string targetAvatar = row["targetAvatar"].ToString();
                string proposerAvatar = row["proposerAvatar"].ToString();
                row["targetDepartment"] = DepartmentFilter(row["targetDepartment"].ToString(), ref targetAvatar);
                row["proposerDepartment"] = DepartmentFilter(row["proposerDepartment"].ToString(), ref proposerAvatar);
                row["targetAvatar"] = targetAvatar;
                row["proposerAvatar"] = proposerAvatar;
            }
            res.Add("ErrCode", 0);
            res.Add("ErrMsg", "查询成功");
            res.Add("document", JsonHelper.DataTable2Json(ds.Tables[0]));
        }
        return res.ToString();
    }

    private string DepartmentFilter(string departmentName,ref string avatar)
    {
        if(departmentName=="")
        {
            avatar = "resources/detail.png";
            return "该成员已离职";            
        }
        string[] departmentList = departmentName.Split(',');
        string name = "";
        foreach (string department in departmentList)
        {
            if (department.Split('/').Length - 1 > 1)
            {
                string str1 = department.Substring(department.LastIndexOf('/') + 1, department.Length - department.LastIndexOf('/') - 1);
                string str2 = department.Substring(0, department.LastIndexOf('/'));
                string str3 = str2.Substring(str2.LastIndexOf('/') + 1, str2.Length - str2.LastIndexOf('/') - 1);
                name += str3 + "/" + str1 + "，";
            }
            else
            {
                name += department + '，';
            }
            //name += department.Substring(department.IndexOf('/') + 1, department.Length - department.IndexOf('/') - 1) + ",";
        }
        return name.Substring(0, name.Length - 1);
    }
}