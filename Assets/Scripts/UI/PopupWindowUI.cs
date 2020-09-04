using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindowUI : MonoBehaviour, ISaveFileCompatible
{
    public static PopupWindowUI Instance;

    public List<PopupData> PopupQue = new List<PopupData>();

    public PopupData CurrentPopup;


    [SerializeField]
    TextMeshProUGUI Description;

    [SerializeField]
    Image SceneImage;



    [SerializeField]
    Transform LeftPortraitsContainer;

    [SerializeField]
    Transform RightPortraitsContainer;


    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ShowNextPopup();
        }
    }

    public void AddPopup(PopupData popup)
    {
        if(CurrentPopup != null)
        {
            PopupQue.Add(popup);
            return;
        }

        ShowPopup(popup);
    }

    void ShowPopup(PopupData data)
    {
        if(data == null || data.Preset == null)
        {
            return;
        }

        MapViewManager.Instance.ForceInteractWithMap();

        MouseLook.Instance.CurrentWindow = this.gameObject;

        this.gameObject.SetActive(true);

        Dictionary<string, object> parameters = new Dictionary<string, object>();

        GenderType GenderActor = GenderType.Male;
        if (data.CharactersLeft != null && data.CharactersLeft.Count > 0)
        {
            parameters.Add("ActorName", data.CharactersLeft[0].name);
            GenderActor = data.CharactersLeft[0].Gender;
        }

        if(data.CharactersRight != null && data.CharactersRight.Count > 0)
        {
            if (data.CharactersRight[0].IsKnown("Name", CORE.PC))
            {
                parameters.Add("TargetName", data.CharactersRight[0].name);
            }
            else
            {
                if (!string.IsNullOrEmpty(data.CharactersRight[0].CurrentRole))
                {
                    parameters.Add("TargetName", "the " + data.CharactersRight[0].CurrentRole);
                }
                else
                {
                    parameters.Add("TargetName", "the stranger");
                }
            }
        }

        if(data.Parameters != null && data.Parameters.Count > 0)
        {
            for(int i=0;i<data.Parameters.Keys.Count;i++)
            {
                if(parameters.ContainsKey(data.Parameters.Keys.ElementAt(i)))
                {
                    parameters[data.Parameters.Keys.ElementAt(i)] = data.Parameters[data.Parameters.Keys.ElementAt(i)];
                    continue;
                }

                parameters.Add(data.Parameters.Keys.ElementAt(i), data.Parameters[data.Parameters.Keys.ElementAt(i)]);
            }
        }

        CurrentPopup = data;

        Description.text = Util.FormatTags(data.Preset.Description, parameters, GenderActor);
        SceneImage.sprite = data.Preset.Image;

        ClearContainers();

        if (data.CharactersLeft != null)
        {
            CORE.Instance.DelayedInvokation(0.2f, () =>
            {
                foreach (Character character in data.CharactersLeft)
                {
                
                    GameObject portraitObj = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
                    portraitObj.transform.SetParent(LeftPortraitsContainer, false);
                    portraitObj.transform.localScale = Vector3.one;
                    portraitObj.transform.position = Vector3.zero;
                    portraitObj.GetComponent<PortraitUI>().SetCharacter(character);
                }
            });
        }

        if (data.CharactersRight != null)
        {
            CORE.Instance.DelayedInvokation(0.3f, () =>
            {
                foreach (Character character in data.CharactersRight)
                {
                    GameObject portraitObj = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
                    portraitObj.transform.SetParent(RightPortraitsContainer, false);
                    portraitObj.transform.localScale = Vector3.one;
                    portraitObj.transform.position = Vector3.zero;
                    portraitObj.GetComponent<PortraitUI>().SetCharacter(character);
                }
            });
        }

        
    }

    public void HideCurrentPopup()
    {
        MouseLook.Instance.CurrentWindow = null;
        CurrentPopup = null;
        this.gameObject.SetActive(false);
    }

    public void ShowNextPopup()
    {
        if(CurrentPopup != null)
        {
            CurrentPopup.OnPopupDisplayed?.Invoke();
            CurrentPopup.Preset.OnOpenAction?.Activate();
        }

        if(PopupQue.Count == 0)
        {
            HideCurrentPopup();
            return;
        }

        CurrentPopup = null;
        ShowPopup(PopupQue[0]);
        PopupQue.RemoveAt(0);
    }

    void ClearContainers()
    {
        while(LeftPortraitsContainer.childCount > 0)
        {
            LeftPortraitsContainer.GetChild(0).gameObject.SetActive(false);
            LeftPortraitsContainer.GetChild(0).SetParent(transform);
        }

        while (RightPortraitsContainer.childCount > 0)
        {
            RightPortraitsContainer.GetChild(0).gameObject.SetActive(false);
            RightPortraitsContainer.GetChild(0).SetParent(transform);
        }
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();
        node["CurrentPopup"] = CurrentPopup.ToJSON();
        for(int i=0;i<PopupQue.Count;i++)
        {
            node["Que"][i] = PopupQue[i].ToJSON();
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        for(int i=0;i<node["Que"].Count;i++)
        {
            PopupData data = new PopupData();
            data.FromJSON(node["Que"][i]);
            PopupQue.Add(data);
        }

        if (node["CurrentPopup"] != null)
        {
            PopupData data = new PopupData();
            data.FromJSON(node["CurrentPopup"]);
            ShowPopup(CurrentPopup); 
        }
    }

    public void ImplementIDs()
    {
        throw new System.NotImplementedException();
    }
}

