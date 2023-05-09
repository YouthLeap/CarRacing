using UnityEngine;
using System.Collections;

public class SignInData : IData {
	private static SignInData instance;
	public static SignInData Instance
	{
		get
		{
			if(instance == null)
				instance = new SignInData();
			return instance;
		}
	}

	private SignInData()
	{
		InitData("Data/SignInData");
	}

	public string[] GetRewardIdArr(int id)
	{
		string str = GetProperty("RewardId",id);
		return ConvertTool.StringToAnyTypeArray<string>(str,'|');
	}

	public int[] GetRewardCountArr(int id)
	{
		string str = GetProperty("RewardCount",id);
		return ConvertTool.StringToAnyTypeArray<int>(str,'|');
	}

	public string GetTitle(int id)
	{
		return GetProperty("Title", id);
	}
}
