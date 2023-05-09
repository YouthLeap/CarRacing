using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景加载
/// 道路模型加载
/// 路标加载
/// </summary>
[ExecuteInEditMode]
public class RoadPathManager : MonoBehaviour {

	public static RoadPathManager Instance;

    [Space(10)]
	public bool isTest=true;
    [Space(10)]
	public int curLevel;

	[HideInInspector]
	public  List<Vector3> smoothPointList = new List<Vector3>();
	[HideInInspector]
	public List<Vector3> origPointList = new List<Vector3>();



	public bool isShowDebugLine=true;
	public bool isRefresh=false;

	public string sceneName="";
	public string pathModelName="";
	public int pathId=0;
	public float pathLen;

	[HideInInspector]
	public List<float> pointLenInPath=new List<float>();
	public int wujinPathId=1;

	private Vector3[] origPointArray;
	private Transform roadTrans;
	private GameObject roadModel,sceneModel;

	// Use this for initialization
	void Awake () {
		Instance = this;
		Init();
	}

	void OnEnable()
	{
		Instance = this;
	}

	void Init()
	{
		if(isTest== false)
		{
			if(PlayerData.Instance.IsWuJinGameMode())
			{
				int row = WuJingScenepathData.Instance.GetDataRow();
				wujinPathId = Random.Range(1,row);
				pathModelName = WuJingScenepathData.Instance.GetPathModelName(wujinPathId);
				pathId = WuJingScenepathData.Instance.GetPathID(wujinPathId);
				sceneName = WuJingScenepathData.Instance.GetSceneName(wujinPathId);
			}else
			{
				curLevel = PlayerData.Instance.GetSelectedLevel();
				pathModelName = GameLevelData.Instance.GetRoadModelName(curLevel);
				pathId = int.Parse( GameLevelData.Instance.GetRoadPointID(curLevel) );
				sceneName = GameLevelData.Instance.GetSceneType(curLevel);
			}
		}

		smoothPointList.Clear();
		origPointList.Clear();
		List<Vector3> dataPointList= RoadData.Instance.GetPointList(pathId);
		origPointList.AddRange(dataPointList);
		origPointList.Add(dataPointList[0]);
		origPointArray = origPointList.ToArray();
		smoothPointList = iTweenPath.GetSmoothPath(origPointList);
		CalPahtLen();
	
		if(Application.isPlaying)
		{
			LoadPathModel(this.pathModelName);
			LoadSceneModel(this.sceneName);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(isRefresh)
		{
			Instance = this;
			isRefresh = false;
			Init();
			Debug.Log("refresh road path");
		}

		if(isShowDebugLine)
		{
			for(int i=1;i<smoothPointList.Count;i=i+2)
			{
				Debug.DrawLine(smoothPointList[i-1],smoothPointList[i],Color.red);
			}
		}
	}

/// <summary>
/// 道路模型加载
/// </summary>
/// <param name="modelName">Model name.</param>
	void LoadPathModel(string modelName)
	{
		GameObject resGO=Resources.Load<GameObject>("Road/"+modelName);
		roadModel= Instantiate( resGO);
		roadModel.transform.parent= transform;
		this.roadTrans = roadModel.transform;
	}

	/// <summary>
	/// 场景加载
	/// </summary>
	/// <param name="sceneModelName">Scene model name.</param>
	void LoadSceneModel(string sceneModelName)
	{
		GameObject resGO =Resources.Load<GameObject>("Environment/"+sceneModelName);
		sceneModel = Instantiate(resGO);
		sceneModel.transform.parent = transform;
	}

	public void DestoryResource()
	{
		Destroy(roadModel);
		Destroy(sceneModel);
		Resources.UnloadUnusedAssets();
	}

	#region Path 路径处理

	private void CalPahtLen()
	{
		if(smoothPointList == null || smoothPointList.Count<2)
		{
			Debug.Log("no path");
			return;
		}
		pointLenInPath.Clear();
		pointLenInPath.Add(0);
		pathLen=0;
		for(int i=1;i<smoothPointList.Count;++i)
		{
			float d = Vector3.Distance(smoothPointList[i-1],smoothPointList[i]);
			pathLen+=d;
			pointLenInPath.Add(pathLen);
		}
	}

	public Vector3 GetPointByLen(float len, float offset=0)
	{
	     while(len>pathLen)
		{
			len-=pathLen;
		}

		int pointCount=smoothPointList.Count;
		int startIndex=0;
		int endIndex=smoothPointList.Count-1;
		int middleIndex= (startIndex+endIndex)/2;

		while(endIndex - startIndex >1)
		{
			if(len>=pointLenInPath[middleIndex])
			{
				startIndex=middleIndex;
				middleIndex=  (startIndex+endIndex)/2;
			}else
			{
				endIndex= middleIndex;
				middleIndex= (startIndex+endIndex)/2;
			}
		}

		float delLen=len - pointLenInPath[startIndex];
		float delPercent = delLen / ( pointLenInPath[(startIndex+1)%pointCount] - pointLenInPath[startIndex] );

		Vector3 pos = Vector3.Lerp(smoothPointList[startIndex] ,smoothPointList[(startIndex+1)%pointCount], delPercent);

		if(offset !=0)
		{
			Vector3 forward= smoothPointList[(startIndex+1)%pointCount] - smoothPointList[startIndex];
			Vector3 offsetVec =( Vector3.Cross(forward,Vector3.up)).normalized;
			pos = pos+ offsetVec*offset;
		}

		return pos;
	}

	public Vector3 GetPointByPercent(float percent)
	{
		if(percent >1)
			percent=1f;

		float len = pathLen*percent;
		Vector3 pos = GetPointByLen(len);
		return pos;
	}

	#endregion

	#region path

	void AddMeshCollider()
	{
		if(this.roadTrans == null)
		{
			Debug.Log("No Road Model");
			return;
		}
		List<Transform> list= new List<Transform>();
		list.Add(roadTrans);
		while(list.Count>0)
		{
			Transform curTran = list[0];
			if(curTran.GetComponent<MeshRenderer>() != null  && curTran.GetComponent<MeshCollider>()==null)
			{
				MeshCollider col= curTran.gameObject.AddComponent<MeshCollider>();
				col.convex=true;
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
		if(roadTrans== null)
		{
			Debug.Log("No Road Model");
			return;
		}
		List<Transform> list= new List<Transform>();
		list.Add(roadTrans);
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
	/// 水平方向上自动对齐顶点
	/// </summary>
	public void AdjustPointHorizontal()
	{
		for(int i=1;i<smoothPointList.Count;++i)
		{
			Vector3 firstPos =smoothPointList[i];
			Vector3 lastPos = smoothPointList[i-1];
			
			Vector3 forwarVec= firstPos - lastPos;
			Vector3 horizontalVec = Vector3.Cross(forwarVec,Vector3.up);
			
			Vector3 leftPos = horizontalVec.normalized*60f + firstPos;
			Vector3 leftVec = firstPos -leftPos;
			RaycastHit leftHit;
			bool isLeftCheck= Physics.Raycast(leftPos,leftVec,out leftHit);
			//Debug.DrawRay(leftPos,leftVec,Color.grey,100f);
			
			Vector3 rightPos = -horizontalVec.normalized*60f +firstPos;
			Vector3 rightVec = firstPos - rightPos;
			RaycastHit rightHit;
			bool isRightCheck = Physics.Raycast(rightPos,rightVec,out rightHit);
			//Debug.DrawRay(rightPos,rightVec,Color.cyan,100f);
			
			if(isLeftCheck && isRightCheck )
			{
				Vector3 targetPos = (leftHit.point + rightHit.point) *0.5f;
				smoothPointList[i] = targetPos;
			}else
			{
				Debug.Log("Point check Error");
			}
		}
	}
	#endregion
}
