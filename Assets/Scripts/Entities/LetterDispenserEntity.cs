using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetterDispenserEntity : MonoBehaviour
{
    public static LetterDispenserEntity Instance;

    [SerializeField]
    GameObject LetterPrefab;

    [SerializeField]
    Transform targetPoint;

    [SerializeField]
    float DispensingSpeed = 1f;

    List<EnvelopeEntity> Envelopes = new List<EnvelopeEntity>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameClock.Instance.OnWeekPassed.AddListener(OnWeekPassed);

        if(GameClock.Instance.CurrentTurn == 0)
        {
            foreach (LetterPreset letter in CORE.Instance.Database.Timeline[0].Letters)
            {
                DispenseLetter(new Letter(letter));
            }
        }

        
    }

    void OnWeekPassed()
    {
        if(GameClock.Instance.CurrentWeek >= CORE.Instance.Database.Timeline.Length)
        {
            return;
        }

        foreach (LetterPreset letter in CORE.Instance.Database.Timeline[GameClock.Instance.CurrentWeek].Letters)
        {
            DispenseLetter(new Letter(letter));
        }
    }

    public void DispenseLetters(Letter[] letters)
    {
        if(letters == null || letters.Length == 0)
        {
            return;
        }

        for(int i=0;i<letters.Length;i++)
        {
            StartCoroutine(DispenseLetterRoutine(GenerateLetter(letters[i]).transform, i));
        }
    }

    public void DispenseLetter(Letter letter)
    {
        StartCoroutine(DispenseLetterRoutine(GenerateLetter(letter).transform));
    }

    IEnumerator DispenseLetterRoutine(Transform letterTransform, float addedY = 0f)
    {
        CORE.Instance.InvokeEvent("NewLetter");

        yield return 0;

        Quaternion targetRotation = Quaternion.Euler(letterTransform.rotation.x+90f, letterTransform.rotation.y, letterTransform.rotation.z+ Random.Range(0f, 180f));

        float t = 0f;
        while(t<1f)
        {
            t += DispensingSpeed * Time.deltaTime;

            letterTransform.position = Vector3.Lerp(letterTransform.position, targetPoint.position +new Vector3(0f,addedY/100f+Envelopes.Count/100f,0f), t);
            letterTransform.rotation = Quaternion.Lerp(letterTransform.rotation, targetRotation, t);

            yield return 0;
        }

        RefreshEnvelopesHeight();
    }

    void RefreshEnvelopesHeight()
    {
        for(int i=0;i<Envelopes.Count;i++)
        {
            Envelopes[i].transform.position = targetPoint.position + new Vector3(0f, i / 100f, 0f);
        }
    }

    GameObject GenerateLetter(Letter letter)
    {
        GameObject tempLetter = Instantiate(LetterPrefab);
        tempLetter.transform.position = transform.position;
        tempLetter.transform.rotation = transform.rotation;

        EnvelopeEntity envelope = tempLetter.GetComponent<EnvelopeEntity>();
        envelope.SetInfo(letter, DisposeLetter);
        Envelopes.Add(envelope);

        return tempLetter;
    }

    public void DisposeLetter(EnvelopeEntity envelope)
    {
        Envelopes.Remove(envelope);
        RefreshEnvelopesHeight();
    }

}