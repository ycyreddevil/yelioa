using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json.Linq;
using System.IO;

public partial class TotalFeeManage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["action"];

        if (action == null)
        {
            action = Request.Params["action"];
        }

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();

            if ("getTotalFeeDatagrid".Equals(action))
            {
                Response.Write(getTotalFeeDatagrid());
            }
            else if ("importVoucher".Equals(action)) 
            {
                Response.Write(importVoucher());
            }
            else if ("getTotalTaxDatagrid".Equals(action))
            {
                Response.Write(getTotalTaxDatagrid());
            }
            else if ("exportExcel".Equals(action))
            {
                Response.Write(exportExcel());
            }
            else if ("accountApproval".Equals(action))
            {
                Response.Write(accountApproval());
            }
            else if ("exportVoucher".Equals(action))
            {
                Response.Write(exportVoucher());
            }

            Response.End();
        }
    }

    private string exportVoucher()
    {
        JObject res = new JObject();
        string type = Request.Form["type"];
        DateTime date = Convert.ToDateTime(Request.Form["date"]);
        //ArrayList data = JsonHelper.Json2ArrayList(Request.Form["data"]);
        DataTable dtData = JsonHelper.Json2Dtb(Request.Form["data"]);
        DataTable dtExport = new DataTable();
        dtExport.Columns.Add("1");//制单日期
        dtExport.Columns.Add("2");//凭证类别
        dtExport.Columns.Add("3");//凭证编号
        dtExport.Columns.Add("7");//摘要
        dtExport.Columns.Add("8");//科目编码
        dtExport.Columns.Add("10");//币种
        dtExport.Columns.Add("14");//借贷方向
        dtExport.Columns.Add("16");//本币
        dtExport.Columns.Add("26");//往来单位编码
        dtExport.Columns.Add("27");//往来单位
        dtExport.Columns.Add("28");//部门编码
        dtExport.Columns.Add("29");//部门
        dtExport.Columns.Add("34");//项目编码
        dtExport.Columns.Add("35");//项目
        //dtExport.Columns.Add("1");
        string sql = "select * from v_user_department_post\r\n;";
        sql += "select * from fee_type_code\r\n;";
        sql += "select * from fee_type_name\r\n;";
        string msg = "";
        DataSet ds = SqlHelper.Find(sql, ref msg);
        if (ds == null)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", msg);
            return res.ToString();
        }
        string company = "";
        string line = "销售";
        if (type == "东森销售"|| type == "东森职能")
        {
            company = "东森";     
        }
        else if (type == "业力销售" || type == "业力职能")
        {
            company = "业力";
        }
        else if (type == "中申销售" || type == "中申职能")
        {
            company = "中申";
        }
        else if (type == "业力科技")
        {
            company = "业力科技";
        }
        else if (type == "业力医学")
        {
            company = "业力医学";
        }
        else
        {
            company = type.Substring(0, 2);
        }

        //if (type.Contains("职能"))
        //    line = "职能";
        List<string> ErrMsgList = new List<string>();
        //借方
        //二级科目借方
        int iEnd = dtData.Columns.Count - 1;
        for (int i = 3; i <= iEnd; i++)//3至iEnd-3二级科目，iEnd-2税额，iEnd-1核销,iEnd总计
        {
            foreach (DataRow row in dtData.Rows)
            {
                if (row["姓名"].ToString().Equals("总计"))
                {
                    continue;
                }
                if (Convert.ToDouble(row[i]) > 0)
                {
                    string subjects1 = "";//一级科目名称
                    string subjects2 = "";//二级科目名称
                    string itemCode = "";//辅助项名称
                    string direction = "借方";
                    string Abstract="";
                    string detail = row["明细"].ToString();
                    if(detail.Contains("制造")|| detail.Contains("研发")|| detail.Contains("总部管理") || detail.Contains("-"))
                        line = "职能";
                    else
                        line = "销售";

                    string subjectsCode = "";
                    string[] departs = GetDepartmentByName(ds.Tables[0], row["姓名"].ToString());
                    if (departs.Length == 0)
                    {
                        //res.Add("ErrCode", 3);
                        //res.Add("ErrMsg", string.Format("未找到{0}的部门信息!", row["姓名"].ToString()));
                        //return res.ToString();
                        JObject obj = new JObject();
                        obj.Add("ErrCode", 3);
                        obj.Add("ErrMsg", string.Format("未找到{0}的部门信息!", row["姓名"].ToString()));
                        ErrMsgList.Add(obj.ToString());
                        continue;
                    }
                    
                    if (i == iEnd - 2)//税金
                    {
                        //subjects1 = "未抵扣进项税额";
                        //subjects2 = "其他";
                        itemCode = "";
                        direction = "借方";
                        if (company == "东森")
                            subjectsCode = "141402";//未抵扣进项税额-其他
                        else if (company == "中申"|| company == "业力")
                            subjectsCode = "1413";//未抵扣进项税-其他
                        else 
                            subjectsCode = "1413";//未抵扣进项税-其他
                        //subjectsCode = GetSubjectsCode(ds.Tables[1], subjects1, subjects2, company);
                        Abstract = string.Format("支付{0}月{1}线报账", date.Month, line);
                    }
                    else if (i == iEnd - 1)//核销
                    {
                        subjects1 = "预付账款";
                        subjects2 = "单位";
                        itemCode = "";
                        direction = "贷方";
                        subjectsCode = GetSubjectsCode(ds.Tables[1], subjects1, subjects2, company);
                        Abstract = string.Format("支付{0}月{1}线报账", date.Month, line);
                    }
                    else if (i == iEnd)//总计
                    {
                        subjects1 = "银行存款";
                        subjects2 = "单位";
                        itemCode = "";
                        direction = "贷方";
                        if (type.Contains("东森") || type.Contains("业力") || type.Contains("中申"))
                            subjectsCode = "100201";
                        else
                            subjectsCode = "1002";
                        Abstract = string.Format("支付{0}月{2}线报账({1})", date.Month, row["姓名"].ToString(), line);
                    }
                    else
                    {
                        if (row["明细"].ToString().Contains("工资社保金额"))
                        {
                            subjects1 = GetSubjectsCode(ds.Tables[2], row["姓名"].ToString());
                            if (string.IsNullOrEmpty(subjects1))
                            {
                                //res.Add("ErrCode", 2);
                                //res.Add("ErrMsg", string.Format("未找到{2}公司的{0}-{1}科目编码!", subjects1, subjects2, company));
                                //return res.ToString();
                                JObject obj = new JObject();
                                obj.Add("ErrCode", 3);
                                obj.Add("ErrMsg", string.Format("未找到{0}的工资社保对应费用类型!", row["姓名"].ToString()));
                                ErrMsgList.Add(obj.ToString());
                                continue;
                            }
                        }
                        else
                        {
                            if(line=="销售")
                                subjects1 = "销售费用";
                            else
                            {
                                if (detail.Contains("制造"))
                                    subjects1 = "制造费用";
                                else if(detail.Contains("总部管理"))
                                    subjects1 = "管理费用";
                                else if ( detail.Contains("研发") || detail.Contains("-"))
                                    subjects1 = "研发支出";
                            }
                        }
                        itemCode = GetItemCode(departs, company);
                        subjects2 = dtData.Columns[i].ColumnName;
                        if (company == "东森" || company == "傲沐")
                        {

                        }
                        else
                        {
                            if (subjects2 == "业务招待费")
                                subjects2 = "招待费";
                        }
                        direction = "借方";
                        subjectsCode = GetSubjectsCode(ds.Tables[1], subjects1, subjects2, company);
                        Abstract = string.Format("支付{0}月{1}线报账", date.Month, line);
                    }
                    //if (i == iEnd)//总计
                    //{
                    //    if (type.Contains("东森") || type.Contains("业力") || type.Contains("中申"))
                    //        subjectsCode = "100201";
                    //    else
                    //        subjectsCode = "1002";
                    //    Abstract = string.Format("支付{0}月{2}线报账({1})", date.Month, row["姓名"].ToString(), line);
                    //}
                    //else
                    //{
                    //    subjectsCode = GetSubjectsCode(ds.Tables[1], subjects1, subjects2, company);
                    //    Abstract = string.Format("支付{0}月{1}线报账", date.Month, line);
                    //}
                    if (string.IsNullOrEmpty(subjectsCode))
                    {
                        //res.Add("ErrCode", 2);
                        //res.Add("ErrMsg", string.Format("未找到{2}公司的{0}-{1}科目编码!", subjects1, subjects2, company));
                        //return res.ToString();
                        JObject obj = new JObject();
                        obj.Add("ErrCode", 2);
                        obj.Add("ErrMsg", string.Format("未找到{2}公司的{0}-{1}科目编码!", subjects1, subjects2, company));
                        ErrMsgList.Add(obj.ToString());
                        continue;
                    }
                    //int index = GetVoucherIndex(dtExport, subjectsCode, itemCode);

                    int index = -1;
                    int len = dtExport.Rows.Count;
                    for (int j = 0; j < len; j++)
                    {
                        //DataRow row = dtExport.Rows[i];
                        if (direction== "贷方")//"贷方"
                        {
                            if(dtExport.Rows[j]["7"].ToString().Contains(row["姓名"].ToString())&& 
                                dtExport.Rows[j]["8"].ToString().Equals(subjectsCode))
                            {
                                index = j;
                                break;
                            }
                        }
                        else//借方
                        {
                            if (dtExport.Rows[j]["8"].ToString().Equals(subjectsCode))
                            {
                                if (dtExport.Rows[j]["26"].ToString().Equals(itemCode) ||
                                    dtExport.Rows[j]["28"].ToString().Equals(itemCode) ||
                                    dtExport.Rows[j]["34"].ToString().Equals(itemCode))
                                {
                                    index = j;
                                    break;
                                }
                            }
                        }
                    }
                    if (index < 0)
                    {
                        DataRow newRow = dtExport.NewRow();
                        newRow["1"] = DateTime.Now.ToString("yyyy-MM-dd");
                        newRow["2"] = "记账凭证";
                        newRow["3"] = "999";
                        newRow["7"] = Abstract;
                        newRow["8"] = subjectsCode;
                        newRow["10"] = "人民币";
                        newRow["14"] = direction;
                        newRow["16"] = row[i].ToString();
                        if(!string.IsNullOrEmpty(itemCode))
                            newRow["28"] = itemCode;
                        dtExport.Rows.Add(newRow);
                    }
                    else
                    {
                        DataRow newRow = dtExport.Rows[index];
                        newRow["16"] = (Convert.ToDouble(row[i]) + Convert.ToDouble(newRow["16"]));
                    }
                }
            }
        }

        try
        {
            string fileName = string.Format("凭证用友-{0}-{1}", type, date.ToString("yyyyMM"));
            byte[] data = null;
            string path = Server.MapPath("~/Template/凭证导入导出.xls");
            data = ExcelHelperV2_0.CreateVoucher(dtExport, path, ref msg);
            string filecode = ValideCodeHelper.GetRandomCode(64);
            string newPath = Server.MapPath("~/tempExportFile");
            newPath = newPath + @"\" + filecode + ".xls";
            BytesToFile(newPath, data);
            res.Add("ErrCode", 0);
            res.Add("FileCode", filecode);
            res.Add("FileName", fileName);
            if(ErrMsgList.Count>0)
            {
                string ErrMsg = "";
                foreach(string errMsg in ErrMsgList)
                {
                    ErrMsg += "{" + errMsg + "},";
                }
                ErrMsg = ErrMsg.Substring(0, ErrMsg.Length - 1);
                res.Add("ErrMsg", ErrMsg);
            }
        }
        catch(Exception ex)
        {
            res.Add("ErrCode", 500);
            res.Add("ErrMsg", ex.ToString());
        }
        


        return res.ToString();
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

    private string GetItemCode(string[] departs, string company)
    {
        string code = "";
        //用友系统，东森:销售部：0203，市场技术部：0202
        //中申: 销售部：0206，市场部：0205
        //业力：销售部：0204，市场技术部：0203
        if (company == "东森")
        {
            code = "0203";
            foreach (string d in departs)
            {
                if (d.Contains("市场"))
                {
                    code = "0202";
                    break;
                }
            }
        }
        else if (company == "中申")
        {
            code = "0206";
            foreach (string d in departs)
            {
                if (d.Contains("市场"))
                {
                    code = "0205";
                    break;
                }
            }
        }
        else if (company == "业力")
        {
            code = "0204";
            foreach (string d in departs)
            {
                if (d.Contains("市场"))
                {
                    code = "0203";
                    break;
                }
            }
        }
        return code;
    }

    private string[] GetDepartmentByName(DataTable dt, string name)
    {
        List<string> list = new List<string>();
        foreach (DataRow row in dt.Rows)
        {
            if (row["userName"].ToString().Equals(name))
            {
                list.Add(row["department"].ToString());
            }
        }
        return list.ToArray();
    }

    /// <summary>
    /// 通过一级科目及二级科目名称，查找二级科目代码
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="subjects1"></param>
    /// <param name="subjects2"></param>
    /// <returns></returns>
    private string GetSubjectsCode(DataTable dt, string subjects1, string subjects2, string company)
    {
        string res = "";
        string subjects1Code = "";
        foreach (DataRow row in dt.Rows)
        {
            if (row["Name"].ToString().Equals(subjects1) && Convert.ToInt32(row["Level"]) == 1
                && row["Company"].ToString().Equals(company))
            {
                subjects1Code = row["Code"].ToString();
                break;
            }
        }
        if (string.IsNullOrEmpty(subjects1Code))
            return res;
        foreach (DataRow row in dt.Rows)
        {
            if (row["Name"].ToString().Equals(subjects2) && row["Code"].ToString().Contains(subjects1Code)
                && row["Company"].ToString().Equals(company))
            {
                res = row["Code"].ToString();
                break;
            }
        }
        return res;
    }

    private string GetSubjectsCode(DataTable dt, string userName)
    {
        string res = "";
        foreach (DataRow row in dt.Rows)
        {
            if (row["Name"].ToString().Equals(userName))
            {
                res = row["FeeType"].ToString();
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// 查收是否有重复的凭证
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="subjectsCode">二级科目code</param>
    /// <param name="itemCode">辅助项code</param>
    /// <returns></returns>
    private int GetVoucherIndex(DataTable dt, string subjectsCode, string itemCode)
    {
        int res = -1;
        int len = dt.Rows.Count;
        for (int i = 0; i < len; i++)
        {
            DataRow row = dt.Rows[i];
            if(string.IsNullOrEmpty(itemCode))//贷方
            {

            }
            else//借方
            {
                if (row["8"].ToString().Equals(subjectsCode))
                {
                    if (row["26"].ToString().Equals(itemCode) ||
                        row["28"].ToString().Equals(itemCode) ||
                        row["34"].ToString().Equals(itemCode))
                    {
                        res = i;
                        break;
                    }
                }
            }
        }
        return res;
    }


    private string getTotalFeeDatagrid()
    {
        //string dept = Request.Form["dept"];
        string startTm = Request.Form["startTm"] + " 00:00:00";
        //string endTm = Request.Form["endTm"] + " 23:59:59";
        string type = Request.Form["type"].ToString();

        string[] salesHeader = { "出差车船费", "住宿费", "出差补贴", "实报实销", "餐费", "市内交通费", "会议费", "培训费", "办公用品", "工作餐","场地费","招待餐费","纪念品","外协劳务","外部人员机票/火车票","外部人员住宿费","外部人员交通费","学术会费","营销办公费"};
        string[] notSalesHeader = { "出差车船费", "交通费", "汽车使用费", "实报实销", "住宿费", "出差补贴", "业务招待费", "培训费","办公费","租赁费","劳保费","通讯费","福利费","物业费","水电费","招聘费","运输费","材料费","试验费","检测费","专利费","注册费","服务费","其他"};

        string sql = "";

        string[] feeDetailHeader = { };

        if (type == "1" || type == "3" || type == "6" || type == "7" || type == "9" || type == "10" || type == "12" || type == "13")
            feeDetailHeader = notSalesHeader;
        else
            feeDetailHeader = salesHeader;

        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

        List<string> nameList = new List<string>();

        foreach (string header in feeDetailHeader)
        {
            if (type == "1")
            {
                // 业力职能
                sql = getTotalSql(startTm, null, header, "like '江西业力医疗器械%'", "not like '%营销中心%'");
            }
            else if (type == "2")
            {
                // 业力销售
                sql = getTotalSql(startTm, null, header, "like '江西业力医疗器械%'", "like '%营销中心%'");
            }
            else if (type == "3")
            {
                // 中申职能
                sql = getTotalSql(startTm, null, header, "like '南昌市中申%'", "not like '%营销中心%'");
            }
            else if (type == "4")
            {
                // 中申销售
                sql = getTotalSql(startTm, null, header, "like '南昌市中申%'", "like '%营销中心%'");
            }
            else if (type == "5")
            {
                // 东森销售
                sql = getTotalSql(startTm, null, header, "like '江西东森科技发展%'", "like '%营销中心%'");
            }
            else if (type == "6")
            {
                // 东森职能
                sql = getTotalSql(startTm, null, header, "like '江西东森科技发展%'", "not like '%营销中心%'");
            }
            else if (type == "7")
            {
                sql = getTotalSql(startTm, null, header, "= '江西业力科技集团有限公司'", "like '%'");
            }
            else if (type == "8")
            {
                // 老康销售
                sql = getTotalSql(startTm, null, header, "= '南昌老康科技有限公司'", "like '营销中心%'");
            }
            else if (type == "9")
            {
                sql = getTotalSql(startTm, null, header, "= '天津吉诺泰普生物科技有限公司'", "like '%'");
            }
            else if (type == "10")
            {
                sql = getTotalSql(startTm, null, header, "= '南昌业力医学检验实验室有限公司'", "like '%'");
            }
            else if (type == "11")
            {
                // 傲沐销售
                sql = getTotalSql(startTm, null, header, "= '九江傲沐科技发展有限公司'", "like '营销中心%'");
            }
            else if (type == "12")
            {
                // 老康职能
                sql = getTotalSql(startTm, null, header, "= '南昌老康科技有限公司'", "not like '%营销中心%'");
            }
            else if (type == "13")
            {
                // 傲沐职能
                sql = getTotalSql(startTm, null, header, "= '九江傲沐科技发展有限公司'", "not like '%营销中心%'");
            }

            sql = "select name, (sum(receiptAmount)-ifnull(sum(receiptTax), 0)) amount, ifnull(sum(receiptTax), 0) tax,fee_department,fee_detail from (select * from ( " + sql + " ) tt where not exists ( select 1 from ( " + sql + " ) aaa where aaa.id = tt.id and aaa.id1 > tt.id1)) fffinal group by name,fee_department,fee_detail";

            DataTable dt = SqlHelper.Find(sql).Tables[0];

            if (dt.Rows.Count == 0)
            {
                if (result.Count > 0)
                {
                    foreach (Dictionary<string, object> temp in result)
                    {
                        string convertHeader = convertFeeDetailHeader(header);
                        if (!temp.ContainsKey(convertHeader))
                            temp.Add(convertHeader, 0);
                    }
                }

                continue;
            }

            foreach (DataRow dr in dt.Rows)
            {
                string name = dr[0].ToString();
                double amount = double.Parse(dr[1].ToString());
                double tax = double.Parse(dr[2].ToString());
                string fee_department = dr[3].ToString();
                string fee_detail = dr[4].ToString();

                Boolean hasRepeatName = false;

                if (result.Count > 0)
                {
                    foreach (Dictionary<string, object> tempDict in result)
                    {
                        if (tempDict["姓名"].ToString() == name && tempDict["部门"].ToString() == fee_department && tempDict["明细"].ToString() == fee_detail)
                        {
                            hasRepeatName = true;

                            double totalTax = double.Parse(tempDict["税额"].ToString()) + tax;
                            double total = double.Parse(tempDict["总计"].ToString()) + amount + tax;

                            if (!tempDict.ContainsKey(convertFeeDetailHeader(header)))
                                tempDict.Add(convertFeeDetailHeader(header), amount);
                            else
                                tempDict[convertFeeDetailHeader(header)] = double.Parse(tempDict[convertFeeDetailHeader(header)].ToString()) + amount;

                            if (!tempDict.ContainsKey("税额"))
                                tempDict.Add("税额", Math.Round(totalTax, 3));
                            else
                                tempDict["税额"] = totalTax;

                            if (!tempDict.ContainsKey("总计"))
                                tempDict.Add("总计", Math.Round(total, 3));
                            else
                                tempDict["总计"] = total;

                            break;
                        }
                    }
                }

                if (!hasRepeatName)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>
                    {
                        { "姓名", name },
                        { "部门", fee_department},
                        { "明细", fee_detail},
                        { convertFeeDetailHeader(header), amount },
                        { "税额", tax },
                        { "总计", amount+tax}
                    };

                    int index = result.FindIndex(u => name.Equals(u["姓名"].ToString()));

                    result.Insert(index + 1, temp);
                }
            }
        }

        foreach (string tempHeader in feeDetailHeader)
        {
            foreach (Dictionary<string, object> temp in result)
            {
                if (!temp.ContainsKey(convertFeeDetailHeader(tempHeader)))
                    temp.Add(convertFeeDetailHeader(tempHeader), 0);
            }
        }

        // 加上核销 备用金
        foreach (Dictionary<string, object> temp in result)
        {
            string name = temp["姓名"].ToString();
            string fee_department = temp["部门"].ToString();
            string fee_detail = temp["明细"].ToString();

            if (type == "1" || type == "2")
            {
                sql = getSql(startTm, null, name, fee_department, fee_detail, "like '江西业力医疗器械%'");
            }
            else if (type == "3" || type == "4")
            {
                sql = getSql(startTm, null, name, fee_department, fee_detail, "like '南昌市中申%'");
            }
            else if (type == "5" || type == "6")
            {
                sql = getSql(startTm, null, name, fee_department, fee_detail, "like '江西东森科技发展%'");
            }
            else if (type == "7")
            {
                sql = getSql(startTm, null, name, fee_department, fee_detail, "= '江西业力科技集团有限公司'");
            }
            else if (type == "8" || type == "12")
            {
                sql = getSql(startTm, null, name, fee_department, fee_detail, "= '南昌老康科技有限公司'");
            }
            else if (type == "9")
            {
                sql = getSql(startTm, null, name, fee_department, fee_detail, "= '天津吉诺泰普生物科技有限公司'");
            }
            else if (type == "10")
            {
                sql = getSql(startTm, null, name, fee_department, fee_detail, "= '南昌业力医学检验实验室有限公司'");
            }
            else if (type == "11" || type == "13")
            {
                sql = getSql(startTm, null, name, fee_department, fee_detail, "= '九江傲沐科技发展有限公司'");
            }

            DataSet ds = SqlHelper.Find(sql);

            double isPrepaidAmount = ds.Tables[0].Rows.Count == 0 ? 0 : double.Parse(ds.Tables[0].Rows[0][0].ToString());
            double loanAmount = ds.Tables[1].Rows.Count == 0 ? 0 : double.Parse(ds.Tables[1].Rows[0][0].ToString());
            double payAmount = ds.Tables[2].Rows.Count == 0 ? 0 : double.Parse(ds.Tables[2].Rows[0][0].ToString());
            //double actualAmount = double.Parse(ds.Tables[2].Rows[0][0].ToString());

            temp.Add("核销", isPrepaidAmount);
            temp.Add("备用金", loanAmount);
            temp.Add("实付金额", payAmount);

            double total = double.Parse(temp["总计"].ToString());

            total -= isPrepaidAmount;

            temp["总计"] = total;
            temp["应付金额"] = total - loanAmount;
            temp["实付金额"] = payAmount == 0 ? total - loanAmount : payAmount;
        }

        List<Dictionary<string, object>> finalResult = new List<Dictionary<string, object>>();

        // 排序
        List<string> keyList = new List<string>();

        foreach (Dictionary<string, object> temp in result)
        {
            Dictionary<string, object> tempDict = new Dictionary<string, object>();

            foreach (string key in temp.Keys)
            {
                if (!keyList.Contains(key) && !key.Contains("_addition"))
                {
                    keyList.Add(key);
                }

                if (key != "核销" && key != "总计" && key != "税额" && key != "备用金" && key != "应付金额" && key != "实付金额")
                {
                    tempDict.Add(key, temp[key]);
                }
            }

            tempDict.Add("税额", temp["税额"]);
            tempDict.Add("核销", temp["核销"]);
            tempDict.Add("总计", temp["总计"]);
            tempDict.Add("备用金", temp["备用金"]);
            tempDict.Add("应付金额", temp["应付金额"]);
            tempDict.Add("实付金额", temp["实付金额"]);

            finalResult.Add(tempDict);
        }

        Dictionary<string, object> additionDict = new Dictionary<string, object>();

        // 增加竖向汇总总计
        foreach (string key in keyList)
        {
            double tempAmount = 0;

            if (key == "姓名")
            {
                additionDict.Add(key, "总计");
                continue;
            }
            else if (key == "部门" || key == "明细")
            {
                additionDict.Add(key, "");
                continue;
            }

            foreach (Dictionary<string, object> temp in finalResult)
            {
                tempAmount += double.Parse(temp[key].ToString());
            }

            additionDict.Add(key, tempAmount);
        }

        finalResult.Add(additionDict);

        return JsonHelper.SerializeObject(finalResult);
    }

    private string convertFeeDetailHeader(string feeDetail)
    {
        if (feeDetail == "出差车船费" || feeDetail == "住宿费" || feeDetail == "出差补贴" || feeDetail == "实报实销")
            return "差旅费";
        else if (feeDetail == "餐费")
            return "业务招待费";
        else if (feeDetail == "市内交通费")
            return "交通费";
        else if (feeDetail == "办公用品")
            return "办公费";
        else if (feeDetail == "营销办公费")
            return "业务宣传费";
        else if (feeDetail == "工作餐" || feeDetail == "场地费" || feeDetail == "招待餐费" || feeDetail == "纪念品" || feeDetail == "外协劳务" || feeDetail == "外部人员机票/火车票" || feeDetail == "外部人员住宿费" ||
             feeDetail == "外部人员交通费" || feeDetail == "学术会费")
            return "会议费";
        else
            return feeDetail;
    }

    private string convertFeeDetailHeaderReverse(string feeDetail)
    {
        if (feeDetail == "差旅费")
            return "出差车船费,住宿费,出差补贴,实报实销";
        else if (feeDetail == "业务招待费")
            return "餐费";
        else if (feeDetail == "交通费")
            return "市内交通费";
        else if (feeDetail == "办公费")
            return "办公用品";
        else if (feeDetail == "业务宣传费")
            return "营销办公费";
        else if (feeDetail == "会议费")
            return "工作餐,场地费,招待餐费,纪念品,外协劳务,外部人员机票/火车票,外部人员住宿费,外部人员交通费,学术会费";
        else
            return feeDetail;
    }

    /// <summary>
    /// 获取核销、备用金、实付金额sql
    /// </summary>
    /// <param name="startTm"></param>
    /// <param name="endTm"></param>
    /// <param name="name"></param>
    /// <param name="fee_department"></param>
    /// <param name="fee_detail"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    private string getSql(string startTm, string endTm, string name, string fee_department, string fee_detail, string condition)
    {
        //string sql = string.Format("select ifnull(sum(temp.fee_amount), 0) from (select distinct id,code,fee_amount,actual_fee_amount_time from " +
        //    "yl_reimburse where name = '{2}' and status = '已审批' and (account_result != '拒绝' or account_result is null) and isPrepaid = '1' and fee_department = '{3}' " +
        //    "and fee_detail like '{4}%' and fee_company {5} and (month('{0}') = month(actual_fee_amount_time) - 1)) temp; ", startTm, endTm, name, fee_department, fee_detail, condition);

        //sql += string.Format("select ifnull(sum(temp.amount), 0) from (select distinct t1.id,t1.code,t1.actual_fee_amount_time,t3.amount from " +
        //    "yl_reimburse t1 inner join yl_reimburse_loan t3 on t1.code = t3.reimburseCode " +
        //    "where t1.fee_company {5} and t1.name = '{2}' and t1.status = '已审批' and t1.fee_department = '{3}' and t1.fee_detail like '{4}%' and (month('{0}') = month(t1.actual_fee_amount_time) - 1)) temp; ", startTm, endTm, name, fee_department, fee_detail, condition);

        //sql += string.Format("select ifnull(sum(temp.pay_amount), 0) from (select distinct id,code,pay_amount,actual_fee_amount_time from yl_reimburse " +
        //    "where name = '{2}' and status = '已审批' and (account_result != '拒绝' or account_result is null) and fee_company {5} and (month('{0}') = month(actual_fee_amount_time) - 1) " +
        //    "and fee_department = '{3}' and fee_detail like '{4}%') temp ", startTm, endTm, name, fee_department, fee_detail, condition);

        string sql = string.Format("select ifnull(sum(temp.receiptAmount), 0) from (select distinct t1.id,t2.id1,t1.code,t1.actual_fee_amount,t2.receiptAmount from (select * from yl_reimburse where name = '{2}' and status = '已审批' " +
        "and (account_result != '拒绝' or account_result is null) and fee_company {5} and (month('{0}') = month(actual_fee_amount_time) - 1)) t1 inner join " +
        "(select ReceiptAmount,status,code,id id1 from yl_reimburse_detail where status = '同意' ) t2  on t2.code like concat('%', t1.code, '%') " +
        "where t1.isPrepaid = '1' and t1.fee_department = '{3}' and t1.fee_detail like '{4}%') temp; ", startTm, endTm, name, fee_department, fee_detail, condition);

        sql += string.Format("select ifnull(sum(temp.amount), 0) from (select distinct t1.id,t2.id1,t1.code,t1.actual_fee_amount,t3.amount from (select * from yl_reimburse where name = '{2}' and status = '已审批' " +
        "and (account_result != '拒绝' or account_result is null) and fee_company {5} and (month('{0}') = month(actual_fee_amount_time) - 1)) t1 inner join " +
        "(select ReceiptAmount,status,code,id id1 from yl_reimburse_detail where status = '同意') t2 on t2.code like concat('%', t1.code, '%') " +
        "inner join yl_reimburse_loan t3 on t1.code = t3.reimburseCode where t1.fee_department = '{3}' and t1.fee_detail like '{4}%') temp; ", startTm, endTm, name, fee_department, fee_detail, condition);

        //sql += string.Format("select ifnull(sum(temp.pay_amount), 0) from (select distinct t1.id,t1.code,t1.actual_fee_amount,t1.pay_amount from (select * from yl_reimburse where name = '{2}' and status = '已审批' " +
        //"and (account_result != '拒绝' or account_result is null) and fee_company {5} and (month('{0}') = month(actual_fee_amount_time) - 1)) t1 inner join " +
        //"(select ReceiptAmount,status,code from yl_reimburse_detail) t2  on t2.code like concat('%', t1.code, '%') " +
        //"where t2.status = '同意' and t1.fee_department = '{3}' and t1.fee_detail like '{4}%') temp ", startTm, endTm, name, fee_department, fee_detail, condition);

        sql += string.Format("select ifnull(sum(temp.pay_amount), 0) from (select distinct id,code,pay_amount,actual_fee_amount_time from yl_reimburse " +
            "where name = '{2}' and status = '已审批' and (account_result != '拒绝' or account_result is null) and fee_company {5} and (month('{0}') = month(actual_fee_amount_time) - 1) " +
            "and fee_department = '{3}' and fee_detail like '{4}%') temp ", startTm, endTm, name, fee_department, fee_detail, condition);

        return sql;
    }

    /// <summary>
    /// 汇总表总sql
    /// </summary>
    /// <param name="startTm"></param>
    /// <param name="endTm"></param>
    /// <param name="header"></param>
    /// <param name="companyCondition"></param>
    /// <param name="departmentCondition"></param>
    /// <returns></returns>
    private string getTotalSql(string startTm, string endTm, string header, string companyCondition, string departmentCondition)
    {
        return string.Format("select * from (select a.id id1,a.code code1,a.name,a.fee_department,a.actual_fee_amount_time,REVERSE(SUBSTR(REVERSE(a.fee_detail) FROM " +
            "INSTR(REVERSE(a.fee_detail),'-')+1)) fee_detail from yl_reimburse a where status = '已审批' and (account_result != '拒绝' or account_result is null) " +
            "and fee_company {3} and department {4} and (month('{0}') = month(actual_fee_amount_time) - 1)) t1 left join (select ReceiptTax,receiptType,ReceiptAmount,status,code,id " +
            "from yl_reimburse_detail ) t2 on t2.code like concat('%', t1.code1, '%') " +
            " where t2.receiptType = '{2}' and t2.status = '同意' order by t1.name ", startTm, endTm, header, companyCondition, departmentCondition);
    }

    /// <summary>
    /// 税额表sql
    /// </summary>
    /// <param name="startTm"></param>
    /// <param name="endTm"></param>
    /// <param name="companyCondition"></param>
    /// <param name="departmentCondition"></param>
    /// <returns></returns>
    private string getTaxSql(string startTm, string endTm, string companyCondition, string departmentCondition)
    {
        return string.Format("select distinct t2.activityDate 日期, t1.name 人员名称, t2.originAmount 含税金额, t2.receiptTax 税额, (t2.originAmount - t2.receiptTax) 不含税金额, " +
        "round(t2.receiptTax/(t2.originAmount - t2.receiptTax), 2) 税率, t2.feeType 票据类型, t2.receiptNum 票号 " +
        "from (select * from yl_reimburse where status = '已审批' and (account_result != '拒绝' or account_result is null) " +
        "and fee_company {2} and department {3} and (month('{0}') = month(actual_fee_amount_time) - 1)) t1 left join (select code,activityDate,originAmount,receiptTax,feeType,receiptNum,status from yl_reimburse_detail) t2 " +
        "on t2.code like concat('%', t1.code, '%') where t2.status = '同意' and t2.receiptTax != 0 and t2.receiptTax is not null order by t1.name"
        , startTm, endTm, companyCondition, departmentCondition);
    }

    private string getTotalTaxDatagrid()
    {
        string startTm = Request.Form["startTm"] + " 00:00:00";
        //string endTm = DateTime.Parse(startTm).AddMonths(1).AddDays(-7).ToString("yyyy-MM-dd") + " 23:59:59";
        string type = Request.Form["type"].ToString();

        string sql = "";

        if (type == "1")
        {
            // 业力职能
            sql = getTaxSql(startTm, null, "like '江西业力医疗器械%'", "not like '%营销中心%'");
        }
        else if (type == "2")
        {
            sql = getTaxSql(startTm, null, "like '江西业力医疗器械%'", "like '%营销中心%'");
        }
        else if (type == "3")
        {
            sql = getTaxSql(startTm, null, "like '南昌市中申%'", "not like '%营销中心%'");
            // 中申职能
        }
        else if (type == "4")
        {
            sql = getTaxSql(startTm, null, "like '南昌市中申%'", "like '%营销中心%'");
            // 中申销售
        }
        else if (type == "5")
        {
            sql = getTaxSql(startTm, null, "like '江西东森科技发展%'", "like '%营销中心%'");
            // 东森销售
        }
        else if (type == "6")
        {
            sql = getTaxSql(startTm, null, "like '江西东森科技发展%'", "not like '%营销中心%'");
            // 东森职能
        }
        else if (type == "7")
        {
            sql = getTaxSql(startTm, null, "like '江西业力科技集团有限公司%'", "like '%'");
        }
        else if (type == "8")
        {
            sql = getTaxSql(startTm, null, "= '南昌老康科技有限公司'", "like '%营销中心%'");
        }
        else if (type == "9")
        {
            sql = getTaxSql(startTm, null, "= '天津吉诺泰普生物科技有限公司'", "like '%'");
        }
        else if (type == "10")
        {
            sql = getTaxSql(startTm, null, "= '南昌业力医学检验实验室有限公司'", "like '%'");
        }
        else if (type == "11")
        {
            sql = getTaxSql(startTm, null, "= '九江傲沐科技发展有限公司'", "like '%营销中心%'");
        }
        else if (type == "12")
        {
            sql = getTaxSql(startTm, null, "= '南昌老康科技有限公司'", "not like '%营销中心%'");
        }
        else if (type == "13")
        {
            sql = getTaxSql(startTm, null, "= '九江傲沐科技发展有限公司'", "not like '%营销中心%'");
        }

        return JsonHelper.DataTable2Json(SqlHelper.Find(sql).Tables[0]);
    }

    private string accountApproval()
    {
        var result = new JObject
        {
            { "code", 200 },
            { "msg", "ok" }
        };

        string startTm = Request.Form["startTm"] + " 00:00:00";
        //string endTm = Request.Form["endTm"] + " 23:59:59";
        string names = Request.Form["names"];
        string departments = Request.Form["departments"];
        string feeDetails = Request.Form["feeDetails"];
        string type = Request.Form["type"];
        string payAmounts = Request.Form["payAmounts"];

        int month = Convert.ToDateTime(startTm).Month;

        List<string> nameList = JsonHelper.DeserializeJsonToObject<List<string>>(names);
        List<string> departmentList = JsonHelper.DeserializeJsonToObject<List<string>>(departments);
        List<string> feeDetailList = JsonHelper.DeserializeJsonToObject<List<string>>(feeDetails);
        List<string> payAmountList = JsonHelper.DeserializeJsonToObject<List<string>>(payAmounts);

        string company = "江西业力医疗器械有限公司";

        if (type == "3" || type == "4")
            company = "南昌市中申医疗器械有限公司";
        else if (type == "5" || type == "6")
            company = "江西东森科技发展有限公司";
        else if (type == "7")
            company = "江西业力科技集团有限公司";
        else if (type == "8" || type == "12")
            company = "南昌老康科技有限公司";
        else if (type == "9")
            company = "天津吉诺泰普生物科技有限公司";
        else if (type == "10")
            company = "南昌业力医学检验实验室有限公司";
        else if (type == "11" || type == "13")
            company = "九江傲沐科技发展有限公司";

        int index = 0;
        foreach (string name in nameList)
        {
            // 找到每个人所有发票关联的移动报销
            string sql = string.Format("select distinct t2.code,t3.wechatUserId from yl_reimburse_detail t1 " +
                "inner join (select * from yl_reimburse where name = '{2}' and fee_detail like '%{3}%' and fee_department like '%{4}%' and fee_company = '{5}' " +
                "and (month('{0}') = month(actual_fee_amount_time) - 1)) t2 on t1.code like concat('%', t2.code, '%') " +
                "inner join users t3 on t1.submitterId = t3.userId ", startTm, null, name, feeDetailList[index], departmentList[index], company);

            DataTable dt = SqlHelper.Find(sql).Tables[0];

            if (dt.Rows.Count == 0)
                continue;

            List<string> sqls = new List<string>();

            //总出纳付款金额
            double totalAmount = double.Parse(payAmountList[index].ToString());

            // 给每个人发消息提示财务已付款
            string wechatUserId = dt.Rows[0][1].ToString();

            WxCommon wx = new WxCommon("mMobileReimbursement", "UM0i5TXSIqQIOWk-DmUlfTqBqvZAfbZdGGDKiFZ-nRk", "1000006", "");

            wx.SendWxMsg(wechatUserId, "出纳付款通知", string.Format("您的{0}月单据出纳已付款，总计{1}元,请查收", month, totalAmount), "http://yelioa.top/mMySubmittedReimburse.aspx");

            foreach (DataRow dr in dt.Rows)
            {
                if (totalAmount <= 0)
                    break;

                string code = dr[0].ToString();

                string temp_sql = string.Format("select actual_fee_amount from yl_reimburse where code = '{0}';", code);
                temp_sql += string.Format("select ifnull(sum(amount),0) from yl_reimburse_loan where reimburseCode = '{0}'", code);

                DataSet tempDs = SqlHelper.Find(temp_sql);

                if (tempDs == null)
                    continue;

                double actual_fee_amount = double.Parse(tempDs.Tables[0].Rows[0][0].ToString());
                double loan_amount = double.Parse(tempDs.Tables[1].Rows[0][0].ToString());

                double pay_amount = (actual_fee_amount - loan_amount) < totalAmount ? (actual_fee_amount - loan_amount) : totalAmount;

                sqls.Add(string.Format("update yl_reimburse set pay_amount = {1} where code = '{0}';", code, pay_amount));

                totalAmount -= pay_amount;
            }

            SqlHelper.Exce(sqls.ToArray());
            index++;
        }

        return result.ToString();
    }

    private string importVoucher()
    {
        return null;
    }

    private string exportExcel()
    {
        JObject res = new JObject();
        DateTime date = Convert.ToDateTime(Request.Form["date"]);
        string type = Request.Form["type"];
        string strData = Request.Form["dataTotal"];
        DataTable dtTotal = JsonHelper.Json2Dtb(strData);
        DataTable dtFax = JsonHelper.Json2Dtb(Request.Form["dataFax"]);

        int totalLen = dtTotal.Rows.Count;
        if (totalLen == 0)
        {
            res.Add("ErrCode", 1);
            res.Add("ErrMsg", "导入数据为空！");
            return res.ToString();
        }

        string sql = "select * from fee_type_name\r\n;";
        sql += "select * from account_fee_detail";
        DataSet ds = SqlHelper.Find(sql);

        string company = GetCompanyName(type);

        //新增FeeType列，并填入相应数据
        dtTotal.Columns.Add("FeeType");
        List<string> listFeeType = new List<string>();
        for(int i = 0; i < dtTotal.Rows.Count; i++)
        {
            if (dtTotal.Rows[i]["姓名"].ToString().Equals("总计"))
                continue;
            string feeType = GetFeeTypeByName(ds.Tables[0], dtTotal.Rows[i]["姓名"].ToString());
            dtTotal.Rows[i]["FeeType"] = feeType;
            if (!listFeeType.Contains(feeType))
                listFeeType.Add(feeType);
        }

        DataTable dt = new DataTable();
        for(int i = 1; i <= 55; i++)
        {
            dt.Columns.Add(i.ToString());
        }

        //借方汇总
        foreach(string feeType in listFeeType)
        {
            DataRow[] rows = dt.Select(string.Format("FeeType = '{0}'", feeType));
            //double total = 0;
            
            foreach (DataColumn c in dtTotal.Columns)
            {
                if (c.ColumnName == "姓名" || c.ColumnName == "总计")
                    continue;
                double total = 0;
                foreach (DataRow row in rows)
                {
                    total += Convert.ToDouble(row[c.ColumnName]);
                }

                if (total == 0)
                    continue;
                DataRow newRow = dt.NewRow();
                newRow[0] = DateTime.Now.ToString("yyyy-MM-dd");
                newRow[1] = "记账凭证";
                newRow[2] = "请手动修改！";
                newRow[6] = string.Format("XX.XX支付{0}月", date.Month);
                string AuxiliaryItems = "";
                //string code = GetFeeCodeByName(ds,)
            }
        }
        //DataRow rowTotal = dtTotal.Rows[totalLen - 1];
        
       


        //借方税金
        int faxLen = dtFax.Rows.Count;

        //贷方




        return res.ToString();
    }

    private string GetFeeTypeByName(DataTable dt,string name)
    {
        string feeType = "";
        foreach (DataRow row in dt.Rows)
        {
            if (row["Name"].ToString().Equals(name))
            {
                feeType = row["FeeType"].ToString();
                break;
            }
        }
        return feeType;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ds">table[0]:fee_type_name,table[1]:account_fee_detail</param>
    /// <param name="name"></param>
    /// <returns></returns>
    private string GetFeeCodeByName(DataSet ds, string feeType, string company,ref string AuxiliaryItems)
    {
        if (ds == null)
            return "";
        
        string parentCode = "";
        foreach (DataRow row in ds.Tables[1].Rows)
        {
            if (row["Company"].ToString().Equals(company) && row["Name"].ToString().Equals(feeType))
            {
                parentCode = row["Code"].ToString();
                break;
            }
        }
        string code = "";
        foreach (DataRow row in ds.Tables[1].Rows)
        {
            if (row["Code"].ToString().Contains(parentCode) && row["Company"].ToString().Equals(company) && row["Name"].ToString().Equals(company))
            {
                parentCode = row["Code"].ToString();
                break;
            }
        }
        return code;
    }

    private string GetCompanyName(string type)
    {
        string company = "";
        if (type.Contains("东森"))
            company = "东森";
        else if (type.Contains("业力"))
            company = "业力";
        else if (type.Contains("中申"))
            company = "中申";
        return company;
    }
}