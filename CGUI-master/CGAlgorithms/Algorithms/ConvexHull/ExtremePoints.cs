using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Basic Shape
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }

            // Remove Duplicate
            List<Point> unique = new List<Point>();
            for (int i = 0; i < points.Count; i++)
                if (!unique.Contains(points[i]))
                    unique.Add(points[i]);
            points = unique;

            // Algorithm
            var Eliminated = new Dictionary<Point, bool>();
            for (int i = 0; i < points.Count; i++)
            {
                Eliminated.Add(points[i], false);
            }
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    for (int k = 0; k < points.Count; k++)
                    {
                        for (int l = 0; l < points.Count; l++)
                        {
                            if (j != i && k != i && l != i)
                            {
                                if (Eliminated[points[i]])
                                    continue;
                                if (CGUtilities.HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]) != Enums.PointInPolygon.Outside)
                                {
                                    //points.Remove(points[i]);
                                    Eliminated[points[i]] = true;
                                }
                            }

                        }
                    }

                }

            }
            for (int i = 0; i < points.Count; i++)
            {
                if (!Eliminated[points[i]])
                    outPoints.Add(points[i]);
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}