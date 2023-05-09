using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameResumeControllor : UIBoxBase {

	public EasyFontTextMesh textCountDown;

	public override void Init ()
	{
		base.Init ();

	}

	public override void Show ()
	{
		base.Show ();
		CountDown();
	}

	public override void Hide ()
	{
		base.Hide ();
		gameObject.SetActive (false);
		GameUIManager.Instance.HideModule(UISceneModuleType.GameResume);
	}

	public override void Back ()
	{
	}

	#region 倒计时
	int iCount;
	void CountDown()
	{
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = false;
		textCountDown.gameObject.SetActive(true);
		iCount = 3;
		StartCoroutine("CountDownIE");
	}
	
	IEnumerator CountDownIE()
	{
		textCountDown.text = iCount.ToString();
		textCountDown.transform.localScale = new Vector3(2f, 2f, 2f);
		DOTween.Kill("ResumeCountDown");
		textCountDown.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.8f).SetEase(Ease.OutBack).SetId("ResumeCountDown");
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CountDown);
		yield return new WaitForSeconds(1.5f);
		iCount --;
		if(iCount <= 0)
		{
			CountDownIECall();
		}else
		{
			StartCoroutine("CountDownIE");
		}
	}

	void CountDownIECall()
	{
		GameController.Instance.ResumeGame();
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = true;
		Hide();
	}
	#endregion
}
