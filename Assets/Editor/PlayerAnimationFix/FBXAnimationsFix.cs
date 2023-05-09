using UnityEditor;  
using UnityEngine;
using PathologicalGames;
using UnityEditor.Animations;
using System.Collections.Generic;

/// <summary>
/// 奥特曼模型的导入切割动画脚本.
/// </summary>
public class FBXAnimationsFix : AssetPostprocessor
{

	#region 动作文件同步到其他五个奥特曼.
	
	static string[] playerName = new string[]{"zhuzhuxia", "feifei", "xiaodaidai", "bobi", "chaorenqiang"};
	static Dictionary<string,Vector3> playerPos = new Dictionary<string, Vector3>{{"zhuzhuxia",new Vector3(0,0.3f,0.03f)},{"feifei",new Vector3(0,0.24f,-0.18f)},{"xiaodaidai",new Vector3(0,0f,0.2f)},
		{"bobi",new Vector3(0,0,-0f)},{"chaorenqiang",new Vector3(0,0f,-0f)}};
	static Dictionary<string,float> modelScale = new Dictionary<string, float>{{"zhuzhuxia",1},{"feifei",1},{"xiaodaidai",1},{"bobi",1},{"chaorenqiang",1},
		{"tiequanhujijia",1.5f},{"huoyanhejijia",1.4f},{"shenmuyuanjijia",1.4f},{"shijiaxiongjijia",1.4f},{"xiaodaidaijijia",1.4f},
		{"boss",3f}};

	[MenuItem("Assets/Refresh All Player AnimatorController")]
	static void CopyAnimController()
	{
		string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		
		string last = selectedPath.Substring(selectedPath.Substring(0, selectedPath.LastIndexOf('/')).LastIndexOf('/') + 1);
		string selectedName = last.Remove(last.LastIndexOf('/'));
		
		for(int i = 0; i < playerName.Length; i++)
		{
			if(playerName[i].CompareTo(selectedName) == 0)
				continue;
			
			string copyPath = selectedPath.Replace(selectedName, playerName[i]);
			
			AssetDatabase.DeleteAsset(copyPath);
			AssetDatabase.CopyAsset(selectedPath, copyPath);
			
			AssetDatabase.Refresh();
			
			AnimationClipConfig.isOpen = true;
			AnimationClipConfig.isRefreshController = true;
			AssetDatabase.ImportAsset(copyPath.Remove(copyPath.LastIndexOf('/') + 1)+playerName[i]+".FBX");
			
			AssetDatabase.Refresh();
			
		}
		
		AssetDatabase.Refresh();
	}
	
	[MenuItem("Assets/Refresh All Player AnimatorController", true)]
	static bool ValidateCopyAnimController()
	{
		if(Selection.activeObject == null)
			return false;
		return Selection.activeObject.GetType() == typeof(AnimatorController);
	}
	
#endregion

	[MenuItem("Assets/Split Animation")]
	private static void SplitFBXAnimation()
	{
		AnimationClipConfig.isOpen = true;
		AnimationClipConfig.isRefreshController = false;
		EditorApplication.ExecuteMenuItem("Assets/Reimport");
	}
	
	[MenuItem("Assets/Split Animation", true)]
	private static bool ValidateSplitFBXAnimation()
	{
		if(Selection.activeObject == null)
			return false;
		return Selection.activeObject.GetType() == typeof(GameObject);
	}
	
	[MenuItem("Assets/Split Animation And Refresh Controller")]
	private static void SplitFBXAnimationAndRefreshController()
	{
		AnimationClipConfig.isOpen = true;
		AnimationClipConfig.isRefreshController = true;
		EditorApplication.ExecuteMenuItem("Assets/Reimport");
	}
	
	[MenuItem("Assets/Split Animation And Refresh Controller", true)]
	private static bool ValidateSplitFBXAnimationAndRefreshController()
	{
		if(Selection.activeObject == null)
			return false;
		return Selection.activeObject.GetType() == typeof(GameObject);
	}

	static List<GameObject> newImportObjList = new List<GameObject>();

