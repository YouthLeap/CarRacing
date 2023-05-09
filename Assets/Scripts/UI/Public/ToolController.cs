using UnityEngine;
using System;
using System.Collections;

public class ToolController {

	public static int GetTipCounnt(int modelId)
	{
		int modelLevel = IDTool.GetModelLevel (modelId);
		if (modelLevel == 0) {
			int costCount = ModelData.Instance.GetZhaohuanCost (modelId);
			string costTypeStr = ModelData.Instance.GetZhaohuanCostType (modelId);
			PlayerData.ItemType costType = (PlayerData.ItemType) Enum.Parse (typeof (PlayerData.ItemType), costTypeStr);
			int itemCount = PlayerData.Instance.GetItemNum (costType);
			if(costCount > itemCount)
				return 0;
			else
				return 1;
		}

		int coinCount = PlayerData.Instance.GetItemNum (PlayerData.ItemType.Coin);
		int jewelCount = PlayerData.Instance.GetItemNum (PlayerData.ItemType.Jewel);
		int maxModelId = IDTool.GetModelType (modelId) * 100 + ModelData.Instance.GetMaxLevel (modelId);
		int tipCount = 0;
		for(int i=modelId; i<maxModelId; ++i)
		{
			int costCount = ModelData.Instance.GetUpgradeCost (i + 1);
			string costTypeStr = ModelData.Instance.GetUpgradeCostType (i + 1);
			PlayerData.ItemType costType = (PlayerData.ItemType) Enum.Parse (typeof (PlayerData.ItemType), costTypeStr);
			if(costType == PlayerData.ItemType.Coin)
			{
				if(coinCount < costCount)
					break;
				tipCount ++;
				coinCount -= costCount;
			}
			else if(costType == PlayerData.ItemType.Jewel)
			{
				if(jewelCount < costCount)
					break;
				tipCount ++;
				jewelCount -= costCount;
			}
		}
		return tipCount;
	}

	public static Vector3 GetDropPosition(Vector3 o, float radius, float angle)
	{
		float x = radius * Mathf.Cos (Mathf.Deg2Rad * angle);
		float y = radius * Mathf.Sin (Mathf.Deg2Rad * angle);
		return o + new Vector3 (x, y, 0);
	}
}