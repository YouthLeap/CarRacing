using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using PathologicalGames;

public class LevelSelectControllor : UIBoxBase {

	public static LevelSelectControllor Instance;

	public bool bMainInterfaceEnter = true; //区别是从主界面还是活动界面进入关卡选择界面

	public GameObject goNewPlayerGiftButton, goThreeStarBtn, goFourStarBtn, goFiveStarBtn;
	public tk2dUIScrollableArea scrollArea;
	public Transform ItemContainer, LevelItemContainer, RoadItemContainer;

	public ParticleSystem DianJiPS;

	private int CurChallengeLevel;
	private Transform itemTran;

	public void PlayDianJiPS(Vector3 pos)
	{
		DianJiPS.transform.position = pos;
		DianJiPS.Play();
	}

	public override void Init ()
	{
		Instance = this;
		InitData ();
		base.Init();
	}

	private void InitData()
	{
		bMainInterfaceEnter = true;
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian) {
			int cLevel = PlayerData.Instance.GetCurrentChallengeLevel ();
			if (cLevel > 4 && cLevel < GlobalConst.MaxLevel) {
				for (int k = cLevel; k <= GlobalConst.MaxLevel; ++k) {
					PlayerData.Instance.SetLevelStarState (k, 0);
				}
				PlayerData.Instance.SetCurrentChallengeLevel (GlobalConst.MaxLevel);

			}
			goNewPlayerGiftButton.SetActive (false);
			goThreeStarBtn.SetActive (false);
			goFourStarBtn.SetActive (false);
			goFiveStarBtn.SetActive (false);
		}

		CurChallengeLevel = PlayerData.Instance.GetCurrentChallengeLevel ();
		int[] levelStarState = PlayerData.Instance.GetLevelStarState ();

		SpawnPool spUIItems = PoolManager.Pools["UIItemsPool"];
		
		TextAsset binAsset = Resources.Load ("Data/LevelItemJsonData", typeof(TextAsset)) as TextAsset;
		Dictionary<string, object> jsonData = Json.Deserialize(binAsset.text) as Dictionary<string, object>;
		int levelCount = jsonData.Count;
		string[] posArr;

		scrollArea.VisibleAreaLength = 800;
		scrollArea.ContentLength = 10 * 800 - 100;//levelCount * 90 + 120;
		LevelSelectItem itemScript;
		for (int i=0; i<levelCount; ++i) {
			posArr = jsonData["level" + i].ToString().Split('|');
			itemTran = spUIItems.Spawn("LevelItem");
			itemTran.name = "LevelItem" + (i+1);
			itemTran.gameObject.SetActive(true);
			itemTran.parent = LevelItemContainer;
			itemTran.localPosition = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
			itemTran.localScale = Vector3.one;
			itemScript = itemTran.GetComponent<LevelSelectItem>();
			itemScript.Init();
			itemScript.SetLevelText(i+1);
			string iconSprite = LevelData.Instance.GetLevelIcon(i+1);
			if((i < CurChallengeLevel - 1) || (CurChallengeLevel == GlobalConst.MaxLevel && levelStarState.Length == GlobalConst.MaxLevel))
			{
				if(i>=levelStarState.Length)
				{
					Debug.LogError("star State");
					continue;
				}

				if(levelStarState[i] == 3)
				{
					itemScript.SetIconImage("btn_level_color");
					itemScript.IconHeadImage.gameObject.SetActive(false);
				}
				else
				{
					itemScript.SetIconImage("btn_level_green");

					itemScript.IconHeadImage.gameObject.SetActive(true);

					itemScript.SetIconHeadImage(iconSprite);
				}
				itemScript.SetLevelTextColor(new Color(193/255f, 91/255f, 28/255f, 1));
				itemScript.SetLockFlag(false);
				itemScript.SetButtonEnable(true);
				itemScript.ShowStar(true);
				itemScript.SetStarCount(levelStarState[i]);
				itemScript.IconHeadGrayImage.gameObject.SetActive(false);
			}
			else if(i == CurChallengeLevel - 1)
			{
				itemScript.IconHeadGrayImage.gameObject.SetActive(false);
				itemScript.IconHeadImage.gameObject.SetActive(true);
				itemScript.SetIconHeadImage(iconSprite);
				itemScript.SetIconImage("btn_level_yellow");
				itemScript.SetLevelTextColor(new Color(0, 217/255f, 95/255f, 1));
				itemScript.SetLockFlag(false);
				itemScript.SetButtonEnable(true);
				itemScript.ShowStar(false);
				itemScript.ShowLevelPS();
			}
			else
			{
				itemScript.IconHeadImage.gameObject.SetActive(false);
				itemScript.IconHeadGrayImage.gameObject.SetActive(true);
				itemScript.SetIconHeadGrayImage(iconSprite);
				itemScript.SetIconImage("btn_level_gray");
				itemScript.SetLevelTextColor(new Color(100/255f, 114/255f, 112/255f, 1));
				itemScript.SetLockFlag(true);
				itemScript.SetButtonEnable(false);
				itemScript.ShowStar(false);
			}
		}

