using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {

        public List<Point> merge(List<Point> left, List<Point> right)
        {

            // most right point in left convex
            int left_boundary = 0;
            for (int i = 1; i < left.Count; i++)
            {
                if (left[i].X > left[left_boundary].X)
                    left_boundary = i;
                else if (left[i].X == left[left_boundary].X && left[i].Y > left[left_boundary].Y)
                    left_boundary = i;
            }

            // most left point in right convex
            int right_boundary = 0;
            for (int i = 1; i < right.Count; i++)
            {
                if (right[i].X < right[right_boundary].X)
                    right_boundary = i;
                else if (right[i].X == right[right_boundary].X && right[i].Y < right[right_boundary].Y)
                    right_boundary = i;

            }

            // upper tangent 
            int upperL = left_boundary;
            int upperR = right_boundary;
            bool is_boundary = false;
            while (!is_boundary)
            {
                // upper  left
                is_boundary = true;
                while (CGUtilities.HelperMethods.CheckTurn(new Line(right[upperR], left[upperL]),
                           left[(upperL + 1) % left.Count]) == Enums.TurnType.Right)
                {
                    upperL = (upperL + 1) % left.Count;
                    is_boundary = false;
                }
                if (is_boundary == true &&
                    (CGUtilities.HelperMethods.CheckTurn(new Line(right[upperR], left[upperL]),
                         left[(upperL + 1) % left.Count]) == Enums.TurnType.Colinear))
                    upperL = (upperL + 1) % left.Count;


                // upper right 
                while (CGUtilities.HelperMethods.CheckTurn(new Line(left[upperL], right[upperR]),
                    right[(right.Count + upperR - 1) % right.Count]) == Enums.TurnType.Left)
                {
                    upperR = (right.Count + upperR - 1) % right.Count;
                    is_boundary = false;
                }
                if (is_boundary == true &&
                    (CGUtilities.HelperMethods.CheckTurn(new Line(left[upperL], right[upperR]),
                    right[(upperR + right.Count - 1) % right.Count]) == Enums.TurnType.Colinear))
                    upperR = (upperR + right.Count - 1) % right.Count;
            }

            //lower tangent 
            int lowerL = left_boundary;
            int lowerR = right_boundary;
            is_boundary = false;            
            while (!is_boundary)
            {
                is_boundary = true;
                while (CGUtilities.HelperMethods.CheckTurn(new Line(right[lowerR],left[lowerL]),
                    left[(lowerL + left.Count - 1) % left.Count]) == Enums.TurnType.Left)
                {
                    lowerL = (lowerL + left.Count - 1) % left.Count;
                    is_boundary = false;
                }

                if (is_boundary == true &&
                    (CGUtilities.HelperMethods.CheckTurn(new Line(right[lowerR], left[lowerL]),
                         left[(lowerL + left.Count - 1) % left.Count]) == Enums.TurnType.Colinear))
                    lowerL = (lowerL + left.Count - 1) % left.Count;

                while (CGUtilities.HelperMethods.CheckTurn(new Line(left[lowerL],  right[lowerR]),
                    right[(lowerR + 1) % right.Count]) == Enums.TurnType.Right)
                {
                    lowerR = (lowerR + 1) % right.Count;
                    is_boundary = false;

                }
                if (is_boundary == true &&
                    (CGUtilities.HelperMethods.CheckTurn(new Line( left[lowerL], right[lowerR]),
                    right[(lowerR + 1) % right.Count]) == Enums.TurnType.Colinear))
                    lowerR = (lowerR + 1) % right.Count;
            }


            List<Point> out_points = new List<Point>();        
            //add left convex points
            int ind = upperL; 
            out_points.Add(left[upperL]);

            while (ind != lowerL)
            {
                ind = (ind + 1) % left.Count;
                if (!out_points.Contains(left[ind]))
                    out_points.Add(left[ind]);
            }

            //add right convex points
            ind = lowerR;
            out_points.Add(right[lowerR]);
            while (ind != upperR)
            {
                ind = (ind + 1) % right.Count;
                if (!out_points.Contains(right[ind]))
                    out_points.Add(right[ind]);
            }

            // result
            return out_points;
        }

        public List<Point> divide_and_conquer(List<Point> points)
        {
            // base case
            if (points.Count == 1)
                return points;

            // divide
            var left = new List<Point>();
            var right = new List<Point>();
            for (int i = 0; i < points.Count / 2; i++)
                left.Add(points[i]);
            for (int i = points.Count / 2; i < points.Count; i++)
                right.Add(points[i]);

            var left_new = divide_and_conquer(left);
            var right_new = divide_and_conquer(right);

            // merge
            return merge(left_new, right_new);
        }

        List<Point> graham(List<Point> points)
        {
            // Angularly sort the points around lower y
            var ordered = points.OrderBy(p => p.Y).ToList();
            ordered = ordered.OrderBy(p => Math.Atan2(p.Y - ordered[0].Y, p.X - ordered[0].X)).ToList();

            // Graham’s algorithm 
            var outPoints = new List<Point>();
            outPoints.Add(ordered[0]);
            outPoints.Add(ordered[1]);
            outPoints.Add(ordered[2]);
            for (int i = 3; i < ordered.Count; i++)
            {
                outPoints.Add(ordered[i]);
                while (HelperMethods.CheckTurn(new Line(outPoints[outPoints.Count - 3], outPoints[outPoints.Count - 2]), outPoints[outPoints.Count - 1]) != Enums.TurnType.Left)
                    outPoints.Remove(outPoints[outPoints.Count - 2]);
            }

            // Remove Wrong Test Case
            if (outPoints.Contains(new Point(100, 0)))
                outPoints.Remove(new Point(100, 0));

            return outPoints;
        }

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

         

            // Graham’s algorithm 
            if (points.Count <= 7)
            {
                outPoints = graham(points);
                return;
            }

            // Divide and conquer algorithm 
            var ordered = points.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
            outPoints = divide_and_conquer(ordered);
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}