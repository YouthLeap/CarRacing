using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景道路点数据
/// </summary>
public class RoadData : IData {
	private RoadData()
	{
		InitData("Data/RoadData");
	}
	
	private static RoadData instance;
	public static RoadData Instance
	{
		get
		{
			if(instance == null)
				instance = new RoadData();
			
			return instance;
		}
	}

	public void RefreshData()
	{
		//Debug.Log ("RefreshData ");
		InitData("Data/RoadData");
	}


	public string GetNote(int Id)
	{
		return GetProperty("Note", Id);
	}
	public string GetType(int Id)
	{
		return GetProperty("Type", Id);
	}
	public string GetContent(int Id)
	{
		return GetProperty("Content", Id);
	}

	public List<Vector3> GetPointList(int Id)
	{
		List<Vector3> pointList = new List<Vector3>();

		string content = GetContent(Id);
		List<string> strList = ConvertTool.StringToAnyTypeList<string>(content,'^');
		for(int i=0;i<strList.Count;++i)
		{
			float[] posArray= ConvertTool.StringToAnyTypeArray<float>(strList[i],'*');
			Vector3 pos = new Vector3();
			pos.x=posArray[0];
			pos.y=posArray[1];
			pos.z =posArray[2];
			pointList.Add(pos);
		}

		return pointList;
	}

}
