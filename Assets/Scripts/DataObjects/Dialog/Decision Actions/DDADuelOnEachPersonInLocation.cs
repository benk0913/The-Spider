using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDADuelOnEachPersonInLocation", menuName = "DataObjects/Dialog/Actions/DDADuelOnEachPersonInLocation", order = 2)]
public class DDADuelOnEachPersonInLocation : DDAPossibleDialogStatDuel
{
    [SerializeField]
    public bool OnlyOfDifferentFaction;


    public override void Activate()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        Character Actor = (Character)DialogWindowUI.Instance.GetDialogParameter("Actor");

        foreach(Character character in location.CharactersInLocation)
        {
            if(OnlyOfDifferentFaction)
            {
                if(character.CurrentFaction == Actor.CurrentFaction)
                {
                    continue;
                }
            }

            float actorSkill = Actor.GetBonus(ActorSkill).Value;

            if (Random.Range(0f, actorSkill + Challenge.RarityValue + Challenge.ChallengeValue) < (actorSkill + Challenge.RarityValue))
            {
                if (WinPiece == null)
                {
                    continue;
                }

                DialogPiece pieceClone = Instantiate(WinPiece);
                pieceClone.TargetCharacter = character;
                DialogWindowUI.Instance.InsertNextPiece(pieceClone);
            }
            else
            {
                if (LosePiece == null)
                {
                    continue;
                }

                DialogPiece pieceClone = Instantiate(LosePiece);
                pieceClone.TargetCharacter = character;
                DialogWindowUI.Instance.InsertNextPiece(pieceClone);
            }
        }
    }
}
