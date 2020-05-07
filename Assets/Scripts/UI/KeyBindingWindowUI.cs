using UnityEngine;
using System.Collections;
using System.Linq;

public class KeyBindingWindowUI : MonoBehaviour {

    public static KeyBindingWindowUI Instance;

    [SerializeField]
    Transform Container;

    KeyBindingPieceUI LastClickedPiece;

    void Awake()
    {
        Instance = this;
    }

	public void Open()
    {
        this.gameObject.SetActive(true);
        Clear();

        GameObject tempPiece;
        string key;
        for(int i=0;i<InputMap.Map.Count;i++)
        {
            tempPiece = ResourcesLoader.Instance.GetRecycledObject("KeyBindingPiece");
            tempPiece.transform.SetParent(Container, false);
            key = InputMap.Map.Keys.ElementAt(i);
            KeyBindingPieceUI KeyBindingPiece = tempPiece.GetComponent<KeyBindingPieceUI>();
            KeyBindingPiece.SetInfo(key, InputMap.Map[key]);
            KeyBindingPiece.m_btn.onClick.RemoveAllListeners();
            KeyBindingPiece.m_btn.onClick.AddListener(delegate {OnKeyBindingPieceClicked(KeyBindingPiece);});
        }
    }

    public void OnKeyBindingPieceClicked(KeyBindingPieceUI KeyBindingPiece)
    {
        bool IsPieceAlreadyActive = KeyBindingPiece.isWaitingForKey;
        if (LastClickedPiece != null)
        {
            LastClickedPiece.CloseBinding();
        }

        if (IsPieceAlreadyActive)
        {
            // keep the piece disabled and clear the last clicked piece
            LastClickedPiece = null;
        }
        else 
        {
            // save the last piece so it can be closed if another one is clicked
            LastClickedPiece = KeyBindingPiece;
            KeyBindingPiece.OnClick();
        }
    }

    private void Clear()
    {
        for(int i=0;i<Container.childCount;i++)
        {
            Container.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ResetMap()
    {
        WarningWindowUI.Instance.Show("Are you sure you wish to reset all of the Key Bindings you have previously made?", () =>
        {
            InputMap.Instance.ResetMapSave();
            Open();
        });
    }
}
