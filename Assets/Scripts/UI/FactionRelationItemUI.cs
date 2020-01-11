using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FactionRelationItemUI : MonoBehaviour
{
    public FactionPortraitUI OfFactionPortrait;
    public FactionPortraitUI WithFactionPortrait;

    [SerializeField]
    TextMeshProUGUI RelationValue;

    public void SetInfo(Faction ofFaction, Faction withFaction)
    {
        OfFactionPortrait.SetInfo(ofFaction);
        WithFactionPortrait.SetInfo(withFaction);

        if(ofFaction.FactionHead == null || withFaction.FactionHead == null)
        {
            RelationValue.text = "--";
            return;
        }

        int relation = ofFaction.Relations.GetRelations(withFaction).TotalValue;

        RelationValue.text = relation.ToString();

        if (relation > 3)
        {
            RelationValue.color = Color.green;
        }
        else if (relation < -3)
        {
            RelationValue.color = Color.red;
        }
        else
        {
            RelationValue.color = Color.yellow;
        }
    }

    public void OnClick()
    {
        FactionRelationsWindowUI.Instance.Show(WithFactionPortrait.CurrentFaction);
    }
}
