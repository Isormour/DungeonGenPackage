using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(fileName = "Collapse_Option", menuName = "WFC/Collapse_Option", order = 1)]
    public class CollapseOption : ScriptableObject
    {
        public GameObject Prefab;
        public CollapseCondition Condition;
        public int RotatedAngle = 0;
    }
}

