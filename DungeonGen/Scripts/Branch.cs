using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    public class Branch
    {
        public List<Cell> cells { private set; get; }
        public Transform rootObject;
        public Cell StairsCell { private set; get; }
        public Branch(List<Cell> cells)
        {
            this.cells = cells;
        }
        public void FindEnterAndExit()
        {
            Cell stairsCell = cells[0];
            Cell exitCell = cells[1];
            //find entry branch

            List<Cell> singleRooms = FindAllCellsWithConditions(new List<ECondition> { ECondition.Pass, ECondition.Wall }, new List<int> { 1, 3 });
            List<Cell> otherRooms = FindAllCellsWithConditions(new List<ECondition> { ECondition.Pass }, new List<int> { 2 });

            if (singleRooms.Count > 0)
            {
                stairsCell = singleRooms[Random.Range(0, singleRooms.Count)];
                singleRooms.Remove(stairsCell);
            }
            else if (otherRooms.Count > 0)
            {
                stairsCell = otherRooms[Random.Range(0, otherRooms.Count)];
                otherRooms.Remove(stairsCell);
            }

            cells.Remove(stairsCell);
            cells.Insert(0, stairsCell);

            Color col = stairsCell.CellObject.GetComponentsInChildren<MeshRenderer>()[1].material.color;
            DungeonManager.ColorCellObject(col + (col * 0.5f), stairsCell);

            if (singleRooms.Count > 0)
            {
                exitCell = singleRooms[Random.Range(0, singleRooms.Count)];
                singleRooms.Remove(exitCell);
            }
            else if (otherRooms.Count > 0)
            {
                exitCell = otherRooms[Random.Range(0, otherRooms.Count)];
                otherRooms.Remove(exitCell);
            }

            cells.Remove(exitCell);
            cells.Insert(0, exitCell);

            cells.Remove(stairsCell);
            cells.Insert(0, stairsCell);

            DungeonManager.ColorCellObject(col - (col * 0.5f), exitCell);
            StairsCell = stairsCell;
            
        }

        List<Cell> FindAllCellsWithConditions(List<ECondition> conds, List<int> numberOfOccurances)
        {
            List<Cell> fits = new List<Cell>();
            for (int i = 0; i < cells.Count; i++)
            {
                fits.Add(cells[i]);
                for (int j = 0; j < conds.Count; j++)
                {
                    if (cells[i].condition.GetConditionAmount(conds[j]) == numberOfOccurances[j])
                    {
                        continue;
                    }
                    if (fits.Contains(cells[i]))
                    {
                        fits.Remove(cells[i]);
                        break;
                    }
                }
            }
            return fits;
        }
        public void CreateContainer(Transform MainParent, string name)
        {
            GameObject branchParent = new GameObject(name);
            branchParent.transform.position = StairsCell.CellObject.transform.position;
            branchParent.transform.SetParent(MainParent);
            rootObject = branchParent.transform;
        }

        public void ReparentCells()
        {
            foreach (var cell in cells)
            {
                cell.CellObject.transform.SetParent(rootObject);
            }
        }

        public void Destroy()
        {
#if UNITY_EDITOR
            foreach (Cell cell in cells)
            {
                cell.Destroy();
            }
#endif
        }
       public static void GrowBranch(Cell cell, List<Cell> branch, List<ECondition> conds)
        {
            List<Cell> tempNeibhours = cell.GetConnectedNeibhours(conds);

            foreach (var item in tempNeibhours)
            {
                if (!branch.Contains(item))
                {
                    branch.Add(item);
                    GrowBranch(item, branch,conds);
                }
            }
        }
    }
}