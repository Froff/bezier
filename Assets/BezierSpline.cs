using UnityEngine;
using System.Collections;

public class BezierSpline : MonoBehaviour
{
    public Vector3[] CtrlPoints = { Vector3.zero, Vector3.right * 0.5f, Vector3.right, Vector3.right * 1.5f };

    BezierCurve[] curves = new BezierCurve[2];

    public Vector3 Position(float t)
    {
        float a = 1 - t;

        return transform.TransformPoint( Mathf.Pow(a, 3) * CtrlPoints[0] +
            3 * Mathf.Pow(a, 2) * t * CtrlPoints[1] +
            3 * a * Mathf.Pow(t, 2) * CtrlPoints[2] +
            Mathf.Pow(t, 3) * CtrlPoints[3]);
    }
}

[System.Serializable]
public class BezierCurve
{
    Vector3 a, b, c;
    public BezierCurve(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }



    public static Vector3 Position(float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        float n = 1 - t;
        Vector3 p = a * n * n * n;  //(1-t)^3*a
        p += 3 * n * n * t * b;     //3(1-t)^2*t*b
        p += 3 * n * t * t * c;     //3(1-t)*t^2*c
        p += t * t * t * d;         //t^3*d

        return p;
    }
}