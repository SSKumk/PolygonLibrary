namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a one-dimensional edge of a convex polytope expressed in 2D-coordinates.
  /// </summary>
  internal sealed class SubTwoDimensionalEdge : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the edge. It equals to 1.
    /// </summary>
    public override int PolytopDim => 1;

    /// <summary>
    /// Gets the type of the edge. It is OneDimensional.
    /// </summary>
    public override SubCPType Type => SubCPType.OneDimensional;

    /// <summary>
    /// Gets the 2D-vertices of the edge.
    /// </summary>
    public override SortedSet<SubPoint> Vertices { get; }

    /// <summary>
    /// Gets the faces of the 2d-edge.
    /// </summary>
    public override List<BaseSubCP>? Faces { get; }

    private SubZeroDimensional first  => (SubZeroDimensional)Faces![0];
    private SubZeroDimensional second => (SubZeroDimensional)Faces![1];

    /// <summary>
    /// Initializes a new instance of the <see cref="SubTwoDimensionalEdge"/> class.
    /// </summary>
    /// <param name="first">The first vertex of the edge.</param>
    /// <param name="second">The second vertex of the edge.</param>
    public SubTwoDimensionalEdge(SubPoint first, SubPoint second) {
      SortedSet<SubPoint> vertices = new SortedSet<SubPoint>() { first, second };
      Vertices = vertices;

      Faces = new List<BaseSubCP>() { new SubZeroDimensional(first), new SubZeroDimensional(second) };
      Faces[0].SuperFaces.Add(this);
      Faces[1].SuperFaces.Add(this);
    }

    /// <summary>
    /// Converts the edge to the previous space.
    /// </summary>
    /// <returns>The converted edge in the previous space.</returns>
    public override BaseSubCP ToPreviousSpace()
      => new SubTwoDimensionalEdge(first.ToPreviousSpace().Vertices.First(), second.ToPreviousSpace().Vertices.Last());

    /// <summary>
    /// Projects the edge to the specified affine basis.
    /// </summary>
    /// <param name="affBasis">The affine basis to project to.</param>
    /// <returns>The projected edge.</returns>
    public override BaseSubCP ProjectTo(AffineBasis affBasis)
      => new SubTwoDimensionalEdge(first.ProjectTo(affBasis).Vertices.First(), second.ProjectTo(affBasis).Vertices.Last());

  }

}
