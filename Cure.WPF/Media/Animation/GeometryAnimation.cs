// https://referencesource.microsoft.com/#PresentationCore/Core/CSharp/System/Windows/Media/Animation/Generated/DoubleAnimation.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cure.WPF.Media.Animation
{
    /// <summary>
    /// Animates the value of a Geometry property using linear interpolation
    /// between two values.  The values are determined by the combination of
    /// From, To, or By values that are set on the animation.
    /// </summary>
    public class GeometryAnimation : GeometryAnimationBase
    {
        #region 字段

        /// <summary>
        /// This is used if the user has specified From, To, and/or By values.
        /// </summary>
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

        /// <summary>
        /// Creates a new GeometryAnimation with all properties set to their default values.
        /// </summary>
        public GeometryAnimation()
            : base()
        {

        }

        /// <summary>
        /// Creates a new GeometryAnimation that will animate a Geometry property from its base value to the value specified by the "toValue" parameter of this constructor.
        /// </summary>
        /// <param name="toValue"></param>
        /// <param name="duration"></param>
        public GeometryAnimation(Geometry toValue, Duration duration)
            : this()
        {
            To = toValue;
            Duration = duration;
        }

        /// <summary>
        /// Creates a new GeometryAnimation that will animate a Geometry property from its base value to the value specified by the "toValue" parameter of this constructor.
        /// </summary>
        /// <param name="toValue"></param>
        /// <param name="duration"></param>
        /// <param name="fillBehavior"></param>
        public GeometryAnimation(Geometry toValue, Duration duration, FillBehavior fillBehavior)
            : this()
        {
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        /// <summary>
        /// Creates a new GeometryAnimation that will animate a Geometry property from the "fromValue" parameter of this constructor to the "toValue" parameter.
        /// </summary>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        /// <param name="duration"></param>
        public GeometryAnimation(Geometry fromValue, Geometry toValue, Duration duration)
            : this()
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
        }

        /// <summary>
        /// Creates a new GeometryAnimation that will animate a Geometry property from the "fromValue" parameter of this constructor to the "toValue" parameter.
        /// </summary>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        /// <param name="duration"></param>
        /// <param name="fillBehavior"></param>
        public GeometryAnimation(Geometry fromValue, Geometry toValue, Duration duration, FillBehavior fillBehavior)
            : this()
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
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

        /// <summary>
        /// Calculates the value this animation believes should be the current value for the property.
        /// </summary>
        /// <param name="defaultOriginValue">
        /// This value is the suggested origin value provided to the animation
        /// to be used if the animation does not have its own concept of a
        /// start value. If this animation is the first in a composition chain
        /// this value will be the snapshot value if one is available or the
        /// base property value if it is not; otherise this value will be the 
        /// value returned by the previous animation in the chain with an 
        /// animationClock that is not Stopped.
        /// </param>
        /// <param name="defaultDestinationValue">
        /// This value is the suggested destination value provided to the animation
        /// to be used if the animation does not have its own concept of an
        /// end value. This value will be the base value if the animation is
        /// in the first composition layer of animations on a property; 
        /// otherwise this value will be the output value from the previous 
        /// composition layer of animations for the property.
        /// </param>
        /// <param name="animationClock">
        /// This is the animationClock which can generate the CurrentTime or
        /// CurrentProgress value to be used by the animation to generate its
        /// output value.
        /// </param>
        /// <returns>The value this animation believes should be the current value for the property.</returns>
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
            // need to validate the default origin and destination values if 
            // the animation uses them as the from, to, or foundation values
            var validateOrigin = false;
            var validateDestination = false;
            switch (_animationType)
            {
                case AnimationType.Automatic:
                    from = defaultOriginValue;
                    to = defaultDestinationValue;
                    foundation = AnimationHelper.GetZeroValueGeometry(from);
                    accumulated = foundation.Clone();
                    validateOrigin = true;
                    validateDestination = true;
                    break;
                case AnimationType.From:
                    from = _keyValues[0];
                    to = defaultDestinationValue;
                    foundation = AnimationHelper.GetZeroValueGeometry(from);
                    accumulated = AnimationHelper.GetZeroValueGeometry(from);
                    validateDestination = true;
                    break;
                case AnimationType.To:
                    from = defaultOriginValue;
                    to = _keyValues[0];
                    foundation = AnimationHelper.GetZeroValueGeometry(from);
                    accumulated = AnimationHelper.GetZeroValueGeometry(from);
                    validateOrigin = true;
                    break;
                case AnimationType.By:
                    // According to the SMIL specification, a By animation is
                    // always additive.  But we don't force this so that a
                    // user can re-use a By animation and have it replace the
                    // animations that precede it in the list without having
                    // to manually set the From value to the base value.
                    foundation = defaultOriginValue;
                    to = _keyValues[0];
                    from = AnimationHelper.GetZeroValueGeometry(foundation);
                    accumulated = AnimationHelper.GetZeroValueGeometry(foundation);
                    validateOrigin = true;
                    break;
                case AnimationType.FromTo:
                    from = _keyValues[0];
                    to = _keyValues[1];
                    if (Additive)
                    {
                        foundation = defaultOriginValue;
                        accumulated = AnimationHelper.GetZeroValueGeometry(foundation);
                        validateOrigin = true;
                    }
                    else
                    {
                        foundation = AnimationHelper.GetZeroValueGeometry(from);
                        accumulated = AnimationHelper.GetZeroValueGeometry(from);
                    }
                    break;
                case AnimationType.FromBy:
                    from = _keyValues[0];
                    to = AnimationHelper.AddGeometry(_keyValues[0], _keyValues[1]);
                    if (Additive)
                    {
                        foundation = defaultOriginValue;
                        accumulated = AnimationHelper.GetZeroValueGeometry(foundation);
                        validateOrigin = true;
                    }
                    else
                    {
                        foundation = AnimationHelper.GetZeroValueGeometry(from);
                        accumulated = AnimationHelper.GetZeroValueGeometry(from);
                    }
                    break;
                default:
                    Debug.Fail("Unknown animation type");
                    foundation = new StreamGeometry();
                    accumulated = new StreamGeometry();
                    from = new StreamGeometry();
                    to = new StreamGeometry();
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

        /// <summary>
        /// Creates a copy of this GeometryAnimation
        /// </summary>
        /// <returns>The copy</returns>
        public new GeometryAnimation Clone()
        {
            return (GeometryAnimation)base.Clone();
        }

        //
        // Note that we don't override the Clone virtuals (CloneCore, CloneCurrentValueCore,
        // GetAsFrozenCore, and GetCurrentValueAsFrozenCore) even though this class has state
        // not stored in a DP.
        // 
        // We don't need to clone _animationType and _keyValues because they are the the cached 
        // results of animation function validation, which can be recomputed.  The other remaining
        // field, isAnimationFunctionValid, defaults to false, which causes this recomputation to happen.
        //

        /// <summary>
        /// Implementation of <see cref="Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        /// <returns>The new Freezable.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new GeometryAnimation();
        }

        #endregion
    }
}
