using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using WFC;

public class DungeonCreator
{
    public int width { private set; get; } = 10;
    public int height { private set; get; } = 10;
    public Cell[,] grid { private set; get; }
    public List<Cell> cells { private set; get; }
    public CollapseOption[] collapseOptions { private set; get; }

    public Transform dungeonParent { private set; get; }

    DungeonProfile profile;

    List<Requrement> currentRequrements;
    Requrement[] totalRequirement;
    public DungeonCreator(CollapseOption[] collapseOptions, int width, int height, DungeonProfile profile)
    {
        this.width = width;
        this.height = height;
        this.collapseOptions = collapseOptions;
        this.profile = profile;
        InitGrid();
        totalRequirement = profile.requrementsData.requrements;
        currentRequrements = profile.requrementsData.requrements.ToList();
    }

    void InitGrid()
    {
        //fill grid
        cells = new List<Cell>();
        grid = new Cell[width, height];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new Cell(i, j, this.collapseOptions);
                cells.Add(grid[i, j]);
            }
        }

        // set neighbours;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                Cell top = GetCellFromGrid(i, j + 1);
                Cell bottom = GetCellFromGrid(i, j - 1);
                Cell left = GetCellFromGrid(i - 1, j);
                Cell right = GetCellFromGrid(i + 1, j);
                grid[i, j].SetNeighbours(top, bottom, left, right);
            }
        }
    }
    Cell GetCellFromGrid(int x, int y)
    {
        Cell cell = null;
        bool xInRange = x >= 0 && x < grid.GetLength(0);
        bool yInRange = y >= 0 && y < grid.GetLength(1);
        if (xInRange && yInRange)
        {
            cell = grid[x, y];
        }
        return cell;
    }


    public void GenerateBorder()
    {
        if (dungeonParent == null)
        {
            dungeonParent = new GameObject("dungeonParent").transform;
        }
        InitGrid();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                bool isBorder = i == 0 || j == 0 || j == grid.GetLength(1) - 1 || i == grid.GetLength(0) - 1;
                if (isBorder)
                {
                    grid[i, j].Collapse(dungeonParent, new CollapseOption[1] { collapseOptions[0] }, false);
                    cells.Remove(grid[i, j]);
                    cells.OrderByDescending(c => c.EntropyValue);
                }
            }
        }
    }
    public void Generate(bool createWorldObject)
    {
        if (cells.Count == 0)
        {
            return;
        }
        if (dungeonParent == null)
        {
            dungeonParent = new GameObject("dungeonParent").transform;
        }
        if (currentRequrements.Count > 0)
        {
            GenerateRequirements();
            return;
        }

        cells[0].Collapse(dungeonParent, createWorldObject);

        cells.RemoveAt(0);
        cells = cells.OrderBy(c => c.EntropyValue).ToList();
    }
    void GenerateRequirements()
    {
        for (int i = currentRequrements.Count-1; i >=0 ; i--)
        {
            Requrement req = currentRequrements[i];
            PlaceReqiredOption(req);
            currentRequrements.RemoveAt(i);
        }
    }

    private void PlaceReqiredOption(Requrement req)
    {
      
        foreach (CollapseOption opt in req.Options)
        {
            int tries = 0;
            bool dontFit = false;
            while (!dontFit)
            {
                int x = UnityEngine.Random.Range(1, grid.GetLength(0) - 1);
                int y = UnityEngine.Random.Range(1, grid.GetLength(0) - 1);

                if (grid[x, y].collapsed)
                {
                    continue;
                }
                if (grid[x, y].GetCollapseCondition().CheckCondition(req.Options[0].Condition))
                {
                    grid[x, y].Collapse(dungeonParent, new CollapseOption[] { req.Options[0] }, true);
                    cells.Remove(grid[x, y]);
    #if UNITY_EDITOR
                    grid[x, y].CellObject.transform.Find("Cube (2)").GetComponent<MeshRenderer>().material.color = Color.white;
                    grid[x, y].CellObject.transform.name += " req";
                    Debug.Log("placed for x= " + x + "  y= "+y);
    #endif
                    dontFit = true;
                }
                tries++;
                if (tries >= 1000)
                {
                    dontFit = true;
                    Debug.LogWarning("no space for option");
                }
            }
        }
    }

    public void GenerateAll(bool createWorldObject)
    {
        dungeonParent = new GameObject("dungeonParent").transform;
        totalRequirement = profile.requrementsData.requrements;
        currentRequrements = profile.requrementsData.requrements.ToList();
        InitGrid();
        GenerateBorder();
        while (cells.Count > 0)
        {
            Generate(createWorldObject);
        }
    }
}


