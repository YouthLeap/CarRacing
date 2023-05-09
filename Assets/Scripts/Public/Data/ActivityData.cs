using UnityEngine;
using System.Collections;
/// <summary>
/// 成就数据表.
/// </summary>
public class ActivityData : IData {
	private ActivityData()
	{
		InitData("Data/ActivityData");
	}
	
	private static ActivityData instance;
	public static ActivityData Instance
	{
		get
		{
			if(instance == null)
				instance = new ActivityData();
			
			return instance;
		}
	}
	public string GetTitle(int Id)
	{
		return GetProperty("Title", Id);
	}

	public string GetIconName(int Id)
	{
		return GetProperty("IconName", Id);
	}

	public string GetDesc(int Id)
	{
		return GetProperty("Desc",Id);
	}
	public UISceneModuleType GetUISceneModuleType(int Id)
	{
		return (UISceneModuleType)System.Enum.Parse(typeof(UISceneModuleType),GetProperty ("SceneModuleType", Id));
	}
}