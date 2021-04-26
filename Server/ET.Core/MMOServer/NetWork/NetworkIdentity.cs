using System;
using System.Collections.Generic;
namespace ETModel
{
    // 网络身份组件
    // 可验证网络身份是否当前角色

    [ObjectSystem]
    public class NetworkIdentityStartSystem : StartSystem<NetworkIdentity>
    {
        public override void Start(NetworkIdentity self)
        {
            self.Start();
        }
    }

    public sealed class NetworkIdentity : Component
    {
        public Entity entity;
        public uint netId { get; internal set; }
        public bool hasAuthority { get; internal set; }

        public static readonly Dictionary<uint, NetworkIdentity> spawned = new Dictionary<uint, NetworkIdentity>();

        public void Start()
        {
            entity = this.GetParent<Entity>();
        }

        public static NetworkIdentity Get(uint netid){
            if (spawned.TryGetValue(netid, out NetworkIdentity identity))
            {
                return identity;
            }
            else
            {
                return null;
            }
        }

        public void OnSpawned()
        {
            netId = GetNextNetworkId();

            // add to spawned
            spawned[netId] = this;
        }

        static uint freeNetWorkId  = 0;
        static uint nextNetworkId = 0;
        internal static uint GetNextNetworkId() {
            uint id = NextNetworkId();
            freeNetWorkId = 0;
            return id;
        }

        internal static uint NextNetworkId(){
            if(freeNetWorkId > 0 )
                return freeNetWorkId;
            else return ++nextNetworkId;
        }

        public override void Dispose()
        {
            base.Dispose();
            // ===> 移除网络Identity对象
            freeNetWorkId = this.netId;
            spawned.Remove(this.netId);
        }
        
    }
}