using UnityEngine;
using System.Collections;

public class MonthCardGiftRewardControllor : UIBoxBase {
	
	
	
	public override void Init ()
	{
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();
		//自定义事件.
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Reward_MonthCard,"State","弹出次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());
		
	}
	
	
	public override void Hide ()
	{
		base.Hide();
		UIManager.Instance.HideModule (UISceneModuleType.MonthCardGiftReward);
		if(preBoxType == UISceneModuleType.LevelInfo)
		{
			if(PlatformSetting.Instance.PayVersionType == PayVersionItemType.ChangWan && PlayerData.Instance.GetHuiKuiMiniGiftState()==false)
			{
				CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_NewPlayerGet,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
				UIManager.Instance.ShowModule (UISceneModuleType.NewPlayerGift);
			}
			else if (AutoGiftChecker.CTypeGiftBagEnabled) {
				CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_LimitTime,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
				UIManager.Instance.ShowModule (UISceneModuleType.DiscountGift);
			}else if (PlatformSetting.Instance.IsOpenCommonGiftType && !PlayerData.Instance.GetCommonGiftIsBuy())
			{
				UIManager.Instance.ShowModule(UISceneModuleType.CommonGift);
			} 
			else if (GlobalConst.StartGameGuideEnabled)
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
	
	public void GetOnClick()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.ButtonClick);
		
		PlayerData.Instance.SetMonthCardGiftRewardsState(true);
		PlayerData.Instance.SetBuyMonthCardDate (System.DateTime.Now.Month);
		
		PlayerData.Instance.AddItemNum(PlayerData.ItemType.Jewel, 400);
		//自定义事件.
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Reward_MonthCard,"State","确认次数","Level",PlayerData.Instance.GetSelectedLevel().ToString());
		
		Hide ();
	}
}
