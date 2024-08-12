using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WFC;

internal class OptionsManager : DungeonCreatorPage
{

    const string OPTIONS_PATH = DungeonCreatorWindow.FOLDER_PATH+"/GeneratedOptions/";
    const string PROTOTYPES_PATH = DungeonCreatorWindow.FOLDER_PATH+"/CollapsePrototypes";

    CollapseOptionPrototype[] prototypes;
    bool drawPrototypes = false;
    public OptionsManager(string name) : base(name)
    {
       prototypes =  DungeonCreatorWindow.FindAssets<CollapseOptionPrototype>(PROTOTYPES_PATH).ToArray();
    }


    public override void Draw()
    {
        base.Draw();
        if (GUILayout.Button("Draw Options " + prototypes.Length))
        {
            drawPrototypes = !drawPrototypes;
        }
        if (drawPrototypes) DrawPrototypes();
        if (prototypes.Length > 0 && GUILayout.Button("Generate Options"))
        {
            foreach (var item in prototypes)
            {
                CreateOptionAssets(CreateOptions(item));
            }
        }
    }
    void DrawPrototypes()
    {
        for (int i = 0; i < prototypes.Length; i++)
        {
            prototypes[i] = (CollapseOptionPrototype)EditorGUILayout.ObjectField(prototypes[i], typeof(CollapseOptionPrototype), false);
        }
    }
    void CreateOptionAssets(List<CollapseOption> options)
    {
        foreach (var item in options)
        {
            AssetDatabase.CreateAsset(item, OPTIONS_PATH + item.name + ".asset");
        }
    }
    List<CollapseOption> CreateOptions(CollapseOptionPrototype prototype)
    {
        List<CollapseOption> generatedOptions = new List<CollapseOption>();

        CollapseOption basicOption = new CollapseOption();
        CollapseCondition pCond = prototype.Condition;
        basicOption.Condition = prototype.Condition;
        basicOption.Condition.Optionals =  pCond.GetOptionals();
        basicOption.Prefab = prototype.Prefab;
        generatedOptions.Add(basicOption);

        if (prototype.IsRotateable)
        {
            CollapseOption rotatedOption = new CollapseOption();
            rotatedOption.Prefab = prototype.Prefab;

            CollapseCondition rotatedCond = new CollapseCondition(pCond.Left, pCond.Right, pCond.Bottom, pCond.Top);
            rotatedOption.Condition = rotatedCond;
            rotatedOption.RotatedAngle = 90;
            rotatedOption.Condition.Optionals = pCond.GetOptionals();
            rotatedOption.Condition.RotateOptionals();
            generatedOptions.Add(rotatedOption);
            if (!prototype.IsSymetric)
            {
                rotatedOption = new CollapseOption();
                rotatedOption.Prefab = prototype.Prefab;

                rotatedCond = new CollapseCondition(pCond.Bottom, pCond.Top, pCond.Right, pCond.Left);
                rotatedOption.Condition = rotatedCond;
                rotatedOption.RotatedAngle = 180;
                rotatedOption.Condition.Optionals = pCond.GetOptionals();
                rotatedOption.Condition.RotateOptionals();
                rotatedOption.Condition.RotateOptionals();
                generatedOptions.Add(rotatedOption);

                rotatedOption = new CollapseOption();
                rotatedOption.Prefab = prototype.Prefab;

                rotatedCond = new CollapseCondition(pCond.Right, pCond.Left, pCond.Top, pCond.Bottom);
                rotatedOption.Condition = rotatedCond;
                rotatedOption.RotatedAngle = 270;
                rotatedOption.Condition.Optionals = pCond.GetOptionals();
                rotatedOption.Condition.RotateOptionals();
                rotatedOption.Condition.RotateOptionals();
                rotatedOption.Condition.RotateOptionals();
                generatedOptions.Add(rotatedOption);
            }
        }
        for (int i = 0; i < generatedOptions.Count; i++)
        {
            generatedOptions[i].name = "Collapse_Option_" + generatedOptions[i].Prefab.name + "_" + generatedOptions[i].RotatedAngle;
        }
        return generatedOptions;
    }
}