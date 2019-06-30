using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    [SerializeField]
    Camera m_Camera;

    private void Awake()
    {
        if(m_Camera)
        {
            m_Camera = Camera.current;
        }
    }

    private void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * -Vector3.forward,
             m_Camera.transform.rotation * Vector3.up);

    }
}
