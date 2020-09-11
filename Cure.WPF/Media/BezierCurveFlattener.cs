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
using System.Collections.Generic;
using System.Windows;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 用于平展贝塞尔曲线的实用工具类。
    /// </summary>
    internal class BezierCurveFlattener
    {
        public const double STANDARD_FLATTENING_TOLERANCE = 0.25;

        /// <summary>
        /// 平展贝塞尔三次曲线并将所生成的折线添加到第三个参数中。
        /// </summary>
        /// <param name="controlPoints">4 个贝塞尔三次方控制点。</param>
        /// <param name="errorTolerance">真实曲线与平展折线上的两个对应点之间的最大距离。必须绝对为正值。</param>
        /// <param name="resultPolyline">添加平展折线的位置。</param>
        /// <param name="skipFirstPoint">如果要在添加平展折线时跳过第一个控制点，则为 True。
        /// <param name="resultParameters">添加与每个折线顶点关联的贝塞尔曲线参数值的位置。</param>
        /// 如果<paramref name="resultPolyline" /> 为空，则始终添加第一个控制点及其关联参数。</param>
        public static void FlattenCubic(
          Point[] controlPoints,
          double errorTolerance,
          ICollection<Point> resultPolyline,
          bool skipFirstPoint,
          ICollection<double> resultParameters = null)
        {
            if (resultPolyline == null)
                throw new ArgumentNullException(nameof(resultPolyline));
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));
            if (controlPoints.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(controlPoints));
            EnsureErrorTolerance(ref errorTolerance);
            if (!skipFirstPoint)
            {
                resultPolyline.Add(controlPoints[0]);
                resultParameters?.Add(0.0);
            }
            if (IsCubicChordMonotone(controlPoints, errorTolerance * errorTolerance))
            {
                AdaptiveForwardDifferencingCubicFlattener differencingCubicFlattener = new AdaptiveForwardDifferencingCubicFlattener(controlPoints, errorTolerance, errorTolerance, true);
                Point p = new Point();
                double u = 0.0;
                while (differencingCubicFlattener.Next(ref p, ref u))
                {
                    resultPolyline.Add(p);
                    resultParameters?.Add(u);
                }
            }
            else
            {
                double x = controlPoints[3].X - controlPoints[2].X + controlPoints[1].X - controlPoints[0].X;
                double y = controlPoints[3].Y - controlPoints[2].Y + controlPoints[1].Y - controlPoints[0].Y;
                double num = 1.0 / errorTolerance;
                uint depth = Log8UnsignedInt32((uint)(MathUtil.Hypotenuse(x, y) * num + 0.5));
                if (depth > 0U)
                    --depth;
                if (depth > 0U)
                    DoCubicMidpointSubdivision(controlPoints, depth, 0.0, 1.0, 0.75 * num, resultPolyline, resultParameters);
                else
                    DoCubicForwardDifferencing(controlPoints, 0.0, 1.0, 0.75 * num, resultPolyline, resultParameters);
            }
            resultPolyline.Add(controlPoints[3]);
            resultParameters?.Add(1.0);
        }

        /// <summary>
        /// 平展贝塞尔二次曲线并将所生成的折线添加到第三个参数中。使用贝塞尔曲线的升阶以重新使用三次方案例的规则。
        /// </summary>
        /// <param name="controlPoints">3 个贝塞尔二次方控制点。</param>
        /// <param name="errorTolerance">真实曲线与平展折线上的两个对应点之间的最大距离。必须绝对为正值。</param>
        /// <param name="resultPolyline">添加平展折线的位置。</param>
        /// <param name="skipFirstPoint">是否要在添加平展折线时跳过第一个控制点。
        /// <param name="resultParameters">添加与每个折线顶点关联的贝塞尔曲线参数值的位置。</param>
        /// 如果<paramref name="resultPolyline" /> 为空，则始终添加第一个控制点及其关联参数。</param>
        public static void FlattenQuadratic(
          Point[] controlPoints,
          double errorTolerance,
          ICollection<Point> resultPolyline,
          bool skipFirstPoint,
          ICollection<double> resultParameters = null)
        {
            if (resultPolyline == null)
                throw new ArgumentNullException(nameof(resultPolyline));
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));
            if (controlPoints.Length != 3)
                throw new ArgumentOutOfRangeException(nameof(controlPoints));
            EnsureErrorTolerance(ref errorTolerance);
            Point[] controlPoints1 = new Point[4]
            {
                controlPoints[0],
                GeometryUtil.Lerp(controlPoints[0], controlPoints[1], 2.0 / 3.0),
                GeometryUtil.Lerp(controlPoints[1], controlPoints[2], 1.0 / 3.0),
                controlPoints[2]
            };
            FlattenCubic(controlPoints1, errorTolerance, resultPolyline, skipFirstPoint, resultParameters);
        }

        private static void EnsureErrorTolerance(ref double errorTolerance)
        {
            if (errorTolerance > 0.0)
                return;
            errorTolerance = 0.25;
        }

        private static uint Log8UnsignedInt32(uint i)
        {
            uint num = 0;
            while (i > 0U)
            {
                i >>= 3;
                ++num;
            }
            return num;
        }

        private static uint Log4UnsignedInt32(uint i)
        {
            uint num = 0;
            while (i > 0U)
            {
                i >>= 2;
                ++num;
            }
            return num;
        }

        private static uint Log4Double(double d)
        {
            uint num = 0;
            while (d > 1.0)
            {
                d *= 0.25;
                ++num;
            }
            return num;
        }

        private static void DoCubicMidpointSubdivision(
          Point[] controlPoints,
          uint depth,
          double leftParameter,
          double rightParameter,
          double inverseErrorTolerance,
          ICollection<Point> resultPolyline,
          ICollection<double> resultParameters)
        {
            Point[] controlPoints1 = new Point[4]
            {
                controlPoints[0],
                controlPoints[1],
                controlPoints[2],
                controlPoints[3]
            };
            Point[] controlPoints2 = new Point[4];
            controlPoints2[3] = controlPoints1[3];
            controlPoints1[3] = GeometryUtil.Midpoint(controlPoints1[3], controlPoints1[2]);
            controlPoints1[2] = GeometryUtil.Midpoint(controlPoints1[2], controlPoints1[1]);
            controlPoints1[1] = GeometryUtil.Midpoint(controlPoints1[1], controlPoints1[0]);
            controlPoints2[2] = controlPoints1[3];
            controlPoints1[3] = GeometryUtil.Midpoint(controlPoints1[3], controlPoints1[2]);
            controlPoints1[2] = GeometryUtil.Midpoint(controlPoints1[2], controlPoints1[1]);
            controlPoints2[1] = controlPoints1[3];
            controlPoints1[3] = GeometryUtil.Midpoint(controlPoints1[3], controlPoints1[2]);
            controlPoints2[0] = controlPoints1[3];
            --depth;
            double num = (leftParameter + rightParameter) * 0.5;
            if (depth > 0U)
            {
                DoCubicMidpointSubdivision(controlPoints1, depth, leftParameter, num, inverseErrorTolerance, resultPolyline, resultParameters);
                resultPolyline.Add(controlPoints2[0]);
                resultParameters?.Add(num);
                DoCubicMidpointSubdivision(controlPoints2, depth, num, rightParameter, inverseErrorTolerance, resultPolyline, resultParameters);
            }
            else
            {
                DoCubicForwardDifferencing(controlPoints1, leftParameter, num, inverseErrorTolerance, resultPolyline, resultParameters);
                resultPolyline.Add(controlPoints2[0]);
                resultParameters?.Add(num);
                DoCubicForwardDifferencing(controlPoints2, num, rightParameter, inverseErrorTolerance, resultPolyline, resultParameters);
            }
        }

        private static void DoCubicForwardDifferencing(
          Point[] controlPoints,
          double leftParameter,
          double rightParameter,
          double inverseErrorTolerance,
          ICollection<Point> resultPolyline,
          ICollection<double> resultParameters)
        {
            double num1 = controlPoints[1].X - controlPoints[0].X;
            double num2 = controlPoints[1].Y - controlPoints[0].Y;
            double num3 = controlPoints[2].X - controlPoints[1].X;
            double num4 = controlPoints[2].Y - controlPoints[1].Y;
            double num5 = controlPoints[3].X - controlPoints[2].X;
            double num6 = controlPoints[3].Y - controlPoints[2].Y;
            double num7 = num3 - num1;
            double num8 = num4 - num2;
            double num9 = num5 - num3;
            double num10 = num6 - num4;
            double num11 = num9 - num7;
            double num12 = num10 - num8;
            Vector vector = controlPoints[3].Subtract(controlPoints[0]);
            double length = vector.Length;
            double num13 = MathUtil.IsVerySmall(length) ? Math.Max(0.0, Math.Max(GeometryUtil.Distance(controlPoints[1], controlPoints[0]), GeometryUtil.Distance(controlPoints[2], controlPoints[0]))) : Math.Max(0.0, Math.Max(Math.Abs((num7 * vector.Y - num8 * vector.X) / length), Math.Abs((num9 * vector.Y - num10 * vector.X) / length)));
            uint num14 = 0U;
            if (num13 > 0.0)
            {
                double d = num13 * inverseErrorTolerance;
                num14 = d < int.MaxValue ? Log4UnsignedInt32((uint)(d + 0.5)) : Log4Double(d);
            }
            int exp1 = -(int)num14;
            int exp2 = exp1 + exp1;
            int exp3 = exp2 + exp1;
            double num15 = MathUtil.DoubleFromMantissaAndExponent(3.0 * num7, exp2);
            double num16 = MathUtil.DoubleFromMantissaAndExponent(3.0 * num8, exp2);
            double num17 = MathUtil.DoubleFromMantissaAndExponent(6.0 * num11, exp3);
            double num18 = MathUtil.DoubleFromMantissaAndExponent(6.0 * num12, exp3);
            double num19 = MathUtil.DoubleFromMantissaAndExponent(3.0 * num1, exp1) + num15 + 1.0 / 6.0 * num17;
            double num20 = MathUtil.DoubleFromMantissaAndExponent(3.0 * num2, exp1) + num16 + 1.0 / 6.0 * num18;
            double num21 = 2.0 * num15 + num17;
            double num22 = 2.0 * num16 + num18;
            double x = controlPoints[0].X;
            double y = controlPoints[0].Y;
            Point point = new Point(0.0, 0.0);
            int num23 = 1 << (int)num14;
            double num24 = num23 > 0 ? (rightParameter - leftParameter) / num23 : 0.0;
            double num25 = leftParameter;
            for (int index = 1; index < num23; ++index)
            {
                x += num19;
                y += num20;
                point.X = x;
                point.Y = y;
                resultPolyline.Add(point);
                num25 += num24;
                resultParameters?.Add(num25);
                num19 += num21;
                num20 += num22;
                num21 += num17;
                num22 += num18;
            }
        }

        private static bool IsCubicChordMonotone(Point[] controlPoints, double squaredTolerance)
        {
            double num1 = GeometryUtil.SquaredDistance(controlPoints[0], controlPoints[3]);
            if (num1 <= squaredTolerance)
                return false;
            Vector lhs = controlPoints[3].Subtract(controlPoints[0]);
            Vector rhs1 = controlPoints[1].Subtract(controlPoints[0]);
            double num2 = GeometryUtil.Dot(lhs, rhs1);
            if (num2 < 0.0 || num2 > num1)
                return false;
            Vector rhs2 = controlPoints[2].Subtract(controlPoints[0]);
            double num3 = GeometryUtil.Dot(lhs, rhs2);
            return num3 >= 0.0 && num3 <= num1 && num2 <= num3;
        }

        private class AdaptiveForwardDifferencingCubicFlattener
        {
            private double _aX;
            private double _aY;
            private double _bX;
            private double _bY;
            private double _cX;
            private double _cY;
            private double _dX;
            private double _dY;
            private int _numSteps = 1;
            private readonly double _flatnessTolerance;
            private readonly double _distanceTolerance;
            private readonly bool _doParameters;
            private double _parameter;
            private double _dParameter = 1.0;

            internal AdaptiveForwardDifferencingCubicFlattener(
              Point[] controlPoints,
              double flatnessTolerance,
              double distanceTolerance,
              bool doParameters)
            {
                this._flatnessTolerance = 3.0 * flatnessTolerance;
                this._distanceTolerance = distanceTolerance;
                this._doParameters = doParameters;
                this._aX = -controlPoints[0].X + 3.0 * (controlPoints[1].X - controlPoints[2].X) + controlPoints[3].X;
                this._aY = -controlPoints[0].Y + 3.0 * (controlPoints[1].Y - controlPoints[2].Y) + controlPoints[3].Y;
                this._bX = 3.0 * (controlPoints[0].X - 2.0 * controlPoints[1].X + controlPoints[2].X);
                this._bY = 3.0 * (controlPoints[0].Y - 2.0 * controlPoints[1].Y + controlPoints[2].Y);
                this._cX = 3.0 * (-controlPoints[0].X + controlPoints[1].X);
                this._cY = 3.0 * (-controlPoints[0].Y + controlPoints[1].Y);
                this._dX = controlPoints[0].X;
                this._dY = controlPoints[0].Y;
            }

            private AdaptiveForwardDifferencingCubicFlattener()
            {
            }

            internal bool Next(ref Point p, ref double u)
            {
                while (this.MustSubdivide(this._flatnessTolerance))
                    this.HalveStepSize();
                if ((this._numSteps & 1) == 0)
                {
                    while (this._numSteps > 1 && !this.MustSubdivide(this._flatnessTolerance * 0.25))
                        this.DoubleStepSize();
                }
                this.IncrementDifferencesAndParameter();
                p.X = this._dX;
                p.Y = this._dY;
                u = this._parameter;
                return this._numSteps != 0;
            }

            private void DoubleStepSize()
            {
                this._aX *= 8.0;
                this._aY *= 8.0;
                this._bX *= 4.0;
                this._bY *= 4.0;
                this._cX += this._cX;
                this._cY += this._cY;
                if (this._doParameters)
                    this._dParameter *= 2.0;
                this._numSteps >>= 1;
            }

            private void HalveStepSize()
            {
                this._aX *= 0.125;
                this._aY *= 0.125;
                this._bX *= 0.25;
                this._bY *= 0.25;
                this._cX *= 0.5;
                this._cY *= 0.5;
                if (this._doParameters)
                    this._dParameter *= 0.5;
                this._numSteps <<= 1;
            }

            private void IncrementDifferencesAndParameter()
            {
                this._dX = this._aX + this._bX + this._cX + this._dX;
                this._dY = this._aY + this._bY + this._cY + this._dY;
                this._cX = this._aX + this._aX + this._aX + this._bX + this._bX + this._cX;
                this._cY = this._aY + this._aY + this._aY + this._bY + this._bY + this._cY;
                this._bX = this._aX + this._aX + this._aX + this._bX;
                this._bY = this._aY + this._aY + this._aY + this._bY;
                --this._numSteps;
                this._parameter += this._dParameter;
            }

            private bool MustSubdivide(double flatnessTolerance)
            {
                double num1 = -(this._aY + this._bY + this._cY);
                double num2 = this._aX + this._bX + this._cX;
                double num3 = Math.Abs(num1) + Math.Abs(num2);
                if (num3 <= this._distanceTolerance)
                    return false;
                double num4 = num3 * flatnessTolerance;
                return Math.Abs(this._cX * num1 + this._cY * num2) > num4 || Math.Abs((this._bX + this._cX + this._cX) * num1 + (this._bY + this._cY + this._cY) * num2) > num4;
            }
        }
    }
}