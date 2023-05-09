using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum UIGuideType
{
	LevelInfoGuide=1,						//有存储数据  第一关关卡选择
	MainInterfaceUpgradeGuide,          //
	MainInterfaceStartGameGuide,        //第一关开始游戏
	CharaterDetailGuide,				//有存储数据 升级角色
	GameRebornGuide,
	GameEndingScoreNextGuide,			//有存储数据
	GameEndingScoreCloseGuide,			//有存储数据
	GamePlayerUILeftGuide,
	GamePlayerUIRightGuide,
	GamePlayerUIUseCurPropGuide,
	GamePlayerUIUseFlyBmobGuide,
	GamePlayerUIUseSpeedupGuide,
	GamePlayerUIShowTextGuide,
	LeftRoleGuide,						//有存储数据

};
public enum DirectionType
{
	LeftUp,
	RightUp,
	LeftDown,
	RightDown
};
public class UIGuideControllor : UIBoxBase {
	public static UIGuideControllor Instance;
	public GameObject goLevelInfoGuideBtn, goMainInterfaceUpgradeGuideBtn, goMainInterfaceStartGameGuideBtn, goCharaterDetailGuideBtn, goGameRebornGuideBtn, goGameEndingScoreNextGuideBtn, goGameEndingScoreCloseGuideBtn, goGamePlayerUILeftGuideBtn, goGamePlayerUIRightGuideBtn, goLeftRoleGuideBtn,goUseCurPropBtn,goUseFlyBmobBtn,goUseSpeedupBtn;
	public GameObject LevelInfoGuideGO, MainInterfaceUpgradeGuideGO, MainInterfaceStartGameGuideGO, CharaterDetailGuideGO, GameRebornGuideGO, GameEndingScoreNextGuideGO, GameEndingScoreCloseGuideGO, GamePlayerUILeftGuideGO, GamePlayerUIRightGuideGO, LeftRoleGuideGO, HandGO,UseCurPropGO,UseFlyBmobGO,UseSpeedupGO;

	public tk2dSprite UseCurPropSprite;

	public Transform BubbleTran;
	public EasyFontTextMesh TipText;
	public EasyFontTextMesh RebornText;
	public EasyFontTextMesh MainInterfaceUpgradeCountText, CharaterDetailUpgradeCountText ;
	public tk2dTextMesh  useFlyBombCountText,useSpeedUpCountText;

	private Vector3 LeftUpHandPos = new Vector3(-40, 35, 0);
	private Vector3 RightUpHandPos = new Vector3(40, 35, 0);
	private Vector3 LeftDownHandPos = new Vector3(-50, -10, 0);
	private Vector3 RightDownHandPos = new Vector3(50, -10, 0);

	private Vector3 LeftUpHandRotation = new Vector3 (0, 0, 270);
	private Vector3 RightUpHandRotation = new Vector3 (0, 0, 90);
	private Vector3 LeftDownHandRotation = new Vector3 (0, 0, 330);
	private Vector3 RightDownHandRotation = new Vector3 (0, 0, 30);

	private Vector3 LeftUpMovePos = new Vector3 (-60, 55, 0);
	private Vector3 RightUpMovePos = new Vector3 (60, 55, 0);
	private Vector3 LeftDownMovePos = new Vector3 (-70, -30, 0);
	private Vector3 RightDownMovePos = new Vector3 (70, -30, 0);

//	[HideInInspector][System.NonSerialized]public string newRoleTip1 = "试试满级超人强\n的实力吧";
//	[HideInInspector][System.NonSerialized]public string newRoleTip2 = "超人强实力\n很强大哦";
//	[HideInInspector][System.NonSerialized]public string upgradeRoleTip1 = "升级加强角色能力";
//	[HideInInspector][System.NonSerialized]public string upgradeRoleTip2 = "升到2级有特技哦";
//	[HideInInspector][System.NonSerialized]public string upgradeRoleTip3 = "点击Use props吧！";

	private Transform CoinTypeTran1, JewelTypeTran1, CoinTypeTran2, JewelTypeTran2;
	private UIGuideType guideType;

