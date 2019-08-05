using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPositionLerperUI : MonoBehaviour
{

    public Vector3 CurrentPos;

    public void SetPosition(Vector3 pos)
    {
        CurrentPos = pos;
    }

    void Update()
    {
        transform.position = MapViewManager.Instance.Cam.WorldToScreenPoint(CurrentPos);
    }
}
