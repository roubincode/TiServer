﻿using UnityEngine;

/// 角色的可学或已学技能
namespace ETModel
{
    [ObjectSystem]
    public class PlayerSkillsStartSystem : StartSystem<PlayerSkillsComponent>
    {
        public override void Start(PlayerSkillsComponent self)
        {
            self.Start();
        }
    }

    public class PlayerSkillsComponent : SkillsComponent
    {   
        public new void Start()
        {
            base.Start();

            Log.Info("====================start PlayerSkillsComponent");
            // 暂时写死一份技能信息数据在此
            int[] charaSkills = new int[3]{1001,1002,1003};

            // SkillInfo[] charaSkills = new SkillInfo[2]{
            //     new SkillInfo{name="Normal Attack (Archer)",skillId=3012001},
            //     new SkillInfo{name="Precise Shot",skillId=3012002}
            // };

            LoadPlayerSkills(charaSkills);
        }

        // 加载角色技能
        public void LoadPlayerSkills(int[] datas){
            foreach (int skillId in datas)
                skills.Add(new Skill(CreateSkill(skillId)));
            Log.Info(skills.Count.ToString());
        }

        public SkillData CreateSkill(int skillID){
            SkillData skill = SkillData.dict[skillID];
            return skill;
        }

        public bool CheckCanUse(int skillIndex){
            Skill skill = skills[skillIndex];
            if (CastCheckSelf(skill) && CastCheckTarget(skill))
                return true;
            else return false;
        }

        /// <summary>尝试使用技能 </summary>
        public void TryUse(int skillIndex, bool ignoreState=false)
        {
            // 当施放技能后切换到默认的0号技能时，需要ignoreState为ture
            // 这样即使state为其它状态，也能切换到默认技能攻击
            if (entity.state != "CASTING" || ignoreState)
            {
                Skill skill = skills[skillIndex];
                if (CastCheckSelf(skill) && CastCheckTarget(skill))
                {
                    // 检测自己与攻击目标之间的距离
                    Vector3 destination;
                    if (CastCheckDistance(skill, out destination))
                    {
                        // cast
                        CmdUse(skillIndex);
                    }
                }
            }
            // 并不取消当前技能，在施放技能的过程中施放的其它技能
            // 有的技能是可以移动中施放的，这样就可以不中断此技能的施放再施放另一个技能，
            // 这样就会挂起一个技能，当此技能施放完后，施放挂起的技能
            else
            {
                pendingSkill = skillIndex;
            }
        }

        /// <summary>执行技能</summary>
        public void CmdUse(int skillIndex,bool auto = false)
        {
            // validate
            if ((entity.state == "IDLE" || entity.state == "MOVING" || entity.state == "CASTING") &&
                0 <= skillIndex && skillIndex < skills.Count)
            {
                // 检测施放的技能是已经学会的
                if (skills[skillIndex].level > 0)
                {
                    currentSkill = skillIndex;
                    Log.Info($"currentSkill-----{currentSkill}");
                }
            }
        }

        public bool HasLearned(string skillName)
        {
            // 至少达到1级为已学会
            return HasLearnedWithLevel(skillName, 1);
        }
        public bool HasLearnedWithLevel(string skillName, int skillLevel)
        {
            // 是否已经学会技能到指定等级
            foreach (Skill skill in skills)
                if (skill.level >= skillLevel && skill.name == skillName)
                    return true;
            return false;
        }

        /// <summary>是否能学习或升级技能</summary>
        public bool CanUpgrade(Skill skill)
        {
            return skill.level < skill.maxLevel &&
                level.current >= skill.upgradeRequiredLevel &&
                skillExperience >= skill.upgradeRequiredSkillExperience &&
                (skill.predecessor == null || (HasLearnedWithLevel(skill.predecessor.Name, skill.predecessorLevel)));
        }

        /// <summary>执行升级技能</summary>
        public void CmdUpgrade(int skillIndex)
        {
            // validate
            if ((entity.state == "IDLE" || entity.state == "MOVING" || entity.state == "CASTING") &&
                0 <= skillIndex && skillIndex < skills.Count)
            {
                // 判断能否升级?
                Skill skill = skills[skillIndex];
                if (CanUpgrade(skill))
                {
                    // 扣除技能经验
                    skillExperience -= skill.upgradeRequiredSkillExperience;

                    // upgrade
                    ++skill.level;
                    skills[skillIndex] = skill;

                    // 背动技能学习
                    if(skill.data.isPassive) skill.Apply(entity);
                }
            }
        }

    }
}