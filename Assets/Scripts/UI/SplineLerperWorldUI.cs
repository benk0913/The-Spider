using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineLerperWorldUI : MonoBehaviour
{
    System.Action CurrentActionInstance;
    Coroutine MovementRoutineInstance;

    [SerializeField]
    float DelayTime = 0f;

    [SerializeField]
    float minSpeed = 0.6f;

    [SerializeField]
    float maxSpeed = 1f;

    public void SetInfo(Transform startPoint, Transform targetPoint, System.Action onComplete = null)
    {
        if(MovementRoutineInstance != null)
        {
            CurrentActionInstance?.Invoke();
            StopCoroutine(MovementRoutineInstance);
        }

        CurrentActionInstance = onComplete;
        MovementRoutineInstance = StartCoroutine(MovementRoutine(startPoint, targetPoint));
    }

    IEnumerator MovementRoutine(Transform startPoint, Transform targetPoint)
    {
        Vector3 startPos;
        Vector3 endPos;
        bool isCanvasElement = (transform.GetComponentInParent<Canvas>() != null);

        if (isCanvasElement)
        {
            if (startPoint.transform.GetComponentInParent<Canvas>() == null)
            {
                startPos = MapViewManager.Instance.Cam.WorldToScreenPoint(startPoint.transform.position);
            }
            else
            {
                startPos = startPoint.position;
            }

            if (targetPoint.transform.GetComponentInParent<Canvas>() == null)
            {
                endPos = MapViewManager.Instance.Cam.WorldToScreenPoint(targetPoint.transform.position);
            }
            else
            {
                endPos = targetPoint.position;
            }
        }
        else
        {
            if (startPoint.transform.GetComponentInParent<Canvas>() == null)
            {
                startPos = startPoint.position;
            }
            else
            {
                RaycastHit rhit;
                if (Physics.Raycast(MapViewManager.Instance.Cam.ScreenPointToRay(startPoint.transform.position), out rhit))
                {
                    startPos = rhit.point;
                }
                else
                {
                    startPos = MapViewManager.Instance.Cam.ScreenToWorldPoint(startPoint.position);
                }
            }

            if (targetPoint.transform.GetComponentInParent<Canvas>() == null)
            {
                endPos = targetPoint.position;
            }
            else
            {
                RaycastHit rhit;
                if(Physics.Raycast(MapViewManager.Instance.Cam.ScreenPointToRay(targetPoint.transform.position), out rhit))
                {
                    endPos = rhit.point;
                }
                else
                {
                    endPos = MapViewManager.Instance.Cam.ScreenToWorldPoint(targetPoint.position);
                }

            }
        }

        float randomHeight = isCanvasElement ? Random.Range(-200f, 200f) : Random.Range(-0.5f, 0.5f);
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        
        if (DelayTime > 0f)
        {
            startPos += (Vector3.one / 100f) * 2f;
            transform.position = startPos;
            yield return new WaitForSeconds(DelayTime);
        }

        float t = 0f;
        while(t<0.9f)
        {
            t += randomSpeed * Time.deltaTime;

            transform.position = Util.SplineLerpX(startPos, endPos, randomHeight, Mathf.SmoothStep(0f,1f, t));

            yield return 0;
        }

        CurrentActionInstance?.Invoke();

        MovementRoutineInstance = null;

        this.gameObject.SetActive(false);
    }
}
