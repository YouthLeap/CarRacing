using UnityEngine;
using System.Collections;

public class ModelAnimationData : IData {

	private ModelAnimationData()
	{
		InitData("Data/ModelAnimationData");
	}
	
	private static ModelAnimationData instance;
	public static ModelAnimationData Instance
	{
		get
		{
			if(instance == null)
				instance = new ModelAnimationData();
			
			return instance;
		}
	}
	
	public void RefreshData()
	{
		InitData("Data/ModelAnimationData");
	}
	
	public string GetModelName(int Id)
	{
		return GetProperty("ModelName", Id);
	}
	
	public string GetAnimationName(int Id)
	{
		return GetProperty("AnimationName", Id);
	}
	
	public int GetFirstFrame(int Id)
	{
		return int.Parse(GetProperty("FirstFrame", Id));
	}
	
	public int GetLastFrame(int Id)
	{
		return int.Parse(GetProperty("LastFrame", Id));
	}
	
	public WrapMode GetWrapMode(int Id)
	{
		return (WrapMode)System.Enum.Parse(typeof(WrapMode), GetProperty("WrapMode", Id));
	}
	
	public bool GetLoop(int id)
	{
		return bool.Parse(GetProperty("Loop", id));
	}
	
	public bool GetLockRotation(int id)
	{
		return bool.Parse(GetProperty("LockRotation", id));
	}
	
	public bool GetLockY(int id)
	{
		return bool.Parse(GetProperty("LockY", id));
	}
	
	public bool GetLockXZ(int id)
	{
		return bool.Parse(GetProperty("LockXZ", id));
	}
	
	public string GetCallBack(int id)
	{
		return GetProperty("CallBack", id);
	}
}
