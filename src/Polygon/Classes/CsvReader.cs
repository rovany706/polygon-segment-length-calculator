using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Polygon.Classes
{
    public class CsvReader
    {
        /// <summary>
        ///     Чтение точек полигона из csv-файла
        /// </summary>
        /// <param name="path">Путь к csv-файлу</param>
        /// <returns>Список точек полигона</returns>
        public static List<Point> ReadPolygonFromFile(string path)
        {
            var polygon = new List<Point>();
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine(); // skip header
                while (!reader.EndOfStream)
                {
                    var stringLine = reader.ReadLine();
                    var values = stringLine.Split(',');

                    var x = Double.Parse(values[0], CultureInfo.InvariantCulture);
                    var y = Double.Parse(values[1], CultureInfo.InvariantCulture);

                    var point = new Point(x, y);
                    polygon.Add(point);
                }
            }

            return polygon;
        }

        /// <summary>
        ///     Чтение точек отрезков из csv-файла
        /// </summary>
        /// <param name="path">Путь к csv-файлу</param>
        /// <returns>Список отрезков</returns>
        public static List<Line> ReadLinesFromFile(string path)
        {
            var lines = new List<Line>();
            using (var reader = new StreamReader(path))
            {
                reader.ReadLine(); // skip header
                while (!reader.EndOfStream)
                {
                    var stringLine = reader.ReadLine();
                    var values = stringLine.Split(',');

                    var x1 = Double.Parse(values[0], CultureInfo.InvariantCulture);
                    var y1 = Double.Parse(values[1], CultureInfo.InvariantCulture);

                    var x2 = Double.Parse(values[2], CultureInfo.InvariantCulture);
                    var y2 = Double.Parse(values[3], CultureInfo.InvariantCulture);

                    var beginPoint = new Point(x1, y1);
                    var endPoint = new Point(x2, y2);
                    var line = new Line
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
