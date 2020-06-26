using System;

namespace Polygon.Classes
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Point()
        {
        }

        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point operator -(Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static Point operator *(Point point, double multiplier)
        {
            return new Point(point.X * multiplier, point.Y * multiplier);
        }

        /// <summary>
        ///     Векторное произведение
        /// </summary>
        public double Cross(Point otherPoint)
        {
            return X * otherPoint.Y - Y * otherPoint.X;
        }

        public double Cross(Point a, Point b)
        {
            return (a - this).Cross(b - this);
        }

        /// <summary>
        ///     Скалярное произведение
        /// </summary>
        public double Dot(Point otherPoint)
        {
            return X * otherPoint.X + Y * otherPoint.Y;
        }

        public double SqrLength => Dot(this);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        protected bool Equals(Point other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }
    }
}