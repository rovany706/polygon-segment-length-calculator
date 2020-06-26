using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Polygon.Classes.Extensions;

namespace Polygon.Classes
{
    public class PolygonChecker
    {
        public List<Point> Polygon { get; set; }

        public PolygonChecker(List<Point> polygon)
        {
            Polygon = polygon;
        }

        public double GetSegmentLengthInPolygon(List<Line> lines)
        {
            double sum = 0;
            foreach (var line in lines)
            {
                var beginPointInPolygon = IsInPolygon(line.BeginPoint);
                var endPointInPolygon = IsInPolygon(line.EndPoint);
                if (beginPointInPolygon && endPointInPolygon) // если обе точки внутри полигона
                {
                    sum += line.Length;
                }
                else if (beginPointInPolygon || endPointInPolygon) // если одна из точек внутри, то ищем отрезок, находящийся внутри
                {
                    Polygon.Add(Polygon[0]); // add first point
                    for (var i = 0; i < Polygon.Count - 1; i++)
                    {
                        var polygonSide = new Line(Polygon[i], Polygon[i + 1]);
                        if (Line.IsIntersecting(polygonSide, line))
                        {
                            var intersection = Line.GetIntersection(polygonSide, line);
                            var pointInPolygon = beginPointInPolygon ? line.BeginPoint : line.EndPoint;

                            var segmentInPolygon = new Line(intersection, pointInPolygon);
                            sum += segmentInPolygon.Length;
                            break;
                        }
                    }

                    Polygon.RemoveAt(Polygon.Count - 1); // remove previously added element
                }
                else // если ни одной из точек нет внутри, то проверяем, пересекает ли отрезок полигон
                {
                    var intersections = new List<Point>();
                    Polygon.Add(Polygon[0]); // add first point
                    for (var i = 0; i < Polygon.Count - 1; i++)
                    {
                        var polygonSide = new Line(Polygon[i], Polygon[i + 1]);
                        if (Line.IsIntersecting(polygonSide, line))
                        {
                            var intersection = Line.GetIntersection(polygonSide, line);
                            intersections.Add(intersection);
                        }
                    }
                    // граничный случай: при прохождении отрезка через вершину, точка пересечения добавится дважды
                    intersections = intersections.Distinct().ToList();
                    if (intersections.Count == 2) // пересечений в выпуклом многоугольнике должно быть ровно 2
                    {
                        var segmentInPolygon = new Line(intersections[0], intersections[1]);
                        sum += segmentInPolygon.Length;
                    }

                    Polygon.RemoveAt(Polygon.Count - 1); // remove previously added element
                }
            }

            return sum;
        }

        public bool IsInPolygon(Point point)
        {
            // идея взята отсюда https://cp-algorithms.com/geometry/point-in-convex-polygon.html
            // поиск левой нижней точки полигона
            var firstPolygonPointIndex = 0;
            for (var i = 1; i < Polygon.Count; i++)
                if (Polygon[i].X < Polygon[firstPolygonPointIndex].X ||
                    Polygon[i].X == Polygon[firstPolygonPointIndex].X &&
                    Polygon[i].Y < Polygon[firstPolygonPointIndex].Y)
                    firstPolygonPointIndex = i;

            Polygon = Polygon.Rotate(firstPolygonPointIndex);

            var n = Polygon.Count;
            // если точка находится в пределах полигона
            if (Polygon[0].Cross(Polygon[1], point) != 0 &&
                Math.Sign(Polygon[0].Cross(Polygon[1], point)) !=
                Math.Sign(Polygon[0].Cross(Polygon[1], Polygon[n - 1])))
                return false;
            if (Polygon[0].Cross(Polygon[n - 1], point) != 0 &&
                Math.Sign(Polygon[0].Cross(Polygon[n - 1], point)) !=
                Math.Sign(Polygon[0].Cross(Polygon[n - 1], Polygon[1])))
                return false;

            // если точка находится на ребре p0, p1
            if (Polygon[0].Cross(Polygon[1], point) == 0)
                return (Polygon[1] - Polygon[0]).SqrLength >= (point - Polygon[0]).SqrLength;
            // если точка находится на ребре p0, pn
            if (Polygon[0].Cross(Polygon[n - 1], point) == 0)
                return (Polygon[n - 1] - Polygon[0]).SqrLength >= (point - Polygon[0]).SqrLength;

            // бинарный поиск треугольника, в котором находится точка
            var left = 1;
            var right = Polygon.Count - 1;
            while (right - left > 1)
            {
                var middle = (left + right) / 2;
                if (Polygon[0].Cross(Polygon[middle], point) >= 0)
                    left = middle;
                else
                    right = middle;
            }

            var pos = left;
            return IsInTriangle(Polygon[pos], Polygon[pos + 1], Polygon[0], point);
        }

        public bool IsInTriangle(Point a, Point b, Point c, Point point)
        {
            // точка находится внутри треугольника, если площадь треугольника и сумма площадей треугольников, образованных точкой и вершинами треугольника, равны
            var s1 = Math.Abs(a.Cross(b, c));
            var s2 = Math.Abs(point.Cross(a, b)) + Math.Abs(point.Cross(b, c)) + Math.Abs(point.Cross(c, a));

            return s1 == s2;
        }
    }
}