using UnityEngine;
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

                        if(ReticleViewUI.Instance != null)
                        ReticleViewUI.Instance.Show();

                        break;
                    }
                case ActorState.ItemInHands:
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;

                        if (ReticleViewUI.Instance != null)
                            ReticleViewUI.Instance.Hide();

                        OnZoom = false;
                        break;
                    }
                case ActorState.Focusing:
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;

                        if (ReticleViewUI.Instance != null)
                            ReticleViewUI.Instance.Hide();

                        OnZoom = false;
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

    public float sensitivityX
    {
        get
        {
            return bl_PauseOptions.Sensitivity;
        }
    }

    public float sensitivityY
    {
        get
        {
            return bl_PauseOptions.Sensitivity;
        }
    }

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

    [SerializeField]
    float ZoomOnItemMin = 0.486f;

    [SerializeField]
    float ZoomOnItemMax = 0.213f;

    [SerializeField]
    float ZoomOnItemSpeed = 5f;

    public bool OnZoom
    {
        get
        {
            return _onZoom;
        }
        set
        {
            if (value != _onZoom)
            {
                if (value)
                {
                    targetFOV = 5;
                    SpyglassViewUI.Instance.gameObject.SetActive(true);
                }
                else
                {
                    targetFOV = 60;
                    SpyglassViewUI.Instance.gameObject.SetActive(false);
                }

                _onZoom = value;
            }
        }
    }
    bool _onZoom;

    float targetFOV = 60;

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

    public bool CanJump = false;
    public float DistFromGround = 5f;
    public float JumpForce = 5f;
    public LayerMask Jumplayermask;
    public bool recentlyJumped;

    public Rigidbody rBody;
    

    public bool TestInput = false;

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

    private void Start()
    {
        State = ActorState.Idle;
    }

    void FixedUpdate()
    {
        if (CORE.Instance != null)
        {
            if (CORE.Instance.isLoading)
            {
                return;
            }
        }

        if (isAbleToMove)
        {
            RefreshMovementInput();
        }

        if (State == ActorState.ItemInHands)
        {
            if(CurrentItemInHands == null)
            {
                State = ActorState.Idle;
                return;
            }

            CurrentItemInHands.transform.position = Vector3.Lerp(CurrentItemInHands.transform.position, ItemPickViewPoint.position, Time.deltaTime * 4f);
            CurrentItemInHands.transform.rotation = Quaternion.Lerp(CurrentItemInHands.transform.rotation, ItemPickViewPoint.rotation, Time.deltaTime * 4f);

            if(Input.GetAxis("Mouse ScrollWheel") > 0f && ItemPickViewPoint.transform.localPosition.z > ZoomOnItemMax)
            {
                ItemPickViewPoint.transform.position -= (ItemPickViewPoint.transform.forward + (ItemPickViewPoint.transform.up/3f)) * ZoomOnItemSpeed * Time.deltaTime;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && ItemPickViewPoint.transform.localPosition.z < ZoomOnItemMin)
            {
                ItemPickViewPoint.transform.position += (ItemPickViewPoint.transform.forward + (ItemPickViewPoint.transform.up / 3f)) * ZoomOnItemSpeed * Time.deltaTime;
            }
        }

        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, targetFOV, Time.deltaTime * 2f);
    }

    private void Update()
    {
        if (CORE.Instance != null)
        {
            if (CORE.Instance.isLoading)
            {
                return;
            }
        }

        RefreshCancelInput();

        if (isAbleToLookaround)
        {
            RefreshMouseLookInput();
        }

        if (isAbleToInteract)
        {
            RefreshInteractionInput();
        }
    }

    #region Input

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
            else if(State == ActorState.Interpolation)
            {
                return;
            }
            else if (State == ActorState.ItemInHands)
            {
                CurrentItemInHands.Retrieve();
            }
            else if(State == ActorState.Idle)
            {
                CORE.Instance.InvokeEvent("EnterBedOut");
            }
        }
    }

    void RefreshMovementInput()
    {
        if (TestInput)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * MovementSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.position += -transform.forward * MovementSpeed / 2f * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position += -transform.right * MovementSpeed / 2f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * MovementSpeed / 2f * Time.deltaTime;
            }

            if (CanJump && Input.GetKey(KeyCode.Space))
            {
                if (!recentlyJumped)
                {
                    RaycastHit raycastHit;
                    if (Physics.Raycast(transform.position, Vector3.down, out raycastHit, DistFromGround, Jumplayermask))
                    {
                        rBody.velocity = Vector2.zero;
                        rBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                        recentlyJumped = true;
                        StartCoroutine(ResetJumpRoutine());
                    }
                }

            }
        }
        else
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

            if (CanJump && Input.GetKey(InputMap.Map["Jump"]))
            {
                if (!recentlyJumped)
                {
                    RaycastHit raycastHit;
                    if (Physics.Raycast(transform.position, Vector3.down, out raycastHit, DistFromGround, Jumplayermask))
                    {
                        rBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                        recentlyJumped = true;
                        StartCoroutine(ResetJumpRoutine());
                    }
                }

            }
        }
    }

    IEnumerator ResetJumpRoutine()
    {
        yield return new WaitForSeconds(1f);

        recentlyJumped = false;
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

        if(Input.GetKeyDown(InputMap.Map["Zoom"]) && CORE.Instance.SessionRules.Rules.Find(x=>x.name == "Can Zoom") != null)
        {
            OnZoom = !OnZoom;
        }
        else
        {
            OnZoom = OnZoom;
        }
    }

    void RefreshInteractionInput()
    {
        if (State == ActorState.Idle)
        {
            InteractRay.EmitRay();

            if (InteractRay.CurrentInteractable != null && InteractRay.CurrentInteractable.ZoomToInteract && OnZoom)
            {
                Interact();
            }
            else if (Input.GetKeyDown(InputMap.Map["Interact"]) && InteractRay.CurrentInteractable != null && !InteractRay.CurrentInteractable.ZoomToInteract)
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
        ActorState originalState = State;
        State = ActorState.Interpolation;

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

        if (State == ActorState.Interpolation)
        {
            State = originalState;
        }

        CameraTransform.gameObject.SetActive(false);
        view.CurrentCamera.gameObject.SetActive(true);
        FocusingRoutineInstance = null;
    }

    IEnumerator UnfocusViewRoutine()
    {
        ActorState originalState = State;
        State = ActorState.Interpolation;

        CameraTransform.gameObject.SetActive(true);

        if (CurrentFocus != null && CurrentFocus.CurrentCamera != null)
        {
            CurrentFocus.CurrentCamera.gameObject.SetActive(false);
        }
        
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


        if (State == ActorState.Interpolation)
        {
            State = originalState;
        }


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
        if(CurrentItemInHands == null)
        {
            return;
        }

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

            if(item == null)
            {
                break;
            }

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
        ItemInHands,
        Interpolation
    }
}