using UnityEngine;
using System.Collections;

public class TurnplateData : IData {

	private TurnplateData()
	{
		InitData("Data/TurnplateData");
	}

	private static TurnplateData instance;
	public static TurnplateData Instance
	{
		get
		{
			if(instance == null)
				instance = new TurnplateData();

			return instance;
		}
	}

	#region  GetData
	public int GetItemCount()
	{
		return GetDataRow();
	}

	public string GetAwardName(int id)
	{
		return GetProperty("AwardName", id + 1);
	}

	public string GetItemType(int id)
	{
		return GetProperty("ItemType", id + 1);
	}

	public int GetAwardCount(int id)
	{
		return int.Parse(GetProperty("AwardCount", id + 1));
	}

	public string GetSpriteName(int id)
	{
		return GetProperty("SpriteName", id + 1);
	}

	public int GetProbability(int id)
	{
		return int.Parse(GetProperty("Probability", id + 1));
	}

	#endregion

	#region SetData
	public void AddAward(int id)
	{
		string type = GetItemType(id);
		int count = GetAwardCount(id);

		if(type.CompareTo("HuaFei") == 0)
		{
			float huaFei = PlayerData.Instance.GetHuaFeiAmount();
			huaFei += count;
			
			PlayerData.Instance.SetHuaFeiAmount(huaFei);
			return;
		}

		PlayerData.Instance.AddItemNum(type, count);

		//自定义事件.
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Turnplate, "类型_" + type.ToString (), "数量_" + count.ToString (),"Level", PlayerData.Instance.GetSelectedLevel ().ToString ());
	}

	#endregion
}
