using UnityEngine;
using System.Collections;

public class SplineCurveFollower : MonoBehaviour
{
	public SplineCurve spline;

	void Start() {
		StartCoroutine(StartMovement());
	}

	IEnumerator StartMovement() {
		float t = 0.0f;
		while (true) {
			transform.position = spline.GetPosition(t);
			t += Time.deltaTime;
			yield return null;
		}
	}
}
