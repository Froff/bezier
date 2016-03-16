using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class SplineCurve : MonoBehaviour {
	public List<Vector3> controlPoints;
	
	public Vector3 GetPosition (float t) {
		return transform.TransformPoint(GetLocalPosition(t));
	}

	protected abstract Vector3 GetLocalPosition(float t);
}