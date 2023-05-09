using UnityEngine;
using System.Collections;

public enum MissionType
{
	LimitTime,
	Ranking,
	Coin,
	Crash,
	Item,
	BeHitByItem,
	SpeedUp,
	CrashCar
}


public class MissionData: IData {
	
	private MissionData()
	{
		InitData("Data/MissionData");
	}
	
	private static MissionData instance;
	public static MissionData Instance
	{
		get
		{
			if(instance == null)
				instance = new MissionData();
			
			return instance;
		}
	}

	public string GetDesc(int Id)
	{
		return GetProperty ("Desc", Id);
	}

	public string GetIcon(int Id)
	{
		//Debug.Log (Id + " "+ GetProperty("Icon", Id) );
		return GetProperty ("Icon", Id);
	}

	public MissionType GetMissionType(int Id)
	{	
		//Debug.Log (Id + " "+ GetProperty("MissionType", Id) );
		return (MissionType)System.Enum.Parse(typeof(MissionType), GetProperty("MissionType", Id));
	}

	public PlayerData.ItemType GetItemType(int Id)
	{
		return (PlayerData.ItemType)System.Enum.Parse(typeof(PlayerData.ItemType), GetProperty("ItemType", Id));;
	}

	public string GetGameDes(int Id)
	{
		return GetProperty ("GameDes", Id);
	}
}
