using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CursorTooltipUI : MonoBehaviour
{
    public static CursorTooltipUI TooltipInstance;

    [SerializeField]
    protected TextMeshProUGUI TextLabel;


    [SerializeField]
    protected CanvasGroup CG;

    Color baseColor;

    private void Awake()
    {
        TooltipInstance = this;
    }

    private void Start()
    {
        baseColor = TextLabel.color;
    }

    public void Show(string message)
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());

        TextLabel.text = message;
        TextLabel.color = baseColor;
    }

    public void Show(string message, float Length)
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(Length));

        TextLabel.text = message;
        TextLabel.color = baseColor;
    }

    public void Show(string message, float Length, Color color)
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(Length));

        TextLabel.text = message;
        TextLabel.color = Color.red;
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());

    }

    protected IEnumerator FadeIn(float Length = 0f)
    {
        while (CG.alpha < 1f)
        {
            CG.alpha += 2f * Time.deltaTime;

            yield return 0;
        }

        if(Length > 0)
        {
            yield return new WaitForSeconds(Length);
            Hide();
        }
    }

    protected IEnumerator FadeOut()
    {
        while (CG.alpha > 0f)
        {
            CG.alpha -= 4f * Time.deltaTime;

            yield return 0;
        }
    }
}