public class PopupData : ISaveFileCompatible
{
    public PopupDataPreset Preset;
    public List<Character> CharactersLeft;
    public List<Character> CharactersRight;
    public Action OnPopupDisplayed;
    public Action OnPopupClosed;
    public Dictionary<string, string> Parameters;

    public PopupData(PopupDataPreset preset = null, List<Character> charactersLeft = null, List<Character> charactersRight = null, Action onPopupDisplay = null,Dictionary<string,string> parameters = null)
    {
        this.Preset = preset;
        this.CharactersLeft = charactersLeft;
        this.CharactersRight = charactersRight;
        this.OnPopupDisplayed = onPopupDisplay;
        this.Parameters = parameters;
    }

    public void FromJSON(JSONNode node)
    {
        this.Preset = CORE.Instance.Database.GetPopupPreset(node["Preset"]);
        
        for(int i=0;i< node["LeftCharactersIDs"].Count;i++)
        {
            leftCharsIDS.Add(node["LeftCharactersIDs"][i]);
        }

        for (int i = 0; i < node["RightCharactersIDs"].Count; i++)
        {
            leftCharsIDS.Add(node["RightCharactersIDs"][i]);
        }

        if(node["Parameters"]!=null&& node["Parameters"].Count > 0)
        {
            Parameters = new Dictionary<string, string>();

            for(int i=0;i< node["Parameters"].Count;i++)
            {
                Parameters.Add(node["Parameters"][i]["Key"].Value, node["Parameters"][i]["Value"].Value);
            }
        }
    }

    List<string> leftCharsIDS = new List<string>();
    List<string> rightCharsIDS = new List<string>();
    public void ImplementIDs()
    {
        CharactersLeft.Clear();
        CharactersRight.Clear();

        foreach(string charID in leftCharsIDS)
        {
            CharactersLeft.Add(CORE.Instance.Characters.Find(x => x.ID == charID));
        }

        foreach (string charID in rightCharsIDS)
        {
            CharactersRight.Add(CORE.Instance.Characters.Find(x => x.ID == charID));
        }
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["Preset"] = Preset.name;
        for(int i=0;i<CharactersLeft.Count;i++)
        {
            node["LeftCharactersIDs"][i] = CharactersLeft[i].ID;
        }

        for (int i = 0; i < CharactersRight.Count; i++)
        {
            node["RightCharactersIDs"][i] = CharactersRight[i].ID;
        }

        if(Parameters != null)
        {
            for(int i=0;i<Parameters.Keys.Count;i++)
            {
                node["Parameters"][i]["Key"] = Parameters.Keys.ElementAt(i);
                node["Parameters"][i]["Value"] = Parameters[Parameters.Keys.ElementAt(i)];
            }
        }
        return node;
    }
}
