using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptorEntity : MonoBehaviour
{
    MeshRenderer[] Meshes;
    PortraitUI[] Portraits;

    [SerializeField]
    Material corruptMaterial;

    [SerializeField]
    Material CorruptorPortraitMaterial;

    public float Speed = 1f;

    public int DelayTime = 5;



    private void OnEnable()
    {
        StartCoroutine(CorruptRoutine());

        CORE.Instance.SubscribeToEvent("PassTimeComplete", OnPassTurn);
    }

    private void Update()
    {
        corruptMaterial.mainTextureOffset = new Vector2(corruptMaterial.mainTextureOffset.x * Speed * Time.deltaTime, corruptMaterial.mainTextureOffset.y);
    }

    void OnPassTurn()
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

        Character character = null;

        while (character == null)
        {
            character = CORE.Instance.Characters[Random.Range(0, CORE.Instance.Characters.Count)];

            if(character .isImportant || character.IsDead || character.IsDisabled || character.NeverDED)
            {
                character = null;
            }
        }

        if (character.TopEmployer == CORE.PC)
        {
            PopupDataPreset preset = CORE.Instance.Database.GetPopupPreset("CorruptionPopup");

            PopupData popup = new PopupData(preset, new List<Character> { character }, new List<Character> { }, () => 
            {
                character.Death();
            }
            , null);

            PopupWindowUI.Instance.AddPopup(popup);
        }
        else
        {
            character.Death();
        }
    }

    IEnumerator CorruptRoutine()
    {
        Meshes = FindObjectsOfType<MeshRenderer>();
        Portraits = FindObjectsOfType<PortraitUI>();

        if (Meshes == null || Meshes.Length == 0)
        {
            Debug.LogError("NO MESHES");
            yield break;
        }

        while (true)
        {
            //yield return new WaitForSeconds(DelayTime);

            //Meshes[Random.Range(0, Meshes.Length)].material = corruptMaterial;

            yield return new WaitForSeconds(DelayTime);

            Portraits = FindObjectsOfType<PortraitUI>();

            if (Portraits != null && Portraits.Length > 0)
            {
                Portraits[Random.Range(0, Portraits.Length)].SetMaterial(CorruptorPortraitMaterial);
            }

            
        }
    }
}
