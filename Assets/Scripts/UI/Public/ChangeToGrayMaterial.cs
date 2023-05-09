using UnityEngine;
using System.Collections;

public class ChangeToGrayMaterial : MonoBehaviour {

	public Material grayMaterial;

	private MeshRenderer mr;

	void OnEnable()
	{
		if((mr = gameObject.GetComponent<MeshRenderer>()) != null)
		{
			if(grayMaterial != null)
				mr.material = grayMaterial;
		}
	}
}
