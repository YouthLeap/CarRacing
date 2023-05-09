using UnityEngine;
using System.Collections;

public class BuySkillData : IData {

	private BuySkillData()
	{
		InitData("Data/BuySkillData");
	}
	
	private static BuySkillData instance;
	public static BuySkillData Instance
	{
		get
		{
			if(instance == null)
				instance = new BuySkillData();
			
			return instance;
		}
	}
	
	public string GetBuyType(int Id)
	{
		return GetProperty("BuyType", Id);
	}

	public string GetCost(int Id)
	{
		return GetProperty("Cost", Id);
	}
}
