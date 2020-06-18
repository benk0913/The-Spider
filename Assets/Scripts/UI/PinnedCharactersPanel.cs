using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinnedCharactersPanel : MonoBehaviour
{
    public static PinnedCharactersPanel Instance;

    [SerializeField]
    GameObject ContentPanel;

    [SerializeField]
    Transform Container;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PassTimeComplete", Refresh);
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
        yield return new WaitForSeconds(1f);

        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).transform.SetParent(transform);

            yield return 0;
        }

        while(CORE.Instance.isLoading)
        {
            yield return 0;
        }

        bool empty = true;
        int counter = 0;
        foreach (Character character in CORE.Instance.Characters)
        {
            if(character.Pinned)
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

            counter++;

            if (counter % 5 == 0)
            {
                yield return 0;
            }
        }

        if(empty)
        {
            ContentPanel.gameObject.SetActive(false);
        }

        RefreshRoutineInstance = null;
    }
}
