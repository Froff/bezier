using UnityEngine;
using System.Collections;

public class BezierSpline : MonoBehaviour
{
    
   //public Vector3[] CtrlPoints = { Vector3.right * 0.5f, Vector3.right, Vector3.right * 1.5f };

    public BezierCurve[] curves = { new BezierCurve(Vector3.right, Vector3.up, Vector3.forward)};

    //public Vector3 Position(float t)
    //{
    //    float a = 1 - t;

    //    return transform.TransformPoint(
    //        3 * Mathf.Pow(a, 2) * t * CtrlPoints[0] +
    //        3 * a * Mathf.Pow(t, 2) * CtrlPoints[1] +
    //        Mathf.Pow(t, 3) * CtrlPoints[2]);
    //}
}

[System.Serializable]
public class BezierCurve
{
    public Vector3 a, b, c;
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