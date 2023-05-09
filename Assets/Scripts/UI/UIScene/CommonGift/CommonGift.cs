using UnityEngine;
using System.Collections;
using DG.Tweening;
using PayBuild;

public class CommonGift : UIBoxBase {

	public static CommonGift Instance = null;
	public Transform rootTran;
	public Transform leftTran;
	public Transform btnTRan;
	public Transform giftTran;
	public Transform handTran;
	public Transform handClick;
	public EasyFontTextMesh btnText;
	public EasyFontTextMesh chargingInfo;
	public EasyFontTextMesh timeText;
	public ProgressBarNoMask progress;
	public tk2dUIMask giftMask;
	public ParticleSystem clickGiftParticle;
	private float powerMaskSizeY = 10;
	private const float countDownTime = 10;
	private int clickCount = 0;
	private const int commonGiftClickCount = 77;
	private const int giftShowClickCount = 10;
	private float handAnimTime = 0.1f;
	private Vector3 handAnimEndPos = new Vector3(140,-235,0);
	private Vector3 handClickAnimEndPos = new Vector3(99,-184,0);
	private float giftAnimTime = 0.6f;
	private bool isPauseCountDownTime = false;
	private PayType commonGiftType;


	public override void Init ()
	{
		base.Init ();
		Instance = this;
		isPauseCountDownTime = false;
		timeText.text = "00:10";
		clickCount = 0;
		giftAnimTime = 0.6f;
		giftMask.size = new Vector2 (250,powerMaskSizeY);
		giftMask.Build ();
		chargingInfo.gameObject.SetActive (false);
		btnText.gameObject.SetActive (false);
		commonGiftType = PlayerData.Instance.GetCommonGiftType();
		float cost = PayData.Instance.GetCost(commonGiftType);
		chargingInfo.text = cost.ToString() + "usd" + PayJsonData.Instance.GetGiftTitle(commonGiftType) + "并开启大量游戏礼包";
		AdjustPosByPlatformItemType ();
	}
	void AdjustPosByPlatformItemType()
	{
		switch(PlatformSetting.Instance.PlatformType)
		{
		case PlatformItemType.YiDong:
			leftTran.localPosition = new Vector3(0,-10,0);
			btnTRan.localPosition = new Vector3(120,-121.5f,0);
			break;
		case PlatformItemType.DianXin:
			leftTran.localPosition = new Vector3(0,-130f,0);
			btnTRan.localPosition = new Vector3(120,-10,0);
			break;
		case PlatformItemType.LianTong:
			leftTran.localPosition = new Vector3(0,-10,0);
			btnTRan.localPosition = new Vector3(120,-121.5f,0);
			break;
		}
	}
	void BtnShake()
	{
		DOTween.Kill ("shakeExActivity");
		Sequence mySequence = DOTween.Sequence();
		mySequence.Append (giftTran.DOShakePosition (0.1f, new Vector3(15,0,0)).SetDelay (1f).SetLoops(4));
		mySequence.SetLoops (-1).SetId("shakeExActivity");
	}
	public override void Show ()
	{
		base.Show ();
		StopCoroutine ("CountDown");
		StartCoroutine ("CountDown");
		handTran.localPosition = new Vector3 (173,-270,0);
		handTran.DOLocalMove (handAnimEndPos, handAnimTime).SetLoops (int.MaxValue, LoopType.Yoyo).SetEase (Ease.InOutSine);
		//handClick.localPosition = new Vector3 (1000,-270,0);
		//handClick.DOLocalMove (handClickAnimEndPos, handAnimTime).SetLoops (int.MaxValue, LoopType.Yoyo).SetEase (Ease.InOutSine);
		StartCoroutine ("IEHandClickAnim");
		giftTran.localScale = Vector3.one;
		giftTran.DOScale (Vector3.one, giftAnimTime).OnComplete(HeartAnimation1).SetEase (Ease.Linear);
		btnTRan.DOScale (Vector3.one * 1.1f, 0.1f).SetLoops (int.MaxValue, LoopType.Yoyo).SetEase (Ease.Linear);
		BtnShake ();
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.CommonGift,"State","弹出次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());
	}
	void HandClickAnim()
	{
		StartCoroutine ("IEHandClickAnim");
	}
	IEnumerator IEHandClickAnim()
	{
		yield return new WaitForSeconds (2 * handAnimTime);
		handClick.gameObject.SetActive (true);
		yield return null;
		handClick.gameObject.SetActive (false);
		StartCoroutine ("IEHandClickAnim");
	}
	void HeartAnimation1()
	{
		if(giftAnimTime > 0.1f)
			giftAnimTime -= 0.03f;
		AudioManger.Instance.PlaySound (AudioManger.SoundName.CommonGift);
		giftTran.DOScale (Vector3.one, giftAnimTime).OnComplete(HeartAnimation2).SetEase (Ease.Linear);
	}
	void HeartAnimation2()
	{
		if(giftAnimTime > 0.1f)
			giftAnimTime -= 0.03f;
		giftTran.DOScale (Vector3.one, giftAnimTime).OnComplete(HeartAnimation1).SetEase (Ease.Linear);
	}
	private IEnumerator CountDown()
	{
		float tempTime = countDownTime;
		while(tempTime > 0)
		{
			while(isPauseCountDownTime)
				yield return null;
			float intTime = Mathf.Round(tempTime);
			if(intTime < 10)
				timeText.text = 0 + intTime.ToString();
			else
				timeText.text = intTime.ToString();
			progress.SetProgress(tempTime / countDownTime);
			tempTime -= Time.deltaTime;
			yield return null;
		}
		Hide ();
		//弹完C包后，显示关卡信息
		if (GlobalConst.StartGameGuideEnabled) {
			UIGuideControllor.Instance.Show (UIGuideType.LevelInfoGuide);
		}
	}
	void Reset()
	{
		isPauseCountDownTime = false;
		timeText.text = "00:10";
		clickCount = 0;
		giftAnimTime = 0.6f;
		giftMask.size = new Vector2 (250,powerMaskSizeY);
		giftMask.Build ();
		chargingInfo.gameObject.SetActive (false);
		btnText.gameObject.SetActive (false);
		giftTran.DOKill ();
		btnTRan.DOKill ();
		handTran.DOKill ();
	}

