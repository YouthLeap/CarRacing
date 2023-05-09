using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 道路顶点编辑器
/// </summary>
[CustomEditor(typeof(RoadPoint))]
public class RoadPointEditor : Editor
{

	RoadPoint roadPoint;

	int addIndex=0;
	int removeIndex=0;

	void Start()
	{
		roadPoint = (RoadPoint)target;
		roadPoint.GetPointList();
	}

	public override  void OnInspectorGUI()
	{
		roadPoint = (RoadPoint)target;
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("刷新"))
		{
			roadPoint.GetPointList();
			roadPoint.UpdateSmoothPath();
		}

	
		if(GUILayout.Button("按顺序命名点对象"))
		{
			ReNameChildPoint();
		}

		if(GUILayout.Button("采样点"))
		{
			roadPoint.GetSamplePoint();
		}
		
//		if(GUILayout.Button("put ground"))
//		{
//			//			int childCount = roadPointGroup.childCount;
//			//			for(int i=0;i<childCount;++i)
//			//			{
//			//				Transform pointTran=  roadPointGroup.GetChild(i);
//			//				Ray  ray = new Ray(pointTran.position+ new Vector3(0,1f,0f), -Vector3.up);
//			//				RaycastHit hit;
//			//				if(Physics.Raycast(ray,out hit,100))
//			//				{
//			//					pointTran.position= hit.point;
//			//				}
//			//			}
//			
//			
//			int childCount  = roadPoint.pointTranList.Count;
//			for(int i=0;i<childCount;++i)
//			{
//				Transform pointTran=  roadPoint.transform.GetChild(i);
//				Vector3 pos = pointTran.position;
//				pos.y=0;
//				pointTran.position = pos;
//			}
//		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		//add  Point
		addIndex = EditorGUILayout.IntField("插入点序号",addIndex);
		if(GUILayout.Button("插入点"))
		{
			if(addIndex >0 && addIndex <=roadPoint.transform.childCount)
			{
				Transform newPont = Instantiate(roadPoint.transform.GetChild(addIndex-1));
				newPont.parent = roadPoint.transform;
				newPont.localPosition = roadPoint.transform.GetChild(addIndex-1).localPosition;
				newPont.SetSiblingIndex(addIndex);

				roadPoint.GetPointList();
				roadPoint.UpdateSmoothPath();
				ReNameChildPoint();
				addIndex=0;
			}
		}

		if(GUILayout.Button("插入下一个点"))
		{
			AddNextPoint();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		//remove point
		removeIndex = EditorGUILayout.IntField("删除点序号",removeIndex);
		if(GUILayout.Button("删除点序号"))
		{
			if(removeIndex>0 && removeIndex<= roadPoint.transform.childCount)
			{
				DestroyImmediate(roadPoint.transform.GetChild(removeIndex-1).gameObject);
				roadPoint.GetPointList();
				roadPoint.UpdateSmoothPath();
				ReNameChildPoint();
				removeIndex=0;
			}
		}

		if(GUILayout.Button("检查线路平滑度"))
		{
			CheckSmooth();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		//GUILayout.Label("1.绑定道路对象 RoadModel 2.添加网格碰撞体 3. 校正坐标");

		EditorGUILayout.Space();

		if(GUILayout.Button("(Y轴方向)根据网格碰撞体校正坐标"))
		{
			roadPoint.AdjustPointVetical();
		}

		if(GUILayout.Button("(水平方向)根据网格碰撞体校正坐标"))
		{
			roadPoint.AdjustPointHorizontal();
		}

	}

	void ReNameChildPoint()
	{
		int childCount = roadPoint.transform.childCount;
		for(int i=0;i<childCount;++i)
		{
			roadPoint.transform.GetChild(i).gameObject.name="Point"+(i+1);
		}
	}
	
	void OnSceneGUI()
	{
		roadPoint = (RoadPoint)target;
		List<Transform> pointTranList=  roadPoint.GetPointList();
		if(pointTranList.Count<1)
			return;

		int pointCount=pointTranList.Count;
		if(roadPoint.isLoop)
		{
			pointCount-=1;
		}

		for(int i=0;i<pointCount;++i)
		{
			Handles.BeginGUI();
			//translate waypoint vector3 position in world space into a position on the screen
			var guiPoint = HandleUtility.WorldToGUIPoint(pointTranList[i].position);
			//create rectangle with that positions and do some offset
			var rect = new Rect(guiPoint.x - 50.0f, guiPoint.y - 40, 200, 20);
			//draw box at position with current waypoint name
			GUI.Box(rect, "P" + (i+1) + "  "+pointTranList[i].position.ToString());
			Handles.EndGUI(); //end GUI block

			//draw handles per waypoint
			Handles.color =roadPoint.pointColor;
			Vector3 wpPos = pointTranList[i].position;
			float size = HandleUtility.GetHandleSize(wpPos) * 0.4f;
			Vector3 newPos = Handles.PositionHandle(wpPos, Quaternion.identity);
			Handles.RadiusHandle(Quaternion.identity, wpPos, size / 2);
			
			if (wpPos != newPos)
			{
				Undo.RecordObject(pointTranList[i], "Move Handles");
				pointTranList[i].position = newPos;

				roadPoint.UpdateSmoothPath();
			}
		}

		//roadPoint.UpdateSmoothPath();


	}

	void AddNextPoint()
	{
		Transform newPont = Instantiate(roadPoint.transform.GetChild(roadPoint.transform.childCount-1));
		newPont.parent = roadPoint.transform;
		newPont.localPosition = roadPoint.transform.GetChild(roadPoint.transform.childCount-2).localPosition;
		newPont.SetSiblingIndex(roadPoint.transform.childCount-1);
		
		roadPoint.GetPointList();
		roadPoint.UpdateSmoothPath();
		ReNameChildPoint();
	}

	void OnGUI()
	{
		Event e = Event.current;
		if (e.isKey && e.type == EventType.keyDown  && e.keyCode == KeyCode.P)
		{
			AddNextPoint();
		}
	}

	void CheckSmooth()
	{
		roadPoint.GetPointList();
		roadPoint.UpdateSmoothPath();

		List<Vector3> path = roadPoint.smoothPath;
		for(int i=2;i<path.Count;++i)
		{
			Vector3 v1 = path[i] -path[i-1];
			Vector3 v2 = path[i-1] - path[i-2];

			float d= Vector3.Dot(v1.normalized,v2.normalized);

			//Debug.Log(d);
			if(d<0.8)
			{
				Debug.Log(d);
				Debug.LogError(" wrong path");
			}
		}
	}
}