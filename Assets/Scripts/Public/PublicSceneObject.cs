using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PublicSceneObject : MonoSingletonBase<PublicSceneObject>
{

    private bool isReceiveAndroidBackKeyEvent;
    public bool IsReceiveAndroidBackKeyEvent
    {
        get
        {
            return isReceiveAndroidBackKeyEvent;
        }
        set
        {
            isReceiveAndroidBackKeyEvent = value;
            //print(value);
        }
    }

    ///*Android返回键的委托事件*/
    public delegate void AndroidBackKeyEventHandler();
    public event AndroidBackKeyEventHandler AndroidBackKeyEvent;

    void Awake()
    {
        Screen.fullScreen = false;
    }

    public void ClearAndroidBackKeyEvent()
    {
        AndroidBackKeyEvent = null;
    }

    public override void Init()
    {
        DontDestroyOnLoad(gameObject);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        QualitySetting.Init();

        Application.targetFrameRate = 30;
    }

    /// <summary>
    /// 当游戏退出到后台.
    /// </summary>
    /// <param name="pauseStatus">If set to <c>true</c> pause status.</param>
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Application.targetFrameRate = 0;
            PlayerData.Instance.SaveData();
            ExchangeActivityData.Instance.SaveData();
        }
        else
        {
            Application.targetFrameRate = 30;
        }
    }

    /*
	void OnApplicationQuit()
	{
		PlayerData.Instance.SaveData ();
	}
	*/

    void OnDisable()
    {
        PlayerData.Instance.SaveData();
    }


