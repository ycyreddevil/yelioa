using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

public partial class MemberManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getInfos") 
            {                
                Response.Write(getInfos());
            }
            else if (action == "getTree")
            {
                Response.Write(getTree());
            }
            else if (action == "addTree")
            {
                Response.Write(AddTree());
            }
            else if (action == "editTree")
            {
                Response.Write(EditTree());
            }
            else if (action == "deletTree")
            {
                Response.Write(DeleteTree());
            }
            else if (action == "GetDepartRemark")
            {
                Response.Write(GetDepartRemark());
            }
            else if (action == "del")
            {
                Response.Write(Delete());
            }
            else if (action == "CheckMobile")
            {
                Response.Write(CheckMobile());
            }
            else if (action == "CheckEmployeeCode")
            {
                Response.Write(CheckEmployeeCode());
            }
            else if (action == "CheckInfo")
            {
                Response.Write(CheckInfo());
            }
            else if (action == "upload")
            {
                Response.Write(uploadFile());
            }
            else if (action == "add")
            {
                Response.Write(Add());
            }
            else if (action == "edit")
            {
                Response.Write(Edit());
            }
            else if (action == "UpdateFromWx")
            {
                Response.Write(UpdateFromWx());
            }
            else if (action == "getSelectedDepartmentId")
            {
                Response.Write(getSelectedDepartmentId());
            }
            else if (action == "getPostList")
            {
                Response.Write(getPosts());
            }
            else if (action == "getDepartmentMember")
            {
                Response.Write(getDepartmentMember());
            }
            Response.End();
        }
        else
        {
            action = Request.Params["act"];
            if (!string.IsNullOrEmpty(action))
            {
                Response.Clear();
                //if (action == "add")
                //{
                //    Response.Write(Add());
                //}
                //else if (action == "edit")
                //{
                //    Response.Write(Edit());
                //}
                if (action == "getPosts")
                {
                    Response.Write(getPosts());
                }
                else if (action == "getTree")
                {
                    Response.Write(getTree());
                }
                else if (action == "getSelectedDepartmentId")
                {
                    string wechatUserId = Request.Params["wechatUserId"];
                    Response.ContentType = "application/json";
                    Response.Write(getSelectedDepartmentId(wechatUserId));
                }
                else if (action == "saveDepartPostData")
                {
                    Response.ContentType = "application/json";
                    Response.Write(saveDepartPostData());
                }
                else if (action == "updateDepartPostData")
                {
                    Response.ContentType = "application/json";
                    Response.Write(updateDepartPostData());
                }
                else if (action == "destroyDepartPostData")
                {
                    Response.ContentType = "application/json";
                    Response.Write(destroyDepartPostData());
                }
                Response.End();
            }
        }
    }

    private string saveDepartPostData()
    {
        string res = "";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("wechatUserId", Request.Form["wechatUserId"]);
        string departmentId = Request.Form["department"];
        if (!StringTools.IsNumeric(departmentId))
            return "请选择部门！";
        dict.Add("departmentId", departmentId);
        string postId = Request.Form["postName"];
        if (!StringTools.IsNumeric(postId))
            return "请选择岗位！";
        dict.Add("postId", postId);
        //res = OrganizationInfoManage.InsertInfo(dict);
        res = UserInfoManage.InsertDepartPost(dict);
        return res;
    }

    private string updateDepartPostData()
    {
        string res = "";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("wechatUserId", Request.Form["wechatUserId"]);
        string departmentId = Request.Form["department"];
        if (StringTools.IsNumeric(departmentId))
            dict.Add("departmentId", departmentId);
        string postId = Request.Form["postName"];
        if (StringTools.IsNumeric(postId))
            dict.Add("postId", postId);
        //res = OrganizationInfoManage.UpdateInfo(dict, Request.Form["Id"]);
        res = UserInfoManage.UpdateDepartPost(dict, Request.Form["Id"]);
        return res;
    }

    private string destroyDepartPostData()
    {
        string res = "";
        //res = OrganizationInfoManage.DeleteInfo(Request.Form["Id"]);
        res = UserInfoManage.DeleteDepartPost(Request.Form["Id"]);
        return res;
    }

    private string getSelectedDepartmentId(string wechatUserId)
    {
        string res = "";
        DataSet ds = UserInfoManage.GetSelectedTree(wechatUserId);
        if (ds != null)
            res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]);
        return res;
    }

    private string getSelectedDepartmentId()
    {
        string wechatUserId = Request.Form["wechatUserId"];
        return getSelectedDepartmentId(wechatUserId);
    }

    private string UpdateFromWx()
    {
        WxUserInfo wxHelper = new WxUserInfo();
        string id = Request.Form["Id"];
        string type = Request.Form["type"];
        string res = "";
        if (type == "department")
        {
            object val = wxHelper.GetWxDepartmentJson(id);            
            res = UserInfoManage.SaveDepartmentFromWx(val);            
        }
        else
        {
            object val = wxHelper.GetWxUserInfoJsonByDepartmentId("1");
            res = UserInfoManage.SaveUserInfoFromWx(val);
        }
        
        return res;
    }

    private string getPosts()
    {
        string json = "";
        ArrayList list = PostInfoManage.GetPosts(GetUserInfo().companyId.ToString());
        json = JsonHelper.SerializeObject(list);        
        return json;
    }

    private string getInfos()
    {
        string departId = Request.Form["departmentId"];
        string searchString = Request.Form["searchString"];
        string json = "";
        DataTable dt = UserInfoManage.getInfos(GetUserInfo().companyId.ToString(), departId,searchString);
        if(dt!=null)
        {
            json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }        
        return json;
    }

    private string getDepartmentMember()
    {
        string departmentId = Request.Form["departmentId"];

        DataTable dt = SqlHelper.Find("select wechatUserId, userName from v_user_department_post where departmentId =" + departmentId).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string getTree()
    {
        DataSet ds = UserInfoManage.getTree(GetUserInfo().companyId.ToString());
        if(ds == null)
        {
            return "F";
        }
        DepartmentTreeHelper tree = new DepartmentTreeHelper(ds.Tables[0]);
        string json = tree.GetJson();
        return json;
    }

    private string AddTree()
    {
        string res = "";
        Dictionary<string, string> dict = new Dictionary<string, string>();        
        dict.Add("name", Request.Form["name"]);
        dict.Add("parentId", Request.Form["parentId"]);
        dict.Add("parentName", Request.Form["parentName"]);
        dict.Add("companyId", GetUserInfo().companyId.ToString());
        dict.Add("remark", Request.Form["remark"]);
        res = UserInfoManage.AddTree(dict);
        return res;
    }

    private string EditTree()
    {
        string res = "";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("name", Request.Form["name"]);
        dict.Add("parentId", Request.Form["parentId"]);
        dict.Add("parentName", Request.Form["parentName"]);
        dict.Add("companyId", GetUserInfo().companyId.ToString());
        dict.Add("remark", Request.Form["remark"]);
        dict.Add("departmentId", Request.Form["departmentId"]);
        dict.Add("wechatUserId", Request.Form["leaderWechatUserId"]);
        res = UserInfoManage.EditTree(dict, Request.Form["id"]);
        return res;
    }

    private string DeleteTree()
    {
        string id = Request.Form["id"];
        return UserInfoManage.DeleteTree(id);
    }

    private string GetDepartRemark()
    {
        string id = Request.Form["id"];
        return UserInfoManage.GetDepartRemark(GetUserInfo().companyId.ToString(), id);
    }

    private string CheckMobile()
    {
        bool isEdit = Convert.ToBoolean(Request.Form["isEdit"]);
        int res = Convert.ToInt32( UserInfoManage.CheckMobile(Request.Form["mobile"]));
        if(isEdit && res<=1)
        {
            return "T";
        }
        else if (!isEdit && res == 0)
        {
            return "T";
        }
        else
        {
            return "F";
        }
    }

    private string CheckEmployeeCode()
    {
        bool isEdit = Convert.ToBoolean(Request.Form["isEdit"]);
        int res = Convert.ToInt32(UserInfoManage.CheckEmployeeCode(Request.Form["code"], GetUserInfo().companyId.ToString()));
        if (isEdit && res <= 1)
        {
            return "T";
        }
        else if (!isEdit && res == 0)
        {
            return "T";
        }
        else
        {
            return "F";
        }
    }
    private string CheckInfo()
    {
        bool isEdit = false;
        if (Request.Form["state"] == "edit")
        {
            isEdit=true;
        }
        DataSet ds = UserInfoManage.CheckInfo(Request.Form["code"], GetUserInfo().companyId.ToString(), Request.Form["mobile"], Request.Form["idNumber"]);
        int standartNumber = 0;//插入模式不允许查询到记录
        if(isEdit)
        {
            standartNumber = 1;//编辑模式最多查询到1条记录
        }
        string  res = "数据库读取失败!";
        if(ds != null )
        {
            string res1 = "";
            if (ds.Tables[0].Rows.Count > standartNumber)
            {
                res1 += "手机号、";
            }
            if (ds.Tables[1].Rows.Count > standartNumber)
            {
                res1 += "工号、";
            }
            if (ds.Tables[2].Rows.Count > standartNumber)
            {
                res1 += "身份证号、";
            }
            if(string.IsNullOrEmpty(res1))
            {
                res = "T";
            }
            else
            {
                res1 = res1.Substring(0, res1.Length - 1);//删掉最后一个顿号
                res1 += "已存在，请重新输入!";
                res = res1;
            }
        }
        
        return res;
    }

    private string Delete()
    {
        string id = Request.Form["id"];
        return UserInfoManage.Delete(id);
    }

    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }

    private string Add()
    {
        string res = "添加失败！";

        DataSet ds = SqlHelper.GetFiledNameAndComment("users");
        if (ds != null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] exceptedField = new string[] { "passWord", "userId", "department", "departmentId", "companyId" , "nativePlace" };
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (!exceptedField.Contains<string>(row["field"].ToString()))
                {
                    dict.Add(row["field"].ToString(), Request.Form[row["field"].ToString()]);
                }
            }
            dict.Add("departmentId", Request.Form["department"]);
            UserInfo user = (UserInfo)Session["user"];
            dict.Add("companyId", user.companyId.ToString());
            string id = Request.Form["userId"];

            res = UserInfoManage.InsertInfos(dict);
        }
        return res;
    }

    private string Edit()
    {
        string res = "更新失败";
        DataSet ds = SqlHelper.GetFiledNameAndComment("users");
        if(ds != null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] exceptedField = new string[] { "passWord", "userId" , "department", "departmentId", "companyId", "nativePlace","post" };
            foreach (DataRow row in ds.Tables[0].Rows)
            {                
                if(!exceptedField.Contains<string>(row["field"].ToString()))
                {
                    dict.Add(row["field"].ToString(), Request.Form[row["field"].ToString()]);
                }                
            }
            //dict.Add("departmentId", Request.Form["department"]);
            string id = Request.Form["userId"];

            res = UserInfoManage.UpdateInfos(dict, id);
        }       

        return res;
    }

    private string uploadFile()
    {
        string res = "读取文件失败！";
        DataTable dt = ExcelHelperV2_0.Import(Request);
        if (dt != null)
        {
            dt.Columns.Add("companyId"); 
            dt.Columns.Add("departmentId");
            dt.Columns.Add("department");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["companyId"] = GetUserInfo().companyId;
                dt.Rows[i]["departmentId"] = "1";
                dt.Rows[i]["department"] = "业力集团";
                string[] values = dt.Rows[i]["毕业学校"].ToString().Split('-');
                if(values.Length>1)//
                {
                    dt.Rows[i]["毕业学校"] = values[0];
                    dt.Rows[i]["专业"] = values[1];
                }

            }
            res = UserInfoManage.InsertInfos(dt);
        }
        return res;
    }

}