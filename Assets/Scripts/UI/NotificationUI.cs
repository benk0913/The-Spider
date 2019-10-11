using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [SerializeField]
    protected GameObject NotificationPanel;

    [SerializeField]
    protected TextMeshProUGUI NotificationLabel;

    [SerializeField]
    protected Animator Anim;

    public int Amount = 0;

    private void Awake()
    {
        if(Amount <= 0)
        {
            Hide();
        }
    }

    public void Add(int amount)
    {
        this.Amount += amount;

        if (amount > 0)
        {
            Show();

            NotificationLabel.text = Amount.ToString();

            Anim.SetTrigger("Pop");
        }
        else
        {
            Hide();
            this.Amount = 0;
        }
    }

    public void Wipe()
    {
        Amount = 0;

        Hide();
    }

    protected void Show()
    {
        if (NotificationPanel.activeInHierarchy)
        {
            return;
        }

        NotificationPanel.SetActive(true);
    }

    protected void Hide()
    {
        if (!NotificationPanel.activeInHierarchy)
        {
            return;
        }

        NotificationPanel.SetActive(false);
    }
}
