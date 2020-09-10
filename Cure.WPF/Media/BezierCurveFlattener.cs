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
    class BezierCurveFlattener
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
                var differencingCubicFlattener = new AdaptiveForwardDifferencingCubicFlattener(controlPoints, errorTolerance, errorTolerance, true);
                var p = new Point();
                var u = 0.0;
                while (differencingCubicFlattener.Next(ref p, ref u))
                {
                    resultPolyline.Add(p);
                    resultParameters?.Add(u);
                }
            }
            else
            {
                var x = controlPoints[3].X - controlPoints[2].X + controlPoints[1].X - controlPoints[0].X;
                var y = controlPoints[3].Y - controlPoints[2].Y + controlPoints[1].Y - controlPoints[0].Y;
                var num = 1.0 / errorTolerance;
                var depth = Log8UnsignedInt32((uint)(MathUtil.Hypotenuse(x, y) * num + 0.5));
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
            var controlPoints1 = new Point[4]
            {
                controlPoints[0],
                GeometryUtil.Lerp(controlPoints[0], controlPoints[1], 2.0 / 3.0),
                GeometryUtil.Lerp(controlPoints[1], controlPoints[2], 1.0 / 3.0),
                controlPoints[2]
            };
            FlattenCubic(controlPoints1, errorTolerance, resultPolyline, skipFirstPoint, resultParameters);
        }

        static void EnsureErrorTolerance(ref double errorTolerance)
        {
            if (errorTolerance > 0.0)
                return;
            errorTolerance = 0.25;
        }

        static uint Log8UnsignedInt32(uint i)
        {
            uint num = 0;
            while (i > 0U)
            {
                i >>= 3;
                ++num;
            }
            return num;
        }

        static uint Log4UnsignedInt32(uint i)
        {
            uint num = 0;
            while (i > 0U)
            {
                i >>= 2;
                ++num;
            }
            return num;
        }

        static uint Log4Double(double d)
        {
            uint num = 0;
            while (d > 1.0)
            {
                d *= 0.25;
                ++num;
            }
            return num;
        }

        static void DoCubicMidpointSubdivision(
          Point[] controlPoints,
          uint depth,
          double leftParameter,
          double rightParameter,
          double inverseErrorTolerance,
          ICollection<Point> resultPolyline,
          ICollection<double> resultParameters)
        {
            var controlPoints1 = new Point[4]
            {
                controlPoints[0],
                controlPoints[1],
                controlPoints[2],
                controlPoints[3]
            };
            var controlPoints2 = new Point[4];
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
            var num = (leftParameter + rightParameter) * 0.5;
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

        static void DoCubicForwardDifferencing(
          Point[] controlPoints,
          double leftParameter,
          double rightParameter,
          double inverseErrorTolerance,
          ICollection<Point> resultPolyline,
          ICollection<double> resultParameters)
        {
            var num1 = controlPoints[1].X - controlPoints[0].X;
            var num2 = controlPoints[1].Y - controlPoints[0].Y;
            var num3 = controlPoints[2].X - controlPoints[1].X;
            var num4 = controlPoints[2].Y - controlPoints[1].Y;
            var num5 = controlPoints[3].X - controlPoints[2].X;
            var num6 = controlPoints[3].Y - controlPoints[2].Y;
            var num7 = num3 - num1;
            var num8 = num4 - num2;
            var num9 = num5 - num3;
            var num10 = num6 - num4;
            var num11 = num9 - num7;
            var num12 = num10 - num8;
            var vector = controlPoints[3].Subtract(controlPoints[0]);
            var length = vector.Length;
            var num13 = MathUtil.IsVerySmall(length) ? Math.Max(0.0, Math.Max(GeometryUtil.Distance(controlPoints[1], controlPoints[0]), GeometryUtil.Distance(controlPoints[2], controlPoints[0]))) : Math.Max(0.0, Math.Max(Math.Abs((num7 * vector.Y - num8 * vector.X) / length), Math.Abs((num9 * vector.Y - num10 * vector.X) / length)));
            var num14 = 0U;
            if (num13 > 0.0)
            {
                double d = num13 * inverseErrorTolerance;
                num14 = d < int.MaxValue ? Log4UnsignedInt32((uint)(d + 0.5)) : Log4Double(d);
            }
            var exp1 = -(int)num14;
            var exp2 = exp1 + exp1;
            var exp3 = exp2 + exp1;
            var num15 = MathUtil.DoubleFromMantissaAndExponent(3.0 * num7, exp2);
            var num16 = MathUtil.DoubleFromMantissaAndExponent(3.0 * num8, exp2);
            var num17 = MathUtil.DoubleFromMantissaAndExponent(6.0 * num11, exp3);
            var num18 = MathUtil.DoubleFromMantissaAndExponent(6.0 * num12, exp3);
            var num19 = MathUtil.DoubleFromMantissaAndExponent(3.0 * num1, exp1) + num15 + 1.0 / 6.0 * num17;
            var num20 = MathUtil.DoubleFromMantissaAndExponent(3.0 * num2, exp1) + num16 + 1.0 / 6.0 * num18;
            var num21 = 2.0 * num15 + num17;
            var num22 = 2.0 * num16 + num18;
            var x = controlPoints[0].X;
            var y = controlPoints[0].Y;
            var point = new Point(0.0, 0.0);
            var num23 = 1 << (int)num14;
            var num24 = num23 > 0 ? (rightParameter - leftParameter) / num23 : 0.0;
            var num25 = leftParameter;
            for (var index = 1; index < num23; ++index)
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

        static bool IsCubicChordMonotone(Point[] controlPoints, double squaredTolerance)
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

        class AdaptiveForwardDifferencingCubicFlattener
        {
            double _aX;
            double _aY;
            double _bX;
            double _bY;
            double _cX;
            double _cY;
            double _dX;
            double _dY;
            int _numSteps = 1;
            readonly double _flatnessTolerance;
            readonly double _distanceTolerance;
            readonly bool _doParameters;
            double _parameter;
            double _dParameter = 1.0;

            internal AdaptiveForwardDifferencingCubicFlattener(
              Point[] controlPoints,
              double flatnessTolerance,
              double distanceTolerance,
              bool doParameters)
            {
                _flatnessTolerance = 3.0 * flatnessTolerance;
                _distanceTolerance = distanceTolerance;
                _doParameters = doParameters;
                _aX = -controlPoints[0].X + 3.0 * (controlPoints[1].X - controlPoints[2].X) + controlPoints[3].X;
                _aY = -controlPoints[0].Y + 3.0 * (controlPoints[1].Y - controlPoints[2].Y) + controlPoints[3].Y;
                _bX = 3.0 * (controlPoints[0].X - 2.0 * controlPoints[1].X + controlPoints[2].X);
                _bY = 3.0 * (controlPoints[0].Y - 2.0 * controlPoints[1].Y + controlPoints[2].Y);
                _cX = 3.0 * (-controlPoints[0].X + controlPoints[1].X);
                _cY = 3.0 * (-controlPoints[0].Y + controlPoints[1].Y);
                _dX = controlPoints[0].X;
                _dY = controlPoints[0].Y;
            }

            AdaptiveForwardDifferencingCubicFlattener()
            {
            }

            internal bool Next(ref Point p, ref double u)
            {
                while (MustSubdivide(_flatnessTolerance))
                    HalveStepSize();
                if ((_numSteps & 1) == 0)
                {
                    while (_numSteps > 1 && !MustSubdivide(_flatnessTolerance * 0.25))
                        DoubleStepSize();
                }
                IncrementDifferencesAndParameter();
                p.X = _dX;
                p.Y = _dY;
                u = _parameter;
                return _numSteps != 0;
            }

            void DoubleStepSize()
            {
                _aX *= 8.0;
                _aY *= 8.0;
                _bX *= 4.0;
                _bY *= 4.0;
                _cX += _cX;
                _cY += _cY;
                if (_doParameters)
                    _dParameter *= 2.0;
                _numSteps >>= 1;
            }

            void HalveStepSize()
            {
                _aX *= 0.125;
                _aY *= 0.125;
                _bX *= 0.25;
                _bY *= 0.25;
                _cX *= 0.5;
                _cY *= 0.5;
                if (_doParameters)
                    _dParameter *= 0.5;
                _numSteps <<= 1;
            }

            void IncrementDifferencesAndParameter()
            {
                _dX = _aX + _bX + _cX + _dX;
                _dY = _aY + _bY + _cY + _dY;
                _cX = _aX + _aX + _aX + _bX + _bX + _cX;
                _cY = _aY + _aY + _aY + _bY + _bY + _cY;
                _bX = _aX + _aX + _aX + _bX;
                _bY = _aY + _aY + _aY + _bY;
                --_numSteps;
                _parameter += _dParameter;
            }

            bool MustSubdivide(double flatnessTolerance)
            {
                var num1 = -(_aY + _bY + _cY);
                var num2 = _aX + _bX + _cX;
                var num3 = Math.Abs(num1) + Math.Abs(num2);
                if (num3 <= _distanceTolerance)
                    return false;
                var num4 = num3 * flatnessTolerance;
                return Math.Abs(_cX * num1 + _cY * num2) > num4 || Math.Abs((_bX + _cX + _cX) * num1 + (_bY + _cY + _cY) * num2) > num4;
            }
        }
    }
}