using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 攻击赛车的飞弹
/// 在赛车旁边出现 飞向赛车
/// </summary>
public class FlyBmobBehaviour : BehaviourBase {

	private bool isFlying = false;
	private Vector3 startPos, endPos;
	private float flyTime, startTime;

	public void Launch(Vector3 startPos, Vector3 endPos)
	{
		this.startPos = startPos;
		this.endPos = endPos;
		this.flyTime = 1.0f;
		
		this.startTime = Time.time;

		isFlying = true;
	}

	void FixedUpdate () {
		if (!isFlying)
			return;
		
		float percent = (Time.time - startTime) / flyTime;
		transform.position = Vector3.Lerp (startPos, endPos, percent);
		transform.LookAt (endPos);
		
		if (percent >= 1) {
			Explore ();
		}
	}

	private float effectRadius = 20;
	NormalCarControl normalCar = null;
	private void Explore()
	{
		isFlying = false;

		SceneParticleController.Instance.PlayParticle ("FX_baozha02_hg",transform.position + Vector3.up * 0.3f);
		AudioManger.Instance.PlaySound(AudioManger.SoundName.MissileExplode);
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);
		for (int i = 0; i < hitColliders.Length; i++) {
			if (!hitColliders[i].CompareTag ("ItemCar"))
				continue;
			propControl = hitColliders [i].transform.GetComponent<PropControl> ();
			normalCar = hitColliders [i].transform.GetComponent<NormalCarControl> ();
			if (propControl != null && propControl != selfPropControl && propControl.carMove.isRunning) {
				propControl.FlyBombHit ();
			} else if (normalCar != null && normalCar.carMove.isRunning) {
				normalCar.carMove.ExplodeCar ();
				if(selfPropControl.isPlayer)
				{
					normalCar.GetComponent<NormalCarItem>().AddCoinByID();
				}
			}
		}

		Recycle ();
	}
}