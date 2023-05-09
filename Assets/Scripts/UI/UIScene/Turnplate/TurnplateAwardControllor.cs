using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TurnplateAwardControllor : UIBoxBase {

	public static TurnplateAwardControllor Instance;

	public delegate void TurnplateAwardHander();
	private event TurnplateAwardHander TurnplateAwardEvent;

	public GameObject goGetButton;
	public tk2dSprite imageIcon;
	public EasyFontTextMesh textCount;
	public ParticleSystem particle;

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show();
		ShowParticle();	
	}

	void ShowParticle()
	{
		particle.gameObject.SetActive (true);
		particle.Play ();
	}

	void HideParticle()
	{
		particle.Stop ();
		particle.gameObject.SetActive (false);
	}

	public override void Hide ()
	{
		base.Hide();
		UIManager.Instance.HideModule (UISceneModuleType.TurnplateAward);
		HideParticle ();
	}
	
	public override void Back ()
	{
	}
	#endregion

	public void InitData(int AwardID, TurnplateAwardHander callBack)
	{
		int awardCount = TurnplateData.Instance.GetAwardCount(AwardID);
		string awardIcon = TurnplateData.Instance.GetSpriteName(AwardID);
		string awardType = TurnplateData.Instance.GetItemType(AwardID);
		TurnplateAwardEvent = callBack;

		imageIcon.SetSprite(awardIcon);
		textCount.gameObject.SetActive(awardType.CompareTo("HuaFei") != 0);
		textCount.text = "X" + awardCount;
	}

	#region 按钮控制
	public void GetButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
		Hide();
		if(TurnplateAwardEvent != null)
			TurnplateAwardEvent();
	}
	#endregion
}
