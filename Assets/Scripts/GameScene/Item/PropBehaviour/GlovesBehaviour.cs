using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 拳套道具的功能
/// </summary>
public class GlovesBehaviour : BehaviourBase {

	public Animation leftGloveAnim, rightGloveAnim;

	void OnEnable()
	{
		leftGloveAnim.Play ();
		rightGloveAnim.Play ();
	}

	void OnDisable()
	{
		leftGloveAnim.Stop ();
		rightGloveAnim.Stop ();
	}

	private void PlayParticle(bool leftGlove = true)
	{
		Vector3 pos;
		if(leftGlove)
			pos = transform.position + transform.right * 3.0f + transform.up;
		else
			pos = transform.position - transform.right * 3.0f + transform.up;
		SceneParticleController.Instance.PlayParticle ("explosion", pos, transform);
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
		selfPropControl.isGlove = false;
	}

	IEnumerator IERecycle()
	{
		selfPropControl.isGlove = true;
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
		selfPropControl.isGlove = false;
	}

	NormalCarControl normalCar = null;
	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("ItemCar")) {
			propControl = col.transform.GetComponent<PropControl> ();
			if (propControl != null && propControl != selfPropControl) {
				propControl.GlovesHit ();
				if (selfPropControl.isPlayer) {
					AudioManger.Instance.PlaySound (AudioManger.SoundName.Bang);
					PlayParticle (selfPropControl.carMove.xOffset > propControl.carMove.xOffset);
				    PropTips.Instance.ShowHit(PropType.Gloves);
				}
			} else if ((normalCar = col.transform.GetComponent<NormalCarControl> ()) != null) {
				normalCar.carMove.ExplodeCar ();
				if (selfPropControl.isPlayer) {
					AudioManger.Instance.PlaySound (AudioManger.SoundName.CarCrashFly);
					PlayParticle (selfPropControl.carMove.xOffset > normalCar.carMove.xOffset);
					PropTips.Instance.ShowHit(PropType.Gloves);
					normalCar.GetComponent<NormalCarItem>().AddCoinByID();
				}
			}
		}
	}
}