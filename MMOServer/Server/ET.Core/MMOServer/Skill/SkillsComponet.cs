using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>Class <c>SkillsComponet</c> 
/// 用于实体对象的技能组件基础类,其它类似的还有:</summary>
namespace ETModel
{
    public abstract class SkillsComponet : Component, IAbility
    {
        public Entity entity;
        public Level level;
        public Health health;
        public Mana mana;

        public long skillExperience = 0;

        /// <summary>
        /// 角色当前技能
        /// </summary>
        public int currentSkill = -1;

        /// <summary>
        /// 角色挂起技能
        /// </summary>
        public int pendingSkill = -1;

        /***************************************************************************
        * 用于持有实体的技能Scriptable对象的数组                                     *
        *   ScriptableSkill[] skillTemplates                                       *
        * 技能的数据容器                                                   *
        *   SyncListSkill skills                                                   *
        * buff的数据容器                                                   *
        *   SyncListBuff buffs                                                     *
        *=========================================================================*/
        public List<Skill> skills = new List<Skill>();
        
        public List<Buff> buffs = new List<Buff>(); 
        // public SyncListBuff buffs = new SyncListBuff(); 

        void Update(){
            // 暂时本地调用，实际是由服务端刷新调用
            CleanupBuffs();
        }

        public int GetManaRecoveryBonus()
        {
            float buffPercent = 0;
            foreach (Buff buff in buffs)
                buffPercent += buff.manaPercentPerSecondBonus;

            return Convert.ToInt32(buffPercent * mana.max);
        }
        public int GetHealthRecoveryBonus()
        {
            float buffPercent = 0;
            foreach (Buff buff in buffs)
                buffPercent += buff.healthPercentPerSecondBonus;

            return Convert.ToInt32(buffPercent * health.max);
        }

        /***************************************************************************
        * 获取技能基本属性方法                                                      *
        * GetHealthBonus 从技能获得生命值提升                                       *
        * GetManaBonus 从技能获得法力值提升                                         *
        * GetEnduranceBonus 从技能获得耐力值提升                                    *
        * GetIntellectBonus 从技能获得智力值提升                                    *
        *=========================================================================*/
        public int GetHealthBonus(int baseHealth)
        {
            int buffBonus = 0;
            foreach (Buff buff in buffs)
                buffBonus += buff.healthMaxBonus;

            return  buffBonus;
        }

        public int GetManaBonus(int baseMana)
        {
            int buffBonus = 0;
            foreach (Buff buff in buffs)
                buffBonus += buff.manaMaxBonus;

            return buffBonus;
        }
        public int GetEnduranceBonus(){
            return 0;
        }
        public int GetIntellectBonus(){
            return 0;
        }

        /***************************************************************************
        * 获取技能防御属性方法                                                      *
        * GetArmorBonus  从被动技能获得护甲防御提升                                  *
        * GetDodgeBonus  从被动技能获得闪避提升                                      *
        * GetBlockChanceBonus  从被动技能获得格挡/抵抗0-1随机机率                     *
        *=========================================================================*/
        public int GetArmorBonus()
        {
            int buffBonus = 0;
            foreach (Buff buff in buffs)
                buffBonus += buff.defenseBonus;

            return buffBonus;
        }
        public int GetMagicDefenseBonus()
        {
            return 0;
        }
        public int GetPhysicalDefenseBonus()
        {
            return 0;
        }
        public float GetDodgeBonus(){
            float buffBonus = 0;
            foreach (Buff buff in buffs)
                buffBonus += buff.dodgeChanceBonus;

            return buffBonus;
        }
        public float GetBlockBonus()
        {
            float buffBonus = 0;
            foreach (Buff buff in buffs)
                buffBonus += buff.blockChanceBonus;

            return buffBonus;
        }

        /***************************************************************************
        * 获取技能伤害属性方法                                                       *
        * GetAgilityBonus  从被动技能获得敏捷提升                                    *
        * GetStrengthBonus  从被动技能获得力量提升                                    *
        * GetSpiritBonus  从被动技能获得精神提升                                      *
        *=========================================================================*/
        public int GetAgilityBonus(){
            return 0;
        }
        public int GetStrengthBonus(){
            return 0;
        }
        public int GetSpiritBonus(){
            return 0;
        }
        public float GetHitrateBonus()
        {
            return 0;
        }
        public int GetMagicBonus(){
            return 0;
        }
        public int GetPhysicalBonus(){
            return 0;
        }
        public int GetHurtBonus()
        {
            int buffBonus = 0;
            foreach (Buff buff in buffs)
                buffBonus += buff.damageBonus;

            return buffBonus;
        }
        public float GetCriticalBonus()
        {
            float buffBonus = 0;
            foreach (Buff buff in buffs)
                buffBonus += buff.criticalChanceBonus;

            return buffBonus;
        }
        public float GetMagicHitrateBonus()
        {
            return 0;
        }
        public float GetPhysicHitrateBonus()
        {
            return 0;
        }
        public float GetMagicCriticalBonus()
        {
            return 0;
        }
        public float GetPhysicCriticalBonus()
        {
            return 0;
        }

