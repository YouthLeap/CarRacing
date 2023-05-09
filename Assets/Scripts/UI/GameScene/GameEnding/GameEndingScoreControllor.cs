using UnityEngine;
using System.Collections;
using DG.Tweening;
using PathologicalGames;

public class GameEndingScoreControllor : UIBoxBase {

	public static GameEndingScoreControllor Instance;

	public GameObject  goLevelModeRestartBtn, goNextLevelBtn, goClassicModeCloseBtn, goClassicModeRestartBtn;
	public GameObject goLevelMode, goClassicMode;

	#region  闯关模式变量
	public EasyFontTextMesh textNextLevelBtn;
	public Transform[] transArrStarsFront;
	public Transform[] tranArrProps;
	public tk2dSprite[] imageArrPropsIcon;
	public EasyFontTextMesh[] textPropsCount;
	public Transform LeftContent, RightContent, tranCharacterParent;
	public EasyFontTextMesh textLevelScoreCount, textLevelCoinCount;
	public ParticleSystem WinPS;
	public tk2dSprite titleSprite;

	public GameObject starBGLight,starBGGray,barLight,barGray;

	private int iOldChallengeLevel, iCurLevel, iMaxLevel = 40;
	private int iStarCount, iOldStarCount;

	public GameObject goLevelColorEgg;
	public EasyFontTextMesh textLevelColorEggCount;

	private bool showLockTip;
	#endregion

	#region  经典模式变量
	public EasyFontTextMesh textClassicJewelCount, textClassicCoinCount;
	public EasyFontTextMesh textClassDistance;

	public GameObject goClassColorEgg;
	public EasyFontTextMesh textClassicColorEggCount;

	#endregion

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show ();
		transform.localPosition = ShowPosV2;