	//模型导入前.
	public void OnPreprocessModel()  
	{

		if(!AnimationClipConfig.isOpen)
			return;
			
		string fbxName = assetPath.Substring(assetPath.LastIndexOf('/')).ToLower().Replace("/","").Replace(".fbx","");
		
		//当前正在导入的模型.
		ModelImporter modelImporter = (ModelImporter) assetImporter;  
		
		AnimationClipConfig.init();  

		foreach (AnimationClipConfig.modelST item in AnimationClipConfig.modelList)  
		{
			//当前导入模型的路径包含我们modelST动作数据表中的模型名字，那就要对这个模型的动画进行切割.
			if (fbxName.CompareTo(item.ModelName) == 0)
			{
				
				if(IsJustImportAnimation(fbxName))
				{
					modelImporter.importMaterials = false;
					modelImporter.normalImportMode = ModelImporterTangentSpaceMode.None;
					modelImporter.tangentImportMode = ModelImporterTangentSpaceMode.None;
				}
				else
				{
					modelImporter.importMaterials = true;
					modelImporter.normalImportMode = ModelImporterTangentSpaceMode.None;
					modelImporter.tangentImportMode = ModelImporterTangentSpaceMode.None;
				}
				
				modelImporter.meshCompression = ModelImporterMeshCompression.High;
				modelImporter.animationType = ModelImporterAnimationType.Generic;
				modelImporter.optimizeGameObjects = true;
				
				ModelImporterClipAnimation[] animations = new ModelImporterClipAnimation[item.animationSTList.Count];
				for (int i = 0; i < item.animationSTList.Count; i++)  
				{  
					animations[i] = SetHumanoidClipAnimation(item.animationSTList[i].name, item.animationSTList[i].firstFrame, item.animationSTList[i].lastFrame, item.animationSTList[i].loop
					                                         , item.animationSTList[i].lockRotation, item.animationSTList[i].lockY, item.animationSTList[i].lockXZ);
				}  
				
				modelImporter.clipAnimations = animations;
				
				modelImporter.globalScale = modelScale[fbxName];
				modelImporter.importBlendShapes = false;
				modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
				
			}
		}
		
		
	}
	
