using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(BezierCurve))]
public class BezierCurveDrawer : PropertyDrawer
{
    const float min = 0;
    const float max = 1;
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty a = prop.FindPropertyRelative("a");
        SerializedProperty b = prop.FindPropertyRelative("b");
        SerializedProperty c = prop.FindPropertyRelative("c");

        GUI.color = Color.green;
        // Draw scale
        GUI.Box(new Rect(pos.x, pos.y, pos.width, pos.height+80), label);

        a.vector3Value = EditorGUI.Vector3Field(pos, "a", a.vector3Value);
        b.vector3Value = EditorGUI.Vector3Field(pos, "a", b.vector3Value);
        c.vector3Value = EditorGUI.Vector3Field(pos, "a", c.vector3Value);

        // Draw curve
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        
        EditorGUI.indentLevel = indent;
    }
}