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
using System.Windows;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 统一 WPF 中的 PropertyMetadata 的接口。提供有关呈现、排列或度量的必要通知。
    /// </summary>
    internal class DrawingPropertyMetadata : FrameworkPropertyMetadata
    {
        public static event EventHandler<DrawingPropertyChangedEventArgs> DrawingPropertyChanged;

        private PropertyChangedCallback _propertyChangedCallback;

        public DrawingPropertyMetadata(object defaultValue)
            : this(defaultValue, DrawingPropertyMetadataOptions.None, null)
        {
        }

        public DrawingPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
            : this(DependencyProperty.UnsetValue,
                DrawingPropertyMetadataOptions.None,
                propertyChangedCallback)
        {
        }

        public DrawingPropertyMetadata(object defaultValue, DrawingPropertyMetadataOptions options)
          : this(defaultValue, options, null)
        {
        }

        public DrawingPropertyMetadata(
          object defaultValue,
          DrawingPropertyMetadataOptions options,
          PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue,
                (FrameworkPropertyMetadataOptions)options,
                AttachCallback(defaultValue, options, propertyChangedCallback))
        {
        }

        /// <summary>
        /// 应仅由 AttachCallback 使用此专用 Ctor。
        /// </summary>
        private DrawingPropertyMetadata(DrawingPropertyMetadataOptions options, object defaultValue)
          : base(defaultValue, (FrameworkPropertyMetadataOptions)options)
        {
        }

        /// <summary>
        /// 链接 InternalCallback() 以在属性回调上附加 DrawingPropertyMetadata 的实例。在 Silverlight 中，属性元数据在设置后被丢弃。使用回调记住它。
        /// </summary>
        private static PropertyChangedCallback AttachCallback(
           object defaultValue,
           DrawingPropertyMetadataOptions options,
           PropertyChangedCallback propertyChangedCallback)
        {
            DrawingPropertyMetadata metadata = new DrawingPropertyMetadata(options, defaultValue)
            {
                _propertyChangedCallback = propertyChangedCallback
            };
            return new PropertyChangedCallback(metadata.InternalCallback);
        }

        /// <summary>
        /// 链接原始回调之前，触发 DrawingPropertyChangedEvent。
        /// </summary>
        private void InternalCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DrawingPropertyChangedEventArgs args = new DrawingPropertyChangedEventArgs()
            {
                Metadata = this,
                Animated = DependencyPropertyHelper.GetValueSource(sender, e.Property).IsAnimated
            };
            DrawingPropertyChanged?.Invoke(sender, args);
            this._propertyChangedCallback?.Invoke(sender, e);
        }

        static DrawingPropertyMetadata()
        {
            DrawingPropertyChanged += (sender, args) =>
            {
                if (!(sender is IShape shape) || !args.Metadata.AffectsRender)
                    return;
                InvalidateGeometryReasons reasons = InvalidateGeometryReasons.PropertyChanged;
                if (args.Animated)
                {
                    reasons |= InvalidateGeometryReasons.IsAnimated;
                }
                shape.InvalidateGeometry(reasons);
            };
        }
    }
}
