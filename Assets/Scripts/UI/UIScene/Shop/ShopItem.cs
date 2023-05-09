using UnityEngine;
using System.Collections;
using PayBuild;

public class ShopItem : MonoBehaviour {

	public GameObject goBuyButton;
	public tk2dSprite IconImage;
	public tk2dSprite TypeImage;
	public EasyFontTextMesh CountText;
	public EasyFontTextMesh BtnText;
	public EasyFontTextMesh TitleText;

	private int count = 0;
	private int payId;
	[HideInInspector]public PlayerData.ItemType itemType;
	private int iIndex;

	public void Init(int index)
	{
		iIndex = index;
		SetIconImage (ShopData.Instance.GetGiftIcon (index));
		SetTitleText (ShopData.Instance.GetItemName (index));
		itemType = ShopData.Instance.GetItemType (index);
		payId = ShopData.Instance.GetPayId (index);

		if(itemType != PlayerData.ItemType.DoubleCoin)
		{
		    SetTypeImage (ShopData.Instance.GetIconName (index));
		    SetCountText (ShopData.Instance.GetCount (index));
			CountText.transform.localPosition = new Vector3(18, 0, 0);
			SetBtnText (ShopData.Instance.GetCost(index) + "usd");
		}else
		{
			CountText.transform.localPosition = new Vector3(0, 0, 0);
			CountText.text = "Permanent";
			if(PlayerData.Instance.GetForeverDoubleCoin() == 1)
			{
				goBuyButton.GetComponent<BoxCollider>().enabled = false;
				SetBtnText ("Activated");
			}
			else
			{
				SetBtnText (ShopData.Instance.GetCost(index) + "usd");
			}
		}
		TypeImage.gameObject.SetActive(itemType != PlayerData.ItemType.DoubleCoin);

	}

	public void SetIconImage(string iconName)
	{
		IconImage.SetSprite (iconName);
	}

	public void SetTypeImage(string typeName)
	{
		TypeImage.SetSprite (typeName);
	}

	public void SetCountText(int count)
	{
		this.count = count;
		CountText.text = count.ToString ();
	}

	public void SetBtnText(string btnName)
	{
		BtnText.text = btnName;
	}

	public void SetTitleText(string titleName)
	{
		TitleText.text = titleName;
	}

	public void GetButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		if(PayJsonData.Instance.GetIsActivedState(PayType.Shop))
		{
			ShopSecondSureControllor.Instance.InitData(iIndex);

			if(GlobalConst.SceneName == SceneType.UIScene)
				UIManager.Instance.ShowModule(UISceneModuleType.ShopSecondSure);
			else
				GameUIManager.Instance.ShowModule(UISceneModuleType.ShopSecondSure);



			/*自定义事件*/
			PayType payType = PayData.Instance.GetPayType (payId);
			switch (payType) {
			case PayType.Jewel20Gift:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_BuyJewel, GiftEventType.Activate);
				break;
			case PayType.JewelGift:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Jewel, GiftEventType.Activate);
				break;
			case PayType.PowerGift:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Strengh, GiftEventType.Activate);
				break;
			case PayType.CoinGift:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_Coin, GiftEventType.Activate);
				break;
			case PayType.MultiCoin:
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_DoubleCoin, GiftEventType.Activate);
				break;
			}



		}else
		{
		    PayBuildPayManage.Instance.Pay(payId, PayCallBack);



			/*自定义事件*/
			PayType payType = PayData.Instance.GetPayType (payId);
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


		}
	}

	private void PayCallBack(string result)
	{
		if (result.CompareTo ("Success") == 0) {
			
			if (itemType == PlayerData.ItemType.DoubleCoin)
				PlayerData.Instance.SetForeverDoubleCoin ();
			else
				PlayerData.Instance.AddItemNum (itemType, count);

			/*自定义事件*/
			PayType payType = PayData.Instance.GetPayType (payId);
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
