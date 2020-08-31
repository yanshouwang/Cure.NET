using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Cure.WPF.Media.Animation
{
    public class GeometryAnimation : GeometryAnimationBase
    {
        #region 字段
        double[] fromPoints;
        double[] toPoints;
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
                new PropertyMetadata(default, OnGeometryChanged));

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
                new PropertyMetadata(default));

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
                new PropertyMetadata(default));

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
                typeof(GeometryAnimation),
                new PropertyMetadata(default));

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
        private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new GeometryAnimation();
        }

        protected override Geometry GetCurrentValueCore(Geometry defaultOriginValue, Geometry defaultDestinationValue, AnimationClock animationClock)
        {
            Debug.Assert(animationClock.CurrentState != ClockState.Stopped);

            var progress = animationClock.CurrentProgress.Value;
            if (EasingFunction != null)
            {
                progress = EasingFunction.Ease(progress);
            }
            // from + (to - from) * progress
            var fromValue = From.ToValue();
            var toValue = To.ToValue();
            var fillRule = fromValue.FillRule;
            var commands = new List<GeometryCommand>();
            var fromCommands = fromValue.Commands;
            var toCommands = toValue.Commands;
            for (int i = 0; i < fromCommands.Count; i++)
            {
                var fromCommand = fromCommands[i];
                var toCommand = toCommands[i];

                var fromArgs = fromCommand.Args;
                var toArgs = toCommand.Args;
                var args = new List<Point>();
                for (int j = 0; j < fromArgs.Count; j++)
                {
                    var fromArg = fromArgs[j];
                    var toArg = toArgs[j];
                    var arg = fromArg + (toArg - fromArg) * progress;
                    args.Add(arg);
                }

                var command = new GeometryCommand(fromCommand.Command, args);
                commands.Add(command);
            }
            var value = new GeometryValue(fillRule, commands);
            return value.ToGeometry();
        }
    }
}
