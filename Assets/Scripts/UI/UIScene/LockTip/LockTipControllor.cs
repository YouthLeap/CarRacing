using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum LockTipType
{
	LockTip,
	UnlockTip
};
public class LockTipControllor : UIBoxBase
{
	public static LockTipControllor Instance;

	public EasyFontTextMesh tipText;

	private string tipStr1 = "Turn off on the seventh，\nabout you come to war！";
	private string tipStr2 = "Has been opened，\nimmediately challenge！";
		
	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}

	public void InitData(LockTipType tipType)
	{
		switch (tipType) {
		case LockTipType.LockTip:
			tipText.text = tipStr1;
			break;
		case LockTipType.UnlockTip:
			tipText.text = tipStr2;
			break;
		}
	}
		
	public override void Show ()
	{
		base.Show ();
		transform.localPosition = GlobalConst.LeftHidePos;
		DOTween.Kill (transform);
		if (QualitySetting.IsHighQuality)
			transform.DOLocalMove (ShowPosV2, GlobalConst.PlayTime).SetEase (Ease.OutBack);
		else
			transform.localPosition = ShowPosV2;
	}
		
	public override void Hide ()
	{
		base.Hide ();
		DOTween.Kill (transform);
		if (QualitySetting.IsHighQuality)
			transform.DOLocalMove (GlobalConst.LeftHidePos, GlobalConst.PlayTime).SetEase (Ease.OutBack);
		else
			transform.localPosition = GlobalConst.LeftHidePos;
		UIManager.Instance.HideModule (UISceneModuleType.LockTip);
	}
		
	public override void Back ()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.ButtonClick);
		Hide ();
	}
	#endregion
		
	#region 按钮控制
	public void CloseOnClick ()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.CloseBtClick);
		Hide ();
	}
	#endregion
}