using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏关卡数据
/// </summary>
public class GameLevelData : IData {

	private GameLevelData()
	{
		InitData("Data/GameLevelData");
	}
	
	private static GameLevelData instance;
	public static GameLevelData Instance
	{
		get
		{
			if(instance == null)
				instance = new GameLevelData();
			
			return instance;
		}
	}

	public void RefreshData()
	{
		InitData("Data/GameLevelData");
	}

	public string GetNode(int Id)
	{
		//Debug.Log (GetProperty("Note", Id));
		return GetProperty("Note", Id);
	}
	public string GetContent(int Id)
	{
		//Debug.Log (GetProperty("Content", Id));
		return GetProperty("Content", Id);
	}

	public List<string> GetGroupIDList(int id)
	{
		string content = GetContent (id);
		List<string> groupIDList = ConvertTool.StringToAnyTypeList<string>( content,'|');
		return groupIDList;
	}
	
	/// <summary>
	/// 关卡使用时间
	/// </summary>
	/// <returns>The use time.</returns>
	/// <param name="id">Identifier.</param>
	public int GetUseTime(int id)
	{
		int useTime=int.Parse( GetProperty ("UseTime",id) );
		return useTime;
	}
	public int GetCircleCount(int id)
	{
		int circleCount = int.Parse(GetProperty("CircleCount",id));
		return circleCount;
	}

	public string GetStrOpponent(int id)
	{
		string strOpponent= GetProperty("OpponentList",id);
		return strOpponent;
	}

	public string GetGameLevelModel(int id)
	{
		string gameModel= GetProperty("GameLevelModel",id);
		return gameModel;
	}


	public List<int> GetOpponentList(int id)
	{
		string strOpponent= GetProperty("OpponentList",id);
		List<int> list= ConvertTool.StringToAnyTypeList<int>(strOpponent,'|');
		return list;
	}
	
	public string GetSceneType(int id)
	{
		string typeStr = GetProperty ("SceneType",id);
		return typeStr;
	}

	public string GetRoadModelName(int id)
	{
		return GetProperty("RoadModelName",id);
	}

	public string GetRoadPointID(int id)
	{
		return GetProperty("RoadPointID",id);
	}

	public float GetLevelLen(int levelId)
	{

		float levelPathLength = 0;
		List<string> IDList = GameLevelData.Instance.GetGroupIDList (levelId);
		for (int i=0; i<IDList.Count; ++i) {
			levelPathLength += ItemGroupData.Instance.GetGroupLen (int.Parse (IDList[i]));
		}

		return levelPathLength;
	}
}

/// <summary>
/// 比赛模式
/// </summary>
public enum GameLevelModel{Rank,DualMeet,Weedout,LimitTime,WuJing}



