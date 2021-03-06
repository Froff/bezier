﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(CameraSpline)), CanEditMultipleObjects]
public class CameraSplineEditor : Editor
{
	private const float handleSize = 0.10f;

	private ReorderableList
		controlPointList,
		cameraStateList;

	private SerializedProperty
		periodProp,
		loopingProp;

	private CatmullRomSpline spline;

	private Transform handleTransform;
	private Quaternion handleRotation;

	private int selectedControlPointIndex = -1;
	private Vector3? clipboardPoint = null;
	private static class Styles
	{
		public static Color endPointElementBackground = new Color(0.3f, 0.3f, 0.7f);
		public static Color elementBackground = Color.gray;
		public static Color selectedElementBackground = new Color(0.3f, 0.3f, 1);

		public static Color controlPointColor = Color.blue;
		public static Color splineColor = Color.green;

		public static Color guiColor = GUI.color;
	}

	void OnEnable()
	{
		spline = target as CameraSpline;
		controlPointList = new ReorderableList(serializedObject, serializedObject.FindProperty("controlPoints"), true, true, true, true);
		if (cameraStateList == null)
		{
			Debug.Log("the list of camerastates is null which makes sense");
		}
		cameraStateList = new ReorderableList(serializedObject, serializedObject.FindProperty("_cameraStates"), true, true, true, true);
		if (cameraStateList == null)
		{
			Debug.Log("the list of camerastates is null wtf man?");
		}
		periodProp = serializedObject.FindProperty("period");
		loopingProp = serializedObject.FindProperty("looping");


		handleTransform = spline.transform;
		handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;


		controlPointList.elementHeight *= 2;
		controlPointList.drawHeaderCallback += ControlPointHeaderDraw;
		controlPointList.drawElementCallback += ControlPointElementDraw;
		controlPointList.drawFooterCallback += ControlPointFooterDraw;
		controlPointList.drawElementBackgroundCallback += ControlPointElementBackgroundDraw;

		cameraStateList.elementHeight *= 2;
		cameraStateList.drawHeaderCallback += CameraStateHeaderDraw;
		cameraStateList.drawElementCallback += CameraStateElementDraw;
		cameraStateList.drawFooterCallback += CameraStateFooterDraw;
		cameraStateList.drawElementBackgroundCallback += CameraStateElementBackgroundDraw;

	}

	void OnDisable()
	{
		controlPointList.elementHeight /= 2;
		controlPointList.drawHeaderCallback -= ControlPointHeaderDraw;
		controlPointList.drawElementCallback -= ControlPointElementDraw;
		controlPointList.drawFooterCallback -= ControlPointFooterDraw;

		cameraStateList.elementHeight /= 2;
		cameraStateList.drawHeaderCallback -= CameraStateHeaderDraw;
		cameraStateList.drawElementCallback -= CameraStateElementDraw;
		cameraStateList.drawFooterCallback -= CameraStateFooterDraw;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		controlPointList.DoLayoutList();
		EditorGUILayout.Space();
		cameraStateList.DoLayoutList();
		EditorGUILayout.Space();
		periodProp.floatValue = Mathf.Max(EditorGUILayout.DelayedFloatField("Period", periodProp.floatValue), 0.001f);
		serializedObject.ApplyModifiedProperties();
	}

	#region listCallbacks

	void ControlPointHeaderDraw(Rect hRect)
	{
		hRect.y += 1f;
		hRect.height -= 2;
		GUI.Label(new Rect(hRect.x, hRect.y, 100F, hRect.height), "Control Points");

		EditorGUI.BeginChangeCheck();
		bool loop = EditorGUI.Toggle(new Rect(hRect.x + hRect.width - 125F, hRect.y, 70F, hRect.height), loopingProp.boolValue, EditorStyles.miniButton);
		if (EditorGUI.EndChangeCheck())
		{
			loopingProp.boolValue = loop;
		}
		GUI.Label(new Rect(hRect.x + hRect.width - 112F, hRect.y - 2F, 70F, hRect.height + 2F), new GUIContent("Looping", "Should the spline loop?"), EditorStyles.miniBoldLabel);
		EditorGUI.BeginChangeCheck();
		int value = EditorGUI.DelayedIntField(new Rect(hRect.x + hRect.width - 45F, hRect.y, 45F, hRect.height), controlPointList.count);
		if (EditorGUI.EndChangeCheck())
		{
			if (controlPointList.count > value)
			{
				while (controlPointList.count > value)
				{
					controlPointList.serializedProperty.DeleteArrayElementAtIndex(controlPointList.count - 1);
				}
			}
			else if (controlPointList.count < value)
			{
				while (controlPointList.count < value)
				{
					controlPointList.serializedProperty.InsertArrayElementAtIndex(controlPointList.count);
				}
			}
		}
	}

