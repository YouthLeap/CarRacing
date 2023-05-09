using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using PathologicalGames;


/// <summary>
/// 道具 障碍物 关卡编辑器
/// shenzhan
/// </summary>
public class LevelEditorWindow :  EditorWindow {

	#region 编辑器固有的变量 Transform
	
	static Transform _itemGroup;
	static Transform itemGroup
	{
		get{
			if(_itemGroup ==null)
			{
				if(GameObject.Find("ItemGroup") != null)
				{
					_itemGroup =GameObject.Find("ItemGroup").transform;
				}else
				{
					GameObject GO= new GameObject();
					GO.name ="ItemGroup";
					_itemGroup=GO.transform;
				}
			}
			
			return  _itemGroup;
		}
		
		set{_itemGroup =value;}
	}

	static Transform _roadPointGroup;
	static Transform roadPointGroup
	{
		get{
			if(_roadPointGroup ==null)
			{
				if(GameObject.Find("roadPoint") != null)
				{
					_roadPointGroup =GameObject.Find("roadPoint").transform;
				}else
				{
					GameObject GO= new GameObject();
					GO.AddComponent<RoadPoint>();
					GO.name ="roadPoint";
					_roadPointGroup=GO.transform;
				}
			}
			
			return  _roadPointGroup;
		}
		
		set{_roadPointGroup =value;}
	}


	static Transform _editorRoadGroup;
	static Transform editorRoadGroup
	{
		get{
			if(_editorRoadGroup ==null)
			{
				if(GameObject.Find("EditorRoadGroup") != null)
				{
					_editorRoadGroup =GameObject.Find("EditorRoadGroup").transform;
				}else
				{
					GameObject GO= new GameObject();
					GO.name ="EditorRoadGroup";
					_editorRoadGroup=GO.transform;
				}
			}
			
			return  _editorRoadGroup;
		}
		
		set{_editorRoadGroup =value;}
	}

	
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
	
	static LevelEditorWindow window;


	private float[] Lanes = {-6.6f,-3.3f,0f,3.3f,6.6f};
	
	#endregion

	[MenuItem ("Window/LevelEditorWindow &zx", false)]

	static void Init () {
		window = (LevelEditorWindow)EditorWindow.GetWindow (typeof (LevelEditorWindow), true,"关卡编辑");

	}
	
	#region   基础funtion

	void OnEnable()
	{
		InitItemPrefabsData ();

		InitLevelData ();

		InitGroupEditor ();

		InitRaodData();
	}



	void OnGUI()
	{
		DrawTab ();

		if (curTab == TabType.ItemTab) {
			DrawItemEditor ();
		} else if (curTab == TabType.LevelTab) {
			DrawLevelEditor ();
		} else if (curTab == TabType.Group) {
		   DrawGroupEditor();
		}else if(curTab == TabType.Road)
		{
			DrawRoadEditor();
		}

	}


    enum TabType{ItemTab,Group,Road,LevelTab};
	TabType curTab=TabType.ItemTab;

    int selectIndex = 0;
    string[] selectEditorFunction = {"Item编辑", "道具组", "路标编辑", "关卡编辑"};


	/// <summary>
	/// Draws the tab.
	/// </summary>
	void DrawTab()
	{
		EditorGUILayout.BeginHorizontal ();
//		if (GUILayout.Button ("Item编辑")) {
//			InitItemPrefabsData ();
//			curTab = TabType.ItemTab;
//		}
//
//		
//		if (GUILayout.Button ("道具组")) {
//			InitGroupEditor();
//			curTab = TabType.Group;
//		}
//
//		if (GUILayout.Button ("路标编辑")) {
//			InitRaodData();
//			curTab = TabType.Road;
//		}
//
//		if (GUILayout.Button ("关卡编辑")) {
//			InitLevelData();
//			curTab = TabType.LevelTab;
//		}

        int selectIndexTemp = GUILayout.SelectionGrid(selectIndex, selectEditorFunction, selectEditorFunction.Length);

        if (selectIndexTemp != selectIndex)
        {
            switch (selectIndexTemp)
            {
                case 0:
                    InitItemPrefabsData ();
                    break;
                case 1:
                    InitGroupEditor ();
                    break;
                case 2:
                    InitRaodData ();
                    break;
                case 3:
                    InitLevelData ();
                    break;
            }
        }
        selectIndex = selectIndexTemp;
        curTab = (TabType)selectIndex;

		EditorGUILayout.EndHorizontal ();
	}

	#endregion



	#region 公用 函数
	
	/// <summary>
	/// 获取ItemGroup 下Item对象
	/// </summary>
	/// <returns>The item trans list.</returns>
	List<Transform> GetItemTransList()
	{
		List<Transform> list = new List<Transform> ();
		for (int i=0; i<itemGroup.transform.childCount; ++i) {
			if( itemGroup.GetChild(i).GetComponent<ItemBase>() != null )
			{
				list.Add(itemGroup.GetChild(i));
			}
		}
		return list;
		
	}

	/// <summary>
	/// 对物品的列表进行排序 按照坐标Z 的大小
	/// </summary>
	/// <param name="list">List.</param>
	void SortList(List<Transform> list)
	{
		int listCount = list.Count;
		for(int n=0;n<listCount-1;++n)
		{
			float min = list[n].localPosition.z;
			int index =n;
			for(int m= (n+1); m<listCount;++m)
			{
				if(list[m].localPosition.z < min )
				{
					min = list[m].localPosition.z;
					index = m;
				}
			}
			if(index != n)
			{
				Transform temp =list[n];
				list[n] = list[index];
				list[index]= temp;
			}	
		}
	}


