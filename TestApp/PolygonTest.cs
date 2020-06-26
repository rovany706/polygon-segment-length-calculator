using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace TestApp
{
    [TestFixture]
    class PolygonTest
    {
        private static List<List<Point>> testPolygons;
        private static List<Line> lines;
        private static readonly string POLYGON_CSV = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "table1.csv");
        private static readonly string LINES_CSV = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "table2.csv");

        private static object[] IsInPolygonTestCases =
        {
            new object[] {0, new Point {X = 6254708.076745, Y = 7958521.945621}, true},
            new object[] {0, new Point {X = 6254024.819002, Y = 7959228.763976}, false},
            new object[] {1, new Point(7,6), true},
            new object[] {1, new Point(3,9), false},
            new object[] {1, new Point(7,2), true},
            new object[] {1, new Point(7,10), true},
            new object[] {1, new Point(8,11), false},
            new object[] {1, new Point(9,10), true},
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

        [SetUp]
        public void SetUp()
        {
            testPolygons = new List<List<Point>>();
            testPolygons.Add(Program.ReadPolygonFromFile(POLYGON_CSV)); // polygonIndex = 0
            testPolygons.Add(new List<Point> // polygonIndex = 1
            {
                new Point(4,5),
                new Point(7,2),
                new Point(13,3),
                new Point(15,6),
                new Point(13,10),
                new Point(7,10)
            });
            lines = Program.ReadLinesFromFile(LINES_CSV);
        }

        [Test]
        [TestCaseSource(nameof(IsInPolygonTestCases))]
        public void IsInPolygon(int polygonIndex, Point point, bool expected)
        {
            bool result = Program.IsInPolygon(point, testPolygons[polygonIndex]);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCaseSource(nameof(PointOfIntersectionTestCases))]
        public void PointOfIntersectionTest(Line lineA, Line lineB, Point expected)
        {
            Point result = Program.GetIntersection(lineA, lineB);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCaseSource(nameof(LineIntersectionTestCases))]
        public void LineIntersectionTest(Line lineA, Line lineB, bool expected)
        {
            bool result = Program.IsIntersecting(lineA, lineB);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ReadPolygonSuccess()
        {
            var polygon = Program.ReadPolygonFromFile(POLYGON_CSV);

            Assert.IsTrue(polygon.Any());
        }

        [Test]
        public void ReadLinesSuccess()
        {
            var lines = Program.ReadLinesFromFile(LINES_CSV);

            Assert.IsTrue(lines.Any());
        }

    }
}
