// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Cure.WPF.Media.Animation
{
    internal static class AnimationUtil
    {
        #region 计算插值

        public static byte InterpolateByte(byte from, byte to, double progress)
            => (byte)(from + (int)((to - from + (double)0.5) * progress));

        public static Color InterpolateColor(Color from, Color to, double progress)
            => from + ((to - from) * (float)progress);

        public static decimal InterpolateDecimal(decimal from, decimal to, double progress)
            => from + ((to - from) * (decimal)progress);

        public static double InterpolateDouble(double from, double to, double progress)
            => from + ((to - from) * progress);

        public static short InterpolateInt16(short from, short to, double progress)
        {
            if (progress == 0.0)
            {
                return from;
            }
            else if (progress == 1.0)
            {
                return to;
            }
            else
            {
                double addend = to - from;
                addend *= progress;
                addend += (addend > 0.0) ? 0.5 : -0.5;
                return (short)(from + (short)addend);
            }
        }

        public static int InterpolateInt32(int from, int to, double progress)
        {
            if (progress == 0.0)
            {
                return from;
            }
            else if (progress == 1.0)
            {
                return to;
            }
            else
            {
                double addend = to - from;
                addend *= progress;
                addend += (addend > 0.0) ? 0.5 : -0.5;
                return from + (int)addend;
            }
        }

        public static long InterpolateInt64(long from, long to, double progress)
        {
            if (progress == 0.0)
            {
                return from;
            }
            else if (progress == 1.0)
            {
                return to;
            }
            else
            {
                double addend = to - from;
                addend *= progress;
                addend += (addend > 0.0) ? 0.5 : -0.5;
                return from + (long)addend;
            }
        }

        public static Point InterpolatePoint(Point from, Point to, double progress)
            => from + ((to - from) * progress);

        public static Point3D InterpolatePoint3D(Point3D from, Point3D to, double progress)
            => from + ((to - from) * progress);

        public static Quaternion InterpolateQuaternion(Quaternion from, Quaternion to, double progress, bool useShortestPath)
            => Quaternion.Slerp(from, to, progress, useShortestPath);

        public static Rect InterpolateRect(Rect from, Rect to, double progress)
        {
            Rect temp = new Rect
            {
                // from + ((from - to) * progress)
                Location = new Point(
                from.Location.X + ((to.Location.X - from.Location.X) * progress),
                from.Location.Y + ((to.Location.Y - from.Location.Y) * progress)),
                Size = new Size(
                from.Size.Width + ((to.Size.Width - from.Size.Width) * progress),
                from.Size.Height + ((to.Size.Height - from.Size.Height) * progress))
            };
            return temp;
        }

        public static Rotation3D InterpolateRotation3D(Rotation3D from, Rotation3D to, double progress)
        {
            QuaternionRotation3D r1 = (QuaternionRotation3D)from;
            QuaternionRotation3D r2 = (QuaternionRotation3D)to;
            Quaternion quaternion = InterpolateQuaternion(r1.Quaternion, r2.Quaternion, progress, /* useShortestPath = */ true);
            return new QuaternionRotation3D(quaternion);
        }

        public static float InterpolateSingle(float from, float to, double progress) => from + (float)((to - from) * progress);

        public static Size InterpolateSize(Size from, Size to, double progress) => (Size)InterpolateVector((Vector)from, (Vector)to, progress);

        public static Vector InterpolateVector(Vector from, Vector to, double progress) => from + ((to - from) * progress);

        public static Vector3D InterpolateVector3D(Vector3D from, Vector3D to, double progress) => from + ((to - from) * progress);

        public static Geometry InterpolateGeometry(Geometry from, Geometry to, double progress)
        {
            PathGeometry fromGeometry = PathGeometry.CreateFromGeometry(from);
            PathGeometry toGeometry = PathGeometry.CreateFromGeometry(to);
            List<PathFigure> figures = new List<PathFigure>();
            for (int i = 0; i < fromGeometry.Figures.Count; i++)
            {
                PathFigure fromFigure = fromGeometry.Figures[i];
                PathFigure toFigure = toGeometry.Figures[i];
                Point start = InterpolatePoint(fromFigure.StartPoint, toFigure.StartPoint, progress);
                List<PathSegment> segments = new List<PathSegment>();
                for (int j = 0; j < fromFigure.Segments.Count; j++)
                {
                    PathSegment fromSegment = fromFigure.Segments[j];
                    PathSegment toSegment = toFigure.Segments[j];
                    PathSegment segment;
                    if (fromSegment is LineSegment fromLine && toSegment is LineSegment toLine)
                    {
                        Point point = InterpolatePoint(fromLine.Point, toLine.Point, progress);
                        segment = new LineSegment(point, fromLine.IsStroked);
                    }
                    else if (fromSegment is ArcSegment fromArc && toSegment is ArcSegment toArc)
                    {
                        Point point = InterpolatePoint(fromArc.Point, toArc.Point, progress);
                        Size size = InterpolateSize(fromArc.Size, toArc.Size, progress);
                        double rotationAngle = InterpolateDouble(fromArc.RotationAngle, toArc.RotationAngle, progress);
                        segment = new ArcSegment(point, size, rotationAngle, fromArc.IsLargeArc, fromArc.SweepDirection, fromArc.IsStroked);
                    }
                    else if (fromSegment is BezierSegment fromBezier && toSegment is BezierSegment toBezier)
                    {
                        Point point1 = InterpolatePoint(fromBezier.Point1, toBezier.Point1, progress);
                        Point point2 = InterpolatePoint(fromBezier.Point2, toBezier.Point2, progress);
                        Point point3 = InterpolatePoint(fromBezier.Point3, toBezier.Point3, progress);
                        segment = new BezierSegment(point1, point2, point3, fromBezier.IsStroked);
                    }
                    else if (fromSegment is QuadraticBezierSegment fromQuadraticBezier && toSegment is QuadraticBezierSegment toQuadraticBezier)
                    {
                        Point point1 = InterpolatePoint(fromQuadraticBezier.Point1, toQuadraticBezier.Point1, progress);
                        Point point2 = InterpolatePoint(fromQuadraticBezier.Point2, toQuadraticBezier.Point2, progress);
                        segment = new QuadraticBezierSegment(point1, point2, fromQuadraticBezier.IsStroked);
                    }
                    else if (fromSegment is PolyLineSegment fromPolyLine && toSegment is PolyLineSegment toPolyLine)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < fromPolyLine.Points.Count; k++)
                        {
                            Point fromPoint = fromPolyLine.Points[k];
                            Point toPoint = toPolyLine.Points[k];
                            Point point = InterpolatePoint(fromPoint, toPoint, progress);
                            points.Add(point);
                        }
                        segment = new PolyLineSegment(points, fromPolyLine.IsStroked);
                    }
                    else if (fromSegment is PolyBezierSegment fromPolyBezier && toSegment is PolyBezierSegment toPolyBezier)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < fromPolyBezier.Points.Count; k++)
                        {
                            Point fromPoint = fromPolyBezier.Points[k];
                            Point toPoint = toPolyBezier.Points[k];
                            Point point = InterpolatePoint(fromPoint, toPoint, progress);
                            points.Add(point);
                        }
                        segment = new PolyBezierSegment(points, fromPolyBezier.IsStroked);
                    }
                    else if (fromSegment is PolyQuadraticBezierSegment fromPolyQuadraticBezier && toSegment is PolyQuadraticBezierSegment toPolyQuadraticBezier)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < fromPolyQuadraticBezier.Points.Count; k++)
                        {
                            Point fromPoint = fromPolyQuadraticBezier.Points[k];
                            Point toPoint = toPolyQuadraticBezier.Points[k];
                            Point point = InterpolatePoint(fromPoint, toPoint, progress);
                            points.Add(point);
                        }
                        segment = new PolyQuadraticBezierSegment(points, fromPolyQuadraticBezier.IsStroked);
                    }
                    else
                    {
                        throw new NotImplementedException("暂不支持此转换");
                    }
                    segments.Add(segment);
                }
                PathFigure figure = new PathFigure(start, segments, fromFigure.IsClosed);
                figures.Add(figure);
            }
            return new PathGeometry(figures, fromGeometry.FillRule, fromGeometry.Transform);
        }

        #endregion

        #region 加法

        public static byte AddByte(byte value1, byte value2)
            => (byte)(value1 + value2);

        public static Color AddColor(Color value1, Color value2)
            => value1 + value2;

        public static decimal AddDecimal(decimal value1, decimal value2)
            => value1 + value2;

        public static double AddDouble(double value1, double value2)
            => value1 + value2;

        public static short AddInt16(short value1, short value2)
            => (short)(value1 + value2);

        public static int AddInt32(int value1, int value2)
            => value1 + value2;

        public static long AddInt64(long value1, long value2)
            => value1 + value2;

        public static Point AddPoint(Point value1, Point value2)
        {
            double x = value1.X + value2.X;
            double y = value1.Y + value2.Y;
            return new Point(x, y);
        }

        public static Point3D AddPoint3D(Point3D value1, Point3D value2)
        {
            double x = value1.X + value2.X;
            double y = value1.Y + value2.Y;
            double z = value1.Z + value2.Z;
            return new Point3D(x, y, z);
        }

        public static Quaternion AddQuaternion(Quaternion value1, Quaternion value2)
            => value1 * value2;

        public static float AddSingle(float value1, float value2)
            => value1 + value2;

        public static Size AddSize(Size value1, Size value2)
        {
            double width = value1.Width + value2.Width;
            double height = value1.Height + value2.Height;
            return new Size(width, height);
        }

        public static Vector AddVector(Vector value1, Vector value2)
            => value1 + value2;

        public static Vector3D AddVector3D(Vector3D value1, Vector3D value2)
            => value1 + value2;

        public static Rect AddRect(Rect value1, Rect value2)
        {
            Point location = AddPoint(value1.Location, value2.Location);
            Size size = AddSize(value1.Size, value2.Size);
            return new Rect(location, size);
        }

        public static Rotation3D AddRotation3D(Rotation3D value1, Rotation3D value2)
        {
            if (value1 == null)
            {
                value1 = Rotation3D.Identity;
            }
            if (value2 == null)
            {
                value2 = Rotation3D.Identity;
            }
            QuaternionRotation3D r1 = (QuaternionRotation3D)value1;
            QuaternionRotation3D r2 = (QuaternionRotation3D)value2;
            Quaternion quaternion = AddQuaternion(r1.Quaternion, r2.Quaternion);
            return new QuaternionRotation3D(quaternion);
        }

        public static Geometry AddGeometry(Geometry value1, Geometry value2)
        {
            PathGeometry geometry1 = PathGeometry.CreateFromGeometry(value1);
            PathGeometry geometry2 = PathGeometry.CreateFromGeometry(value2);
            List<PathFigure> figures = new List<PathFigure>();
            for (int i = 0; i < geometry1.Figures.Count; i++)
            {
                PathFigure figure1 = geometry1.Figures[i];
                PathFigure figure2 = geometry2.Figures[i];
                Point start = AddPoint(figure1.StartPoint, figure2.StartPoint);
                List<PathSegment> segments = new List<PathSegment>();
                for (int j = 0; j < figure1.Segments.Count; j++)
                {
                    PathSegment segment1 = figure1.Segments[j];
                    PathSegment segment2 = figure2.Segments[j];
                    PathSegment segment;
                    if (segment1 is LineSegment line1 && segment2 is LineSegment line2)
                    {
                        Point point = AddPoint(line1.Point, line2.Point);
                        segment = new LineSegment(point, line1.IsStroked);
                    }
                    else if (segment1 is ArcSegment arc1 && segment2 is ArcSegment arc2)
                    {
                        Point point = AddPoint(arc1.Point, arc2.Point);
                        Size size = AddSize(arc1.Size, arc2.Size);
                        double rotationAngle = AddDouble(arc1.RotationAngle, arc2.RotationAngle);
                        segment = new ArcSegment(point, size, rotationAngle, arc1.IsLargeArc, arc1.SweepDirection, arc1.IsStroked);
                    }
                    else if (segment1 is BezierSegment bezier1 && segment2 is BezierSegment bezier2)
                    {
                        Point point1 = AddPoint(bezier1.Point1, bezier2.Point1);
                        Point point2 = AddPoint(bezier1.Point2, bezier2.Point2);
                        Point point3 = AddPoint(bezier1.Point3, bezier2.Point3);
                        segment = new BezierSegment(point1, point2, point3, bezier1.IsStroked);
                    }
                    else if (segment1 is QuadraticBezierSegment quadraticBezier1 && segment2 is QuadraticBezierSegment quadraticBezier2)
                    {
                        Point point1 = AddPoint(quadraticBezier1.Point1, quadraticBezier2.Point1);
                        Point point2 = AddPoint(quadraticBezier1.Point2, quadraticBezier2.Point2);
                        segment = new QuadraticBezierSegment(point1, point2, quadraticBezier1.IsStroked);
                    }
                    else if (segment1 is PolyLineSegment polyLine1 && segment2 is PolyLineSegment polyLine2)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < polyLine1.Points.Count; k++)
                        {
                            Point point1 = polyLine1.Points[k];
                            Point point2 = polyLine2.Points[k];
                            Point point = AddPoint(point1, point2);
                            points.Add(point);
                        }
                        segment = new PolyLineSegment(points, polyLine1.IsStroked);
                    }
                    else if (segment1 is PolyBezierSegment polyBezier1 && segment2 is PolyBezierSegment polyBezier2)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < polyBezier1.Points.Count; k++)
                        {
                            Point point1 = polyBezier1.Points[k];
                            Point point2 = polyBezier2.Points[k];
                            Point point = AddPoint(point1, point2);
                            points.Add(point);
                        }
                        segment = new PolyBezierSegment(points, polyBezier1.IsStroked);
                    }
                    else if (segment1 is PolyQuadraticBezierSegment polyQuadraticBezier1 && segment2 is PolyQuadraticBezierSegment polyQuadraticBezier2)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < polyQuadraticBezier1.Points.Count; k++)
                        {
                            Point point1 = polyQuadraticBezier1.Points[k];
                            Point point2 = polyQuadraticBezier2.Points[k];
                            Point point = AddPoint(point1, point2);
                            points.Add(point);
                        }
                        segment = new PolyQuadraticBezierSegment(points, polyQuadraticBezier1.IsStroked);
                    }
                    else
                    {
                        throw new NotImplementedException("暂不支持此转换");
                    }
                    segments.Add(segment);
                }
                PathFigure figure = new PathFigure(start, segments, figure1.IsClosed);
                figures.Add(figure);
            }
            return new PathGeometry(figures, geometry1.FillRule, geometry1.Transform);
        }

        #endregion

        #region 减法

        public static byte SubtractByte(byte value1, byte value2)
            => (byte)(value1 - value2);

        public static Color SubtractColor(Color value1, Color value2)
            => value1 - value2;

        public static decimal SubtractDecimal(decimal value1, decimal value2)
            => value1 - value2;

        public static double SubtractDouble(double value1, double value2)
            => value1 - value2;

        public static short SubtractInt16(short value1, short value2)
            => (short)(value1 - value2);

        public static int SubtractInt32(int value1, int value2)
            => value1 - value2;

        public static long SubtractInt64(long value1, long value2)
            => value1 - value2;

        public static Point SubtractPoint(Point value1, Point value2)
        {
            double x = value1.X - value2.X;
            double y = value1.Y - value2.Y;
            return new Point(x, y);
        }

        public static Point3D SubtractPoint3D(Point3D value1, Point3D value2)
        {
            double x = value1.X - value2.X;
            double y = value1.Y - value2.Y;
            double z = value1.Z - value2.Z;
            return new Point3D(x, y, z);
        }

        public static Quaternion SubtractQuaternion(Quaternion value1, Quaternion value2)
        {
            value2.Invert();
            return value1 * value2;
        }

        public static float SubtractSingle(float value1, float value2)
            => value1 - value2;

        public static Size SubtractSize(Size value1, Size value2)
        {
            double width = value1.Width - value2.Width;
            double height = value1.Height - value2.Height;
            return new Size(width, height);
        }

        public static Vector SubtractVector(Vector value1, Vector value2)
            => value1 - value2;

        public static Vector3D SubtractVector3D(Vector3D value1, Vector3D value2)
            => value1 - value2;

        public static Rect SubtractRect(Rect value1, Rect value2)
        {
            Point location = SubtractPoint(value1.Location, value2.Location);
            Size size = SubtractSize(value1.Size, value2.Size);
            return new Rect(location, size);
        }

        public static Rotation3D SubtractRotation3D(Rotation3D value1, Rotation3D value2)
        {
            QuaternionRotation3D r1 = (QuaternionRotation3D)value1;
            QuaternionRotation3D r2 = (QuaternionRotation3D)value2;
            Quaternion quaternion = SubtractQuaternion(r1.Quaternion, r2.Quaternion);
            return new QuaternionRotation3D(quaternion);
        }

        public static Geometry SubtractGeometry(Geometry value1, Geometry value2)
        {
            PathGeometry geometry1 = PathGeometry.CreateFromGeometry(value1);
            PathGeometry geometry2 = PathGeometry.CreateFromGeometry(value2);
            List<PathFigure> figures = new List<PathFigure>();
            for (int i = 0; i < geometry1.Figures.Count; i++)
            {
                PathFigure figure1 = geometry1.Figures[i];
                PathFigure figure2 = geometry2.Figures[i];
                Point start = SubtractPoint(figure1.StartPoint, figure2.StartPoint);
                List<PathSegment> segments = new List<PathSegment>();
                for (int j = 0; j < figure1.Segments.Count; j++)
                {
                    PathSegment segment1 = figure1.Segments[j];
                    PathSegment segment2 = figure2.Segments[j];
                    PathSegment segment;
                    if (segment1 is LineSegment line1 && segment2 is LineSegment line2)
                    {
                        Point point = SubtractPoint(line1.Point, line2.Point);
                        segment = new LineSegment(point, line1.IsStroked);
                    }
                    else if (segment1 is ArcSegment arc1 && segment2 is ArcSegment arc2)
                    {
                        Point point = SubtractPoint(arc1.Point, arc2.Point);
                        Size size = SubtractSize(arc1.Size, arc2.Size);
                        double rotationAngle = SubtractDouble(arc1.RotationAngle, arc2.RotationAngle);
                        segment = new ArcSegment(point, size, rotationAngle, arc1.IsLargeArc, arc1.SweepDirection, arc1.IsStroked);
                    }
                    else if (segment1 is BezierSegment bezier1 && segment2 is BezierSegment bezier2)
                    {
                        Point point1 = SubtractPoint(bezier1.Point1, bezier2.Point1);
                        Point point2 = SubtractPoint(bezier1.Point2, bezier2.Point2);
                        Point point3 = SubtractPoint(bezier1.Point3, bezier2.Point3);
                        segment = new BezierSegment(point1, point2, point3, bezier1.IsStroked);
                    }
                    else if (segment1 is QuadraticBezierSegment quadraticBezier1 && segment2 is QuadraticBezierSegment quadraticBezier2)
                    {
                        Point point1 = SubtractPoint(quadraticBezier1.Point1, quadraticBezier2.Point1);
                        Point point2 = SubtractPoint(quadraticBezier1.Point2, quadraticBezier2.Point2);
                        segment = new QuadraticBezierSegment(point1, point2, quadraticBezier1.IsStroked);
                    }
                    else if (segment1 is PolyLineSegment polyLine1 && segment2 is PolyLineSegment polyLine2)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < polyLine1.Points.Count; k++)
                        {
                            Point point1 = polyLine1.Points[k];
                            Point point2 = polyLine2.Points[k];
                            Point point = SubtractPoint(point1, point2);
                            points.Add(point);
                        }
                        segment = new PolyLineSegment(points, polyLine1.IsStroked);
                    }
                    else if (segment1 is PolyBezierSegment polyBezier1 && segment2 is PolyBezierSegment polyBezier2)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < polyBezier1.Points.Count; k++)
                        {
                            Point point1 = polyBezier1.Points[k];
                            Point point2 = polyBezier2.Points[k];
                            Point point = SubtractPoint(point1, point2);
                            points.Add(point);
                        }
                        segment = new PolyBezierSegment(points, polyBezier1.IsStroked);
                    }
                    else if (segment1 is PolyQuadraticBezierSegment polyQuadraticBezier1
                        && segment2 is PolyQuadraticBezierSegment polyQuadraticBezier2)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < polyQuadraticBezier1.Points.Count; k++)
                        {
                            Point point1 = polyQuadraticBezier1.Points[k];
                            Point point2 = polyQuadraticBezier2.Points[k];
                            Point point = SubtractPoint(point1, point2);
                            points.Add(point);
                        }
                        segment = new PolyQuadraticBezierSegment(points, polyQuadraticBezier1.IsStroked);
                    }
                    else
                    {
                        throw new NotImplementedException("暂不支持此转换");
                    }
                    segments.Add(segment);
                }
                PathFigure figure = new PathFigure(start, segments, figure1.IsClosed);
                figures.Add(figure);
            }
            return new PathGeometry(figures, geometry1.FillRule, geometry1.Transform);
        }

        #endregion

        #region 获取长度

        public static double GetSegmentLengthBoolean(bool from, bool to)
        {
            if (from != to)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

        public static double GetSegmentLengthByte(byte from, byte to)
            => Math.Abs(to - from);

        public static double GetSegmentLengthChar(char from, char to)
        {
            if (from != to)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

        public static double GetSegmentLengthColor(Color from, Color to)
            => Math.Abs(to.ScA - from.ScA) + Math.Abs(to.ScR - from.ScR) + Math.Abs(to.ScG - from.ScG) + Math.Abs(to.ScB - from.ScB);

        // We may lose precision here, but it's not likely going to be a big deal
        // for the purposes of this method.  The relative lengths of Decimal
        // segments will still be adequately represented.
        public static double GetSegmentLengthDecimal(decimal from, decimal to)
            => (double)Math.Abs(to - from);

        public static double GetSegmentLengthDouble(double from, double to)
            => Math.Abs(to - from);

        public static double GetSegmentLengthInt16(short from, short to)
            => Math.Abs(to - from);

        public static double GetSegmentLengthInt32(int from, int to)
            => Math.Abs(to - from);

        public static double GetSegmentLengthInt64(long from, long to)
            => Math.Abs(to - from);

        public static double GetSegmentLengthMatrix(Matrix from, Matrix to)
        {
            if (from != to)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

        public static double GetSegmentLengthObject(object from, object to)
            => 1.0;

        public static double GetSegmentLengthPoint(Point from, Point to)
            => Math.Abs((to - from).Length);

        public static double GetSegmentLengthPoint3D(Point3D from, Point3D to)
            => Math.Abs((to - from).Length);

        public static double GetSegmentLengthQuaternion(Quaternion from, Quaternion to)
        {
            from.Invert();
            return (to * from).Angle;
        }

        public static double GetSegmentLengthRect(Rect from, Rect to)
        {
            // This seems to me to be the most logical way to define the
            // distance between two rects.  Lots of sqrt, but since paced
            // rectangle animations are such a rare thing, we may as well do
            // them right since the user obviously knows what they want.
            double a = GetSegmentLengthPoint(from.Location, to.Location);
            double b = GetSegmentLengthSize(from.Size, to.Size);
            // Return c.
            return Math.Sqrt((a * a) + (b * b));
        }

        public static double GetSegmentLengthRotation3D(Rotation3D from, Rotation3D to)
        {
            QuaternionRotation3D r1 = (QuaternionRotation3D)from;
            QuaternionRotation3D r2 = (QuaternionRotation3D)to;
            return GetSegmentLengthQuaternion(r1.Quaternion, r2.Quaternion);
        }

        public static double GetSegmentLengthSingle(float from, float to)
            => Math.Abs(to - from);

        public static double GetSegmentLengthSize(Size from, Size to)
            => Math.Abs(((Vector)to - (Vector)from).Length);

        public static double GetSegmentLengthString(string from, string to)
        {
            if (from != to)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

        public static double GetSegmentLengthVector(Vector from, Vector to)
            => Math.Abs((to - from).Length);

        public static double GetSegmentLengthVector3D(Vector3D from, Vector3D to)
            => Math.Abs((to - from).Length);

        #endregion

        #region 缩放

        public static byte ScaleByte(byte value, double factor)
            => (byte)(value * factor);

        public static Color ScaleColor(Color value, double factor)
            => value * (float)factor;

        public static decimal ScaleDecimal(decimal value, double factor)
            => value * (decimal)factor;

        public static double ScaleDouble(double value, double factor)
            => value * factor;

        public static short ScaleInt16(short value, double factor)
            => (short)(value * factor);

        public static int ScaleInt32(int value, double factor)
            => (int)(value * factor);

        public static long ScaleInt64(long value, double factor)
            => (long)(value * factor);

        public static Point ScalePoint(Point value, double factor)
        {
            double x = value.X * factor;
            double y = value.Y * factor;
            return new Point(x, y);
        }

        public static Point3D ScalePoint3D(Point3D value, double factor)
        {
            double x = value.X * factor;
            double y = value.Y * factor;
            double z = value.Z * factor;
            return new Point3D(x, y, z);
        }

        public static Quaternion ScaleQuaternion(Quaternion value, double factor)
            => new Quaternion(value.Axis, value.Angle * factor);

        public static Rect ScaleRect(Rect value, double factor)
        {
            double x = value.Location.X * factor;
            double y = value.Location.Y * factor;
            Point location = new Point(x, y);
            double width = value.Size.Width * factor;
            double height = value.Size.Height * factor;
            Size size = new Size(width, height);
            return new Rect(location, size);
        }

        public static Rotation3D ScaleRotation3D(Rotation3D value, double factor)
        {
            QuaternionRotation3D r = (QuaternionRotation3D)value;
            Quaternion quaternion = ScaleQuaternion(r.Quaternion, factor);
            return new QuaternionRotation3D(quaternion);
        }

        public static float ScaleSingle(float value, double factor)
            => (float)(value * factor);

        public static Size ScaleSize(Size value, double factor)
            => (Size)((Vector)value * factor);

        public static Vector ScaleVector(Vector value, double factor)
            => value * factor;

        public static Vector3D ScaleVector3D(Vector3D value, double factor)
            => value * factor;

        public static Geometry ScaleGeometry(Geometry value, double factor)
        {
            PathGeometry baseGeometry = PathGeometry.CreateFromGeometry(value);
            List<PathFigure> figures = new List<PathFigure>();
            for (int i = 0; i < baseGeometry.Figures.Count; i++)
            {
                PathFigure baseFigure = baseGeometry.Figures[i];
                Point start = ScalePoint(baseFigure.StartPoint, factor);
                List<PathSegment> segments = new List<PathSegment>();
                for (int j = 0; j < baseFigure.Segments.Count; j++)
                {
                    PathSegment baseSegment = baseFigure.Segments[j];
                    PathSegment segment;
                    if (baseSegment is LineSegment baseLine)
                    {
                        Point point = ScalePoint(baseLine.Point, factor);
                        segment = new LineSegment(point, baseLine.IsStroked);
                    }
                    else if (baseSegment is ArcSegment baseArc)
                    {
                        Point point = ScalePoint(baseArc.Point, factor);
                        Size size = ScaleSize(baseArc.Size, factor);
                        double rotationAngle = ScaleDouble(baseArc.RotationAngle, factor);
                        segment = new ArcSegment(point, size, rotationAngle, baseArc.IsLargeArc, baseArc.SweepDirection, baseArc.IsStroked);
                    }
                    else if (baseSegment is BezierSegment baseBezier)
                    {
                        Point point1 = ScalePoint(baseBezier.Point1, factor);
                        Point point2 = ScalePoint(baseBezier.Point2, factor);
                        Point point3 = ScalePoint(baseBezier.Point3, factor);
                        segment = new BezierSegment(point1, point2, point3, baseBezier.IsStroked);
                    }
                    else if (baseSegment is QuadraticBezierSegment baseQuadraticBezier)
                    {
                        Point point1 = ScalePoint(baseQuadraticBezier.Point1, factor);
                        Point point2 = ScalePoint(baseQuadraticBezier.Point2, factor);
                        segment = new QuadraticBezierSegment(point1, point2, baseQuadraticBezier.IsStroked);
                    }
                    else if (baseSegment is PolyLineSegment basePolyLine)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < basePolyLine.Points.Count; k++)
                        {
                            Point basePoint = basePolyLine.Points[k];
                            Point point = ScalePoint(basePoint, factor);
                            points.Add(point);
                        }
                        segment = new PolyLineSegment(points, basePolyLine.IsStroked);
                    }
                    else if (baseSegment is PolyBezierSegment basePolyBezier)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < basePolyBezier.Points.Count; k++)
                        {
                            Point basePoint = basePolyBezier.Points[k];
                            Point point = ScalePoint(basePoint, factor);
                            points.Add(point);
                        }
                        segment = new PolyBezierSegment(points, basePolyBezier.IsStroked);
                    }
                    else if (baseSegment is PolyQuadraticBezierSegment basePolyQuadraticBezier)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < basePolyQuadraticBezier.Points.Count; k++)
                        {
                            Point basePoint = basePolyQuadraticBezier.Points[k];
                            Point point = ScalePoint(basePoint, factor);
                            points.Add(point);
                        }
                        segment = new PolyQuadraticBezierSegment(points, basePolyQuadraticBezier.IsStroked);
                    }
                    else
                    {
                        throw new NotImplementedException("暂不支持此转换");
                    }
                    segments.Add(segment);
                }
                PathFigure figure = new PathFigure(start, segments, baseFigure.IsClosed);
                figures.Add(figure);
            }
            return new PathGeometry(figures, baseGeometry.FillRule, baseGeometry.Transform);
        }

        #endregion

        #region 校验

        public static bool IsValidAnimationValueBoolean(bool value) => true;

        public static bool IsValidAnimationValueByte(byte value) => true;

        public static bool IsValidAnimationValueChar(char value) => true;

        public static bool IsValidAnimationValueColor(Color value) => true;

        public static bool IsValidAnimationValueDecimal(decimal value) => true;

        public static bool IsValidAnimationValueDouble(double value)
        {
            if (IsInvalidDouble(value))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidAnimationValueInt16(short value) => true;

        public static bool IsValidAnimationValueInt32(int value) => true;

        public static bool IsValidAnimationValueInt64(long value) => true;

        public static bool IsValidAnimationValueMatrix(Matrix value) => true;

        public static bool IsValidAnimationValuePoint(Point value)
        {
            if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidAnimationValuePoint3D(Point3D value)
        {
            if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y) || IsInvalidDouble(value.Z))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidAnimationValueQuaternion(Quaternion value)
        {
            if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y) || IsInvalidDouble(value.Z) || IsInvalidDouble(value.W))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidAnimationValueRect(Rect value)
        {
            if (IsInvalidDouble(value.Location.X) || IsInvalidDouble(value.Location.Y) || IsInvalidDouble(value.Size.Width) || IsInvalidDouble(value.Size.Height) || value.IsEmpty)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidAnimationValueRotation3D(Rotation3D value)
        {
            QuaternionRotation3D r = (QuaternionRotation3D)value;
            return IsValidAnimationValueQuaternion(r.Quaternion);
        }

        public static bool IsValidAnimationValueSingle(float value)
        {
            if (IsInvalidDouble(value))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidAnimationValueSize(Size value)
        {
            if (IsInvalidDouble(value.Width) || IsInvalidDouble(value.Height))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidAnimationValueString(string value)
            => true;

        public static bool IsValidAnimationValueVector(Vector value)
        {
            if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidAnimationValueVector3D(Vector3D value)
        {
            if (IsInvalidDouble(value.X) || IsInvalidDouble(value.Y) || IsInvalidDouble(value.Z))
            {
                return false;
            }
            return true;
        }

        // TODO: 是否需要对 Geometry 进行校验?
        public static bool IsValidAnimationValueGeometry(Geometry value)
            => true;

        private static bool IsInvalidDouble(double value)
            => double.IsInfinity(value) || double.IsNaN(value);


        #endregion

        #region 初始值

        public static byte GetZeroValueByte(byte baseValue)
            => 0;

        public static Color GetZeroValueColor(Color baseValue)
            => Color.FromScRgb(0.0F, 0.0F, 0.0F, 0.0F);

        public static decimal GetZeroValueDecimal(decimal baseValue)
            => decimal.Zero;

        public static double GetZeroValueDouble(double baseValue)
            => 0.0;

        public static short GetZeroValueInt16(short baseValue)
            => 0;

        public static int GetZeroValueInt32(int baseValue)
            => 0;

        public static long GetZeroValueInt64(long baseValue)
            => 0;

        public static Point GetZeroValuePoint(Point baseValue)
            => new Point();

        public static Point3D GetZeroValuePoint3D(Point3D baseValue)
            => new Point3D();

        public static Quaternion GetZeroValueQuaternion(Quaternion baseValue)
            => Quaternion.Identity;

        public static float GetZeroValueSingle(float baseValue)
            => 0.0F;

        public static Size GetZeroValueSize(Size baseValue)
            => new Size();

        public static Vector GetZeroValueVector(Vector baseValue)
            => new Vector();

        public static Vector3D GetZeroValueVector3D(Vector3D baseValue)
            => new Vector3D();

        public static Rect GetZeroValueRect(Rect baseValue)
            => new Rect(new Point(), new Vector());

        public static Rotation3D GetZeroValueRotation3D(Rotation3D baseValue)
            => Rotation3D.Identity;

        public static Geometry GetZeroValueGeometry(Geometry baseValue)
        {
            PathGeometry baseGeometry = PathGeometry.CreateFromGeometry(baseValue);
            List<PathFigure> figures = new List<PathFigure>();
            for (int i = 0; i < baseGeometry.Figures.Count; i++)
            {
                PathFigure baseFigure = baseGeometry.Figures[i];
                Point start = new Point();
                List<PathSegment> segments = new List<PathSegment>();
                for (int j = 0; j < baseFigure.Segments.Count; j++)
                {
                    PathSegment baseSegment = baseFigure.Segments[j];
                    PathSegment segment;
                    if (baseSegment is LineSegment baseLine)
                    {
                        Point point = new Point();
                        segment = new LineSegment(point, baseLine.IsStroked);
                    }
                    else if (baseSegment is ArcSegment baseArc)
                    {
                        Point point = new Point();
                        Size size = new Size();
                        double rotationAngle = 0.0;
                        segment = new ArcSegment(point, size, rotationAngle, baseArc.IsLargeArc, baseArc.SweepDirection, baseArc.IsStroked);
                    }
                    else if (baseSegment is BezierSegment baseBezier)
                    {
                        Point point1 = new Point();
                        Point point2 = new Point();
                        Point point3 = new Point();
                        segment = new BezierSegment(point1, point2, point3, baseBezier.IsStroked);
                    }
                    else if (baseSegment is QuadraticBezierSegment baseQuadraticBezier)
                    {
                        Point point1 = new Point();
                        Point point2 = new Point();
                        segment = new QuadraticBezierSegment(point1, point2, baseQuadraticBezier.IsStroked);
                    }
                    else if (baseSegment is PolyLineSegment basePolyLine)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < basePolyLine.Points.Count; k++)
                        {
                            Point point = new Point();
                            points.Add(point);
                        }
                        segment = new PolyLineSegment(points, basePolyLine.IsStroked);
                    }
                    else if (baseSegment is PolyBezierSegment basePolyBezier)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < basePolyBezier.Points.Count; k++)
                        {
                            Point point = new Point();
                            points.Add(point);
                        }
                        segment = new PolyBezierSegment(points, basePolyBezier.IsStroked);
                    }
                    else if (baseSegment is PolyQuadraticBezierSegment basePolyQuadraticBezier)
                    {
                        List<Point> points = new List<Point>();
                        for (int k = 0; k < basePolyQuadraticBezier.Points.Count; k++)
                        {
                            Point point = new Point();
                            points.Add(point);
                        }
                        segment = new PolyQuadraticBezierSegment(points, basePolyQuadraticBezier.IsStroked);
                    }
                    else
                    {
                        throw new NotImplementedException("暂不支持此转换");
                    }
                    segments.Add(segment);
                }
                PathFigure figure = new PathFigure(start, segments, baseFigure.IsClosed);
                figures.Add(figure);
            }
            return new PathGeometry(figures, baseGeometry.FillRule, baseGeometry.Transform);
        }

        #endregion
    }
}