	void XYAlign()
	{
		Transform[] trans = Selection.GetTransforms(SelectionMode.TopLevel);
		
		for(int m=0;m <trans.Length;++m)
		{
			Transform tranChild = trans[m];
			Vector3 firstPos = tranChild.localPosition;
			
			//set y
			if(tranChild.GetComponent<PropsBase>()!=null)
			{
				firstPos.y = 1f;
			}
		
			
			//set x
			float curx=0f;
			float disX =float.MaxValue;
			for(int i=0;i<Lanes.Length;++i)
			{
				if(Mathf.Abs(firstPos.x - Lanes[i])  <disX)
				{
					disX =Mathf.Abs(firstPos.x - Lanes[i]) ;
					curx = Lanes[i];
				}
			}
			firstPos.x = curx;
			
			
			trans[m].localPosition = firstPos;
		}
	}

	#endregion


	#region 物品编辑
	bool pickPropShow= true;
	string pickPropStatus = "可捡道具";
	
	bool roadPropShow= false;
	string roadPropStatus = "道路道具";
	
	
	bool trafficShow = false;
	string trafficStatus="车";
	
	bool obstracleShow = false;
	string obstracleStatus="路障";
	
	bool otherShow = false;
	string otherStatus = "其他";
	
	
	//Item 预设信息的List
	List<EditorItemStru> pickupPropsList= new List<EditorItemStru>();
	List<EditorItemStru> roadPropsList = new List<EditorItemStru>();
	List<EditorItemStru> trafficList =new List<EditorItemStru>();
	List<EditorItemStru> obstracList =new List<EditorItemStru>();
	List<EditorItemStru> otherList =new List<EditorItemStru>();
	
	
	
	
	//config data
	string itemId;
	string itemNote;
	string itemPosConfiContent="";
	
	
	//show item group 
	string inputItemId;
	
	Vector2 itemScrollViewPos;

	List<EditorItemGroupStru> itemGroupList = new List<EditorItemGroupStru> ();

	Vector2 itemEditorScrollPos;


	int editorRoadCount=0;


//	CreateRoadManager.SceneRoadType itemRoadType= CreateRoadManager.SceneRoadType.City;

	/// <summary>
	/// 加载数据
	/// </summary>
	void InitItemPrefabsData()
	{
		
		pickupPropsList.Clear ();
		roadPropsList.Clear ();
		trafficList.Clear ();
		obstracList.Clear ();
		otherList.Clear ();
		
		ItemData.Instance.RefreshData ();
		int ItemDataCount = ItemData.Instance.GetDataRow ();
		
		for (int i=1; i<=ItemDataCount; ++i) {
			
			EditorItemStru itemtemp= new EditorItemStru();
			itemtemp.id=i;
			itemtemp.editName=ItemData.Instance.GetEditName(i);
			itemtemp.prefabName=ItemData.Instance.GetPrefableName(i);
			itemtemp.itemType= ItemData.Instance.GetPrefableItemType(i);

			
			switch(itemtemp.itemType)
			{
			case EditItemType.PickupProp:
				pickupPropsList.Add(itemtemp);
				break;
			case EditItemType.RoadProp:
				roadPropsList.Add(itemtemp);
				break;
			case EditItemType.Obstacle:
				obstracList.Add(itemtemp);
				break;
			case EditItemType.Traffic:
				trafficList.Add(itemtemp);
				break;
			case EditItemType.Other:
				otherList.Add(itemtemp);
				break;
			}
			
		}
		
		//Debug.Log (itemCollec.itemPrefabs[1].name);
		//loadData
		itemGroupList.Clear ();
		CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/ItemGroupData.csv");
		for (int i=1; i<=csv.rowCount; ++i) {

			EditorItemGroupStru itemGroup = new EditorItemGroupStru();
			itemGroup.id=csv[i,1];
			itemGroup.node =csv[i,3];
			itemGroup.content = csv[i,4];

			string str=csv[i,2];
			str= (str!="")?str:"0";
//			itemGroup.roadType = (CreateRoadManager.SceneRoadType)int.Parse (str);

			itemGroupList.Add(itemGroup);
		}
		csv.SaveCSV ();
		
		ItemGroupData.Instance.RefreshData ();
	}

