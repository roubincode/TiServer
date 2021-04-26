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
            unitStateComponent.GetInput(unitStateComponent.currGetInputFrame, commandInput_Move);

			await ETTask.CompletedTask;
        }
	}
}