using UnityEngine;
using System.Collections;
using PathologicalGames;

public class GameUIManager : MonoBehaviour {

	public static GameUIManager Instance;

	public Transform UICanvas, FirstCanvas, SecondCanvas, ThirdCanvas;

	Transform tranUIMask, tranDialog;

	UIBoxBase hGamePlayerUIControllor;           Transform tranGamePlayerUI;
	UIBoxBase hGamePauseControllor;              Transform tranGamePause;
	UIBoxBase hGameRebornControllor;             Transform tranGameReborn;
	UIBoxBase hGameGiftPackageControllor;        Transform tranGameGiftPackage;
	UIBoxBase hGameEndingScoreControllor;        Transform tranGameEndingScore;
	UIBoxBase hGamePassedGfitControllor;         Transform tranGamePassedGfit;
	UIBoxBase hCharacterDetailControllor;        Transform tranCharacterDetail;
	UIBoxBase hPropertyDisplayControllor;        Transform tranPropertyDisplay;
	UIBoxBase hShopControllor;        			 Transform tranShop;
	UIBoxBase hOneKeyToFullLevelControllor;      Transform tranOneKeyToFullLevel;
	UIBoxBase hHintInGameControllor;             Transform tranHintInGame;
	UIBoxBase hShopSecondSureControllor;     	 Transform tranShopSecondSure;
	UIBoxBase hStrengthControllor;      	 	 Transform tranStrength;
	UIBoxBase hGameResumeControllor;             Transform tranGameResume;
	UIBoxBase hUIGuideControllor;     		 	 Transform tranUIGuide;
	UIBoxBase hGameSkillControllor;              Transform tranGameSkill;
	UIBoxBase hLevelGiftControllor;           	 Transform tranLevelGift;
	UIBoxBase hProtectShieldControllor;          Transform tranProtectShield;
	UIBoxBase hGameRankControllor;              Transform tranGameRank;

	public UISceneModuleType curBoxType;
	SpawnPool spUIModules;

	void Awake ()
	{
		Instance = this;
	}
	
	public void Init () {
		if (GlobalConst.FirstIn) {
			GlobalConst.FirstIn = false;

			GameObject PlatformSetting = (GameObject)Instantiate(Resources.Load("UIScene/PlatformSetting"));
			PlatformSetting.SetActive(true);

			GameObject LDCanvas = (GameObject)Instantiate(Resources.Load ("UIScene/LoadingPage"));
			GlobalConst.SceneName = SceneType.GameScene;
			LDCanvas.GetComponentInChildren<LoadingPage>().InitScene ();
		}
		spUIModules = PoolManager.Pools["UIModulesPool"];
		StartCoroutine ("UIBoxInit");

		EventLayerController.Instance.Init ();
	}

