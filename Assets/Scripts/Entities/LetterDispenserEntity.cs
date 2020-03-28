using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class LetterDispenserEntity : MonoBehaviour, ISaveFileCompatible
{
    public static LetterDispenserEntity Instance;

    [SerializeField]
    GameObject LetterPrefab;

    [SerializeField]
    GameObject LetterPrefabRaven;

    [SerializeField]
    Transform targetPoint;

    [SerializeField]
    Transform targetPointRaven;

    [SerializeField]
    float DispensingSpeed = 1f;


    [SerializeField]
    public UnityEvent OnReceiveLetter;

    [SerializeField]
    public UnityEvent OnReceiveLetterRaven;

    public List<EnvelopeEntity> Envelopes = new List<EnvelopeEntity>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameClock.Instance.OnWeekPassed.AddListener(OnWeekPassed);

        if(GameClock.Instance.CurrentTurn == 0 && CORE.Instance.Database.Timeline.Length > 0)
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
            StartCoroutine(DispenseLetterRoutine(GenerateLetter(letters[i]).transform, i, letters[i].Preset.FromRaven));
        }
    }

    public void DispenseLetter(Letter letter)
    {
        StartCoroutine(DispenseLetterRoutine(GenerateLetter(letter).transform,0f,letter.Preset.FromRaven));
    }

    IEnumerator DispenseLetterRoutine(Transform letterTransform, float addedY = 0f, bool isRaven = false)
    {
       

        if(isRaven)
        {
            RavenEntity.Instance.ReceiveLetter();
            CORE.Instance.InvokeEvent("NewLetterRaven");
        }
        else
        {
            CORE.Instance.InvokeEvent("NewLetter");
        }

        yield return 0;

        Quaternion targetRotation = Quaternion.Euler(letterTransform.rotation.x+90f, letterTransform.rotation.y, letterTransform.rotation.z+ Random.Range(0f, 180f));

        float t = 0f;
        while(t<1f)
        {
            t += DispensingSpeed * Time.deltaTime;

            if(isRaven)
            {
                letterTransform.position = Vector3.Lerp(letterTransform.position, targetPointRaven.position + new Vector3(0f, addedY / 100f + Envelopes.Count / 100f, 0f), t);
                letterTransform.rotation = Quaternion.Lerp(letterTransform.rotation, targetRotation, t);
            }
            else
            {
                letterTransform.position = Vector3.Lerp(letterTransform.position, targetPoint.position + new Vector3(0f, addedY / 100f + Envelopes.Count / 100f, 0f), t);
                letterTransform.rotation = Quaternion.Lerp(letterTransform.rotation, targetRotation, t);
            }

            yield return 0;
        }

        RefreshEnvelopesHeight();
    }

    void RefreshEnvelopesHeight()
    {
        for(int i=0;i<Envelopes.Count;i++)
        {
            if (Envelopes[i].PresetLetter.FromRaven)
            {
                Envelopes[i].transform.position = targetPointRaven.position + new Vector3(0f, i / 100f, 0f);
            }
            else
            {
                Envelopes[i].transform.position = targetPoint.position + new Vector3(0f, i / 100f, 0f);
            }
        }
    }

    GameObject GenerateLetter(Letter letter)
    {
        GameObject tempLetter;

        if (letter.Preset.FromRaven)
        {
            tempLetter = Instantiate(LetterPrefabRaven);
            tempLetter.transform.position = targetPointRaven.position;
            tempLetter.transform.rotation = targetPointRaven.rotation;
            tempLetter.transform.SetParent(targetPointRaven);
            OnReceiveLetterRaven.Invoke();
        }
        else
        {
            tempLetter = Instantiate(LetterPrefab);
            tempLetter.transform.position = transform.position;
            tempLetter.transform.rotation = transform.rotation;
            OnReceiveLetter.Invoke();
        }

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

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        for(int i=0;i<Envelopes.Count;i++)
        {
            node["Envelopes"][i] = Envelopes[i].ToJSON();
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        Envelopes.Clear();

        for (int i=0;i<node["Envelopes"].Count;i++)
        {
            Letter tempLetter = new Letter(CORE.Instance.Database.PresetLetters.Find(x => x.name == node["Envelopes"][i]["Preset"]));
            EnvelopeEntity tempEnvelope = GenerateLetter(tempLetter).GetComponent<EnvelopeEntity>();
            tempEnvelope.FromJSON(node["Envelopes"][i]);
            StartCoroutine(DispenseLetterRoutine(tempEnvelope.transform,0f,tempLetter.Preset.FromRaven));
        }
    }

    public void ImplementIDs()
    {
        foreach(EnvelopeEntity envelope in Envelopes)
        {
            envelope.ImplementIDs();
        }
    }
}