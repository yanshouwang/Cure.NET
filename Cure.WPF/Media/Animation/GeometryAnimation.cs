// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// ---------------------------------------------------------------------------

using System;
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
        private Geometry[] _keyValues;
        private AnimationType _animationType;
        private bool _isAnimationFunctionValid;

        #endregion

        #region 属性

        public Geometry From
        {
            get => (Geometry)this.GetValue(FromProperty);
            set => this.SetValue(FromProperty, value);
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
            get => (Geometry)this.GetValue(ToProperty);
            set => this.SetValue(ToProperty, value);
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
            get => (Geometry)this.GetValue(ByProperty);
            set => this.SetValue(ByProperty, value);
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
            get => (IEasingFunction)this.GetValue(EasingFunctionProperty);
            set => this.SetValue(EasingFunctionProperty, value);
        }

        // Using a DependencyProperty as the backing store for EasingFunction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction),
                typeof(IEasingFunction),
                typeof(GeometryAnimation));

        public bool Additive
        {
            get => (bool)this.GetValue(IsAdditiveProperty);
            set => this.SetValue(IsAdditiveProperty, value);
        }

        public bool Cumulative
        {
            get => (bool)this.GetValue(IsCumulativeProperty);
            set => this.SetValue(IsCumulativeProperty, value);
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
            this.To = toValue;
            this.Duration = duration;
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
            this.To = toValue;
            this.Duration = duration;
            this.FillBehavior = fillBehavior;
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
            this.From = fromValue;
            this.To = toValue;
            this.Duration = duration;
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
            this.From = fromValue;
            this.To = toValue;
            this.Duration = duration;
            this.FillBehavior = fillBehavior;
        }

        #endregion

        #region 方法

        private static void OnAnimationFunctionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GeometryAnimation a = (GeometryAnimation)d;
            a._isAnimationFunctionValid = false;
            //a.PropertyChanged(e.Property);
        }

        private static bool ValidateFromToOrByValue(object value)
        {
            Geometry geometry = (Geometry)value;
            if (geometry != null)
            {
                return AnimationUtil.IsValidAnimationValueGeometry(geometry);
            }
            return true;
        }

        private void ValidateAnimationFunction()
        {
            this._animationType = AnimationType.Automatic;
            this._keyValues = null;
            if (this.From != null)
            {
                if (this.To != null)
                {
                    this._animationType = AnimationType.FromTo;
                    this._keyValues = new Geometry[2];
                    this._keyValues[0] = this.From;
                    this._keyValues[1] = this.To;
                }
                else if (this.By != null)
                {
                    this._animationType = AnimationType.FromBy;
                    this._keyValues = new Geometry[2];
                    this._keyValues[0] = this.From;
                    this._keyValues[1] = this.By;
                }
                else
                {
                    this._animationType = AnimationType.From;
                    this._keyValues = new Geometry[1];
                    this._keyValues[0] = this.From;
                }
            }
            else if (this.To != null)
            {
                this._animationType = AnimationType.To;
                this._keyValues = new Geometry[1];
                this._keyValues[0] = this.To;
            }
            else if (this.By != null)
            {
                this._animationType = AnimationType.By;
                this._keyValues = new Geometry[1];
                this._keyValues[0] = this.By;
            }
            this._isAnimationFunctionValid = true;
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
            if (!this._isAnimationFunctionValid)
            {
                this.ValidateAnimationFunction();
            }
            double progress = animationClock.CurrentProgress.Value;
            if (this.EasingFunction != null)
            {
                progress = this.EasingFunction.Ease(progress);
            }
            Geometry from;
            Geometry to;
            Geometry accumulated;
            Geometry foundation;
            // need to validate the default origin and destination values if 
            // the animation uses them as the from, to, or foundation values
            bool validateOrigin = false;
            bool validateDestination = false;
            switch (this._animationType)
            {
                case AnimationType.Automatic:
                    from = defaultOriginValue;
                    to = defaultDestinationValue;
                    foundation = AnimationUtil.GetZeroValueGeometry(from);
                    accumulated = foundation.Clone();
                    validateOrigin = true;
                    validateDestination = true;
                    break;
                case AnimationType.From:
                    from = this._keyValues[0];
                    to = defaultDestinationValue;
                    foundation = AnimationUtil.GetZeroValueGeometry(from);
                    accumulated = AnimationUtil.GetZeroValueGeometry(from);
                    validateDestination = true;
                    break;
                case AnimationType.To:
                    from = defaultOriginValue;
                    to = this._keyValues[0];
                    foundation = AnimationUtil.GetZeroValueGeometry(from);
                    accumulated = AnimationUtil.GetZeroValueGeometry(from);
                    validateOrigin = true;
                    break;
                case AnimationType.By:
                    // According to the SMIL specification, a By animation is
                    // always additive.  But we don't force this so that a
                    // user can re-use a By animation and have it replace the
                    // animations that precede it in the list without having
                    // to manually set the From value to the base value.
                    foundation = defaultOriginValue;
                    to = this._keyValues[0];
                    from = AnimationUtil.GetZeroValueGeometry(foundation);
                    accumulated = AnimationUtil.GetZeroValueGeometry(foundation);
                    validateOrigin = true;
                    break;
                case AnimationType.FromTo:
                    from = this._keyValues[0];
                    to = this._keyValues[1];
                    if (this.Additive)
                    {
                        foundation = defaultOriginValue;
                        accumulated = AnimationUtil.GetZeroValueGeometry(foundation);
                        validateOrigin = true;
                    }
                    else
                    {
                        foundation = AnimationUtil.GetZeroValueGeometry(from);
                        accumulated = AnimationUtil.GetZeroValueGeometry(from);
                    }
                    break;
                case AnimationType.FromBy:
                    from = this._keyValues[0];
                    to = AnimationUtil.AddGeometry(this._keyValues[0], this._keyValues[1]);
                    if (this.Additive)
                    {
                        foundation = defaultOriginValue;
                        accumulated = AnimationUtil.GetZeroValueGeometry(foundation);
                        validateOrigin = true;
                    }
                    else
                    {
                        foundation = AnimationUtil.GetZeroValueGeometry(from);
                        accumulated = AnimationUtil.GetZeroValueGeometry(from);
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
            if (validateOrigin && !AnimationUtil.IsValidAnimationValueGeometry(defaultOriginValue))
            {
                throw new InvalidOperationException();
            }
            if (validateDestination && !AnimationUtil.IsValidAnimationValueGeometry(defaultDestinationValue))
            {
                throw new InvalidOperationException();
            }
            if (this.Cumulative)
            {
                double currentRepeat = (double)(animationClock.CurrentIteration - 1);
                if (currentRepeat > 0.0)
                {
                    Geometry accumulator = AnimationUtil.SubtractGeometry(to, from);
                    accumulated = AnimationUtil.ScaleGeometry(accumulator, currentRepeat);
                }
            }
            // return foundation + accumulated + from + ((to - from) * progress)
            Geometry geometry1 = AnimationUtil.AddGeometry(foundation, accumulated);
            Geometry geometry2 = AnimationUtil.InterpolateGeometry(from, to, progress);
            return AnimationUtil.AddGeometry(geometry1, geometry2);
        }

        #endregion

        #region Freezable

        /// <summary>
        /// Creates a copy of this GeometryAnimation
        /// </summary>
        /// <returns>The copy</returns>
        public new GeometryAnimation Clone() => (GeometryAnimation)base.Clone();

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
        protected override Freezable CreateInstanceCore() => new GeometryAnimation();

        #endregion
    }
}