	/// <summary>
	/// 绘制物品编辑的UI
	/// </summary>
	void  DrawItemEditor()
	{
		
		itemEditorScrollPos = EditorGUILayout.BeginScrollView (itemEditorScrollPos);
		
		EditorGUILayout.Space();
		if (GUILayout.Button ("重置")) {
			ResetEditor();
		}
		EditorGUILayout.Space();
		
		
		//可捡道具
		pickPropShow = EditorGUILayout.Foldout(pickPropShow, pickPropStatus);
		if(pickPropShow)
		{
			for(int i = 0; i <pickupPropsList.Count ; i++)
			{
				pickupPropsList[i].count= EditorGUILayout.IntField(pickupPropsList[i].editName, pickupPropsList[i].count);
			}
			
		}
		
		//道具
		roadPropShow = EditorGUILayout.Foldout(roadPropShow, roadPropStatus);
		if(roadPropShow)
		{
			for(int i = 0; i <roadPropsList.Count ; i++)
			{
				roadPropsList[i].count= EditorGUILayout.IntField(roadPropsList[i].editName, roadPropsList[i].count);
			}
			
		}
		
		//障碍物
		obstracleShow = EditorGUILayout.Foldout(obstracleShow, obstracleStatus);
		if(obstracleShow)
		{
			for(int i = 0; i < obstracList.Count; i++)
			{
				obstracList[i].count= EditorGUILayout.IntField(obstracList[i].editName, obstracList[i].count);
			}
		}
		

		trafficShow = EditorGUILayout.Foldout(trafficShow, trafficStatus);
		if(trafficShow)
		{
			for(int i = 0; i < trafficList.Count; i++)
			{
				trafficList[i].count= EditorGUILayout.IntField(trafficList[i].editName, trafficList[i].count);
			}
			
		}
		
		//其他
		otherShow = EditorGUILayout.Foldout(otherShow, otherStatus);
		if(otherShow)
		{
			for(int i = 0; i < otherList.Count; i++)
			{
				otherList[i].count= EditorGUILayout.IntField(otherList[i].editName, otherList[i].count);
			}
			
		}
		
		
		EditorGUILayout.Space();
		
		if (GUILayout.Button ("生成物品")) {
			
			CreateItemObject();
			
		}
		
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal ();

//		itemRoadType = (CreateRoadManager.SceneRoadType)EditorGUILayout.EnumPopup ("道路类型", itemRoadType);

		editorRoadCount = EditorGUILayout.IntField ("Length",editorRoadCount);
		if (GUILayout.Button ("生成参考路面")) {
			CreateEditorRoad();
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button ("XY对齐")) {

			XYAlign();
		}
		if (GUILayout.Button ("Z轴排列")) {
			
			Transform[] trans = Selection.GetTransforms(SelectionMode.TopLevel);
			if(trans.Length>1)
			{
				Vector3 firstPos = trans[0].localPosition;
				
				for(int m=1;m <trans.Length;++m)
				{
					trans[m].localPosition = firstPos + new Vector3(0f,0,m*2f);
				}
			}
		}
		if (GUILayout.Button ("X轴排列")) {
			Transform[] trans = Selection.GetTransforms(SelectionMode.TopLevel);
			if(trans.Length>1)
			{
				Vector3 firstPos = trans[0].localPosition;
				
				for(int m=1;m <trans.Length;++m)
				{
					trans[m].localPosition = firstPos + new Vector3(m*3.4f,0f,0f);
				}
			}
			
		}

		
		if (GUILayout.Button ("跳跃对齐")) {
			Transform[] trans = Selection.GetTransforms(SelectionMode.TopLevel);
			if(trans.Length>1)
			{
				Vector3 firstPos = trans[0].localPosition;
				for(int i=1;i <trans.Length;++i)
				{
					trans[i].localPosition = firstPos + new Vector3(0,i*0.6f,i*1.4f);
				}
			}
		}
		

		
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		

		if (GUILayout.Button ("生成配置信息")) {

			//XYAlign();

			CreateConfigContent();
		}
		itemId = EditorGUILayout.TextField("ItemID",itemId);
		
		itemNote = EditorGUILayout.TextField("Note",itemNote);
		
		itemPosConfiContent = EditorGUILayout.TextField("Item配置信息",itemPosConfiContent);
		
		if (GUILayout.Button ("保存配置信息")) {
			WriteConfigData();
			InitItemPrefabsData();
		}



		itemScrollViewPos = EditorGUILayout.BeginScrollView (itemScrollViewPos, GUILayout.Height (300));
		for (int k =0; k<itemGroupList.Count; ++k) {
			
			EditorGUILayout.BeginHorizontal();

			//id button
			if (GUILayout.Button ( itemGroupList[k].id )) {
				
				itemId = itemGroupList[k].id;
				itemNote = itemGroupList[k].node;
				itemPosConfiContent = itemGroupList[k].content;
//				itemRoadType = itemGroupList[k].roadType;

				PreViewItemGroup();
			}

			// note button
			if (GUILayout.Button  ( itemGroupList[k].node )) {
				itemId = itemGroupList[k].id;
				itemNote = itemGroupList[k].node;
				itemPosConfiContent = itemGroupList[k].content;
//				itemRoadType = itemGroupList[k].roadType;
				
				PreViewItemGroup();
			}

			//delete button
			if(GUILayout.Button ("删除"))
			{
				DeleteItemGroup(itemGroupList[k].id );
			}
			
			EditorGUILayout.EndHorizontal();
		}
		
		
		EditorGUILayout.EndScrollView ();



		EditorGUILayout.EndScrollView ();
	}



	/// <summary>
	/// 根据编辑器设定 产生物体
	/// </summary>
	void CreateItemObject()
	{
		List<EditorItemStru> productList = new List<EditorItemStru> ();
		productList.AddRange (pickupPropsList);
		productList.AddRange (roadPropsList);
		productList.AddRange (trafficList);
		productList.AddRange (obstracList);
		productList.AddRange (otherList);
		
		for (int i =0; i<productList.Count; ++i) {
			if(productList[i].count <=0)
			{
				continue;
			}
			for(int index=0;index<productList[i].count;++index)
			{
				Transform newTrans =  prefabsCollection.getItemTransformByName(productList[i].prefabName);
				Debug.Log(newTrans.name);
				newTrans.parent= itemGroup.transform;
			}
			
			productList[i].count=0;
			
		}
		
		
	}


	/// <summary>
	/// Creates the content of the config.
	/// 生成配置 信息 字符串
	/// 格式为 预设名^位置^旋转^缩放^动画名称
	/// 缩放^动画名称 可以为空
	/// </summary>
	void CreateConfigContent()
	{
		string configContent = "";
		List<Transform> list = GetItemTransList ();
		
		SortList (list);
		
		for (int i=0; i <list.Count; ++i) {
			
		
			Transform trans = list[i];

			if(trans.GetComponent<ItemBase>() == null)
				continue;


			string itemConfig="";
			//prefab Name
			string prefabName = trans.name;
			int delIndex=-1; 
			if( (delIndex=prefabName.IndexOf('(')) >=0 )
			{
				prefabName = prefabName.Remove(delIndex);

			}else if( (delIndex=prefabName.IndexOf(' '))>=0   )
			{
				prefabName = prefabName.Remove(delIndex);
			}
		
			itemConfig+=prefabName+"^";
			
			//position
			string posStr= trans.localPosition.ToString().Replace(",","*");
			posStr=posStr.Replace("(","");
			posStr=posStr.Replace(")","");
			itemConfig+=posStr+"^";
			
			//rotation
			string rotate="";
			if(trans.localRotation == Quaternion.identity)
			{
				
			}else
			{
				rotate=trans.localRotation.ToString().Replace(",","*");
				rotate=rotate.Replace("(","");
				rotate=rotate.Replace(")","");
			}
			itemConfig+=rotate;
			itemConfig+="^";
			
			//scale
			string scale="";
			if(trans.localScale == Vector3.one)
			{
				
			}else
			{
				scale = trans.localScale.ToString().Replace(",","*");
				scale = scale.Replace("(","");
				scale = scale.Replace(")","");
			}
			
			itemConfig+=scale;
			itemConfig+="^";
			
			//other info
			itemConfig += ItemOtherInfo.Instance.CreateOtherInfo(trans);
			
			//Debug.Log(itemConfig);
			
			if( i <list.Count -1 )
			{
				itemConfig +="|";
			}
			
			configContent += itemConfig;
			
		}
		
		this.itemPosConfiContent = configContent;
	}


