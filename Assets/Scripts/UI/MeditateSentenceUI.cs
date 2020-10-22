using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeditateSentenceUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    TextMeshProUGUI Label;

    public string Content;

    public Vector2 Direction;
    public float RotationZ;

    public float Speed = 1f;

    System.Action OnClick;

    public float TimeExisting = 3f;

    float TimeLeft;

    public void Show(string content, System.Action onClick = null)
    {
        this.OnClick = onClick;

        this.Content = content;
        this.Label.text = content;
        this.Direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        this.Speed = Random.Range(1f, 5f);
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f));
        StartCoroutine(AnimateRoutine());

        if(CORE.Instance.PsychoEffectRate <= 0)
        {
            GlobalMessagePrompterUI.Instance.Show("I have things to do!", 2f, Color.yellow);
        }
    }

    IEnumerator AnimateRoutine()
    {
        TimeLeft = TimeExisting;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            Label.text = Content.Remove(Random.Range(0, Label.text.Length), 1);
            TimeLeft -= 0.2f;

            Label.color = Color.Lerp(Color.clear, Color.white, TimeLeft / TimeExisting);

            if(TimeLeft <= 0f)
            {
                this.gameObject.transform.SetParent(transform.parent.parent);
                this.gameObject.SetActive(false);
                yield break;
            }
        }
    }

    private void Update()
    {
        transform.position += transform.TransformDirection(Direction.x, Direction.y, 0f) * Time.deltaTime * Speed;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
        this.gameObject.SetActive(false);
    }
}
