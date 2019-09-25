using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

public partial class CustomizedForm : System.Web.UI.Page
{
    public string ifGetData = "";
    public string formName = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.Form["act"];
        ifGetData = Request.QueryString["getData"];
        formName = Request.QueryString["formName"];

        if (!string.IsNullOrEmpty(action))
        {
            Response.Clear();
            if (action == "saveForm")
            {
                Response.Write(saveForm());
            }
            else if (action == "loadForm")
            {
                Response.Write(loadForm());
            }
            Response.End();
        }
    }

    public string loadForm()
    {
        string formName = Request.Form["formName"];
        DataTable dt = CustomizedFormManage.getFormData(formName);

        return JsonHelper.DataTable2Json(dt);
    }

    public string saveForm()
    {
        string formTitle = Request.Form["formTitle"];

        string itemNames = Request.Form["itemNames"];
        itemNames = itemNames.Substring(0, itemNames.Length - 1);
        string[] itemNameArray = Regex.Split(itemNames, ";");

        string itemDefaultValues = Request.Form["itemDefaultValues"];
        itemDefaultValues = itemDefaultValues.Substring(0, itemDefaultValues.Length - 1);
        string[] itemDefaultValuesArray = Regex.Split(itemDefaultValues, ";");

        string types = Request.Form["types"];
        types = types.Substring(0, types.Length - 1);
        string[] typesArray = Regex.Split(types, ";");

        string extras = Request.Form["extras"];
        extras = extras.Substring(0, extras.Length - 1);
        string[] extrasArray = Regex.Split(extras, ";");

        string isNecessary = Request.Form["isnecessary"];
        isNecessary = isNecessary.Substring(0, isNecessary.Length - 1);
        string[] isNecessaryArray = Regex.Split(isNecessary, ";");

        string length = Request.Form["length"];
        length = length.Substring(0, length.Length - 1);
        string[] lengthArray = Regex.Split(length, ";");

        DataTable dt = new DataTable();
        dt.Columns.Add("formName", typeof(string));
        dt.Columns.Add("fieldName", typeof(string));
        dt.Columns.Add("defaultValue", typeof(string));
        dt.Columns.Add("type", typeof(string));
        dt.Columns.Add("extra", typeof(string));
        dt.Columns.Add("orders", typeof(int));
        dt.Columns.Add("isNecessary", typeof(string));
        dt.Columns.Add("length", typeof(int));

        for (int i = 0; i < itemNameArray.Length; i++)
        {
            DataRow dr = dt.NewRow();
            dr["formName"] = formTitle;
            dr["fieldName"] = itemNameArray[i];
            dr["defaultValue"] = itemDefaultValuesArray[i];
            dr["type"] = typesArray[i];
            dr["extra"] = extrasArray[i];
            dr["orders"] = i;
            
            if (!"undefined".Equals(isNecessaryArray[i]))
            {
                dr["isNecessary"] = isNecessaryArray[i];
            }
            else
            {
                dr["isNecessary"] = "是";
            }
            if (!"undefined".Equals(lengthArray[i]))
            {
                dr["length"] = lengthArray[i];
            }

            dt.Rows.Add(dr);
        }

        // 先删除再新增
        List<string> sqlList = new List<string>();
        string sql = string.Format("delete from form_attribute where formName = {0}", formName);
        sqlList.Add(sql);
        sql = SqlHelper.GetInsertString(dt, "form_attribute");
        sqlList.Add(sql);
        SqlHelper.Exce(sqlList.ToArray());

        return "";
    }
}