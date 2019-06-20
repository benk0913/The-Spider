using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusView : MonoBehaviour
{
    [SerializeField]
    public Camera CurrentCamera;

    public void Activate()
    {
        this.gameObject.SetActive(true);
        MouseLook.Instance.FocusOnView(this);
    }
    
    public void Deactivate()
    {
        MouseLook.Instance.UnfocusCurrentView();
        this.gameObject.SetActive(false);
    }

}
