using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(CatmullRomSpline)), CanEditMultipleObjects]
public class CatmullRomEditor : Editor
{
    private const float handleSize = 0.10f;

    private ReorderableList controlPointList;

    private SerializedProperty periodProp;
    private CatmullRomSpline spline;

    private Transform handleTransform;
    private Quaternion handleRotation;

    private int selectedControlPointIndex = -1;

	private static class Styles
	{
		public static Color endPointElementBackground = new Color(0.3f,0.3f,0.7f);
		public static Color elementBackground = Color.gray;
		public static Color selectedElementBackground = new Color(0.3f, 0.3f, 1);

		public static Color controlPointColor = Color.blue;
		public static Color splineColor = Color.green;

		public static Color guiColor = GUI.color;
	}

    void OnEnable()
    {
        spline = target as CatmullRomSpline;
        controlPointList = new ReorderableList(serializedObject, serializedObject.FindProperty("controlPoints"), true, true, true, true);
        periodProp = serializedObject.FindProperty("period");

        handleTransform = spline.transform;
        handleRotation = ( Tools.pivotRotation == PivotRotation.Local ) ? handleTransform.rotation : Quaternion.identity;


		controlPointList.elementHeight *= 2;
        controlPointList.drawHeaderCallback += HeaderDraw;
        controlPointList.drawElementCallback += ElementDraw;
        controlPointList.drawFooterCallback += FooterDraw;
		controlPointList.drawElementBackgroundCallback += ElementBackgroundDraw;
        
    }

    void OnDisable()
    {
		controlPointList.elementHeight /= 2;
		controlPointList.drawHeaderCallback -= HeaderDraw;
        controlPointList.drawElementCallback -= ElementDraw;
        controlPointList.drawFooterCallback -= FooterDraw;
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
        int value = EditorGUI.DelayedIntField(new Rect(hRect.x+hRect.width-45F,hRect.y,45F, hRect.height-2), controlPointList.count);
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
		eRect.height *= 0.5f;
        eRect.height -= 2;
		
        if (focused && active)
        {
            selectedControlPointIndex = i;
            SceneView.RepaintAll();
        }

        if(GUI.Button(new Rect(eRect.x + 1, eRect.y, 38, eRect.height), new GUIContent("SET", "Use the current Scene-view camera position")))
        {
            controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = spline.transform.InverseTransformPoint(SceneView.lastActiveSceneView.camera.transform.position);
        }
        //eRect.xMin += 40;
       
        //eRect.xMin -= 40;
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

	void ElementBackgroundDraw(Rect ebRect, int i, bool active, bool focused)
	{
		ebRect.y += 1;
		ebRect.height -= 2;
		ebRect.x += 1;
		ebRect.width -= 2;

		GUI.color = (i == 0 || i == controlPointList.count - 1) ? Styles.endPointElementBackground : Styles.elementBackground;
		if(active)
		{
			GUI.color = Styles.selectedElementBackground;
		}
		GUI.Box(ebRect, GUIContent.none);
		GUI.color = Styles.guiColor;
	}

    void FooterDraw(Rect fRect)
    {
		
        ReorderableList.defaultBehaviours.DrawFooter(fRect, controlPointList);
		fRect.x += EditorGUI.indentLevel+1;
		fRect.y -= 3;
        fRect.height += 2;
        EditorGUI.BeginChangeCheck();
        int value = EditorGUI.DelayedIntField(new Rect(fRect.x, fRect.y, 40F, fRect.height), controlPointList.count);
        if(EditorGUI.EndChangeCheck())
        {
            if(controlPointList.count > value)
            {
                while(controlPointList.count > value)
                {
                    controlPointList.serializedProperty.DeleteArrayElementAtIndex(controlPointList.count - 1);
                }
            }
            else if(controlPointList.count < value)
            {
                while(controlPointList.count < value)
                {
                    controlPointList.serializedProperty.InsertArrayElementAtIndex(controlPointList.count);
                }
            }
        }
    }
    #endregion

    #region SceneView

    void OnSceneGUI()
	{
		DrawCurve();

		for (int i = 0; i < spline.controlPoints.Count; i++)
        {
            DrawControlPoint(i);
		}
    }

    private void DrawControlPoint(int index)
    {
        Vector3 worldSpaceOriginalPoint = spline.transform.TransformPoint(spline.controlPoints[index]);
        if(selectedControlPointIndex == index)
        {
            //Do position handle
            EditorGUI.BeginChangeCheck();
            Vector3 worldSpacePoint = Handles.DoPositionHandle(worldSpaceOriginalPoint, handleRotation);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move Catmull-Rom control point");
                controlPointList.index = -1;
                EditorUtility.SetDirty(spline);
                spline.controlPoints[index] = spline.transform.InverseTransformPoint(worldSpacePoint);
            }
        }
        else
        {
            //Do selection button
            Handles.color = Styles.controlPointColor;
            float hSize = HandleUtility.GetHandleSize(worldSpaceOriginalPoint) * handleSize;
            if(Handles.Button(worldSpaceOriginalPoint, handleRotation, hSize, hSize + 0.05f, Handles.DotCap))
            {
                selectedControlPointIndex = index;
            }
        }
    }

    void DrawCurve()
    {
		CatmullRomSpline spline = target as CatmullRomSpline;
        Handles.color = Styles.splineColor;
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