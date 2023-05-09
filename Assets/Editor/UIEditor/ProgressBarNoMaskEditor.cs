using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProgressBarNoMask))]
public class ProgressBarNoMaskEditor : Editor {

	ProgressBarNoMask pb;
	string[] selectOptions = {"改变X值","改变Y值"};

	public override void OnInspectorGUI ()
	{
		pb = (ProgressBarNoMask)target;

		pb.changeType = (ProgressBarNoMask.ChangeType)EditorGUILayout.Popup("改变的类型 : ", (int)pb.changeType, selectOptions);
		pb.changeSprite = (tk2dSlicedSprite)EditorGUILayout.ObjectField (pb.changeSprite, typeof(tk2dSlicedSprite), true);
		pb.tiledSprite = (tk2dTiledSprite)EditorGUILayout.ObjectField (pb.tiledSprite, typeof(tk2dTiledSprite), true);

		if (pb.changeType == ProgressBarNoMask.ChangeType.ChangeX) {
			pb.minX = EditorGUILayout.FloatField("最小X值",pb.minX);
			pb.maxX = EditorGUILayout.FloatField("最大X值",pb.maxX);
		} else {
			pb.minY = EditorGUILayout.FloatField("最小Y值",pb.minY);
			pb.maxY = EditorGUILayout.FloatField("最大Y值",pb.maxY);
		}
	}
}
