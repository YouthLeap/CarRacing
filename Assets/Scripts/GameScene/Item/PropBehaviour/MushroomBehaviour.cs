using UnityEngine;
using System.Collections;

public class MushroomBehaviour : BehaviourBase {

	private float waitTime;
	public void Launch(float skillTime)
	{
		waitTime = skillTime;
		StartCoroutine ("IERecycle");
	}

	IEnumerator IERecycle()
	{
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
	}

	NormalCarControl normalCar = null;
	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("ItemCar")) {
			propControl = col.transform.GetComponent<PropControl> ();
			if (propControl != null && propControl != selfPropControl) {
				propControl.MushroomHit ();
				if(selfPropControl.isPlayer)
				{
					PropTips.Instance.ShowHit(PropType.Mushroom);
				}
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
