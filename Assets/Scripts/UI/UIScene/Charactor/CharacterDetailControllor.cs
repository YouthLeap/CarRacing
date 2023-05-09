using UnityEngine;
using System.Collections;
using PathologicalGames;
using DG.Tweening;

public class CharacterDetailControllor : UIBoxBase {

	public static CharacterDetailControllor Instance;

	public GameObject goBackButton;
	public Transform LeftContent, RightContent;
	public GameObject SlideArea;

	public GameObject goLeftButton, goRightButton, goZaohuanButton, goOneKeyToFullButton, goMaxLevelButton;
	public Transform modelListContainer;
	
	public PagePoint pagePoint;
	public GameObject CoinTypeGO, JewelTypeGO;
	public tk2dSprite CoinTypeCostTypeImage, JewelTypeCostTypeImage;
	public EasyFontTextMesh CoinTypeCostCountText,JewelTypeCostCountText, CoinTypeBtnText,JewelTypeBtnText;
	public EasyFontTextMesh ZaohuanCountText;

	public int iCurModelId;
	private int maxLevel;
	private int skillLevel;
	private int modelIndex;
	private int modelCount;
	private int modelType;
	private int modelLevel;
	
	private string upgradeType;
	private int upgradeCount;
	
	private int[] playerModelStateIds, modelAllStateIds;
	private Transform[] modelTranArr;
	private AnimatorManager[] modelAnimArr;

	public EasyFontTextMesh textTitle;
	public EasyFontTextMesh textRunSpeedValue, textAccelerationValue, textShapeTimeValue;
	public ProgressBarNoMask imageRunSpeedCurLevel, imageRunSpeedNextLevel, imageAccelerationCurLevel, imageAccelerationNextLevel, imageShapeTimeCurLevel, imageShapeTimeNextLevel;
	public tk2dSprite imageMaxRunSpeedIcon, imageAccelerationIcon, imageShapeTimeOilIcon;
	public EasyFontTextMesh textBuff1Name, textBuff1Info;
	public tk2dSprite imageBuff1Icon;
	public EasyFontTextMesh enableDesc1Text;
	public ParticleSystem upgradeParticle, upgradeBtnParticle;

	#region 重写父类方法
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
			if(modelAllStateIds[i] % 100 != 0)
			{
				showId = modelAllStateIds[i];
			}
			else
			{
				showId = modelAllStateIds[i] + 1;
			}
			
			if(!ModelData.Instance.GetIsUse(showId))
				continue;
			
			Transform model = modelPool.Spawn(ModelData.Instance.GetPrefabName(showId));
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
			model.gameObject.SetActive(true);
		    
			//修改材质球，修改展示效果.
//			for(int j = 0; j < model.transform.childCount; j++)
//			{
//				if(model.transform.GetChild(j).GetComponent<SkinnedMeshRenderer>() != null)
//				{
//					Material mater = model.transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material;
//					if(GlobalConst.SceneName == SceneType.UIScene)
//						mater.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
//					else
//						mater.shader = Shader.Find("Mobile/Diffuse");
//				}
//			}


