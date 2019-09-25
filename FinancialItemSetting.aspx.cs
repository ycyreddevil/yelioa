using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

public partial class FinancialItemSetting : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        WxNetSalesHelper wx = new WxNetSalesHelper("http://yelioa.top/mNetSalesApproval.aspx");
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
            if ("getData".Equals(action))
            {
                Response.Write(getData());
            }
            else if ("saveData".Equals(action))
            {
                Response.Write(saveData());
            }
            else if ("getExcel".Equals(action))
            {
                Response.Write(getExcel());
            }
            else if ("importExcel".Equals(action))
            {
                Response.Write(importExcel());
            }
            else if ("getSector".Equals(action))
            {
                Response.Write(getSector());
            }
            Response.End();
        }
    }

    private string getExcel()
    {
        var oFile = HttpContext.Current.Request.Files["file"];

        if (oFile == null)
            return null;

        IWorkbook workbook = null;
        if (oFile.FileName.EndsWith(".xls"))
        {
            workbook = new HSSFWorkbook(oFile.InputStream);
        }
        else if (oFile.FileName.EndsWith(".xlsx"))
        {
            workbook = new XSSFWorkbook(oFile.InputStream);
        }
        if (workbook == null)
        {
            return null;
        }
        ISheet iSheet = workbook.GetSheetAt(0);

        if (iSheet == null)
        {
            return null;
        }

        JArray jArray = new JArray();

        // 验证excel是否是模板
        IRow firstRow = iSheet.GetRow(0);
        if (!"报销人".Equals(firstRow.GetCell(0).ToString()) || !"所属部门".Equals(firstRow.GetCell(1).ToString()) || !"报销单编号".Equals(firstRow.GetCell(2).ToString())
             || !"费用归属区域".Equals(firstRow.GetCell(3).ToString()) || !"费用明细".Equals(firstRow.GetCell(4).ToString()) || !"金额".Equals(firstRow.GetCell(5).ToString())
             || !"产生日期-开始".Equals(firstRow.GetCell(6).ToString()) || !"产生日期-结束".Equals(firstRow.GetCell(7).ToString()) || !"备注".Equals(firstRow.GetCell(8).ToString())
             || !"审核情况".Equals(firstRow.GetCell(9).ToString()))
        {
            return "excel文件有误，请使用标准格式上传";
        }

        for (int i = 1; i <= iSheet.LastRowNum; i ++)
        {
            IRow iRow = iSheet.GetRow(i);

            if (iRow == null)
            {
                return null;
            }

            JObject jObject = new JObject();

            if (iRow.GetCell(0) != null)
            {
                string name = iRow.GetCell(0).ToString();
                jObject.Add("name", name);
            }

            if (iRow.GetCell(1) != null)
            {
                string department = iRow.GetCell(1).ToString();
                jObject.Add("department", department);
            }

            if (iRow.GetCell(2) != null)
            {
                string filecode = iRow.GetCell(2).ToString();
                jObject.Add("filecode", filecode);
            }

            if (iRow.GetCell(3) != null)
            {
                string feearea = iRow.GetCell(3).ToString();
                jObject.Add("feearea", feearea);
            }

            if (iRow.GetCell(4) != null)
            {
                string feedetail = iRow.GetCell(4).ToString();
                jObject.Add("feedetail", feedetail);
            }

            if (iRow.GetCell(5) != null)
            {
                string money = iRow.GetCell(5).ToString();
                jObject.Add("money", money);
            }

            if (iRow.GetCell(6) != null)
            {
                string beginDate = iRow.GetCell(6).ToString();
                jObject.Add("beginDate", beginDate);
            }

            if (iRow.GetCell(7) != null)
            {
                string endDate = iRow.GetCell(7).ToString();
                jObject.Add("endDate", endDate);
            }

            if (iRow.GetCell(8) != null)
            {
                string remark = iRow.GetCell(8).ToString();
                jObject.Add("remark", remark);
            }

            if (iRow.GetCell(9) != null)
            {
                string status = iRow.GetCell(9).ToString();
                jObject.Add("status", status);
            }
           
            jArray.Add(jObject);
        }

        return jArray.ToString();
    }

    private string saveData()
    {
        int year = Int32.Parse(Request.Form["year"]);
        int month = Int32.Parse(Request.Form["month"]);
        float num = float.Parse(Request.Form["num"]);
        string type = Request.Form["type"];
        string sector = Request.Form["sector"];

        string itemnm = "";
        if (Int32.Parse(type) == 1)
        {
            itemnm = "FixedAssetsCost";
        }
        else if (Int32.Parse(type) == 2)
        {
            itemnm = "FinancialCost";
        }
        else if (Int32.Parse(type) == 3)
        {
            itemnm = "RdCost";
        }
        else if (Int32.Parse(type) == 4)
        {
            itemnm = "HeadOfficeManageCost";
        }
        else if (Int32.Parse(type) == 5)
        {
            itemnm = "IncomeTax";
        }
        else if (Int32.Parse(type) == 6)
        {
            itemnm = "ValueAddedTax";
        }
        else if (Int32.Parse(type) == 7)
        {
            itemnm = "AdditionalTax";
        }
        else if (Int32.Parse(type) == 8)
        {
            itemnm = "StampTax";
        }

        return ItemSettingManage.saveOrUpdateFinancialData(year, month, itemnm, num, sector);
    }

    private string getData()
    {
        string sector = Request.Form["sector"];
        // 区分是财务部还是人事部
        //UserInfo user = (UserInfo)Session["user"];
        UserInfo user = (UserInfo)Session["user"];
        DataSet ds = UserInfoSrv.GetDepartmentPostList(user);
        JArray jArray = new JArray();
        if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows[0]["departmentId"] != null)
        {
            int departmentId = Int32.Parse(ds.Tables[0].Rows[0]["departmentId"].ToString());

            DateTime time = DateTime.Now;
            DataTable table = ItemSettingManage.getData(time.Year, time.Month, sector);

            // 如果是财务部
            if (departmentId == 1)
            {
                JObject jObject = new JObject();
                jObject.Add("index", 1);
                jObject.Add("item", "固定资产分摊");
                jObject = HandleEmptyJObject(jObject, "FixedAssetsCost", table);
                jArray.Add(jObject);

                jObject = new JObject();
                jObject.Add("index", 2);
                jObject.Add("item", "财务费用金额");
                jObject = HandleEmptyJObject(jObject, "FinancialCost", table);
                jArray.Add(jObject);

                jObject = new JObject();
                jObject.Add("index", 3);
                jObject.Add("item", "研发费用金额");
                jObject = HandleEmptyJObject(jObject, "RdCost", table);
                jArray.Add(jObject);

                jObject = new JObject();
                jObject.Add("index", 4);
                jObject.Add("item", "总部管理费用");
                jObject = HandleEmptyJObject(jObject, "HeadOfficeManageCost", table);
                jArray.Add(jObject);

                jObject = new JObject();
                jObject.Add("index", 5);
                jObject.Add("item", "所得税");
                jObject = HandleEmptyJObject(jObject, "IncomeTax", table);
                jArray.Add(jObject);

                jObject = new JObject();
                jObject.Add("index", 6);
                jObject.Add("item", "增值税");
                jObject = HandleEmptyJObject(jObject, "ValueAddedTax", table);
                jArray.Add(jObject);

                jObject = new JObject();
                jObject.Add("index", 7);
                jObject.Add("item", "附加税");
                jObject = HandleEmptyJObject(jObject, "AdditionalTax", table);
                jArray.Add(jObject);

                jObject = new JObject();
                jObject.Add("index", 8);
                jObject.Add("item", "印花税");
                jObject = HandleEmptyJObject(jObject, "StampTax", table);
                jArray.Add(jObject);
            }
            // 或者是人事部
            else if (departmentId == 101)
            {
                JObject jObject = new JObject();
                jObject.Add("index", 1);
                jObject.Add("item", "工资社保金额");
                jObject.Add("num", 0);
                jArray.Add(jObject);
            }
            else
            {
                return "该用户权限不足";
            }
        }
        else
        {
            return "查询数据为空";
        }

        return jArray.ToString();
    }

    private JObject HandleEmptyJObject(JObject jObject, string itemnm, DataTable table)
    {
        if (table == null || table.Rows.Count == 0)
        {
            jObject.Add("num", "0");
        }
        else
        {
            jObject.Add("num", table.Rows[0][itemnm].ToString());
        }
        return jObject;
    }

    private string importExcel()
    {
        int year = Int32.Parse(Request.Form["year"]);
        int month = Int32.Parse(Request.Form["month"]);
        var oFile = HttpContext.Current.Request.Files["txt_file"];

        if (oFile == null)
            return null;

        IWorkbook workbook = null;
        if (oFile.FileName.EndsWith(".xls"))
        {
            workbook = new HSSFWorkbook(oFile.InputStream);
        }
        else if (oFile.FileName.EndsWith(".xlsx"))
        {
            workbook = new XSSFWorkbook(oFile.InputStream);
        }
        if (workbook == null)
        {
            return null;
        }
        ISheet iSheet = workbook.GetSheetAt(1);

        if (iSheet == null)
        {
            return null;
        }

        //JArray jArray = new JArray();
        DataTable dt = new DataTable();

        dt = createDataTable(dt);

        List<string> errorList = new List<string>();

        Dictionary<string, object> dict = new Dictionary<string, object>();

        for (int i = 1; i <= iSheet.LastRowNum; i++)
        {
            IRow iRow = iSheet.GetRow(i);

            if (iRow == null)
            {
                return null;
            }

            DataRow dr = dt.NewRow();
            string productName = iRow.GetCell(4) == null ? "" : iRow.GetCell(4).ToString();
            string specification = iRow.GetCell(5) == null ? "" : iRow.GetCell(5).ToString();
            string productCode = iRow.GetCell(3) == null ? "" : iRow.GetCell(3).ToString();

            string tempSql = "";
            if ("".Equals(specification))
            {
                tempSql = string.Format("update products set code = '{0}' where name = '{1}'", productCode, productName, specification);
            }
            else
            {
                tempSql = string.Format("update products set code = '{0}' where name = '{1}' and specification = '{2}'", productCode, productName, specification);
            }

            SqlHelper.Exce(tempSql);

            //    Boolean addFlag = true;
            //    //JObject jObject = new JObject();
            //    DataRow dr = dt.NewRow();
            //    string name = null;
            //    string filecode = null;
            //    try
            //    {
            //        name = iRow.GetCell(0).ToString();
            //        dr["name"] = name;

            //        string department = iRow.GetCell(1).ToString();
            //        dr["department"] = department;
            //        //dr.Add("department", department);

            //        filecode = iRow.GetCell(2).ToString();
            //        dr["filecode"] = filecode;
            //        //dr.Add("filecode", filecode);

            //        string feearea = iRow.GetCell(3).ToString();
            //        dr["feearea"] = feearea;
            //        //dr.Add("feearea", feearea);

            //        string feedetail = iRow.GetCell(4).ToString();
            //        dr["feedetail"] = feedetail;
            //        //dr.Add("feedetail", feedetail);

            //        string money = iRow.GetCell(5).ToString();
            //        dr["money"] = money;
            //        //dr.Add("money", money);

            //        string beginDate = iRow.GetCell(6).ToString();
            //        dr["beginDate"] = beginDate;
            //        //dr.Add("beginDate", beginDate);

            //        string endDate = iRow.GetCell(7).ToString();
            //        dr["endDate"] = endDate;
            //        //dr.Add("endDate", endDate);

            //        string remark = iRow.GetCell(8).ToString();
            //        dr["remark"] = remark;
            //        //dr.Add("remark", remark);

            //        string status = iRow.GetCell(9).ToString();
            //        dr["status"] = status;
            //        //dr.Add("status", status);
            //    }
            //    catch (Exception e)
            //    {
            //        // 失败的数据要记录下来
            //        string msg = "提交人:" + name + ", 报销编号:" + filecode +  "的单据导入失败。";
            //        errorList.Add(msg);
            //        addFlag = !addFlag;
            //    }

            //    if (addFlag)
            //    {
            //        dt.Rows.Add(dr);
            //        // 进行数据汇总
            //        string archiveMsg = dataArchive(dr, year, month);

            //        if ("".Equals(archiveMsg))
            //        {
            //            string msg = "提交人:" + name + ", 报销编号:" + filecode + "的单据数据汇总失败。";
            //            errorList.Add(msg);
            //        }
            //    }
            //}

            //string sql = SqlHelper.GetInsertString(dt, "upload_reimburse");

            //SqlExceRes sqlRes = new SqlExceRes(SqlHelper.Exce(sql));

            //string res = sqlRes.GetResultString("上传成功！", "上传失败", "上传失败");

            //dict.Add("msg", res);
            //dict.Add("errorList", errorList);

            //return JsonHelper.SerializeObject(dict);
            
        }
        return "";
    }

    private string getSector()
    {
        JArray jArray = new JArray();
        DataTable dt = ItemSettingManage.getSector();
        int count = dt.Rows.Count;
        if (dt == null || count == 0)
        {
            return "出错";
        }
        else
        {
            for (int i = 0; i < count; i ++)
            {
                DataRow dr = dt.Rows[i];
                JObject jObject = new JObject();
                jObject.Add("sector", dr["Sector"].ToString());

                jArray.Add(jObject);
            }
        }
        return jArray.ToString();
    }

    private string dataArchive(DataRow dataRow, int year, int month)
    {
        return ItemSettingManage.DataArchive(dataRow, year, month);
    }

    private DataTable createDataTable(DataTable dt)
    {
        DataColumn nameColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "name"
        };
        dt.Columns.Add(nameColumn);

        DataColumn departmentColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "department"
        };
        dt.Columns.Add(departmentColumn);

        DataColumn filecodeColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "filecode"
        };
        dt.Columns.Add(filecodeColumn);

        DataColumn feeareaColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "feearea"
        };
        dt.Columns.Add(feeareaColumn);

        DataColumn feedetailColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "feedetail"
        };
        dt.Columns.Add(feedetailColumn);

        DataColumn moneyColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "money"
        };
        dt.Columns.Add(moneyColumn);

        DataColumn beginDateColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "beginDate"
        };
        dt.Columns.Add(beginDateColumn);

        DataColumn endDateColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "endDate"
        };
        dt.Columns.Add(endDateColumn);

        DataColumn remarkColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "remark"
        };
        dt.Columns.Add(remarkColumn);

        DataColumn statusColumn = new DataColumn
        {
            DataType = Type.GetType("System.String"),
            ColumnName = "status"
        };
        dt.Columns.Add(statusColumn);

        return dt;
    }
}