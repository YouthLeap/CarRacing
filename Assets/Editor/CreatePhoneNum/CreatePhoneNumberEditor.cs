using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CreatePhoneNumberEditor : EditorWindow {

	string PrefixDataPath = Application.dataPath + "/Resources/Data/RandomPhoneNumData/PhoneNumPrefixData.csv";
	string PhoneNumDataPath = Application.dataPath + "/Resources/Data/RandomPhoneNumData/PhoneNumData.csv";
	
	List<string> NumList = new List<string>();

	string HiddenChar = "*";
	int HiddenCharAmounnt = 4;
	int PhoneNumAmount = 0;
	string CompleteCountStr = "0"; //用于显示
	int CompleteCount = 0; //用于计数
	bool IsCreating = false;

#if !Unuse_For_Build
	CSVFileTool PrefixData, NumData;


	#region 编辑器方法
	[MenuItem ("MenuTool/生成随机手机号码", false)]
	static void OpenConfigWindow () {
		CreatePhoneNumberEditor window = (CreatePhoneNumberEditor)EditorWindow.GetWindow (typeof (CreatePhoneNumberEditor));
	}
	
	[MenuItem ("MenuTool/生成随机手机号码", true)]
	static bool ValidateOpenConfigData()
	{
		return true;
	}

	void OnFocus()
	{
		PrefixData = new CSVFileTool(PrefixDataPath);
		NumData = new CSVFileTool(PhoneNumDataPath);
	}

	void OnGUI()
	{
		GUILayout.Label ("随机手机号码生成配置", EditorStyles.boldLabel);

		PhoneNumAmount = EditorGUILayout.IntField("号码总数量", PhoneNumAmount, GUILayout.Height(20));
		HiddenChar     = EditorGUILayout.TextField("隐藏数据代替字符", HiddenChar, GUILayout.Height(20));
		HiddenCharAmounnt = EditorGUILayout.IntField("隐藏字符个数", HiddenCharAmounnt, GUILayout.Height(20));

		GUILayout.Space(25);

		if(GUILayout.Button("生成"))
		{
			if(IsCreating)
				return;

			IsCreating = true;

		    CreatePhoneNum();
		}

		GUILayout.Space(25);
		GUILayout.Space(25);

		EditorGUILayout.TextField("生成进度", CompleteCountStr, GUILayout.Height(20));


	}
	#endregion

	#region 生成号码
	/// <summary>
	/// 读表生成前缀
	/// </summary>
	/// <returns>The prefix.</returns>
	string CreatePrefix()
	{
		int prefixId = Random.Range(1, PrefixData.rowCount + 1);

		return GetPrefixByID(prefixId.ToString(), PrefixData);
	}

	/// <summary>
	/// 生成后缀
	/// </summary>
	/// <returns>The postfix.</returns>
	string CreatePostfix()
	{
		int minNum = 0;
		int postfixLength = 11 - 3 - HiddenCharAmounnt;
		if(postfixLength != 1)
			minNum = Mathf.RoundToInt(Mathf.Pow(10, postfixLength - 1));

		int postfix = Random.Range(minNum, minNum * 10);

		return postfix.ToString();
	}

	void CreatePhoneNum()
	{
		string hiddenChars = "";
		CompleteCount = 0;

		for(int i = 0; i < HiddenCharAmounnt; i ++)
			hiddenChars += HiddenChar;

		NumList.Clear();
		for(int i = 0; i < PhoneNumAmount; i ++)
		{
			string temp = CreatePrefix() + hiddenChars + CreatePostfix();
			while(CheckPhoneNumEnabled(temp) == false)
				temp = CreatePrefix() + hiddenChars + CreatePostfix();

			if(NumList.Count > i)
			    NumList[i] = temp;
			else
				NumList.Add(temp);

			CompleteCount ++;

			WriteConfigData(i, NumList[i]);

			if(CompleteCount == PhoneNumAmount)
			{
				CompleteCountStr = "完成";
				IsCreating = true;
			}else
			{
				CompleteCountStr = CompleteCount.ToString();
			}
		}

		for(int i = NumData.rowCount; i >= PhoneNumAmount; i --)
			DeleteUselessData(i);

		SaveData();
	}

	bool CheckPhoneNumEnabled(string Num)
	{
		if(Num.Length != 11)
			return false;
		if(NumList.Contains(Num))
			return false;

		return true;
	}
	#endregion

	#region 文件处理方法
	string GetPrefixByID(string ID, CSVFileTool csv)
	{
		string content = "";
		for (int i=1; i<=csv.rowCount; ++i) {
			
			if(csv[i,1] == ID)
			{
				content= csv[i,2];
				
				break;
			}
		}
		csv.SaveCSV ();
		
		return content;
	}

	/// <summary>
	/// 把配置信息写入文件
	/// </summary>
	void WriteConfigData(int ListId, string PhoneNum)
	{

		if (ListId < NumData.rowCount) {
			NumData[(ListId + 1),2]= PhoneNum;
		} else {
			string[] newRow = {(ListId + 1).ToString(),PhoneNum};
			NumData.AddNewRow (newRow);
		}
	}

	/// <summary>
	/// 删除多余数据
	/// </summary>
	void DeleteUselessData(int Id)
	{
		NumData.RemoveRow(Id);
	}

	void SaveData()
	{
		NumData.SaveCSV ();
		AssetDatabase.Refresh ();
	}
    #endregion

	#endif
}
