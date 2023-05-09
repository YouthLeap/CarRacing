using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HuaFeiManager : UIBoxBase {

	public GameObject goCloseBt, HuaFeiHint, NoHuaFeiHint; 
	public EasyFontTextMesh PhoneNumberText, HuaFeiAmountText; 

	#region 重写父类方法
	public override void Init ()
	{
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();

		if(PlayerData.Instance.GetHuaFeiAmount() != 0)
		{
			HuaFeiHint.SetActive(true);
			NoHuaFeiHint.SetActive(false);
			PhoneNumberText.text = PlayerData.Instance.GetPhoneNumber();
			HuaFeiAmountText.text = PlayerData.Instance.GetHuaFeiAmount().ToString() + "usd";
		}else
		{
			HuaFeiHint.SetActive(false);
			NoHuaFeiHint.SetActive(true);
		}
	}
	
	public override void Hide ()
	{
		base.Hide();
		UIManager.Instance.HideModule(UISceneModuleType.HuaFeiDisplay);
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
