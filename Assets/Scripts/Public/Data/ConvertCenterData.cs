using UnityEngine;
using System.Collections;

public class ConvertCenterData : IData {

	private ConvertCenterData()
	{
		InitData("Data/ConvertCenterData");
	}

	private static ConvertCenterData instance;

	public static ConvertCenterData Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new ConvertCenterData();
			}

			return instance;
		}
	}

	public PlayerData.ItemType GetItemType(int Id)
	{
		return (PlayerData.ItemType)System.Enum.Parse(typeof(PlayerData.ItemType), GetProperty("TargetItemType", Id));
	}
	
	public PlayerData.ItemType GetFruitItemType(int Id)
	{
		return (PlayerData.ItemType)System.Enum.Parse(typeof(PlayerData.ItemType), GetProperty("ItemType", Id));
	}

	public string GetTargetName(int Id)
	{
		return GetProperty("TargetName", Id);
	}

	public string GetTargetIcon(int Id)
	{
		return GetProperty("TargetIcon", Id);
	}

	public string GetMaterialName(int Id)
	{
		return GetProperty("MaterialName", Id);
	}

	public string GetMaterialIcon(int Id)
	{
		return GetProperty("MaterialIcon", Id);
	}

	public int GetMaterialCount(int Id)
	{
		return int.Parse(GetProperty("MaterialCount", Id));
	}

	public string GetDescription(int Id)
	{
		return GetProperty("Description", Id);
	}
}
