using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 编辑器 道具组
/// </summary>
[ExecuteInEditMode]
public class GroupItem : MonoBehaviour {


   #if UNITY_EDITOR

	public int count=1;
	public float preDistance=10f;
	public bool isExecute = false;
	
	public float lenZ=10f;
	public float lenX =7f;


	private float maxZ = 0;

	private List<Transform> itemList=new List<Transform>();


	private List<string> pathList=new List<string>();


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isExecute) {
			isExecute =false;
		
			CreateRandItem();
		}
		
	}

	
	public void CreateRandItem()
	{
		ReflashItem();

		itemList.Clear ();
		//destroy gameobject
		while (transform.childCount>0) {
			Transform childTran = transform.GetChild (0);
			DestroyImmediate (childTran.gameObject);
		}

		maxZ = 0;

		int colCount = 0;
		for (int i=-2; i<3; ++i) {
		   
			bool isCreate = Random.value >0.6f;
			if(isCreate && colCount <count)
			{
				Transform group = GetGrupItem();
				group.parent = transform;

				Vector3 pos = Vector3.zero;
				pos.x = i*lenX;
				group.localPosition = pos;

				colCount++;

				for(int k=0;k<group.childCount;++k)
				{
					itemList.Add(group.GetChild(k));

					if(group.GetChild(k).localPosition.z >maxZ)
					{
						maxZ= group.GetChild(k).localPosition.z;
					}
				}

			}
		}

		lenZ = maxZ;
	}

	private Transform GetGrupItem()
	{
	
		int index = Random.Range (0, pathList.Count);

		Debug.Log (index +"   "+ pathList.Count );
		string str = pathList [index];

		Transform trans = AssetDatabase.LoadAssetAtPath<Transform>(str);
		Transform newTran = Instantiate (trans);
		//Debug.Log (str );
		return newTran;

	}

	public void PutItemInGroup()
	{
		for (int i=0; i<itemList.Count; ++i) {
			if(itemList[i] !=null)
			{
			   itemList[i].parent = transform;
			}
		}
	}
	public void PutItemOutGroup()
	{
		for (int i=0; i<itemList.Count; ++i) {
			if(itemList != null)
			{
			  itemList[i].parent = transform.parent;
			}
		}
	}


	private void ReflashItem()
	{
		pathList.Clear ();
		string itemPath=Application.dataPath + "/" +"TT";
		string[] directoryEntries;
		try
		{
			directoryEntries = System.IO.Directory.GetFileSystemEntries(itemPath);
			for(int i = 0; i < directoryEntries.Length ; i ++)
			{  
				string p = directoryEntries[i];  
				if(p.EndsWith(".meta"))
					continue;
				p = p.Substring(p.IndexOf("Assets"));
				pathList.Add(p);
			}
			
		}
		catch
		{
			
		}
	}

#endif
}
