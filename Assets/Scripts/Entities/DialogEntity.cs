using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

//TODO Make normal
public class DialogEntity : MonoBehaviour, ISaveFileCompatible
{
    public static DialogEntity Instance;

    public DialogPiece CurrentDialog;

    public UnityEvent OnInteract;

    public UnityEvent OnDialogFinished;

    public bool wasInteracted = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CORE.Instance.SubscribeToEvent("DialogClosed", () => 
        {
            if (wasInteracted)
            {
                OnDialogFinished?.Invoke(); wasInteracted = false;
            }
        });
    }

    public void SetDialog(DialogPiece dialog)
    {
        this.CurrentDialog = dialog;
    }

    public void Interact()
    {
        if(CurrentDialog == null)
        {
            GlobalMessagePrompterUI.Instance.Show("You have no reason to go anywhere...", 1f, Color.yellow);
            return;
        }

        wasInteracted = true;

        OnInteract.Invoke();

        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("Actor", CORE.PC);
        parameters.Add("ActorName", CORE.PC.name);
        parameters.Add("Location", CORE.PC.CurrentLocation);
        parameters.Add("LocationName", CORE.PC.CurrentLocation.Name);
        parameters.Add("Target", null);
        parameters.Add("TargetName", "");

        DialogWindowUI.Instance.StartNewDialog(CurrentDialog, parameters);

        CurrentDialog = null;
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        if(CurrentDialog == null)
        {
            node["CurrentDialog"] = "";
            return node;
        }

        node["CurrentDialog"] = CurrentDialog.name;

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        if(string.IsNullOrEmpty(node["CurrentDialog"]))
        {
            return;
        }

        CurrentDialog = CORE.Instance.Database.AllDialogPieces.Find(x => x.name == node["CurrentDialog"]);
    }

    public void ImplementIDs()
    {
        
    }
}
