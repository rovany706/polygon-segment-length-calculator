using System.IO;
using System.Reflection;
using Polygon.Classes;

namespace Polygon.Console
{
    internal class Program
    {
        private static readonly string POLYGON_CSV =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "table1.csv");

        private static readonly string LINES_CSV =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "table2.csv");

        private static void Main(string[] args)
        {
            var polygon = CsvReader.ReadPolygonFromFile(POLYGON_CSV);
            var lines = CsvReader.ReadLinesFromFile(LINES_CSV);
            var polygonChecker = new PolygonChecker(polygon);

            var sum = polygonChecker.GetSegmentLengthInPolygon(lines);

            System.Console.WriteLine($"Посчитанная сумма = {sum}");
            System.Console.ReadLine();
        }
    }
}