using UnityEngine;
using System.Collections;

public class ComplainData : IData {

	public enum ComplainType
	{
		Phone = 1,
		Email = 2
	}

	private ComplainData()
	{
		InitData("Data/ComplainData");
	}
	
	private static ComplainData instance;
	public static ComplainData Instance
	{
		get
		{
			if(instance == null)
				instance = new ComplainData();
			
			return instance;
		}
	}

	private string GetDesc(int id)
	{
		return GetProperty("Desc", id);
	}

	public string GetDesc(ComplainType type)
	{
		return GetDesc((int)type);
	}

	private string GetContent(int id)
	{
		return GetProperty("Content", id);
	}

	public string GetContent(ComplainType type)
	{
		return GetContent((int)type);
	}
}
