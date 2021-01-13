using System;
using System.Net;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Gate)]
	public class EnterMapHandler : AMHandler<EnterMap_C2G>
	{
		protected override async ETTask Run(Session session, EnterMap_C2G message)
		{
			User user = session.GetComponent<SessionUserComponent>().User;
			
			// 在map服务器上创建战斗Unit
			IPEndPoint mapAddress = StartConfigComponent.Instance.MapConfigs[0].GetComponent<InnerConfig>().IPEndPoint;
			Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);

			mapSession.Send(new CreateUnit_G2M() { 
				UserId = user.UserId, 
				CharaId = message.CharaId,
				GActorId = user.InstanceId,
            	CActorId = user.GateSessionId
			});

			await ETTask.CompletedTask;
		}
	}
}