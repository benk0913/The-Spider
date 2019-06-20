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

    private void Update()
    {
        RefreshInput();
    }

    void RefreshInput()
    {
        if (Input.GetKey(InputMap.Map["MoveForward"]) && isBeforeBorderTop)
        {
            transform.position += transform.up * MovementSpeed * CurrentCamera.orthographicSize * Time.deltaTime;
        }
        else if (Input.GetKey(InputMap.Map["MoveBackward"]) && isBeforeBorderBottom)
        {
            transform.position += -transform.up * MovementSpeed * CurrentCamera.orthographicSize * Time.deltaTime;
        }

        if (Input.GetKey(InputMap.Map["MoveLeft"]) && isBeforeBorderLeft)
        {
            transform.position += -transform.right * MovementSpeed * CurrentCamera.orthographicSize * Time.deltaTime;
        }
        else if (Input.GetKey(InputMap.Map["MoveRight"]) && isBeforeBorderRight)
        {
            transform.position += transform.right * MovementSpeed * CurrentCamera.orthographicSize * Time.deltaTime;
        }

        if(Input.mouseScrollDelta.y > 0 && CurrentCamera.orthographicSize > maxZoom)
        {
            CurrentCamera.orthographicSize -= ZoomSpeed * Time.deltaTime;

        }
        else if (Input.mouseScrollDelta.y < 0 && CurrentCamera.orthographicSize < minZoom)
        {
            CurrentCamera.orthographicSize += ZoomSpeed * Time.deltaTime;
        }
    }
}
