using UnityEngine;
using System.Collections;

public class NormalCarControl : MonoBehaviour {

	public CarMove carMove;
	void OnEnable()
	{
		carMove = GetComponent<CarMove>();
	}
}
