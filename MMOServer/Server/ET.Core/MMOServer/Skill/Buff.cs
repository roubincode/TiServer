using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//装备道具技能系统的数据结构体
// Item            道具数据结构体
// ItemSlot        背包道具格子数据结构体
// Equipment       装备数据结构体
// Skill,Buff      技能,buff数据结构体

namespace ETModel
{
    public struct Buff
    {
        public int skilid;
        public int level;
        public double buffTimeEnd;  
        public bool isPassive;

        // constructors
        public Buff(BuffSkill data, int level)
        {
            skilid = data.SkillId;
            this.level = level;
            buffTimeEnd = NetworkTime.time + data.buffTime.Get(level);
            isPassive = data.isPassive;
        }

        // wrappers for easier access
        public BuffSkill data
        {
            get
            {
                if (!SkillData.dict.ContainsKey(skilid))
                    throw new KeyNotFoundException("没有这个技能数据SkillData， hash==" + skilid + ".");
                return (BuffSkill)SkillData.dict[skilid];
            }
        }
        public string name => data.Name;
        public float buffTime => data.buffTime.Get(level);
        public bool remainAfterDeath => data.remainAfterDeath;
        public int healthMaxBonus => data.healthMaxBonus.Get(level);
        public int manaMaxBonus => data.manaMaxBonus.Get(level);
        public int damageBonus => data.damageBonus.Get(level);
        public int defenseBonus => data.armorBonus.Get(level);
        public float blockChanceBonus => data.blockChanceBonus.Get(level);
        public float dodgeChanceBonus => data.dodgeChanceBonus.Get(level);
        public float criticalChanceBonus => data.criticalChanceBonus.Get(level);
        public float healthPercentPerSecondBonus => data.healthPercentPerSecondBonus.Get(level);
        public float manaPercentPerSecondBonus => data.manaPercentPerSecondBonus.Get(level);
        public float speedBonus => data.speedBonus.Get(level);
        public int maxLevel => data.maxLevel;

        public float BuffTimeRemaining()
        {
            return NetworkTime.time >= buffTimeEnd ? 0 : (float)(buffTimeEnd - NetworkTime.time);
        }
    }
}

