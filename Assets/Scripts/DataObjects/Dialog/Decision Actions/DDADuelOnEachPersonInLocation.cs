using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDADuelOnEachEmployeeInLocation", menuName = "DataObjects/Dialog/Actions/DDADuelOnEachEmployeeInLocation", order = 2)]
public class DDADuelOnEachEmployeeInLocation : DDAPossibleDialogStatDuel
{
    public override void Activate()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        Character Actor = (Character)DialogWindowUI.Instance.GetDialogParameter("Actor");

        foreach(Character character in location.EmployeesCharacters)
        {
            if(!location.CharactersInLocation.Contains(character))
            {
                continue;
            }

            float actorSkill = Actor.GetBonus(ActorSkill).Value;

            if (Random.Range(0f, actorSkill + Challenge.RarityValue + Challenge.ChallengeValue) < (actorSkill + Challenge.RarityValue))
            {
                if (WinPiece == null)
                {
                    continue;
                }

                DialogWindowUI.Instance.InsertNextPiece(WinPiece);
            }
            else
            {
                if (LosePiece == null)
                {
                    continue;
                }

                DialogWindowUI.Instance.InsertNextPiece(LosePiece);
            }
        }
    }
}