	IEnumerator UIBoxInit()
	{
		curBoxType = UISceneModuleType.GamePlayerUI;

		//transform
		yield return new WaitForEndOfFrame ();
		tranUIMask = CreateUIBox ("TranslucentUIMask", transform);
		tranUIMask.GetComponent<TranslucentUIMaskManager>().Init ();

		//UICanvas
		yield return new WaitForEndOfFrame ();
		tranGamePlayerUI = CreateUIBox (UISceneModuleType.GamePlayerUI.ToString (), UICanvas);
		hGamePlayerUIControllor = tranGamePlayerUI.GetComponent<GamePlayerUIControllor> ();
		hGamePlayerUIControllor.Init();

		yield return new WaitForEndOfFrame ();
		tranGameSkill = CreateUIBox(UISceneModuleType.GameSkill.ToString(), UICanvas);
		hGameSkillControllor = tranGameSkill.GetComponent<GameSkillControl>();
		hGameSkillControllor.Init();

		//游戏UI准备标志
		GlobalConst.IsUIReady = true;

		//DialogBox
		yield return new WaitForEndOfFrame ();
		tranDialog = CreateUIBox ("DialogBox",ThirdCanvas);
		tranDialog.GetComponent<DialogBox> ().Init ();

		yield return new WaitForEndOfFrame ();
		tranUIGuide = CreateUIBox (UISceneModuleType.UIGuide.ToString (), ThirdCanvas);
		hUIGuideControllor = tranUIGuide.GetComponent<UIGuideControllor> ();
		hUIGuideControllor.Init ();
		yield return new WaitForEndOfFrame ();
		tranHintInGame = CreateUIBox(UISceneModuleType.HintInGame.ToString(), UICanvas);
		hHintInGameControllor = tranHintInGame.GetComponent<HintInGameControllor>();
		hHintInGameControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranGameResume = CreateUIBox(UISceneModuleType.GameResume.ToString(), UICanvas);
		hGameResumeControllor = tranGameResume.GetComponent<GameResumeControllor>();
		hGameResumeControllor.Init();

		//FirstCanvas
		yield return new WaitForEndOfFrame ();
		tranPropertyDisplay = CreateUIBox(UISceneModuleType.PropertyDisplay.ToString(), FirstCanvas);
		hPropertyDisplayControllor = tranPropertyDisplay.GetComponent<PropertyDisplayControllor>();
		hPropertyDisplayControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranShop = CreateUIBox (UISceneModuleType.Shop.ToString (), FirstCanvas);
		hShopControllor = tranShop.GetComponent<ShopControllor> ();
		hShopControllor.Init ();
		yield return new WaitForEndOfFrame ();
		tranGamePause = CreateUIBox(UISceneModuleType.GamePause.ToString(), FirstCanvas);
		hGamePauseControllor = tranGamePause.GetComponent<GamePauseControllor>();
		hGamePauseControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranGameRank = CreateUIBox(UISceneModuleType.GameRank.ToString(), FirstCanvas);
		hGameRankControllor = tranGameRank.GetComponent<GameRankControler>();
		hGameRankControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranGameReborn = CreateUIBox(UISceneModuleType.GameReborn.ToString(), FirstCanvas);
		hGameRebornControllor = tranGameReborn.GetComponent<GameRebornControllor>();
		hGameRebornControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranGameEndingScore = CreateUIBox(UISceneModuleType.GameEndingScore.ToString(), FirstCanvas);
		hGameEndingScoreControllor = tranGameEndingScore.GetComponent<GameEndingScoreControllor>();
		hGameEndingScoreControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranCharacterDetail = CreateUIBox(UISceneModuleType.CharacterDetail.ToString(), FirstCanvas);
		hCharacterDetailControllor = tranCharacterDetail.GetComponent<CharacterDetailControllor>();
		hCharacterDetailControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranLevelGift = CreateUIBox(UISceneModuleType.LevelGift.ToString(), FirstCanvas);
		hLevelGiftControllor = tranLevelGift.GetComponent<LevelGiftControllor>();
		hLevelGiftControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranProtectShield = CreateUIBox(UISceneModuleType.ProtectShield.ToString(), FirstCanvas);
		hProtectShieldControllor = tranProtectShield.GetComponent<ProtectShieldControllor>();
		hProtectShieldControllor.Init();

		//SecondCanvas
		yield return new WaitForEndOfFrame ();
		tranGamePassedGfit = CreateUIBox(UISceneModuleType.GamePassedGfit.ToString(), SecondCanvas);
		hGamePassedGfitControllor = tranGamePassedGfit.GetComponent<GamePassedGfitControllor>();
		hGamePassedGfitControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranOneKeyToFullLevel = CreateUIBox(UISceneModuleType.OneKeyToFullLevel.ToString(), SecondCanvas);
		hOneKeyToFullLevelControllor = tranOneKeyToFullLevel.GetComponent<OneKeyToFullLevelControllor>();
		hOneKeyToFullLevelControllor.Init();
		yield return new WaitForEndOfFrame ();
		tranShopSecondSure = CreateUIBox (UISceneModuleType.ShopSecondSure.ToString (), SecondCanvas);
		hShopSecondSureControllor = tranShopSecondSure.GetComponent<ShopSecondSureControllor> ();
		hShopSecondSureControllor.Init ();
		yield return new WaitForEndOfFrame ();
		tranStrength = CreateUIBox (UISceneModuleType.Strength.ToString (), SecondCanvas);
		hStrengthControllor = tranStrength.GetComponent<StrengthControllor> ();
		hStrengthControllor.Init ();

		//ThirdCanvas
		yield return new WaitForEndOfFrame ();
		tranGameGiftPackage = CreateUIBox(UISceneModuleType.GiftPackage.ToString(), ThirdCanvas);
		hGameGiftPackageControllor = tranGameGiftPackage.GetComponent<GiftPackageControllor>();
		hGameGiftPackageControllor.Init();
	}

