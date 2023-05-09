using UnityEngine;
using System.Collections;
using PathologicalGames;
using System.Collections.Generic;

/// <summary>
/// 编辑器 道具组
/// </summary>
[ExecuteInEditMode]
public class GroupCar : MonoBehaviour {

	public int carCount=2;
	public float preDistance=10f;
	public bool isExecute = false;

	public int colCount=3;
	public float lenZ=10f;
	public float lenX =7f;

	private string[] carPrefabName = {
		"NormalCar1",
		"NormalCar2",
		"NormalCar3",
		"NormalCar4",
		"NormalCar5",
		"NormalNPCCar1",
		"NormalNPCCar2",
		"NormalNPCCar3",
		"NormalNPCCar4",
	};

	private List<Transform> itemList=new List<Transform>();

	static ItemPrefabsCollection _prefabsColletion;
	static ItemPrefabsCollection prefabsCollection{
		get{
			if(_prefabsColletion ==null)
			{
				if(GameObject.Find("ItemPool") != null)
				{
					_prefabsColletion =GameObject.Find("ItemPool").GetComponent<ItemPrefabsCollection>();
				}else
				{
					Debug.LogError("no PrefabsCollection here, please new one ");
				}
			}
			
			return  _prefabsColletion;
		}
		set{
			_prefabsColletion = value;
		}
	}
	
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
		itemList.Clear ();
		//destroy gameobject
		while (transform.childCount>0)
		{
			Transform childTran = transform.GetChild(0);
			DestroyImmediate(childTran.gameObject);
		}


		int createCarCount = 0;

		//create new 
		for (int k=0; k < this.colCount;  k++) {
		    
			int colCount=0;
			for(int j=-2;j<3;j++)
			{

				bool isCreate = Random.value>0.7f;
				if(isCreate && colCount <3  && createCarCount<carCount)
				{
					Transform randCar = GetRandItem();
					randCar.parent = transform;
					colCount++;
					++createCarCount ;

					Vector3 pos  = new Vector3(lenX*j,0,k*lenZ);
					randCar.localPosition = pos;

					itemList.Add(randCar);
				}
			}
		}
	}

	private Transform GetRandItem()
	{
		int randIndex = Random.Range (0, carPrefabName.Length);
		string strName = carPrefabName [randIndex];

		Transform randCar=prefabsCollection.getItemTransformByName(strName);
		return randCar;
	}

	public void PutItemInGroup()
	{
		for (int i=0; i<itemList.Count; ++i) {
			if(itemList[i] != null)
			{
			  itemList[i].parent = transform;
			}
		}
	}
	public void PutItemOutGroup()
	{
		for (int i=0; i<itemList.Count; ++i) {
			if(itemList[i] !=null)
			{
			  itemList[i].parent = transform.parent;
			}
		}
	}
}
