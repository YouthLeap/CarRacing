using UnityEngine;
using System.Collections;

public class SkillItem : MonoBehaviour {

	public void Init()
	{

	}

	public void UpgradeOnClick()
	{
		Debug.Log("升级成功   " + gameObject.name);
	}
}
