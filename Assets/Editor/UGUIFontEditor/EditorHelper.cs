using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class EditorHelper : MonoBehaviour {

	[MenuItem("Assets/BatchCreateArtistFont", false)]
	static public void BatchCreateArtistFont()
	{
		ArtistFont.BatchCreateArtistFont();
	}
	
	[MenuItem("Assets/BatchCreateArtistFont", true)]
	static public bool ValidataBatchCreateArtistFont()
	{
		string dirName = "";
		string selectedFileName = EditorUtils.SelectObjectPathInfo(ref dirName);
		return selectedFileName.EndsWith(".fnt");
	}
}
