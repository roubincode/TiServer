using ETModel;
/// 战斗普攻技能 Normal_Attack_Warrior
namespace ETHotfix
{
    public class Normal_Attack_Warrior : TargetDamageSkill
    {
        public Normal_Attack_Warrior(){
            castRange = new LinearFloat(){baseValue = 2,bonusPerLevel=0};
        }
    }
}