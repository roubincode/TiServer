using ETModel;
using System.Linq;
using System.Collections.Generic;
namespace ETHotfix
{
    public static class RoomSystem
    {
        public static void InitRoom(this Room self)
        {
            // 添加一些组件
            // ...
        }

        public static void BroadcastSprits(this Room self){
            M2C_CreateSprites createSprites = new M2C_CreateSprites();
            if(MapHelper.mapSprites.TryGetValue(self.roomId,out List<Sprite> sprites)){
                foreach (Sprite sprite in sprites){
                    createSprites.SpriteInfos.Add(sprite.ToSprInfo());
                }
            }
            
            // 向进入地图的玩家发送初始场景中的网络实体（小怪，npc等）
            if(createSprites.SpriteInfos.Count>0){
                self.Broadcast(createSprites);
            }
        }

        public static void Add(this Room self, Player player)
        {
            if (!self.players.TryGetValue(player.UserId, out Player pl))
            {
                self.players.Add(player.UserId,player);
                self.BroadcastSprits();
            }
            else
            {
                Log.Error("玩家已存在");
            }
        }
        public static void Remove(this Room self, Player player)
        {
            Log.Info($"将{player.UserId}玩家从房间移除");
            if (self.players.TryGetValue(player.UserId, out Player pl))
            {
                self.players.Remove(player.UserId);
            }
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="message"></param>
        public static void Broadcast(this Room self, IActorMessage message)
        {
            foreach (KeyValuePair<long,Player> kp in self.players)
            {
                Player player = kp.Value;
                //如果玩家不存在或者不在线
                if (player == null)
                {
                    continue;
                }
                //向客户端User发送Actor消息
                ActorMessageSenderComponent actorProxyComponent = Game.Scene.GetComponent<ActorMessageSenderComponent>();
                ActorMessageSender actorProxy = actorProxyComponent.Get(player.CActorId);
                actorProxy.Send(message);
            }
        }

        public static void ClearRoom(this Room self){
            // RoomComponent roomComponet = Game.Scene.GetComponent<RoomComponent>();
            for(int i=0;i<self.players.Count;i++)
            {
                Entity entity = self.players[i];
                // roomComponet.rooms.Remove(rooms.roomId);
                entity.Dispose();
                self.Dispose();
            }
        }
    }
}
