using UnityEngine;
using System.Collections;
using UnityEditor;

public class ModifyModlePosEditor : Editor {

	[MenuItem("Tools/Modify Modle Child Pos", false)]
	static void ModifyPos()
	{

		if(Selection.activeObject != null)
		{
			Transform[] tran = Selection.transforms;
			for(int i = 0; i < tran.Length; i++)
			{
				Transform[] childTran = tran[i].GetComponentsInChildren<Transform>();

				for(int j = 0; j < childTran.Length; j++)
				{
					if(childTran[j].name == "huatan")
					{
						childTran[j].localPosition = new Vector3(childTran[j].localPosition.x,childTran[j].localPosition.y,-7.5f);
					}
					else if(childTran[j].name == "a")
					{
						childTran[j].localPosition = new Vector3(childTran[j].localPosition.x,childTran[j].localPosition.y,7.5f);
					}
					else if(childTran[j].name == "yan")
					{
						childTran[j].localPosition = new Vector3(childTran[j].localPosition.x,childTran[j].localPosition.y,0);
					}
				}
			}
		}
	}
}
