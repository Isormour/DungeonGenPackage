using System.Collections.Generic;
using UnityEngine;
using WFC;

public class DungeonProfile
{
    public List<DungeonLevel> levels;
    public DungeonRequirments requrementsData;
    public float cellSize { private set; get; } = 1;
    public float levelHeight { private set; get; } = 1;

    public DungeonProfile(float cellSize, float levelHeight, DungeonRequirments requrementsData)
    {
        levels = new List<DungeonLevel>();
        this.cellSize = cellSize;
        this.levelHeight = levelHeight;
        this.requrementsData = requrementsData;
    }
    public class DungeonLevel
    {
        public Transform BranchParent;
        public List<Cell> Cells;
        public Cell Entry;
        public Cell Exit;
        public List<List<Cell>> Rooms;
        public List<Cell> SinglePass;
        public List<Cell> MultiplePass;

        public DungeonLevel()
        {
            Cells = new List<Cell>();
            Rooms = new List<List<Cell>>();
            SinglePass = new List<Cell>();
            MultiplePass = new List<Cell>();
        }
    }

}