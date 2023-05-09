using UnityEngine;
using System.Collections;

public class Tk2dAdditionalMaterial : MonoBehaviour {

	public Material AdditionalMaterial;

	void OnEnable()
	{
		GetComponent<MeshRenderer>().material = AdditionalMaterial;
	}
}
