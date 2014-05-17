using UnityEngine;
using System.Collections;

public class movingPlatform : MonoBehaviour {

    public Transform pointB;
    public bool pauseAfterFirstMove;
    public float firstMovePauseDelay;
    public float speed;
    public bool pauseAfterCycle;
    public float startAfter;
    public float cyclePauseDelay;
    
    //TODO : Start at match begin time instead of when level loads
    IEnumerator Start()
    {
        Vector3 pointA = transform.position;
        if (pauseAfterCycle)
            yield return new WaitForSeconds(startAfter);

        while (true)
        {
            yield return StartCoroutine(MoveObject(transform, pointA, pointB.position, 3.0f));
            if (pauseAfterFirstMove)
                yield return new WaitForSeconds(firstMovePauseDelay);
            yield return StartCoroutine(MoveObject(transform, pointB.position, pointA, 3.0f));
            if (pauseAfterCycle)
                yield return new WaitForSeconds(cyclePauseDelay);
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
