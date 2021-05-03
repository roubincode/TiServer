using System;
using System.Collections.Generic;
using ETModel;
using PF;
using UnityEngine;

namespace ETHotfix
{
    public static class UnitStateComponentSystem
    {
        
        public static void FixedUpdate(this UnitStateComponent unitStateComponent)
        {

            if (!unitStateComponent.haveInited) return;
            if (unitStateComponent.unitStatesDic.Count == 0) return;
            UnitStateMgrComponent mgr = Game.Scene.GetComponent<UnitStateMgrComponent>();
            Log.Info(string.Format("frame {0} : 玩家位置信息 {1}", mgr.currFrame, unitStateComponent.unit.Position));

            //每间隔3帧发一次数据，每秒20次
            if (mgr.currFrame - unitStateComponent.preSendMsgFrame < UnitStateMgrComponent.sendMsgDelta)
            {
                return;
            }
            unitStateComponent.preSendMsgFrame = mgr.currFrame;

            if (!unitStateComponent.collectInput)
            {
                return;
            }

            //TODO : 这里有大量GC,需要处理
            //if (unitStateComponent.currGetInputFrame - unitStateComponent.preClearInputFrame >= UnitStateComponent.maxFrameCount_SaveStateDelta)
            //{
            //    for (int i = unitStateComponent.preClearInputFrame; i < unitStateComponent.currGetInputFrame - UnitStateComponent.maxFrameCount_SaveStateDelta; i++)
            //    {
            //        if (unitStateComponent.unitStatesDic.ContainsKey(i))
            //        {
            //            unitStateComponent.unitStatesDic.Remove(i);
            //        }
            //    }
            //}

            //每次发送都发最新的的结果
            //var state = unitStateComponent.unitStatesDic[unitStateComponent.currGetInputFrame];

            //foreach (var v in state.commandResults)
            //{
            //    CommandResultInfo_Move commandResultInfo_Move = new CommandResultInfo_Move();
            //    commandResultInfo_Move.Frame = state.frame;

            //    switch (v.Value)
            //    {
            //        case CommandResult_Move result_Move:
                        
            //            continue;
            //    }
            //}
            unitStateComponent.collectInput = false;
        }

        public static void GetInput(this UnitStateComponent unitStateComponent, int frame, ICommandInput commandInput)
        {
            try
            {
                CommandSimulaterComponent sim = Game.Scene.GetComponent<CommandSimulaterComponent>();
                var result = sim.commandSimulaters[commandInput.GetType()].Simulate(commandInput, unitStateComponent.unit);
                switch (result)
                {
                    case CommandResult_Move result_Move:
                        // 输出数据传入CharacterMovementComponet
                        // 服务器上要有一份同样的角色移动数据，用于更新服务端的移动状态
                        Move move = new Move((MoveState)result_Move.state,result_Move.frame,
												result_Move.movePosition, result_Move.yRotation,result_Move.nSpeed,result_Move.jumpLeg);
                        unitStateComponent.unit.GetComponent<CharacterMoveComponent>().TryMove(move);
                        unitStateComponent.result_Move.UnitId = unitStateComponent.unit.Id;
                        unitStateComponent.result_Move.Frame = frame;
                        unitStateComponent.result_Move.State = (StateInfo)result_Move.state;
                        unitStateComponent.result_Move.MovePosition = result_Move.movePosition.ToV3Info();
                        unitStateComponent.result_Move.YRotation = result_Move.yRotation;
                        unitStateComponent.result_Move.NSpeed = result_Move.nSpeed;
                        unitStateComponent.result_Move.JumpLeg = result_Move.jumpLeg;
                        
                        // 广播移动消息
                        MapHelper.Broadcast(unitStateComponent.result_Move,unitStateComponent.unit.Id);
                        break;
                    case CommandResult_UseSkill result_UseSkill:
                        // 输出数据传入PlayerSkillsComponent
                        // 服务器上要有一份同样的角色技能使用数据，用于更新服务端的技能状态
                        PlayerSkillsComponent skillsComponent = unitStateComponent.unit.player.GetComponent<PlayerSkillsComponent>();
                        int index = skillsComponent.GetSkillIndexById(result_UseSkill.skillId);

                        bool checkResult = skillsComponent.CheckCanUse(index);
                        if(checkResult) skillsComponent.TryUse(index);

                        unitStateComponent.result_UseSkill.UnitId = unitStateComponent.unit.Id;
                        unitStateComponent.result_UseSkill.TargetId = result_UseSkill.targetId;
                        unitStateComponent.result_UseSkill.Frame = frame;
                        unitStateComponent.result_UseSkill.SkillId = result_UseSkill.skillId;
                        unitStateComponent.result_UseSkill.Success = checkResult;
                        
                        // 广播技能消息
                        // MapHelper.Broadcast(unitStateComponent.result_UseSkill,unitStateComponent.unit.Id);
                        break;
                }

                // 缓存帧状态数据
                //unitStateDelta.commandResults.Add(result.GetType(), result);
                //unitStateComponent.unitStatesDic[unitStateComponent.currGetInputFrame] = unitStateDelta;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());

            }
        }

    
    }
}