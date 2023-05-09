using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ColorEditor {

	public class ColorData
	{
		public string name;
		public Color color;
	}

	[MenuItem("UIEditor/Create Color")]
	static void Build()
	{
		//复制一份新的模板到newColorPath目录下
		string templateColorPath="Assets/Editor/ColorEditor/color.colors";
		string newColorPath="Assets/Editor/自定义颜色.colors";
		AssetDatabase.DeleteAsset(newColorPath);
		AssetDatabase.CopyAsset(templateColorPath,newColorPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		
		
		//这里我写了两条临时测试数据
		List<ColorData>colorList = new List<ColorData>(){
			new ColorData(){name ="白色", color = new Color(1,1,1,1)},
			new ColorData(){name ="浅蓝边框",color = new Color(54/255.0f, 182/255.0f, 201/255.0f, 1)},
			new ColorData(){name ="蓝色边框",color = new Color(0, 146/255.0f, 192/255.0f, 1)},
			new ColorData(){name ="绿色边框",color = new Color(0, 119/255.0f, 23/255.0f, 1)},
			new ColorData(){name ="深蓝边框",color = new Color(0, 85/255.0f, 116/255.0f, 1)},
			new ColorData(){name ="深黄边框",color = new Color(193/255.0f, 91/255.0f, 28/255.0f, 1)},
		};
		
		UnityEngine.Object newColor = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(newColorPath);
		SerializedObject serializedObject = new SerializedObject(newColor);
		SerializedProperty property = serializedObject.FindProperty("m_Presets");
		property.ClearArray();
		
		//把我的测试数据写进去
		for(int i =0; i< colorList.Count; i++){
			property.InsertArrayElementAtIndex(i);
			SerializedProperty colorsProperty = property.GetArrayElementAtIndex(i);
			colorsProperty.FindPropertyRelative("m_Name").stringValue = colorList[i].name;
			colorsProperty.FindPropertyRelative("m_Color").colorValue = colorList[i].color;
		}
		//保存
		serializedObject.ApplyModifiedProperties();
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}
