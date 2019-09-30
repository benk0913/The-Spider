using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDADuelOnEachTargetInPiece", menuName = "DataObjects/Dialog/Actions/DDADuelOnEachTargetInPiece", order = 2)]
public class DDADuelOnEachTargetInPiece : DDAPossibleDialogStatDuel
{
    public int MaxPeople = 999;

    public override void Activate()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        Character Actor = (Character)DialogWindowUI.Instance.GetDialogParameter("Actor");

        List<Character> enemyWinners = new List<Character>();

        if (DialogWindowUI.Instance.CurrentPiece.TargetCharacters != null)
        {
            foreach (Character character in DialogWindowUI.Instance.CurrentPiece.TargetCharacters)
            {
                if (enemyWinners.Count > MaxPeople)
                {
                    break;
                }

                float actorSkill = Actor.GetBonus(ActorSkill).Value;
                float enemySkill = character.GetBonus(Challenge.Type).Value;

                if (Random.Range(0f, actorSkill + enemySkill) < actorSkill)
                {
                    continue;
                }
                else
                {
                    character.Known.Know("Appearance");

                    enemyWinners.Add(character);
                }
            }
        }

        if (enemyWinners.Count > 0)
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
