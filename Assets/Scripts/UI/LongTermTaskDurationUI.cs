using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LongTermTaskDurationUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TextMeshProUGUI TurnsLeft;
    
    [SerializeField]
    Image Filer;

    [SerializeField]
    WorldPositionLerperUI Lerper;

    [SerializeField]
    PortraitUI CharacterPortrait;

    [SerializeField]
    TextMeshProUGUI InstanceCount;

    [SerializeField]
    GameObject ButtonsContainer;

    [SerializeField]
    ActionPortraitUI ActionPortrait;

    public List<LongTermTaskEntity> Instances = new List<LongTermTaskEntity>();

    public int CurrentIndex = 0;

    public void AddEntity(LongTermTaskEntity entity)
    {
        Instances.Add(entity);
        CurrentIndex = Instances.Count - 1;

        Refresh();
    }

    public void RemoveEntity(LongTermTaskEntity entity)
    {
        Instances.Remove(entity);
        CurrentIndex = Instances.Count - 1;

        Refresh();

    }

    public void Refresh()
    {
        if(Instances.Count == 0)
        {
            return;
        }

        LongTermTaskEntity currentEntity = Instances[CurrentIndex];

        ActionPortrait.SetAction(currentEntity);
        Title.text = currentEntity.CurrentTask.name;
        Lerper.SetTransform(currentEntity.transform);

        TurnsLeft.text = currentEntity.TurnsLeft+"<size=10>-Turns...</size>";
        Filer.fillAmount = (float)currentEntity.TurnsLeft / currentEntity.CurrentTask.TurnsToComplete;

        CharacterPortrait.SetCharacter(currentEntity.CurrentCharacter);

        InstanceCount.gameObject.SetActive(Instances.Count > 1);
        ButtonsContainer.gameObject.SetActive(Instances.Count > 1);

        InstanceCount.text = (CurrentIndex) + "/" + (Instances.Count);
    }

    public void NavigateRight()
    {
        CurrentIndex++;
        if(CurrentIndex >= Instances.Count)
        {
            CurrentIndex = 0;
        }

        Refresh();
    }

    public void NavigateLeft()
    {
        CurrentIndex--;
        if (CurrentIndex < 0)
        {
            CurrentIndex = Instances.Count - 1;
        }

        Refresh();
    }
}
