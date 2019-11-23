using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DEBUG_PersonalStatistics : MonoBehaviour
{
    public List<TurnReportSTATISTICS> turns = new List<TurnReportSTATISTICS>();
    public TurnReportSTATISTICS CurrentTurn;
    
    private void Start()
    {
        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);
        EditorApplication.playmodeStateChanged += ModeChanged;
    }

    private void Update()
    {
        if(CurrentTurn == null)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            CurrentTurn.Clicks++;
        }

        CurrentTurn.turnLength += Time.deltaTime;
    }

    void TurnPassed()
    {
        if(CurrentTurn != null)
        {
            turns.Add(CurrentTurn);
        }

        CurrentTurn = new TurnReportSTATISTICS();
    }

    private void ModeChanged()
    {
        float totalLength = 0f;
        float totalClicks = 0f;
        for(int i=0;i<turns.Count;i++)
        {
            totalLength += turns[i].turnLength;
            totalClicks += turns[i].Clicks;
            Debug.Log("####" + i + "_" + turns[i].turnLength + "_" + turns[i].Clicks);
        }

        Debug.Log("$$$___LENGTH:" + (totalLength / turns.Count) + "___CLICKS:" + (totalClicks / turns.Count) + "___$$$");
    }

    public class TurnReportSTATISTICS
    {
        public float turnLength;
        public int Clicks;
    }
}
