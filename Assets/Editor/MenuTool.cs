using UnityEngine;
using UnityEditor;
using System.IO;
using PathologicalGames;
using UnityEditor.Callbacks;

public class MenuTool {

	const string MenuRoot = "MenuTool/";

	#region 兑换码生成检测功能.
	
	[MenuItem(MenuRoot + "兑换码生成功能", false)]
	static void ShowExchangeCodeWindow()
	{
		EditorWindow.GetWindowWithRect<ExchangeCodeEditor>(new Rect(Screen.width / 2, 100, 600, 500), true, "兑换码功能窗口", true).Show();
	}
	
	#endregion;

	[MenuItem(MenuRoot + "Clean ExchangeActivityData")]
	static void CleanExchangeActivityData()
	{
		if (FileTool.IsFileExists ("ExchangeActivityData"))
			FileTool.DelectFile ("ExchangeActivityData");
	}
	
	[MenuItem(MenuRoot + "Clean Player Data", false, 10101)]
	static void CleanPlayerData()
	{
		if(ValidatePlayerDataExists())
			FileTool.DelectFile("PlayerData");
		AssetDatabase.Refresh ();
	}
	[MenuItem(MenuRoot + "Clean Player Data", true)]
	static bool ValidatePlayerDataExists()
	{
		return FileTool.IsFileExists("PlayerData");
	}
	
	[MenuItem(MenuRoot + "Clean Pay Data", false, 10102)]
	static void CleanPayData()
	{
		FileTool.DelectFile("ChangWan");
		FileTool.DelectFile("LiBaoHei");
		FileTool.DelectFile("LiBaoBai");
		FileTool.DelectFile("ShenHe");
		FileTool.DelectFile("XiaoMiShenHe");
		FileTool.DelectFile("XiaoMiTuiGuang");
		FileTool.DelectFile("GuangDian");
		AssetDatabase.Refresh ();
	}

	[MenuItem(MenuRoot + "Clean Pay Data",true)]
	static bool ValidatePayDataExists()
	{
		return FileTool.IsFileExists ("ChangWan")
		|| FileTool.IsFileExists ("LiBaoHei")
		|| FileTool.IsFileExists ("LiBaoBai")
		|| FileTool.IsFileExists ("ShenHe")
		|| FileTool.IsFileExists ("XiaoMiShenHe")
		|| FileTool.IsFileExists ("XiaoMiTuiGuang")
		|| FileTool.IsFileExists ("GuangDian");
	}

	[MenuItem(MenuRoot + "Animator Controller #`", false)]
	static void OpenAnimController()
	{
		EditorApplication.ExecuteMenuItem("Window/Animator");
	}

	[MenuItem(MenuRoot + "Change Texture Type", false)]
	static void ChangeTextureType()
	{
		string[] textureArr = AssetDatabase.FindAssets("t:Texture", GetSelectedPath());
		for (int i = 0; i < textureArr.Length; i++) {
			string pathName = AssetDatabase.GUIDToAssetPath (textureArr [i]);
			EditorUtility.DisplayProgressBar ("Compress Texture", pathName, (i * 1.0f / textureArr.Length));
			
			//修改图片导入设置，可以读写图片的像素信息。
			TextureImporter improter = TextureImporter.GetAtPath (pathName) as TextureImporter;
			if(improter.textureType != TextureImporterType.Sprite)
			{
				improter.textureType = TextureImporterType.Sprite;
				improter.spritePixelsPerUnit = 100;
				improter.mipmapEnabled = false;
				improter.filterMode = FilterMode.Bilinear;
				improter.maxTextureSize = 2048;
				improter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				improter.spritePackingTag = "UI";
				improter.SaveAndReimport();
			}
			else if(improter.spritePackingTag.CompareTo("UI") != 0 || improter.spritePackingTag.CompareTo("BG") != 0)
			{
				improter.spritePackingTag = "UI";
				improter.SaveAndReimport();
			}
		}
		EditorUtility.ClearProgressBar ();
	}
	
