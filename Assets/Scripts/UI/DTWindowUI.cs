using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DTWindowUI : MonoBehaviour
{
    public static DTWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI OriginalText;

    [SerializeField]
    TextMeshProUGUI KeywordText;

    [SerializeField]
    TMP_InputField InputArea;

    [SerializeField]
    Transform ColumnContainer;

    public string abcIndex = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public string OriginalInput;
    public string EncryptedText;
    public string OriginalKeyword;
    public int SplitsNumber = 1;
    

    /// <summary>
    /// DEBUG
    public string messageTEST;
    public string KEYWORDTEST;
    public bool Test;
    public char KeyHoveredLetter;
    public int KeySelectedIndex;

    public int ChunkSize
    {
        get
        {
            if(SplitsNumber == 0)
            {
                return EncryptedText.Length;
            }
            return EncryptedText.Length / SplitsNumber;
        }
    }

    List<char> SortedKeywordLetters = new List<char>();
    /// </summary>

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.UnfocusOnItemInHands();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }

        if(Test)
        {
            Show(messageTEST, KEYWORDTEST, null);
            Test = false;
        }
    }

    public void Show(string encryptedMessage, string keyword, System.Action OnResolve = null)
    {
        this.OriginalInput = encryptedMessage;

        this.OriginalKeyword = keyword;

        SortedKeywordLetters.Clear();
        foreach (char keywordLetter in OriginalKeyword)
        {
            SortedKeywordLetters.Add(keywordLetter);
        }

        SortedKeywordLetters = SortedKeywordLetters.OrderBy(x => abcIndex.IndexOf(x)).ToList();

        this.EncryptedText = Util.DTEncipher(OriginalInput, OriginalKeyword, '-');

        this.gameObject.SetActive(true);

        //Remove comment later... TODO
        //MouseLook.Instance.FocusOnItemInHands();
        
        SplitsNumber = 1;
        RefreshSplitAmount();
    }

    public void RefreshSplitAmount()
    {
        if(string.IsNullOrEmpty(OriginalInput) || string.IsNullOrEmpty(OriginalKeyword))
        {
            return;
        }

        if (string.IsNullOrEmpty(InputArea.text))
        {
            InputArea.text = "2";
        }

        SplitsNumber = int.Parse(InputArea.text);

        RefreshUI();
    }
    
    void RefreshUI()
    {
        if(RefreshUIRoutineInstance != null)
        {
            StopCoroutine(RefreshUIRoutineInstance);
        }
             
        RefreshUIRoutineInstance = StartCoroutine(RefreshUIRoutine());
    }

    Coroutine RefreshUIRoutineInstance;
    IEnumerator RefreshUIRoutine()
    {
        ClearTable();
        OriginalText.text = EncryptedText;
        KeywordText.text = OriginalKeyword;

        for (int i = 0; i < SplitsNumber; i++)
        {
            GameObject columnObj = ResourcesLoader.Instance.GetRecycledObject("DTColumn");
            columnObj.transform.SetParent(ColumnContainer, false);
            columnObj.transform.localScale = Vector3.one;
            columnObj.transform.position = Vector3.one;

            DTColumnUI column = columnObj.GetComponent<DTColumnUI>();
            
            int capturedIndex = new int();
            capturedIndex += i;
            column.OriginalIndex = capturedIndex;

            column.OnHover.RemoveAllListeners();
            column.OnHover.AddListener(()=>
            {
               
                HoverColumn(columnObj.transform);
            });

            column.OnClickDown.RemoveAllListeners();
            column.OnClickDown.AddListener(() =>
            {
                StartDragColumn(columnObj.transform);
            });

            yield return 0;

            string columnString = EncryptedText.Substring(i * ChunkSize, ChunkSize);
            for (int c = 0; c < columnString.Length; c++)
            {
                GameObject letterObj = ResourcesLoader.Instance.GetRecycledObject("DTLetter");
                letterObj.transform.SetParent(columnObj.transform, false);
                columnObj.transform.localScale = Vector3.one;
                columnObj.transform.position = Vector3.one;
                letterObj.GetComponent<TextMeshProUGUI>().text = columnString[c].ToString();

                yield return 0;
            }

        }

        yield return 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(ColumnContainer.GetComponent<RectTransform>());


        RefreshUIRoutineInstance = null;
    }

    void ClearTable()
    {
        while (ColumnContainer.childCount > 0)
        {
            while (ColumnContainer.GetChild(0).childCount > 0)
            {
                ColumnContainer.GetChild(0).GetChild(0).gameObject.SetActive(false);
                ColumnContainer.GetChild(0).GetChild(0).SetParent(transform);
            }

            ColumnContainer.GetChild(0).gameObject.SetActive(false);
            ColumnContainer.GetChild(0).SetParent(transform);
        }
    }

    public void HoverLetterSentence(char character, int index)
    {
        //Max lenght / index - ceil = index in keyword
        int chunkIndex = (index - (index % ChunkSize)) / ChunkSize;
        char keywordLetter = SortedKeywordLetters[chunkIndex];

        HoverLetterKeyword(keywordLetter, OriginalKeyword.IndexOf(keywordLetter));
    }

    public void HoverColumn(Transform columnTransform)
    {
        char keywordLetter = SortedKeywordLetters[columnTransform.GetComponent<DTColumnUI>().OriginalIndex];

        HoverLetterKeyword(keywordLetter, OriginalKeyword.IndexOf(keywordLetter));
    }

    public void HoverLetterKeyword(char character, int index)
    {
        KeyHoveredLetter = character;
        KeySelectedIndex = index;

        string originalText;
        string startTag = "<color=blue>";
        string endTag = "</color>";

        KeywordText.text = OriginalKeyword.Insert(KeySelectedIndex+1, endTag).Insert(KeySelectedIndex, startTag);

        int chunkIndex = SortedKeywordLetters.IndexOf(KeyHoveredLetter);

        if (chunkIndex > SplitsNumber)
        {
            OriginalText.text = EncryptedText;
        }
        else
        {
            OriginalText.text = EncryptedText.Insert((chunkIndex * ChunkSize) + ChunkSize, endTag).Insert((chunkIndex * ChunkSize), startTag);
        }

        if (DragColumnRoutineInstance == null)
        {
            for (int i = 0; i < ColumnContainer.childCount; i++)
            {
                if (ColumnContainer.GetChild(i).GetComponent<DTColumnUI>().OriginalIndex == chunkIndex)
                {
                    ColumnContainer.GetChild(i).GetComponent<_2dxFX_Outline>().enabled = true;
                    continue;
                }

                ColumnContainer.GetChild(i).GetComponent<_2dxFX_Outline>().enabled = false;
            }
        }
    }

    public void StartDragColumn(Transform columnTrans)
    {
        if(DragColumnRoutineInstance != null)
        {
            if(CurrentlyDRAGGED != null)
            {
                CurrentlyDRAGGED.GetComponent<_2dxFX_Outline>().enabled = false;
            }
            StopCoroutine(DragColumnRoutineInstance);
        }

        DragColumnRoutineInstance = StartCoroutine(DragColumnRoutine(columnTrans));
    }

    Coroutine DragColumnRoutineInstance;
    Transform CurrentlyDRAGGED;
    IEnumerator DragColumnRoutine(Transform columnTrans)
    {
        CurrentlyDRAGGED = columnTrans;
        columnTrans.GetComponent<_2dxFX_Outline>().enabled = true;
        while (Input.GetMouseButton(0))
        {
            if(Input.GetAxis("Mouse X") > 0.5f)
            {
                RepositionColumnToRight(columnTrans);
                yield return new WaitForSeconds(0.1f);
            }
            else if (Input.GetAxis("Mouse X") < -0.5f)
            {
                RepositionColumnToLeft(columnTrans);
                yield return new WaitForSeconds(0.1f);
            }

            yield return 0;
        }

        columnTrans.GetComponent<_2dxFX_Outline>().enabled = false;
        DragColumnRoutineInstance = null;
    }

    public void RepositionColumnToLeft(Transform columnTrans)
    {
        int siblingIndex = columnTrans.GetSiblingIndex();
        if (siblingIndex <= 0)
        {
            return;
        }
        Debug.LogError("REACH " + siblingIndex );

        columnTrans.SetSiblingIndex(siblingIndex-1);
    }

    public void RepositionColumnToRight(Transform columnTrans)
    {
        int siblingIndex = columnTrans.GetSiblingIndex();
        if (siblingIndex+1 >= columnTrans.parent.childCount)
        {
            return;
        }
        Debug.LogError("REACH " + siblingIndex);

        columnTrans.SetSiblingIndex(siblingIndex+1);
    }

    public void ResolvePuzzle()
    {
        this.gameObject.SetActive(false);

        CORE.Instance.InvokeEvent("Letter Deciphered");
    }
}
