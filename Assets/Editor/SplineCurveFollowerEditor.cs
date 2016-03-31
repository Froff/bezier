using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplineCurveFollower))]
class SplineCurveFollowerEditor : Editor
{
	SplineCurveFollower follower;
	void OnEnable()
	{
		follower = target as SplineCurveFollower;
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (GUILayout.Button(new GUIContent("Place On Spline", "Move the follower to the first control node of the spline")))
		{
			follower.transform.position = follower.spline.GetPosition(0);
			EditorGUIUtility.PingObject(follower.spline);
			SceneView.lastActiveSceneView.LookAt(follower.transform.position);
			Selection.activeObject = follower.spline;
        }
	}
}
