using UnityEngine;
using System.Collections;
using PayBuild ;

public class GamePassedGfitControllor : UIBoxBase {

	public GameObject goShenheBG;
	public static GamePassedGfitControllor Instance;

	public GameObject goCloseBtn, goGetBtn, goCancelBtn;
	public GameObject goGiftBox, goGiftItems;
	public Transform[] tranGifts;
	public tk2dSprite[] imageGiftIcons;
	public EasyFontTextMesh[] textGiftCounts;

	public EasyFontTextMesh textTitleText;
	public EasyFontTextMesh textHint1, textHint2, textPayBt;
	public EasyFontTextMesh textDesc1, textDesc2;
	public tk2dSprite imageCloseBt;
	public tk2dSprite imageGiftBoxIcon;

	public ParticleSystem getParticle;

	bool IsShowingItems = false;

	enum GiftType
	{
		OneStar,
		TwoStar,
		ThreeStar,
		Gift
	}
	private GiftType curGiftType;
	private bool bHasShowedATypeBag = false, hasATypeBag = false;
	private bool bHasShowedBTypeBag = false, hasBTypeBag = false;
	private bool bHasShowedNormalBag = false, hasNormalBag = false;
	private PayType passGameGiftJsonTpye;

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();

		goGetBtn.GetComponent<BoxCollider>().enabled = true;
		IsShowingItems = false;

		if (PlatformSetting.Instance.IsBTypeBag && (iCurLevel == 1 || iCurLevel == 3)) {
			hasBTypeBag = true;
			bHasShowedBTypeBag = false;
		}	
		else {
			hasBTypeBag = false;
			bHasShowedBTypeBag = true;
		}

		if (PlatformSetting.Instance.IsATypeBag && iCurLevel == 1) {
			hasATypeBag = true;
			bHasShowedATypeBag = false;
		}
		else {
			hasATypeBag = false;
			bHasShowedATypeBag = true;
		}

		if (LevelData.Instance.GetPassGameGiftState (iCurLevel)) {
			hasNormalBag = true;
			bHasShowedNormalBag = false;
		}
		else {
			hasNormalBag = false;
			bHasShowedNormalBag = true;
		}

