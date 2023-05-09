using UnityEngine;
using System.Collections;
using PathologicalGames;

public class BGManager : MonoBehaviour {

	public static BGManager Instance;

	SpawnPool spUIModules;

	Transform tranMainBg, tranLevelBg;

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		spUIModules = PoolManager.Pools["UIModulesPool"];
		InitBG();
	}

	void InitBG()
	{
		tranMainBg = CreateBg ("MainBg");
		tranMainBg.gameObject.SetActive (true);
	}

	Transform CreateBg(string prefabName)
	{
		Transform tranTemp = spUIModules.Spawn (prefabName);
		tranTemp.SetParent (transform, false);
		tranTemp.gameObject.SetActive (false);
		return tranTemp;
	}

	public void Show()
	{
		tranMainBg.gameObject.SetActive (true);
	}

	public void Hide()
	{
		tranMainBg.gameObject.SetActive (false);
	}
}