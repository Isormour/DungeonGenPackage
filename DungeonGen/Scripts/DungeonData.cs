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
            for (int i = 0; i < LevelCells.Count; i++)
            {
                LevelCells[i].SetNeibhours(level.Cells[i], level.Cells);
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
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public RoomFillConfigData FillConfigData;
        public int CellNeibhourTop = -1;
        public int CellNeibhourBottom = -1;
        public int CellNeibhourLeft = -1;
        public int CellNeibhourRight = -1;
        public CollapseCondition condition;
        public Vector3 Position()
        {
            return new Vector3(PositionX, PositionY, PositionZ);
        }
        public DungeonCellData(Cell cell)
        {
            List<CollapseOption> Options = DungeonManager.Instance.Options.ToList();
            OptionID = Options.IndexOf(cell.currentOption);

            PositionX = cell.CellObject.transform.position.x;
            PositionY = cell.CellObject.transform.position.y;
            PositionZ = cell.CellObject.transform.position.z;

            if (cell.CellObject && cell.CellObject.GetComponent<DungeonRoomFill>())
            {
                if (cell.CellObject.GetComponent<DungeonRoomFill>().config)
                    FillConfigData = cell.CellObject.GetComponent<DungeonRoomFill>().config.data;
            }
            condition = cell.condition;
        }
        public void SetNeibhours(Cell cell, List<Cell> Cells)
        {
            if (cell.Neibhours[0] != null)
            {
                CellNeibhourTop = Cells.IndexOf(cell.Neibhours[0]);
            }
            if (cell.Neibhours[1] != null)
            {
                CellNeibhourBottom = Cells.IndexOf(cell.Neibhours[1]);
            }
            if (cell.Neibhours[2] != null)
            {
                CellNeibhourLeft = Cells.IndexOf(cell.Neibhours[2]);
            }
            if (cell.Neibhours[3] != null)
            {
                CellNeibhourRight = Cells.IndexOf(cell.Neibhours[3]);
            }
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
