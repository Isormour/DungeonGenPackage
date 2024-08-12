using System;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(fileName = "Conditions_Config", menuName = "WFC/Conditions_config")]
    public class ConditionsConfig : ScriptableObject
    {
        public Condition[] conditions;

        [Serializable]
        public class Condition
        {
            public ECondition condition;
            public int Mask;
        }
        public int GetMask(ECondition condition)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].condition == condition)
                {
                    return conditions[i].Mask;
                }
            }
            return -1;
        }
    }
}