        /***************************************************************************
        * 获取技能伤害属性的接口方法                                                   *
        *=========================================================================*/
        /// <summary>秒伤,从技能获得的dps值</summary>
        public float GetDpsBonus()
        {
            return 0;
        }
        /// <summary>从技能获得的攻速值</summary>
        public float GetAttackRateBonus()
        {
            return 0;
        }

        // helper function 取得 skill index
        public int GetSkillIndexByName(string skillName)
        {
            for (int i = 0; i < skills.Count; ++i)
                if (skills[i].name == skillName)
                    return i;
            return -1;
        }

        // helper function to find a buff index
        public int GetBuffIndexByName(string buffName)
        {
            // (avoid FindIndex to minimize allocations)
            for (int i = 0; i < buffs.Count; ++i)
                if (buffs[i].name == buffName)
                    return i;
            return -1;
        }

        public bool CastCheckSelf(Skill skill, bool checkSkillReady = true) =>
            skill.CheckSelf(entity, checkSkillReady);

        public bool CastCheckTarget(Skill skill) =>
            skill.CheckTarget(entity);

        public bool CastCheckDistance(Skill skill, out Vector3 destination) =>
            skill.CheckDistance(entity, out destination);

        
        /// <summary>还没有服务端这里模仿一个施放技能的方法 </summary>
        public void StartCast(Skill skill,int currentSkill)
        {
            // 开始施放技能并设置施放结束时间
            skill.castTimeEnd = NetworkTime.time + skill.castTime;
            // skill属性改变，保存到List中的引用
            skills[currentSkill] = skill;

            // 这里是直接调用前端要执行的方法，实际在服务端这是向前端发消息
            RpcCastStarted(skill);
        }

        /// <summary>还没有服务端这里模仿一个完成施放技能的方法 </summary>
        public void FinishCast(Skill skill,int currentSkill)
        {
            if (CastCheckSelf(skill, false) && CastCheckTarget(skill))
            {
                // 执行技能方法
                skill.Apply(entity);

                RpcCastFinished(skill);

                // decrease mana in any case
                mana.current -= skill.manaCosts;

                // skill属性改变，保存到List中的引用
                // start the cooldown
                skill.cooldownEnd = NetworkTime.time + skill.cooldown;
                skills[currentSkill] = skill;
            }
            else
            {
                currentSkill = -1;
            }
        }

        /// <summary>取消技能施放的方法，到后面课时这也只是服务端才需要的方法 </summary>
        public void CancelCast(bool resetCurrentSkill = true)
        {
            if (currentSkill != -1)
            {
                Skill skill = skills[currentSkill];

                // skill属性改变，保存到List中的引用
                skill.castTimeEnd = NetworkTime.time - skill.castTime;
                skills[currentSkill] = skill;
                
                // reset current skill
                if (resetCurrentSkill)
                    currentSkill = -1;
            }
        }

        /// <summary>前端施放技能调用的方法 </summary>
        public void RpcCastStarted(Skill skill)
        {
            // 判断是否活着
            if (health.current > 0)
            {
                // ...
            }
        }

        /// <summary>前端结束施放技能调用的方法 </summary>
        public void RpcCastFinished(Skill skill)
        {
            // 判断是否活着
            if (health.current > 0)
            {
                // ...
            }
        }

        // helper function to add or refresh a buff
        public void AddOrRefreshBuff(Buff buff)
        {
            // reset if already in buffs list, otherwise add
            int index = GetBuffIndexByName(buff.name);
            if (index != -1) buffs[index] = buff;
            else buffs.Add(buff);
        }

        public bool HasPassiveBuff(Buff buff){
            int index = GetBuffIndexByName(buff.name);
            if (index != -1) return true;
            else return false;
        }

        // helper function to remove all buffs that ended
        public void CleanupBuffs()
        {
            for (int i = 0; i < buffs.Count; ++i)
            {
                if (buffs[i].BuffTimeRemaining() == 0 && !buffs[i].isPassive)
                //if (buffs[i].BuffTimeRemaining() == 0)
                {
                    buffs.RemoveAt(i);
                    --i;
                }
            }
        }

    }
}