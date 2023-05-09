using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  

/// <summary>
/// 奥特曼模型的动画切割脚本.
/// </summary>
public static class AnimationClipConfig  
{  
	/// <summary>
	/// 是否开启导入模型剪切动作功能.
	/// </summary>
	public static bool isOpen = false;
	public static bool isRefreshController = false;
	public static List<modelST> modelList = new List<modelST>();  
	
	public static void init()  
	{  
		modelList.Clear();
		ModelAnimationData.Instance.RefreshData();
		
		int dataCount = 0;
		string curModelName = "";
		modelST model = null;
		
		dataCount = ModelAnimationData.Instance.GetDataRow();
		for(int i = 1; i <= dataCount; i++)
		{
			if(string.IsNullOrEmpty(ModelAnimationData.Instance.GetAnimationName(i)))
				continue;
		
			animationST animationTemp = new animationST();
			animationTemp.name = ModelAnimationData.Instance.GetAnimationName(i);
			animationTemp.firstFrame = ModelAnimationData.Instance.GetFirstFrame(i);
			animationTemp.lastFrame = ModelAnimationData.Instance.GetLastFrame(i);
//			animationTemp.wrapMode = ModelAnimationData.Instance.GetWrapMode(i);
			animationTemp.loop = ModelAnimationData.Instance.GetLoop(i);
			animationTemp.lockRotation = ModelAnimationData.Instance.GetLockRotation(i);
			animationTemp.lockY = ModelAnimationData.Instance.GetLockY(i);
			animationTemp.lockXZ = ModelAnimationData.Instance.GetLockXZ(i);
			
//			string callBack = ModelAnimationData.Instance.GetCallBack(i);
//			if(!string.IsNullOrEmpty(callBack))
//			{
//				string[] eventParams = callBack.Split('*');
//				AnimationEvent animEvent = new AnimationEvent();
//				animEvent.functionName = eventParams[0];
//				animEvent.time = float.Parse(eventParams[1]);
//				animEvent.stringParameter = eventParams[2];
//				animationTemp.animEventList.Add(animEvent);
//			}
			
		
			if(curModelName.CompareTo(ModelAnimationData.Instance.GetModelName(i)) != 0)
			{
				curModelName = ModelAnimationData.Instance.GetModelName(i);
				
				model = new modelST();
				model.ModelName = curModelName;
				modelList.Add(model);
			}
			
			if(model != null)
				model.animationSTList.Add(animationTemp);
		}
	}  
	
	#region ST  
	public class animationST  
	{  
		public string name;  
		public int firstFrame;  
		public int lastFrame;  
		public WrapMode wrapMode;
		public bool loop;
		public bool lockRotation;
		public bool lockY;
		public bool lockXZ;
		public List<AnimationEvent> animEventList = new List<AnimationEvent>();
	}  
	
	public class modelST{  
		public string ModelName;  
		public List<animationST> animationSTList = new List<animationST>();
	}  
	#endregion  
} 