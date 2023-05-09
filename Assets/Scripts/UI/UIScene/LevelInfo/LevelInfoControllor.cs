using UnityEngine;
using System.Collections;
using DG.Tweening;
using PathologicalGames;

public class LevelInfoControllor : UIBoxBase {

	public static LevelInfoControllor Instance;

	//LeftContent
	public Transform TaskContainer;
	public Transform DropContainer;
	public Transform IconImageTran;
	public EasyFontTextMesh LevelText;

	//RightContent
	public GameObject goBackButton, goStartButton, goZaohuanButton;
	public Transform LeftContent, RightContent;
	public GameObject goLeftButton, goRightButton;
	public Transform modelListContainer;
	
	public PagePoint pagePoint;
	public GameObject CoinTypeGO, JewelTypeGO;
	public tk2dSprite CoinTypeCostTypeImage, JewelTypeCostTypeImage;
	public EasyFontTextMesh CoinTypeCostCountText,JewelTypeCostCountText;
	public EasyFontTextMesh ZaohuanCountText;

	public GameObject EventMask;
	public ParticleSystem OnClickParticle;

	private int modelIndex;
	private int modelCount;
	private int modelType;
	private int modelLevel;
	
	private string upgradeType;
	private int upgradeCount;
	
	private int[] playerModelStateIds, modelAllStateIds;
	private Transform[] modelTranArr;
	//机甲
	private int[] playerMechaModelStateIds;
	private AnimatorManager[] modelAnimArr;

	private int levelId;

	public override void Init ()
	{
		Instance = this;
		transform.localPosition = ShowPosV2;
		LeftContent.localPosition = GlobalConst.LeftHidePos;
		RightContent.localPosition = GlobalConst.RightHidePos;

		PlayerData.Instance.UpdateModelChangeEvent += UpgradeModel;

		InitData ();
		base.Init();
	}

	public void InitData(int levelId)
	{
		this.levelId = levelId;
		LevelText.text = "Race " + levelId.ToString () + "";
		int[] missionIdArr = LevelData.Instance.GetMissionIdList (levelId);
		int[] missionNumArr = LevelData.Instance.GetMissionNumList (levelId);
		int[] dropIdArr = LevelData.Instance.GetDropItemRewardIdList (levelId);
		Transform itemTran;
		TaskItem itemScript;
		for(int i=0; i<3; ++i)
		{
			/*特俗任务 根据关卡配置*/
			MissionType missionType = MissionData.Instance.GetMissionType(missionIdArr[i]);
		    if(missionType == MissionType.LimitTime)
			{
				missionNumArr[i] = (int)GameLevelData.Instance.GetUseTime( levelId);
			}

			itemTran = TaskContainer.Find("TaskItem" + i);
			itemScript = itemTran.GetComponent<TaskItem>();
			itemScript.SetContentText(MissionData.Instance.GetDesc(missionIdArr[i]).Replace("N", missionNumArr[i].ToString()));
//			Debug.Log("levelId: " + levelId + " missionIdArr: " + missionIdArr[i] + " " + PlayerData.Instance.GetLevelMissionState(levelId, missionIdArr[i]).ToString());
			itemScript.SetGetImage(PlayerData.Instance.GetLevelMissionState(levelId, missionIdArr[i]));
			
			itemTran = DropContainer.Find("DropItem" + i);
			itemTran.Find("IconImage").GetComponent<tk2dSprite>().SetSprite (RewardData.Instance.GetIconName(dropIdArr[i]));
		}
	}

