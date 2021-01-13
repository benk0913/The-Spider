using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseDisposalUI : MonoBehaviour
{
    public static CorpseDisposalUI Instance;

    [SerializeField]
    Transform Container;

    [SerializeField]
    List<Character> Characters = new List<Character>();

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(List<Character> characters)
    {
        this.gameObject.SetActive(true);
        this.Characters = characters;

        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }

        foreach(Character character in Characters)
        {
            GameObject temp = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
            temp.transform.SetParent(Container, false);
            temp.transform.localScale = Vector3.one;
            temp.transform.position = Container.transform.position;

            temp.GetComponent<PortraitUI>().SetCharacter(character);
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void LeaveThemThere()
    {
        CORE.PC.Heat++;
        Hide();
    }

    public void DropAtSea()
    {
        Item boat = CORE.PC.Belogings.Find(X => X.name == "Boat");
        if (boat == null)
        {
            GlobalMessagePrompterUI.Instance.Show("You don't have a Boat ready for action.", 1f, Color.red);
            return;
        }

        Character agent = CORE.PC.CharactersInCommand.Find(X => X.GetBonus(CORE.Instance.Database.GetBonusType("Stealthy")).Value > 2 && X.IsAgent);
        if (agent == null)
        {
            GlobalMessagePrompterUI.Instance.Show("You don't have an agent which is stealthy enough. (Stealthy - 3)", 1f, Color.red);
            return;
        }

        SelectAgentWindowUI.Instance.Show(
           x =>
           {

               Hide();

               float agentSkill = x.GetBonus(CORE.Instance.Database.GetBonusType("Stealthy")).Value;
               if (Random.Range(0f, agentSkill + 2f) > agentSkill)
               {
                   WarningWindowUI.Instance.Show("Your agent has failed!", () =>
                   {
                       CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, x, x.CurrentLocation);
                   }
                   , true);
                   
                   return;
               }

           },
           x => x.TopEmployer == CORE.PC && !x.IsDead && x.CurrentTaskEntity == null && x.IsAgent && x.Age > 15 && x.GetBonus(CORE.Instance.Database.GetBonusType("Sneaky")).Value > 2,
           "Who will bury the corpses?");
        

    }

    public void HideInTheWilds()
    {
        Item carriage = CORE.PC.Belogings.Find(X => X.name == "Carriage");
        if (carriage == null)
        {
            GlobalMessagePrompterUI.Instance.Show("You don't have a Carriage ready for action.", 1f, Color.red);
            return;
        }

        Character agent = CORE.PC.CharactersInCommand.Find(X => X.GetBonus(CORE.Instance.Database.GetBonusType("Stealthy")).Value > 2 && X.IsAgent);
        if (agent == null)
        {
            GlobalMessagePrompterUI.Instance.Show("You don't have an agent which is stealthy enough. (Stealthy - 3)", 1f, Color.red);
            return;
        }

        SelectAgentWindowUI.Instance.Show(
           x =>
           {

               Hide();

               float agentSkill = x.GetBonus(CORE.Instance.Database.GetBonusType("Stealthy")).Value;
               if (Random.Range(0f, agentSkill + 2f) > agentSkill)
               {
                   WarningWindowUI.Instance.Show("Your agent has failed!", () =>
                   {
                       CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, x, x.CurrentLocation);
                   }
                   , true);

                   return;
               }

           },
           x => x.TopEmployer == CORE.PC && !x.IsDead && x.CurrentTaskEntity == null && x.IsAgent && x.Age > 15 && x.GetBonus(CORE.Instance.Database.GetBonusType("Sneaky")).Value > 2,
           "Who will bury the corpses?");
    }

    public void HideThemInAProperty()
    {
        SelectLocationViewUI.Instance.Show(x =>
        {
            x.CorpsesBuried+=Characters.Count;
            Hide();
        }
        , x => x.OwnerCharacter != null && x.OwnerCharacter.TopEmployer == CORE.PC);
    }


}
