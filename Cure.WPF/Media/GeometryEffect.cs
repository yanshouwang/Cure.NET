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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 为将几何图形转换为另一种几何图形的 GeometryEffect 提供基类。
    /// </summary>
    /// <remarks>
    /// 此类为以下操作提供基本实现：在传递 IShape 以进行呈现之前处理 IShape 的已呈现几何图形。典型实现将扩展虚拟函数 <see cref="ProcessGeometry" /> 以转换输入的几何图形。<see cref="GeometryEffect" /> 通常将作为附加的属性附加到 <see cref="IShape" />，并在更新 <see cref="IShape" /> 几何图形时被激活。<see cref="GeometryEffect" /> 的 <see cref="OutputGeometry" /> 将替换 <see cref="IShape" /> 中呈现的几何图形。
    /// </remarks>
    [TypeConverter(typeof(GeometryEffectConverter))]
    public abstract class GeometryEffect : Freezable
    {
        private static GeometryEffect _defaultGeometryEffect;
        private bool _effectInvalidated;

        /// <summary>
        /// 指定上次处理几何图形效果后产生的几何图形。
        /// </summary>
        protected Geometry CachedGeometry { get; set; }

        /// <summary>
        /// 获取作为给定依赖对象附加属性的几何图形效果。
        /// </summary>
        public static GeometryEffect GetGeometryEffect(DependencyObject obj)
            => (GeometryEffect)obj.GetValue(GeometryEffectProperty);

        /// <summary>
        /// 设置作为给定依赖对象附加属性的几何图形效果。
        /// </summary>
        public static void SetGeometryEffect(DependencyObject obj, GeometryEffect value)
            => obj.SetValue(GeometryEffectProperty, value);

        public static readonly DependencyProperty GeometryEffectProperty =
            DependencyProperty.RegisterAttached(
                nameof(GeometryEffect),
                typeof(GeometryEffect),
                typeof(GeometryEffect),
                new DrawingPropertyMetadata(DefaultGeometryEffect, DrawingPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGeometryEffectChanged)));

        private static void OnGeometryEffectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GeometryEffect oldValue = e.OldValue as GeometryEffect;
            GeometryEffect newValue = e.NewValue as GeometryEffect;
            if (oldValue == newValue)
                return;
            if (oldValue != null && obj.Equals(oldValue.Parent))
                oldValue.Detach();
            if (newValue == null)
                return;
            if (newValue.Parent != null)
            {
                Action method = new Action(() =>
                {
                    GeometryEffect geometryEffect = newValue.CloneCurrentValue();
                    obj.SetValue(GeometryEffectProperty, geometryEffect);
                });
                obj.Dispatcher.BeginInvoke(method, DispatcherPriority.Send, null);
            }
            else
            {
                newValue.Attach(obj);
            }
        }

        /// <summary>
        /// 使用 <see cref="GeometryEffect" /> 的当前值对其进行深层复制。
        /// </summary>
        public new GeometryEffect CloneCurrentValue()
            => (GeometryEffect)base.CloneCurrentValue();

        /// <summary>
        /// 对几何图形效果进行深层复制。
        /// </summary>
        /// <returns>几何图形效果的当前实例的克隆。</returns>
        protected abstract GeometryEffect DeepCopy();

        /// <summary>
        /// 测试给定几何图形效果是否等效于当前实例。
        /// </summary>
        /// <param name="geometryEffect">用来比较的几何图形效果。</param>
        /// <returns>当两种效果呈现相同的外观时返回 true。</returns>
        public abstract bool Equals(GeometryEffect geometryEffect);

        /// <summary>
        /// 仅作用于输入的几何图形的默认几何图形效果。
        /// </summary>
        public static GeometryEffect DefaultGeometryEffect
            => _defaultGeometryEffect ??= new NoGeometryEffect();

        /// <summary>
        /// 获取此几何图形效果的输出几何图形。
        /// </summary>
        public Geometry OutputGeometry
            => this.CachedGeometry;

        static GeometryEffect()
        {
            DrawingPropertyMetadata.DrawingPropertyChanged += (sender, args) =>
            {
                if (!(sender is GeometryEffect geometryEffect) || !args.Metadata.AffectsRender)
                    return;
                geometryEffect.InvalidateGeometry(InvalidateGeometryReasons.PropertyChanged);
            };
        }

        /// <summary>
        /// 使几何图形效果无效而不实际计算几何图形。通知所有父形状或效果以相应地使之无效。
        /// </summary>
        public bool InvalidateGeometry(InvalidateGeometryReasons reasons)
        {
            if (this._effectInvalidated)
                return false;
            this._effectInvalidated = true;
            if (reasons != InvalidateGeometryReasons.ParentInvalidated)
                InvalidateParent(this.Parent);
            return true;
        }

        /// <summary>
        /// 处理对给定输入几何图形的几何图形效果。将结果存储在 GeometryEffect.OutputGeometry 中。
        /// </summary>
        /// <returns>如果未更改任何内容，则返回 false。</returns>
        public bool ProcessGeometry(Geometry input)
        {
            bool flag = false;
            if (this._effectInvalidated)
            {
                flag |= this.UpdateCachedGeometry(input);
                this._effectInvalidated = false;
            }
            return flag;
        }

        /// <summary>
        /// 扩展基于给定输入几何图形更新 CachedGeometry 的方式。
        /// </summary>
        protected abstract bool UpdateCachedGeometry(Geometry input);

        /// <summary>
        /// 父项可以是 IShape 或 GeometryEffectGroup。
        /// </summary>
        protected internal DependencyObject Parent { get; private set; }

        /// <summary>
        /// 从父链中分离时收到通知。
        /// </summary>
        protected internal virtual void Detach()
        {
            this._effectInvalidated = true;
            this.CachedGeometry = null;
            if (this.Parent == null)
                return;
            InvalidateParent(this.Parent);
            this.Parent = null;
        }

        /// <summary>
        /// 附加到父链时收到通知。
        /// </summary>
        protected internal virtual void Attach(DependencyObject obj)
        {
            if (this.Parent != null)
                this.Detach();
            this._effectInvalidated = true;
            this.CachedGeometry = null;
            if (!InvalidateParent(obj))
                return;
            this.Parent = obj;
        }

        /// <summary>
        /// 在对象为有效的父类型（IShape 或 GeometryEffect）时使给定依赖对象上的几何图形无效。
        /// </summary>
        private static bool InvalidateParent(DependencyObject parent)
        {
            switch (parent)
            {
                case IShape shape:
                    shape.InvalidateGeometry(InvalidateGeometryReasons.ChildInvalidated);
                    return true;
                case GeometryEffect geometryEffect:
                    geometryEffect.InvalidateGeometry(InvalidateGeometryReasons.ChildInvalidated);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 在 WPF 中实现该 Freezable。
        /// </summary>
        protected override Freezable CreateInstanceCore()
            => (Freezable)Activator.CreateInstance(this.GetType());

        private class NoGeometryEffect : GeometryEffect
        {
            protected override bool UpdateCachedGeometry(Geometry input)
            {
                this.CachedGeometry = input;
                return false;
            }

            protected override GeometryEffect DeepCopy()
                => new NoGeometryEffect();

            public override bool Equals(GeometryEffect geometryEffect)
                => geometryEffect == null || geometryEffect is NoGeometryEffect;
        }
    }
}