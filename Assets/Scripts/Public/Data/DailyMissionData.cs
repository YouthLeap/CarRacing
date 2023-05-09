using UnityEngine;
using System.Collections;

public class DailyMissionData : IData {

	private DailyMissionData()
	{
		InitData("Data/DailyMissionData");
	}
	
	private static DailyMissionData instance;
	public static DailyMissionData Instance
	{
		get
		{
			if(instance == null)
				instance = new DailyMissionData();
			
			return instance;
		}
	}
	
	public void RefreshData()
	{
		InitData("Data/DailyMissionData");
	}
	

	public string GetMissionName(int id)
	{
		return GetProperty("missionName", id);
	}

	public string GetMissionDes(int id)
	{
		return GetProperty("missionDes", id);
	}

	public string GetMissionType(int id)
	{
		return GetProperty("missionType", id);
	}

	public int GetMissionTarget(int id)
	{
		return  int.Parse( GetProperty("missonTarget", id) );
	}


	public string GetMissionReward(int id)
	{
		return  GetProperty("missionReward", id) ;
	}

	public string GetMissionIcon(int id)
	{
		return  GetProperty("missionIcon", id) ;
	}

}
