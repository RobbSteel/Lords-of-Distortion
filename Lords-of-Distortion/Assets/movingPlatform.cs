using UnityEngine;
using System.Collections;

public class movingPlatform : MonoBehaviour {

    public Transform pointB;
    public float speed;
    public bool wait;

    IEnumerator Start()
    {
        var pointA = transform.position;
        while (true)
        {
            yield return StartCoroutine(MoveObject(transform, pointA, pointB.position, 3.0f));
            yield return StartCoroutine(MoveObject(transform, pointB.position, pointA, 3.0f));
            if (wait)
                yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        var i = 0.0f;
        var rate = speed / time;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
    }

}
