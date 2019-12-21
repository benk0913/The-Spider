using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickableTraitUI : TraitUI
{
    public void SetTrait()
    {
        ChangePerkWindowUI.Instance.SelectTrait(CurrentTrait);
    }
    
}