	private void InitData()
	{
		//先检测所有的模型Id情况.
		playerModelStateIds = PlayerData.Instance.GetModelState();
		
		modelCount = ModelData.Instance.GetUseModelDataCount();
		modelAllStateIds = new int[modelCount];
		
		bool hasModel = false;
		for(int i = 1, k = 0; i <= modelCount; ++i)
		{
			hasModel = false;
			
			//检测玩家数据中是否存在模型.
			for(int j = 0; j < playerModelStateIds.Length; ++j)
			{
				if(playerModelStateIds[j] / 100 == i)
				{
					modelAllStateIds[k] = playerModelStateIds[j];
					++ k;
					hasModel = true;
					break;
				}
			}
			
			if(hasModel == false)
			{
				modelAllStateIds[k] = i * 100;
				++ k;
			}
		}
		
		//再根据Id情况生成对应模型.
		modelTranArr = new Transform[modelCount];
		modelAnimArr = new AnimatorManager[modelCount];
		int showId = 0;
		SpawnPool modelPool = PoolManager.Pools["CharactersPool"];
		SkinnedMeshRenderer skin;
		
		for(int i = 0; i < modelCount; i ++)
		{
			if(IDTool.GetModelLevel(modelAllStateIds[i]) != 0)
			{
				showId = modelAllStateIds[i];
			}
			else
			{
				showId = modelAllStateIds[i] + 1;
			}
			
			if(!ModelData.Instance.GetIsUse (showId))
				continue;
			
			Transform model = modelPool.Spawn(ModelData.Instance.GetPrefabName (showId));
			model.parent = modelListContainer;
			model.localPosition = Vector3.zero;
			model.localRotation = Quaternion.identity;
			model.localScale = Vector3.one;
			//车
			if(IDTool.GetModelType(showId) == 1)
			{
				model.GetChild(0).localPosition = new Vector3(-34,1,-36);
				model.GetChild(0).localRotation = Quaternion.Euler(6,315,0);
				model.GetChild(0).localScale = Vector3.one * 46;
				
				model.GetChild(1).localPosition = new Vector3(67,1,91);
				model.GetChild(1).localRotation = Quaternion.Euler(0,350,0);
				model.GetChild(1).localScale = Vector3.one * 66;
			}
			else if(IDTool.GetModelType(showId) == 2)
			{
				model.GetChild(0).localPosition = new Vector3(-45,1,-33);
				model.GetChild(0).localRotation = Quaternion.Euler(6,315,0);
				model.GetChild(0).localScale = Vector3.one * 46;
				
				model.GetChild(1).localPosition = new Vector3(37f,1,77);
				model.GetChild(1).localRotation = Quaternion.Euler(0f,350,0);
				model.GetChild(1).localScale = Vector3.one * 66;
			}
			else if(IDTool.GetModelType(showId) == 3)
			{
				model.GetChild(0).localPosition = new Vector3(-20,7,-74);
				model.GetChild(0).localRotation = Quaternion.Euler(6,315,0);
				model.GetChild(0).localScale = Vector3.one * 46;
				//人
				model.GetChild(1).localPosition = new Vector3(45f,1,87);
				model.GetChild(1).localRotation = Quaternion.Euler(0,350,0);
				model.GetChild(1).localScale = Vector3.one * 66;
			}
			else if(IDTool.GetModelType(showId)== 4)
			{
				model.GetChild(0).localPosition = new Vector3(-25f,4f,-68);
				model.GetChild(0).localRotation = Quaternion.Euler(6,315,0);
				model.GetChild(0).localScale = Vector3.one * 46;
				
				model.GetChild(1).localPosition = new Vector3(45.6f,1,96.1f);
				model.GetChild(1).localRotation = Quaternion.Euler(0,350,0);
				model.GetChild(1).localScale = Vector3.one * 66;
			}
			else
			{
				model.GetChild(0).localPosition = new Vector3(-20,1,-74);
				model.GetChild(0).localRotation = Quaternion.Euler(6,315,0);
				model.GetChild(0).localScale = Vector3.one * 46;
				//人
				model.GetChild(1).localPosition = new Vector3(20,1,55);
				model.GetChild(1).localRotation = Quaternion.Euler(0,350,0);
				model.GetChild(1).localScale = Vector3.one * 66;
			}
			model.gameObject.SetActive(true);
			//修改材质球，修改展示效果.
			for(int j = 0; j < model.transform.childCount; j++)
			{
				if(model.transform.GetChild(j).GetComponent<SkinnedMeshRenderer>() != null)
				{
					Material mater = model.transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material;
					mater.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
				}
			}

			
			model.gameObject.layer = 16;
			SetGameObjecLayer(model,16);
			
			modelAnimArr[i] = model.GetChild(1).GetComponent<AnimatorManager>();
			modelAnimArr[i].Init();
			//modelAnimArr[i].Show1();
//			modelAnimArr[i].Idle();

			model.gameObject.SetActive(false);
			modelTranArr[i] = model;
		}

		//新角色引入流程
		if (PlayerData.Instance.GetUIGuideState (UIGuideType.LeftRoleGuide) && PlayerData.Instance.GetCurrentChallengeLevel () == 4)
			modelAllStateIds [4] = 505;
		
		modelIndex = 0;
		pagePoint.Init(modelCount, modelIndex, true);
	}
	void SetGameObjecLayer(Transform trans, int layer)
	{
		foreach(Transform child in trans)
		{
			child.gameObject.layer = layer;
			SetGameObjecLayer(child,layer);
		}
	}
	public void SetModelIndex(int modelIndex)
	{
		this.modelIndex = modelIndex;
	}

