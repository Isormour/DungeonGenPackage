using System;
using UnityEditor;
using UnityEngine;
using WFC;

internal class DungeonCreationPage : DungeonCreatorPage
{
    private string v;
    public DungeonManager  dungeonManager{ private set; get; }
    bool drawOptions = false;
    public DungeonCreationPage(string v,DungeonManager manager):base(v)
    {
        this.v = v;
        this.dungeonManager = manager;

    }
    public override void InitPage()
    {

    }
    public override void Draw()
    {
        base.Draw();

      
        if (GUILayout.Button("Draw Options " + dungeonManager.conditionConfig.conditions.Length))
        {
            drawOptions = !drawOptions;
        }
        if (drawOptions) DrawProperties();


        if (GUILayout.Button("CollapseBorder"))
        {
            dungeonManager.creator.GenerateBorder();
        }

        if (GUILayout.Button("Create Single"))
        {
            dungeonManager.creator.Generate(true);
        }
        if (GUILayout.Button("Create All"))
        {
            GenerateAll();
        }
    }
    public void GenerateAll()
    {
        dungeonManager.creator.GenerateAll(true);
    }
    void DrawProperties()
    {
        CollapseOption[] options = dungeonManager.creator.collapseOptions;
        for (int i = 0; i < options.Length; i++)
        {
            options[i] = (CollapseOption)EditorGUILayout.ObjectField(options[i], typeof(CollapseOption), false);
        }
    }
}