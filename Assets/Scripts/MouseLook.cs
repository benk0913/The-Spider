using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    public static MouseLook Instance;

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

    float rotationY = 0F;

    float translation;
    float straffe;

    FocusView CurrentFocus;

    Vector3 preFocusPosition;
    Quaternion preFocusRotation;


    public bool isAbleToMove
    {
        get
        {
            return CurrentFocus == null;
        }
    }

    public bool isAbleToLookaround
    {
        get
        {
            return CurrentFocus == null;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        RefreshInput();
        
    }

    void RefreshInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(CurrentFocus != null)
            {
                CurrentFocus.Deactivate();
            }
        }

        if(isAbleToMove)
        {
            RefreshMovementInput();
        }

        if(isAbleToLookaround)
        {
            RefreshMouseLookInput();
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

    public void FocusOnView(FocusView view)
    {
        if (FocusingRoutineInstance != null)
        {
            return;
        }

        CurrentFocus = view;
        preFocusPosition = CameraTransform.position;
        preFocusRotation = CameraTransform.rotation;

        FocusingRoutineInstance = StartCoroutine(FocusOnViewRoutine(view));
    }

    public void UnfocusCurrentView()
    {
        if (FocusingRoutineInstance != null)
        {
            return;
        }
        
        FocusingRoutineInstance = StartCoroutine(UnfocusViewRoutine());
    }

    Coroutine FocusingRoutineInstance;
    IEnumerator FocusOnViewRoutine(FocusView view)
    {
        float t = 0f;
        while(t<1f)
        {
            t += 1f * Time.deltaTime;

            CameraTransform.position = Vector3.Lerp(preFocusPosition,    view.CurrentCamera.transform.position, t);
            CameraTransform.rotation = Quaternion.Lerp(preFocusRotation, view.CurrentCamera.transform.rotation, t);

            yield return 0;
        }

        CameraTransform.gameObject.SetActive(false);
        view.CurrentCamera.gameObject.SetActive(true);
        FocusingRoutineInstance = null;
    }

    IEnumerator UnfocusViewRoutine()
    {
        CameraTransform.gameObject.SetActive(true);
        CurrentFocus.CurrentCamera.gameObject.SetActive(false);
        
        float t = 0f;
        while (t < 1f)
        {
            t += 1f * Time.deltaTime;

            CameraTransform.position = Vector3.Lerp(CurrentFocus.CurrentCamera.transform.position, preFocusPosition, t);
            CameraTransform.rotation = Quaternion.Lerp(CurrentFocus.CurrentCamera.transform.rotation, preFocusRotation, t);

            yield return 0;
        }

        FocusingRoutineInstance = null;
        CurrentFocus = null;
    }


}