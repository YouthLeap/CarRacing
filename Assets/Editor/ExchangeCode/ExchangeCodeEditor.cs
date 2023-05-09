using UnityEngine;
using UnityEditor;
using System.IO;
using System;

/// <summary>
/// 兑换码功能编辑器.
/// </summary>
public class ExchangeCodeEditor : EditorWindow {

	int createCodeNum = 0, codeTypeIndex = 0;
	bool hasHengxian = true;

	void OnFocus()
	{
		ExchangeCodeTool.InitCodeData();
		InitRewardData ();
		codeTypeIndex = 0;
		hasHengxian = ExchangeCodeTool.hasHengXian;
	}

	int[] rewardIds;
	string[] rewardNames;
	int reward1Index = 0, reward2Index = 0, reward3Index = 0;
	int reward1Count = 1000, reward2Count = 1000, reward3Count = 0;
	void InitRewardData()
	{
		if (rewardIds != null)
			return;

		int rewardCount = RewardData.Instance.GetDataRow ();
		rewardIds = new int[rewardCount];
		rewardNames = new string[rewardCount];

		for (int i = 0; i < rewardIds.Length; i++) {
			rewardIds [i] = i + 1;
			rewardNames [i] = RewardData.Instance.GetName (rewardIds[i]);
		}
	}

	void OnGUI()
	{
		EditorGUILayout.BeginVertical();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("刷新数据", GUILayout.Width(100)))
		{
			ExchangeCodeTool.InitCodeData();
		}
		if(GUILayout.Button("删除兑换码文件", GUILayout.Width(100)))
		{
			FileTool.DelectFile(ExchangeCodeTool.codeFileName);
			if(!FileTool.IsFileExists(ExchangeCodeTool.codeFileName))
			{
				ShowNotification(new GUIContent("兑换码文件成功删除。"));
				return;
			}
		}
		if(GUILayout.Button("检测所有兑换码", GUILayout.Width(100)))
		{
			if(!FileTool.IsFileExists(ExchangeCodeTool.codeFileName))
			{
				ShowNotification(new GUIContent("不存在兑换码文件，请先生成。"));
				return;
			}
		
			if(CheckFileExchangeCode())
			{
				EditorUtility.DisplayDialog("注意！", "所有礼包码正常解析！" ,"好的！");
			}
			else
			{
				EditorUtility.DisplayDialog("注意！", "存在不正常礼包码！" ,"好的！");
				EditorApplication.ExecuteMenuItem("Window/Console");
			}
		}

		if (GUILayout.Button ("打开兑换码所在文件夹", GUILayout.Width (150))) 
		{
			EditorUtility.RevealInFinder (Application.dataPath);
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("=========================生成相应平台活动兑换码===========================");
		CreateCodeGUI();
		EditorGUILayout.Space();
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("==========================增加平台活动兑换码==============================");
		AddCodeGUI();
		EditorGUILayout.Space();
		
		EditorGUILayout.EndVertical();
	}
	
