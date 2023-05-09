using System;
using UnityEngine;

public static class BezierSpline
{

    //
    // Nested Types
    //
    public struct BezierControls
    {
        public Vector3 controlPt0;

        public Vector3 controlPt1;

        public Vector3 controlPt2;

        public Vector3 controlPt3;
    }

    //
    // Static Methods
    //
    public static void CalculateBounds(ref Vector3[] box, int numSteps, BezierSpline.BezierControls controls)
    {
        BezierSpline.BezierControls controls2;
        controls2.controlPt0 = Vector3.zero;
        controls2.controlPt1 = controls.controlPt1 - controls.controlPt0;
        controls2.controlPt2 = controls.controlPt2 - controls.controlPt0;
        controls2.controlPt3 = controls.controlPt3 - controls.controlPt0;
        Quaternion quaternion = Quaternion.FromToRotation(controls2.controlPt3, Vector3.forward);
        controls2.controlPt1 = quaternion * controls2.controlPt1;
        controls2.controlPt2 = quaternion * controls2.controlPt2;
        controls2.controlPt3 = quaternion * controls2.controlPt3;
        Vector3 zero = Vector3.zero;
        Vector3 zero2 = Vector3.zero;
        BezierSpline.CalculateBoundsAxisAligned(ref zero, ref zero2, numSteps, controls2);
        box[0].Set(zero.x, zero.y, zero.z);
        box[1].Set(zero.x, zero.y, zero2.z);
        box[2].Set(zero2.x, zero.y, zero2.z);
        box[3].Set(zero2.x, zero.y, zero.z);
        box[4].Set(zero.x, zero2.y, zero.z);
        box[5].Set(zero.x, zero2.y, zero2.z);
        box[6].Set(zero2.x, zero2.y, zero2.z);
        box[7].Set(zero2.x, zero2.y, zero.z);
        for (int i = 0; i < 8; i++)
        {
            box[i] = Quaternion.Inverse(quaternion) * box[i];
            box[i] += controls.controlPt0;
        }
    }

    public static void CalculateBoundsAxisAligned(ref Vector3 min, ref Vector3 max, int numSteps, BezierSpline.BezierControls controls)
    {
        int num = 0;
        float[] array = new float[16];
        for (int i = 0; i < numSteps + 1; i++)
        {
            float startPoint = (float)i / (float)numSteps;
            float num2 = BezierSpline.NewtonRaphsonIterationX(startPoint, controls);
            if (num2 > 0 && num2 < 1)
            {
                bool flag = false;
                for (int j = 0; j < num; j++)
                {
                    if ((double)Mathf.Abs(num2 - array[j]) <= 0.001)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    array[num] = num2;
                    num++;
                }
            }
            num2 = BezierSpline.NewtonRaphsonIterationZ(startPoint, controls);
            if (num2 > 0 && num2 < 1)
            {
                bool flag2 = false;
                for (int k = 0; k < num; k++)
                {
                    if ((double)Mathf.Abs(num2 - array[k]) <= 0.001)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    array[num] = num2;
                    num++;
                }
            }
        }
        min = Vector3.Min(controls.controlPt0, controls.controlPt3);
        max = Vector3.Max(controls.controlPt0, controls.controlPt3);
        for (int l = 0; l < num; l++)
        {
            float t = array[l];
            Vector3 vector = BezierSpline.PointAlongBezier(t, controls);
            min = Vector3.Min(min, vector);
            max = Vector3.Max(max, vector);
        }
    }

    public static float CalculateLength(BezierSpline.BezierControls controls)
    {
        return BezierSpline.CalculateSegmentLengthRecursive(0, 1, controls);
    }

    public static float CalculateLengthBruteForce(BezierSpline.BezierControls controls, int iterations)
    {
        float num = 0;
        for (int i = 0; i < iterations; i++)
        {
            float t = (float)i / (float)iterations;
            Vector3 vector = BezierSpline.PointAlongBezier(t, controls);
            float t2 = (float)(i + 1) / (float)iterations;
            Vector3 vector2 = BezierSpline.PointAlongBezier(t2, controls);
            num += (vector2 - vector).magnitude;
        }
        return num;
    }

    public static float CalculateParallelSplineLengthScale(BezierSpline.BezierControls controls, Vector3 upStart, Vector3 upEnd, float offset)
    {
        Vector3 vector = BezierSpline.TangentAlongBezier(0, controls);
        Vector3 vector2 = BezierSpline.TangentAlongBezier(1, controls);
        Vector3 vector3 = Vector3.Cross(upStart, vector);
        Vector3 vector4 = Vector3.Cross(upEnd, vector2);
        vector3.Normalize();
        vector4.Normalize();
        Vector3 vector5 = controls.controlPt0 + vector3 * offset;
        Vector3 vector6 = controls.controlPt3 + vector4 * offset;
        float magnitude = (controls.controlPt0 - controls.controlPt3).magnitude;
        float magnitude2 = (vector5 - vector6).magnitude;
        return magnitude2 / magnitude;
    }

