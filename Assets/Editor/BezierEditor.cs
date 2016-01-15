using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(BezierSpline)), CanEditMultipleObjects]
public class BezierEditor : Editor
{
    private const float handleSize = 0.14f;
    private const float pickSize = 0.16f;

    private int selectedIndex = -1;

    private bool curveEdit = false;
    private CurveEditSettings editSettings = CurveEditSettings.MAIN_NODES;

    [System.Flags]
    private enum CurveEditSettings : int
    {
        MAIN_NODES = 1, //0001
        CTRL_NODES = 2, //0010
        ALONG_AXIS = 4, //0100

    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfDirtyOrScript();
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

        
        curveEdit = GUILayout.Toggle(curveEdit, "CtrlNodes",EditorStyles.toolbarButton);
        if(curveEdit)
        {
            GUI.skin.button.active.textColor = Color.blue;

            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            
            
            if (GUILayout.Toggle((editSettings & CurveEditSettings.CTRL_NODES) == CurveEditSettings.CTRL_NODES, "Control Nodes", EditorStyles.toolbarButton))
            {
                editSettings |= CurveEditSettings.CTRL_NODES;
            }
            GUILayout.EndHorizontal();
            
            GUI.skin.button.active.textColor = Color.black;
            GUI.color = guiColor;
            SceneView.RepaintAll();
            
        }
        
        if (curves.arraySize <= 1)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Remove Curve"))
        {
            curves.DeleteArrayElementAtIndex(curves.arraySize - 1);
            serializedObject.ApplyModifiedProperties();
        }
        GUI.enabled = true;



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

            if (curveEdit)
            {
                //DRAW HANDLES
                if ((editSettings & CurveEditSettings.CTRL_NODES) == CurveEditSettings.CTRL_NODES)
                {

                    //DRAW HELPLINES
                    DrawHelpLine(o, a, Color.red);
                    DrawHelpLine(c, b, Color.blue);

                    Handles.color = Color.white;

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
                            spline.curves[i].a = curve.a;
                            curve.a = handleTransform.InverseTransformPoint(a);
                        }
                    }

                    if (Handles.Button(b, handleRotation, handleSize * HandleUtility.GetHandleSize(b), pickSize * HandleUtility.GetHandleSize(b), Handles.SphereCap))
                    {
                        selectedIndex = (selectedIndex < (i * 3) + 1) ? (i * 3) + 1 : -1;
                    }
                    if (selectedIndex == (i * 3) + 1)
                    {
                        EditorGUI.BeginChangeCheck();
                        b = Handles.DoPositionHandle(b, handleRotation);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(spline, "Move Bezier Point");
                            EditorUtility.SetDirty(spline);
                            spline.curves[i].b = curve.b;
                            curve.b = handleTransform.InverseTransformPoint(b);
                        }
                    }
                }
                if ((editSettings & CurveEditSettings.MAIN_NODES) == CurveEditSettings.MAIN_NODES)
                {
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
                            spline.curves[i].c = curve.c;
                            curve.c = handleTransform.InverseTransformPoint(c);
                        }
                    }
                }

            }
            o = curve.c; //Last endpoint is new startpoint
        }
    }

    static void DrawCurve(Vector3 o, Vector3 a, Vector3 b, Vector3 c)
    {
        Handles.color = Color.green;
        Vector3 v = BezierCurve.Position(0.0f, o, a, b, c);

        for (int i = 1; i <= 30; i++)
        {
            Vector3 u = BezierCurve.Position((float)i/30.0f, o, a, b, c);
            
            Handles.DrawLine(v, u);
            v = u;
        }
    }

    static void DrawHelpLine(Vector3 a, Vector3 b, Color col)
    { 
        Handles.color = col;
        Handles.DrawLine(a, b);
    }
}