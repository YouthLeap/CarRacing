using UnityEngine;
using System.Collections;

/// <summary>
/// 道具信息配置表格
/// </summary>
public class PropConfigData : IData {


	private PropConfigData()
	{
		InitData("Data/PropConfigData");
	}
	
	private static PropConfigData instance;
	public static PropConfigData Instance
	{
		get
		{
			if(instance == null)
				instance = new PropConfigData();
			
			return instance;
		}
	}
	
	public void RefreshData()
	{
		InitData("Data/PropConfigData");
	}
	
	public int GetAddCoin(int Id)
	{
		return int.Parse (GetProperty("AddCoin",Id));
	}

	public float GetSkillTime(int Id)
	{
		return float.Parse ( GetProperty("SkillTime",Id) );
	}

	public float GetVertigoTime(int Id)
	{
		return float.Parse ( GetProperty("VertigoTime",Id) );
	}

	public string GetIconName(int Id)
	{
		return GetProperty ("IconName", Id);
	}

	public float GetPropRate(int Id)
	{
		return float.Parse (GetProperty ("PropRate", Id));
	}

	public float GetNPCPropRate(int Id)
	{
		return float.Parse (GetProperty ("NPCPropRate", Id));
	}
}