	Transform CreateUIBox(string prefabName, Transform parentTran)
	{
		Transform moduleTran = spUIModules.Spawn (prefabName);
		moduleTran.SetParent (parentTran, false);
		moduleTran.localPosition = Vector3.zero;//GlobalConst.TopHidePos;
		moduleTran.gameObject.SetActive (false);
		return moduleTran;
	}
	
	public void ShowModule(UISceneModuleType boxType)
	{
		switch(boxType)
		{
		case UISceneModuleType.GamePlayerUI:
			hGamePlayerUIControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.PropertyDisplay:
			break;
		case UISceneModuleType.GamePause:
			hGamePauseControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.GameRank:
			hGameRankControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.GameReborn:
			hGameRebornControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.GiftPackage:
			hGameGiftPackageControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.GameEndingScore:
			hGameEndingScoreControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.CharacterDetail:
			hCharacterDetailControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.OneKeyToFullLevel:
			hOneKeyToFullLevelControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.GamePassedGfit:
			hGamePassedGfitControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.HintInGame:
			hHintInGameControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.GameResume:
			hGameResumeControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.Shop:
			hShopControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.Strength:
			hStrengthControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.ShopSecondSure:
			hShopSecondSureControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.UIGuide:
			hUIGuideControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.GameSkill:
			hGameSkillControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.LevelGift:
			hLevelGiftControllor.preBoxType = curBoxType;
			break;
		case UISceneModuleType.ProtectShield:
			hProtectShieldControllor.preBoxType = curBoxType;
			break;
		}

		if (boxType != UISceneModuleType.PropertyDisplay) {
			curBoxType = boxType;
			EventLayerController.Instance.SetEventLayer (curBoxType);
			TranslucentUIMaskManager.Instance.Show (curBoxType);
		}

		switch(boxType)
		{
		case UISceneModuleType.GamePlayerUI:
			hGamePlayerUIControllor.Show();
			break;
		case UISceneModuleType.PropertyDisplay:
			hPropertyDisplayControllor.Show();
			break;
		case UISceneModuleType.GamePause:
			hGamePauseControllor.Show();
			break;
		case UISceneModuleType.GameRank:
			hGameRankControllor.Show();
			break;
		case UISceneModuleType.GameReborn:
			hGameRebornControllor.Show();
			break;
		case UISceneModuleType.GiftPackage:
			hGameGiftPackageControllor.Show();
			break;
		case UISceneModuleType.GameEndingScore:
			hGameEndingScoreControllor.Show();
			break;
		case UISceneModuleType.CharacterDetail:
			hCharacterDetailControllor.Show();
			break;
		case UISceneModuleType.OneKeyToFullLevel:
			hOneKeyToFullLevelControllor.Show();
			break;
		case UISceneModuleType.GamePassedGfit:
			hGamePassedGfitControllor.Show();
			break;
		case UISceneModuleType.HintInGame:
			hHintInGameControllor.Show();
			break;
		case UISceneModuleType.GameResume:
			hGameResumeControllor.Show();
			break;
		case UISceneModuleType.Shop:
			hShopControllor.Show();
			break;
		case UISceneModuleType.Strength:
			hStrengthControllor.Show();
			break;
		case UISceneModuleType.ShopSecondSure:
			hShopSecondSureControllor.Show();
			break;
		case UISceneModuleType.UIGuide:
			hUIGuideControllor.Show();
			break;
		case UISceneModuleType.GameSkill:
			hGameSkillControllor.Show();
			break;
		case UISceneModuleType.LevelGift:
			hLevelGiftControllor.Show();
			break;
		case UISceneModuleType.ProtectShield:
			hProtectShieldControllor.Show();
			break;
		}
	}

