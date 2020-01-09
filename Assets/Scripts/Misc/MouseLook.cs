﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    public static MouseLook Instance;

    public ActorState State
    {
        get
        {
            return _state;
        }
        set
        {
            switch(value)
            {
                case ActorState.Idle:
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        break;
                    }
                case ActorState.ItemInHands:
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        break;
                    }
                case ActorState.Focusing:
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        break;
                    }
            }

            _state = value;
        }
    }
    ActorState _state;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }

    [SerializeField]
    public RotationAxes axes = RotationAxes.MouseXAndY;

    [SerializeField]
    public float sensitivityX = 15F;

    [SerializeField]
    public float sensitivityY = 15F;

    [SerializeField]
    public float minimumX = -360F;

    [SerializeField]
    public float maximumX = 360F;

    [SerializeField]
    public float minimumY = -60F;

    [SerializeField]
    public float maximumY = 60F;

    [SerializeField]
    public float MovementSpeed = 10.0f;

    [SerializeField]
    Transform CameraTransform;

    [SerializeField]
    public Camera Cam;

    [SerializeField]
    InteractionRay InteractRay;

    [SerializeField]
    Transform ItemPickViewPoint;

    float rotationY = 0F;

    float translation;
    float straffe;

    FocusView CurrentFocus;

    Vector3 preFocusPosition;
    Quaternion preFocusRotation;


    PickableItem CurrentItemInHands;

    Vector3 prePickupItemPosition;
    Quaternion prePickupItemRotation;

    public GameObject CurrentWindow;

    public bool isAbleToMove
    {
        get
        {
            return (State == ActorState.Idle || State == ActorState.ItemInHands) && FocusingRoutineInstance == null;
        }
    }

    public bool isAbleToLookaround
    {
        get
        {
            return (State == ActorState.Idle || State == ActorState.ItemInHands) && FocusingRoutineInstance == null;
        }
    }

    public bool isAbleToInteract
    {
        get
        {
            return (State == ActorState.Idle || State == ActorState.ItemInHands) && FocusingRoutineInstance == null;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(CORE.Instance.isLoading)
        {
            return;
        }

        RefreshInput();

        if(State == ActorState.ItemInHands)
        {
            CurrentItemInHands.transform.position = Vector3.Lerp(CurrentItemInHands.transform.position, ItemPickViewPoint.position, Time.deltaTime * 4f);
            CurrentItemInHands.transform.rotation = Quaternion.Lerp(CurrentItemInHands.transform.rotation, ItemPickViewPoint.rotation, Time.deltaTime * 4f);
        }
    }

    #region Input

    void RefreshInput()
    {
        RefreshCancelInput();

        if(isAbleToMove)
        {
            RefreshMovementInput();
        }

        if(isAbleToLookaround)
        {
            RefreshMouseLookInput();
        }

        if(isAbleToInteract)
        {
            RefreshInteractionInput();
        }
    }

    void RefreshCancelInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(CurrentWindow != null)
            {
                return;
            }

            if (State == ActorState.Focusing)
            {
                CurrentFocus.Deactivate();
            }
            else if (State == ActorState.ItemInHands)
            {
                CurrentItemInHands.Retrieve();
            }
        }
    }

    void RefreshMovementInput()
    {
        if (Input.GetKey(InputMap.Map["MoveForward"]))
        {
            transform.position += transform.forward * MovementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(InputMap.Map["MoveBackward"]))
        {
            transform.position += -transform.forward * MovementSpeed / 2f * Time.deltaTime;
        }

        if (Input.GetKey(InputMap.Map["MoveLeft"]))
        {
            transform.position += -transform.right * MovementSpeed / 2f * Time.deltaTime;
        }
        else if (Input.GetKey(InputMap.Map["MoveRight"]))
        {
            transform.position += transform.right * MovementSpeed / 2f * Time.deltaTime;
        }
    }

    void RefreshMouseLookInput()
    {
        if(CurrentItemInHands != null && CurrentItemInHands.DisableLookaround)
        {
            return;
        }

        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            CameraTransform.localEulerAngles = new Vector3(-rotationY, CameraTransform.localEulerAngles.y, 0);

        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            CameraTransform.localEulerAngles = new Vector3(-rotationY, CameraTransform.localEulerAngles.y, 0);
        }
    }

    void RefreshInteractionInput()
    {
        if (State == ActorState.Idle)
        {
            InteractRay.EmitRay();

            if (Input.GetKeyDown(InputMap.Map["Interact"]))
            {
                Interact();
            }
        }
        else if (State == ActorState.Focusing)
        {
            UnfocusCurrentView();
        }
        else if (State == ActorState.ItemInHands)
        {
            if (Input.GetKeyDown(InputMap.Map["Interact"]))
            {
                CurrentItemInHands.Interact();
            }
            else if (Input.GetKeyDown(InputMap.Map["Secondary Interaction"]))
            {
                CurrentItemInHands.SecondaryInteract();
            }
            else if (Input.GetKeyDown(InputMap.Map["Accept Interaction"]))
            {
                CurrentItemInHands.AcceptInteract();
            }
        }
    }

    #endregion

    #region Focusing

    public void FocusOnView(FocusView view)
    {
        if (FocusingRoutineInstance != null)
        {
            StopCoroutine(FocusingRoutineInstance);
        }

        CurrentFocus = view;
        preFocusPosition = CameraTransform.position;
        preFocusRotation = CameraTransform.rotation;

        State = ActorState.Focusing;

        FocusingRoutineInstance = StartCoroutine(FocusOnViewRoutine(view));
    }

    public void FocusOnItemInHands()
    {
        if(CurrentItemInHands == null)
        {
            return;
        }

        if (FocusingRoutineInstance != null)
        {
            StopCoroutine(FocusingRoutineInstance);
        }

        FocusingRoutineInstance = StartCoroutine(FocusOnItemInHandsRoutine());
    }

    public void UnfocusOnItemInHands()
    {
        if(FocusingRoutineInstance != null)
        {
            StopCoroutine(FocusingRoutineInstance);
            FocusingRoutineInstance = null;
        }
    }

    public void UnfocusCurrentView()
    {
        if (FocusingRoutineInstance != null)
        {
            StopCoroutine(FocusingRoutineInstance);
        }

        State = ActorState.Idle;

        FocusingRoutineInstance = StartCoroutine(UnfocusViewRoutine());
    }

    Coroutine FocusingRoutineInstance = null;
    IEnumerator FocusOnViewRoutine(FocusView view)
    {
        float t = 0f;
        while(t<0.1f)
        {
            t += 0.1f * Time.deltaTime;

            CameraTransform.position = Vector3.Lerp(CameraTransform.position,    view.CurrentCamera.transform.position, t);
            CameraTransform.rotation = Quaternion.Lerp(CameraTransform.rotation, view.CurrentCamera.transform.rotation, t);

            yield return 0;
        }

        CameraTransform.position = view.CurrentCamera.transform.position;
        CameraTransform.rotation = view.CurrentCamera.transform.rotation;

        CameraTransform.gameObject.SetActive(false);
        view.CurrentCamera.gameObject.SetActive(true);
        FocusingRoutineInstance = null;
    }

    IEnumerator UnfocusViewRoutine()
    {
        CameraTransform.gameObject.SetActive(true);
        CurrentFocus.CurrentCamera.gameObject.SetActive(false);
        
        float t = 0f;
        while (t < 0.1f)
        {
            t += 0.1f * Time.deltaTime;

            CameraTransform.position = Vector3.Lerp(CameraTransform.position, preFocusPosition, t);
            CameraTransform.rotation = Quaternion.Lerp(CameraTransform.rotation, preFocusRotation, t);

            yield return 0;
        }

        CameraTransform.position = preFocusPosition;
        CameraTransform.rotation = preFocusRotation;


        FocusingRoutineInstance = null;
        CurrentFocus = null;
    }

    IEnumerator FocusOnItemInHandsRoutine()
    {
        Vector3 angle = CameraTransform.position - CurrentItemInHands.transform.GetChild(0).position;

        float t = 0f;
        while(t<1f)
        {
            if(CurrentItemInHands == null)
            {
                FocusingRoutineInstance = null;
                yield break;
            }

            if(Input.GetMouseButtonDown(0))
            {
                angle = CameraTransform.position - CurrentItemInHands.transform.GetChild(0).position;
            }
            
            if(Input.GetMouseButton(0))
            {
                
                CurrentItemInHands.transform.GetChild(0).Rotate(
                0f,
                (angle.y * -Input.GetAxis("Mouse X") * 3000f * Time.deltaTime),
                (angle.z * Input.GetAxis("Mouse Y") * 3000f * Time.deltaTime), Space.World);
            
            }

            yield return 0;
        }

        FocusingRoutineInstance = null;
    }

    
    #endregion

    #region PickableItems
    public void PickUpItem(PickableItem item)
    {
        if (FocusingRoutineInstance != null)
        {
            return;
        }

        State = ActorState.ItemInHands;
        CurrentItemInHands = item;
        prePickupItemPosition = CurrentItemInHands.transform.position;
        prePickupItemRotation = CurrentItemInHands.transform.rotation;

        if(item.GetComponent<Rigidbody>() != null)
        {
            item.GetComponent<Rigidbody>().isKinematic = true;
        }

        PickupItemRoutineInstance = StartCoroutine(PickUpItemRoutine(item));
    }

    public void RetreiveItem()
    {
        if (PickupItemRoutineInstance != null)
        {
            return;
        }

        PickupItemRoutineInstance = StartCoroutine(RetreiveItemRoutine(CurrentItemInHands));

        if (CurrentItemInHands.GetComponent<Rigidbody>() != null)
        {
            CurrentItemInHands.GetComponent<Rigidbody>().isKinematic = false;
        }

        CurrentItemInHands = null;
        State = ActorState.Idle;
    }

    public void ReleaseItem()
    {
        if (CurrentItemInHands.GetComponent<Rigidbody>() != null)
        {
            CurrentItemInHands.GetComponent<Rigidbody>().isKinematic = false;
        }

        CurrentItemInHands.transform.GetChild(0).rotation = Quaternion.Euler(Vector3.zero);

        PickupItemRoutineInstance = null;
        CurrentItemInHands = null;
        State = ActorState.Idle;
    }

    Coroutine PickupItemRoutineInstance = null;
    IEnumerator PickUpItemRoutine(PickableItem view)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += 1f * Time.deltaTime;

            if(CurrentItemInHands == null)
            {
                break;
            }

            CurrentItemInHands.transform.position = Vector3.Lerp(CurrentItemInHands.transform.position, ItemPickViewPoint.position, t);
            CurrentItemInHands.transform.rotation = Quaternion.Lerp(CurrentItemInHands.transform.rotation, ItemPickViewPoint.rotation, t);

            yield return 0;
        }
        
        PickupItemRoutineInstance = null;
    }

    IEnumerator RetreiveItemRoutine(PickableItem item)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += 1f * Time.deltaTime;

            item.transform.position = Vector3.Lerp(item.transform.position, prePickupItemPosition, t);
            item.transform.rotation = Quaternion.Lerp(item.transform.rotation, prePickupItemRotation, t);

            yield return 0;
        }

        PickupItemRoutineInstance = null;
    }


    public

    #endregion

    void Interact()
    {
        if (InteractRay.CurrentInteractable == null)
        {
            return;
        }

        InteractRay.CurrentInteractable.Interact();
        InteractRay.UnHover();
    }

    public enum ActorState
    {
        Idle,
        Focusing,
        ItemInHands
    }
}