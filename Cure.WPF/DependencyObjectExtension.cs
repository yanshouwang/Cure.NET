// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System.Windows;

namespace Cure.WPF
{
    static class DependencyObjectExtension
    {
        /// <summary>
        /// 如果不同，则设置值。在可能的情况下避免设置本地值。在值发生了更改的情况下返回 true。
        /// </summary>
        public static bool SetIfDifferent(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, object value)
        {
            if (Equals(dependencyObject.GetValue(dependencyProperty), value))
                return false;
            dependencyObject.SetValue(dependencyProperty, value);
            return true;
        }

        /// <summary>
        /// 在针对给定依赖对象以本地方式设置了依赖属性的情况下删除该依赖属性。在未以本地方式设置依赖属性的情况下返回 false。
        /// </summary>
        public static bool ClearIfSet(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            if (dependencyObject.ReadLocalValue(dependencyProperty) == DependencyProperty.UnsetValue)
                return false;
            dependencyObject.ClearValue(dependencyProperty);
            return true;
        }
    }
}
