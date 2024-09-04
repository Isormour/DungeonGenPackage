using UnityEngine;
namespace WFC
{

    public class DungeonRoomFill : MonoBehaviour
    {
        public RoomFillConfig config { private set; get; }
        // Start is called before the first frame update
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }
        public virtual void ApplyConfig(RoomFillConfig config)
        {
            this.config = config;
        }
    }
}
