using System;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cure.WPF.Media.Animation
{
    public abstract class GeometryAnimationBase : AnimationTimeline
    {
        #region 构造函数
        protected GeometryAnimationBase()
            : base()
        {

        }
        #endregion

        #region Freezable
        public new GeometryAnimationBase Clone()
        {
            return (GeometryAnimationBase)base.Clone();
        }
        #endregion

        #region IAnimation
        public override sealed Type TargetPropertyType
        {
            get
            {
                ReadPreamble();
                return typeof(Geometry);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            return GetCurrentValue((Geometry)defaultOriginValue, (Geometry)defaultDestinationValue, animationClock);
        }

        public Geometry GetCurrentValue(Geometry defaultOriginValue, Geometry defaultDestinationValue, AnimationClock animationClock)
        {
            ReadPreamble();
            if (animationClock == null)
            {
                throw new ArgumentNullException("animationClock");
            }
            if (animationClock.CurrentState == ClockState.Stopped)
            {
                return defaultDestinationValue;
            }
            return GetCurrentValueCore(defaultOriginValue, defaultDestinationValue, animationClock);
        }

        protected abstract Geometry GetCurrentValueCore(Geometry defaultOriginValue, Geometry defaultDestinationValue, AnimationClock animationClock);
        #endregion
    }
}
