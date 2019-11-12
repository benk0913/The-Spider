using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class CharacterNodeTreeUI : NodeTreeUI
{
    public virtual void ShowCharactersHirarchy(Character character)
    {
        CharacterNodeTreeUIInstance rootNode = GenerateCharacterNode(null, character);

        GenerateTree(rootNode);
    }

    protected virtual CharacterNodeTreeUIInstance GenerateCharacterNode(CharacterNodeTreeUIInstance parent, Character character)
    {
        CharacterNodeTreeUIInstance node = new CharacterNodeTreeUIInstance();
        node.CurrentCharacter = character;
        node.Parent = parent;

        foreach (LocationEntity property in node.CurrentCharacter.PropertiesOwned)
        {
            foreach (Character employee in property.EmployeesCharacters)
            {
                node.Children.Add(GenerateCharacterNode(node, employee));
            }
        }

        return node;
    }



    protected override IEnumerator GenerateTreeRoutine(NodeTreeUIInstance origin)
    {
        yield return StartCoroutine(base.GenerateTreeRoutine(origin));

        yield return StartCoroutine(SetCharacters((CharacterNodeTreeUIInstance)origin));

    }

    protected virtual IEnumerator SetCharacters(CharacterNodeTreeUIInstance node)
    {
        node.nodeObject.transform.GetChild(0).GetChild(0).GetComponent<PortraitUI>().SetCharacter(node.CurrentCharacter);

        yield return 0;

        for (int i = 0; i < node.Children.Count; i++)
        {
            yield return StartCoroutine(SetCharacters((CharacterNodeTreeUIInstance) node.Children[i]));
        }
    }
}

public class CharacterNodeTreeUIInstance : NodeTreeUIInstance
{
    public Character CurrentCharacter;
}
