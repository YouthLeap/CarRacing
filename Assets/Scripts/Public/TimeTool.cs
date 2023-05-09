using UnityEngine;
using System;
using System.Collections;

public class TimeTool {

	
	/// <summary>
	/// 时间戳转为C#格式时间
	/// </summary>
	/// <param name="timeStamp">Unix时间戳格式</param>
	/// <returns>C#格式时间</returns>
	public static DateTime TimestampToDateTime(string timeStamp)
	{
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(timeStamp + "0000000");
		TimeSpan toNow = new TimeSpan(lTime);
		return dtStart.Add(toNow);
	}
	
	/// <summary>
	/// DateTime时间格式转换为Unix时间戳格式
	/// </summary>
	/// <param name="time"> DateTime时间格式</param>
	/// <returns>Unix时间戳格式</returns>
	public static long DateTimeToTimestamp(DateTime time)
	{
		DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		return (long)(time - startTime).TotalSeconds;
	}
}
