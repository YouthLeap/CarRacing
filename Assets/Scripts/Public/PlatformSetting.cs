using UnityEngine;
using System.Collections;

public enum PlatformItemType
{
	YiDong = 0,
	DianXin = 1,
	LianTong = 2
};
public enum ChannelItemType
{
	SDK = 0,		//使用SDK的退出接口
	Default = 1		//使用游戏自带的退出接口
};
public enum PayVersionItemType
{
	LiBaoBai = 0,
	LiBaoHei = 1,
	ShenHe = 2,
	ChangWan = 3,
	XiaoMiShenHe = 4,
	XiaoMiTuiGuang = 5,
	GuangDian = 6,
};

public class PlatformSetting : MonoBehaviour {
	
	public static PlatformSetting Instance;

	public void Awake()
	{
		Instance = this;
		DontDestroyOnLoad (gameObject);
		gameObject.name = "PlatformSetting";
		Init ();
	}


	/// <summary>
	/// 审核,畅玩版本不自动弹出礼包，不加按钮特效
	/// </summary>
	/// <value><c>true</c> if this instance is no aotu gift ver; otherwise, <c>false</c>.</value>
	bool IsNoAotuGiftVer
	{
		get{
			if(PayVersionType == PayVersionItemType.ShenHe
			   ||PayVersionType == PayVersionItemType.XiaoMiShenHe
				||PayVersionType == PayVersionItemType.ChangWan
				||PayVersionType == PayVersionItemType.GuangDian)
				return true;

			return false;
		}
	}

	#region 变量: ABC包控制, 按钮效果控制 
	// AB包控制
	/*
	 AB包：
     A: 第一关显示通关奖励
     B: 第一关、第三关通关奖励显示380宝石 
     优先显示B包
	 */
	private bool isATypeBag;
	public bool IsATypeBag
	{
		get
		{
			if(IsNoAotuGiftVer)
				return false;

			return isATypeBag;
		}
		set{isATypeBag = value;}
	}

	private bool isBTypeBag;
	public bool IsBTypeBag
	{
		get
		{
			if(IsNoAotuGiftVer)
				return false;

			return isBTypeBag;
		}
		set{isBTypeBag = value;}
	}

	/*
	 * C包
	 * 第一次进入游戏的时候进入首页，弹出“大堆金币”礼包（30元计费点）
	 */
	private bool isCTypeBag;
	public bool IsCTypeBag
	{
		get
		{
			if(IsNoAotuGiftVer)
				return false;
			
			return isCTypeBag;
		}
		set{isCTypeBag = value;}
	}

	/*
	 * 按钮效果控制
     * 呼吸效果 和 手指指示点击
	 */
	private bool playBreathingEffect;
	public bool PlayBreathingEffect
	{
		get
		{
			if(IsNoAotuGiftVer)
				return false;

			return playBreathingEffect;
		}
		set{playBreathingEffect = value;}
	}
	private bool playFingerGuide;
	public bool PlayFingerGuide
	{
		get
		{
			if(IsNoAotuGiftVer)
				return false;

			return playFingerGuide;
		}
		set{playFingerGuide = value;}
	}
	#endregion

	#region 变量: 平台，渠道，付费版本，更多游戏，关于信息，电话号码
	private PlatformItemType platformType;
	public PlatformItemType PlatformType
	{
		get
		{
			return platformType;
		}
		set
		{
			platformType = value;
		}
	}
	private ChannelItemType channelType;
	public ChannelItemType ChannelType
	{
		get
		{
			return channelType;
		}
		set
		{
			channelType = value;
		}
	}
	private PayVersionItemType payVersionType;
	public PayVersionItemType PayVersionType
	{
		get 
		{
			return payVersionType;
		}set
		{
			payVersionType = value;
		}
	}

	private bool showMoreGame;
	public bool ShowMoreGame
	{
		get 
		{
			return showMoreGame;
		}set
		{
			showMoreGame = value;
		}
	}

	private bool showAboutInfo;
	public bool ShowAboutInfo
	{
		get 
		{
			return showAboutInfo;
		}set
		{
			showAboutInfo = value;
		}
	}

	private string telephoneNumber;
	public string TelephoneNumber
	{
		get 
		{
			return telephoneNumber;
		}set
		{
			telephoneNumber = value;
		}
	}

	private string appName;
	public string AppName
	{
		get 
		{
			return appName;
		}set
		{
			appName = value;
		}
	}
	#endregion

