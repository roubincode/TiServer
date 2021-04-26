using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security.Cryptography;
using System.Reflection;
using UnityEngine;

namespace ETModel
{
    public class Utils
    {
        // 仅适用于float和int，这个方法支持更多
        public static long Clamp(long value, long min, long max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        // 两个闭合点之间的距离
        // Vector3.Distance(a.transform.position, b.transform.position):
        //    _____        _____
        //   |     |      |     |
        //   |  x==|======|==x  |
        //   |_____|      |_____|
        //
        //
        // Utils.ClosestDistance(a.collider, b.collider):
        //    _____        _____
        //   |     |      |     |
        //   |     |x====x|     |
        //   |_____|      |_____|
        public static float ClosestDistance(Entity a, Entity b)
        {
            float distance = Vector3.Distance(a.Position, b.Position);

            // 两个碰撞体的半径
            float radiusA = BoundsRadius(a.bounds);
            float radiusB = BoundsRadius(b.bounds);

            // 减去两个半径
            float distanceInside = distance - radiusA - radiusB;

            // 返回距离。如果它小于0，它们在彼此内部，那么返回0
            return Mathf.Max(distanceInside, 0);
        }
        public static float BoundsRadius(Bounds bounds) =>
        (bounds.extents.x + bounds.extents.z) / 2;

        // // 从实体的碰撞器到另一个点的最近点
        public static Vector3 ClosestPoint(Entity entity, Vector3 point)
        {
            float radius = BoundsRadius(entity.bounds);

            Vector3 direction = entity.Position - point;
            //Debug.DrawLine(point, point + direction, Color.red, 1, false);

            // 从direction length减去半径
            Vector3 directionSubtracted = Vector3.ClampMagnitude(direction, direction.magnitude - radius);

            // return the point
            //Debug.DrawLine(point, point + directionSubtracted, Color.green, 1, false);
            return point + directionSubtracted;
        }

        // 解析字符串中最后一个大写名词
        //   EquipmentWeaponBow => Bow
        //   EquipmentShield => Shield
        static Regex lastNountRegEx = new Regex(@"([A-Z][a-z]*)"); // cache to avoid allocations. this is used a lot.
        public static string ParseLastNoun(string text)
        {
            MatchCollection matches = lastNountRegEx.Matches(text);
            return matches.Count > 0 ? matches[matches.Count-1].Value : "";
        }


        // NIST推荐的PBKDF2哈希
        // http://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-132.pdf
        // salt should be at least 128 bits = 16 bytes
        public static string PBKDF2Hash(string text, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(text, saltBytes, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        // 利用反射，通过前缀调用多个函数
        // -> 缓存它，以便足够快地进行更新调用
        static Dictionary<KeyValuePair<Type,string>, MethodInfo[]> lookup = new Dictionary<KeyValuePair<Type,string>, MethodInfo[]>();
        public static MethodInfo[] GetMethodsByPrefix(Type type, string methodPrefix)
        {
            KeyValuePair<Type, string> key = new KeyValuePair<Type, string>(type, methodPrefix);
            if (!lookup.ContainsKey(key))
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                                        .Where(m => m.Name.StartsWith(methodPrefix))
                                        .ToArray();
                lookup[key] = methods;
            }
            return lookup[key];
        }

        public static void InvokeMany(Type type, object onObject, string methodPrefix, params object[] args)
        {
            foreach (MethodInfo method in GetMethodsByPrefix(type, methodPrefix))
                method.Invoke(onObject, args);
        }

        // clamp a rotation around x axis
        // public static Quaternion ClampRotationAroundXAxis(Quaternion q, float min, float max)
        // {
        //     q.x /= q.w;
        //     q.y /= q.w;
        //     q.z /= q.w;
        //     q.w = 1.0f;

        //     float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
        //     angleX = Mathf.Clamp (angleX, min, max);
        //     q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

        //     return q;
        // }
    }
}