namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a vertex of the polytop.
  /// </summary>
  internal sealed class SubZeroDimensional : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the vertex. It equals to 0.
    /// </summary>
    public override int PolytopDim => 0;

    /// <summary>
    /// Gets the type of the vertex. It is ZeroDimensional.
    /// </summary>
    public override SubCPType Type => SubCPType.ZeroDimensional;

    /// <summary>
    /// Gets the vertex.
    /// </summary>
    public override SortedSet<SubPoint> Vertices { get; }

    public SubPoint vertex { get; }

    /// <summary>
    /// There are no Faces of the 0-dimensional vertex.
    /// </summary>
    public override List<BaseSubCP>? Faces => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubZeroDimensional"/> class.
    /// </summary>
    /// <param name="vertex">The vertex.</param>
    public SubZeroDimensional(SubPoint vertex) {
      this.vertex = vertex;
      Vertices    = new SortedSet<SubPoint>() { vertex };
    }

    /// <summary>
    /// Converts the vertex to the previous space.
    /// </summary>
    /// <returns>The converted vertex in the previous space.</returns>
    public override BaseSubCP ToPreviousSpace() => new SubZeroDimensional(vertex.Parent!);

    /// <summary>
    /// Projects the vertex to the specified affine basis.
    /// </summary>
    /// <param name="affBasis">The affine basis to project to.</param>
    /// <returns>The projected vertex.</returns>
    public override BaseSubCP ProjectTo(AffineBasis affBasis) => new SubZeroDimensional(vertex.ProjectTo(affBasis));

  }

}
