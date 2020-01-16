using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


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


        EditorUtility.SetDirty(db);
    }
}