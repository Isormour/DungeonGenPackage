using UnityEngine;

public class DungeonCreatorPage 
{
    string name;
    public DungeonCreatorPage(string name)
    {
        this.name = name;
    }
    public virtual void InitPage()
    {
        
    }
    public virtual void Draw()
    {
        GUILayout.Label(name);
        GUILayout.Space(10);
    }
}
