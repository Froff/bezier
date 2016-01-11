using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(BezierSpline))]
public class BezierEditor : Editor
{
    private const float handleSize = 0.14f;
    private const float pickSize = 0.16f;

    private int selectedIndex = -1;

    void OnSceneGUI()
    {
        BezierSpline bezier = target as BezierSpline;
        
        Transform handleTransform = bezier.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        for (int i = 1; i < bezier.CtrlPoints.Length; i+=3)
        {
            //DEFINE POINTS in localspace

            Vector3 a = handleTransform.TransformPoint(bezier.CtrlPoints[i - 1]);
            Vector3 b = handleTransform.TransformPoint(bezier.CtrlPoints[i]);
            Vector3 c = handleTransform.TransformPoint(bezier.CtrlPoints[i + 1]);
            Vector3 d = handleTransform.TransformPoint(bezier.CtrlPoints[i + 2]);

            Vector3[] curvePoints = { a, b, c, d }; //These have been transformed into localspace points

            BezierEditor.DrawCurve(bezier);

            DrawControlPointLine(a, b, Color.red);
            DrawControlPointLine(d, c, Color.blue);

            Handles.color = Color.white;
            for(int l = 0; l < 4; l++)
            {
                if (Handles.Button(curvePoints[l], handleRotation, handleSize * HandleUtility.GetHandleSize(curvePoints[l]), pickSize * HandleUtility.GetHandleSize(curvePoints[l]), Handles.SphereCap))
                {
                    selectedIndex = (selectedIndex != (i-1) + l) ? (i - 1) + l : -1;
                }
                if (selectedIndex == (i - 1) + l)
                {
                    EditorGUI.BeginChangeCheck();
                    curvePoints[l] = Handles.DoPositionHandle(curvePoints[l], handleRotation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(bezier, "Move Bezier Point");
                        EditorUtility.SetDirty(bezier);
                        bezier.CtrlPoints[(i - 1) + l] = handleTransform.InverseTransformPoint(curvePoints[l]);
                    }
                }
            }
            
            
        }
        
    }

    static void DrawCurve(BezierSpline spline)
    {
        Handles.color = Color.green;
        Vector3 v = spline.Position(0f);
        for (int i = 1; i <= 50*((spline.CtrlPoints.Length-1)/3); i++)
        {
            Vector3 u = spline.Position((float)i / 50 * ((spline.CtrlPoints.Length - 1) / 3));
            Handles.DrawLine(v, u);
            v = u;
        }
    }

    static void DrawControlPointLine(Vector3 a, Vector3 b, Color col)
    { 
        Handles.color = col;
        Handles.DrawLine(a, b);
    }
}