	void ControlPointElementDraw(Rect eRect, int i, bool active, bool focused)
	{
		if (spline.looping && (i == 0 || i == controlPointList.count - 1))
		{
			GUI.enabled = false;
		}
		eRect.y += 1;
		eRect.height *= 0.5f;
		eRect.height -= 2;


		if (GUI.Button(new Rect(eRect.x + 1, eRect.y, 38, eRect.height), new GUIContent("SET", "Use the current Scene-view camera position")))
		{
			controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = spline.transform.InverseTransformPoint(SceneView.lastActiveSceneView.camera.transform.position);
		}
		eRect.xMax -= 2;
		eRect.y += 2;
		eRect.height -= 1;
		EditorGUI.BeginChangeCheck();
		Vector3 value = EditorGUI.Vector3Field(new Rect(eRect.x + 40, eRect.y, eRect.width - 40, eRect.height - 2), GUIContent.none, controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value);
		if (EditorGUI.EndChangeCheck())
		{
			controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = value;
		}
		eRect.y += eRect.height;

		if (GUI.Button(new Rect(eRect.x + 1, eRect.y, eRect.width * 0.5f, eRect.height), new GUIContent("Copy", "Copy position"), EditorStyles.miniButtonLeft))
		{
			clipboardPoint = controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value;
		}
		if (clipboardPoint == null)
		{
			GUI.enabled = false;
		}
		if (GUI.Button(new Rect(eRect.x + 1 + eRect.width * 0.5f, eRect.y, eRect.width * 0.5f, eRect.height), new GUIContent("Paste", ((clipboardPoint != null) ? "Paste " + clipboardPoint.ToString() : "Clipboard empty")), EditorStyles.miniButtonRight))
		{
			GUI.FocusControl("");
			if (clipboardPoint != null)
				controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = clipboardPoint.GetValueOrDefault();
		}
		GUI.enabled = true;
	}

	void ControlPointElementBackgroundDraw(Rect ebRect, int i, bool active, bool focused)
	{
		if (spline.looping && (i == 0 || i == controlPointList.count - 1))
		{
			GUI.enabled = false;
		}

		if (focused && active)
		{
			selectedControlPointIndex = i;
			SceneView.RepaintAll();
		}
		Event current = Event.current;
		if (current.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseDown)
		{
			if (current.clickCount == 2)
			{
				if (ebRect.Contains(current.mousePosition))
				{
					SceneView.lastActiveSceneView.LookAt(spline.transform.TransformPoint(controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value));
				}
			}
		}
		ebRect.y += 1;
		ebRect.height -= 2;
		ebRect.x += 1;
		ebRect.width -= 2;

		if (active)
		{
			GUI.color = Styles.selectedElementBackground;
		}
		else if (focused)
		{
			GUI.color = 0.5f * ((i == 0 || i == controlPointList.count - 1) ? Styles.endPointElementBackground : Styles.elementBackground);
		}
		else
		{
			GUI.color = (i == 0 || i == controlPointList.count - 1) ? Styles.endPointElementBackground : Styles.elementBackground;
		}
		GUI.Box(ebRect, GUIContent.none);
		GUI.color = Styles.guiColor;
		GUI.enabled = true;
	}

	void ControlPointFooterDraw(Rect fRect)
	{
		fRect.x += EditorGUI.indentLevel + 1;
		fRect.y -= 3;
		EditorGUI.BeginChangeCheck();
		int value = EditorGUI.DelayedIntField(new Rect(fRect.x + 2, fRect.y, 38F, fRect.height), controlPointList.count, EditorStyles.toolbarPopup);
		if (EditorGUI.EndChangeCheck())
		{
			if (controlPointList.count > value)
			{
				while (controlPointList.count > value)
				{
					controlPointList.serializedProperty.DeleteArrayElementAtIndex(controlPointList.count - 1);
				}
			}
			else if (controlPointList.count < value)
			{
				while (controlPointList.count < value)
				{
					controlPointList.serializedProperty.InsertArrayElementAtIndex(controlPointList.count);
				}
			}
		}

		if (clipboardPoint == null)
		{
			GUI.enabled = false;
		}
		if (GUI.Button(new Rect(fRect.x + 40F, fRect.y, fRect.width - 100F, fRect.height), new GUIContent("Paste as new", ((clipboardPoint != null) ? "Paste " + clipboardPoint.ToString() : "Clipboard empty")), EditorStyles.toolbarButton))
		{
			if (clipboardPoint != null)
			{
				int i = controlPointList.count;
				controlPointList.serializedProperty.InsertArrayElementAtIndex(i);
				controlPointList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = clipboardPoint.GetValueOrDefault();
			}
		}
		GUI.enabled = true;
		if (GUI.Button(new Rect(fRect.x + fRect.width - 64F, fRect.y, 30F, fRect.height), EditorGUIUtility.IconContent("Toolbar Plus", "Add to list"), EditorStyles.toolbarButton))
		{
			ReorderableList.defaultBehaviours.DoAddButton(controlPointList);
		}
		if (GUI.Button(new Rect(fRect.x + fRect.width - 34F, fRect.y, 30F, fRect.height), EditorGUIUtility.IconContent("Toolbar Minus", "Remove element from list"), EditorStyles.toolbarButton))
		{
			ReorderableList.defaultBehaviours.DoRemoveButton(controlPointList);
		}
	}

