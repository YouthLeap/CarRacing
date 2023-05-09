using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WuJingConfigData : IData {

	private static WuJingConfigData instance;
	public static WuJingConfigData Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new WuJingConfigData();
			}
			
			return instance;
		}
	}
	
	private WuJingConfigData()
	{
		InitData("Data/WuJingConfigData");;
	}
	
	public float GetInitTime()
	{
		//Debug.Log ( GetProperty("InitTime", 1));
		return float.Parse( GetProperty("InitTime", 1) );
	}
	
	public float  GetReduceTime()
	{
		//Debug.Log ( GetProperty("ReduceTime", 1));
		return float.Parse(  GetProperty("ReduceTime", 1));
	}
	public float GetMinTime()
	{
		return float.Parse(  GetProperty("MinTime", 1));
	}

	public List<int> GetOpponentList()
	{
		string str = GetProperty("OpponentList",1);
		List<int> oppoList = ConvertTool.StringToAnyTypeList<int>(str,'|');
		return oppoList;
	}

	public List<string> GetIDList()
	{
		string str = GetProperty ("IDList",1);
		List<string> idList=  ConvertTool.StringToAnyTypeList<string>(str, '|');
		return idList;
	}

   public int GetJewelCountByDistance(float distance)
	{
		string disStr= GetProperty("RewardDis",1);
		List<int> disList = ConvertTool.StringToAnyTypeList<int>(disStr,'|');
		string jewelStr = GetProperty("RewardJewelCount",1);
		List<int> jewelList = ConvertTool.StringToAnyTypeList<int>(jewelStr,'|');

		int index=0;
		for(int i=disList.Count-1;i>=0;--i)
		{
			if(distance>disList[i])
			{
				index=i;
				break;
			}
		}

		index = Mathf.Clamp(index,0,jewelList.Count);
		return jewelList[index];
	}

	public int OverTakeInitExp
	{
		get{
			return 20;
		}
	}

	public int OverTakeAddExp
	{
		get{
			return 5;
		}
	}
	public int OverTakeMaxExp
	{
		get{
			return 40;
		}
	}
}
