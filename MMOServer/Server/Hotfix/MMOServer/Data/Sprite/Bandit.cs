using UnityEngine;
using ETModel;
namespace ETHotfix
{
	public class Bandit: Sprite
	{
		public Bandit(){
			this.bounds = new Bounds(new Vector3(0f, 1.2f, 0f),new Vector3(0.3f, 0.4f, 0.2f));
		}
    }
}