using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 关卡路段数据配置
/// </summary>
public class ItemGroupData : IData {

	private ItemGroupData()
	{
		InitData("Data/ItemGroupData");
	}
	
	private static ItemGroupData instance;
	public static ItemGroupData Instance
	{
		get
		{
			if(instance == null)
				instance = new ItemGroupData();
			
			return instance;
		}
	}

	public void RefreshData()
	{
		InitData("Data/ItemGroupData");
	}

	public string GetContent(int Id)
	{
		return GetProperty("Content", Id);
	}
	public string GetNode(int Id)
	{
		return GetProperty ("Note",Id);
	}


	/// <summary>
	/// 解释ItemGroup 的数据返回 一组结构体列表
	/// </summary>
	/// <returns>The content list.</returns>
	/// <param name="id">Identifier.</param>
	public List<ItemContentStru> GetContentList(int id)
	{

		string itemContent = GetContent (id);
		List< ItemContentStru> contentList = new List<ItemContentStru> ();

		if (itemContent == null || itemContent == "") {
			Debug.LogError("no content");
			return contentList;
		}

		//解释Content 的字符串
		string[] itemUnits = itemContent.Split ('|');
		for (int i=0; i<itemUnits.Length; ++i) {
			
			string[] item = itemUnits[i].Split('^');
			
			//get prefab name
			ItemContentStru itemCon = new ItemContentStru();
		
			itemCon.prefabName= item[0];
			
			// get position
			string[] pos = item[1].Split('*');
			Vector3 itemPos =new Vector3( float.Parse(pos[0]),float.Parse(pos[1]), float.Parse(pos[2]) );
			itemCon.pos = itemPos;
			
			// get rotation
			Quaternion itemRota;
			if(item[2]=="" || item[2] == "0.0* 0.0* 0.0* 1.0")
			{
				itemRota=new Quaternion(0f,0f,0f,1f);
			}else
			{
				string[] rotate = item[2].Split('*');
				itemRota = new Quaternion( float.Parse(rotate[0]),float.Parse(rotate[1]), float.Parse(rotate[2]) ,float.Parse(rotate[3]) );
			}
			itemCon.rotate= itemRota;
			
			
			
			//get scale
			if(item.Length>=4)
			{
				Vector3 itemScale;
				if(item[3] =="" || item[3] == "1.0* 1.0* 1.0")
				{
					itemScale= Vector3.one;
				}else
				{
					string[] scale = item[3].Split('*');
					itemScale = new Vector3 (float.Parse(scale[0]),float.Parse(scale[1]),float.Parse(scale[2]));
				}
				itemCon.scale = itemScale;
			}else
			{
				itemCon.scale = new Vector3(1f,1f,1f);
			}
			
			
			//get config infomation
			if(item.Length>=5)
			{
				itemCon.otherInfo = item[4];
			}else
			{
				itemCon.otherInfo = "";
			}
			
			
			//default value
			itemCon.pathZ= 0;
			itemCon.parent = null;
			
			contentList.Add(itemCon);
		}
	
		return contentList;
   }

	/// <summary>
	/// 获取Group Z轴方向的长度  list 在Editor 中已经做好了排序的
	/// </summary>
	/// <returns>The group length.</returns>
	/// <param name="list">List.</param>
	public float GetGroupLen(List<ItemContentStru> list)
	{
		Vector3 pos2 = list [list.Count - 1].pos;
		return pos2.z;
	}
	public float GetGroupLen(int id)
	{
		List<ItemContentStru> list = GetContentList (id);
		Vector3 pos2 = list [list.Count - 1].pos;
		return pos2.z;
	}


//	public CreateRoadManager.SceneRoadType GetRoadType(int id)
//	{
//		string typeStr = GetProperty ("SceneRoadType",id);
//		int typeID = int.Parse (typeStr);
//		return (CreateRoadManager.SceneRoadType)typeID;
//	}

}



#region 物品用到的数据
/// <summary>
/// 每一个Item的位置信息
/// </summary>
public class ItemContentStru{
	public int id;
	public string editName;
	public string prefabName;
	public EditItemType itemType;

	public Vector3 pos;
	public Quaternion rotate;
	public Vector3 scale;
	public string otherInfo;
	public float pathZ;
	public Transform parent;
	public Transform selfTrans;
	public bool isCar;
}
#endregion