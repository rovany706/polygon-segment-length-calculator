using System;

namespace Polygon.Classes
{
    public class Line
    {
        public Line()
        {
        }

        public Line(double x1, double y1, double x2, double y2)
        {
            BeginPoint = new Point(x1, y1);
            EndPoint = new Point(x2, y2);
        }

        public Line(Point beginPoint, Point endPoint)
        {
            BeginPoint = beginPoint;
            EndPoint = endPoint;
        }

        public Point BeginPoint { get; set; }
        public Point EndPoint { get; set; }
        public double A => EndPoint.Y - BeginPoint.Y;
        public double B => BeginPoint.X - EndPoint.X;

        public double C => A * BeginPoint.X + B * BeginPoint.Y;

        public double Length => Math.Sqrt((EndPoint - BeginPoint).SqrLength);

        public static Point GetIntersection(Line lineA, Line lineB)
        {
            // https://stackoverflow.com/a/4543530
            var delta = lineA.A * lineB.B - lineB.A * lineA.B;

            var x = (lineB.B * lineA.C - lineA.B * lineB.C) / delta;
            var y = (lineA.A * lineB.C - lineB.A * lineA.C) / delta;

            return new Point(x, y);
        }

        public static bool IsIntersecting(Line lineA, Line lineB)
        {
            // https://stackoverflow.com/a/565282
            var p = lineA.BeginPoint;
            var r = lineA.EndPoint;

            var q = lineB.BeginPoint;
            var s = lineB.EndPoint;

            var t = (q - p).Cross(s) / r.Cross(s);
            var u = (q - p).Cross(r) / r.Cross(s);
            if (r.Cross(s) != 0 && t <= 1 && t >= 0 && u <= 1 && u >= 0)
                return true;
            return false;
        }
    }
}