	public override void Show ()
	{
		PropertyDisplayControllor.Instance.ChangeLayer ();
		base.Show();
		if (PlayerData.Instance.GetUIGuideState (UIGuideType.LevelInfoGuide) && PlayerData.Instance.GetCurrentChallengeLevel () == 2)
			UIGuideControllor.Instance.Show (UIGuideType.LevelInfoGuide);
		else if (PlayerData.Instance.GetUIGuideState (UIGuideType.LevelInfoGuide) && PlayerData.Instance.GetCurrentChallengeLevel () == 4) {
			UIGuideControllor.Instance.Show (UIGuideType.LevelInfoGuide);
			UIGuideControllor.Instance.ShowBubbleTipByID(1);
			TranslucentUIMaskManager.Instance.SetLayer (12);
			//ActorCameraController2.Instance.SetCameraDepth (4);
		}

		DOTween.Kill (LeftContent);
		DOTween.Kill (RightContent);

		//新手教程条件
		if(QualitySetting.IsHighQuality && PlayerData.Instance.GetCurrentChallengeLevel() != 1 && PlayerData.Instance.GetCurrentChallengeLevel() != 2 && PlayerData.Instance.GetCurrentChallengeLevel() != 4)
		{
			LeftContent.DOLocalMove (new Vector3 (-185, 0, 0), GlobalConst.PlayTime).SetEase (Ease.OutBack);
			RightContent.DOLocalMove (new Vector3 (210, 0, 0), GlobalConst.PlayTime).SetEase (Ease.OutBack);
			Invoke("DelayCheckGiftBag", GlobalConst.PlayTime);
		}
		else
		{
			LeftContent.localPosition = new Vector3 (-185, 0, 0);
			RightContent.localPosition = new Vector3 (210, 0, 0);
			if(PlayerData.Instance.GetCurrentChallengeLevel() != 2 && PlayerData.Instance.GetCurrentChallengeLevel() != 4)
				Invoke("DelayCheckGiftBag", 0);
		}

		InitAnim ();
		pagePoint.Init (modelCount, modelIndex, false);
		SetButtonData ();
		
		ResetAllPlayerAnim ();
		ShowModelAnim ();
	}

	void DelayCheckGiftBag()
	{
		if (PlayerData.Instance.GetMonthCardGiftState () && !PlayerData.Instance.GetMonthCardGiftRewardsState ()) {
			UIManager.Instance.ShowModule (UISceneModuleType.MonthCardGiftReward);	
		}
		else if (PlayerData.Instance.GetIsShowedMonthCardGift ()
		         && !PlayerData.Instance.GetMonthCardGiftState ()
		         && PlatformSetting.Instance.PlatformType == PlatformItemType.LianTong
		         && !AutoGiftChecker.IsNoAutoGiftVer) {
			UIManager.Instance.ShowModule (UISceneModuleType.MonthCardGift);
		}
		else if(PlatformSetting.Instance.PayVersionType == PayVersionItemType.ChangWan && PlayerData.Instance.GetHuiKuiMiniGiftState()==false)
		{
			CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_NewPlayerGet,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
			UIManager.Instance.ShowModule (UISceneModuleType.NewPlayerGift);
		}
		else if (AutoGiftChecker.CTypeGiftBagEnabled) {
			CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_LimitTime,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
			UIManager.Instance.ShowModule (UISceneModuleType.DiscountGift);
		}else if (PlatformSetting.Instance.IsOpenCommonGiftType && !PlayerData.Instance.GetCommonGiftIsBuy())
		{
			UIManager.Instance.ShowModule(UISceneModuleType.CommonGift);
		} 
		else if (GlobalConst.StartGameGuideEnabled)
			UIGuideControllor.Instance.Show (UIGuideType.LevelInfoGuide);
	}

	public override void Hide ()
	{
		base.Hide ();
		DOTween.Kill (LeftContent);
		DOTween.Kill (RightContent);
		if(QualitySetting.IsHighQuality)
		{
			LeftContent.DOMove (GlobalConst.LeftHidePos, GlobalConst.PlayTime).SetEase (Ease.OutBack);
			RightContent.DOMove (GlobalConst.RightHidePos, GlobalConst.PlayTime).SetEase (Ease.OutBack).OnComplete(HideCallBack);
		}
		else
		{
			LeftContent.localPosition = GlobalConst.LeftHidePos;
			RightContent.localPosition = GlobalConst.RightHidePos;
			HideCallBack();
		}
		UIManager.Instance.HideModule (UISceneModuleType.LevelInfo);

		PropertyDisplayControllor.Instance.ChangeLayer ();

	}

