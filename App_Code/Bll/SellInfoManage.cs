using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// SellInfoManage 的摘要说明
/// </summary>
public class SellInfoManage
{
    public SellInfoManage()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DataSet GetData(string searchString)
    {
        return SellInfoSrv.GetData();
    }
}