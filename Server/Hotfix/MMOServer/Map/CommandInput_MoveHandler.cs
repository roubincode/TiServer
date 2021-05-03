using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class CommandInput_MoveHandler : AMActorLocationHandler<Unit, C2M_Input_Move>
	{
		protected override async ETTask Run(Unit unit, C2M_Input_Move message)
		{
            UnitStateComponent unitStateComponent = unit.GetComponent<UnitStateComponent>();
			// 上次广播消息的frame数+1
			unitStateComponent.currGetInputFrame = unitStateComponent.preSendMsgFrame+1;
            
            CommandInput_Move commandInput_Move = new CommandInput_Move();
			commandInput_Move.state = (MoveState)message.State;
			commandInput_Move.frame = message.Frame;
			commandInput_Move.movePosition = message.MovePosition.ToV3();
			commandInput_Move.yRotation = message.YRotation;
			commandInput_Move.nSpeed = message.NSpeed;
			commandInput_Move.jumpLeg = message.JumpLeg;

			Move move = new Move((MoveState)message.State,message.Frame,
												message.MovePosition.ToV3(), message.YRotation,
												message.NSpeed,message.JumpLeg);
			unitStateComponent.unit.GetComponent<CharacterMoveComponent>().TryMove(move);
			
			unitStateComponent.result_Move.UnitId = unitStateComponent.unit.Id;
			unitStateComponent.result_Move.Frame = message.Frame;
			unitStateComponent.result_Move.State = message.State;
			unitStateComponent.result_Move.MovePosition = message.MovePosition;
			unitStateComponent.result_Move.YRotation = message.YRotation;
			unitStateComponent.result_Move.NSpeed = message.NSpeed;
			unitStateComponent.result_Move.JumpLeg = message.JumpLeg;
			
			// 广播移动消息
			MapHelper.Broadcast(unitStateComponent.result_Move,unitStateComponent.unit.Id);

			await ETTask.CompletedTask;
        }
	}
}