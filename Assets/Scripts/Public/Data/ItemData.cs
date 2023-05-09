using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemData : IData {

	private ItemData()
	{
		InitData("Data/ItemData");

	}
	
	private static ItemData instance;
	public static ItemData Instance
	{
		get
		{
			if(instance == null)
				instance = new ItemData();
			
			return instance;
		}
	}

	public void RefreshData()
	{
		//Debug.Log ("RefreshData ");
		InitData("Data/ItemData");
	}

	public string GetEditName(int Id)
	{
		return GetProperty("EditName", Id);
	}
	public string GetPrefableName(int Id)
	{
		return GetProperty("PrefableName", Id);
	}
	public EditItemType GetPrefableItemType(int Id)
	{
		return (EditItemType)System.Enum.Parse(typeof(EditItemType),GetProperty("ItemType", Id)) ;
	}
}

public enum EditItemType{  PickupProp,RoadProp, Obstacle,Traffic, Other};

