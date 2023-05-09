using UnityEngine;
using System.Collections;
using DG.Tweening;
using PayBuild;

public class DoubleCoinControllor : UIBoxBase {

	public GameObject goBuyButton, goCancelBtn;
	public EasyFontTextMesh textHint1, textHint2, textDesc1, textDesc2, textPayBt;
	public tk2dSprite imageCloseBt;

	public override void Init ()
	{
		base.Init();
	}

	public override void Show ()
	{
		base.Show ();

		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			// goShenheBG.SetActive (true); // 关闭背景切换
			goBuyButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false); //审核版不显示按钮特效，更换特效时要改名称
		} else {
			// goShenheBG.SetActive(false); // 关闭背景切换
		}

		PayUIConfigurator.PayUIConfig(PayType.MultiCoin, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goBuyButton.GetComponent<BoxCollider>(), SetCancelBt);

		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_DoubleCoin, GiftEventType.Show);
	}
	
	void SetCancelBt(bool state)
	{
		if(state)
		{
			goCancelBtn.SetActive(true);
			goCancelBtn.transform.localPosition = new Vector3(-50, -124, 0);
			goBuyButton.transform.localPosition = new Vector3(128, -124, 0);
		}else
		{
			goCancelBtn.SetActive(false);
			goBuyButton.transform.localPosition = new Vector3(100, -124, 0);
		}
	}

	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.DoubleCoin);
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
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		PayBuildPayManage.Instance.Pay(PayData.Instance.GetPayCode(PayType.MultiCoin), BuyDoubleCoinCall);
		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_DoubleCoin, GiftEventType.Click);
	}

	void BuyDoubleCoinCall(string result)
	{
		if(result.CompareTo("Success") == 0)
		{
			Hide();
			PlayerData.Instance.SetForeverDoubleCoin();
			//自定义事件.
			CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_DoubleCoin, GiftEventType.Pay);
		}
	}
}
