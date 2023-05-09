using UnityEngine;
using System.Collections;

public static class QualitySetting {

	public static bool IsHighQuality
	{
		get
		{
			return deviceQualityLevel == DeviceQualityLevel.High;
		}
	}

	public static DeviceQualityLevel deviceQualityLevel = DeviceQualityLevel.High;
	
	
	public static void Init()
	{
#if UNITY_EDITOR
		deviceQualityLevel = DeviceQualityLevel.High;
		return;
#endif

#if UNITY_IOS
		deviceQualityLevel = DeviceQualityLevel.High;
		return;
#endif

		if (SystemInfo.graphicsDeviceVersion.ToLower ().Contains ("opengl es 2") || SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3) {
			deviceQualityLevel = DeviceQualityLevel.Low;
			return;
		}
	
		if(SystemInfo.graphicsMemorySize > 180 && SystemInfo.systemMemorySize > 1024 && SystemInfo.processorCount >= 2)
			deviceQualityLevel = DeviceQualityLevel.High;
		else
			deviceQualityLevel = DeviceQualityLevel.Middle;
			
	}
}

public enum DeviceQualityLevel
{
	High = 0,
	Middle,
	Low
}
