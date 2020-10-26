using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEntity : MonoBehaviour
{
    public Transform TargetPoint;

    public void Teleport()
    {
        MouseLook.Instance.transform.position = TargetPoint.position;
        MouseLook.Instance.rBody.velocity = Vector2.zero;

    }
}
