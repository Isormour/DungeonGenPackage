using System;
using UnityEditor;
using UnityEngine;
using WFC;
internal class TestingPage : DungeonCreatorPage
{
    int cond;
    int mask = 0;
    bool checkResult = false;
    bool hasChecked = false;
    CollapseOption opt;
    bool drawOptionData = false;
    public TestingPage(string name) : base(name)
    {
    }
    public override void Draw()
    {
        base.Draw();
        
        Enum enumMask = (ECondition)cond;
        cond = (int)(ECondition)EditorGUILayout.EnumFlagsField("first", enumMask);
        
        enumMask = (ECondition)mask;
        mask = (int)(ECondition)EditorGUILayout.EnumFlagsField("Secound", enumMask);

        opt = (CollapseOption)EditorGUILayout.ObjectField("option", opt,typeof(CollapseOption));

        if (GUILayout.Button("test flags"))
        {
            hasChecked = true;
            checkResult = enumMask.HasFlag((ECondition)cond);
        }
        if (GUILayout.Button("check options"))
        {
            drawOptionData = !drawOptionData;
            
        }
        if (drawOptionData&& opt!=null)
        {
            GUILayout.Label("top cond = " + opt.Condition.Top);
            GUILayout.Label("bottom cond = " + opt.Condition.Bottom);
            GUILayout.Label("left cond = " + opt.Condition.Left);
            GUILayout.Label("right cond = " + opt.Condition.Right);
        }
        if (hasChecked)
        {
            GUILayout.Label(checkResult ? "positive" : "negative");
        }
    }
}