using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UIBoxTween : MonoBehaviour {

	#region 变量
	public bool TweenEnabled = true;

	public _ShowTweenMode ShowTweenMode;
	public Ease ShowEaseType = Ease.OutBack;
	private float ShowDuration = 0.3f;

	public _HideTweenMode HideTweenMode;
	public Ease HideEasyType = Ease.InBack;
	private float HideDuration = 0.3f;

	Vector3 ShowPos = Vector3.zero;
	Vector3 LeftHidePos = new Vector3 (-1000, 0, 0);
	Vector3 RightHidePos = new Vector3 (1000, 0, 0);
	Vector3 TopHidePos = new Vector3 (0, 1000, 0);
	Vector3 BottomHidePos = new Vector3 (0, -1000, 0);
	Vector3 ShowScale = Vector3.one;
	Vector3 HideScale = Vector3.zero;
	Vector3 HorizontalHideScale = new Vector3(0, 1, 0);
	Vector3 VerticalHideScale = new Vector3(1, 0, 0);

	public enum _ShowTweenMode
	{
		LeftToRight,  //从左到右移动
		RightToLeft,
		TopToBottom,
		BottomToTop,
		Enlarge,     //从小变大
		HorizontalEnlarge,  //水平方向上从小变大
		VerticalEnlarge   //垂直方向从小变大
	}

	public enum _HideTweenMode
	{
		LeftToRight,  //从左到右移动
		RightToLeft,
		TopToBottom,
		BottomToTop,
		Shrink,      //从大变小
		HorizontalShrink, //水平方向上从大变小
		VerticalShrink,  //垂直方向从大变小
	}
	#endregion

	#region  显示模块
	public void ShowUIBoxTween()
	{
		if(!TweenEnabled)
		{
			transform.localPosition = ShowPos;
			transform.localScale = ShowScale;
			return;
		}

		DOTween.Kill("Show" + transform.name);
		DOTween.Kill("Hide" + transform.name);

		switch(ShowTweenMode)
		{
		case _ShowTweenMode.LeftToRight:
			transform.localPosition = LeftHidePos;
			transform.localScale = ShowScale;
			MoveShow();
			break;
		case _ShowTweenMode.RightToLeft:
			transform.localPosition = RightHidePos;
			transform.localScale = ShowScale;
			MoveShow();
			break;
		case _ShowTweenMode.TopToBottom:
			transform.localPosition = TopHidePos;
			transform.localScale = ShowScale;
			MoveShow();
			break;
		case _ShowTweenMode.BottomToTop:
			transform.localPosition = BottomHidePos;
			transform.localScale = ShowScale;
			MoveShow();
			break;
		case _ShowTweenMode.Enlarge:
			transform.localPosition = ShowPos;
			transform.localScale = HideScale;
			ScaleShow();
			break;
		case _ShowTweenMode.HorizontalEnlarge:
			transform.localPosition = ShowPos;
			transform.localScale = HorizontalHideScale;
			ScaleShow();
			break;
		case _ShowTweenMode.VerticalEnlarge:
			transform.localPosition = ShowPos;
			transform.localScale = VerticalHideScale;
			ScaleShow();
			break;
		default:
			break;
		}
	}

	void MoveShow()
	{
		if(QualitySetting.IsHighQuality)
			transform.DOLocalMove (ShowPos, ShowDuration).SetEase(ShowEaseType).SetId("Show" + transform.name);
		else
			transform.localPosition = ShowPos;
	}

	void ScaleShow()
	{
		if(QualitySetting.IsHighQuality)
			transform.DOScale (ShowScale, ShowDuration).SetEase(ShowEaseType).SetId("Show" + transform.name);
		else
			transform.localScale = ShowScale;
	}
#endregion

	#region 隐藏模块
	Vector3 hidePos;
	Vector3 hideScale;
	public void HideUIBoxTween()
	{
		if(!TweenEnabled)
		{
			gameObject.SetActive(false);
			return;
		}

		DOTween.Kill("Show" + transform.name);
		DOTween.Kill("Hide" + transform.name);

		switch(HideTweenMode)
		{
		case _HideTweenMode.LeftToRight:
			hidePos = RightHidePos;
			MoveHide();
			break;
		case _HideTweenMode.RightToLeft:
			hidePos = LeftHidePos;
			MoveHide();
			break;
		case _HideTweenMode.TopToBottom:
			hidePos = BottomHidePos;
			MoveHide();
			break;
		case _HideTweenMode.BottomToTop:
			hidePos = TopHidePos;
			MoveHide();
			break;
		case _HideTweenMode.Shrink:
			hideScale = HideScale;
			ScaleHide();
			break;
		case _HideTweenMode.HorizontalShrink:
			hideScale = HorizontalHideScale;
			ScaleHide();
			break;
		case _HideTweenMode.VerticalShrink:
			hideScale = VerticalHideScale;
			ScaleHide();
			break;
		default:
			break;
		}
	}

	void MoveHide()
	{
		if(QualitySetting.IsHighQuality)
		{
			transform.DOLocalMove (hidePos, HideDuration).SetEase(HideEasyType).SetId("Hide" + transform.name).OnComplete(HideCallBack);
		}
		else
		{
			transform.localPosition = hidePos;
			HideCallBack();
		}
	}
	
	void ScaleHide()
	{
		if(QualitySetting.IsHighQuality)
		{
			transform.DOScale (hideScale, HideDuration).SetEase(HideEasyType).SetId("Hide" + transform.name).OnComplete(HideCallBack);
		}
		else
		{
			transform.localScale = hideScale;
			HideCallBack();
		}
	}

	void HideCallBack()
	{
		gameObject.SetActive(false);
	}

#endregion
}
