using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HistoricRecordManager : UIBoxBase {

	public GameObject goCloseBt;
	public EasyFontTextMesh HuaFeiAmountText; 

	#region 重写父类方法
	public override void Init ()
	{
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();
		Debug.Log("GetHuaFeiAmount : " + PlayerData.Instance.GetHuaFeiAmount());
		
		HuaFeiAmountText.text = PlayerData.Instance.GetHistoricHuaFeiAmount().ToString() + "usd";
	}
	
	public override void Hide ()
	{
		base.Hide();
		UIManager.Instance.HideModule(UISceneModuleType.HistoricHuaFeiDisplay);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	#endregion

	#region Button On Click
	void CloseBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	#endregion
}