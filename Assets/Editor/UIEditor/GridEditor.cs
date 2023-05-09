using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {
	Grid grid;
	string[] selectOptions = {"横排列","竖排列","田字格排列","页面排列","圆形排列"};

	public override void OnInspectorGUI ()
	{
		grid = (Grid)target;
		
		grid.arrangement = (Grid.Arrangement)EditorGUILayout.Popup("排列类型 : ", (int)grid.arrangement, selectOptions);
		
		DrawDiffGUI((int)grid.arrangement);
		
		if(GUILayout.Button("排序"))
		{
			grid.ApplySortEffect();
		}
	}
	
	void DrawDiffGUI(int index)
	{
		switch(index)
		{
		case 0:
			grid.cellWidth = EditorGUILayout.FloatField("单项宽度",grid.cellWidth);
			grid.IsInOrder = EditorGUILayout.Toggle("顺序排列", grid.IsInOrder);
			grid.IsMiddleInOrder = EditorGUILayout.Toggle("是否中间排列", grid.IsMiddleInOrder);
			break;
		case 1:
			grid.cellHeight = EditorGUILayout.FloatField("单项高度",grid.cellHeight);
			grid.IsInOrder = EditorGUILayout.Toggle("顺序排列", grid.IsInOrder);
			grid.IsMiddleInOrder = EditorGUILayout.Toggle("是否中间排列", grid.IsMiddleInOrder);
			break;
		case 2:
			grid.cellWidth = EditorGUILayout.FloatField("单项宽度",grid.cellWidth);
			grid.cellHeight = EditorGUILayout.FloatField("单项高度",grid.cellHeight);
			grid.GridWidthNum = EditorGUILayout.IntField("横排个数", grid.GridWidthNum);
			break;
		case 3:
			grid.cellWidth = EditorGUILayout.FloatField("单项宽度",grid.cellWidth);
			grid.cellHeight = EditorGUILayout.FloatField("单项高度",grid.cellHeight);
			grid.EachPageItemWidthNum = EditorGUILayout.IntField("每页横排个数", grid.EachPageItemWidthNum);
			grid.EachPageItemNum = EditorGUILayout.IntField("每页个数", grid.EachPageItemNum);
			EditorGUILayout.LabelField("页面个数：", grid.PageCount.ToString());
			break;
		case 4:
			grid.radiu = EditorGUILayout.FloatField ("圆形半径", grid.radiu);
			grid.centerPos = EditorGUILayout.Vector3Field ("圆点位置", grid.centerPos);
			break;
		}
	}
}
