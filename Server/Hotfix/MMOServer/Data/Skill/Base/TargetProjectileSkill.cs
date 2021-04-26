using System;
using UnityEngine;
using ETModel;
using System.Collections.Generic;
/// 远程投射类攻击技能

namespace ETHotfix
{
    public class TargetProjectileSkill : DamageSkill
    {
        public ProjectileSkillEffect effect; // 箭射击打,子弹炮弹，远程法术火球冰箭等 ...

        /// 远程技能对对武器弹药的检查
        bool HasRequiredWeaponAndAmmo(Entity caster)
        {
            Player player = caster as Player;

            // 对武器的要求
            if (string.IsNullOrWhiteSpace(requiredWeaponCategory))
                return true;
            
            // int weaponIndex = player.equipment.GetEquippedWeaponIndex();
            // if (weaponIndex != -1)
            // {
            //     // 不需要弹药，或者有弹药装备吗？
            //     WeaponItem itemData = (WeaponItem)player.equipment.slots[weaponIndex].item.data;
            //     return itemData.requiredAmmo == null ||
            //         player.equipment.GetItemIndexByName(itemData.requiredAmmo.name) != -1;
            // }
            return false;
        }

        /// 消耗所需武器弹药
        void ConsumeRequiredWeaponsAmmo(Entity caster)
        {
            Player player = caster as Player;

            if (string.IsNullOrWhiteSpace(requiredWeaponCategory))
                return;

            // int weaponIndex = player.equipment.GetEquippedWeaponIndex();
            // if (weaponIndex != -1)
            // {
            //     // 不需要弹药，或者有弹药装备吗？
            //     WeaponItem itemData = (WeaponItem)player.equipment.slots[weaponIndex].item.data;
            //     if (itemData.requiredAmmo != null)
            //     {
            //         int ammoIndex = player.equipment.GetItemIndexByName(itemData.requiredAmmo.name);
            //         if (ammoIndex != 0)
            //         {
            //             // reduce it
            //             ItemSlot slot = player.equipment.slots[ammoIndex];
            //             --slot.amount;
            //             player.equipment.slots[ammoIndex] = slot;
            //         }
            //     }
            // }
        }

        public override bool CheckSelf(Entity caster, int skillLevel)
        {
            return base.CheckSelf(caster, skillLevel) &&
                HasRequiredWeaponAndAmmo(caster);
        }

        public override bool CheckTarget(Entity caster)
        {
            // 目标存在, 存活, not self, 可攻击?
            return caster.target != null && caster.CanAttack(caster.target);
        }

        public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
        {
            // 目标还在附近?
            if (caster.target != null)
            {
                destination = Utils.ClosestPoint(caster.target, caster.Position);
                return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
            }
            destination = caster.Position;
            return false;
        }

        public override void Apply(Entity caster,int skillId, int skillLevel)
        {
            // 是否需要则减少弹药数量
            ConsumeRequiredWeaponsAmmo(caster);

            effect = (ProjectileSkillEffect)Activator.CreateInstance(Type.GetType("ETHotfix." + pType));
            effect.target = caster.target;
            effect.caster = caster;
            effect.damageType = damageType;
            effect.skillId = skillId;
            effect.damage = damage;
            effect.stunChance = stunChance.Get(skillLevel);
            effect.stunTime = stunTime.Get(skillLevel);

        }
    }
}