	int countChange = 0;
	//模型导入后.
	public void OnPostprocessModel(GameObject g)
	{
	
		if(!AnimationClipConfig.isOpen)
			return;
		
		if(!AnimationClipConfig.isRefreshController)
			return;
		
		string fbxName = g.name;
		
		if(IsJustImportAnimation(fbxName))
			return;
		if (!IsAnimationObj ())
			return;
		
		List<AnimationClip> clipList = new List<AnimationClip>();
		clipList.Clear();
		Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
		foreach (Object obj in objects)
		{
			AnimationClip clip = obj as AnimationClip;
			if (clip != null)
			{
				clipList.Add(clip);
			}
		}
		
		//导入fbx之后生成相应预设.
		string prefabPath = "";
		string animControllerPath = "";
		GameObject objToPrefab;
		GameObject prefabObj;
		AnimatorController animController;
		
		if(IsMecha(fbxName))
		{
			prefabPath = "Assets/Prefab/Characters/"  + fbxName + ".prefab";
			animControllerPath = assetPath.Remove(assetPath.LastIndexOf('/') + 1)  + fbxName + "AnimController.controller";
			objToPrefab = AssetDatabase.LoadAssetAtPath(assetPath,typeof(GameObject)) as GameObject;
			prefabObj = PrefabUtility.CreatePrefab(prefabPath, objToPrefab, ReplacePrefabOptions.Default);
			prefabObj.AddComponent<MechaAnimatorManager>();
			//更新AnimationController
			animController = AssetDatabase.LoadAssetAtPath(animControllerPath,typeof(AnimatorController)) as AnimatorController;
			prefabObj.GetComponent<Animator>().runtimeAnimatorController = animController;
			prefabObj.GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;

		}
		else if(IsBoss(fbxName))
		{
			prefabPath = "Assets/Prefab/GameScene/Traffic/" + fbxName + ".prefab";
			animControllerPath = assetPath.Remove(assetPath.LastIndexOf('/') + 1) + fbxName + "AnimController.controller";
			objToPrefab = AssetDatabase.LoadAssetAtPath(assetPath,typeof(GameObject)) as GameObject;
			prefabObj = PrefabUtility.CreatePrefab(prefabPath, objToPrefab, ReplacePrefabOptions.Default);
			//prefabObj.AddComponent<BossTrafficAnimControllor>();
			animController = AssetDatabase.LoadAssetAtPath(animControllerPath,typeof(AnimatorController)) as AnimatorController;
			prefabObj.GetComponent<Animator>().runtimeAnimatorController = animController;
			prefabObj.GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
		else
		{
			prefabPath = "Assets/Prefab/Characters/" + fbxName + ".prefab";
			animControllerPath = assetPath.Remove(assetPath.LastIndexOf('/') + 1) + fbxName + "AnimController.controller";


			GameObject playerObj = AssetDatabase.LoadAssetAtPath(assetPath,typeof(GameObject)) as GameObject;
			string carPath = assetPath.Remove(assetPath.LastIndexOf('/') + 1) + fbxName + "che.fbx";
			GameObject carPbj = AssetDatabase.LoadAssetAtPath(carPath,typeof(GameObject)) as GameObject;

			objToPrefab = EditorUtility.CreateGameObjectWithHideFlags(fbxName,0);
			GameObject tempPlayerObj = GameObject.Instantiate(playerObj);
			GameObject tempCarObj = GameObject.Instantiate(carPbj);
			tempCarObj.name = fbxName + "che";
			tempCarObj.transform.parent = objToPrefab.transform;
			tempCarObj.transform.localPosition = Vector3.zero;
			tempCarObj.transform.localScale = Vector3.one;
			tempCarObj.transform.localRotation = Quaternion.identity;
			AddPlayerWheelsRotationComment(tempCarObj.transform,"wheel");
			tempPlayerObj.transform.parent = objToPrefab.transform;
			tempPlayerObj.name = fbxName;
			tempPlayerObj.transform .localPosition = playerPos[fbxName];
			tempPlayerObj.transform.localRotation = Quaternion.identity;
			tempPlayerObj.transform.localScale = Vector3.one;

			tempPlayerObj.AddComponent<AnimatorManager>();
			//更新AnimationController
			animController = AssetDatabase.LoadAssetAtPath(animControllerPath,typeof(AnimatorController)) as AnimatorController;
			tempPlayerObj.GetComponent<Animator>().runtimeAnimatorController = animController;
			tempPlayerObj.GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
			prefabObj = PrefabUtility.CreatePrefab(prefabPath, objToPrefab, ReplacePrefabOptions.Default);
			GameObject.DestroyImmediate(objToPrefab);
		}
		countChange = 0;
		for(int i = 0; i < animController.layers.Length; i++)
		{
			CheckAndRefreshAnimatorController(clipList.ToArray(), animController.layers[i].stateMachine);
		}
		if(countChange == clipList.Count)
			Debug.Log("Success to replace the animation!");
		else
			Debug.LogError("There is somthing miss! Change Count : " + countChange +" clipList Count : " + clipList.Count);


		
		/*
		prefabObj.transform.GetChild(0).gameObject.layer = 9;
		prefabObj.transform.GetChild(1).gameObject.layer = 9;
		prefabObj.layer = 9;
		
		//生成预设就查找到相对应的材质球，修改材质球的属性.
		SkinnedMeshRenderer skin;
		for(int i = 0; i < prefabObj.transform.childCount; i++)
		{
			if((skin = prefabObj.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>()) != null)
			{
				skin.sharedMaterial.shader = Shader.Find("Custom/RimLightCurved");
				skin.sharedMaterial.SetVector("_QOffset", new Vector4(-6,-5,0,0));
				skin.sharedMaterial.SetFloat("_Brightness", 1);
				skin.sharedMaterial.SetTexture("_MainTex",AssetDatabase.LoadAssetAtPath(assetPath.Remove(assetPath.IndexOf('.'))+".png",typeof(Texture)) as Texture);
			}
		}
		*/
		
		//怪兽要更新预设
		if(IsBoss(fbxName))
		{
			string bossPrefabPath = "Assets/Prefab/GameScene/Traffic/" + "BossTraffic.prefab";
			//把预设生成到场景里，进行GameObject的更改
			GameObject bossGameObj = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(bossPrefabPath)) as GameObject;
			for(int i = 0; i < bossGameObj.transform.childCount; i++)
			{
				if(bossGameObj.transform.GetChild(i).name.CompareTo(fbxName) == 0)
				{
					GameObject clonePrefabObj = PrefabUtility.InstantiatePrefab(prefabObj) as GameObject;
					clonePrefabObj.transform.parent = bossGameObj.transform;
					clonePrefabObj.transform.localPosition = bossGameObj.transform.GetChild(i).localPosition;
					clonePrefabObj.transform.localScale = bossGameObj.transform.GetChild(i).localScale;
					clonePrefabObj.transform.localRotation = bossGameObj.transform.GetChild(i).localRotation;
					clonePrefabObj.name = fbxName;
					
					GameObject.DestroyImmediate(bossGameObj.transform.GetChild(i).gameObject, true);
					
					Debug.Log("Success to replace the monster prefab !");
					break;
				}
				
				if(i == (bossGameObj.transform.childCount - 1))
				{
					Debug.LogError("path: "+ bossGameObj+" , Fail to find the monster prefab !");
				}
			}
			
			PrefabUtility.ReplacePrefab(bossGameObj, PrefabUtility.GetPrefabParent(bossGameObj), ReplacePrefabOptions.ConnectToPrefab);
			
			AssetDatabase.DeleteAsset(prefabPath);
			GameObject.DestroyImmediate(bossGameObj);
		}
		else
		{
			//导入之后把预设关联到场景对象池上.
			GameObject poolObj = GameObject.Find("CharactersPool");
			if(poolObj == null)
			{
				Debug.LogError("GameObject Pool Can't be Found!");
				return;
			}
			SpawnPool pool = poolObj.GetComponent<SpawnPool>();
			bool hasAdd = false;
			for(int i = 0; i < pool._perPrefabPoolOptions.Count; i++)
			{
				if(pool._perPrefabPoolOptions[i].prefab == null)
				{
					pool._perPrefabPoolOptions[i].prefab = prefabObj.transform;
					hasAdd = true;
					break;
				}
			}
			if(!hasAdd)
			{
				PrefabPool newOne = new PrefabPool();
				newOne.prefab = prefabObj.transform;
				pool._perPrefabPoolOptions.Add(newOne);
			}
			
			PrefabUtility.ReplacePrefab(poolObj, PrefabUtility.GetPrefabParent(poolObj), ReplacePrefabOptions.ConnectToPrefab);

		}
		
		AnimationClipConfig.isOpen = false;
		
	}
	
