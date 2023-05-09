using UnityEngine;
using System.Collections;

public class AboutUsData : IData {

	private static AboutUsData instance;
	public static AboutUsData Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new AboutUsData();
			}
			
			return instance;
		}
	}
	
	private AboutUsData()
	{
		InitData("Data/AboutUsData");;
	}
	
	public int GetAboutUsItemCount()
	{
		return GetDataRow();
	}
	
	public string GetAboutUsItem(int id)
	{
		return GetProperty("Content", id);
	}
}
