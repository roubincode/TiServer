using UnityEngine;
using System;

// ============================================
// 施法时间与效果加成关系暂时没有
// 法术免疫随N次数递减（16 秒後从新计算）规则暂时没有
// 未考虑主属性对单手/双手武器的差别作用，如需要增加 float PrefHand(ActOnType type)
// 未考虑主属性对不同职业的差别作用,如需要增加 int PrefProfession(ActOnType type)
    //-> 力量/敏捷型物理职业对力量/敏捷到物攻强度换算不同
    //-> 物理职业对敏捷到物理暴击/闪避的换算不同
    //-> 法术职业对精神到回蓝、法术暴击的换算不同
    //-> 物理/法术职业从武器获得的近战攻击换算不同
namespace ETModel
{
    public enum AttackType{ physical ,magic }
    public enum DamageType { Normal, Block, Crit }

    public class CombatComponent :  Component
    {
        /// <summary>
        /// 角色是否无敌不可战胜
        /// </summary>
        public bool invincible = false;

        public Entity entity;

        /// <summary>
        /// 获取角色的Level组件,public但不序列化
        /// </summary>
        public Level level;

        public DamageType damageType;


        ////////////////////////////////////////////////////////////////////////////
        // 获取及缓存 角色上的装备，技能，宠物坐骑等组件实例
        ICombatBonus[] _bonusComponents;
        ICombatBonus[] bonusComponents =>
            _bonusComponents ?? (_bonusComponents = this.GetParent<Entity>().GetComponents<ICombatBonus>());


        public LinearInt baseArmor = new LinearInt{baseValue=10};
        public LinearFloat baseDodge = new LinearFloat{baseValue=0f}; 


        ////////////////////////////////////////////////////////////////////////////
        // 攻防相关的基础属性的get方法
        // -> 通过下面这些方法计算角色的各种基础属性值
        /// <summary>获得角色敏捷属性值</summary>
        public int agility
        {
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetAgilityBonus();
                return bonus;
            }
        }
        /// <summary>获得角色力量属性值</summary>
        public int strength
        {
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetStrengthBonus();
                return bonus;
            }
        }
        /// <summary>获得角色精神属性值</summary>
        public int spirit
        {
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetSpiritBonus();
                return bonus;
            }
        }
        /// <summary>获得角色智力属性值</summary>
        public int intellect
        {
            get
            {
                int bonus = 0;
                foreach (IManaBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetIntellectBonus();
                return bonus;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        // 攻击伤害属性的get方法
        // -> 通过下面这些方法计算角色的各种战斗相关属性值
        /// <summary>总物理攻击强度，结合秒伤与物理攻击强度</summary>
        /// 总攻击强度 = 秒伤*攻速 + 额外的物理伤害加成
        /// 课程只做了单武器,如果你要做双手武器,那就是分别计算左右手的攻击强度与速度
        public int attackStrength
        {
            get
            {
                int bonus = 0;
                bonus += (int)(damagePerSecond*attackRate)+physicalDamage;
                return bonus;
            }
        }
        /// <summary>秒伤,从武器装备获得的dps值</summary>
        public float damagePerSecond
        {
            get
            {
                float bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetDpsBonus();
                return bonus;
            }
        }
        /// <summary>从武器装备获得的攻速值</summary>
        public float attackRate
        {
            get
            {
                float bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetAttackRateBonus();
                return bonus;
            }
        }
        /// <summary>武器伤害值,当有些技能可以产生武器伤害时可以调用</summary>
        public int hurtDamage
        {
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetHurtBonus();
                return  bonus;
            }
        }
        /// <summary>物理攻击强度，除从秒伤得来的物理伤害外，额外产生的的物理伤害加成</summary>
        public int physicalDamage
        {
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetPhysicalBonus();
                return bonus + strength / 6 + agility / 12 ;
            }
        }
        /// <summary>获得法术伤害强度</summary>
        public int magicDamage
        {
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetMagicBonus();
                return bonus + (int)spirit/4;
            }
        }


        ////////////////////////////////////////////////////////////////////////////
        // 伤害防御属性的get方法
        // -> 通过下面这些方法计算角色的各种防御相关属性值
        /// <summary> 获得防御护甲属性值 </summary>
        public int armorDefense
        {
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetArmorBonus();
                return baseArmor.Get(level.current) + bonus + agility * 2;
            }
        }
        /// <summary> 获得的物理抗性值 </summary>
        public int physicalDefense{
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetPhysicalDefenseBonus();
                return bonus;
            }
        }
        /// <summary> 获得的法术抗性值 </summary>
        public int magicDefense
        {
            get
            {
                int bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetMagicDefenseBonus();
                return bonus;
            }
        }
        /// <summary> 获得的抵抗伤害比率 </summary>
        public float blockChance
        {
            get
            {
                float bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetBlockBonus();
                return bonus;
            }
        }
        /// <summary> 获得的闪避机率 </summary>
        public float dodgeChance
        {
            get
            {
                float bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetDodgeBonus();
                return baseDodge.Get(level.current) + bonus + (float)agility/20*0.01f;
            }
        }

        
        ////////////////////////////////////////////////////////////////////////////
        // 命中属性的get方法
        // -> 通过下面这些方法计算角色的命中属性值
        /// <summary> 获得的法术命中 </summary>
        public float magicHitrate
        {
            get
            {
                float bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetMagicHitrateBonus();
                return bonus;
            }
        }
        /// <summary> 获得的物理命中 </summary>
        public float physicHitrate
        {
            get
            {
                float bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetPhysicHitrateBonus();
                return bonus;
            }
        }


        ////////////////////////////////////////////////////////////////////////////
        // 暴击属性的get方法
        // -> 通过下面这些方法计算角色的暴击属性值
        /// <summary> 获得的法术暴击 </summary>
        public float magicCritical
        {
            get
            {
                float bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetMagicCriticalBonus();
                return  bonus + (float)intellect/42f;
            }
        }
        /// <summary> 获得的物理暴击 </summary>
        public float physicCritical
        {
            get
            {
                float bonus = 0;
                foreach (ICombatBonus bonusComponent in bonusComponents)
                    bonus += bonusComponent.GetPhysicCriticalBonus();
                return  bonus + (float)agility/35f;
            }
        }


        public void Start()
        {
            entity = GetParent<Entity>();
            level = GetParent<Entity>().GetComponent<Level>();
        }
        
    }
}