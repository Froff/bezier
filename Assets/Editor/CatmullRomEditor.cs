using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
[CustomEditor(typeof(CatmullRomSpline)), CanEditMultipleObjects]
public class CatmullRomEditor : Editor
{
    private const float handleSize = 0.14f;
    private const float pickSize = 0.16f;

    void OnSceneGUI()
	{
        CatmullRomSpline spline = target as CatmullRomSpline;
		DrawCurve();
			Handles.color = Color.blue;

			for (int i = 0; i < spline.curve.controlPoints.Count; i++) {
				Vector3 originalPoint = spline.curve.controlPoints[i];
				EditorGUI.BeginChangeCheck();
				Vector3 point = Handles.FreeMoveHandle(originalPoint, Quaternion.identity, HandleUtility.GetHandleSize(originalPoint) * handleSize, Vector3.zero, Handles.DotCap);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(target, "Move Catmull-Rom control point");
					spline.curve.controlPoints[i] = point;
				}
			}
    }

    void DrawCurve()
    {
		CatmullRomSpline spline = target as CatmullRomSpline;
        Handles.color = Color.green;
        Vector3 v = spline.GetPosition(0.0f);

        for (float i = 0.01f; i <= spline.period; i += 0.01f)
        {
			Vector3 u = spline.GetPosition(i);
            
            Handles.DrawLine(v, u);
            v = u;
        }
    }
}