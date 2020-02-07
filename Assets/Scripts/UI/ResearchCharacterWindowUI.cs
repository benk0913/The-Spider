using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchCharacterWindowUI : MonoBehaviour
{
    public static ResearchCharacterWindowUI Instance;

    [SerializeField]
    public PortraitUI Portrait;

    [SerializeField]
    Transform UnlockablesContainer;

    [SerializeField]
    Transform RumorsContainer;

    public Character CurrentCharacter;

    public DragDroppableRumorUI CurrentDragged;
    public KnowledgeItemUnlockableUI CurrentHovered;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(CurrentDragged != null && CurrentHovered != null)
            {
                CurrentHovered.Consume(CurrentDragged);
            }
        }
    }

    public void Show(Character ofCharacter)
    {
        MouseLook.Instance.CurrentWindow = this.gameObject;
        this.gameObject.SetActive(true);

        CurrentCharacter = ofCharacter;
        RefreshUI();
    }

    void RefreshUI()
    {
        Portrait.SetCharacter(CurrentCharacter);

        ClearContainers();
        foreach (KnowledgeInstance item in CurrentCharacter.Known.Items)
        {
            GameObject tempItem = ResourcesLoader.Instance.GetRecycledObject("KnowledgeItemUnlockableUI");

            tempItem.transform.SetParent(UnlockablesContainer, false);
            tempItem.transform.localScale = Vector3.one;
            tempItem.GetComponent<KnowledgeItemUnlockableUI>().SetInfo(CurrentCharacter, item);
        }

        foreach(KnowledgeRumor rumor in CurrentCharacter.KnowledgeRumors)
        {
            GameObject tempItem = ResourcesLoader.Instance.GetRecycledObject("DragDroppableRumorUI");

            tempItem.transform.SetParent(RumorsContainer, false);
            tempItem.transform.localScale = Vector3.one;

            RectTransform rectTrans = RumorsContainer.GetComponent <RectTransform>();

            tempItem.transform.position = new Vector2(Random.Range(Screen.width/4f, Screen.width - Screen.width/4f ), Random.Range(Screen.height / 4f, Screen.height - Screen.height/ 4f));
            tempItem.GetComponent<DragDroppableRumorUI>().SetInfo(rumor);
        }

    }

    void ClearContainers()
    {
        while(UnlockablesContainer.childCount > 0)
        {
            UnlockablesContainer.GetChild(0).gameObject.SetActive(false);
            UnlockablesContainer.GetChild(0).SetParent(transform);
        }

        while (RumorsContainer.childCount > 0)
        {
            RumorsContainer.GetChild(0).gameObject.SetActive(false);
            RumorsContainer.GetChild(0).SetParent(transform);
        }

    }
}
