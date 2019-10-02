using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPositionLerperUI : MonoBehaviour
{

    public Vector3 CurrentPos;
    public Transform CurrentTransform;

    [SerializeField]
    bool StickToEdgeOfScreen = false;

    public void SetPosition(Vector3 pos)
    {
        CurrentTransform = null;
        CurrentPos = pos;
    }


    public void SetTransform(Transform targetTransform)
    {
        CurrentTransform = targetTransform;
        CurrentPos = targetTransform.position;
    }

    void Update()
    {
        if(CurrentTransform != null)
        {
            transform.position = MapViewManager.Instance.Cam.WorldToScreenPoint(CurrentTransform.position);
            return;
        }

        Vector3 newPos = MapViewManager.Instance.Cam.WorldToScreenPoint(CurrentPos);

        if (StickToEdgeOfScreen)
        {
            newPos = new Vector3(newPos.x < 0 ? 0f : newPos.x, newPos.y < 0 ? 0f : newPos.y, newPos.z);
            newPos = new Vector3(newPos.x > Screen.width? Screen.width : newPos.x, newPos.y > Screen.height ? Screen.height : newPos.y, newPos.z);
        }

        transform.position = newPos;
    }
}
