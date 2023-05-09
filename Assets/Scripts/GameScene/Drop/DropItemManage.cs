using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;


/// <summary>
/// 游戏中标签管理
/// </summary>
public class DropItemManage : MonoBehaviour {

	public static DropItemManage Instance = null;

	private SpawnPool dropItemPool;
	private List<Transform> dropItemList;

	private Transform uiCanva;
	private Camera uiCamera;
	private Camera mainCamera;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		dropItemPool = PoolManager.Pools["DropItemPool"];	

		uiCanva = GameObject.Find ("UICanvas").transform;
		uiCamera = GameObject.Find ("UICamera").GetComponent<Camera>();
		mainCamera = GameObject.Find ("MainCamera").GetComponent<Camera> ();
	}

	
	/// <summary>
	/// 场景中掉落金币
	/// </summary>
	/// <param name="worldPos">World position.</param>
	/// <param name="Count">Count.</param>
	public void DropCoin(Vector3 worldPos,int Count=0)
	{
		
		Vector3 uiPos = GetUIPos(worldPos);
		for(int i = 0; i < Count; i++)
		{
			Transform coinTran = dropItemPool.Spawn ("DropCoinItem");
			coinTran.parent = uiCanva;
			coinTran.localPosition = uiPos;
			coinTran.GetComponent<DropCoinItem> ().ShowOneAndMoveToBar();
		}

	}

	public void ShowWeedOut(int playerID)
	{
		string nameStr= ModelData.Instance.GetName(playerID);

		Transform labelTran = dropItemPool.Spawn ("ShowLabel");
		labelTran.parent = uiCanva;
		labelTran.GetComponent<ShowLabel> ().Show ("Passed " + nameStr);
	}

	public void ShowCircleCount(int count)
	{
		Transform labelTran = dropItemPool.Spawn ("ShowLabel");
		labelTran.parent = uiCanva;
		labelTran.GetComponent<ShowLabel> ().Show (""+count+"");
	}

	public void ShowGetColorEgg()
	{
		Transform labelTran = dropItemPool.Spawn ("ShowLabel");
		labelTran.parent = uiCanva;
		labelTran.GetComponent<ShowLabel> ().Show ("Egg treasure");
	}
	
	public void Recycle(Transform item)
	{
		item.SetParent(dropItemPool.transform);
		dropItemPool.Despawn(item);
	}


	Vector3 GetUIPos(Vector3 worldPos)
	{
		Vector3 screenPos = mainCamera.WorldToScreenPoint (worldPos);
		screenPos.z = 0;
		Vector3 uiPos = uiCamera.ScreenToWorldPoint (screenPos);
		uiPos.z=0f;
		return uiPos;
	}

	public int GetRandomPropId(bool isPlayer)
	{
		int startIndex = (int)PropType.ShapeShift;
		int endIndex = (int)PropType.SpeedAdd + 1;
		float rateSum = 0;
		int randomValue = Random.Range (1, 101);
		for (int i=startIndex; i<endIndex; ++i) {
			if (isPlayer)
				rateSum += PropConfigData.Instance.GetPropRate (i) * 100;
			else
				rateSum += PropConfigData.Instance.GetNPCPropRate (i) * 100;
			if(randomValue <= rateSum)
				return i;
		}
		return startIndex;
	}
}