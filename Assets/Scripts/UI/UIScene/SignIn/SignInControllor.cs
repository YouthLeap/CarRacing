using UnityEngine;
using System.Collections;
using DG.Tweening;
using PathologicalGames;

public class SignInControllor : UIBoxBase {

	public static SignInControllor Instance;

	public GameObject goSignInButton;
	public GameObject goCloseButton;
	public EasyFontTextMesh SignInBtnText;

	public SignInItem nextDaySignInItem;
	public SignInItem toDaySignInItem;

	private System.DateTime nowTime;
	private int SignInCount;//连续签到的天数
	private float DeltaDay;//两次签到相隔的天数


	public override void Init ()
	{
		Instance = this;

		nowTime = System.DateTime.Now;
		string lastDate = PlayerData.Instance.GetLastSignInDate();
		
		//连续签到进度条
		SignInCount = PlayerData.Instance.GetSignInTimes();
		
		if (string.IsNullOrEmpty (lastDate) || SignInCount == 0) {
			GlobalConst.IsSignIn = false;
			SignInCount = 0;
		} else {
			DeltaDay = (float)(nowTime - System.DateTime.Parse(lastDate)).TotalDays;
			if (DeltaDay < 1) {
				GlobalConst.IsSignIn = true;
			} else if (DeltaDay < 2) {
				if (SignInCount == 7) {
					SignInCount = 0;
				}
				GlobalConst.IsSignIn = false;
			} else {
				SignInCount = 0;
				GlobalConst.IsSignIn = false;
			}
		}

		InitData ();
		if (GlobalConst.IsSignIn) {
			goCloseButton.SetActive(true);
			SignInBtnText.text = "Received";
		} else {
			goCloseButton.SetActive(false);
			SignInBtnText.text = "Receive";
		}

		base.Init();

	}

	private void InitData()
	{
		int signInDay = SignInCount;
		if(GlobalConst.IsSignIn)
		{
			SignInCount-=1;
		}

		toDaySignInItem.Init (SignInCount%7 +1);
		nextDaySignInItem.Init ((SignInCount + 1)%7 +1);

		toDaySignInItem.GetedReward (false);
		nextDaySignInItem.GetedReward (false);
	
	}

	public override void Show ()
	{
		base.Show ();
		if (GlobalConst.IsSignIn) {
			goCloseButton.SetActive(true);
			SignInBtnText.text = "Received";
		} else {
			goCloseButton.SetActive(false);
			SignInBtnText.text = "Receive";
		}
	}
	
	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.SignIn);

		if (GlobalConst.IsSignIn && PlayerData.Instance.GetCurrentChallengeLevel () == 3 && PlayerData.Instance.GetUIGuideState (UIGuideType.CharaterDetailGuide)) {
			int[] playerModelStateIds = PlayerData.Instance.GetModelState();
			for(int i=0; i<playerModelStateIds.Length; ++i)
			{
				if(IDTool.GetModelType(playerModelStateIds[i]) == 1 && IDTool.GetModelLevel(playerModelStateIds[i]) == 1)
				{
					UIGuideControllor.Instance.Show (UIGuideType.MainInterfaceUpgradeGuide);
					UIGuideControllor.Instance.ShowBubbleTipByID(2);
					MainInterfaceControllor.Instance.HideFingerGuide ();
					break;
				}
			}
		}
		else if (GlobalConst.IsSignIn && PlayerData.Instance.GetUIGuideState (UIGuideType.LeftRoleGuide) && PlayerData.Instance.GetCurrentChallengeLevel () == 4) {
			UIGuideControllor.Instance.Show (UIGuideType.LeftRoleGuide);
			TranslucentUIMaskManager.Instance.SetLayer (11);
			ActorCameraController1.Instance.SetCameraDepth (4);
			MainInterfaceControllor.Instance.HideFingerGuide ();
		}
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	public void SignInOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		if (GlobalConst.IsSignIn) {
			Hide ();
			return;
		}
		if (DeltaDay >= 1 && DeltaDay <= 2f)
			++ SignInCount;
		else
			SignInCount = 1;
		SignInRewardControllor.Instance.InitData(SignInCount);
		UIManager.Instance.ShowModule(UISceneModuleType.SignInReward);
		
		PlayerData.Instance.SetSignInTimes(SignInCount);
		PlayerData.Instance.SetLastSignInDate(nowTime.ToString());
		GlobalConst.IsSignIn = true;
		
		//自定义事件.
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.SignIn, "The number of days the user has checked in continuously", SignInCount.ToString ());
	}

	public void SetSignIn()
	{
		toDaySignInItem.GetedReward (true);
		SignInBtnText.text = "Received";

		if(GlobalConst.IsSignIn)
		{
			AutoGiftChecker.iEnterMainInterfaceTimes ++ ;
		}
	}

	public void CloseOnClick()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.CloseBtClick);
		Hide ();
	}

}
