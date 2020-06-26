using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TestApp
{
    class Program
    {
        private static readonly string POLYGON_CSV = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "table1.csv");
        private static readonly string LINES_CSV = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "table2.csv");

        static void Main(string[] args)
        {
            // Read polygon
            List<Point> polygon = ReadPolygonFromFile(POLYGON_CSV);

            // Read lines
            List<Line> lines = ReadLinesFromFile(LINES_CSV);

            double sum = 0;
            foreach (Line line in lines)
            {
                bool beginPointInPolygon = IsInPolygon(line.BeginPoint, polygon);
                bool endPointInPolygon = IsInPolygon(line.EndPoint, polygon);
                if (beginPointInPolygon && endPointInPolygon)
                {
                    sum += line.Length;
                }
                else if (beginPointInPolygon || endPointInPolygon)
                {
                    polygon.Add(polygon[0]); // add first point
                    for (int i = 0; i < polygon.Count - 1; i++)
                    {
                        Line polygonSide = new Line(polygon[i], polygon[i + 1]);
                        if (IsIntersecting(polygonSide, line))
                        {
                            Point intersection = GetIntersection(polygonSide, line);
                            Point pointInPolygon = beginPointInPolygon ? line.BeginPoint : line.EndPoint;

                            Line segmentInPolygon = new Line(intersection, pointInPolygon);
                            sum += segmentInPolygon.Length;
                            break;
                        }
                    }

                    polygon.RemoveAt(polygon.Count - 1); // remove previously added element
                }
                else
                {
                    List<Point> intersections = new List<Point>();
                    polygon.Add(polygon[0]); // add first point
                    for (int i = 0; i < polygon.Count - 1; i++)
                    {
                        Line polygonSide = new Line(polygon[i], polygon[i + 1]);
                        if (IsIntersecting(polygonSide, line))
                        {
                            Point intersection = GetIntersection(polygonSide, line);
                            intersections.Add(intersection);
                        }
                    }

                    if (intersections.Count == 2)
                    {
                        Line segmentInPolygon = new Line(intersections[0], intersections[1]);
                        sum += segmentInPolygon.Length;
                    }

                    polygon.RemoveAt(polygon.Count - 1); // remove previously added element
                }
            }

            Console.WriteLine(sum);

            Console.ReadLine();
        }

        public static Point GetIntersection(Line lineA, Line lineB)
        {
            double delta = lineA.A * lineB.B - lineB.A * lineA.B;

            if (delta == 0)
                throw new ArgumentException("Lines are parallel");

            double x = (lineB.B * lineA.C - lineA.B * lineB.C) / delta;
            double y = (lineA.A * lineB.C - lineB.A * lineA.C) / delta;

            return new Point(x, y);
        }

        public static bool IsIntersecting(Line lineA, Line lineB)
        {
            // https://stackoverflow.com/a/565282
            Point p = lineA.BeginPoint;
            Point r = lineA.EndPoint;

            Point q = lineB.BeginPoint;
            Point s = lineB.EndPoint;

            double t = (q - p).Cross(s) / r.Cross(s);
            double u = (q - p).Cross(r) / r.Cross(s);
            if (r.Cross(s) != 0 && t <= 1 && t >= 0 && u <= 1 && u >= 0)
                return true;
            return false;
        }

        public static bool IsInPolygon(Point point, List<Point> polygon)
        {
            // идея взята отсюда https://cp-algorithms.com/geometry/point-in-convex-polygon.html
            // поиск левой нижней точки полигона
            int firstPolygonPointIndex = 0;
            for (int i = 1; i < polygon.Count; i++)
            {
                if (polygon[i].X < polygon[firstPolygonPointIndex].X || polygon[i].X == polygon[firstPolygonPointIndex].X && polygon[i].Y < polygon[firstPolygonPointIndex].Y)
                    firstPolygonPointIndex = i;
            }

            polygon = polygon.Rotate(firstPolygonPointIndex);

            int n = polygon.Count;
            // если точка находится в пределах полигона
            if (polygon[0].Cross(polygon[1], point) != 0 &&
                //Math.Sign(seq[0].Cross(point)) != Math.Sign(seq[0].Cross(seq[seq.Count - 1])))
                Math.Sign(polygon[0].Cross(polygon[1], point)) != Math.Sign(polygon[0].Cross(polygon[1], polygon[n - 1])))
                return false;
            if (polygon[0].Cross(polygon[n - 1], point) != 0 &&
                //Math.Sign(seq[seq.Count - 1].Cross(point)) != Math.Sign(seq[seq.Count - 1].Cross(seq[0])))
                Math.Sign(polygon[0].Cross(polygon[n - 1], point)) != Math.Sign(polygon[0].Cross(polygon[n - 1], polygon[1])))
                return false;

            // если точка находится на ребре p0, p1
            if (polygon[0].Cross(polygon[1], point) == 0)
                return (polygon[1] - polygon[0]).SqrLength >= (point - polygon[0]).SqrLength;
            // если точка находится на ребре p0, pn
            if (polygon[0].Cross(polygon[n - 1], point) == 0)
                return (polygon[n - 1] - polygon[0]).SqrLength >= (point - polygon[0]).SqrLength;

            // бинарный поиск треугольника, в котором находится точка
            int left = 1;
            int right = polygon.Count - 1;
            while (right - left > 1)
            {
                int middle = (left + right) / 2;
                if (polygon[0].Cross(polygon[middle], point) >= 0)
                    left = middle;
                else
                    right = middle;
            }

            int pos = left;
            return IsInTriangle(polygon[pos], polygon[pos + 1], polygon[0], point);
        }

        public static bool IsInTriangle(Point a, Point b, Point c, Point point)
        {
            // точка находится внутри треугольника, если площадь треугольника и сумма площадей треугольников, образованных точкой и вершинами треугольника, равны
            double s1 = Math.Abs(a.Cross(b, c));
            double s2 = Math.Abs(point.Cross(a, b)) + Math.Abs(point.Cross(b, c)) + Math.Abs(point.Cross(c, a));

            return s1 == s2;
        }

        public static List<Point> ReadPolygonFromFile(string path)
        {
            List<Point> polygon = new List<Point>();
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine(); // skip header
                while (!reader.EndOfStream)
                {
                    var stringLine = reader.ReadLine();
                    var values = stringLine.Split(',');

                    double x = double.Parse(values[0], CultureInfo.InvariantCulture);
                    double y = double.Parse(values[1], CultureInfo.InvariantCulture);

                    Point point = new Point(x, y);
                    polygon.Add(point);
                }
            }

            return polygon;
        }

        public static List<Line> ReadLinesFromFile(string path)
        {
            List<Line> lines = new List<Line>();
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine(); // skip header
                while (!reader.EndOfStream)
                {
                    var stringLine = reader.ReadLine();
                    var values = stringLine.Split(',');

                    double x1 = double.Parse(values[0], CultureInfo.InvariantCulture);
                    double y1 = double.Parse(values[1], CultureInfo.InvariantCulture);

                    double x2 = double.Parse(values[2], CultureInfo.InvariantCulture);
                    double y2 = double.Parse(values[3], CultureInfo.InvariantCulture);

                    Point beginPoint = new Point(x1, y1);
                    Point endPoint = new Point(x2, y2);
                    Line line = new Line
                    {
                        BeginPoint = beginPoint,
                        EndPoint = endPoint
                    };

                    lines.Add(line);
                }
            }

            return lines;
        }
    }
}
