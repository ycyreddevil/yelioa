using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

public partial class ExportExcelHelper : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UserInfo user = (UserInfo)Session["user"];

        JObject res = new JObject();
        
        Response.Clear();
        if (user==null)
        {
            res.Add("success", 0);
            res.Add("msg", "用户登录超时或无用户信息！");
            Response.Write(res.ToString());
            Response.End();
        }
        else
        {
            string fileCode = Request.Params["fileCode"];
            string path = Server.MapPath("~/tempExportFile");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (string.IsNullOrEmpty(fileCode))
            {
                string data = Request.Form["data"];
                string title = Request.Form["title"];                
                string headText = Request.Form["headText"];
                string colNames = Request.Form["columnName"];
                List<string> listHeadText = JsonHelper.DeserializeJsonToList<string>(headText);
                List<string> listcolNames = JsonHelper.DeserializeJsonToList<string>(colNames);
                DataTable dt = JsonHelper.Json2Dtb(data);
                DataTable newDt = new DataTable();
                foreach(string name in listcolNames)
                {
                    newDt.Columns.Add(name);
                }
                foreach(DataRow row in dt.Rows)
                {
                    DataRow newRow = newDt.NewRow();
                    for (int j = 0; j < listcolNames.Count; j++)
                    {
                        if (listcolNames[j] != "operate")
                            newRow[listcolNames[j]] = row[listcolNames[j]];                        
                    }
                    newDt.Rows.Add(newRow);
                }

                //for (int i = dt.Columns.Count - 1; i >= 0; i--)
                //{
                //    DataColumn c = dt.Columns[i];
                //    bool isContain = false;
                //    for (int j = 0; j < listcolNames.Count; j++)
                //    {
                //        if (c.ColumnName == listcolNames[j])
                //        {
                //            isContain = true;
                //            break;
                //        }
                //    }
                //    if (!isContain)
                //        dt.Columns.RemoveAt(i);
                //}
                string filecode = ValideCodeHelper.GetRandomCode(64);
                path = path + @"\" + filecode + ".xls";
                BytesToFile(ExcelHelperV2_0.Export(newDt, title, listHeadText.ToArray()).GetBuffer(), path);
                res.Add("success", 1);
                res.Add("fileCode", filecode);
                Response.Write(res.ToString());
                Response.End();
            }
            else
            {
                string fileName = Request.Params["fileName"];

                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Charset = "";
                Response.AppendHeader("Content-Disposition", "attachment;filename="
                    + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
                path = path + @"\" + fileCode + ".xls";
                Response.BinaryWrite(FileToBytes(path));
                File.Delete(path);
                Response.End();                
            }                        
        }
    }

    private byte[] FileToBytes(string fileName)
    {
        // 打开文件
        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        // 读取文件的 byte[]
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, bytes.Length);
        fileStream.Close();
        return bytes;
    }

    private void BytesToFile(byte[] bytes,string fileName)
    {
        // 把 byte[] 写入文件
        FileStream fs = new FileStream(fileName, FileMode.Create);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(bytes);
        bw.Close();
        fs.Close();
    }
}