	public override void Init()
	{
		Instance = this;
		transform.localPosition = ShowPosV2;

		CoinTypeTran1 = goMainInterfaceUpgradeGuideBtn.transform.Find ("CoinType");
		JewelTypeTran1 = goMainInterfaceUpgradeGuideBtn.transform.Find ("JewelType");
		CoinTypeTran2 = goCharaterDetailGuideBtn.transform.Find ("CoinType");
		JewelTypeTran2 = goCharaterDetailGuideBtn.transform.Find ("JewelType");
		if (ModelData.Instance.GetUpgradeCostType (102).CompareTo ("Coin") == 0) {
			CoinTypeTran1.gameObject.SetActive (true);
			JewelTypeTran1.gameObject.SetActive (false);
			CoinTypeTran2.gameObject.SetActive (true);
			JewelTypeTran2.gameObject.SetActive (false);
			CoinTypeTran1.Find("CostCount").GetComponent<EasyFontTextMesh>().text = ModelData.Instance.GetUpgradeCost(102).ToString();
			CoinTypeTran1.Find("BtnText").GetComponent<EasyFontTextMesh>().text = "refit";
			CoinTypeTran2.Find("CostCount").GetComponent<EasyFontTextMesh>().text = ModelData.Instance.GetUpgradeCost(102).ToString();
			CoinTypeTran2.Find("BtnText").GetComponent<EasyFontTextMesh>().text = "refit";
		} else {
			CoinTypeTran1.gameObject.SetActive (false);
			JewelTypeTran1.gameObject.SetActive (true);
			CoinTypeTran2.gameObject.SetActive (false);
			JewelTypeTran2.gameObject.SetActive (true);
			JewelTypeTran1.Find("CostCount").GetComponent<EasyFontTextMesh>().text = ModelData.Instance.GetUpgradeCost(102).ToString();
			JewelTypeTran1.Find("BtnText").GetComponent<EasyFontTextMesh>().text = "refit";
			JewelTypeTran2.Find("CostCount").GetComponent<EasyFontTextMesh>().text = ModelData.Instance.GetUpgradeCost(102).ToString();
			JewelTypeTran2.Find("BtnText").GetComponent<EasyFontTextMesh>().text = "refit";
		}

		if (PayJsonData.Instance.GetNeedShowCancelBt (PayType.FreeReborn)) {
			goGameRebornGuideBtn.transform.localPosition = new Vector3 (72, -165, 0);
		} else {
			goGameRebornGuideBtn.transform.localPosition = new Vector3 (0, -165, 0);
		}

		RebornText.text = PayJsonData.Instance.GetButtonText (PayType.FreeReborn);

		base.Init();
	}

	public void SetUpgradeCount()
	{
		int count = ToolController.GetTipCounnt (101);
		if (count > 0) {
			MainInterfaceUpgradeCountText.transform.parent.gameObject.SetActive (true);
			MainInterfaceUpgradeCountText.text = count.ToString ();
			CharaterDetailUpgradeCountText.transform.parent.gameObject.SetActive (true);
			CharaterDetailUpgradeCountText.text = count.ToString ();
		} else {
			MainInterfaceUpgradeCountText.transform.parent.gameObject.SetActive (false);
			CharaterDetailUpgradeCountText.transform.parent.gameObject.SetActive (false);
		}
	}


	public void ShowBubbleTipByID(int id)
	{
		string tipStr = GuideTextData.Instance.GetText(id);
		Vector3 showPos = GuideTextData.Instance.GetPosition(id);
		float xScale = GuideTextData.Instance.GetXScale(id);
		ShowBubbleTip(tipStr,showPos,xScale);
	}

	public void ShowBubbleTip(string tipStr, Vector3 showPos, float XScale = 1)
	{
		BubbleTran.gameObject.SetActive (true);
		TipText.text = tipStr;
		BubbleTran.localPosition = showPos;
		TipText.transform.localScale = new Vector3(XScale,1,1);
		BubbleTran.localScale = new Vector3(XScale,1,1);
	}

	private Transform iconTran;
	private Vector3 iconPos;
	public void ShowAnim(UIGuideType guideType)
	{
		if(GlobalConst.SceneName == SceneType.UIScene)
			UIManager.Instance.ShowModule (UISceneModuleType.UIGuide);
		else
			GameUIManager.Instance.ShowModule (UISceneModuleType.UIGuide);
		HandGO.SetActive (false);

		switch (guideType) {
		case UIGuideType.GamePlayerUILeftGuide:
			GamePlayerUILeftGuideGO.SetActive (true);
			iconTran = goGamePlayerUILeftGuideBtn.transform;
			break;
		case UIGuideType.GamePlayerUIRightGuide:
			GamePlayerUIRightGuideGO.SetActive (true);
			iconTran = goGamePlayerUIRightGuideBtn.transform;
			break;
		}

		StartCoroutine (MoveAnim(guideType));
	}

