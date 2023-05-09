using UnityEngine;
using System.Collections;

public class ClearanceRedPaperData : IData {

	private ClearanceRedPaperData()
	{
		InitData("Data/ClearanceRedPaperData");
	}
	
	private static ClearanceRedPaperData instance;
	public static ClearanceRedPaperData Instance
	{
		get
		{
			if(instance == null)
				instance = new ClearanceRedPaperData();
			
			return instance;
		}
	}
	public string GetTargetLevel(int Id)
	{
		return GetProperty("TargetLevel", Id);
	}
	public int[] GetAwardType(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("AwardType",Id).ToString(), '|');
	}
	public int[] GetAwardCount(int Id)
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("AwardCount",Id).ToString(), '|');
	}
}
