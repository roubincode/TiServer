using UnityEngine;
namespace ETModel
{
    public class PlayerBrainComponent : CommonBrain
    {
        public bool continueCastAfterStunned = true;

        // events //////////////////////////////////////////////////////////////////
        public bool EventCancelAction(Player player)
        {
            bool result = player.cancelActionRequested;
            player.cancelActionRequested = false; // reset
            return result;
        }
        public bool EventRespawn(Player player)
        {
            bool result = player.respawnRequested;
            player.respawnRequested = false; // reset
            return result;
        }

        public void UseNextTargetIfAny(Player player)
        {
            if (player.nextTarget != null)
            {
                player.target = player.nextTarget;
                player.nextTarget = null;
            }
        }
    }
}