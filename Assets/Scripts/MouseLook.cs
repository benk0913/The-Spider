using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public float MovementSpeed = 10.0f;

    float rotationY = 0F;

    [SerializeField]
    Transform CameraTransform;

    private float translation;
    private float straffe;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //Movement
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

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }


        //Look Around
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

}