	void CreateCodeGUI()
	{
		EditorGUILayout.BeginHorizontal();
		codeTypeIndex = EditorGUILayout.Popup(codeTypeIndex, ExchangeCodeTool.codeTypeList.ToArray(), GUILayout.Width(100));

		if (ExchangeCodeTool.codeTypeList.Count > 0) {
			//描述
			EditorGUILayout.LabelField (ExchangeCodeTool.codeDescArray [ExchangeCodeTool.codeTypeList [codeTypeIndex]], GUILayout.Width (80));

			//有效期
			EditorGUILayout.LabelField (ExchangeCodeTool.codeStartTimeArray [ExchangeCodeTool.codeTypeList [codeTypeIndex]].ToString ("yyyy-MM-dd")
			+ " 到 " + ExchangeCodeTool.codeEndTimeArray [ExchangeCodeTool.codeTypeList [codeTypeIndex]].ToString ("yyyy-MM-dd")
			                           , GUILayout.Width (160));

			//奖励
			string[] rewardInfos = ConvertTool.StringToAnyTypeArray<string> (ExchangeCodeTool.codeRewardContentArray[ExchangeCodeTool.codeTypeList [codeTypeIndex]], '|');
			string content = "";
			for (int i = 0; i < rewardInfos.Length; i++) {
				int id = int.Parse(ConvertTool.StringToAnyTypeArray<string>(rewardInfos[i], '*')[0]);
				int count = int.Parse(ConvertTool.StringToAnyTypeArray<string>(rewardInfos[i], '*')[1]);
				content += RewardData.Instance.GetName (id) + count +"个";
				if(i != rewardInfos.Length - 1)
					content += "和";
			}
			EditorGUILayout.LabelField (content);

		}

		EditorGUILayout.EndHorizontal();
		
		createCodeNum = EditorGUILayout.IntField("兑换码数量:", createCodeNum, GUILayout.Width(200));
		hasHengxian = EditorGUILayout.Toggle("兑换码是否需要横线", hasHengxian);

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("生成兑换码文件", GUILayout.Width(100)))
		{
			if(createCodeNum <= 0)
			{
				ShowNotification(new GUIContent("生成兑换码数量不能为0."));
				return;
			}
            ExchangeCodeTool.hasHengXian = hasHengxian;
			
			if(!FileTool.IsFileExists(ExchangeCodeTool.codeFileName))
			{
				ExchangeCodeTool.GenerateExchangeCode(ExchangeCodeTool.codeTypeList[codeTypeIndex], createCodeNum);
				EditorUtility.RevealInFinder (Application.dataPath);
				if(FileTool.IsFileExists(ExchangeCodeTool.codeFileName))
				{
					ShowNotification(new GUIContent("兑换码文件成功生成。"));
					return;
				}
			}
			else
			{
				if(EditorUtility.DisplayDialog("注意","已经存在一个兑换码的生成文件，需要替换吗？","替换","取消"))
				{
					ExchangeCodeTool.GenerateExchangeCode(ExchangeCodeTool.codeTypeList[codeTypeIndex], createCodeNum);
					EditorUtility.RevealInFinder (Application.dataPath);
				}
			}
		}

		/*  删除功能不开放
		GUI.contentColor = Color.red;

		if(GUILayout.Button("删除此平台活动码", GUILayout.Width(100)))
		{
			if(ExchangeCodeTool.codeTypeList.Count < 2)
			{
				ShowNotification(new GUIContent("仅剩一个活动兑换码了！"));
				return;
			}

			#if !Unuse_For_Build

			CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/ExchangeCodeData.csv");
			csv.RemoveRow(codeTypeIndex);
			csv.SaveCSV();
			AssetDatabase.Refresh();

			#endif

			ExchangeCodeData.Instance.RefreshData();
			ExchangeCodeTool.InitCodeData();
			codeTypeIndex = 0;

			ShowNotification(new GUIContent("删除成功！"));
		}
		GUI.contentColor = Color.white;
		*/

