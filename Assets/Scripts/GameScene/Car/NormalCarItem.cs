using UnityEngine;
using System.Collections;

/// <summary>
/// 场景中的普通障碍车
/// </summary>
public class NormalCarItem : ItemBase {

	private CarMove carMove;
	public int addCoin=0;

	public override void Init ()
	{
		base.Init ();
		if (id != 0) {
			addCoin = PropConfigData.Instance.GetAddCoin(id);
		}
	}

	public void InitNormalCarPos(float moveLen ,float xOffset)
	{
		//Debug.Log(transform.name +"  InitNormalCarPos");

		carMove = GetComponent<CarMove>();
		carMove.startPosLen= moveLen;
		carMove.xOffset= xOffset;
		carMove.StartRun();
		gameObject.SetActive(true);
	}

	public override void GetItem (PropControl propCon)
	{
		return;
	}

	public void AddCoinByID()
	{
		GameData.Instance.curCoin+=addCoin * GameData.Instance.curMulti;
		DropItemManage.Instance.DropCoin(transform.position,addCoin);
	}
}
