using System.Collections.Generic;
using UnityEngine;
using WFC;

public partial class DungeonCreatorFixRooms : DungeonCreatorPage
{
    DungeonGraphCreator graphCreator;
    public DungeonCreatorFixRooms(string name, DungeonManager manager) : base(name)
    {
        graphCreator = manager.graph;
    }
    public override void InitPage()
    {
        base.InitPage();
    }
    public override void Draw()
    {
        base.Draw();
        if (GUILayout.Button("Create graph"))
        {
            graphCreator.CreateGraph();
        }
        if (GUILayout.Button("reparent branches"))
        {
            graphCreator.ReparentBranches();
        }
        if (GUILayout.Button("reposition branches"))
        {
            graphCreator.RepositionBranches();
        }
    }

}
