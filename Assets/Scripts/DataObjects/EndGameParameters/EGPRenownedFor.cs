using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EGPRenownedFor", menuName = "DataObjects/EndGameParameter/EGPRenownedFor", order = 2)]
public class EGPRenownedFor : EndGameParameter
{
    public List<EndgameParamInstance> Instances = new List<EndgameParamInstance>();

    public override string GetValue()
    {
  


        if (CORE.Stats.PlotsWithBrute > CORE.Stats.PlotsWithCunning && CORE.Stats.PlotsWithBrute > CORE.Stats.PlotsWithStealth)
        {
            return Instances.Find(x => x.Key == "Bloodthirsty").Key;
        }
        else if (CORE.Stats.PlotsWithStealth > CORE.Stats.PlotsWithCunning && CORE.Stats.PlotsWithStealth > CORE.Stats.PlotsWithBrute)
        {
            return Instances.Find(x => x.Key == "Silent And Deadly").Key;
        }
        else if (CORE.Stats.PlotsWithCunning > CORE.Stats.PlotsWithStealth && CORE.Stats.PlotsWithCunning > CORE.Stats.PlotsWithBrute)
        {
            return Instances.Find(x => x.Key == "Trickster").Key;
        }
        else if(CORE.Stats.PlotsWithBrute == 0 && CORE.Stats.PlotsWithCunning== 0 && CORE.Stats.PlotsWithStealth== 0)
        {
            return Instances.Find(x => x.Key == "Pacifist").Key;
        }

        return Instances.Find(x => x.Key == "Professional").Key;
    }

    public override Sprite GetIcon()
    {
        if (CORE.Stats.PlotsWithBrute > CORE.Stats.PlotsWithCunning && CORE.Stats.PlotsWithBrute > CORE.Stats.PlotsWithStealth)
        {
            return Instances.Find(x => x.Key == "Bloodthirsty").Icon;
        }
        else if (CORE.Stats.PlotsWithStealth > CORE.Stats.PlotsWithCunning && CORE.Stats.PlotsWithStealth > CORE.Stats.PlotsWithBrute)
        {
            return Instances.Find(x => x.Key == "Silent And Deadly").Icon;
        }
        else if (CORE.Stats.PlotsWithCunning > CORE.Stats.PlotsWithStealth && CORE.Stats.PlotsWithCunning > CORE.Stats.PlotsWithBrute)
        {
            return Instances.Find(x => x.Key == "Trickster").Icon;
        }
        else if (CORE.Stats.PlotsWithBrute == 0 && CORE.Stats.PlotsWithCunning == 0 && CORE.Stats.PlotsWithStealth == 0)
        {
            return Instances.Find(x => x.Key == "Pacifist").Icon;
        }

        return Instances.Find(x => x.Key == "Professional").Icon;
    }
}