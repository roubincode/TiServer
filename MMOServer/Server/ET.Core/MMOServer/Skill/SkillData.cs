using UnityEngine;
namespace ETModel
{
	public abstract class SkillData 
	{
		public long Id { get; set; }
		public string Name;
		public LinearInt manaCosts;
        public LinearFloat castTime;
        public LinearFloat cooldown;
        public LinearFloat castRange;
        public LinearInt requiredLevel; 
        public LinearLong requiredExperience;
        public bool learnDefault;
        public bool cancelCastIfTargetDied;
        public bool followupDefaultAttack;
        public bool allowMovement;
        public int maxLevel;
        public string requiredWeaponCategory;
        public bool isPassive;
        public bool showCastBar;
        public int damage;
        public string type;
        public float stunChance; 
        public float stunTime;  

        bool CheckWeapon(Entity caster)
        {

            // 对武器的要求
            if (string.IsNullOrWhiteSpace(requiredWeaponCategory))
                return true;

            // 对武器类型的要求
            // if (caster.baseEquipment.GetEquippedWeaponCategory().StartsWith(requiredWeaponCategory))
            // {
            //     // 耐久度检查
            //     int weaponIndex = caster.baseEquipment.GetEquippedWeaponIndex();
            //     if (weaponIndex != -1)
            //     {
            //         return caster.baseEquipment.slots[weaponIndex].item.CheckDurability();
            //     }
            // }
            return false;
        }


        public virtual bool CheckSelf(Entity caster, int skillLevel)
        {
            // 有武器, no cooldown, hp, mp?
            // return caster.health.current > 0 &&
            //     caster.mana.current >= manaCosts.Get(skillLevel) &&
            //     CheckWeapon(caster);

            return true;
        }

        // 2. 目标检查
        public abstract bool CheckTarget(Entity caster);

        // 3. 距离检查
        public abstract bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination);

        // 4. 执行技能: deal damage, heal, launch projectiles, etc.
        public abstract void Apply(Entity caster, int skillLevel);

	}
}