	void HideCallBack()
	{
		gameObject.SetActive(false);
	}

	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	void ResetAllPlayerAnim()
	{
		for(int i = 0; i < modelCount; i++)
		{
			//modelAnimArr[i].Idle();
		}
	}
	
	void PlayModelAnim()
	{
		modelAnimArr[modelIndex].Show2();
	}
	
	void ShowModelAnim()
	{
		modelAnimArr[modelIndex].Show2();
		CancelInvoke ("PlayModelAnim");
		Invoke ("PlayModelAnim", Random.Range(4.0f, 7.0f));
	}
	
	public void TurnToLeft()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		
		-- modelIndex;
		if(modelIndex < 0)
			modelIndex = modelCount - 1;
		
		for(int i = 0; i < modelCount; ++i)
		{
			if(i == modelIndex)
			{
				modelTranArr[i].gameObject.SetActive(true);
				AudioManger.Instance.PlaySoundByName(ModelData.Instance.GetAotemanSound(modelAllStateIds[i]));
			}
			else
			{
				modelTranArr[i].gameObject.SetActive(false);
				AudioManger.Instance.StopSoundByName(ModelData.Instance.GetAotemanSound(modelAllStateIds[i]));
			}
		}
		
		modelTranArr [modelIndex].localPosition = new Vector3(0, 100, 0);
		modelTranArr [modelIndex].DOLocalMove (Vector3.zero, 0.3f);
		
		ResetAllPlayerAnim();
		ShowModelAnim ();
		
