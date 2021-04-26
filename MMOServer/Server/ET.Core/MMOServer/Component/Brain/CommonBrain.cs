// 所有的brain中都有一些共同的事件。
// 不是必须继承CommonBrain,不需要这些事件可以直接继承ScriptableBrain
using UnityEngine;
namespace ETModel
{
    public abstract class CommonBrain : Component
    {
        public bool EventAggro(Entity entity) =>
            entity.target != null && entity.target.health.current > 0;

        public bool EventDied(Entity entity) =>
            entity.health.current == 0;

        public bool EventMoveEnd(Entity entity) =>
            entity.state == "MOVING" && !entity.movement.IsMoving();

        public bool EventMoveStart(Entity entity) =>
            entity.state != "MOVING" && entity.movement.IsMoving();

        public bool EventSkillFinished(Entity entity) =>
            0 <= entity.skillsCom.currentSkill && entity.skillsCom.currentSkill < entity.skillsCom.skills.Count &&
            entity.skillsCom.skills[entity.skillsCom.currentSkill].CastTimeRemaining() == 0;

        public bool EventSkillRequest(Entity entity) =>
            0 <= entity.skillsCom.currentSkill && entity.skillsCom.currentSkill < entity.skillsCom.skills.Count;


        public bool EventTargetDied(Entity entity) =>
            entity.target != null && entity.target.health.current == 0;

        public bool EventTargetDisappeared(Entity entity) =>
            entity.target == null;
            
        public bool EventTargetTooFarToAttack(Entity entity) =>
            entity.target != null &&
            0 <= entity.skillsCom.currentSkill && entity.skillsCom.currentSkill < entity.skillsCom.skills.Count &&
            !entity.skillsCom.CastCheckDistance(entity.skillsCom.skills[entity.skillsCom.currentSkill], out Vector3 destination);
    }
}