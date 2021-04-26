// 基于等级的数据结构体，如技能等级，角色等级
// 比如：升级所需经验（前面讲过了），角色血蓝量，基础近战攻击
// 技能随技能等级增长攻击数值。
using System;
namespace ETModel
{
    public struct LinearInt
    {
        public int baseValue;
        public int bonusPerLevel;
        public int Get(int level) => bonusPerLevel * (level - 1) + baseValue;
    }

    public struct LinearLong
    {
        public long baseValue;
        public long bonusPerLevel;
        public long Get(int level) => bonusPerLevel * (level - 1) + baseValue;
    }

    public struct LinearFloat
    {
        public float baseValue;
        public float bonusPerLevel;
        public float Get(int level) => bonusPerLevel * (level - 1) + baseValue;
    }


    public struct ExponentialInt
    {
        public int multiplier;
        public float baseValue;
        public int Get(int level) => Convert.ToInt32(multiplier * Math.Pow(baseValue, (level - 1)));
    }

    public struct ExponentialLong
    {
        public long multiplier;
        public float baseValue;
        public long Get(int level) => Convert.ToInt64(multiplier * Math.Pow(baseValue, (level - 1)));
    }

    public struct ExponentialFloat
    {
        public float multiplier;
        public float baseValue;
        public float Get(int level) => multiplier * (float)Math.Pow(baseValue, (level - 1));
    }
}