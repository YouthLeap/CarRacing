using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ComplainControllor : UIBoxBase {

	public GameObject goCloseBtn, goComplainBtn;

	public EasyFontTextMesh PhoneComplainTitle, PhoneComplainNO;//, MailComplainTitle, MailComplainNO;

	#region 重写父类方法
	public override void Init ()
	{
        // PhoneComplainTitle.text = ComplainData.Instance.GetDesc(ComplainData.ComplainType.Phone);
        // PhoneComplainNO.text    = PlatformSetting.Instance.TelephoneNumber;

        //MailComplainTitle.text  = ComplainData.Instance.GetDesc(ComplainData.ComplainType.Email);
        //MailComplainNO.text     = ComplainData.Instance.GetContent(ComplainData.ComplainType.Email);
        base.Init();
	}

	public override void Show ()
	{
		base.Show ();

		CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Complain,"Level", PlayerData.Instance.GetSelectedLevel ().ToString ());
	}
	
	
	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule(UISceneModuleType.Complain);
	}
	
	public override void Back ()
	{	
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	#endregion

	void Complain()
	{
		// Application.OpenURL("tel:" + PhoneComplainNO.text);
	}

	#region 按钮控制

	void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	void ComplainOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Complain();

	}
    #endregion
}
