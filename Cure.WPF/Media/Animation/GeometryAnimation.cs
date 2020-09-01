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
                return AnimationHelper.IsValidAnimationValueGeometry(geometry);
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

        protected override Geometry GetCurrentValueCore(Geometry defaultOriginValue, Geometry defaultDestinationValue, AnimationClock animationClock)
        {
            Debug.Assert(animationClock.CurrentState != ClockState.Stopped);
            if (!_isAnimationFunctionValid)
            {
                ValidateAnimationFunction();
            }
            var progress = animationClock.CurrentProgress.Value;
            if (EasingFunction != null)
            {
                progress = EasingFunction.Ease(progress);
            }
            Geometry from;
            Geometry to;
            Geometry accumulated;
            Geometry foundation;
            var validateOrigin = false;
            var validateDestination = false;
            switch (_animationType)
            {
                case AnimationType.Automatic:
                    from = defaultOriginValue;
                    to = defaultDestinationValue;
                    foundation = accumulated = AnimationHelper.CreateEmptyGeometry(from);
                    validateOrigin = true;
                    validateDestination = true;
                    break;
                case AnimationType.From:
                    from = _keyValues[0];
                    to = defaultDestinationValue;
                    foundation = accumulated = AnimationHelper.CreateEmptyGeometry(from);
                    validateDestination = true;
                    break;
                case AnimationType.To:
                    from = defaultOriginValue;
                    to = _keyValues[0];
                    foundation = accumulated = AnimationHelper.CreateEmptyGeometry(from);
                    validateOrigin = true;
                    break;
                case AnimationType.By:
                    foundation = defaultOriginValue;
                    to = _keyValues[0];
                    from = accumulated = AnimationHelper.CreateEmptyGeometry(foundation);
                    validateOrigin = true;
                    break;
                case AnimationType.FromTo:
                    from = _keyValues[0];
                    to = _keyValues[1];
                    if (Additive)
                    {
                        foundation = defaultOriginValue;
                        accumulated = AnimationHelper.CreateEmptyGeometry(foundation);
                        validateOrigin = true;
                    }
                    else
                    {
                        foundation = accumulated = AnimationHelper.CreateEmptyGeometry(from);
                    }
                    break;
                case AnimationType.FromBy:
                    from = _keyValues[0];
                    to = AnimationHelper.AddGeometry(_keyValues[0], _keyValues[1]);
                    if (Additive)
                    {
                        foundation = defaultOriginValue;
                        accumulated = AnimationHelper.CreateEmptyGeometry(foundation);
                        validateOrigin = true;
                    }
                    else
                    {
                        foundation = accumulated = AnimationHelper.CreateEmptyGeometry(from);
                    }
                    break;
                default:
                    Debug.Fail("Unknown animation type");
                    foundation = accumulated = from = to = new PathGeometry();
                    break;
            }
            if (validateOrigin && !AnimationHelper.IsValidAnimationValueGeometry(defaultOriginValue))
            {
                throw new InvalidOperationException();
            }
            if (validateDestination && !AnimationHelper.IsValidAnimationValueGeometry(defaultDestinationValue))
            {
                throw new InvalidOperationException();
            }
            if (Cumulative)
            {
                var currentRepeat = (double)(animationClock.CurrentIteration - 1);
                if (currentRepeat > 0.0)
                {
                    var accumulator = AnimationHelper.SubtractGeometry(to, from);
                    accumulated = AnimationHelper.ScaleGeometry(accumulator, currentRepeat);
                }
            }
            // return foundation + accumulated + from + ((to - from) * progress)
            var geometry1 = AnimationHelper.AddGeometry(foundation, accumulated);
            var geometry2 = AnimationHelper.InterpolateGeometry(from, to, progress);
            return AnimationHelper.AddGeometry(geometry1, geometry2);
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
    }
}
