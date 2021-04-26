using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETModel
{
	public abstract class SkillData 
	{
		public int SkillId { get; set; }
        public string Class ;
		public string Name;
        public string type;
		public bool isPassive;
        public bool learnDefault;

        public LinearInt requiredLevel;
        public int maxLevel;

        public LinearInt manaCosts;
		public LinearFloat castTime;
		public LinearFloat cooldown;

        public LinearInt damage;

        public int physicalDamage;
        public int magicDamage;
        public LinearInt healthMaxBonus;
        public LinearInt manaMaxBonus;

        public string pType;

        public SkillData predecessor; 
        public int predecessorLevel = 1; 
		public LinearFloat castRange;
        public bool cancelCastIfTargetDied;
        public bool followupDefaultAttack = true;
        public bool allowMovement;
        
        public string requiredWeaponCategory;
        public LinearLong requiredExperience;
        
        public bool showCastBar; 

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
            // 有武器,hp, mp?
            return caster.health.current > 0 &&
                caster.mana.current >= manaCosts.Get(skillLevel);
                // && CheckWeapon(caster);
        }

        // 2. 目标检查
        public abstract bool CheckTarget(Entity caster);

        // 3. 距离检查
        public abstract bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination);

        // 4. 执行技能: deal damage, heal, launch projectiles, etc.
        public abstract void Apply(Entity caster,int skillId, int skillLevel);


        // caching /////////////////////////////////////////////////////////////////
        static Dictionary<int, SkillData> cache;
        public static Dictionary<int, SkillData> dict
        {
            get
            {
                // not loaded yet?
                if (cache == null)
                {
                    SkillData[] skills = MMOComponent.AllSkill;

                    // 检查重复项，然后添加到缓存
                    List<string> duplicates = skills.ToList().FindDuplicates(skill => skill.Name);
                    if (duplicates.Count == 0)
                    {
                        cache = skills.ToDictionary(skill => skill.SkillId, skill => skill);
                    }
                    else
                    {
                        foreach (string duplicate in duplicates)
                            Log.Error("存在重复的技能数据 name " + duplicate + ". ");
                    }
                }
                return cache;
            }
        }

	}
}
