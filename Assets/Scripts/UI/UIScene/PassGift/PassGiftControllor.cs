using UnityEngine;
using System.Collections;
using PayBuild;

public class PassGiftControllor : UIBoxBase {

	public GameObject goShenheBG;
	public GameObject goBuyButton, goCancelBtn;
	public EasyFontTextMesh titleText, textHint1, textHint2, textDesc1, textDesc2, textPayBt;
	public tk2dSprite imageCloseBt;
	
	public tk2dSprite[] iconSpriteArr;
	public tk2dTextMesh[] countTextArr;
	
	public override void Init ()
	{
		titleText.text = PayJsonData.Instance.GetGiftTitle(PayType.RewardsGift5Stars);
		string[] IconArr  = PayJsonData.Instance.GetGiftItemsIconArr(PayType.RewardsGift5Stars);
		int[]    CountArr = PayJsonData.Instance.GetGiftItemsCountsArr(PayType.RewardsGift5Stars);
		
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
		
		PayUIConfigurator.PayUIConfig(PayType.RewardsGift5Stars, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goBuyButton.GetComponent<BoxCollider>(), SetCancelBt);

		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_5Start, GiftEventType.Show);

		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			// goShenheBG.SetActive (true); // 关闭背景切换
			goBuyButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false); //审核版不显示按钮特效，更换特效时要改名称
		} else {
			// goShenheBG.SetActive(false); // 关闭背景切换
		}
	}
	
	void SetCancelBt(bool state)
	{
		//特殊处理字体颜色
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.LiBaoBai) {
			textHint1.FontColorTop = new Color(255/255.0f, 255/255.0f, 1, 1);
			textHint1.FontColorBottom = new Color(255/255.0f, 255/255.0f, 1, 1);
			if(textHint1.EnableShadow)
				textHint1.ShadowColor = new Color(30/255.0f, 105/255.0f, 190/255.0f, 0);
			if(textHint1.EnableOutline)
				textHint1.OutlineColor = new Color(31/255.0f, 125/255.0f, 185/255.0f, 1);
		} else if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.LiBaoHei) {
			textHint1.FontColorTop = new Color(255/255.0f, 172/255.0f, 0/255.0f, 255/255.0f);
			textHint1.FontColorBottom = new Color(255/255.0f, 172/255.0f, 0/255.0f, 255/255.0f);
			if(textHint1.EnableShadow)
				textHint1.ShadowColor = new Color(54/255.0f, 182/255.0f, 201/255.0f, 0);
			if(textHint1.EnableOutline)
				textHint1.OutlineColor = new Color(214/255.0f, 98/255.0f, 30/255.0f, 0);
		}
		textHint2.gameObject.SetActive (false);
		
		if(state)
		{
			goCancelBtn.SetActive(true);
			goCancelBtn.transform.localPosition = new Vector3(-50, -136, 0);
			goBuyButton.transform.localPosition = new Vector3(128, -136, 0);
		}
		else
		{
			goCancelBtn.SetActive(false);
			goBuyButton.transform.localPosition = new Vector3(100, -136, 0);
		}
	}
	
	public override void Hide ()
	{
		base.Hide();
		UIManager.Instance.HideModule (UISceneModuleType.PassGift);
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
		PayBuildPayManage.Instance.Pay((int)PayType.RewardsGift5Stars, BuyPassGiftCall);

		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_5Start, GiftEventType.Click);
	}
	
	void BuyPassGiftCall(string result)
	{
		if(result.CompareTo("Success") != 0)
			return;
		
		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_5Start, GiftEventType.Pay);
		
		PlayerData.ItemType[] itemType = PayJsonData.Instance.GetGiftItemsTypeArr(PayType.RewardsGift5Stars);
		int[] count = PayJsonData.Instance.GetGiftItemsCountsArr(PayType.RewardsGift5Stars);
		
		for(int i = 0; i < itemType.Length; i ++)
		{
			PlayerData.Instance.AddItemNum(itemType[i], count[i]);
		}
		
		Hide();
	}
}
