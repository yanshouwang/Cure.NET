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
    static class ICollectionExtension
    {
        /// <summary>
        /// 将项目范围添加到集合的末尾。如果集合是列表，则使用 List.AddRange。
        /// </summary>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (collection is List<T> objList)
            {
                objList.AddRange(newItems);
            }
            else
            {
                foreach (var newItem in newItems)
                {
                    collection.Add(newItem);
                }
            }
        }
    }
}