		SetStarGiftBox();

		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			//goShenheBG.SetActive (true);
		} else {
			//goShenheBG.SetActive(false);
		}
	}
	
	public override void Hide ()
	{
		base.Hide();
		if(GlobalConst.SceneName == SceneType.GameScene )
		{
			GameEndingScoreControllor.Instance.SetButtonEnable (true);
			GameUIManager.Instance.HideModule(UISceneModuleType.GamePassedGfit);
		}else
		{
			UIManager.Instance.HideModule(UISceneModuleType.GamePassedGfit);
		}

		if (PlayerData.Instance.GetUIGuideState (UIGuideType.GameEndingScoreNextGuide) && iCurLevel == 1)
			UIGuideControllor.Instance.Show (UIGuideType.GameEndingScoreNextGuide);
		else if (PlayerData.Instance.GetUIGuideState (UIGuideType.GameEndingScoreCloseGuide) && iCurLevel == 2)
			UIGuideControllor.Instance.Show (UIGuideType.GameEndingScoreCloseGuide);
		else if (PlayerData.Instance.GetUIGuideState (UIGuideType.GameEndingScoreCloseGuide) && iCurLevel == 3)
			UIGuideControllor.Instance.Show (UIGuideType.GameEndingScoreCloseGuide);
	}

	public override void Back ()
	{
	}
	#endregion

	#region 数据处理
	private int iOldStarCount, iStarCount, iCurStarCount;
	private int iCurLevel;
	private float fGiftPosMinX = -140, fGiftPosMaxX = 140, fGiftPosY = 0;
	public void SaveData()
	{
		iCurLevel = PlayerData.Instance.GetSelectedLevel();
		iOldStarCount = PlayerData.Instance.GetLevelStarState(iCurLevel);
		iStarCount = GameData.Instance.StarCount;

		if(iStarCount > iOldStarCount)
		{
			PlayerData.Instance.SetLevelStarState(iCurLevel, iStarCount);
			//三星奖励写入数据
			for(int i = iOldStarCount + 1; i <= iStarCount; i ++)
			{
				string sAwards = LevelData.Instance.GetLevelAwardStarStr(i, iCurLevel);
				string[] sArrAwards = sAwards.Split('|');
				for(int j = 0; j < sArrAwards.Length; j ++)
				{
					string[] sArrInfo = sArrAwards[j].Split('=');
					PlayerData.ItemType itemType = (PlayerData.ItemType)System.Enum.Parse(typeof(PlayerData.ItemType), sArrInfo[1]);
					int count = int.Parse(sArrInfo[3]);
					
					PlayerData.Instance.AddItemNum(itemType, count);
				}
			}
		}

		iCurStarCount = iOldStarCount + 1;
	}

	void SetBoxTitle(string title)
	{
		textTitleText.text = title;
	}

	void SetStarGiftBox()
	{
		if(GlobalConst.SceneName == SceneType.UIScene || iStarCount < iCurStarCount){
			SetPassedGiftBox();
			return;
		}

		goGiftBox.SetActive(true);
		goGiftItems.SetActive(false);
		goCloseBtn.SetActive(false);

		string sAwards = LevelData.Instance.GetLevelAwardStarStr(iCurStarCount, iCurLevel);
		string[] sArrAwards = sAwards.Split('|');
		string sGiftText = "";
		for(int i = 0; i < tranGifts.Length; i ++)
		{
			if(i >= sArrAwards.Length)
			{
				tranGifts[i].gameObject.SetActive(false);
				continue;
			}
			
			string[] sArrInfo = sArrAwards[i].Split('=');
			string sName = sArrInfo[0];
			string sItemType = sArrInfo[1];
			string sIcon = sArrInfo[2];
			string sCount = sArrInfo[3];

		    sGiftText += sCount + sName + ((i==sArrAwards.Length-1) ? "" : "、");
		}

	switch(iCurStarCount)
	{
	case 1:
		curGiftType = GiftType.OneStar;
		SetBoxTitle("1 Star reward");
		textHint2.text = "" + sGiftText;
		textHint1.text = "Win with one star to receive, limit once";
		imageGiftBoxIcon.SetSprite("giftbox_star1");
		break;
	case 2:
		curGiftType = GiftType.TwoStar;
		SetBoxTitle("2 Star reward");
		textHint2.text = "" + sGiftText;
		textHint1.text = "Win with two star to receive, limit once";
		imageGiftBoxIcon.SetSprite("giftbox_star2");
		break;
	case 3:
		curGiftType = GiftType.ThreeStar;
		SetBoxTitle("3 Star reward");
		textHint2.text = "" + sGiftText;
		textHint1.text = "Win with three star to receive, limit once";
		imageGiftBoxIcon.SetSprite("giftbox_star3");
		break;
	}

		SetCancelBt(false);
	}

	void SetStarGiftItems()
	{
		string sAwards = LevelData.Instance.GetLevelAwardStarStr(iCurStarCount, iCurLevel);
		string[] sArrAwards = sAwards.Split('|');
		string sGiftText = "";
		for(int i = 0; i < tranGifts.Length; i ++)
		{
			if(i >= sArrAwards.Length)
			{
				tranGifts[i].gameObject.SetActive(false);
				continue;
			}
			
			string[] sArrInfo = sArrAwards[i].Split('=');
			string sName = sArrInfo[0];
			string sItemType = sArrInfo[1];
			string sIcon = sArrInfo[2];
			string sCount = sArrInfo[3];
			
			tranGifts[i].gameObject.SetActive(true);
			imageGiftIcons[i].SetSprite (sIcon);
			
			//图标大小修正
			imageGiftIcons[i].scale = Vector3.one;
			float iconScale = 60f / Mathf.Max(imageGiftIcons[i].GetBounds().size.x, imageGiftIcons[i].GetBounds().size.y);
			//			Debug.Log(imageGiftIcons[i].transform.parent.name + ": " + imageGiftIcons[i].GetBounds().size.x + " " + imageGiftIcons[i].GetBounds().size.y + " " + iconScale);
			imageGiftIcons[i].scale = new Vector3(iconScale, iconScale, iconScale);
			
			textGiftCounts[i].text = "+" + sCount;
			tranGifts[i].localPosition = new Vector3(fGiftPosMinX + (fGiftPosMaxX - fGiftPosMinX) / (sArrAwards.Length * 2) * (2 * i + 1), fGiftPosY, 0);
		}
	}

	void SetPassedGiftBox()
	{
		if(bHasShowedATypeBag && bHasShowedBTypeBag && bHasShowedNormalBag)
		{
			Hide();
			return;
		}

		goGiftBox.SetActive(true);
		goGiftItems.SetActive(false);
		goCloseBtn.SetActive(true);

		curGiftType = GiftType.Gift;

		if(hasBTypeBag && !bHasShowedBTypeBag)
		{
			bHasShowedBTypeBag = true;
			//显示B包内容
			passGameGiftJsonTpye = PayType.RewardsGift5Stars;

			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_5Start, GiftEventType.Show);
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_5Start, GiftEventType.Auto);

		}else if(hasATypeBag && !bHasShowedATypeBag)
		{
			bHasShowedATypeBag = true;
			//显示A包内容
			passGameGiftJsonTpye = PayType.RewardsGift4Stars;

			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_4Star, GiftEventType.Show);
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_4Star, GiftEventType.Auto);
		}else if(hasNormalBag)
		{
			bHasShowedNormalBag = true;
			//显示正常通关奖励礼包内容
			passGameGiftJsonTpye = PayType.RewardsGift3Stars;

			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_3Start, GiftEventType.Show);
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_3Start, GiftEventType.Auto);
		}

		SetBoxTitle(PayJsonData.Instance.GetGiftTitle(passGameGiftJsonTpye));
		imageGiftBoxIcon.SetSprite("giftbox_pay");
		goGetBtn.GetComponent<BoxCollider>().enabled = true;

		PayUIConfigurator.PayUIConfig(passGameGiftJsonTpye, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goGetBtn.GetComponent<BoxCollider>(), SetCancelBt);
		textDesc1.gameObject.SetActive(false);
		textDesc2.gameObject.SetActive(false);
		
	}

	void SetPassedGiftItems()
	{
		string sGiftText = "";
		string[] sArrName;
		string[] sArrIcon;
		int[]    iArrCount;
		sArrName = new string[PayJsonData.Instance.GetGiftItemsNameArr(passGameGiftJsonTpye).Length];
		sArrName = PayJsonData.Instance.GetGiftItemsNameArr(passGameGiftJsonTpye);
		sArrIcon = new string[PayJsonData.Instance.GetGiftItemsIconArr(passGameGiftJsonTpye).Length];
		sArrIcon = PayJsonData.Instance.GetGiftItemsIconArr(passGameGiftJsonTpye);
		iArrCount = new int[PayJsonData.Instance.GetGiftItemsCountsArr(passGameGiftJsonTpye).Length];
		iArrCount = PayJsonData.Instance.GetGiftItemsCountsArr(passGameGiftJsonTpye);

		for(int i = 0; i < tranGifts.Length; i ++)
		{
			if(i >= sArrName.Length)
			{
				tranGifts[i].gameObject.SetActive(false);
				continue;
			}
			
			tranGifts[i].gameObject.SetActive(true);
			imageGiftIcons[i].SetSprite(sArrIcon[i]);
			//图标大小修正
			imageGiftIcons[i].scale = Vector3.one;
			float iconScale = 86f / Mathf.Max(imageGiftIcons[i].GetBounds().size.x, imageGiftIcons[i].GetBounds().size.y);
			imageGiftIcons[i].scale = new Vector3(iconScale, iconScale, iconScale);
			
			textGiftCounts[i].text = "+" + iArrCount[i].ToString();
			tranGifts[i].localPosition = new Vector3(fGiftPosMinX + (fGiftPosMaxX - fGiftPosMinX) / (sArrName.Length * 2) * (2 * i + 1), fGiftPosY, 0);
			
			sGiftText += iArrCount[i] + sArrName[i] + ((i==sArrName.Length-1) ? "" : "、");
		}
	}

	void SetCancelBt(bool state)
	{
		//提示颜色统一用通关奖励的
		textHint1.FontColorTop = PayJsonData.Instance.GetHintText1Color(PayType.RewardsGift3Stars);
		textHint1.FontColorBottom = PayJsonData.Instance.GetHintText1Color(PayType.RewardsGift3Stars);
		textHint2.FontColorTop = PayJsonData.Instance.GetHintText2Color(PayType.RewardsGift3Stars);
		textHint2.FontColorBottom = PayJsonData.Instance.GetHintText2Color(PayType.RewardsGift3Stars);

		if(state)
		{
			goCancelBtn.SetActive(true);
			goCancelBtn.transform.localPosition = new Vector3(-85, -115, 0);
			goGetBtn.transform.localPosition = new Vector3(72, -115, 0);
		}else
		{
			goCancelBtn.SetActive(false);
			goGetBtn.transform.localPosition = new Vector3(0, -115, 0);
		}
	}

	#endregion

	#region 显示奖励
	IEnumerator ShowAwardItemIE()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CoinCollision);
		getParticle.Stop();
		getParticle.Play ();
		IsShowingItems = true;

		if(iCurStarCount <= iStarCount && GlobalConst.SceneName == SceneType.GameScene)
		    SetStarGiftItems();
		else
			SetPassedGiftItems();

		goGiftBox.SetActive(false);
		goGiftItems.SetActive(true);

		yield return new WaitForSeconds(1.2f);

		IsShowingItems = false;

		getParticle.Stop ();

		if(curGiftType == GiftType.Gift && bHasShowedATypeBag && bHasShowedBTypeBag && bHasShowedNormalBag)
		{
			Hide();
		}else
		{
			iCurStarCount ++;
			if(iCurStarCount <= iStarCount){
				SetStarGiftBox();
			}else{
				SetPassedGiftBox();
			}
		}
	}

	#endregion

	#region 扣费代码 

	void BuyGamePassedGiftCall(string result)
	{
		goGetBtn.GetComponent<BoxCollider>().enabled = true;
		if(result.CompareTo("Success") == 0)
		{
			PlayerData.Instance.SetPassGameGiftGetedState(PlayerData.Instance.GetSelectedLevel(), true);

			PlayerData.ItemType[] type = PayJsonData.Instance.GetGiftItemsTypeArr(passGameGiftJsonTpye);
			int[] count = PayJsonData.Instance.GetGiftItemsCountsArr(passGameGiftJsonTpye);

			for(int i = 0; i < type.Length; i ++)
				PlayerData.Instance.AddItemNum(type[i], count[i]);

			StartCoroutine("ShowAwardItemIE");

			//自定义事件.
			if(passGameGiftJsonTpye ==PayType.RewardsGift3Stars)
			{
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_3Start, GiftEventType.Pay);
				
			}else if(passGameGiftJsonTpye == PayType.RewardsGift4Stars)
			{
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_4Star, GiftEventType.Pay);
			}else if(passGameGiftJsonTpye == PayType.RewardsGift5Stars)
			{
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_5Start, GiftEventType.Pay);
			}
		}
	}

	#endregion

	#region 按钮控制
	void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);

		if(hasBTypeBag && !bHasShowedATypeBag)
		{
			//显示B包未购买，则显示A包
			SetPassedGiftBox();
			return;
		}else if(hasATypeBag && !bHasShowedNormalBag)
		{
			//显示A包未购买，则显示正常礼包
			SetPassedGiftBox();
			return;
		}

		Hide();
	}
	
	void GetOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
	
		if (curGiftType == GiftType.Gift) {
			goGetBtn.GetComponent<BoxCollider> ().enabled = false;

			PayBuildPayManage.Instance.Pay ((int)passGameGiftJsonTpye, BuyGamePassedGiftCall);
			//自定义事件.
			
			if (passGameGiftJsonTpye == PayType.RewardsGift3Stars) {
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_3Start, GiftEventType.Click);
				
			} else if (passGameGiftJsonTpye == PayType.RewardsGift4Stars) {
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_4Star, GiftEventType.Click);
			} else if (passGameGiftJsonTpye == PayType.RewardsGift5Stars) {
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_5Start, GiftEventType.Click);
			}
		} else {
			if (IsShowingItems) {
				iCurStarCount++;
				if (iCurStarCount <= iStarCount) {
					SetStarGiftBox ();
					StopCoroutine ("ShowAwardItemIE");
					StartCoroutine ("ShowAwardItemIE");
				} else {
					StopCoroutine ("ShowAwardItemIE");
					SetPassedGiftBox ();
				}
			} else {
				StopCoroutine ("ShowAwardItemIE");
				StartCoroutine ("ShowAwardItemIE");
			}
		}
	}
	#endregion
}
