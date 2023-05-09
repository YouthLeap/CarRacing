using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum DailyMissionType
{
	UseProp,
	ClassModel,
	LevelModel
}

/// <summary>
/// Daily mission check manager.
/// </summary>
public class DailyMissionCheckManager : MonoSingletonBase<DailyMissionCheckManager> {
	
	public List<DailyMissionDataStruct> missionList = new List<DailyMissionDataStruct>();
	
	public int[] CurNum;
	public int[] GetState;
	public int[] CurMissionId;
	
	private int dailyMissionTypeCount = System.Enum.GetNames(typeof(DailyMissionType)).Length;
	private int dailyMissionCount = 3;
	
	static bool firstInit=true;
	
	void Start () {
		DontDestroyOnLoad(gameObject);
	}
	
	public override void Init ()
	{
		if (firstInit == false)
			return;
		else if (firstInit == true)
			firstInit = false;
		
		int missionCount = DailyMissionData.Instance.GetDataRow ();
		CurNum = new int[missionCount];
		GetState = new int[missionCount];
		CurMissionId = new int[dailyMissionCount];
		
		for (int i=1; i<=missionCount; ++i) {
			DailyMissionDataStruct itemData= new DailyMissionDataStruct();
			itemData.id = i;
			itemData.missionName= DailyMissionData.Instance.GetMissionName(i);
			itemData.missionDes = DailyMissionData.Instance.GetMissionDes(i);
			itemData.missionType =   (DailyMissionType) System.Enum.Parse(typeof(DailyMissionType), DailyMissionData.Instance.GetMissionType(i) );
			itemData.missionTarget = DailyMissionData.Instance.GetMissionTarget(i);
			itemData.missionCurCount =0;
			
			string rewardStr= DailyMissionData.Instance.GetMissionReward(i);
			string[] reward=rewardStr.Split('*');
			itemData.missionRewardType= int.Parse(reward[0]);
			itemData.missionRewardCount = int.Parse(reward[1]);
			itemData.isGetReward=false;
			itemData.icon = DailyMissionData.Instance.GetMissionIcon(i);
			
			missionList.Add(itemData);
		}
	}
	
	public void RreshNum()
	{
		int missionCount = DailyMissionData.Instance.GetDataRow ();
		for (int i=0; i<missionCount; ++i) {
			CurNum[i]=0;
			GetState[i]=0;
		}
		PlayerData.Instance.SetDailyMissionCurNum(CurNum);
		PlayerData.Instance.SetDailyMissionState(GetState);
	}
	
	/// <summary>
	/// 加载存档
	/// </summary>
	public void LoadNum()
	{
		CurNum = PlayerData.Instance.GetDailyMissionCurNum ();
		GetState = PlayerData.Instance.GetDailyMissionState ();
		CurMissionId = PlayerData.Instance.GetDailMissionCurMissionId ();
		
		for (int i=0; i<missionList.Count; ++i) {
			missionList[i].missionCurCount=CurNum[i];
			missionList[i].isGetReward= GetState[i]==0?false:true;
		}
		
	}
	
	/// <summary>
	/// Saves the state.
	/// </summary>
	public void SaveState()
	{
		for (int i=0; i<missionList.Count; ++i) {
			CurNum[i]= missionList[i].missionCurCount;
			GetState[i] = missionList[i].isGetReward?1:0;
		}
		PlayerData.Instance.SetDailyMissionCurNum(CurNum);
		PlayerData.Instance.SetDailyMissionState(GetState);
	}
	
	public int[] GetRandomId()
	{
//		Dictionary<DailyMissionType,List<int>> missionTypeDictionary = new Dictionary<DailyMissionType, List<int>> ();
//		
//		for (int i=0; i<missionList.Count; ++i) {
//			if(!missionTypeDictionary.ContainsKey(missionList[i].missionType))
//			{
//				List<int> tempList = new List<int> ();
//				missionTypeDictionary.Add(missionList[i].missionType,tempList);
//			}
//			missionTypeDictionary[missionList[i].missionType].Add(missionList[i].id);
//		}
//		//		foreach(DailyMissionType key in missionTypeDictionary.Keys)
//		//		{
//		//			for(int i = 0; i < missionTypeDictionary[key].Count; i++)
//		//			{
//		//				Debug.Log("key: " + key + "Id: " +missionTypeDictionary[key][i]);
//		//			}
//		//		}
//		
//		int[] ids = new int[dailyMissionCount];
//		//int[] randomMissionType = RandomCountInRange (dailyMissionCount,0,8);
//		int[] randomMissionType = RandomCountInRange (dailyMissionCount,0,dailyMissionTypeCount);
//		for(int j = 0; j < randomMissionType.Length; j++)
//		{
//			Debug.Log("DailyMissionType: " + (DailyMissionType)randomMissionType[j]);
//			ids[j] = missionTypeDictionary[(DailyMissionType)randomMissionType[j]][Random.Range(0,2)];
//			Debug.Log(ids[j]);
//		}

		int[] ids  = {1,3,5};
		return ids;
	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{
			GetRandomId();
		}
	}
	int[] RandomCountInRange(int count, int min, int max)
	{
		int[] arr = new int[count];
		List<int> listAll = new List<int>();
		for(int i = min; i < max; i++)
		{
			listAll.Add(i);
		}
		for(int j = 0; j < count; j++)
		{
			arr[j] = listAll[Random.Range(0,listAll.Count)];
			listAll.Remove(arr[j]);
		}
		return arr;
	}
	
	
	/// <summary>
	/// Changes to next level mission.
	/// </summary>
	/// <param name="missionId">Mission identifier.</param>
	public bool ChangeToNextLevelMission(int missionId)
	{
		bool isChange = false;
		int index = 0;
		
		for (int i=0; i<missionList.Count; ++i) {
			if(missionId == missionList[i].id)
			{
				index=i;
			}
		}
		
		if (index < missionList.Count - 1  ) {
			if(missionList[index].missionType == missionList[index+1].missionType)
			{
				isChange=true;
			}
		}
		
		if (isChange) {
			for(int i=0;i<this.CurMissionId.Length;++i)
			{
				if(CurMissionId[i] == missionId)
				{
					CurMissionId[i]=  missionList[index+1].id;
				}
			}
		}
		
		PlayerData.Instance.SetDailyMissionCurMissionId (CurMissionId);
		DailyMissionControllor.Instance.curMissionIds = this.CurMissionId;
		return isChange;
	}
	
	
	public void Check(DailyMissionType type,int num)
	{
		int missionCount = missionList.Count;
		for (int i=0; i<missionCount; ++i) {
			DailyMissionDataStruct itemData = missionList[i];
			if(itemData.missionType == type)
			{
				itemData.missionCurCount+=num;
				CurNum[i]=itemData.missionCurCount;
				//Debug.Log ("DailyMissionCheck:"+type.ToString()+itemData.missionCurCount);
			}
		}
		
		PlayerData.Instance.SetDailyMissionCurNum (CurNum);
	}
	
}
public class DailyMissionDataStruct{
	public int id;
	public string missionName;
	public string missionDes;
	public DailyMissionType missionType;
	public int missionCurCount;
	public int missionTarget;
	public int missionRewardCount;
	public int missionRewardType;
	public bool isGetReward=false;
	public string icon;
}