using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopdownCameraMovement : MonoBehaviour
{
    [SerializeField]
    float MovementSpeed = 1f;

    [SerializeField]
    float ZoomSpeed = 1f;


    [SerializeField]
    float maxZoom = 0.1f;

    [SerializeField]
    float minZoom = 1f;

    [SerializeField]
    Camera CurrentCamera;

    [SerializeField]
    Transform TopRightBorder;

    [SerializeField]
    Transform BottomLeftBorder;

    [SerializeField]
    public bool IsOrthographic = true;


    bool isBeforeBorderTop
    {
        get
        {
            return CurrentCamera.transform.position.z < TopRightBorder.position.z;
        }
    }

    bool isBeforeBorderBottom
    {
        get
        {
            return CurrentCamera.transform.position.z > BottomLeftBorder.position.z;
        }
    }

    bool isBeforeBorderLeft
    {
        get
        {
            return CurrentCamera.transform.position.x > BottomLeftBorder.position.x;
        }
    }

    bool isBeforeBorderRight
    {
        get
        {
            return CurrentCamera.transform.position.x < TopRightBorder.position.x;
        }
    }

    float PerspectiveZoomValue
    {
        get
        {
            return Mathf.Clamp(transform.position.y - BottomLeftBorder.position.y, 0.01f, 99f);
        }
    } 

    private void Update()
    {
        RefreshInput();
    }

    void RefreshInput()
    {
        if (Input.GetKey(InputMap.Map["MoveForward"]) && isBeforeBorderTop)
        {
            transform.position += transform.forward * MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);
        }
        else if (Input.GetKey(InputMap.Map["MoveBackward"]) && isBeforeBorderBottom)
        {
            transform.position += -transform.forward * MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);
        }

        if (Input.GetKey(InputMap.Map["MoveLeft"]) && isBeforeBorderLeft)
        {
            transform.position += -transform.right * MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);
        }
        else if (Input.GetKey(InputMap.Map["MoveRight"]) && isBeforeBorderRight)
        {
            transform.position += transform.right * MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);
        }

        if (IsOrthographic)
        {
            if (Input.mouseScrollDelta.y > 0 && CurrentCamera.orthographicSize > maxZoom)
            {
                CurrentCamera.orthographicSize -= ZoomSpeed * Time.deltaTime;
            }
            else if (Input.mouseScrollDelta.y < 0 && CurrentCamera.orthographicSize < minZoom)
            {
                CurrentCamera.orthographicSize += ZoomSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (Input.mouseScrollDelta.y > 0 && transform.position.y > BottomLeftBorder.position.y)
            {
                transform.position -= Vector3.up * ZoomSpeed * Time.deltaTime;
            }
            else if (Input.mouseScrollDelta.y < 0 && transform.position.y < TopRightBorder.position.y)
            {
                transform.position += Vector3.up * ZoomSpeed * Time.deltaTime;
            }
        }

        
    }
}
