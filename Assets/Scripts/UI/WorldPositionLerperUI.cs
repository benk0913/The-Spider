using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPositionLerperUI : MonoBehaviour
{

    public Vector3 CurrentPos;
    public Transform CurrentTransform;
    bool transformIsCanvas;

    [SerializeField]
    bool StickToEdgeOfScreen = false;

    public bool IsIgnoringMapBackfacing = false;
    public bool IsMapElement;


    public void SetPosition(Vector3 pos)
    {
        CurrentTransform = null;
        CurrentPos = pos;
    }


    public void SetTransform(Transform targetTransform)
    {
        CurrentTransform = targetTransform;

        if (CurrentTransform == null)
        {
            return;
        }

        CurrentPos = CurrentTransform.position;

        transformIsCanvas = (CurrentTransform.transform.GetType() == typeof(RectTransform));


        IsMapElement = targetTransform.IsChildOf(MapViewManager.Instance.transform);
    }

    void Update()
    {
        if (CurrentTransform == null)
        {
            return;
        }

        this.CurrentPos = CurrentTransform.position;


        if (!transformIsCanvas)
        {
            if (MapViewManager.Instance.Cam.gameObject.activeInHierarchy)
            {
                CurrentPos = MapViewManager.Instance.Cam.WorldToScreenPoint(this.CurrentPos);
            }
            else
            {
                CurrentPos = MouseLook.Instance.Cam.WorldToScreenPoint(this.CurrentPos);
            }
        }
        

        if (StickToEdgeOfScreen)
        {
            if (CurrentPos.z < 0f && !(IsIgnoringMapBackfacing && IsMapElement))
            {
                CurrentPos = new Vector3(CurrentPos.x < Screen.width /2f ? 0f : Screen.width, CurrentPos.y < Screen.height/2f? 0f : Screen.height, CurrentPos.z);
            }
            else
            {
                CurrentPos = new Vector3(CurrentPos.x < 0 ? 0f : CurrentPos.x, CurrentPos.y < 0 ? 0f : CurrentPos.y, CurrentPos.z);
                CurrentPos = new Vector3(CurrentPos.x > Screen.width ? Screen.width : CurrentPos.x, CurrentPos.y > Screen.height ? Screen.height : CurrentPos.y, CurrentPos.z);
            }
        }

        transform.position = CurrentPos;
    }
}
