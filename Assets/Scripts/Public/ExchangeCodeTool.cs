using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class ExchangeCodeTool
{
	// 兑换码一共为14位，XXXXXXXXXX（10位兑换码时间戳）XXXX（4位活动数字编号）
	
	
	public const string codeFileName = "ExchangCode.txt";
	
	public const int codeLenght = 14;
	
	public static Dictionary<string, DateTime> codeEndTimeArray = new Dictionary<string, DateTime>();
	public static Dictionary<string, DateTime> codeStartTimeArray = new Dictionary<string, DateTime>();
	
	public static Dictionary<string, string> codeDescArray = new Dictionary<string, string>();
	public static Dictionary<string, string> codeRewardContentArray = new Dictionary<string, string>();
	
	public static List<string> codeTypeList = new List<string>();
	public static bool hasHengXian = true;

	/// <summary>
	/// 初始化所有活动的结束时间.
	/// </summary>
	public static void InitCodeData()
	{
		codeTypeList.Clear();
		codeStartTimeArray.Clear();
		codeEndTimeArray.Clear();
		codeDescArray.Clear();
		
		int exchangeCodeDataCount = ExchangeCodeData.Instance.GetDataRow();
		for(int i = 1; i <= exchangeCodeDataCount; i++)
		{
			string codeTypeTemp = ExchangeCodeData.Instance.GetExchangeCodeType(i);
			
			codeTypeList.Add(codeTypeTemp);
			codeStartTimeArray[codeTypeTemp] = Convert.ToDateTime(ExchangeCodeData.Instance.GetStartTime(i));
			codeEndTimeArray[codeTypeTemp] = Convert.ToDateTime(ExchangeCodeData.Instance.GetEndTime(i));
			codeDescArray[codeTypeTemp] = ExchangeCodeData.Instance.GetDesc(i);
			codeRewardContentArray [codeTypeTemp] = ExchangeCodeData.Instance.GetRewardContent (i);
			//Debug.Log(codeDescArray[codeTypeTemp]);
		}

	}
	
	public static void GenerateExchangeCode(string codeType, int generateCodeNum)
	{
	
		DateTime startTime = Convert.ToDateTime(codeStartTimeArray[codeType]);
		
		DateTime codeTime = startTime;
	
		StringBuilder codeStr = new StringBuilder();
		
		StringBuilder fileContent = new StringBuilder();
		
		for(int i = 0; i < generateCodeNum; i ++)
		{
			long intTime = TimeTool.DateTimeToTimestamp(codeTime);
			string timeStr = intTime.ToString();	//时间戳.
			StringBuilder timeStrBuild = new StringBuilder(timeStr);
			timeStrBuild.Append(codeType);
			
			//生成时间对应的随机字符.
			for(int j = 0; j < 10; j++)
			{
				timeStrBuild[j] = IntToCharRule((int)timeStr[j] - (int)'0');
				
				if((int)timeStrBuild[j] > (int)'a' && (int)timeStrBuild[j] < (int)'z')
				{
					int randomTemp = UnityEngine.Random.Range(0,10);
					if(randomTemp < 5)
						timeStrBuild[j] = timeStrBuild[j].ToString().ToUpper()[0];
				}
			}
			
			//活动编号
			for(int j = 0; j < 4; j++)
			{
				int index = j + 10;
				timeStrBuild[index] = IntToCharRule((int)codeType[j] - (int)'0');
				
				if((int)timeStrBuild[index] > (int)'a' && (int)timeStrBuild[index] < (int)'z')
				{
					int randomTemp = UnityEngine.Random.Range(0,10);
					if(randomTemp < 5)
						timeStrBuild[index] = timeStrBuild[index].ToString().ToUpper()[0];
				}
			}
			
			codeTime = codeTime.AddSeconds(1);
			
			if (hasHengXian) {
				timeStrBuild.Insert (4, "-");
				timeStrBuild.Insert (8, "-");
				timeStrBuild.Insert (12, "-");
			}
//			Debug.Log(timeStrBuild.ToString());
			fileContent.Append(timeStrBuild.ToString()+"\r\n");
		}
		
		FileTool.createORwriteFile(codeFileName, fileContent.ToString());
	}
	
	static string GetCodeTimeStr(string code)
	{
		return code.Substring(0, 10);
	}
	
	static string GetCodeTypeStr(string code)
	{
		return code.Substring(10, 4);
	}
	
	/// <summary>
	/// 检测兑换码是否可用.
	/// </summary>
	/// <returns><c>true</c>, if exchange code was checked, <c>false</c> otherwise.</returns>
	/// <param name="code">Code.</param>
	public static bool CheckExchangeCode(string code, out string rewardContent)
	{
		rewardContent = null;
		if(code.Length != codeLenght)
			return false;

		InitCodeData();
		
		string codeStr = ExchangeCodeToNormalString(GetCodeTimeStr(code));
		string codeTypeStr = ExchangeCodeToNormalString(GetCodeTypeStr(code));
		
		try
		{
			DateTime codeTime = TimeTool.TimestampToDateTime(codeStr);
			
			DateTime start = codeStartTimeArray[codeTypeStr];
			DateTime end = codeEndTimeArray[codeTypeStr];

			rewardContent = codeRewardContentArray[codeTypeStr];
			
			if(codeTime >= start && codeTime <= end)
				return true;
			else
				return false;	
		}
		catch
		{
			Debug.Log("The Wrong Code!");
			return false;
		}
	}
	
	static string ExchangeCodeToNormalString(string code)
	{
		StringBuilder timeStrBuild = new StringBuilder();
		
		for(int j = 0; j < code.Length; j++)
		{
			timeStrBuild.Append(CharToIntRule(code[j]));
		}
		
		return timeStrBuild.ToString();
	}
	
	/// <summary>
	/// 整型转换为随机的字符.
	/// </summary>
	/// <returns>The to char rule.</returns>
	/// <param name="intTemp">Int temp.</param>
	static char IntToCharRule(int intTemp)
	{
		int randomTemp = UnityEngine.Random.Range(0,10);
		if(randomTemp < 4)
			return (char)((int)'a' + intTemp);
		else if(randomTemp < 7)
			return (char)((int)'z' - intTemp);
		else
			return (char)(intTemp + (int)'0');
	}
	
	/// <summary>
	/// 字符转换为整型.
	/// </summary>
	/// <returns>The to int rule.</returns>
	/// <param name="charTemp">Char temp.</param>
	static int CharToIntRule(char charTemp)
	{
		if((int)charTemp <= (int)'9')
			return (int)charTemp - (int)'0';
		else if((int)charTemp < ((int)'a' + 10))
			return (int)charTemp - (int)'a';
		else
			return (int)'z' - (int)charTemp;
	}
	
	/// <summary>
	/// 检测当前输入的兑换码跟以前输过的是否一个类型的兑换码
	/// </summary>
	/// <returns><c>true</c>, if player has get one type code was checked, <c>false</c> otherwise.</returns>
	/// <param name="code">Code.</param>
	public static bool CheckPlayerHasGetOneTypeCode(string code)
	{
		string[] codes = PlayerData.Instance.GetExchangeCode();
		
		if(codes == null)
			return false;
		
		string codeTypeStr = ExchangeCodeToNormalString(GetCodeTypeStr(code));
		
		for(int i = 0; i < codes.Length; i++)
		{
			if(codes[i].Length != code.Length)
				continue;
				
			string codeTypeStrTemp = ExchangeCodeToNormalString(GetCodeTypeStr(codes[i]));
			if(codeTypeStr.CompareTo(codeTypeStrTemp) == 0)
				return true;
		}
		
		return false;
	}
}
