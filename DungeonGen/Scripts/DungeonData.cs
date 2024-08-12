using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WFC;

[System.Serializable]
public class DungeonData
{
    public string name = "Dungeon";
    public List<DungeonLevelData> Levels;

    [System.Serializable]
    public class DungeonLevelData
    {
        public List<DungeonCellData> LevelCells;
        public int EntryId;
        public int ExitId;

        public List<List<int>> Rooms;
        public List<int> SinglePass;
        public List<int> MultiplePass;
        public DungeonLevelData(DungeonProfile.DungeonLevel level)
        {
            List<Cell> CellToId = level.Cells.ToList();
            //pass all cells
            LevelCells = new List<DungeonCellData>();
            for (int i = 0; i < level.Cells.Count; i++)
            {
                LevelCells.Add(new DungeonCellData(level.Cells[i]));
            }

            //room pass ids
            Rooms = new List<List<int>>();
            for (int i = 0; i < level.Rooms.Count; i++)
            {
                Rooms.Add(new List<int>());
                for (int j = 0; j < level.Rooms[i].Count; j++)
                {
                    Rooms[i].Add(CellToId.IndexOf(level.Rooms[i][j]));
                }
            }
            SinglePass = new List<int>();
            for (int i = 0; i < level.SinglePass.Count; i++)
            {
                SinglePass.Add(CellToId.IndexOf(level.SinglePass[i]));
            }
            MultiplePass = new List<int>();
            for (int i = 0; i < level.MultiplePass.Count; i++)
            {
                MultiplePass.Add(CellToId.IndexOf(level.MultiplePass[i]));
            }

            EntryId = CellToId.IndexOf(level.Entry);
            ExitId = CellToId.IndexOf(level.Exit);
        }
    }
    [System.Serializable]
    public class DungeonCellData
    {
        public int OptionID;// id=0 -> entry, id==1 -> exit
        public Vector3 Position;
        public DungeonCellData(Cell cell)
        {
            List<CollapseOption> Options = DungeonManager.Instance.options.ToList();
            OptionID = Options.IndexOf(cell.currentOption);
            Position = cell.CellObject.transform.position;
        }
    }
    public DungeonData(DungeonProfile profile)
    {
        Levels = new List<DungeonLevelData>();
        for (int i = 0; i < profile.levels.Count; i++)
        {
            DungeonLevelData levelData = new DungeonLevelData(profile.levels[i]);
            Levels.Add(levelData);
        }
    }
}
