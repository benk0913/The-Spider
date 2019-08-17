using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointAndClickTooltipUI : MonoBehaviour
{
    public static PointAndClickTooltipUI Instance;

    [SerializeField]
    TextMeshProUGUI Text;

    [SerializeField]
    CanvasGroup CG;

    Coroutine ShowRoutineInstance;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string message)
    {
        if(string.IsNullOrEmpty(message))
        {
            return;
        }

        Text.text = message;

        this.gameObject.SetActive(true);

        if (ShowRoutineInstance != null)
        {
            StopCoroutine(ShowRoutineInstance);
        }

        ShowRoutineInstance = StartCoroutine(ShowRoutine());

        transform.SetAsLastSibling();
    }

    IEnumerator ShowRoutine()
    {
        CG.alpha = 0f;

        yield return new WaitForSeconds(0.1f);

        while (CG.alpha < 1f)
        {
            CG.alpha += 2f * Time.deltaTime;

            yield return 0;
        }
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
