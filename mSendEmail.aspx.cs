using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;

public partial class mSendEmail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mPointApply",
           "i0SFeOuq3eonsbAWYfmAnrB0k_4K5d3Ub7Y6Z-KkYrc",
           "1000008",
           "http://yelioa.top/mSendEmail.aspx");
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
            if (action == "uploadAttachment")
            {
                uploadAttachment();
            }
            else if (action == "findTarget")
            {
                FindTarget();
            }
            else if (action == "saveDraft")
            {
                saveDraft();
            }
            else if (action == "sendEmail")
            {
                sendEmail();
            }
            else if (action == "initUserTree")
            {
                InitUserTree();
            }
            else if (action == "deleteDraft")
            {
                deleteDraft();
            }
            Response.End();
        }
    }
    private void FindTarget()
    {
        string name = Request.Form["name"];
        string json = "";
        string sql = "select distinct wechatUserId,userName from users where isValid='在职' ORDER BY userName DESC";
        DataSet ds = SqlHelper.Find(sql);
        if (ds != null)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("value", Type.GetType("System.String"));
            dt.Columns.Add("target", Type.GetType("System.String"));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (PinYinHelper.IsEqual(row["userName"].ToString(), name)
                   || row["userName"].ToString().Trim().Contains(name)
                    )

                    dt.Rows.Add(row["wechatUserId"], row["userName"]);

            }
            json = JsonHelper.DataTable2Json(dt);
        }
        Response.Write(json);
    }

    private void deleteDraft()
    {
        string emailId = Request.Form["emailId"];
        string res = EmailHelper.deleteDraft(emailId);
        Response.Write(res);
    }

    private void InitUserTree()
    {
        DataSet ds = UserInfoManage.getTree(GetUserInfo().companyId.ToString());
        string json = "";
        UserTreeHelper users;
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            users = new UserTreeHelper(ds.Tables[0],true);
            json = users.GetJson();
        }
        Response.Write(json);
    }
    public void uploadAttachment()
    {
        HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
        if (files.Count == 0)
            return;
        MD5 md5Hasher = new MD5CryptoServiceProvider();
        byte[] arrbytHashValue = md5Hasher.ComputeHash(files[0].InputStream);
        string strHashData = System.BitConverter.ToString(arrbytHashValue).Replace("-", "");
        string FileEextension = Path.GetExtension(files[0].FileName);
        string uploadDate = DateTime.Now.ToString("yyyyMMdd");
        string virtualPath = string.Format("/Data/ComponentAttachments/{0}/{1}/{2}", uploadDate, strHashData, files[0].FileName);
        string fullFileName = Server.MapPath(virtualPath);
        //创建文件夹，保存文件
        string path = Path.GetDirectoryName(fullFileName);
        Directory.CreateDirectory(path);
        JObject jObject = new JObject();
        if (!System.IO.File.Exists(fullFileName))
        {
            files[0].SaveAs(fullFileName);
        }
        if (files[0].ContentLength > 20 * 1024 * 1024)
        {
            jObject.Add("status", "无法上传超过20m的文件");
            Directory.Delete(fullFileName);
        }
        else
        {
            //string reducedImageUrl = ReducedImage(1024, 768, fullFileName);

            jObject.Add("status", "文件上传成功");
            jObject.Add("filePath", Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf("/")) + virtualPath);
            jObject.Add("fileName", files[0].FileName);

            string emailId = Request.Form["emailId"];
            //EmailHelper.InsertAttachment(emailId, files[0].FileName, Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf("/")) + virtualPath);
        }

        Response.Write(jObject.ToString());
    }

    /// <summary>  
    ///按大小压缩  
    /// </summary>  
    /// <param name="Width">压缩宽</param>  
    /// <param name="Height">压缩高</param>  
    /// <param name="targetFilePath">要压缩的图片路径</param>  
    /// <returns>返回压缩后的图片路径</returns>  
    public static string ReducedImage(int Width, int Height, string targetFilePath)
    {
        using (Image ResourceImage = Image.FromFile(targetFilePath))
        {
            Image ReducedImage;
            Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(delegate { return false; });
            ReducedImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);  //按大小压缩  
            string newImagePath = targetFilePath.Substring(0, targetFilePath.LastIndexOf("\\") + 1) + Guid.NewGuid().ToString() + ".jpg";  //压缩图片的存储路径  
            ReducedImage.Save(newImagePath, ImageFormat.Jpeg);   //保存压缩图片  
            ReducedImage.Dispose();
            return newImagePath;   //返回压缩图片的存储路径  
        }
    }

    private void saveDraft()
    {
        string uploadFileUrls = Request.Form["uploadFiles"];
        List<JObject> uploadFileUrlsList = JsonHelper.DeserializeJsonToList<JObject>(uploadFileUrls);
        string emailId = Request.Form["emailId"];
        foreach (JObject jObject in uploadFileUrlsList)
        {
            EmailHelper.InsertAttachment(emailId, jObject["FileName"].ToString(), jObject["FilePath"].ToString());
        }
        string subject = Request.Form["subject"];
        string text = Request.Form["text"];
        string recipients = Request.Form["recipients"];
        List<JObject> recipientsList = JsonHelper.DeserializeJsonToList<JObject>(recipients);

        List<string> recipientName = new List<string>();

        foreach (JObject jObject in recipientsList)
        {
            recipientName.Add(jObject["wechatUserId"].ToString());
        }

        Response.Write(EmailHelper.SaveDraft(emailId, subject, text, recipientName.ToArray()));
    }

    private void sendEmail()
    {
        string emailId = Request.Form["emailId"];
        UserInfo user = (UserInfo)Session["user"];
        WxCommon wx = new WxCommon("mSendEmail", "Wnn6eWDVIeZtNd8Bt69Kx3VNvICci-cH-TSHPwpXkYQ", "1000012", "http://yelioa.top/mSendEmail.aspx");
        JObject SendRes = JObject.Parse(EmailHelper.SendEmail(emailId));
        string res = "";
        if (SendRes["ErrCode"].ToString() == "0")
        {
            JObject jObject = new JObject();
            jObject.Add("title", "邮件通知");
            jObject.Add("description", "您收到一封来自《"
                + SendRes["SendName"].ToString() + "》,</br>主题为《" + SendRes["Subject"].ToString() + "》的邮件。</br>请注意查收！");
            jObject.Add("url", "http://yelioa.top/mEmailDetail.aspx?id=" + emailId);
            res = wx.SendWxMsg(SendRes["recipientId"].ToString(), "textcard", jObject);
        }
        else
            res = SendRes.ToString();
        Response.Write(res);
    }
    private UserInfo GetUserInfo()
    {
        return (UserInfo)Session["user"];
    }
}