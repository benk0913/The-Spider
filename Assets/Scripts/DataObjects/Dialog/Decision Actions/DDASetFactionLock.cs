using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDASetFactionLock", menuName = "DataObjects/Dialog/Actions/DDASetFactionLock", order = 2)]
public class DDASetFactionLock : DialogDecisionAction
{
    public Faction faction;
    public bool IsLocked = false;
    public PopupDataPreset PopupPreset;

    public override void Activate()
    {
        if (PlayerPrefs.GetString(faction.name + "LOCK", faction.isLockedByDefault.ToString()) == IsLocked.ToString())
        {
            if (CORE.Instance.DEBUG)
            {
                Debug.Log("TRIED TO SET FACTION LOCK - ALREADY THE SAME STATE.");
            }
            return;
        }

        CORE.Instance.Database.Factions.Find(X => X.name == faction.name).isLocked = IsLocked;
        CORE.Instance.Factions.Find(X => X.name == faction.name).isLocked = IsLocked;
        PlayerPrefs.SetString(faction.name + "LOCK", IsLocked.ToString());

        CORE.Instance.DelayedInvokation(3f, () => 
        {
            if (PopupPreset != null)
            {
                PopupData data = new PopupData(PopupPreset, null, null, () => { });
                PopupWindowUI.Instance.AddPopup(data);
            }
        });
    }
}
