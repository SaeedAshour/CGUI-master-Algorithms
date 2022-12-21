using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        // A Helper Orientation Method
        public static int Orientation(Point p, Point q, Point r)
        {
            int val = (int)((q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y));
            if (val == 0)
                return 0;      // colinear
            return (val > 0) ? 1 : 2;   // 1 for clock or 2 for counterclock wise
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Point Origin = new Point(0, 0);

            // In Case of 3 or less points
            if (points.Count < 4)
            {
                outPoints = points;
            }
            else
            {
                // Getting the Extream Left point
                int extreamLeft = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].X < points[extreamLeft].X)
                        extreamLeft = i;
                }

                // Initializing next and current as our points
                int next;
                int current = extreamLeft;
                do
                {
                    outPoints.Add(points[current]);
                    next = (current + 1) % points.Count;

                    // Finding the next point
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (Orientation(points[current], points[i], points[next]) == 2)
                        {
                            next = i;
                        }

                    }
                    // Setting the next point to be the current
                    current = next;

                    // Break if reach start
                } while (current != extreamLeft);


                // ask
                List<Point> Collinear = new List<Point>();

                for (int i = 0; i < outPoints.Count; i++)
                {
                    if (i == outPoints.Count - 2)
                        break;
                    if (HelperMethods.CheckTurn(new Line(outPoints[i], outPoints[i + 1]), outPoints[i + 2]) == Enums.TurnType.Colinear)
                        Collinear.Add(outPoints[i + 1]);
                }

                if (outPoints.Contains(Origin))
                    outPoints.Remove(Origin);

                for (int i = 0; i < Collinear.Count; i++)
                    outPoints.Remove(Collinear[i]);
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}