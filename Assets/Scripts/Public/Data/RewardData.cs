using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 物品数据表
/// </summary>
public class RewardData : IData {

	private RewardData()
	{
		InitData("Data/RewardData");
	}
	
	private static RewardData instance;
	public static RewardData Instance
	{
		get
		{
			if(instance == null)
				instance = new RewardData();
			
			return instance;
		}
	}

	public string GetName(int id)
	{
		return GetProperty("Name", id);
	}

	public PlayerData.ItemType GetItemType(int id)
	{
		return (PlayerData.ItemType)Enum.Parse (typeof(PlayerData.ItemType), GetProperty ("ItemType", id));
	}

	public string GetIconName(int id)
	{
		return GetProperty("IconName", id);
	}
}