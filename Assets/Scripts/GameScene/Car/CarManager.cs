using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

/// <summary>
/// 初始化游戏的主角 和对手车
/// </summary>
public class CarManager : MonoBehaviour {

	public static CarManager Instance=null;

    [Space(10)]
    public bool isTest=false;
    public bool isTestSelfLocalLevelData = false;
    [Space(10)]
	public int playerCarID;
	public Transform playerCarTrans;
	public List<Transform> opponentCarTrans;
	public List<int> opponnetCarIDList=new List<int>();
	
	public List<CarMove> carMoveList= new List<CarMove>();
	public List<PropControl> propConList = new List<PropControl>();
    
	private SpawnPool characterPool ;
	private Vector3 carScale = Vector3.one;
	public Vector3 carBackOffset= new Vector3(0f,0f,-1.8f);

	void Awake()
	{
		Instance = this;
	}
	// Use this for initialization
	void Start () {

		characterPool = PoolManager.Pools["CharactersPool"];

		if(isTest == false)
		{
			if(PlayerData.Instance.IsWuJinGameMode())
			{
				playerCarID = PlayerData.Instance.GetSelectedModel();
				gameLevelModel = GameLevelModel.WuJing;
				//opponnetCarIDList = WuJingConfigData.Instance.GetOpponentList();
				SetWujinOpponentIDList();
			}else
			{
				playerCarID = PlayerData.Instance.GetSelectedModel();
				curLevel = PlayerData.Instance.GetSelectedLevel();
				string strModel = GameLevelData.Instance.GetGameLevelModel(curLevel);
				gameLevelModel =(GameLevelModel) System.Enum.Parse(typeof(GameLevelModel),strModel);
				opponnetCarIDList = GameLevelData.Instance.GetOpponentList(curLevel);

				aheadSpeedDownDis = LevelData.Instance.GetAheadDis(curLevel);
				lastSpeedUpDis = LevelData.Instance.GetLastDis(curLevel);
			}
		}

		InitCarModel();
		InitGameModel();
	}

	void SetWujinOpponentIDList()
	{
		int playerLevel =IDTool.GetModelLevel(playerCarID);
		opponnetCarIDList.Clear();
		for(int i=1;i<=5;++i)
		{
			opponnetCarIDList.Add(i*100+playerLevel);
		}
	}

