using System.Text;
using UnityEngine;
namespace ETModel
{
    public abstract class DamageSkill : SkillData
    {
        public DamageType damageType;
        public AttackType attackType = AttackType.physical; 
        public LinearFloat stunChance; // range [0,1]
        public LinearFloat stunTime; // in seconds
    }
}