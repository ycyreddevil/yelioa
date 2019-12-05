using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Image = System.Drawing.Image;

public partial class mFinanceReimburseDetail : System.Web.UI.Page
{
    public UserInfo userInfo;

    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mFinanceReimburseDetail",
           "v5afj_CYpboe-JWNOrCU0Cy-xP5krFq6cWYM9KZfe4o",
           "1000020",
           "http://yelioa.top/mFinanceReimburseDetail.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        string action = Request.Params["action"];

        userInfo = (UserInfo)Session["user"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getSelfReimburseList")
            {
                Response.Write(getSelfReimburseList());
            }
            else if (action == "getSalesOrNotSales")
            {
                Response.Write(getSalesOrNotSales());
            }
            else if (action == "uploadFile")
            {
                Response.Write(uploadFile());
            }
            else if (action == "submit")
            {
                Response.Write(submit());
            }
            else if (action == "draft")
            {
                Response.Write(draft());
            }
            else if (action == "getReSubmitData")
            {
                Response.Write(getReSubmitData());
            }
            else if (action == "getDraftData")
            {
                Response.Write(getDraftData());
            }
            Response.End();
        }
    }

    private string getSelfReimburseList()
    {
        UserInfo user = (UserInfo)Session["user"];
        string batchNo = Request.Form["batchNo"];

        
        string sql = string.Format("select * from yl_reimburse where name = '{0}' and remain_fee_amount > 0 and " +
        "approval_time is not null and status = '已审批' and fee_detail != 'VIP维护' and account_result is null", user.userName);

        if (!string.IsNullOrEmpty(batchNo))
        {
            sql = string.Format("select t1.* from yl_reimburse t1 left join yl_reimburse_detail t2 on t2.code like concat(%,t1.code,%) " +
                " where t2.batchNo = '{1}' t1.name = '{0}' and t1.remain_fee_amount > 0 and " +
            "t1.approval_time is not null and t1.status = '已审批' and t1.fee_detail != 'VIP维护' and account_result is null", user.userName, batchNo);
        }

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        if (dt != null && dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
            }
        }

        return JsonHelper.DataTable2Json(dt);
    }

    private string getSalesOrNotSales()
    {
        UserInfo user = (UserInfo)Session["user"];

        string sql = string.Format("select department from v_user_department_post where userId = '{0}'", user.userId);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        string departmentName = "";

        foreach (DataRow dr in dt.Rows)
        {
            departmentName += dr[0].ToString() + ",";
        }

        JObject jobject = new JObject
        {
            { "name", departmentName },
        };

        return jobject.ToString();
    }

    private string uploadFile()
    {
        List<string> filesList = JsonHelper.DeserializeJsonToList<string>(Request.Params["files"]);
        List<string> fileNamesList = JsonHelper.DeserializeJsonToList<string>(Request.Params["fileNames"]);

        JArray jarray = new JArray();

        for (var index = 0; index < filesList.Count; index++)
        {
            var filePath = Server.MapPath(Path.Combine("/Data/ComponentAttachments", DateTime.Now.ToString("yyyyMMdd")));

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var match = Regex.Match(filesList[index].ToString(), "data:image/jpeg;base64,([\\w\\W]*)$");

            if (filesList[index].ToString().Contains("data:image/png;base64,"))
                match = Regex.Match(filesList[index].ToString(), "data:image/png;base64,([\\w\\W]*)$");

            if (!match.Success)
                continue;

            var photoBytes = Convert.FromBase64String(match.Groups[1].Value);
            var _filePath = Path.Combine(filePath, fileNamesList[index].ToString());
            File.WriteAllBytes(_filePath, photoBytes);
            string originBase64 = match.Groups[1].Value;

            // 获取百度云access_token
            //string url = string.Format("https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}",
            //   "lFdOUMTnK860z67dfgmiCKiD", "C8HIhq2fL8FMGhnz02mamcuQdr0Txrfs ");

            //string access_token = JsonHelper.DeserializeJsonToObject<JObject>(HttpHelper.Get(url))["access_token"].ToString();

            //识别发票
            //url = string.Format("https://aip.baidubce.com/rest/2.0/solution/v1/iocr/recognise/finance?access_token={0}&classifierId=1", access_token);

            double timeStamp = ConvertToUnixTimestamp(DateTime.Now);
            string token = CalculateMD5Hash("5d883630+"+ timeStamp + "+3dcae883c9b3fec8b114c9c89493f85a");

            ////压缩图片的存储路径
            string reducedPath = _filePath;

            if (photoBytes.Length > 1 * 1024 * 1024)
            {
                reducedPath = _filePath.Substring(0, _filePath.LastIndexOf("\\") + 1) + Guid.NewGuid().ToString() + ".jpg";
                CompressImage(_filePath, reducedPath, 80, 300, false);
            }

            //// 获取压缩图片的base64编码
            string reducedBase64 = ImgToBase64String(reducedPath);

            // 旋转图片
            //originBase64 = OrientationImage(access_token, originBase64, Base64StringToImage(originBase64, _filePath));

            //Base64StringToImage(originBase64, _filePath);

            NameValueCollection c = new NameValueCollection
            {
                { "app_key", "5d883630" },
                { "timestamp", timeStamp.ToString() },
                { "token", token },
                { "image_data", reducedBase64 }
            };

            string url = string.Format("https://fapiao.glority.cn/v1/item/get_multiple_items_info");

            var result = HttpHelper.Post(url, c);

            Dictionary<string, object> dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(result);

            if (dict["result"].ToString() == "0")
            {
                return JsonHelper.SerializeObject(dict);
            }

            string tempStr = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(dict["response"].ToString())["data"].ToString();

            dict = JsonHelper.DeserializeJsonToObject<Dictionary<string, object>>(tempStr);

            List<JObject> jObjectList = JsonHelper.DeserializeJsonToList<JObject>(dict["identify_results"].ToString());

            for (int i = 0; i < jObjectList.Count; i ++)
            {

                // 加入裁剪图
                string region = jObjectList[i]["region"].ToString();
                string orientation = jObjectList[i]["orientation"].ToString();

                List<string> regionList = JsonHelper.DeserializeJsonToList<string>(region);

                int x0 = int.Parse(regionList[0]);
                int y0 = int.Parse(regionList[1]);
                int x1 = int.Parse(regionList[2]);
                int y1 = int.Parse(regionList[3]);

                string newPath = cutPicture(reducedPath, x0, y0, x1 - x0, y1 - y0, i);

                if (orientation != "0")
                {
                    reducedBase64 = OrientationImage(new Bitmap(newPath), orientation);
                    Base64StringToImage(reducedBase64, newPath);
                }

                newPath = newPath.Replace("\\", "/");
                jObjectList[i].Add("url", newPath.Substring(newPath.IndexOf("/Data/ComponentAttachments/")));

                jarray.Add(jObjectList[i]);
            }

            WipeFile(_filePath, 1);
        }

        return jarray.ToString();
    }

    /// <summary>
    /// 文件彻底删除
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="timesToWrite"></param>
    public void WipeFile(string filename, int timesToWrite)
    {
        try
        {
            if (File.Exists(filename))
            {
                //设置文件的属性为正常，这是为了防止文件是只读 
                File.SetAttributes(filename, FileAttributes.Normal);
                //计算扇区数目 
                double sectors = Math.Ceiling(new FileInfo(filename).Length / 512.0);
                // 创建一个同样大小的虚拟缓存 
                byte[] dummyBuffer = new byte[512];
                // 创建一个加密随机数目生成器 
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                // 打开这个文件的FileStream 
                FileStream inputStream = new FileStream(filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                for (int currentPass = 0; currentPass < timesToWrite; currentPass++)
                {
                    // 文件流位置 
                    inputStream.Position = 0;
                    //循环所有的扇区 
                    for (int sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                    {
                        //把垃圾数据填充到流中 
                        rng.GetBytes(dummyBuffer);
                        // 写入文件流中 
                        inputStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                    }
                }
                // 清空文件 
                inputStream.SetLength(0);
                // 关闭文件流 
                inputStream.Close();
                // 清空原始日期需要 
                DateTime dt = new DateTime(2037, 1, 1, 0, 0, 0);
                File.SetCreationTime(filename, dt);
                File.SetLastAccessTime(filename, dt);
                File.SetLastWriteTime(filename, dt);
                // 删除文件 
                File.Delete(filename);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
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

    /// <summary>
    /// 无损压缩图片
    /// </summary>
    /// <param name="sFile">原图片地址</param>
    /// <param name="dFile">压缩后保存图片地址</param>
    /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
    /// <param name="size">压缩后图片的最大大小</param>
    /// <param name="sfsc">是否是第一次调用</param>
    /// <returns></returns>
    public static bool CompressImage(string sFile, string dFile, int flag, int size, bool sfsc)
    {
        Image iSource = Image.FromFile(sFile);
        ImageFormat tFormat = iSource.RawFormat;
        //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
        FileInfo firstFileInfo = new FileInfo(sFile);
        if (sfsc == true && firstFileInfo.Length < size * 1024)
        {
            firstFileInfo.CopyTo(dFile);
            return true;
        }

        int dHeight = iSource.Height / 2;
        int dWidth = iSource.Width / 2;
        int sW = 0, sH = 0;
        //按比例缩放
        Size tem_size = new Size(iSource.Width, iSource.Height);
        if (tem_size.Width > dHeight || tem_size.Width > dWidth)
        {
            if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
            {
                sW = dWidth;
                sH = (dWidth * tem_size.Height) / tem_size.Width;
            }
            else
            {
                sH = dHeight;
                sW = (tem_size.Width * dHeight) / tem_size.Height;
            }
        }
        else
        {
            sW = tem_size.Width;
            sH = tem_size.Height;
        }

        Bitmap ob = new Bitmap(dWidth, dHeight);
        Graphics g = Graphics.FromImage(ob);

        g.Clear(Color.WhiteSmoke);
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

        g.Dispose();

        //以下代码为保存图片时，设置压缩质量
        EncoderParameters ep = new EncoderParameters();
        long[] qy = new long[1];
        qy[0] = flag;//设置压缩的比例1-100
        EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
        ep.Param[0] = eParam;

        try
        {
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICIinfo = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICIinfo = arrayICI[x];
                    break;
                }
            }
            if (jpegICIinfo != null)
            {
                ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                FileInfo fi = new FileInfo(dFile);
                if (fi.Length > 1024 * size)
                {
                    flag = flag - 10;
                    CompressImage(sFile, dFile, flag, size, false);
                }
            }
            else
            {
                ob.Save(dFile, tFormat);
            }
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            iSource.Dispose();
            ob.Dispose();
        }
    }

    public static string ImgToBase64String(string Imagefilename)
    {
        try
        {
            Bitmap bmp = new Bitmap(Imagefilename);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public Bitmap Base64StringToImage(string strbase64, string url)
    {
        try
        {
            byte[] arr = Convert.FromBase64String(strbase64);
            MemoryStream ms = new MemoryStream(arr);
            Bitmap bmp = new Bitmap(ms);

            bmp.Save(@url, ImageFormat.Jpeg);
            ms.Close();
            return bmp;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static string OrientationImage(Bitmap bmp, string direction)
    {
        Bitmap dstBmp = null;
        BitmapData srcBmpData = null;
        BitmapData dstBmpData = null;

        if (direction == "90")
        {
            // 逆时针90度
            bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
        }
        else if (direction == "180")
        {
            // 逆时针180度
            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }
        else if (direction == "270")
        {
            // 逆时针270度
            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        MemoryStream ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Jpeg);
        byte[] arr = new byte[ms.Length];
        ms.Position = 0;
        ms.Read(arr, 0, (int)ms.Length);
        ms.Close();
        return Convert.ToBase64String(arr);
    }

    private string draft()
    {
        List<string> codeList = JsonHelper.DeserializeJsonToList<string>(Request.Params["code"]);
        List<JObject> receiptList = JsonHelper.DeserializeJsonToList<JObject>(Request.Params["receipt"]);
        string batchNo = Request.Form["batchNo"].ToString();
        string receiptDesc = Request.Form["receiptDesc"].ToString();

        JObject result = new JObject(); // 返回结果jobject

        string codeString = "";
        foreach (string code in codeList)
        {
            codeString += code + ",";
        }

        // 生成批次号
        if (string.IsNullOrEmpty(batchNo))
            batchNo = Guid.NewGuid().ToString("N");
        else
        {
            string tempSql = string.Format("select status from yl_reimburse_detail where batchNo = '{0}' limit 1", batchNo);

            DataTable dt = SqlHelper.Find(tempSql).Tables[0];

            if (dt.Rows.Count > 0 && "已提交".Equals(dt.Rows[0][0].ToString()))
            {
                result.Add("code", 200);
                result.Add("msg", "操作成功");

                return result.ToString();
            }
        }

        foreach (JObject temp in receiptList)
        {
            temp["code"] = codeString;
            temp["status"] = "草稿";
            temp["createTime"] = DateTime.Now;
            temp["batchNo"] = batchNo;
            temp["receiptDesc"] = receiptDesc;
            temp["submitterId"] = userInfo.userId.ToString();
        }

        string sql = string.Format("delete from yl_reimburse_detail where submitterId = '{0}' and status = '草稿';", userInfo.userId.ToString());
        sql += SqlHelper.GetInsertString(receiptList, "yl_reimburse_detail");

        string msg = SqlHelper.Exce(sql);

        if (msg.Contains("操作成功"))
        {
            result.Add("code", 200);
            result.Add("msg", "操作成功");
        }
        else
        {
            result.Add("code", 500);
            result.Add("msg", msg);
        }

        return result.ToString();
    }

    private string submit()
    {
        List<string> codeList = JsonHelper.DeserializeJsonToList<string>(Request.Params["code"]);
        List<JObject> receiptList = JsonHelper.DeserializeJsonToList<JObject>(Request.Params["receipt"]);
        string batchNo = Request.Form["batchNo"].ToString();
        string receiptDesc = Request.Form["receiptDesc"].ToString();

        JObject result = new JObject(); // 返回结果jobject

        string codeString = "";
        foreach (string code in codeList)
        {
            codeString += code + ",";
        }

        double totalReceiptAmount = 0.0;

        Boolean isReSubmit = false;
        // 生成批次号
        if (string.IsNullOrEmpty(batchNo))
            batchNo = Guid.NewGuid().ToString("N");
        else
            isReSubmit = true;

        foreach (JObject temp in receiptList)
        {
            temp["code"] = codeString;
            temp["status"] = "已提交";
            temp["createTime"] = DateTime.Now;
            temp["batchNo"] = batchNo;
            temp["receiptDesc"] = receiptDesc;
            temp["submitterId"] = userInfo.userId.ToString();

            DataTable dt = new DataTable();

            // 发票公司税号校验
            if (!string.IsNullOrEmpty(temp["sellerRegisterNum"].ToString()))
            {
                string sellerRegisterNum = temp["sellerRegisterNum"].ToString();

                string checkRegisterNumSql = string.Format("select 1 from company_register_num t1 left join yl_reimburse t2 on t1.companyName = t2.fee_company " +
                    "where t1.registerNum = '{0}' and t2.code = '{1}'", sellerRegisterNum, codeString.Split(',')[0]);

                DataTable checkRegisterNumTable = SqlHelper.Find(checkRegisterNumSql).Tables[0];

                if (checkRegisterNumTable.Rows.Count == 0)
                {
                    result.Add("code", 400);
                    result.Add("msg", string.Format("发票税号:{0},与所选公司不匹配！", sellerRegisterNum));

                    return result.ToString();
                }
            }

            // 发票查重
            if (temp["receiptCode"].ToString() != "" && temp["receiptCode"].ToString() != "")
            {
                string checkDuplicateSql = string.Format("select 1 from yl_reimburse_detail where receiptCode = '{0}' and receiptNum = '{1}' and status != '拒绝' and status != '草稿'", temp["receiptCode"], temp["receiptNum"]);

                DataTable checkDuplicateDt = SqlHelper.Find(checkDuplicateSql).Tables[0];

                if (checkDuplicateDt.Rows.Count > 0)
                {
                    result.Add("code", 400);
                    result.Add("msg", string.Format("发票代码:{0},发票号码:{1},此发票重复使用！", temp["receiptCode"], temp["receiptNum"]));

                    return result.ToString();
                }
            }

            // 发票验真伪
            if (temp["feeType"] != null && (temp["feeType"].ToString() == "出租车票" || temp["feeType"].ToString() == "通用机打发票" 
                || temp["feeType"].ToString() == "定额发票" || temp["feeType"].ToString() == "汽车票") && temp["receiptNum"].ToString() != temp["receiptCode"].ToString())
            {
                string province = temp["receiptPlace"].ToString();

                if (!province.Contains("北京") && !province.Contains("上海") && !province.Contains("天津") && !province.Contains("重庆"))
                    province = province.Substring(0, province.IndexOf("省"));

                string validateMsg = "";

                if (province == "江西")
                {
                    //validateMsg = ReceiptValidate.JxHandWriting(temp["receiptNum"].ToString(), temp["receiptCode"].ToString());

                    //JObject tempJObject = JObject.Parse(validateMsg);

                    //if (tempJObject.Property("LX") == null || tempJObject.Property("LX").ToString() == "")
                    //{
                    //    result.Add("code", 400);
                    //    result.Add("msg", string.Format("发票代码:{0},发票号码:{1},此发票与税务机关信息不符,请确定发票代码和发票号码后重新提交！", temp["receiptCode"], temp["receiptNum"]));

                    //    return result.ToString();
                    //}
                }
                else if (province == "湖南")
                {
                    validateMsg = ReceiptValidate.HnQuotaInvoice(temp["receiptCode"].ToString(), temp["receiptNum"].ToString());

                    JObject tempJObject = JObject.Parse(validateMsg);

                    if (tempJObject["status"].ToString() == "0")
                    {
                        result.Add("code", 400);
                        result.Add("msg", string.Format("发票代码:{0},发票号码:{1},此发票与税务机关信息不符,请确定发票代码和发票号码后重新提交！", temp["receiptCode"], temp["receiptNum"]));

                        return result.ToString();
                    }
                }
            }

            // 费用标准控制
            UserInfo user = (UserInfo)Session["user"];

            if (user.userName != "李茂龙")
            {
                string temp_sql = string.Format("select post from users where userId = '{0}';", user.userId);

                string relativePerson = temp["relativePerson"].ToString();
                temp_sql += string.Format("select post from users where userName = '{0}';", relativePerson);
                temp_sql += string.Format("select department from v_user_department_post where userId = '{0}'", user.userId);

                DataSet ds = SqlHelper.Find(temp_sql);

                dt = ds.Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    string startTm = temp["activityDate"].ToString();
                    string endTm = string.IsNullOrEmpty(temp["activityEndDate"].ToString()) ? temp["activityDate"].ToString() : temp["activityEndDate"].ToString();
                    string post = dt.Rows[0][0].ToString();
                    double receiptAmount = double.Parse(temp["receiptAmount"].ToString());

                    DateTime d1 = Convert.ToDateTime(endTm);
                    DateTime d2 = Convert.ToDateTime(startTm);

                    DateTime d3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d1.Year, d1.Month, d1.Day));
                    DateTime d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d2.Year, d2.Month, d2.Day));

                    double interval_time = (d3 - d4).Days + 1;
                    //double interval_time = Convert.ToDateTime(endTm).Subtract(Convert.ToDateTime(startTm)).Days + 1;    ///// 差旅费算头算尾

                    int endHour = Convert.ToDateTime(endTm).Hour;
                    int startHour = Convert.ToDateTime(startTm).Hour;

                    if (endHour <= 12)
                    {
                        interval_time -= 0.5;
                    }
                    if (startHour >= 12 && startHour <= 18)
                    {
                        interval_time -= 0.5;
                    }
                    else if (startHour > 18)
                    {
                        interval_time -= 1;
                    }

                    // 此处实报实销和出差补贴是反的 方便页面显示
                    if (temp["receiptType"].ToString() == "实报实销")
                    {
                        dt = ds.Tables[2];

                        string userDepartmentName = dt.Rows[0][0].ToString();

                        if (userDepartmentName.Contains("营销中心"))
                        {
                            if (!post.Contains("经理") && !post.Contains("总监") && !post.Contains("副总经理") && !post.Contains("总经理"))
                            {
                                if (receiptAmount > 80 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "出差补贴不能超过一天80");

                                    return result.ToString();
                                }
                            }
                            else
                            {
                                if (receiptAmount > 150 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "出差补贴不能超过一天150");

                                    return result.ToString();
                                }
                            }
                        }
                        else
                        {
                            if (receiptAmount > 80 * interval_time)
                            {
                                result.Add("code", 400);
                                result.Add("msg", "出差补贴不能超过一天80");

                                return result.ToString();
                            }
                        }
                    }
                    else if (temp["receiptType"].ToString() == "住宿费")
                    {
                        // 比较提交人的岗位和同行人的岗位 按大的计算
                        dt = ds.Tables[1];

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            string relativePost = dt.Rows[0][0].ToString();

                            if (relativePost == "总经理")
                                post = "总经理";
                            else if (relativePost == "副总经理")
                            {
                                if (post != "总经理")
                                    post = relativePost;
                            }
                            else if (relativePost.Contains("总监"))
                            {
                                if (post != "总经理" && post != "副总经理")
                                    post = relativePost;
                            }
                            else if (relativePost.Contains("经理"))
                            {
                                if (post != "总经理" && post != "副总经理" && !post.Contains("总监"))
                                    post = relativePost;
                            }
                            else if (relativePost.Contains("主管"))
                            {
                                if (post != "总经理" && post != "副总经理" && !post.Contains("总监") && !post.Contains("经理"))
                                    post = relativePost;
                            }
                            else
                            {
                                post = relativePost;
                            }
                        }

                        // 住宿费算头不算尾
                        //interval_time = Convert.ToDateTime(endTm).Subtract(Convert.ToDateTime(startTm)).Days;

                        d1 = Convert.ToDateTime(endTm);
                        d2 = Convert.ToDateTime(startTm);

                        d3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d1.Year, d1.Month, d1.Day));
                        d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d2.Year, d2.Month, d2.Day));

                        interval_time = (d3 - d4).Days;

                        if (post.Contains("总经理"))
                        {

                        }
                        else if (post.Contains("副总经理"))
                        {
                            if (checkCityLevel(temp["receiptPlace"].ToString()) == "1" || checkCityLevel(temp["receiptPlace"].ToString()) == "2")
                            {
                                if (receiptAmount > 360 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚360");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "3" || checkCityLevel(temp["receiptPlace"].ToString()) == "4")
                            {
                                if (receiptAmount > 280 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚280");

                                    return result.ToString();
                                }
                            }
                        }
                        else if (post.Contains("总监"))
                        {
                            if (checkCityLevel(temp["receiptPlace"].ToString()) == "1" || checkCityLevel(temp["receiptPlace"].ToString()) == "2")
                            {
                                if (receiptAmount > 280 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚280");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "3" || checkCityLevel(temp["receiptPlace"].ToString()) == "4")
                            {
                                if (receiptAmount > 220 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚220");

                                    return result.ToString();
                                }
                            }
                        }
                        else if (post.Contains("经理"))
                        {
                            if (checkCityLevel(temp["receiptPlace"].ToString()) == "1")
                            {
                                if (receiptAmount > 220 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚220");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "2")
                            {
                                if (receiptAmount > 180 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚180");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "3")
                            {
                                if (receiptAmount > 150 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚150");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "4")
                            {
                                if (receiptAmount > 120 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚120");

                                    return result.ToString();
                                }
                            }
                        }
                        else if (post.Contains("主管"))
                        {
                            if (checkCityLevel(temp["receiptPlace"].ToString()) == "1")
                            {
                                if (receiptAmount > 200 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚200");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "2")
                            {
                                if (receiptAmount > 160 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚160");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "3")
                            {
                                if (receiptAmount > 130 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚130");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "4")
                            {
                                if (receiptAmount > 110 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚110");

                                    return result.ToString();
                                }
                            }
                        }
                        else
                        {
                            if (checkCityLevel(temp["receiptPlace"].ToString()) == "1")
                            {
                                if (receiptAmount > 180 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚180");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "2")
                            {
                                if (receiptAmount > 150 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚150");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "3")
                            {
                                if (receiptAmount > 120 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚120");

                                    return result.ToString();
                                }
                            }
                            else if (checkCityLevel(temp["receiptPlace"].ToString()) == "4")
                            {
                                if (receiptAmount > 100 * interval_time)
                                {
                                    result.Add("code", 400);
                                    result.Add("msg", "住宿费不能超过一晚100");

                                    return result.ToString();
                                }
                            }
                        }
                    }
                }
            }

            // 计算税率
            if (temp["receiptPerson"].ToString() != "" && temp["receiptPerson"].ToString() != "无")
            {
                dt = SqlHelper.Find(string.Format("select 1 from users where userName = '{0}'", temp["receiptPerson"].ToString())).Tables[0];

                if (dt.Rows.Count > 0 && (temp["receiptType"].ToString().Equals("出差车船费") || temp["receiptType"].ToString().Equals("培训费")))
                {
                    if (temp["feeType"].ToString().Equals("火车票"))
                    {
                        temp["receiptTax"] = Math.Round(double.Parse(temp["receiptAmount"].ToString()) / 1.09 * 0.09, 3);
                    }
                    else if (temp["feeType"].ToString().Equals("飞机票"))
                    {
                        temp["receiptTax"] = Math.Round((double.Parse(temp["receiptAmount"].ToString())-50) / 1.09 * 0.09, 3);
                    }
                    else if (temp["feeType"].ToString().Equals("汽车票"))
                    {
                        temp["receiptTax"] = Math.Round(double.Parse(temp["receiptAmount"].ToString()) / 1.03 * 0.03, 3);
                    }
                }
                if (dt.Rows.Count == 0)
                {
                    if (temp["receiptType"].ToString().Equals("出差车船费") || temp["receiptType"].ToString().Equals("住宿费") || temp["receiptType"].ToString().Equals("出差补贴"))
                    {
                        result.Add("code", 400);
                        result.Add("msg", "外部人员不能报销差旅费");

                        return result.ToString();
                    }
                }
            }

            totalReceiptAmount += double.Parse(temp["receiptAmount"].ToString());
        }

        string sql = "";

        if (isReSubmit)
        {
            // 如果是被拒绝后重新提交 则删除之前记录再新增
            sql += string.Format("delete from yl_reimburse_detail where batchNo = '{0}';", batchNo);
        }

        sql += SqlHelper.GetInsertString(receiptList, "yl_reimburse_detail");

        string msg = SqlHelper.Exce(sql);

        if (msg.Contains("操作成功"))
        {
            result.Add("code", 200);
            result.Add("msg", batchNo);

            // 更新单据可用额度
            foreach (string code in codeList)
            {
                double remain_fee_amount = double.Parse(SqlHelper.Find(string.Format("select remain_fee_amount from yl_reimburse where code = '{0}'", code)).Tables[0].Rows[0][0].ToString());
                string tempSql = "";
                if (remain_fee_amount >= totalReceiptAmount)
                {
                    tempSql = string.Format("update yl_reimburse set remain_fee_amount = {0} where code = '{1}';", (remain_fee_amount - totalReceiptAmount), code);
                    tempSql += string.Format("delete from yl_reimburse_detail_relevance where batchNo = '{0}' and reimburseCode = '{1}';", batchNo, code);
                    tempSql += string.Format("insert into yl_reimburse_detail_relevance (batchNo,reimburseCode,amount) values ('{0}','{1}',{2});", batchNo, code, totalReceiptAmount);
                    SqlHelper.Exce(tempSql);
                    break;
                }
                else
                {
                    tempSql = string.Format("update yl_reimburse set remain_fee_amount = {0} where code = '{1}';", 0, code);
                    tempSql += string.Format("delete from yl_reimburse_detail_relevance where batchNo = '{0}' and reimburseCode = '{1}';", batchNo, code);
                    tempSql += string.Format("insert into yl_reimburse_detail_relevance (batchNo,reimburseCode,amount) values ('{0}','{1}',{2});", batchNo, code, remain_fee_amount);
                    totalReceiptAmount -= remain_fee_amount;
                    SqlHelper.Exce(tempSql);
                }
            }

            UserInfo user = (UserInfo)Session["user"];
            // 发送审批消息给财务进行审批
            WxNetSalesHelper wxNetSalesHelper = new WxNetSalesHelper("v5afj_CYpboe-JWNOrCU0Cy-xP5krFq6cWYM9KZfe4o", "发票上报", "1000020");
            //// 给待审批人发送消息 
            wxNetSalesHelper.GetJsonAndSendWxMsg("18100661|19080720", "请及时审批 提交人为:" + user.userName
                + "的发票,谢谢!", "http://yelioa.top//mApprovalReimburseDetail.aspx", "1000020");

            //// 给提交人发送消息
            wxNetSalesHelper.GetJsonAndSendWxMsg(user.wechatUserId, "您的发票信息已提交 请耐心等待财务审批", "http://yelioa.top//mMySubmittedReimburseDetail.aspx", "1000020");
        }
        else
        {
            result.Add("code", 500);
            result.Add("msg", msg);
        }

        return result.ToString();
    }

    private string getReSubmitData()
    {
        string batchNo = Request.Form["batchNo"];

        string sql = string.Format("update yl_reimburse_detail set status = '草稿' where batchNo = '{0}';", batchNo);

        SqlHelper.Exce(sql);

        sql = string.Format("select relativePerson,receiptAttachment,feeType,receiptDate,activityDate,receiptCode,receiptAmount,receiptTax,receiptPlace," +
            "receiptNum,receiptPerson,activityEndDate,sellerRegisterNum,originAmount,receiptType from yl_reimburse_detail where batchNo = '{0}'", batchNo);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string getDraftData()
    {
        string userId = userInfo.userId.ToString();

        string sql = string.Format("select batchNo from yl_reimburse_detail where submitterId = '{0}' and status = '草稿' ", userId);

        DataTable dt = SqlHelper.Find(sql).Tables[0];

        return JsonHelper.DataTable2Json(dt);
    }

    private string checkCityLevel(string city)
    {
        if (city.Contains("北京") || city.Contains("上海") || city.Contains("广州") || city.Contains("深圳") || city.Contains("杭州"))
        {
            return "1";
        }
        else if (city.Contains("海口") || city.Contains("大连") || city.Contains("青岛") || city.Contains("苏州") || city.Contains("重庆") ||
            city.Contains("宁波") || city.Contains("温州") || city.Contains("厦门") || city.Contains("无锡") || city.Contains("天津") || 
            city.Contains("珠海") || city.Contains("济南") || city.Contains("石家庄") || city.Contains("长春") || city.Contains("哈尔滨") ||
            city.Contains("沈阳") || city.Contains("呼和浩特") || city.Contains("乌鲁木齐") || city.Contains("兰州") || city.Contains("银川") ||
            city.Contains("太原") || city.Contains("西安") || city.Contains("郑州") || city.Contains("合肥") || city.Contains("南京") ||
            city.Contains("福州") || city.Contains("南昌") || city.Contains("南宁") || city.Contains("贵阳") || city.Contains("长沙") ||
            city.Contains("武汉") || city.Contains("成都") || city.Contains("昆明") || city.Contains("拉萨") || city.Contains("西宁") ||
            city.Contains("天津"))
        {
            return "2";
        }
        else if (city.Contains("市"))
        {
            return "3";
        }
        else if (city.Contains("县"))
        {
            return "4";
        }

        return "0";
    }

    private static string CalculateMD5Hash(string input)
    {
        // step 1, calculate MD5 hash from input 
        MD5 md5 = MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string 
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString().ToLower();
    }

    private static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return Math.Floor(diff.TotalSeconds);
    }

    /// 图片裁剪，生成新图，保存在同一目录下,名字加_new，格式1.png  新图1_new.png
    /// </summary>
    /// <param name="picPath">要修改图片完整路径</param>
    /// <param name="x">修改起点x坐标</param>
    /// <param name="y">修改起点y坐标</param>
    /// <param name="width">新图宽度</param>
    /// <param name="height">新图高度</param>
    private static string cutPicture(String picPath, int x, int y, int width, int height, int index)
    {
        //图片路径
        String oldPath = picPath;
        //新图片路径
        String newPath = Path.GetExtension(oldPath);
        //计算新的文件名，在旧文件名后加_new
        newPath = oldPath.Substring(0, oldPath.Length - newPath.Length) + "_" + index + newPath;
        //定义截取矩形
        Rectangle cropArea = new Rectangle(x, y, width, height);
        //要截取的区域大小
        //加载图片
        Image img = Image.FromStream(new MemoryStream(File.ReadAllBytes(oldPath)));
        //判断超出的位置否
        if ((img.Width < x + width) || img.Height < y + height)
        {
            //MessageBox.Show("裁剪尺寸超出原有尺寸！");
            img.Dispose();
            return "";
        }
        //定义Bitmap对象
        Bitmap bmpImage = new Bitmap(img);
        //进行裁剪
        Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        //保存成新文件
        bmpCrop.Save(newPath, ImageFormat.Jpeg);
        //释放对象
        img.Dispose();
        bmpCrop.Dispose();

        return newPath;
    }
}