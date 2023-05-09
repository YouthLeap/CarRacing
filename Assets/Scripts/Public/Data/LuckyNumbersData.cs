using UnityEngine;
using System.Collections;

public class LuckyNumbersData : IData {

	private LuckyNumbersData()
	{
		InitData("Data/LuckyNumbersData");
	}
	
	private static LuckyNumbersData instance;
	public static LuckyNumbersData Instance
	{
		get
		{
			if(instance == null)
				instance = new LuckyNumbersData();
			
			return instance;
		}
	}
	
	public int GetAwardType(int id)
	{
		return int.Parse(GetProperty ("AwardType",id));
	}
	public int GetAwardCount(int id)
	{
		return int.Parse(GetProperty ("AwardCount",id));
	}
	public string GetDesc(int id)
	{
		return GetProperty ("Desc",id);
	}
	public LuckyNumbersType GetLuckyNumbersType(int id)
	{
		return (LuckyNumbersType)System.Enum.Parse(typeof(LuckyNumbersType),GetProperty("LuckyNumbersType",id));
	}
}
