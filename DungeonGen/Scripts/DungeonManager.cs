using System.IO;
using UnityEngine;

namespace WFC
{
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance;
        public ConditionsConfig conditionConfig;
        public CollapseOption[] options;
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

        private void Awake()
        {


            Initialize();
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
            dungeonProfile = new DungeonProfile(cellScale, levelHeight);
            dungeonProfile.requrementsData = restrictions;
        }
        // Start is called before the first frame update
        void Start()
        {
            LoadDungeon();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void CreateDungeon(bool createWorldObject)
        {
            creator = new DungeonCreator(options, sizeX, sizeY, dungeonProfile);
            creator.GenerateAll(createWorldObject);
            graph = new DungeonGraphCreator(creator.grid, creator.dungeonParent, dungeonProfile);
            roomInterpreter = new DungeonRoomInterpreter(dungeonProfile);
            CountRooms();
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
        public void SaveDungeon()
        {
            DungeonData data = new DungeonData(dungeonProfile);

            string dataJson = JsonUtility.ToJson(data);
            File.WriteAllText("Assets/test.json", dataJson);
        }
        public void LoadDungeon()
        {
            string jsonData = File.ReadAllText("Assets/test.json");
            DungeonData dungeonData = JsonUtility.FromJson<DungeonData>(jsonData);
            RebuildDungeon(dungeonData);
        }
        void RebuildDungeon(DungeonData data)
        {
            Transform Root = new GameObject("Rebuild Dungeon").transform;
            for (int i = 0; i < data.Levels.Count; i++)
            {
                Transform Branch = new GameObject("Branch " + i).transform;
                Branch.SetParent(Root);
                for (int j = 0; j < data.Levels[i].LevelCells.Count; j++)
                {
                    DungeonData.DungeonCellData cell = data.Levels[i].LevelCells[j];
                    GameObject cellObjectPrefab = options[cell.OptionID].Prefab;
                    GameObject cellObject = Instantiate(cellObjectPrefab);
                    cellObject.transform.SetParent(Branch);
                    cellObject.transform.position = cell.Position;
                    cellObject.transform.rotation = Quaternion.Euler(0, options[cell.OptionID].RotatedAngle, 0);
                }
            }
            for (int i = 1; i < data.Levels.Count; i++)
            {
                int stairsCellId = data.Levels[i - 1].ExitId;

                Vector3 targetPosition = data.Levels[i - 1].LevelCells[stairsCellId].Position + new Vector3(0, 1, 0);

                GameObject stairsObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stairsObject.name = "stairs";
                stairsObject.transform.localScale = new Vector3(0.75f, 1.0f, 0.75f);
                stairsObject.transform.position = targetPosition - new Vector3(0, 0.5f, 0);
                stairsObject.transform.SetParent(Root);

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
    }
}
