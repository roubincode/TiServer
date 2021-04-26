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
    [CommandInput(typeof(CommandInput_Move))]
    public class CommandInputSystem_Move: ICommandSimulater
    {

        public ICommandResult Simulate(ICommandInput commandInput, Unit unit)
        {
            CommandInput_Move input_Move = commandInput as CommandInput_Move;

            //TODO:做一些移动检查,判定
            // ...

            CommandResult_Move result_Move = CommandGCHelper.GetCommandResult<CommandResult_Move>();
            result_Move.unitId = input_Move.unitId;
            result_Move.state = (MoveState)input_Move.state;
			result_Move.frame = input_Move.frame;
			result_Move.movePosition = input_Move.movePosition;
			result_Move.yRotation = input_Move.yRotation;
            return result_Move;

        }
    }
}
