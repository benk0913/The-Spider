using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CursorTooltipUI : MonoBehaviour
{
    public static CursorTooltipUI Instance;

    [SerializeField]
    TextMeshProUGUI TextLabel;

    [SerializeField]
    CanvasGroup CG;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(string message)
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());

        TextLabel.text = message;
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());

    }

    IEnumerator FadeIn()
    {
        while(CG.alpha < 1f)
        {
            CG.alpha += 2f * Time.deltaTime;

            yield return 0;
        }
    }

    IEnumerator FadeOut()
    {
        while (CG.alpha > 0f)
        {
            CG.alpha -= 4f * Time.deltaTime;

            yield return 0;
        }
    }
}
