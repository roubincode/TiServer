using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class SelectTargetHandler : AMActorLocationHandler<Unit, C2M_SelectTarget>
	{
		protected override async ETTask Run(Unit unit, C2M_SelectTarget message)
		{
            if(message.TargetNetId>0){
                Entity target = NetworkIdentity.Get((uint)message.TargetNetId).entity;
                unit.player.target = unit.target = target;
            }else{
                unit.player.target = unit.target = null;
            }
            
            // 广播技能消息
            MapHelper.Broadcast(new M2C_SelectTarget{
                UnitId = unit.Id,
                TargetNetId = message.TargetNetId
            },unit.Id);

			await ETTask.CompletedTask;
        }
	}
}