		pagePoint.PrePage();
		SetButtonData ();
	}
	
	/// <summary>
	/// 模型点击升级的更新.
	/// </summary>
	/// <param name="modelId">Model identifier.</param>
	public void UpgradeModel(int modelId)
	{
		for(int i = 0; i < modelAllStateIds.Length; i++)
		{
			if(IDTool.GetModelType(modelAllStateIds[i]) == IDTool.GetModelType(modelId))
			{
				modelAllStateIds[i] = modelId;
				break;
			}
		}
		
		SetButtonData ();
	}

	public void SetUpgradeCount()
	{
		int count = ToolController.GetTipCounnt (modelAllStateIds [modelIndex]);
		if (count > 0) {
			ZaohuanCountText.transform.parent.gameObject.SetActive (true);
			ZaohuanCountText.text = count.ToString ();
		} else {
			ZaohuanCountText.transform.parent.gameObject.SetActive (false);
		}
	}
	
	private void SetButtonData()
	{
		modelLevel = IDTool.GetModelLevel(modelAllStateIds[modelIndex]);
		modelType = IDTool.GetModelType(modelAllStateIds[modelIndex]);

		SetUpgradeCount ();

		if (modelLevel == 0) {
			goZaohuanButton.SetActive (true);
			goStartButton.SetActive (false);
			upgradeType = ModelData.Instance.GetZhaohuanCostType (modelAllStateIds[modelIndex]);
			upgradeCount = ModelData.Instance.GetZhaohuanCost (modelAllStateIds[modelIndex]);
			if(upgradeType.CompareTo("Coin") == 0)
			{
				CoinTypeGO.SetActive(true);
				JewelTypeGO.SetActive(false);
				CoinTypeCostTypeImage.SetSprite (upgradeType.ToLower() + "_icon");
				CoinTypeCostCountText.text = upgradeCount.ToString();
			}
			else
			{
				CoinTypeGO.SetActive(false);
				JewelTypeGO.SetActive(true);
				JewelTypeCostTypeImage.SetSprite (upgradeType.ToLower() + "_icon");
				JewelTypeCostCountText.text = upgradeCount.ToString();
			}
		} else {
			goZaohuanButton.SetActive (false);
			goStartButton.SetActive (true);
		}
	}
	
	public void TurnToRight()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		
		modelIndex ++;
		if(modelIndex >= modelCount)
			modelIndex = 0;
		
		for(int i = 0; i < modelCount; i++)
		{
			if(i == modelIndex)
			{
				modelTranArr[i].gameObject.SetActive(true);
				AudioManger.Instance.PlaySoundByName(ModelData.Instance.GetAotemanSound(modelAllStateIds[i]));
			}
			else
			{
				modelTranArr[i].gameObject.SetActive(false);
				AudioManger.Instance.StopSoundByName(ModelData.Instance.GetAotemanSound(modelAllStateIds[i]));
			}
		}
		
		modelTranArr [modelIndex].localPosition = Vector3.zero;
		modelTranArr[modelIndex].DOLocalMove(new Vector3(0, 10, 0), 0.3f).From(true);
		
		ResetAllPlayerAnim();
		ShowModelAnim ();
		
		pagePoint.NextPage();
		SetButtonData ();
	}
	
	void InitAnim()
	{
		for(int i = 0; i < modelCount; ++i)
		{
			if(i == modelIndex)
				modelTranArr[i].gameObject.SetActive(true);
			else
				modelTranArr[i].gameObject.SetActive(false);
		}
	}

	public void BackOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	public void StartOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		//OnClickEffect.Instance.PlayEffect(OnClickEffect.ParticleType.Mid, goStartButton);
		if(PlayerData.Instance.GetItemNum(PlayerData.ItemType.Strength) > 0)
		{
			PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Strength, 1);
		}
		else
		{
			GiftPackageControllor.Instance.Show(PayType.PowerGift);
			return;
		}
		
		PlayerData.Instance.SetSelectedLevel (levelId);
		PlayerData.Instance.SetGameMode("Level");
		OnClickParticle.Play ();

		//试驾
		if (PlayerData.Instance.GetEachLevelFailCount () >= 3 && PlayerData.Instance.GetLevePlayCount () <= levelId
			&& levelId > 2 && levelId <= 10) {
			modelIndex = modelAllStateIds.Length - 1;
			modelAllStateIds [4] = 505;
			PlayerData.Instance.SetSelectModelIsTestDrive (true);
			PlayerData.Instance.SetSelectedModel(modelAllStateIds[modelIndex]);
		} else {
			PlayerData.Instance.SetSelectModelIsTestDrive (false);
			PlayerData.Instance.SetSelectedModel(modelAllStateIds[modelIndex]);
		}
		IconImageTran.gameObject.SetActive(true);
		IconImageTran.DOScale(Vector3.one, 1.5f).From().SetEase(Ease.Linear);
		IconImageTran.DOLocalMove(new Vector3(-552, 355, 0), 1.5f).From().SetEase(Ease.OutCubic).OnComplete(LoadScene);
		EventMask.SetActive (true);
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = false;
		EventLayerController.Instance.SetEventLayer (EventLayer.Nothing);
	}

	public void ZaohuanOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		ZaohuanEvent ();
	}
	
	private void ZaohuanEvent()
	{
		if(upgradeType.CompareTo("Coin") == 0)
		{
			if(upgradeCount > PlayerData.Instance.GetItemNum(PlayerData.ItemType.Coin))
			{
				GiftPackageControllor.Instance.Show(PayType.CoinGift, ZaohuanEvent);
				CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Coin,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
				return;
			}
			else
			{
				PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Coin, upgradeCount);
			}
		}
		else if(upgradeType.CompareTo("Jewel") == 0)
		{
			if(upgradeCount > PlayerData.Instance.GetItemNum(PlayerData.ItemType.Jewel))
			{
				GiftPackageControllor.Instance.Show(PayType.JewelGift, ZaohuanEvent);
				CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Jewel,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
				return;
			}
			else
			{
				PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Jewel, upgradeCount);
			}
		}
		
		++ modelAllStateIds[modelIndex];
		PlayerData.Instance.UpdateModelState (modelAllStateIds [modelIndex]);
		
		//自定义事件.
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Player_WakeUp, "Role call", ModelData.Instance.GetName (modelType));
	}

	private Vector3 downPos, upPos;
	private void SlideOnDown()
	{
		downPos = Input.mousePosition;
		//		isMouseDown = true;
	}
	
	private void SlideOnUp()
	{
		//		isMouseDown = false;
		//		return;
		
		upPos = Input.mousePosition;
		
		if(upPos.x - downPos.x > 30)
			TurnToLeft();
		
		if(upPos.x - downPos.x < -30)
			TurnToRight();
	}
	
	//滑动旋转查看角色模型功能.
	//	bool isMouseDown = false;
	//	void LateUpdate()
	//	{
	//		if(!isMouseDown)
	//			return;
	//			
	//		modelObjs[modelIndex].transform.Rotate(0, (Input.mousePosition.x - downPos.x) * 0.1f, 0);
	//	}

	public void LeftOnClick()
	{
		TurnToLeft();
	}

	public void RightOnClick()
	{
		TurnToRight();
	}

	private void LoadScene ()
	{
		//EventMask.SetActive (false);

		GlobalConst.SceneName = SceneType.GameScene;
		LoadingPage.Instance.LoadScene ();
	}
}