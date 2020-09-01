using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BalanceSheetUI : MonoBehaviour
{
    public static BalanceSheetUI Instance;

    public Transform Container;

    public List<string> Log = new List<string>();

    public BalanceStamp ThisTurnStamp;
    public BalanceStamp LastTurnStamp;
    public BalanceStamp EndOfDayStamp;
    public BalanceStamp PreviousWeekStamp;

    private void Awake()
    {
        Instance = this;
        CORE.Instance.SubscribeToEvent("PassTimeComplete", PassTurn);
    }


    public void PassTurn()
    {

        if (GameClock.Instance.CurrentTimeOfDay == GameClock.GameTime.Morning)
        {
            EndOfDayStamp = ThisTurnStamp;
        }

        LastTurnStamp = ThisTurnStamp;
        ThisTurnStamp = new BalanceStamp(CORE.PC.CGold, CORE.PC.CRumors, CORE.PC.CConnections, CORE.PC.CProgress);

        if (GameClock.Instance.CurrentTimeOfDay == GameClock.GameTime.Morning && GameClock.Instance.CurrentTurn%35 == 0)
        {
            PreviousWeekStamp = ThisTurnStamp;
        }
    }
    
    public void RefreshUI()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }

        List<string> tempLog = new List<string>();

        tempLog.Add("<u>Balance Compared With:</u>");

        int valueDiff;
        if (ThisTurnStamp != null)
        {
            tempLog.Add("<u>This Turn:</u>");
            valueDiff = (CORE.PC.CGold - ThisTurnStamp.Gold);
            tempLog.Add("<color=black>Gold</color>"
                + (valueDiff < 0? "<color=red>" : "<color=green>+")
                + valueDiff+"</color>");

            valueDiff = (CORE.PC.CRumors- ThisTurnStamp.Rumors);
            tempLog.Add("<color=black>Rumors</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CConnections- ThisTurnStamp.Connections);
            tempLog.Add("<color=black>Connections</color>"
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CProgress - ThisTurnStamp.Progression);
            tempLog.Add("<color=black>Progression</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");
        }
        else
        {
            tempLog.Add("--No Records For This Turn Yet--");
        }

        if (LastTurnStamp != null)
        {
            tempLog.Add("<u>Last Turn:</u>");
            valueDiff = (CORE.PC.CGold - LastTurnStamp.Gold);
            tempLog.Add("<color=black>Gold</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CRumors - LastTurnStamp.Rumors);
            tempLog.Add("<color=black>Rumors</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CConnections - LastTurnStamp.Connections);
            tempLog.Add("<color=black>Connections</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CProgress - LastTurnStamp.Progression);
            tempLog.Add("<color=black>Progression</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");
        }
        else
        {
            tempLog.Add("--No Records For The Last Turn Yet--");
        }

        if (EndOfDayStamp != null)
        {
            tempLog.Add("<u>Previous Day:</u>");
            valueDiff = (CORE.PC.CGold - EndOfDayStamp.Gold);
            tempLog.Add("<color=black>Gold</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CRumors - EndOfDayStamp.Rumors);
            tempLog.Add("<color=black>Rumors</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CConnections - EndOfDayStamp.Connections);
            tempLog.Add("<color=black>Connections</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CProgress - EndOfDayStamp.Progression);
            tempLog.Add("<color=black>Progression</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");
        }
        else
        {
            tempLog.Add("--No Records For The Previous Day Yet--");
        }

        if (PreviousWeekStamp != null)
        {
            tempLog.Add("<u>Previous Week:</u>");
            valueDiff = (CORE.PC.CGold - PreviousWeekStamp.Gold);
            tempLog.Add("<color=black>Gold</color> " 
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CRumors - PreviousWeekStamp.Rumors);
            tempLog.Add("<color=black>Rumors</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CConnections - PreviousWeekStamp.Connections);
            tempLog.Add("<color=black>Connections</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");

            valueDiff = (CORE.PC.CProgress - PreviousWeekStamp.Progression);
            tempLog.Add("<color=black>Progression</color> "
                + (valueDiff < 0 ? "<color=red>" : "<color=green>+")
                + valueDiff + "</color>");
        }
        else
        {
            tempLog.Add("--No Records For The Previous Week Yet--");
        }

        tempLog.AddRange(Log);

        foreach(string logItem in tempLog)
        {
            TextMeshProUGUI item = ResourcesLoader.Instance.GetRecycledObject("SheetLogLine").GetComponent<TextMeshProUGUI>();
            item.transform.SetParent(Container, false);
            item.transform.position = new Vector3(item.transform.position.x,item.transform.position.y, Container.transform.position.z);
            item.transform.localScale = Vector3.one;
            item.text = logItem;
        }
    }

    public class BalanceStamp
    {
        public int Gold;
        public int Rumors;
        public int Connections;
        public int Progression;

        public BalanceStamp(int gold, int rumors, int connections, int progression)
        {
            this.Gold = gold;
            this.Rumors = rumors;
            this.Connections = connections;
            this.Progression = progression;
        }
    }
}
