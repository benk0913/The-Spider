using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActiveTaskLineUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TextLabel;

    [SerializeField]
    Transform PortraitContainer;

    [SerializeField]
    ActionPortraitUI ActionPortrait;

    public void SetInfo(LongTermTaskEntity task)
    {
        TextLabel.text = task.CurrentTask.name;
        ActionPortrait.SetAction(task);

        ClearPortraits();
        GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
        tempPortrait.transform.SetParent(PortraitContainer, false);
        tempPortrait.transform.localScale = Vector3.one;
        tempPortrait.GetComponent<PortraitUI>().SetCharacter(task.CurrentCharacter, false);
    }

    public void SetInfo(List<LongTermTaskEntity> tasks)
    {
        if(tasks == null || tasks.Count == 0)
        {
            return;
        }

        TextLabel.text = tasks[0].CurrentTask.name;
        ActionPortrait.SetAction(tasks[0]);

        ClearPortraits();

        for (int i = 0; i < tasks.Count; i++)
        {
            if (!tasks[i].isKnownTask)
            {
                continue;
            }

            GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
            tempPortrait.transform.SetParent(PortraitContainer, false);
            tempPortrait.transform.localScale = Vector3.one;
            tempPortrait.GetComponent<PortraitUI>().SetCharacter(tasks[i].CurrentCharacter, false);
        }
    }



    void ClearPortraits()
    {
        while(PortraitContainer.childCount > 0)
        {
            PortraitContainer.GetChild(0).gameObject.SetActive(false);
            PortraitContainer.GetChild(0).SetParent(transform);
        }
    }
}
