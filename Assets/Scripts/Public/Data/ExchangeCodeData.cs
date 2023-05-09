using UnityEngine;
using System.Collections;

public class ExchangeCodeData : IData {

	private static ExchangeCodeData instance;
	public static ExchangeCodeData Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new ExchangeCodeData();
			}
			
			return instance;
		}
	}
	
	private ExchangeCodeData()
	{
		InitData("Data/ExchangeCodeData");
	}
	
	public void RefreshData()
	{
		InitData("Data/ExchangeCodeData");
	}
	
	public bool CheckHasSameCode(string codeType)
	{
		for(int i = 1; i <= GetDataRow(); i ++)
		{
			if(GetExchangeCodeType(i).CompareTo(codeType) == 0)
				return true;
		}
		
		return false;
	}
	
	public string GetExchangeCodeType(int id)
	{
		return GetProperty("CodeType", id);
	}
	
	public string GetStartTime(int id)
	{
		return GetProperty("StartTime", id);
	}
	
	public string GetEndTime(int id)
	{
		return GetProperty("EndTime", id);
	}
	
	public string GetDesc(int id)
	{
		return GetProperty("Desc", id);
	}

	public string GetRewardContent(int id)
	{
		return GetProperty("RewardContent", id);
	}
}