	public void Init()
	{
//		Debug.Log ("PlatformSetting_Init");

		PlatformType = (PlatformItemType)System.Enum.Parse(typeof(PlatformItemType), PlayerData.Instance.GetPlatformType());
		ChannelType = (ChannelItemType)System.Enum.Parse(typeof(ChannelItemType), PlayerData.Instance.GetChannelType());
		PayVersionType = (PayVersionItemType)System.Enum.Parse(typeof(PayVersionItemType), PlayerData.Instance.GetPayVersionType());
		IsATypeBag = PlayerData.Instance.GetIsATypeBagState();
		IsBTypeBag = PlayerData.Instance.GetIsBTypeBagState();
		isCTypeBag = PlayerData.Instance.GetIsCTypeBagState();
		playBreathingEffect = PlayerData.Instance.GetPlayBreathingEffectState();
		playFingerGuide = PlayerData.Instance.GetPlayFingerGuideState();

		ShowMoreGame = false;
		ShowAboutInfo = false;

		TelephoneNumber = "-";
		AppName = "Super Hero Transformer Car Racing";

		SetCommonGift ("4");
		SetOpenCommonGiftType ("false");

		#if UNITY_IOS
		SetPlatformType ("0");
		SetPayVersionType ("2");
		SetPayVersionEnable ("true");
		#endif
	}

	#region ABC包控制
	public void SetATypeBagState(string result)
	{
		bool state = bool.Parse(result);
		print ("AType: " + state.ToString ());
		PlatformSetting.Instance.IsATypeBag = state;
		PlayerData.Instance.SetIsATypeBagState (state);
	}

	public void SetBTypeBagState(string result)
	{
		bool state = bool.Parse(result);
		print ("BType: " + state.ToString ());
		PlatformSetting.Instance.IsBTypeBag = state;
		PlayerData.Instance.SetIsBTypeBagState (state);
	}

	public void SetCTypeBagState(string result)
	{
		bool state = bool.Parse(result);
		print ("CType: " + state.ToString ());
		PlatformSetting.Instance.IsCTypeBag = state;
		PlayerData.Instance.SetIsCTypeBagState (state);
	}
	#endregion

	#region 按钮效果控制  呼吸效果和手指指示点击
	public void SetPlayBreathingEffectState(string result)
	{
		bool state = bool.Parse(result);
		print ("PlayBreathingEffect: " + state.ToString ());
		PlatformSetting.Instance.PlayBreathingEffect = state;
		PlayerData.Instance.SetPlayBreathingEffectState (state);
	}
	
	public void SetPlayFingerGuideState(string result)
	{
		bool state = bool.Parse(result);
		print ("PlayFingerGuide: " + state.ToString ());
		PlatformSetting.Instance.PlayFingerGuide = state;
		PlayerData.Instance.SetPlayFingerGuideState (state);
	}
	#endregion


	#region 平台，渠道，付费版本，更多游戏，关于信息，电话号码
	public void SetPlatformType(string result)
	{
		PlatformItemType itemType = (PlatformItemType)int.Parse(result);
		print ("PlatformType: " + itemType.ToString ());
		PlatformSetting.Instance.PlatformType = itemType;
		PlayerData.Instance.SetPlatformType (itemType.ToString ());
	}

	public void SetChannelType(string result)
	{		
		ChannelItemType itemType = (ChannelItemType)int.Parse(result);
		print ("ChannelType: " + itemType.ToString ());
		PlatformSetting.Instance.ChannelType = itemType;
		PlayerData.Instance.SetChannelType (itemType.ToString ());
	}

	public void SetPayVersionType(string result)
	{
		PayVersionItemType itemType = (PayVersionItemType)int.Parse(result);
		print ("PayVersionType: " + itemType.ToString ());
		PlatformSetting.Instance.PayVersionType = itemType;
		PlayerData.Instance.SetPayVersionType (itemType.ToString ());
		PayJsonData.Instance.RefreshJsonData();
	}

	public void SetShowMoreGame(string result)
	{
		bool flag = bool.Parse (result);
		print ("ShowMoreGame: " + flag.ToString ());
		PlatformSetting.Instance.ShowMoreGame = flag;
	}

	public void SetShowAboutInfo(string result)
	{
		bool flag = bool.Parse (result);
		print ("ShowAboutInfo: " + flag.ToString ());
		PlatformSetting.Instance.ShowAboutInfo = flag;
	}

	public void SetTelephoneNumber(string result)
	{
		print ("TelephoneNumber: " + result);
		PlatformSetting.Instance.TelephoneNumber = result;
	}
	#endregion

