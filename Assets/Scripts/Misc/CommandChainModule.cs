using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandChainModule : MonoBehaviour
{
    [SerializeField]
    LineRenderer _LineRenderer;

    [SerializeField]
    float AdditionalY = 0.1f;

    [SerializeField]
    float LineSpeed = 1f;

    Character CurrentCharacter;

    private void Start()
    {
        GameClock.Instance.OnTurnPassed.AddListener(Refresh);
    }

    private void Update()
    {
        _LineRenderer.material.SetTextureOffset("_MainTex", new Vector2(_LineRenderer.material.GetTextureOffset("_MainTex").x + (LineSpeed * Time.deltaTime), 1f));
    }

    public void SetInfo(Character forCharacter)
    {
        CurrentCharacter = forCharacter;

        Refresh();
    }

    void Refresh()
    {
        DisposeCurrentChain();

        if(CurrentCharacter == null)
        {
            return;
        }

        GenerateLine();

    }

    void GenerateLine()
    {
        List<Vector3> Points = new List<Vector3>();

        Character tempChar = CurrentCharacter;
        while(true)
        {
            Points.Add(tempChar.CurrentLocation.transform.position + new Vector3(0f,AdditionalY, 0f));

            if(tempChar.Employer == tempChar || tempChar.Employer == null)
            {
                break;
            }

            tempChar = tempChar.Employer;
        }


        if (Points.Count > 1)
        {
            _LineRenderer.enabled = true;
            _LineRenderer.positionCount = Points.Count;
            _LineRenderer.SetPositions(Points.ToArray());
        }
        else
        {
            _LineRenderer.enabled = false;
        }
    }

    public void DisposeCurrentChain()
    {
        _LineRenderer.enabled = false;
    }
}