	private static string[] GetSelectedPath()
	{
		string[] path = {"Assets/Image/NewUI"};
		foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
			path[0] = AssetDatabase.GetAssetPath(obj);
			if(!string.IsNullOrEmpty(path[0]) && File.Exists(path[0]))
			{
				path[0] = Path.GetDirectoryName(path[0]);
				break;
			}
		}
		return path;
	}

	private static string[] UI_ModulePath = { "Assets/Prefab/UI/UISceneUI/Modules", "Assets/Prefab/UI/Public/Modules" };
	private static string[] UI_ItemPath = { "Assets/Prefab/UI/UISceneUI/Items", "Assets/Prefab/UI/Public/Items" };

	private static string[] Game_ModulePath = { "Assets/Prefab/UI/GameSceneUI/Modules", "Assets/Prefab/UI/Public/Modules" };
	private static string[] Game_ItemPath = { "Assets/Prefab/UI/GameSceneUI/Items", "Assets/Prefab/UI/Public/Items" };

	[MenuItem(MenuRoot + "UIScene/Fix UIModulesPool", false)]
	static void FixUIModulesPool()
	{
		GameObject spUIModules = GameObject.Find("UIModulesPool");
		if (spUIModules == null) {
			Debug.Log("UIModulesPool Can't Be Found!");
			return;
		}
		SpawnPool pool = spUIModules.GetComponent<SpawnPool> ();
		string[] prefabArr = AssetDatabase.FindAssets("t:Prefab", UI_ModulePath);
		if (prefabArr == null || prefabArr.Length <= 0) {
			Debug.Log("Prefab Can't Be Found!");
			return;
		}
		pool._perPrefabPoolOptions.Clear ();
		for(int i=0; i<prefabArr.Length; ++i)
		{
			string pathName = AssetDatabase.GUIDToAssetPath (prefabArr [i]);
			Transform prefabTran = AssetDatabase.LoadAssetAtPath<Transform>(pathName);
			
			PrefabPool prefabPool = new PrefabPool(prefabTran);
			pool._perPrefabPoolOptions.Add(prefabPool);
		}
		
		PrefabUtility.ReplacePrefab (spUIModules, PrefabUtility.GetPrefabParent (spUIModules), ReplacePrefabOptions.ConnectToPrefab);
	}
	
	[MenuItem(MenuRoot + "UIScene/Fix UIItemsPool", false)]
	static void FixUIItemsPool()
	{
		GameObject spUIItems = GameObject.Find("UIItemsPool");
		if (spUIItems == null) {
			Debug.Log("UIItemsPool Can't Be Found!");
			return;
		}
		SpawnPool pool = spUIItems.GetComponent<SpawnPool> ();
		string[] prefabArr = AssetDatabase.FindAssets("t:Prefab", UI_ItemPath);
		if (prefabArr == null || prefabArr.Length <= 0) {
			Debug.Log("Prefab Can't Be Found!");
			return;
		}
		pool._perPrefabPoolOptions.Clear ();
		for(int i=0; i<prefabArr.Length; ++i)
		{
			string pathName = AssetDatabase.GUIDToAssetPath (prefabArr [i]);
			Transform prefabTran = AssetDatabase.LoadAssetAtPath<Transform>(pathName);
			
			PrefabPool prefabPool = new PrefabPool(prefabTran);
			pool._perPrefabPoolOptions.Add(prefabPool);
		}
		
		PrefabUtility.ReplacePrefab (spUIItems, PrefabUtility.GetPrefabParent (spUIItems), ReplacePrefabOptions.ConnectToPrefab);
	}

	[MenuItem(MenuRoot + "GameScene/Fix UIModulesPool", false)]
	static void FixGameModulesPool()
	{
		GameObject spUIModules = GameObject.Find("UIModulesPool");
		if (spUIModules == null) {
			Debug.Log("UIModulesPool Can't Be Found!");
			return;
		}
		SpawnPool pool = spUIModules.GetComponent<SpawnPool> ();
		string[] prefabArr = AssetDatabase.FindAssets("t:Prefab", Game_ModulePath);
		if (prefabArr == null || prefabArr.Length <= 0) {
			Debug.Log("Prefab Can't Be Found!");
			return;
		}
		pool._perPrefabPoolOptions.Clear ();
		for(int i=0; i<prefabArr.Length; ++i)
		{
			string pathName = AssetDatabase.GUIDToAssetPath (prefabArr [i]);
			Transform prefabTran = AssetDatabase.LoadAssetAtPath<Transform>(pathName);
			
			PrefabPool prefabPool = new PrefabPool(prefabTran);
			pool._perPrefabPoolOptions.Add(prefabPool);
		}
		
		PrefabUtility.ReplacePrefab (spUIModules, PrefabUtility.GetPrefabParent (spUIModules), ReplacePrefabOptions.ConnectToPrefab);
	}
	
	[MenuItem(MenuRoot + "GameScene/Fix UIItemsPool", false)]
	static void FixGameItemsPool()
	{
		GameObject spUIItems = GameObject.Find("UIItemsPool");
		if (spUIItems == null) {
			Debug.Log("UIItemsPool Can't Be Found!");
			return;
		}
		SpawnPool pool = spUIItems.GetComponent<SpawnPool> ();
		string[] prefabArr = AssetDatabase.FindAssets("t:Prefab", Game_ItemPath);
		if (prefabArr == null || prefabArr.Length <= 0) {
			Debug.Log("Prefab Can't Be Found!");
			return;
		}
		pool._perPrefabPoolOptions.Clear ();
		for(int i=0; i<prefabArr.Length; ++i)
		{
			string pathName = AssetDatabase.GUIDToAssetPath (prefabArr [i]);
			Transform prefabTran = AssetDatabase.LoadAssetAtPath<Transform>(pathName);
			
			PrefabPool prefabPool = new PrefabPool(prefabTran);
			pool._perPrefabPoolOptions.Add(prefabPool);
		}
		
		PrefabUtility.ReplacePrefab (spUIItems, PrefabUtility.GetPrefabParent (spUIItems), ReplacePrefabOptions.ConnectToPrefab);
	}

	#region 只是为了用快捷键打开版本控制器.
	[MenuItem(MenuRoot + "Version Control %#v", false)]
	static void VersionControl()
	{
		EditorApplication.ExecuteMenuItem("Window/Version Control");
	}
	
	#endregion
	
	#region 为了在编译前把没用的东西删除
	[MenuItem(MenuRoot + "Clean Before Build", false)]
	static void CleanBeforeBuild()
	{		
		//删除对于项目没用的库.
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "Unuse_For_Build");
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "Unuse_For_Build");
		AssetDatabase.DeleteAsset("Assets/Plugins/System.Data.dll");
		PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
		PlayerSettings.strippingLevel = StrippingLevel.UseMicroMSCorlib;
		AssetDatabase.Refresh();
	}
	[MenuItem(MenuRoot + "Clean Before Build", true)]
	static bool VaildateCleanBeforeBuild()
	{
		return FileTool.IsFileExists("Assets/Plugins/System.Data.dll");
	}

	[MenuItem(MenuRoot + "Clean NewPlayer", false)]
	static void  VaildateCleanNewPlayer()
	{
		PlayerData.Instance.SetNewPlayerToFalse ();
	}
	#endregion

	#region 换TK2D的快捷键
	/*
	[MenuItem(MenuRoot + "Key/Deleta All Compnont #1", false, 2)]
	static void DeletaAllCompnont()
	{
		Component[] cs = Selection.activeTransform.GetComponents<Component>();

		for(int i = cs.Length-1; i > 0; i--) {	
			MonoBehaviour.DestroyImmediate (cs[i]);
		}
	}

	[MenuItem(MenuRoot + "Key/Add tk2d Sprite #2", false, 2)]
	static void AddTK2DSprite()
	{
		Selection.activeGameObject.AddComponent<tk2dSprite> ();
	}

	[MenuItem(MenuRoot + "Key/Add tk2d UIItem #4", false, 2)]
	static void AddTK2DUIItem()
	{
		tk2dUIItem item = Selection.activeGameObject.AddComponent<tk2dUIItem> ();
		Selection.activeGameObject.AddComponent<BoxCollider> ();
	}

	[MenuItem(MenuRoot + "Key/Add EasyFont #3", false, 2)]
	static void AddEasyFont()
	{
		EasyFontTextMesh ea = Selection.activeGameObject.AddComponent<EasyFontTextMesh> ();
		ea.Textanchor = EasyFontTextMesh.TEXT_ANCHOR.MiddleCenter;
		ea.Textalignment = EasyFontTextMesh.TEXT_ALIGNMENT.center;
	}
*/
	#endregion
	
}
