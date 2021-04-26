using System;
using UnityEngine;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
namespace ETModel
{
    public abstract partial class Entity : ComponentWithId
    {
        public Unit Unit;
        public Entity target { get; set; }

        public SkillsComponent skillsCom;
        public CharacterMoveComponent movement;
        public CombatComponent combat;
        // public Inventory baseInventory;
        // public Equipment baseEquipment;
        // public Experience experience;

        // public PlayerInventory inventory => baseInventory as PlayerInventory;
        // public PlayerEquipment equipment => baseEquipment as PlayerEquipment;

        [BsonIgnore]
        public Bounds bounds;

        /// <summary>
        /// 角色等级  
        /// </summary>
        public Level level;
        /// <summary>
        /// 角色血量
        /// </summary>
        public Health health;
        /// <summary>
        /// 角色蓝量  
        /// </summary>
        public Mana mana;

        /// <summary>
        /// 角色眩晕结束时间
        /// </summary>
        public double stunTimeEnd;

        /// <summary>
        /// 角色是否在安全区
        /// </summary>
        public bool inSafeZone;

        /// <summary>
        /// 角色战斗或脱战状态  
        /// </summary>
        public bool inbattle = false; 

        long _gold = 0;
        /// <summary>
        /// 角色拥有的金币  
        /// </summary>
        public long gold { 
            get { return _gold; } 
            set { _gold = Math.Max(value, 0); }
        }

        /// <summary>
        /// 角色动作状态
        /// </summary>
        string _state = "IDLE";
        public string state{
            get { return _state; } 
            set { _state = value; }
        }

        public float attackToMoveRangeRatio = 0.8f;
        public int useSkillWhenCloser = -1;

        [BsonIgnore]
        public Vector3 Position { get; set; }
        [BsonIgnore]
		public Quaternion Rotation { get; set; } 

        // CanAttack /////////////////////////////////////////////////////////////
        /// -><summary>判断是否可攻击目标。主要是判断双方都存活，并且不是攻击自己</summary>
        public virtual bool CanAttack(Entity entity)
        {
            return health.current > 0 && 
                entity.health.current > 0 &&
                entity != this ;
        }

        // death /////////////////////////////////////////////////////////////
        /// -><summary><c>OnDeath</c> 角色死亡时可以被子类调用的虚方法,
        /// 清除角色的目标属性值。</summary>
        public virtual void OnDeath()
        {
            //调用死亡动画，声音，躺了
            Log.Info("啊!我躺了"); //暂时用在输出面板打印一句话代替
            // 清除目标
            target = null;
        }

        // Revive ///////////////////////////////////////////////////////////////////
        public void Revive(float healthPercentage = 1,float manaPercentage = 1)
        {
            health.current = Mathf.RoundToInt(health.max * healthPercentage);
            mana.current = Mathf.RoundToInt(health.max * manaPercentage);
        }

        public K[] GetComponents<K>() where K : class 
		{
			List<K> list = new List<K>();
			foreach(Component c in this.componentDict.Values){
				if (c is K) list.Add(c as K);
			}
			return list.ToArray();
		}
    }
}
