using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move : MonoBehaviour {

	public enum Direction
	{
		Left,
		Right,
		Up,
		Down
	}
	
	public Direction direction = Direction.Right;

	public float width;
	public float speed;

	private int index;
	private float nextPosX;
	private List<Transform> tranList = new List<Transform>();
	
	void Start ()
	{
		for (int i=0; i<transform.childCount; ++i) {
			tranList.Add (transform.GetChild(i));
		}
		index = 0;
		nextPosX = width;
	}
	
	void LateUpdate () {

		if (speed == 0)
			return;

		switch (direction) {
		case Direction.Left:
			transform.Translate (Vector3.left * speed * Time.deltaTime);
			break;
		case Direction.Right:
			transform.Translate (Vector3.right * speed * Time.deltaTime);
			break;
		case Direction.Up:
			transform.Translate (Vector3.up * speed * Time.deltaTime);
			break;
		case Direction.Down:
			transform.Translate (Vector3.down * speed * Time.deltaTime);
			break;
		}
		
		if (Mathf.Abs(transform.localPosition.x) > nextPosX) {
			switch(direction)
			{
			case Direction.Left:
				tranList[index].localPosition += new Vector3(width * tranList.Count, 0, 0);
				break;
			case Direction.Right:
				tranList[index].localPosition -= new Vector3(width * tranList.Count, 0, 0);
				break;
			case Direction.Up:
				tranList[index].localPosition += new Vector3(0, width * tranList.Count, 0);
				break;
			case Direction.Down:
				tranList[index].localPosition -= new Vector3(0, width * tranList.Count, 0);
				break;
			}
			++ index;
			if(index >= tranList.Count)
				index = 0;
			nextPosX += width;
		}
	}
}
