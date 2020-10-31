using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelEntity : MonoBehaviour
{
    public string LevelName;
    public bool StopMusic;
    public bool HideObjectives;
    
    public void LoadLevel()
    {
        StopAllCoroutines();
        CORE.Instance.StartCoroutine(LoadLevelRoutine());
    }

    IEnumerator LoadLevelRoutine()
    {

        CORE.Instance.FadeOutScreen();

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(LevelName);


        yield return 0;
        while (SceneManager.GetActiveScene().name != LevelName)
        {
            yield return 0;
        }

        yield return new WaitForSeconds(1f);
        if (StopMusic)
        {
            AudioControl.Instance.StopAllCoroutines();
            AudioControl.Instance.SetPlaylist(0);
            AudioControl.Instance.SetPlaylistIndex(0);
        }

        if (HideObjectives)
        {
            WorldMissionPanelUI.Instance.FoldedPanel.gameObject.SetActive(false);
            WorldMissionPanelUI.Instance.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        CORE.Instance.FadeInScreen();
    }

    public void LoadLevel(string levelName)
    {
        this.LevelName = levelName;
        LoadLevel();
    }
}
