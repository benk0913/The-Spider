using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class AgentActionLogListUI : MonoBehaviour
{
    public static AgentActionLogListUI Instance;

    [SerializeField]
    Transform Container;



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //CORE.Instance.SubscribeToEvent("PassTimeComplete",OnTurnPassed);
        CORE.Instance.SubscribeToEvent("PassTime", Refresh);
        CORE.Instance.SubscribeToEvent("AgentRefreshedAction", Refresh);
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if(CORE.Instance.TurnPassedRoutineInstance != null)
        {
            return;
        }

        ClearContainer();

        List<Character> Characters = CORE.PC.CharactersInCommand;

        Characters.RemoveAll(x => !x.IsAgent);

        Characters = Characters.OrderByDescending(x => x.CurrentTaskEntity != null).ToList();

        foreach (Character character in Characters)
        {
            GameObject instancePanel = ResourcesLoader.Instance.GetRecycledObject("AgentActionLogUI");
            instancePanel.transform.SetParent(Container, false);
            instancePanel.GetComponent<AgentActionLogUI>().SetInfo(character, character.CurrentTaskEntity);
        }
    }



    void ClearContainer()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }
    }


}
