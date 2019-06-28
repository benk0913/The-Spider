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
                        break;
                    }
                case SelectionType.Location:
                    {
                        this.gameObject.SetActive(true);
                        CharacterPanel.Hide();
                        LocationPanel.Show();
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

    public void Deselect()
    {
        this.gameObject.SetActive(false);

        CurrentSelectionType = SelectionType.None;
    }



    public enum SelectionType
    {
        None,
        Agent,
        Location
    }
}
