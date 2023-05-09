using UnityEngine;
using System.Collections;

public class ShowSystemInfo : MonoBehaviour {
	float eachY = 80;
	void OnGUI()
	{
		GUI.skin.label.normal.textColor = new Color (0f, 0f, 0f, 1f);

		GUI.Label(new Rect(500, 0, 400, 200), "SystemInfo.deviceModel:" + SystemInfo.deviceModel);
		GUI.Label(new Rect(500, eachY*1, 400, 200), "SystemInfo.deviceName:" + SystemInfo.deviceName);
		GUI.Label(new Rect(500, eachY*2, 400, 200), "SystemInfo.deviceType:" + SystemInfo.deviceType);
		GUI.Label(new Rect(500, eachY*3, 400, 200), "SystemInfo.graphicsDeviceID:" + SystemInfo.graphicsDeviceID);
		GUI.Label(new Rect(500, eachY*4, 400, 200), "SystemInfo.graphicsDeviceName:" + SystemInfo.graphicsDeviceName);
		GUI.Label(new Rect(500, eachY*5, 400, 200), "SystemInfo.graphicsDeviceType:" + SystemInfo.graphicsDeviceType);
		GUI.Label(new Rect(500, eachY*6, 400, 200), "SystemInfo.graphicsDeviceVendor:" + SystemInfo.graphicsDeviceVendor);

		GUI.Label(new Rect(100, 0, 400, 200), "SystemInfo.graphicsDeviceVendorID:" + SystemInfo.graphicsDeviceVendorID);
		GUI.Label(new Rect(100, eachY*1, 400, 200), "SystemInfo.graphicsDeviceVersion:" + SystemInfo.graphicsDeviceVersion);
		GUI.Label(new Rect(100, eachY*2, 400, 200), "SystemInfo.graphicsMemorySize:" + SystemInfo.graphicsMemorySize);
		GUI.Label(new Rect(100, eachY*3, 400, 200), "SystemInfo.processorCount:" + SystemInfo.processorCount);
		GUI.Label(new Rect(100, eachY*4, 400, 200), "SystemInfo.processorType:" + SystemInfo.processorType);
		GUI.Label(new Rect(100, eachY*5, 400, 200), "SystemInfo.systemMemorySize:" + SystemInfo.systemMemorySize);
		
	}
}
