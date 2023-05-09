using UnityEngine;
using System.Collections;

public class IDTool{

	/// <summary>
	/// 获得模型的等级.
	/// </summary>
	/// <returns>The level.</returns>
	/// <param name="modelId">Model identifier.</param>
	public static int GetModelLevel(int modelId)
	{
		return modelId % 100;
	}
	/// <summary>
	/// 获得模型类型，1 初代，2 艾斯，3 奥之母，4 赛文，5 泰罗
	/// </summary>
	/// <returns>The modle kind.</returns>
	/// <param name="modeId">Mode identifier.</param>
	public static int GetModelType(int modeId)
	{
		return modeId / 100;
	}
	
	/// <summary>
	/// 获得血量的格数.
	/// </summary>
	/// <returns>The HP step.</returns>
	/// <param name="hp">Hp.</param>
	public static int GetHPStep(float hp)
	{
		return ((int)hp) / 100;
	}
	
	/// <summary>
	/// 获得攻击的格数.
	/// </summary>
	/// <returns>The attack step.</returns>
	/// <param name="attack">Attack.</param>
	public static int GetAttackStep(float attack)
	{
		return ((int)attack) / 100;
	}
	
	/// <summary>
	/// 获得防御的格数.
	/// </summary>
	/// <returns>The protect step.</returns>
	/// <param name="protect">Protect.</param>
	public static int GetProtectStep(float protect)
	{
		return ((int)protect) / 40;
	}
}
