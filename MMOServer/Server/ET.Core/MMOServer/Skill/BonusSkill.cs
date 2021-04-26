using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETModel
{
	public abstract class BonusSkill : SkillData
	{        
        public LinearInt damageBonus;
        public LinearInt armorBonus;
        public LinearFloat blockChanceBonus; // range [0,1]
        public LinearFloat dodgeChanceBonus; // range [0,1]
        public LinearFloat criticalChanceBonus; // range [0,1]
        public LinearFloat healthPercentPerSecondBonus; // 0.1=10%; can be negative too
        public LinearFloat manaPercentPerSecondBonus; // 0.1=10%; can be negative too
        public LinearFloat speedBonus; // 也可能是负面的
        
    }
}