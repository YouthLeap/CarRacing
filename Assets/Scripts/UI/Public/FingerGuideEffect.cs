using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FingerGuideEffect : MonoBehaviour {
	
	public bool IgnoreVisibleConfig = false; // 忽略后台控制，所有版本都显示
	public bool IsShowBgPS = true;
	
	private int FingerLayer, CircleLayer, BgLayer;
	private Transform Finger, CirclePS, BgPS;
	private float Duration = 0.55f;
	private Ease EaseType = Ease.Linear;
	private Vector3 StartPos = new Vector3(70, -81, 0);
	private Vector3 EndPos   = new Vector3(35, -30, 0);
	
	#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
	public bool  Refresh = false;
	void Update()
	{
		if(Refresh)
		{
			StopFingerGuideEffect();
			CirclePS.GetComponent<ParticleSystem>().Stop();
			if(IsShowBgPS)
				BgPS.GetComponent<ParticleSystem>().Stop();
			
			StartFingerGuideEffect();
			CirclePS.GetComponent<ParticleSystem>().Play();
			if(IsShowBgPS)
				BgPS.GetComponent<ParticleSystem>().Play();
			
			Refresh = false;
		}
	}
	#endif
	
	void OnEnable()
	{
		//缓存池初始化，Platfromsetting未初始化
		if(GlobalConst.FirstIn == true)
			return;
		
		if(!(PlatformSetting.Instance.PlayFingerGuide || IgnoreVisibleConfig))
		{
			return;
		}
		
		InitFinger();
		
		StartFingerGuideEffect();
	}
	
	void InitFinger()
	{
		Transform fingerGuideTran;
		Transform temp = transform.Find("FingerGuide");
		if(temp == null){
			fingerGuideTran = ((GameObject)Instantiate(Resources.Load("UIScene/FingerGuide"))).transform;
		}else{
			fingerGuideTran = temp;
		}
		
		fingerGuideTran.gameObject.SetActive (true);
		fingerGuideTran.parent = this.transform;
		fingerGuideTran.name = "FingerGuide";
		fingerGuideTran.localPosition = Vector3.zero;
		SetGOLayer(fingerGuideTran);
		
		switch (this.gameObject.layer) {
		case 5:
			BgLayer = 1;
			CircleLayer = 18;
			FingerLayer = 19;
			break;
		case 11:
			BgLayer = 31;
			CircleLayer = 38;
			FingerLayer = 39;
			break;
		case 12:
			BgLayer = 51;
			CircleLayer = 58;
			FingerLayer = 59;
			break;
		case 13:
			BgLayer = 71;
			CircleLayer = 78;
			FingerLayer = 79;
			break;
		case 14:
			BgLayer = 91;
			CircleLayer = 98;
			FingerLayer = 99;
			break;
		}
		
		Finger = fingerGuideTran.Find("Finger");
		CirclePS = fingerGuideTran.Find("CirclePS");
		BgPS = fingerGuideTran.Find("BgPS");
		if(BgPS != null)
			BgPS.gameObject.SetActive (IsShowBgPS);
		
		MeshRenderer mr = Finger.GetComponent<MeshRenderer>();
		if(mr != null)
			mr.sortingOrder = FingerLayer;
		
		SetParticalLayer (CirclePS, CircleLayer);
		if (BgPS != null && IsShowBgPS) {
			SetParticalLayer (BgPS, BgLayer);
			SetParticalLayer (BgPS.Find ("Particle System (3)"), BgLayer - 8);
		}
	}
	
	void SetGOLayer(Transform tran)
	{
		tran.gameObject.layer = this.gameObject.layer;
		
		for(int i = 0; i < tran.childCount; i ++)
			SetGOLayer(tran.GetChild(i));
	}
	
	void SetParticalLayer(Transform tran, int sortingOrder)
	{
		Renderer r = tran.GetComponent<Renderer>();
		if(r!= null)
			r.sortingOrder = sortingOrder;
		
		for(int i = 0; i < tran.childCount; i ++)
			SetParticalLayer(tran.GetChild(i), sortingOrder);
	}
	
	void OnDisable()
	{
		StopFingerGuideEffect();
	}
	
	void StartFingerGuideEffect()
	{
		DOTween.Kill(Finger);
		
		Finger.localPosition = StartPos;
		Finger.DOLocalMove(EndPos, Duration).SetLoops(-1, LoopType.Yoyo).SetEase(EaseType);
	}
	
	void StopFingerGuideEffect()
	{
		DOTween.Kill(Finger);
	}
}