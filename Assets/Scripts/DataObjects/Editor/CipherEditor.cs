using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Cipher))]
public class CipherEditor : Editor
{
    string fromABC;
    string toABC;

    public override void OnInspectorGUI()
    {
        Cipher cipher = (Cipher)target;

        fromABC = GUILayout.TextField(fromABC);
        toABC = GUILayout.TextField(toABC);

        if (GUILayout.Button("AutoCreateFromABC"))
        {
            CreateFromABC(cipher);
        }



        DrawDefaultInspector();
    }

    void CreateFromABC(Cipher cipher)
    {
        cipher.Replacements.Clear();
        for(int i=0;i<fromABC.Length;i++)
        {
            CipherLetter replacement = new CipherLetter();
            replacement.letter = fromABC[i];
            replacement.toLetter = toABC[i];
            cipher.Replacements.Add(replacement);
        }

        EditorUtility.SetDirty(cipher);
    }
}