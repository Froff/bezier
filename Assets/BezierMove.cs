using UnityEngine;
using System.Collections;

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
