using UnityEngine;
using System.Collections;
using PathologicalGames;

public class BehaviourBase : MonoBehaviour {

	protected SpawnPool itemPool;
	[HideInInspector]
	public PropControl selfPropControl, propControl;

	public void Init(PropControl selfPropControl)
	{
		itemPool = PoolManager.Pools["ItemPool"];
		this.selfPropControl = selfPropControl;
	}

	public void Recycle()
	{
		itemPool.Despawn(transform);
	}
}
