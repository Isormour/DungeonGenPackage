using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(fileName = "Dungeon_Requrement", menuName = "WFC/Dungeon_Requrement")]
    public class DungeonRequirments : ScriptableObject
    {
        public Requrement[] requrements;
    }
    [System.Serializable]
    public class Requrement
    {
        public CollapseOption[] Options;
    }
    
}