using System;

namespace TestApp
{
    class Line
    {
        public Line() { }

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
        public double A
        {
            get => EndPoint.Y - BeginPoint.Y;
        }
        public double B
        {
            get => BeginPoint.X - EndPoint.X;
        }
        public double C
        {
            get => A * BeginPoint.X + B * BeginPoint.Y;
        }
        public double Length
        {
            get => Math.Sqrt((EndPoint - BeginPoint).SqrLength);
        }
    }
}