	[HideInInspector]public Transform tranPs;
	public override void Hide ()
	{
		Reset ();
		gameObject.SetActive (false);
		UIManager.Instance.HideModule(UISceneModuleType.CommonGift);
		if(tranPs != null)
		{
			//SceneParticleController.Instance.Recycle(tranPs,0.01f);
		}
	}
	public override void Back ()
	{
		Hide ();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		if (GlobalConst.StartGameGuideEnabled) {
			UIGuideControllor.Instance.Show (UIGuideType.LevelInfoGuide);
		}
	}
	public void Close()
	{
		Hide ();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.CommonGift,"State","点关闭按钮次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());
		if (GlobalConst.StartGameGuideEnabled) {
			UIGuideControllor.Instance.Show (UIGuideType.LevelInfoGuide);
		}
	}
	public void OnClick()
	{
		if (clickCount > commonGiftClickCount)
			return;
		clickCount++;
		clickGiftParticle.Play();
		giftMask.size = new Vector2 (250, powerMaskSizeY + 168 * Mathf.Sin(Mathf.PI / 2 * clickCount / commonGiftClickCount * 1.0f));
		giftMask.Build();
		if (clickCount == giftShowClickCount) {
			chargingInfo.gameObject.SetActive (true);
			btnText.gameObject.SetActive (true);
		} else if (clickCount == giftShowClickCount + 1) {
			chargingInfo.gameObject.SetActive (false);
			btnText.gameObject.SetActive (false);
			Buy();
		}else if(clickCount == commonGiftClickCount)
		{
			Hide ();
			PlayerData.Instance.SetCommonGiftIsBuy(true);
			AudioManger.Instance.PlaySound (AudioManger.SoundName.CashMachine);
			UIManager.Instance.ShowModule (UISceneModuleType.CommonGiftAward);
			CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.CommonGift,"State","点满次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());
		}
	}
	#region  扣费处理
	void Buy()
	{
		isPauseCountDownTime = true;
		
		PayBuildPayManage.Instance.Pay(PayData.Instance.GetPayCode(commonGiftType), CallBack);
		
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.CommonGift,"State","购买次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());
	}
	
	
	void CallBack(string result)
	{
		isPauseCountDownTime = false;
		if (result.CompareTo ("Success") == 0) {
			PlayerData.Instance.SetCommonGiftIsBuy(true);
			Hide ();
			if(commonGiftType == PayType.NewPlayerGift )
			{
				PlayerData.Instance.SetHuiKuiMiniGiftState(true);
			}
			else if(commonGiftType == PayType.MultiCoin)
			{
				PlayerData.Instance.SetForeverDoubleCoin();
			}
			else if(commonGiftType == PayType.CharactersGift)
			{
				PlayerData.Instance.SetAoteBrotherState (true);
			}
			else
			{
				PlayerData.ItemType[] itemType = PayJsonData.Instance.GetGiftItemsTypeArr(commonGiftType);
				int[] count = PayJsonData.Instance.GetGiftItemsCountsArr(commonGiftType);
				
				for(int i = 0; i < itemType.Length; i ++)
				{
					PlayerData.Instance.AddItemNum(itemType[i], count[i]);
				}
			}
			AudioManger.Instance.PlaySound (AudioManger.SoundName.CashMachine);
			UIManager.Instance.ShowModule (UISceneModuleType.CommonGiftAward);
			CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.CommonGift, "State", "付费成功次数", "Level", PlayerData.Instance.GetSelectedLevel ().ToString ());
		} else {
			CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.CommonGift, "State", "付费失败次数", "Level", PlayerData.Instance.GetSelectedLevel ().ToString ());
		}
	}
	#endregion
}
