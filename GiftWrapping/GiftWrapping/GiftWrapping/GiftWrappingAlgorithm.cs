using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Transactions;
using GiftWrapping.Helpers;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;

namespace GiftWrapping
{
    public class GiftWrappingAlgorithm
    {
        private readonly IList<PlanePoint> _points;
        private readonly PlaneFinder _planeFinder;
        private readonly IAlgorithm _algorithm2d;
        public GiftWrappingAlgorithm(IList<PlanePoint> points)
        {
            if (points.Count < 3)
            {
                throw new ArgumentException("The number of _points must be more than three.");
            }
            _planeFinder = new PlaneFinder();
            _algorithm2d = new GiftWrapping2d();
            _points = points;
        }

        public IFace Create()
        {
            return FindConvexHull(_points);
        }

        private IFace FindConvexHullvK(IList<PlanePoint> points)
        {
            int dim = points[0].Dim;
            if (dim == 2)
            {
                return _algorithm2d.FindConvexHull(points);
            }
            if (points.Count == dim + 1)
            {
                return CreateSimplex(points);
            }
            ConvexHull convexHull = new ConvexHull(dim);
            Queue<(IFace, bool[])> unprocessedFaces = new Queue<(IFace, bool[])>();
            Dictionary<Hyperplane, ICell> processedCells = new Dictionary<Hyperplane, ICell>(new VectorComparer());
            Hyperplane currentHyperplane = _planeFinder.FindFirstPlane(points);
            List<PlanePoint> planePoints = new List<PlanePoint>();
            bool[] processedPoints = new bool[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                if (!currentHyperplane.IsPointInPlane(points[i])) continue;
                processedPoints[i] = true;
                planePoints.Add(currentHyperplane.ConvertPoint(points[i]));
            }
 
            IFace currentHull = FindConvexHull(planePoints);
          
            if (planePoints.Count == points.Count)
            {
                return currentHull;
            }
            currentHull.Hyperplane = currentHyperplane;
            unprocessedFaces.Enqueue((currentHull, processedPoints));
            convexHull.AddInnerCell(currentHull);
            processedCells.Add(currentHull.Hyperplane, currentHull);
            List<int> hasList = new List<int>();
            VectorComparer rere = new VectorComparer();
            while (unprocessedFaces.Any())
            {
                (currentHull, processedPoints) = unprocessedFaces.Dequeue();
                IEnumerable<PlanePoint> innerPoints = currentHull.GetPoints();
                //currentHull.InnerCells.Select((cell => cell.Hyperplane.MainPoint));
                foreach (ICell cell in currentHull.InnerCells)
                {
                 
                    double maxCos = double.MinValue;
                    Hyperplane nextHyperplane = currentHyperplane;
                    foreach (Hyperplane hyperplane in GetHyperplanes(cell, currentHull, processedPoints, points))
                    {
                        try
                        {
                            hyperplane.SetOrientationNormal(innerPoints);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        double newCos = currentHull.Hyperplane.Cos(hyperplane);
                        if (Tools.GT(newCos, maxCos))
                        {
                            maxCos = newCos;
                            nextHyperplane = hyperplane;
                        }
                    }
                    hasList.Add(rere.GetHashCode(nextHyperplane));
                    ICell adjacentCell = processedCells.GetValueOrDefault(nextHyperplane);
                    if (adjacentCell is { })
                    {
                        currentHull.AddAdjacentCell(adjacentCell);
                        continue;
                    }

                    if (convexHull.InnerCells.Count == 5)
                    {

                    }
                    planePoints.Clear();
                    bool[] newPointMap = new bool[points.Count];
                    for (int j = 0; j < points.Count; j++)
                    {
                        if (!nextHyperplane.IsPointInPlane(points[j])) continue;
                        newPointMap[j] = true;
                        planePoints.Add(nextHyperplane.ConvertPoint(points[j]));
                    }
                    IFace newHull = FindConvexHull(planePoints);
                    newHull.Hyperplane = nextHyperplane;
                    convexHull.AddInnerCell(newHull);
                    currentHull.AddAdjacentCell(newHull);
                    processedCells.Add(nextHyperplane, newHull);
                    unprocessedFaces.Enqueue((newHull, newPointMap));
                }
            }
            return convexHull;
        }

        private IFace FindConvexHull(IList<PlanePoint> points)
        {
            int dim = points[0].Dim;
            if (dim == 2)
            {
                return _algorithm2d.FindConvexHull(points);
            }
            if (points.Count == dim + 1)
            {
                return CreateSimplex(points);
            }
            ConvexHull convexHull = new ConvexHull(dim);
            Queue<(IFace, bool[])> unprocessedFaces = new Queue<(IFace, bool[])>();
            Dictionary<Hyperplane, ICell> processedCells = new Dictionary<Hyperplane, ICell>(new VectorComparer());
            Hyperplane currentHyperplane = _planeFinder.FindFirstPlane(points);
            List<PlanePoint> planePoints = new List<PlanePoint>();
            bool[] processedPoints = new bool[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                if (!currentHyperplane.IsPointInPlane(points[i])) continue;
                processedPoints[i] = true;
                planePoints.Add(currentHyperplane.ConvertPoint(points[i]));
            }

            IFace currentHull = FindConvexHull(planePoints);

            if (planePoints.Count == points.Count)
            {
                return currentHull;
            }
            currentHull.Hyperplane = currentHyperplane;
            unprocessedFaces.Enqueue((currentHull, processedPoints));
            convexHull.AddInnerCell(currentHull);
            processedCells.Add(currentHull.Hyperplane, currentHull);
            List<int> hasList = new List<int>();
            VectorComparer rere = new VectorComparer();
            while (unprocessedFaces.Any())
            {
                (currentHull, processedPoints) = unprocessedFaces.Dequeue();
                IEnumerable<PlanePoint> innerPoints = currentHull.GetPoints();
                //currentHull.InnerCells.Select((cell => cell.Hyperplane.MainPoint));
                foreach (ICell cell in currentHull.InnerCells)
                {

                    double maxCos = double.MinValue;
                    Hyperplane nextHyperplane = currentHyperplane;
                    foreach (Hyperplane hyperplane in GetHyperplanes(cell, currentHull, processedPoints, points))
                    {
                        try
                        {
                            hyperplane.SetOrientationNormal(innerPoints);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        double newCos = currentHull.Hyperplane.Cos(hyperplane);
                        if (Tools.GT(newCos, maxCos))
                        {
                            maxCos = newCos;
                            nextHyperplane = hyperplane;
                        }
                    }
                    hasList.Add(rere.GetHashCode(nextHyperplane));
                    ICell adjacentCell = processedCells.GetValueOrDefault(nextHyperplane);
                    if (adjacentCell is { })
                    {
                        currentHull.AddAdjacentCell(adjacentCell);
                        continue;
                    }

                    if (convexHull.InnerCells.Count == 5)
                    {

                    }
                    planePoints.Clear();
                    bool[] newPointMap = new bool[points.Count];
                    for (int j = 0; j < points.Count; j++)
                    {
                        if (!nextHyperplane.IsPointInPlane(points[j])) continue;
                        newPointMap[j] = true;
                        planePoints.Add(nextHyperplane.ConvertPoint(points[j]));
                    }
                    IFace newHull = FindConvexHull(planePoints);
                    newHull.Hyperplane = nextHyperplane;
                    convexHull.AddInnerCell(newHull);
                    currentHull.AddAdjacentCell(newHull);
                    processedCells.Add(nextHyperplane, newHull);
                    unprocessedFaces.Enqueue((newHull, newPointMap));
                }
            }
            return convexHull;
        }

        //private IFace GGFindConvexHull(IList<PlanePoint> points)
        //{
        //    int dim = points[0].Dim;
        //    if (dim == 2)
        //    {
        //        return _algorithm2d.FindConvexHull(points);
        //    }
        //    if (points.Count == dim + 1)
        //    {
        //        return CreateSimplex(points);
        //    }

        //    Dictionary<ICell, int> edges = new Dictionary<ICell, int>();
        //    List<bool[]> pointMaps = new List<bool[]>();
        //    List<ICell> unprocessedEdges = new List<ICell>();
        //    Hyperplane currentHyperplane = _planeFinder.FindFirstPlane(points);
          
        //    List<PlanePoint> planePoints = new List<PlanePoint>();
        //    bool[] processedPoints = new bool[points.Count];
        //    for (int i = 0; i < points.Count; i++)
        //    {
        //        if ( i == 7 || i == 17 || i == 23)
        //        {

        //        }
        //        if (!currentHyperplane.IsPointInPlane(points[i])) continue;
        //        processedPoints[i] = true;
        //        planePoints.Add(currentHyperplane.ConvertPoint(points[i]));
        //    }

        //    IFace currentHull = FindConvexHull(planePoints);
        //    currentHull.Hyperplane = currentHyperplane;
        //    if (planePoints.Count == points.Count)
        //    {
        //        return currentHull;
        //    }

        //    ConvexHull convexHull = new ConvexHull(dim);
        //    convexHull.AddInnerCell(currentHull);
        //    foreach (ICell value in currentHull.InnerCells)
        //    {
        //        pointMaps.Add(processedPoints);
        //        unprocessedEdges.Add(value);
        //        edges.Add(value, unprocessedEdges.Count - 1);
          
        //    }

        //    for (int i = 0; i < unprocessedEdges.Count; i++)
        //    {
        //        ICell cell = unprocessedEdges[i];
        //        processedPoints = pointMaps[i];
        //        if (ReferenceEquals(cell, null)) continue;
        //        IEnumerable<PlanePoint> innerPoints = cell.Parent.GetPoints();
        //        double maxCos = double.MinValue;
        //        Hyperplane nextHyperplane = currentHyperplane;
        //        int len = cell.Hyperplane.Basis.Length;
        //        Vector[] basis = new Vector[len + 1];
        //        Array.Copy(cell.Hyperplane.Basis, 0, basis, 0, len);
        //        for (int j = 0; j < basis.Length - 1; j++)
        //        {
        //            basis[j] = cell.Parent.Hyperplane.ConvertVector(basis[j]);
        //        }
        //        PlanePoint p = cell.Hyperplane.MainPoint.GetPoint(cell.Parent.Dimension + 1);
        //        planePoints.Clear();
        //        bool[] newPointMap = new bool[points.Count];
        //        for (int j = 0; j < points.Count; j++)
        //        {
        //            if (processedPoints[j]) continue;
        //            basis[^1] = Point.ToVector(p, points[j]);
        //            Hyperplane newHyperplane = default;
        //            try
        //            {
        //                newHyperplane = HyperplaneHelper.Create(p, basis);
        //            }
        //            catch
        //            {
        //               continue;
        //            }
        //            newHyperplane.SetOrientationNormal(innerPoints);
        //            double newCos = cell.Parent.Hyperplane.Cos(newHyperplane);
        //            if (Tools.GT(newCos, maxCos))
        //            {
        //                maxCos = newCos;
        //                nextHyperplane = newHyperplane;
        //            }

        //        }
        //        for (int j = 0; j < points.Count; j++)
        //        {
        //            if (!nextHyperplane.IsPointInPlane(points[j])) continue;
        //            newPointMap[j] = true;
        //            planePoints.Add(nextHyperplane.ConvertPoint(points[j]));
        //        }

        //        IFace newHull = FindConvexHull(planePoints);
        //        newHull.Hyperplane = nextHyperplane;
        //        convexHull.AddInnerCell(newHull);
        //        foreach (ICell c in newHull.InnerCells)
        //        {
        //            if (edges.TryGetValue(c, out int adj) && adj >= 0)
        //            {
        //                UniteAdjacentCells(newHull, unprocessedEdges[adj].Parent);
        //                unprocessedEdges[adj] = default;
        //                edges[c] = -1;
        //                continue;
        //            }
        //            unprocessedEdges.Add(c);
        //            pointMaps.Add(newPointMap);
        //            edges[c] = unprocessedEdges.Count - 1;
        //        }
        //    }
        //    return convexHull;
        //}

        private Vector FindNormal(Vector[] basis)
        {
            Matrix leftSide = basis.ToHorizontalMatrix();
            double[] rightSide = new double[basis.Length];

            return GaussWithChoiceSolveSystem.FindAnswer(leftSide, rightSide);
        }

        private IEnumerable<Hyperplane> GetHyperplanes(ICell cell, ICell parent, bool[] processedPoints, IList<PlanePoint> points)
        {
            Hyperplane hyperplane = parent.Hyperplane;
            int len = cell.Hyperplane.Basis.Length;
            Vector[] basis = new Vector[len+1];
            Array.Copy(cell.Hyperplane.Basis,0,basis,0,len);
            for (int i = 0; i < basis.Length-1; i++)
            {
                basis[i] = hyperplane.ConvertVector(basis[i]);
            }
            PlanePoint p = cell.Hyperplane.MainPoint.GetPoint(parent.Dimension+1);
            for (int i = 0; i < points.Count; i++)
            {
                if (processedPoints[i]) continue;
                PlanePoint point = points[i];
                basis[^1] = Point.ToVector(p, point);
                Hyperplane newHyperplane =
                    HyperplaneBuilder.Create(p, basis);

                yield return newHyperplane;
            }
        }

        //private void UniteAdjacentCells(IFace c1, IFace c2)
        //{
        //    c1.AddAdjacentCell(c2);
        //    c2.AddAdjacentCell(c1);
        //}

        private IFace CreateSimplex(IList<PlanePoint> points)
        {
            int dim = points[0].Dim;
            if (dim == 2)
            {
                return _algorithm2d.FindConvexHull(points);
            }
            ConvexHull convexHull = new ConvexHull(dim);
            for (int i = 0; i < points.Count; i++)
            {
                List<PlanePoint> planePoints = points.Where((_, j) => j != i).ToList();
                Hyperplane hyperplane = HyperplaneBuilder.Create(planePoints);
                List<PlanePoint> convertPoints = 
                    planePoints.Select((point => hyperplane.ConvertPoint(point))).ToList();
                IFace face = CreateSimplex(convertPoints);
                face.Hyperplane = hyperplane;
                foreach (ICell f in convexHull.InnerCells)
                {
                    face.AddAdjacentCell((IFace)f);
                    ((IFace)f).AddAdjacentCell(face);
                }

                convexHull.AddInnerCell(face);
            }

            return convexHull;
        }
    }
}