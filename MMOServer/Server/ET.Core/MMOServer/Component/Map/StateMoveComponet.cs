using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ETModel
{
    public class StateMoveComponent: Component
    {
        public Queue<Move> pendingMoves = new Queue<Move>();

        public Vector3 lastPosition;
        public Move lastMove;
        public Move gamerMove;

        public byte route = 0; 
        
        public int minMoveBuffer = 2;
        public int combineMovesAfter = 5;
        public int maxMoveBuffer = 10;

        public float rubberDistance = 1;
        public float epsilon = 0.1f;
        public float validSpeedTolerance = 0.2f;
        public float runSpeed = 8;
        public float swimSpeed = 4;

        public int rubbered = 0;
        public int combinedMoves = 0;

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
        }
    }
}
