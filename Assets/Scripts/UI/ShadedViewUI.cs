using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadedViewUI : MonoBehaviour
{
    public static ShadedViewUI Instance;

    [SerializeField]
    List<ShadeState> ShadeRate = new List<ShadeState>();

    [SerializeField]
    Image ShadeImage;

    public string lastAudio;
    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(int rate = 0)
    {
        if (rate >= ShadeRate.Count || rate < 0)
        {
            rate = 0;
        }

        ShadeImage.sprite = ShadeRate[rate].StateSprite;

        ShadeImage.enabled = true;

        if (ShadeRate[rate].StateAudio != lastAudio)
        {
            StopAllSounds();
            AudioControl.Instance.Play(ShadeRate[rate].StateAudio, true);
            lastAudio = ShadeRate[rate].StateAudio;
        }

        if (rate > 0)
        {
            TutorialScreenUI.Instance.Show("Voices1");
        }
    }

    public void Hide()
    {
        ShadeImage.enabled = false;
        lastAudio = "";
        StopAllSounds();
    }

    public void StopAllSounds()
    {
        foreach(ShadeState state in ShadeRate)
        {
            AudioControl.Instance.StopSound(state.StateAudio);
        }
    }

    [System.Serializable]
    public class ShadeState
    {
        public Sprite StateSprite;
        public string StateAudio;
    }
}
