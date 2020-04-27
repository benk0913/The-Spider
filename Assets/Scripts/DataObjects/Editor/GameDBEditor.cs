using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomEditor(typeof(GameDB))]
public class GameDBEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameDB db = (GameDB)target;

        if(GUILayout.Button("Auto Load"))
        {
            AutoLoad(db);
        }

        if (GUILayout.Button("Calculate Word Count"))
        {
            CalcWordCount(db);
        }

        if (GUILayout.Button("Print Custom Analysis"))
        {
            PrintCustomAnalysis(db);
        }

        DrawDefaultInspector();
    }

    void AutoLoad(GameDB db)
    {
        string[] guids = AssetDatabase.FindAssets("t:Character", new[] { "Assets/" + db.DataPath });
        db.PresetCharacters.Clear();
        foreach (string guid in guids)
        {
            db.PresetCharacters.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Character)) as Character);
        }

        guids = AssetDatabase.FindAssets("t:Faction", new[] { "Assets/" + db.DataPath });
        db.Factions.Clear();
        foreach (string guid in guids)
        {
            db.Factions.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Faction)) as Faction);
        }

        guids = AssetDatabase.FindAssets("t:Property", new[] { "Assets/" + db.DataPath });
        db.Properties.Clear();
        foreach (string guid in guids)
        {
            db.Properties.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Property)) as Property);
        }

        guids = AssetDatabase.FindAssets("t:Trait", new[] { "Assets/" + db.DataPath });
        db.Traits.Clear();
        foreach (string guid in guids)
        {
            db.Traits.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Trait)) as Trait);
        }

        guids = AssetDatabase.FindAssets("t:Quest", new[] { "Assets/" + db.DataPath });
        db.AllQuests.Clear();
        foreach (string guid in guids)
        {
            db.AllQuests.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Quest)) as Quest);
        }

        guids = AssetDatabase.FindAssets("t:Item", new[] { "Assets/" + db.DataPath });
        db.AllItems.Clear();
        foreach (string guid in guids)
        {
            db.AllItems.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Item)) as Item);
        }

        guids = AssetDatabase.FindAssets("t:PopupDataPreset", new[] { "Assets/" + db.DataPath });
        db.AllPopupPresets.Clear();
        foreach (string guid in guids)
        {
            db.AllPopupPresets.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(PopupDataPreset)) as PopupDataPreset);
        }

        guids = AssetDatabase.FindAssets("t:LongTermTask", new[] { "Assets/" + db.DataPath });
        db.LongTermTasks.Clear();
        foreach (string guid in guids)
        {
            db.LongTermTasks.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(LongTermTask)) as LongTermTask);
        }

        guids = AssetDatabase.FindAssets("t:SchemeType", new[] { "Assets/" + db.DataPath });
        db.Schemes.Clear();
        foreach (string guid in guids)
        {
            db.Schemes.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(SchemeType)) as SchemeType);
        }

        guids = AssetDatabase.FindAssets("t:FavorDecision", new[] { "Assets/" + db.DataPath });
        db.FavorDecisions.Clear();
        foreach (string guid in guids)
        {
            db.FavorDecisions.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(FavorDecision)) as FavorDecision);
        }

        guids = AssetDatabase.FindAssets("t:DialogPiece", new[] { "Assets/" + db.DataPath });
        db.AllDialogPieces.Clear();
        foreach (string guid in guids)
        {
            db.AllDialogPieces.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(DialogPiece)) as DialogPiece);
        }

        guids = AssetDatabase.FindAssets("t:LetterPreset", new[] { "Assets/" + db.DataPath });
        db.PresetLetters.Clear();
        foreach (string guid in guids)
        {
            db.PresetLetters.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(LetterPreset)) as LetterPreset);
        }

        guids = AssetDatabase.FindAssets("t:SessionRule", new[] { "Assets/" + db.DataPath });
        db.SessionRules.Clear();
        foreach (string guid in guids)
        {
            db.SessionRules.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(SessionRule)) as SessionRule);
        }

        guids = AssetDatabase.FindAssets("t:DuelProc", new[] { "Assets/" + db.DataPath });
        db.DuelProcs.Clear();
        foreach (string guid in guids)
        {
            db.DuelProcs.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(DuelProc)) as DuelProc);
        }

        guids = AssetDatabase.FindAssets("t:TutorialScreenInstance", new[] { "Assets/" + db.DataPath });
        db.TutorialScreenInstances.Clear();
        foreach (string guid in guids)
        {
            db.TutorialScreenInstances.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(TutorialScreenInstance)) as TutorialScreenInstance);
        }

        guids = AssetDatabase.FindAssets("t:RecruitmentPool", new[] { "Assets/" + db.DataPath });
        db.RecruitmentPools.Clear();
        foreach (string guid in guids)
        {
            db.RecruitmentPools.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(RecruitmentPool)) as RecruitmentPool);
        }


        EditorUtility.SetDirty(db);
    }

    void CalcWordCount(GameDB db)
    {
        string[] guids;
        guids = AssetDatabase.FindAssets("t:DialogPiece", new[] { "Assets/" + db.DataPath });

        int wordCountTotal = 0;
        int senencesCountTotal= 0;
        foreach (string guid in guids)
        {
            DialogPiece piece = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(DialogPiece)) as DialogPiece;

            string desc = piece.Description;
            Regex regex = new Regex("\"(.*?)\"");

            MatchCollection matches = regex.Matches(desc);

            senencesCountTotal += matches.Count;

            for(int i=0;i<matches.Count;i++)
            {
                for (int g = 1; g < matches[i].Groups.Count; g++)
                {
                    string matchdesc = matches[i].Groups[g].ToString();

                    int wordCount = 0, index = 0;

                    // skip whitespace until first word
                    while (index < matchdesc.Length && char.IsWhiteSpace(matchdesc[index]))
                        index++;

                    while (index < matchdesc.Length)
                    {
                        // check if current char is part of a word
                        while (index < matchdesc.Length && !char.IsWhiteSpace(matchdesc[index]))
                            index++;

                        wordCount++;

                        // skip whitespace until next word
                        while (index < matchdesc.Length && char.IsWhiteSpace(matchdesc[index]))
                            index++;
                    }

                    wordCountTotal += wordCount;
                }
            }

        }

        Debug.Log("Total Sentences In Dialogs - " + senencesCountTotal);
        Debug.Log("Total Words In Dialogs - " + wordCountTotal);

        wordCountTotal = 0;

        guids = AssetDatabase.FindAssets("t:LetterPreset", new[] { "Assets/" + db.DataPath });

        Dictionary<string, int> wordCountPerCharacter = new Dictionary<string, int>();
        wordCountPerCharacter.Add("Unknown", 0);
        foreach (string guid in guids)
        {
            LetterPreset letter = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(LetterPreset)) as LetterPreset;

            if(letter == null)
            {
                continue;
            }

            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < letter.Description.Length && char.IsWhiteSpace(letter.Description[index]))
                index++;

            while (index < letter.Description.Length)
            {
                // check if current char is part of a word
                while (index < letter.Description.Length && !char.IsWhiteSpace(letter.Description[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < letter.Description.Length && char.IsWhiteSpace(letter.Description[index]))
                    index++;
            }

            if(letter.PresetSender == null)
            {
                wordCountPerCharacter["Unknown"] += wordCount;
                continue;
            }

            if(!wordCountPerCharacter.ContainsKey(letter.PresetSender.name))
            {
                wordCountPerCharacter.Add(letter.PresetSender.name, 0);
            }

            wordCountPerCharacter[letter.PresetSender.name] += wordCount;
            wordCountTotal += wordCount;
        }

        Debug.Log("- Total Words In Letterss - "+wordCountTotal);
        foreach(string key in wordCountPerCharacter.Keys)
        {
            Debug.Log(key + " - " + wordCountPerCharacter[key]);
        }
    }

    void PrintCustomAnalysis(GameDB db)
    {
        List<Character> characters = new List<Character>();

        characters.AddRange(db.PresetCharacters.FindAll(x => x.Clothing == null));

        characters.ForEach((x) => { Debug.Log(x.name); });
    }
}