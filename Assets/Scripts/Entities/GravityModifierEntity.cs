using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifierEntity : MonoBehaviour
{
    [SerializeField]
    float TargetGravity;

    [SerializeField]
    float OriginalGravity;

    public bool OnEnableEffect;

    private void OnEnable()
    {
        OriginalGravity = Physics.gravity.y;

        if(OnEnableEffect)
        {
            Modify();
        }
    }

    public void Modify()
    {
        Physics.gravity = new Vector3(0f, TargetGravity, 0f);
    }
}
