using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecificBookDisplayUI : MonoBehaviour
{
    [SerializeField]
    Image CentralImage;

    [SerializeField]
    GameObject LeftArrow;

    [SerializeField]
    GameObject RightArrow;

    SpecificBook CurrentBook;

    public int CurrentPage = 0;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
    }

    public void Set(SpecificBook book)
    {
        CurrentBook = book;
        CurrentPage = 0;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if(CurrentBook == null)
        {
            return;
        }

        if(CurrentBook.Pages.Count <= CurrentPage)
        {
            CurrentPage = 0;
        }

        CentralImage.sprite = CurrentBook.Pages[CurrentPage];

        RightArrow.SetActive(CurrentPage < CurrentBook.Pages.Count - 1);

        LeftArrow.SetActive(CurrentPage > 0);
    }

    public void MoveRight()
    {
        CurrentPage++;
        if(CurrentPage >= CurrentBook.Pages.Count)
        {
            CurrentPage = CurrentBook.Pages.Count - 1;
        }

        RefreshUI();
    }

    public void MoveLeft()
    {
        CurrentPage--;
        if (CurrentPage < 0)
        {
            CurrentPage = 0;
        }

        RefreshUI();
    }
}
