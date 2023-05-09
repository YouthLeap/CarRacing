using UnityEngine;
using System.Collections;

public class BananaBehaviour : BehaviourBase {

	private bool isPlayerCanHit = false;

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("ItemCar")) {
			propControl = col.transform.GetComponent<PropControl> ();
			if (propControl != null) {
				if (propControl == selfPropControl && isPlayerCanHit == false)
					return;
				propControl.SlipHit ();
				if(selfPropControl.isPlayer)
				{
				    PropTips.Instance.ShowHit(PropType.BananaPeel);
				}
				Recycle ();
			}
		}
	}

	void OnEnable()
	{
		StartCoroutine ("IECheck");
	}

	IEnumerator IECheck()
	{
		isPlayerCanHit = false;
		
		float waitTime = 3.0f;
		while(waitTime>0)
		{
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
			waitTime -= Time.deltaTime;
			yield return null;
		}

		isPlayerCanHit = true;
	}
}