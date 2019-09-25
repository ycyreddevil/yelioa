using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// PostInfo 的摘要说明
/// </summary>
public class PostInfo
{
    public PostInfo()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    private uint _postId;
    public uint postId
    {
        get { return _postId; }
        set { _postId = value; }
    }

    private string _postName;
    public string postName
    {
        get { return _postName; }
        set { _postName = value; }
    }

    private string _company;
    public string company
    {
        get { return _company; }
        set { _company = value; }
    }

    private DateTime _creatTime; 
    public DateTime creatTime
    {
        get { return _creatTime; }
        set { _creatTime = value; }
    }

    private string _remark;
    public string remark
    {
        get { return _remark; }
        set { _remark = value; }
    }
}