	void CameraStateHeaderDraw(Rect hRect)
	{
		hRect.y += 1f;
		hRect.height -= 2;
		GUI.Label(new Rect(hRect.x, hRect.y, 100F, hRect.height), "Control Points");

		EditorGUI.BeginChangeCheck();
		bool loop = EditorGUI.Toggle(new Rect(hRect.x + hRect.width - 125F, hRect.y, 70F, hRect.height), loopingProp.boolValue, EditorStyles.miniButton);
		if (EditorGUI.EndChangeCheck())
		{
			loopingProp.boolValue = loop;
		}
		GUI.Label(new Rect(hRect.x + hRect.width - 112F, hRect.y - 2F, 70F, hRect.height + 2F), new GUIContent("Looping", "Should the spline loop?"), EditorStyles.miniBoldLabel);
		EditorGUI.BeginChangeCheck();
		int value = EditorGUI.DelayedIntField(new Rect(hRect.x + hRect.width - 45F, hRect.y, 45F, hRect.height), cameraStateList.count);
		if (EditorGUI.EndChangeCheck())
		{
			if (cameraStateList.count > value)
			{
				while (cameraStateList.count > value)
				{
					cameraStateList.serializedProperty.DeleteArrayElementAtIndex(cameraStateList.count - 1);
				}
			}
			else if (cameraStateList.count < value)
			{
				while (cameraStateList.count < value)
				{
					cameraStateList.serializedProperty.InsertArrayElementAtIndex(cameraStateList.count);
				}
			}
		}
	}

	void CameraStateElementDraw(Rect eRect, int i, bool active, bool focused)
	{
		if (spline.looping && (i == 0 || i == cameraStateList.count - 1))
		{
			GUI.enabled = false;
		}
		eRect.y += 1;
		eRect.height *= 0.5f;
		eRect.height -= 2;


		if (GUI.Button(new Rect(eRect.x + 1, eRect.y, 38, eRect.height), new GUIContent("SET", "Use the current Scene-view camera position")))
		{
			cameraStateList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = spline.transform.InverseTransformPoint(SceneView.lastActiveSceneView.camera.transform.position);
		}
		eRect.xMax -= 2;
		eRect.y += 2;
		eRect.height -= 1;
		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(new Rect(eRect.x + 40, eRect.y, eRect.width - 40, eRect.height - 2), cameraStateList.serializedProperty.GetArrayElementAtIndex(i));
		if (EditorGUI.EndChangeCheck())
		{

		}
		eRect.y += eRect.height;

		if (GUI.Button(new Rect(eRect.x + 1, eRect.y, eRect.width * 0.5f, eRect.height), new GUIContent("Copy", "Copy position"), EditorStyles.miniButtonLeft))
		{
			clipboardPoint = cameraStateList.serializedProperty.GetArrayElementAtIndex(i).vector3Value;
		}
		if (clipboardPoint == null)
		{
			GUI.enabled = false;
		}
		if (GUI.Button(new Rect(eRect.x + 1 + eRect.width * 0.5f, eRect.y, eRect.width * 0.5f, eRect.height), new GUIContent("Paste", ((clipboardPoint != null) ? "Paste " + clipboardPoint.ToString() : "Clipboard empty")), EditorStyles.miniButtonRight))
		{
			GUI.FocusControl("");
			if (clipboardPoint != null)
				cameraStateList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = clipboardPoint.GetValueOrDefault();
		}
		GUI.enabled = true;
	}

