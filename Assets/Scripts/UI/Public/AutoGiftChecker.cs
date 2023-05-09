using UnityEngine;
using System.Collections;

public enum AutomaticGiftName
{
	DoubleCoin,
	OneKeyToFullLevel,
	HuiKuiBigGift,
	AoteJiaZhu,
	AoteZhaohuan,
	NewPlayerGift
}

public class AutoGiftChecker{

	#region 自动礼包开启关卡参数
	private static int OpenChangWanGiftLevel = PayJsonData.Instance.GetMinLevelToShow(PayType.ChangWanGift);
	private static int OpenDoubleCoinLevel = PayJsonData.Instance.GetMinLevelToShow(PayType.MultiCoin);
	private static int OpenOneKeyToFullLevel = PayJsonData.Instance.GetMinLevelToShow(PayType.OneKey2FullLV);
	private static int OpenAoteJiaZhuLevel = PayJsonData.Instance.GetMinLevelToShow(PayType.CharactersGift);
	private static int OpenNewPlayerGiftLevel = PayJsonData.Instance.GetMinLevelToShow (PayType.NewPlayerGift);

	public static int MaxModelLevelOpenOneKeyToFull
	{
		get{
			return PayJsonData.Instance.GetMaxRoleLevelToShow(PayType.OneKey2FullLV);
		}
	} 
	#endregion

	#region 屏蔽礼包自动弹出条件
	public static bool IsNoAutoGiftVer
	{
		get
		{
			if(PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe
				||PlatformSetting.Instance.PayVersionType == PayVersionItemType.XiaoMiShenHe
				|| PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian
				|| PlatformSetting.Instance.PayVersionType == PayVersionItemType.ChangWan)
				return true;

			return false;
		}
	}
	#endregion

	#region 礼包自动弹出检测变量
	public static bool bIsRandomModel{
		get{
			if(IsNoAutoGiftVer)
				return false;

			if(PayJsonData.Instance.GetLevelLimitActiveState(PayType.AoteZhaohuan))
			    if(PlayerData.Instance.GetCurrentChallengeLevel() < PayJsonData.Instance.GetMinLevelToShow(PayType.AoteZhaohuan))
				    return false;
			return AutoGiftCheck(AutomaticGiftName.AoteZhaohuan, _iEnterMainInterfaceTimes + 1);//进入主页次数计数有延迟，所以+1
		}
	}

	public static bool CTypeGiftBagEnabled{
		get
		{
			if(IsNoAutoGiftVer)
				return false;
			
			if(!PlatformSetting.Instance.IsCTypeBag)
				return false;
			
			if(PlayerData.Instance.GetCTypeBagState())
				return false;
			
			return true;
		}
	}
	#endregion

	#region 礼包自动弹出检测函数（依据签到天数）
	private static bool DateLimitedCheck(PayType payJsonType)
	{
		if(IsNoAutoGiftVer)
			return false;
		
		int[] days = PayJsonData.Instance.GetSignInDaysToShow(PayType.ChangWanGift);
		int day = Mathf.Min(PlayerData.Instance.GetSignInTimes(), 7);
		for(int i = 0; i < days.Length; i ++)
		{
			if(days[i] == day)
				return true;
		}
		return false;
	}
	
	#endregion

