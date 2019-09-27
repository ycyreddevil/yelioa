using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Web;


public partial class mFinanceReimburse : System.Web.UI.Page
{
    public UserInfo user;

    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mFinanceReimburse",
           "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
           "1000006",
           "http://yelioa.top/mFinanceReimburse.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);

        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        user = (UserInfo)Session["user"];

        string action = Request.Form["act"];

        if (Common.GetApplicationValid("mFinanceReimburse.aspx") == "0" && !"uploadReimburseImage".Equals(action) && !"deleteReimburseImage".Equals(action))
        {
            Response.Clear();
            Response.Write("<script language='javascript'>location.href='Default.aspx';</script>");
            Response.End();
            return;
        }

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "findProductName")
            {
                Response.Write(findProductName());
            }
            else if (action == "findBranch")
            {
                Response.Write(findBranch());
            }
            else if (action == "findFeeDetail")
            {
                Response.Write(findFeeDetail());
            }
            else if (action == "findChildrenFeeDetail")
            {
                Response.Write(findChildrenFeeDetail());
            }
            else if (action == "findFeeDepartment")
            {
                Response.Write(findFeeDepartment());
            }
            else if (action == "findInformer")
            {
                Response.Write(findInformer());
            }
            else if (action == "submitReimburse")
            {
                Response.Write(submitReimburse());
            }
            else if (action == "getProcessInfo")
            {
                Response.Write(getProcessInfo());
            }
            else if (action == "checkMultiDepartment")
            {
                Response.Write(checkMultiDepartment());
            }
            else if (action == "uploadReimburseImage")
            {
                Response.Write(uploadReimburseImage());
            }
            else if (action == "deleteReimburseImage")
            {
                Response.Write(deleteReimburseImage());
            }
            else if (action == "dataEchoed")
            {
                Response.Write(dataEchoed());
            }
            else if (action == "findProjectCode")
            {
                Response.Write(findProjectCode());
            }
            else if (action == "findTravelApply")
            {
                Response.Write(findTravelApply());
            }
            else if (action == "findTravelApplyAmount")
            {
                Response.Write(findTravelApplyAmount());
            }
            Response.End();
        }
    }

    private string checkMultiDepartment()
    {
        UserInfo user = (UserInfo)Session["user"];

        DataTable dt = MobileReimburseManage.checkMultiDepartment(user);

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string findProductName()
    {
        string productName = Request.Form["name"];
        string departmentName = Request.Form["department"];
        List<DepartmentPost> departmentList = (List<DepartmentPost>)Session["DepartmentPostList"];

        //DataTable dt = MobileReimburseManage.findProduct(productName, departmentList, departmentName);
        UserInfo user = (UserInfo)Session["user"];
        DataTable dt = MobileReimburseManage.FindProduct(Convert.ToInt32(user.userId), productName);

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string findBranch()
    {
        string branch = Request.Form["name"];
        string departmentName = Request.Form["department"];
        List<DepartmentPost> departmentList = (List<DepartmentPost>)Session["DepartmentPostList"];

        //DataTable dt = MobileReimburseManage.findBranch(branch, departmentList, departmentName);
        UserInfo user = (UserInfo)Session["user"];
        DataTable dt = MobileReimburseManage.FindClient((user.userId), branch);

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string findFeeDetail()
    {
        string feeDetail = Request.Form["name"];
        string department = Request.Form["department"];
        string product = Request.Form["product"];

        DataTable dt = MobileReimburseManage.findParentFeeDetail(department,product);

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string findChildrenFeeDetail()
    {
        string feeDetail = Request.Form["name"];
        string feeDepartment = Request.Form["department"];
        DataTable dt = MobileReimburseManage.findChildrenFeeDetail(feeDetail, feeDepartment);

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string findFeeDepartment()
    {
        string feeDepartment = Request.Form["name"];

        DataTable dt = MobileReimburseManage.findFeeDepartment(feeDepartment);

        if (dt == null)
            return null;

        return JsonHelper.DataTable2Json(dt);
    }

    private string findInformer()
    {
        string informer = Request.Form["name"];

        DataTable dt = MobileReimburseManage.findInformer(informer);

        if (dt == null)
            return null;
        return JsonHelper.DataTable2Json(dt);
    }

    private string submitReimburse()
    {
        UserInfo user = (UserInfo)Session["user"];

        string apply_time = Request.Form["apply_time"];
        string product = Request.Form["product"];
        string branch = Request.Form["branch"];
        string fee_department = Request.Form["fee_department"];
        string fee_company = Request.Form["fee_company"];
        string fee_detail = Request.Form["fee_detail"];
        string fee_amount = Request.Form["fee_amount"];
        string file = Request.Form["file"];
        string remark = SqlHelper.DesEncrypt(Request.Form["remark"]);
        string approver = Request.Form["approvers"];
        string department = Request.Form["department"];
        string informer = Request.Form["chooseInformerId"];
        string approverData = Request.Form["approverData"];
        string uploadFileUrls = Request.Form["uploadFileUrls"];
        string docCode = Request.Form["docCode"];
        string project = Request.Form["project"];
        string isOverBudget = Request.Form["isOverBudget"];//是否是预算外的标志，1表示预算外，0表示预算内
        string isPrepaid = Request.Form["isPrepaid"];
        string isHasReceipt = Request.Form["isHasReceipt"];
        string reimburseDetail = Request.Form["reimburseDetail"];
        string travelCode = Request.Form["travelCode"];

        List<string> informerList = JsonHelper.DeserializeJsonToList<string>(informer);
        List<JObject> approverDataList = JsonHelper.DeserializeJsonToList<JObject>(approverData);
        List<string> uploadFileUrlsList = JsonHelper.DeserializeJsonToList<string>(uploadFileUrls);
        List<JObject> reimburseDetailList = JsonHelper.DeserializeJsonToList<JObject>(reimburseDetail);

        string msg = "";
        JObject res = ReimbursementManage.IsOverBudget(fee_department, fee_detail,Convert.ToDouble(fee_amount)
            ,Convert.ToDateTime(apply_time));
        if (res==null||Convert.ToDouble(res["budget"])>=Convert.ToDouble(res["hasApprove"])+ Convert.ToDouble(fee_amount) || isOverBudget == "1" || isPrepaid == "1")
        {
            msg= MobileReimburseManage.insertMobileReimburse(apply_time, product, branch, fee_department, fee_detail, fee_amount,
            file, remark, user, approver, department, informerList, approverDataList, uploadFileUrlsList, docCode, project,isOverBudget,isPrepaid,isHasReceipt, reimburseDetailList, fee_company);           
        }
        else
        {
            string code = GenerateDocCode.getReimburseCode();

            // 差旅申请表关联移动报销编号
            string sql = string.Format("update wf_form_差旅申请 set reimburseCode = '{0}' where '{1}' like concat('%', docCode, '%')", code, travelCode);

            SqlHelper.Exce(sql);

            if (department == null || "".Equals(department))
            {
                department = MobileReimburseSrv.findDepartmentNameByWechatUserId(user.wechatUserId).Tables[0].Rows[0][0].ToString();
            }
            if (string.IsNullOrEmpty(docCode))
            {
                MobileReimburseSrv.insertMobileReimburse(code, apply_time, product, branch, fee_department, fee_detail, fee_amount,
                file, remark, user, approver, department, project, isOverBudget, isPrepaid, isHasReceipt, fee_company, "");

                SqlHelper.Exce(string.Format("update yl_reimburse set status = '草稿' where code = '{0}'", code));
            }
                
            msg = "本月预算为" + res["budget"].ToString() + ",已用" + res["hasApprove"].ToString() + ",预算不足，请减少申请金额或者走预算外报销申请,"+code;            
        }
        return msg;
    }

    public string getProcessInfo()
    {
        UserInfo user = (UserInfo)Session["user"];
        string feeDetail = Request.Form["feeDetail"];
        double feeAmount = Double.Parse(Request.Form["feeAmount"]);
        string feeDepartment = Request.Form["feeDepartment"];
        string department = Request.Form["department"];
        string isOverBudget = Request.Form["isOverBudget"];//是否是预算外的标志，1表示预算外，0表示预算内
        int feeDepartmentId = 0;
        if (feeDepartment != null && !"".Equals(feeDepartment))
        {
            feeDepartmentId = Int32.Parse(MobileReimburseManage.findDepartmentIdByName(feeDepartment).Rows[0][0].ToString());
        }

        if (feeDepartmentId == 0)
        {
            string self_wechatUserId = user.wechatUserId;

            feeDepartmentId = Int32.Parse(MobileReimburseSrv.findDepartmentByWechatUserId(self_wechatUserId).Tables[0].Rows[0][0].ToString());
        }
        
        int departmentId = 0;
        if (department != null && !"".Equals(department)) {
            departmentId = Int32.Parse(MobileReimburseManage.findDepartmentIdByName(department).Rows[0][0].ToString());
        }

        JArray jobjectList = new JArray();

        string leaders = MobileReimburseManage.confirmApprovalProcess(ref jobjectList, feeDepartmentId, feeDetail, feeAmount, user, departmentId);
        //预算外添加总经理和财务总监两级审批
        if (isOverBudget == "1")
        {
            //总经理
            JObject topManager = new JObject();
            topManager.Add("name", "吕正和");
            topManager.Add("userId", "100000142");
            jobjectList.Insert(1, topManager);
            //财务总监
            JObject chiefFinancialOfficer = new JObject();
            chiefFinancialOfficer.Add("name", "张代俊");
            chiefFinancialOfficer.Add("userId", "100000324");
            jobjectList.Insert(1, chiefFinancialOfficer);
        }

        leaders = "";
        for (int i = 0; i < jobjectList.Count; i++)
        {
            
            JObject jObjectForIndex = (JObject)jobjectList[i];

            if (i > 0)
            {
                leaders += jObjectForIndex["name"] + ",";
            }

            jObjectForIndex.Add("index", i);
        }

        JObject jObject = new JObject();
        jObject.Add("leaders", leaders);
        jObject.Add("selfName", user.userName);
        jObject.Add("approverData", jobjectList);
        return jObject.ToString();
    }

    public string uploadReimburseImage()
    {
        HttpFileCollection files = HttpContext.Current.Request.Files;
        if (files.Count == 0)
            return "Failed";
        MD5 md5Hasher = new MD5CryptoServiceProvider();
        byte[] arrbytHashValue = md5Hasher.ComputeHash(files[0].InputStream);
        string strHashData = System.BitConverter.ToString(arrbytHashValue).Replace("-", "");
        string FileEextension = Path.GetExtension(files[0].FileName);
        string uploadDate = DateTime.Now.ToString("yyyyMMdd");
        string virtualPath = string.Format("/Data/ComponentAttachments/{0}/{1}", uploadDate, files[0].FileName);
        string fullFileName = Server.MapPath(virtualPath);
        //创建文件夹，保存文件
        string path = Path.GetDirectoryName(fullFileName);
        Directory.CreateDirectory(path);
        JObject jObject = new JObject();
        if (!System.IO.File.Exists(fullFileName))
        {
            files[0].SaveAs(fullFileName);
        }
        if (files[0].ContentLength > 2 * 1024 * 1024)
        {
            jObject.Add("status", "无法上传超过2m的图片");
            Directory.Delete(fullFileName);
        }
        else
        {
            //string reducedImageUrl = ReducedImage(1024, 768, fullFileName);

            jObject.Add("status", "文件上传成功");
            jObject.Add("filePath", virtualPath);
        }

        return jObject.ToString();
    }

    public string deleteReimburseImage()
    {
        string filepath = Request.Form["filePath"];
        File.Delete(filepath);

        return null;
    }
    /// <summary>
    /// 获取文件大小
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private string GetFileSize(long bytes)
    {
        long kblength = 1024;
        long mbLength = 1024 * 1024;
        if (bytes < kblength)
            return bytes.ToString() + "B";
        if (bytes < mbLength)
            return decimal.Round(decimal.Divide(bytes, kblength), 2).ToString() + "KB";
        else
            return decimal.Round(decimal.Divide(bytes, mbLength), 2).ToString() + "MB";
    }

    /// <summary>  
    /// 按大小压缩  
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

    public string dataEchoed()
    {
        string docCode = Request.Form["docCode"];
        DataTable dt = ReimbursementManage.findByCode(docCode);

        foreach (DataRow dr in dt.Rows)
        {
            dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
        }

        // 再获取知悉人和图片
        DataTable informerDt = MobileReimburseManage.findInformerByCode(docCode).Tables[0];

        string informerId = informerDt.Rows[0]["userId"].ToString();
        string informerName = informerDt.Rows[0]["userName"].ToString();
        
        dt.Columns.Add("informerId", Type.GetType("System.String"));
        dt.Columns.Add("informerName", Type.GetType("System.String"));

        dt.Rows[0]["informerId"] = informerId;
        dt.Rows[0]["informerName"] = informerName;

        DataTable attachmentDt = MobileReimburseManage.findInformerByCode(docCode).Tables[1];

        dt.Columns.Add("attachmentUrl", Type.GetType("System.String"));

        dt.Rows[0]["attachmentUrl"] = attachmentDt.Rows[0]["url"].ToString();

        // 明细数据回调
        DataTable tempDt = SqlHelper.Find(string.Format("select * from yl_reimburse_detail where code = '{0}'", docCode)).Tables[0];

        dt.Columns.Add("reimburseDetail", Type.GetType("System.String"));

        dt.Rows[0]["reimburseDetail"] = JsonHelper.SerializeObject(tempDt);

        return JsonHelper.DataTable2Json(dt);
    }

    public string findProjectCode()
    {
        string code = Request.Form["name"];

        string sql = "SELECT id value, code target from yl_project";

        if (!string.IsNullOrEmpty(code))
        {
            sql += " where code like '%" + code + "%'";
        }

        DataSet ds = SqlHelper.Find(sql);

        if (ds != null)
        {
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                return JsonHelper.DataTable2Json(dt);
            }

            return null;
        }

        return null;
    }
    public string getAccessToken()
    {
        string url = string.Format("https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}",
               "lFdOUMTnK860z67dfgmiCKiD", "C8HIhq2fL8FMGhnz02mamcuQdr0Txrfs ");

        return HttpHelper.Get(url);
    }

    public string findTravelApply()
    {
        UserInfo user = (UserInfo)Session["user"];

        string sql = string.Format("select docCode value, concat(申请出差日期, ',',出发地和目的地,',',合计金额,'元') target from wf_form_差旅申请 where userId = '{0}' and status = '已审批' and reimburseCode is null", user.userId);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt); ;
    }

    public string findTravelApplyAmount()
    {
        string code = Request.Form["code"];

        string[] codeArray = code.Split(',');

        double amount = 0.0;
        string sql = "";

        foreach (string temp in codeArray)
        {
            if (!string.IsNullOrEmpty(temp))
                sql += string.Format("select 合计金额 from wf_form_差旅申请 where docCode = '{0}';", temp); 
        }

        DataSet ds = SqlHelper.Find(sql);

        foreach (DataTable dt in ds.Tables)
        {
            amount += double.Parse(dt.Rows[0][0].ToString());
        }

        JObject result = new JObject
        {
            { "amount", amount}
        };

        return result.ToString();
    }
}