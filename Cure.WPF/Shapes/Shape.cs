// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/11: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Shapes
{
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public abstract class Shape : FrameworkElement
    {
        #region Constructors

        /// <summary>
        /// Shape Constructor
        /// </summary>
        protected Shape()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// DependencyProperty for the Stretch property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty
            = DependencyProperty.Register(
                "Stretch",                  // Property name
                typeof(Stretch),            // Property type
                typeof(Shape),              // Property owner
            new FrameworkPropertyMetadata(Stretch.None, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public Stretch Stretch
        {
            get => (Stretch)this.GetValue(StretchProperty);
            set => this.SetValue(StretchProperty, value);
        }

        /// <summary>
        /// The RenderedGeometry property returns the final rendered geometry
        /// </summary>
        public virtual Geometry RenderedGeometry
        {
            get
            {
                this.EnsureRenderedGeometry();

                Geometry geometry = this._renderedGeometry.CloneCurrentValue();
                if (geometry == null || geometry == Geometry.Empty)
                {
                    return Geometry.Empty;
                }

                // We need to return a frozen copy
                if (Object.ReferenceEquals(geometry, this._renderedGeometry))
                {
                    // geometry is a reference to _renderedGeometry, so we need to copy
                    geometry = geometry.Clone();
                    geometry.Freeze();
                }

                return geometry;
            }
        }

        /// <summary>
        /// Return the transformation applied to the geometry before rendering
        /// </summary>
        public virtual Transform GeometryTransform
        {
            get
            {
                BoxedMatrix stretchMatrix = StretchMatrixField.GetValue(this);

                if (stretchMatrix == null)
                {
                    return Transform.Identity;
                }
                else
                {
                    return new MatrixTransform(stretchMatrix.Value);
                }
            }
        }

        private static void OnPenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            // Called when any of the Stroke properties is invalidated.
            // That means that the cached pen should be recalculated.
            ((Shape)d)._pen = null;

        /// <summary>
        /// Fill property
        /// </summary>
        public static readonly DependencyProperty FillProperty =
                DependencyProperty.Register(
                        "Fill",
                        typeof(Brush),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                null,
                                FrameworkPropertyMetadataOptions.AffectsRender |
                                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        /// <summary>
        /// Fill property
        /// </summary>
        public Brush Fill
        {
            get => (Brush)this.GetValue(FillProperty);
            set => this.SetValue(FillProperty, value);
        }

        /// <summary>
        /// Stroke property
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
                DependencyProperty.Register(
                        "Stroke",
                        typeof(Brush),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                null,
                                FrameworkPropertyMetadataOptions.AffectsMeasure |
                                FrameworkPropertyMetadataOptions.AffectsRender |
                                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                                new PropertyChangedCallback(OnPenChanged)));

        /// <summary>
        /// Stroke property
        /// </summary>
        public Brush Stroke
        {
            get => (Brush)this.GetValue(StrokeProperty);
            set => this.SetValue(StrokeProperty, value);
        }

        /// <summary>
        /// StrokeThickness property
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
                DependencyProperty.Register(
                        "StrokeThickness",
                        typeof(double),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                1.0d,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                new PropertyChangedCallback(OnPenChanged)));

        /// <summary>
        /// StrokeThickness property
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double StrokeThickness
        {
            get => (double)this.GetValue(StrokeThicknessProperty);
            set => this.SetValue(StrokeThicknessProperty, value);
        }

        /// <summary>
        /// StrokeStartLineCap property
        /// </summary>
        public static readonly DependencyProperty StrokeStartLineCapProperty =
                DependencyProperty.Register(
                        "StrokeStartLineCap",
                        typeof(PenLineCap),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                PenLineCap.Flat,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                new PropertyChangedCallback(OnPenChanged)),
                        new ValidateValueCallback(ValidateEnums.IsPenLineCapValid));

        /// <summary>
        /// StrokeStartLineCap property
        /// </summary>
        public PenLineCap StrokeStartLineCap
        {
            get => (PenLineCap)this.GetValue(StrokeStartLineCapProperty);
            set => this.SetValue(StrokeStartLineCapProperty, value);
        }


        /// <summary>
        /// StrokeEndLineCap property
        /// </summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty =
                DependencyProperty.Register(
                        "StrokeEndLineCap",
                        typeof(PenLineCap),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                PenLineCap.Flat,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                new PropertyChangedCallback(OnPenChanged)),
                        new ValidateValueCallback(ValidateEnums.IsPenLineCapValid));

        /// <summary>
        /// StrokeEndLineCap property
        /// </summary>
        public PenLineCap StrokeEndLineCap
        {
            get => (PenLineCap)this.GetValue(StrokeEndLineCapProperty);
            set => this.SetValue(StrokeEndLineCapProperty, value);
        }


        /// <summary>
        /// StrokeDashCap property
        /// </summary>
        public static readonly DependencyProperty StrokeDashCapProperty =
                DependencyProperty.Register(
                        "StrokeDashCap",
                        typeof(PenLineCap),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                PenLineCap.Flat,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                new PropertyChangedCallback(OnPenChanged)),
                        new ValidateValueCallback(ValidateEnums.IsPenLineCapValid));

        /// <summary>
        /// StrokeDashCap property
        /// </summary>
        public PenLineCap StrokeDashCap
        {
            get => (PenLineCap)this.GetValue(StrokeDashCapProperty);
            set => this.SetValue(StrokeDashCapProperty, value);
        }

        /// <summary>
        /// StrokeLineJoin property
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
                DependencyProperty.Register(
                        "StrokeLineJoin",
                        typeof(PenLineJoin),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                PenLineJoin.Miter,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                new PropertyChangedCallback(OnPenChanged)),
                        new ValidateValueCallback(ValidateEnums.IsPenLineJoinValid));

        /// <summary>
        /// StrokeLineJoin property
        /// </summary>
        public PenLineJoin StrokeLineJoin
        {
            get => (PenLineJoin)this.GetValue(StrokeLineJoinProperty);
            set => this.SetValue(StrokeLineJoinProperty, value);
        }

        /// <summary>
        /// StrokeMiterLimit property
        /// </summary>
        public static readonly DependencyProperty StrokeMiterLimitProperty =
                DependencyProperty.Register(
                        "StrokeMiterLimit",
                        typeof(double),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                10.0,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                new PropertyChangedCallback(OnPenChanged)));

        /// <summary>
        /// StrokeMiterLimit property
        /// </summary>
        public double StrokeMiterLimit
        {
            get => (double)this.GetValue(StrokeMiterLimitProperty);
            set => this.SetValue(StrokeMiterLimitProperty, value);
        }

        /// <summary>
        /// StrokeDashOffset property
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
                DependencyProperty.Register(
                        "StrokeDashOffset",
                        typeof(double),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                                0.0,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                new PropertyChangedCallback(OnPenChanged)));

        /// <summary>
        /// StrokeDashOffset property
        /// </summary>
        public double StrokeDashOffset
        {
            get => (double)this.GetValue(StrokeDashOffsetProperty);
            set => this.SetValue(StrokeDashOffsetProperty, value);
        }

        /// <summary>
        /// StrokeDashArray property
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
                DependencyProperty.Register(
                        "StrokeDashArray",
                        typeof(DoubleCollection),
                        typeof(Shape),
                        new FrameworkPropertyMetadata(
                            new DoubleCollection(), // TODO: 修改了源代码
                            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                            new PropertyChangedCallback(OnPenChanged)));

        /// <summary>
        /// StrokeDashArray property
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)this.GetValue(StrokeDashArrayProperty);
            set => this.SetValue(StrokeDashArrayProperty, value);
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Updates DesiredSize of the shape.  Called by parent UIElement during is the first pass of layout.
        /// </summary>
        /// <param name="constraint">Constraint size is an "upper limit" that should not exceed.</param>
        /// <returns>Shape's desired size.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.CacheDefiningGeometry();

            Size newSize;

            Stretch mode = this.Stretch;

            if (mode == Stretch.None)
            {
                newSize = this.GetNaturalSize();
            }
            else
            {
                newSize = this.GetStretchedRenderSize(mode, this.GetStrokeThickness(), constraint, this.GetDefiningGeometryBounds());
            }

            if (this.SizeIsInvalidOrEmpty(newSize))
            {
                // We've encountered a numerical error. Don't draw anything.
                newSize = new Size(0, 0);
                this._renderedGeometry = Geometry.Empty;
            }

            return newSize;
        }

        /// <summary>
        /// Compute the rendered geometry and the stretching transform.
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size newSize;

            Stretch mode = this.Stretch;

            if (mode == Stretch.None)
            {
                StretchMatrixField.ClearValue(this);

                this.ResetRenderedGeometry();

                newSize = finalSize;
            }
            else
            {
                newSize = this.GetStretchedRenderSizeAndSetStretchMatrix(
                    mode, this.GetStrokeThickness(), finalSize, this.GetDefiningGeometryBounds());
            }

            if (this.SizeIsInvalidOrEmpty(newSize))
            {
                // We've encountered a numerical error. Don't draw anything.
                newSize = new Size(0, 0);
                this._renderedGeometry = Geometry.Empty;
            }

            return newSize;
        }

        /// <summary>
        /// Render callback.
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            this.EnsureRenderedGeometry();

            if (this._renderedGeometry != Geometry.Empty)
            {
                drawingContext.DrawGeometry(this.Fill, this.GetPen(), this._renderedGeometry);
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Get the geometry that defines this shape
        /// </summary>
        protected abstract Geometry DefiningGeometry
        {
            get;
        }

        #endregion Protected Properties

        #region Internal Methods

        internal bool SizeIsInvalidOrEmpty(Size size) => (double.IsNaN(size.Width) ||
                    double.IsNaN(size.Height) ||
                    size.IsEmpty);

        internal bool IsPenNoOp
        {
            get
            {
                double strokeThickness = this.StrokeThickness;
                return (this.Stroke == null) || double.IsNaN(strokeThickness) || strokeThickness.IsZero();
            }
        }

        internal double GetStrokeThickness()
        {
            if (this.IsPenNoOp)
            {
                return 0;
            }
            else
            {
                return Math.Abs(this.StrokeThickness);
            }

        }

        internal Pen GetPen()
        {
            if (this.IsPenNoOp)
            {
                return null;
            }

            if (this._pen == null)
            {
                double thickness = 0.0;
                double strokeThickness = this.StrokeThickness;

                thickness = Math.Abs(strokeThickness);

                // This pen is internal to the system and
                // must not participate in freezable treeness
                this._pen = new Pen
                {
                    //_pen.CanBeInheritanceContext = false;

                    Thickness = thickness,
                    Brush = this.Stroke,
                    StartLineCap = this.StrokeStartLineCap,
                    EndLineCap = this.StrokeEndLineCap,
                    DashCap = this.StrokeDashCap,
                    LineJoin = this.StrokeLineJoin,
                    MiterLimit = this.StrokeMiterLimit
                };

                // StrokeDashArray is usually going to be its default value and GetValue
                // on a mutable default has a per-instance cost associated with it so we'll
                // try to avoid caching the default value
                //DoubleCollection strokeDashArray = null;
                //if (GetValueSource(StrokeDashArrayProperty, null, out bool hasModifiers)
                //    != BaseValueSourceInternal.Default || hasModifiers)
                //{
                //    strokeDashArray = this.StrokeDashArray;
                //}
                // TODO: 修改了源代码
                DoubleCollection strokeDashArray = this.StrokeDashArray;

                // Avoid creating the DashStyle if we can
                double strokeDashOffset = this.StrokeDashOffset;
                if (strokeDashArray != null || strokeDashOffset != 0.0)
                {
                    this._pen.DashStyle = new DashStyle(strokeDashArray, strokeDashOffset);
                }
            }

            return this._pen;
        }

        // Double verification helpers.  Property system will verify type for us; we only need to verify the value.
        internal static bool IsDoubleFiniteNonNegative(object o)
        {
            double d = (double)o;
            return !(double.IsInfinity(d) || double.IsNaN(d) || d < 0.0);
        }
        internal static bool IsDoubleFinite(object o)
        {
            double d = (double)o;
            return !(double.IsInfinity(d) || double.IsNaN(d));
        }
        internal static bool IsDoubleFiniteOrNaN(object o)
        {
            double d = (double)o;
            return !(double.IsInfinity(d));
        }

        internal virtual void CacheDefiningGeometry() { }

        internal Size GetStretchedRenderSize(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds)
        {

            this.GetStretchMetrics(mode, strokeThickness, availableSize, geometryBounds,
                out double xScale, out double yScale, out double dX, out double dY, out Size renderSize);

            return renderSize;
        }

        internal Size GetStretchedRenderSizeAndSetStretchMatrix(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds)
        {

            this.GetStretchMetrics(mode, strokeThickness, availableSize, geometryBounds,
                out double xScale, out double yScale, out double dX, out double dY, out Size renderSize);

            // Construct the matrix
            Matrix stretchMatrix = Matrix.Identity;
            stretchMatrix.ScaleAt(xScale, yScale, geometryBounds.Location.X, geometryBounds.Location.Y);
            stretchMatrix.Translate(dX, dY);
            StretchMatrixField.SetValue(this, new BoxedMatrix(stretchMatrix));

            this.ResetRenderedGeometry();

            return renderSize;
        }

        internal void ResetRenderedGeometry() =>
            // reset rendered geometry
            this._renderedGeometry = null;

        internal void GetStretchMetrics(Stretch mode, double strokeThickness, Size availableSize, Rect geometryBounds,
                                             out double xScale, out double yScale, out double dX, out double dY, out Size stretchedSize)
        {
            if (!geometryBounds.IsEmpty)
            {
                double margin = strokeThickness / 2;
                bool hasThinDimension = false;

                // Initialization for mode == Fill
                xScale = Math.Max(availableSize.Width - strokeThickness, 0);
                yScale = Math.Max(availableSize.Height - strokeThickness, 0);
                dX = margin - geometryBounds.Left;
                dY = margin - geometryBounds.Top;

                // Compute the scale factors from the geometry to the size.
                // The scale factors are ratios, and they have already been initialize to the numerators.
                // To prevent fp overflow, we need to make sure that numerator / denomiator < limit;
                // To do that without actually deviding, we check that denominator > numerator / limit.
                // We take 1/epsilon as the limit, so the check is denominator > numerator * epsilon

                // See Dev10 bug #453150.
                // If the scale is infinite in both dimensions, return the natural size.
                // If it's infinite in only one dimension, for non-fill stretch modes we constrain the size based
                // on the unconstrained dimension.
                // If our shape is "thin", i.e. a horizontal or vertical line, we can ignore non-fill stretches.
                if (geometryBounds.Width > xScale * double.Epsilon)
                {
                    xScale /= geometryBounds.Width;
                }
                else
                {
                    xScale = 1;
                    // We can ignore uniform and uniform-to-fill stretches if we have a vertical line.
                    if (geometryBounds.Width == 0)
                    {
                        hasThinDimension = true;
                    }
                }

                if (geometryBounds.Height > yScale * double.Epsilon)
                {
                    yScale /= geometryBounds.Height;
                }
                else
                {
                    yScale = 1;
                    // We can ignore uniform and uniform-to-fill stretches if we have a horizontal line.
                    if (geometryBounds.Height == 0)
                    {
                        hasThinDimension = true;
                    }
                }

                // Because this case was handled by the caller
                Debug.Assert(mode != Stretch.None);

                // We are initialized for Fill, but for the other modes
                // If one of our dimensions is thin, uniform stretches are
                // meaningless, so we treat the stretch as fill.
                if (mode != Stretch.Fill && !hasThinDimension)
                {
                    if (mode == Stretch.Uniform)
                    {
                        if (yScale > xScale)
                        {
                            // Resize to fit the size's width
                            yScale = xScale;
                        }
                        else // if xScale >= yScale
                        {
                            // Resize to fit the size's height
                            xScale = yScale;
                        }
                    }
                    else
                    {
                        Debug.Assert(mode == Stretch.UniformToFill);

                        if (xScale > yScale)
                        {
                            // Resize to fill the size vertically, spilling out horizontally
                            yScale = xScale;
                        }
                        else // if yScale >= xScale
                        {
                            // Resize to fill the size horizontally, spilling out vertically
                            xScale = yScale;
                        }
                    }
                }

                stretchedSize = new Size(geometryBounds.Width * xScale + strokeThickness, geometryBounds.Height * yScale + strokeThickness);
            }
            else
            {
                xScale = yScale = 1;
                dX = dY = 0;
                stretchedSize = new Size(0, 0);
            }
        }

        /// <summary>
        /// Get the natural size of the geometry that defines this shape
        /// </summary>
        internal virtual Size GetNaturalSize()
        {
            Geometry geometry = this.DefiningGeometry;

            Debug.Assert(geometry != null);

            //
            // For the purposes of computing layout size, don't consider dashing. This will give us
            // slightly different bounds, but the computation will be faster and more stable.
            //
            // NOTE: If GetPen() is ever made public, we will need to change this logic so the user
            // isn't affected by our surreptitious change of DashStyle.
            //
            Pen pen = this.GetPen();
            DashStyle style = null;

            if (pen != null)
            {
                style = pen.DashStyle;

                if (style != null)
                {
                    pen.DashStyle = null;
                }
            }

            Rect bounds = geometry.GetRenderBounds(pen);

            if (style != null)
            {
                pen.DashStyle = style;
            }

            return new Size(Math.Max(bounds.Right, 0),
                Math.Max(bounds.Bottom, 0));
        }

        /// <summary>
        /// Get the bonds of the geometry that defines this shape
        /// </summary>
        internal virtual Rect GetDefiningGeometryBounds()
        {
            Geometry geometry = this.DefiningGeometry;

            Debug.Assert(geometry != null);

            return geometry.Bounds;
        }

        internal void EnsureRenderedGeometry()
        {
            if (this._renderedGeometry == null)
            {
                this._renderedGeometry = this.DefiningGeometry;

                Debug.Assert(this._renderedGeometry != null);

                if (this.Stretch != Stretch.None)
                {
                    Geometry currentValue = this._renderedGeometry.CloneCurrentValue();
                    if (Object.ReferenceEquals(this._renderedGeometry, currentValue))
                    {
                        this._renderedGeometry = currentValue.Clone();
                    }
                    else
                    {
                        this._renderedGeometry = currentValue;
                    }

                    Transform renderedTransform = this._renderedGeometry.Transform;

                    BoxedMatrix boxedStretchMatrix = StretchMatrixField.GetValue(this);
                    Matrix stretchMatrix = (boxedStretchMatrix == null) ? Matrix.Identity : boxedStretchMatrix.Value;
                    if (renderedTransform == null || renderedTransform.IsIdentity())
                    {
                        this._renderedGeometry.Transform = new MatrixTransform(stretchMatrix);
                    }
                    else
                    {
                        this._renderedGeometry.Transform = new MatrixTransform(renderedTransform.Value * stretchMatrix);
                    }
                }
            }
        }

        #endregion Internal Methods

        #region Private Fields

        private Pen _pen = null;

        private Geometry _renderedGeometry = Geometry.Empty;

        private static UncommonField<BoxedMatrix> StretchMatrixField = new UncommonField<BoxedMatrix>(null);

        #endregion Private Fields
    }

    internal class BoxedMatrix
    {
        public BoxedMatrix(Matrix value)
        {
            this.Value = value;
        }

        public Matrix Value;
    }
}
