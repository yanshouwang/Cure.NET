// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// ---------------------------------------------------------------------------

using System;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cure.WPF.Media.Animation
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseGeometryAnimation : AnimationTimeline
    {
        #region 构造

        /// <summary>
        /// Creates a new GeometryAnimationBase.
        /// </summary>
        protected BaseGeometryAnimation()
            : base()
        {

        }

        #endregion

        #region 方法

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
        protected abstract Geometry GetCurrentValue(Geometry defaultOriginValue, Geometry defaultDestinationValue, AnimationClock animationClock);

        #endregion

        #region 重写

        /// <summary>
        /// Creates a copy of this DoubleAnimationBase
        /// </summary>
        /// <returns>The copy</returns>
        public new BaseGeometryAnimation Clone() => (BaseGeometryAnimation)base.Clone();

        /// <summary>
        /// Returns the type of the target property
        /// </summary>
        public sealed override Type TargetPropertyType
        {
            get
            {
                this.ReadPreamble();
                return typeof(Geometry);
            }
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
        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            this.ReadPreamble();
            if (animationClock == null)
            {
                throw new ArgumentNullException("animationClock");
            }
            if (animationClock.CurrentState == ClockState.Stopped)
            {
                return defaultDestinationValue;
            }
            return this.GetCurrentValue((Geometry)defaultOriginValue, (Geometry)defaultDestinationValue, animationClock);
        }

        #endregion
    }
}
