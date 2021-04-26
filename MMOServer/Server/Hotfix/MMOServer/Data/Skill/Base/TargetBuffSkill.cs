using UnityEngine;
using ETModel;
/// 对目录施放buff技能
namespace ETHotfix
{
    public class TargetBuffSkill : BuffSkill
    {
        public bool canBuffSelf = true;
        public bool canBuffOthers = false;  
        public bool canBuffEnemies = false;  

        // helper function 
        Entity CorrectedTarget(Entity caster)
        {
            // 没有目标？那就试着释放给自己
            if (caster.target == null)
                return canBuffSelf ? caster : null;

            // targeting self?
            if (caster.target == caster)
                return canBuffSelf ? caster : null;

            // 同一类型的目标？
            if (caster.target.GetType() == caster.GetType())
            {
                if (canBuffOthers)
                    return caster.target;
                else if (canBuffSelf)
                    return caster;
                else
                    return null;
            }

            // 敌对目标
            if (caster.CanAttack(caster.target))
            {
                if (canBuffEnemies)
                    return caster.target;
                else if (canBuffSelf)
                    return caster;
                else
                    return null;
            }

            // 没有效目标？试着施展自己或者根本不施展
            return canBuffSelf ? caster : null;
        }

        public override bool CheckTarget(Entity caster)
        {
            caster.target = CorrectedTarget(caster);
            return caster.target != null && caster.target.health.current > 0;
        }

        public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
        {
            // 目标还在附近?
            if (caster.target != null)
            {
                destination = Utils.ClosestPoint(caster.target, caster.Position);
                return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
            }
            destination = caster.Position;
            return false;
        }

        public override void Apply(Entity caster,int skillId, int skillLevel)
        {
            if (caster.target != null && caster.target.health.current > 0)
            {
                // 添加buff或者刷新已经存在的此种buff
                Buff buff = new Buff(this, skillLevel);
                caster.target.skillsCom.AddOrRefreshBuff(buff);

                // 向客户端发送buff技能消息
                // (这里没有传目标id,但最好是传,要以服务端当前的目标为准)
                // M2C_BuffSkill buffSkill = new M2C_BuffSkill();
                // buffSkill.UnitId = caster.Unit.Id;
                // buffSkill.SkillId = skillId;
                // buffSkill.BuffTime = buff.buffTime;
                // buffSkill.State = caster.state;
                
                // // 广播技能消息
                // MapHelper.Broadcast(buffSkill,caster.Unit.Id);
            }
        }
    }
}
