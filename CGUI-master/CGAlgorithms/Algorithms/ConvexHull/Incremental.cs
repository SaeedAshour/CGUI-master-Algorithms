using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
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

            // Incremental algorithm
            points = points.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
            int[] next = new int[points.Count];
            int[] previous = new int[points.Count];
            next[0] = 1;
            previous[0] = 1;
            // 0 --- 1

            for (int i = 2; i < points.Count; i++)
            {
                if (points[i].Y >= points[i-1].Y)
                {
                    next[i] = next[i-1]; 
                    previous[i] = i-1; 
                }
                else
                {
                    next[i] = i-1; 
                    previous[i] = previous[i-1];
                }

                next[previous[i]] = i;
                previous[next[i]] = i;

                // support line
                while (HelperMethods.CheckTurn(new Line(points[i], points[next[i]]), points[next[next[i]]]) != Enums.TurnType.Left)
                {
                    next[i] = next[next[i]];
                    previous[next[i]] = i;
                }

                while (HelperMethods.CheckTurn(new Line(points[i], points[previous[i]]), points[previous[previous[i]]]) != Enums.TurnType.Right)
                {
                    previous[i] = previous[previous[i]];
                    next[previous[i]] = i;
                }
            }

            int iterator = 0;
            while (true)
            {
                outPoints.Add(points[iterator]);
                iterator = next[iterator];
                if (iterator == 0)
                    break;
            }
            return;
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}