using UnityEngine;
using System.Collections;


public enum PayType: int
{
	NewPlayerGift = 1,
	MultiCoin = 2,
	RewardsGift3Stars = 3,
	RewardsGift4Stars = 4,
	PowerGift = 5,
	CoinGift = 6,
	Jewel20Gift = 7,
	JewelGift = 8,
	RewardsGift5Stars = 9,
	DiscountGift = 10,
	CharactersGift = 11,
	OneKey2FullLV = 12,
	InnerGameGift = 13,
	Reborn = 14,
	SheildGift = 15,
	MonthCardGift = 16,
	ChangWanGift = 17,
	Shop = 18,
	FreeInnerGameGift = 19,
	FreeReborn = 20,
	AoteZhaohuan = 21,
    RemoveAds = 1000,
};

public class PayData : IData {

	private PayData()
	{
		InitData("Data/PayData");
	}

	private static PayData instance;
	public static PayData Instance
	{
		get
		{
			if(instance == null)
				instance = new PayData();
			
			return instance;
		}
	}

	public int GetPayCode(int id)
	{
		return int.Parse(GetProperty("PayCode", id));
	}

	public int GetPayCode(PayType type)
	{
		return GetPayCode((int)type);
	}

	public string GetDesc(int id)
	{
		return GetProperty("Desc", id);
	}

	public float GetCost(int id)
	{
		//电信价格限定
//		if(PlatformSetting.Instance.PlatformType == PlatformItemType.DianXin)
//			return Mathf.Min(PayData.Instance.GetCost(id), 20);

		//return float.Parse(GetProperty("Cost", id));

		return PayBuild.PayBuildPayManage.Instance.GetProductPrice (id);
	}

	public float GetCost(PayType type)
	{
		return GetCost((int)type);
	}

	public string GetDesc(PayType type)
	{
		return GetDesc((int)type);
	}

    public string GetPayTypeStr(int id)
    {
        return GetProperty("PayType", id);
    }

	public PayType GetPayType(int id)
	{
		return (PayType)System.Enum.Parse(typeof(PayType), GetProperty("PayType", id));
	}

	public PayType GetPayType(PayType type)
	{
		return GetPayType((int)type);
	}

	public string GetPayBtName(int id)
	{
		return GetProperty("BuyBtName", id);
	}

	public string GetPayBtName(PayType type)
	{
		return GetPayBtName((int)type);
	}
}
