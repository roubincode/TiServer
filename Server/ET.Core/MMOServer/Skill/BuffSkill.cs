// buff skill 基类
// => 可用于有目标buff，无目标buff，aoe buff等。
using System.Text;
using UnityEngine;

namespace ETModel
{
    public abstract class BuffSkill : BonusSkill
    {
        public bool remainAfterDeath;
        public LinearFloat buffTime = new LinearFloat{baseValue=60};
    }
}