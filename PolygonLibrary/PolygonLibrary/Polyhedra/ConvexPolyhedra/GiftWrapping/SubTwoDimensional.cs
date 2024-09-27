namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents convex two-dimensional polygon in a d-dimensional space.
  /// </summary>
  internal sealed class SubTwoDimensional : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the polygon. It equals to 2.
    /// </summary>
    public override int PolytopDim => 2;

    /// <summary>
    /// Gets the type of the polygon. It is TwoDimensional.
    /// </summary>
    public override SubCPType Type => SubCPType.TwoDimensional;

    /// <summary>
    /// Gets the vertices of the polygon. It is used by drawing procedures.
    /// </summary>
    public override SortedSet<SubPoint> Vertices { get; }

    /// <summary>
    /// Gets the faces of the polygon.
    /// </summary>
    public override List<BaseSubCP>? Faces { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubTwoDimensional"/> class.
    /// </summary>
    /// <param name="Vs">The vertices list of the polygon.</param>
    public SubTwoDimensional(SortedSet<SubPoint> Vs) {
      Debug.Assert
        (Vs.Count > 2, $"SubTwoDimensional: At least three points must be used to construct a TwoDimensional! Found {Vs.Count}");

      Debug.Assert(Vs.First().SpaceDim == 2, $"SubTwoDimensional: The dimension of the points must be equal to 2!");

      // наши "плоские" алгоритмы не создают новые точки, то мы можем спокойно приводить типы.
      List<Vector2D>       convexPolygon2D = Convexification.GrahamHull(Vs.Select(s => new SubPoint2D(s)));
      List<SubPoint>       VsInOrder = convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList();
      List<BaseSubCP> faces = new List<BaseSubCP>() { new SubTwoDimensionalEdge(VsInOrder[^1], VsInOrder[0]) };
      for (int i = 0; i < VsInOrder.Count - 1; i++) {
        faces.Add(new SubTwoDimensionalEdge(VsInOrder[i], VsInOrder[i + 1]));
      }

      Vertices = VsInOrder.ToSortedSet();
      Faces    = faces;

      foreach (BaseSubCP face in faces) {
        face.SuperFaces.Add(this);
      }
    }

    private SubTwoDimensional(List<BaseSubCP> faces) {
      SortedSet<SubPoint> Vs = new SortedSet<SubPoint>();
      foreach (BaseSubCP face in faces) {
        Vs.UnionWith(face.Vertices);
        face.SuperFaces.Add(this);
      }

      Vertices = Vs;
      Faces    = faces;
    }

    /// <summary>
    /// Converts the polygon to the previous space.
    /// </summary>
    /// <returns>The converted polygon in the previous space.</returns>
    public override BaseSubCP ToPreviousSpace() {
      Debug.Assert(Faces is not null, $"SubTwoDimensional.ToPreviousSpace: Faces are null");

      return new SubTwoDimensional(new List<BaseSubCP>(Faces.Select(F => F.ToPreviousSpace())));
    }

    /// <summary>
    /// Projects the polygon to the specified affine basis.
    /// </summary>
    /// <param name="affBasis">The affine basis to project to.</param>
    /// <returns>The projected polygon.</returns>
    public override BaseSubCP ProjectTo(AffineBasis affBasis) {
      Debug.Assert(Faces is not null, $"SubTwoDimensional.ProjectTo: Faces are null");

      return new SubTwoDimensional(new List<BaseSubCP>(Faces.Select(F => F.ProjectTo(affBasis))));
    }

  }

}
