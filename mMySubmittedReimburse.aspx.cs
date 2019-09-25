using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

public partial class mMySubmittedReimburse : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxCommon wx = new WxCommon("mMySubmittedReimburse",
            "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk",
            "1000006",
            "http://yelioa.top/mMySubmittedReimburse.aspx");
        string res = wx.CheckAndGetUserInfo(HttpContext.Current);
        string action = Request.Form["act"];
        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "getInfos")
            {
                Response.Write(getInfos());
            }
            else if (action == "getDocument")
            {
                Response.Write(getDocument());
            }
            else if (action == "getProcessInfoAndAttachment")
            {
                Response.Write(getProcessInfoAndAttachment());
            }
            else if (action == "cancel")
            {
                Response.Write(cancel());
            }
            else if (action == "DownloadExcel")
            {
                //Response.Write(DownloadExcel());
                DownloadExcel();
            }
            Response.End();
        }
    }
    private string getInfos()
    {
        string sort = Request.Form["sort"];
        string order = Request.Form["order"];
        string keyword = Request.Form["keyword"];

        string res = "F";
        UserInfo user = (UserInfo)Session["user"];
        if (user != null)
        {
            DataSet ds = ReimbursementManage.GetInfos(user.userName, keyword);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = PinYinHelper.SortByPinYin(ds.Tables[0], sort, order);

                foreach (DataRow dr in dt.Rows)
                {
                    dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
                }

                res = JsonHelper.DataTableToJsonForEasyUiDataGridLoadDataMethod(dt);
            }
        }
        return res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
    }

    private string getDocument()
    {
        string code = Request.Form["docCode"];
        UserInfo user = (UserInfo)Session["user"];
        DataSet ds = ReimbursementManage.GetDocumnetsInfosByCode(code, user.userName);
        string res = "单据未找到或无权限查看该单据！";
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                dr["remark"] = SqlHelper.DesDecrypt(dr["remark"].ToString());
            }
            res = JsonHelper.SerializeObject(dt);
        }
        return res.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
    }

    private string getProcessInfoAndAttachment()
    {
        string docCode = Request.Form["docCode"];
        DataTable dt = ReimbursementManage.getApproverByCode(docCode);

        DataTable recordDt = ReimbursementManage.getApprovalRecordCount(docCode);

        DataTable attachmentDt = ReimbursementManage.getAttachmentByCode(docCode);

        DataTable reimburseDetailDt = SqlHelper.Find(string.Format("select * from yl_reimburse_detail t1 left join yl_reimburse t2 on t1.code = t2.code where t2.id = '{0}'", docCode)).Tables[0];

        JObject jObject = new JObject
        {
            { "data", JsonHelper.DataTable2Json(dt) },
            { "count", JsonHelper.DataTable2Json(recordDt) },
            { "attachment", JsonHelper.DataTable2Json(attachmentDt) },
            { "detail", JsonHelper.DataTable2Json(reimburseDetailDt) }
        };

        if (dt == null || dt.Columns.Count == 0)
            return "";

        return jObject.ToString();
    }

    private string cancel()
    {
        string docCode = Request.Form["code"];
        UserInfo userInfo = (UserInfo)Session["user"];

        string msg = ReimbursementManage.cancel(docCode, userInfo);

        JObject jObject = new JObject();

        if (msg.Contains("操作成功"))
        {
            jObject.Add("ErrCode", "0");
        }
        else
        {
            jObject.Add("ErrCode", "-1");
        }

        return jObject.ToString();
    }

    private void DownloadExcel()
    {
        UserInfo user = (UserInfo)Session["user"];
        JObject res = new JObject();
        string date =  Request.Form["date"];
        DateTime dateStart = Convert.ToDateTime(date + "-01");
        DateTime dateEnd = dateStart.AddMonths(1);
        //string[] dates = date.Split('-');
        //int year = Convert.ToInt32(dates[0]);
        //int month = Convert.ToInt32(dates[1]);
        string msg = "";
        string sql = string.Format("select * from v_user_department_post where userId={0}\r\n;", user.userId);
        sql += string.Format("SELECT r.*, rd.* FROM yl_reimburse AS r RIGHT JOIN yl_reimburse_detail AS rd ON rd.Code LIKE concat('%', r.code, '%')" +
          " where (rd.ActivityDate between '{0}' and '{1}') and (rd.Status='同意' and r.status='已审批' and r.account_result='同意') and r.name='{2}' ORDER BY r.code ASC"
          , dateStart.ToString("yyyy-MM-dd"),dateEnd.ToString("yyyy-MM-dd"),user.userName);
        DataSet ds = SqlHelper.Find(sql, ref msg);

        if(ds == null)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", msg);
            Response.Write(res.ToString());
            return;
        }

        string fileName = "";
        byte[] data = null;
        bool IsSalesMan = false;

        foreach(DataRow row in ds.Tables[0].Rows)
        {
            if(row["department"].ToString().Contains("营销中心"))
            {
                IsSalesMan = true;
                break;
            }
        }
        //IsSalesMan = true;//测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试

        Dictionary<string, object> UserInfo = new Dictionary<string, object>();
        UserInfo.Add("user", user);
        UserInfo.Add("depart", ds.Tables[0].Rows[0]["department"].ToString());
        UserInfo.Add("dateStart", dateStart);
        UserInfo.Add("dateEnd", dateEnd);

        DataTable dtVal = null;
        if (IsSalesMan)
        {
            fileName = string.Format("营销中心费用报销新套表-{0}{1}", user.userName, dateStart.ToString("yyyyMM"));
            string path = Server.MapPath("~/Template/营销中心费用报销新套表-.xls");
            dtVal = CreateSalesmanReimbursementSlipCoverDetailValue(ds.Tables[1], ref UserInfo);
            if(dtVal.Rows.Count > 15)
            {
                data = ExcelHelperV2_0.EditSalesmanExcel(GetRowsFromDataTable(dtVal,0,14), ref msg, path, UserInfo);
                string filecode = ValideCodeHelper.GetRandomCode(64);
                string newPath = Server.MapPath("~/tempExportFile");
                newPath = newPath + @"\" + filecode + ".xls";

                BytesToFile(newPath, data);
                res.Add("ErrCode", 0);
                res.Add("FileCount", 2);
                res.Add("FileCode1", filecode);
                res.Add("FileName1", fileName);

                UserInfo.Remove("餐费");
                UserInfo.Remove("市内交通费");
                UserInfo.Remove("核销预付款");
                UserInfo.Remove("税金");

                data = ExcelHelperV2_0.EditSalesmanExcel(GetRowsFromDataTable(dtVal, 15, dtVal.Rows.Count-1), ref msg, path, UserInfo);
                filecode = ValideCodeHelper.GetRandomCode(64);
                newPath = Server.MapPath("~/tempExportFile");
                newPath = newPath + @"\" + filecode + ".xls";

                BytesToFile(newPath, data);
                res.Add("FileCode2", filecode);
                res.Add("FileName2", fileName);
            }
            else
            {
                data = ExcelHelperV2_0.EditSalesmanExcel(dtVal, ref msg, path, UserInfo);
                string filecode = ValideCodeHelper.GetRandomCode(64);
                string newPath = Server.MapPath("~/tempExportFile");
                newPath = newPath + @"\" + filecode + ".xls";

                BytesToFile(newPath, data);
                res.Add("ErrCode", 0);
                res.Add("FileCount", 1);
                res.Add("FileCode1", filecode);
                res.Add("FileName1", fileName);
            }
            
        }
        else
        {
            fileName = string.Format("职能线费用报销套表-{0}{1}", user.userName, dateStart.ToString("yyyyMM"));
            string path = Server.MapPath("~/Template/职能线费用报销套表-.xls");
            //data = ExcelHelperV2_0.EditNoSalesmanExcel(ds.Tables[1], ref msg, path, UserInfo);
            dtVal = CreateNoSalesmanReimbursementSlipCoverDetailValue(ds.Tables[1], ref UserInfo);
            if (dtVal.Rows.Count > 15)
            {
                data = ExcelHelperV2_0.EditSalesmanExcel(GetRowsFromDataTable(dtVal, 0, 14), ref msg, path, UserInfo);
                string filecode = ValideCodeHelper.GetRandomCode(64);
                string newPath = Server.MapPath("~/tempExportFile");
                newPath = newPath + @"\" + filecode + ".xls";

                BytesToFile(newPath, data);
                res.Add("ErrCode", 0);
                res.Add("FileCount", 2);
                res.Add("FileCode1", filecode);
                res.Add("FileName1", fileName);
                
                UserInfo.Remove("交通费");
                UserInfo.Remove("核销预付款");
                UserInfo.Remove("税金");

                data = ExcelHelperV2_0.EditNoSalesmanExcel(GetRowsFromDataTable(dtVal, 15, dtVal.Rows.Count - 1), ref msg, path, UserInfo);
                filecode = ValideCodeHelper.GetRandomCode(64);
                newPath = Server.MapPath("~/tempExportFile");
                newPath = newPath + @"\" + filecode + ".xls";

                BytesToFile(newPath, data);
                res.Add("FileCode2", filecode);
                res.Add("FileName2", fileName);
            }
            else
            {
                data = ExcelHelperV2_0.EditNoSalesmanExcel(dtVal, ref msg, path, UserInfo);
                string filecode = ValideCodeHelper.GetRandomCode(64);
                string newPath = Server.MapPath("~/tempExportFile");
                newPath = newPath + @"\" + filecode + ".xls";

                BytesToFile(newPath, data);
                res.Add("ErrCode", 0);
                res.Add("FileCount", 1);
                res.Add("FileCode1", filecode);
                res.Add("FileName1", fileName);
            }
        }

        if (string.IsNullOrEmpty(msg))
        {
            //Response.ContentType = "application/vnd.ms-excel";
            //Response.ContentEncoding = Encoding.UTF8;
            //Response.Charset = "";
            //Response.AppendHeader("Content-Disposition", "attachment;filename="
            //    + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
            //Response.BinaryWrite(data);
            //string filecode = ValideCodeHelper.GetRandomCode(64);
            //string path = Server.MapPath("~/tempExportFile");
            //path = path + @"\" + filecode + ".xls";
           
            //using (FileStream fs = new FileStream(path, FileMode.Create))
            //{
            //    using (BinaryWriter bw = new BinaryWriter(fs))
            //    {
            //        bw.Write(data);
            //    }
            //}
            //res.Add("ErrCode", 0);
            //res.Add("FileCode", filecode);
            //res.Add("FileName", fileName);
        }
        else
        {
            res.Add("ErrCode", 2);
            res.Add("ErrMsg", msg);
        }
        Response.Write(res.ToString());
    }

    private void BytesToFile(string path,byte[] data)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(data);
            }
        }
    }

    private DataTable GetRowsFromDataTable(DataTable dtSrc,int startRowIndex,int endRowIndex)
    {
        if (startRowIndex < 0 || endRowIndex > dtSrc.Rows.Count - 1 || startRowIndex > endRowIndex)
            return null;
        if (startRowIndex == 0 && endRowIndex == dtSrc.Rows.Count - 1)
            return dtSrc;
        DataTable dt = dtSrc.Clone();
        DataRow[] rows = dtSrc.Select("1=1");
        for(int i = startRowIndex; i <= endRowIndex; i++)
        {
            dt.ImportRow(rows[i]);
        }
        return dt;
    }

    private DataTable CreateNoSalesmanReimbursementSlipCoverDetailValue(DataTable dt, ref Dictionary<string, object> dictInfo)
    {
        DateTime dateStart = (DateTime)dictInfo["dateStart"];
        DateTime dateEnd = (DateTime)dictInfo["dateEnd"];
        DataRow[] rows = dt.Select(string.Format("fee_detail like '%岗位补贴%'"));
        double TrafficFee = 0;
        foreach (DataRow dtRow in rows)
        {
            if (dtRow["ReceiptType"].ToString() == "交通费")
            {
                TrafficFee += Convert.ToDouble(dtRow["ReceiptAmount"]);
            }
        }
        dictInfo.Add("交通费", TrafficFee);

        //填入具体单据内容
        int len = dateEnd.AddDays(-1).Day;
        double advancePayment = 0.0;//预付款
        double advancePayment2 = 0.0;//预付款
        double tax = 0.0;
        double tax2 = 0.0;
        DataTable dtVal = new DataTable();
        dtVal.Columns.Add("date", typeof(string));
        dtVal.Columns.Add("地点", typeof(string));
        dtVal.Columns.Add("出差项目编号", typeof(string));
        dtVal.Columns.Add("项目编号", typeof(string));
        dtVal.Columns.Add("出差车船费", typeof(double));
        dtVal.Columns.Add("出差补贴", typeof(double));
        dtVal.Columns.Add("住宿费", typeof(double));
        dtVal.Columns.Add("交通费", typeof(double));
        dtVal.Columns.Add("汽车使用费", typeof(double));
        dtVal.Columns.Add("业务招待费", typeof(double));
        dtVal.Columns.Add("培训费", typeof(double));
        dtVal.Columns.Add("办公费", typeof(double));
        dtVal.Columns.Add("租赁费", typeof(double));
        dtVal.Columns.Add("劳保费", typeof(double));
        dtVal.Columns.Add("通讯费", typeof(double));
        dtVal.Columns.Add("福利费", typeof(double));
        dtVal.Columns.Add("物业费", typeof(double));
        dtVal.Columns.Add("水电费", typeof(double));
        dtVal.Columns.Add("招聘费", typeof(double));
        dtVal.Columns.Add("运输费", typeof(double));
        dtVal.Columns.Add("材料费", typeof(double));
        dtVal.Columns.Add("试验费", typeof(double));
        dtVal.Columns.Add("检测费", typeof(double));
        dtVal.Columns.Add("专利费", typeof(double));
        dtVal.Columns.Add("注册费", typeof(double));
        dtVal.Columns.Add("服务费", typeof(double));
        dtVal.Columns.Add("其他", typeof(double));
        //dtVal.Columns.Add("核销预付款", typeof(double));


        //差旅费部分
        for (int i = 1; i <= len; i++)
        {
            string condition = string.Format("ActivityDate >= #{0}-{1}-{2} 00:00:00# and ActivityDate <= #{0}-{1}-{2} 23:59:59# and fee_detail like '%差旅%'"
                , dateStart.Year, dateStart.Month, i);
            rows = dt.Select(condition);
            if (rows.Length > 0)
            {
                DataRow newRow = dtVal.NewRow();
                newRow["date"] = string.Format("{0}-{1}-{2}", dateStart.Year, dateStart.Month, i);
                for (int j = 0; j < dtVal.Columns.Count; j++)
                {
                    if (typeof(double) == dtVal.Columns[j].DataType)
                    {
                        newRow[j] = 0;
                    }
                }


                foreach (DataRow dtRow in rows)
                {
                    newRow["出差项目编号"] = AddString(newRow["出差项目编号"].ToString(), 
                        GetProjectNumberFromFeeDetail(dtRow["fee_detail"].ToString()));
                    if (!string.IsNullOrEmpty(dtRow["ReceiptPlace"].ToString()))
                    {
                        newRow["地点"] = AddString(newRow["地点"].ToString(),  dtRow["ReceiptPlace"].ToString());
                    }

                    string ReceiptType = dtRow["ReceiptType"].ToString();

                    for (int j = 0; j < dtVal.Columns.Count; j++)
                    {
                        if (ReceiptType == dtVal.Columns[j].ColumnName && typeof(double) == dtVal.Columns[j].DataType)
                        {
                            if ((ReceiptType == "交通费") && dtRow["fee_detail"].ToString().Contains("岗位补贴"))
                            {

                            }
                            else
                                newRow[ReceiptType] = Convert.ToDouble(newRow[ReceiptType]) + Convert.ToDouble(dtRow["ReceiptAmount"]);
                        }
                    }

                    if (dtRow["isPrepaid"].ToString().Equals("1"))
                    {
                        if (dtVal.Rows.Count <= 15)
                            advancePayment += Convert.ToDouble(dtRow["ReceiptAmount"]);
                        else
                            advancePayment2 += Convert.ToDouble(dtRow["ReceiptAmount"]);
                    }
                    if (dtVal.Rows.Count <= 15)
                        tax += Convert.ToDouble(dtRow["ReceiptTax"]);
                    else
                        tax2 += Convert.ToDouble(dtRow["ReceiptTax"]);
                }
                dtVal.Rows.Add(newRow);
            }
        }
        //项目部分
        List<string> listProject = new List<string>();
        foreach (DataRow r in dt.Rows)
        {
            string val = GetProjectNumberFromFeeDetail(r["fee_detail"].ToString());
            if (!listProject.Contains(val))
                listProject.Add(val);
        }
        foreach (string projectNumber in listProject)
        {
            string condition = string.Format("fee_detail like '%{0}%' and fee_detail not like '%差旅%' and fee_detail not like '%岗位补贴%'", projectNumber);
            rows = dt.Select(condition);
            if (rows.Length > 0)
            {
                DataRow newRow = dtVal.NewRow();
                newRow["date"] = dateStart.ToString("yyyy年MM月");
                newRow["项目编号"] = projectNumber;
                for (int j = 0; j < dtVal.Columns.Count; j++)
                {
                    if (typeof(double) == dtVal.Columns[j].DataType)
                    {
                        newRow[j] = 0;
                    }
                }

                foreach (DataRow dtRow in rows)
                {
                    string ReceiptType = dtRow["ReceiptType"].ToString();
                    newRow[ReceiptType] = Convert.ToDouble(newRow[ReceiptType]) + Convert.ToDouble(dtRow["ReceiptAmount"]);
                    if (dtRow["isPrepaid"].ToString().Equals("1"))
                    {
                        if (dtVal.Rows.Count <= 15)
                            advancePayment += Convert.ToDouble(dtRow["ReceiptAmount"]);
                        else
                            advancePayment2 += Convert.ToDouble(dtRow["ReceiptAmount"]);
                    }
                    if (dtVal.Rows.Count <= 15)
                        tax += Convert.ToDouble(dtRow["ReceiptTax"]);
                    else
                        tax2 += Convert.ToDouble(dtRow["ReceiptTax"]);
                }
                dtVal.Rows.Add(newRow);
            }
        }

        if (advancePayment > 0)
            dictInfo.Add("核销预付款", advancePayment);
        if (advancePayment2 > 0)
            dictInfo.Add("核销预付款2", advancePayment2);
        if (tax > 0)
            dictInfo.Add("税金", tax);
        if (tax2 > 0)
            dictInfo.Add("税金2", tax2);
        return dtVal;
    }

    private DataTable CreateSalesmanReimbursementSlipCoverDetailValue(DataTable dt,ref Dictionary<string, object> dictInfo)
    {
        DateTime dateStart = (DateTime)dictInfo["dateStart"];
        DateTime dateEnd = (DateTime)dictInfo["dateEnd"];

        DataRow[] rows = dt.Select(string.Format("fee_detail like '%岗位补贴%'"));
        double MealsFee = 0;
        double TrafficFee = 0;
        foreach (DataRow dtRow in rows)
        {
            if (dtRow["ReceiptType"].ToString() == "餐费")
            {
                MealsFee += Convert.ToDouble(dtRow["ReceiptAmount"]);
            }
            else if (dtRow["ReceiptType"].ToString() == "市内交通费")
            {
                TrafficFee += Convert.ToDouble(dtRow["ReceiptAmount"]);
            }
        }
        dictInfo.Add("餐费", MealsFee);
        dictInfo.Add("市内交通费", TrafficFee);

        //填入具体单据内容
        int len = dateEnd.AddDays(-1).Day;
        double advancePayment = 0.0;//预付款
        double advancePayment2 = 0.0;//预付款
        double tax = 0.0;
        double tax2 = 0.0;
        DataTable dtVal = new DataTable();
        dtVal.Columns.Add("date", typeof(string));
        dtVal.Columns.Add("地点", typeof(string));
        dtVal.Columns.Add("出差内容描述", typeof(string));
        dtVal.Columns.Add("活动申请编号", typeof(string));
        dtVal.Columns.Add("活动内容描述", typeof(string));
        dtVal.Columns.Add("出差车船费", typeof(double));
        dtVal.Columns.Add("出差补贴", typeof(double));
        dtVal.Columns.Add("住宿费", typeof(double));
        dtVal.Columns.Add("餐费", typeof(double));
        dtVal.Columns.Add("市内交通费", typeof(double));
        dtVal.Columns.Add("会议费", typeof(double));
        dtVal.Columns.Add("培训费", typeof(double));
        dtVal.Columns.Add("办公用品", typeof(double));
        dtVal.Columns.Add("工作餐", typeof(double));
        dtVal.Columns.Add("场地费", typeof(double));
        dtVal.Columns.Add("招待餐费", typeof(double));
        dtVal.Columns.Add("纪念品", typeof(double));
        dtVal.Columns.Add("外协劳务", typeof(double));
        dtVal.Columns.Add("外部人员机票/火车票", typeof(double));
        dtVal.Columns.Add("外部人员住宿费", typeof(double));
        dtVal.Columns.Add("外部人员交通费", typeof(double));
        dtVal.Columns.Add("学术会费", typeof(double));
        dtVal.Columns.Add("营销办公费", typeof(double));
        //dtVal.Columns.Add("核销预付款", typeof(double));
        for (int i = 1; i <= len; i++)
        {
            string condition = string.Format("ActivityDate >= #{0}-{1}-{2} 00:00:00# and ActivityDate <= #{0}-{1}-{2} 23:59:59#", dateStart.Year, dateStart.Month, i);
            rows = dt.Select(condition);
            if (rows.Length > 0)
            {
                DataRow newRow = dtVal.NewRow();
                newRow["date"] = string.Format("{0}-{1}-{2}", dateStart.Year, dateStart.Month, i);
                for (int j = 0; j < dtVal.Columns.Count; j++)
                {
                    if (typeof(double) == dtVal.Columns[j].DataType)
                    {
                        newRow[j] = 0;
                    }
                }

                List<string> list = new List<string>();
                string oldReceiptDesc = "";
                foreach (DataRow dtRow in rows)
                {
                    if (!list.Contains(dtRow["id"].ToString()))
                    {
                        list.Add(dtRow["id"].ToString());

                        bool isRepeatDesc = false;

                        if (string.IsNullOrEmpty(oldReceiptDesc) || oldReceiptDesc != dtRow["receiptDesc"].ToString())
                        {
                            oldReceiptDesc = dtRow["receiptDesc"].ToString();
                            isRepeatDesc = false;
                        }
                        else
                            isRepeatDesc = true;

                        if (dtRow["project"].ToString().Equals("请选择") && !dtRow["fee_detail"].ToString().Contains("区域日常"))
                        {
                            if (!string.IsNullOrEmpty(dtRow["remark"].ToString()) && !isRepeatDesc)
                            {
                                newRow["出差内容描述"] = AddString(newRow["出差内容描述"].ToString(),dtRow["receiptDesc"].ToString());
                                //newRow["出差内容描述"] = newRow["出差内容描述"].ToString() + ",\r\n" +
                                //    SqlHelper.DesDecrypt(dtRow["remark"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dtRow["ReceiptPlace"].ToString()))
                            {
                                newRow["地点"] = AddString(newRow["地点"].ToString(), dtRow["ReceiptPlace"].ToString())  ;
                            }
                        }
                        else
                        {
                            newRow["活动申请编号"] = AddString(newRow["活动申请编号"].ToString(), dtRow["project"].ToString())   ;
                            if (!string.IsNullOrEmpty(dtRow["remark"].ToString()) && !isRepeatDesc)
                                newRow["活动内容描述"] = AddString(newRow["活动内容描述"].ToString(),
                                    dtRow["receiptDesc"].ToString())  ;
                        }
                    }
                    string ReceiptType = dtRow["ReceiptType"].ToString();
                 
                    for (int j = 0; j < dtVal.Columns.Count; j++)
                    {
                        if (ReceiptType == dtVal.Columns[j].ColumnName && typeof(double) == dtVal.Columns[j].DataType)
                        {
                            if((ReceiptType == "餐费" || ReceiptType== "市内交通费") && dtRow["fee_detail"].ToString().Contains("岗位补贴"))
                            {

                            }
                            else
                                newRow[ReceiptType] = Convert.ToDouble(newRow[ReceiptType]) + Convert.ToDouble(dtRow["ReceiptAmount"]);
                        }
                    }

                    if(dtRow["isPrepaid"].ToString().Equals("1"))
                    {
                        if(dtVal.Rows.Count<=15)
                            advancePayment += Convert.ToDouble(dtRow["ReceiptAmount"]);
                        else
                            advancePayment2 += Convert.ToDouble(dtRow["ReceiptAmount"]);
                    }
                    if (dtVal.Rows.Count <= 15)
                        tax += Convert.ToDouble(dtRow["ReceiptTax"]);
                    else
                        tax2 += Convert.ToDouble(dtRow["ReceiptTax"]);
                }
                dtVal.Rows.Add(newRow);
            }            
        }
        if (advancePayment > 0)
            dictInfo.Add("核销预付款", advancePayment);
        if (advancePayment2 > 0)
            dictInfo.Add("核销预付款2", advancePayment2);
        if (tax > 0)
            dictInfo.Add("税金", tax);
        if (tax2 > 0)
            dictInfo.Add("税金2", tax2);
        return dtVal;
    }

    private string AddString(string old,string add)
    {
        if (string.IsNullOrEmpty(old))
            return add;
        else
        {
            if (old.Contains(add))
                return old;
            else
                return old + ",\r\n" + add;
        }
            
    }

    private string GetProjectNumberFromFeeDetail(string fee)
    {
        string[] vals = fee.Split('-');
        int numberCount = StringTools.CountNumbers(fee);
        if(numberCount > 1)//带有研发项目编号
        {
            return string.Format("{0}-{1}-{2}", vals[0], vals[1], vals[2]);
        }
        else
        {
            if (vals[0] == "研发费用金额")
                return "天津研发费用金额";
            else
                return vals[0];
        }
    }
}