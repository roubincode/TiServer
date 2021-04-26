using UnityEngine;
using ETModel;
/// 普通的近战攻击技能
namespace ETHotfix
{
    public class TargetDamageSkill : DamageSkill
    {
        public override bool CheckTarget(Entity caster)
        {
            // 目标存在, 存活, not self, 可攻击?
            return caster.target != null && caster.CanAttack(caster.target);
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
            // damage.Get(skillLevel)为技能等级基础伤害
            // d 为外传总伤害
            // c 为技能属性伤害
            int c = 0;
            if(attackType == AttackType.physical) c = c + physicalDamage;
            else c = c + magicDamage;

            // 造成基础伤害（装备属性形成）+ 技能伤害
            caster.combat.DealDamageAt(caster,
                                    attackType,
                                    out int d,
                                    damage.Get(skillLevel)+c,                      
                                    stunChance.Get(skillLevel),
                                    stunTime.Get(skillLevel));

            // 向客户端发送技能伤害消息
            // (这里没有传目标id,因客户端攻击者也有目标属性的,但最好是传,要以服务端当前的目标为准
            // 不然可能出现客户端突然切换了目标显示的是扣新目标的血,而服务端却扣的前一个目标的血)
            M2C_FinishSkill finishSkill = new M2C_FinishSkill();
            finishSkill.UnitId = caster.Unit.Id;
            finishSkill.SkillId = skillId;
            finishSkill.State = caster.state;
            finishSkill.Damage = d;

            // 广播技能消息
            MapHelper.Broadcast(finishSkill,caster.Unit.Id);
        }
    }
}