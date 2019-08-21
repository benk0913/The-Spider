using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    Transform TaskListContainer;

    public Dictionary<LongTermTask, List<LongTermTaskEntity>> Instances = new Dictionary<LongTermTask, List<LongTermTaskEntity>>();

    public int ChunkIndex = 0;
    public int ListIndex  = 0;

    public void AddEntity(LongTermTaskEntity entity)
    {
        if(!Instances.ContainsKey(entity.CurrentTask))
        {
            Instances.Add(entity.CurrentTask, new List<LongTermTaskEntity>());
        }

        Instances[entity.CurrentTask].Add(entity);

        ListIndex = Instances.Keys.Count - 1;
        ChunkIndex = Instances[Instances.Keys.ElementAt(ListIndex)].Count - 1;

        Refresh();
    }

    public void RemoveEntity(LongTermTaskEntity entity)
    {
        if (!Instances.ContainsKey(entity.CurrentTask))
        {
            return;
        }
        
        if(!Instances[entity.CurrentTask].Contains(entity))
        {
            return;
        }

        Instances[entity.CurrentTask].Remove(entity);

        ListIndex = Instances.Keys.Count - 1;
        ChunkIndex = Instances[Instances.Keys.ElementAt(ListIndex)].Count - 1;

        if(Instances[entity.CurrentTask].Count == 0)
        {
            Instances.Remove(entity.CurrentTask);
        }

        Refresh();


    }

    public void Refresh()
    {
        if(Instances.Count == 0)
        {
            return;
        }

        if(Instances.Keys.Count <= ListIndex || !Instances.Keys.Contains(Instances.Keys.ElementAt(ListIndex)))
        {
            return;
        }

        LongTermTaskEntity currentEntity = Instances[Instances.Keys.ElementAt(ListIndex)][ChunkIndex];

        ActionPortrait.SetAction(currentEntity);
        Title.text = currentEntity.CurrentTask.name;
        Lerper.SetTransform(currentEntity.transform);

        TurnsLeft.text = currentEntity.TurnsLeft+"<size=10>-Turns...</size>";
        Filer.fillAmount = (float)currentEntity.TurnsLeft / currentEntity.CurrentTask.TurnsToComplete;

        CharacterPortrait.SetCharacter(currentEntity.CurrentCharacter);

        InstanceCount.gameObject.SetActive(Instances.Count > 1);
        ButtonsContainer.gameObject.SetActive(Instances.Count > 1);

        int totalCount = 0;
        for (int i = 0; i < Instances.Keys.Count; i++)
        {
            totalCount += Instances[Instances.Keys.ElementAt(i)].Count;
        }

        int elementsBeforeCurrentList = 0;
        for (int i = 0; i < ListIndex; i++)
        {
            elementsBeforeCurrentList += Instances[Instances.Keys.ElementAt(i)].Count;
        }
        

        InstanceCount.text = (ChunkIndex+elementsBeforeCurrentList)+ "/" + totalCount;

        ClearTaskList();

        for(int i=0;i<6;i++)
        {
            if(i < Instances.Keys.Count)
            {
                GameObject tempTaskLine = ResourcesLoader.Instance.GetRecycledObject("ActiveTaskLineUI");
                tempTaskLine.transform.SetParent(TaskListContainer, false);
                tempTaskLine.transform.localScale = Vector3.one;
                tempTaskLine.GetComponent<ActiveTaskLineUI>().SetInfo(Instances[Instances.Keys.ElementAt(i)]);
            }
            else
            {
                break;
            }
        }
    }

    void ClearTaskList()
    {
        while(TaskListContainer.childCount > 0)
        {
            TaskListContainer.GetChild(0).gameObject.SetActive(false);
            TaskListContainer.GetChild(0).SetParent(transform);
        }
    }

    public void NavigateRight()
    {
        ChunkIndex++;
        if(ChunkIndex >= Instances[Instances.Keys.ElementAt(ListIndex)].Count)
        {
            ListIndex++;
            if(ListIndex >= Instances.Keys.Count)
            {
                ListIndex = 0;
            }

            ChunkIndex = 0;
        }

        Refresh();
    }

    public void NavigateLeft()
    {
        ChunkIndex--;

        if (ChunkIndex < 0)
        {
            ListIndex--;
            if (ListIndex < 0)
            {
                ListIndex = Instances.Keys.Count - 1;
            }

            ChunkIndex = Instances[Instances.Keys.ElementAt(ListIndex)].Count - 1;
        }

        Refresh();
    }
}
