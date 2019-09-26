﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLayoutUI : MonoBehaviour
{
    [SerializeField]
    float DistanceFromCenter = 1f;

    [SerializeField]
    bool RefreshOnEnable;

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

        float angleDistance = 360f / transform.childCount;

        for (int i=0;i<transform.childCount;i++)
        {
            transform.GetChild(i).transform.position = CirclePosition(transform.position, DistanceFromCenter, (i+1) * angleDistance);
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
