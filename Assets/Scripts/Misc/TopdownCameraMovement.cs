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
    
    public bool PlayerGivesInput { private set; get; }

    public bool replaceXZ = false;


    bool isBeforeBorderTop
    {
        get
        {
            return replaceXZ ? CurrentCamera.transform.position.x < TopRightBorder.position.x : CurrentCamera.transform.position.z < TopRightBorder.position.z;
        }
    }

    bool isBeforeBorderBottom
    {
        get
        {
            return replaceXZ ? CurrentCamera.transform.position.x > BottomLeftBorder.position.x : CurrentCamera.transform.position.z > BottomLeftBorder.position.z;
        }
    }

    bool isBeforeBorderLeft
    {
        get
        {
            return replaceXZ ? CurrentCamera.transform.position.z < BottomLeftBorder.position.z : CurrentCamera.transform.position.x > BottomLeftBorder.position.x;
        }
    }

    bool isBeforeBorderRight
    {
        get
        {
            return replaceXZ ? (CurrentCamera.transform.position.z > TopRightBorder.position.z) : (CurrentCamera.transform.position.x < TopRightBorder.position.x);
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

            PlayerGivesInput = true;
        }
        else if (Input.GetKey(InputMap.Map["MoveBackward"]) && isBeforeBorderBottom)
        {
            transform.position += -transform.forward * MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);

            PlayerGivesInput = true;
        }

        if (Input.GetKey(InputMap.Map["MoveLeft"]) && isBeforeBorderLeft)
        {
            transform.position += -transform.right * MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);

            PlayerGivesInput = true;
        }
        else if (Input.GetKey(InputMap.Map["MoveRight"]) && isBeforeBorderRight)
        {
            transform.position += transform.right * MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);

            PlayerGivesInput = true;
        }
        else
        {
            PlayerGivesInput = false;
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
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

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

    public void ViewTarget(Transform target)
    {
        if(ViewTargetRoutineInstance != null)
        {
            StopCoroutine(ViewTargetRoutineInstance);
        }

        ViewTargetRoutineInstance = StartCoroutine(ViewTargetRoutine(target));
    }

    Coroutine ViewTargetRoutineInstance;
    IEnumerator ViewTargetRoutine(Transform target)
    {
        Vector3 targetPos;

        float t = 0f;
        while(t<1f)
        {
            if(PlayerGivesInput)
            {
                ViewTargetRoutineInstance = null;
                yield break;
            }

            t += 0.3f * Time.deltaTime;

            targetPos = new Vector3(target.position.x - 0.20f, transform.position.y, target.position.z - 0.20f);
            transform.position = Vector3.Slerp(transform.position, targetPos, t);

            yield return 0;
        }

        ViewTargetRoutineInstance = null;
    }
}
