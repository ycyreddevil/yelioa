using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// RMBHelper 的摘要说明
/// </summary>
public class RMBHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static decimal ToLowerCaseFunction(string value)
    {
        List<string> list = new List<string>();
        if (value.Contains("仟"))
        {
            var arr1 = Regex.Split(value, @"仟", RegexOptions.IgnoreCase);
            if (arr1 != null && arr1.Length > 0)
            {
                list.Add(arr1[0] + "000");
            }
            if (arr1.Length > 1)
            {
                value = arr1[1];
                if (value.Contains("佰"))
                {
                    var arr2 = Regex.Split(value, @"佰", RegexOptions.IgnoreCase);
                    if (arr2 != null && arr2.Length > 0)
                    {
                        list.Add(arr2[0] + "00");
                    }
                    if (arr2.Length > 1)
                    {
                        value = arr2[1];
                        if (value.Contains("拾"))
                        {
                            var arr3 = Regex.Split(value, @"拾", RegexOptions.IgnoreCase);
                            if (arr3 != null && arr3.Length > 0)
                            {
                                list.Add(arr3[0] + "0");
                            }
                            if (arr3.Length > 1)
                            {
                                list.Add(arr3[1]);
                            }
                        }
                        else //不含拾 拾
                        {
                            list.Add(value);
                        }
                    }
                }
                else //不含佰的情况
                {
                    if (value.Contains("拾"))
                    {
                        var arr3 = Regex.Split(value, @"拾", RegexOptions.IgnoreCase);
                        if (arr3 != null && arr3.Length > 0)
                        {
                            list.Add(arr3[0] + "0");
                        }
                        if (arr3.Length > 1)
                        {
                            list.Add(arr3[1]);
                        }
                    }
                    else //不含拾 拾
                    {
                        list.Add(value);
                    }
                }
            }
        }
        else  //不含仟的情况
        {
            if (value.Contains("佰"))
            {
                var arr2 = Regex.Split(value, @"佰", RegexOptions.IgnoreCase);
                if (arr2 != null && arr2.Length > 0)
                {
                    list.Add(arr2[0] + "00");
                }
                if (arr2.Length > 1)
                {
                    value = arr2[1];
                    if (value.Contains("拾"))
                    {
                        var arr3 = Regex.Split(value, @"拾", RegexOptions.IgnoreCase);
                        if (arr3 != null && arr3.Length > 0)
                        {
                            list.Add(arr3[0] + "0");
                        }
                        if (arr3.Length > 1)
                        {
                            list.Add(arr3[1]);
                        }
                    }
                    else //不含拾
                    {
                        list.Add(value);
                    }
                }
            }
            else //不含佰的情况
            {
                if (value.Contains("拾"))
                {
                    var arr3 = Regex.Split(value, @"拾", RegexOptions.IgnoreCase);
                    if (arr3 != null && arr3.Length > 0)
                    {
                        list.Add(arr3[0] + "0");
                    }
                    if (arr3.Length > 1)
                    {
                        list.Add(arr3[1]);
                    }
                }
                else //不含拾
                {
                    list.Add(value);
                }
            }
        }
        decimal result = 0;
        if (list != null && list.Count > 0)
        {
            foreach (var item in list)
            {
                decimal number = 0;
                if (decimal.TryParse(item, out number))
                {
                    result += number;
                }
            }
        }
        return result;
    }
    /// <summary>
    /// 将人民币金额转为数字格式
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal ToLowerCase(string str)
    {
        NameValueCollection filterList = new NameValueCollection();
        filterList.Add("零", "0");
        filterList.Add("壹", "1");
        filterList.Add("贰", "2");
        filterList.Add("叁", "3");
        filterList.Add("肆", "4");
        filterList.Add("伍", "5");
        filterList.Add("五", "5");
        filterList.Add("陆", "6");
        filterList.Add("柒", "7");
        filterList.Add("捌", "8");
        filterList.Add("玖", "9");
        filterList.Add("元", "圆");
        filterList.Add("正", "整");
        filterList.Add("千", "仟");
        filterList.Add("整", "");
        for (int i = 0; i < filterList.Count; i++)
        {
            string key = filterList.GetKey(i);
            if (str.Contains(key))
            {
                str = str.Replace(key, filterList.Get(i));
            }
        }

        List<decimal> decimalList = new List<decimal>();
        decimal tempValue = 0;
        //拆分
        if (str.Contains("亿"))
        {
            var arr = Regex.Split(str, @"亿", RegexOptions.IgnoreCase);
            tempValue = ToLowerCaseFunction(arr[0]);
            if (arr.Length > 1)
            {
                str = arr[1];
            }
            decimalList.Add(tempValue * 100000000);
        }
        if (str.Contains("万"))
        {
            var arr = Regex.Split(str, @"万", RegexOptions.IgnoreCase);
            tempValue = ToLowerCaseFunction(arr[0]);
            if (arr.Length > 1)
            {
                str = arr[1];
            }
            decimalList.Add(tempValue * 10000);
        }
        if (str.Contains("圆"))
        {
            var arr = Regex.Split(str, @"圆", RegexOptions.IgnoreCase);
            tempValue = ToLowerCaseFunction(arr[0]);
            if (arr.Length > 1)
            {
                str = arr[1];
            }
            decimalList.Add(tempValue);
        }
        if (str.Contains("角") || str.Contains("分"))
        {
            str = str.Replace("角", "").Replace("分", "");
            decimal.TryParse(str, out tempValue);
            decimalList.Add(tempValue / 100);
        }
        return Math.Round(decimalList.Sum(), 2); //保留两位小数
    }
}