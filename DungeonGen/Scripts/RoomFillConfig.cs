using UnityEngine;
namespace WFC
{
    [CreateAssetMenu(fileName = "RoomFillConfig", menuName = "WFC/Room_Fill_Config", order = 1)]

    public class RoomFillConfig : ScriptableObject
    {
        public RoomFillConfigData data { private set; get; }
        public readonly int RoomFillId = 0;
        public virtual void SetData(RoomFillConfigData data)
        {
            this.data = data;
        }
    }

    [System.Serializable]
    public class RoomFillConfigData
    {
        public readonly int roomFillId = 0;
    }

}