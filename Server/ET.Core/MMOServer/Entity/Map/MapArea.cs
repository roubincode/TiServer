using System.Collections.Generic;
using System.Threading;

namespace ETModel
{
    [ObjectSystem]
    public class InstanceAwakeSystem : AwakeSystem<MapArea,RoomConfig>
    {
        public override void Awake(MapArea self, RoomConfig config)
        {
            self.Awake(config);
        }
    }
    public sealed class MapArea : Room
    {

        public override void Awake(RoomConfig config)
        {
            this.config = config;
            this.roomId = config.roomId;
        }

        
    }
}
