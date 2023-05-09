using UnityEngine;
using System.Collections;
using PayBuild;

public class GameRebornControllor : UIBoxBase {
	public static GameRebornControllor Instance;

	public GameObject goCloseBtn, goUpgradeBtn, goRebornBtn, goCancelBtn;
	public GameObject goUpgradeDesc, goManjiDesc, goTips;
	public EasyFontTextMesh textHint1, textHint2, textDesc1, textDesc2, textPayBt, textTipsNum;
	public tk2dSprite imageCloseBt, imageCostType;

	private bool bIsFreeReborn;
	private bool isGuideReborn;

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;

		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();
		if (PlayerData.Instance.GetUIGuideState (UIGuideType.GameRebornGuide)) {
			UIGuideControllor.Instance.Show (UIGuideType.GameRebornGuide);
			isGuideReborn = true;

			Transform FingerGuideTran;
			FingerGuideTran = goRebornBtn.transform.Find("FingerGuide");
			if(FingerGuideTran != null)
				FingerGuideTran.gameObject.SetActive(false);
		} else {
			isGuideReborn = false;
		}

		transform.localPosition = ShowPosV2;
		//AudioManger.Instance.PlaySound(AudioManger.SoundName.AotemanDeath);


		if(PlayerData.Instance.GetFreeRebornTimes() >= PayJsonData.Instance.GetFreeRebornTimes() 
		   && (PlayerData.Instance.GetSelectedLevel() > 2 || PlayerData.Instance.GetGameMode().CompareTo("WuJin") == 0)
		   && isGuideReborn == false)
		{
			bIsFreeReborn = false;
			PayUIConfigurator.PayUIConfig(PayType.Reborn, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goRebornBtn.GetComponent<BoxCollider>(), SetCancelBt);
		}else
		{
			bIsFreeReborn = true;
			PayUIConfigurator.PayUIConfig(PayType.FreeReborn, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt,goRebornBtn.GetComponent<BoxCollider>(),  SetCancelBt);
		}

		if(PayJsonData.Instance.GetUsePropsState(PayType.Reborn) && bIsFreeReborn == false)
		{
			imageCostType.gameObject.SetActive(true);
			imageCostType.SetSprite(PayJsonData.Instance.GetPropsCostIcon(PayType.Reborn));
			textPayBt.transform.localPosition = new Vector3(9, textPayBt.transform.localPosition.y, textPayBt.transform.localPosition.z);
		}else
		{
			imageCostType.gameObject.SetActive(false);
			textPayBt.transform.localPosition = new Vector3(0, textPayBt.transform.localPosition.y, textPayBt.transform.localPosition.z);
		}


		RefreshTipsNum();

		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Reborn, GiftEventType.Activate);
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Reborn, GiftEventType.Show);

	}

	void SetCancelBt(bool state)
	{
		if(state)
		{
			goCancelBtn.SetActive(true);
			goCancelBtn.transform.localPosition = new Vector3(-95, -165, 0f);
			goRebornBtn.transform.localPosition = new Vector3(72f, -165, 0f);
		}else
		{
			goCancelBtn.SetActive(false);
			goRebornBtn.transform.localPosition = new Vector3(0f, -164.4f, 0f);
		}
	}

	public override void Hide ()
	{
		gameObject.SetActive (false);
		transform.localPosition = GlobalConst.LeftHidePos;
		AudioManger.Instance.StopSound(AudioManger.SoundName.AotemanDeath);
		GameUIManager.Instance.HideModule(UISceneModuleType.GameReborn);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
		GameUIManager.Instance.ShowModule(UISceneModuleType.GameEndingScore);
	}
	#endregion

	#region  数据处理
	public void RefreshTipsNum()
	{
		int iCoinCount = PlayerData.Instance.GetItemNum(PlayerData.ItemType.Coin);
		int iModelId = PlayerData.Instance.GetSelectedModel();
		int iModelMaxId = iModelId / 100 * 100 + ModelData.Instance.GetMaxLevel(iModelId);
		int iTipsNum = 0;

		goUpgradeBtn.SetActive(iModelId != iModelMaxId);
		goUpgradeDesc.SetActive(iModelId != iModelMaxId);
		goManjiDesc.SetActive(iModelId == iModelMaxId);

		for(int i = iModelId; i < iModelMaxId; i ++)
		{
			int cost = ModelData.Instance.GetUpgradeCost(i);

			if(iCoinCount < cost)
				break;

			iTipsNum ++;
			iCoinCount -= cost;
		}
		goTips.SetActive(iTipsNum != 0);
		textTipsNum.text = iTipsNum.ToString();
	}
	#endregion
	
	#region 按钮控制
	void CloseBtnOnClick(){
		
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
		GameUIManager.Instance.ShowModule(UISceneModuleType.GameEndingScore);
		return;
	}
	void UpgradeBtnOnClick(){
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		CharacterDetailControllor.Instance.SetModelIndex(GameData.Instance.selectedModelType - 1);
		GameUIManager.Instance.ShowModule(UISceneModuleType.CharacterDetail);
		return;
	}
	
	public void RebornBtnOnClick(){
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Reborn();
		return;
	}
	#endregion

	#region  扣费处理
	void Reborn()
	{
		if(bIsFreeReborn)
		{
			RebornCall("Success");
			return;
		}

		if(PayJsonData.Instance.GetUsePropsState(PayType.Reborn))
		{
			int count = PayJsonData.Instance.GetPropsCostCount(PayType.Reborn);
			PlayerData.ItemType type = PayJsonData.Instance.GetPropsCostType(PayType.Reborn);

			if(PlayerData.Instance.GetItemNum(type) >= count)
			{
				PlayerData.Instance.ReduceItemNum(type, count);
				RebornCall("Success");
			}else
			{
				PayType payType= (type == PlayerData.ItemType.Coin)?PayType.CoinGift : PayType.JewelGift;
				GiftPackageControllor.Instance.Show(payType, Reborn);

				if(payType == PayType.CoinGift)
				{
					CollectInfoEvent.SendGiftEvent(CollectInfoEvent.EventType.Gift_Coin, GiftEventType.Auto);
				}else{
					CollectInfoEvent.SendGiftEvent(CollectInfoEvent.EventType.Gift_Jewel, GiftEventType.Auto);
				}
			}
		}else
		{
		    PayBuildPayManage.Instance.Pay(PayData.Instance.GetPayCode(PayType.Reborn), RebornCall);
		}

		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Reborn, GiftEventType.Click);
	}


	void RebornCall(string result)
	{
		if(result.CompareTo("Success") == 0)
		{
			Hide();
			//第二关之前无限免费复活, 教程不计入复活次数
			if(bIsFreeReborn  && !isGuideReborn && (PlayerData.Instance.GetSelectedLevel() > 2 || PlayerData.Instance.GetGameMode().CompareTo("WuJin") == 0))
				PlayerData.Instance.AddFreeRebornTimes();
			GameController.Instance.Reborn ();
			//自定义事件.
			if(bIsFreeReborn == false)
			{
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Reborn, GiftEventType.Pay);
			}

		}
	}
	#endregion
}
