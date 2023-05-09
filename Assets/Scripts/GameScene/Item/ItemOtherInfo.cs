using UnityEngine;
using System.Collections;


/// <summary>
/// Item 配置信息的生成 
/// 和根据配置信息设定Item
/// 
/// 编辑器中信息的保存
/// 和游戏中信息的加载
/// 
/// </summary>
public class ItemOtherInfo  {

	static ItemOtherInfo _instance=null;
	
	public static ItemOtherInfo Instance
	{
		get{
			if(_instance == null)
			{
				_instance = new ItemOtherInfo();
			}
			
			return _instance;
		}
	}

	/// <summary>
	/// 生成其他备注信息 
	/// 例如 刺球的动画信息等
	/// 
	/// 不能包含以下字符 , | ^
	/// </summary>
	/// <returns>The other info.</returns>
	/// <param name="tran">Tran.</param>
	public	string CreateOtherInfo(Transform tran)
	{
        if (tran.GetComponent<GuideItem> () != null) {
			GuideItem item = tran.GetComponent<GuideItem> ();
			string info = item.id.ToString () + "&";
			info += (int)(item.curStep);
			return info;
		} 
		else if(tran.GetComponent<RandomProp> () != null)
		{
			RandomProp prop =tran.GetComponent<RandomProp>();
			string info = prop.id.ToString() +"&";
			info+=prop.propId.ToString();
			return info;
		}
	
		return "";
	}
	
	/// <summary>
	/// 根据配置表里的信息设置 新建的GameObject
	/// </summary>
	/// <param name="trans">Trans.</param>
	/// <param name="info">Info.</param>
	public void SetOtherInfoOfTrans(Transform tran,string info)
	{

		if (info == null || info == "") {
			return;
		}
	  if (tran.GetComponent<GuideItem> () != null) {
			GuideItem item = tran.GetComponent<GuideItem> ();
			string[] infos = info.Split ('&');
			item.curStep = (GameGuideStep)(int.Parse (infos [1]));
		} 
		else  if(tran.GetComponent<RandomProp>()!=null)
		{
			RandomProp prop =tran.GetComponent<RandomProp>();
			string[] infos = info.Split ('&');
			prop.id = int.Parse(infos[0]);
			prop.propId = int.Parse(infos[1]);
		}
	}
	

	string VectorToString(Vector3 vec)
	{
		string str = vec.ToString ();
		str = str.Replace (",","*");
		str= str.Replace("(","");
		str = str.Replace(")","");
		
		return str;
	}
	
	Vector3 StringToVector(string vecStr)
	{
		string[] v = vecStr.Split ('*');
		Vector3 vec = new Vector3 (float.Parse(v[0]),float.Parse(v[1]),float.Parse(v[2]));
		return vec;
	}

}
