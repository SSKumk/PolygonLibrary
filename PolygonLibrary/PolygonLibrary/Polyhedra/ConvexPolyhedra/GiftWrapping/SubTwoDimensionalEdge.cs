namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a one-dimensional edge of a convex polytope expressed in 2D-coordinates.
  /// </summary>
  public class SubTwoDimensionalEdge : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the edge. It equal to 1.
    /// </summary>
    public override int PolytopDim => 1;

    /// <summary>
    /// Gets the type of the edge. It is OneDimensional.
    /// </summary>
    public override SubCPType Type => SubCPType.OneDimensional;

    /// <summary>
    /// Gets the 2D-vertices of the edge.
    /// </summary>
    public override HashSet<SubPoint> Vertices { get; }

    private SubPoint first { get; }
    private SubPoint second { get; }

    /// <summary>
    /// There are no Faces of the 1-dimensional edge.
    /// </summary>
    public override HashSet<BaseSubCP>? Faces => null;

    /// <summary>
    /// There are no Faces of the 1-dimensional edge.
    /// </summary>
    public override SubIncidenceInfo? FaceIncidence => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubTwoDimensionalEdge"/> class.
    /// </summary>
    /// <param name="first">The first vertex of the edge.</param>
    /// <param name="second">The second vertex of the edge.</param>
    public SubTwoDimensionalEdge(SubPoint first, SubPoint second) {
      this.first = first;
      this.second = second;
      HashSet<SubPoint> vertices = new HashSet<SubPoint>() { first, second };
      Vertices = vertices;
    }

    /// <summary>
    /// Converts the edge to the previous space.
    /// </summary>
    /// <returns>The converted edge in the previous space.</returns>
    public override BaseSubCP ToPreviousSpace() => new SubTwoDimensionalEdge(first.Parent!, second.Parent!);

    /// <summary>
    /// Projects the edge to the specified affine basis.
    /// </summary>
    /// <param name="aBasis">The affine basis to project to.</param>
    /// <returns>The projected edge.</returns>
    public override BaseSubCP ProjectTo(AffineBasis aBasis) => new SubTwoDimensionalEdge
      (
       first.ProjectTo(aBasis)
     , second.ProjectTo(aBasis)
      );

  }

}
