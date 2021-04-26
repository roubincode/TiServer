using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [ObjectSystem]
    public class PlayerBrainComponentUpdateSystem : FixedUpdateSystem<PlayerBrainComponent>
    {
        public override void FixedUpdate(PlayerBrainComponent self)
        {
            self.FixedUpdate();
        }
    }

    public static class PlayerBrainComponentSystem
    {
        // states //////////////////////////////////////////////////////////////////
        public static string UpdateServer_IDLE(this PlayerBrainComponent self,Player player)
        {
            if (self.EventDied(player))
            {
                // we died.
                return "DEAD";
            }
            if (self.EventCancelAction(player))
            {
                // 直接取消目标
                player.target = null;
                return "IDLE";
            }
            if (self.EventMoveStart(player))
            {
                // 移动会取消释放
                player.skillsCom.CancelCast();
                return "MOVING";
            }
            if (self.EventSkillRequest(player))
            {
                // 检查自己（存活，法力，武器等）和目标和距离
                Skill skill = player.skillsCom.skills[player.skillsCom.currentSkill];
                player.nextTarget = player.target; 
                if (player.skillsCom.CastCheckSelf(skill) &&
                    player.skillsCom.CastCheckTarget(skill) &&
                    player.skillsCom.CastCheckDistance(skill, out Vector3 destination))
                {
                    // start casting 
                    Log.Info(skill.skillId.ToString());
                    player.skillsCom.StartCast(skill,player.skillsCom.currentSkill);
                    return "CASTING";
                }
                else
                {
                    // 检查失败。重置当前技能
                    player.skillsCom.currentSkill = -1;
                    player.nextTarget = null; 
                    return "IDLE";
                }
            }
            return "IDLE";
        }

        public static string UpdateServer_MOVING(this PlayerBrainComponent self,Player player)
        {
            if (self.EventDied(player))
            {
                // we died.
                return "DEAD";
            }
            if (self.EventMoveEnd(player))
            {
                // 移动完毕。
                return "IDLE";
            }
            if (self.EventCancelAction(player))
            {
                // cancel casting 
                player.skillsCom.CancelCast();
                return "IDLE";
            }
            if (self.EventSkillRequest(player))
            {
                Skill skill = player.skillsCom.skills[player.skillsCom.currentSkill];
                if (player.skillsCom.CastCheckSelf(skill) &&
                    player.skillsCom.CastCheckTarget(skill) &&
                    player.skillsCom.CastCheckDistance(skill, out Vector3 destination))
                {
                    player.skillsCom.StartCast(skill,player.skillsCom.currentSkill);
                    return "CASTING";
                }
            }
            return "MOVING"; 
        }

        public static string UpdateServer_CASTING(this PlayerBrainComponent self,Player player)
        {
            // 保持面向目标
            // if (player.target && player.movement.DoCombatLookAt())
            //     player.movement.LookAtY(player.target.transform.position);

            if (self.EventDied(player))
            {
                // we died.
                return "DEAD";
            }
            if (self.EventMoveStart(player))
            {
                Skill skill = player.skillsCom.skills[player.skillsCom.currentSkill];
                //判断是否瞬发技能或者是否可移动中施放技能
                if(!skill.allowMovement){
                    player.skillsCom.CancelCast();
                    return "MOVING";
                }else return "CASTING";
            }
            if (self.EventCancelAction(player))
            {
                // cancel casting
                player.skillsCom.CancelCast();
                return "IDLE";
            }
            if (self.EventTargetDisappeared(player))
            {
                // 必须目标的技能，目标消失则取消技能
                if (player.skillsCom.skills[player.skillsCom.currentSkill].cancelCastIfTargetDied)
                {
                    player.skillsCom.CancelCast();
                    self.UseNextTargetIfAny(player); 
                    return "IDLE";
                }
            }
            if (self.EventTargetDied(player))
            {
                // 必须目标的技能，目标死亡则取消技能
                if (player.skillsCom.skills[player.skillsCom.currentSkill].cancelCastIfTargetDied)
                {
                    player.skillsCom.CancelCast();
                    self.UseNextTargetIfAny(player); 
                    return "IDLE";
                }
            }
            if (self.EventSkillFinished(player))
            {
                // 施法完成后应用技能
                Skill skill = player.skillsCom.skills[player.skillsCom.currentSkill];
                player.skillsCom.FinishCast(skill);

                // clear current skill for now
                if(!skill.followupDefaultAttack)
                    player.skillsCom.currentSkill = -1;

                // 如果用户在施放时试图瞄准另一个目标，则使用下一个目标
                self.UseNextTargetIfAny(player);

                // go back to IDLE
                return "IDLE";
            }
            return "CASTING"; 
        }

        public static string UpdateServer_DEAD(this PlayerBrainComponent self,Player player)
        {
            // 死亡后出生点复活
            if (self.EventRespawn(player))
            {
                // 最近的出生点复活,有50%的红蓝
                // Vector3 start = Manager.RPG.GetNearestStartPosition(player.movement.unit.Position);
                // player.movement.Warp(start); 
                player.Revive(0.5f);
                return "IDLE";
            }
            return "DEAD"; 
        }

        public static  string UpdateState(this PlayerBrainComponent self,Entity entity)
        {
            Player player = (Player)entity;

            // 角色状态更新，idle,moving,casting,dead是基本的，后期可添加击晕，交易，生产制作等状态
            if (player.state == "IDLE")     return self.UpdateServer_IDLE(player);
            if (player.state == "MOVING")   return self.UpdateServer_MOVING(player);
            if (player.state == "CASTING")  return self.UpdateServer_CASTING(player);
            // if (player.state == "STUNNED")  return self.UpdateServer_STUNNED(player);
            // if (player.state == "TRADING")  return self.UpdateServer_TRADING(player);
            // if (player.state == "CRAFTING") return self.UpdateServer_CRAFTING(player);
            if (player.state == "DEAD")     return self.UpdateServer_DEAD(player);

            Log.Error("invalid state:" + player.state);
            return "IDLE";
        }

        public static void FixedUpdate(this PlayerBrainComponent self)
        {
                Player player = (Player) self.GetParent<Entity>();

                // 角色动画状态更新
                player.state = self.UpdateState(player);
                // Log.Info(player.state);
        }
    }

}