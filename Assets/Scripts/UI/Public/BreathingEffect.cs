using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BreathingEffect : MonoBehaviour {
	
	private Vector3 originalScale;
	private Vector3 effectedScale;
	private float ScaleValue = 0.1f;
	private float ScaleDuration = 0.8f;
	
	void Awake()
	{
		originalScale = transform.localScale;
		effectedScale = originalScale + new Vector3 (ScaleValue, ScaleValue, ScaleValue);
	}
	
	void OnEnable()
	{
		StartCoroutine (BreathingEffectInit(0.5f));
	}
	
	void OnDisable()
	{
		DOTween.Kill (transform);
	}

	IEnumerator BreathingEffectInit(float time){
		//添加延时
		yield return new WaitForSeconds (time);
		//根据版本类型判断按钮是否有呼吸效果
		if (PlatformSetting.Instance.PayVersionType != PayVersionItemType.ShenHe)
		{
			//根据接口判断按钮是否有呼吸效果
			if (PlayerData.Instance.GetPlayBreathingEffectState()){
				transform.localScale = originalScale;
				transform.DOScale (effectedScale, ScaleDuration).SetLoops (int.MaxValue, LoopType.Yoyo).SetEase (Ease.Linear);
			}
		}
	}
}
