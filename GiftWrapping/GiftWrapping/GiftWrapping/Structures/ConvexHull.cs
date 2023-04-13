using System.Collections.Generic;
using System.Linq;

namespace GiftWrapping.Structures
{
    public class ConvexHull : IFace
    {
        public int Dimension { get; }
        public List<ICell> AdjacentCells { get; }
        public IFace Parent { get; set; }
        public List<ICell> InnerCells { get; }
        public void AddAdjacentCell(ICell cell) => AdjacentCells.Add(cell);

        public void AddInnerCell(ICell cell)
        {
            cell.Parent = this;
            InnerCells.Add(cell);
        } 
        public Hyperplane Hyperplane { get; set; }
 
        public ConvexHull(int dimension)
        {
            Dimension = dimension;
            InnerCells = new List<ICell>();
            AdjacentCells = new List<ICell>();
        }

        public ConvexHull(Hyperplane hyperplane)
        {
            Hyperplane = hyperplane;
            Dimension = hyperplane.Dimension;
            InnerCells = new List<ICell>();
            AdjacentCells = new List<ICell>();
        }
        public ICollection<PlanePoint> GetPoints()
        {
            HashSet<PlanePoint> points = new HashSet<PlanePoint>();
            foreach (ICell innerFace in InnerCells)
            {
                points.UnionWith(innerFace.GetPoints());
            }

            return points;
        }

        

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IFace) obj);
        }

        public override int GetHashCode()
        {
            int res = 0;
            foreach (ICell cell in InnerCells)
                res += cell.GetHashCode();
            
            res += Dimension.GetHashCode();
            return res;
      
        }

        public bool Equals(ICell other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            ConvexHull convexHull = (ConvexHull)other;
            return Dimension == other.Dimension &&
                   InnerCells.Count == convexHull.InnerCells.Count &&
                   GetPoints().All(other.GetPoints().Contains);
        }
    }
}