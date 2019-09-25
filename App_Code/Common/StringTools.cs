using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

/// <summary>
/// StringTools 的摘要说明
/// </summary>
public class StringTools
{
    public StringTools()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 判断字符串中是否包含中文
    /// </summary>
    /// <param name="str">需要判断的字符串</param>
    /// <returns>判断结果</returns>
    public static bool HasChinese(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;
        return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
    }

    public static bool IsNumeric(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;
        return Regex.IsMatch(value, @"^\d*[.]?\d*$");
    }
    public static bool IsInt(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;
        return Regex.IsMatch(value, @"^[+-]?\d*$");
    }
    public static bool IsUnsign(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;
        return Regex.IsMatch(value, @"^\d*[.]?\d*$");
    }

    public static bool isTel(string strInput)
    {
        if (string.IsNullOrEmpty(strInput))
            return false;
        return Regex.IsMatch(strInput, @"\d{3}-\d{8}|\d{4}-\d{7}");
    }

    public static int CountNumbers(string val)
    {
        int len = val.Length;
        int count = 0;
        for(int i = 0; i < len; i++)
        {
            if (Char.IsDigit(val[i]))
                count++;
        }
        return count;
    }

    public static string JustKeepLetters(string input)
    {
        string res = "";
        if (string.IsNullOrEmpty(input))
            return res;
        input = input.ToUpper();
        foreach(char c in input)
        {
            if (c >= 'A' && c <= 'Z')
                res += c.ToString();
        }
        return res;
    }

    public static string JustKeepNumbers(string input)
    {
        string res = "";
        if (string.IsNullOrEmpty(input))
            return res;
        foreach (char c in input)
        {
            if ((c >= '0' && c <= '9')|| c == '.' || c=='-')
                res += c.ToString();
        }
        return res;
    }

    public static double StringToDouble(string value)
    {
        if(value.Contains("-"))
        {
            return - Convert.ToDouble(value.Substring(1, value.Length - 1));
          
        }
        else
        {
            return Convert.ToDouble(value);
        }
    }
}