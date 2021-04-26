using System;
using System.Reflection;
using ETModel;
using System.Collections.Generic;
using UnityEngine;
namespace ETHotfix
{
    [ObjectSystem]
    public class MMOStartSystem : StartSystem<MMOComponent>
    {
        public override void Start(MMOComponent self)
        {
            self.Start();
        }
    }
    
    public static class MMOComponentHelper
	{
        
		public static void Start(this MMOComponent self)
		{
			self.LoadSkillData();
            // self.LoadItemData();
            // sefl.LoadEquipData();

            // 初始服务器地图（没做专门的地图服务器，先在这里简单加载地图数据功能）
            self.InitMapServer();
		}

        public static void InitMapServer(this MMOComponent self){
            self.LoadMapData(7001);
            // 其它需要加载初始的地图数据
            // ...
        }

        public static void LoadMapData(this MMOComponent self,int mapId){
            // 简化一下，mapList 直接写死在此
            int[] maplist = new int [1]{7001};

            Assembly assem = typeof(MMOComponentHelper).Assembly;

            var mapConfigs = Game.Scene.GetComponent<ConfigComponent>().GetAll(typeof(MapConfig));
            MMOComponent.AllSprites = new Sprite[mapConfigs.Length];
            for (int k = 0;k<maplist.Length;k++){
                List<Sprite> roomSprites = new List<Sprite>();
                for (int i = 0;i<mapConfigs.Length;i++)
                {
                    MapConfig mapConfig = mapConfigs[i] as MapConfig;
                    // 根据配置文件中的技能TypeName，使用Activator.CreateInstance创建具体的技能数据对象实例
                    Sprite sprite = (Sprite)Activator.CreateInstance(assem.GetType("ETHotfix." + mapConfig.Class));
                    self.SetSpriteData(mapConfig,sprite); // 属性
                    MapHelper.WrapSprite(sprite); // 组件
                    MMOComponent.AllSprites[i] = sprite;
                    
                    // 各地图的精灵（npc,怪物）添加到对应mapId的集合中
                    if(sprite.mapId == maplist[k]) roomSprites.Add(sprite);
                    
                }
                MapHelper.mapSprites.Add(maplist[k],roomSprites);
            }
            
        }

        public static void SetSpriteData(this MMOComponent self,MapConfig mapConfig, Sprite sprite)
		{
            sprite.Class = mapConfig.Class;
            sprite.Name = mapConfig.Name;
            sprite.type = mapConfig.type;
            sprite.baseHealth = mapConfig.baseHealth;
            sprite.perHealth = mapConfig.perHealth;
            sprite.mapId = mapConfig.mapId;
            sprite.taskId = mapConfig.taskId;
            sprite.battleState = mapConfig.battleState;
            sprite.pickingsId = mapConfig.pickingsId;
            sprite.Position = new UnityEngine.Vector3(mapConfig.x,mapConfig.y,mapConfig.z);
            sprite.Rotation = Quaternion.Euler(new Vector3(0,mapConfig.Rotation,0));
            // ...
        }

        // 加载游戏的全部技能数据配置
        public static void LoadSkillData(this MMOComponent self)
        {
            Assembly assem = typeof(MMOComponentHelper).Assembly;

            var skillConfigs = Game.Scene.GetComponent<ConfigComponent>().GetAll(typeof(SkillConfig));
            MMOComponent.AllSkill = new SkillData[skillConfigs.Length];
            for (int i = 0;i<skillConfigs.Length;i++)
            {
                SkillConfig skillConfig = skillConfigs[i] as SkillConfig;
                // 根据配置文件中的技能TypeName，使用Activator.CreateInstance创建具体的技能数据对象实例
                SkillData skillData = (SkillData)Activator.CreateInstance(assem.GetType("ETHotfix." + skillConfig.Class));
                self.SetSKillData(skillConfig,skillData);
                MMOComponent.AllSkill[i] = skillData;
            }
        }
        
        public static void SetSKillData(this MMOComponent self,SkillConfig skillConfig, SkillData skillData)
		{
            skillData.SkillId = (int)skillConfig.Id;
            skillData.Name = skillConfig.Name;
            skillData.type = skillConfig.type;
            skillData.isPassive = skillConfig.isPassive;
            skillData.learnDefault = skillConfig.learnDefault;
            skillData.requiredLevel = new LinearInt(){baseValue = 0,bonusPerLevel = skillConfig.requiredLevel};
            skillData.maxLevel = skillConfig.maxLevel;
            skillData.manaCosts = new LinearInt(){baseValue = 0,bonusPerLevel = skillConfig.manaCosts};
            skillData.castTime = new LinearFloat(){baseValue = skillConfig.castTime,bonusPerLevel =0};
            skillData.cooldown = new LinearFloat(){baseValue = skillConfig.cooldown,bonusPerLevel = 0};
            skillData.damage = new LinearInt(){baseValue = skillConfig.baseDamage,bonusPerLevel = skillConfig.damage};
            skillData.physicalDamage = skillConfig.physicalDamage;
            skillData.healthMaxBonus = new LinearInt(){baseValue = 0,bonusPerLevel =skillConfig.healthMaxBonus };
            skillData.pType = skillConfig.pType;
            // ...
        }
		
	}
}