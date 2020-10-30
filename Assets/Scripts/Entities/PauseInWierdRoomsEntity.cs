using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseInWierdRoomsEntity : MonoBehaviour
{
    public bool ShowingWindow = false;
    public FocusView focusView;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(ShowingWindow)
            {
                return;
            }

            focusView.Activate();
            ShowingWindow = true;
            WarningWindowUI.Instance.Show("Quit to the main menu?", () =>
            {
                CORE.Instance.DisposeCurrentGame();
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                ShowingWindow = false;
                focusView.Deactivate();
            },false,()=> { ShowingWindow = false; focusView.Deactivate(); });
        }
    }
}
