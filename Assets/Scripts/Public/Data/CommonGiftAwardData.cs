using UnityEngine;
using System.Collections;

public class CommonGiftAwardData : IData {

	private CommonGiftAwardData()
	{
		InitData("Data/CommonGiftAwardData");
	}
	
	private static CommonGiftAwardData instance;
	public static CommonGiftAwardData Instance
	{
		get
		{
			if(instance == null)
				instance = new CommonGiftAwardData();
			
			return instance;
		}
	}

	public PlayerData.ItemType[] GetAwardType (int Id)
	{
		string getAwardType = GetProperty ("RewardId", Id);
		string[] arr = getAwardType.Split ('|');
		PlayerData.ItemType[] typeArr = new PlayerData.ItemType[arr.Length];

		for(int i = 0; i < arr.Length; i ++)
		{
			typeArr[i] = RewardData.Instance.GetItemType (int.Parse(arr[i].Split('*')[0]));
		}

		return typeArr;
	}
	
	public int[] GetAwardCount (int Id)
	{
		string getAwardCount = GetProperty("RewardCount", Id);
		string[] arr = getAwardCount.Split('|');

		return ConvertTool.StringToAnyTypeArray<int>(ConvertTool.AnyTypeArrayToString<string>(arr, "|"), '|');
	}

	public string[] GetAwardIconName (int Id)
	{
		string getAwardType = GetProperty ("RewardId", Id);
		string[] arr = getAwardType.Split ('|');
		
		for(int i = 0; i < arr.Length; i ++)
		{
			arr[i] = RewardData.Instance.GetIconName(int.Parse(arr[i].Split('*')[0]));
		}
		
		return arr;
	}
}
