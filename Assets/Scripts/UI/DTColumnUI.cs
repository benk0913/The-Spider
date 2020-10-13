using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DTColumnUI : MonoBehaviour, IPointerEnterHandler,  IPointerDownHandler
{
    public UnityEvent OnHover;

    public UnityEvent OnClickDown;
    public UnityEvent OnRelease;

    public int OriginalIndex;

    public char RelevantKeywordCharacter;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickDown?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover?.Invoke();
    }

    
}
