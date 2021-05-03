using System;
using UnityEngine;
namespace ETModel
{
    public struct Move
    {
        public MoveState state;
        public int frame;
        public Vector3 position; 
        public float yRotation;
        public float nSpeed;
        public float jumpLeg;
        public Move(MoveState state, int frame ,Vector3 position,
                     float yRotation,float nSpeed,float jumpLeg)
        {
            this.state = state;
            this.frame = frame;
            this.position = position;
            this.yRotation = yRotation;
            this.nSpeed = nSpeed;
            this.jumpLeg  = jumpLeg;
        }
    }

    
    public struct SkillInfo {
        public string name{get;set;}
        public int skillId{get;set;}
        public float distance {get;set;}
        public int level {get;set;}
        public float castTime {get;set;}
        public float cooldown {get;set;}
    }

    public enum MoveState : byte { IDLE, RUNNING, AIRBORNE, SWIMMING, MOUNTED, MOUNTED_AIRBORNE, MOUNTED_SWIMMING, DEAD }
}