		if (PlayerData.Instance.IsWuJinGameMode ()) {
			goLevelMode.SetActive (false);
			goClassicMode.SetActive (true);

			InitClassicMode ();
		} else {
			goLevelMode.SetActive (true);
			goClassicMode.SetActive (false);

			InitLevelMode ();
		}
	}
	
	public override void Hide ()
	{

		if(PlayerData.Instance.GetGameMode().CompareTo("Level") == 0)
		{
			modelPool.Despawn(model);
		}

		transform.localPosition = GlobalConst.LeftHidePos;
		GameUIManager.Instance.HideModule(UISceneModuleType.GameEndingScore);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		GlobalConst.ShowModuleType = UISceneModuleType.MainInterface;
		GlobalConst.SceneName = SceneType.UIScene;
		LoadingPage.Instance.LoadScene ();
	}
	#endregion

	#region 闯关模式处理
	SpawnPool modelPool;
	Transform model;
	void ShowLevelModeAni()
	{
		LeftContent.localPosition = GlobalConst.LeftHidePos;
		RightContent.localPosition = GlobalConst.RightHidePos;
		DOTween.Kill(LeftContent);
		DOTween.Kill(RightContent);
		LeftContent.DOLocalMove (new Vector3 (0, 0, 0), GlobalConst.PlayTime).SetEase (Ease.OutBack);
		RightContent.DOLocalMove (new Vector3 (0, 0, 0), GlobalConst.PlayTime).SetEase (Ease.OutBack).OnComplete(ShowStartEffect);
	}

	void ShowLevelMode()
	{
		LeftContent.localPosition = new Vector3 (0, 0, 0);
		RightContent.localPosition = new Vector3 (0, 0, 0);
	}

	void InitLevelMode()
	{
		iCurLevel = PlayerData.Instance.GetSelectedLevel();
		iOldChallengeLevel = PlayerData.Instance.GetCurrentChallengeLevel();

		float fCoinAddition = GameData.Instance.AddCoinPecent - 1;
		int fCoinCount = Mathf.RoundToInt(GameData.Instance.curCoin * GameData.Instance.AddCoinPecent);

		iStarCount = GameData.Instance.StarCount;
		iOldStarCount = PlayerData.Instance.GetLevelStarState(iCurLevel);

		if(GameData.Instance.IsWin)
		{
			//Add expertience
			PlayerData.Instance.AddItemNum(PlayerData.ItemType.ColorEgg,GameData.Instance.curEggCount);

			if(GameData.Instance.curEggCount > 0)
			{
				goLevelColorEgg.SetActive(false);
				textLevelColorEggCount.text ="x"+GameData.Instance.curEggCount.ToString();
			}else{
				goLevelColorEgg.SetActive(false);
			}

			ShowLevelModeAni();
			SetButtonEnable(false);

			starBGGray.SetActive(false);
			starBGLight.SetActive(true);
			barGray.SetActive(false);
			barLight.SetActive(true);

			textNextLevelBtn.text = "Next level";
			titleSprite.SetSprite ("text_crosslevel");
			WinPS.gameObject.SetActive(true);
			AudioManger.Instance.PlaySound(AudioManger.SoundName.Win);
			PlayerData.Instance.SetCurrentChallengeLevel((iCurLevel == iOldChallengeLevel && iCurLevel < iMaxLevel)? iCurLevel + 1 : iOldChallengeLevel);

			if (iCurLevel == iOldChallengeLevel
			   && iCurLevel == GlobalConst.UnlockWujinLevel - 1
			   && PlatformSetting.Instance.PayVersionType != PayVersionItemType.ShenHe
			   && PlatformSetting.Instance.PayVersionType != PayVersionItemType.ChangWan
			   && PlatformSetting.Instance.PayVersionType != PayVersionItemType.GuangDian)
				showLockTip = true;
			else
				showLockTip = false;

			if(GameData.Instance.completeMissionIds.Count>0)
			{
			    PlayerData.Instance.SetLevelMissionState(iCurLevel, ConvertTool.StringToAnyTypeArray<int>(ConvertTool.AnyTypeListToString(GameData.Instance.completeMissionIds, "*"), '*'));
			}
			PlayerData.Instance.AddItemNum(PlayerData.ItemType.Coin, fCoinCount);

			if(PlayerData.Instance.GetNewPlayerState())
				PlayerData.Instance.SetNewPlayerToFalse();


			if(CarManager.Instance.gameLevelModel == GameLevelModel.LimitTime)
			{
				textLevelScoreCount.text = ((int)(CarManager.Instance.playerUseTime))+ "(s)";
			}else
			{
				textLevelScoreCount.text = "Rank " + CarManager.Instance.finalRank+"";
			}

			textLevelCoinCount.text  = fCoinCount.ToString();


			SetProps();
			GamePassedGfitControllor.Instance.SaveData();
			//自定义事件.
			CollectInfoEvent.SendEventWithCount (CollectInfoEvent.EventType.Level_, iCurLevel.ToString(), "关卡结果" , "胜利", "星星数目", iStarCount.ToString());
			CollectInfoEvent.FinishLevel(iCurLevel);
			PlayerData.Instance.SetEachLevelFailCount(0);

			DailyMissionCheckManager.Instance.Check(DailyMissionType.LevelModel,1);

			CheckAchievement();

		}else
		{
			goLevelColorEgg.SetActive(false);

			ShowLevelMode();
		
			starBGGray.SetActive(true);
			starBGLight.SetActive(false);
			barGray.SetActive(true);
			barLight.SetActive(false);

			textNextLevelBtn.text = "Level info";
			titleSprite.SetSprite ("again_word");
			WinPS.gameObject.SetActive(false);
			AudioManger.Instance.PlaySound(AudioManger.SoundName.Lose);
			if(CarManager.Instance.gameLevelModel == GameLevelModel.Weedout)
			{
				textLevelScoreCount.text = ((int)(CarManager.Instance.playerUseTime))+"(s)";
			}else
			{
				textLevelScoreCount.text ="Rank "+CarManager.Instance.finalRank+"";
			}
			textLevelCoinCount.text  = "0";


			for(int i = 0; i < tranArrProps.Length; i ++)
			{
				tranArrProps[i].gameObject.SetActive(false);
			}

			for(int i = 0; i < transArrStarsFront.Length; i ++)
			{
				transArrStarsFront[i].gameObject.SetActive(false);
			}

			//设置关卡失败次数
			PlayerData.Instance.SetEachLevelFailCount(1);

			//自定义事件.
			CollectInfoEvent.SendEventWithCount (CollectInfoEvent.EventType.Level_, iCurLevel.ToString(), "关卡结果" , "失败", "星星数目", iStarCount.ToString());
			CollectInfoEvent.FailLevel(iCurLevel);
		}

		//显示角色
		SkinnedMeshRenderer skin;
		modelPool = PoolManager.Pools["CharactersPool"];
		model = modelPool.Spawn(ModelData.Instance.GetPrefabName(PlayerData.Instance.GetSelectedModel()));
		model.parent = tranCharacterParent;
		model.localPosition = Vector3.zero;
		model.localRotation = Quaternion.Euler(0, 0, 0);
		model.localScale = new Vector3(45, 45, 45);
		model.gameObject.SetActive(true);
		model.gameObject.SetActive(true);
			
		model.gameObject.layer = 15;
		for(int j=0; j<model.childCount; ++j)
		{
			model.GetChild(j).gameObject.layer = 15;
		}
			
	}

	void CheckAchievement()
	{
		AchievementCheckManager.Instance.Check(AchievementType.UsePropCount_Total,GameData.Instance.itemUseCount);
		AchievementCheckManager.Instance.Check(AchievementType.WinCountTotal,1);
		switch(CarManager.Instance.gameLevelModel)
		{
		case GameLevelModel.DualMeet:
			AchievementCheckManager.Instance.Check(AchievementType.DealMeatWinCount,1);
			break;
		case GameLevelModel.LimitTime:
			AchievementCheckManager.Instance.Check(AchievementType.LimitTimeWinCount,1);
			break;
		case GameLevelModel.Rank:
			AchievementCheckManager.Instance.Check(AchievementType.RankWinCount,1);
			break;
		case GameLevelModel.Weedout:
			AchievementCheckManager.Instance.Check(AchievementType.WeedoutWinCount,1);
			break;
		}
		
		int[] starArray= PlayerData.Instance.GetLevelStarState();
		int starCount=0;
		for(int i=0;i<starArray.Length;++i)
		{
			starCount+=starArray[i];
		}
		AchievementCheckManager.Instance.Check(AchievementType.Level_Star_Count_Total,starCount);
		
		int maxGameLevel = PlayerData.Instance.GetCurrentChallengeLevel();
		AchievementCheckManager.Instance.Check(AchievementType.Level_Count,maxGameLevel);
	}
	
	void SetProps()
	{
		int iPropsNum = 0;
		PlayerData.ItemType[] PropsType = new PlayerData.ItemType[GameData.Instance.RewardDic.Count];
		int[] iArrCounts = new int[GameData.Instance.RewardDic.Count];
		PlayerData.ItemType tempType;
		
		foreach(PlayerData.ItemType type in GameData.Instance.RewardDic.Keys)
		{
			if(GameData.Instance.RewardDic[type] > 0)
			{
				PropsType[iPropsNum++] = type;
				iArrCounts[iPropsNum - 1] = GameData.Instance.RewardDic[type];
				PlayerData.Instance.AddItemNum(type, iArrCounts[iPropsNum - 1]);
			}
		}
		
		for(int i = 0; i < tranArrProps.Length; i ++)
		{
			tranArrProps[i].gameObject.SetActive(i < iPropsNum);
			
			tempType = PropsType[i];
			if(i < iPropsNum)
			{         
				switch(tempType)
				{
				case PlayerData.ItemType.Apple:
					imageArrPropsIcon[i].SetSprite (RewardData.Instance.GetIconName(10));
					break;
				case PlayerData.ItemType.Banana:
					imageArrPropsIcon[i].SetSprite (RewardData.Instance.GetIconName(12));
					break;
				case PlayerData.ItemType.Ganoderma:
					imageArrPropsIcon[i].SetSprite (RewardData.Instance.GetIconName(15));
					break;
				case PlayerData.ItemType.Ginseng:
					imageArrPropsIcon[i].SetSprite (RewardData.Instance.GetIconName(14));
					break;
				case PlayerData.ItemType.Pear:
					imageArrPropsIcon[i].SetSprite (RewardData.Instance.GetIconName(11));
					break;
				case PlayerData.ItemType.Grape:
					imageArrPropsIcon[i].SetSprite (RewardData.Instance.GetIconName(13));
					break;
				}

				textPropsCount[i].text = "x" + iArrCounts[i];
				//tranArrProps[i].localPosition = new Vector3(fGiftPosMinX + (fGiftPosMaxX - fGiftPosMinX) / (iPropsNum * 2) * (2 * i + 1), fGiftPosY, 0);
			}
		}
	}

	void ShowStartEffect()
	{
		for(int i = 0; i < transArrStarsFront.Length; i ++)
		{
			transArrStarsFront[i].gameObject.SetActive(false);
		}

		StartCoroutine("ShowStartEffectIE");
	}

	[HideInInspector]public Vector3 FronStarTempScale;
	private Transform tranFronStarTemp;
	private ParticleSystem FronStarParticle;
	IEnumerator ShowStartEffectIE()
	{
		for(int i = 0; i < transArrStarsFront.Length; i ++)
		{
			tranFronStarTemp = transArrStarsFront[i];
			FronStarParticle = tranFronStarTemp.Find("UI_XingXing").GetComponent<ParticleSystem>();
			
			if(i < iStarCount && GameData.Instance.IsWin)
			{
				FronStarTempScale = new Vector3(1.8f, 1.8f, 1.8f);
				tranFronStarTemp.localScale = FronStarTempScale;
				tranFronStarTemp.gameObject.SetActive(true);

				yield return new WaitForSeconds(0.2f);
				DOTween.Kill("StarEffectTween");
				DOTween.To(()=> FronStarTempScale, x => FronStarTempScale = x, new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.InOutBack).OnUpdate(UpdateStarScale).SetId("StarEffectTween");
				AudioManger.Instance.PlaySound(AudioManger.SoundName.Star);
				FronStarParticle.Play();
				yield return new  WaitForSeconds(0.5f);
			}else
			{
				tranFronStarTemp.gameObject.SetActive(false);
			}
		}

		if(iStarCount > 0 && QualitySetting.IsHighQuality && GameData.Instance.IsWin)
		{
			yield return new WaitForSeconds(0.5f);
		}

		if(iStarCount > iOldStarCount || LevelData.Instance.GetPassGameGiftState(iCurLevel))
		{
			GameUIManager.Instance.ShowModule(UISceneModuleType.GamePassedGfit);
		}else
		{
			SetButtonEnable(true);
		}
	}

	void UpdateStarScale()
	{
		tranFronStarTemp.localScale = FronStarTempScale;
	}
	#endregion

	#region 经典模式处理
	private float fGiftPosMinX = -120, fGiftPosMaxX = 120, fGiftPosY = 0;
	void InitClassicMode()
	{
		PlayerData.Instance.AddItemNum(PlayerData.ItemType.ColorEgg,GameData.Instance.curEggCount);

		float fCoinAddition = GameData.Instance.AddCoinPecent - 1;
		int iCoinCount = Mathf.RoundToInt(GameData.Instance.curCoin * GameData.Instance.AddCoinPecent);
		PlayerData.Instance.AddItemNum(PlayerData.ItemType.Coin, iCoinCount);
		textClassicCoinCount.text  = iCoinCount.ToString();

		int distance = ((int)PlayerCarControl.Instance.carMove.moveLen);
		textClassicJewelCount.text =WuJingConfigData.Instance.GetJewelCountByDistance(distance).ToString();
		textClassDistance.text =  distance.ToString ()+ " m";

		CollectInfoEvent.FinishLevel(0);

		if(GameData.Instance.curEggCount > 0)
		{
			goClassColorEgg.SetActive(true);
			textClassicColorEggCount.text = "x"+GameData.Instance.curEggCount.ToString();
		}else{
			goLevelColorEgg.SetActive(false);
		}
		//游戏进行时活动
		if(PlatformSetting.Instance.isOpenGamePlayingActivity)
		{
			PlayerData.Instance.SetGamePlayingActivityScore((int)PlayerCarControl.Instance.carMove.moveLen + PlayerData.Instance.GetGamePlayingActivityScore());
		}
		//幸运数字活动
		if(PlatformSetting.Instance.isOpenLuckyNumbersActivity)
		{
			if( (int)PlayerCarControl.Instance.carMove.moveLen % 10 == 1)
			{
				PlayerData.Instance.SetLuckyNumbersOneState(true);
			}
			else if( (int)PlayerCarControl.Instance.carMove.moveLen * 100 % 10 == 6)
			{
				PlayerData.Instance.SetLuckyNumbersSixState(true);
			}
			else if( (int)PlayerCarControl.Instance.carMove.moveLen % 10 == 8)
			{
				PlayerData.Instance.SetLuckyNumbersEightState(true);
			}
			else if( (int)PlayerCarControl.Instance.carMove.moveLen % 1000 == 168)
			{
				PlayerData.Instance.SetLuckyNumbersOneSixEightState(true);
			}
		}
		DailyMissionCheckManager.Instance.Check(DailyMissionType.ClassModel,1);
	}
	#endregion

	#region 按钮控制
	void CheckAutoGift()
	{
		int modelLevel = IDTool.GetModelLevel(PlayerData.Instance.GetSelectedModel());
		OneKeyToFullLevelControllor.Instance.bOneKeyToFullLevelBoxIsHidden = false;

		if (PlayerData.Instance.GetGameMode ().CompareTo ("Level") == 0
		    && GameData.Instance.IsWin == false
		    && PayJsonData.Instance.GetIsActivedState (PayType.OneKey2FullLV)
		    && modelLevel <= AutoGiftChecker.MaxModelLevelOpenOneKeyToFull
		    && PlatformSetting.Instance.PayVersionType != PayVersionItemType.ShenHe
		    && PlatformSetting.Instance.PayVersionType != PayVersionItemType.ChangWan
		    && PlatformSetting.Instance.PayVersionType != PayVersionItemType.GuangDian) {
			OneKeyToFullLevelControllor.Instance.InitData (PlayerData.Instance.GetSelectedModel ());
			GameUIManager.Instance.ShowModule (UISceneModuleType.OneKeyToFullLevel);
		} else {
			OneKeyToFullLevelControllor.Instance.bOneKeyToFullLevelBoxIsHidden = true;
		}
	}

	IEnumerator CloseBoxIE()
	{
		CheckAutoGift();

		while (OneKeyToFullLevelControllor.Instance.bOneKeyToFullLevelBoxIsHidden == false) 
		{
			yield return 0;
		}

		if(showLockTip)
		{
			GlobalConst.ShowModuleType = UISceneModuleType.LockTip;
			GlobalConst.SceneName = SceneType.UIScene;
			LoadingPage.Instance.LoadScene ();
		}
		else
		{
			GlobalConst.ShowModuleType = UISceneModuleType.MainInterface;
			GlobalConst.SceneName = SceneType.UIScene;
			LoadingPage.Instance.LoadScene ();
		}
	}

	IEnumerator LevelModeRestartIE()
	{
		CheckAutoGift();
		
		while (OneKeyToFullLevelControllor.Instance.bOneKeyToFullLevelBoxIsHidden == false) 
		{
			yield return 0;
		}

		if(showLockTip)
		{
			GlobalConst.ShowModuleType = UISceneModuleType.LockTip;
			GlobalConst.SceneName = SceneType.UIScene;
			LoadingPage.Instance.LoadScene ();
		}else if(AutoGiftChecker.ForeseeAutoGiftCheck(AutomaticGiftName.HuiKuiBigGift))
		{
			GlobalConst.ShowModuleType = UISceneModuleType.MainInterface;
			GlobalConst.SceneName = SceneType.UIScene;
			LoadingPage.Instance.LoadScene ();
		}
		else
		{
			if(PlayerData.Instance.GetItemNum(PlayerData.ItemType.Strength) > 0)
			{
				PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Strength, 1);
				GameController.Instance.RestartGame();
			}
			else
			{
				GiftPackageControllor.Instance.Show(PayType.PowerGift, RestartCall);
			}
		}
	}

	IEnumerator NextLevelIE()
	{
		CheckAutoGift();
		
		while (OneKeyToFullLevelControllor.Instance.bOneKeyToFullLevelBoxIsHidden == false) 
		{
			yield return 0;
		}

		if(showLockTip)
		{
			GlobalConst.ShowModuleType = UISceneModuleType.LockTip;
			GlobalConst.SceneName = SceneType.UIScene;
			LoadingPage.Instance.LoadScene ();
		}
		else
		{
			if(AutoGiftChecker.ForeseeAutoGiftCheck(AutomaticGiftName.HuiKuiBigGift))
			{
				GlobalConst.ShowModuleType = UISceneModuleType.MainInterface;
				GlobalConst.SceneName = SceneType.UIScene;
				LoadingPage.Instance.LoadScene ();
				
			}else if(AutoGiftChecker.ForeseeAutoGiftCheck(AutomaticGiftName.DoubleCoin)  || AutoGiftChecker.ForeseeAutoGiftCheck(AutomaticGiftName.NewPlayerGift) )
			{
				//自动弹出双倍金币 新手礼包 与关卡信息显示冲突
				GlobalConst.ShowModuleType = UISceneModuleType.LevelSelect;
				GlobalConst.SceneName = SceneType.UIScene;
				LoadingPage.Instance.LoadScene ();
			}else{
				GlobalConst.ShowModuleType = UISceneModuleType.LevelInfo;
				GlobalConst.SceneName = SceneType.UIScene;
				LoadingPage.Instance.LoadScene ();
			}
		}
	}

	public void CloseOnClick()
	{
		if(isButtonEnable==false)
		{
			return;
		}

			AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
			goLevelMode.SetActive(false);
			StartCoroutine("CloseBoxIE");
	}

	public void LevelModeRestartOnClick()
	{
		if(isButtonEnable==false)
		{
			return;
		}
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		//goLevelMode.SetActive(false);
		StartCoroutine("LevelModeRestartIE");
	}
		
	public void NextLevelOnClick()	
	{
		if(isButtonEnable==false)
		{
			return;
		}
		//goLevelMode.SetActive(false);
		StartCoroutine("NextLevelIE");
		return;
	}

	public void ClassicModeRestartOnClick()
	{
		if(isButtonEnable==false)
		{
			return;
		}
		GameController.Instance.RestartGame();
		return;
	}


	private void RestartCall()
	{
		if(PlayerData.Instance.GetItemNum(PlayerData.ItemType.Strength) > 0)
		{
			PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Strength, 1);
			GameController.Instance.RestartGame();
		}
		else
		{
			GiftPackageControllor.Instance.Show(PayType.PowerGift, RestartCall);
			return;
		}
	}

	private bool isButtonEnable=true;
	public void SetButtonEnable(bool enable)
	{
		isButtonEnable=enable;
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = enable;
		goLevelModeRestartBtn.GetComponent<BoxCollider>().enabled = enable;
		goNextLevelBtn.GetComponent<BoxCollider>().enabled = enable;
		goClassicModeCloseBtn.GetComponent<BoxCollider>().enabled = enable;
		goClassicModeRestartBtn.GetComponent<BoxCollider>().enabled = enable;;
	}
	#endregion
}