	private void ShowHandAnim(DirectionType directionType, Transform targetTran)
	{
		HandGO.SetActive (true);

		//Debug.Log ("ShowHandAnim : " + targetTran.gameObject.name + " ---> " + targetTran.localPosition);
		
		Sequence myQuence = DOTween.Sequence().SetLoops(int.MaxValue, LoopType.Yoyo).SetId("HandQuence");

		switch (directionType) {
		case DirectionType.LeftUp:
			HandGO.transform.localPosition = targetTran.localPosition + LeftUpHandPos;
			HandGO.transform.eulerAngles = LeftUpHandRotation;
			HandGO.transform.localScale = new Vector3(-1, 1, 1);
			myQuence.Append(HandGO.transform.DOLocalMove(targetTran.localPosition + LeftUpMovePos, 0.5f));
			break;
		case DirectionType.RightUp:
			HandGO.transform.localPosition = targetTran.localPosition + RightUpHandPos;
			HandGO.transform.eulerAngles = RightUpHandRotation;
			HandGO.transform.localScale = Vector3.one;
			myQuence.Append(HandGO.transform.DOLocalMove(targetTran.localPosition + RightUpMovePos, 0.5f));
			break;
		case DirectionType.LeftDown:
			HandGO.transform.localPosition = targetTran.localPosition + LeftDownHandPos;
			HandGO.transform.eulerAngles = LeftDownHandRotation;
			HandGO.transform.localScale = new Vector3(-1, 1, 1);
			myQuence.Append(HandGO.transform.DOLocalMove(targetTran.localPosition + LeftDownMovePos, 0.5f));
			break;
		case DirectionType.RightDown:
			HandGO.transform.localPosition = targetTran.localPosition + RightDownHandPos;
			HandGO.transform.eulerAngles = RightDownHandRotation;
			HandGO.transform.localScale = Vector3.one;
			myQuence.Append(HandGO.transform.DOLocalMove(targetTran.localPosition + RightDownMovePos, 0.5f));
			break;
		}

		myQuence.Play();
	}

	IEnumerator MoveAnim(UIGuideType guideType)
	{
		iconPos = iconTran.localPosition;
		iconTran.localPosition = Vector3.zero;
		iconTran.GetComponent<BoxCollider> ().enabled = false;
		BubbleTran.gameObject.SetActive (false);
		yield return new WaitForSeconds (0.5f);
		iconTran.DOLocalMove (iconPos, 0.5f).SetEase (Ease.Linear);
		yield return new WaitForSeconds (0.5f);
		iconTran.GetComponent<BoxCollider> ().enabled = true;
		BubbleTran.gameObject.SetActive (true);
		
		switch (guideType) {
		case UIGuideType.GamePlayerUILeftGuide:
			//GamePlayerUIControllor.Instance.goLeftButton.SetActive(true);
			ShowHandAnim(DirectionType.RightDown, iconTran);
			break;
		case UIGuideType.GamePlayerUIRightGuide:
			//GamePlayerUIControllor.Instance.goRightButton.SetActive(true);
			ShowHandAnim(DirectionType.LeftDown, iconTran);
			break;
		}
	}

	public override void Back ()
	{

	}

	public override void Hide ()
	{
		if(GlobalConst.SceneName == SceneType.UIScene)
			UIManager.Instance.HideModule (UISceneModuleType.UIGuide);
		else
			GameUIManager.Instance.HideModule (UISceneModuleType.UIGuide);
	}

	public override void Show()
	{
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = false;
		gameObject.SetActive (true);
	}

	public void Show(UIGuideType guideType)
	{
		if(GlobalConst.SceneName == SceneType.UIScene)
			UIManager.Instance.ShowModule (UISceneModuleType.UIGuide);
		else
			GameUIManager.Instance.ShowModule (UISceneModuleType.UIGuide);

		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Guide, "UIGuide", guideType.ToString () +" max_level"+PlayerData.Instance.GetCurrentChallengeLevel());

