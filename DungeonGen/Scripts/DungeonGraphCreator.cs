using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WFC;

public class DungeonGraphCreator
{
    List<Cell> CellsToCheck;
    public List<Branch> dungeonBranches { private set; get; }
    Transform dungeonParent;
    Cell[,] grid;
    DungeonProfile currentProfile;
    public DungeonGraphCreator(Cell[,] grid, Transform dungeonParent,DungeonProfile dungeonProfile)
    {
        this.grid = grid;
        CellsToCheck = new List<Cell>();
        ECondition[] connectedCond = new ECondition[] { ECondition.Pass, ECondition.RoomL, ECondition.RoomR };

        this.dungeonParent = dungeonParent;
        //check connected cells
        foreach (var item in grid)
        {
            foreach (var con in connectedCond)
            {
                if (CheckCellForCondition(item, con))
                {
                    if (!CellsToCheck.Contains(item))
                        CellsToCheck.Add(item);
                }
            }
        }
        currentProfile = dungeonProfile;

        dungeonBranches = CreateGraph();
        ReparentBranches(dungeonBranches);
        RepositionBranches(dungeonBranches);
        CleanUp();
    }
    bool CheckCellForCondition(Cell cell, ECondition cond)
    {
        return (cell.condition.Top == cond ||
                cell.condition.Bottom == cond ||
                cell.condition.Left == cond ||
                cell.condition.Right == cond);
    }
    void CleanUp()
    {
        foreach (Cell item in grid)
        {
            bool toRemove = true;
            for (int i = 0; i < dungeonBranches.Count; i++)
            {
                if (dungeonBranches[i].cells.Contains(item))
                {
                    toRemove = false;
                    break;
                }
            }
            if (toRemove)
                item.Destroy();
        }
    }
    private void RemoveDuoBranches(List<Branch> branches)
    {
        for (int i = 0; i < branches.Count; i++)
        {
            if (branches[i].cells.Count == 2)
            {
                Debug.LogWarning("Removed cell" + branches[i].cells[0].CellObject.name);
                Debug.LogWarning("Removed cell" + branches[i].cells[1].CellObject.name);

                branches[i].Destroy();
                branches.Remove(branches[i]);
                i--;
            }
        }
    }
    public void ReparentBranches()
    {
        ReparentBranches(dungeonBranches);
    }
    void ReparentBranches(List<Branch> graph)
    {
        for (int i = 0; i < graph.Count; i++)
        {

            DungeonProfile.DungeonLevel level = new DungeonProfile.DungeonLevel();
            currentProfile.levels.Add(level);

            Branch branch = graph[i];
            branch.FindEnterAndExit();
            level.Entry = branch.cells[0];
            level.Exit = branch.cells[1];

            branch.CreateContainer(dungeonParent, "Branch " + i);
            branch.ReparentCells();
            level.BranchParent = branch.rootObject;
            level.Cells = branch.cells;
        }
    }
    public void RepositionBranches()
    {
        RepositionBranches(dungeonBranches);
    }
    void RepositionBranches(List<Branch> graph)
    {
        graph.Sort((a, b) => b.cells.Count.CompareTo(a.cells.Count));

        for (int i = 1; i < graph.Count; i++)
        {
            Vector3 targetPosition = graph[i - 1].cells[1].CellObject.transform.position + new Vector3(0, 1, 0);
            graph[i].rootObject.transform.position = targetPosition;
            GameObject stairsObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairsObject.name = "stairs";
            stairsObject.transform.localScale = MultiplyVector(stairsObject.transform.localScale, new Vector3(0.75f, 1.0f, 0.75f));
            stairsObject.transform.position = targetPosition - new Vector3(0, 0.5f, 0);
            stairsObject.transform.SetParent(graph[i].rootObject.transform);
        }
    }
   public List<Branch> CreateGraph()
    {
        List<Branch> branches = new List<Branch>();
        int cellCount = CellsToCheck.Count;
        while (cellCount > 0)
        {
            Cell root = CellsToCheck[0];
            List<Cell> temp = new List<Cell>();
            
            List<ECondition> ConnectedConditions = new List<ECondition>
            {
                ECondition.Pass,
                ECondition.RoomL,
                ECondition.RoomR,
                ECondition.Room
            };
            Branch.GrowBranch(root, temp, ConnectedConditions);

            Color col = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            foreach (var item in temp)
            {
                DungeonManager.ColorCellObject(col, item);
                CellsToCheck.Remove(item);
            }
            branches.Add(new Branch(temp));
            cellCount = CellsToCheck.Count;
        }
        RemoveDuoBranches(branches);
        RemoveBigRoomsNoConnect(branches);
        return branches;
    }

    private void RemoveBigRoomsNoConnect(List<Branch> branches)
    {
        for (int i = 0; i < branches.Count; i++)
        {
            if (branches[i].cells.Count == 4 || branches[i].cells.Count == 6)
            {
                int passes = 0;
                for (int j = 0; j < branches[i].cells.Count; j++)
                {
                    passes += branches[i].cells[j].condition.GetConditionAmount(ECondition.Pass);

                }
                if (passes == 0)
                {
                    branches.RemoveAt(i);
                    Debug.LogWarning("remove BigRoom no connections");
                    i--;
                }
            }
        }
    }

    public static Vector3 MultiplyVector(Vector3 a, Vector3 b)
    {
        Vector3 result = a;
        result.x *= b.x;
        result.y *= b.y;
        result.z *= b.z;
        return result;
    }
}