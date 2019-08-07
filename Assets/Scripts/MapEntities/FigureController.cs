using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureController : MonoBehaviour
{
    [SerializeField]
    MeshRenderer[] Renderers;

    public void SetMaterial(Material material)
    {
        foreach(MeshRenderer renderer in Renderers)
        {
            renderer.material = material;
        }
    }

    private void Reset()
    {
        Renderers = GetComponentsInChildren<MeshRenderer>();
    }
}
