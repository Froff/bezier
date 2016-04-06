using UnityEngine;
using System.Collections;

public class SplineCurveRotationFollower : SplineCurveFollower
{
	public float cameraDelay = 0.001f;
	internal override IEnumerator StartMovement()
	{
		cameraDelay = Mathf.Max(cameraDelay, 0.001f);
		float t = 0.0f;
		while (true) {
			transform.position = spline.GetPosition(t);
			transform.rotation = Quaternion.LookRotation((spline.GetPosition(t + cameraDelay) - transform.position).normalized);
			t += Time.deltaTime;
			Debug.DrawLine(transform.position,spline.GetPosition(t+cameraDelay), Color.red);
			yield return null;
		}
	}
}
