using UnityEngine;
using System.Collections;

public class PlayerWheelsRotation : MonoBehaviour
{
    public CarMove runCar;

    [Range(0,1)]
    public float rotateYPercent = 1;
    public float rotateDeltaY = 10;
    public float rotateDeltaX = 100;

    private Transform mTran;
    private float rotateX, rotateY;
	private float clamSpeed=0f;

	void Start()
    {
        mTran = transform;

		runCar = GetComponentInParent<CarMove>();
    }

	void FixedUpdate ()
    {
        if (runCar == null)
            return;
		if(GameData.Instance.IsPause)
		{
			return;
		}
		clamSpeed = runCar.speed;
		clamSpeed=Mathf.Clamp(clamSpeed,0,20f);
        rotateX += runCar.speed * rotateDeltaX * Time.fixedDeltaTime;
        if (rotateX >= 360)
            rotateX -= 360;

        rotateY = Mathf.Lerp(0, runCar.yMaxRotate * rotateYPercent, Time.fixedDeltaTime * rotateDeltaY);

        mTran.localEulerAngles = new Vector3(rotateX, rotateY, 0);
	}
}
