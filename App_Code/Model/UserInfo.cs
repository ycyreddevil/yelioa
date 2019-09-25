using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

/// <summary>
/// UserInfo 的摘要说明
/// </summary>
[Serializable]
public class UserInfo
{
    public UserInfo()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    public string wechatUserId { get; set; }

    private uint _userId;
    public uint userId
    {
        get { return _userId; }
        set { _userId = value; }
    }

    private string _userName;
    public string userName
    {
        get { return _userName; }
        set { _userName = value; }
    }

    //public string name;
    private string _mobilePhone;
    public string mobilePhone
    {
        get { return _mobilePhone; }
        set { _mobilePhone = value; }
    }

    private string _passWord;
    public string passWord
    {
        get { return _passWord; }
        set { _passWord = value; }
    }

    private string _sex;
    public string sex
    {
        get { return _sex; }
        set { _sex = value; }
    }

    private DateTime _birthday;
    public DateTime birthday
    {
        get { return _birthday; }
        set { _birthday = value; }
    }

    private DateTime _hiredate;
    public DateTime hiredate
    {
        get { return _hiredate; }
        set { _hiredate = value; }
    }

    private string _idNumber;
    public string idNumber
    {
        get { return _idNumber; }
        set { _idNumber = value; }
    }

    private string _nativePlace;
    public string nativePlace
    {
        get { return _nativePlace; }
        set { _nativePlace = value; }
    }

    private string _employeeCode;
    public string employeeCode
    {
        get { return _employeeCode; }
        set { _employeeCode = value; }
    }

    private string _address;
    public string address
    {
        get { return _address; }
        set { _address = value; }
    }

    //private string _post; 
    //public string post
    //{
    //    get { return _post; }
    //    set { _post = value; }
    //}

    private string _company;
    public string company
    {
        get { return _company; }
        set { _company = value; }
    }

    public int companyId { get; set; }

    //public string departmentId { get; set; }

    //private string _department;
    //public string department
    //{
    //    get { return _department; }
    //    set { _department = value; }
    //}


    private string _weiXin;
    public string weiXin
    {
        get { return _weiXin; }
        set { _weiXin = value; }
    }


    private string _isValid;
    public string isValid
    {
        get { return _isValid; }
        set { _isValid = value; }
    }


    private string _email;
    public string email
    {
        get { return _email; }
        set { _email = value; }
    }

    private string _remark;
    public string remark
    {
        get { return _remark; }
        set { _remark = value; }
    }

    //public string name;








    //public string location;
    //public string right;
}

[Serializable]
public class DepartmentPost
{
    public int Id { get; set; }
    public string wechatUserId { get; set; }
    public int userId { get; set; }
    public int departmentId { get; set; }
    public int postId { get; set; }
    public int isHead { get; set; }
}

public class UserAndDepartmentInfo
{
    public Dictionary<string, string> UserInfo { get; set; }
    public ArrayList DepartmentPost { get; set; }
}