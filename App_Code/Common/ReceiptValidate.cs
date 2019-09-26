using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// ReceiptValidate 的摘要说明
/// </summary>
public class ReceiptValidate
{
    public ReceiptValidate()
    {
    }

    public static string JxHandWriting(string fphm, string fpdm)
    {
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
        randomT = "0." + ValideCodeHelper.CreateNumberVerifyCode(16);
        string url = "http://117.40.128.134:7002/fpcx/CxfpxxServlet?t=" + randomT;

        NameValueCollection para = new NameValueCollection();
        para.Add("fplx", "39");
        para.Add("fpdm", fpdm);
        para.Add("fphm", fphm);
        para.Add("kprq", DateTime.Now.AddMonths(-1).ToString("yyyyMMdd"));
        para.Add("kpmm", "");
        para.Add("kjje", "100");
        para.Add("yzm", vc);

        string res = HttpHelper.Post(url, para, wc);
        return res;
    }

    private static string JxHandWritingValideCodeOrc(string randomT, WebClient wc)
    {
        string url = "http://117.40.128.134:7002/fpcx//Validate?t=" + randomT;
        return BaseInvoiceValideCodeOrc(wc, url);
        //Stream stream = null;
        //MemoryStream ms = null;
        //byte[] bytes = null;
        //int bytesConter = 0;

        //try
        //{
        //    stream = wc.OpenRead(url);
        //    ms = new MemoryStream();
        //    bytes = new byte[4096];
        //    int actual = stream.Read(bytes, 0, bytes.Length);
        //    if (actual > 0)
        //    {
        //        ms.Write(bytes, 0, actual);
        //        bytesConter += actual;
        //    }
        //    ms.Position = 0;

        //}
        //catch (Exception e)
        //{
        //    throw e;
        //}
        //finally
        //{
        //    ms.Close();
        //    stream.Close();
        //}
        //string user = "yuyaoyi";
        //string psw = NetRecognizePic.MD5String("y123456");
        //string lpSoftId = "96001";
        //string lpCodeType = "1004";

        //string str = NetRecognizePic.CJY_RecognizeBytes(bytes, bytesConter, user, psw, lpSoftId, lpCodeType, "0", "0", "");
        //string strerr = GetTextByKey(str, "err_str");
        //if (strerr != "OK")
        //{
        //    return strerr;
        //}
        //else
        //    return GetTextByKey(str, "pic_str");
    }

    /// <summary>
    /// 根据关键字获取JSON数据里面的信息
    /// </summary>
    /// <param name="jsonText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string GetTextByKey(string jsonText, string key)
    {
        JObject jsonObj = JObject.Parse(jsonText);
        string str = jsonObj[key].ToString();
        return str;
    }

