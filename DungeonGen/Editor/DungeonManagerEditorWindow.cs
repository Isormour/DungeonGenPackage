using UnityEditor;
using UnityEngine;
using WFC;

[CustomEditor(typeof(DungeonManager))]
public class DungeonManagerEditorWindow : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Dungeon"))
        {
            DungeonManager script = (DungeonManager)target;
            script.CreateDungeon();
        }
    }
}
