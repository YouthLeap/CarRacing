using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Level data.
/// </summary>
public class LevelData : IData {
	
	private LevelData()
	{
		InitData("Data/LevelData");
	}
	
	private static LevelData instance;
	public static LevelData Instance
	{
		get
		{
			if(instance == null)
				instance = new LevelData();
			
			return instance;
		}
	}
	
	public int[] GetDropItemRewardIdList(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("DropItemRewardIdList", Id), '|');
	}
	public int[] GetDropItemRewardIdListCount(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("DropItemRewardIdListCount", Id), '|');
	}
	public int[] GetMissionIdList(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("MissionIdList", Id), '|');
	}

	public int[] GetMissionNumList(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("MissionNumList", Id), '|');
	}
	
	public string[] GetLittleMonsterPropPercent(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<string>(GetProperty("LittleMonsterPropPercent", Id), '|');
	}
	
	public string[] GetBigMonsterPropPercent(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<string>(GetProperty("BigMonsterPropPercent", Id), '|');
	}

	public bool GetFreeYizeGiftState(int Id)
	{
		if(PlayerData.Instance.GetYizeGiftGetedState(Id))
			return false;
		if(!PayJsonData.Instance.GetIsActivedState(PayType.FreeInnerGameGift))
			return false;
			
		int[] levels = PayJsonData.Instance.GetLevelsArrayToShow(PayType.FreeInnerGameGift);
		for(int i = 0; i < levels.Length; i ++)
		{
			if(Id == levels[i])
				return true;
		}
		
		return false;
	}

	public bool GetYizeGiftState(int Id)
	{
		if(PlayerData.Instance.GetYizeGiftGetedState(Id))
			return false;
		if(!PayJsonData.Instance.GetIsActivedState(PayType.InnerGameGift))
			return false;

		int[] levels = PayJsonData.Instance.GetLevelsArrayToShow(PayType.InnerGameGift);
		for(int i = 0; i < levels.Length; i ++)
		{
			if(Id == levels[i])
				return true;
		}
		
		return false;
	}

	public bool GetPassGameGiftState(int Id)
	{
		if(PlayerData.Instance.GetPassGameGiftGetedState(Id))
			return false;
		if(!PayJsonData.Instance.GetIsActivedState(PayType.RewardsGift3Stars))
			return false;

		int[] levels = PayJsonData.Instance.GetLevelsArrayToShow(PayType.RewardsGift3Stars);
		for(int i = 0; i < levels.Length; i ++)
		{
			if(Id == levels[i])
				return true;
		}

		return false;
	}

	/// <summary>
	/// 三星奖励字符串, 格式 name:ItemType:Icon:count|name:ItemType:Icon:count
	/// </summary>
	/// <returns>The level award star string.</returns>
	/// <param name="starNum">Star number.</param>
	/// <param name="id">Identifier.</param>
	public string GetLevelAwardStarStr(int starNum,int id)
	{
		string tempStr = "";
		string idCountStr = GetProperty("LevelAward" +  starNum + "Star", id);
		string[] idCountArr = idCountStr.Split('|');
		
		for(int i = 0; i < idCountArr.Length; i ++)
		{
			string[] tempIdCount = idCountArr[i].Split('=');
			string tempId = tempIdCount[0];
			string tempCount = tempIdCount[1];
			
			tempStr += RewardData.Instance.GetName(int.Parse(tempId));
			tempStr += '=' + RewardData.Instance.GetItemType(int.Parse(tempId)).ToString();
			tempStr += '=' + RewardData.Instance.GetIconName(int.Parse(tempId));
			tempStr += '=' + tempCount;
			
			if(i != idCountArr.Length -1)
			{
				tempStr += '|';
			}
		}
		
		return tempStr;
	}
	public float GetLevelTotalTime(int id)
	{
		return float.Parse (GetProperty("LevelTotalTime",id));
	}
	public float GetMechaCoolTimeReduceValue(int id)
	{
		return float.Parse (GetProperty("MechaCoolTimeReduceValue",id));
	}

	public string GetLevelIcon(int id)
	{
		string str = GetProperty ("LevelIcon",id);
		return str;
	}

	public float GetAheadDis(int id)
	{
		float dis = float.Parse(GetProperty("AheadDis",id));
		return dis;
	}

	public float GetLastDis(int id)
	{
		float dis=float.Parse(GetProperty("LastDis",id));
		return dis;
	}
}