	/// <summary>
	/// 初始化角色模型
	/// </summary>
	void InitCarModel()
	{

		//setup player model
		string playerPrafabName = ModelData.Instance.GetPrefabName(playerCarID);
		Transform playerTrans = characterPool.Spawn(playerPrafabName);
		this.playerCarTrans.localScale = Vector3.one;
		playerTrans.parent = this.playerCarTrans;
		playerTrans.localPosition = carBackOffset;
		playerTrans.localRotation = Quaternion.identity;
		playerTrans.localScale = carScale;
		CarMove playerCarMove =this.playerCarTrans.GetComponent<CarMove>();
		playerCarMove.isPlayer= true;
		playerCarMove.SetDataById(playerCarID);
		playerCarMove.carTransform= playerTrans;
		carMoveList.Add(playerCarMove);
		propConList.Add(this.playerCarTrans.GetComponent<PropControl>());
		PlayerCarControl.Instance.Init();

		Transform robotTrans = characterPool.Spawn(ModelData.Instance.GetMechaPrefabName(playerCarID));
		robotTrans.parent = this.playerCarTrans;
		robotTrans.localPosition = carBackOffset;
		robotTrans.localRotation = Quaternion.identity;
		robotTrans.localScale =Vector3.one;

		AnimatorManager animManager = playerTrans.GetComponentInChildren<AnimatorManager>();
		if (animManager != null) {
			animManager.Init ();
			animManager.Setting();
		}
		playerCarMove.animManager = animManager;
		MechaAnimatorManager mechaAnimManager = robotTrans.GetComponentInChildren<MechaAnimatorManager>();
		playerCarMove.mechaAnimManager = mechaAnimManager;

		PropControl playerShape = this.playerCarTrans.GetComponent<PropControl>();
		playerShape.carTrans= playerTrans;
		playerShape.shapeShiftTrans = robotTrans;
		robotTrans.gameObject.SetActive(false);


		for(int k=0;k<opponentCarTrans.Count;++k)
		{
			opponentCarTrans[k].gameObject.SetActive(false);
		}

		//setup npc model
		int opponIndex=0;
		int playerType = IDTool.GetModelType(playerCarID);
		float[] xOffsets={-18,-9f,9f,18f};
		for(int i=0;i<opponnetCarIDList.Count;++i)
		{
			int carId= opponnetCarIDList[i];
			if(IDTool.GetModelType(carId) == playerType)
			{
				continue;
			}

			if(gameLevelModel == GameLevelModel.DualMeet)
			{
				opponIndex = 2;
			}
			Transform parentCar = opponentCarTrans[opponIndex];
			parentCar.gameObject.SetActive(true);
			parentCar.localScale = Vector3.one;

			string prafabName = ModelData.Instance.GetPrefabName(carId);
			Transform trans = characterPool.Spawn(prafabName);
			trans.parent = parentCar;
			trans.localPosition = carBackOffset;
			trans.localRotation = Quaternion.identity;
			trans.localScale = carScale;
			CarMove oppoCarMove = parentCar.GetComponent<CarMove>();
			oppoCarMove.isPlayer=false;
			oppoCarMove.xOffset=xOffsets[opponIndex];
			oppoCarMove.SetDataById(carId);
			oppoCarMove.carTransform=trans;
			carMoveList.Add(oppoCarMove);
			propConList.Add(parentCar.GetComponent<PropControl>());

			Transform npcRobotTrans = characterPool.Spawn(ModelData.Instance.GetMechaPrefabName(carId));
			npcRobotTrans.parent = parentCar;
			npcRobotTrans.localPosition = carBackOffset;
			npcRobotTrans.localRotation = Quaternion.identity;
			npcRobotTrans.localScale = Vector3.one;

			AnimatorManager npcAnimManager = trans.GetComponentInChildren<AnimatorManager>();
			if (npcAnimManager != null) {
				npcAnimManager.Init ();
				npcAnimManager.Setting();
			}
			oppoCarMove.animManager = npcAnimManager;
			MechaAnimatorManager npcMechaAnimManager = npcRobotTrans.GetComponentInChildren<MechaAnimatorManager>();
			oppoCarMove.mechaAnimManager = npcMechaAnimManager;

			PropControl npcShape = parentCar.GetComponent<PropControl>();
			npcShape.carTrans= trans;
			npcShape.shapeShiftTrans = npcRobotTrans;
			npcRobotTrans.gameObject.SetActive(false);
			++opponIndex;

			parentCar.gameObject.SetActive(true);
			parentCar.GetComponent<OpponenetCarControl>().Init();

			if(gameLevelModel == GameLevelModel.DualMeet)
			{
				break;
			}
		}
	}

	#region  游戏的速度限制
	private float aheadSpeedDownDis=150;
	private float lastSpeedUpDis=150;
	private float speedUpChangeValue=20f;
	private float speedDownChangeValue=70f;
	private int changeSpeedCal=0;
	void UpdateOpponentCarSpeedChange()
	{
		++changeSpeedCal;
		if(changeSpeedCal <40)
		{
			return;
		}

		changeSpeedCal=0;

		CarMove playerMove = carMoveList[0];
		for(int i=1;i<carMoveList.Count;++i)
		{
			CarMove oppCar = carMoveList[i];
			if(oppCar.moveLen>playerMove.moveLen+aheadSpeedDownDis )
			{
				oppCar.maxSpeed=playerMove.speed-speedUpChangeValue;
			}
			else if(oppCar.moveLen<playerMove.moveLen-aheadSpeedDownDis )
			{
				oppCar.maxSpeed=playerMove.speed+speedDownChangeValue;
			}else if( !oppCar.propCon.isSpeedUp  && !oppCar.propCon.isMushroom 
			         && !oppCar.propCon.isShapeShift && !oppCar.propCon.isSpeedAdd )
			{

				oppCar.maxSpeed= oppCar.origMaxSpeed;
			}
		}
	}
	#endregion

