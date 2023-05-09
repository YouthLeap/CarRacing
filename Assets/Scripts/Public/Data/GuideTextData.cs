using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideTextData : IData {

	private static GuideTextData instance;
	public static GuideTextData Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GuideTextData();
			}
			
			return instance;
		}
	}
	
	private GuideTextData()
	{
		InitData("Data/GuideTextData");;
	}
	
	public bool  GetIsShow(int id)
	{
		string str = GetProperty("IsShow",id);
		bool isShow = bool.Parse(str);
		return isShow;
	}

	public string GetText(int id)
	{
		return GetProperty("Text",id);
	}
	public Vector3 GetPosition(int id)
	{
		string str=GetProperty("Position",id);
		string[] posStr=str.Split('*');
		Vector3 pos = new Vector3(float.Parse(posStr[1]),  float.Parse(posStr[2]),  float.Parse(posStr[3]));
		return pos;
	}

	public float GetXScale(int id)
	{
		return float.Parse(GetProperty("XScale",id));
	}
}
