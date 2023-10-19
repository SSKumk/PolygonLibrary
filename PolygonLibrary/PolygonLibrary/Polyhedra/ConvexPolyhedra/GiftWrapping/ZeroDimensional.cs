using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a one-dimensional vertex of a convex polytope expressed in k-space.
  /// </summary>
  public class SubZeroDimensional : BaseSubCP {

    /// <summary>
    /// Gets the dimension of the vertex. It equal to 0.
    /// </summary>
    public override int PolytopDim => 0;

    /// <summary>
    /// Gets the type of the vertex. It is ZeroDimensional.
    /// </summary>
    public override SubCPType Type => SubCPType.ZeroDimensional;

    /// <summary>
    /// Gets the the vertex.
    /// </summary>
    public override HashSet<SubPoint> Vertices => new HashSet<SubPoint>() { _vertex };

    /// <summary>
    /// The vertex expressed in k-space.
    /// </summary>
    public readonly SubPoint _vertex;

    /// <summary>
    /// The inner point of a vertex is the vertex itself.
    /// </summary>
    public override Point InnerPoint => new Point(_vertex);

    /// <summary>
    /// The affine space of a vertex is the vertex itself.
    /// </summary>
    public override List<Point> Affine => new List<Point>(){_vertex};

    /// <summary>
    /// Gets the the vertex. (By conversation).
    /// </summary>
    public override HashSet<BaseSubCP> Faces => new HashSet<BaseSubCP>(){new SubZeroDimensional(_vertex)};

    /// <summary>
    /// There are no FaceIncidence of the 0-dimensional vertex.
    /// </summary>
    public override SubIncidenceInfo? FaceIncidence => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubZeroDimensional"/> class.
    /// </summary>
    /// <param name="vertex">The vertex.</param>
    public SubZeroDimensional(SubPoint vertex) {
      _vertex   = vertex;
    }

    /// <summary>
    /// Converts the vertex to the previous space.
    /// </summary>
    /// <returns>The converted vertex in the previous space.</returns>
    public override BaseSubCP ToPreviousSpace() => new SubZeroDimensional(_vertex.Parent!);

    /// <summary>
    /// Projects the vertex to the specified affine basis.
    /// </summary>
    /// <param name="aBasis">The affine basis to project to.</param>
    /// <returns>The projected vertex.</returns>
    public override BaseSubCP ProjectTo(AffineBasis aBasis) => new SubZeroDimensional
      (new SubPoint(_vertex.ProjectTo(aBasis), _vertex, _vertex.Original));

  }

}
