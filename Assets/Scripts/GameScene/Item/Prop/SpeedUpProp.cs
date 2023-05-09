using UnityEngine;
using System.Collections;

/// <summary>
/// 道路中的加速带
/// </summary>
public class SpeedUpProp : PropsBase {

	public override void Init ()
	{
		base.Init ();
	}

	public override void GetItem (PropControl  propCon= null)
	{
		propCon.StartSpeedAdd (1.0f);
	}
}