    public static float CalculateSegmentLengthRecursive(float start, float end, BezierSpline.BezierControls controls)
    {
        Vector3 vector = BezierSpline.PointAlongBezier(start, controls);
        Vector3 vector2 = BezierSpline.PointAlongBezier(end, controls);
        float magnitude = (vector2 - vector).magnitude;
        float num = (start + end) * 0.5f;
        Vector3 vector3 = BezierSpline.PointAlongBezier(num, controls);
        float num2 = (vector - vector3).magnitude + (vector2 - vector3).magnitude;
        if ((double)num2 > 0.1f && (double)Mathf.Abs(magnitude - num2) > 1E-06)
        {
            return BezierSpline.CalculateSegmentLengthRecursive(start, num, controls) + BezierSpline.CalculateSegmentLengthRecursive(num, end, controls);
        }
        return num2;
    }

    public static BezierSpline.BezierControls CombineSplines(BezierSpline.BezierControls spline0, BezierSpline.BezierControls spline1)
    {
        float num = 0.5f;
        Vector3 vector = 0.5f * (spline0.controlPt1 - spline0.controlPt0);
        BezierSpline.BezierControls bezierControls;
        bezierControls.controlPt0 = spline0.controlPt0;
        bezierControls.controlPt1 = spline0.controlPt1;
        bezierControls.controlPt2 = spline1.controlPt2;
        bezierControls.controlPt3 = spline1.controlPt3;
        Vector3 vector2 = BezierSpline.PointAlongBezier(num, bezierControls);
        Vector3 vector3 = bezierControls.controlPt1 + num * (bezierControls.controlPt2 - bezierControls.controlPt1);
        float num2 = num * num;
        float num3 = num * num2;
        float num4 = 1 - num;
        float num5 = num4 * num4;
        float num6 = num5 * num4;
        float num7 = Mathf.Abs((num3 + num6) / (num3 + num6 - 1));
        Vector3 vector4 = vector2 + (vector2 - vector3) / num7;
        Vector3 vector5 = vector4 + (1 + num7) * (spline0.controlPt3 - vector4);
        Vector3 vector6 = spline0.controlPt3 - vector * 0.5f;
        Vector3 vector7 = spline0.controlPt3 + vector * 0.5f;
        Vector3 vector8 = vector5 + 2 * (vector6 - vector5);
        Vector3 vector9 = vector5 + 2 * (vector7 - vector5);
        bezierControls.controlPt1 = bezierControls.controlPt0 + 2 * (vector8 - bezierControls.controlPt0);
        bezierControls.controlPt2 = bezierControls.controlPt3 + 2 * (vector9 - bezierControls.controlPt3);
        return bezierControls;
    }

    public static BezierSpline.BezierControls CreateParallelSpline(BezierSpline.BezierControls controls, Vector3 upStart, Vector3 upEnd, float offset)
    {
        Vector3 vector = BezierSpline.TangentAlongBezier(0, controls);
        Vector3 vector2 = BezierSpline.TangentAlongBezier(1, controls);
        Vector3 vector3 = Vector3.Cross(upStart, vector);
        Vector3 vector4 = Vector3.Cross(upEnd, vector2);
        vector3.Normalize();
        vector4.Normalize();
        Vector3 vector5 = controls.controlPt0 + vector3 * offset;
        Vector3 vector6 = controls.controlPt3 + vector4 * offset;
        float magnitude = (controls.controlPt0 - controls.controlPt3).magnitude;
        float magnitude2 = (vector5 - vector6).magnitude;
        float num = magnitude2 / magnitude;
        Vector3 controlPt = (controls.controlPt1 - controls.controlPt0) * num + vector5;
        Vector3 controlPt2 = (controls.controlPt2 - controls.controlPt3) * num + vector6;
        return new BezierSpline.BezierControls {
            controlPt0 = vector5,
            controlPt1 = controlPt,
            controlPt2 = controlPt2,
            controlPt3 = vector6
        };
    }

    public static Vector3 DerivativeAlongBezier(float t, BezierSpline.BezierControls controls)
    {
        float num = t * t;
        float num2 = 1 - t;
        float num3 = num2 * num2;
        return 3 * (controls.controlPt1 - controls.controlPt0) * num3 + 3 * (controls.controlPt2 - controls.controlPt1) * 2 * t * num2 + 3 * (controls.controlPt3 - controls.controlPt2) * num;
    }

    public static float FindNearestPointOnSpline(Vector3 point, BezierSpline.BezierControls controls, float epsilon)
    {
        float num = 0.5f;
        float num2 = 0.5f;
        int num3 = 1000;
        while (num2 > epsilon && num3 > 0)
        {
            num3--;
            Vector3 vector = BezierSpline.PointAlongBezier(num, controls);
            Vector3 vector2 = BezierSpline.PointAlongBezier(num - num2 / 2, controls);
            Vector3 vector3 = BezierSpline.PointAlongBezier(num + num2 / 2, controls);
            if ((vector - point).sqrMagnitude > (vector2 - point).sqrMagnitude || (vector - point).sqrMagnitude > (vector3 - point).sqrMagnitude)
            {
                if ((vector2 - point).sqrMagnitude < (vector3 - point).sqrMagnitude)
                {
                    num -= num2 / 2;
                }
                else
                {
                    num += num2 / 2;
                }
            }
            num2 *= 0.5f;
        }
        return num;
    }

