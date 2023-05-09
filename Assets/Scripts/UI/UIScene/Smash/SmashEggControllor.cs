using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Trứng hun khói
/// </summary>
public class SmashEggControllor : UIBoxBase {

	public static SmashEggControllor Instance = null;

	public EasyFontTextMesh titleText, desText,timeText;
	public GameObject hitButtonGO,grayHitButtonGO;

	private int leftSecond;
	private bool isCanSmash = false;
	private List<float> probabiliryList = new List<float>();
	private  List<SmashEggData.AwardItemStruct> itemList;
	int rewardIndex=0;
	private bool isSmashing=false;
	bool isInit= false;

	public override void Init ()
	{
		base.Init ();
		SmashEggData.Instance.Init();
		SetCanSmash(false);
		SetData ();

		isInit = true;

		base.Init();
	}

	void SetData()
	{
		SmashEggData.Instance.RefreshServerData();

		titleText.text = SmashEggData.Instance.GetTitle ();
		desText.text = SmashEggData.Instance.GetDes ();
		itemList = SmashEggData.Instance.GetItemList();

		InitTimeRecover ();


	}

	void OnApplicationFocus(bool focusStatus)
	{

		if(false == isInit)
			return;
		SetData();
	}

	public override void Show ()
	{
		base.Show ();
		SetData();
	}

	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.SmashEgg);
	}

	public override void Back ()
	{
		if (isCanSmash == false)
			return;
		Hide ();
	}

	#region Button Event
	public void CloseOnClick()
	{
		if(isSmashing )
		{
			return;
		}
		AudioManger.Instance.PlaySound (AudioManger.SoundName.CloseBtClick);
		Hide ();
	}
	public void SmashEggOnCick()
	{
		if (isCanSmash == false)
			return;

		MainInterfaceControllor.Instance.SetSmashEggCount(0);
		SetCanSmash(false);
		PlayerData.Instance.SetUseSmashTime (++leftUseTime);
		PlayerData.Instance.SetLastSmashRecoverLT (System.DateTime.Now.Ticks);

		StartCoroutine(IEHitEffect());

		InitTimeRecover ();

	}
	#endregion

	private void  SetCanSmash(bool isCanSmash)
	{
		this.isCanSmash = isCanSmash;
		hitButtonGO.SetActive( isCanSmash);
		grayHitButtonGO.SetActive( !isCanSmash);
	}

	private void GetReward()
	{
		probabiliryList = SmashEggData.Instance.GetProbabilityList();

		float randValue = Random.value;
		float upLimit = 0;
	
		for(int i=0;i<probabiliryList.Count;++i)
		{
			upLimit += probabiliryList[i];
			if(randValue <= upLimit)
			{
				rewardIndex = i;
				break;
			}
		}
	
		SmashEggData.AwardItemStruct item =  itemList[rewardIndex];
		int rewardId = item.rewardId;
		int count = item.rewardCount;
		if(item.isTrueGift == false)
		{
			PlayerData.ItemType rewardType = RewardData.Instance.GetItemType(rewardId);
			PlayerData.Instance.AddItemNum(rewardType,count);
		}
		AwardItemBoxControllor.Instance.InitData(rewardId,count,GetPhoneNumber);
		UIManager.Instance.ShowModule(UISceneModuleType.AwardItemBox);

	}
	private void GetPhoneNumber()
	{
		SmashEggData.AwardItemStruct item =  itemList[rewardIndex];
		if(item.isTrueGift == false)
		{
			return;
		}
		PhoneNumberInputControllor.CommitPhoneNumCallBack += SendInfo;
		UIManager.Instance.ShowModule (UISceneModuleType.PhoneNumberInput);

	}

	private void SendInfo()
	{
		SmashEggData.Instance.SendAwardInfo(rewardIndex+1);
	}



	#region 砸蛋效果
	public GameObject fullEggGO,halfEggGO,hammerGO,effectPartGO;

	IEnumerator IEHitEffect()
	{
		isSmashing = true;

		fullEggGO.SetActive(true);
		halfEggGO.SetActive(false);
		effectPartGO.SetActive(false);
		hammerGO.SetActive(true);

		Animator animator = hammerGO.GetComponent<Animator>();
		animator.Play("SmashEgg");
		yield return new WaitForSeconds(1.3f);
		hammerGO.SetActive(false);

		effectPartGO.SetActive(true);
		effectPartGO.GetComponent<ParticleSystem>().Play();
		yield return new WaitForSeconds(0.2f);

		fullEggGO.SetActive(false);
		halfEggGO.SetActive(true);

		GetReward();

		yield return new WaitForSeconds(2f);
		fullEggGO.SetActive(true);
		halfEggGO.SetActive(false);
		effectPartGO.SetActive(false);
		hammerGO.SetActive(false);

		isSmashing = false;

	}

	#endregion

	#region 砸金蛋倒计时
	private int intervalSecond;
	private int TotalTime;
	private int leftUseTime;

	/// <summary>
	/// Inits the time recover.
	/// </summary>
	private void InitTimeRecover()
	{
		// date refresh
		System.DateTime nowTime = System.DateTime.Now;
		string lastDateStr = PlayerData.Instance.GetLastSmashDate();
		int deltaDay = 0;
		if (!string.IsNullOrEmpty (lastDateStr)) {
			System.DateTime lastDate = System.DateTime.Parse (lastDateStr);
			deltaDay = nowTime.DayOfYear - lastDate.DayOfYear;
		} else {
			deltaDay =1;
		}
		/*不是同一天就刷新*/
		if (deltaDay >0) {
			PlayerData.Instance.SetUseSmashTime(0);
			PlayerData.Instance.SetLastSmashRecoverLT(System.DateTime.Now.Ticks);
			PlayerData.Instance.SetLastSmashDate();
		}

		
		intervalSecond =  SmashEggData.Instance.GetIntervalTime();
		TotalTime = SmashEggData.Instance.GetTotalTime ();
		leftUseTime = PlayerData.Instance.GetUseSmashTime ();

		//time refresh
		if (leftUseTime >= TotalTime) {
			//have been used
			SetCanSmash(false);
			timeText.text= "Come back tomorrow";
			return;
		}
		
		if (isCanSmash) {
			return;
		}

		long lastTicks = PlayerData.Instance.GetLastSmashRecoverLT ();
		//Debug.Log ("Init tick "+PlayerData.Instance.GetLastSmashRecoverLT().ToString());
		long nowTicks = System.DateTime.Now.Ticks;
		long nowSecond = SystemTicksToSecond (nowTicks);
		long lastSecond = SystemTicksToSecond (lastTicks);
		int deltaSecond = (int)(nowSecond - lastSecond);

		if (deltaSecond >= intervalSecond) {
			//can smash egg
			SetCanSmash(true);
			timeText.text = "Smash Egg";
			MainInterfaceControllor.Instance.SetSmashEggCount(1);
			CancelInvoke("UpdateTimeRecorver");
		} else {
		   //calculate left second
			leftSecond = intervalSecond - deltaSecond;
			//Debug.Log("LeftSecond "+leftSecond);
			MainInterfaceControllor.Instance.SetSmashEggCount(0);
			//updateTimeRecorver
			if (IsInvoking("UpdateTimeRecorver") ==false)
			{
			    InvokeRepeating("UpdateTimeRecorver",0,1);
			}
		}


	}

	private void UpdateTimeRecorver()
	{
		string str = SecondsToTimeStr (leftSecond);
		timeText.text = str;
	
		if (leftSecond <= 0) {
			CancelInvoke("UpdateTimeRecorver");
			SetCanSmash(true);
			timeText.text = "Smash Egg";
			MainInterfaceControllor.Instance.SetSmashEggCount(1);
		}
		--leftSecond;
	}

	string SecondsToTimeStr(int sec)
	{
		string sSecond;
		string sMinute;
		int iSecond;
		int iMinute;
		
		iSecond = sec % 60;
		if (iSecond < 10)
			sSecond = "0" + iSecond;
		else
			sSecond = iSecond + "";
		
		iMinute = sec / 60;
		sMinute = iMinute + "";
		
		return sMinute + ":" + sSecond;
	}
	
	long SystemTicksToSecond(long tick)
	{
		return  ( tick / (10000000) );
	}
	
	long SystemSecondToTicks(long second)
	{
		return second * (10000000);
	}

	#endregion
}
