using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SplineCurve {
	public SplineCurve (List<Vector3> controlPoints) {
		this.controlPoints = controlPoints;
	}

	public List<Vector3> controlPoints;
}