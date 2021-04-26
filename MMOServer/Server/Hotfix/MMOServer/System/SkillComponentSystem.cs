using System.Collections.Generic;
using ETModel;
namespace ETHotfix
{
    public static class SkillComponentSystem
    {

        /// <summary>还没有服务端这里模仿一个施放技能的方法 </summary>
        public static void StartCast(this SkillsComponent self,Skill skill,int currentSkill)
        {
            // 开始施放技能并设置施放结束时间
            skill.castTimeEnd = NetworkTime.time + skill.castTime;
            // skill属性改变，保存到List中的引用
            self.skills[currentSkill] = skill;

            // 这里是直接调用前端要执行的方法，实际在服务端这是向前端发消息
            self.RpcCastStarted(skill);
        }

        /// <summary>还没有服务端这里模仿一个完成施放技能的方法 </summary>
        public static void FinishCast(this SkillsComponent self,Skill skill)
        {
            if (self.CastCheckSelf(skill, false) && self.CastCheckTarget(skill))
            {
                // 执行技能方法
                skill.Apply(self.entity);

                //self.RpcCastFinished(skill,d);

                // decrease mana in any case
                self.mana.current -= skill.manaCosts;

                // skill属性改变，保存到List中的引用
                // start the cooldown
                skill.cooldownEnd = NetworkTime.time + skill.cooldown;
                self.skills[self.currentSkill] = skill;
            }
            else
            {
                self.currentSkill = -1;
            }
        }

        /// <summary>取消技能施放的方法，到后面课时这也只是服务端才需要的方法 </summary>
        public static void CancelCast(this SkillsComponent self,bool resetCurrentSkill = true)
        {
            if (self.currentSkill != -1)
            {
                Skill skill = self.skills[self.currentSkill];

                // skill属性改变，保存到List中的引用
                skill.castTimeEnd = NetworkTime.time - skill.castTime;
                self.skills[self.currentSkill] = skill;
                
                // reset current skill
                if (resetCurrentSkill)
                    self.currentSkill = -1;
            }
        }

        /// <summary>通知前端技能释放开始 </summary>
        public static void RpcCastStarted(this SkillsComponent self,Skill skill)
        {
            // 判断是否活着
            if (self.health.current > 0)
            {
                Entity entity = self.GetParent<Entity>();
                self.startSkill.UnitId = entity.Unit.Id;
                self.startSkill.SkillId = skill.skillId;
                self.startSkill.TargetId = entity.target.GetComponent<NetworkIdentity>().netId;
                self.startSkill.State = entity.state;

                 // 广播技能消息
                 MapHelper.Broadcast(self.startSkill,entity.Unit.Id);
            }
        }

        /// <summary>通知前端技能释放完成 </summary>
        public static void RpcCastFinished(this SkillsComponent self,Skill skill,int damage)
        {
            // 判断是否活着
            if (self.health.current > 0)
            {
                M2C_FinishSkill finishSkill = new M2C_FinishSkill();
                Entity entity = self.GetParent<Entity>();
                finishSkill.UnitId = entity.Unit.Id;
                finishSkill.SkillId = skill.skillId;
                finishSkill.State = entity.state;
                finishSkill.Damage = damage;

                // 广播技能消息
                MapHelper.Broadcast(self.finishSkill,entity.Unit.Id);
            }
        }
    }
}