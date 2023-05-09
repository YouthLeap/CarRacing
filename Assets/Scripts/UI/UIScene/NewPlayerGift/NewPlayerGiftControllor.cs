using UnityEngine;
using System.Collections;
using PayBuild;

public class NewPlayerGiftControllor : UIBoxBase {


	public GameObject goShenheBG;
	public GameObject goBuyButton, goCancelBtn;
	public EasyFontTextMesh titleText, textHint1, textHint2, textDesc1, textDesc2, textPayBt;
	public tk2dSprite imageCloseBt;

	public tk2dSprite[] iconSpriteArr;
	public tk2dTextMesh[] countTextArr;
	
	public override void Init ()
	{
		titleText.text = PayJsonData.Instance.GetGiftTitle(PayType.NewPlayerGift);
		string[] IconArr  = PayJsonData.Instance.GetGiftItemsIconArr(PayType.NewPlayerGift);
		int[]    CountArr = PayJsonData.Instance.GetGiftItemsCountsArr(PayType.NewPlayerGift);
		
		for(int i=0; i<IconArr.Length; ++i)
		{
			if(i < IconArr.Length)
			{
				if(IconArr[i].CompareTo("coin_icon") == 0 || IconArr[i].CompareTo("jewel_icon") == 0)
					IconArr[i] = IconArr[i].Replace("icon", "2");
				iconSpriteArr[i].SetSprite (IconArr[i]);
				countTextArr[i].text = "X" + CountArr[i].ToString();
			}
		}

		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();
		
		PayUIConfigurator.PayUIConfig(PayType.NewPlayerGift, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goBuyButton.GetComponent<BoxCollider>(), SetCancelBt);
		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_NewPlayerGet, GiftEventType.Show);

		
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			// goShenheBG.SetActive (true); // 关闭背景切换
			goBuyButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false); //审核版不显示按钮特效，更换特效时要改名称
		} else {
			// goShenheBG.SetActive(false); // 关闭背景切换
		}
	}
	
	void SetCancelBt(bool state)
	{
		if(state)
		{
			goCancelBtn.SetActive(true);
			goCancelBtn.transform.localPosition = new Vector3(-50, -136, 0);
			goBuyButton.transform.localPosition = new Vector3(128, -136, 0);
		}else
		{
			goCancelBtn.SetActive(false);
			goBuyButton.transform.localPosition = new Vector3(100, -136, 0);
		}
	}
	
	public override void Hide ()
	{
		base.Hide();
		UIManager.Instance.HideModule (UISceneModuleType.NewPlayerGift);

		//弹完 畅玩版 新手礼包 后，显示关卡信息
		if (GlobalConst.StartGameGuideEnabled) {
			UIGuideControllor.Instance.Show (UIGuideType.LevelInfoGuide);
		}
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	
	public void CancelOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	
	public void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	public void BuyOnClick()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.ButtonClick);
		PayBuildPayManage.Instance.Pay ((int)PayType.NewPlayerGift, GetGiftPayCallBack);
		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_NewPlayerGet, GiftEventType.Click);
	}

	void GetGiftPayCallBack(string result)
	{
		if (result.CompareTo ("Success") != 0)
			return;

		PlayerData.Instance.SetHuiKuiMiniGiftState(true);
		LevelSelectControllor.Instance.goNewPlayerGiftButton.SetActive(false);
		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_NewPlayerGet, GiftEventType.Pay);

		PlayerData.ItemType[] itemType = PayJsonData.Instance.GetGiftItemsTypeArr(PayType.NewPlayerGift);
		int[] count = PayJsonData.Instance.GetGiftItemsCountsArr(PayType.NewPlayerGift);
		
		for(int i = 0; i < itemType.Length; i ++)
		{
			PlayerData.Instance.AddItemNum(itemType[i], count[i]);
		}
		
		Hide();
	}
}
