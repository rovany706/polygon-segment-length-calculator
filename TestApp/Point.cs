namespace TestApp
{
    class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Point() { }

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
        /// Векторное произведение
        /// </summary>
        public double Cross(Point otherPoint)
        {
            return this.X * otherPoint.Y - this.Y * otherPoint.X;
        }

        public double Cross(Point a, Point b)
        {
            return (a - this).Cross(b - this);
        }

        /// <summary>
        /// Скалярное произведение
        /// </summary>
        public double Dot(Point otherPoint)
        {
            return this.X * otherPoint.X + this.Y * otherPoint.Y;
        }

        public double SqrLength
        {
            get => this.Dot(this);
        }

        public override bool Equals(object obj)
        {
            Point otherPoint = (Point)obj;

            return this.X == otherPoint.X && this.Y == otherPoint.Y;
        }
    }
}