		switch (guideType) {
		case UIGuideType.LevelInfoGuide:
			LevelInfoGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.RightUp, goLevelInfoGuideBtn.transform);
			break;
		case UIGuideType.MainInterfaceUpgradeGuide:
			SetUpgradeCount ();
			MainInterfaceUpgradeGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.LeftUp, goMainInterfaceUpgradeGuideBtn.transform);
			break;
		case UIGuideType.MainInterfaceStartGameGuide:
			MainInterfaceStartGameGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.RightUp, goMainInterfaceStartGameGuideBtn.transform);
			break;
		case UIGuideType.CharaterDetailGuide:
			SetUpgradeCount ();
			CharaterDetailGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.RightDown, goCharaterDetailGuideBtn.transform);
			break;
		case UIGuideType.GameRebornGuide:
			GameRebornGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.RightDown, goGameRebornGuideBtn.transform);
			break;
		case UIGuideType.GameEndingScoreNextGuide:
			GameEndingScoreNextGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.RightUp, goGameEndingScoreNextGuideBtn.transform);
			break;
		case UIGuideType.GameEndingScoreCloseGuide:
			GameEndingScoreCloseGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.LeftDown, goGameEndingScoreCloseGuideBtn.transform);
			break;
		case UIGuideType.GamePlayerUILeftGuide:
			GamePlayerUILeftGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.RightDown, goGamePlayerUILeftGuideBtn.transform);
			break;
		case UIGuideType.GamePlayerUIRightGuide:
			GamePlayerUIRightGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.LeftDown, goGamePlayerUIRightGuideBtn.transform);
			break;
		case UIGuideType.GamePlayerUIUseCurPropGuide:
			UseCurPropGO.SetActive(true);
			ShowHandAnim(DirectionType.LeftDown,goUseCurPropBtn.transform);
			int  spId = GamePlayerUIControllor.Instance.propIconSprite.spriteId;
			UseCurPropSprite.SetSprite(spId);
			break;
		case UIGuideType.GamePlayerUIUseFlyBmobGuide:
			UseFlyBmobGO.SetActive(true);
			useFlyBombCountText.text="X"+PlayerData.Instance.GetItemNum(PlayerData.ItemType.FlyBomb);
			ShowHandAnim(DirectionType.RightUp,goUseFlyBmobBtn.transform);
			break;
		case UIGuideType.GamePlayerUIUseSpeedupGuide:
			UseSpeedupGO.SetActive(true);
			useSpeedUpCountText.text ="X"+ PlayerData.Instance.GetItemNum(PlayerData.ItemType.SpeedUp);
			ShowHandAnim(DirectionType.RightUp,goUseSpeedupBtn.transform);
			break;
		case UIGuideType.GamePlayerUIShowTextGuide:
			HandGO.SetActive (false);
			break;
		case UIGuideType.LeftRoleGuide:
			LeftRoleGuideGO.SetActive (true);
			ShowHandAnim(DirectionType.RightDown, goLeftRoleGuideBtn.transform);
			break;
		}
	}

	public IEnumerator HideInvoke(float delayTime, UIGuideType type)
	{
		yield return new WaitForSeconds (delayTime);

		Hide (type);
	}

	public void Hide(UIGuideType guideType)
	{
		Hide ();
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = true;
		gameObject.SetActive (false);
		BubbleTran.gameObject.SetActive (false);

		DOTween.Kill ("HandQuence");

		switch (guideType) {
		case UIGuideType.LevelInfoGuide:
			LevelInfoGuideGO.SetActive (false);
			break;
		case UIGuideType.MainInterfaceUpgradeGuide:
			MainInterfaceUpgradeGuideGO.SetActive (false);
			break;
		case UIGuideType.MainInterfaceStartGameGuide:
			MainInterfaceStartGameGuideGO.SetActive (false);
			break;
		case UIGuideType.CharaterDetailGuide:
			CharaterDetailGuideGO.SetActive (false);
			break;
		case UIGuideType.GameRebornGuide:
			GameRebornGuideGO.SetActive (false);
			break;
		case UIGuideType.GameEndingScoreNextGuide:
			GameEndingScoreNextGuideGO.SetActive (false);
			break;
		case UIGuideType.GameEndingScoreCloseGuide:
			GameEndingScoreCloseGuideGO.SetActive (false);
			break;
		case UIGuideType.GamePlayerUILeftGuide:
			GamePlayerUILeftGuideGO.SetActive (false);
			GameController.Instance.ResumeGame();
			break;
		case UIGuideType.GamePlayerUIRightGuide:
			GamePlayerUIRightGuideGO.SetActive (false);
			GameController.Instance.ResumeGame();
			break;
		case UIGuideType.GamePlayerUIUseCurPropGuide:
			UseCurPropGO.SetActive(false);
			GameController.Instance.ResumeGame();
			break;
		case UIGuideType.GamePlayerUIUseFlyBmobGuide:
			UseFlyBmobGO.SetActive(false);
			GameController.Instance.ResumeGame();
			GamePlayerUIControllor.Instance.useFlyBombBtnGO.SetActive(true);
			break;
		case UIGuideType.GamePlayerUIUseSpeedupGuide:
			UseSpeedupGO.SetActive(false);
			GameController.Instance.ResumeGame();
			GamePlayerUIControllor.Instance.useSpeedupBtnGO.SetActive(true);
			GamePlayerUIControllor.Instance.useShieldBtnGO.SetActive(true);
			break;
		case UIGuideType.GamePlayerUIShowTextGuide:
			GameController.Instance.ResumeGame();
			break;
		case UIGuideType.LeftRoleGuide:
			LeftRoleGuideGO.SetActive (false);
			break;
		}
	}

	void LevelInfoGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.LevelInfoGuide);
		if(PlayerData.Instance.GetCurrentChallengeLevel () == 4)
		{
			PlayerData.Instance.SetUIGuideToFalse (UIGuideType.LevelInfoGuide);
			PlayerData.Instance.SetUIGuideToFalse (UIGuideType.LeftRoleGuide);
			PlayerData.Instance.SetIsUIGuideEndToTrue ();
		}
		LevelInfoControllor.Instance.StartOnClick();
	}
	
	void MainInterfaceUpgradeGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.MainInterfaceUpgradeGuide);
		MainInterfaceControllor.Instance.ZaohuanOnClick();
	}
	
	void MainInterfaceStartGameGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.MainInterfaceStartGameGuide);
		MainInterfaceControllor.Instance.LevelGameOnClick();
		if (GlobalConst.LeftRoleGuideEnabled) {
			//下一步教程
			LevelInfoControllor.Instance.InitData (PlayerData.Instance.GetCurrentChallengeLevel());
			UIManager.Instance.ShowModule (UISceneModuleType.LevelInfo);
		}
	}
	
	void CharaterDetailGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.CharaterDetailGuide);
		PlayerData.Instance.SetUIGuideToFalse (UIGuideType.CharaterDetailGuide);
		CharacterDetailControllor.Instance.ZaohuanOnClick ();
	}
	
	void GameRebornGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.GameRebornGuide);
		PlayerData.Instance.SetUIGuideToFalse (UIGuideType.GameRebornGuide);
		GameRebornControllor.Instance.RebornBtnOnClick ();
	}
	
	void GameEndingScoreNextGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.GameEndingScoreNextGuide);
		PlayerData.Instance.SetUIGuideToFalse (UIGuideType.GameEndingScoreNextGuide);
		GameEndingScoreControllor.Instance.NextLevelOnClick ();
	}
	
	void GameEndingScoreCloseGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.GameEndingScoreCloseGuide);
		if(PlayerData.Instance.GetCurrentChallengeLevel () == 4)
			PlayerData.Instance.SetUIGuideToFalse (UIGuideType.GameEndingScoreCloseGuide);
		GameEndingScoreControllor.Instance.CloseOnClick ();
	}
	
	void LeftRoleGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.LeftRoleGuide);
		MainInterfaceControllor.Instance.LeftOnClick();
		//下一步教程
		UIGuideControllor.Instance.Show (UIGuideType.MainInterfaceStartGameGuide);
		UIGuideControllor.Instance.ShowBubbleTipByID(3);
		TranslucentUIMaskManager.Instance.SetLayer (11);
		ActorCameraController1.Instance.SetCameraDepth (4);
	}
	
	void GamePlayerUILeftGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.GamePlayerUILeftGuide);
		GamePlayerUIControllor.Instance.LeftDown();
		GamePlayerUIControllor.Instance.leftBtnGO.SetActive(true);
		Invoke("LeftRecov",0.5f);
	}

	void LeftRecov()
	{
		GamePlayerUIControllor.Instance.LeftUp();
	}
	
	void GamePlayerUIRightGuideBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide (UIGuideType.GamePlayerUIRightGuide);
		GamePlayerUIControllor.Instance.RightDown();
		GamePlayerUIControllor.Instance.rightBtnGO.SetActive(true);

		Invoke("RightRecov",0.6f);
	}


	void RightRecov()
	{
		GamePlayerUIControllor.Instance.RightUp();
	}

	void GamePlayerUIUseCurPropGuideBtnOnClick()
	{
		GamePlayerUIControllor.Instance.UsePropOnClick();
		Hide(UIGuideType.GamePlayerUIUseCurPropGuide);
		GamePlayerUIControllor.Instance.pauseBtnGO.SetActive(true);
		if(PlatformSetting.Instance.PayVersionType != PayVersionItemType.GuangDian)
			GamePlayerUIControllor.Instance.giftBagGO.SetActive(true);
		GamePlayerUIControllor.Instance.pickPropBtnGO.SetActive (true);
	}

	void GamePlayerUIUseFlyBmobGuideBtnOnClick()
	{
		GamePlayerUIControllor.Instance.UseFlyBmobProp();
		Hide(UIGuideType.GamePlayerUIUseFlyBmobGuide);
	}

	void GamePlayerUIUseSpeedupGuideBtnOnClick()
	{
		GamePlayerUIControllor.Instance.UseSpeedUpProp();
		Hide(UIGuideType.GamePlayerUIUseSpeedupGuide);
	}


}