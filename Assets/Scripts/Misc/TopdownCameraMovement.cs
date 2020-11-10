using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    void MoveForward(float addedSpeed = 1f)
    {
        if(!isBeforeBorderTop)
        {
            return;
        }

        transform.position += transform.forward * addedSpeed* MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);

        PlayerGivesInput = true;
    }

    void MoveBackward(float addedSpeed = 1f)
    {
        if (!isBeforeBorderBottom)
        {
            return;
        }

        transform.position += -transform.forward * addedSpeed* MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);

        PlayerGivesInput = true;
    }

    void MoveRight(float addedSpeed = 1f)
    {
        if (!isBeforeBorderRight)
        {
            return;
        }

        transform.position += transform.right * addedSpeed * MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);

        PlayerGivesInput = true;
    }

    void MoveLeft(float addedSpeed = 1f)
    {
        if (!isBeforeBorderLeft)
        {
            return;
        }

        transform.position += -transform.right * addedSpeed* MovementSpeed * Time.deltaTime *
                (IsOrthographic ? CurrentCamera.orthographicSize : PerspectiveZoomValue);

        PlayerGivesInput = true;
    }

    void RefreshInput()
    {
        if(AllCharactersWindowUI.Instance.gameObject.activeInHierarchy || SelectCharacterViewUI.Instance.gameObject.activeInHierarchy)
        {
            return;
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        { 
            if (Input.mousePosition.x > Screen.width - Screen.width / 100f)
            {
                MoveRight();
            }
            else if (Input.mousePosition.x < Screen.width / 100f)
            {
                MoveLeft();
            }

            if (Input.mousePosition.y > Screen.height - Screen.height / 100f)
            {
                MoveForward();
            }
            else if (Input.mousePosition.y < Screen.height /100f)
            {
                MoveBackward();
            }
        }

        if (Input.GetKey(InputMap.Map["MoveForward"]))
        {
            MoveForward();
        }
        else if (Input.GetKey(InputMap.Map["MoveBackward"]))
        {
            MoveBackward();
        }

        if (Input.GetKey(InputMap.Map["MoveLeft"]))
        {
            MoveLeft();
        }
        else if (Input.GetKey(InputMap.Map["MoveRight"]))
        {
            MoveRight();
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
