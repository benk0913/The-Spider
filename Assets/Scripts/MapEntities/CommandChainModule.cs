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

    GameObject CurrentArrowObject;

    List<GameObject> PortraitObjects = new List<GameObject>();

    private void Update()
    {
        _LineRenderer.material.SetTextureOffset("_MainTex", new Vector2(_LineRenderer.material.GetTextureOffset("_MainTex").x + (LineSpeed * Time.deltaTime), 1f));
    }

    public void SetInfo(Character forCharacter)
    {
        CurrentCharacter = forCharacter;

        GameClock.Instance.OnTurnPassed.AddListener(Refresh);

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
        Vector3 targetLocation;
        Character tempChar;

        targetLocation = CurrentCharacter.CurrentLocation.transform.position + new Vector3(0f, AdditionalY, 0f);

        Points.Add(targetLocation);
        SpawnPortraitObject(CurrentCharacter, targetLocation);

        tempChar = CurrentCharacter;
        while(true)
        {
            if (tempChar.WorkLocation != null)
            {
                targetLocation = tempChar.WorkLocation.transform.position + new Vector3(0f, AdditionalY, 0f);
            }
            else if (tempChar.PropertiesOwned.Count > 0)
            {
                targetLocation = tempChar.PropertiesOwned[0].transform.position + new Vector3(0f, AdditionalY, 0f);
            }
            else
            {
                targetLocation = tempChar.CurrentLocation.transform.position + new Vector3(0f, AdditionalY, 0f);
            }

            Points.Add(targetLocation);

            if (tempChar != CurrentCharacter)
            {
                SpawnPortraitObject(tempChar, targetLocation);
            }

            if (tempChar.Employer == tempChar || tempChar.Employer == null)
            {
                break;
            }

            tempChar = tempChar.Employer;
        }
        
        CurrentArrowObject = ResourcesLoader.Instance.GetRecycledObject("ArrowPointer");
        CurrentArrowObject.transform.SetParent(MapViewManager.Instance.MapElementsContainer);
        CurrentArrowObject.transform.position = CurrentCharacter.CurrentLocation.transform.position + new Vector3(0f, AdditionalY, 0f);

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

    public void SpawnPortraitObject(Character character, Vector3 position)
    {
        GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUIWorld");
        tempPortrait.transform.SetParent(CORE.Instance.MainCanvas.transform);
        tempPortrait.transform.SetAsFirstSibling();
        tempPortrait.transform.GetComponent<PortraitUI>().SetCharacter(character, position);
        PortraitObjects.Add(tempPortrait);
    }

    public void DisposeCurrentChain()
    {
        _LineRenderer.enabled = false;

        if (CurrentArrowObject != null)
        {
            CurrentArrowObject.gameObject.SetActive(false);
            CurrentArrowObject = null;
        }

        while(PortraitObjects.Count > 0)
        {
            PortraitObjects[0].transform.SetParent(null);
            PortraitObjects[0].gameObject.SetActive(false);
            PortraitObjects.RemoveAt(0);
        }
    }

    public void Hide()
    {
        DisposeCurrentChain();
        GameClock.Instance.OnTurnPassed.RemoveListener(Refresh);
    }

    
}
