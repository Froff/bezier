using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class CameraSpline : CatmullRomSpline
{
	public List<CameraStateOnCurve> _cameraStates = new List<CameraStateOnCurve>();
	public List<CameraStateOnCurve> cameraStates
	{
		get
		{
			_cameraStates.Sort();
			return _cameraStates;
		}
		set
		{
			_cameraStates = value;
		}
	}

	public CameraState GetCameraState(float t)
	{
		Vector3 position = GetPosition(t);
		Quaternion rotation = GetRotation(t);
		return new CameraState(position, rotation);
	}

	Quaternion GetRotation(float t)
	{
		int currentSegment = GetRotationSegment(t);
		CameraStateOnCurve a, b;
		a = cameraStates[currentSegment];
		if (currentSegment == cameraStates.Count - 1 || currentSegment == 0)
		{
			return a.rotation;
		}
		b = cameraStates[currentSegment + 1];
        float segmentTime = t - a.t;
		float normalizedSegmentTime = segmentTime / (b.t - a.t);
		return Quaternion.Slerp(a.rotation, b.rotation, normalizedSegmentTime);
	}

	int GetRotationSegment(float t)
	{
		//Binary search for correct segment
		int ai = 0, bi = cameraStates.Count;
		while (bi - ai < 1) {
			int mid = (ai + bi) / 2;
			if (cameraStates[mid].t > t)
			{
				bi = mid;
				continue;
			}
			else if (cameraStates[mid].t < t)
			{
				ai = mid;
				continue;
			}
			else
			{
				ai = mid;
				break;
			}
		}
		return ai;
	}
}

[System.Serializable]
public struct CameraStateOnCurve
{
	public float t;
	public Quaternion rotation;

	public CameraStateOnCurve (float t, Quaternion rotation)
	{
		this.t = t;
		this.rotation = rotation;
	}
}

public class CameraStateOnCurveComp : Comparer<CameraStateOnCurve>
{
	public override int Compare (CameraStateOnCurve a, CameraStateOnCurve b)
	{
		return a.t.CompareTo(b.t);
	}
}

public struct CameraState
{
	public Vector3 position;
	public Quaternion rotation;

	public CameraState (Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}
}
