using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseInWierdRoomsEntity : MonoBehaviour
{
    public bool ShowingWindow = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(ShowingWindow)
            {
                return;
            }

            MouseLook.Instance.State = MouseLook.ActorState.Focusing;
            ShowingWindow = true;
            WarningWindowUI.Instance.Show("Quit to the main menu?", () =>
            {
                CORE.Instance.DisposeCurrentGame();
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                ShowingWindow = false;
            },false,()=> { CORE.Instance.DelayedInvokation(1f, () => { ShowingWindow = false; }); MouseLook.Instance.State = MouseLook.ActorState.Idle; });
        }
    }
}
