using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public class PropertyDisplayControllor : UIBoxBase {

	public static PropertyDisplayControllor Instance;

	public EasyFontTextMesh StrengthCountText, StrengthRecoverCountTimeText, CoinCountText, JewelCountText;
	[HideInInspector]
	public int strengthCount, coinCount, jewelCount;

	public void ChangeLayer ()
	{
		UISceneModuleType curBoxType = UISceneModuleType.MainInterface;
		if (GlobalConst.SceneName == SceneType.UIScene)
			curBoxType = UIManager.Instance.curBoxType;
		else if (GlobalConst.SceneName == SceneType.GameScene)
			curBoxType = GameUIManager.Instance.curBoxType;

		switch (curBoxType) {
		case UISceneModuleType.MainInterface:
			ChangeLayer (transform, 5);
			break;
		case UISceneModuleType.LevelSelect:
			ChangeLayer(transform, 5);
			break;
		case UISceneModuleType.Activity:
			ChangeLayer(transform, 11);
			break;
		case UISceneModuleType.LevelInfo:
			ChangeLayer(transform, 11);
			break;
		case UISceneModuleType.CharacterDetail:
			ChangeLayer(transform, 12);
			break;
		case UISceneModuleType.Shop:
			ChangeLayer(transform, 13);
			break;
		}
	}

	private void ChangeLayer(Transform parentTran, int layer)
	{
		parentTran.gameObject.layer = layer;
		for (int i = 0; i<parentTran.childCount; ++i) {
			ChangeLayer (parentTran.GetChild(i), layer);
		}
		if(gameObject.activeSelf && SignInControllor.Instance.gameObject.activeInHierarchy)
			StartCoroutine(PublicSceneObject.Instance.AddButtonToSelectList(BoxAllButtonList));
	}

	public override void Init ()
	{
		Instance = this;
		transform.localPosition = Vector3.zero;

		ChangeLayer (transform, 5);

		InitStrengthRecover ();

		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian) {
			if (PlayerData.Instance.GetItemNum (PlayerData.ItemType.Coin) < 10000) {
				PlayerData.Instance.SetItemNum (PlayerData.ItemType.Coin, 9999999);
			}
			if (PlayerData.Instance.GetItemNum (PlayerData.ItemType.Jewel) < 10000) {
				PlayerData.Instance.SetItemNum (PlayerData.ItemType.Jewel, 9999999);
			}
			if (PlayerData.Instance.GetItemNum (PlayerData.ItemType.Strength) < 11) {
				PlayerData.Instance.SetItemNum (PlayerData.ItemType.Strength, 99);
			}
		}

		InitData ();

		base.Init();
	}

	private void InitData()
	{
		ChangeStrength (PlayerData.Instance.GetItemNum(PlayerData.ItemType.Strength));
		CoinCountText.text = PlayerData.Instance.GetItemNum(PlayerData.ItemType.Coin).ToString();
		JewelCountText.text = PlayerData.Instance.GetItemNum(PlayerData.ItemType.Jewel).ToString();
	}
	
	public override void Show ()
	{
		base.Show();
		transform.localPosition = GlobalConst.TopHidePos;
		transform.DOLocalMove (ShowPosV2, GlobalConst.PlayTime).SetEase (Ease.OutCubic);
	}
	
	public override void Hide ()
	{
		base.Hide();
		gameObject.SetActive(false);
	}
	
	public override void Back ()
	{
	}

	void OnEnable()
	{
		PlayerData.Instance.StrengthChangeEvent += ChangeStrength;
		PlayerData.Instance.CoinChangeEvent += ChangeCoin;
		PlayerData.Instance.JewelChangeEvent += ChangeJewel;
	}

	void OnDisable()
	{
		PlayerData.Instance.StrengthChangeEvent -= ChangeStrength;
		PlayerData.Instance.CoinChangeEvent -= ChangeCoin;
		PlayerData.Instance.JewelChangeEvent -= ChangeJewel;
	}

	void ChangeStrength(int strength)
	{
		StrengthCountText.text = strength.ToString ();
		if (strength >= strengthRecoverLimit) {
			StrengthRecoverCountTimeText.gameObject.SetActive (false);
		} else {
			StrengthRecoverCountTimeText.gameObject.SetActive (true);
			CheckStrength (strength);
		}
	}

	void UpdataStrengthRecoverTime(string timeStr)
	{
		StrengthRecoverCountTimeText.text = timeStr;
	}

	void ChangeCoin(int coin)
	{
		if (GlobalConst.SceneName == SceneType.UIScene) {
			MainInterfaceControllor.Instance.SetUpgradeCount ();
			CharacterDetailControllor.Instance.SetUpgradeCount ();
			LevelInfoControllor.Instance.SetUpgradeCount ();
			TurnplateControllor.Instance.SetTurnTimes();
		} else {
			CharacterDetailControllor.Instance.SetUpgradeCount ();
		}
		coinCount = int.Parse(CoinCountText.text);
		DOTween.Kill ("ChangeCoinId");
		DOTween.To (() => this.coinCount, x => this.coinCount = x, coin, 1f).OnUpdate (ChangeCoinAnim).SetId ("ChangeCoinId");
	}
	
	void ChangeCoinAnim()
	{
		CoinCountText.text = coinCount.ToString();
	}
	
	void ChangeJewel(int jewel)
	{
		if (GlobalConst.SceneName == SceneType.UIScene) {
			MainInterfaceControllor.Instance.SetUpgradeCount ();
			CharacterDetailControllor.Instance.SetUpgradeCount ();
			LevelInfoControllor.Instance.SetUpgradeCount ();
			TurnplateControllor.Instance.SetTurnTimes();
		} else {
			CharacterDetailControllor.Instance.SetUpgradeCount ();
		}
		jewelCount = int.Parse(JewelCountText.text);
		DOTween.Kill ("ChangeJewelId");
		DOTween.To (() => this.jewelCount, x => this.jewelCount = x, jewel, 1f).OnUpdate (ChangeJewelAnim).SetId ("ChangeJewelId");
	}
	
	void ChangeJewelAnim()
	{
		JewelCountText.text = jewelCount.ToString();
	}
	
	#region On Button Click

	public void AddStrengthOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);

		if(GlobalConst.SceneName == SceneType.UIScene)
		{
			switch(UIManager.Instance.curBoxType)
			{
			case UISceneModuleType.Shop:
				ShopControllor.Instance.Hide();
				UIManager.Instance.ShowModule(UISceneModuleType.Strength);
				break;
			case UISceneModuleType.Strength:
				break;
			default:
				UIManager.Instance.ShowModule(UISceneModuleType.Strength);
				break;
			}
		}
		else
		{
			switch(GameUIManager.Instance.curBoxType)
			{
			case UISceneModuleType.Shop:
				ShopControllor.Instance.Hide();
				GameUIManager.Instance.ShowModule(UISceneModuleType.Strength);
				break;
			case UISceneModuleType.Strength:
				break;
			default:
				GameUIManager.Instance.ShowModule(UISceneModuleType.Strength);
				break;
			}
		}
	}

	public void AddCoinOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);

		if(GlobalConst.SceneName == SceneType.UIScene)
		{
			switch(UIManager.Instance.curBoxType)
			{
			case UISceneModuleType.Shop:
				break;
			case UISceneModuleType.Strength:
				StrengthControllor.Instance.Hide();
				UIManager.Instance.ShowModule(UISceneModuleType.Shop);
				break;
			default:
				UIManager.Instance.ShowModule(UISceneModuleType.Shop);
				break;
			}
		}
		else
		{
			switch(GameUIManager.Instance.curBoxType)
			{
			case UISceneModuleType.Shop:
				break;
			case UISceneModuleType.Strength:
				StrengthControllor.Instance.Hide();
				GameUIManager.Instance.ShowModule(UISceneModuleType.Shop);
				break;
			default:
				GameUIManager.Instance.ShowModule(UISceneModuleType.Shop);
				break;
			}
		}
	}

	public void AddJewelOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);

		if(GlobalConst.SceneName == SceneType.UIScene)
		{
			switch(UIManager.Instance.curBoxType)
			{
			case UISceneModuleType.Shop:
				break;
			case UISceneModuleType.Strength:
				StrengthControllor.Instance.Hide();
				UIManager.Instance.ShowModule(UISceneModuleType.Shop);
				break;
			default:
				UIManager.Instance.ShowModule(UISceneModuleType.Shop);
				break;
			}
		}
		else
		{
			switch(GameUIManager.Instance.curBoxType)
			{
			case UISceneModuleType.Shop:
				break;
			case UISceneModuleType.Strength:
				StrengthControllor.Instance.Hide();
				GameUIManager.Instance.ShowModule(UISceneModuleType.Shop);
				break;
			default:
				GameUIManager.Instance.ShowModule(UISceneModuleType.Shop);
				break;
			}
		}
	}


	public void ShowPlayerLevelOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);

		if (GlobalConst.SceneName == SceneType.UIScene) {
			UIManager.Instance.ShowModule (UISceneModuleType.PlayerLevel);
		} else {
			GameUIManager.Instance.ShowModule (UISceneModuleType.PlayerLevel);
		}
	}
	#endregion

	#region 体力定时回复功能
	private int recoverTime = 5 * 60; //体力回复的倒计时长，5分钟
	private int addStrengthEachTime = 1; //体力每次回复的值
	private int strengthRecoverLimit = 10; //体力自动回复的上限值
	private int countDownTime = 0;

	void InitStrengthRecover()
	{
		//为了区别一开始做的在线存储300秒的值，先设置回来一个正常ticks值
		if (PlayerData.Instance.GetStrengthRecoverLT () < 500) { 
			PlayerData.Instance.SetStrengthRecoverLT (DateTime.Now.Ticks);
		}

		//计算过去的时间值, 获得倒数秒数
		long crossTime = SystemTicksToSecond (DateTime.Now.Ticks - PlayerData.Instance.GetStrengthRecoverLT ());
        if (crossTime <= 0)
        {
            countDownTime = recoverTime;
            PlayerData.Instance.SetStrengthRecoverLT(DateTime.Now.Ticks);
        }
        else
        {
            if (crossTime > recoverTime)
            {
                long div = crossTime / recoverTime;  //可恢复的体力时间次数
                int mod = (int)(crossTime % recoverTime); //恢复下个体力的剩余时间

                if (PlayerData.Instance.GetItemNum(PlayerData.ItemType.Strength) < strengthRecoverLimit)
                {
                    if (PlayerData.Instance.GetItemNum(PlayerData.ItemType.Strength) + div * addStrengthEachTime > strengthRecoverLimit)
                        PlayerData.Instance.SetItemNum(PlayerData.ItemType.Strength, strengthRecoverLimit);
                    else
                        PlayerData.Instance.AddItemNum(PlayerData.ItemType.Strength, (int)(div * addStrengthEachTime));
                }

                long rightSavedTicks = System.DateTime.Now.Ticks - SystemSecondToTicks(mod);
                PlayerData.Instance.SetStrengthRecoverLT(rightSavedTicks);

                countDownTime = recoverTime - mod;
            }
            else
            {
                countDownTime = recoverTime - (int)crossTime;
            }
        }

		if (PlayerData.Instance.GetItemNum (PlayerData.ItemType.Strength) >= strengthRecoverLimit) {
			CancelInvoke ("StrengthRecoverCountDown");
		} else {
			if (!IsInvoking ("StrengthRecoverCountDown"))
				InvokeRepeating ("StrengthRecoverCountDown", 0, 1);
		}
	}

	void CheckStrength(int strength)
	{
		if (strength >= strengthRecoverLimit) {
			CancelInvoke ("StrengthRecoverCountDown");
			PlayerData.Instance.SetStrengthRecoverLT (DateTime.Now.Ticks);
		} else {
			if (!IsInvoking ("StrengthRecoverCountDown")) {
				countDownTime = recoverTime;
				InvokeRepeating ("StrengthRecoverCountDown", 0, 1);

				if((strength + 1) == strengthRecoverLimit)
					PlayerData.Instance.SetStrengthRecoverLT (DateTime.Now.Ticks);
			}
		}
	}

	/// <summary>
	/// 体力回复的倒计时
	/// </summary>
	/// <returns>The recover I.</returns>
	void StrengthRecoverCountDown()
	{
		countDownTime--;
		UpdataStrengthRecoverTime(SecondsToTimeStr(countDownTime));

		if (countDownTime <= 0) {
			if (PlayerData.Instance.GetItemNum (PlayerData.ItemType.Strength) < strengthRecoverLimit) {
				PlayerData.Instance.AddItemNum (PlayerData.ItemType.Strength, addStrengthEachTime);
			}
			countDownTime = recoverTime;
			PlayerData.Instance.SetStrengthRecoverLT (DateTime.Now.Ticks);
		}
	}


	string SecondsToTimeStr(int sec)
	{
		string sSecond;
		string sMinute;
		int iSecond;
		int iMinute;

		iSecond = sec % 60;
		if (iSecond < 10)
			sSecond = "0" + iSecond;
		else
			sSecond = iSecond + "";

		iMinute = sec / 60;
		sMinute = iMinute + "";

		return sMinute + ":" + sSecond;
	}

	long SystemTicksToSecond(long tick)
	{
		return tick / (10000000);
	}

	long SystemSecondToTicks(long second)
	{
		return second * (10000000);
	}

	/// <summary>
	/// 退到后台回来要更新倒计时
	/// </summary>
	/// <param name="pauseStatus">If set to <c>true</c> pause status.</param>
	void OnApplicationPause(bool pauseStatus)
	{
		if(pauseStatus)
		{
		}
		else
		{
			InitStrengthRecover ();
		}
	}

	#endregion
}
