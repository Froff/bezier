using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class SplineCurve : MonoBehaviour {
	public List<Vector3> controlPoints = new List<Vector3>();
	
	void OnEnable()
	{
		while(controlPoints.Count < 4)
		{
			controlPoints.Add(Vector3.zero);
		}
	}

	public Vector3 GetPosition (float t) {
		return transform.TransformPoint(GetLocalPosition(t));
	}

	protected abstract Vector3 GetLocalPosition(float t);
}