	#region 百度退出接口
	public void ExitGame()
	{
		#if UNITY_ANDROID
		AndroidJavaClass androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJO = androidJC.GetStatic<AndroidJavaObject>("currentActivity");
		androidJO.Call("OnExitGame");
		#endif
	}
	#endregion 

	#region 更多游戏接口
	public void MoreGame()
	{
		#if UNITY_ANDROID
		AndroidJavaClass androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJO = androidJC.GetStatic<AndroidJavaObject>("currentActivity");
		androidJO.Call("OnMoreGame");
		#endif
	}
	#endregion

	#region 更多游戏接口
	public void AboutInfo()
	{
		#if UNITY_ANDROID
		AndroidJavaClass androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJO = androidJC.GetStatic<AndroidJavaObject>("currentActivity");
		androidJO.Call("OnAboutInfo");
		#endif
	}
	#endregion

	#region 更新版本控制数据
	public void UpdatePayVersionData(string result)
	{
		print ("UpdatePayVersionData: " + result);
		PayJsonData.Instance.UpdateJsonData (result);
	}
	#endregion

	#region 开启或者关闭版本控制
	public void SetPayVersionEnable(string result)
	{
		bool flag = bool.Parse (result);
		print ("PayVersionEnable: " + flag.ToString ());
		PlayerData.Instance.SetPayVersionEnable (flag);
	}
	#endregion

	#region 开启或者关闭游戏声音
	public void SetGameMusicEnable(string result)
	{
		bool flag = bool.Parse (result);
		print ("GameMusicEnable: " + flag.ToString ());
		if (flag) {
			AudioManger.Instance.MusicVolume = 1.0f;
			AudioManger.Instance.SoundVolume = 1.0f;
		} else {
			AudioManger.Instance.MusicVolume = 0.0f;
			AudioManger.Instance.SoundVolume = 0.0f;
		}
	}
	#endregion

