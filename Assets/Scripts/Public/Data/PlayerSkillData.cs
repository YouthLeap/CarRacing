using UnityEngine;
using System.Collections;

public class PlayerSkillData : IData {

	private PlayerSkillData()
	{
		InitData("Data/PlayerSkillData");
	}
	
	private static PlayerSkillData instance;
	public static PlayerSkillData Instance
	{
		get
		{
			if(instance == null)
				instance = new PlayerSkillData();
			
			return instance;
		}
	}
	
	public string GetTitle(int Id)
	{
		return GetProperty ("Title", Id);
	}
	public string GetDesc(int Id)
	{
		return GetProperty ("Desc", Id);
	}
	public string GetIconName(int Id)
	{
		return GetProperty("IconName", Id);
	}
	public int GetCostCount(int Id)
	{
		return int.Parse(GetProperty ("CostCount", Id));
	}
	public PlayerData.ItemType GetCostType(int Id)
	{
		return (PlayerData.ItemType)System.Enum.Parse (typeof(PlayerData.ItemType), GetProperty ("CostType", Id));
	}
	public string GetTypeName(int Id)
	{
		return GetProperty("TypeName", Id);
	}

	public string GetItemType(int Id)
	{
		return GetProperty ("ItemType", Id);
	}
}
