using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(BezierCurve))]
public class BezierCurveDrawer : PropertyDrawer
{
    public const float LABEL_WIDTH = 40;
    public const float PAD = 45;

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
        
        Color guiColor = GUI.color;
        GUI.color = Color.green * 0.75f;
        // Draw scale
        GUI.Box(new Rect(pos.x + LABEL_WIDTH, pos.y, pos.width-PAD, h*2), "");
        GUI.color = Color.green;

        GUI.Label(new Rect(pos.x, pos.y + h, LABEL_WIDTH, h), "Points");

        a.vector3Value = EditorGUI.Vector3Field(
            new Rect(pos.x + LABEL_WIDTH, pos.y, pos.width-PAD, h), "", a.vector3Value);

        b.vector3Value = EditorGUI.Vector3Field(
            new Rect(pos.x + LABEL_WIDTH, pos.y + h, pos.width-PAD, h), "", b.vector3Value);
        
        GUI.color = Color.yellow * 0.75f;
        // Draw scale
        GUI.Box(new Rect(pos.x + LABEL_WIDTH, pos.y + h*2, pos.width-PAD, h), "");

        GUI.color = Color.yellow;

        c.vector3Value = EditorGUI.Vector3Field(
            new Rect(pos.x + LABEL_WIDTH, pos.y + h*2, pos.width - PAD, h), "", c.vector3Value);

        GUI.color = guiColor;
    }
}