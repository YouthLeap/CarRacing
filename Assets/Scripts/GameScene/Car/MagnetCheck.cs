using UnityEngine;
using System.Collections;

public class MagnetCheck : MonoBehaviour {

	public enum MagnetType{
		Normal,
		Big,
	}

	private BoxCollider mCollider;
	public MagnetType magnetType = MagnetType.Normal;

	void OnTriggerEnter(Collider col)
	{
		//Debug.Log ("enter");
		PropsBase prop;
		if(col.CompareTag("Item"))
		{
			if((prop = col.transform.parent.GetComponent<PropsBase>()) != null)
			{
				//Debug.Log("Magnet");
				prop.UseAbsorbAnim(transform);
			}
		}
		
	}

	public void SetMagnetType(MagnetType type)
	{
		if (mCollider == null)
			mCollider = GetComponent<BoxCollider> ();

		magnetType = type;
		switch(type)
		{
		case MagnetType.Normal:
			mCollider.size = new Vector3 (20,5,20);
			break;
		case MagnetType.Big:
			mCollider.size = new Vector3 (30,5,30);
			break;
		}
	}
}
