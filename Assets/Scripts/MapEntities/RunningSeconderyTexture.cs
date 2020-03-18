using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RunningSeconderyTexture : MonoBehaviour
{
    MeshRenderer mRenderer;

    Vector2 noiseOffset;

    [SerializeField]
    float Speed = 1f;
    

    private void Start()
    {
        mRenderer = GetComponent<MeshRenderer>();
        noiseOffset = Vector2.zero;
    }

    void Update()
    {
        noiseOffset += Vector2.one * Time.deltaTime * Speed;
        mRenderer.material.SetTextureOffset("_DetailAlbedoMap", noiseOffset);
    }
}
