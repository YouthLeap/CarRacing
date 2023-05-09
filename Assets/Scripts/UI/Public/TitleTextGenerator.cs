using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleTextGenerator : MonoBehaviour {

	[HideInInspector][SerializeField]
	public string sTitleText = "标题";
	public bool bApplyNow = false;
	[HideInInspector][SerializeField]
	public Text[] textTextArray;
	[HideInInspector][SerializeField]
	public Transform[] tranTextBGArray;

	public Sprite imageTextBG;
	public Font fontTextFont;
	[HideInInspector][SerializeField]
	public int iFontSize = 45;
	[HideInInspector][SerializeField]
	public float fCharSpacing = 2;

	int iChildrenCount, iTextLength, iMidLength;
	bool bDoubleChar;

	// Use this for initialization
	void Awake () {
	}

	public void RebuildText()
	{
		if(bApplyNow)
			bApplyNow = !bApplyNow;
		else
			return;

		iChildrenCount = transform.childCount;
		iTextLength = sTitleText.Length;
		iMidLength = iTextLength / 2;
		bDoubleChar = iTextLength % 2 == 0 ? true : false;

		tranTextBGArray = new Transform[iTextLength];
		textTextArray = new Text[iTextLength];

//		Debug.Log("iTextLength: " + iTextLength + "  iChildrenCount: " +  iChildrenCount);
		for(int i = 0; i < Mathf.Max(iTextLength, iChildrenCount); i ++)
		{
			if(i >= iTextLength && i < iChildrenCount)
			{
				DestroyImmediate(transform.Find("Text" + i).gameObject);
				continue;
			}

			if(i >= iChildrenCount && i < iTextLength)
			{
				GameObject goTextBG = new GameObject("Text" + i);
				goTextBG.transform.parent = transform;
				goTextBG.transform.localPosition = Vector3.zero;
				goTextBG.AddComponent<Image>();
				tranTextBGArray[i] = goTextBG.transform;

				GameObject goText = new GameObject("Text");
				goText.transform.parent = goTextBG.transform;
				goText.transform.localPosition = Vector3.zero;
				textTextArray[i] = goText.AddComponent<Text>();
				textTextArray[i].fontSize = iFontSize;
				textTextArray[i].alignment = TextAnchor.MiddleCenter;
				goText.AddComponent<Shadow>();
			}
		
			tranTextBGArray[i] = transform.GetChild(i);
			Image bgImage = tranTextBGArray[i].GetComponent<Image>();
			bgImage.sprite = imageTextBG;
			tranTextBGArray[i].GetComponent<RectTransform>().sizeDelta = new Vector2(bgImage.preferredWidth, bgImage.preferredHeight);
//			Debug.Log(imageTextBG.name);
//			Debug.Log(sTitleText.ToCharArray()[i].ToString());
			textTextArray[i] = transform.GetChild(i).GetChild(0).GetComponent<Text>();
			textTextArray[i].font = fontTextFont;
			textTextArray[i].text = sTitleText.ToCharArray()[i].ToString();
		}

		SetHorizontalPosition();
	}

	void SetHorizontalPosition()
	{
		for(int i = 0; i < iTextLength; i ++)
		{
			if(bDoubleChar)
			{
				if(i + 1 <= iMidLength)
				{
					float fY = tranTextBGArray[iMidLength - (i+1)].transform.localPosition.y, fZ = tranTextBGArray[iMidLength - (i+1)].transform.localPosition.z;

					if(i == 0)
						tranTextBGArray[iMidLength - (i+1)].transform.localPosition = new Vector3( - 0.5f * fCharSpacing * textTextArray[iMidLength - (i+1)].fontSize, fY, fZ );
					else
						tranTextBGArray[iMidLength - (i+1)].transform.localPosition = new Vector3(tranTextBGArray[iMidLength - (i+1) + 1].transform.localPosition.x - 0.5f * fCharSpacing * (textTextArray[iMidLength - (i+1) + 1].fontSize + textTextArray[iMidLength - (i+1)].fontSize), fY, fZ );
				}else
				{
					float fY = tranTextBGArray[i].transform.localPosition.y, fZ = tranTextBGArray[i].transform.localPosition.z;

					if(i == iMidLength)
						tranTextBGArray[i].transform.localPosition = new Vector3( 0.5f * fCharSpacing * textTextArray[i].fontSize, fY, fZ );
					else
						tranTextBGArray[i].transform.localPosition = new Vector3( tranTextBGArray[i - 1].transform.localPosition.x + 0.5f * fCharSpacing * (textTextArray[i - 1].fontSize + textTextArray[i].fontSize), fY, fZ );
				}
			}else
			{
				if(i == 0)
				{
					float fY = tranTextBGArray[iMidLength].transform.localPosition.y, fZ = tranTextBGArray[iMidLength].transform.localPosition.z;
					
					tranTextBGArray[iMidLength].transform.localPosition = new Vector3(0, fY, fZ );
				}
				else if(i <= iMidLength)
				{
					float fY = tranTextBGArray[iMidLength - i].transform.localPosition.y, fZ = tranTextBGArray[iMidLength - i].transform.localPosition.z;

					tranTextBGArray[iMidLength - i].transform.localPosition = new Vector3( tranTextBGArray[iMidLength - i + 1].transform.localPosition.x - 0.5f * fCharSpacing * (textTextArray[iMidLength - i + 1].fontSize + textTextArray[iMidLength - i].fontSize), fY, fZ );
				}else if(i > iMidLength)
				{
					float fY = tranTextBGArray[i].transform.localPosition.y, fZ = tranTextBGArray[i].transform.localPosition.z;

					tranTextBGArray[i].transform.localPosition = new Vector3( tranTextBGArray[i - 1].transform.localPosition.x + 0.5f * fCharSpacing * (textTextArray[i - 1].fontSize + textTextArray[i].fontSize), fY, fZ );
				}
			}
	    }
	}
}