	/// <summary>
	/// 把配置信息写入文件
	/// </summary>
	void WriteConfigData()
	{
		CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/ItemGroupData.csv");
		// if the ID exist , change and save Data
		int  index = -1;
		for (int i=0; i<itemGroupList.Count; ++i) {
		    if(itemGroupList[i].id == itemId)
			{
				index =i;
			}
		}

		if ( index>=0) {
			csv[(index+1),2]="";
			csv[(index+1),3]=itemNote;
			csv[(index+1),4]=itemPosConfiContent;
		} else {
			//if ID not exist , new a row
			string[] newRow = {itemId,"",itemNote,itemPosConfiContent};
			csv.AddNewRow (newRow);
			
		}
		csv.SaveCSV ();
		AssetDatabase.Refresh ();
		
		//InitItemUIData ();
		
		Debug.Log ("Save Data");
	}


	/// <summary>
	/// Pres the view item group.
	/// 
	/// </summary>
	void PreViewItemGroup()
	{
		//Delete all object in ItemGruop
		int itemGroupChildCount = itemGroup.transform.childCount;
		for (int i=0; i<itemGroupChildCount; ++i) {
			DestroyImmediate(itemGroup.GetChild(0).gameObject);
		}
		
		// create new ItemGruop 
		Transform trans=  GenerateGameObjectByID(itemId);
		int childCount = trans.childCount;
		for (int k=0; k<childCount; ++k) {
			trans.GetChild(0).parent = itemGroup;
		}
		
		DestroyImmediate (trans.gameObject);

	}

	/// <summary>
	/// Generates the content of the game object by.
	/// 根据配置字符串生成 相应的GameObject
	/// </summary>
	/// <param name="itemContent">Item content.</param>
	Transform GenerateGameObjectByID(string ID)
	{



		List<ItemContentStru> contentList = ItemGroupData.Instance.GetContentList ( int.Parse(ID) );


		Debug.Log (  ItemGroupData.Instance.GetGroupLen(contentList));

		//根据contenList 生成 GameObject 组
		GameObject newItemGruopGO = new GameObject ();
		newItemGruopGO.name = "ItemGruopGO:"+ID;
		
		for (int k=0; k<contentList.Count; ++k) {
			try{
				Transform newTrans = prefabsCollection.getItemTransformByName( contentList[k].prefabName);
				newTrans.parent = newItemGruopGO.transform;
				
				newTrans.localPosition = contentList[k].pos;
				newTrans.localRotation = contentList[k].rotate;
				newTrans.localScale = contentList[k].scale;
				ItemOtherInfo.Instance.SetOtherInfoOfTrans(newTrans,contentList[k].otherInfo);
				
			}catch{
				Debug.LogError("error prefabs  "+contentList[k].prefabName);
			}
			
			
		}
		
		return newItemGruopGO.transform;
		
	}

	/// <summary>
	/// 删除一条数据
	/// </summary>
	void DeleteItemGroup(string ID)
	{
		CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/ItemGroupData.csv");
		// if the ID exist , change and save Data
		int  index = -1;
		for (int i=0; i<itemGroupList.Count; ++i) {
			if(itemGroupList[i].id == ID)
			{
				index =i;
			}
		}
		if ( index>= 0) {
			csv.RemoveRow(index);
		} else {
			//if ID not exist , new a row
			Debug.LogError("No this ID");
		}
		csv.SaveCSV ();
		AssetDatabase.Refresh ();
		
		InitItemPrefabsData ();
		Debug.Log ("Save Data");
	}





	void CreateEditorRoad()
	{

		if (editorRoadGroup != null) {
			DestroyImmediate(editorRoadGroup.gameObject);
		}

		int roadCount = 30;
		if (editorRoadCount > 1) {
			roadCount = editorRoadCount;
		}
		float len = 100f;
		for (int i=0; i<roadCount; ++i) {
			Transform road =	prefabsCollection.getItemTransformByName( "EditorRoad");
			road.parent =editorRoadGroup;
			road.localPosition = new Vector3(0f,0f,len*i);
		}
	}

	/// <summary>
	/// 重置编辑器
	/// </summary>
	void ResetEditor()
	{
		itemPosConfiContent = "";
		List<Transform> childTrans = new List<Transform> ();
		for (int i=0; i <itemGroup.transform.childCount; ++i) {
			Transform trans = itemGroup.transform.GetChild(i);
			childTrans.Add(trans);
		}
		
		for (int k =0; k <childTrans.Count; ++k) {
			
			DestroyImmediate(childTrans[k].gameObject);
		}
		
		
		//ReSetEditor Data
		itemId = "";
		itemNote = "";
		itemPosConfiContent = "";
		
		inputItemId = "";

		DestroyImmediate(editorRoadGroup.gameObject);
		DestroyImmediate (itemGroup.gameObject);
		
		
	}


	#endregion


	#region  路标编辑

	List<EditorRoadInfoStru> roadInfoList= new List<EditorRoadInfoStru>();

	string roadId;
	string roadNote;
	string roadType;
	string roadContent;

	Vector2 roadEditorScrollPos;
	Vector2 roadScrollViewPos;

