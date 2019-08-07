using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPositionLerperUI : MonoBehaviour
{

    public Vector3 CurrentPos;
    public Transform CurrentTransform;

    public void SetPosition(Vector3 pos)
    {
        CurrentTransform = null;
        CurrentPos = pos;
    }


    public void SetTransform(Transform targetTransform)
    {
        CurrentTransform = targetTransform;
    }

    void Update()
    {
        if(CurrentTransform != null)
        {
            transform.position = MapViewManager.Instance.Cam.WorldToScreenPoint(CurrentTransform.position);
            return;
        }

        transform.position = MapViewManager.Instance.Cam.WorldToScreenPoint(CurrentPos);

        
    }
}
