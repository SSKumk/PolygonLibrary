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
  /// Represents a one-dimensional edge of a convex polytope expressed in k-space.
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
    /// The faces of 1-dimensional edge is its vertex.
    /// </summary>
    public override HashSet<BaseSubCP> Faces { get; }

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
      Faces = new HashSet<BaseSubCP>() { new SubZeroDimensional(first), new SubZeroDimensional(second) };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubTwoDimensionalEdge"/> class base on ZeroDimensional.
    /// </summary>
    /// <param name="first">The first vertex of the edge.</param>
    /// <param name="second">The second vertex of the edge.</param>
    public SubTwoDimensionalEdge(SubZeroDimensional first, SubZeroDimensional second) {
      Faces = new HashSet<BaseSubCP>() { first, second };
    }

    /// <summary>
    /// Converts the edge to the previous space.
    /// </summary>
    /// <returns>The converted edge in the previous space.</returns>
    public override BaseSubCP ToPreviousSpace() => new SubTwoDimensionalEdge
      ((SubZeroDimensional)Faces.First().ToPreviousSpace(), (SubZeroDimensional)Faces.Last().ToPreviousSpace());

    /// <summary>
    /// Projects the edge to the specified affine basis.
    /// </summary>
    /// <param name="aBasis">The affine basis to project to.</param>
    /// <returns>The projected edge.</returns>
    public override BaseSubCP ProjectTo(AffineBasis aBasis) {
      SubPoint first  = Faces.First().Vertices.First();
      SubPoint second = Faces.Last().Vertices.First();

      return new SubTwoDimensionalEdge
        (
         new SubPoint(first.ProjectTo(aBasis), first, first.Original)
       , new SubPoint(second.ProjectTo(aBasis), second, second.Original)
        );
    }

  }

}