    private void InitRaodData()
	{
		RoadData.Instance.RefreshData();

		roadInfoList.Clear();
		CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/RoadData.csv");
		for (int i=1; i<=csv.rowCount; ++i) {
			
			EditorRoadInfoStru roadInfo = new EditorRoadInfoStru();
			roadInfo.id=csv[i,1];
			roadInfo.note =csv[i,2];
			roadInfo.type = csv[i,3];
			roadInfo.content = csv[i,4];
			roadInfoList.Add(roadInfo);
		}
		csv.SaveCSV ();
	}
	private void  DrawRoadEditor()
	{
		roadEditorScrollPos = EditorGUILayout.BeginScrollView (itemEditorScrollPos);

		EditorGUILayout.Space();
		if (GUILayout.Button ("重置")) {
			ResetRoadData();
		}
		EditorGUILayout.Space();


		EditorGUILayout.Space();
		EditorGUILayout.Space();



		if (GUILayout.Button ("创建路标 （快捷键N 创建路标）  ")) {
			CreateRoadPoint();
		}

		EditorGUILayout.Space();

		roadId = EditorGUILayout.TextField("roadId",roadId);
		roadNote = EditorGUILayout.TextField("roadNote",roadNote);
		roadType = EditorGUILayout.TextField("roadType",roadType);
		roadContent = EditorGUILayout.TextField("路标配置信息",roadContent);

		if (GUILayout.Button ("生成路标配置")) {
			CreateRoadConfigContent();
		}

		if (GUILayout.Button ("保存路标配置")) {
			WriteRoadData();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		roadScrollViewPos = EditorGUILayout.BeginScrollView (roadScrollViewPos, GUILayout.Height (300));
		for (int k =0; k<roadInfoList.Count; ++k) {
			
			EditorGUILayout.BeginHorizontal();
			
			//id button
			if (GUILayout.Button ( roadInfoList[k].id )) {
				
				roadId = roadInfoList[k].id;
				roadNote = roadInfoList[k].note;
				roadType = roadInfoList[k].type;
				roadContent = roadInfoList[k].content;
				PreviewRoadData(roadId);

				RoadPoint roadScript=   roadPointGroup.GetComponent<RoadPoint>();
				roadScript.GetPointList();
				roadScript.UpdateSmoothPath();
			}
			
			// note button
			if (GUILayout.Button  ( roadInfoList[k].note )) {
				roadId = roadInfoList[k].id;
				roadNote = roadInfoList[k].note;
				roadType = roadInfoList[k].type;
				roadContent = roadInfoList[k].content;
				PreviewRoadData(roadId);
			}
			
			//delete button
			if(GUILayout.Button ("删除"))
			{
				DeleteRoadPoint(roadInfoList[k].id );
			}
			
			EditorGUILayout.EndHorizontal();
		}
		
		
		EditorGUILayout.EndScrollView ();


		EditorGUILayout.EndScrollView ();

	}
	private Transform CreateRoadPoint()
	{
		Transform roadPoint = prefabsCollection.getItemTransformByName("RoadPointEditor");
		roadPoint.parent = roadPointGroup;
		return roadPoint;
	}

	private void CreateRoadConfigContent()
	{
		string content="";
		int childCount = roadPointGroup.childCount;
		for(int i=0;i<childCount;++i)
		{
			Vector3 pos = roadPointGroup.GetChild(i).position;
			string posStr= pos.ToString().Replace(",","*");
			posStr=posStr.Replace("(","");
			posStr=posStr.Replace(")","");

			if(i !=childCount-1)
			{
				posStr+="^";
			}
			content+=posStr;
		}
		roadContent= content;
	}

	private void WriteRoadData()
	{
		CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/RoadData.csv");
		// if the ID exist , change and save Data
		int  index = -1;
		for (int i=0; i<roadInfoList.Count; ++i) {
			if(roadInfoList[i].id == roadId)
			{
				index =i;
			}
		}
		
		if ( index>=0) {
			csv[(index+1),2]= roadNote;
			csv[(index+1),3]=roadType;
			csv[(index+1),4]=roadContent;
		} else {
			//if ID not exist , new a row
			string[] newRow = {roadId,roadNote,roadType,roadContent};
			csv.AddNewRow (newRow);
			
		}
		csv.SaveCSV ();
		AssetDatabase.Refresh ();

		InitRaodData();	
		Debug.Log ("Save Data");
	}
	private void PreviewRoadData(string idStr)
	{
		Debug.Log("PreviewRoad");

		while(roadPointGroup.childCount>0)
		{
			DestroyImmediate(  roadPointGroup.GetChild(0).gameObject );
		}


		int id=int.Parse(idStr);
		List<Vector3> pointList= RoadData.Instance.GetPointList(id);

		for(int i=0;i<pointList.Count;++i)
		{
			GameObject pointGO= new GameObject();
			Transform roadPoint = pointGO.transform;
			roadPoint.parent = roadPointGroup;
			roadPoint.position= pointList[i];

			pointGO.name="Point"+(i+1);
		}

	}
	private void ResetRoadData()
	{
		roadId="";
		roadNote = "";
		roadType = "";
		roadContent ="";

		while(roadPointGroup.childCount>0)
		{
			DestroyImmediate(  roadPointGroup.GetChild(0).gameObject );
		}

	}
	
	void  DeleteRoadPoint(string ID)
	{
		CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/RoadData.csv");
		// if the ID exist , change and save Data
		int  index = -1;
		for (int i=0; i<roadInfoList.Count; ++i) {
			if(roadInfoList[i].id == ID)
			{
				index =i;
			}
		}
		if ( index>= 0) {
			csv.RemoveRow(index);
		} else {
			//if ID not exist , new a row
			Debug.LogError("No this ID");
		}
		csv.SaveCSV ();
		AssetDatabase.Refresh ();
		
		InitRaodData ();
		Debug.Log ("Save Data");
	}

	#endregion


	#region  关卡编辑

    int maxLevelID = 40;
	string levelID;
	string levelNote;
	string levelConfigContent;
	
	int levelUseTime;
	int levelCirlcleCount;
	string levelStrOpponent;

	GameLevelModel levelModel=GameLevelModel.Rank;

	List<EditorLevelInfoStru> levelList = new List<EditorLevelInfoStru>();

	Vector2 levelScoreViewPos;

	string levelRoadTypeList="";
    string roadModelName = "";
    string roadPointID = "";
//	CreateRoadManager.SceneRoadType levelRoadType=CreateRoadManager.SceneRoadType.City;

    int levelDataType = 0; //关卡数据的类型，0为服务器关卡数据，1为本地数据
    string[] levelDataTypeStr = {"初始化服务器关卡数据", "初始化本地自身关卡数据"};

	void DrawLevelEditor()
	{
		EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
		if (GUILayout.Button ("重置")) {
			ResetLevelEditor();
		}

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

//        if (GUILayout.Button("初始化服务器关卡数据(默认)"))
//        {
//            InitLevelData();
//        }
//
//        if (GUILayout.Button("初始化本地自身关卡数据"))
//        {
//            InitSelfLocalLevelData();
//        }

        int levelDataTypeTemp = GUILayout.SelectionGrid(levelDataType, levelDataTypeStr, levelDataTypeStr.Length);
        if (levelDataTypeTemp != levelDataType)
        {
            if(levelDataTypeTemp == 0)
                InitLevelData();
            else
                InitSelfLocalLevelData();
        }
        levelDataType = levelDataTypeTemp;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
		EditorGUILayout.Space();

        maxLevelID = EditorGUILayout.IntField ("最大关卡ID",maxLevelID);

        EditorGUILayout.Space();
		levelID = EditorGUILayout.TextField("LevelID",levelID);
		levelNote = EditorGUILayout.TextField("Note",levelNote);
		levelConfigContent = EditorGUILayout.TextField("Level配置信息",levelConfigContent);
		levelRoadTypeList = EditorGUILayout.TextField ("关卡道路",levelRoadTypeList);
		levelModel =  (GameLevelModel) EditorGUILayout.EnumPopup("比赛模式",levelModel);

		EditorGUILayout.Space();
		levelUseTime = EditorGUILayout.IntField ("关卡时间",levelUseTime);
		levelCirlcleCount = EditorGUILayout.IntField("圈数",levelCirlcleCount);
        levelStrOpponent = EditorGUILayout.TextField("对手ID",levelStrOpponent);
        roadModelName = EditorGUILayout.TextField("道路模型名",roadModelName);
        roadPointID = EditorGUILayout.TextField("道路路线ID",roadPointID);
	
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (levelDataType == 0 && GUILayout.Button ("写入服务器关卡数据")) {
            if(EditorUtility.DisplayDialog("写入警告","确定直接写入服务器数据吗？你可以先写入本地数据，再合并到服务器数据。", "确定写入", "取消写入"))
			    WriteLevelData();
		}

        if (levelDataType != 0 && GUILayout.Button ("写入本地自身关卡数据")) {
            WriteSelfLocalLevelData();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button ("本地数据合并到服务器数据")) {
            if (EditorUtility.DisplayDialog("合并警告", "确定合并本地数据到服务器数据吗？", "确定合并", "我点错了"))
            {
                SelfLocalDataReplaceLevelData();
            }
        }

        if (levelDataType != 0)
        {
            GUI.color = Color.red;
            if (GUILayout.Button("删除本地关卡数据"))
            {
                if (EditorUtility.DisplayDialog("删除警告", "确定完全删除本地关卡数据吗？这操作无法还原！", "确定删除", "我点错了"))
                {
                    if (FileTool.IsFileExists(SelfLocalGameLevelData.fileName))
                    {
                        FileTool.DelectFile(SelfLocalGameLevelData.fileName);
                        InitLevelData();
                        this.ShowNotification(new GUIContent("成功删除本地关卡数据文件！"));
                    }
                }
            }
            GUI.color = Color.white;
        }

        EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();


		levelScoreViewPos = EditorGUILayout.BeginScrollView (levelScoreViewPos, GUILayout.Height (300));
		for (int k =0; k<levelList.Count; ++k) {
			
			EditorGUILayout.BeginHorizontal();

			//Debug.Log( k +" "+levelList.Count+" "+levelList[k].note);
			//id button
            if (GUILayout.Button ( levelList[k].id , GUILayout.Width(400))) {
				
				levelID = levelList[k].id;
				levelNote = levelList[k].note;
				levelConfigContent = levelList[k].content;

			
				levelUseTime = levelList[k].useTime;
				levelCirlcleCount = levelList[k].circleCount;
				levelStrOpponent = levelList[k].opponent;

				levelModel =(GameLevelModel) (System.Enum.Parse(typeof(GameLevelModel),levelList[k].levelModel));
				levelRoadTypeList = levelList[k].roadType;
                roadModelName = levelList[k].roadModelName;
                roadPointID = levelList[k].roadPointId;
			}
			
			// note button
            if (GUILayout.Button  ( levelList[k].note , GUILayout.Width(400))) {
				levelID = levelList[k].id;
				levelNote = levelList[k].note;
				levelConfigContent = levelList[k].content;
				
				
				levelUseTime = levelList[k].useTime;
				levelCirlcleCount = levelList[k].circleCount;
				levelStrOpponent = levelList[k].opponent;
				
				levelModel =(GameLevelModel) (System.Enum.Parse(typeof(GameLevelModel),levelList[k].levelModel));
				levelRoadTypeList = levelList[k].roadType;
                roadModelName = levelList[k].roadModelName;
                roadPointID = levelList[k].roadPointId;
			}
			
			//delete button
            if (levelDataType != 0)
            {
                GUI.color = Color.red;
                if (GUILayout.Button("删除", GUILayout.Width(200)))
                {
                    if (levelDataType != 0) //服务器的数据不允许删除
                    DeleteSelfDataItem(levelList[k].id);
                }
                GUI.color = Color.white;
            }

			EditorGUILayout.EndHorizontal();
		}
		
		
		EditorGUILayout.EndScrollView ();


	}

	void InitLevelData()
	{
        levelDataType = 0;
		levelList.Clear ();
		GameLevelData.Instance.RefreshData ();
		int levelCount = GameLevelData.Instance.GetDataRow ();
		for (int i=1; i<=levelCount; ++i) {
			EditorLevelInfoStru info = new EditorLevelInfoStru();
			info.id= i.ToString();
			info.note = GameLevelData.Instance.GetNode(i);
			info.content = GameLevelData.Instance.GetContent(i);
			info.useTime = GameLevelData.Instance.GetUseTime(i);
			info.circleCount= GameLevelData.Instance.GetCircleCount(i);
			info.opponent = GameLevelData.Instance.GetStrOpponent(i);
			info.roadType = GameLevelData.Instance.GetSceneType(i);
			info.levelModel = GameLevelData.Instance.GetGameLevelModel(i);
            info.roadModelName = GameLevelData.Instance.GetRoadModelName(i);
            info.roadPointId = GameLevelData.Instance.GetRoadPointID(i);

			levelList.Add(info);
		}
	}

    void InitSelfLocalLevelData()
    {
        levelDataType = 1;
        levelList.Clear ();
        SelfLocalGameLevelData.Instance.CheckAndInitData();
        int levelCount = SelfLocalGameLevelData.Instance.GetDataRow ();
        for (int i=1; i<= maxLevelID; ++i) {

            if (!SelfLocalGameLevelData.Instance.IsIDExist(i))
                continue;
            
            EditorLevelInfoStru info = new EditorLevelInfoStru();
            info.id= i.ToString();
            info.note = SelfLocalGameLevelData.Instance.GetNode(i);
            info.content = SelfLocalGameLevelData.Instance.GetContent(i);
            info.useTime = SelfLocalGameLevelData.Instance.GetUseTime(i);
            info.circleCount= SelfLocalGameLevelData.Instance.GetCircleCount(i);
            info.opponent = SelfLocalGameLevelData.Instance.GetStrOpponent(i);
            info.roadType = SelfLocalGameLevelData.Instance.GetSceneType(i);
            info.levelModel = SelfLocalGameLevelData.Instance.GetGameLevelModel(i);
            info.roadModelName = SelfLocalGameLevelData.Instance.GetRoadModelName(i);
            info.roadPointId = SelfLocalGameLevelData.Instance.GetRoadPointID(i);

            levelList.Add(info);
        }
    }

	void WriteLevelData()
	{
		CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/GameLevelData.csv");
		// if the ID exist , change and save Data
		int  index = -1;
		for (int i=0; i<levelList.Count; ++i) {
			if(levelList[i].id == levelID)
			{
				index =i;
                break;
			}
		}
		
		if ( index>=0) {
			csv[(index+1),2]=levelNote;
			csv[(index+1),3]=levelConfigContent;
			csv[(index+1),4]=levelUseTime.ToString();
			csv[(index+1),5]=levelCirlcleCount.ToString();
			csv[(index+1),6]=levelStrOpponent;
			csv[(index+1),7]=levelModel.ToString();
			csv[(index+1),8]=levelRoadTypeList;
            csv[(index + 1), 9] = roadModelName;
            csv[(index + 1), 10] = roadPointID;

		} else {
			//if ID not exist , new a row
            string[] newRow = {levelID,levelNote,levelConfigContent,levelUseTime.ToString(),levelCirlcleCount.ToString(),levelStrOpponent,levelModel.ToString(),levelRoadTypeList,roadModelName,roadPointID};
			csv.AddNewRow (newRow);
			
		}
		csv.SaveCSV ();
		AssetDatabase.Refresh ();
		
		InitLevelData ();
		
		Debug.Log ("Save Data");

        this.ShowNotification(new GUIContent("成功保存数据到服务器文件！"));
	}

    void WriteSelfLocalLevelData()
    {
        SelfLocalGameLevelData.Instance.CheckAndInitData();
        CSVFileTool csv = new CSVFileTool(FileTool.RootPath + SelfLocalGameLevelData.fileName);
        // if the ID exist , change and save Data
        int  index = -1;
        for (int i=1; i<=csv.rowCount; ++i) {
            if (levelID.CompareTo(csv[i, "Id"]) == 0)
            {
                index = i;
                break;
            }
        }

        if ( index>=0) {
            csv[index, 2]=levelNote;
            csv[index, 3]=levelConfigContent;
            csv[index, 4]=levelUseTime.ToString();
            csv[index, 5]=levelCirlcleCount.ToString();
            csv[index, 6]=levelStrOpponent;
            csv[index, 7]=levelModel.ToString();
            csv[index, 8]=levelRoadTypeList;
            csv[index, 9] = roadModelName;
            csv[index, 10] = roadPointID;
        } else {
            //if ID not exist , new a row
            string[] newRow = {levelID,levelNote,levelConfigContent,levelUseTime.ToString(),levelCirlcleCount.ToString(),levelStrOpponent,levelModel.ToString(),levelRoadTypeList,roadModelName,roadPointID};
            csv.AddNewRow (newRow);

        }
        csv.SaveCSV ();

        InitSelfLocalLevelData ();

        Debug.Log ("Save Self Local Data");

//        this.ShowNotification(new GUIContent("成功保存数据到本地文件！"));
    }

    /// <summary>
    /// 删除一条本地关卡数据
    /// </summary>
    void DeleteSelfDataItem(string ID)
    {
        CSVFileTool csv = new CSVFileTool(FileTool.RootPath + SelfLocalGameLevelData.fileName);
        // if the ID exist , change and save Data
        int  index = -1;
        for (int i=1; i<=csv.rowCount; ++i) {
            if (ID.CompareTo(csv[i, "Id"]) == 0)
            {
                index = i;
                break;
            }
        }
        if ( index>= 0) {
            csv.RemoveRow(index-1);
        } else {
            //if ID not exist , new a row
            Debug.LogError("No this ID");
            this.ShowNotification(new GUIContent("成功删除本地数据！"));
        }
        csv.SaveCSV ();

        InitSelfLocalLevelData();
        Debug.Log ("Delete Data Success");
    }

    /// <summary>
    /// 本地关卡数据替换掉服务器关卡数据
    /// </summary>
    void SelfLocalDataReplaceLevelData()
    {
        CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/GameLevelData.csv");
        CSVFileTool selfCSV = new CSVFileTool(FileTool.RootPath + SelfLocalGameLevelData.fileName);

        for (int i = 1; i <= selfCSV.rowCount; ++i)
        {
            for (int j = 1; j <= csv.rowCount; j++)
            {
                if (selfCSV[i, "Id"].CompareTo(csv[j, "Id"]) == 0)
                {
                    csv[j, 2] = selfCSV[i, 2];
                    csv[j, 3] = selfCSV[i, 3];
                    csv[j, 4] = selfCSV[i, 4];
                    csv[j, 5] = selfCSV[i, 5];
                    csv[j, 6] = selfCSV[i, 6];
                    csv[j, 7] = selfCSV[i, 7];
                    csv[j, 8] = selfCSV[i, 8];
                    csv[j, 9] = selfCSV[i, 9];
                    csv[j, 10] = selfCSV[i, 10];

                    break;
                }
            }
        }

        csv.SaveCSV();

        AssetDatabase.Refresh ();
        levelDataType = 0;
        InitLevelData();

        this.ShowNotification(new GUIContent("成功保存合并关卡数据！"));
    }


	void ResetLevelEditor()
	{
        maxLevelID = 40;
		levelID = "";
		levelNote = "";
		levelConfigContent = "";

		levelUseTime = 0;
		levelCirlcleCount =0;
		levelStrOpponent = "";
        roadModelName = "";
        roadPointID = "";

	}

	void PreviewLevelData()
	{

	}

	float GetLevelLen(int levelID)
	{
		float levelLen = 0;
		List<string> itemGroupIDList = GameLevelData.Instance.GetGroupIDList(levelID);
		for (int i=0; i<itemGroupIDList.Count; ++i) {
			levelLen+= ItemGroupData.Instance.GetGroupLen( int.Parse(itemGroupIDList[i]));
		}
		return levelLen;
	}

	#endregion


	#region 道具组编辑器

	int groupCount=0;
	int nCarGroup=2;
	List<Transform> groupList = new List<Transform>();


	/*编辑的UI*/
	void DrawGroupEditor()
	{

		groupCount = EditorGUILayout.IntField ("生成道路组的数量",groupCount);
		nCarGroup = EditorGUILayout.IntField ("每N组中有有一组车，N>=2",nCarGroup);
		nCarGroup = nCarGroup<2?2:nCarGroup;

		if (GUILayout.Button ("生成道路组")) {
			CreateEditorGroup();
		}

		if (GUILayout.Button ("随机组内道具")) {
			EditorGroupRest();
		}

		if (GUILayout.Button ("把车移动到组外 生成配置")) {

			for (int k=0; k<groupList.Count; ++k) {
				if(groupList[k].GetComponent<GroupCar>() !=null)
				{
					GroupCar groupItem =  groupList[k].GetComponent<GroupCar>();
					groupItem.PutItemOutGroup();
				}else if(groupList[k].GetComponent<GroupItem>() !=null)
				{
					GroupItem groupItem =  groupList[k].GetComponent<GroupItem>();
					groupItem.PutItemOutGroup();
				}
			}

		}
		if (GUILayout.Button ("生成配置把车移动到组内 再次编辑")) {
			for (int k=0; k<groupList.Count; ++k) {
				if(groupList[k].GetComponent<GroupCar>() !=null)
				{
					GroupCar groupItem =  groupList[k].GetComponent<GroupCar>();
					groupItem.PutItemInGroup();
				}else if(groupList[k].GetComponent<GroupItem>() !=null)
				{
					GroupItem groupItem =  groupList[k].GetComponent<GroupItem>();
					groupItem.PutItemInGroup();
				}
			}
		}
	}

	void InitGroupEditor()
	{

	}

	void CreateEditorGroup()
	{
		groupList.Clear ();
		for (int i=1; i<=groupCount; ++i) {

			if(i%nCarGroup == 0)
			{
				GameObject groupGO = new GameObject();
				groupGO.name= "EditorGroupCar"+i;
				groupGO.transform.parent = itemGroup;
				groupGO.AddComponent<GroupCar>();
				groupList.Add(groupGO.transform);
			}else
			{
				GameObject groupItemGO = new GameObject();
				groupItemGO.name= "EditorGroupItem"+i;
				groupItemGO.transform.parent = itemGroup;
				groupItemGO.AddComponent<GroupItem>();
				groupList.Add(groupItemGO.transform);
			}


		}

		EditorGroupRest ();

	}

	void EditorGroupRest()
	{
		float curLen = 0;
		for (int k=0; k<groupList.Count; ++k) {

			if(  groupList[k].GetComponent<GroupCar>() !=null)
			{

				GroupCar groupItem =  groupList[k].GetComponent<GroupCar>();
				Vector3 pos = Vector3.zero;
				pos.z = curLen + groupItem.preDistance;
				groupItem.transform.localPosition = pos;

				curLen = groupItem.transform.localPosition.z + groupItem.colCount *groupItem.lenZ;

				groupItem.CreateRandItem();

			}else if( groupList[k].GetComponent<GroupItem>() !=null)
			{
				GroupItem groupItem =  groupList[k].GetComponent<GroupItem>();
				Vector3 pos = Vector3.zero;
				pos.z = curLen + groupItem.preDistance;
				groupItem.transform.localPosition = pos;
				
				curLen = groupItem.transform.localPosition.z + groupItem.lenZ;
				
				groupItem.CreateRandItem();
			}
		}
	}

	#endregion





	class EditorLevelInfoStru{
		public string id;
		public string note;
		public string content;
		public int useTime;
		public int circleCount;
		public string opponent;
		public string roadType;
		public string levelModel;
        public string roadModelName;
        public string roadPointId;
	}

	class EditorItemStru{
		public int id;
		public string editName;
		public string prefabName;
		public EditItemType itemType;
		public int count;

	}

	class EditorItemGroupStru
	{
		public string id;
		public string node;
		public string content;
//		public CreateRoadManager.SceneRoadType roadType;
	}

	class EditorRoadInfoStru{
		public string id;
		public string note;
		public string type;
		public string content;
	}


}