	public void HideModule(UISceneModuleType boxType)
	{
		switch(boxType)
		{
		case UISceneModuleType.GamePlayerUI:
			curBoxType = hGamePlayerUIControllor.preBoxType;
			break;
		case UISceneModuleType.PropertyDisplay:
			//curBoxType = hCharacterDetailControllor.preBoxType;
			hPropertyDisplayControllor.Hide();
			break;
		case UISceneModuleType.GamePause:
			curBoxType = hGamePauseControllor.preBoxType;
			break;
		case UISceneModuleType.GameRank:
			curBoxType = hGameRankControllor.preBoxType;
			break;
		case UISceneModuleType.GameReborn:
			curBoxType = hGameRebornControllor.preBoxType;
			break;
		case UISceneModuleType.GiftPackage:
			curBoxType = hGameGiftPackageControllor.preBoxType;
			break;
		case UISceneModuleType.GameEndingScore:
			curBoxType = hGameEndingScoreControllor.preBoxType;
			break;
		case UISceneModuleType.CharacterDetail:
			curBoxType = hCharacterDetailControllor.preBoxType;
			break;
		case UISceneModuleType.OneKeyToFullLevel:
			curBoxType = hOneKeyToFullLevelControllor.preBoxType;
			break;
		case UISceneModuleType.GamePassedGfit:
			curBoxType = hGamePassedGfitControllor.preBoxType;
			break;
		case UISceneModuleType.HintInGame:
			curBoxType = hHintInGameControllor.preBoxType;
			break;
		case UISceneModuleType.GameResume:
			curBoxType = hGameResumeControllor.preBoxType;
			break;
		case UISceneModuleType.Shop:
			curBoxType = hShopControllor.preBoxType;
			break;
		case UISceneModuleType.Strength:
			curBoxType = hStrengthControllor.preBoxType;
			break;
		case UISceneModuleType.ShopSecondSure:
			curBoxType = hShopSecondSureControllor.preBoxType;
			break;
		case UISceneModuleType.UIGuide:
			curBoxType = hUIGuideControllor.preBoxType;
			break;
		case UISceneModuleType.GameSkill:
			curBoxType = hGameSkillControllor.preBoxType;
			break;
		case UISceneModuleType.LevelGift:
			curBoxType = hLevelGiftControllor.preBoxType;
			break;
		case UISceneModuleType.ProtectShield:
			curBoxType = hProtectShieldControllor.preBoxType;
			break;
		}
		EventLayerController.Instance.SetEventLayer (curBoxType);
		TranslucentUIMaskManager.Instance.Show (curBoxType);
	}
	
	/// <summary>
	/// Android手机返回键的点击事件.
	/// </summary>
	private void AndroidBackOnClick()
	{
		switch(curBoxType)
		{
		case UISceneModuleType.GamePlayerUI:
			hGamePlayerUIControllor.Back();
			break;
		case UISceneModuleType.GamePause:
			hGamePauseControllor.Back();
			break;
		case UISceneModuleType.GameRank:
			hGameRankControllor.Back();
			break;
		case UISceneModuleType.GameReborn:
			hGameRebornControllor.Back();
			break;
		case UISceneModuleType.GiftPackage:
			hGameGiftPackageControllor.Back();
			break;
		case UISceneModuleType.GameEndingScore:
			hGameEndingScoreControllor.Back();
			break;
		case UISceneModuleType.OneKeyToFullLevel:
			hOneKeyToFullLevelControllor.Back();
			break;
		case UISceneModuleType.GamePassedGfit:
			hGamePassedGfitControllor.Back();
			break;
		case UISceneModuleType.HintInGame:
			hHintInGameControllor.Back();
			break;
		case UISceneModuleType.GameResume:
			hGameResumeControllor.Back();
			break;
		case UISceneModuleType.Shop:
			hShopControllor.Back();
			break;
		case UISceneModuleType.Strength:
			hStrengthControllor.Back();
			break;
		case UISceneModuleType.ShopSecondSure:
			hShopSecondSureControllor.Back();
			break;
		case UISceneModuleType.CharacterDetail:
			hCharacterDetailControllor.Back();
			break;
		case UISceneModuleType.UIGuide:
			hUIGuideControllor.Back();
			break;
		case UISceneModuleType.GameSkill:
			hGameSkillControllor.Back();
			break;
		case UISceneModuleType.LevelGift:
			hLevelGiftControllor.Back();
			break;
		case UISceneModuleType.ProtectShield:
			hProtectShieldControllor.Back();
			break;
		}
	}

	void OnEnable()
	{
		PublicSceneObject.Instance.ClearAndroidBackKeyEvent ();
		PublicSceneObject.Instance.AndroidBackKeyEvent += AndroidBackOnClick;	
	}
}