#if UNITY_ANDROID || UNITY_IOS

    void Update()
    {
        /*Android	返回键*/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsReceiveAndroidBackKeyEvent)
                return;

            if (AndroidBackKeyEvent != null)
            {
                AndroidBackKeyEvent();
            }
        }

        ///*Android Home键*/
        if (Input.GetKeyDown(KeyCode.Home))
        {
        }

        #region 电视TV的适配

        if (Input.GetKeyDown(KeyCode.Return))
        {
            KeyBoardHandle(KeyOrder.Enter);
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            KeyBoardHandle(KeyOrder.LeftDown);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            KeyBoardHandle(KeyOrder.LeftUp);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            KeyBoardHandle(KeyOrder.RightDown);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            KeyBoardHandle(KeyOrder.RightUp);
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            KeyBoardHandle(KeyOrder.Up);
            Input.ResetInputAxes();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            KeyBoardHandle(KeyOrder.Down);
            Input.ResetInputAxes();
        }






        if (Input.GetKeyDown(KeyCode.JoystickButton0)) //中间按键
        {
            KeyBoardHandle(KeyOrder.Enter);
        }

        #endregion
    }


    #region 电视TV的适配

    enum KeyOrder
    {
        Up,
        Down,
        LeftDown,
        LeftUp,
        RightDown,
        RightUp,
        Enter,
        Back,
        Attack,
        Jump,
    }

    [SerializeField]
    class FocusPoint //: MonoBehaviour
    {
        public Transform focusPointTrans;
        public tk2dUIItem curFocusItem;

        public FocusPoint(Transform focusTrans)
        {
            this.focusPointTrans = focusTrans;
            this.focusPointTrans.parent = null;
            this.focusPointTrans.gameObject.SetActive(false);
        }

        public void SetCurrentFocusItem(tk2dUIItem item)
        {
            this.curFocusItem = item;
            if (LevelSelectControllor.Instance != null && LevelSelectControllor.Instance.gameObject.activeInHierarchy)
            {
                LevelSelectControllor.Instance.SetCenterPosition(item);
            }

            if (AchievementControllor.Instance != null && AchievementControllor.Instance.gameObject.activeInHierarchy)
            {
                AchievementControllor.Instance.SetCenterPosition(item);
                PublicSceneObject.Instance.PlaceBtnToGrid(true);
            }

            focusPointTrans.gameObject.SetActive(true);
            focusPointTrans.gameObject.layer = item.gameObject.layer;

            if (GlobalConst.SceneName == SceneType.GameScene)
            {
                focusPointTrans.position = item.transform.position + new Vector3(0, 0, 1);
            }
            else
            {
                focusPointTrans.position = item.transform.position + new Vector3(0, 0, 0);
            }


        }



        public void RemoveFocusItem()
        {
            this.curFocusItem = null;
            focusPointTrans.gameObject.SetActive(false);
        }

        public void OnClick()
        {
            if (this.curFocusItem == null)
                return;

            curFocusItem.SimulateClick();
        }
    }

    //指示当前的焦点
    private FocusPoint focusPoint;

    //记录所有可以选择的按钮
    private Dictionary<LayerMask, List<tk2dUIItem>> layerBtnDic = new Dictionary<LayerMask, List<tk2dUIItem>>();

    //记录所有当前层可以选择的按钮
    private List<tk2dUIItem> curSelectBtnList = new List<tk2dUIItem>();
    int selectBtnIndex = 0;

    void Start()
    {
        GameObject focusObj = (GameObject)Instantiate(Resources.Load("UIScene/AndroidTVPoint"));
        DontDestroyOnLoad(focusObj);
        focusPoint = new FocusPoint(focusObj.transform);
    }

    /// <summary>
    /// 跳转场景
    /// </summary>
    public void ChangeScene()
    {
        if (curShowFocusPointIe != null)
            StopCoroutine(curShowFocusPointIe);
        layerBtnDic.Clear();
    }

    /// <summary>
    /// 把所有的窗体都注册
    /// </summary>
    /// <param name="btnList">Button list.</param>
    /// <param name="layer">Layer.</param>
    public void RegisterBoxAllButton(List<tk2dUIItem> btnList, LayerMask layer)
    {
        if (!layerBtnDic.ContainsKey(layer))
        {
            layerBtnDic.Add(layer, btnList);
        }
        else
        {
            layerBtnDic[layer].AddRange(btnList);
        }
    }

    /// <summary>
    /// 当事件层改变
    /// </summary>
    /// <param name="curLayer">Current layer.</param>
    public void ChangeLayerEvent(LayerMask curLayer)
    {
        if (curShowFocusPointIe != null)
            StopCoroutine(curShowFocusPointIe);

        if (!layerBtnDic.ContainsKey(curLayer) || curLayer.value <= 0)
        {
            focusPoint.RemoveFocusItem();
            return;
        }

        //Debug.Log("ChangeLayerEvent");
        focusPoint.RemoveFocusItem();
        curShowFocusPointIe = StartCoroutine(DelayShowFocusPoint(curLayer));
    }

    Coroutine curShowFocusPointIe;
    IEnumerator DelayShowFocusPoint(LayerMask curLayer)
    {
        //Debug.Log("DelayShowFocusPoint 1");
        yield return new WaitForSeconds(0.1f);

        //Debug.Log("DelayShowFocusPoint 2");

        curSelectBtnList.Clear();

        for (int i = 0; i < layerBtnDic[curLayer].Count; i++)
        {
            tk2dUIItem item = layerBtnDic[curLayer][i];
            if (item != null && item.gameObject.activeInHierarchy)
                curSelectBtnList.Add(layerBtnDic[curLayer][i]);
        }

        if (curSelectBtnList.Count > 0)
        {
            //			selectBtnIndex = -1;
            //			SelectNextBtn();
            PlaceBtnToGrid();
        }
        else
        {
            focusPoint.RemoveFocusItem();
            try
            {
                throw new System.Exception(LayerMask.LayerToName((int)Mathf.Log(curLayer.value, 2)) + " Layer hasn't any button, please check again!");
            }
            catch(System.Exception exp)
            {
                Debug.LogError(exp.ToString());
            }            
        }
    }

    /// <summary>
    /// 用于增加商店那三个按钮的动态变层的按钮丢失问题
    /// </summary>
    /// <param name="otherList">Other list.</param>
    public IEnumerator AddButtonToSelectList(List<tk2dUIItem> otherList)
    {
        yield return new WaitForSeconds(0.5f);

        curSelectBtnList.AddRange(otherList);

        if (curSelectBtnList.Count > 0)
        {
            PlaceBtnToGrid();
        }
        else
        {
            focusPoint.RemoveFocusItem();
        }
    }

    void KeyBoardHandle(KeyOrder key)
    {

        switch (key)
        {
            case KeyOrder.Enter:


                focusPoint.OnClick();

                break;
            case KeyOrder.Up:

                if (UIGuideControllor.Instance.gameObject.activeInHierarchy && UIGuideControllor.Instance.UseCurPropGO.activeInHierarchy)
                {
                    focusPoint.OnClick();
                }

                if (IsPlayingGame())
                {
                    GamePlayerUIControllor.Instance.UsePropOnClick();
                    //SelectNextBtn(0,1);
                }
                else
                {
                    SelectNextBtn(0, 1);
                }
                break;
            case KeyOrder.Down:
                if (IsPlayingGame())
                {
                    //curSelectBtnList.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
                    SelectNextBtnInList();
                }
                else
                {
                    SelectNextBtn(0, -1);
                }
                break;
            case KeyOrder.LeftDown:

                if (UIGuideControllor.Instance.gameObject.activeInHierarchy && UIGuideControllor.Instance.GamePlayerUILeftGuideGO.activeInHierarchy)
                {
                    focusPoint.OnClick();
                }

                if (IsPlayingGame())
                {
                    GamePlayerUIControllor.Instance.LeftDown();
                }
                else
                {
                    SelectNextBtn(-1, 0);
                }
                break;
            case KeyOrder.LeftUp:
                if (IsPlayingGame())
                {
                    GamePlayerUIControllor.Instance.LeftUp();


                }
                break;
            case KeyOrder.RightDown:

                if (UIGuideControllor.Instance.gameObject.activeInHierarchy && UIGuideControllor.Instance.GamePlayerUIRightGuideGO.activeInHierarchy)
                {
                    focusPoint.OnClick();
                }
                if (IsPlayingGame())
                {
                    GamePlayerUIControllor.Instance.RightDown();
                }
                else
                {
                    SelectNextBtn(1, 0);
                }
                break;
            case KeyOrder.RightUp:
                if (IsPlayingGame())
                {
                    GamePlayerUIControllor.Instance.RightUp();


                }
                break;
            case KeyOrder.Attack:
                if (IsPlayingGame())
                {
                    GamePlayerUIControllor.Instance.UseFlyBmobProp();
                }
                break;
            case KeyOrder.Jump:
                if (IsPlayingGame())
                {
                    GamePlayerUIControllor.Instance.UseSpeedUpProp();
                }
                else
                {
                    focusPoint.OnClick();
                }
                break;
        }

    }

    bool IsPlayingGame()
    {
        if (GlobalConst.SceneName == SceneType.GameScene && !GameData.Instance.IsPause && !GameData.Instance.IsWin)
            return true;
        return false;
    }

    /*
	/// <summary>
	/// 选择下一个有效的按钮
	/// </summary>
	void SelectNextBtn()
	{
		if (curSelectBtnList.Count <= 0)
			return;
		
		selectBtnIndex++;
		selectBtnIndex = selectBtnIndex >= curSelectBtnList.Count ? 0 : selectBtnIndex;
		//Debug.Log("SelectNextBtn "+selectBtnIndex);
		
		int cal=0;
		
		while (!curSelectBtnList[selectBtnIndex].HasOnClickEvent  && cal <curSelectBtnList.Count )
		{
			++cal;
			selectBtnIndex++;
			selectBtnIndex = selectBtnIndex >= curSelectBtnList.Count ? 0 : selectBtnIndex;
		}
		
		focusPoint.SetCurrentFocusItem(curSelectBtnList[selectBtnIndex]);
	}
	
	/// <summary>
	/// 选择上一个有效的按钮
	/// </summary>
	void SelectPrevBtn()
	{
		if (curSelectBtnList.Count <= 0)
			return;
		
		selectBtnIndex--;
		selectBtnIndex = selectBtnIndex < 0 ? curSelectBtnList.Count - 1 : selectBtnIndex;
		int cal=0;
		while (!curSelectBtnList[selectBtnIndex].HasOnClickEvent  && cal <curSelectBtnList.Count)
		{
			++cal;
			selectBtnIndex--;
			selectBtnIndex = selectBtnIndex <0? curSelectBtnList.Count-1 : selectBtnIndex;
		}
		
		focusPoint.SetCurrentFocusItem(curSelectBtnList[selectBtnIndex]);
	}
	
	*/

    int x = 0, y = 0;
    static int xWidth = 30, yWidth = 30;
    static int xCount = Screen.width / xWidth + 1, yCount = Screen.height / yWidth + 1;
    tk2dUIItem[,] btnPlaceGrid = new tk2dUIItem[xCount, yCount];

    //把所有按钮放到一个屏幕的二维数组
    void PlaceBtnToGrid(bool focus = false)
    {
        for (int i = 0; i < xCount; i++)
            for (int j = 0; j < yCount; j++)
                btnPlaceGrid[i, j] = null;

        bool findFirst = false;
        for (int i = 0; i < curSelectBtnList.Count; i++)
        {
            try
            {


                int btnX = ((int)curSelectBtnList[i].transform.position.x + Screen.width / 2) / xWidth;
                int btnY = ((int)curSelectBtnList[i].transform.position.y + Screen.height / 2) / yWidth;

                if (btnX > xCount || btnY > yCount || btnX < 0 || btnY < 0)
                    continue;

                // if(i >= curSelectBtnList.Count) { continue; }
                btnPlaceGrid[btnX, btnY] = curSelectBtnList[i];

                if (focus)
                {
                    if (focusPoint.curFocusItem == curSelectBtnList[i])
                    {
                        x = btnX;
                        y = btnY;
                    }
                    continue;
                }

                if (!findFirst)
                {
                    if (IsFirstBtn(curSelectBtnList[i].transform))
                    {
                        findFirst = true;
                    }

                    //默认的那个按钮
                    x = btnX;
                    y = btnY;
                    focusPoint.SetCurrentFocusItem(curSelectBtnList[i]);
                }
            }
            catch(System.Exception exp)
            {
                Debug.LogError("PlaceBtnToGrid error:" + exp.ToString());
            }

        }
        focusPoint.RemoveFocusItem();

    }

    //查找是不是作为默认的光标位置
    bool IsFirstBtn(Transform btn)
    {
        string btName = btn.name.ToLower();
        if (btName.Contains("sure") || btName.Contains("enter")
            || btName.Contains("start") || btName.Contains("buy")
            || btName.Contains("yellow") || btName.Contains("reborn")
            || btName.Contains("classic") || btName.Contains("zaohuan")
            || btName.Contains("levelgame") || btName.Contains("continue")
            || btName.Contains("pause"))
            return true;

        string btParentName = btn.parent.name.ToLower();
        if (btParentName.Contains("sure") || btParentName.Contains("enter")
            || btParentName.Contains("start") || btParentName.Contains("buy")
            || btParentName.Contains("yellow") || btParentName.Contains("reborn")
            || btParentName.Contains("classic") || btParentName.Contains("zaohuan"))
            return true;

        return false;
    }

    /// <summary>
    /// 选择下一个按钮.
    /// </summary>
    /// <param name="xInc">二维数组的X增量.</param>
    /// <param name="yInc">二维数组的Y增量.</param>
    void SelectNextBtn(int xInc, int yInc)
    {
        if (xInc != 0)
        {
            int xTemp = x;
            for (int j = 0; j < yCount; j++)
            {

                for (int i = 0; i < xCount; i++)
                {
                    int curx = x + xInc;
                    if (curx < 0 || curx > xCount - 1)
                        break;

                    x = curx;
                    if (btnPlaceGrid[x, y] != null && btnPlaceGrid[x, y].HasOnClickEvent)
                    {
                        focusPoint.SetCurrentFocusItem(btnPlaceGrid[x, y]);
                        return;
                    }
                }

                y -= xInc;
                y = y < 0 ? yCount - 1 : (y > yCount - 1 ? 0 : y);

                x = xTemp;
            }
        }

        if (yInc != 0)
        {
            int yTemp = y;
            for (int i = 0; i < xCount; i++)
            {

                for (int j = 0; j < yCount; j++)
                {
                    int cury = y + yInc;
                    if (cury < 0 || cury > yCount - 1)
                        break;
                    y = cury;

                    if (btnPlaceGrid[x, y] != null && btnPlaceGrid[x, y].HasOnClickEvent)
                    {
                        focusPoint.SetCurrentFocusItem(btnPlaceGrid[x, y]);
                        return;
                    }
                }

                x += yInc;
                x = x < 0 ? xCount - 1 : (x > xCount - 1 ? 0 : x);

                y = yTemp;
            }
        }
    }

    void SelectNextBtnInList()
    {
        tk2dUIItem item = focusPoint.curFocusItem;
        int index = curSelectBtnList.IndexOf(item);
        if (index < 0)
            return;

        ++index;
        if (index > curSelectBtnList.Count - 1)
        {
            index = 0;
        }

        focusPoint.SetCurrentFocusItem(curSelectBtnList[index]);
    }

    #endregion


#endif
}
