using UnityEngine;
using System.Collections;

public class GamePlayingActivityData : IData {

	private GamePlayingActivityData()
	{
		InitData("Data/GamePlayingActivityData");
	}
	
	private static GamePlayingActivityData instance;
	public static GamePlayingActivityData Instance
	{
		get
		{
			if(instance == null)
				instance = new GamePlayingActivityData();
			
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
	public int GetScore(int id)
	{
		return int.Parse(GetProperty ("Score",id));
	}
}
