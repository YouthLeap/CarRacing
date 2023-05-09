using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 火球道具
/// </summary>
public class FireBallBehaviour : BehaviourBase {
	public float rotateSpeed = 6.0f;
	public List<Transform> fireTransList= new List<Transform>();

	private Transform selfTrans;

	void OnEnable()
	{
		selfTrans= this.transform;
	}

	private float waitTime;
	public void Show(float skillTime)
	{
		gameObject.SetActive (true);
		waitTime = skillTime;
		StartCoroutine ("IERecycle");
	}

	public void Hide()
	{
		StopCoroutine ("IERecycle");
		Recycle ();
		selfPropControl.isFireBall = false;
	}

	IEnumerator IERecycle()
	{
		selfPropControl.isFireBall = true;
		while(waitTime>0)
		{
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
			waitTime -= Time.deltaTime;
			yield return null;
		}
		Recycle ();
		selfPropControl.isFireBall = false;
	}

	void Update () {
		for(int i=0;i<fireTransList.Count;++i)
		{
			fireTransList[i].RotateAround(selfTrans.position,selfTrans.up,rotateSpeed);
		}
	}

	NormalCarControl normalCar = null;
	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("ItemCar")) {
			propControl = col.transform.GetComponent<PropControl> ();
			if (propControl != null && propControl != selfPropControl) {
				propControl.FireBallHit ();
				PropTips.Instance.ShowHit(PropType.FireBall);
			} else if ((normalCar = col.transform.GetComponent<NormalCarControl> ()) != null) {
				if (selfPropControl.isPlayer)
				{
					AudioManger.Instance.PlaySound (AudioManger.SoundName.CarCrashFly);
					normalCar.GetComponent<NormalCarItem>().AddCoinByID();
				}
				normalCar.carMove.ExplodeCar ();
			}
		}
	}
}
