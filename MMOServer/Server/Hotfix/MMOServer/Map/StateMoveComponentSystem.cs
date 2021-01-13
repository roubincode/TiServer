using System;
using System.Collections.Generic;
using ETModel;
using PF;
using UnityEngine;

namespace ETHotfix
{
    public static class StateMoveComponentHelper
    {
        public static void SyncStateMovement(this StateMoveComponent self, Move move)
        {
            // 获取输出数据，传入CharacterMovementComponet   
            
             
            // 向附近玩家广播gamerMove
            self.gamerMove = new Move(move.route, move.state, move.position, move.yRotation);
            MapHelper.BroadcastMove(self.gamerMove,self.GetParent<Unit>());
        }


        public static void RubberbandCheck(this StateMoveComponent self,Vector3 expectedPosition)
        {
            if (Vector3.Distance(self.lastPosition, expectedPosition) >= self.rubberDistance)
            {
                self.Warp(self.lastPosition);
                ++self.rubbered; // debug information only
            }
        }

        public static void Warp(this StateMoveComponent self, Vector3 destination)
		{	
			// ignore all incoming movements with old route
			++self.route;

			// clear buffer so we don't apply old moves
			self.pendingMoves.Clear();

			// 向客户端发送强制复位
			// RpcWarp(destination, self.route);
			
		}

        public static bool IsValidMove(this StateMoveComponent self, Vector3 move)
        {
            if (move.magnitude <= self.epsilon)
                return true;

            // are we in a state where movement is allowed (not dead etc.)?
            // return player.IsMovementAllowed();
            return true;
        }

        static bool WasValidSpeed(this StateMoveComponent self,Vector3 moveVelocity,
                            MoveState previousState, MoveState nextState, bool combining)
        {
            // calculate xz magnitude for max speed check.
            // we'll have to check y separately because gravity, sliding,
            // falling, jumping, etc. will cause faster speeds then max speed.
            float speed = new Vector2(moveVelocity.x, moveVelocity.z).magnitude;

            // are we going below maximum allowed speed for the given state?
            // -> we might be going from crouching to running or vice versa, so
            //    let's allow max speed of both states
            float maxSpeed = Mathf.Max(self.GetMaximumSpeedForState(previousState),
                                    self.GetMaximumSpeedForState(nextState));

            // calculate max speed with a small tolerance because it's never
            // exact when going over the network
            float maxSpeedWithTolerance = maxSpeed * (1 + self.validSpeedTolerance);

            // we allow twice the speed when applying a combined move
            if (combining)
                maxSpeedWithTolerance *= 2;

            // we should have a small tolerance for lags, latency, etc.
            //Debug.Log(name + " speed=" + speed + " / " + maxSpeedWithTolerance + " in state=" + targetState);
            if (speed <= maxSpeedWithTolerance)
            {
                return true;
            }
            else Log.Info(" move rejected because too fast: combining=" + combining + " xz speed=" + speed + " / " + maxSpeedWithTolerance + " state=" + previousState + "=>" + nextState);
            return false;
        }

        static float GetMaximumSpeedForState(this StateMoveComponent self,MoveState moveState)
        {
            switch (moveState)
            {
                // idle, running, mounted use runSpeed which is set by Entity
                case MoveState.IDLE:
                case MoveState.RUNNING:
                case MoveState.MOUNTED:
                    return self.runSpeed;
                // swimming uses swimSpeed
                case MoveState.SWIMMING:
                case MoveState.MOUNTED_SWIMMING:
                    return self.swimSpeed;
                // airborne accelerates with gravity.
                // maybe check xz and y speed separately.
                case MoveState.AIRBORNE:
                case MoveState.MOUNTED_AIRBORNE:
                    return float.MaxValue;
                case MoveState.DEAD:
                    return 0;
                default:
                    Log.Warning("Don't know how to calculate max speed for state: " + moveState);
                    return 0;
            }
        }

    }
}