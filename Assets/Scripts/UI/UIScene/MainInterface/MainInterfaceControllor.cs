using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PathologicalGames;

public class MainInterfaceControllor : UIBoxBase {

	public static MainInterfaceControllor Instance;

	public GameObject goCharactersButton, goZaohuanButton, goMaxLevelButton,/*goExchangeActivityButton,goSmashEggButton,*/goWinnerDisplay,goActivityButton,goMonthCardGiftButton;
	public GameObject goExchangeCodeBtn, goComplainBtn, goLimitTimeBtn, goShopBtn, goTurnplate;
    public GameObject goRemoveAdBtn;

	public Transform modelListContainer;

	public PagePoint pagePoint;
	public GameObject CoinTypeGO, JewelTypeGO;
	public tk2dSprite CoinTypeCostTypeImage, JewelTypeCostTypeImage;
	public EasyFontTextMesh CoinTypeCostCountText,JewelTypeCostCountText, CoinTypeBtnText,JewelTypeBtnText;
	public EasyFontTextMesh AchievementCountText, TaskCountText, ConvertCountText, TurnplateCountText, UpgradeCountText, exchangeActivityCountText,smashEggCountText;

	public EasyFontTextMesh NameText, DescText;

	public GameObject ActivityTips;

	public ParticleSystem ChangePlayerParticle;
	public ParticleSystem classGameBtnParticle, levelGameBtnParticle, upgradeBtnParticle;

	public tk2dSprite turnplateSprite;

	private int maxLevel = 30;
	private int modelIndex;
	private int modelCount;
	private int modelType;
	private int modelLevel;

	private string upgradeType;
	private int upgradeCount;

	private int[] playerModelStateIds, modelAllStateIds;
	private Transform[] modelTranArr;
	private AnimatorManager[] modelAnimArr;
	private List<int> DisableModelIndexList = new List<int> ();
	

	void OnEnable()
	{
		StartCoroutine("CheckAutoGiftIE");
	}

	IEnumerator CheckAutoGiftIE()
	{
		yield return new WaitForSeconds(0.2f);
		if(GlobalConst.IsSignIn)
		{
			AutoGiftChecker.iEnterMainInterfaceTimes ++;
		}
	}
	
	//设置月卡礼包按钮
	void SetMonthCardGiftBtn()
	{
		if (PlatformSetting.Instance.PlatformType == PlatformItemType.LianTong) 
		{
			goMonthCardGiftButton.SetActive (true);
			//判断是否已经获得月卡礼包，获得前显示BeforeGetBtn，获得后显示AfterGetBtn。
			if (PlayerData.Instance.GetMonthCardGiftState()) {
				if (System.DateTime.Now.Month != PlayerData.Instance.GetBuyMonthCardDate()){
					if (PlayerData.Instance.GetMonthCardGiftAutoRenewState()){
						//当月卡礼包设置为自动续费后，设置月卡礼包奖励状态为未获取；
						PlayerData.Instance.SetMonthCardGiftRewardsState(false);
						goMonthCardGiftButton.transform.Find("BeforeGetBtn").gameObject.SetActive(false);
						goMonthCardGiftButton.transform.Find("AfterGetBtn").gameObject.SetActive(true);
					} else{
						//当月卡礼包不自动续费后，设置月卡礼包状态为未领取；
						PlayerData.Instance.SetMonthCardGiftState(false);
						goMonthCardGiftButton.transform.Find("BeforeGetBtn").gameObject.SetActive(true);
						goMonthCardGiftButton.transform.Find("AfterGetBtn").gameObject.SetActive(false);
					}
				}
				else{
					goMonthCardGiftButton.transform.Find("BeforeGetBtn").gameObject.SetActive(false);
					goMonthCardGiftButton.transform.Find("AfterGetBtn").gameObject.SetActive(true);
				}
			}
			else{
				goMonthCardGiftButton.transform.Find("BeforeGetBtn").gameObject.SetActive(true);
				goMonthCardGiftButton.transform.Find("AfterGetBtn").gameObject.SetActive(false);
			}
		} else {
			goMonthCardGiftButton.SetActive (false);
		}
	}

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;

		transform.localPosition = Vector3.zero;

		PlayerData.Instance.UpdateModelChangeEvent += UpgradeModel;

