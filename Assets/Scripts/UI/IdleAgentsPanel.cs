using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAgentsPanel : MonoBehaviour
{
    public static IdleAgentsPanel Instance;

    [SerializeField]
    GameObject ContentPanel;

    [SerializeField]
    Transform Container;

    private void Awake()
    {
        Instance = this;
        ContentPanel.SetActive(false);
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("AgentRefreshedAction", OnAgentChangedAction);
        Refresh();
    }

    void OnAgentChangedAction()
    {
        Refresh();
    }

    void Refresh()
    {
        if(RefreshRoutineInstance != null)
        {
            StopCoroutine(RefreshRoutineInstance);
        }

        RefreshRoutineInstance = StartCoroutine(RefreshRoutine());
    }

    Coroutine RefreshRoutineInstance;
    IEnumerator RefreshRoutine()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).transform.SetParent(transform);

            yield return 0;
        }

        bool empty = true;
        foreach (Character character in CORE.Instance.Characters)
        {
            if(character.TopEmployer == CORE.PC && character != CORE.PC && character.CurrentTaskEntity == null)
            {
                PortraitUI portrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
                portrait.SetCharacter(character);
                portrait.transform.SetParent(Container, false);
                portrait.transform.localScale = Vector3.one;

                if (empty)
                {
                    ContentPanel.gameObject.SetActive(true);
                    empty = false;
                }
            }

            yield return 0;
        }

        if(empty)
        {
            ContentPanel.gameObject.SetActive(false);
        }

        RefreshRoutineInstance = null;
    }
}
