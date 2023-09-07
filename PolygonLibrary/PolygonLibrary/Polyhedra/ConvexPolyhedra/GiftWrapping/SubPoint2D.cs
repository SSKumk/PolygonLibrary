

namespace CGLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping; 

/// <summary>
/// Auxiliary class that represents the two-dimensional points in 2D-space
/// and holds the information about sub-point from this forms. 
/// </summary>
public class SubPoint2D : Point2D {

   /// <summary>
   /// sub-point based on which current 
   /// </summary>
   public SubPoint SubPoint { get; }

   /// <summary>
   /// Construct a aux point, based on given sub-point, which dimension should equals to 2. 
   /// </summary>
   /// <param name="subPoint"></param>
   public SubPoint2D(SubPoint subPoint) : base(subPoint[0], subPoint[1])  { SubPoint = subPoint; }

}
