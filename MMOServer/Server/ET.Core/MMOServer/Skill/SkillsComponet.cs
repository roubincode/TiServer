using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>Class <c>SkillsComponet</c> 
/// 用于实体对象的技能组件基础类,其它类似的还有:</summary>
namespace ETModel
{
    public abstract class SkillsComponent : Component, IAbility
    {
        public Entity entity;
        public Level level;
        public Health health;
        public Mana mana;

        protected SkillData[] skillTemplates;

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

        public M2C_StartSkill startSkill = new M2C_StartSkill();
        public M2C_FinishSkill finishSkill = new M2C_FinishSkill();

        public void Start(){
            entity = GetParent<Entity>();
            health = entity.GetComponent<Health>();
            level = entity.GetComponent<Level>();
            mana = entity.GetComponent<Mana>();
        }

        public void Update(){
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
        public int GetSkillIndexById(int skillId)
        {
            for (int i = 0; i < skills.Count; ++i)
                if (skills[i].skillId == skillId)
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