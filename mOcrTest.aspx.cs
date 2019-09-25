using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;

public partial class mOcrTest : System.Web.UI.Page
{
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

        string action = Request.Form["act"];

        //if (Common.GetApplicationValid("mFinanceReimburse.aspx") == "0" && !"uploadReimburseImage".Equals(action) && !"deleteReimburseImage".Equals(action))
        //{
        //    Response.Clear();
        //    Response.Write("<script language='javascript'>location.href='Default.aspx';</script>");
        //    Response.End();
        //    return;
        //}

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "uploadImage")
            {
                //Response.Write(AliAiInvoice());
                //Response.Write(JxHandWriting("",""));
                Response.Write(HnQuotaInvoiceOrcTest());
            }
            Response.End();
        }
    }

    private string HnQuotaInvoiceValideCodeOrcTest()
    {
        var wc = new HttpClient();
        return null;
        //return ReceiptValidate.HnQuotaInvoiceValideCodeOrc(wc);
    }

    private string HnQuotaInvoiceOrcTest()
    {
        string fpdm = Request.Form["fpdm"];
        string fphm = Request.Form["fphm"];
        return ReceiptValidate.JlQuotaInvoice(fpdm, fphm);
    }

    private string uploadImage()
    {
        //JObject res = new JObject();
        string token = "24.ca06f6e6acd341a33397a2aa5c91134f.2592000.1566457327.282335-10644534";
        string url = string.Format("https://aip.baidubce.com/rest/2.0/ocr/v1/train_ticket?access_token=" + token);
        JObject paraJson = new JObject();
        string imageBase = Request.Form["imageBase"];
        paraJson.Add("image", imageBase);
        //string para = System.Web.HttpUtility.UrlEncode(paraJson.ToString(), System.Text.Encoding.UTF8);
        //string para = string.Format("{\"image\":\"{0}\"}", imageBase);
        NameValueCollection para = new NameValueCollection();
        para.Add("image", imageBase);
        string res = HttpHelper.Post(url, para);
        return res;

        //return res.ToString();
    }

    private string HandWrite()
    {
        string token = "24.ca06f6e6acd341a33397a2aa5c91134f.2592000.1566457327.282335-10644534";
        string url = string.Format("https://aip.baidubce.com/rest/2.0/ocr/v1/webimage?access_token=" + token);
        string randomT = "0." + ValideCodeHelper.CreateNumberVerifyCode(16);
        NameValueCollection para = new NameValueCollection();
        //para.Add("url", "http://117.40.128.134:7002/fpcx//Validate?t=" + randomT);
        para.Add("image", Request.Form["imageBase"]);
        para.Add("language_type", "ENG");
        string res = HttpHelper.Post(url, para);
        return res;
    }

    private string JxHandWriting(string fphm,string fpdm)
    {
        //wc.BaseAddress = "http://117.40.128.134:7002";
        //wc.Headers.Add(HttpRequestHeader.Cookie, "JSESSIONIDFPCXQD121=QPPRlRhifIxXE9pYTvA4pLCbOyjz1GEcc_IPowmuQx2VYB50_PWl2N-BiHBT2vQKDIw9evpl41IUvJLSdE3sM7XLFLqu9Eh9m0XTKA**");
        //wc.Headers.Add(HttpRequestHeader.Cookie, "AntiLeech=2670161455");
        //wc.Headers.Add(HttpRequestHeader.Referer, "http://117.40.128.134:7002/fpcx/");
        CookieContainer c = new CookieContainer();
        Cookie cookie = new Cookie("JSESSIONIDFPCXQD121", "VPPRlRhifIxXE9pYTvA4pLCbOyjz1GEcc_IPowmuQx2VYB50_PWl2N-BiHBT2vQKDIw9evpl41IUvJLSdE3sM7XLFLqu9Eh9m0XTKA**");
        cookie.Domain = "http://localhost";
        c.Add(cookie);

        cookie = new Cookie("AntiLeech", "2670161455");
        cookie.Domain = "http://localhost";
        c.Add(cookie);

        var wc = new HttpClient(c);

        string randomT = "0." + ValideCodeHelper.CreateNumberVerifyCode(16);
        string vc = JxHandWritingValideCodeOrc(randomT, wc);
        //var aaa = HttpContext.Current.Session[""];
        randomT = "0." + ValideCodeHelper.CreateNumberVerifyCode(16);
        string url = "http://117.40.128.134:7002/fpcx/CxfpxxServlet?t=" + randomT;
        NameValueCollection para = new NameValueCollection();
        para.Add("fplx", "39");
        para.Add("fpdm", "136011915331");
        para.Add("fphm", "02031615");
        para.Add("kprq", DateTime.Now.AddMonths(-1).ToString("yyyyMMdd"));
        para.Add("kpmm", "");
        para.Add("kjje", "100");
        para.Add("yzm", vc);

        //foreach(string key in para.Keys)
        //{
        //    url += string.Format("&{0}={1}", key, para[key]);
        //}
        ////paraString = paraString.Substring(0, paraString.Length - 1);
        //string res = NetRecognizePic.PostWebRequest(url, "", Encoding.UTF8);
        string res = HttpHelper.Post(url, para, wc);
        return res;
    }

    private string JxHandWritingValideCodeOrc(string randomT, WebClient wc)
    {
        string url = "http://117.40.128.134:7002/fpcx//Validate?t=" + randomT;
        Stream stream = null;
        MemoryStream ms = null;
        byte[] bytes = null;
        int bytesConter = 0;

        try
        {
            //var wc = new WebClient { Encoding = Encoding.UTF8 };
            //wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            stream = wc.OpenRead(url);
            ms = new MemoryStream();
            bytes = new byte[4096];
            int actual = stream.Read(bytes, 0, bytes.Length);
            if (actual > 0)
            {
                ms.Write(bytes, 0, actual);
                bytesConter += actual;
            }
            ms.Position = 0;

        }
        catch (Exception e)
        {
            throw e;
        }
        //try
        //{
        //    WebRequest req = WebRequest.Create(url);
        //    stream = req.GetResponse().GetResponseStream();
        //    ms = new MemoryStream();
        //    //using (var ms = new MemoryStream())
        //    //{
        //    //    await stream..CopyToAsync(ms);
        //    //    //...
        //    //}
        //    bytes = new byte[4096];
        //    int actual = stream.Read(bytes, 0, bytes.Length);
        //    if (actual > 0)
        //    {
        //        ms.Write(bytes, 0, actual);
        //        bytesConter += actual;
        //    }
        //    ms.Position = 0;
        //    //stream.Read(bytes, 0, (int)stream.Length);
        //}
        finally
        {
            ms.Close();
            stream.Close();
        }
        string user = "yuyaoyi";
        string psw = NetRecognizePic.MD5String("y123456");
        string lpSoftId = "96001";
        string lpCodeType = "1004";

        string str = NetRecognizePic.CJY_RecognizeBytes(bytes, bytesConter, user, psw, lpSoftId, lpCodeType, "0", "0", "");
        string strerr = GetTextByKey(str, "err_str");
        if (strerr != "OK")
        {
            //this.richTextBox1.Text += "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + strerr + "\r\n";
            //MessageBox.Show(strerr);
            return strerr;
        }
        else
            return GetTextByKey(str, "pic_str");
        //string strpic_id = GetTextByKey(str, "pic_id");
        //this.txtPicID.Text = strpic_id;

        //string strpic_str = GetTextByKey(str, "pic_str");
        //this.txtshibie.Text = strpic_str;


        //this.richTextBox1.Text += "[" + DateTime.Now.ToString("HH:mm:ss") + "]识别结果:" + strpic_str + "\r\n";
    }

    /// <summary>
    /// 根据关键字获取JSON数据里面的信息
    /// </summary>
    /// <param name="jsonText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private string GetTextByKey(string jsonText, string key)
    {
        JObject jsonObj = JObject.Parse(jsonText);
        string str = jsonObj[key].ToString();
        return str;
    }


    public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    {
        return true;
    }

    private string AliAiTranTickt()
    {
        string appcode = "bf07d96228624762bc4273828ca5e6e7";
        string url = string.Format("https://ocrhcp.market.alicloudapi.com/api/predict/ocr_train_ticket");
        string imageBase = Request.Form["imageBase"];
        string bodys = "{\"image\":\"" + imageBase + "\"";       
        bodys += "}";
        return AliAiOrc(appcode, url, imageBase, bodys);
    }

    private string AliAiInvoice()
    {
        string appcode = "bf07d96228624762bc4273828ca5e6e7";
        string url = string.Format("https://ocrapi-invoice.taobao.com/ocrservice/invoice");
        string imageBase = Request.Form["imageBase"];
        string bodys = "{\"img\":\"" + imageBase + "\"";
        bodys += "}";
        return AliAiOrc(appcode, url, imageBase, bodys);
    }

    private string AliAiOrc(string appcode, string url,string imageBase, String bodys)
    {
        //string appcode = "bf07d96228624762bc4273828ca5e6e7";
        //string url = string.Format("https://ocrhcp.market.alicloudapi.com/api/predict/ocr_train_ticket");
        //string imageBase = Request.Form["imageBase"];       

        //如果输入带有inputs, 设置为True，否则设为False
        bool is_old_format = false;

        //如果没有configure字段，config设为''
        String config = "";
        //String config = "{\\\"side\\\":\\\"face\\\"}";

        String method = "POST";

        String querys = "";

        String base64 = Request.Form["imageBase"];
        //String bodys;
        //if (is_old_format)
        //{
        //    bodys = "{\"inputs\" :" +
        //                        "[{\"image\" :" +
        //                            "{\"dataType\" : 50," +
        //                             "\"dataValue\" :\"" + base64 + "\"" +
        //                             "}";
        //    if (config.Length > 0)
        //    {
        //        bodys += ",\"configure\" :" +
        //                        "{\"dataType\" : 50," +
        //                         "\"dataValue\" : \"" + config + "\"}" +
        //                         "}";
        //    }
        //    bodys += "]}";
        //}
        //else
        //{
        //    bodys = "{\"image\":\"" + base64 + "\"";
        //    if (config.Length > 0)
        //    {
        //        bodys += ",\"configure\" :\"" + config + "\"";
        //    }
        //    bodys += "}";
        //}
        HttpWebRequest httpRequest = null;
        HttpWebResponse httpResponse = null;

        if (0 < querys.Length)
        {
            url = url + "?" + querys;
        }

        if (url.Contains("https://"))
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
        }
        else
        {
            httpRequest = (HttpWebRequest)WebRequest.Create(url);
        }
        httpRequest.Method = method;
        httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
        //根据API的要求，定义相对应的Content-Type
        httpRequest.ContentType = "application/json; charset=UTF-8";
        if (0 < bodys.Length)
        {
            byte[] data = Encoding.UTF8.GetBytes(bodys);
            using (Stream stream = httpRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }
        try
        {
            httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        }
        catch (WebException ex)
        {
            httpResponse = (HttpWebResponse)ex.Response;
        }

        //JObject res = new JObject();

        if (httpResponse.StatusCode != HttpStatusCode.OK)
        {
            //Console.WriteLine("http error code: " + httpResponse.StatusCode);
            //Console.WriteLine("error in header: " + httpResponse.GetResponseHeader("X-Ca-Error-Message"));
            //Console.WriteLine("error in body: ");
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            //Console.WriteLine(reader.ReadToEnd());
            return reader.ReadToEnd();
        }
        else
        {

            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            //Console.WriteLine(reader.ReadToEnd());
            return reader.ReadToEnd();
        }
    }


}