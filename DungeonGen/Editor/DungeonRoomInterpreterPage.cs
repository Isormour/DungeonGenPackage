using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WFC;

internal class DungeonRoomInterpreterPage : DungeonCreatorPage
{
    private string v;
    private DungeonManager dungeonManager;
    List<LevelDrawer> drawers = new List<LevelDrawer>();

    public DungeonRoomInterpreterPage(string v, DungeonManager dungeonManager):base (v)
    {
        this.v = v;
        this.dungeonManager = dungeonManager;
        foreach (var item in this.dungeonManager.dungeonProfile.levels)
        {
            drawers.Add(new LevelDrawer(item));
        }
    }
    public override void Draw()
    {
        base.Draw();
        for (int i = 0; i < drawers.Count; i++)
        {
            drawers[i].Draw();
        }
    }
}
class LevelDrawer
{
    public bool drawSpecifics = false;
    DungeonProfile.DungeonLevel levelToDraw;
    public LevelDrawer(DungeonProfile.DungeonLevel level)
    {
        this.levelToDraw = level;
    }
    public void Draw()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(levelToDraw.BranchParent.name);
        drawSpecifics = EditorGUILayout.Toggle("Show", drawSpecifics);
        EditorGUILayout.EndHorizontal();
        if (drawSpecifics)
        {
            GUILayout.Space(2);
            GuiLine(1);
            GUILayout.Label("CellCount = "+ levelToDraw.Cells.Count);
            GUILayout.Label("Rooms big = " + levelToDraw.Rooms.Count);
            GUILayout.Label("Rooms small = " + levelToDraw.SinglePass.Count);
            GUILayout.Label("Rooms pass = " + levelToDraw.MultiplePass.Count);
            GuiLine(1);
            GUILayout.Space(10);
        }
    }
    void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
