using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    [SerializeField]
    Camera m_Camera;

    [SerializeField]
    public string CameraTag;

    private void Awake()
    {
        if(m_Camera == null)
        {
            m_Camera = GameObject.FindGameObjectWithTag(CameraTag).GetComponent<Camera>();
        }
    }

    private void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * -Vector3.forward,
             m_Camera.transform.rotation * Vector3.up);

    }
}
