using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DialogBox : MonoBehaviour {
	public static DialogBox Instance = null;
	public GameObject goBgImageBtn;
	public EasyFontTextMesh TextInfo;
	private float TimeHideTime = 2f;
	public void Init ()
	{
		Instance = this;
		transform.localPosition = GlobalConst.RightHidePos;
	}
	public void ShowDialog(string text , float time = 2f)
	{
		gameObject.SetActive (true);
		TextInfo.text = text;
		transform.localPosition = Vector3.zero;
//		transform.localPosition = GlobalConst.RightHidePos;
//		transform.DOLocalMoveX (0,0.5f).SetEase(Ease.Linear).SetId("DialogAni");
		TimeHideTime = time;
		StartCoroutine ("TimeHide");
	}
	public void Hide ()
	{
		gameObject.SetActive (false);
//		transform.DOLocalMoveX (GlobalConst.LeftHidePos.x,0.5f).SetEase(Ease.Linear).SetId("DialogAni");
	}
	IEnumerator TimeHide()
	{
		while(TimeHideTime > 0)
		{
			while(GameData.Instance.IsPause)
				yield return null;
			TimeHideTime -= Time.deltaTime;
			yield return null;
		}
		Hide ();
	}

	#region 点击事件
	public void OnButtonClick()
	{
		StopCoroutine ("TimeHide");
		DOTween.Kill ("DialogAni");
		Hide ();
	}
	#endregion

}
