using UnityEngine;
using System.Collections;

public class RandomPhoneNumData : IData {

	private RandomPhoneNumData()
	{
		InitData("Data/RandomPhoneNumData/PhoneNumData");
	}

	private static RandomPhoneNumData instance;
	public static RandomPhoneNumData Instance
	{
		get
		{
			if(instance == null)
				instance = new RandomPhoneNumData();
			
			return instance;
		}
	}

	public string GetPhoneNumber(int id)
	{
		return GetProperty("PhoneNum", id);
	}
}
