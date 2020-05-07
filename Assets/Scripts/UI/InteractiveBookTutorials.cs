using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveBookTutorials : MonoBehaviour
{
    [SerializeField]
    Transform TutorailsListContainer;

    private void OnEnable()
    {
        ClearContainer();

        foreach(TutorialScreenInstance instance in TutorialScreenUI.Instance.Instances)
        {
            if(!instance.WasSeen)
            {
                continue;
            }

            GameObject viewItem = ResourcesLoader.Instance.GetRecycledObject("TutorialListItem");

            viewItem.transform.SetParent(TutorailsListContainer, false);
            viewItem.transform.localScale = Vector3.one;

            viewItem.GetComponent<TextMeshProUGUI>().text = Regex.Replace(instance.name, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

            Button but = viewItem.GetComponent<Button>();
            but.onClick.RemoveAllListeners();
            but.onClick.AddListener(() => { instance.WasSeen = false; TutorialScreenUI.Instance.Show(instance.name,0f); });
        }
    }

    void ClearContainer()
    {
        while(TutorailsListContainer.childCount > 0)
        {
            TutorailsListContainer.GetChild(0).gameObject.SetActive(false);
            TutorailsListContainer.GetChild(0).transform.SetParent(transform);
        }
    }
}
