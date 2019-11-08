using UnityEngine;
using System.Collections;

public class AutoRot : MonoBehaviour {

    public float Speed = 50f;
    public Vector3 Direction;

    private void OnEnable()
    {
        if (Direction == Vector3.zero)
        {
            Direction = transform.forward;
        }
    }

    void Update()
    {
        transform.Rotate(Direction * Speed * Time.deltaTime);
    }
}
