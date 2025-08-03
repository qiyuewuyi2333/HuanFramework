using System.Collections.Generic;
using UnityEngine;

namespace Huan.Framework.Helper
{
    public static class CollectionExtensions
    {
        private static System.Random _random = new System.Random();

        /// <summary>
        /// 从集合中随机选择一个元素
        /// </summary>
        public static T RandomChoice<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                return default(T);

            int index = _random.Next(0, list.Count);
            return list[index];
        }

        /// <summary>
        /// 从集合中随机选择多个元素（不重复）
        /// </summary>
        public static List<T> RandomChoice<T>(this IList<T> list, int count)
        {
            if (list == null || list.Count == 0 || count <= 0)
                return new List<T>();

            count = Mathf.Min(count, list.Count);
            List<T> result = new List<T>();
            List<T> tempList = new List<T>(list);

            for (int i = 0; i < count; i++)
            {
                int index = _random.Next(0, tempList.Count);
                result.Add(tempList[index]);
                tempList.RemoveAt(index);
            }

            return result;
        }

        /// <summary>
        /// 使用Unity的Random.Range实现随机选择
        /// </summary>
        public static T RandomChoiceUnity<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                return default(T);

            int index = Random.Range(0, list.Count);
            return list[index];
        }
    }
}