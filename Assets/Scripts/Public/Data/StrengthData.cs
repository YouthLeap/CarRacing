using UnityEngine;
using System.Collections;

public class StrengthData : IData {

	private StrengthData()
	{
		InitData("Data/StrengthData");
	}
	
	private static StrengthData instance;
	public static StrengthData Instance
	{
		get
		{
			if(instance == null)
				instance = new StrengthData();
			
			return instance;
		}
	}

	public int GetCount(int Id)
	{
		return int.Parse(GetProperty ("Count", Id));
	}
	public int GetCostCount(int Id)
	{
		return int.Parse(GetProperty ("CostCount", Id));
	}
	public string GetTypeName(int Id)
	{
		return GetProperty("TypeName", Id);
	}
	public PlayerData.ItemType GetItemType(int Id)
	{
		return (PlayerData.ItemType)System.Enum.Parse (typeof(PlayerData.ItemType), GetProperty ("ItemType", Id));
	}
}
