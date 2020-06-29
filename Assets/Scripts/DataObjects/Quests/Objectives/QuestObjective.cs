using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

[CreateAssetMenu(fileName = "QuestObjective", menuName = "DataObjects/Quests/QuestObjective", order = 2)]
public class QuestObjective : ScriptableObject
{
    public Sprite Icon;

    public bool IsComplete = false;

    [System.NonSerialized]
    public Coroutine ValidateRoutine;

    [System.NonSerialized]
    public Quest ParentQuest;

    public bool RandomTarget;

    [System.NonSerialized]
    public LocationEntity TargetLocation;

    [System.NonSerialized]
    public Character TargetCharacter;

    [SerializeField]
    public bool ShowObjectiveMarker = false;

    protected GameObject MarkerTarget;
    protected GameObject MarkerObject;

    public List<QuestObjective> FailConditions = new List<QuestObjective>();

    public virtual QuestObjective CreateClone()
    {

        QuestObjective objective = Instantiate(this);
        objective.name = this.name;

        return objective;
    }

    public virtual bool Validate()
    {
        return true;
    }

    public virtual bool Failed()
    {

        foreach(QuestObjective failCondition in FailConditions)
        {
            if(failCondition.Validate())
            {
                if (CORE.Instance.DEBUG)
                    Debug.LogError("FAILED OBJECTIVE - " + this.name);

                return true;
            }
        }
        
        return false;
    }

    public virtual void ObjectiveFailed()
    {
        IsComplete = true;
        QuestsPanelUI.Instance.ObjectiveFailed(this);
    }

    public virtual void Complete()
    {
        IsComplete = true;
        QuestsPanelUI.Instance.ObjectiveComplete(this);

        if (MarkerObject != null && MarkerObject.activeInHierarchy)
        {
            MarkerObject.gameObject.SetActive(false);
        }
    }

    public virtual void RefreshMarker()
    {
        if(IsComplete)
        {
            if(MarkerObject != null && MarkerObject.activeInHierarchy)
            {
                MarkerObject.gameObject.SetActive(false);
            }

            return;
        }
        if (MarkerTarget == null)
        {
            MarkerTarget = GetMarkerTarget();

            if (MarkerTarget == null)
            {
                return;
            }
            
            if (MarkerObject != null)
            {
                MarkerObject.GetComponent<WorldPositionLerperUI>().SetTransform(MarkerTarget.transform);
            }
        }

        if (MarkerObject == null)
        {
            MarkerObject = ResourcesLoader.Instance.GetRecycledObject("MarkerWorld");
            MarkerObject.transform.SetParent(CORE.Instance.DisposableContainer);
            MarkerObject.transform.SetAsLastSibling();
            MarkerObject.GetComponent<WorldPositionLerperUI>().SetTransform(MarkerTarget.transform);
        }
        else
        {

            if (MarkerTarget.transform.parent != null && MarkerTarget.transform.parent.GetComponent<UILineRenderer>() != null) //PATCH- SPECIFIC HARDCODED CHECK FOR TECH TREE...
            {
                if (TechNodeTreeUI.Instance.IsHidden)
                {
                    if (MarkerObject.activeInHierarchy)
                        MarkerObject.SetActive(false);
                }
                else
                {
                    if (!MarkerObject.activeInHierarchy)
                        MarkerObject.SetActive(true);
                }

            }
            else
            {
                if (MarkerTarget.gameObject.activeInHierarchy)
                {
                    if (!MarkerObject.activeInHierarchy)
                    {
                        MarkerObject.gameObject.SetActive(true);
                    }
                }
                else
                {
                    MarkerObject.gameObject.SetActive(false);
                }
            }
        }
    }

    public virtual GameObject GetMarkerTarget()
    {
        return null;
    }
}