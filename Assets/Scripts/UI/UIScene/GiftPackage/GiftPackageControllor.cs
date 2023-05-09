using UnityEngine;
using System.Collections;
using PayBuild;

public class GiftPackageControllor : UIBoxBase {

	public static GiftPackageControllor Instance;
	
	public delegate void GiftBuyEventHandler();
	event GiftBuyEventHandler GiftBuyEvent;
	
	public GameObject goCloseButton, goBuyButton, goCancelBtn;
	public EasyFontTextMesh textTitleText, textHint1, textHint2, textDesc1, textDesc2, textPayBt;
	//public EasyFontTextMesh textAdsDesc1, textAdsDesc2;
	public tk2dSprite imageCloseBt;
	
	public tk2dSprite iconSprite;
	public tk2dTextMesh countText;
	
	private PayType payJsonType;

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();

		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			goBuyButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false); //审核版不显示按钮特效，更换特效时要改名称
		}
	}
	
	public override void Hide ()
	{
		base.Hide();
		if(GlobalConst.SceneName == SceneType.GameScene)
		{
			GameUIManager.Instance.HideModule(UISceneModuleType.GiftPackage);
		}
		else
		{
			UIManager.Instance.HideModule(UISceneModuleType.GiftPackage);
		}

		if(GlobalConst.SceneName == SceneType.GameScene)
		{
			if(GameUIManager.Instance.curBoxType == UISceneModuleType.GamePlayerUI || GameUIManager.Instance.curBoxType == UISceneModuleType.GameSkill)
			{
				GameController.Instance.ResumeGame();
			}

		}
	}

	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	#endregion

	#region 数据处理

	/// <summary>
	/// 基础礼包外部调用接口
	/// </summary>
	/// <param name="itemType">Item type.</param>
	public void Show(PayType payType, GiftBuyEventHandler e = null)
	{
		switch(payType)
		{
		case PayType.CoinGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Coin, GiftEventType.Show);
			break;
		case PayType.JewelGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Jewel, GiftEventType.Show);
			break;
		case PayType.PowerGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Strengh, GiftEventType.Show);
			break;
		}

		payJsonType = payType;
		GiftBuyEvent = e;
		
		if(GlobalConst.SceneName == SceneType.UIScene)
			UIManager.Instance.ShowModule(UISceneModuleType.GiftPackage);
		else
			GameUIManager.Instance.ShowModule(UISceneModuleType.GiftPackage);

		textTitleText.text = PayJsonData.Instance.GetGiftTitle(payJsonType);

		int shopIndex = ShopData.Instance.GetIndex (payJsonType);
		countText.text = "X" + ShopData.Instance.GetCount(shopIndex);
		iconSprite.SetSprite(ShopData.Instance.GetGiftIcon(shopIndex));

		PayUIConfigurator.PayUIConfig(payJsonType, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goBuyButton.GetComponent<BoxCollider>(), SetCancelBt);
	}

	void SetCancelBt(bool state)
	{
		if(state)
		{
			goCancelBtn.SetActive(true);
			goCancelBtn.transform.localPosition = new Vector3(-50, -122, 0);
			goBuyButton.transform.localPosition = new Vector3(128, -122, 0);
		}else
		{
			goCancelBtn.SetActive(false);
			goBuyButton.transform.localPosition = new Vector3(100, -122, 0);
		}
	}
	#endregion

	#region 按钮控制
	public void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	public void CancelOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	public void BuyOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		GetGiftPayCode();
		
		//自定义事件.
		switch(payJsonType)
		{
		case PayType.CoinGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Coin, GiftEventType.Click);
			break;
		case PayType.JewelGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Jewel, GiftEventType.Click);
			break;
		case PayType.PowerGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Strengh, GiftEventType.Click);
			break;
		}
	}
	#endregion

	#region 扣费代码
	void GetGiftPayCode()
	{
		if(payJsonType == PayType.FreeInnerGameGift)
		{
			GetGiftPayCallBack("Success");
			return;
		}

		PayBuildPayManage.Instance.Pay ((int)payJsonType, GetGiftPayCallBack);
	}
	
	void GetGiftPayCallBack(string result)
	{
		if (result.CompareTo ("Success") != 0)
			return;

		PlayerData.ItemType[] itemType = PayJsonData.Instance.GetGiftItemsTypeArr(payJsonType);
		int[] count = PayJsonData.Instance.GetGiftItemsCountsArr(payJsonType);

		for(int i = 0; i < itemType.Length; i ++)
		{
			PlayerData.Instance.AddItemNum(itemType[i], count[i]);
		}

		//自定义事件.
		switch(payJsonType)
		{
		case PayType.CoinGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Coin, GiftEventType.Pay);
			break;
		case PayType.JewelGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Jewel, GiftEventType.Pay);
			break;
		case PayType.PowerGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Strengh, GiftEventType.Pay);
			break;
		}

		Hide();

		if(GiftBuyEvent != null)
		{
			GiftBuyEvent();
			GiftBuyEvent = null;
		}
	}
	#endregion
}