	#region 在线抽奖
	public string GetPhoneInfoJsonData()
	{
		#if UNITY_ANDROID
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJO = androidJavaClass.GetStatic<AndroidJavaObject> ("currentActivity");
			return androidJO.Call<string> ("GetPhoneInfoJsonData");
		}catch
		{
			//Debug.Log("获取手机信息失败");
			return "{\"IMEI\":\"XXXX\", \"IMSI\":\"XXX\", \"MAC\":\"XXXX\", \"versionCode\":\"1\",\"channelid\":\"6866023\"}";
		}
		#else
		return "{\"IMEI\":\"123456789012348\", \"IMSI\":\"123456789012348\", \"MAC\":\"XXXX8F\", \"versionCode\":\"000\",\"channelid\":\"6000011\"}";
		#endif
	}
	#endregion

	#region 控制活动按钮的显示
	public bool isOpenActivity = false;
	public bool isOpenExchangeActivity = false;
	public bool isOpenSmashEgg = false;
	public bool isOpenLuckyNumbersActivity = false;
	public bool isOpenClearanceRedPaper = false;
	public bool isOpenGamePlayingActivity = false;
	
	public void SetOpenActivity(string Open)
	{
		bool isOpen = bool.Parse(Open);
		Debug.Log("SetOpenActivity" + isOpen.ToString());
		isOpenActivity = isOpen;
	}
	
	public void SetOpenExchangeActivity(string Open)
	{
		bool isOpen = bool.Parse(Open);
		Debug.Log("SetOpenExchangeActivity " + isOpen.ToString());
		isOpenExchangeActivity = isOpen;
	}
	
	public void SetOpenSmashEgg(string Open)
	{
		bool isOpen = bool.Parse(Open);
		Debug.Log("SetOpenSmashEgg" + isOpen.ToString());
		isOpenSmashEgg = isOpen;
	}
	public void SetOpenLuckyNumbersActivity(string Open)
	{
		bool isOpen = bool.Parse(Open);
		Debug.Log("isOpenLuckyNumbersActivity "+isOpen.ToString());
		isOpenLuckyNumbersActivity = isOpen;
	}
	public void SetOpenClearanceRedPaper(string Open)
	{
		bool isOpen = bool.Parse(Open);
		Debug.Log("isOpenClearanceRedPaper "+isOpen.ToString());
		isOpenClearanceRedPaper = isOpen;
	}
	public void SetOpenGamePlayingActivity(string Open)
	{
		bool isOpen = bool.Parse(Open);
		Debug.Log("isOpenGamePlayingActivity "+isOpen.ToString());
		isOpenGamePlayingActivity = isOpen;
		if (isOpen) {
			if (!PlayerData.Instance.GetGamePlayingActivityInit ()) {
				PlayerData.Instance.SetGamePlayingActivityInit (true);
				PlayerData.Instance.SetGamePlayingActivityScore (0);
				PlayerData.Instance.SetGamePlayingActivityStage (1);
				PlayerData.Instance.SetGamePlayingActivityAchievie (false);
				PlayerData.Instance.SetGamePlayingActivityAllAchievie (false);
			}
		} else {
			PlayerData.Instance.SetGamePlayingActivityInit (false);
		}
	}
	#endregion

	#region 按钮文字控制
	private string payButtonText;
	public string PayButtonText{
		get{ return payButtonText;}
		set{ payButtonText = value;}
	}

	public void SetPayButtonText(string result)
	{
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe)
		{
			PlatformSetting.Instance.PayButtonText = "0";
			return;
		}
		print ("SetPayButtonText" + result);
		PlatformSetting.Instance.PayButtonText = result;
	}
	#endregion

	#region 控制投诉按钮的显示
	private bool isShowComplainBtn;
	public bool IsShowComplainBtn
	{
		get
		{
			return isShowComplainBtn;
		}set
		{
			isShowComplainBtn = value;
		}
	}

	public void SetIsShowComplainBtn(string result)
	{
		bool flag = bool.Parse (result);
		print ("IsShowComplainBtn: " + flag.ToString ());
		PlatformSetting.Instance.IsShowComplainBtn = flag;
	}
	#endregion

	#region 联通月卡礼包弹出控制
	// 月卡礼包弹出控制
	private bool isShowedMonthCardGift;
	public bool IsShowedMonthCardGift
	{
		get
		{
			if(IsNoAotuGiftVer)
				return false;
			
			return isShowedMonthCardGift;
		}
		set{isShowedMonthCardGift = value;}
	}
	
	// 月卡礼包是否自动续费控制
	private bool monthCardGiftAutoRenew;
	public bool MonthCardGiftAutoRenew
	{
		get
		{
			if(IsNoAotuGiftVer)
				return false;
			
			return monthCardGiftAutoRenew;
		}
		set{monthCardGiftAutoRenew = value;}
	}
	public void SetIsShowedMonthCardGift(string result)
	{
		bool state = bool.Parse(result);
		print ("ShowedMonthCardGift: " + state.ToString ());
		PlatformSetting.Instance.IsShowedMonthCardGift = state;
		PlayerData.Instance.SetIsShowedMonthCardGift (state);
	}
	#endregion
	
	#region 联通月卡礼包是否自动续费
	public void SetMonthCardGiftAutoRenewState(string result)
	{
		bool state = bool.Parse(result);
		print ("MonthCardGiftAutoRenew: " + state.ToString ());
		PlatformSetting.Instance.MonthCardGiftAutoRenew = state;
		PlayerData.Instance.SetMonthCardGiftAutoRenewState (state);
	}
	#endregion
	#region 全民抢礼包
	public void SetCommonGift (string result)
	{
		PayType state = (PayType)int.Parse (result);
		if(state == PayType.NewPlayerGift && PlayerData.Instance.GetHuiKuiMiniGiftState())
		{
			PlayerData.Instance.SetCommonGiftIsBuy(true);
		}
		else if(state == PayType.MultiCoin && PlayerData.Instance.GetForeverDoubleCoin() == 1)
		{
			PlayerData.Instance.SetCommonGiftIsBuy(true);
		}
		else if(state == PayType.CharactersGift && PlayerData.Instance.GetAoteBrotherState())
		{
			PlayerData.Instance.SetCommonGiftIsBuy(true);
		}
		else if(PlayerData.Instance.GetCommonGiftType() != state)
		{
			PlayerData.Instance.SetCommonGiftIsBuy(false);
			PlayerData.Instance.SetCommonGiftType(state);
		}
	}
	
	private bool isOpenCommonGiftType = false;
	public bool IsOpenCommonGiftType
	{
		set{isOpenCommonGiftType = value;}
		get
		{
			if(IsNoAotuGiftVer)
				return false;
			return isOpenCommonGiftType;
		}
	}
	public void SetOpenCommonGiftType (string result)
	{
		bool state = bool.Parse(result);
		IsOpenCommonGiftType = state;
	}
	#endregion
}