		/*
		binAsset = Resources.Load ("Data/RoadItemJsonData", typeof(TextAsset)) as TextAsset;
		jsonData = Json.Deserialize(binAsset.text) as Dictionary<string, object>;
		int roadCount = jsonData.Count;
		string[] rotationArr;
		for (int i=0; i<roadCount; ++i) {
			Dictionary<string, object> itemJsonData = Json.Deserialize(jsonData["road" + i].ToString()) as Dictionary<string, object>;
			posArr = itemJsonData["position"].ToString().Split('|');
			rotationArr = itemJsonData["rotation"].ToString().Split('|');
			itemTran = spUIItems.Spawn("RoadItem");
			itemTran.name = "RoadItem" + (i+1);
			itemTran.gameObject.SetActive(true);
			itemTran.parent = RoadItemContainer;
			itemTran.localPosition = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
			itemTran.eulerAngles = new Vector3(float.Parse(rotationArr[0]), float.Parse(rotationArr[1]), float.Parse(rotationArr[2]));
			itemTran.localScale = Vector3.one;
		}
		*/
	}

	public void SetCenterPosition(tk2dUIItem item=null)
	{
		
		int levelId=1;
		if(item==null || item.sendMessageTarget==null)
			levelId=CurChallengeLevel;
		else{
			LevelSelectItem levelItem =item.sendMessageTarget.GetComponent<LevelSelectItem>();
			if(levelItem==null)
				return;
			levelId = levelItem.levelId;
		}
		itemTran = LevelItemContainer.Find ("LevelItem" + levelId);
		//float width = ItemContainer.GetComponent<RectTransform> ().sizeDelta.x;
		float width = scrollArea.ContentLength - scrollArea.VisibleAreaLength;
		float moveX = itemTran.localPosition.x;
		Vector3 pos ;
		if (moveX < 0) {
			pos = new Vector3 (0, 0, 0);
		} else if (moveX > width) {
			pos = new Vector3 (-width, 0, 0);
		} else {
			pos = new Vector3 (-moveX, 0, 0);
		}
		ItemContainer.localPosition =pos;
	}

	public override void Show ()
	{
		PropertyDisplayControllor.Instance.ChangeLayer ();

		base.Show();
		transform.localPosition = ShowPosV2;
		//MainInterfaceControllor.Instance.transform.localPosition = GlobalConst.TopHidePos;
		MainInterfaceControllor.Instance.gameObject.SetActive (false);
		BGManager.Instance.Hide ();

		SetCenterPosition ();

		if (PlatformSetting.Instance.PayVersionType != PayVersionItemType.GuangDian) {
			if (PlayerData.Instance.GetHuiKuiMiniGiftState ()) {
				goNewPlayerGiftButton.SetActive (false);
			} else {
				goNewPlayerGiftButton.SetActive (true);
			}
		}

		//新手教程条件
		Invoke("DelayShowLevelInfo", 0.1f);

//		Invoke("DelayCheckAutoGift", 0.1f);
	} 

	void DelayShowLevelInfo()
	{
		/*畅玩版 自动弹出新手礼包*/
//		if (GlobalConst.ShowModuleType != UISceneModuleType.LevelInfo &&
//		    PlatformSetting.Instance.PayVersionType == PayVersionItemType.ChangWan && 
//		    CurChallengeLevel == 3 && 
//		    PayJsonData.Instance.GetIsActivedState(PayType.NewPlayerGift) &&
//		    !PlayerData.Instance.GetHuiKuiMiniGiftState())
//		{
//			UIManager.Instance.ShowModule(UISceneModuleType.NewPlayerGift);
//			CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_NewPlayerGet,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
//		}
//		else
		if (GlobalConst.ShowModuleType != UISceneModuleType.LevelInfo && CurChallengeLevel <= 3 || bMainInterfaceEnter == false) {
			LevelInfoControllor.Instance.InitData (PlayerData.Instance.GetCurrentChallengeLevel());
			UIManager.Instance.ShowModule (UISceneModuleType.LevelInfo);
		}else
		{
			DelayCheckAutoGift();
		}
	}

	void DelayCheckAutoGift()
	{
		AutoGiftChecker.iEnterLevelSelectTimes ++;
	}

	public override void Hide ()
	{
		gameObject.SetActive (false);
		transform.localPosition = GlobalConst.TopHidePos;
		//MainInterfaceControllor.Instance.transform.localPosition = GlobalConst.ShowPos;
		MainInterfaceControllor.Instance.gameObject.SetActive (true);
		BGManager.Instance.Show ();

		UIManager.Instance.HideModule (UISceneModuleType.LevelSelect);

		PropertyDisplayControllor.Instance.ChangeLayer ();

	}

	public override void Back ()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.ButtonClick);
		Hide ();
	}

	public void BackOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	public void NewPlayerGiftOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule (UISceneModuleType.NewPlayerGift);
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_NewPlayerGet,"State","手动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
	}

	void RewardGift3StarBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule (UISceneModuleType.ThreeStar);
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_3Start,"State","手动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
	}

	void RewardGift4StarBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule (UISceneModuleType.FourStar);
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_4Star,"State","手动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
	}

	void RewardGift5StarBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe)
			UIManager.Instance.ShowModule (UISceneModuleType.PassGift);
		else
			UIManager.Instance.ShowModule (UISceneModuleType.PassGift);

		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_5Start,"State","手动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
	}
}

