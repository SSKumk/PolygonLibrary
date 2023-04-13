using System;
using System.Collections.Generic;
using System.Linq;

namespace GiftWrapping.Structures
{
    public class Edge:ICell
    {
        private readonly PlanePoint[] _points;
        public int Dimension => 1;
        public Hyperplane Hyperplane { get; set; }
        public IFace Parent { get; set; }
        public ICollection<PlanePoint> GetPoints()
        {
            return _points;
        }



        public Edge(PlanePoint p1, PlanePoint p2)
        {
            if (p1.Dim != p2.Dim) throw new ArgumentException("_points have different dimensions.");
            if (p1 == p2) throw new ArgumentException("Objects are equal.");
            _points = new[] {p1, p2};
        }

        public Edge()
        {
            _points = new PlanePoint[2];
        }
        public override int GetHashCode()
        {
            int sum = 0;
            foreach (PlanePoint point in _points)
            {
                int sum1 = point.GetHashCode();
                sum += sum1;
            }
            
            return sum;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Edge)obj);
        }

        public bool Equals(ICell obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return _points.All(((Edge)obj)._points.Contains);
        }

    }
}