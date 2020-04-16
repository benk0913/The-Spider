using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QRResearchComplete", menuName = "DataObjects/Quests/Rewards/QRResearchComplete", order = 2)]
public class QRResearchComplete : QuestReward
{
    public TechTreeItem Item;

    public override void Claim(Character byCharacter)
    {
        base.Claim(byCharacter);

        if(byCharacter.TopEmployer == CORE.PC)
        {
            TechTreeItem item = CORE.Instance.TechTree.Find(x => x.name == Item.name);

            if (item == null)
            {
                return;
            }

            item.Research();
            TechNodeTreeUI.Instance.RefreshNodes();
            CORE.Instance.InvokeEvent("ResearchComplete");
        }
    }
}