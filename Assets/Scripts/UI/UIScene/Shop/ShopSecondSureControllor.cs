using UnityEngine;
using System.Collections;
using DG.Tweening;
using PayBuild;

public class ShopSecondSureControllor : UIBoxBase {


	public GameObject goShenheBG;
	public static ShopSecondSureControllor Instance;

	public GameObject goBuyButton, goCloseButton, goCancelBtn;
	public EasyFontTextMesh textTitleText;
	public EasyFontTextMesh textHint, textAdWords, textPayBt, textDesc1, textDesc2, textGiftCount;
	public tk2dSprite imageCloseBt, imageGiftIcon, ImageGiftType;

	private PlayerData.ItemType itemType;
	private int iPayId, iCount;
	private string sTitle;

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;

		base.Init();
	}
	
	public override void Show ()
	{
		base.Show ();
		/*自定义事件*/
		PayType payType = PayData.Instance.GetPayType (iPayId);
		switch (payType) {
		case PayType.Jewel20Gift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_BuyJewel, GiftEventType.Show);
			break;
		case PayType.JewelGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Jewel, GiftEventType.Show);
			break;
		case PayType.PowerGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Strengh, GiftEventType.Show);
			break;
		case PayType.CoinGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Coin, GiftEventType.Show);
			break;
		case PayType.MultiCoin:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_DoubleCoin, GiftEventType.Show);
			break;
		}

		
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			// goShenheBG.SetActive (true); // 关闭背景切换
			goBuyButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false); //审核版不显示按钮特效，更换特效时要改名称
		} else {
			// goShenheBG.SetActive(false); // 关闭背景切换
		}

	}
	
	public override void Hide ()
	{
		base.Hide();
		if(GlobalConst.SceneName == SceneType.UIScene)
			UIManager.Instance.HideModule (UISceneModuleType.ShopSecondSure);
		else
			GameUIManager.Instance.HideModule (UISceneModuleType.ShopSecondSure);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	#endregion

	public void InitData(int index)
	{
		itemType = ShopData.Instance.GetItemType (index);
		iPayId = ShopData.Instance.GetPayId(index);

		sTitle = ShopData.Instance.GetItemName(index);

		textTitleText.text = sTitle;


		imageGiftIcon.SetSprite(ShopData.Instance.GetGiftIcon(index));

		if(itemType != PlayerData.ItemType.DoubleCoin)
		{
			iCount = ShopData.Instance.GetCount(index);
			ImageGiftType.SetSprite(ShopData.Instance.GetIconName(index));
		    textGiftCount.text = iCount.ToString();
			textGiftCount.transform.localPosition = new Vector3(70, -111, 0);
		}else
		{
			textGiftCount.transform.localPosition = new Vector3(70, -111, 0);
			textGiftCount.text = "Permanent";
		}
		//ImageGiftType.gameObject.SetActive(itemType != PlayerData.ItemType.DoubleCoin);

		PayUIConfigurator.PayUIConfig(ShopData.Instance.GetPayJsonType(index), textHint, textAdWords, textDesc1, textDesc2, textPayBt, imageCloseBt,goBuyButton.GetComponent<BoxCollider>(),SetCancelBt);
	   

	}

	void SetCancelBt(bool state)
	{
		if(PlatformSetting.Instance.PlatformType == PlatformItemType.DianXin)
		{
			string hint = textHint.text, pay = textPayBt.text;
			textHint.text = hint.Replace("30", "20");
			textPayBt.text = pay.Replace("30", "20");
		}

		if(state)
		{
			goCancelBtn.SetActive(true);
			//goCancelBtn.transform.localPosition = new Vector3(-125, -185, 0);
			//goBuyButton.transform.localPosition = new Vector3(90, -185, 0);
		}else
		{
			goCancelBtn.SetActive(false);
			//goBuyButton.transform.localPosition = new Vector3(0, -185, 0);
		}
	}

	public void CloseButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
		return;
	}
	
	public void BuyButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		PayBuildPayManage.Instance.Pay(iPayId, PayCallBack);
		
		/*自定义事件*/
		PayType payType = PayData.Instance.GetPayType (iPayId);
		switch (payType) {
		case PayType.Jewel20Gift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_BuyJewel, GiftEventType.Click);
			break;
		case PayType.JewelGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Jewel, GiftEventType.Click);
			break;
		case PayType.PowerGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Strengh, GiftEventType.Click);
			break;
		case PayType.CoinGift:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Coin, GiftEventType.Click);
			break;
		case PayType.MultiCoin:
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_DoubleCoin, GiftEventType.Click);
			break;
		}
		
		return;
	}

	private void PayCallBack(string result)
	{
		if (result.CompareTo ("Success") == 0) {

			Hide();

			if(itemType == PlayerData.ItemType.DoubleCoin)
				PlayerData.Instance.SetForeverDoubleCoin();
			else
				PlayerData.Instance.AddItemNum (itemType, iCount);

			/*自定义事件*/
			PayType payType = PayData.Instance.GetPayType (iPayId);
			switch (payType) {
			case PayType.Jewel20Gift:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_BuyJewel, GiftEventType.Pay);
				break;
			case PayType.JewelGift:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Jewel, GiftEventType.Pay);
				break;
			case PayType.PowerGift:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Strengh, GiftEventType.Pay);
				break;
			case PayType.CoinGift:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Coin, GiftEventType.Pay);
				break;
			case PayType.MultiCoin:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_DoubleCoin, GiftEventType.Pay);
				break;
			}
		}
	}
}