	#region 对手提示标签
	private float tipsMinDistance=100f;
	private void UpdateOpponentTips()
	{

		int closestIndex=0;
		float closeDis=float.MaxValue;
		CarMove playerMove = carMoveList[0];
		for(int i=1;i<carMoveList.Count;++i)
		{
			CarMove oppCar = carMoveList[i];
			if(oppCar.moveLen>=playerMove.moveLen)
				continue;

			float dis = playerMove.moveLen - oppCar.moveLen;
			if(dis<closeDis)
			{
				closeDis=dis;
				closestIndex=i;
			}
		}

		if( 5<closeDis  && closeDis<tipsMinDistance && closestIndex!=0)
		{
			CarMove closeCar=carMoveList[closestIndex];
			float xPercent = (playerMove.xOffset-closeCar.xOffset) / (closeCar.maxXOffset) +0.5f;
			GamePlayerUIControllor.Instance.ShowOpponentTips(closeCar.carId,xPercent,closeDis);
		}else
		{
			GamePlayerUIControllor.Instance.HideOpponentTips();
		}

	}
	#endregion

	#region  Car Manager

	/// <summary>
	/// 获取角色排名
	/// </summary>
	/// <returns>The player rank.</returns>
	public int GetPlayerRank()
	{
		//the first one is playercar
		float playerLen= carMoveList[0].moveLen;
		int count=0;
		for(int i=0;i<carMoveList.Count;++i)
		{
			if(carMoveList[i].moveLen>playerLen)
			{
				count++;
			}
		}

		return count+1;
	}

	public void SetFinalRank()
	{
		finalRank = GetPlayerRank();
		GameData.Instance.rank = finalRank;
	}

	/// <summary>
	/// 获取角色的前几名列表
	/// </summary>
	/// <returns>The pre car list.</returns>
	/// <param name="preCount">Pre count.</param>
	private List<CarMove> sortCarList = new List<CarMove>();
	private List<CarMove> targetCarList = new List<CarMove> ();
	public List<CarMove> GetPreCarList(CarMove playerCar, int preCount)
	{
		sortCarList.Clear ();
		sortCarList.AddRange (carMoveList);
		sortCarList.Remove (playerCar);
		sortCarList.Sort (CompareList);
		/*
		for (int i = 0; i < sortCarList.Count; ++i) {
			print (sortCarList [i].moveLen);
		}
		*/
		targetCarList.Clear ();
		for (int i = 0; i < sortCarList.Count; ++i) {
			if (sortCarList [i].moveLen > playerCar.moveLen) {
				targetCarList.Add (sortCarList [i]);
				if (targetCarList.Count >= preCount)
					break;
			}
		}

		return targetCarList;
	}

	private int CompareList(CarMove x, CarMove y){
		return x.moveLen.CompareTo (y.moveLen);
	}

	/// <summary>
	/// 获取距离角色最近的车
	/// </summary>
	/// <returns>The nearest car.</returns>
	private List<CarMove> nearCarList = new List<CarMove>();
	public CarMove GetNearestCar(CarMove playerCar)
	{
		nearCarList.Clear ();
		nearCarList.AddRange (carMoveList);
		nearCarList.Remove (playerCar);
		CarMove targetCar = nearCarList [0];
		float distance = Mathf.Abs (targetCar.moveLen - playerCar.moveLen);
		for (int i = 1; i < nearCarList.Count; ++i) {
			if (distance > Mathf.Abs (nearCarList [i].moveLen - playerCar.moveLen)) {
				distance = Mathf.Abs (nearCarList [i].moveLen - playerCar.moveLen);
				targetCar = nearCarList [i];
			}
		}
		return targetCar;
	}

	/// <summary>
	//  获取赛车排名数据
	/// </summary>
	/// <returns>The car move sort list.</returns>
	public List<CarMove> GetCarMoveSortList()
	{
		List<CarMove> carMoveSortList = new List<CarMove>();
		carMoveSortList.Clear();
		carMoveSortList.AddRange(carMoveList);
		carMoveSortList.Sort(CompareList);
		return carMoveSortList;
	}

