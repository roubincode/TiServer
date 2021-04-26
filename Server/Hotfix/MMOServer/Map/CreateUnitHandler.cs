using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler(AppType.Map)]
	public class CreateUnitsHandler : AMHandler<CreateUnit_G2M>
	{
		protected override async ETTask Run(Session session, CreateUnit_G2M message)
		{
			// 查询玩家角色信息中是否已有地图与位置坐标数据
			// ...

			Unit unit = ComponentFactory.CreateWithId<Unit>(IdGenerater.GenerateId());
			unit.Position = new Vector3(-10, 0, -10);

			// unit添加unit状态组件与角色移动组件
			unit.AddComponent<UnitStateComponent>();
			unit.AddComponent<CharacterMoveComponent>();
			unit.AddComponent<NetworkIdentity>();
			
			// unit添加到UnitComponet，添加MailBoxComponent
			await unit.AddComponent<MailBoxComponent>().AddLocation();
			Game.Scene.GetComponent<UnitComponent>().Add(unit);
			

			// 创建地图房间的玩家,用UserID构建player
			Player player = ComponentFactory.Create<Player, long>(message.UserId);
			player.GActorId = message.GActorId;
			player.CActorId = message.CActorId;
			// 更新player的Unit信息
			player.UnitId = unit.Id;
			player.Unit = unit;
			player.CharaId = message.CharaId;

			// 为player添加重要组件
			player.AddComponent<Level>();
			player.AddComponent<Mana>();
			player.AddComponent<Health>();
			
			player.AddComponent<PlayerSkillsComponent>();
			player.AddComponent<CombatComponent>();

			player.AddComponent<TimeInvokeComponent>();
			player.AddComponent<PlayerBrainComponent>();
			await player.AddComponent<MailBoxComponent>().AddLocation();
			

			//更新网关user的ActorID
			ActorMessageSender actorProxy = Game.Scene.GetComponent<ActorMessageSenderComponent>().Get(player.GActorId);
			actorProxy.Send(new Actor_EnterMapSucess_M2G() { GamerId = player.InstanceId, UnitId = unit.Id });

			// 将player添加到地图房间
			RoomComponent roomComponent = Game.Scene.GetComponent<RoomComponent>();
			Room room = roomComponent.GetMapRoom(7001); // 暂时用黎明镇的地图编号
			room.Add(player); 

			// 更新unit的entity与room
			unit.player = player;
			unit.room = room;

			UnitStateComponent unitStateCom = unit.GetComponent<UnitStateComponent>();
            Game.Scene.GetComponent<UnitStateMgrComponent>().Add(unitStateCom);

			// 广播创建的units
			CreateUnits_M2C createUnits = new CreateUnits_M2C();
			Unit[] units = Game.Scene.GetComponent<UnitComponent>().GetAll();
			foreach (Unit u in units)
			{
				UnitInfo unitInfo = new UnitInfo();
				unitInfo.X = u.Position.x;
				unitInfo.Y = u.Position.y;	
				unitInfo.Z = u.Position.z;
				unitInfo.UnitId = u.Id;
				unitInfo.UserId = u.player.UserId;
				unitInfo.CharaId = u.player.CharaId;
				createUnits.Units.Add(unitInfo);
			}
			
			// 应该是只向玩家可见范围的其它玩家广播，目前还没实现暂时向整个地图房间广播
			room.Broadcast(createUnits);
		}
	}
}