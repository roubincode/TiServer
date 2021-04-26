using UnityEngine;

//角色等级组件，经验的增长改变等级
namespace ETModel
{
    public class Level : Component
    {
        /// <summary>
        /// 角色当前等级
        /// </summary>
        public int current = 1;
        /// <summary>
        /// 角色最高等级
        /// </summary>
        public int max = 1;

  
    }
}