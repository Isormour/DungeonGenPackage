using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC
{
    public class DungeonRoomInterpreter
    {
        DungeonProfile dungeonProfile;
        public DungeonRoomInterpreter(DungeonProfile dungeonProfile)
        {
            this.dungeonProfile = dungeonProfile;
            dungeonProfile.levels.Sort((a, b) => b.Cells.Count.CompareTo(a.Cells.Count));
            FindRooms();
        }
        void FindRooms()
        {
            FindBigRooms();
            FindPassRooms();
        }

        private void FindPassRooms()
        {
            for (int i = 0; i < dungeonProfile.levels.Count; i++)
            {
                DungeonProfile.DungeonLevel level = dungeonProfile.levels[i];
                List<Cell> CellsToCheck = level.Cells.ToList();
                for (int j = 0; j < CellsToCheck.Count; j++)
                {
                    CollapseCondition cond = CellsToCheck[j].currentOption.Condition;
                    int passAmount = cond.GetConditionAmount(ECondition.Pass);
                    int roomsAmount = cond.GetConditionAmount(ECondition.RoomL);
                    if (roomsAmount == 0)
                        switch (passAmount)
                        {
                            case 1:
                                level.SinglePass.Add(CellsToCheck[j]);
                                break;
                            case 2:
                                level.MultiplePass.Add(CellsToCheck[j]);
                                break;
                            case 3:
                                level.MultiplePass.Add(CellsToCheck[j]);
                                break;
                            case 4:
                                level.MultiplePass.Add(CellsToCheck[j]);
                                break;
                        }
                }
            }
        }
        private void FindBigRooms()
        {
            for (int i = 0; i < dungeonProfile.levels.Count; i++)
            {
                List<Cell> CellsToCheck = dungeonProfile.levels[i].Cells.ToList();
                List<Cell> Room = new List<Cell>();
                int cellCount = CellsToCheck.Count;
                while (cellCount > 0)
                {
                    Cell root = CellsToCheck[0];
                    List<Cell> temp = new List<Cell>();

                    List<ECondition> ConnectedConditions = new List<ECondition>
                    {
                        ECondition.RoomL,
                        ECondition.RoomR,
                        ECondition.Room
                    };
                    Branch.GrowBranch(root, temp, ConnectedConditions);
                    CellsToCheck.Remove(root);
                    Color col = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    foreach (var item in temp)
                    {
                        DungeonManager.ColorCellObject(col, item);
                        CellsToCheck.Remove(item);
                    }
                    if (temp.Count > 1)
                    {
                        dungeonProfile.levels[i].Rooms.Add(temp);
                    }
                    cellCount = CellsToCheck.Count;
                }
            }
        }
    }
}