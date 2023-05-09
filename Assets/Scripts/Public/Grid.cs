using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal,
		Vertical,
		Grid,
		Page,
		Circle,
	}

	public Arrangement arrangement = Arrangement.Horizontal;

	//宽高
	public float cellWidth = 200f;
	public float cellHeight = 200f;
	
	//田字格排序
	public int GridWidthNum = 0;

	//圆形排序
	public float radiu = 10;
	public Vector3 centerPos = Vector3.zero;

	//页面排序
	public int EachPageItemWidthNum = 0;//每个页面的宽个数
	public int EachPageItemNum = 0;//每个页面的个数
	[HideInInspector]
	public int PageCount = -1;
	
	public bool sortedByName = false;
	public bool IsInOrder = true;//是否顺序排列
	public bool IsMiddleInOrder = false; //是否已中间为标准排列
	
	static public int SortByName (Transform a, Transform b) { return string.Compare(a.name, b.name); }

	
	public Vector3[] ApplySortEffect ()
	{
		Transform myTrans = transform;
		int childCount = myTrans.childCount;
		
		if(childCount == 0)
			return null;
		
		Vector3[] childPos = new Vector3[childCount];

		//页面排序
		if(arrangement == Arrangement.Page)
		{
			if(EachPageItemNum == 0 || EachPageItemWidthNum == 0)
				return null;
			
			int x = 0, y = -1;
			PageCount = -1;
			for(int i = 0; i < childCount; i ++)
			{
				if(i % EachPageItemNum == 0)
				{
					PageCount ++;
					y = -1;
				}
			
				if(i % EachPageItemWidthNum == 0)
				{
					y ++;
					x = 0;
				}
				
				Transform t = myTrans.GetChild(i);
				
				t.localPosition = myTrans.GetChild(0).localPosition + new Vector3(cellWidth * x + PageCount * EachPageItemWidthNum * cellWidth, - cellHeight * y);
				
				x ++;
				
				childPos[i] = myTrans.GetChild(i).localPosition;
			}
			
			PageCount ++;
			
			return childPos;
		}

		//圆形排序
		if (arrangement == Arrangement.Circle) {
			if (radiu == 0)
				return null;

			for (int i = 0; i < childCount; i++) {
				
				Transform t = myTrans.GetChild(i);

				t.localPosition = CalculateAngleToPoint(centerPos, radiu, i * (2*Mathf.PI / childCount));

				childPos[i] = t.localPosition;
			}

			return childPos;
		}

		//田字格排序
		if(arrangement == Arrangement.Grid)
		{
			if(GridWidthNum == 0)
				return null;
		
			int x = 0, y = -1;
			for(int i = 0; i < childCount; i ++)
			{
				if(i % GridWidthNum == 0)
				{
					y ++;
					x = 0;	
				}
			
				Transform t = myTrans.GetChild(i);
				
				t.localPosition = myTrans.GetChild(0).localPosition + new Vector3(cellWidth * x, - cellHeight * y);
				
				x ++;
				
				childPos[i] = myTrans.GetChild(i).localPosition;
				
			}
			
			return childPos;
		}

		//横向或者竖向排序
		if (sortedByName)
		{
			List<Transform> list = new List<Transform>();
			
			for (int i = 0; i < childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				if (t) list.Add(t);
			}
			list.Sort(SortByName);
			
			for (int i = 0, imax = list.Count; i < imax; ++i)
			{
				Transform t = list[i];
				
				float depth = t.localPosition.z;
				if(arrangement == Arrangement.Horizontal)
				{
					if(IsInOrder)
						t.localPosition = new Vector3(myTrans.GetChild(0).localPosition.x + cellWidth * i, myTrans.GetChild(0).localPosition.y, depth);
					else
						t.localPosition = new Vector3(myTrans.GetChild(0).localPosition.x - cellWidth * i, myTrans.GetChild(0).localPosition.y, depth);
				}
				else
				{
					if(IsInOrder)
						t.localPosition = new Vector3(myTrans.GetChild(0).localPosition.x, myTrans.GetChild(0).localPosition.y - cellHeight * i, depth);
					else
						t.localPosition = new Vector3(myTrans.GetChild(0).localPosition.x, myTrans.GetChild(0).localPosition.y + cellHeight * i, depth);
				}

				childPos[i] = myTrans.GetChild(i).localPosition;

			}
		}
		else
		{
			if(IsMiddleInOrder)
			{
				if(arrangement == Arrangement.Horizontal)
				{
					myTrans.GetChild(0).localPosition = new Vector3(myTrans.GetChild(0).localPosition.x - (myTrans.childCount - 1) * cellWidth / 2, myTrans.GetChild(0).localPosition.y, myTrans.GetChild(0).localPosition.z);
				}
				else
				{
					myTrans.GetChild(0).localPosition = new Vector3(myTrans.GetChild(0).localPosition.x, myTrans.GetChild(0).localPosition.y + (myTrans.childCount - 1) * cellHeight / 2, myTrans.GetChild(0).localPosition.z);
				}
			}
		
			for (int i = 0; i < childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				
				float depth = t.localPosition.z;
				if(arrangement == Arrangement.Horizontal)
				{
					if(IsInOrder)
						t.localPosition = new Vector3(myTrans.GetChild(0).localPosition.x + cellWidth * i, myTrans.GetChild(0).localPosition.y, depth);
					else
						t.localPosition = new Vector3(myTrans.GetChild(0).localPosition.x - cellWidth * i, myTrans.GetChild(0).localPosition.y, depth);
				}
				else
				{
					if(IsInOrder)
						t.localPosition = new Vector3(myTrans.GetChild(0).localPosition.x, myTrans.GetChild(0).localPosition.y - cellHeight * i, depth);
					else
						t.localPosition = new Vector3(myTrans.GetChild(0).localPosition.x, myTrans.GetChild(0).localPosition.y + cellHeight * i, depth);
				}
				
				childPos[i] = myTrans.GetChild(i).localPosition;
				
			}
		}

		return childPos;
	}

	Vector3 CalculateAngleToPoint(Vector3 center, float radius, float theta)
	{
		Vector3 result = new Vector3(center.x + radius*Mathf.Cos(theta), center.y + radius*Mathf.Sin(theta), 0);
		return result;
	}

}