	//不能用递归，因为用不了ref.
	void CheckAndRefreshAnimatorController(AnimationClip[] newClips, AnimatorStateMachine stateMachine)
	{
		for(int i = 0; i < stateMachine.states.Length; i++) 
		{
			ChildAnimatorState childState = stateMachine.states[i];
			if(childState.state.motion == null)
			{
				if(childState.state.name.CompareTo("New State") == 0)
					continue;
				
				Debug.LogError("Null : " + childState.state.name);
				continue;
			}
			if(childState.state.motion.GetType() == typeof(AnimationClip))
			{
				for(int j = 0; j < newClips.Length; j++)
				{
					if(newClips[j].name.CompareTo(childState.state.motion.name) == 0)
					{
						childState.state.motion = (Motion)newClips[j];
						countChange ++;
						break;
					}
				}
			}else if(childState.state.motion.GetType() == typeof(BlendTree))
			{
			
				//BlendTree这个类有BUG，不能直接修改Motion, 要先记录原本的信息，再全部删除原本的，再修改，再加上去.
				
				List<Motion> allMotion = new List<Motion>();
				List<float> allThreshold = new List<float>();
				BlendTree tree = (BlendTree)childState.state.motion;
				
				for(int k = 0; k < tree.children.Length; k++)
				{
					allMotion.Add(tree.children[k].motion);
					allThreshold.Add(tree.children[k].threshold);
				}
				
				for(int k = 0; k < allMotion.Count; k++)
				{
					if(allMotion[k].GetType() == typeof(AnimationClip))
					{
						for(int j = 0; j < newClips.Length; j++)
						{
							if(newClips[j].name.CompareTo(allMotion[k].name) == 0)
							{
								allMotion[k] = (Motion)newClips[j];
								countChange ++;
								break;
							}
						}
					}else if(allMotion[k].GetType() == typeof(BlendTree))
					{
						Debug.LogError("You need to change it!");
					}
				}
				
				for(int k = tree.children.Length - 1; k >= 0; k--)
				{
					tree.RemoveChild(k);
				}
				
				for(int k = 0; k < allMotion.Count; k++)
				{
					tree.AddChild(allMotion[k], allThreshold[k]);
				}
				
			}
		}
		
		for(int i = 0; i < stateMachine.stateMachines.Length; i++) 
		{
			CheckAndRefreshAnimatorController(newClips, stateMachine.stateMachines[i].stateMachine);
		}
	}
	
	ModelImporterClipAnimation SetHumanoidClipAnimation(string _name, int _first, int _last, bool loop, bool lockRotation, bool lockY, bool lockXZ)  
	{  
		ModelImporterClipAnimation tempClip = new ModelImporterClipAnimation();  
		tempClip.name = _name;  
		tempClip.firstFrame = _first;  
		tempClip.lastFrame = _last;
		tempClip.loopTime = loop;
		tempClip.lockRootRotation = lockRotation;
		tempClip.lockRootHeightY = lockY;
		tempClip.lockRootPositionXZ = lockXZ;
		
		tempClip.keepOriginalPositionY = true;
		tempClip.keepOriginalOrientation = true;
		tempClip.keepOriginalPositionXZ = true;
		
//		tempClip.events = animEvent;
		
		return tempClip;  
	}

	bool IsAnimationObj()
	{
		Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
		foreach (Object obj in objects)
		{
			AnimationClip clip = obj as AnimationClip;
			if (clip != null)
				return true;
		}
		return false;
	}

	bool IsJustImportAnimation(string fbxName)
	{
		if(fbxName.CompareTo("tongyong") == 0 || fbxName.Contains("diejia"))
			return true;
		return false;
	}
	
	bool IsMecha(string fbxName)
	{
		return fbxName.Contains("jijia");
	}
	bool IsBoss(string fbxName)
	{
		return fbxName.Contains ("boss");
	}
	void AddPlayerWheelsRotationComment(Transform trans, string name)
	{
		foreach(Transform child in trans)
		{
//			if(child.name.Contains(name))
//			{
//				child.gameObject.AddComponent<PlayerWheelsRotation>();
//			}
			AddPlayerWheelsRotationComment(child,name);
		}
	}
}