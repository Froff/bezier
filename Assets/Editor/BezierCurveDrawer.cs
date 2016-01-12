using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(BezierCurve))]
public class BezierCurveDrawer : PropertyDrawer
{
    const float LABEL_WIDTH = 40;
    const float PAD = 45;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3f;
    }

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty a = prop.FindPropertyRelative("a");
        SerializedProperty b = prop.FindPropertyRelative("b");
        SerializedProperty c = prop.FindPropertyRelative("c");

        float h = pos.height / 3;

        GUI.Label(new Rect(pos.x, pos.y + h, LABEL_WIDTH, h), "Points");

        Color guiColor = GUI.color;
        GUI.color = Color.green;
        // Draw scale
        GUI.Box(new Rect(pos.x + LABEL_WIDTH, pos.y, pos.width-PAD, h*2), "");
        GUI.color = guiColor;

        a.vector3Value = EditorGUI.Vector3Field(
            new Rect(pos.x + LABEL_WIDTH, pos.y, pos.width-PAD, h), "", a.vector3Value);
        b.vector3Value = EditorGUI.Vector3Field(
            new Rect(pos.x + LABEL_WIDTH, pos.y + h, pos.width-PAD, h), "", b.vector3Value);
        
        GUI.color = Color.yellow;
        // Draw scale
        GUI.Box(new Rect(pos.x + LABEL_WIDTH, pos.y + h*2, pos.width-PAD, h), "");
        GUI.color = guiColor;

        c.vector3Value = EditorGUI.Vector3Field(
            new Rect(pos.x + LABEL_WIDTH, pos.y + h*2, pos.width - PAD, h), "", c.vector3Value);

        // Draw curve
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        
        EditorGUI.indentLevel = indent;
    }
}