	#endregion

	#region 游戏模式处理
	public GameLevelModel gameLevelModel;
	public int curLevel;


	CarMove playerCarMove;

	public int finalRank=0;
	public float playerUseTime=0;
	public float totalUseTime=0;
	public float totalPathLen=0;
	public int circleCount=1;

	public bool isFinish=false;
	private bool isStart=false;

	private List<CarMove> rankTempList = new List<CarMove>();
	private List<CarMove> rankFinalList = new List<CarMove>();

	private float  weedoutTime;
	private float  weedoutCircleCount;
	private List<CarMove> weedoutCarList = new List<CarMove>();

	private int wujinCircleCount=1;

	/// <summary>
	/// 初始化比赛模式
	/// </summary>
	void InitGameModel()
	{
		playerCarMove = PlayerCarControl.Instance.carMove;
		circleCount = GameLevelData.Instance.GetCircleCount(curLevel);
        if (isTestSelfLocalLevelData)
            circleCount = SelfLocalGameLevelData.Instance.GetCircleCount(curLevel);
		switch(gameLevelModel)
		{
		case GameLevelModel.LimitTime:
			totalUseTime=GameLevelData.Instance.GetUseTime(curLevel);
            if (isTestSelfLocalLevelData)
                totalUseTime = SelfLocalGameLevelData.Instance.GetUseTime(curLevel);
			totalPathLen = circleCount * RoadPathManager.Instance.pathLen;
			break;
		case GameLevelModel.Rank:
			totalPathLen =circleCount * RoadPathManager.Instance.pathLen;
			rankTempList.Clear();
			rankFinalList.Clear();
			rankTempList.AddRange(carMoveList);
			break;
		case GameLevelModel.DualMeet:
			totalPathLen = circleCount * RoadPathManager.Instance.pathLen;
			rankTempList.Clear();
			rankFinalList.Clear();
			rankTempList.AddRange(carMoveList);
			break;
		case GameLevelModel.Weedout:
			weedoutTime = GameLevelData.Instance.GetUseTime(curLevel);
            if (isTestSelfLocalLevelData)
                weedoutTime = SelfLocalGameLevelData.Instance.GetUseTime(curLevel);
			weedoutCircleCount =1;
			totalUseTime= weedoutTime;
			weedoutCarList.Clear();
			weedoutCarList.AddRange(carMoveList);
			break;
		case GameLevelModel.WuJing:
			totalUseTime=WuJingScenepathData.Instance.GetInitTime(RoadPathManager.Instance.wujinPathId);
			totalPathLen = RoadPathManager.Instance.pathLen;
			break;
		}

		isStart = true;
	}

	void UpdateWujin()
	{
		if(isFinish)
		{
			return;
		}
		
		if(playerCarMove.moveLen >=totalPathLen)
		{
			//next circle
			totalPathLen +=  RoadPathManager.Instance.pathLen;
			float time =WuJingScenepathData.Instance.GetInitTime(RoadPathManager.Instance.wujinPathId) - wujinCircleCount* WuJingScenepathData.Instance.GetReduceTime(RoadPathManager.Instance.wujinPathId);
			time=Mathf.Max(time,WuJingScenepathData.Instance.GetMinTime(RoadPathManager.Instance.wujinPathId));
			totalUseTime= playerUseTime+ time;
			++wujinCircleCount;
		}else if(playerUseTime>totalUseTime)
		{
			//fail
			isFinish= true;
			GameController.Instance.Dead();
		}
	}

	/// <summary>
	/// Time limit模式检测
	/// </summary>
	void UpdateCheckLimitTime()
	{
		if(isFinish)
		{
			return;
		}

		if(playerCarMove.moveLen >= totalPathLen)
		{
			//win
			isFinish= true;
			GameController.Instance.FinishGame();
		}else if(playerUseTime>totalUseTime)
		{
			//fail
			isFinish = true;
			GameController.Instance.Dead();
		}

	}

