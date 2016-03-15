using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline : MonoBehaviour {
	public SplineCurve curve;
	public float period;

	void Start () {
		StartCoroutine(StartMovement());
	}

	IEnumerator StartMovement() {
		float timeElapsed = 0, normalTimeElapsed = 0;
		int segmentCount = curve.controlPoints.Count - 3;
		while (normalTimeElapsed <= 1) {
			int currentSegment = Mathf.FloorToInt(normalTimeElapsed * segmentCount);
			float t = (normalTimeElapsed * segmentCount) % 1;

			transform.position = GetPosition(t, currentSegment);

			timeElapsed += Time.deltaTime;
			normalTimeElapsed = timeElapsed / period;
			yield return null;
		}
		StartCoroutine(StartMovement());
	}

	public Vector3 GetPosition(float t) {
		float normalTimeElapsed = t / period;
		int segmentCount = curve.controlPoints.Count - 3;
		int currentSegment = Mathf.FloorToInt(normalTimeElapsed * segmentCount);
		float localT = (normalTimeElapsed * segmentCount) % 1;
		return GetPosition(localT, currentSegment);
	}

	Vector3 GetPosition (float t, int segment) {
		return GetPosition(t, curve.controlPoints.GetRange(segment, 4));
	}

	public static Vector3 GetPosition(float t, int segment, SplineCurve curve) {
		return GetPosition(t, curve.controlPoints.GetRange(segment, 4));
	}

	public static Vector3 GetPosition(float t, SplineCurve curve) {
		int segmentCount = curve.controlPoints.Count - 3;
		int currentSegment = Mathf.FloorToInt(t * segmentCount);
		float localT = (t * segmentCount) % 1;
		return GetPosition(t, currentSegment, curve);
	}

	public static Vector3 GetPosition(float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
		return GetPosition(t, new List<Vector3>() { a, b, c, d });
	}

	public static Vector3 GetPosition(float t, List<Vector3> P) {
		return 0.5f * (
			(2 * P[1]) +
			(P[2] - P[0]) * t +
			(2 * P[0] - 5 * P[1] + 4 * P[2] - P[3]) * t * t +
			(3 * P[1] - P[0] - 3 * P[2] + P[3]) * t * t * t);
	}
}
