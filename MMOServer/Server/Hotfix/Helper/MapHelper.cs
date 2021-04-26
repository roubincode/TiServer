using ETModel;
using System.Net;
using Google.Protobuf;
using System.Collections.Generic;
namespace ETHotfix
{
    public static class MapHelper
    {
        public static Dictionary<long, List<Sprite>> mapSprites = new Dictionary<long, List<Sprite>>();
        public static Session GetGateSession()
        {
            StartConfigComponent config = Game.Scene.GetComponent<StartConfigComponent>();
            IPEndPoint gateIPEndPoint = config.GateConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
            //Log.Debug(gateIPEndPoint.ToString());
            Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gateIPEndPoint);
            return gateSession;
        }

        public static void Broadcast(IActorMessage message,long id)
        {
            // 这里是向整个房间发消息
            // AOI（Area Of Interest）十字链表或九宫格逻辑需要另外实现
            Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(id);
            unit.room.Broadcast(message);
        }

        public static void WrapSprite(Sprite sprite){
            sprite.AddComponent<NetworkIdentity>();
            sprite.GetComponent<NetworkIdentity>().OnSpawned();

            // 组件
            sprite.AddComponent<Level>();
            sprite.AddComponent<Mana>();
            sprite.AddComponent<Health>();
            sprite.AddComponent<CombatComponent>();

            // 属性
            sprite.GetComponent<Mana>().canRecover = false;
            sprite.GetComponent<Health>().canRecover = false;
            sprite.level = sprite.GetComponent<Level>();
            sprite.level.max = 50;
            sprite.level.current = 20;
			sprite.mana = sprite.GetComponent<Mana>();
			sprite.health = sprite.GetComponent<Health>();
            sprite.health.baseHealth = new LinearInt(){baseValue = sprite.baseHealth,bonusPerLevel = sprite.perHealth};
            sprite.health.spawnFull = true;
			sprite.identity = sprite.GetComponent<NetworkIdentity>();
            sprite.netId = sprite.identity.netId;
            sprite.combat = sprite.GetComponent<CombatComponent>();
            // sprite.AddComponent<CharacterMoveComponent>();
        }

    }
}