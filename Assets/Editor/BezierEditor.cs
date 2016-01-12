using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(BezierSpline)), CanEditMultipleObjects]
public class BezierEditor : Editor
{
    private const float handleSize = 0.14f;
    private const float pickSize = 0.16f;

    private int selectedIndex = -1;

    public override void OnInspectorGUI()
    {
        SerializedProperty curves = serializedObject.FindProperty("curves");
        SerializedProperty sp = serializedObject.GetIterator();
       

        GUI.enabled = false;

        Color guiColor = GUI.color;
        GUI.color = Color.yellow;

        Rect r = EditorGUILayout.GetControlRect();
        r = new Rect(r.x + BezierCurveDrawer.LABEL_WIDTH, r.y, r.width - BezierCurveDrawer.PAD, r.height);

        GUI.Box(r, "");
        GUI.color = guiColor;

        EditorGUI.Vector3Field(r, "", Vector3.zero);
        GUI.enabled = true;

        int index = 0;
        int childStep = 0;
        while (sp.NextVisible(childStep<3))
        {
            
            if(index > 2)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sp);
                if(EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
            childStep++;
            index++;
        }
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Add Curve"))
        {
            curves.InsertArrayElementAtIndex(curves.arraySize);
            serializedObject.ApplyModifiedProperties();
        }
        if(curves.arraySize <= 1)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Remove Curve"))
        {
            curves.DeleteArrayElementAtIndex(curves.arraySize-1);
            serializedObject.ApplyModifiedProperties();
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();



    }

    void OnSceneGUI()
    {
        BezierSpline spline = target as BezierSpline;

        Transform handleTransform = spline.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 o = handleTransform.position;

        for (int i = 0; i < spline.curves.Length; i++)
        {
            BezierCurve curve = spline.curves[i];

            //DEFINE POINTS in localspace

            Vector3 a = handleTransform.TransformPoint(curve.a);
            Vector3 b = handleTransform.TransformPoint(curve.b);
            Vector3 c = handleTransform.TransformPoint(curve.c);

            //DRAW CURVES

            DrawCurve(o, a, b, c);

            //DRAW HELPLINES
            DrawHelpLine(o, a, Color.red);
            DrawHelpLine(c, b, Color.blue);

            Handles.color = Color.white;


            //DRAW HANDLES
            if (Handles.Button(a, handleRotation, handleSize * HandleUtility.GetHandleSize(a), pickSize * HandleUtility.GetHandleSize(a), Handles.SphereCap))
            {
                selectedIndex = (selectedIndex < (i * 3) + 0) ? (i * 3) + 0 : -1;
            }
            if (selectedIndex == (i * 3) + 0)
            {
                EditorGUI.BeginChangeCheck();
                a = Handles.DoPositionHandle(a, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Bezier Point");
                    EditorUtility.SetDirty(spline);

                    curve.a = handleTransform.InverseTransformPoint(a);
                }
            }

            if (Handles.Button(b, handleRotation, handleSize * HandleUtility.GetHandleSize(b), pickSize * HandleUtility.GetHandleSize(b), Handles.SphereCap))
            {
                selectedIndex = (selectedIndex < (i*3)+1) ? (i * 3) + 1 : -1;
            }
            if (selectedIndex == (i * 3) + 1)
            {
                EditorGUI.BeginChangeCheck();
                b = Handles.DoPositionHandle(b, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Bezier Point");
                    EditorUtility.SetDirty(spline);

                    curve.b = handleTransform.InverseTransformPoint(b);
                }
            }

            if (Handles.Button(c, handleRotation, handleSize * HandleUtility.GetHandleSize(c), pickSize * HandleUtility.GetHandleSize(c), Handles.SphereCap))
            {
                selectedIndex = (selectedIndex < (i * 3) + 2) ? (i * 3) + 2 : -1;
            }
            if (selectedIndex == (i * 3) + 2)
            {
                EditorGUI.BeginChangeCheck();
                c = Handles.DoPositionHandle(c, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Bezier Point");
                    EditorUtility.SetDirty(spline);

                    curve.c = handleTransform.InverseTransformPoint(c);
                }
            }


            o = curve.c; //Last endpoint is new startpoint
        }
    }

    static void DrawCurve(Vector3 o, Vector3 a, Vector3 b, Vector3 c)
    {
        Handles.color = Color.green;
        Vector3 v = BezierCurve.Position(0.0f, o, a, b, c);

        for (int i = 1; i <= 50; i++)
        {
            Vector3 u = BezierCurve.Position((float)i/50.0f, o, a, b, c);
            Handles.DrawLine(v, u);
            v = u;
        }
    }

    

    //    for (int i = 0; i < bezier.CtrlPoints.Length; i+=3)
    //    {
    //        //DEFINE POINTS in localspace

    //        Vector3 b = handleTransform.TransformPoint(bezier.CtrlPoints[i]);
    //        Vector3 c = handleTransform.TransformPoint(bezier.CtrlPoints[i + 1]);
    //        Vector3 d = handleTransform.TransformPoint(bezier.CtrlPoints[i + 2]);

    //        Vector3[] curvePoints = { a, b, c, d }; //These have been transformed into localspace points

    //        BezierEditor.DrawCurve(bezier);

    //        DrawControlPointLine(a, b, Color.red);
    //        DrawControlPointLine(d, c, Color.blue);

    //        Handles.color = Color.white;
    //        for(int l = 0; l < 4; l++)
    //        {
    //            if (Handles.Button(curvePoints[l], handleRotation, handleSize * HandleUtility.GetHandleSize(curvePoints[l]), pickSize * HandleUtility.GetHandleSize(curvePoints[l]), Handles.SphereCap))
    //            {
    //                selectedIndex = (selectedIndex != (i-1) + l) ? (i - 1) + l : -1;
    //            }
    //            if (selectedIndex == (i - 1) + l)
    //            {
    //                EditorGUI.BeginChangeCheck();
    //                curvePoints[l] = Handles.DoPositionHandle(curvePoints[l], handleRotation);
    //                if (EditorGUI.EndChangeCheck())
    //                {
    //                    Undo.RecordObject(bezier, "Move Bezier Point");
    //                    EditorUtility.SetDirty(bezier);
    //                    bezier.CtrlPoints[(i - 1) + l] = handleTransform.InverseTransformPoint(curvePoints[l]);
    //                }
    //            }
    //        }


    //    }

    //}

    //static void DrawCurve(BezierSpline spline)
    //{
    //    Handles.color = Color.green;
    //    Vector3 v = spline.Position(0f);
    //    for (int i = 1; i <= 50*((spline.CtrlPoints.Length-1)/3); i++)
    //    {
    //        Vector3 u = spline.Position((float)i / 50 * ((spline.CtrlPoints.Length - 1) / 3));
    //        Handles.DrawLine(v, u);
    //        v = u;
    //    }
    //}

    static void DrawHelpLine(Vector3 a, Vector3 b, Color col)
    { 
        Handles.color = col;
        Handles.DrawLine(a, b);
    }
}