using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEntity : MonoBehaviour
{
    public void SetTutorial(TutorialScreenInstance Tutorial)
    {
        TutorialScreenUI.Instance.Show(Tutorial.name);
    }
}
