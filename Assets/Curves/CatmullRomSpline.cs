using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline : SplineCurve {
	public float period = 1f;

	/// <summary>
	/// Calculates and returns point t on this Catmull-Rom curve
	/// </summary>
	/// <param name="t">Time passed. Determines how far along the curve the returned point is</param>
	/// <returns></returns>
	protected override Vector3 GetLocalPosition (float t) {
		float normalTimeElapsed = (t / period)%1;
		int segmentCount = controlPoints.Count - 3;
		int currentSegment = Mathf.FloorToInt(normalTimeElapsed * segmentCount);
		float localT = (normalTimeElapsed * segmentCount) % 1;
		return GetLocalPosition(localT, currentSegment);
	}

	/// <summary>
	/// Calculates and returns point t on a specified segment of this Catmull-Rom curve
	/// </summary>
	/// <param name="t">Time between 0 and 1. Determines how far along the curve the returned point is</param>
	/// <param name="segment">The segment in which the desired point is located</param>
	/// <returns></returns>
	Vector3 GetLocalPosition (float t, int segment) {
		if (controlPoints.Count < 4) {
			return Vector3.zero;
		}
		return GetLocalPosition(t, controlPoints.GetRange(segment, 4));
	}

	/// <summary>
	/// Calculates and returns the point on a Catmull-Rom curve segment described by a t between 0 and 1, and four sequential control points
	/// </summary>
	/// <param name="t">A value between 0 and 1 describing how far along the curve the returned point should be</param>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="c"></param>
	/// <param name="d"></param>
	/// <returns></returns>
	public static Vector3 GetLocalPosition (float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
		return GetLocalPosition(t, new List<Vector3>() { a, b, c, d });
	}

	/// <summary>
	/// Calculates and returns the point on a Catmull-Rom curve segment described by a t between 0 and 1, and a list of four control points
	/// </summary>
	/// <param name="t">A value between 0 and 1 describing how far along the curve the returned point should be</param>
	/// <param name="P">A list of four points used to describe the curve</param>
	/// <returns></returns>
	public static Vector3 GetLocalPosition (float t, List<Vector3> P) {
		if (P.Count < 4) {
			return Vector3.zero;
		}
		return 0.5f * (
			(2 * P[1]) +
			(P[2] - P[0]) * t +
			(2 * P[0] - 5 * P[1] + 4 * P[2] - P[3]) * t * t +
			(3 * P[1] - P[0] - 3 * P[2] + P[3]) * t * t * t);
	}
}
