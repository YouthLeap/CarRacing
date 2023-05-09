using UnityEngine;
using System.Collections;

public class AwardItemBoxControllor : UIBoxBase {

	public static AwardItemBoxControllor Instance;

	public delegate void AwardHander();
	private event AwardHander AwardEvent;
	
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
		UIManager.Instance.HideModule (UISceneModuleType.AwardItemBox);
		HideParticle ();
	}
	
	public override void Back ()
	{
	}
	#endregion
	
	public void InitData(int AwardID, int AwardCount ,AwardHander callBack=null)
	{
		AwardEvent = callBack;
		string iconName = RewardData.Instance.GetIconName (AwardID);
		imageIcon.SetSprite(iconName);
		textCount.text = "X" + AwardCount;
	}
	
	#region 按钮控制
	public void GetButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CashMachine);
		Hide();
		if(AwardEvent != null)
			AwardEvent();
	}
	#endregion
}
