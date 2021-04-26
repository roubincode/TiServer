using UnityEngine;

namespace ETModel
{
	public class Sprite: Entity
	{
		public Entity entity {get;set;}
		public NetworkIdentity identity{ get; set; }

		public uint netId;
		public string Class ;
        public string Name;
        public string type;

		public int baseHealth;
		public int perHealth;

		public int mapId;

        public int taskId;

        public int battleState;

        public long pickingsId;

    }
}