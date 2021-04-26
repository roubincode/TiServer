using ETModel;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class CommandInput_UseSkillHandler : AMActorLocationHandler<Unit, C2M_Input_UseSkill>
	{
		protected override async ETTask Run(Unit unit, C2M_Input_UseSkill message)
		{
            UnitStateComponent unitStateComponent = unit.GetComponent<UnitStateComponent>();
            // 上次广播消息的frame数+1
			unitStateComponent.currGetInputFrame = unitStateComponent.preSendMsgFrame+1;

            CommandInput_UseSkill commandInput_UseSkill = CommandGCHelper.GetCommandInput<CommandInput_UseSkill>();
            commandInput_UseSkill.skillId = message.SkillId;
            commandInput_UseSkill.targetId = message.TargetId;
            // commandInput_UseSkill.direction = message.Direction.ToV3();
            unitStateComponent.GetInput(unitStateComponent.currGetInputFrame, commandInput_UseSkill);

            await ETTask.CompletedTask;
        }
	}
}