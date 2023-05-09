using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
public class RoadPoint : MonoBehaviour {

	public Color pointColor;
	[HideInInspector]
	public bool isLoop=true;
	public bool isShowLine=true;

	public List<Transform> pointTranList = new List<Transform>();

	[HideInInspector]
	public  List<Vector3> smoothPath= new List<Vector3>();

	public Transform roadModel;

	public float sampleDis=10f;

	// Use this for initialization
	void Start () {
		GetPointList();
	}

	public List<Transform> GetPointList()
	{
		pointTranList.Clear();

		if(transform.childCount <1)
			return pointTranList;


		for(int i=0;i<this.transform.childCount;++i)
		{
			pointTranList.Add(transform.GetChild(i));
		}

		if(isLoop)
		{
			pointTranList.Add(transform.GetChild(0));
		}

		return pointTranList;
	}

	public void UpdateSmoothPath()
	{
		List<Vector3> posList = new List<Vector3>();
		for(int i=0;i<pointTranList.Count;++i)
		{
			posList.Add(pointTranList[i].position);
		}
		//posList.Add(pointTranList[0].position);

		smoothPath= iTweenPath.GetSmoothPath(posList);
		
	}

	void Update()
	{
		for(int k=1;k<smoothPath.Count;++k)
		{
			Debug.DrawLine(smoothPath[k-1],smoothPath[k],Color.blue);
		}
	}

	/// <summary>
	/// 坐标点自动对齐
	/// </summary>
	public void AdjustPointVetical()
	{
		AddMeshCollider(true);

		for(int i=0;i<pointTranList.Count;++i)
		{
			Transform curTrans = pointTranList[i];

			RaycastHit upHit;
			bool isUpHit= Physics.Raycast(curTrans.position,Vector3.up,out upHit);

			RaycastHit downHit;
			bool isDownHit = Physics.Raycast(curTrans.position,-Vector3.up,out downHit);

			Vector3 adjustPos=curTrans.position;
			if(isUpHit && isDownHit)
			{
				float disUp = Vector3.Distance(curTrans.position,upHit.point);
				float disDown = Vector3.Distance(curTrans.position, downHit.point);
				adjustPos = disUp>disDown?downHit.point:upHit.point;
			}else if(isUpHit)
			{
				adjustPos = upHit.point;
			}else if(isDownHit)
			{
				adjustPos = downHit.point;
			}
			curTrans.position = adjustPos;
		
		}
		UpdateSmoothPath();

		RemoveMeshCollider();
	}

	/// <summary>
	/// 水平方向上自动对齐顶点
	/// </summary>
	public void AdjustPointHorizontal()
	{
		AddMeshCollider();

		for(int i=1;i<this.pointTranList.Count;++i)
		{
			Vector3 firstPos = pointTranList[i].position;
			Vector3 lastPos = pointTranList[i-1].position;

			Vector3 forwarVec= firstPos - lastPos;
			Vector3 horizontalVec = Vector3.Cross(forwarVec,Vector3.up);

			Vector3 leftPos = horizontalVec.normalized*60f + firstPos;
			Vector3 leftVec = firstPos -leftPos;
			RaycastHit leftHit;
			bool isLeftCheck= Physics.Raycast(leftPos,leftVec,out leftHit);
			Debug.DrawRay(leftPos,leftVec,Color.grey,100f);

			Vector3 rightPos = -horizontalVec.normalized*60f +firstPos;
			Vector3 rightVec = firstPos - rightPos;
			RaycastHit rightHit;
			bool isRightCheck = Physics.Raycast(rightPos,rightVec,out rightHit);
			Debug.DrawRay(rightPos,rightVec,Color.cyan,100f);

			if(isLeftCheck && isRightCheck )
			{
				Vector3 targetPos = (leftHit.point + rightHit.point) *0.5f;
				pointTranList[i].position = targetPos;
			}else
			{
				Debug.Log("Point check Error");
			}
		}
		UpdateSmoothPath();

		RemoveMeshCollider();
	}

	void AddMeshCollider(bool isConvex=false)
	{
		if(roadModel == null)
		{
			Debug.Log("No Road Model");
			return;
		}
		List<Transform> list= new List<Transform>();
		list.Add(roadModel);
		while(list.Count>0)
		{
			Transform curTran = list[0];
			if(curTran.GetComponent<MeshRenderer>() != null  && curTran.GetComponent<MeshCollider>()==null)
			{
				MeshCollider col= curTran.gameObject.AddComponent<MeshCollider>();
				col.convex=isConvex;
			}
			
			list.RemoveAt(0);
			
			for(int i=0;i<curTran.childCount;++i)
			{
				list.Add(curTran.GetChild(i));
			}
		}
	}
	
	void RemoveMeshCollider()
	{
		if(roadModel == null)
		{
			Debug.Log("No Road Model");
			return;
		}
		List<Transform> list= new List<Transform>();
		list.Add(roadModel);
		while(list.Count>0)
		{
			Transform curTran = list[0];
			MeshCollider col = curTran.GetComponent<MeshCollider>();
			if(col != null  )
			{
				DestroyImmediate(col);
			}
			
			list.RemoveAt(0);
			
			for(int i=0;i<curTran.childCount;++i)
			{
				list.Add(curTran.GetChild(i));
			}
		}
	}

	/// <summary>
	/// Gets the sample point.
	/// </summary>
	public void GetSamplePoint()
	{
		while(transform.childCount>0)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}

		GameObject pointStartGO = new GameObject("Point"+0);
		pointStartGO.transform.parent= transform;
		pointStartGO.transform.position = smoothPath[0];

		int part=1;
		float curLen=0;
		for(int i=1;i<smoothPath.Count;++i)
		{
			curLen+=Vector3.Distance(smoothPath[i-1],smoothPath[i]);
			if(curLen>part*this.sampleDis)
			{
				++part;
				GameObject pointGO = new GameObject("Point"+i);
				pointGO.transform.parent= transform;
				pointGO.transform.position = smoothPath[i];
			}
		}

		GetPointList();
		UpdateSmoothPath();
	}
	
}
