using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TechNodeTreeUI : MonoBehaviour
{
    public static TechNodeTreeUI Instance;

    [SerializeField]
    ScrollRect CurrentScrollRect;

    [SerializeField]
    public GameObject FirstNode;

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    TextMeshProUGUI ProgressLabel;

    public List<TechTreeItemUI> TechTreeItems = new List<TechTreeItemUI>();

    public float MinScale = 0.5f;
    public float MaxScale = 1f;


    public bool IsHidden = false;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide()
    {
        if (AudioControl.Instance != null)
        {
            AudioControl.Instance.StopSound("soundscape_research_tech");
            AudioControl.Instance.UnmuteMusic();
        }

        CG.alpha = 0f;
        CG.interactable = false;
        CG.blocksRaycasts = false;
        IsHidden = true;

        if(CORE.Instance != null)
        CORE.Instance.UnoccupyFocusView(this);

        this.gameObject.SetActive(false);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0f && FirstNode.transform.localScale.x < MaxScale)
        {
            FirstNode.transform.localScale += Vector3.one *2f* Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && FirstNode.transform.localScale.x > MinScale)
        {
            FirstNode.transform.localScale -= Vector3.one * 2f * Time.deltaTime;
        }

        ProgressLabel.text = System.String.Format("{0:n0}", CORE.PC.CProgress);
    }

    public void Reveal()
    {
        CG.alpha = 1f;
        CG.interactable = true;
        CG.blocksRaycasts = true;

        this.gameObject.SetActive(true);
        
        foreach(TechTreeItemUI item in TechTreeItems)
        {
            item.RefreshUI();
        }

        IsHidden = false;

        if(CORE.Instance != null)
        {
            CORE.Instance.OccupyFocusView(this);
        }

    }

    public void Show()
    {
        
        if(AudioControl.Instance != null)
        {
            AudioControl.Instance.Play("soundscape_research_tech", true);
            AudioControl.Instance.MuteMusic();
        }

        Reveal();


        FirstNode.transform.localScale = Vector3.one * MinScale;
    }

    public void RefreshNodes()
    {
        if(this.CG.interactable == false)
        {
            return;
        }

        foreach(TechTreeItemUI item in TechTreeItems)
        {
            item.RefreshUI();
        }
    }
}


