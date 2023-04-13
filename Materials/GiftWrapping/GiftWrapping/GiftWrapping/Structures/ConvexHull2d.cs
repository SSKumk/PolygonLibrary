using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using GiftWrapping.Helpers;

namespace GiftWrapping.Structures
{
    public class ConvexHull2d:IFace 
    {
        private readonly List<PlanePoint> _points;
        public int Dimension => 2;
        public List<ICell> AdjacentCells { get; }
        public List<ICell> InnerCells { get; }
        public Hyperplane Hyperplane { get; set; }
        public IFace Parent { get; set; }
        public ConvexHull2d(IEnumerable<PlanePoint> points)
        {
            _points = new List<PlanePoint>(points);
            AdjacentCells = new List<ICell>();
            InnerCells = new List<ICell>(_points.Count);
            
            ComputeData();
        }
        private void ComputeData()
        {
            Edge edge = new Edge(_points[^1], _points[0]);
            edge.Hyperplane = HyperplaneBuilder.Create(edge.GetPoints().ToArray());
           // edge.Hyperplane.OrthonormalBasis();
            edge.Hyperplane.SetOrientationNormal(_points);
            //InnerCells.Add(edge);
            AddInnerCell(edge);
            for (int i = 0; i < _points.Count - 1; i++)
            {
                edge = new Edge(_points[i], _points[i + 1]);
                edge.Hyperplane = HyperplaneBuilder.Create(edge.GetPoints().ToList());
             //   edge.Hyperplane.OrthonormalBasis();
                edge.Hyperplane.SetOrientationNormal(_points);
               // InnerCells.Add(edge);
                AddInnerCell(edge);
            }
       

        }

        private void AddInnerCell(ICell cell)
        {
            cell.Parent = this;
            InnerCells.Add(cell);
        }

        public void AddAdjacentCell(ICell cell)
        {
            if (cell.Dimension == Dimension)
            {
                AdjacentCells.Add(cell);
            }
        }

        public ICollection<PlanePoint> GetPoints()
        {
            return _points;
        }

        public bool Equals(ICell other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            ConvexHull2d convexHull = (ConvexHull2d) other;
            return Dimension == other.Dimension &&
                   _points.Count == convexHull._points.Count &&
                   _points.All(convexHull._points.Contains);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConvexHull2d)obj);
        }

        public override int GetHashCode()
        {
            int res = 0;
            foreach (PlanePoint point in _points)
                res += point.GetHashCode();
            res += Dimension.GetHashCode();
            return res;
 
        }
    }
}