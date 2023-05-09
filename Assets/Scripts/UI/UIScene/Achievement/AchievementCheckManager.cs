using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementCheckManager :  MonoSingletonBase<AchievementCheckManager> {
	private int[] AchievementIds;
	private int DataCount;
	private const int AchievementCount = 12;
	private const int AchiecementLevelMax =3;
	private int[,] AchiecementArray= new int[AchievementCount,AchiecementLevelMax];
	HashSet<int> AchievementAlreadyDone = new HashSet<int>();

	void Start () {
		DontDestroyOnLoad(gameObject);
	}
	public override void Init ()
	{
		AchievementIds = PlayerData.Instance.GetAchievementIds ();
		if (AchievementIds == null || AchievementIds.Length == 0 || AchievementIds.Length != AchievementCount) {
			CreateAchievement ();
			AchievementIds = PlayerData.Instance.GetAchievementIds ();
		}

		DataCount = AchievementData.Instance.GetDataRow ();

		for(int i = 0; i < AchievementCount; i++)
			for(int j = 0; j < AchiecementLevelMax; j++)
				AchiecementArray[i,j] = -1;
		for(int i = 1; i <= DataCount; i++)
		{
			AchiecementArray[AchievementData.Instance.GetCheckType(i) - 1,AchievementData.Instance.GetLevel(i) - 1] = AchievementData.Instance.GetTargetNum(i);
		}
		AchievementAlreadyDone.Clear();
		int[] AlreadyGet = PlayerData.Instance.GetAchievementAreadyIsGet();
		if(AlreadyGet != null)
		{
			for(int i = 0; i < AlreadyGet.Length; i++)
			{
				AchievementAlreadyDone.Add(AlreadyGet[i]);
			}
		}
	}
	public void RefreshIds(int[] newIds)
	{
		AchievementIds = newIds;
	}
	public void RefreshAlreadyDone (int[] AlreadyDone)
	{
		AchievementAlreadyDone.Clear();
		if(AlreadyDone != null)
		{
			for(int i = 0; i < AlreadyDone.Length; i++)
			{
				AchievementAlreadyDone.Add(AlreadyDone[i]);
			}
		}
	}

	private void UpdateState(int id, int index ,float num)
	{
		float targetNum = AchievementData.Instance.GetTargetNum(id);
		float curNum = PlayerData.Instance.GetAchievementCurrentNum(index);
		if(curNum < targetNum)
		{
			if(num >= targetNum)
			{
				PlayerData.Instance.SetAchievementCurrentNum(index , targetNum);
			}
			else
			{
				PlayerData.Instance.SetAchievementCurrentNum(index , num);
			}
			//刷新数据
			if(GlobalConst.SceneName == SceneType.UIScene)
			{
				AchievementControllor.Instance.RefreshData ();
			}
		}
	}
	public void Check(AchievementType type, int num)
	{
		for(int i = 1; i <= DataCount; i++)
		{
			if(AchievementData.Instance.GetCheckType(i) == (int)type)
			{
				for(int j = 1;j <= AchiecementLevelMax;j++)
				{
					if(AchiecementArray[(int)type - 1,j - 1] == -1)
						break;
					int id = i + j -1;
					if(AchievementAlreadyDone.Contains(id))
						break;
					float origNum = 0;
					if(AchievementIds != null)
					{
						for(int k = 0; k < AchievementIds.Length; k++)
						{
							if(AchievementIds[k] == id)
							{
								origNum = PlayerData.Instance.GetAchievementCurrentNum(k);
								if(AchievementData.Instance.GetIsCumulative(id))
								{
									origNum += num;
									UpdateState(id, k, origNum);
								}
								else
									UpdateState(id, k, num);
							}
						}
					}
					break;
				}
			}
		}
	}

	/// <summary>
	/// 领取成就之后再次检测一遍成就是否达成
	/// </summary>
	public void CheckAgainAfterGetReward(int id)
	{
		AchievementType type = (AchievementType)System.Enum.Parse(typeof(AchievementType), AchievementData.Instance.GetCheckType(id).ToString());
		switch(type)
		{
		case AchievementType.Role_Count_Total:
			Check (type, PlayerData.Instance.GetModelState ().Length);
			break;
		
		case AchievementType.AnyRole_Level:
			int[] modelState = PlayerData.Instance.GetModelState ();
			int modelMaxLevel = 0, modelLevelTemp = 0;
			for (int i = 0; i < modelState.Length; i++) {
				modelLevelTemp = IDTool.GetModelLevel (modelState [i]);
				if (modelLevelTemp > modelMaxLevel)
					modelMaxLevel = modelLevelTemp;
			}
			Check (type, modelMaxLevel);
			break;
		}
	}

	public void CreateAchievement()
	{
		//dataCount 3的倍数
		int dataCount = AchievementData.Instance.GetDataRow ();
		int count = dataCount / 3;
		int[] initIds = new int[count];
		float[] initCurrentNum = new float[count];
		int[] initState = new int[count];
		int[] initAlreadyIsGet = new int[count];
		for(int i =1; i <= dataCount; i++)
		{
			if(i % 3 == 1)
			{
				initIds[i / 3] = i;
				initCurrentNum[i / 3] = 0;
				initState[i / 3] = 0;
				initAlreadyIsGet[i / 3] = 0;
			}
		}
		PlayerData.Instance.SetAchievementIds (initIds);
		PlayerData.Instance.SetAchievementCurrentNum (initCurrentNum);
		PlayerData.Instance.SetAchievementState (initState);
		PlayerData.Instance.SetAchievementAreadyIsGet (initAlreadyIsGet);
	}
}
