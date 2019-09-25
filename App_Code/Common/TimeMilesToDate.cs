using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// TimeMilesToDate 的摘要说明
/// </summary>
public class TimeMilesToDate
{
    public TimeMilesToDate()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public static DateTime IntToDateTime(int timestamp)
    {
        return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(timestamp);
    }

    public static long DateTimeToInt(DateTime datetime)
    {
        return (datetime.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
    }
}