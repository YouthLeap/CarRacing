using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

/// <summary>
/// Create opponet car manager.
/// </summary>
public class CreateOpponetCarManager : MonoBehaviour {

	public static CreateOpponetCarManager Instance;

	public int carGroupID;

	private SpawnPool itemPool;
	
	void Awake()
	{
		Instance = this;
	}
	
	// Use this for initialization
	void Start () {
		itemPool = PoolManager.Pools["ItemPool"];
		
		CreateByID(carGroupID);
	}
	
	public void CreateByID(int Id)
	{
		List<ItemContentStru> list = ItemGroupData.Instance.GetContentList (Id);
		for (int i=0; i<list.Count; ++i) {
			list[i].parent =transform;
		}
		
		for(int k=0;k<list.Count;++k)
		{
			ItemContentStru itemInfo = list[k];
			Transform itemTran = itemPool.Spawn( itemInfo.prefabName);
			itemTran.parent = itemInfo.parent;
			
			Vector3 curvePos = RoadPathManager.Instance.GetPointByLen(itemInfo.pos.z,itemInfo.pos.x);
			Vector3 curveHead=  RoadPathManager.Instance.GetPointByLen(itemInfo.pos.z+1f ,itemInfo.pos.x);
			Vector3 direcetion = curveHead - curvePos;
			
			itemTran.localPosition = curvePos;
			itemTran.forward = direcetion;
			
			CarMove carMoveScript = itemTran.GetComponent<CarMove>();
			carMoveScript.startPosLen=itemInfo.pos.z;
			carMoveScript.moveLen = itemInfo.pos.z;
			carMoveScript.xOffset = itemInfo.pos.x;

			itemTran.GetComponent<OpponenetCarControl>().Init();
		}
		
	}
}
