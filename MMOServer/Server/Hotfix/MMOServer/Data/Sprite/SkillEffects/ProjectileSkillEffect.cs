using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [ObjectSystem]
    public class ProjectileSkillEffectUpdateSystem : FixedUpdateSystem<ProjectileSkillEffect>
    {
        public override void FixedUpdate(ProjectileSkillEffect self)
        {
            self.FixedUpdate();
        }
    }
    public class ProjectileSkillEffect : SkillEffect
    {
        public float speed = 35;
        public int skillId;
        public LinearInt damage; // set by skill
        public int physicalDamage;
        public int magicDamage;
        public AttackType attackType = AttackType.physical;
        public int skillLevel = 1;
        public float stunChance; // set by skill
        public float stunTime; // set by skill


        public void FixedUpdate()
        {
            // 远程技能会一直朝目标飞去，即使它已经死了。
            // 比如，飞到一半的火球被取消的话，看起来很奇怪。
            if (target != null && caster != null)
            {
                Vector3 goal = target.bounds.center;
                this.Position = Vector3.MoveTowards(this.Position, goal, speed * Time.fixedDeltaTime);
                // transform.LookAt(goal);

                if (this.Position == goal)
                {
                    if (target.health.current > 0)
                    {
                        // damage.Get(skillLevel)为技能等级基础伤害
                        // d 为外传总伤害
                        
                        // c 为技能属性伤害
                        int c = 0;
                        if(attackType == AttackType.physical) c = c + physicalDamage;
                        else c = c + magicDamage;
                        
                        caster.combat.DealDamageAt(caster, 
                                                attackType,
                                                out int d,
                                                damage.Get(skillLevel)+c, 
                                                stunChance, stunTime);
                        
                        // 向客户端发送技能伤害消息
                        // (这里没有传目标id,因客户端攻击者也有目标属性的,但最好是传,要以服务端当前的目标为准)
                        M2C_FinishSkill finishSkill = new M2C_FinishSkill();
                        finishSkill.UnitId = caster.Unit.Id;
                        finishSkill.SkillId = skillId;
                        finishSkill.State = caster.state;
                        finishSkill.Damage = d;

                        // 广播技能消息
                        MapHelper.Broadcast(finishSkill,caster.Unit.Id);
                    }
                    this.Dispose();
                }
            }
            
        }
    }
}