	void CameraStateElementBackgroundDraw(Rect ebRect, int i, bool active, bool focused)
	{
		if (spline.looping && (i == 0 || i == cameraStateList.count - 1))
		{
			GUI.enabled = false;
		}

		if (focused && active)
		{
			selectedControlPointIndex = i;
			SceneView.RepaintAll();
		}
		Event current = Event.current;
		if (current.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseDown)
		{
			if (current.clickCount == 2)
			{
				if (ebRect.Contains(current.mousePosition))
				{
					SceneView.lastActiveSceneView.LookAt(spline.transform.TransformPoint(cameraStateList.serializedProperty.GetArrayElementAtIndex(i).vector3Value));
				}
			}
		}
		ebRect.y += 1;
		ebRect.height -= 2;
		ebRect.x += 1;
		ebRect.width -= 2;

		if (active)
		{
			GUI.color = Styles.selectedElementBackground;
		}
		else if (focused)
		{
			GUI.color = 0.5f * ((i == 0 || i == cameraStateList.count - 1) ? Styles.endPointElementBackground : Styles.elementBackground);
		}
		else
		{
			GUI.color = (i == 0 || i == cameraStateList.count - 1) ? Styles.endPointElementBackground : Styles.elementBackground;
		}
		GUI.Box(ebRect, GUIContent.none);
		GUI.color = Styles.guiColor;
		GUI.enabled = true;
	}

	void CameraStateFooterDraw(Rect fRect)
	{
		fRect.x += EditorGUI.indentLevel + 1;
		fRect.y -= 3;
		EditorGUI.BeginChangeCheck();
		int value = EditorGUI.DelayedIntField(new Rect(fRect.x + 2, fRect.y, 38F, fRect.height), cameraStateList.count, EditorStyles.toolbarPopup);
		if (EditorGUI.EndChangeCheck())
		{
			if (cameraStateList.count > value)
			{
				while (cameraStateList.count > value)
				{
					cameraStateList.serializedProperty.DeleteArrayElementAtIndex(cameraStateList.count - 1);
				}
			}
			else if (cameraStateList.count < value)
			{
				while (cameraStateList.count < value)
				{
					cameraStateList.serializedProperty.InsertArrayElementAtIndex(cameraStateList.count);
				}
			}
		}

		if (clipboardPoint == null)
		{
			GUI.enabled = false;
		}
		if (GUI.Button(new Rect(fRect.x + 40F, fRect.y, fRect.width - 100F, fRect.height), new GUIContent("Paste as new", ((clipboardPoint != null) ? "Paste " + clipboardPoint.ToString() : "Clipboard empty")), EditorStyles.toolbarButton))
		{
			if (clipboardPoint != null)
			{
				int i = cameraStateList.count;
				cameraStateList.serializedProperty.InsertArrayElementAtIndex(i);
				cameraStateList.serializedProperty.GetArrayElementAtIndex(i).vector3Value = clipboardPoint.GetValueOrDefault();
			}
		}
		GUI.enabled = true;
		if (GUI.Button(new Rect(fRect.x + fRect.width - 64F, fRect.y, 30F, fRect.height), EditorGUIUtility.IconContent("Toolbar Plus", "Add to list"), EditorStyles.toolbarButton))
		{
			ReorderableList.defaultBehaviours.DoAddButton(cameraStateList);
		}
		if (GUI.Button(new Rect(fRect.x + fRect.width - 34F, fRect.y, 30F, fRect.height), EditorGUIUtility.IconContent("Toolbar Minus", "Remove element from list"), EditorStyles.toolbarButton))
		{
			ReorderableList.defaultBehaviours.DoRemoveButton(cameraStateList);
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
		if (spline.looping && (index == 0 || index == spline.controlPoints.Count - 1))
		{
			return;
		}
		Vector3 worldSpaceOriginalPoint = spline.transform.TransformPoint(spline.controlPoints[index]);
		float hSize = HandleUtility.GetHandleSize(worldSpaceOriginalPoint) * handleSize;
		Handles.color = Styles.controlPointColor;
		if (selectedControlPointIndex == index)
		{
			//Do position handle
			Handles.DotCap(100, worldSpaceOriginalPoint, Quaternion.identity, hSize);
			EditorGUI.BeginChangeCheck();
			Vector3 worldSpacePoint = Handles.DoPositionHandle(worldSpaceOriginalPoint, handleRotation);
			if (EditorGUI.EndChangeCheck())
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
			if (Handles.Button(worldSpaceOriginalPoint, handleRotation, hSize, hSize + 0.05f, Handles.DotCap))
			{
				selectedControlPointIndex = index;
			}
		}
	}

	void DrawCurve()
	{
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

[CustomPropertyDrawer(typeof(CameraStateOnCurve))]
public class CameraStateDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		Rect contentPosition = position;
		contentPosition.width = 100;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("t"), GUIContent.none);
		contentPosition.x += 120;
		contentPosition.width = position.width;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("rotation"));
	}
}