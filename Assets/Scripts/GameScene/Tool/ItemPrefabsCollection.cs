using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ItemPrefabsCollection : MonoBehaviour {


	public Transform getItemTransformByName(string name)
	{
		SpawnPool pool = GetComponent<SpawnPool>();
		for (int i =0; i <pool._perPrefabPoolOptions.Count; ++i) {
			
			try{
				
				if(pool._perPrefabPoolOptions[i].prefab.name == name)
				{
					Transform newTrans = Instantiate(pool._perPrefabPoolOptions[i].prefab);
					return newTrans;
				}
			}catch{
				Debug.LogError("error in  "+ pool._perPrefabPoolOptions[i].prefab.name);
			}
			
		}
		
		return null;
	}


	#if UNITY_EDITOR
	public bool isRreshPrefab;
	// Update is called once per frame
	void Update () {
		
		if (isRreshPrefab) {
			isRreshPrefab = false;
			
			SpawnPool pool = GetComponent<SpawnPool>();
			pool._perPrefabPoolOptions.Clear();
			
			string objPath1  = Application.dataPath + "/" + "Prefab/GameScene/Prop/"; 
			string objPath2 = Application.dataPath + "/" + "Prefab/GameScene/Traffic/"; 
			
			string[] directoryEntries;
			List<string> objPaths = new List<string>();
			List<Transform> transList = new List<Transform>();
			try
			{
				directoryEntries = System.IO.Directory.GetFileSystemEntries(objPath1);
				for(int i = 0; i < directoryEntries.Length ; i ++)
				{  
					string p = directoryEntries[i];  
					if(p.EndsWith(".meta"))
						continue;
					p = p.Substring(p.IndexOf("Assets"));
					objPaths.Add(p);
				}
				
				directoryEntries = System.IO.Directory.GetFileSystemEntries(objPath2);
				for(int i = 0; i < directoryEntries.Length ; i ++)
				{  
					string p = directoryEntries[i];  
					if(p.EndsWith(".meta"))
						continue;
					p = p.Substring(p.IndexOf("Assets"));
					objPaths.Add(p);
				}
			}
			catch
			{
				
			}
			
			transList.Clear();
			for(int i = 0; i < objPaths.Count; i++)
			{
				
				Transform trans = AssetDatabase.LoadAssetAtPath<Transform>(objPaths[i]);
				pool._perPrefabPoolOptions.Add( new PrefabPool(trans) );
			}

			for(int i=0;i<pool._perPrefabPoolOptions.Count;++i)
			{
				Debug.Log(pool._perPrefabPoolOptions[i].prefab.name);
			}

			
			//			roadprefabs = null;
			//			roadprefabs = transList.ToArray();
			//			
			
			PrefabUtility.ReplacePrefab(gameObject, PrefabUtility.GetPrefabParent(gameObject), ReplacePrefabOptions.ConnectToPrefab);
			
		}
	}
	#endif




	
}
