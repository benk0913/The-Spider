using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomPopEffectUI : MonoBehaviour
{
    public static RandomPopEffectUI Instance;

    [SerializeField]
    Image ImageA;

    [SerializeField]
    Image ImageB;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(Sprite imageA, Sprite imageB, string soundKey)
    {
        this.gameObject.SetActive(true);
        ImageA.sprite = imageA;
        ImageB.sprite = imageB;

        AudioControl.Instance.Play(soundKey);
    }

    public void OnAnimationFinished()
    {
        this.gameObject.SetActive(false);
    }
}
