using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using Newtonsoft.Json.Linq;

public partial class PostManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getPosts")
            {
                Response.Write(getPosts(GetUserInfo().companyId.ToString()));
            }
            else if (action == "del")
            {
                Response.Write(Delete());
            }
            else if (action == "TbLoad")
            {
                Response.Write(TbLoad());
            }
            else if (action == "save")
            {
                Response.Write(Save());
            }
            Response.End();
        }
        else
        {
            action = Request.Params["act"];
            if (!string.IsNullOrEmpty(action))
            {
                Response.Clear();
                if (action == "add")
                {
                    Response.Write(Add());
                }
                else if (action == "edit")
                {
                    Response.Write(Edit());
                }
                Response.End();
            }
        }
    }

    private string getPosts(string companyId)
    {
        string json = "";
        DataSet ds = PostInfoManage.GetPostsDataset(companyId);
        json = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0].Rows.Count, ds.Tables[0]);
        return json;
    }

    private string TbLoad()
    {
        string postName = Request.Form["post"];
        JArray res = new JArray();

        DataTable dt = PostInfoManage.GetRightsItem(postName);
        string strRes = "";
        if (dt != null)
        {
            strRes = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt.Rows.Count, dt);
        }
        JObject objRight = JObject.Parse(strRes);
        res.Add(objRight);

        DataSet ds = PostInfoManage.GetUserByPost(postName);
        strRes = "";
        if(ds != null)
        {
            strRes = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(ds.Tables[0]);
        }
        JObject objUsers = JObject.Parse(strRes);
        res.Add(objUsers);
        return res.ToString();
    }

    private string Save()
    {
        string res="";
        string state = Request.Form["state"];
        string formData = Request.Form["formData"];
        string rightsData = Request.Form["rightsData"];
        Dictionary<string, string> dictForm = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(formData);
        ArrayList list = JsonHelper.DeserializeJsonToObject<ArrayList>(rightsData);
        //Dictionary<string, string> dictRights = JsonHelper.DeserializeJsonToObject<Dictionary<string, string>>(rightsData);
        if(state=="add")
        {
            UserInfo user = (UserInfo)Session["user"];
            dictForm.Add("creatTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            dictForm.Add("companyId", user.companyId.ToString());
            res = PostInfoManage.InsertInfos(dictForm,list);            
        }
        else
        {
            res = PostInfoManage.UpdateInfos(dictForm, dictForm["postId"], list);
        }
        return res;
    }

    private string Delete()
    {
        string id = Request.Form["id"];
        string postName = Request.Form["postName"];
        return PostInfoManage.Delete(id, postName);
    }

    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }

    private string Add()
    {
        //System.Threading.Thread.Sleep(3000);
        string res = "添加成功！";
        //string name = Request.Form["name"];
        UserInfo user = (UserInfo)Session["user"];
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("postName", Request.Form["postName"]);
        dict.Add("creatTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict.Add("remark", Request.Form["remark"]);
        dict.Add("companyId", user.companyId.ToString());

        //res = PostInfoManage.InsertInfos(dict);
        return res;
    }

    private string Edit()
    {
        string res = "添加成功！";
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("code", Request.Form["postName"]);
        dict.Add("creatTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict.Add("remark", Request.Form["remark"]);

        string id = Request.Form["postId"];

        //res = PostInfoManage.UpdateInfos(dict, id);
        return res;
    }
}