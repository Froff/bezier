using UnityEngine;
using System.Collections;

public class BezierSpline : MonoBehaviour
{
    
   //public Vector3[] CtrlPoints = { Vector3.right * 0.5f, Vector3.right, Vector3.right * 1.5f };

    public BezierCurve[] curves = { new BezierCurve(Vector3.right, Vector3.up, Vector3.forward) };

    public Vector3 Position(float t)
    {

        t = (Mathf.Clamp01(t) * (curves.Length));
        int i = (int)t;
        t -= i;
        
        if (i==0)
        {
            return curves[i].Position(t, transform.position);
        }
        return curves[i].Position(t, curves[i - 1].c);
    }
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

    public Vector3 Position(float t, Vector3 start)
    {
        float n = 1 - t;
        Vector3 p = start * n * n * n;  //(1-t)^3*a
                p += 3 * n * n * t * a; //3(1-t)^2*t*b
                p += 3 * n * t * t * b; //3(1-t)*t^2*c
                p += t * t * t * c;     //t^3*d

        return p;
    }

    public static Vector3 Position(float t, Vector3 start, BezierCurve curve)
    {
        return Position(t, start, curve.a, curve.b, curve.c);
    }

    public static Vector3 Position(float t, Vector3 o, Vector3 a, Vector3 b, Vector3 c)
    {
        float n = 1 - t;
        Vector3 p = o * n * n * n; 
        p += 3 * n * n * t * a;    
        p += 3 * n * t * t * b;     
        p += t * t * t * c;         

        return p;
    }
}