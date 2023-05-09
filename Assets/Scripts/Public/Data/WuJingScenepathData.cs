using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WuJingScenepathData : IData {

	private static WuJingScenepathData instance;
	public static WuJingScenepathData Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new WuJingScenepathData();
			}
			
			return instance;
		}
	}
	
	private WuJingScenepathData()
	{
		InitData("Data/WuJingScenepathData");;
	}
	
	public string GetSceneName(int id)
	{
		return GetProperty("SceneName",id);
	}

	public string GetPathModelName(int id)
	{
		return GetProperty("PathModelName",id);
	}
	public int GetPathID(int id)
	{
		return  int.Parse(GetProperty("PathId",id));
	}

	public float GetInitTime(int id)
	{
		//Debug.Log ( GetProperty("InitTime", 1));
		return float.Parse( GetProperty("InitTime", id) );
	}
	
	public float  GetReduceTime(int id)
	{
		//Debug.Log ( GetProperty("ReduceTime", 1));
		return float.Parse(  GetProperty("ReduceTime", id));
	}
	public float GetMinTime(int id)
	{
		return float.Parse(  GetProperty("MinTime", id));
	}
}
