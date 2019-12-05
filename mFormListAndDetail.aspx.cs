using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mFormListAndDetail : System.Web.UI.Page
{
    public UserInfo userInfo;
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMobileReimbursement",
           "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
           "1000006",
        "http://yelioa.top/mFormListAndDetail.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        if (res != "success")
        {
            Response.Clear();
            Response.Write("<script language='javascript'>alert('" + res + "')</script>");
            Response.End();
            return;
        }

        userInfo = (UserInfo)Session["user"];

        string action = Request.Form["act"];
        if(!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if(action=="getData")
            {
                Response.Write(GetData());
            }
            else if(action=="getDetail")
            {
                Response.Write(GetDetail());
            }
            else if(action=="back")
            {
                Response.Write(Back());
            }
            else if (action == "approve")
            {
                Response.Write(Approve());
            }
            else if (action == "downloadExcel")
            {
                DownloadExcel();
            }
            Response.End();
        }
    }

    private string GetData()
    {
        string formName = Request.Form["formName"];
        UserInfo user = (UserInfo)Session["user"];

        JObject res = new JObject();

        res.Add("toBeSubmitedByMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 1));
        res.Add("toBeApprovedByMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 2));
        res.Add("submitedByMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 3));
        res.Add("hasApprovedByMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 4));
        res.Add("relatedToMe", WorkFlowHelper.getFormDataOverall(user.userId.ToString(), 5));

        return res.ToString();
    }
    private string GetDetail()
    {
        string formName = Request.Form["formName"];
        string docId = Request.Form["docId"];
        string type = Request.Form["type"];
        return WorkFlowHelper.GetDocumentDetail(formName, docId, ((UserInfo)Session["user"]).userId.ToString(), type);
    }
    private string Back()
    {
        string formName = Request.Form["formName"];
        string docId = Request.Form["docId"];
        UserInfo user = (UserInfo)Session["user"];
        return WorkFlowHelper.BackDocument(formName, docId, user.userId.ToString());
    }

    private string Approve()
    {
        string formName = Request.Form["formName"];
        string docId = Request.Form["docId"];
        string result = Request.Form["result"];
        string opinion = Request.Form["opinion"];
        string hospitalCode = Request.Form["hospitalCode"];
        string agentCode = Request.Form["agentCode"];

        UserInfo user = (UserInfo)Session["user"];

        return WorkFlowHelper.Approving(formName, docId, user.userId.ToString(), result, opinion, hospitalCode, agentCode);
    }

    private void DownloadExcel()
    {
        JObject res = new JObject();

        string docId = Request.Form["docId"];
        string tableName = Request.Form["formName"];

        DataTable dt = SqlHelper.Find(string.Format("select * from wf_form_{0} where id = '{1}'", tableName, docId)).Tables[0];

        if (dt == null || dt.Rows.Count == 0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "");
            Response.Write(res.ToString());
            return;
        }

        // 单据内容
        DataRow dr = dt.Rows[0];
        // 单据审批人详情
        //DataTable approverDt = SqlHelper.Find(string.Format("select t1.userId, t2.userName from wf_approver t1 left join users t2 on t1.userId = t2.userId " +
        //    "where t1.tableName = '{0}' and t1.docId = '{1}'", tableName, docId)).Tables[0];
        // 单据审批记录详情
        DataTable recordDt = SqlHelper.Find(string.Format("select t1.userId, t2.userName,t1.approvalTime from wf_record t1 left join users t2 on t1.userId = t2.userId " +
            "where t1.tableName = '{0}' and t1.docId = '{1}'", tableName, docId)).Tables[0];

        string path = Server.MapPath(string.Format("~/Template/wf_form_{0}.xls", tableName));

        UserInfo user = (UserInfo)Session["user"];  // 个人信息
        string dateTime = DateTime.Now.ToString("yyyy年MM月dd日");

        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                HSSFWorkbook wbHssf = new HSSFWorkbook(fs);
                ISheet sheet = wbHssf.GetSheetAt(0);
                sheet.ForceFormulaRecalculation = true;

                IRow row = sheet.GetRow(1);
                ICell cell = row.GetCell(4);
                cell.SetCellValue(dateTime);

                row = sheet.GetRow(2);
                cell = row.GetCell(1);
                cell.SetCellValue(dr["借款单位、个人"].ToString());
                cell = row.GetCell(4);
                cell.SetCellValue(dr["付款单位"].ToString());

                row = sheet.GetRow(3);
                cell = row.GetCell(1);
                cell.SetCellValue(dr["收款户名、开户行"].ToString());
                cell = row.GetCell(4);
                cell.SetCellValue(dr["付款行"].ToString());

                row = sheet.GetRow(4);
                cell = row.GetCell(1);
                cell.SetCellValue(dr["账号"].ToString());
                cell = row.GetCell(4);
                cell.SetCellValue(dr["付款方式"].ToString());

                row = sheet.GetRow(5);
                cell = row.GetCell(1);
                cell.SetCellValue(dr["借款用途"].ToString());
                cell = row.GetCell(4);
                cell.SetCellValue(dr["还款期限"].ToString() + "之前");

                row = sheet.GetRow(6);
                cell = row.GetCell(5);
                cell.SetCellValue("￥" + dr["借款金额"].ToString());

                row = sheet.GetRow(7);
                cell = row.GetCell(1);
                // 审批流程
                string msg = string.Empty;
                foreach (DataRow recordDr in recordDt.Rows)
                {
                    msg += recordDr[1].ToString() + "->";
                }

                cell.SetCellValue(msg);

                row = sheet.GetRow(9);
                cell = row.GetCell(1);
                cell.SetCellValue(dr["备注"].ToString());

                //填完数据后，统一更新公式计算
                wbHssf.GetCreationHelper().CreateFormulaEvaluator().EvaluateAll();

                sheet.ProtectSheet("Yelioa123");    // 加密

                drawWaterRemarkPath(wbHssf, Server.MapPath("~/resources/南昌业力医学检验实验室有限公司.png"));

                byte[] data = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    wbHssf.Write(ms);
                    ms.Flush();
                    ms.Position = 0;
                    data = ms.GetBuffer();
                }
                wbHssf = null;
                sheet = null;

                string filecode = ValideCodeHelper.GetRandomCode(64);
                string newPath = Server.MapPath("~/tempExportFile");
                newPath = newPath + @"\" + filecode + ".xls";

                BytesToFile(newPath, data);
                res.Add("ErrCode", 0);
                res.Add("FileCount", 1);
                res.Add("FileCode1", filecode);
                //res.Add("FileName1", fileName);
            }
        }
        catch (Exception ex)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "暂无对应模板！");
        }

        Response.Write(res.ToString());
    }

    private void BytesToFile(string path, byte[] data)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(data);
            }
        }
    }
    public static void drawWaterRemarkPath(HSSFWorkbook wb, string waterRemarkPath)
    {
        int startXCol = 0; // 水印起始列
        int startYRow = 2; // 水印起始行
        int betweenXCol = 1; // 水印横向之间间隔多少列
        int betweenYRow = 2; // 水印纵向之间间隔多少行
        int XCount = 0; // 横向共有水印多少个
        int YCount = 0; // 纵向共有水印多少个
        int waterRemarkWidth = 0; // 水印图片宽度为多少列
        int waterRemarkHeight = 0; // 水印图片高度为多少行

        // 校验传入的水印图片格式
        if (!waterRemarkPath.EndsWith("png") && !waterRemarkPath.EndsWith("PNG")) {
            //throw new RuntimeException("向Excel上面打印水印，目前支持png格式的图片。");
        }

        // 加载图片
            FileStream fs = new FileStream(waterRemarkPath, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
            BinaryReader br = new BinaryReader(fs);
            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中

            for (int i = 0; i < wb.NumberOfSheets; i++)
            { // wb.getNumberOfSheets()
                ISheet sheet = wb.GetSheetAt(0);
                sheet.ProtectSheet("Yelioa123");
                try
                {
                    XCount = (sheet.GetRow(0).LastCellNum); 

                    if (XCount >= 0 && XCount <= 5)
                    {
                        XCount = 1;
                    }

                    if (XCount >= 5)
                    {
                        XCount = XCount / 2;
                    }

                }
                catch (Exception e)
                {
                    XCount = 1;
                }
                try
                {
                    YCount = sheet.LastRowNum / (betweenYRow); // 纵向共有水印多少个
                    if (YCount < 4)
                    {
                        YCount = 2;
                    }

                }
                catch (Exception e)
                {
                    YCount = 50;
                }

                // 开始打水印
                IDrawing drawing = sheet.CreateDrawingPatriarch();
                // 按照共需打印多少行水印进行循环
                for (int yCount = 0; yCount < YCount; yCount++)
                {
                    // 按照每行需要打印多少个水印进行循环
                    for (int xCount = 0; xCount < XCount; xCount++)
                    {
                        // 创建水印图片位置
                        int xIndexInteger = startXCol + (xCount * 2) + (xCount * betweenXCol);
                        int yIndexInteger = startYRow + (yCount * 1) + (yCount * betweenYRow);

                        /*参数定义： 第一个参数是（x轴的开始节点）； 第二个参数是（是y轴的开始节点）； 第三个参数是（是x轴的结束节点）； 第四个参数是（是y轴的结束节点）；
                            第五个参数是（是从Excel的第几列开始插入图片，从0开始计数）； 第六个参数是（是从excel的第几行开始插入图片，从0开始计数）；
                            第七个参数是（图片宽度，共多少列）； 第8个参数是（图片高度，共多少行）；
                        */
                        IClientAnchor anchor = drawing.CreateAnchor(0, 0, 0, 0, xIndexInteger, yIndexInteger,
                                xIndexInteger + waterRemarkWidth, yIndexInteger + waterRemarkHeight);
                        IPicture pic = drawing.CreatePicture(anchor,
                            wb.AddPicture(imgBytesIn, PictureType.PNG));
                        pic.Resize();
                    }
                }
            }
    }
}