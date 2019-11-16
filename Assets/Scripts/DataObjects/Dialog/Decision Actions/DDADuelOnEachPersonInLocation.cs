using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDADuelOnEachPersonInLocation", menuName = "DataObjects/Dialog/Actions/DDADuelOnEachPersonInLocation", order = 2)]
public class DDADuelOnEachPersonInLocation : DDAPossibleDialogStatDuel
{
    [SerializeField]
    public bool OnlyOfDifferentFaction;

    public int MaxPeople = 999;

    public override void Activate()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        Character Actor = (Character)DialogWindowUI.Instance.GetDialogParameter("Actor");

        List<Character> enemyWinners = new List<Character>();

        foreach(Character character in location.CharactersInLocation)
        {
            if (enemyWinners.Count > MaxPeople)
            {
                break;
            }

            if (OnlyOfDifferentFaction)
            {
                if(character.CurrentFaction == Actor.CurrentFaction)
                {
                    continue;
                }
            }

            float actorSkill = Actor.GetBonus(ActorSkill).Value;
            float enemySkill = character.GetBonus(Challenge.Type).Value;

            if (Random.Range(0f, actorSkill + enemySkill) < actorSkill)
            {
                continue;
            }
            else
            {
                character.Known.Know("Appearance", character.TopEmployer);

                enemyWinners.Add(character);
            }
        }

        if(enemyWinners.Count > 0)
        {
            DialogPiece piece = LosePiece.Clone();
            piece.TargetCharacters = enemyWinners.ToArray();
            DialogWindowUI.Instance.ShowDialogPiece(piece);
        }
        else
        {
            DialogWindowUI.Instance.ShowDialogPiece(WinPiece);
        }
    }
}
