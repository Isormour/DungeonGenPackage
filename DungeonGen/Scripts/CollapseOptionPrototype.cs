using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(fileName = "Collapse_Condition_Prototype", menuName = "WFC/Collapse_Condition_Prototype")]
    public class CollapseOptionPrototype : ScriptableObject
    {
        public GameObject Prefab;
        public CollapseCondition Condition;
        public bool IsRotateable;
        public bool IsSymetric;
    }
}