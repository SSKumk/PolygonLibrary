using System;
using System.Collections.Generic;

namespace GiftWrapping.Structures
{
    public class PlanePoint:Point
    {
        public Point OriginalPoint { get; }

        private readonly PlanePoint _previousPoint;
      
        public PlanePoint GetPoint(int dimension)
        {
            if (dimension == Dim) return this;
            return _previousPoint.GetPoint(dimension) ??
                   throw new ArgumentException("There is no point of this dimension.");
        }

        public PlanePoint(double x, double y, double z) : base(new double[] { x, y, z })
        {
            OriginalPoint = new Point(new double[] {x, y, z});

        }
        public PlanePoint(int n, PlanePoint point) : base(n)
        {
            OriginalPoint = point.OriginalPoint;
            _previousPoint = point;

        }
        public PlanePoint(double[] np, PlanePoint point) : base(np)
        {
            OriginalPoint = point.OriginalPoint;
            _previousPoint = point;
        }

        public PlanePoint(Point p, PlanePoint point) : base(p)
        {
            OriginalPoint = point.OriginalPoint;
            _previousPoint = point;
        }

        public PlanePoint(int n) : base(n)
        {
            OriginalPoint = new Point(n);
        }
        public PlanePoint(double[] np) : base(np)
        {
            OriginalPoint = new Point(np);
        }

        public PlanePoint(Point p) : base(p)
        {
            OriginalPoint = new Point(p);
        }

        public PlanePoint(PlanePoint p) : base(p)
        {
            OriginalPoint = p.OriginalPoint;
            _previousPoint = p._previousPoint;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlanePoint)obj);
        }
        public bool Equals(PlanePoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return OriginalPoint.Equals(other.OriginalPoint);
        }

        public override int GetHashCode()
        {
            return OriginalPoint.GetHashCode();
        }
    }
}