    public static float FindNearestPointOnSpline(Vector3 point, BezierSpline.BezierControls controls)
    {
        return BezierSpline.FindNearestPointOnSpline(point, controls, 0.001f);
    }

    //牛顿迭代法，单精度
    public static float NewtonRaphsonIterationX(float startPoint, BezierSpline.BezierControls controls)
    {
        float num = startPoint;
        int num2 = 0;
        while (true)
        {
            Vector3 vector = BezierSpline.DerivativeAlongBezier(num, controls);
            Vector3 vector2 = BezierSpline.SecondDerivativeAlongBezier(num, controls);
            float num3 = num - vector.x / vector2.x;
            if (Mathf.Abs(num3 - num) < 0.001f)
            {
                break;
            }
            num2++;
            if (num2 > 10)
            {
                goto Block_2;
            }
            num = num3;
        }
        return num;
        Block_2:
        return 0;
    }

    //牛顿迭代法，双精度
    public static float NewtonRaphsonIterationZ(float startPoint, BezierSpline.BezierControls controls)
    {
        float num = startPoint;
        int num2 = 0;
        while (true)
        {
            Vector3 vector = BezierSpline.DerivativeAlongBezier(num, controls);
            Vector3 vector2 = BezierSpline.SecondDerivativeAlongBezier(num, controls);
            float num3 = num - vector.z / vector2.z;
            if ((double)Mathf.Abs(num3 - num) < 0.001f)
            {
                break;
            }
            num2++;
            if (num2 > 10)
            {
                goto Block_2;
            }
            num = num3;
        }
        return num;
        Block_2:
        return 0;
    }

    public static Vector3 PointAlongBezier(float t, BezierSpline.BezierControls controls)
    {
        float num = t * t;
        float num2 = num * t;
        float num3 = 1 - t;
        float num4 = num3 * num3;
        float num5 = num4 * num3;
        return num5 * controls.controlPt0 + 3 * num4 * t * controls.controlPt1 + 3 * num3 * num * controls.controlPt2 + num2 * controls.controlPt3;
    }

    public static float PointAndTangentAlongBezier(ref Vector3 point, ref Vector3 tangent, float t, BezierSpline.BezierControls controls)
    {
        float num = t * t;
        float num2 = num * t;
        float num3 = 1 - t;
        float num4 = num3 * num3;
        float num5 = num4 * num3;
        point = num5 * controls.controlPt0 + 3 * num4 * t * controls.controlPt1 + 3 * num3 * num * controls.controlPt2 + num2 * controls.controlPt3;
        tangent = (controls.controlPt1 - controls.controlPt0) * num4 + (controls.controlPt2 - controls.controlPt1) * 2 * t * num3 + (controls.controlPt3 - controls.controlPt2) * num;
        float magnitude = tangent.magnitude;
        if (magnitude > 1E-05)
        {
            tangent /= magnitude;
        }
        else
        {
            tangent = Vector3.zero;
        }
        return magnitude;
    }

    public static Vector3 SecondDerivativeAlongBezier(float t, BezierSpline.BezierControls controls)
    {
        float num = 1 - t;
        return 6 * (controls.controlPt2 - 2 * controls.controlPt1 + controls.controlPt0) * num + 6 * (controls.controlPt3 - 2 * controls.controlPt2 + controls.controlPt1) * t;
    }

    public static void SplitSpline(ref BezierSpline.BezierControls spline0, ref BezierSpline.BezierControls spline1, BezierSpline.BezierControls controls, float ratio)
    {
        Vector3 vector = controls.controlPt0 + ratio * (controls.controlPt1 - controls.controlPt0);
        Vector3 vector2 = controls.controlPt1 + ratio * (controls.controlPt2 - controls.controlPt1);
        Vector3 vector3 = controls.controlPt2 + ratio * (controls.controlPt3 - controls.controlPt2);
        Vector3 vector4 = vector + ratio * (vector2 - vector);
        Vector3 vector5 = vector2 + ratio * (vector3 - vector2);
        Vector3 vector6 = vector4 + ratio * (vector5 - vector4);
        spline0.controlPt0 = controls.controlPt0;
        spline0.controlPt1 = vector;
        spline0.controlPt2 = vector4;
        spline0.controlPt3 = vector6;
        spline1.controlPt0 = vector6;
        spline1.controlPt1 = vector5;
        spline1.controlPt2 = vector3;
        spline1.controlPt3 = controls.controlPt3;
    }

    public static Vector3 TangentAlongBezier(float t, BezierSpline.BezierControls controls)
    {
        float num = t * t;
        float num2 = 1 - t;
        float num3 = num2 * num2;
        Vector3 result = (controls.controlPt1 - controls.controlPt0) * num3 + (controls.controlPt2 - controls.controlPt1) * 2 * t * num2 + (controls.controlPt3 - controls.controlPt2) * num;
        result.Normalize();
        return result;
    }


}
