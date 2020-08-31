// https://referencesource.microsoft.com/#PresentationCore/Core/CSharp/System/Windows/Media/Animation/Generated/DoubleAnimation.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cure.WPF.Media.Animation
{
    public class GeometryAnimation : GeometryAnimationBase
    {
        #region 字段
        Geometry[] _keyValues;
        AnimationType _animationType;
        bool _isAnimationFunctionValid;
        #endregion

        #region 属性
        public Geometry From
        {
            get => (Geometry)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(
                nameof(From),
                typeof(Geometry),
                typeof(GeometryAnimation),
                new PropertyMetadata(default, OnAnimationFunctionChanged),
                ValidateFromToOrByValue);

        public Geometry To
        {
            get => (Geometry)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(
                nameof(To),
                typeof(Geometry),
                typeof(GeometryAnimation),
                new PropertyMetadata(default, OnAnimationFunctionChanged),
                ValidateFromToOrByValue);

        public Geometry By
        {
            get => (Geometry)GetValue(ByProperty);
            set => SetValue(ByProperty, value);
        }

        // Using a DependencyProperty as the backing store for By.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ByProperty =
            DependencyProperty.Register(
                nameof(By),
                typeof(Geometry),
                typeof(GeometryAnimation),
                new PropertyMetadata(default, OnAnimationFunctionChanged),
                ValidateFromToOrByValue);

        public IEasingFunction EasingFunction
        {
            get => (IEasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        // Using a DependencyProperty as the backing store for EasingFunction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction),
                typeof(IEasingFunction),
                typeof(GeometryAnimation));

        public bool Additive
        {
            get => (bool)GetValue(IsAdditiveProperty);
            set => SetValue(IsAdditiveProperty, value);
        }

        public bool Cumulative
        {
            get => (bool)GetValue(IsCumulativeProperty);
            set => SetValue(IsCumulativeProperty, value);
        }
        #endregion

        #region 构造
        public GeometryAnimation()
            : base()
        {

        }
        #endregion

        #region 方法
        static void OnAnimationFunctionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var a = (GeometryAnimation)d;
            a._isAnimationFunctionValid = false;
            //a.PropertyChanged(e.Property);
        }

        static bool ValidateFromToOrByValue(object value)
        {
            Geometry geometry = (Geometry)value;
            if (geometry != null)
            {
                return AnimatedTypeHelper.IsValidAnimationValueGeometry(geometry);
            }
            return true;
        }

        void ValidateAnimationFunction()
        {
            _animationType = AnimationType.Automatic;
            _keyValues = null;
            if (From != null)
            {
                if (To != null)
                {
                    _animationType = AnimationType.FromTo;
                    _keyValues = new Geometry[2];
                    _keyValues[0] = From;
                    _keyValues[1] = To;
                }
                else if (By != null)
                {
                    _animationType = AnimationType.FromBy;
                    _keyValues = new Geometry[2];
                    _keyValues[0] = From;
                    _keyValues[1] = By;
                }
                else
                {
                    _animationType = AnimationType.From;
                    _keyValues = new Geometry[1];
                    _keyValues[0] = From;
                }
            }
            else if (To != null)
            {
                _animationType = AnimationType.To;
                _keyValues = new Geometry[1];
                _keyValues[0] = To;
            }
            else if (By != null)
            {
                _animationType = AnimationType.By;
                _keyValues = new Geometry[1];
                _keyValues[0] = By;
            }
            _isAnimationFunctionValid = true;
        }
        #endregion

        #region Freezable
        public new GeometryAnimation Clone()
        {
            return (GeometryAnimation)base.Clone();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GeometryAnimation();
        }
        #endregion

        protected override Geometry GetCurrentValueCore(Geometry defaultOriginValue, Geometry defaultDestinationValue, AnimationClock animationClock)
        {
            Debug.Assert(animationClock.CurrentState != ClockState.Stopped);
            var progress = animationClock.CurrentProgress.Value;
            if (EasingFunction != null)
            {
                progress = EasingFunction.Ease(progress);
            }
            // 将 Geometry 转化为 PathGeometry 便于计算动画的当前值
            // value = from + (to - from) * progress
            var fromGeometry = PathGeometry.CreateFromGeometry(From);
            var toGeometry = PathGeometry.CreateFromGeometry(To);
            var figures = new List<PathFigure>();
            for (int i = 0; i < fromGeometry.Figures.Count; i++)
            {
                var fromFigure = fromGeometry.Figures[i];
                var toFigure = toGeometry.Figures[i];
                var fromStart = fromFigure.StartPoint;
                var toStart = toFigure.StartPoint;
                var start = fromStart + (toStart - fromStart) * progress;
                var segments = new List<PathSegment>();
                for (int j = 0; j < fromFigure.Segments.Count; j++)
                {
                    var fromSegment = fromFigure.Segments[j];
                    var toSegment = toFigure.Segments[j];
                    PathSegment segment;
                    if (fromSegment is LineSegment fromLine && toSegment is LineSegment toLine)
                    {
                        var fromPoint = fromLine.Point;
                        var toPoint = toLine.Point;
                        var point = fromPoint + (toPoint - fromPoint) * progress;
                        var stroked = fromLine.IsStroked;
                        segment = new LineSegment(point, stroked);
                    }
                    else if (fromSegment is ArcSegment fromArc && toSegment is ArcSegment toArc)
                    {
                        var fromPoint = fromArc.Point;
                        var toPoint = toArc.Point;
                        var point = fromPoint + (toPoint - fromPoint) * progress;
                        var fromWidth = fromArc.Size.Width;
                        var toWidth = toArc.Size.Width;
                        var width = fromWidth + (toWidth - fromWidth) * progress;
                        var fromHeigth = fromArc.Size.Height;
                        var toHeight = toArc.Size.Height;
                        var height = fromHeigth + (toHeight - fromHeigth) * progress;
                        var size = new Size(width, height);
                        var fromAngle = fromArc.RotationAngle;
                        var toAngle = toArc.RotationAngle;
                        var angle = fromAngle + (toAngle - fromAngle) * progress;
                        var largeArc = fromArc.IsLargeArc;
                        var sweepDirection = fromArc.SweepDirection;
                        var stroked = fromArc.IsStroked;
                        segment = new ArcSegment(point, size, angle, largeArc, sweepDirection, stroked);
                    }
                    else if (fromSegment is BezierSegment fromBezier && toSegment is BezierSegment toBezier)
                    {
                        var fromPoint1 = fromBezier.Point1;
                        var toPoint1 = toBezier.Point1;
                        var point1 = fromPoint1 + (toPoint1 - fromPoint1) * progress;
                        var fromPoint2 = fromBezier.Point2;
                        var toPoint2 = toBezier.Point2;
                        var point2 = fromPoint2 + (toPoint2 - fromPoint2) * progress;
                        var fromPoint3 = fromBezier.Point3;
                        var toPoint3 = toBezier.Point3;
                        var point3 = fromPoint3 + (toPoint3 - fromPoint3) * progress;
                        var stroked = fromBezier.IsStroked;
                        segment = new BezierSegment(point1, point2, point3, stroked);
                    }
                    else if (fromSegment is QuadraticBezierSegment fromQuadraticBezier && toSegment is QuadraticBezierSegment toQuadraticBezier)
                    {
                        var fromPoint1 = fromQuadraticBezier.Point1;
                        var toPoint1 = toQuadraticBezier.Point1;
                        var point1 = fromPoint1 + (toPoint1 - fromPoint1) * progress;
                        var fromPoint2 = fromQuadraticBezier.Point2;
                        var toPoint2 = toQuadraticBezier.Point2;
                        var point2 = fromPoint2 + (toPoint2 - fromPoint2) * progress;
                        var stroked = fromQuadraticBezier.IsStroked;
                        segment = new QuadraticBezierSegment(point1, point2, stroked);
                    }
                    else if (fromSegment is PolyLineSegment fromPolyLine && toSegment is PolyLineSegment toPolyLine)
                    {
                        var points = new List<Point>();
                        for (int k = 0; k < fromPolyLine.Points.Count; k++)
                        {
                            var fromPoint = fromPolyLine.Points[k];
                            var toPoint = toPolyLine.Points[k];
                            var point = fromPoint + (toPoint - fromPoint) * progress;
                            points.Add(point);
                        }
                        var stroked = fromPolyLine.IsStroked;
                        segment = new PolyLineSegment(points, stroked);
                    }
                    else if (fromSegment is PolyBezierSegment fromPolyBezier && toSegment is PolyBezierSegment toPolyBezier)
                    {
                        var points = new List<Point>();
                        for (int k = 0; k < fromPolyBezier.Points.Count; k++)
                        {
                            var fromPoint = fromPolyBezier.Points[k];
                            var toPoint = toPolyBezier.Points[k];
                            var point = fromPoint + (toPoint - fromPoint) * progress;
                            points.Add(point);
                        }
                        var stroked = fromPolyBezier.IsStroked;
                        segment = new PolyBezierSegment(points, stroked);
                    }
                    else if (fromSegment is PolyQuadraticBezierSegment fromPolyQuadraticBezier && toSegment is PolyQuadraticBezierSegment toPolyQuadraticBezier)
                    {
                        var points = new List<Point>();
                        for (int k = 0; k < fromPolyQuadraticBezier.Points.Count; k++)
                        {
                            var fromPoint = fromPolyQuadraticBezier.Points[k];
                            var toPoint = toPolyQuadraticBezier.Points[k];
                            var point = fromPoint + (toPoint - fromPoint) * progress;
                            points.Add(point);
                        }
                        var stroked = fromPolyQuadraticBezier.IsStroked;
                        segment = new PolyQuadraticBezierSegment(points, stroked);
                    }
                    else
                    {
                        throw new NotImplementedException("暂不支持此转换");
                    }
                    segments.Add(segment);
                }
                var closed = fromFigure.IsClosed;
                var figure = new PathFigure(start, segments, closed);
                figures.Add(figure);
            }
            // FillRule 和 Transform 不支持动画, 默认使用 From 的值
            var fillRule = fromGeometry.FillRule;
            var transfrom = fromGeometry.Transform;
            var geometry = new PathGeometry(figures, fillRule, transfrom);
            return geometry;
        }
    }
}
