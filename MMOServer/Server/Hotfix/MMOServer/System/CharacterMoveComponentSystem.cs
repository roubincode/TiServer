using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [ObjectSystem]
    public class CharacterMoveComponentUpdateSystem : FixedUpdateSystem<CharacterMoveComponent>
    {
        public override void FixedUpdate(CharacterMoveComponent self)
        {
            self.FixedUpdate();
        }
    }

    public static class CharacterMoveComponentSystem
    {

        public static void TryMove(this CharacterMoveComponent self, Move target)
        {
            // 验证移动是否有效,延迟补偿等
            // ...

            // 验证通过或不需补偿，更新targetMove，获得服务器currFrame
            self.targetMove = target;
            
            // 获得移动速度
            // ...

            // 暂时使用baseMoveSpeed，实际上要加上buff,技能,装备,坐骑能产生的速度，这是一个变化的数值
            self.MoveTo(self.baseMoveSpeed);
        }

        public static void MoveTo(this CharacterMoveComponent self,float speed)
        {
            ++self.selfFrame;

            self.yRotation = self.targetMove.yRotation;
            self.yEuler = self.yRotation.ToYQ();
            if(self.selfFrame ==1) self.unit.Position = self.targetMove.position;

            // 移动速度 = 此帧移动的距离/时间(服务器上定的是1/60秒)
            float m_dis = Vector3.Distance(self.unit.Position, self.targetMove.position);
            self.velocity = m_dis / EventSystem.FixedUpdateTimeDelta;
            //Log.Info(to.ToString()+"/"+self.velocity.ToString());
            
            // 移动距离过大或过小都不更新移动位置
            float distance = Vector3.Distance(self.unit.Position, self.targetMove.position);
            if (distance < 0.02f || distance >15) return;

            self.startPosition = self.targetMove.position; 
            self.targetPosition = self.targetMove.position;
            self.moveSpeed = speed;
            
            // 计算移动到新位置需要的与结束的时间点
            self.startTime = TimeHelper.Now();
            float time = distance / speed;
            self.needTime = (long)(time * 1000);
            self.endTime = self.startTime + self.needTime;

            ++self.tempFrame;
        }


        public static void FixedUpdate(this CharacterMoveComponent self)
        {
            long timeNow = TimeHelper.Now();

            // 移动距离过大不进行插值移动
            float distance = Vector3.Distance(self.unit.Position, self.targetPosition);
            if (distance >15) return;

            // 目标move的位置与角度插值
            self.unit.Rotation = Quaternion.Slerp(self.unit.Rotation, self.yEuler, EventSystem.FixedUpdateTimeDelta * 15);
            if(self.unit.player!=null) self.unit.player.Rotation = self.unit.Rotation;

            float amount = (timeNow - self.startTime) * 1f / self.needTime;
            // Log.Info(amount.ToString()+"/"+(timeNow - self.startTime)+"/"+ self.needTime);
            Vector3 to = Vector3.Lerp(self.startPosition, self.targetPosition, amount);
            if(!float.IsNaN(to.x)) {
                self.unit.Position = to;
                if(self.unit.player!=null) self.unit.player.Position = self.unit.Position;
            }

            // 检测角色不再移动
            ++self.tempFrame;
            if(self.tempFrame >= 6)
            {
                self.tempFrame = 0;
                if(self.lastFrame == self.selfFrame) self.velocity = 0;
                self.lastFrame = self.selfFrame;
            } 
        }

    }
}

