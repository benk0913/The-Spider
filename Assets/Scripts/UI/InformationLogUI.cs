using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class InformationLogUI : MonoBehaviour
{
    public static InformationLogUI Instance;

    public List<NewInformationInstance> InformationGatheredThisTurn = new List<NewInformationInstance>();

    [SerializeField]
    Transform Container;

    [SerializeField]
    public NotificationUI Notification;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //CORE.Instance.SubscribeToEvent("PassTimeComplete",OnTurnPassed);
        CORE.Instance.SubscribeToEvent("PassTime", OnTurnPassed);
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        ShowVisible();
    }

    public void OnTurnPassed()
    {
        InformationGatheredThisTurn.Clear();
        Notification.Wipe();

        if(this.gameObject.activeInHierarchy)
        {
            ClearContainer();
        }
    }

    public void AddInformationGathered(string information, Character aboutChar)
    {
        if(CORE.Instance.isLoading)
        {
            return;
        }

        AddInformationGathered(new NewInformationInstance(information, aboutChar));
    }

    public void AddInformationGathered(NewInformationInstance instance)
    {
        if (CORE.Instance.isLoading)
        {
            return;
        }

        InformationGatheredThisTurn.Add(instance);

        if (this.gameObject.activeInHierarchy)
        {
            AddInformationToContainer(instance);
        }
        else
        {
            Notification.Add(1);
        }
    }

    public void ShowVisible()
    {
        ClearContainer();
        Notification.Wipe();

        foreach (NewInformationInstance instance in InformationGatheredThisTurn)
        {
            AddInformationToContainer(instance);
        }
    }


    void AddInformationToContainer(NewInformationInstance instance)
    {
        GameObject instancePanel = ResourcesLoader.Instance.GetRecycledObject("InformationGatheredUI");
        instancePanel.transform.SetParent(Container, false);
        instancePanel.GetComponent<InformationGatheredUI>().SetInfo(instance);
    }

    void ClearContainer()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }
    }

    public class NewInformationInstance
    {
        public Character AboutCharacter;
        public string Information;
        
        public NewInformationInstance(string info, Character character)
        {
            this.AboutCharacter = character;
            this.Information = info;
        }
    }
}
