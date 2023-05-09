using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Move))]
public class MoveEditor : Editor {

	Move move;
	string[] selectOptions = {"左","右","上","下"};
	public override void OnInspectorGUI ()
	{
		move = (Move)target;
		move.direction = (Move.Direction)EditorGUILayout.Popup("移动类型:", (int)move.direction, selectOptions);
		move.width = EditorGUILayout.FloatField ("拼接宽度:", move.width);
		move.speed = EditorGUILayout.FloatField ("移动速度:", move.speed);
	}

}
