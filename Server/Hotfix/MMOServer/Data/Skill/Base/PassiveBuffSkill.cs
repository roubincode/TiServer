using UnityEngine;
using ETModel;
/// 施放背运buff技能
namespace ETHotfix
{
    public class PassiveBuffSkill : BuffSkill
    {
        public override bool CheckTarget(Entity caster) { return false; }
        public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
        {
            destination = caster.Position;
            return true;
        }
        public override void Apply(Entity caster,int damage, int skillLevel) {
            Buff buff = new Buff(this, skillLevel);
            bool hasBuff = caster.skillsCom.HasPassiveBuff(buff);

            // 添加buff或者刷新已经存在的此种buff
            caster.skillsCom.AddOrRefreshBuff(buff);

            // 向客户端发送buff技能消息
            // (这里没有传目标id,但最好是传,要以服务端当前的目标为准)
            // M2C_BuffSkill buffSkill = new M2C_BuffSkill();
            // buffSkill.UnitId = caster.Unit.Id;
            // buffSkill.SkillId = skillId;
            // buffSkill.BuffTime = -1;
            // buffSkill.State = caster.state;
            
            // // 广播技能消息
            // MapHelper.Broadcast(buffSkill,caster.Unit.Id);
        }
    }
}
