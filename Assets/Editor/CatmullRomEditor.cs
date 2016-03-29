using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(CatmullRomSpline)), CanEditMultipleObjects]
public class CatmullRomEditor : Editor
{
    private const float handleSize = 0.10f;

    ReorderableList controlPointList;

    SerializedProperty periodProp;
    CatmullRomSpline spline;

    void OnEnable()
    {
        spline = target as CatmullRomSpline;
        controlPointList = new ReorderableList(serializedObject, serializedObject.FindProperty("controlPoints"), true, true, true, true);
        periodProp = serializedObject.FindProperty("period");
        
        controlPointList.drawHeaderCallback += HeaderDraw;
        controlPointList.drawElementCallback += ElementDraw;
        
    }

    void OnDisable()
    {
        controlPointList.drawHeaderCallback -= HeaderDraw;
        controlPointList.drawElementCallback -= ElementDraw;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        controlPointList.DoLayoutList();
        periodProp.floatValue = EditorGUILayout.FloatField("Period", periodProp.floatValue);
        serializedObject.ApplyModifiedProperties();
        
        
        
    }

    #region listCallbacks

    void HeaderDraw(Rect hRect)
    {
        hRect.y += 1f;
        GUI.Label(new Rect(hRect.x, hRect.y, 100F, hRect.height-2),"Control Points");
        EditorGUI.BeginChangeCheck();
        int value = EditorGUI.IntField(new Rect(hRect.x+hRect.width-45F,hRect.y,45F, hRect.height-2), controlPointList.count);
        if(EditorGUI.EndChangeCheck())
        {
            if(controlPointList.count > value)
            {
                while(controlPointList.count > value)
                {
                    controlPointList.serializedProperty.DeleteArrayElementAtIndex(controlPointList.count-1);
                }
            }
            else if (controlPointList.count < value)
            {
                while(controlPointList.count < value)
                {
                    controlPointList.serializedProperty.InsertArrayElementAtIndex(controlPointList.count);
                }
            }
        }
    }

    void ElementDraw(Rect eRect, int i, bool active, bool focused)
    {
        eRect.y += 1;
        eRect.height -= 1;

        if(GUI.Button(new Rect(eRect.x+1,eRect.y,38, eRect.height),"SET"))
        {
            controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = SceneView.lastActiveSceneView.camera.transform.position + spline.transform.position;
        }

        eRect.xMin += 40;
        eRect.xMax += 2;
        Color guiColor = GUI.color;
        GUI.color = Color.gray;
        GUI.Box(eRect, GUIContent.none, EditorStyles.helpBox);
        GUI.color = guiColor;
        eRect.xMin -= 40;
        eRect.xMax -= 2;
        eRect.y += 2;
        eRect.height -= 1;
        EditorGUI.BeginChangeCheck();
        Vector3 value = EditorGUI.Vector3Field(new Rect (eRect.x+40, eRect.y, eRect.width-40, eRect.height-2),GUIContent.none, controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value);
        if(EditorGUI.EndChangeCheck())
        {
            controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = value;
        }

    }

    #endregion

    #region SceneView

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
    #endregion
}