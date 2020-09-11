// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Cure.WPF.Media;

namespace Cure.WPF.Shapes
{
    /// <summary>
    /// 呈现弧形，支持 ArcThickness 控制的弧形、环形和饼形模式。
    /// </summary>
    public sealed class Arc : PrimitiveShape, IArcGeometrySourceParameters, IGeometrySourceParameters
    {
        #region PrimitiveShape

        protected override IGeometrySource CreateGeometrySource()
            => new ArcGeometrySource();

        #endregion

        #region IArcGeometrySourceParameters

        /// <summary>
        /// 获取或设置开始角度。
        /// </summary>
        /// <value>
        /// 以度为单位的开始角度。0 度朝上。
        /// </value>
        public double StartAngle
        {
            get => (double)this.GetValue(StartAngleProperty);
            set => this.SetValue(StartAngleProperty, value);
        }

        // Using a DependencyProperty as the backing store for StartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(
                nameof(StartAngle),
                typeof(double),
                typeof(Arc),
                new DrawingPropertyMetadata(0.0, DrawingPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置结束角度。
        /// </summary>
        /// <value>
        /// 以度为单位的结束角度。0 度朝上。
        /// </value>
        public double EndAngle
        {
            get => (double)this.GetValue(EndAngleProperty);
            set => this.SetValue(EndAngleProperty, value);
        }

        // Using a DependencyProperty as the backing store for EndAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register(
                nameof(EndAngle),
                typeof(double),
                typeof(Arc),
                new DrawingPropertyMetadata(90.0, DrawingPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置弧粗细。
        /// </summary>
        /// <value>
        /// 弧粗细以像素或百分比为单位，具体取决于“ArcThicknessUnit”。
        /// </value>
        public double ArcThickness
        {
            get => (double)this.GetValue(ArcThicknessProperty);
            set => this.SetValue(ArcThicknessProperty, value);
        }

        // Using a DependencyProperty as the backing store for ArcThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArcThicknessProperty =
            DependencyProperty.Register(
                nameof(ArcThickness),
                typeof(double),
                typeof(Arc),
                new DrawingPropertyMetadata(0.0, DrawingPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// 获取或设置弧粗细单位。
        /// </summary>
        /// <value>
        /// 弧粗细单位（像素或百分比）。
        /// </value>
        public UnitType ArcThicknessUnit
        {
            get => (UnitType)this.GetValue(ArcThicknessUnitProperty);
            set => this.SetValue(ArcThicknessUnitProperty, value);
        }

        // Using a DependencyProperty as the backing store for ArcThicknessUnit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArcThicknessUnitProperty =
            DependencyProperty.Register(
                nameof(ArcThicknessUnit),
                typeof(UnitType),
                typeof(Arc),
                new DrawingPropertyMetadata(UnitType.Pixel, DrawingPropertyMetadataOptions.AffectsRender));

        #endregion

        #region IGeometrySourceParameters

        [SpecialName]
        Stretch IGeometrySourceParameters.Stretch => this.Stretch;
        [SpecialName]
        Brush IGeometrySourceParameters.Stroke => this.Stroke;
        [SpecialName]
        double IGeometrySourceParameters.StrokeThickness => this.StrokeThickness;

        #endregion
    }
}
