using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeditateEntity : MonoBehaviour
{
    public void Meditate()
    {
        MeditatePanelUI.Instance.Show();
    }

    public void Demedittate()
    {
        MeditatePanelUI.Instance.AnimateExit();
    }
}
