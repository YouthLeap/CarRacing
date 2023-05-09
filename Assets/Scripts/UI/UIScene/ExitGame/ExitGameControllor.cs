using UnityEngine;
using System.Collections;
using PayBuild;

#if UNITY_EDITOR_OSX || UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 退出游戏框控制类，by有辉
/// </summary>
public class ExitGameControllor : UIBoxBase {

	#region 重写父类方法
	public override void Init ()
	{
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();
	}
	
	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule(UISceneModuleType.ExitGame);
	}

	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	#endregion

	#region Button On Click
	public void CancelOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	public void SureOnClick()
	{
		#if UNITY_EDITOR_OSX || UNITY_EDITOR
		EditorApplication.ExecuteMenuItem("Edit/Play");
		#endif
		PlayerData.Instance.SaveData ();
		Application.Quit();
	}
	#endregion
}