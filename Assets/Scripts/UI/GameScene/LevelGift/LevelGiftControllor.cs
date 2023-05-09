using UnityEngine;
using System.Collections;
using DG.Tweening;
using PayBuild;

public class LevelGiftControllor : UIBoxBase {

	public GameObject goShenheBG;
	public static LevelGiftControllor Instance;
	
	public delegate void GiftBuyEventHandler();
	event GiftBuyEventHandler GiftBuyEvent;
	
	public GameObject goBuyButton, goCancelBtn, goCloseButton;
	public EasyFontTextMesh textHint1, textHint2, textPayBt, titleText, textDesc1, textDesc2; 
	public tk2dSprite imageCloseBt;
	
	public Transform  tranCountDown, tranCount;
	public tk2dSprite[] iconSpriteArr;
	public tk2dTextMesh[] countTextArr;
	
	private PayType payJsonType;
	
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();
	}
	
	public void Show(PayType payItemType, GiftBuyEventHandler e = null, bool showCountDown = false)
	{
		payJsonType = payItemType;
		GiftBuyEvent = e;
		
		GameUIManager.Instance.ShowModule(UISceneModuleType.LevelGift);


		PayUIConfigurator.PayUIConfig(payJsonType, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goBuyButton.GetComponent<BoxCollider>(), SetCancelBt);
		
		AudioManger.Instance.PlaySound(AudioManger.SoundName.AotemanKuizeng);
		//自定义事件.
		//CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Light_YiZe_Show, "选择关卡", "Level_" + PlayerData.Instance.GetSelectedLevel());
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Light,"State","弹出次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());

		titleText.text = PayJsonData.Instance.GetGiftTitle(payJsonType);
		string[] IconArr  = PayJsonData.Instance.GetGiftItemsIconArr(payJsonType);
		int[]    CountArr = PayJsonData.Instance.GetGiftItemsCountsArr(payJsonType);
		
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
		
		if(showCountDown)
		{
			PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = false;
			goCloseButton.SetActive(false);
			tranCountDown.gameObject.SetActive(true);
			CountDown();
		}
		else
		{
			goCloseButton.SetActive(true);
			tranCountDown.gameObject.SetActive(false);
		}

		if (payJsonType == PayType.FreeInnerGameGift) {
			goCloseButton.SetActive(false);
		}

		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			// goShenheBG.SetActive (true); // 关闭背景切换
			goBuyButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false); //审核版不显示按钮特效，更换特效时要改名称
		} else {
			// goShenheBG.SetActive(false); // 关闭背景切换
		}
	}
	
	#region 倒计时
	int iCount;
	void CountDown()
	{
		goCloseButton.SetActive(false);
		tranCountDown.gameObject.SetActive(true);
		iCount = 3;
		StartCoroutine("CountDownIE");
	}
	
	IEnumerator CountDownIE()
	{
		tranCount.GetComponent<EasyFontTextMesh>().text = iCount.ToString();
		tranCount.localScale = new Vector3(1.2f, 1.2f, 1.2f);
		DOTween.Kill("CountDown");
		tranCount.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.8f).SetEase(Ease.OutBack).SetId("CountDown");
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CountDown);
		yield return new WaitForSeconds(1.5f);
		iCount --;
		if(iCount <= 0)
		{
			CountDownIECall();
		}
		else
		{
			StartCoroutine("CountDownIE");
		}
	}
	
	void CountDownIECall()
	{
		if (payJsonType != PayType.FreeInnerGameGift) {
			PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = true;
			goCloseButton.SetActive (true);
		}
		tranCountDown.gameObject.SetActive(false);
	}
	#endregion
	
	void SetCancelBt(bool state)
	{
		if(state)
		{
			goCancelBtn.SetActive(true);
			goCancelBtn.transform.localPosition = new Vector3(-90, -136, 0);
			goBuyButton.transform.localPosition = new Vector3(114, -136, 0);
		}else
		{
			goCancelBtn.SetActive(false);
			goBuyButton.transform.localPosition = new Vector3(60, -124, 0);
		}
	}
	
	public override void Hide ()
	{
		GameController.Instance.ResumeGame();
		base.Hide();
		GameUIManager.Instance.HideModule (UISceneModuleType.LevelGift);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
		//自定义事件.
		//CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Light_YiZe_Close, "选择关卡", "Level_" + PlayerData.Instance.GetSelectedLevel());
	}
	
	public void CancelOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = true;
		StopCoroutine("CountDownIE");
		CountDownIECall();
		Hide();
		//自定义事件.
		//CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Light_YiZe_Close, "选择关卡", "Level_" + PlayerData.Instance.GetSelectedLevel());
	}
	
	public void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = true;
		StopCoroutine("CountDownIE");
		CountDownIECall();
		Hide();
		//自定义事件.
		//CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Light_YiZe_Close, "选择关卡", "Level_" + PlayerData.Instance.GetSelectedLevel());
	}
	
	public void BuyOnClick()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.ButtonClick);
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = true;
		StopCoroutine("CountDownIE");
		CountDownIECall();
		
		if(payJsonType == PayType.FreeInnerGameGift)
		{
			GetGiftPayCallBack("Success");
			return;
		}
		
		PayBuildPayManage.Instance.Pay ((int)payJsonType, GetGiftPayCallBack);
		//自定义事件.
		//CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Light_YiZe_Buy, "选择关卡", "Level_" + PlayerData.Instance.GetSelectedLevel());
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Light,"State","确认次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());
	}
	
	void GetGiftPayCallBack(string result)
	{
		if(result.CompareTo("Success") != 0)
			return;
		
		PlayerData.Instance.SetYizeGiftGetedState (PlayerData.Instance.GetSelectedLevel (), true);
		if (payJsonType != PayType.FreeInnerGameGift) {
			//自定义事件.
			//CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Purchase_Light_YiZe, "选择关卡", "Level_" + PlayerData.Instance.GetSelectedLevel ());
			CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Gift_Light, "State", "付费次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());
		}
		
		PlayerData.ItemType[] itemType = PayJsonData.Instance.GetGiftItemsTypeArr(payJsonType);
		int[] count = PayJsonData.Instance.GetGiftItemsCountsArr(payJsonType);
		
		for(int i = 0; i < itemType.Length; i ++)
		{
			PlayerData.Instance.AddItemNum(itemType[i], count[i]);
		}
		
		Hide();
		
		if(GiftBuyEvent != null)
		{
			GiftBuyEvent();
			GiftBuyEvent = null;
		}
	}
}
