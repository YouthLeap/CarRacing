using UnityEngine;
using System.Collections;

/// <summary>
/// Superman model data read class.
/// </summary>
public class ModelData : IData {

	private ModelData()
	{
		InitData("Data/ModelData");
	}
	
	private static ModelData instance;
	public static ModelData Instance
	{
		get
		{
			if(instance == null)
				instance = new ModelData();
			
			return instance;
		}
	}

	protected override string GetProperty (string name, int id)
	{
		id = GetValidId (id);
		return base.GetProperty (name, id);
	}

	private int GetValidId(int Id)
	{
		return Id = IDTool.GetModelLevel (Id) == 0 ? Id + 1 : Id;
	}
	
	public int GetUseModelDataCount()
	{
		int allData = GetDataRow();
		int modelCount = allData / 5;
		int useCount = 0;

		for(int i = 1; i <= modelCount; i ++)
		{
			if(GetIsUse(i*100+1))
				useCount++;
		}
		
		return useCount;
	}
	
	public string GetName(int Id)
	{
		return GetProperty("Name", Id);
	}

	public string GetDesc(int Id)
	{
		return GetProperty("Desc", Id);
	}

	public string GetPrefabName(int Id)
	{
		return GetProperty("PrefabName", Id);
	}
	public string GetMechaPrefabName(int Id)
	{
		return GetProperty("MechaPrefabName", Id);
	}
	
	public string GetAotemanSound(int Id)
	{
		return GetProperty("AotemanSound", Id);
	}

	public string GetAotemanShowSound(int Id)
	{
		return GetProperty("AotemanShowSound", Id);
	}

	public string GetSkillSound(int Id)
	{
		return GetProperty ("SkillSound",Id);
	}
	public string GetChangeSound(int Id)
	{
		return GetProperty ("ChangeSound",Id);
	}
	public string GetZhaohuanCostType(int Id)
	{
		return GetUpgradeCostType(Id);
	}

	public int GetZhaohuanCost(int Id)
	{
		return GetUpgradeCost(Id);
	}

	public string GetUpgradeCostType(int Id)
	{
		return GetProperty ("UpgradeCostType", Id);
	}

	public int GetUpgradeCost(int Id)
	{
		return int.Parse(GetProperty( "UpgradeCost", Id).ToString());
	}

	public string GetPlayerIcon(int Id)
	{
		return GetProperty("PlayerIcon", Id);
	}

	public string GetRoleIcon(int Id)
	{
		return GetProperty("RoleIcon", Id);
	}
	
	#region 角色属性
		
	public float GetRunSpeed(int Id)
	{
		Id = IDTool.GetModelLevel (Id) == 0 ? Id + 1 : Id;
		return float.Parse (GetProperty ("RunSpeed", Id));
	}

	public float GetNextRunSpeed(int Id)
	{
		int modelType = IDTool.GetModelType (Id);
		int maxLevel = GetMaxLevel (Id);
		int curLevel = IDTool.GetModelLevel (Id);
		Id = curLevel == 0 ? Id + 1 : Id;
		int nextId = curLevel < maxLevel ? Id + 1 : modelType * 100 + maxLevel;
		return float.Parse (GetProperty ("RunSpeed", nextId));
	}

	public float GetMaxRunSpeed(int Id)
	{
		int modelType = IDTool.GetModelType (Id);
		int maxLevel = GetMaxLevel (Id);
		int maxId = modelType * 100 + maxLevel;
		return float.Parse (GetProperty ("RunSpeed", maxId));
	}

	public float GetAcceleration(int Id)
	{
		Id = IDTool.GetModelLevel (Id) == 0 ? Id + 1 : Id;
		return float.Parse (GetProperty ("Acceleration", Id));
	}

	public float GetOppRunSpeed(int Id)
	{
		Id = IDTool.GetModelLevel (Id) == 0 ? Id + 1 : Id;
		return float.Parse (GetProperty ("OppRunSpeed", Id));
	}
	
	public float GetOppAcceleration(int Id)
	{
		Id = IDTool.GetModelLevel (Id) == 0 ? Id + 1 : Id;
		return float.Parse (GetProperty ("OppAccleration", Id));
	}

	public float GetNextAcceleration(int Id)
	{
		int modelType = IDTool.GetModelType (Id);
		int maxLevel = GetMaxLevel (Id);
		int curLevel = IDTool.GetModelLevel (Id);
		Id = curLevel == 0 ? Id + 1 : Id;
		int nextId = curLevel < maxLevel ? Id + 1 : modelType * 100 + maxLevel;
		return float.Parse (GetProperty ("Acceleration", nextId));
	}

	public float GetMaxAcceleration(int Id)
	{
		int modelType = IDTool.GetModelType (Id);
		int maxLevel = GetMaxLevel (Id);
		int maxId = modelType * 100 + maxLevel;
		return float.Parse (GetProperty ("Acceleration", maxId));
	}

	public float GetShapeTime(int Id)
	{
		Id = IDTool.GetModelLevel (Id) == 0 ? Id + 1 : Id;
		return float.Parse (GetProperty("ShapeTime", Id));
	}

	public float GetNextShapeTime(int Id)
	{
		int modelType = IDTool.GetModelType (Id);
		int maxLevel = GetMaxLevel (Id);
		int curLevel = IDTool.GetModelLevel (Id);
		Id = curLevel == 0 ? Id + 1 : Id;
		int nextId = curLevel < maxLevel ? Id + 1 : modelType * 100 + maxLevel;
		return float.Parse (GetProperty("ShapeTime", nextId));
	}

	public float GetMaxShapeTime(int Id)
	{
		int modelType = IDTool.GetModelType (Id);
		int maxLevel = GetMaxLevel (Id);
		int maxId = modelType * 100 + maxLevel;
		return float.Parse (GetProperty("ShapeTime", maxId));
	}
	#endregion

	public int GetMaxLevel(int Id)
	{
		return int.Parse(GetProperty("MaxLevel", Id));
	}

	public int GetSkillLevel(int Id)
	{
		return int.Parse(GetProperty("SkillLevel", Id));
	}

	public string GetLevelSkill(int Id)
	{
		return GetProperty("LevelSkill", Id).ToString();
	}

	public string GetLevelSkillIcon(int Id)
	{
		return GetProperty("LevelSkillIcon", Id).ToString();
	}

	public string GetLevelSkillDesc(int Id)
	{
		return GetProperty("LevelSkillDesc", Id).ToString();
	}

	public bool GetIsUse(int Id)
	{
		return bool.Parse(GetProperty("IsUse", Id));
	}
}