using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace WFC
{
    [ExecuteAlways()]
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance;
        public ConditionsConfig ConditionConfig;
        public CollapseOption[] Options;
        public RoomFillConfig[] RoomConfigs;
        bool initialized = false;

        public DungeonCreator creator { private set; get; }
        public DungeonGraphCreator graph { private set; get; }
        public DungeonRoomInterpreter roomInterpreter { private set; get; }
        public DungeonProfile dungeonProfile { private set; get; }

        public DungeonRequirments restrictions;

        [SerializeField] float cellScale = 1;
        [SerializeField] float levelHeight = 1;
        [SerializeField] int sizeX = 8;
        [SerializeField] int sizeY = 6;
        [SerializeField] bool colorDungeonParts = true;
        [SerializeField] bool CreateOnStart = false;
        [SerializeField] bool LoadOnStart = false;

        public UnityEvent<DungeonProfile> OnDungeonGenerated;

        private void Awake()
        {
            Initialize();
            if (CreateOnStart) { CreateDungeon(); };
            if (LoadOnStart) { LoadDungeon(); };

        }
        public void Initialize()
        {
            if (initialized)
                return;
            initialized = true;
            if (Instance != this)
            {
                if (Instance != null)
                    Destroy(Instance.gameObject);
            }
            Instance = this;
            dungeonProfile = new DungeonProfile(cellScale, levelHeight, restrictions);
        }
        public void CreateDungeon()
        {
            dungeonProfile = new DungeonProfile(cellScale, levelHeight, restrictions);
            bool FitDungeonConditions = false;
            float creationTime = Time.time;
            int iterations = 0;
            while (!FitDungeonConditions)
            {
                creator = new DungeonCreator(Options, sizeX, sizeY, dungeonProfile);
                creator.GenerateAll(true);
                graph = new DungeonGraphCreator(creator.grid, creator.dungeonParent, dungeonProfile);
                roomInterpreter = new DungeonRoomInterpreter(dungeonProfile);
                CountRooms();
                FitDungeonConditions = CheckDungeonConditions(dungeonProfile);
                iterations++;
            }
            Debug.Log("iterations = " + iterations + " At time  = " + (creationTime - Time.time).ToString());
            CreateDungeonObjects();
            graph.ReparentBranches();
            graph.RepositionBranches();

            OnDungeonGenerated?.Invoke(dungeonProfile);
        }

        protected virtual bool CheckDungeonConditions(DungeonProfile dungeonProfile)
        {
            for (int i = 1; i < dungeonProfile.levels.Count; i++)
            {
                CollapseCondition exitCond = dungeonProfile.levels[i - 1].Exit.condition;
                CollapseCondition entryCond = dungeonProfile.levels[i].Entry.condition;
                bool exitFit = exitCond.GetConditionAmount(ECondition.Pass) == 1 &&
                   exitCond.GetConditionAmount(ECondition.Wall) == 3;
                bool entryFit = entryCond.GetConditionAmount(ECondition.Pass) == 1 &&
                   entryCond.GetConditionAmount(ECondition.Wall) == 3;
                if (!entryFit || !exitFit)
                    return false;
            }
            return true;
        }


        void CreateDungeonObjects()
        {
            foreach (Branch item in graph.dungeonBranches)
            {
                foreach (Cell cell in item.cells)
                {
                    cell.CreatePrefabInstance();
                }
            }
        }
        public void CountRooms()
        {
            int roomCount = 0;
            for (int i = 0; i < dungeonProfile.levels.Count; i++)
            {
                roomCount += dungeonProfile.levels[i].Cells.Count;
            }
            Debug.Log("Rooms Count = " + roomCount);
        }
        public virtual void SaveDungeon()
        {
            DungeonData data = new DungeonData(dungeonProfile);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            string dataJson = JsonConvert.SerializeObject(data, settings);

            File.WriteAllText("Assets/test.json", dataJson);
        }
        public virtual void LoadDungeon()
        {
            string jsonData = File.ReadAllText("Assets/test.json");
            DungeonData dungeonData = JsonUtility.FromJson<DungeonData>(jsonData);
            dungeonProfile = new DungeonProfile(cellScale, levelHeight, this.restrictions);
            RebuildDungeon(dungeonData, dungeonProfile);
            OnDungeonGenerated?.Invoke(dungeonProfile);
        }
        protected virtual void RebuildDungeon(DungeonData data, DungeonProfile profile)
        {
            Transform Root = new GameObject("Rebuild Dungeon").transform;
            for (int i = 0; i < data.Levels.Count; i++)
            {
                Transform Branch = new GameObject("Branch " + i).transform;
                Branch.SetParent(Root);
                DungeonProfile.DungeonLevel level = new DungeonProfile.DungeonLevel();
                profile.levels.Add(level);
                level.BranchParent = Branch;
                for (int j = 0; j < data.Levels[i].LevelCells.Count; j++)
                {
                    DungeonData.DungeonCellData cell = data.Levels[i].LevelCells[j];

                    GameObject cellObjectPrefab = Options[cell.OptionID].Prefab;
                    GameObject cellObject = Instantiate(cellObjectPrefab);
                    cellObject.transform.SetParent(Branch);
                    cellObject.transform.position = cell.Position();
                    cellObject.transform.rotation = Quaternion.Euler(0, Options[cell.OptionID].RotatedAngle, 0);
                    RoomFillConfigData roomFillConfigData = cell.FillConfigData;
                    RoomFillConfig config = GetRoomConfiByID(cell.FillConfigData.roomFillId);
                    config.SetData(cell.FillConfigData);
                    DungeonRoomFill roomFill = cellObject.GetComponent<DungeonRoomFill>();
                    roomFill.ApplyConfig(config);
                    level.Cells.Add(new Cell(cell, cellObject));
                }
                //set neibhours 
                for (int j = 0; j < level.Cells.Count; j++)
                {

                }
            }
            for (int i = 1; i < data.Levels.Count; i++)
            {
                int stairsCellId = data.Levels[i - 1].ExitId;
                Vector3 targetPosition = data.Levels[i - 1].LevelCells[stairsCellId].Position() + new Vector3(0, 1, 0);

                GameObject stairsObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stairsObject.name = "stairs";
                stairsObject.transform.localScale = new Vector3(0.75f, 1.0f, 0.75f);
                stairsObject.transform.position = targetPosition - new Vector3(0, 0.5f, 0);
                stairsObject.transform.SetParent(Root);
            }
            for (int i = 0; i < data.Levels.Count; i++)
            {
                profile.levels[i].Entry = profile.levels[i].Cells[data.Levels[i].EntryId];
                profile.levels[i].Exit = profile.levels[i].Cells[data.Levels[i].ExitId];
                for (int j = 0; j < data.Levels[i].Rooms.Count; j++)
                {
                    profile.levels[i].Rooms.Add(new System.Collections.Generic.List<Cell>());
                    for (int k = 0; k < data.Levels[i].Rooms[j].Count; k++)
                    {
                        profile.levels[i].Rooms[j].Add(profile.levels[i].Cells[data.Levels[i].Rooms[j][k]]);
                    }
                }
                for (int j = 0; j < data.Levels[i].MultiplePass.Count; j++)
                {
                    profile.levels[i].MultiplePass.Add(profile.levels[i].Cells[data.Levels[i].MultiplePass[j]]);
                }
            }

        }
        public static void ColorCellObject(Color col, Cell item)
        {
            if (!Instance.colorDungeonParts) return;
            MeshRenderer[] rends = item.CellObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var rend in rends)
            {
                rend.material.color = rend.material.color * col;
            }
        }
        public RoomFillConfig GetRoomConfiByID(int id)
        {
            for (int i = 0; i < RoomConfigs.Length; i++)
            {
                if (RoomConfigs[i].RoomFillId == id)
                {
                    return RoomConfigs[i];
                }
            }
            Debug.Log("config not found for id = " + id);
            return null;
        }

    }
}
