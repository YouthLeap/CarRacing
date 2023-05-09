using UnityEngine;
using System.Collections;
/// <summary>
/// 成就数据表.
/// </summary>
public class AchievementData : IData {
	private AchievementData()
	{
		InitData("Data/AchievementData");
	}
	
	private static AchievementData instance;
	public static AchievementData Instance
	{
		get
		{
			if(instance == null)
				instance = new AchievementData();
			
			return instance;
		}
	}
	public string GetTitleName(int Id)
	{
		return GetProperty("TitleName", Id);
	}

	public string GetIconName(int Id)
	{
		return GetProperty("IconName", Id);
	}

	public string GetDesc(int Id)
	{
		return GetProperty("Desc",Id);
	}

	public int GetCheckType(int Id)
	{
		return int.Parse (GetProperty ("CheckType", Id));
	}
	
	public bool GetIsCumulative(int Id)
	{
		return bool.Parse (GetProperty ("IsCumulative", Id));
	}
	
	public int GetLevel(int Id)
	{
		return int.Parse(GetProperty("Level",Id));
	}
	
	public int GetTargetNum(int Id)
	{
		return int.Parse(GetProperty("TargetNum",Id));
	}
	
	public int[] GetRewardIdArr(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("RewardId",Id).ToString(), '|');
	}
	public int[] GetRewardCountArr(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("RewardCount",Id).ToString(), '|');
	}
}