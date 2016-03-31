using UnityEngine;
using System.Collections;

public class CameraSplineFollower : MonoBehaviour {
	public CameraSpline spline;

	void Start()
	{
		StartCoroutine(StartMovement());
	}

	IEnumerator StartMovement()
	{
		float t = 0.0f;
		while (true)
		{
			CameraState camState = spline.GetCameraState(t);
			transform.position = camState.position;
			transform.rotation = camState.rotation;
			t += Time.deltaTime;
			yield return null;
		}
	}
}