		InitData ();

		SetMonthCardGiftBtn ();

		base.Init();
	}

	private void ShowAotemanFamily ()
	{
		bool showFlag = false;
		for (int i=0; i<modelCount; ++i) {
			if(IDTool.GetModelLevel(modelAllStateIds[i]) == 0)
			{
				showFlag = true;
				break;
			}
		}
		//是否显示角色礼包
		if(showFlag && PayJsonData.Instance.GetIsActivedState(PayType.CharactersGift))
		{
			goCharactersButton.SetActive(true);
		}
		else
		{
			goCharactersButton.SetActive(false);
		}
	}

	public void SetAchievementCount(int count)
	{
		if (count > 0) {
			AchievementCountText.transform.parent.gameObject.SetActive (true);
			AchievementCountText.text = count.ToString ();
		} else {
			AchievementCountText.transform.parent.gameObject.SetActive (false);
		}
	}
	
	public void SetTaskCount(int count)
	{
		if (count > 0) {
			TaskCountText.transform.parent.gameObject.SetActive (true);
			TaskCountText.text = count.ToString ();
		} else {
			TaskCountText.transform.parent.gameObject.SetActive (false);
		}
	}
	
	public void SetConvertCount(int count)
	{
		if (count > 0) {
			ConvertCountText.transform.parent.gameObject.SetActive (true);
			ConvertCountText.text = count.ToString ();
		} else {
			ConvertCountText.transform.parent.gameObject.SetActive (false);
		}
	}

	public void SetTurnplateCount(int count)
	{
		if (count > 0) {
			TurnplateCountText.transform.parent.gameObject.SetActive (true);
			TurnplateCountText.text = (count > 99) ? "99+":count.ToString ();
		} else {
			TurnplateCountText.transform.parent.gameObject.SetActive (false);
		}
	}

	public void SetUpgradeCount()
	{
		int count = ToolController.GetTipCounnt (modelAllStateIds [modelIndex]);
		if (count > 0) {
			UpgradeCountText.transform.parent.gameObject.SetActive (true);
			UpgradeCountText.text = count.ToString ();
		} else {
			UpgradeCountText.transform.parent.gameObject.SetActive (false);
		}
	}

	public void SetExchangeActivityCount(int count)
	{
		if (count > 0) {
			exchangeActivityCountText.transform.parent.gameObject.SetActive (true);
			exchangeActivityCountText.text = count.ToString ();
		} else {
			exchangeActivityCountText.transform.parent.gameObject.SetActive (false);
		}
	}
	public void SetSmashEggCount(int count)
	{
		if(count > 0)
		{
			smashEggCountText.transform.parent.gameObject.SetActive(true);
			smashEggCountText.text = count.ToString();
		}else{
			smashEggCountText.transform.parent.gameObject.SetActive(false);
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
				if(IDTool.GetModelType(playerModelStateIds[j]) == i)
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
			if(IDTool.GetModelType(modelAllStateIds[i]) == 1)
			{
				model.GetChild(0).localPosition = new Vector3(-34,1,-36);
				model.GetChild(0).localRotation = Quaternion.Euler(6,315,0);
				model.GetChild(0).localScale = Vector3.one * 46;
				
				model.GetChild(1).localPosition = new Vector3(67,1,91);
				model.GetChild(1).localRotation = Quaternion.Euler(0,350,0);
				model.GetChild(1).localScale = Vector3.one * 66;
			}
			else if(IDTool.GetModelType(modelAllStateIds[i]) == 2)
			{
				model.GetChild(0).localPosition = new Vector3(-45,1,-33);
				model.GetChild(0).localRotation = Quaternion.Euler(6,315,0);
				model.GetChild(0).localScale = Vector3.one * 46;
				
				model.GetChild(1).localPosition = new Vector3(37f,1,77);
				model.GetChild(1).localRotation = Quaternion.Euler(0f,350,0);
				model.GetChild(1).localScale = Vector3.one * 66;
			}
			else if(IDTool.GetModelType(modelAllStateIds[i]) == 3)
			{
				model.GetChild(0).localPosition = new Vector3(-20,7,-74);
				model.GetChild(0).localRotation = Quaternion.Euler(6,315,0);
				model.GetChild(0).localScale = Vector3.one * 46;
				//人
				model.GetChild(1).localPosition = new Vector3(45f,1,87);
				model.GetChild(1).localRotation = Quaternion.Euler(0,350,0);
				model.GetChild(1).localScale = Vector3.one * 66;
			}
			else if(IDTool.GetModelType(modelAllStateIds[i]) == 4)
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
			model.gameObject.SetActive(false);

			model.gameObject.layer = 15;
			SetGameObjecLayer(model,15);

			modelAnimArr[i] = model.GetChild(1).GetComponent<AnimatorManager>();
			modelAnimArr[i].Init();
			//modelAnimArr[i].Idle();
			
			modelTranArr[i] = model;
		}

		//初始化
		modelIndex = 0;
		pagePoint.Init (modelCount, modelIndex, true);

		ShowAotemanFamily ();
	}
	void SetGameObjecLayer(Transform trans, int layer)
	{
		foreach(Transform child in trans)
		{
			child.gameObject.layer = layer;
			SetGameObjecLayer(child,layer);
		}
	}
	#region 设置随机角色

	private void SetRandomModel()
	{
		DisableModelIndexList.Clear();
		for(int i = 0; i < modelCount; ++i)
		{
			if(IDTool.GetModelLevel(modelAllStateIds[i]) == 0)
			{
				DisableModelIndexList.Add(i);
			}
		}
		
		modelIndex = IDTool.GetModelType(PlayerData.Instance.GetSelectedModel ()) - 1;
		
		//随机显示一个未召唤的角色
		if(DisableModelIndexList.Count > 0  && AutoGiftChecker.bIsRandomModel)
		{
			modelIndex = DisableModelIndexList [Random.Range (0, DisableModelIndexList.Count)];
		}
	}

	#endregion

	/// <summary>
	/// 设置角色第几个的关联显示
	/// </summary>
	/// <param name="modelIndex">Model index.</param>
	public void SetModelIndex(int modelIndex)
	{
		this.modelIndex = modelIndex;
		pagePoint.Init (modelCount, modelIndex, false);
		SetButtonData ();
		for(int i = 0; i < modelCount; ++i)
		{
			if(i == modelIndex)
			{
				modelTranArr[i].gameObject.SetActive(true);
			}
			else
			{
				modelTranArr[i].gameObject.SetActive(false);
			}
		}
	}

	void InitAnim()
	{
		//角色教程，默认第一个角色.
		if (PlayerData.Instance.GetUIGuideState (UIGuideType.LeftRoleGuide) && PlayerData.Instance.GetCurrentChallengeLevel () == 4) {
			modelIndex = 0;
		} else {
			//角色随机选定设置.
			SetRandomModel ();
		}

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
	}

	public override void Show ()
	{
		PropertyDisplayControllor.Instance.ChangeLayer ();

		base.Show();
		if (GlobalConst.StartGameGuideEnabled) {
			UIGuideControllor.Instance.Show (UIGuideType.MainInterfaceStartGameGuide);
			HideFingerGuide ();
		} else if (GlobalConst.IsSignIn && PlayerData.Instance.GetCurrentChallengeLevel () == 3 && PlayerData.Instance.GetUIGuideState (UIGuideType.CharaterDetailGuide)) {
			if(modelIndex == 0 && IDTool.GetModelLevel (modelAllStateIds[modelIndex]) == 1)
			{
				UIGuideControllor.Instance.Show (UIGuideType.MainInterfaceUpgradeGuide);
				UIGuideControllor.Instance.ShowBubbleTipByID(2);
				HideFingerGuide ();
			}
		} else if (GlobalConst.IsSignIn && PlayerData.Instance.GetUIGuideState (UIGuideType.LeftRoleGuide) && PlayerData.Instance.GetCurrentChallengeLevel () == 4) {
			UIGuideControllor.Instance.Show (UIGuideType.LeftRoleGuide);
			TranslucentUIMaskManager.Instance.SetLayer (11);
			ActorCameraController1.Instance.SetCameraDepth (4);
			HideFingerGuide ();
		}

		//设置随机角色
		InitAnim ();
		pagePoint.Init (modelCount, modelIndex, false);
		SetButtonData ();

		ResetAllPlayerAnim ();
		ShowBeforeAnim ();
		SetBtnOnlineControl ();
	}
	void BtnShake()
	{
		if (!PlatformSetting.Instance.isOpenActivity)
			return;
		DOTween.Kill ("shakeExActivity");
		Sequence mySequence = DOTween.Sequence();
		mySequence.Append (goActivityButton.transform.DOShakePosition (0.1f, new Vector3(5,5,0)).SetDelay (2f).SetLoops(4));
		mySequence.SetLoops (-1).SetId("shakeExActivity");
	}
	public void SetBtnOnlineControl()
	{
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian) {
			goCharactersButton.SetActive (false);
			goLimitTimeBtn.SetActive (false);
		}
		
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe
		    || PlatformSetting.Instance.PayVersionType == PayVersionItemType.ChangWan
		    || PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian) {
			goActivityButton.SetActive (false);
			goWinnerDisplay.SetActive (false);
			goExchangeCodeBtn.SetActive(false);
			return;
		}
		BtnShake ();
		goWinnerDisplay.SetActive (PlatformSetting.Instance.isOpenActivity);
		goActivityButton.SetActive (PlatformSetting.Instance.isOpenActivity);
		goExchangeCodeBtn.SetActive (PlatformSetting.Instance.isOpenActivity);
		
	}
	/// <summary>
	/// 有新手引导的时候隐藏冒险模式按钮上的手指
	/// </summary>
	public void HideFingerGuide()
	{
		Transform FingerGuideTran;
		FingerGuideTran = MainInterfaceControllor.Instance.transform.Find("Buttons").Find("LevelGameButton").Find("FingerGuide");
		if (FingerGuideTran != null)
			FingerGuideTran.gameObject.SetActive (false);
	}

	public override void Hide ()
	{
		gameObject.SetActive (false);
		transform.localPosition = GlobalConst.TopHidePos;
		UIManager.Instance.HideModule(UISceneModuleType.MainInterface);

		PropertyDisplayControllor.Instance.ChangeLayer ();

	}

	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);

		if(PlatformSetting.Instance.ChannelType.ToString().CompareTo(ChannelItemType.SDK.ToString()) == 0)
		{
			PlatformSetting.Instance.ExitGame();
		}else
		{
		    UIManager.Instance.ShowModule(UISceneModuleType.ExitGame);
		}
	}

	#region 动作控制部分

	void ResetAllPlayerAnim()
	{
		for(int i = 0; i < modelCount; i++)
		{
			//modelAnimArr[i].Idle();
		}
	}

	void PlayModelAnim()
	{
		modelAnimArr[modelIndex].Show();
	}

	void ShowBeforeAnim()
	{
		//modelAnimArr [modelIndex].BeginShow ();
		CancelInvoke ("PlayModelAnim");
		InvokeRepeating ("PlayModelAnim", 0, Random.Range(3.5f, 5.0f));
	}

	#endregion

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
		
		modelTranArr [modelIndex].localPosition = Vector3.zero;
		modelTranArr[modelIndex].DOLocalMove(new Vector3(0, 50, 0), 0.1f).From(true);
		ResetAllPlayerAnim();
		ShowBeforeAnim ();
		
		pagePoint.PrePage();
		SetButtonData ();
		ChangePlayerParticle.Play ();
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
		modelTranArr[modelIndex].DOLocalMove(new Vector3(0, 50, 0), 0.1f).From(true);
		
		ResetAllPlayerAnim();
		ShowBeforeAnim ();
		
		pagePoint.NextPage();
		SetButtonData ();
		ChangePlayerParticle.Play ();
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
		ShowAotemanFamily ();
	}

	private void SetButtonData()
	{
		modelLevel = IDTool.GetModelLevel(modelAllStateIds[modelIndex]);
		modelType = IDTool.GetModelType(modelAllStateIds[modelIndex]);
		maxLevel = ModelData.Instance.GetMaxLevel (modelAllStateIds[modelIndex]);

		string[] temp = ConvertTool.StringToAnyTypeArray<string>(ModelData.Instance.GetDesc (modelAllStateIds[modelIndex]), '|');
		string tempDesc = "";
		for(int i = 0; i < temp.Length; i ++)
		{
			tempDesc += temp[i] + ((i == temp.Length - 1)? "" : "\n");
		}

		DescText.text = tempDesc;
//		DescText.text = temp[0] + "\n" + temp[1];// ModelData.Instance.GetDesc (modelType);

		SetUpgradeCount ();

		if(modelLevel == 0)
		{
			NameText.text = ModelData.Instance.GetName (modelAllStateIds[modelIndex]);
			upgradeType = ModelData.Instance.GetZhaohuanCostType(modelAllStateIds[modelIndex]);
			upgradeCount = ModelData.Instance.GetZhaohuanCost(modelAllStateIds[modelIndex]);

			goMaxLevelButton.SetActive(false);
			goZaohuanButton.SetActive(true);
			
			if(upgradeType.CompareTo("Coin") == 0)
			{
				CoinTypeGO.SetActive(true);
				JewelTypeGO.SetActive(false);
				CoinTypeCostTypeImage.SetSprite (upgradeType.ToLower() + "_icon");
				CoinTypeCostCountText.text = upgradeCount.ToString();
				CoinTypeBtnText.text = "Buy";
			}
			else
			{
				CoinTypeGO.SetActive(false);
				JewelTypeGO.SetActive(true);
				JewelTypeCostTypeImage.SetSprite (upgradeType.ToLower() + "_icon");
				JewelTypeCostCountText.text = upgradeCount.ToString();
				JewelTypeBtnText.text = "Buy";
			}
		}
		else
		{
			NameText.text = ModelData.Instance.GetName (modelAllStateIds[modelIndex]) + " Lvl" + modelLevel;
			if(modelLevel < maxLevel)
			{
				upgradeType = ModelData.Instance.GetUpgradeCostType(modelAllStateIds[modelIndex]+1);
				upgradeCount = ModelData.Instance.GetUpgradeCost(modelAllStateIds[modelIndex]+1);
				goMaxLevelButton.SetActive(false);
				goZaohuanButton.SetActive(true);

				if(upgradeType.CompareTo("Coin") == 0)
				{
					CoinTypeGO.SetActive(true);
					JewelTypeGO.SetActive(false);
					CoinTypeCostTypeImage.SetSprite (upgradeType.ToLower() + "_icon");
					CoinTypeCostCountText.text = upgradeCount.ToString();
					CoinTypeBtnText.text = "Up";
				}
				else
				{
					CoinTypeGO.SetActive(false);
					JewelTypeGO.SetActive(true);
					JewelTypeCostTypeImage.SetSprite (upgradeType.ToLower() + "_icon");
					JewelTypeCostCountText.text = upgradeCount.ToString();
					JewelTypeBtnText.text = "Up";
				}
			}
			else
			{
				goZaohuanButton.SetActive(false);
				goMaxLevelButton.SetActive(true);
			}
		}

        UpdateRemoveAdBtn();
    }
    #endregion

    public void UpdateRemoveAdBtn()
    {
        if(AndroidPackage.instance.isUseAds == false)
        {
            goRemoveAdBtn.SetActive(false);
        }
        else
        {
            goRemoveAdBtn.SetActive(!PlayerData.Instance.GetRemoveAds());
        }
        
    }

    private void UpgradeModelEvent()
	{
		if(upgradeType.CompareTo("Coin") == 0)
		{
			if(upgradeCount > PlayerData.Instance.GetItemNum(PlayerData.ItemType.Coin))
			{
				GiftPackageControllor.Instance.Show(PayType.CoinGift, UpgradeModelEvent);
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
				GiftPackageControllor.Instance.Show(PayType.JewelGift, UpgradeModelEvent);
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

		modelLevel = IDTool.GetModelLevel (modelAllStateIds [modelIndex]);

		upgradeBtnParticle.Play ();

		//自定义事件.
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Player_WakeUp, "Role call", ModelData.Instance.GetName (modelAllStateIds [modelIndex]));
	}

	#region 按钮控制
	public void MaxLevelOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		CharacterDetailControllor.Instance.SetModelIndex(modelIndex);
		UIManager.Instance.ShowModule(UISceneModuleType.CharacterDetail);
	}

	public void ZaohuanOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		modelLevel = IDTool.GetModelLevel(modelAllStateIds[modelIndex]);
		
		if(modelLevel == 0)
		{
			/*
				AotemanZhaohuanControllor.Instance.InitData(modelAllStateIds[modelIndex]);
				UIManager.Instance.ShowModule(UISceneModuleType.AotemanZhaohuan);
				*/
			UpgradeModelEvent ();
		}
		else
		{
			CharacterDetailControllor.Instance.SetModelIndex(modelIndex);
			UIManager.Instance.ShowModule(UISceneModuleType.CharacterDetail);
		}
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

	//经典模式、无尽模式
	public void ClassicGameOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		classGameBtnParticle.Play();
		
		modelLevel = IDTool.GetModelLevel(modelAllStateIds[modelIndex]);
		
		if(modelLevel == 0)
		{
			AotemanZhaohuanControllor.Instance.InitData(modelAllStateIds[modelIndex]);
			UIManager.Instance.ShowModule(UISceneModuleType.AotemanZhaohuan);
		}
		else
		{
			if(PlayerData.Instance.GetCurrentChallengeLevel() >= GlobalConst.UnlockWujinLevel)
			{
				PlayerData.Instance.SetGameMode ("WuJin");
				PlayerData.Instance.SetSelectedModel(modelAllStateIds[modelIndex]);

				PlayerData.Instance.SetSelectModelIsTestDrive (false);

				PlayerData.Instance.SaveData();

				GlobalConst.SceneName = SceneType.GameScene;
				LoadingPage.Instance.LoadScene ();
			}
			else
			{
				LockTipControllor.Instance.InitData (LockTipType.LockTip);
				UIManager.Instance.ShowModule (UISceneModuleType.LockTip);
			}
		}
	}

	//闯关模式
	public void LevelGameOnClick()
	{
        
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		modelLevel = IDTool.GetModelLevel(modelAllStateIds[modelIndex]);
		levelGameBtnParticle.Play();
		
		//新角色引导流程
		if(PlayerData.Instance.GetUIGuideState(UIGuideType.LeftRoleGuide) && PlayerData.Instance.GetCurrentChallengeLevel() == 4)
		{
			LevelSelectControllor.Instance.bMainInterfaceEnter = true;
			LevelInfoControllor.Instance.SetModelIndex(modelIndex);
			UIManager.Instance.ShowModule(UISceneModuleType.LevelSelect);
			return;
		}
		
		if(modelLevel == 0)
		{
			AotemanZhaohuanControllor.Instance.InitData(modelAllStateIds[modelIndex]);
			UIManager.Instance.ShowModule(UISceneModuleType.AotemanZhaohuan);
		}
		else
		{
			LevelSelectControllor.Instance.bMainInterfaceEnter = true;
			LevelInfoControllor.Instance.SetModelIndex(modelIndex);
			UIManager.Instance.ShowModule(UISceneModuleType.LevelSelect);
		}
	}

	public void TurnplateOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.Turnplate);
	}

	public void SettingOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.Setting);
	}

	public void ComplainOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.Complain);
	}

	public void ShopOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.Shop);
	}

	public void ConversionOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.ConvertCenter);
	}

	public void MissionOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.DailyMission);
	}

	public void AchieveOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.Achievement);
	}

	public void ActivityOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.Activity);
	}

	public void AotemanFamilyOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		AudioManger.Instance.StopSoundByName(ModelData.Instance.GetAotemanSound(modelAllStateIds[modelIndex]));
		UIManager.Instance.ShowModule(UISceneModuleType.AotemanFamily);
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_AoteBrother, GiftEventType.Activate);
	}

	public void DiscountGiftOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule (UISceneModuleType.DiscountGift);
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_LimitTime, GiftEventType.Activate);
	}

	public void ExchangeCodeOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.ExchangeCode);
	}

	public void ExchagneActivityOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule(UISceneModuleType.ExchangeActivity);
	}
	public void SignInOnclick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule (UISceneModuleType.SignIn);
	}

    public void RemoveAdsOnclick()
    {
        AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
        UIManager.Instance.ShowModule(UISceneModuleType.RemoveAds);
    }

    public void SmashEggOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule (UISceneModuleType.SmashEgg);
	}
	public void MonthCardGiftOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		UIManager.Instance.ShowModule (UISceneModuleType.MonthCardGift);
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_MonthCardGift,"State","手动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
	}

   
    #endregion
}
