using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
[CustomEditor(typeof(CatmullRomSpline)), CanEditMultipleObjects]
public class CatmullRomEditor : Editor
{
    private const float handleSize = 0.10f;

    void OnSceneGUI()
	{
        CatmullRomSpline spline = target as CatmullRomSpline;
		DrawCurve();
		Handles.color = Color.blue;

		for (int i = 0; i < spline.controlPoints.Count; i++) {
			Vector3 worldSpaceOriginalPoint = spline.transform.TransformPoint(spline.controlPoints[i]);
			EditorGUI.BeginChangeCheck();
			Vector3 worldSpacePoint = Handles.FreeMoveHandle(worldSpaceOriginalPoint, Quaternion.identity, HandleUtility.GetHandleSize(worldSpaceOriginalPoint) * handleSize, Vector3.zero, Handles.DotCap);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(target, "Move Catmull-Rom control point");
				spline.controlPoints[i] = spline.transform.InverseTransformPoint(worldSpacePoint);
			}
		}
    }

    void DrawCurve()
    {
		CatmullRomSpline spline = target as CatmullRomSpline;
        Handles.color = Color.green;
        Vector3 v = spline.GetPosition(0.0f);

        for (float i = 0.03f; i <= spline.period; i += 0.03f)
        {
			Vector3 u = spline.GetPosition(i);
            
            Handles.DrawLine(v, u);
            v = u;
        }
    }
}