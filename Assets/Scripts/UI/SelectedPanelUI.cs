using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedPanelUI : MonoBehaviour
{
    public static SelectedPanelUI Instance;

    public SelectionType CurrentSelectionType
    {
        set
        {
            _currentSelectionType = value;

            switch(value)
            {
                case SelectionType.None:
                    {
                        this.gameObject.SetActive(false);
                        break;
                    }
                case SelectionType.Agent:
                    {
                        this.gameObject.SetActive(true);
                        CharacterPanel.Show();
                        LocationPanel.Hide();
                        PurchasePlotPanel.Hide();
                        break;
                    }
                case SelectionType.Location:
                    {
                        this.gameObject.SetActive(true);
                        CharacterPanel.Hide();
                        LocationPanel.Show();
                        PurchasePlotPanel.Hide();
                        break;
                    }
                case SelectionType.PurchasablePlot:
                    {
                        this.gameObject.SetActive(true);
                        CharacterPanel.Hide();
                        LocationPanel.Hide();
                        PurchasePlotPanel.Show();
                        break;
                    }
            }
        }
        get
        {
            return _currentSelectionType;
        }
    }
    SelectionType _currentSelectionType;

    [SerializeField]
    public ControlCharacterPanelUI CharacterPanel;

    [SerializeField]
    public ControlLocationPanelUI LocationPanel;

    [SerializeField]
    public PurchasePlotPanelUI PurchasePlotPanel;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void SetSelected(Character character)
    {
        if (character.Employer == null)
        {
            return;
        }

        if (character.TopEmployer != CORE.PC)
        {
            return;
        }

        if (character != null)
        {
            Deselect();
        }

        CharacterPanel.SetInfo(character);

        CurrentSelectionType = SelectionType.Agent;
    }

    public void SetSelected(LocationEntity location)
    {
        LocationPanel.Select(location);

        CurrentSelectionType = SelectionType.Location;
    }

    public void SetSelected(PurchasableEntity plot)
    {
        PurchasePlotPanel.Select(plot);

        CurrentSelectionType = SelectionType.PurchasablePlot;
    }

    public void Deselect()
    {
        this.gameObject.SetActive(false);

        CurrentSelectionType = SelectionType.None;
    }



    public enum SelectionType
    {
        None,
        Agent,
        Location,
        PurchasablePlot
    }
}