	#region 礼包自动弹出检测次数记录 (用于参数A)
	private static int _iEnterModelUpgradeTimes = 0;
	public static int iEnterModelUpgradeTimes{
		get{
			return _iEnterModelUpgradeTimes;
		}
		
		set{
			if(IsNoAutoGiftVer)
				return;

			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenOneKeyToFullLevel)
				return;

			_iEnterModelUpgradeTimes = value;

			if(AutoGiftCheck(AutomaticGiftName.OneKeyToFullLevel, _iEnterModelUpgradeTimes))
			{
				OneKeyToFullLevelControllor.Instance.InitData(CharacterDetailControllor.Instance.iCurModelId);

				if(GlobalConst.SceneName == SceneType.UIScene)
				    UIManager.Instance.ShowModule(UISceneModuleType.OneKeyToFullLevel);
				else
					GameUIManager.Instance.ShowModule(UISceneModuleType.OneKeyToFullLevel);

				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_OneKeyFullLevel, GiftEventType.Auto);

				return;
			}
		}
	}

	private static int _iEnterLevelSelectTimes = 0;
	public static int iEnterLevelSelectTimes{
		get{
			return _iEnterLevelSelectTimes;
		}
		
		set{
			if(IsNoAutoGiftVer)
				return;

			_iEnterLevelSelectTimes = value;

			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenNewPlayerGiftLevel)
				return;

			if(AutoGiftCheck(AutomaticGiftName.NewPlayerGift,_iEnterLevelSelectTimes))
			{
				UIManager.Instance.ShowModule(UISceneModuleType.NewPlayerGift);
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_NewPlayerGet, GiftEventType.Auto);
				return;
			}

			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenDoubleCoinLevel)
				return;

			if(AutoGiftCheck(AutomaticGiftName.DoubleCoin, _iEnterLevelSelectTimes))
			{
				UIManager.Instance.ShowModule(UISceneModuleType.DoubleCoin);
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_DoubleCoin, GiftEventType.Auto);
				return;
			}
		}
	}
	
	private static int _iEnterMainInterfaceTimes = 0;
	public static int iEnterMainInterfaceTimes
	{
		get{
			return _iEnterMainInterfaceTimes;
		}
		set{
			if(IsNoAutoGiftVer)
				return;

			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenAoteJiaZhuLevel && PayJsonData.Instance.GetLevelLimitActiveState(PayType.CharactersGift))
				if((PayJsonData.Instance.GetDateLimitActiveState(PayType.ChangWanGift) && !DateLimitedCheck(PayType.ChangWanGift)))
					return;

			_iEnterMainInterfaceTimes = value;

			if(AutoGiftCheck(AutomaticGiftName.AoteJiaZhu, _iEnterMainInterfaceTimes))
			{
				UIManager.Instance.ShowModule(UISceneModuleType.AotemanFamily);
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_AoteBrother, GiftEventType.Auto);
				return;
			}

			if(AutoGiftCheck(AutomaticGiftName.HuiKuiBigGift, _iEnterMainInterfaceTimes))
			{
				GiftPackageControllor.Instance.Show(PayType.ChangWanGift);
				CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_LimitTime, GiftEventType.Auto);
				return;
			}
		}
	}
	#endregion

	#region 礼包自动弹出检测函数（依据参数A）

	/// <summary>
	/// “预测”是否显示相应礼包
	/// </summary>
	/// <returns><c>true</c>, if auto gift check was foreseen, <c>false</c> otherwise.</returns>
	/// <param name="giftName">Gift name.</param>
	public static bool ForeseeAutoGiftCheck(AutomaticGiftName giftName)
	{
		if(IsNoAutoGiftVer)
			return false;
		
		int a, curTimes;

		int level = PlayerData.Instance.GetCurrentChallengeLevel();

		switch(giftName)
		{
		case AutomaticGiftName.NewPlayerGift:
			if(level < OpenNewPlayerGiftLevel)
				return false;
			curTimes = _iEnterLevelSelectTimes + 1;
			break;
		case AutomaticGiftName.DoubleCoin:
			if(level < OpenDoubleCoinLevel)
				return false;
			curTimes = _iEnterLevelSelectTimes + 1;
			break;
		case AutomaticGiftName.OneKeyToFullLevel:
			if(level < OpenOneKeyToFullLevel)
				return false;
			curTimes = _iEnterModelUpgradeTimes + 1;
			break;
		case AutomaticGiftName.HuiKuiBigGift:
			if(level < OpenChangWanGiftLevel)
				return false;
			curTimes = _iEnterMainInterfaceTimes + 1;
			break;
		default:
			a = 0;
			curTimes = 0;
			break;
		}

		bool isShow= AutoGiftCheck(giftName, curTimes);
		//Debug.Log ("ForeseeAutoGiftCheck "+giftName.ToString()+isShow.ToString());
		return isShow;
	}

	private static bool AutoGiftCheck(AutomaticGiftName giftName, int curTimes)
	{
		if(IsNoAutoGiftVer)
			return false;
		
		int a;
		PayType payJsonType;

		switch(giftName)
		{
		case AutomaticGiftName.NewPlayerGift:
			if(PlayerData.Instance.GetUIGuideState(UIGuideType.LeftRoleGuide))
				return false;
			if(PlayerData.Instance.GetHuiKuiMiniGiftState())
				return false;
			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenNewPlayerGiftLevel && PayJsonData.Instance.GetLevelLimitActiveState(PayType.NewPlayerGift))
				return false;
			payJsonType = PayType.NewPlayerGift;
			break;
		case AutomaticGiftName.DoubleCoin:
			if(PlayerData.Instance.GetUIGuideState(UIGuideType.LeftRoleGuide))
				return false;
			if(PlayerData.Instance.GetForeverDoubleCoin() == 1)
				return false;
			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenDoubleCoinLevel && PayJsonData.Instance.GetLevelLimitActiveState(PayType.MultiCoin))
				return false;
			payJsonType = PayType.MultiCoin;
			break;
		case AutomaticGiftName.OneKeyToFullLevel:
			if(!PayJsonData.Instance.GetIsActivedState(PayType.OneKey2FullLV))
				return false;

			if(PlayerData.Instance.GetOneKeyToFullLevelGetedState())
				return false;
			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenOneKeyToFullLevel && PayJsonData.Instance.GetLevelLimitActiveState(PayType.OneKey2FullLV))
				return false;
			payJsonType = PayType.OneKey2FullLV;
			break;
		case AutomaticGiftName.HuiKuiBigGift:
			if(!PayJsonData.Instance.GetIsActivedState(PayType.ChangWanGift))
				return false;

			if(PlayerData.Instance.CheckHasGetedHuiKuiBigGiftState(PayJsonData.Instance.GetGiftID(PayType.ChangWanGift)))
				return false;
			
			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenChangWanGiftLevel && PayJsonData.Instance.GetLevelLimitActiveState(PayType.ChangWanGift))
				return false;
			
			if(!DateLimitedCheck(PayType.ChangWanGift) && PayJsonData.Instance.GetDateLimitActiveState(PayType.ChangWanGift))
				return false;
			
			payJsonType = PayType.ChangWanGift;
			break;
		case AutomaticGiftName.AoteJiaZhu:
			if(GlobalConst.ShowModuleType == UISceneModuleType.LockTip)
				return false;

			if(CheckGetedAllAotemanState())
				return false;

			if(PlayerData.Instance.GetCurrentChallengeLevel() < OpenAoteJiaZhuLevel && PayJsonData.Instance.GetLevelLimitActiveState(PayType.CharactersGift))
				return false;

			payJsonType = PayType.CharactersGift;
			break;
		case AutomaticGiftName.AoteZhaohuan:
			payJsonType = PayType.AoteZhaohuan;
			break;
		default:
			//没用，但不赋值会报错: error CS0165: Use of unassigned local variable `payJsonType'
			payJsonType = PayType.MultiCoin;
			break;
		}

		a = PayJsonData.Instance.GetParameterA(payJsonType);
	
		if(!PlayerData.Instance.GetIsUIGuideEnd())
			return false;
		if(!PayJsonData.Instance.GetLevelLimitActiveState(payJsonType) && !PayJsonData.Instance.GetDateLimitActiveState(payJsonType))
			return false;
		return AutoGiftCheck(a, curTimes);

	}

	private static bool CheckGetedAllAotemanState()
	{
		int[] playerModelStateIds = PlayerData.Instance.GetModelState();
		int modelCount = ModelData.Instance.GetUseModelDataCount();
		
		bool bGetedAllAoteman = true;
		for(int i = 1; i <= modelCount; ++i)
		{
			//检测玩家数据中是否存在模型.
			bool hasThis = false;
			for(int j = 0; j < playerModelStateIds.Length; ++j)
			{
				if(IDTool.GetModelType(playerModelStateIds[j]) == i)
				{
					hasThis = true;
					break;
				}
			}
			
			if(!hasThis)
			{
				bGetedAllAoteman = false;
				break;
			}
		}

		return bGetedAllAoteman;
	}

	private static bool AutoGiftCheck(int a,  int curTimes)
	{
		//参数定义：每A次
		//A为0则只第1次弹；
		if(a == 0){
			if(curTimes == 1)
				return true;
			else
				return false;
		}
		
		//为1则第1次不弹，之后次次弹；
		if(a == 1){
			if(curTimes == 1)
				return false;
			else
				return true;
		}
		
		//为2则第1次弹，之后隔1次弹1次，为3，...，以此类推 （隔N-1次显示）
		if(curTimes % a == 1)
			return true;
		else
			return false;
	}
	#endregion
}
