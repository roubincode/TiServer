using MongoDB.Bson.Serialization.Attributes;
namespace ETModel
{
    [ObjectSystem]
    public class GamerAwakeSystem : AwakeSystem<Player, long>
    {
        public override void Awake(Player self, long userid)
        {
            self.Awake(userid);
        }
    }

    [ObjectSystem]
    public class PlayerStartSystem : StartSystem<Player>
    {
        public override void Start(Player self)
        {
            self.Start();
        }
    }

    /// <summary>
    /// 房间玩家对象
    /// </summary>
    public partial class Player : Entity
    {
        public long UserId { get;set; }
        public long CharaId { get; set; }

        /// 玩家GateActorId,是网关User的实例id
        public long GActorId { get; set; }

        /// 玩家ClientActorId,是网关session的实例id
        public long CActorId { get; set; }
        
        /// <summary>
        /// 默认为假 Session断开/离开房间时触发离线
        /// </summary>
        public bool isOffline { get; set; }


        public Entity nextTarget { get; set; }

        internal bool respawnRequested;
        public void CmdRespawn() { 
            respawnRequested = true; 
        }

        internal bool cancelActionRequested;
        public void CmdCancelAction() { 
            cancelActionRequested = true; 
        }

        public void Awake(long userid)
        {
            this.UserId = userid;
            
        }

        public void Start(){
            Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(this.UnitId);
            this.movement = unit.GetComponent<CharacterMoveComponent>();
            this.skillsCom = GetComponent<PlayerSkillsComponent>();
            this.combat = GetComponent<CombatComponent>();
            this.level = GetComponent<Level>();
            this.health = GetComponent<Health>();
            this.mana = GetComponent<Mana>();
        }
        
        public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			this.UserId = 0;
            this.GActorId = 0;
            this.isOffline = false;
		}
    }
}