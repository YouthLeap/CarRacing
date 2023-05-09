using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// GamePlayerUI 内部的关卡任务模块
/// 分离之前的代码
/// </summary>
public class MissionBoard : MonoBehaviour {

	public static MissionBoard Instance=null;

	public List<Transform> tranMissionList;

	public bool isFailMission=false;

	public int minRank =5;
	
	private int iCurLevel;
	
	private List<MissionStruct> missionList= new List<MissionStruct>();
	private MissionType curMissionType;



	private bool isUpdateLimitTimeCheck=false;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		if(PlayerData.Instance.IsWuJinGameMode())
		{
			gameObject.SetActive(false);
			return;
		}
		gameObject.SetActive(true);
		InitMission();
	}

	
	private void InitMission()
	{
		isUpdateLimitTimeCheck = false;

		iCurLevel = PlayerData.Instance.GetSelectedLevel();
		int[] missionCountArr = LevelData.Instance.GetMissionNumList(iCurLevel);
		int[] missionIdArr = LevelData.Instance.GetMissionIdList (iCurLevel);

		for (int i = 0; i < missionIdArr.Length; i ++) {
			MissionStruct mission =new MissionStruct();
			mission.id = missionIdArr[i];
			mission.maxNum =  (missionCountArr[i]);
			mission.curNum =0;
			mission.icon =  MissionData.Instance.GetIcon(missionIdArr[i]);
			mission.type = MissionData.Instance.GetMissionType(missionIdArr[i]);
			mission.showTrans = tranMissionList[i];
			mission.isComplete = false;

			mission.countText = mission.showTrans.Find("Count").GetComponent<tk2dTextMesh>();
			mission.desText = mission.showTrans.Find("Desc").GetComponent<EasyFontTextMesh>();

			this.missionList.Add(mission);
		

			InitMissionItem(mission);
		}

		for (int k=0; k<this.missionList.Count; ++k) {
		    
			MissionType missionType = missionList[k].type;
			switch(missionType)
			{
			case MissionType.LimitTime:
				isUpdateLimitTimeCheck = true;
				UpdateLimitTimeCheck();
				break;
			case MissionType.Ranking:
				GameData.Instance.RankChangeEvent+= RankChange;
				minRank = (int)missionList[k].maxNum;
				break;
			case MissionType.Coin:
				GameData.Instance.CoinChangeEvnet+=CoinChange;
				break;
			case MissionType.Crash:
				GameData.Instance.CrashChangeEvent+=CrashChange;
				break;
			case MissionType.CrashCar:
				GameData.Instance.CrashCarChangeEvent+=CrashCarChange;
				break;
			case MissionType.Item:
				GameData.Instance.ItemUseChangeEvent += ItemUseChange;
				break;
			case MissionType.BeHitByItem:
				GameData.Instance.BeHitChangeEvent += BeHitChange;
				break;
			case MissionType.SpeedUp:
				GameData.Instance.SpeedUpChangeEvent += SpeedUpChange;
				break;
			}
		}
	}

	#region  关卡任务 变化

	void CoinChange(int num)
	{
		MissionStruct mission = GetMissionByType (MissionType.Coin);
		mission.curNum = num;
		SetMissionItemData (mission);
	}

	void CrashChange(int num)
	{
		MissionStruct mission = GetMissionByType (MissionType.Crash);
		mission.curNum = num;
		SetMissionItemData (mission,false);
	}

	void ItemUseChange(int num)
	{
		MissionStruct mission = GetMissionByType (MissionType.Item);
		mission.curNum = num;
		SetMissionItemData (mission);
	}

	void BeHitChange(int num)
	{
		MissionStruct mission = GetMissionByType (MissionType.BeHitByItem);
		mission.curNum = num;
		SetMissionItemData (mission,false);
	}

	void SpeedUpChange(int num)
	{
		MissionStruct mission = GetMissionByType (MissionType.SpeedUp);
		mission.curNum = num;
		SetMissionItemData (mission);
	}

	void CrashCarChange(int num)
	{
		MissionStruct mission = GetMissionByType (MissionType.CrashCar);
		mission.curNum = num;
		SetMissionItemData (mission);
	}

	void RankChange(int num)
	{
		MissionStruct mission = GetMissionByType (MissionType.Ranking);
		mission.curNum = num;
		SetMissionItemData (mission,false);
	}


	#endregion



	void Update()
	{
		if (isUpdateLimitTimeCheck && GameData.Instance.IsPause == false) {
			UpdateLimitTimeCheck();
		}
	}

	void UpdateLimitTimeCheck()
	{
		MissionStruct mission = GetMissionByType (MissionType.LimitTime);
		if (mission.isComplete == false) {
			float leftSec = CarManager.Instance.totalUseTime - CarManager.Instance.playerUseTime;
			mission.curNum = leftSec;
			SetMissionItemData(mission,false);
		}
	}

	void InitMissionItem(MissionStruct mission)
	{
		Transform tranMission = mission.showTrans;
		int missionID = mission.id;
		float iMissionNum = mission.maxNum;
		tranMission.gameObject.SetActive(true);
		mission.countText.text = "0I" + (int)iMissionNum;
		mission.desText.text = MissionData.Instance.GetGameDes(missionID);

	}

	/// <summary>
	/// 设置UI的数据
	/// </summary>
	/// <param name="tranMission">Tran mission.</param>
	/// <param name="curNum">Current number.</param>
	/// <param name="maxNum">Max number.</param>
	/// <param name="isWithComplete">If set to <c>true</c> is with complete.</param>
	void SetMissionItemData(MissionStruct mission,bool isWithComplete=true)
	{

		Transform tranMission = mission.showTrans;
		float curNum = mission.curNum;
		float maxNum = mission.maxNum;

		/*格式化时间显示*/
		if (mission.type == MissionType.LimitTime ) {

			int secondValue = Mathf.FloorToInt(mission.curNum);
			int msValue =   Mathf.FloorToInt((mission.curNum - secondValue)*100);

			string str = secondValue+":"+msValue.ToString("00");
			tranMission.gameObject.SetActive(true);
			mission.countText.text = str; //+ "I" + (int)maxNum;
		} else {
		
			tranMission.gameObject.SetActive(true);
			mission.countText.text =  (int)curNum + "I" + (int)maxNum;
		}
	

		if (isWithComplete == false) {
			return;
		}

		if (curNum >=maxNum && mission.isComplete==false) {

			mission.isComplete = true;

			Transform tranComplete= tranMission.Find("Complete");
			tranComplete.gameObject.SetActive(true);
			tranComplete.localScale = new Vector3(2f, 2f, 2f);
			tranComplete.DOScale(Vector3.one, GlobalConst.PlayTime).SetEase(Ease.OutBack);

		    
			//完成任务
			GameData.Instance.completeMissionIds.Add(mission.id);
			GameData.Instance.StarCount ++;
			Debug.Log(mission.type.ToString());
		}
	}


	/// <summary>
	/// 关卡完成时检查 任务是否完成
	// 检测 用时少于 撞车少于的任务
	/// </summary>
	public void CheckMission()
	{
		for (int i=0; i<this.missionList.Count; ++i) {
			MissionType type = missionList[i].type;
			MissionStruct mission = missionList[i];

			//Debug.Log(mission.type.ToString() +" "+mission.curNum + " "+mission.maxNum);

			switch(type)
			{
			    case MissionType.BeHitByItem:
				case MissionType.Crash:
					if(mission.curNum < mission.maxNum)
					{
						//完成任务
						GameData.Instance.completeMissionIds.Add(mission.id);
						GameData.Instance.StarCount ++;
						//Debug.Log(mission.type.ToString());
					}
				break;
			case MissionType.Ranking:
				if(mission.curNum <= mission.maxNum)
				{
					//完成任务
					GameData.Instance.completeMissionIds.Add(mission.id);
					GameData.Instance.StarCount ++;
					//Debug.Log(mission.type.ToString());
				}
				break;
			case MissionType.LimitTime:
				if(mission.curNum>=0)
				{
					GameData.Instance.completeMissionIds.Add(mission.id);
					GameData.Instance.StarCount ++;
					//Debug.Log(mission.type.ToString());
				}
				break;

			}
		}
	}

	
	public void MissionReborn()
	{
		isFailMission = false;
		for (int i=0; i<this.missionList.Count; ++i) {
		
			MissionStruct mission = missionList [i];
			MissionType type = mission.type;
			switch (type) {
			case MissionType.LimitTime:
				mission.maxNum = CarManager.Instance.totalUseTime;
				break;
			}
		}
	}


	MissionStruct GetMissionByType(MissionType type)
	{
		for (int i=0; i<this.missionList.Count; ++i) {
		   if(missionList[i].type == type)
			{
				return missionList[i];
			}
		}
		return null;
	}

	private class MissionStruct{
		public int id=0;
		public MissionType type;
		public string icon="";
		public float curNum=0;
		public float maxNum=0;
		public bool isComplete=false;
		public Transform showTrans;

		public tk2dTextMesh countText;
		public EasyFontTextMesh desText;
	}
	
}