    private static string BaseInvoiceValideCodeOrc(WebClient wc,string url)
    {
        Stream stream = null;
        MemoryStream ms = null;
        byte[] bytes = null;
        int bytesConter = 0;

        try
        {
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
        finally
        {
            ms.Close();
            stream.Close();
        }
        string user = "yuyaoyi";
        string psw = NetRecognizePic.MD5String("y123456");
        string lpSoftId = "96001";
        string lpCodeType = "8001";

        string str = NetRecognizePic.CJY_RecognizeBytes(bytes, bytesConter, user, psw, lpSoftId, lpCodeType, "0", "0", "");
        string strerr = GetTextByKey(str, "err_str");
        if (strerr != "OK")
        {
            return strerr;
        }
        else
            return GetTextByKey(str, "pic_str");
    }


    private static string HnQuotaInvoiceValideCodeOrc(WebClient wc)
    {
        string url = "http://fpbw.hunan.chinatax.gov.cn/fpbw/imageServlet";
        return BaseInvoiceValideCodeOrc(wc, url);
    }

    public static string HnQuotaInvoice(string fpdm , string fphm)
    {
        //CookieContainer c = new CookieContainer();
        //Cookie cookie = new Cookie("JSESSIONIDFPCXQD121", "VPPRlRhifIxXE9pYTvA4pLCbOyjz1GEcc_IPowmuQx2VYB50_PWl2N-BiHBT2vQKDIw9evpl41IUvJLSdE3sM7XLFLqu9Eh9m0XTKA**");
        //cookie.Domain = "http://localhost";
        //c.Add(cookie);

        //cookie = new Cookie("AntiLeech", "2670161455");
        //cookie.Domain = "http://localhost";
        //c.Add(cookie);

        //var wc = new HttpClient(c);
        var wc = new HttpClient();

        string vc = HnQuotaInvoiceValideCodeOrc(wc);
        string url = "http://fpbw.hunan.chinatax.gov.cn/fpbw/pc/fpmxjy";

        NameValueCollection para = new NameValueCollection();
        para.Add("sjly", "39");
        para.Add("fpdm", fpdm);
        para.Add("fphm", fphm);
        para.Add("yzm", vc);

        string res = HttpHelper.Post(url, para, wc);

        //url = "http://fpbw.hunan.chinatax.gov.cn/fpbw/pc/fpmxFinal";
        ////para.Clear();
        ////para.Add("sjly", "39");
        ////para.Add("viewFlag", "0");
        ////para.Add("fpdm", fpdm);
        ////para.Add("fphm", fphm);
        ////para.Add("yzm", vc);
        //para.Add("viewFlag", "0");

        //res = HttpHelper.Post(url, para, wc);
        return res;
    }

    private static string JlQuotaInvoiceValideCodeOrc(WebClient wc)
    {
        string randomT = "0." + ValideCodeHelper.CreateNumberVerifyCode(17);
        //string url = "https://etax.jilin.chinatax.gov.cn:10812/download.sword?ctrl=CxzxFplxcxCtrl_getCheckcode&0.19244537476612233";
        string url = "https://etax.jilin.chinatax.gov.cn:10812/download.sword?ctrl=CxzxFplxcxCtrl_getCheckcode&"+randomT;
        return BaseInvoiceValideCodeOrc(wc, url);
    }

    public static string JlQuotaInvoice(string fpdm, string fphm)
    {
        CookieContainer c = new CookieContainer();
        Cookie cookie = new Cookie("UM_distinctid", "16ccba01cd517-054bfb8084beb44-5d5e490e-1fa400-16ccba01cd62d6");
        cookie.Domain = "http://localhost";
        c.Add(cookie);
        cookie = new Cookie("yfx_c_g_u_id_10003718", "_ck19082609524318055132734503131");
        cookie.Domain = "http://localhost";
        c.Add(cookie);
        cookie = new Cookie("yfx_f_l_v_t_10003718", "f_t_1566784363802__r_t_1566784363802__v_t_1566784363802__r_c_0");
        cookie.Domain = "http://localhost";
        c.Add(cookie);
        cookie = new Cookie("yfx_c_g_u_id_10003139", "ck19082614483910703332143517515");
        cookie.Domain = "http://localhost";
        c.Add(cookie);
        cookie = new Cookie("yfx_f_l_v_t_10003139", "f_t_1566802119002__r_t_1566802119002__v_t_1566802119002__r_c_0");
        cookie.Domain = "http://localhost";
        c.Add(cookie);
        cookie = new Cookie("_trs_uv", "jzs1j0g7_735_4xor");
        cookie.Domain = "http://localhost";
        c.Add(cookie);
        cookie = new Cookie("DS_SESSION", "eC3M-ysj5RqGv-YQf2EqHSFkUbxGtW9-8DUIGHNcDUO95ZolxX0e!-675258273");
        cookie.Domain = "http://localhost";
        c.Add(cookie);

        var wc = new HttpClient(c);
        //var wc = new HttpClient();

        string vc = JlQuotaInvoiceValideCodeOrc(wc);
        //string url = "https://etax.jilin.chinatax.gov.cn:10812/ajax.sword?r=0.8563231255827108&rUUID=rgqtWhpHayARq6NXTzc5DbaPbOoiki2r";
        string randomT = "0." + ValideCodeHelper.CreateNumberVerifyCode(16);
        string uid = ValideCodeHelper.GetRandomCode(32);
        string url = string.Format("https://etax.jilin.chinatax.gov.cn:10812/ajax.sword?r={0}&rUUID={1}",randomT,uid);

        JObject param = new JObject();
        param.Add("tid", "");
        param.Add("ctrl", string.Format("CxzxFplxcxCtrl_initQueryFpxxList?rUUID={0}", uid));
        param.Add("page", "");

        JArray data = new JArray();
        JObject dm = new JObject();
        dm.Add("name", "fpDm");
        dm.Add("value", fpdm);
        dm.Add("sword", "attr");
        data.Add(dm);
        JObject hm = new JObject(); 
        hm.Add("name", "fphm");
        hm.Add("value", fphm);
        hm.Add("sword", "attr");
        data.Add(hm);
        JObject cxfs = new JObject();
        cxfs.Add("name", "cxfs");
        cxfs.Add("value", "1");
        cxfs.Add("sword", "attr");
        data.Add(cxfs);

        JObject SwordForm = new JObject();
        SwordForm.Add("sword", "SwordForm");
        SwordForm.Add("name", "fplgxxcxForm");

        JObject SwordFormData = new JObject();
        JObject dmVal = new JObject();
        dmVal.Add("value", fpdm);
        SwordFormData.Add("fpDm", dmVal);
        JObject hmVal = new JObject();
        hmVal.Add("value", fphm);
        SwordFormData.Add("fphm", hmVal);
        JObject cxfsVal = new JObject();
        cxfsVal.Add("value", "1");
        SwordFormData.Add("cxfs", cxfsVal);
        JObject yzmVal = new JObject();
        yzmVal.Add("value", "1");
        SwordFormData.Add("yzm", vc);
        SwordForm.Add("data", SwordFormData);
        data.Add(SwordForm);
        param.Add("data", data);
        param.Add("bindParam", true);

        NameValueCollection para = new NameValueCollection();
        para.Add("postData", param.ToString());

        string res = HttpHelper.Post(url, para, wc);     
        return res;
    }
}