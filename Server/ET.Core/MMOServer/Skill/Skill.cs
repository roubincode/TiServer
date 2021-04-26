using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//技能的属性方法结构体
namespace ETModel
{
    public struct Skill
    {
        public int skillId;
        public int level; 
        public double castTimeEnd; 
        public double cooldownEnd; 

        // 方便访问技能资源数据对象的包装
        public SkillData data
        {
            get
            {
                if (!SkillData.dict.ContainsKey(skillId))
                    throw new KeyNotFoundException("没有这个技能数据SkillData， hash=" + skillId + ". ");
                return SkillData.dict[skillId];
            }
        }

        public Skill(SkillData data)
        {
            skillId = data.SkillId;
            // 默认学会的技能等级显示为1
            level = data.learnDefault ? 1 : 0;

            castTimeEnd = cooldownEnd = NetworkTime.time;
        }

        public string name => data.Name;
        public float castTime => data.castTime.Get(level);
        public float cooldown => data.cooldown.Get(level);
        public float castRange => data.castRange.Get(level);
        public int manaCosts => data.manaCosts.Get(level);
        public int damage => data.damage.Get(level);
        public int physicalDamage => data.physicalDamage;
        public int magicDamage => data.magicDamage;
        public bool followupDefaultAttack => data.followupDefaultAttack;
        public bool learnDefault => data.learnDefault;
        public SkillData predecessor => data.predecessor;
         public int predecessorLevel => data.predecessorLevel;
        public bool showCastBar => data.showCastBar;
        public bool cancelCastIfTargetDied => data.cancelCastIfTargetDied;
        public bool allowMovement => data.allowMovement;
        public int maxLevel => data.maxLevel;
        public string requiredWeaponCategory => data.requiredWeaponCategory;
        public int upgradeRequiredLevel => data.requiredLevel.Get(level+1);
        public long upgradeRequiredSkillExperience => data.requiredExperience.Get(level+1);

        public bool CheckSelf(Entity caster, bool checkSkillReady=true)
        {
            return (!checkSkillReady || IsReady()) &&
                data.CheckSelf(caster, level);
        }
        public bool CheckTarget(Entity caster) { return data.CheckTarget(caster); }
        public bool CheckDistance(Entity caster, out Vector3 destination) { return data.CheckDistance(caster, level, out destination); }
        public void Apply(Entity caster) { data.Apply(caster,skillId, level); }

    
        // 离技能施放时间结束还有多少时间
        public float CastTimeRemaining() => NetworkTime.time >= castTimeEnd ? 0 : (float)(castTimeEnd - NetworkTime.time);

        // 正在施放技能，如果剩余施法时间大于0
        public bool IsCasting() => CastTimeRemaining() > 0;

        // 离技能冷却结束还有多少时间
        public float CooldownRemaining() => NetworkTime.time >= cooldownEnd ? 0 : (float)(cooldownEnd - NetworkTime.time);

        public bool IsOnCooldown() => CooldownRemaining() > 0;

        public bool IsReady() => !IsCasting() && !IsOnCooldown();

    }
}