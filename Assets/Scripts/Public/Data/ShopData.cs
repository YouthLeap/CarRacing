using UnityEngine;
using System.Collections;

public enum ShopItemType
{
	Null = 0,
	DoubleCoin,
	Coin,
	Jewel,
	Power
};
public class ShopData : IData {

	private ShopData()
	{
		InitData("Data/ShopData");
	}
	
	private static ShopData instance;
	public static ShopData Instance
	{
		get
		{
			if(instance == null)
				instance = new ShopData();
			
			return instance;
		}
	}

	public int GetPayId(int Id)
	{
		//计费相关的数据已统一到PayData
		return PayData.Instance.GetPayCode(GetPayJsonType(Id));
	}

	public string GetItemName(int Id)
	{
		return PayJsonData.Instance.GetGiftTitle(GetPayJsonType(Id));
	}
	public string GetGiftIcon(int Id)
	{
		return GetProperty("GiftIcon", Id);
	}

	public string GetIconName(int Id)
	{
		string content = PayJsonData.Instance.GetContent(GetPayJsonType(Id));
		return RewardData.Instance.GetIconName(int.Parse(content.Split('*')[0]));
	}

	public PayType GetPayJsonType(int Id)
	{
		return (PayType)System.Enum.Parse(typeof(PayType), GetProperty("PayJsonType", Id));
	}

	public int GetCount(int Id)
	{
		string content = PayJsonData.Instance.GetContent(GetPayJsonType(Id));
		return int.Parse(content.Split('*')[1]);
	}
	public float GetCost(int Id)
	{
		//计费相关的数据已统一到PayData
		return PayData.Instance.GetCost((int)GetPayJsonType(Id));
	}

	public PlayerData.ItemType GetItemType(int Id)
	{
		if (Id == 3)
			return PlayerData.ItemType.DoubleCoin;
		string content = PayJsonData.Instance.GetContent(GetPayJsonType(Id));
		return RewardData.Instance.GetItemType(int.Parse(content.Split('*')[0]));
	}

	public int GetIndex(PayType payType)
	{
		for (int i=1; i<=this.GetDataRow(); ++i) {
			if(this.GetPayJsonType(i) == payType)
				return i;
		}
		return 0;
	}
}