		EditorGUILayout.EndHorizontal();

	}
	
	
	string codeTypeTemp = "1001", startTimeTemp = "2015-10-21 00:00:00", endTimeTemp = "2015-12-21 00:00:00", desc = "活动";
	void AddCodeGUI()
	{
		EditorGUILayout.BeginHorizontal();
		codeTypeTemp = EditorGUILayout.TextField("兑换码类型:", codeTypeTemp, GUILayout.Width(300));
		EditorGUILayout.LabelField("（2位平台编号）+（2位活动编号）");
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		startTimeTemp = EditorGUILayout.TextField("开始时间:", startTimeTemp, GUILayout.Width(300));
		EditorGUILayout.LabelField("格式 : 2015-07-01 12:34:45");
		EditorGUILayout.EndHorizontal();

		endTimeTemp = EditorGUILayout.TextField("有效结束时间:", endTimeTemp, GUILayout.Width(300));

		desc = EditorGUILayout.TextField("兑换码描述:", desc, GUILayout.Width(300));

		EditorGUILayout.BeginHorizontal();
		reward1Index = EditorGUILayout.Popup(reward1Index, rewardNames, GUILayout.Width(100));
		reward1Count = EditorGUILayout.IntField("第一种奖励数量:", reward1Count, GUILayout.Width(300));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		reward2Index = EditorGUILayout.Popup(reward2Index, rewardNames, GUILayout.Width(100));
		reward2Count = EditorGUILayout.IntField("第二种奖励数量:", reward2Count, GUILayout.Width(300));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		reward3Index = EditorGUILayout.Popup(reward3Index, rewardNames, GUILayout.Width(100));
		reward3Count = EditorGUILayout.IntField("第三种奖励数量:", reward3Count, GUILayout.Width(300));
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.Space();

		if(GUILayout.Button("增加平台活动兑换码", GUILayout.Width(200)))
		{
			if(string.IsNullOrEmpty(codeTypeTemp) || string.IsNullOrEmpty(startTimeTemp) || string.IsNullOrEmpty(endTimeTemp) || string.IsNullOrEmpty(desc))
			{
				ShowNotification(new GUIContent("任何字段不能为空。"));
				return;
			}
			
			if(ExchangeCodeData.Instance.CheckHasSameCode(codeTypeTemp))
			{
				ShowNotification(new GUIContent("已经存在相同活动码！"));
				return;
			}

			string rewardContent = "";
			if (reward1Count > 0)
				rewardContent += rewardIds [reward1Index] + "*" + reward1Count + "|";
			if (reward2Count > 0)
				rewardContent += rewardIds [reward2Index] + "*" + reward2Count + "|";
			if (reward3Count > 0)
				rewardContent += rewardIds [reward3Index] + "*" + reward3Count + "|";

			if (string.IsNullOrEmpty (rewardContent)) {
				ShowNotification (new GUIContent ("第一种奖励的数量不能小于0！"));
				return;
			} else {
				rewardContent = rewardContent.TrimEnd ('|');
			}
			
			#if !Unuse_For_Build
			
			CSVFileTool csv = new CSVFileTool(Application.dataPath +"/Resources/Data/ExchangeCodeData.csv");
			//Debug.Log(desc);
			csv.AddNewRow((ExchangeCodeData.Instance.GetDataRow()+1).ToString(), codeTypeTemp, startTimeTemp, endTimeTemp, desc, rewardContent);
			csv.SaveCSV();
			AssetDatabase.Refresh();
			
			#endif
			
			ExchangeCodeData.Instance.RefreshData();
			ExchangeCodeTool.InitCodeData();
			
			ShowNotification(new GUIContent("增加成功！"));
		}
	}


	/// <summary>
	/// 检测文件里面的礼包码是否对.
	/// </summary>
	bool CheckFileExchangeCode()
	{
		string fileContent, rewardContent; 
		StreamReader sr = null;
		try{
			sr = File.OpenText(FileTool.RootPath + ExchangeCodeTool.codeFileName);
		}
		catch(Exception e){
			Debug.Log(e.Message);
			return false;
		}
		int i = 0;
		fileContent = sr.ReadLine();
		while (fileContent != null || !string.IsNullOrEmpty(fileContent))
		{
			if(fileContent.Length < ExchangeCodeTool.codeLenght)
				break;

			if(!ExchangeCodeTool.CheckExchangeCode(fileContent.Replace("-","").ToLower().Substring(0, ExchangeCodeTool.codeLenght), out rewardContent))
			{
				Debug.Log("Index : "+ i +" , Wrong Code : " + fileContent);
				sr.Close ();
				sr.Dispose ();
				return false;
			}
			fileContent = sr.ReadLine();
			i++;

			if(i > 100000)
				break;
		}

		Debug.Log("All Code Count : " + i);

		sr.Close ();
		sr.Dispose ();
		return true;
	}
	
}
