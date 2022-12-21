using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        private List<Point> Get_NSEW_points(List<Point> points)
        {
            Point west = (Point)points[0].Clone();
            Point east = (Point)points[0].Clone();
            Point north = (Point)points[0].Clone();
            Point south = (Point)points[0].Clone();


            for (int i = 1; i < points.Count(); i++)
            {
                if (points[i].X < west.X)
                {
                    west.X = points[i].X;
                    west.Y = points[i].Y;
                }
                if (points[i].X > east.X)
                {
                    east.X = points[i].X;
                    east.Y = points[i].Y;
                }
                if (points[i].Y < south.Y)
                {
                    south.X = points[i].X;
                    south.Y = points[i].Y;
                }
                if (points[i].Y > north.Y)
                {
                    north.X = points[i].X;
                    north.Y = points[i].Y;
                }
            }
            return new List<Point> { south, east, north, west };
        }
        private void Check_within_polygon(List<Point> polygon, Point point, ref List<List<Point>> sec_cord_points)
        {
            for (int i = 0; i < polygon.Count() - 1; i++)
            {
                if (i == polygon.Count() - 2)
                {
                    if (HelperMethods.CheckTurn(new Line(polygon[i], polygon[i + 1]), point) != HelperMethods.CheckTurn(new Line(polygon[i + 1], polygon[0]), point))
                    {
                        if (HelperMethods.CheckTurn(new Line(polygon[i], polygon[i + 1]), point) == Enums.TurnType.Right)

                        {
                            if (sec_cord_points.Count() != 0)
                                if (!sec_cord_points[i].Contains(point))
                                    sec_cord_points[i].Add(point);
                        }

                        else if (HelperMethods.CheckTurn(new Line(polygon[i + 1], polygon[0]), point) == Enums.TurnType.Right)
                            if (!sec_cord_points[i + 1].Contains(point))
                                sec_cord_points[i + 1].Add(point);

                        break;
                    }
                }
                else
                {
                    if (HelperMethods.CheckTurn(new Line(polygon[i], polygon[i + 1]), point) != HelperMethods.CheckTurn(new Line(polygon[i + 1], polygon[i + 2]), point))
                    {
                        if (HelperMethods.CheckTurn(new Line(polygon[i], polygon[i + 1]), point) == Enums.TurnType.Right)
                        {
                            if (!sec_cord_points[i].Contains(point))
                                sec_cord_points[i].Add(point);
                        }

                        else if (HelperMethods.CheckTurn(new Line(polygon[i + 1], polygon[i + 2]), point) == Enums.TurnType.Right)
                            if (!sec_cord_points[i + 1].Contains(point))
                                sec_cord_points[i + 1].Add(point);

                        break;
                    }


                }
            }
            return;
        }
        private Point Get_intersection_point(double slope, double slope_inv, Point p, Point p_inv)
        {
            double x = (slope_inv * p_inv.X - p_inv.Y - slope * p.X + p.Y) / (slope_inv - slope);
            double y = slope_inv * (x - p_inv.X) + p_inv.Y;
            return new Point(x, y);
        }
        private Point Get_the_furthest_point(Line line, List<Point> points)
        {
            double max_dest = double.NegativeInfinity;
            Point furthest_point = null;
            foreach (Point point in points)
            {
                double dest = Get_distance(line, point);
                if (dest > max_dest)
                {
                    max_dest = dest;
                    furthest_point = point;
                }
            }
            if (max_dest != 0)
                return furthest_point;
            else
                return null;
        }
        private double Get_distance(Line l, Point p)
        {
            double slope = (l.End.Y - l.Start.Y) / (l.End.X - l.Start.X);
            double inv_slope = -1 * 1 / slope;

            Point intersection_point = Get_intersection_point(slope, inv_slope, l.Start, p);

            return Math.Sqrt(Math.Pow(p.X - intersection_point.X, 2) + Math.Pow(p.Y - intersection_point.Y, 2));
        }
        private List<Point> Get_remaining_points(List<List<Point>> sec_cord_point, List<Point> main_cord_points)
        {
            List<Point> outpoints = new List<Point>();
            Point furthest_point;
            List<List<Point>> new_sec_cord_points = new List<List<Point>>();
            for (int i = 0; i < 2; i++)
                new_sec_cord_points.Add(new List<Point>());

            bool not_end = false;
            foreach (List<Point> region in sec_cord_point)
            {
                if (region.Count() != 0)
                {
                    not_end = true;
                    break;
                }
            }
            if (!not_end)
                return new List<Point>();

            for (int i = 0; i < sec_cord_point.Count(); i++)
            {
                if (i < sec_cord_point.Count() - 1)
                {
                    furthest_point = Get_the_furthest_point(new Line(main_cord_points[i], main_cord_points[i + 1]), sec_cord_point[i]);
                    if (furthest_point != null)
                        outpoints.Add(furthest_point);
                    for (int j = 0; j < sec_cord_point[i].Count(); j++)
                    {
                        if (HelperMethods.PointInTriangle(sec_cord_point[i][j], main_cord_points[i], main_cord_points[i + 1], furthest_point) == Enums.PointInPolygon.Outside)
                        {
                            if (HelperMethods.CheckTurn(new Line(main_cord_points[i], furthest_point), sec_cord_point[i][j]) == Enums.TurnType.Left)
                                new_sec_cord_points[0].Add(sec_cord_point[i][j]);
                            else
                                new_sec_cord_points[1].Add(sec_cord_point[i][j]);

                        }
                    }
                    outpoints.AddRange(Get_remaining_points(new_sec_cord_points, new List<Point> { furthest_point, main_cord_points[i + 1], main_cord_points[i] }));

                }
                else
                {
                    furthest_point = Get_the_furthest_point(new Line(main_cord_points[i], main_cord_points[0]), sec_cord_point[i]);
                    if (furthest_point != null)
                        outpoints.Add(furthest_point);
                    for (int j = 0; j < sec_cord_point[i].Count(); j++)
                    {
                        if (HelperMethods.PointInTriangle(sec_cord_point[i][j], main_cord_points[i], main_cord_points[0], furthest_point) == Enums.PointInPolygon.Outside)
                            if (HelperMethods.CheckTurn(new Line(main_cord_points[i], furthest_point), sec_cord_point[i][j]) == Enums.TurnType.Left)
                                new_sec_cord_points[0].Add(sec_cord_point[i][j]);
                            else if (HelperMethods.CheckTurn(new Line(main_cord_points[i], furthest_point), sec_cord_point[i][j]) == Enums.TurnType.Right)
                                new_sec_cord_points[1].Add(sec_cord_point[i][j]);

                    }
                    outpoints.AddRange(Get_remaining_points(new_sec_cord_points, new List<Point> { furthest_point, main_cord_points[0], main_cord_points[i] }));
                }
            }

            return outpoints;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //Get the Unique points
            List<Point> unique_points = HelperMethods.Get_Distinct(points);

            //Calculate the North, South, East, West points
            List<Point> main_cord_points = Get_NSEW_points(unique_points);
            main_cord_points = HelperMethods.Get_Distinct(main_cord_points);
            outPoints.AddRange(main_cord_points);

            List<List<Point>> sec_cord_points = new List<List<Point>>();
            for (int i = 0; i < main_cord_points.Count(); i++)
                sec_cord_points.Add(new List<Point>());

            foreach (Point p in main_cord_points)
                unique_points.Remove(p);

            foreach (Point p in unique_points)
                Check_within_polygon(main_cord_points, p, ref sec_cord_points);


            outPoints.AddRange(Get_remaining_points(sec_cord_points, main_cord_points));
        }
        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }

    }
}