using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierMove : MonoBehaviour
{
    public BezierSpline s;
    public float t = 1f;
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(Move());
	}

    IEnumerator Move()
    {
		List<Vector3> Positions = new List<Vector3> ();
        float elapsedTime = 0.0f;
        while (elapsedTime < t)
        {
            transform.position = s.Position(elapsedTime / t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = s.Position(0);
        elapsedTime = 0.0f;
        StartCoroutine(Move());
    }
}
