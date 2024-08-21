using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WFC;

internal class OptionsManager : DungeonCreatorPage
{

    string OPTIONS_PATH = DungeonCreatorWindow.configMain.MainFolderPath + "/GeneratedOptions/";
    string PROTOTYPES_PATH = DungeonCreatorWindow.configMain.MainFolderPath + "/CollapsePrototypes";
    const string PROTOTYPES_PREF_KEY = "Prototypes_Pref_Key";
    const string OPTIONS_PREF_KEY = "Options_Pref_Key";

    CollapseOptionPrototype[] prototypes;
    bool drawPrototypes = false;
    string currentPrototypesPath = "";
    string currentOptionsPath = "";
    public OptionsManager(string name) : base(name)
    {
        currentPrototypesPath = PlayerPrefs.GetString(PROTOTYPES_PREF_KEY, "");
        currentOptionsPath = PlayerPrefs.GetString(OPTIONS_PREF_KEY, "");
        if (currentPrototypesPath == "") currentPrototypesPath = PROTOTYPES_PATH;
        prototypes = DungeonCreatorWindow.FindAssets<CollapseOptionPrototype>(currentPrototypesPath).ToArray();
    }


    public override void Draw()
    {
        base.Draw();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Prototype Path"))
        {
            currentPrototypesPath = EditorUtility.OpenFolderPanel("Select Directory", currentPrototypesPath, "pick prototypes folder");
            currentPrototypesPath = currentPrototypesPath.Substring(Application.dataPath.Length - "Assets".Length);
            if (currentPrototypesPath != "")
                PlayerPrefs.SetString(PROTOTYPES_PREF_KEY, currentPrototypesPath);
        }
        EditorGUILayout.LabelField(currentPrototypesPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Options Path"))
        {
            currentOptionsPath = EditorUtility.OpenFolderPanel("Select Directory", currentOptionsPath, "pick prototypes folder");
            currentOptionsPath = currentOptionsPath.Substring(Application.dataPath.Length - "Assets".Length);
            if (currentOptionsPath != "")
                PlayerPrefs.SetString(OPTIONS_PREF_KEY, currentOptionsPath);
        }
        EditorGUILayout.LabelField(currentOptionsPath);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space(10);
        if (GUILayout.Button("Draw prototypes " + prototypes.Length))
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
        basicOption.Condition.Optionals = pCond.GetOptionals();
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