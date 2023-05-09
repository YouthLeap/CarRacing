using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DailyMissionRewardControllor : UIBoxBase {
	
	public static DailyMissionRewardControllor Instance;
	
	public tk2dSprite imageIcon;
	public EasyFontTextMesh textCount;
	
	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show ();
	}

	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.DailyMissionReward);
	}
	
	public override void Back ()
	{
	}
	#endregion
	
	public void InitData(string iconName, int rewardCount)
	{		
		imageIcon.SetSprite(iconName);
		textCount.text = "x" + rewardCount;
	}
	
	#region 按钮控制
	public void GetButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
		Hide();
	}
	#endregion
}