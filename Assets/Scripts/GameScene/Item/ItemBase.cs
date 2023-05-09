using UnityEngine;
using System.Collections;
using PathologicalGames;

/// <summary>
/// 所有道具、障碍物的抽象类.
/// </summary>
/// <summary>
/// 所有道路上的物品虚基类，包括障碍物跟道具.
/// </summary>
public  class ItemBase : MonoBehaviour {

	public int id;
	
	protected SpawnPool pool;

	[HideInInspector] public bool isUseAbsorbing = false;
	
	/// <summary>
	/// 此道具是否对玩家已经产生伤害
	/// </summary>
	[HideInInspector] public bool IsHertPlayer = false;
	

	
	/// <summary>
	/// 当人物碰到物品时触发.
	/// </summary>
	public virtual void GetItem(PropControl  propCon= null)
	{}


	
	/// <summary>
	/// 初始化.
	/// </summary>
	public virtual void Init()
	{
		gameObject.SetActive (true);
		if(pool == null)
		{
			pool = PoolManager.Pools["ItemPool"];
		}
	}
	
	/// <summary>
	/// 回收的重置.
	/// </summary>
	public virtual void Reset()
	{
	}
	
	/// <summary>
	/// 使用磁铁吸收的效果.
	/// </summary>
	/// <param name="target">Target.</param>
	public virtual void UseAbsorbAnim(Transform target)
	{
		
	}
	
}