	/// <summary>
	/// 竞速赛的检测
	/// </summary>
	void UpdateCheckRank()
	{
		if(isFinish)
		{
			return;
		}

		int i=0;
		while(i<rankTempList.Count)
		{
			if(rankTempList[i].moveLen >= totalPathLen)
			{
				rankFinalList.Add(rankTempList[i]);
				if(rankTempList[i] == playerCarMove)
				{
					/*比赛胜利失败判断*/
					if(rankFinalList.Count> MissionBoard.Instance.minRank)
					{
						GameController.Instance.FinishGame(false);
					}else
					{
						GameController.Instance.FinishGame();
					}
				}
				rankTempList.RemoveAt(i);
			}else 
			{
				++i;
			}

		}

		if(rankTempList.Count<1)
			isFinish = true;

	}

	/// <summary>
	/// Against the race的检测
	/// </summary>
	void UpdateCheckDualMeet()
	{
		if(isFinish)
		{
			return;
		}
		
		int i=0;
		while(i<rankTempList.Count)
		{
			if(rankTempList[i].moveLen >= totalPathLen)
			{
				rankFinalList.Add(rankTempList[i]);
				if(rankTempList[i] == playerCarMove)
				{
					/*比赛胜利失败判断*/
					if(rankFinalList.Count>1)
					{
						GameController.Instance.FinishGame(false);
					}else
					{
						GameController.Instance.FinishGame();
					}
				}
				rankTempList.RemoveAt(i);
			}else 
			{
				++i;
			}
			
		}
		
		if(rankTempList.Count<1)
			isFinish = true;
	}

	/// <summary>
	/// Knockout的检测
	/// </summary>
	void UpdateCheckWeedout()
	{
		if(isFinish)
		{
			return;
		}
		if(playerUseTime >=totalUseTime)
		{
			totalUseTime+=weedoutTime;
			++weedoutCircleCount;

			/*淘汰最后一台车*/
			float minMoveLen=float.MaxValue;
			int minIndex=0;
			for(int i=0;i<weedoutCarList.Count;++i)
			{
			     if(weedoutCarList[i].moveLen<minMoveLen)
				{
					minMoveLen = weedoutCarList[i].moveLen;
					minIndex = i;
				}
			}


			CarMove  curCarMove = weedoutCarList[minIndex];
			if(playerCarMove == curCarMove) /*淘汰角色*/
			{
				if(weedoutCarList.Count<=3)
				{
					GameController.Instance.FinishGame(true);
				}else
				{
					GameController.Instance.FinishGame(false);
				}
				isFinish = true;

			}else
			{
				curCarMove.gameObject.SetActive(false);
				int id = curCarMove.carId;
				DropItemManage.Instance.ShowWeedOut(id);

				weedoutCarList.RemoveAt(minIndex);
				if(weedoutCarList.Count<2)
				{
					GameController.Instance.FinishGame(true);
					isFinish= true;
				}
			}
		}
	}

	public void Reborn()
	{
		totalUseTime+=20f;
		isFinish=false;

		PlayerCarControl.Instance.propCon.UsePropByType(PropType.SpeedUp);
		GamePlayerUIControllor.Instance.ShieldItemData.ShowClippedEffect ();
		GamePlayerUIControllor.Instance.SpeedUpItemData.ShowClippedEffect ();
		GamePlayerUIControllor.Instance.FlyBombItemData.ShowClippedEffect ();
	}

	void FixedUpdate()
	{

		if(GameData.Instance.IsPause)
		{
			return;
		}

		if(isStart== false)
		{
			return;
		}

		if(isFinish)
		{
			return;
		}

		playerUseTime+=Time.fixedDeltaTime;

		/*比赛模式的胜利失败检测*/
		switch(gameLevelModel)
		{
		case GameLevelModel.LimitTime:
			UpdateCheckLimitTime();
			break;
		case GameLevelModel.Rank:
			UpdateCheckRank();
			break;
		case GameLevelModel.DualMeet:
			UpdateCheckDualMeet();
			break;
		case GameLevelModel.Weedout:
			UpdateCheckWeedout();
			break;
		case GameLevelModel.WuJing:
			UpdateWujin();
			break;
		}

		UpdateOpponentTips();
		UpdateOpponentCarSpeedChange();
	}
	#endregion
}
