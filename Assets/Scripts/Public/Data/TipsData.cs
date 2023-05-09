using UnityEngine;
using System.Collections;

public class TipsData : IData {
	private static TipsData instance;
	public static TipsData Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new TipsData();
			}
			return instance;
		}
	}

	private TipsData()
	{
		InitData("Data/TipsData");
		InitRandomSumData ();
	}

	public string GetContent(int id)
	{
		return GetProperty ("Content", id);
	}

	public int GetProportion(int id)
	{
		return int.Parse (GetProperty ("Proportion", id));
	}
	
	public string GetRandomContent()
	{
		return GetContent (GetRandomId ());
	}

	private int[] dataRandomSum;
	private int rowConut;
	private int randomId;
	void InitRandomSumData()
	{
		//初始化随机概率数值
		rowConut = GetDataRow ();
		int[] probability = new int[rowConut];
		dataRandomSum = new int[rowConut];
		for(int i=1; i<rowConut; ++i)
		{
			probability[i] = GetProportion (i);
			dataRandomSum[i] = 0;
			for(int j=1; j<=i; ++j)
			{
				dataRandomSum[i] += probability[j];
			}
		}
	}
	
	//随机生成奖励
	int GetRandomId()
	{
		int randomNumber = Random.Range (0, dataRandomSum [rowConut - 1]);
		for(int i=1; i<rowConut; ++i)
		{
			if(randomNumber < dataRandomSum[i])
			{
				return i;
			}
		}
		return 0;
	}
}
