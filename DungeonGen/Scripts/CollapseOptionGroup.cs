

using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(fileName = "Collapse_Option_group", menuName = "WFC/Collapse_Option_group", order = 1)]
    public class CollapseOptionGroup : ScriptableObject
    {
        public CollapseOption[] Group;
    }
}