using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public static class NetworkDataConvert
    {
        public static Vector3Info ToV3Info(this Vector3 vector3)
        {
            return new Vector3Info()
            {
                X = vector3.x,
                Y = vector3.y,
                Z = vector3.z
            };
        }
        public static Vector3 ToV3(this Vector3Info vector3)
        {
            return new Vector3()
            {
                x = vector3.X,
                y = vector3.Y,
                z = vector3.Z
            };
        }

        public static SpriteInfo ToSprInfo(this Sprite sprite)
        {
            return new SpriteInfo()
            {
                Class = sprite.Class,
                Name = sprite.Name,
                Type = sprite.type,
                Position = sprite.Position.ToV3Info(),
                Rotation = sprite.Rotation.ToYF(),
                MapId = sprite.mapId,
                TaskId = sprite.taskId,
                BattleState = sprite.battleState,
                PickingsId = sprite.pickingsId,
                NetId =  (int)sprite.netId
            };
        }

        public static Quaternion ToYQ(this float rotation)
        {
            return Quaternion.Euler(new Vector3(0,rotation,0));
        }

        public static float ToYF(this Quaternion rotation)
        {
            return Quaternion.QuaternionToEuler(rotation).y;
        }

        public static Sprite ToSprite(this SpriteInfo info)
        {
            return new Sprite()
            {
                Class = info.Class,
                Name = info.Name,
                type = info.Type,
                Position = info.Position.ToV3(),
                Rotation = info.Rotation.ToYQ(),
                mapId = info.MapId,
                taskId = info.TaskId,
                battleState = info.BattleState,
                pickingsId = info.PickingsId,
                netId = (uint)info.NetId
            };
        }
    }
}