			model.gameObject.layer = 16;
			SetGameObjecLayer(model,16);

			
			modelAnimArr[i] = model.GetChild(1).GetComponent<AnimatorManager>();
			modelAnimArr[i].Init();
			//modelAnimArr[i].Show1();
			//modelAnimArr[i].Idle();
			model.gameObject.SetActive(false);
			modelTranArr[i] = model;
		}
		
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
		iCurModelId = modelAllStateIds[modelIndex];
	}

	public override void Show ()
	{
		PropertyDisplayControllor.Instance.ChangeLayer ();

		if (GlobalConst.IsSignIn && PlayerData.Instance.GetUIGuideState (UIGuideType.CharaterDetailGuide) && PlayerData.Instance.GetCurrentChallengeLevel () == 3) {
			if(modelIndex == 0 && IDTool.GetModelLevel (modelAllStateIds[modelIndex]) == 1)
			{
				UIGuideControllor.Instance.Show (UIGuideType.CharaterDetailGuide);
				UIGuideControllor.Instance.ShowBubbleTipByID(4);
			}
		}
		base.Show();
		DOTween.Kill (LeftContent);
		DOTween.Kill (RightContent);
		if(QualitySetting.IsHighQuality && PlayerData.Instance.GetUIGuideState (UIGuideType.CharaterDetailGuide) == false)
		{
			LeftContent.DOLocalMove (new Vector3 (-185, 0, 0), GlobalConst.PlayTime).SetEase (Ease.OutBack);
			RightContent.DOLocalMove (new Vector3 (210, 0, 0), GlobalConst.PlayTime).SetEase (Ease.OutBack);
		}
		else
		{
			LeftContent.localPosition = new Vector3 (-185, 0, 0);
			RightContent.localPosition = new Vector3 (210, 0, 0);
		}
		
		InitAnim ();
		pagePoint.Init (modelCount, modelIndex, false);
		SetButtonData ();
		CheckAutoGift();

		ResetAllPlayerAnim ();
		ShowModelAnim ();
		if (GlobalConst.SceneName == SceneType.UIScene) {
			SlideArea.SetActive(true);
			goLeftButton.GetComponent<BoxCollider>().enabled = true;
			goRightButton.GetComponent<BoxCollider>().enabled = true;
			goLeftButton.transform.Find("BtnImage").GetComponent<tk2dSprite>().color = Color.white;
			goRightButton.transform.Find("BtnImage").GetComponent<tk2dSprite>().color = Color.white;
		}
		else
		{
			SlideArea.SetActive(false);
			goLeftButton.GetComponent<BoxCollider>().enabled = false;
			goRightButton.GetComponent<BoxCollider>().enabled = false;
			goLeftButton.transform.Find("BtnImage").GetComponent<tk2dSprite>().color = new Color(0.5f, 0.5f, 0.5f, 1);
			goRightButton.transform.Find("BtnImage").GetComponent<tk2dSprite>().color = new Color(0.5f, 0.5f, 0.5f, 1);
			GameUIManager.Instance.ShowModule(UISceneModuleType.PropertyDisplay);
		}

		AudioManger.Instance.PlaySound (AudioManger.SoundName.UpgredeUi);
	}

	void CheckAutoGift()
	{
//		if(modelLevel <= AutoGiftChecker.MaxModelLevelOpenOneKeyToFull)
			Invoke("DelayCheckAutoGift", GlobalConst.PlayTime);
	}
	void DelayCheckAutoGift()
	{
		AutoGiftChecker.iEnterModelUpgradeTimes ++;
	}

	public override void Hide ()
	{
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
		if(GlobalConst.SceneName == SceneType.UIScene)
		{
			UIManager.Instance.HideModule (UISceneModuleType.CharacterDetail);
			MainInterfaceControllor.Instance.SetModelIndex (this.modelIndex);
			PropertyDisplayControllor.Instance.ChangeLayer ();
		}
		else
		{
			GameUIManager.Instance.HideModule(UISceneModuleType.CharacterDetail);
			GameUIManager.Instance.HideModule(UISceneModuleType.PropertyDisplay);
			GameRebornControllor.Instance.RefreshTipsNum();
		}
	}

	void HideCallBack()
	{
		gameObject.SetActive(false);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
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
		for(int i=0; i<modelCount; ++i)
		{
			if(IDTool.GetModelType(modelAllStateIds[i]) == IDTool.GetModelType(modelId))
			{
				modelAllStateIds[i] = modelId;
				iCurModelId = modelId;
				break;
			}
		}

		PlayerData.Instance.SetSelectedModel(modelId);

		SetButtonData ();

		if (IDTool.GetModelLevel (modelId) == 1)
			AchievementCheckRoleCount ();
		else if (IDTool.GetModelLevel (modelId) == maxLevel)
			AchievementCheckRoleFullLevel ();
	}

	public void AchievementCheckRoleCount()
	{
		int hasModelCount = 0;
		for(int i=0; i<modelCount; ++i)
		{
			if(IDTool.GetModelLevel(modelAllStateIds[i]) > 0)
			{
				++ hasModelCount;
			}
		}
	}

	public void AchievementCheckRoleFullLevel()
	{
		int fullLevelCount = 0;
		for(int i=0; i<modelCount; ++i)
		{
			if(IDTool.GetModelLevel(modelAllStateIds[i]) == maxLevel)
			{
				++ fullLevelCount;
			}
		}
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
		maxLevel = ModelData.Instance.GetMaxLevel (modelAllStateIds[modelIndex]);
		skillLevel = ModelData.Instance.GetSkillLevel (modelAllStateIds[modelIndex]);

		SetModelInfo (modelAllStateIds [modelIndex]);

		SetUpgradeCount ();
		
		if(modelLevel == 0)
		{
			goOneKeyToFullButton.SetActive(false);
			goMaxLevelButton.SetActive(false);
			goZaohuanButton.SetActive(true);
			goZaohuanButton.transform.localPosition = new Vector3(0, -185 + UIResAdjust.AdjustY(-1), 0);

			upgradeType = ModelData.Instance.GetZhaohuanCostType(modelAllStateIds[modelIndex]);
			upgradeCount = ModelData.Instance.GetZhaohuanCost(modelAllStateIds[modelIndex]);
			
			if(upgradeType.CompareTo("Coin") == 0)
			{
				CoinTypeGO.SetActive(true);
				JewelTypeGO.SetActive(false);
				CoinTypeCostTypeImage.SetSprite (upgradeType.ToLower() + "_icon");
				CoinTypeCostCountText.text = upgradeCount.ToString();
				CoinTypeBtnText.text = "Refit";
			}
			else
			{
				CoinTypeGO.SetActive(false);
				JewelTypeGO.SetActive(true);
				JewelTypeCostTypeImage.SetSprite (upgradeType.ToLower() + "_icon");
				JewelTypeCostCountText.text = upgradeCount.ToString();
				JewelTypeBtnText.text = "Refit";
			}
		}
		else
		{
			if(modelLevel < maxLevel)
			{
				goZaohuanButton.SetActive(true);
				if(PayJsonData.Instance.GetIsActivedState(PayType.OneKey2FullLV))
				{
				    goOneKeyToFullButton.SetActive(true);
					goOneKeyToFullButton.transform.localPosition = new Vector3(110, -185 + UIResAdjust.AdjustY(-1), 0);
					goZaohuanButton.transform.localPosition = new Vector3(-75, -185 + UIResAdjust.AdjustY(-1), 0);
				}else
				{
					goOneKeyToFullButton.SetActive(false);
					goZaohuanButton.transform.localPosition = new Vector3(0, -185 + UIResAdjust.AdjustY(-1), 0);
				}

				goMaxLevelButton.SetActive(false);

				upgradeType = ModelData.Instance.GetUpgradeCostType(modelAllStateIds[modelIndex]+1);
				upgradeCount = ModelData.Instance.GetUpgradeCost(modelAllStateIds[modelIndex]+1);
				
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
				goOneKeyToFullButton.SetActive(false);
				goZaohuanButton.SetActive(false);
				goMaxLevelButton.SetActive(true);
			}
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
	#endregion

	float fixedSpeedValue = 50;
	float fixedAccelerationValue = 15;
	float fixedPowerValue = 5;
	public void SetModelInfo(int Id)
	{
		modelLevel = IDTool.GetModelLevel(Id);
		modelType = IDTool.GetModelType (Id);

		int   iMaxLevel = ModelData.Instance.GetMaxLevel(Id);

		float fRunSpeed = ModelData.Instance.GetRunSpeed (Id);
		float fAcceleration = ModelData.Instance.GetAcceleration (Id);
		float fShapeTime = ModelData.Instance.GetShapeTime (Id);

		float fNextRunSpeed = ModelData.Instance.GetNextRunSpeed (Id);
		float fNextAcceleration = ModelData.Instance.GetNextAcceleration (Id);
		float fNextShapeTime = ModelData.Instance.GetNextShapeTime (Id);

		float fMaxRunSpeed = ModelData.Instance.GetMaxRunSpeed(Id);
		float fMAxAcceleration = ModelData.Instance.GetMaxAcceleration(Id);
		float fMaxShapeTime = ModelData.Instance.GetMaxShapeTime(Id);

		if(modelLevel == 0)
			textTitle.text = ModelData.Instance.GetName(Id);
		else
			textTitle.text = ModelData.Instance.GetName(Id) + " Lvl." + modelLevel;

		if(modelLevel < iMaxLevel)
		{
			textRunSpeedValue.text = fRunSpeed + "->" + fNextRunSpeed;
			textAccelerationValue.text = fAcceleration + "->" + fNextAcceleration;
			textShapeTimeValue.text = fShapeTime + "->" + fNextShapeTime;
		}else
		{
			textRunSpeedValue.text = fMaxRunSpeed.ToString ();
			textAccelerationValue.text = fAcceleration.ToString ();
			textShapeTimeValue.text = fShapeTime.ToString ();
		}

		imageRunSpeedCurLevel.SetProgress (fRunSpeed / fMaxRunSpeed);
		imageAccelerationCurLevel.SetProgress (fAcceleration / fMAxAcceleration);
		imageShapeTimeCurLevel.SetProgress (fShapeTime / fMaxShapeTime);

		imageRunSpeedNextLevel.SetProgress (fNextRunSpeed / fMaxRunSpeed);
		imageAccelerationNextLevel.SetProgress (fNextAcceleration / fMAxAcceleration);
		imageShapeTimeNextLevel.SetProgress (fNextShapeTime / fMaxShapeTime);

		textBuff1Name.text = ModelData.Instance.GetLevelSkill (Id);
		textBuff1Info.text = ModelData.Instance.GetLevelSkillDesc (Id);

		if (modelLevel < skillLevel) {
			enableDesc1Text.text = "Level：" + skillLevel; // Level activation
        } else {
			enableDesc1Text.text = "Level：" + skillLevel;
			//enableDesc1Text.text = "Activated";
		}

		imageBuff1Icon.SetSprite (ModelData.Instance.GetLevelSkillIcon (Id));
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
		iCurModelId = modelAllStateIds[modelIndex];
		PlayerData.Instance.UpdateModelState (modelAllStateIds [modelIndex]);
		modelLevel = IDTool.GetModelLevel (modelAllStateIds [modelIndex]);

		if (modelLevel == skillLevel) {
			AudioManger.Instance.PlaySound (AudioManger.SoundName.UpgredeMidLevel);
		} else if (modelLevel == maxLevel) {
			AudioManger.Instance.PlaySound (AudioManger.SoundName.UpgredeMaxLevel);
		}
		upgradeParticle.Play ();
		if(upgradeBtnParticle != null)
			upgradeBtnParticle.Play ();

		//自定义事件.
		if (modelLevel == 1) {
			CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Player_WakeUp, "Role call", ModelData.Instance.GetName (modelAllStateIds [modelIndex]));
		} else {
			CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Player_Upgrade, ModelData.Instance.GetName (iCurModelId), iCurModelId.ToString ());
		}
	}

	#region 按钮控制
	public void BackOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide ();
	}

	public void OneKeyToFullOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		OneKeyToFullLevelControllor.Instance.InitData (modelAllStateIds[modelIndex]);
		if(GlobalConst.SceneName == SceneType.UIScene)
			UIManager.Instance.ShowModule(UISceneModuleType.OneKeyToFullLevel);
		else
			GameUIManager.Instance.ShowModule(UISceneModuleType.OneKeyToFullLevel);
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_OneKeyFullLevel, GiftEventType.Activate);
	}

	public void ZaohuanOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		modelLevel = IDTool.GetModelLevel(modelAllStateIds[modelIndex]);
		UpgradeModelEvent();
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
	#endregion
}
