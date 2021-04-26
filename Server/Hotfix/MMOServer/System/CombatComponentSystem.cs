using ETModel;
using UnityEngine;
using System;

namespace ETHotfix
{
    [ObjectSystem]
    public class CombatComponentStartSystem : StartSystem<CombatComponent>
    {
        public override void Start(CombatComponent self)
        {
            self.Start();
        }
    }

    public static class CombatComponentSystem
    {
        /// <summary> 根据施放的攻击技能类型返回物理或法术攻击数值 </summary>
        public static int Attack(this CombatComponent self, AttackType type)
        {
            int damage = 0;
            if(type == AttackType.magic){
                damage = self.magicDamage;
            }else if(type == AttackType.physical)
            {
                damage = self.attackStrength;
            }
            // 对伤害进行浮动
            System.Random r1 = new System.Random();
            System.Random r2 = new System.Random();
            
            // 以20%左右的damage数值上下浮动
            float d1 = damage*(r1.Next(-10,25)*0.01f); 
            int d2 = r2.Next(1,50);

            return damage + (int)d1 +d2;
        }

        /// <summary> 根据施放的攻击技能类型返回物理或法术防御数值 Defensive attack </summary>
        public static int DeAttack(this CombatComponent self, AttackType type)
        {
            int defense = 0;
            if(type == AttackType.magic){
                defense = self.magicDefense;
            }else if(type == AttackType.physical)
            {
                defense = self.physicalDefense;
            }
            return defense;
        }

        /// <summary> 根据施放的攻击技能类型返回物理或法术暴击数值 </summary>
        public static float AttackCritical(this CombatComponent self, AttackType type){
            float critical = 0;
            if(type == AttackType.magic){
                critical = self.magicCritical;
            }else if(type == AttackType.physical)
            {
                critical = self.physicCritical + 0.3f; //暂时加30暴击看效果
            }
            return critical;
        }

        /// <summary>还没有服务端这里模仿一个处理技能伤害的方法 </summary>
        public static void DealDamageAt(this CombatComponent self, Entity caster, AttackType type,
             out int d,int skillDamage,  float stunChance=0, float stunTime=0)
        {
            Entity victim = caster.target; // 目标实体
            CombatComponent vCombat = victim.combat; // 目标的Combat组件
            

            int damageDealt = 0;
            int deAmount = 0;
            d = 0;
            int amount = caster.combat.Attack(type) + skillDamage;
            if(vCombat != null) deAmount = vCombat.DeAttack(type);

            // 是不是不可战胜的,比如npc
            if (!vCombat.invincible)
            {
                self.damageType = DamageType.Normal;
                // dodge
                if (RandomHelper.Randomf() < vCombat.dodgeChance)
                {
                    self.damageType = DamageType.Block;
                }
                // deal damage
                else
                {
                    // 减少去防御与抵抗效果
                    damageDealt = Mathf.Max(amount - deAmount, 1);

                    // 触发暴击
                    if (RandomHelper.Randomf() < self.AttackCritical(type))
                    {
                        damageDealt = damageDealt*2;
                        self.damageType = DamageType.Crit;
                    }

                    // 处理伤害结果
                    d = damageDealt;
                    victim.health.current -= damageDealt;

                }

                // 通知发起攻击方武器掉耐久和防御攻击方装备掉耐久
                // ...

                // 攻击目标是否死亡
                // ...
            }

            // 判断目标aggro范围,决定是否继续追击
            // victim.OnAggro(entity);

            // 从服务端向前端发消息
            // RpcSendDamaged(victim.GetComponent<Player>().UnitId,damageDealt, self.damageType);
            Log.Info($"对目标造成{damageDealt}点伤害------------>");

            // 是否有机率造成目标眩晕
            if (RandomHelper.Randomf() < stunChance)
            {
                // 只需要更新角色的stunTimeEnd属性，角色的状态update中就会更新眩晕状态
                double newStunEndTime = NetworkTime.time + stunTime;
                victim.stunTimeEnd = Math.Max(newStunEndTime, caster.stunTimeEnd);
            }
        }
    }

}