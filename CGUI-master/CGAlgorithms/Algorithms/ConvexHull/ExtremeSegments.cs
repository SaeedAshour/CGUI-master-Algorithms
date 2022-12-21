using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
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
            var isVisted = new bool[points.Count];
            var collinear = new List<Point>();
            for (int i = 0; i < points.Count; i++)
                isVisted[i] = false;



            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j) 
                        continue;
                    int right = 0, left = 0;
                    bool extreme = true;
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k == j || k == i) 
                            continue;

                        if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Right)
                            right++;
                        if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Left)
                            left++;
                        if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Colinear)
                            if (HelperMethods.PointOnSegment(points[k], points[i], points[j]))
                                collinear.Add(points[k]);
                               
                        if (left > 0 && right > 0)
                        {
                            extreme = false;
                            break;
                        }
                    }
                    if (extreme == true)
                    {
                        if (isVisted[i] == false)
                        {
                            isVisted[i] = true;
                            outPoints.Add(points[i]);
                        }

                        if (isVisted[j] == false)
                        {
                            isVisted[j] = true;
                            outPoints.Add(points[j]);
                        }
                    }
                }
            }
            for (int i = 0; i < collinear.Count; i++)
                outPoints.Remove(collinear[i]);
            
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
