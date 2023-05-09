using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PagePoint : MonoBehaviour {

	public Transform tranCurPage;
	public GameObject goBackImage;
	public Grid gird;
	
	private Vector3[] backPos;
	private int curPageIndex = 0;
	
	public void Init(int pageNum, int pageIndex = 0, bool firstInit = false)
	{
		if(firstInit)
		{
			for(int i = 1; i < pageNum; i++)
			{
				GameObject backObj = (GameObject)Instantiate(goBackImage);
				backObj.transform.SetParent(goBackImage.transform.parent, false);
			}
			goBackImage.transform.localPosition = new Vector3 (- gird.cellWidth * (pageNum - 1) / 2, 0, 0);
			backPos = gird.ApplySortEffect();
		}
		
		curPageIndex = pageIndex;
		tranCurPage.localPosition = backPos[curPageIndex];
	}
	
	public void NextPage()
	{
		curPageIndex ++;
		
		if(curPageIndex >= backPos.Length)
			curPageIndex = 0;
			
		tranCurPage.localPosition = backPos[curPageIndex];
	}
	
	public void PrePage()
	{
		curPageIndex --;
		
		if(curPageIndex < 0)
			curPageIndex = backPos.Length - 1;
		
		tranCurPage.localPosition = backPos[curPageIndex];
	}
}
