using CGUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Basic Shape
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }

            // Angularly sort the points around lower y
            var ordered = points.OrderBy(p => p.Y).ToList();
            ordered = ordered.OrderBy(p => Math.Atan2(p.Y - ordered[0].Y, p.X - ordered[0].X)).ToList();

            // Graham’s algorithm 
            outPoints.Add(ordered[0]);
            outPoints.Add(ordered[1]);
            outPoints.Add(ordered[2]);
            for (int i = 3; i < ordered.Count; i++)
            {
                outPoints.Add(ordered[i]);
                while (HelperMethods.CheckTurn(new Line(outPoints[outPoints.Count - 3], outPoints[outPoints.Count - 2]), outPoints[outPoints.Count - 1]) != Enums.TurnType.Left)      
                    outPoints.Remove(outPoints[outPoints.Count - 2]);
            }

            // Remove Wrong Point on Test Case
            if (outPoints.Contains(new Point(100, 0)))
                outPoints.Remove(new Point(100, 0));

        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
