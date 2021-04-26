using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PF;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [CommandInput(typeof(CommandInput_UseSkill))]
    public class CommandInputSystem_UseSkill: ICommandSimulater
    {

        public ICommandResult Simulate(ICommandInput commandInput, Unit unit)
        {
            CommandInput_UseSkill input_UseSkill = commandInput as CommandInput_UseSkill;
            
            //TODO:做一些技能检查,判定
            // ...

            CommandResult_UseSkill result_UseSkill = CommandGCHelper.GetCommandResult<CommandResult_UseSkill>();
            result_UseSkill.unitId = input_UseSkill.unitId;
            result_UseSkill.targetId = input_UseSkill.targetId;
            result_UseSkill.frame = input_UseSkill.frame;
            result_UseSkill.skillId = input_UseSkill.skillId;
            // result_UseSkill.direction = input_UseSkill.direction;
            result_UseSkill.success = true;
            result_UseSkill.bufferId = -1;
            return result_UseSkill;

        }
    }
}
