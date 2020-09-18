using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLayoutUI : MonoBehaviour
{
    [SerializeField]
    float DistanceFromCenter = 1f;

    [SerializeField]
    float SpiralDistance = 1f;

    [SerializeField]
    float SpiralAngleDistance= 60f;

    [SerializeField]
    bool RefreshOnEnable;

    public bool Spiral = false;

    private void OnEnable()
    {
        if (RefreshOnEnable)
        {
            RefreshLayout();
        }
    }

    public void RefreshLayout()
    {
        if(transform.childCount <= 1)
        {
            transform.GetChild(0).transform.position = transform.position;
            return;
        }

        float angleDistance = 60f;

        if (Spiral)
        {
            angleDistance = SpiralAngleDistance;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.position = CirclePosition(transform.position, DistanceFromCenter * (i * SpiralDistance) , (i + 1) * angleDistance);
            }
        }
        else
        {
            angleDistance = 360f / transform.childCount;

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.position = CirclePosition(transform.position, DistanceFromCenter, (i + 1) * angleDistance);
            }
        }
    }

    Vector3 CirclePosition(Vector3 center, float radius, float angle)
    {
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

}
