using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using DG.Tweening;

/// <summary>
/// 角色和NPC上的道具处理控制脚本
/// </summary>
public class PropControl : MonoBehaviour {

	public bool isPlayer=false;
	public CarMove carMove;
	public CarParticleControl carParticleControl;
	public GameObject magnetCheckGO=null;

	private SpawnPool itemPool;

	public void Init () {

		itemPool = PoolManager.Pools["ItemPool"];

		carMove = GetComponent<CarMove> ();
		carParticleControl = GetComponent<CarParticleControl> ();

		if(isPlayer)
		{
		     magnetCheckGO= transform.Find("MagnetCheck").gameObject;
		}
	}
	

	#region OnTrigger

	ItemBase item;
	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("Item")) {
			if ((item = col.transform.parent.GetComponent<ItemBase> ()) != null) {
				item.GetItem(this);
			}
		}
	}
	#endregion


	#region  道具处理
	public bool isSpeedUp = false;
	private float speedUpTime;
	public void StartSpeedUp(float lastTime)
	{
		speedUpTime = lastTime;
		if (isSpeedUp)
			return;

		if (isSpeedAdd) {
			isSpeedAdd = false;
			StopCoroutine ("IEStartSpeedAdd");
		}

		StartCoroutine ("IEStartSpeedUp");

		//眩晕其他车手1秒
		List<CarMove> carMoveList = new List<CarMove> ();
		carMoveList.AddRange (CarManager.Instance.carMoveList);
		carMoveList.Remove (carMove);
		for (int i = 0; i < carMoveList.Count; ++i) {
			carMoveList [i].CarLighting (GameData.Instance.SpeedUpLightingVertigoTime);
		}

		if(isPlayer)
		{
			GameData.Instance.speeedUpCount++;
		}
	}
	private IEnumerator IEStartSpeedUp()
	{
		isSpeedUp = true;

		if (isPlayer) {
			GamePlayerUIControllor.Instance.SetPropLock (true);
			AudioManger.Instance.PlaySound (AudioManger.SoundName.SpeedUp);
			CarCameraFollow.Instance.SetSpeedUp (true);

			CameraEffect.Instance.StartEffect();
		}

		if (isSpeedAdd == false)
			carParticleControl.PlayParticle (CarParticleType.SpeedUp);
		carMove.animManager.SpeedUp ();

		carMove.speed = carMove.SetMaxSpeed ();

		while(speedUpTime > 0)
		{
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
			speedUpTime -= Time.deltaTime;
			yield return null;
		}

		isSpeedUp = false;
		carMove.SetMaxSpeed ();

		if (isPlayer) {
			GamePlayerUIControllor.Instance.SetPropLock (false);
			CarCameraFollow.Instance.SetSpeedUp (false);

			CameraEffect.Instance.StopEffect();
		}

		carParticleControl.StopParticle (CarParticleType.SpeedUp);
		carMove.animManager.SpeedUpBack ();
	}

	public bool isSpeedAdd = false;
	private float speedAddTime;
	public void StartSpeedAdd(float lastTime)
	{
		speedAddTime = lastTime;
		if (isSpeedAdd || isSpeedUp || isShapeShift)
			return;
		StartCoroutine("IEStartSpeedAdd");

		if(isPlayer)
		{
			GameData.Instance.speeedUpCount++;
		}
	}
	private IEnumerator IEStartSpeedAdd()
	{
		isSpeedAdd = true;

		if (isPlayer) {
			AudioManger.Instance.PlaySound (AudioManger.SoundName.SpeedUp);
			CarCameraFollow.Instance.SetSpeedUp (true);

			CameraEffect.Instance.StartEffect();
		}

		carParticleControl.PlayParticle (CarParticleType.SpeedUp);
		carMove.animManager.SpeedUp ();

		carMove.speed = carMove.SetMaxSpeed ();

		while(speedAddTime > 0)
		{
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
			speedAddTime -= Time.deltaTime;
			yield return null;
		}

		isSpeedAdd = false;
		carMove.SetMaxSpeed ();

        if (isPlayer)
        {
            CarCameraFollow.Instance.SetSpeedUp(false);

			CameraEffect.Instance.StopEffect();
        }

		carParticleControl.StopParticle (CarParticleType.SpeedUp);
		carMove.animManager.SpeedUpBack ();
	}

	#region 磁铁
	public bool isMagnet = false;
	private float magnetTime;
	public void StartMagnet(float lastTime)
	{
		magnetTime = lastTime;
		if (isMagnet)
			return;
		StartCoroutine ("IEStartMagnet");
	}

	private IEnumerator IEStartMagnet()
	{
		isMagnet=true;
		magnetCheckGO.SetActive(true);
		carParticleControl.PlayParticle (CarParticleType.Magnet);

		while(this.magnetTime>0)
		{
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
			this.magnetTime-=Time.deltaTime;
			yield return null;
		}

		isMagnet=false;
		magnetCheckGO.SetActive(false);
		carParticleControl.StopParticle (CarParticleType.Magnet);
	}
	#endregion

	#region 无敌护盾
	public bool isShield = false;
	private float shieldTime;
	public void StartShield(float lastTime)
	{
		shieldTime = lastTime;
		if (isShield)
			return;
		StartCoroutine ("IEStartShield");
	}

	private IEnumerator IEStartShield()
	{
		isShield = true;
		carParticleControl.PlayParticle (CarParticleType.Shield);

		if (isPlayer)
			AudioManger.Instance.PlaySound (AudioManger.SoundName.Shield);

		while(shieldTime>0)
		{
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
			shieldTime-=Time.deltaTime;
			yield return null;
		}

		isShield = false;
		carParticleControl.StopParticle (CarParticleType.Shield);

		if (isPlayer)
			AudioManger.Instance.PlaySound (AudioManger.SoundName.ShieldBroken);
	}

	public void StopShield()
	{
		StopCoroutine ("IEStartShield");
		isShield = false;
		carParticleControl.StopParticle (CarParticleType.Shield);

		if (isPlayer)
			AudioManger.Instance.PlaySound (AudioManger.SoundName.ShieldBroken);

		GamePlayerUIControllor.Instance.ShieldItemData.HideClippedEffect ();
	}
	#endregion

	#region 火球
	public bool isFireBall = false;
	public void StartFireBall(float lastTime)
	{
		FireBallBehaviour fireBallScript;
		if (isFireBall) {
			Transform fireBallTrans = transform.Find ("FireBallBehaviour");
			fireBallScript = fireBallTrans.GetComponent<FireBallBehaviour> ();
		} else {
			Transform fireBallTrans = itemPool.Spawn("FireBallBehaviour");
			fireBallScript = fireBallTrans.GetComponent<FireBallBehaviour> ();
			fireBallTrans.name = "FireBallBehaviour";
			fireBallTrans.parent = transform;
			fireBallTrans.localPosition = CarManager.Instance.carBackOffset;
			fireBallTrans.localEulerAngles = Vector3.zero;
			fireBallTrans.localScale = Vector3.one;
			fireBallScript.Init (this);
		}
		fireBallScript.Show (lastTime);
		if (isPlayer)
			AudioManger.Instance.PlaySound (AudioManger.SoundName.FireBall);
	}
	#endregion

	#region 拳套
	public bool isGlove = false;
	public void StartGloves()
	{
		GlovesBehaviour glovesBeha;
		if (isGlove) {
			Transform glovesTrans = transform.Find ("GlovesBehaviour");
			glovesBeha = glovesTrans.GetComponent<GlovesBehaviour> ();
		} else {
			Transform glovesTrans = itemPool.Spawn ("GlovesBehaviour");
			glovesBeha = glovesTrans.GetComponent<GlovesBehaviour> ();
			glovesTrans.name = "GlovesBehaviour";
			glovesTrans.parent = transform;
			glovesTrans.localPosition =CarManager.Instance.carBackOffset;
			glovesTrans.localEulerAngles = Vector3.zero;
			glovesTrans.localScale = Vector3.one;
			glovesBeha.Init (this);
		}
		glovesBeha.Show (GameData.Instance.GloveTime);
	}
	#endregion

	#region 追踪导弹
	public void StartMissile(int missileCount)
	{
		StartCoroutine (IEStartMissile (missileCount));
	}

	IEnumerator IEStartMissile(int missileCount)
	{
		List<CarMove> carMoveList = CarManager.Instance.GetPreCarList (carMove, missileCount);
		CarMove targetCarMove;
		for (int i = 0; i < missileCount; ++i) {
			Transform missileTrans = itemPool.Spawn ("MissileBehaviour");
			MissileBehaviour missileScript = missileTrans.GetComponent<MissileBehaviour> ();
			if (carMoveList.Count > i)
				targetCarMove = carMoveList [i];
			else
				targetCarMove = null;
			float mOffset = carMove.xOffset;
			missileScript.Init (this);
			missileScript.Launch (carMove.moveLen + 5, carMove.xOffset, targetCarMove, mOffset);
			if (isPlayer)
				AudioManger.Instance.PlaySound (AudioManger.SoundName.Shooting);
			while (GameData.Instance.IsPause)
				yield return null;
			yield return new WaitForSeconds (0.5f);
		}
	}
	#endregion

	#region 连发导弹
	public void StartFlyBomb(int bombCount)
	{
		StartCoroutine (IEStartFlyBomb (bombCount));
		CreatePropManager.Instance.InsertGruop();
	}

	IEnumerator IEStartFlyBomb(int bombCount)
	{
		for (int i = 0; i < 10; ++i) {
			for (int j = 1; j < bombCount; ++j) {
				Transform flyBmobTran = itemPool.Spawn ("FlyBombBehaviour");
				FlyBmobBehaviour flyBmobSript = flyBmobTran.GetComponent<FlyBmobBehaviour> ();
				Vector3 startPos = transform.position - transform.forward * 6 + transform.up * 6;
				float moveLen = carMove.moveLen + 100 + j * 30;
				Vector3 endPos = RoadPathManager.Instance.GetPointByLen (moveLen, Random.Range (-10f, 10f));
				flyBmobSript.Init (this);
				flyBmobSript.Launch (startPos, endPos);
				AudioManger.Instance.PlaySound (AudioManger.SoundName.Shooting);
			}
			while (GameData.Instance.IsPause)
				yield return null;
			yield return new WaitForSeconds (0.5f);
		}
	}
	#endregion

	/// <summary>
	/// 使用绿色龟壳
	/// </summary>
	public void StartGreenTurtleShell()
	{
		float pathLen = carMove.moveLen+5;
		float offset = carMove.xOffset;
		Transform tsTran = itemPool.Spawn ("GreenTurtleShellBehaviour");
		TurtleShellBehaviour tsBeha = tsTran.GetComponent<TurtleShellBehaviour>();
		tsBeha.Init (this);
		tsBeha.Launch (pathLen, offset,carMove.maxSpeed*2.6f);
		if (isPlayer)
			AudioManger.Instance.PlaySound (AudioManger.SoundName.ShellShoot);
	}

	/// <summary>
	/// 使用红色龟壳
	/// </summary>
	public void StartRedTurtleShell()
	{
		float pathLen = carMove.moveLen+5;
		float offset = carMove.xOffset;
		if (offset < -12)
			offset = carMove.xOffset;
		else if (offset > 12)
			offset = carMove.xOffset - 12;
		else
			offset = carMove.xOffset - 6;
		for (int i = 0; i < 3; ++i) {
			Transform tsTran = itemPool.Spawn ("RedTurtleShellBehaviour");
			TurtleShellBehaviour tsBeha = tsTran.GetComponent<TurtleShellBehaviour>();
			tsBeha.Init (this);
			tsBeha.Launch (pathLen, offset+i*6,carMove.maxSpeed*2.6f);
		}
		if (isPlayer)
			AudioManger.Instance.PlaySound (AudioManger.SoundName.ShellShoot);
	}

	#region 蘑菇
	public bool isMushroom = false;
	private float mushroomTime;
	public void StartMushroom(float lastTime)
	{
		mushroomTime = lastTime;
		if (isMushroom || isShapeShift)
			return;
		StartCoroutine("IEStartMushroom");
		Transform mushroomTran = itemPool.Spawn("MushroomBehaviour");
		mushroomTran.parent= transform;
		mushroomTran.localPosition = Vector3.zero;
		MushroomBehaviour mushroomScript = mushroomTran.GetComponent<MushroomBehaviour> ();
		mushroomScript.Init (this);
		mushroomScript.Launch (lastTime);
		if (isPlayer) {
			AudioManger.Instance.PlaySound (AudioManger.SoundName.Mushroom);
			CarCameraFollow.Instance.SetChangeBig (true);
		}
	}

	private IEnumerator IEStartMushroom()
	{
		transform.localScale= Vector3.one;
		transform.DOKill();

		isMushroom = true;
		transform.DOScale (1.8f, 0.3f);

		carParticleControl.PlayParticle (CarParticleType.Mushroom, true, 2.0f);

		carMove.speed = carMove.origMaxSpeed;
		carMove.SetMaxSpeed ();

	   	while(mushroomTime>0)
		{
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
			mushroomTime -=Time.deltaTime;
			yield return null;
		}

		isMushroom = false;

		if (isPlayer) {
			CarCameraFollow.Instance.SetChangeBig (false);
		}

		carMove.SetMaxSpeed ();

		transform.DOScale (1.0f, 0.3f);
	}
	#endregion

	/// <summary>
	/// 使用香蕉皮
	/// </summary>
	public void StartBananaPeel()
	{
		Transform bananaPeelTran  = itemPool.Spawn("BananaPeelBehaviour");
		bananaPeelTran.position = transform.position;
		BananaBehaviour bananaPeelScript = bananaPeelTran.GetComponent<BananaBehaviour>();
		bananaPeelScript.Init (this);
	}

	/// <summary>
	/// 使用闪电
	/// </summary>
	public void StartLighting()
	{
		Transform lightingTran  = itemPool.Spawn("LightingBehaviour");
		lightingTran.parent = transform;
		lightingTran.localPosition = CarManager.Instance.carBackOffset;
		LightingBehaviour lightingScript = lightingTran.GetComponent<LightingBehaviour>();
		lightingScript.Init (this);
		lightingScript.Show ();
		StartCoroutine ("IEStartLighting");
	}

	private IEnumerator IEStartLighting()
	{
		yield return new WaitForSeconds (0.5f);
		CarCameraFollow.Instance.SetLightingEffect (true);
		yield return new WaitForSeconds (0.5f);
		CarCameraFollow.Instance.SetLightingEffect (false);
		List<CarMove> carMoveList = new List<CarMove> ();
		carMoveList.AddRange (CarManager.Instance.carMoveList);
		carMoveList.Remove (carMove);
		for (int i = 0; i < carMoveList.Count; ++i) {
			carMoveList [i].propCon.LightingHit ();
		}
		if (isPlayer) {
			PropTips.Instance.ShowHit (PropType.Lighting);
		}
	}

	/// <summary>
	/// 使用墨鱼
	/// </summary>
	public void StartCuttleFish()
	{
		CarMove targetCar = CarManager.Instance.GetNearestCar (carMove);
		targetCar.propCon.CuttleFishHit ();
		if(isPlayer)
		{
			PropTips.Instance.ShowHit(PropType.CuttleFish);
		}
	}

	/// <summary>
	/// 使用地雷
	/// </summary>
	public void StartMine()
	{
		Transform mineTran  = itemPool.Spawn("MineBehaviour");
		mineTran.position = transform.position;
		MineBehaviour mineScript = mineTran.GetComponent<MineBehaviour>();
		mineScript.Init (this);
		mineScript.Launch (carMove.moveLen, carMove.xOffset, carMove);
	}
	#endregion

	#region 道具工具效果 

	/// <summary>
	/// Missiles the hit.
	/// </summary>
	public void MissileHit()
	{
		if (isInvincible)
			return;
		
		if (isSpeedUp || isShapeShift)
			return;
		
		if(isShield)
		{
			StopShield ();
			return;
		}

		//carMove.CarVertigo (GameData.Instance.MissileVertigoTime);
		carMove.CarExplode ();

		if(isPlayer)
		{
			AudioManger.Instance.PlaySound (AudioManger.SoundName.Bomb);
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.Missile);
		}

		StartInvincible ();
	}

	public void FlyBombHit()
	{
		if (isInvincible)
			return;
		
		if (isSpeedUp || isShapeShift)
			return;

		if(isShield)
		{
			StopShield ();
			return;
		}

		carMove.CarExplode ();

		if(isPlayer)
		{
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.FlyBmob);
		}

		StartInvincible ();
	}

	public void FireBallHit()
	{
		if (isSpeedUp || isShapeShift)
			return;
		
		if(isShield)
		{
			StopShield ();
			return;
		}
		carMove.speed = carMove.speed * 0.5f;

		if(isPlayer)
		{
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.FireBall);
		}
	}

	public void MushroomHit()
	{
		if (isInvincible)
			return;
		
		if (isSpeedUp || isShapeShift)
			return;
		
		if(isShield)
		{
			StopShield ();
			return;
		}
		carMove.CarVertigo (GameData.Instance.MushroomVertigoTime);

		if(isPlayer)
		{
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.Mushroom);
		}

		StartInvincible ();
	}

	public void GlovesHit()
	{
		if (isInvincible)
			return;
		
		if (isSpeedUp || isShapeShift)
			return;
		
		if(isShield)
		{
			StopShield ();
			return;
		}
		carMove.CarVertigo (GameData.Instance.GlovesVertigoTime);

		if(isPlayer)
		{
			AudioManger.Instance.PlaySound (AudioManger.SoundName.Bang);
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.Gloves);
		}

		StartInvincible ();
	}

	public void SlipHit()
	{
		if (isInvincible)
			return;
		
		if (isSpeedUp || isShapeShift)
			return;
		
		if(isShield)
		{
			StopShield ();
			return;
		}
		carMove.CarSlip();

		if(isPlayer)
		{
			AudioManger.Instance.PlaySound (AudioManger.SoundName.BananaSlip);
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.BananaPeel);
		}

		StartInvincible ();
	}

	public void LightingHit()
	{
		if (isInvincible)
			return;
		
		if (isSpeedUp || isShapeShift)
			return;
		
		if(isShield)
		{
			StopShield ();
			return;
		}
		carMove.CarLighting (GameData.Instance.LightingVertigoTime);

		if(isPlayer)
		{
			AudioManger.Instance.PlaySound (AudioManger.SoundName.Lighting);
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.Lighting);
		}

		StartInvincible ();
	}

	public void TurtleShellHit()
	{
		if (isInvincible)
			return;
		
		if (isSpeedUp || isShapeShift)
			return;
		
		if(isShield)
		{
			StopShield ();
			return;
		}
		//carMove.CarVertigo (GameData.Instance.TurtleShellVertigoTime);
		carMove.CarExplode ();

		if(isPlayer)
		{
			AudioManger.Instance.PlaySound (AudioManger.SoundName.ShellHit);
			GameData.Instance.beHitCount++;
		}

		StartInvincible ();
	}

	public void MineHit()
	{
		if (isInvincible)
			return;
		
		if (isSpeedUp || isShapeShift)
			return;
		
		if(isShield)
		{
			StopShield ();
			return;
		}
		Debug.Log("MineHit");
		carMove.CarExplode ();

		if(isPlayer)
		{
			AudioManger.Instance.PlaySound (AudioManger.SoundName.Bomb);
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.Mine);
		}

		StartInvincible ();
	}

	public void CuttleFishHit()
	{
		if (isSpeedUp || isShapeShift)
			return;
		
		if (isShield) {
			StopShield ();
			return;
		}
		Debug.Log("CuttleFishHit");
		if (isPlayer) {
			StartCoroutine ("IECuttleFish");
			AudioManger.Instance.PlaySound (AudioManger.SoundName.CuttleFish);
			GameData.Instance.beHitCount++;
			PropTips.Instance.ShowBeHit(PropType.CuttleFish);
		} else {
			carMove.speed = carMove.speed * 0.5f;
		}
	}

	private float cuttleFishTime;
	IEnumerator IECuttleFish()
	{
		GamePlayerUIControllor.Instance.inkSprite.SetActive (true);
		cuttleFishTime = GameData.Instance.CuttleFishTime;
		while(cuttleFishTime>0)
		{
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
			cuttleFishTime -= Time.deltaTime;
			yield return null;
		}
		GamePlayerUIControllor.Instance.inkSprite.SetActive (false);
	}

	#endregion

	#region 被攻击后，设置短暂的无敌时间
	private bool isInvincible = false;
	private float invincibleTime = 2.6f;
	[HideInInspector]
	public float colorA = 1.0f;
	Material skinMaterial;
	Material[] carMaterials;
	Transform cheTran;
	MeshRenderer carMeshRenderer;
	List<Material> lunMaterialList = new List<Material>();
	//Transform shadowTran;
	private void StartInvincible()
	{
		if (gameObject.activeInHierarchy == false)
			return;
		invincibleTime = 2.6f;
		StopCoroutine ("IEInvincible");
		StartCoroutine ("IEInvincible");
	}

	private IEnumerator IEInvincible()
	{
		isInvincible = true;
		yield return new WaitForSeconds (0.8f);

		string prafabName = ModelData.Instance.GetPrefabName (carMove.carId);
		if (skinMaterial == null) {
			skinMaterial = carTrans.Find (prafabName).GetChild(0).GetComponent<SkinnedMeshRenderer> ().material;
			cheTran = carTrans.Find (prafabName + "che");
			carMeshRenderer = cheTran.GetComponent<MeshRenderer> ();
			if (carMeshRenderer == null)
				carMaterials = cheTran.GetChild (0).GetComponent<MeshRenderer> ().materials;
			else
				carMaterials = carMeshRenderer.materials;
			lunMaterialList.Clear ();
			for (int i = 0; i < cheTran.childCount; ++i) {
				Transform lunTran = cheTran.GetChild (i);
				if (lunTran.name.Contains ("lun"))
					lunMaterialList.Add (lunTran.GetComponent<MeshRenderer> ().material);
			}
		}

		this.colorA = 1;
		DOTween.Kill (prafabName + "ChangeColorId");
		DOTween.To (() => this.colorA, x => this.colorA = x, 0, 0.3f).OnUpdate (ChangeColorAnim).SetLoops (6, LoopType.Yoyo).SetId (prafabName + "ChangeColorId");

		while (invincibleTime > 0) {
			while (GameData.Instance.IsPause)
				yield return null;
			invincibleTime -= Time.deltaTime;
			yield return null;
		}
		isInvincible = false;
	}

	private void ChangeColorAnim()
	{
		Color changeColor = new Color (1, 1, 1, this.colorA);
		skinMaterial.SetColor ("_Color", changeColor);
		for (int i = 0; i < carMaterials.Length; ++i) {
			carMaterials [i].SetColor ("_Color", changeColor);
		}
		for (int i = 0; i < lunMaterialList.Count; ++i) {
			lunMaterialList [i].SetColor ("_Color", changeColor);
		}
	}
	#endregion

	#region 变身
	public Transform carTrans;
	public Transform shapeShiftTrans;

	public bool isShapeShift = false;
	public float shapeShftTime;
	
	public void ChangeToCar()
	{
		switch (IDTool.GetModelType (carMove.carId)) {
		case 1:
		case 4:
		case 5:
			carParticleControl.PlayParticle (CarParticleType.Attack, true, 3.0f);
			break;
		case 2:
			break;
		case 3:
			carParticleControl.PlayParticle (CarParticleType.AttackBoBi, true, 3.0f);
			break;
		}
		carMove.mechaAnimManager.Attack ();
		StartCoroutine ("IEChangeToCar");
	}

	IEnumerator IEChangeToCar()
	{
		float waitTime = 1.0f;
		switch (IDTool.GetModelType (carMove.carId)) {
		case 1:
		case 3:
		case 4:
			waitTime = 1.8f;
			break;
		case 2:
		case 5:
			waitTime = 1.0f;
			break;
		}
		yield return new WaitForSeconds (waitTime);

		carParticleControl.StopParticle (CarParticleType.ShapeSpeedUp);
		carParticleControl.PlayParticle (CarParticleType.AttackExplode, true, 2.0f);

		if (isPlayer) {
			AudioManger.Instance.PlaySound (AudioManger.SoundName.ShapeShift);
			GamePlayerUIControllor.Instance.SetPropLock (false);
			CarCameraFollow.Instance.SetChangeBig (false);
			CarCameraFollow.Instance.ActiveShake (0.5f, 1.0f);
		}

		isShapeShift = false;
		shapeShiftTrans.gameObject.SetActive(false);
		carTrans.gameObject.SetActive(true);
		carMove.animManager.Setting ();

		carMove.SetMaxSpeed ();
	}
	
	public void StartShapeShift(float lastTime)
	{
		shapeShftTime = lastTime;
		if (isShapeShift)
			return;

		if (isMushroom) {
			StopCoroutine ("IEStartMushroom");
			transform.localScale = Vector3.one;
			isMushroom = false;
		}

		if (isGlove) {
			Transform glovesTrans = transform.Find ("GlovesBehaviour");
			GlovesBehaviour glovesBeha = glovesTrans.GetComponent<GlovesBehaviour> ();
			glovesBeha.Hide ();
		}

		if (isFireBall) {
			Transform fireBallTrans = transform.Find ("FireBallBehaviour");
			FireBallBehaviour fireBallScript = fireBallTrans.GetComponent<FireBallBehaviour> ();
			fireBallScript.Hide ();
		}
		
		shapeShiftTrans.gameObject.SetActive(true);
		carTrans.gameObject.SetActive(false);
		carMove.mechaAnimManager.Init();
		carMove.mechaAnimManager.Open();
		
		isShapeShift = true;

		carMove.speed = carMove.origMaxSpeed;
		carMove.SetMaxSpeed ();

		StartCoroutine ("IEWaitForPlayParticle");
		StartCoroutine ("IEWaitForChangeToCar");

		if (isPlayer) {
			AudioManger.Instance.PlaySoundByName (ModelData.Instance.GetChangeSound (GameData.Instance.selectedModelId));
			GamePlayerUIControllor.Instance.SetPropLock (true);
			AudioManger.Instance.StopSound(AudioManger.SoundName.Drifting);
			CreatePropManager.Instance.InsertGruop();
		}
	}

	private IEnumerator IEWaitForPlayParticle()
	{
		yield return new WaitForSeconds (0.8f);
		carParticleControl.PlayParticle (CarParticleType.ShapeSpeedUp);
		carParticleControl.PlayParticle (CarParticleType.AttackExplode, true, 2.0f);
		if (isPlayer) {
			AudioManger.Instance.PlaySound (AudioManger.SoundName.ShapeShift);
			CarCameraFollow.Instance.SetChangeBig (true);
			CarCameraFollow.Instance.ActiveShake (0.5f, 1.0f);
		}
	}
	
	private IEnumerator IEWaitForChangeToCar()
	{
		while (shapeShftTime > 0) {
			while (GameData.Instance.IsPause)
				yield return null;
			shapeShftTime -= Time.deltaTime;
			yield return null;
		}
		ChangeToCar();
	}
	#endregion

	#region 捡到道具和释放道具
	public int curHavePropId=0;
	public void GetProp(int propId)
	{
		curHavePropId = propId;

		if (isPlayer == false) {
			//Debug.Log("AutoUseProp");
			StopCoroutine ("IEAutoUseProp");
			StartCoroutine ("IEAutoUseProp");
		} else {
			GamePlayerUIControllor.Instance.SetPropIcon (PropConfigData.Instance.GetIconName (propId));
		}
	}

	public void UseCurProp()
	{
		if(curHavePropId ==0)
		{
			return;
		}
		//PropTips.Instance.ShowUse(this.carMove.carId,curHavePropId);
		PropType propType = (PropType)curHavePropId;
		UsePropByType(propType);
		curHavePropId =0;

	}

	public void UsePropByType(PropType propType)
	{
		switch(propType)
		{
		case PropType.Missile:
			StartMissile (1);
			break;
			
		case PropType.Shield:
			StartShield (GameData.Instance.ShieldTime);
			break;
			
		case PropType.FlyBmob:
			StartFlyBomb (3);
			break;
			
		case PropType.FireBall:
			StartFireBall (GameData.Instance.FireBallTime);
			break;

		case PropType.ShapeShift:
			StartShapeShift (GameData.Instance.ShapeTime);
			break;

		case PropType.Gloves:
			StartGloves();
			break;
			
		case PropType.Mushroom:
			StartMushroom (GameData.Instance.MushroomTime);
			break;
			
		case PropType.SpeedUp:
			StartSpeedUp (GameData.Instance.SpeedUpTime);
			break;

		case PropType.BananaPeel:
			StartBananaPeel ();
			break;

		case PropType.Lighting:
			StartLighting ();
			break;

		case PropType.RedTurtleShell:
			StartRedTurtleShell ();
			break;

		case PropType.GreenTurtleShell:
			StartGreenTurtleShell ();
			break;

		case PropType.CuttleFish:
			StartCuttleFish ();
			break;

		case PropType.Mine:
			StartMine ();
			break;

		case PropType.SpeedAdd:
			StartSpeedAdd (GameData.Instance.SpeedAddTime);
			break;
		}

		if(isPlayer)
		{
			DailyMissionCheckManager.Instance.Check(DailyMissionType.UseProp,1);
			GameData.Instance.itemUseCount++;
		}
	}

	private IEnumerator IEAutoUseProp()
	{
		while (GameData.Instance.IsPause)
			yield return null;
		while (isShapeShift)
			yield return null;
		float randTime = Random.Range(0.5f,2.5f);
		yield return new WaitForSeconds(randTime);
		UseCurProp();
	}

	#endregion
}
