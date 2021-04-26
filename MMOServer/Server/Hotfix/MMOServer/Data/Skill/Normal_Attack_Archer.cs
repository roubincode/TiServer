using ETModel;
/// 战斗普攻技能 Normal_Attack_Warrior
namespace ETHotfix
{
    public class Normal_Attack_Archer : TargetProjectileSkill
    {
        public Normal_Attack_Archer(){
            castRange = new LinearFloat(){baseValue = 15,bonusPerLevel=0.5f};
        }
    }
}