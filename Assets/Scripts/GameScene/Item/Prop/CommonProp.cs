using UnityEngine;
using System.Collections;

/// <summary>
/// 通用的可捡到道具控制代码
/// </summary>
public class CommonProp : PropsBase {

  public override void GetItem (PropControl propCon)
	{
		if(propCon == null)
			return;
		base.GetItem (propCon);
		propCon.GetProp(id);
	}
}
