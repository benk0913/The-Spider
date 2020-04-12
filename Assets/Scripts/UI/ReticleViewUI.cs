using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleViewUI : MonoBehaviour
{
    public static ReticleViewUI Instance;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
