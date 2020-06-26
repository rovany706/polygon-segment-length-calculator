using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Polygon.Classes;

namespace Polygon.Tests
{
    [TestFixture]
    public class PolygonTests
    {
        private static List<List<Point>> testPolygons;
        private static List<List<Line>> testLines;

        private static readonly string POLYGON_CSV =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "table1.csv");

        private static readonly string LINES_CSV =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "table2.csv");

        private static object[] IsInPolygonTestCases =
        {
            new object[] {0, new Point {X = 6254708.076745, Y = 7958521.945621}, true},
            new object[] {0, new Point {X = 6254024.819002, Y = 7959228.763976}, false},
            new object[] {1, new Point(7, 6), true},
            new object[] {1, new Point(3, 9), false},
            new object[] {1, new Point(7, 2), true},
            new object[] {1, new Point(7, 10), true},
            new object[] {1, new Point(8, 11), false},
            new object[] {1, new Point(9, 10), true}
        };

        private static object[] PointOfIntersectionTestCases =
        {
            new object[] {new Line(7, 10, 13, 10), new Line(8, 9, 8, 13), new Point(8, 10)}
        };

        private static object[] LineIntersectionTestCases =
        {
            new object[] {new Line(7, 10, 13, 10), new Line(8, 9, 8, 13), true},
            new object[] {new Line(7, 10, 13, 10), new Line(8, 11, 8, 13), false},
            new object[] {new Line(7, 10, 13, 10), new Line(7, 11, 13, 11), false}
        };

        private static object[] SegmentLengthInPolygonTestCases =
        {
            new object[] {1, 14},
            new object[] {2, 6},
            new object[] {3, 8}
        };

        [OneTimeSetUp]
        public void SetUp()
        {
            testPolygons = new List<List<Point>>
            {
                CsvReader.ReadPolygonFromFile(POLYGON_CSV),
                new List<Point>
                {
                    new Point(4, 5),
                    new Point(7, 2),
                    new Point(13, 3),
                    new Point(15, 6),
                    new Point(13, 10),
                    new Point(7, 10)
                },
                new List<Point> {new Point(1, 1), new Point(1, 7), new Point(7, 7), new Point(7, 1)},
                new List<Point> {new Point(1, 4), new Point(3, 6), new Point(5, 4), new Point(3, 2)}
            };

            testLines = new List<List<Line>>
            {
                CsvReader.ReadLinesFromFile(LINES_CSV),
                new List<Line>
                {
                    new Line(8, 9, 8, 13),
                    new Line(7, 1, 7, 5),
                    new Line(13, 2, 13, 13),
                    new Line(10, 8, 13, 8)
                },
                new List<Line>
                {
                    new Line(0, 3, 8, 3)
                },
                new List<Line>
                {
                    new Line(0, 4, 6, 4),
                    new Line(3, 1, 3, 7)
                }
            };
        }

        [Test]
        [TestCaseSource(nameof(IsInPolygonTestCases))]
        public void IsInPolygon(int polygonIndex, Point point, bool expected)
        {
            var polygonChecker = new PolygonChecker(testPolygons[polygonIndex]);
            var result = polygonChecker.IsInPolygon(point);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCaseSource(nameof(PointOfIntersectionTestCases))]
        public void PointOfIntersectionTest(Line lineA, Line lineB, Point expected)
        {
            var result = Line.GetIntersection(lineA, lineB);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCaseSource(nameof(LineIntersectionTestCases))]
        public void LineIntersectionTest(Line lineA, Line lineB, bool expected)
        {
            var result = Line.IsIntersecting(lineA, lineB);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ReadPolygonSuccess()
        {
            var polygon = CsvReader.ReadPolygonFromFile(POLYGON_CSV);

            Assert.IsTrue(polygon.Any());
        }

        [Test]
        public void ReadLinesSuccess()
        {
            var lines = CsvReader.ReadLinesFromFile(LINES_CSV);

            Assert.IsTrue(lines.Any());
        }

        [Test]
        [TestCaseSource(nameof(SegmentLengthInPolygonTestCases))]
        public void GetSegmentLengthInPolygon(int dataIndex, double expected)
        {
            var polygonChecker = new PolygonChecker(testPolygons[dataIndex]);

            var sum = polygonChecker.GetSegmentLengthInPolygon(testLines[dataIndex]);
#if DEBUG
            TestContext.Out.WriteLine($"Посчитанная сумма = {sum}");
#endif
            Assert.AreEqual(expected, sum, 0.1);
        }
    }
}