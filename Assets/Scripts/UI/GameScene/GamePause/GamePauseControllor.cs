using UnityEngine;
using System.Collections;

public class GamePauseControllor : UIBoxBase {

	#region 重写父类方法
	public override void Init ()
	{
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();
		transform.localPosition = ShowPosV2;
		GameController.Instance.PauseGame();
	}
	
	public override void Hide ()
	{
		gameObject.SetActive (false);
		transform.localPosition = GlobalConst.LeftHidePos;
		GameUIManager.Instance.HideModule(UISceneModuleType.GamePause);

		GameUIManager.Instance.ShowModule(UISceneModuleType.GameResume);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide ();
	}
	#endregion
	
	#region 按钮控制
	void ContinueBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	void RestartBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		if(PlayerData.Instance.GetItemNum(PlayerData.ItemType.Strength) > 0)
		{
			PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Strength, 1);
			GameController.Instance.RestartGame();
		}
		else
		{
			GiftPackageControllor.Instance.Show(PayType.PowerGift);
		}
	}
	
	void BackMainUIBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		//自定义事件.
		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Level_Quit, "关卡模式", PlayerData.Instance.GetGameMode(), "选择关卡", "Level_" + PlayerData.Instance.GetSelectedLevel());

		GlobalConst.ShowModuleType = UISceneModuleType.MainInterface;
		GlobalConst.SceneName = SceneType.UIScene;
		LoadingPage.Instance.LoadScene ();
	}
	#endregion
}
