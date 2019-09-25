using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

public partial class mEmailGroupSetting : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mPointApply",
           "i0SFeOuq3eonsbAWYfmAnrB0k_4K5d3Ub7Y6Z-KkYrc",
           "1000008",
           "http://yelioa.top/mEmailGroupSetting.aspx");
        UserInfo user = new UserInfo();
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "GetGroupList")
            {
                GetGroupList();
            }
            else if (action == "getUsers")
            {
                getUsers();
            }
            else if (action == "DeleteGroup")
            {
                DeleteGroup();
            }
            else if (action == "Submit")
            {
                Submit();
            }
            //else if (action == "initUserTree")
            //{
            //    InitUserTree();
            //}
            //else if (action == "deleteDraft")
            //{
            //    deleteDraft();
            //}
            Response.End();
        }
    }

    private void Submit()
    {
        UserInfo user = (UserInfo)Session["user"];
        string groupName = Request.Form["groupName"];
        string members = Request.Form["members"];
        string type = Request.Form["type"];
        string groupId = Request.Form["groupId"];

        string res = "";
        if (type == "add")
            res = EmailHelper.CreateGroup(user.userId.ToString(), members, groupName);
        else
            res = EmailHelper.SaveGroup(user.userId.ToString(), members, groupName, groupId);
        Response.Write(res);
    }

    private void GetGroupList()
    {
        UserInfo user = (UserInfo)Session["user"];
        JObject obj = JObject.Parse(EmailHelper.GetGroupInfo(user.userId.ToString()));
        string res = "";
        if(obj["ErrCode"].ToString()=="0")
            res = obj["Data"].ToString();
        Response.Write(res);
    }

    private void DeleteGroup()
    {
        UserInfo user = (UserInfo)Session["user"];
        string groupName = Request.Form["groupName"];
        string res = EmailHelper.DeleteGroup(user.userId.ToString(), groupName);
        Response.Write(res);
    }

    private void getUsers()
    {
        DataTable dt = UserInfoManage.GetAllUsers();
        JArray userList = new JArray();
        foreach (DataRow row in dt.Rows)
        {
            bool hasContained = false;
            string FirstLetter = (row["HanZiPinYinSortColumn"].ToString()).Substring(0, 1);
            foreach (JObject userGp in userList)
            {
                if(userGp["Index"].ToString() == FirstLetter)
                {
                    JObject user = new JObject();
                    user.Add("UserName", row["userName"].ToString());
                    user.Add("UserId", row["userId"].ToString());
                    user.Add("Checked", false);
                    user.Add("Avatar", row["avatar"].ToString());
                    ((JArray)userGp["Users"]).Add(user);
                    hasContained = true;
                }
            }
            if(!hasContained)
            {
                JObject userGroup = new JObject();
                userGroup.Add("Index", FirstLetter);
                //userGroup.Add("Checked", false);
                JArray users = new JArray();
                JObject user = new JObject();
                user.Add("UserName", row["userName"].ToString());
                user.Add("UserId", row["userId"].ToString());
                user.Add("Checked", false);
                user.Add("Avatar", row["avatar"].ToString());
                users.Add(user);
                userGroup.Add("Users", users);
                userList.Add(userGroup);
            }            
        }
        Response.Write(userList.ToString()); 
    }
}