using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using WFC;
public class DungeonCreatorWindow : EditorWindow
{
    DungeonCreatorPage currentPage;
    OptionsManager optionsManager;
    DungeonCreationPage creatorPage;
    DungeonCreatorFixRooms fixRooms;
    ConditionConfigPage configCondition;
    DungeonRoomInterpreterPage interpreter;
    TestingPage tests;
    GameObject dungeonManagerObject;
    private static ConditionsConfig _config;
    DungeonManager dungeonManager;
    public static DungeonGenerationConfig configMain;
    public static string folderPath { private set; get; }

    const string DUNGEON_CREATOR_CONFIG_PREF = "Dungeon_Creator_Config_Pref";
    internal static ConditionsConfig condConfig
    {
        get
        {
            if (_config == null)
            {
                _config = FindAsset<ConditionsConfig>(folderPath + "/Conditions_Config.asset");
            }
            return _config;
        }
    }
    [MenuItem("DB/Tools/Dungeon Creator")]
    public static void OpenWindow()
    {
        _config = FindAsset<ConditionsConfig>(folderPath + "Conditions_Config.asset");
        DungeonCreatorWindow tempWindow = GetWindow<DungeonCreatorWindow>("Dungeon Creator");
        tempWindow.InitWindow();

    }
    void InitWindow()
    {
        string configPath = PlayerPrefs.GetString(DUNGEON_CREATOR_CONFIG_PREF, "Packages/com.db.dungeongen/DungeonGen/DungeonGenerationConfig.asset");

        configMain = FindAsset<DungeonGenerationConfig>(configPath);
        folderPath = configMain.MainFolderPath;

        CollapseOption[] options = FindAssets<CollapseOption>(folderPath + "/GeneratedOptions/").ToArray();
        dungeonManagerObject = Instantiate(FindAsset<GameObject>(folderPath + "/DungeonManager.prefab"));

        dungeonManager = dungeonManagerObject.GetComponent<DungeonManager>();

        DungeonProfile profile = new DungeonProfile();
        profile.requrementsData = FindAsset<DungeonRequirments>(folderPath + "/Restrictions/Basic.asset");
        dungeonManager.Initialize(profile);
        dungeonManager.CreateDungeon(true);
        creatorPage = new DungeonCreationPage("Dungeon Creator", dungeonManager);
        fixRooms = new DungeonCreatorFixRooms("Fix Rooms", dungeonManager);
        interpreter = new DungeonRoomInterpreterPage("Interpreter", dungeonManager);
        ChangePage(interpreter);
        dungeonManager.SaveDungeon();
    }
    private void OnDestroy()
    {
        if (dungeonManagerObject != null)
        {
            DestroyImmediate(dungeonManagerObject);
            dungeonManagerObject = null;
        }
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        configMain = EditorGUILayout.ObjectField(configMain, typeof(DungeonGenerationConfig), false) as DungeonGenerationConfig;
        if (GUILayout.Button("ChangeConfig"))
        {
            string configPath = PlayerPrefs.GetString(DUNGEON_CREATOR_CONFIG_PREF);
            configMain = FindAsset<DungeonGenerationConfig>(configPath);
            configPath = EditorUtility.OpenFolderPanel("Select Directory", configPath, "pick config");
            configMain = configMain = FindAsset<DungeonGenerationConfig>(configPath);

            folderPath = configMain.MainFolderPath;
            if (!File.Exists(folderPath + "/GeneratedOptions/"))
            {
                Directory.CreateDirectory(folderPath + "/GeneratedOptions/");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Condition config"))
        {
            configCondition = new ConditionConfigPage("Condition Config");
            ChangePage(configCondition);
        }
        if (GUILayout.Button("OptionsManager"))
        {
            optionsManager = new OptionsManager("Options");
            ChangePage(optionsManager);
        }
        if (GUILayout.Button("Generation"))
        {
            CollapseOption[] options = FindAssets<CollapseOption>(folderPath + "/GeneratedOptions/").ToArray();
            creatorPage = new DungeonCreationPage("Dungeon Creator", dungeonManager);
            ChangePage(creatorPage);
        }
        if (GUILayout.Button("Fix Rooms"))
        {
            fixRooms = new DungeonCreatorFixRooms("Fix Rooms", dungeonManager);
            ChangePage(fixRooms);
        }
        if (GUILayout.Button("RoomInterpreter"))
        {
            interpreter = new DungeonRoomInterpreterPage("Room Interpreter", dungeonManager);
            ChangePage(interpreter);
        }
        if (GUILayout.Button("tests"))
        {
            tests = new TestingPage("Fix Rooms");
            ChangePage(tests);
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            CollapseOption[] options = FindAssets<CollapseOption>(folderPath + "/GeneratedOptions/").ToArray();
            creatorPage = new DungeonCreationPage("Dungeon Creator", dungeonManager);
            fixRooms = new DungeonCreatorFixRooms("Fix Rooms", dungeonManager);
            ChangePage(fixRooms);
        }

        currentPage.Draw();
    }

    public void ChangePage(DungeonCreatorPage page)
    {
        currentPage = page;
        currentPage.InitPage();
    }

    public static T FindAsset<T>(string path) where T : UnityEngine.Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    public static List<T> FindAssets<T>(string folderPath) where T : UnityEngine.Object
    {
        string[] paths = Directory.GetFiles(folderPath);
        List<string> pathsCleaned = new List<string>();
        foreach (var path in paths)
        {
            if (!path.Contains(".meta"))
            {
                pathsCleaned.Add(path);
            }
        }

        List<T> assetsFound = new List<T>();
        foreach (var path in pathsCleaned)
        {
            assetsFound.Add(FindAsset<T>(path));
        }
        return assetsFound;
    }
}
