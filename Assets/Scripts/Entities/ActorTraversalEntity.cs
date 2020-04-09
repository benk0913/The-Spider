using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorTraversalEntity : MonoBehaviour
{
    public float Speed = 1f;

    [SerializeField]
    Transform fromPoint;

    public void Traverse()
    {
        if(TraverseRoutineInstance != null)
        {
            StopCoroutine(TraverseRoutineInstance);
        }

        TraverseRoutineInstance = StartCoroutine(TraverseRoutine());
    }

    Coroutine TraverseRoutineInstance;

    IEnumerator TraverseRoutine()
    {
        MouseLook.Instance.GetComponent<Rigidbody>().isKinematic = true;
        float t;
        Vector3 startPos;

        if (fromPoint != null)
        {
            startPos = MouseLook.Instance.transform.position;

            t = 0f;
            while (t < 1f)
            {

                t += Speed*4 * Time.deltaTime;

                MouseLook.Instance.transform.position = Vector3.Lerp(startPos, fromPoint.position, t);

                yield return 0;
            }

            startPos = fromPoint.position;
        }
        else
        {
            startPos = MouseLook.Instance.transform.position;
        }

        t = 0f;
        while(t<1f)
        {

            t += Speed * Time.deltaTime;

            MouseLook.Instance.transform.position = Vector3.Lerp(startPos , transform.position, t);

            yield return 0;
        }

        MouseLook.Instance.GetComponent<Rigidbody>().isKinematic = false;
        TraverseRoutineInstance = null;
    }
}
