// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Cure.WPF
{
    static class IListExtension
    {
        /// <summary>
        /// 获取给定列表的最后一个项目。
        /// </summary>
        public static T Last<T>(this IList<T> list)
            => list[list.Count - 1];

        /// <summary>
        /// 从给定列表中删除最后一个项目。
        /// </summary>
        public static void RemoveLast<T>(this IList<T> list)
            => list.RemoveAt(list.Count - 1);

        /// <summary>
        /// 确保列表计数达到给定计数。用给定的工厂进行创建或在必要时删除项目。如果输入 IList 为列表，则在没有工厂时使用 AddRange 或 RemoveRange。
        /// </summary>
        public static bool EnsureListCount<T>(this IList<T> list, int count, Func<T> factory = null)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (list.EnsureListCountAtLeast(count, factory))
                return true;
            if (list.Count <= count)
                return false;
            if (list is List<T> objList)
            {
                objList.RemoveRange(count, list.Count - count);
            }
            else
            {
                for (var index = list.Count - 1; index >= count; --index)
                    list.RemoveAt(index);
            }
            return true;
        }

        /// <summary>
        /// 确保列表计数至少为给定计数。用给定的工厂进行创建。
        /// </summary>
        public static bool EnsureListCountAtLeast<T>(this IList<T> list, int count, Func<T> factory = null)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (list.Count >= count)
                return false;
            if (list is List<T> objList && factory == null)
            {
                objList.AddRange(new T[count - list.Count]);
            }
            else
            {
                for (var count1 = list.Count; count1 < count; ++count1)
                    list.Add(factory == null ? default : factory());
            }
            return true;
        }
    }
}
