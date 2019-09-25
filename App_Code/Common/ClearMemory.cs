
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
/// <summary>
/// ClearMmemories 的摘要说明
/// </summary>
public class ClearMemory
{
    public ClearMemory()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static void Clear(DataSet obj)
    {
        if (obj != null)
        {
            obj.Clear();
            obj = null;
            System.GC.Collect();
        }        
    }

    public static void Clear(DataTable obj)
    {
        if (obj != null)
        {
            obj.Clear();
            obj = null;
            System.GC.Collect();
        }
    }

    public static void Clear(ArrayList obj)
    {
        if (obj != null)
        {
            obj.Clear();
            obj = null;
            System.GC.Collect();
        }
    }
}