using UnityEngine;
using System.Collections;

public class GravityFieldMover : MonoBehaviour
{
    

    public float speed;
    public bool wait;
    public float startAt;
    public float pauseFor;

    IEnumerator Start()
    {
        GameObject pointA = new GameObject("GravPointA");
        GameObject pointB = new GameObject("GravPointB");
        pointA.transform.position = new Vector3(transform.position.x + 3, transform.position.y, transform.position.z);
        pointB.transform.position = new Vector3(transform.position.x - 3, transform.position.y, transform.position.z);
        
        if (wait)
        {
            yield return new WaitForSeconds(startAt);
        }
        while (true)
        {
            yield return StartCoroutine(MoveObject(transform, pointA.transform.position, pointB.transform.position, 3.0f));
            yield return StartCoroutine(MoveObject(transform, pointB.transform.position, pointA.transform.position, 3.0f));
            if (wait)
                yield return new WaitForSeconds(pauseFor);
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
