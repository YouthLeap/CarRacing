using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 用来场景画线
/// </summary>
public class Line3D
{
	LineRenderer m_LR;

	static public Line3D Create(Transform p_tf)
	{
		Line3D v_ret;

		v_ret = new Line3D();
		v_ret.Init(p_tf);

		return v_ret;
	}

	public bool Disp
	{
		get
		{
			return m_LR.enabled;
		}
		set
		{
			m_LR.enabled = value;
		}
	}

	public void Init(Transform p_tf)
	{
		m_LR = p_tf.gameObject.AddComponent<LineRenderer>();
        m_LR.material = new Material(Shader.Find("Custom/MobileUnlit"));
		m_LR.SetWidth(0.1f, 0.1f);
	}

	public void setColor(Color p_start, Color p_end)
	{
		m_LR.SetColors(p_start, p_end);
	}

	public void setWidth(float p_val)
	{
		m_LR.SetWidth(p_val, p_val);
	}

	public Vector3[] Data
	{
		set
		{
			m_LR.SetVertexCount(value.Length);
			for (int i = 0; i < value.Length; i++)
				m_LR.SetPosition(i, value[i]);
		}
	}

	public Vector3[] Data3D
	{
		set
		{
			int v_idx;
			int v_next;
			Vector3 v_pos;

			m_LR.SetVertexCount(value.Length * 3);
			v_idx = 0;
			for (int i = 0; i < value.Length; i++)
			{
				v_next = i + 1;
				if (v_next >= value.Length)
					v_next %= value.Length;

				v_pos = value[i];
				m_LR.SetPosition(v_idx, v_pos);
				v_idx++;

				v_pos.y += 2;
				m_LR.SetPosition(v_idx, v_pos);
				v_idx++;

				v_pos = value[v_next];
				m_LR.SetPosition(v_idx, v_pos);
				v_idx++;
			}
		}
	}
}
